
// by simon yeung, 12/11/2022
// all rights reserved

using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.IO;
using WakeOnLANCommon;
using System.Net.NetworkInformation;

namespace WakeOnLANClient
{
	public partial class FormMain : Form
	{
		private byte[]	MACAddress			= { 0, 0, 0, 0, 0, 0 };
		private string	ServerIP			= "192.168.1.111";
		private string	ConfigFilePath		= "./config.txt";
		private string	ConfigTypeServerIP	= "Server_IP";
		private string	ConfigTypeMACAddress= "MAC_Address";
		private Ping	ServerPing			= null;
		private Color	TextColor			= Color.FromArgb(224, 224, 224);
		private bool	IsServerConnected	= false;
		private Socket	ServerReplySocket	= null;
		private int		MachinePingCnt		= 0;

		public FormMain()
		{
			InitializeComponent();
			ServerReplySocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
			ServerReplySocket.Bind((EndPoint)new IPEndPoint(IPAddress.Any, WakeOnLANUtil.ServerReplyPort));
			ServerReplySocket.ReceiveTimeout = 250;	// use a short time to avoid UI hang

			ServerPing = new Ping();
			ServerPing.PingCompleted += new PingCompletedEventHandler(ServerPingCompletedCallback);
			LoadConfig();
			UpdateServerStatusText();
			timer_server_ping.Start();
		}

        private void FormMain_FormClosed(object sender, FormClosedEventArgs e)
        {
			ServerReplySocket.Close();
		}

		private void Log(string text)
		{
			richTextBox_log.AppendText(text + "\n");
		}

		private void LogSameLine(string text)
		{
			richTextBox_log.AppendText(text);
		}

		private void LoadConfig()
		{
			// load from file
			if (File.Exists(ConfigFilePath))
			{
				string configStr = File.ReadAllText(ConfigFilePath);
				string[] configAllLines = configStr.Split('\n');
				for (Int32 i = 0; i < configAllLines.Length; ++i)
				{
					string configLine = configAllLines[i].Trim();
					string[] lineContent = configLine.Split('=');
					if (lineContent.Length != 2)
						continue;
					string configType = lineContent[0];
					string configValue = lineContent[1];

					if (configType == ConfigTypeServerIP)
						LoadConfig_ServerIP(configValue);
					else if (configType == ConfigTypeMACAddress)
						LoadConfig_MACAddress(configValue);
					else
						Log("Unknown Config Type '" + configType + "'");
				}
			}

			// apply to UI
			SetServerIPText();
			SetMACAddressText();
		}

		private void LoadConfig_ServerIP(string configValueString)
		{
			if (!ValidateServerIP(configValueString))
				Log("Invalid Server IP config '"+configValueString+"'");
		}

		private void LoadConfig_MACAddress(string configValueString)
		{
			if (!WakeOnLANUtil.ValidateMACAddress(configValueString, ref MACAddress))
				Log("Invalid MAC Address config '" + configValueString + "'");
		}

		private void SaveConfig()
		{
			string configStr = ConfigTypeServerIP + "=" + ServerIP + "\n" +
								ConfigTypeMACAddress + "=" + WakeOnLANUtil.GetMACAddressString(MACAddress);
			File.WriteAllText(ConfigFilePath, configStr);
		}

		private void SetServerIPText()
		{
			textBox_Server_IP.Text = ServerIP;
		}

		private void SetMACAddressText()
		{
			textBox_MAC_Address.Text = WakeOnLANUtil.GetMACAddressString(MACAddress);
		}

		private bool ValidateServerIP(string IPStr)
		{
			string[] str = IPStr.Split('.');
			if (str.Length != 4)
				return false;

			int val;
			if (!int.TryParse(str[0], out val))
				return false;
			if (val < 0 || val > 255)
				return false;
			if (!int.TryParse(str[1], out val))
				return false;
			if (val < 0 || val > 255)
				return false;
			if (!int.TryParse(str[2], out val))
				return false;
			if (val < 0 || val > 255)
				return false;
			if (!int.TryParse(str[3], out val))
				return false;
			if (val < 0 || val > 255)
				return false;

			ServerIP = IPStr;
			return true;
		}

		private void button_wake_Click(object sender, EventArgs e)
		{
			// early out if server is not connected
			if (!IsServerConnected)
			{
				Log("Cannot connect to the Server.");
				return;
			}

			// try validate input
			if (!ValidateServerIP(textBox_Server_IP.Text.Trim()))
			{
				Log("Invalid Server IP.");
				return;
			}
			if (!WakeOnLANUtil.ValidateMACAddress(textBox_MAC_Address.Text.Trim(), ref MACAddress))
			{
				Log("Invalid MAC Address.");
				return;
			}

			// save config file for last used settings
			SaveConfig();

			// create Wake On LAN message
			byte[] sendData = WakeOnLANUtil.MessageCreate_Wake_PC_With_MAC_Address(MACAddress);

			// send Wake On LAN message to server
			const int	retryCntMax = 3;
			int			retryCnt	= 0;
			bool		isSendRecvOk= false;
			bool		isSendOk	= true;
			IPEndPoint ip = new IPEndPoint(IPAddress.Parse(ServerIP), WakeOnLANUtil.ServerSendPort);
			Socket socket= new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

			while ((!isSendRecvOk) && retryCnt < retryCntMax)
			{
				try
				{
					socket.SendTo(sendData, sendData.Length, SocketFlags.None, ip);
				}
				catch
				{
					isSendOk = false;
				}

				// Wait for server to reply
				int receivedDataLength = -1;
				try
				{
					byte[] recvData = new byte[1024];
					EndPoint Remote = (EndPoint)new IPEndPoint(IPAddress.Parse(ServerIP), 0);
					receivedDataLength = ServerReplySocket.ReceiveFrom(recvData, ref Remote);
				}
				catch (Exception ex)
				{
					Console.WriteLine("client Failed to receive data: " + ex.ToString());
				}
				isSendRecvOk = isSendOk && receivedDataLength != -1;
				++retryCnt;
			}
			socket.Close();

			// log
			if (isSendRecvOk)
			{
				Log("Wake On LAN message sent to broadcast server.");

				// disable wake button until machine powered on or time out
				button_wake.Enabled = false;

				// start pinging the target machine
				LogSameLine("Waiting " + WakeOnLANUtil.GetMACAddressString(MACAddress) + " to be awaken.");
				MachinePingCnt = 0;
				timer_machine_ping.Start();
			}
			else
				Log("Failed to send Wake On LAN message to broadcast server.");
        }

		private void timer_server_ping_Tick(object sender, EventArgs e)
        {
			PingServer();
		}

		private void PingServer()
		{
			ServerPing.SendAsync(textBox_Server_IP.Text.Trim(), 3000, null);
		}

		private void UpdateServerStatusText()
		{
			label_serverStatus.Text = IsServerConnected ? "Server Connected" : "Server Disconnected";
			label_serverStatus.ForeColor = IsServerConnected ? TextColor : Color.Red;
		}

		private void ServerPingCompletedCallback(object sender, PingCompletedEventArgs e)
		{
			IsServerConnected = (!e.Cancelled) && e.Error == null && e.Reply.Status == IPStatus.Success;
			UpdateServerStatusText();
		}

        private async void timer_machine_ping_Tick(object sender, EventArgs e)
		{
			// check is time out
			const int WaitTimeMax	= 60 * 3; // sec
			int waitCntMax			= WaitTimeMax * 1000 / timer_machine_ping.Interval;
			++MachinePingCnt;
			if (MachinePingCnt > waitCntMax)
			{
				timer_machine_ping.Stop();
				button_wake.Enabled = true;
				Log("\n" + WakeOnLANUtil.GetMACAddressString(MACAddress) + " cannot be awaken.");
				return;
			}

			// ask server machine status
			byte[] sendData = WakeOnLANUtil.MessageCreate_PC_Awaken_Query(MACAddress);
			bool isSendOk = true;
			IPEndPoint ip = new IPEndPoint(IPAddress.Parse(ServerIP), WakeOnLANUtil.ServerSendPort);
			Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
			try
			{
				socket.SendTo(sendData, sendData.Length, SocketFlags.None, ip);
			}
			catch
			{
				isSendOk = false;
			}
			socket.Close();

			// Wait for server to reply
			int receivedDataLength = -1;
			byte[] recvData = new byte[1024];
			WakeOnLANUtil.MessageType msgType= WakeOnLANUtil.MessageType.Num;
			if (isSendOk)
			{
				try
				{
					EndPoint Remote = (EndPoint)new IPEndPoint(IPAddress.Parse(ServerIP), 0);
					receivedDataLength = ServerReplySocket.ReceiveFrom(recvData, ref Remote);
					if (receivedDataLength != -1)
						msgType = WakeOnLANUtil.MessageGet_Type(recvData);
				}
				catch (Exception ex)
				{
					Console.WriteLine("client Failed to receive data: " + ex.ToString());
				}
			}

			// log
			if (msgType == WakeOnLANUtil.MessageType.PC_Awaken_Reply)
			{
				bool isAwaken;
				byte IP_0, IP_1, IP_2, IP_3;
				WakeOnLANUtil.MessageGet_PC_Awaken_Reply(recvData, out isAwaken, out IP_0, out IP_1, out IP_2, out IP_3);
				if (isAwaken)
				{
					timer_machine_ping.Stop();
					button_wake.Enabled = true;

					string machineIP = IP_0 + "." + IP_1 + "." + IP_2 + "." + IP_3;
					Log("\n"+WakeOnLANUtil.GetMACAddressString(MACAddress) + " awaken with IP "+ machineIP +".");

					// start remote desktop
					await StartRemoteDesktop(machineIP);
					return;
				}
			}
			LogSameLine(".");
		}

		async Task StartRemoteDesktop(string IP)
		{
			await Task.Delay(100);
			Log("Starting Remote Desktop.");
			System.Diagnostics.ProcessStartInfo ps = new System.Diagnostics.ProcessStartInfo("mstsc", "/v:"+IP);
			ps.CreateNoWindow = true;
			using (System.Diagnostics.Process proc = new System.Diagnostics.Process())
			{
				proc.StartInfo = ps;
				proc.Start();
			}

			await Task.Delay(1000);
			Log("Remote Desktop Started.");

			await Task.Delay(1000);
			LogSameLine("Exiting");

			await Task.Delay(1000);
			LogSameLine(".");

			await Task.Delay(1000);
			LogSameLine(".");

			await Task.Delay(1000);
			LogSameLine(".");

			await Task.Delay(1000);
			this.Close();
		}
	}
}
