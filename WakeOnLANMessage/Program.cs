
// by simon yeung, 20/01/2023
// all rights reserved

using System;
using System.IO;
using WakeOnLANCommon;

namespace WakeOnLANMessage
{
    class Program
    {
        static void Main(string[] args)
        {
            // get input arguments
            if (args.Length != 2)
            {
                Console.WriteLine("Create wake PC message with MAC address and output it to a file.");
                Console.WriteLine("Usage: WakeOnLANMessage.exe [MAC Address, e.g. 11-22-33-44-55-66] [output file name]");
                return;
            }

            // validate input
            string MACAddressStr    = args[0];
		    byte[] MACAddress		= { 0, 0, 0, 0, 0, 0 };
            if (!WakeOnLANUtil.ValidateMACAddress(MACAddressStr, ref MACAddress))
            {
                Console.WriteLine("Failed to parse MAC address " + MACAddressStr + ".");
                return;
            }

            // create Wake On LAN Message and save to file
            try
            {
                string OutputFileName   = args[1];
                byte[] MessageBytes     = WakeOnLANUtil.MessageCreate_Wake_PC_With_MAC_Address(MACAddress);
                File.WriteAllBytes(OutputFileName, MessageBytes);
            }
            catch (Exception e)
            {
				Console.WriteLine("Failed to save Wake On LAN Message: " + e.ToString());
            }
        }
    }
}
