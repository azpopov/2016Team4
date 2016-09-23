﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarMeter.View.Helpers
{
    public class PacketLabelCreator
    {
        public static string GetAddressLabel(byte[] packetAddress)
        {
            var finalAddressString = "";
            if (packetAddress != null)
            {
                if (packetAddress.Length > 1)
                {
                    finalAddressString += "Physical Path: ";
                    for (var i = 0; i < packetAddress.Length - 1; i++)
                    {
                        finalAddressString += Convert.ToInt32(packetAddress[i]) + "  ";
                    }
                }
                else
                {
                    finalAddressString = Convert.ToInt32(packetAddress[0]).ToString();
                }
            }
            else
            {
                finalAddressString = "No Address";
            }
            return finalAddressString;
        }


        public static string GetProtocolLabel(int protocolId)
        {
            if (protocolId == 1)
            {
                return Environment.NewLine + "Protocol: " + protocolId + " (RMAP)";
            }
            return Environment.NewLine + "Protocol: " + protocolId;
        }
    }
}
