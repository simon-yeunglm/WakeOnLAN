
// by simon yeung, 12/11/2022
// all rights reserved

/*
 * 
 * Message Format
 * u32			magic header
 * u32			message type	// 1 = Wake PC with MAC address
 * u32			message size in byte, including header and footer
 * 
 * [ Message Wake_PC_With_MAC_Address
 * byte[6]		PC MAC address
 * ]
 * 
 * [ Message Reply
 * byte[1]		bool
 * ]
 * 
 * [ Message PC_Awaken_Query
 * byte[6]		PC MAC address
 * ]
 * 
 * [ Message PC_Awaken_Reply
 * byte[1]		bool, is awaken
 * byte[4]		IP address if awaken
 * ]
 * 
 * u32			magic footer
 * 
 */

using System.Diagnostics;

namespace WakeOnLANCommon
{
	public class WakeOnLANUtil
	{
		public static int MessageHeader = 0x48124359;
		public static int MessageFooter = 0x54920123;
		public static int ServerSendPort	= 4321;
		public static int ServerReplyPort	= 4322;

		public enum MessageType
		{
			Wake_PC_With_MAC_Address,
			Reply,
			PC_Awaken_Query,
			PC_Awaken_Reply,
			Num
		};

		public static int ByteArrayToInt(byte[] byteData, int idx)
		{
			return (int)byteData[idx + 0] << 24 |
					(int)byteData[idx + 1] << 16 |
					(int)byteData[idx + 2] << 8 |
					(int)byteData[idx + 3] << 0;
		}

		public static void IntToByte(int val, out byte byte0, out byte byte1, out byte byte2, out byte byte3)
		{
			byte0 = (byte)((val >> 24) & 0xff);
			byte1 = (byte)((val >> 16) & 0xff);
			byte2 = (byte)((val >> 8) & 0xff);
			byte3 = (byte)((val >> 0) & 0xff);
		}

		public static string IntToHexString(int val)
		{
			int digit1 = val / 16;
			int digit0 = val % 16;

			char chr1 = (char)((int)(digit1 < 10 ? '0' + digit1 : 'A' + (digit1 - 10)));
			char chr0 = (char)((int)(digit0 < 10 ? '0' + digit0 : 'A' + (digit0 - 10)));

			return chr1 + "" + chr0;
		}

		public static void SplitIP(string IP, out int IP_0, out int IP_1, out int IP_2, out int IP_3)
		{
			string[] parts = IP.Split('.');
			if (parts.Length != 4)
			{
				IP_0 = -1;
				IP_1 = -1;
				IP_2 = -1;
				IP_3 = -1;
			}
			else
			{
				IP_0 = int.Parse(parts[0]);
				IP_1 = int.Parse(parts[1]);
				IP_2 = int.Parse(parts[2]);
				IP_3 = int.Parse(parts[3]);
			}
		}

		public static string GetMACAddressString(byte[] MACAddress)
		{
			return	WakeOnLANUtil.IntToHexString(MACAddress[0]) + "-" +
					WakeOnLANUtil.IntToHexString(MACAddress[1]) + "-" +
					WakeOnLANUtil.IntToHexString(MACAddress[2]) + "-" +
					WakeOnLANUtil.IntToHexString(MACAddress[3]) + "-" +
					WakeOnLANUtil.IntToHexString(MACAddress[4]) + "-" +
					WakeOnLANUtil.IntToHexString(MACAddress[5]);
		}

		private static int MessageGetContentSize(MessageType type)
		{
			Debug.Assert((int)MessageType.Num == 4, "Please update Message Type");
			if (type == MessageType.Wake_PC_With_MAC_Address)
				return 6;
			else if (type == MessageType.Reply)
				return 1;
			else if (type == MessageType.PC_Awaken_Query)
				return 6;
			else if (type == MessageType.PC_Awaken_Reply)
				return 5;
			else
				return 0;
		}

		public static int MessageHeaderSize()
		{
			return 4 + 4 + 4;
		}

		public static int MessageFooterSize()
		{
			return 4;
		}

		private static byte[] MessageCreate(MessageType type)
		{
			int msgSz = MessageHeaderSize() + MessageGetContentSize(type) + MessageFooterSize();
			byte[] msg = new byte[msgSz];
			int typeInt = (int)type;
			WakeOnLANUtil.IntToByte(WakeOnLANUtil.MessageHeader	, out msg[0], out msg[1], out msg[ 2], out msg[ 3]);
			WakeOnLANUtil.IntToByte(typeInt						, out msg[4], out msg[5], out msg[ 6], out msg[ 7]);
			WakeOnLANUtil.IntToByte(msg.Length					, out msg[8], out msg[9], out msg[10], out msg[11]);

			WakeOnLANUtil.IntToByte(WakeOnLANUtil.MessageFooter, out msg[msgSz - 4], out msg[msgSz - 3], out msg[msgSz - 2], out msg[msgSz - 1]);
			return msg;
		}

		public static WakeOnLANUtil.MessageType MessageGet_Type(byte[] message)
		{
			 return (WakeOnLANUtil.MessageType)WakeOnLANUtil.ByteArrayToInt(message, 4);
		}

		public static byte[] MessageCreate_Wake_PC_With_MAC_Address(byte[] MACAddress)
		{
			byte[] message = WakeOnLANUtil.MessageCreate(WakeOnLANUtil.MessageType.Wake_PC_With_MAC_Address);
			int msgStartOffset = WakeOnLANUtil.MessageHeaderSize();
			message[msgStartOffset + 0] = (byte)MACAddress[0];
			message[msgStartOffset + 1] = (byte)MACAddress[1];
			message[msgStartOffset + 2] = (byte)MACAddress[2];
			message[msgStartOffset + 3] = (byte)MACAddress[3];
			message[msgStartOffset + 4] = (byte)MACAddress[4];
			message[msgStartOffset + 5] = (byte)MACAddress[5];
			return message;
		}

		public static void MessageGet_Wake_PC_With_MAC_Address(byte[] message, byte[] MACAddress)
		{
			WakeOnLANUtil.MessageType recvType = (WakeOnLANUtil.MessageType)WakeOnLANUtil.ByteArrayToInt(message, 4);
			Debug.Assert(recvType == WakeOnLANUtil.MessageType.Wake_PC_With_MAC_Address, "Message type mis-match.");
			int msgStartOffset = WakeOnLANUtil.MessageHeaderSize();
			MACAddress[0]= message[msgStartOffset + 0];
			MACAddress[1]= message[msgStartOffset + 1];
			MACAddress[2]= message[msgStartOffset + 2];
			MACAddress[3]= message[msgStartOffset + 3];
			MACAddress[4]= message[msgStartOffset + 4];
			MACAddress[5]= message[msgStartOffset + 5];
		}

		public static byte[] MessageCreate_Reply(bool state)
		{
			byte[] message = WakeOnLANUtil.MessageCreate(WakeOnLANUtil.MessageType.Reply);
			message[WakeOnLANUtil.MessageHeaderSize()] = state ? (byte)1 : (byte)0;
			return message;
		}

		public static byte[] MessageCreate_PC_Awaken_Query(byte[] MACAddress)
		{
			byte[] message = WakeOnLANUtil.MessageCreate(WakeOnLANUtil.MessageType.PC_Awaken_Query);
			int msgStartOffset = WakeOnLANUtil.MessageHeaderSize();
			message[msgStartOffset + 0] = (byte)MACAddress[0];
			message[msgStartOffset + 1] = (byte)MACAddress[1];
			message[msgStartOffset + 2] = (byte)MACAddress[2];
			message[msgStartOffset + 3] = (byte)MACAddress[3];
			message[msgStartOffset + 4] = (byte)MACAddress[4];
			message[msgStartOffset + 5] = (byte)MACAddress[5];
			return message;
		}

		public static void MessageGet_PC_Awaken_Query(byte[] message, byte[] MACAddress)
		{
			WakeOnLANUtil.MessageType recvType = (WakeOnLANUtil.MessageType)WakeOnLANUtil.ByteArrayToInt(message, 4);
			Debug.Assert(recvType == WakeOnLANUtil.MessageType.PC_Awaken_Query, "Message type mis-match.");
			int msgStartOffset = WakeOnLANUtil.MessageHeaderSize();
			MACAddress[0] = message[msgStartOffset + 0];
			MACAddress[1] = message[msgStartOffset + 1];
			MACAddress[2] = message[msgStartOffset + 2];
			MACAddress[3] = message[msgStartOffset + 3];
			MACAddress[4] = message[msgStartOffset + 4];
			MACAddress[5] = message[msgStartOffset + 5];
		}

		public static byte[] MessageCreate_PC_Awaken_Reply(bool isAwaken, byte IP_0, byte IP_1, byte IP_2, byte IP_3)
		{
			byte[] message = WakeOnLANUtil.MessageCreate(WakeOnLANUtil.MessageType.PC_Awaken_Reply);
			int msgStartOffset = WakeOnLANUtil.MessageHeaderSize();
			message[msgStartOffset + 0] = isAwaken ? (byte)1 : (byte)0;
			message[msgStartOffset + 1] = IP_0;
			message[msgStartOffset + 2] = IP_1;
			message[msgStartOffset + 3] = IP_2;
			message[msgStartOffset + 4] = IP_3;
			return message;
		}

		public static void MessageGet_PC_Awaken_Reply(byte[] message, out bool isAwaken, out byte IP_0, out byte IP_1, out byte IP_2, out byte IP_3)
		{
			WakeOnLANUtil.MessageType recvType = (WakeOnLANUtil.MessageType)WakeOnLANUtil.ByteArrayToInt(message, 4);
			Debug.Assert(recvType == WakeOnLANUtil.MessageType.PC_Awaken_Reply, "Message type mis-match.");
			int msgStartOffset = WakeOnLANUtil.MessageHeaderSize();
			bool isAwakenBool= message[msgStartOffset] != 0;
			isAwaken = isAwakenBool;
			IP_0 = isAwakenBool ? message[msgStartOffset + 1] : (byte)0;
			IP_1 = isAwakenBool ? message[msgStartOffset + 2] : (byte)0;
			IP_2 = isAwakenBool ? message[msgStartOffset + 3] : (byte)0;
			IP_3 = isAwakenBool ? message[msgStartOffset + 4] : (byte)0;
		}
	}
}
