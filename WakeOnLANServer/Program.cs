
// by simon yeung, 12/11/2022
// all rights reserved

using System;
using System.Net;
using System.Net.Sockets;
using WakeOnLANCommon;
using System.Threading;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Diagnostics;
using System.Threading.Tasks;

namespace WakeOnLANServer
{
    class Program
	{
		struct MachineStatus
		{
			public string IP;
			public bool IsAwaken;

			public MachineStatus(string _IP, bool _IsAwaken)
			{
				IP = _IP;
				IsAwaken = _IsAwaken;
			}
		}

		static EndPoint RecvMsgEndPoint = null;
		static Socket	ReplySocket		= new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
		static Ping[]	BroadcastPing	= new Ping[256];
		
		static Dictionary<string, MachineStatus>	MacIPTable				= new Dictionary<string, MachineStatus>();
		static Mutex								MacIPTableMutex			= new Mutex();
		static ManualResetEvent						MacIPTableUpdateEvent	= new ManualResetEvent(false);
		static DateTime								MacIPTableLastUpdateTime= DateTime.Now;
		static int									MacIPTableMaxUpdateTime	= 10;	// in sec

		static void PingTask(string IP, int pingIdx, bool[] statusArray)
		{
			PingReply reply= BroadcastPing[pingIdx].Send(IP, 10);
			statusArray[pingIdx] = reply.Status == IPStatus.Success;
		}

		static void UpdateMacIPTable()
		{
			do
			{
				// wait until trigger by main thread
				MacIPTableUpdateEvent.WaitOne();

				// get client IP
				if (RecvMsgEndPoint == null)
					continue;
				int clientIP_0, clientIP_1, clientIP_2, clientIP_3;
				string recvIPPort = RecvMsgEndPoint.ToString();
				WakeOnLANUtil.SplitIP(recvIPPort.Substring(0, recvIPPort.IndexOf(':')), out clientIP_0, out clientIP_1, out clientIP_2, out clientIP_3);

				// ping all the possible IP to update the ARP
				// minimum OS ping time out is 500ms, so we execute them all in parallel
				Task[] tasks		= new Task[256];
				bool[] statusArray	= new bool[256];
				for (int i = 0; i <= 255; ++i)
                {
                    string IP = clientIP_0 + "." + clientIP_1 + "." + clientIP_2 + "." + i;
					int idx= i;
					tasks[i] = new Task(() => PingTask(IP, idx, statusArray));
					tasks[i].Start();
				}
				Task.WaitAll(tasks);

				// refresh the MAC-IP table if time is too long
				MacIPTableMutex.WaitOne();
				DateTime lastUpdateTime= MacIPTableLastUpdateTime;
				MacIPTableMutex.ReleaseMutex();
				if ((DateTime.Now - lastUpdateTime).TotalSeconds > MacIPTableMaxUpdateTime)
					MacIPTable.Clear();

				// get ARP 
				string ARP_string;
				System.Diagnostics.ProcessStartInfo ps = new System.Diagnostics.ProcessStartInfo("arp", "-a");
				ps.CreateNoWindow = true;
				ps.RedirectStandardOutput = true;
				using (System.Diagnostics.Process proc = new System.Diagnostics.Process())
				{
					proc.StartInfo = ps;
					proc.Start();
					System.IO.StreamReader sr = proc.StandardOutput;
					while (!proc.HasExited) ;
					ARP_string = sr.ReadToEnd();
				}

				// parse ARP
				string[] ARP_lines = ARP_string.Split('\n');
				bool isServerInterfaceFound = false;
				for (int l = 0; l < ARP_lines.Length; ++l)
				{
					string line = ARP_lines[l].Trim();

					// find interface
					if (line.StartsWith("Interface:"))
					{
						isServerInterfaceFound = false;
						string[] interfaceWords = line.Split(" ");
						if (interfaceWords.Length == 4)
						{
							string interfaceIP = interfaceWords[1];
                            int interfaceIP_0, interfaceIP_1, interfaceIP_2, interfaceIP_3;
                            WakeOnLANUtil.SplitIP(interfaceIP, out interfaceIP_0, out interfaceIP_1, out interfaceIP_2, out interfaceIP_3);

                            if (interfaceIP_0 == clientIP_0 &&
                                interfaceIP_1 == clientIP_1 &&
                                interfaceIP_2 == clientIP_2)
                            {
                                isServerInterfaceFound = true;
							}
						}

					}

					// parse interface information
					else if (isServerInterfaceFound)
					{
						if (line.StartsWith("Internet Address"))
							continue;
						else
						{
							string[] lineParts = line.Split(' ');
							if (lineParts.Length > 3)
							{
								// get IP
								string IP = lineParts[0];

								// skip white lines
								int i = 1;
								while (i < lineParts.Length && lineParts[i] == "")
									++i;

								// get Mac
								string Mac = "";
								if (i < lineParts.Length)
									Mac = lineParts[i];

								int IP_0, IP_1, IP_2, IP_3;
								WakeOnLANUtil.SplitIP(IP, out IP_0, out IP_1, out IP_2, out IP_3);

								// store matched MAC-IP
								if (IP_0 == clientIP_0 &&
									IP_1 == clientIP_1 &&
									IP_2 == clientIP_2)
								{
									Mac = Mac.ToUpper();
									MacIPTableMutex.WaitOne();
									if (!MacIPTable.ContainsKey(Mac))
										MacIPTable.Add(Mac, new MachineStatus(IP, statusArray[IP_3]));
									MacIPTableMutex.ReleaseMutex();
								}
							}
						}
					}
				}

				// update last update time
				MacIPTableMutex.WaitOne();
				MacIPTableLastUpdateTime = DateTime.Now;
				MacIPTableMutex.ReleaseMutex();

				// wait for next update trigger
				MacIPTableUpdateEvent.Reset();
				RecvMsgEndPoint = null;
			} while (true);
		}

		static void Main(string[] args)
		{
			ReplySocket	= new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
			for (int i = 0; i < BroadcastPing.Length; ++i)
				BroadcastPing[i] = new Ping();

			Thread updateARPThread = new Thread(() => UpdateMacIPTable());
			updateARPThread.Start();

			byte[] recvData = new byte[1024];
			Socket server = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
			server.Bind((EndPoint)new IPEndPoint(IPAddress.Any, WakeOnLANUtil.ServerSendPort));
			server.EnableBroadcast = true;

			IPEndPoint	sender = new IPEndPoint(IPAddress.Any, 0);
			EndPoint	Remote = (EndPoint)sender;

			Console.WriteLine("Server started.");

			try
			{
				while (true)
				{
					int receivedDataLength = 0;
					try
					{
						receivedDataLength = server.ReceiveFrom(recvData, ref Remote);
					}
					catch (Exception e)
					{
						Console.WriteLine("Server Failed to receive data: " + e.ToString());
					}
					if (receivedDataLength < (4 + 4 + 4 + 4))
					{
						Console.WriteLine("Message packet size invalid {0}", receivedDataLength);
						continue;
					}

					int recvHeader = WakeOnLANUtil.ByteArrayToInt(recvData, 0);
					if (recvHeader != WakeOnLANUtil.MessageHeader)
					{
						Console.WriteLine("Message Magic Header Mis-match");
						continue;
					}

					int recvSize = WakeOnLANUtil.ByteArrayToInt(recvData, 8);
					if (recvSize != receivedDataLength)
					{
						Console.WriteLine("Receive size mis-match {0} - {1}", recvSize, receivedDataLength);
						continue;
					}

					int recvFooter = WakeOnLANUtil.ByteArrayToInt(recvData, recvSize - 4);
					if (recvFooter != WakeOnLANUtil.MessageFooter)
					{
						Console.WriteLine("Message Magic Footer Mis-match");
						continue;
					}

					Console.WriteLine("Message received from {0}.", Remote.ToString());
					Debug.Assert((int)WakeOnLANUtil.MessageType.Num == 4, "Please update Message Type");
					WakeOnLANUtil.MessageType recvType = (WakeOnLANUtil.MessageType)WakeOnLANUtil.ByteArrayToInt(recvData, 4);
					if (recvType == WakeOnLANUtil.MessageType.Wake_PC_With_MAC_Address)
						HandleMessage_WakePCWithMACAddress(server, recvData, Remote);
					else if (recvType == WakeOnLANUtil.MessageType.PC_Awaken_Query)
						HandleMessage_PC_Awaken_Query(recvData, Remote);
					else
					{
						Console.WriteLine("Unknown message type {0}.", recvType);
					}

				}
			}
			catch (Exception e)
			{
				Console.WriteLine("Server Exception: " + e.ToString());
			}
			server.Close();
			updateARPThread.Abort();
			ReplySocket.Close();
			Console.WriteLine("Server closed.");
		}

		static void HandleMessage_WakePCWithMACAddress(Socket server, byte[] message, EndPoint recvMsgEndPoint)
		{
            // create broadcast message
            const int WOLPacketSize = 6 + 16 * 6;
            byte[] WOLPacket = new byte[WOLPacketSize];

			// get wake up machine MAC address
			byte[] MAC = new byte[6];
			WakeOnLANUtil.MessageGet_Wake_PC_With_MAC_Address(message, MAC);

			// 6 0xFF header
			for (Int32 i = 0; i < 6; ++i)
                WOLPacket[i] = 0xFF;

            // repeat MAC address 16 times
            for (Int32 i = 0; i < 16; ++i)
            {
                int offset = 6 + i * 6;
                WOLPacket[offset + 0] = MAC[0];
                WOLPacket[offset + 1] = MAC[1];
                WOLPacket[offset + 2] = MAC[2];
                WOLPacket[offset + 3] = MAC[3];
                WOLPacket[offset + 4] = MAC[4];
                WOLPacket[offset + 5] = MAC[5];
            }

            // broadcast WOL packet
            try
            {
                server.SendTo(WOLPacket, WOLPacketSize, SocketFlags.None, new IPEndPoint(IPAddress.Broadcast, 7));
            }
            catch (Exception e)
            {
                Console.WriteLine("Failed to Broadcast Wake On LAN packet: " + e.ToString());
            }

			// update Mac-IP Table
			if (RecvMsgEndPoint == null)
			{
				RecvMsgEndPoint = recvMsgEndPoint;
				MacIPTableUpdateEvent.Set();
			}

			// reply client
			try
			{
				string recvIPPort = recvMsgEndPoint.ToString();
				string recvIP = recvIPPort.Substring(0, recvIPPort.IndexOf(':'));

				byte[] sendData = WakeOnLANUtil.MessageCreate_Reply(true);

				IPEndPoint ip = new IPEndPoint(IPAddress.Parse(recvIP), WakeOnLANUtil.ServerReplyPort);
				ReplySocket.SendTo(sendData, sendData.Length, SocketFlags.None, ip);
			}
			catch
			{
			}
		}

		static void HandleMessage_PC_Awaken_Query(byte[] message, EndPoint recvMsgEndPoint)
		{
			// get wake up machine MAC address
			byte[] MAC = new byte[6];
			WakeOnLANUtil.MessageGet_PC_Awaken_Query(message, MAC);
			string MACStr= WakeOnLANUtil.GetMACAddressString(MAC);

			// only use MAC-IP table if it is updated recently
			MachineStatus machineStatus = new MachineStatus("", false);
			MacIPTableMutex.WaitOne();
			bool	hasIP				= false;
			double	timeSinceLastUpdate = (DateTime.Now - MacIPTableLastUpdateTime).TotalSeconds;
			if (timeSinceLastUpdate < MacIPTableMaxUpdateTime)
				hasIP= MacIPTable.TryGetValue(MACStr, out machineStatus);
			else if (RecvMsgEndPoint == null)
			{
				// trigger update of outdated Mac-IP Table
				RecvMsgEndPoint = recvMsgEndPoint;
				MacIPTableUpdateEvent.Set();
			}
			MacIPTableMutex.ReleaseMutex();

			bool isAwaken = machineStatus.IsAwaken;
			int IP_0 = 0;
			int IP_1 = 0;
			int IP_2 = 0;
			int IP_3 = 0;
			if (hasIP)
				WakeOnLANUtil.SplitIP(machineStatus.IP, out IP_0, out IP_1, out IP_2, out IP_3);

			// reply client
			try
			{
                string recvIPPort = recvMsgEndPoint.ToString();
                string recvIP = recvIPPort.Substring(0, recvIPPort.IndexOf(':'));

                byte[] sendData = WakeOnLANUtil.MessageCreate_PC_Awaken_Reply(isAwaken, (byte)IP_0, (byte)IP_1, (byte)IP_2, (byte)IP_3);

                IPEndPoint ip = new IPEndPoint(IPAddress.Parse(recvIP), WakeOnLANUtil.ServerReplyPort);
                ReplySocket.SendTo(sendData, sendData.Length, SocketFlags.None, ip);
            }
			catch
			{
			}
		}
	}
}
