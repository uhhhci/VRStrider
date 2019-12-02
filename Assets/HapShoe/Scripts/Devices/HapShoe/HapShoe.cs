
/**
 *  Haptics Framework
 *
 *  UHH HCI 
 *  Author: Oscar Ariza <ariza@informatik.uni-hamburg.de>
 *
 */

using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

[Serializable]
public class HapShoe : AbstractDevice
{
    private static System.Diagnostics.Stopwatch sw = System.Diagnostics.Stopwatch.StartNew();
    private long pressureT1 = sw.ElapsedMilliseconds;
    private long pressureT2 = 0;
    private long pressurePacketCounter;
    private long lastPPS;

    public byte[] pressure = new byte[5];

    public override void ProcessPacket()
    {
        parsingToken = currentPacket[0];
        currentPacket = currentPacket.TrimStart(parsingToken);
        packetValues = currentPacket.Split(',');

        if (parsingToken == 'P' && packetValues.Length == 5)
        {
            for (int i = 0; i < 5; i++)
            {
                pressure[i] = Byte.Parse(packetValues[i]);
            }

            if (deviceDebug)
            {
                pressureT2 = sw.ElapsedMilliseconds;
                pressurePacketCounter++;
                if (pressureT2 - pressureT1 >= 1000)
                {
                    lastPPS = pressurePacketCounter;
                    pressureT1 = pressureT2;
                    pressurePacketCounter = 0;
                }
                debugReport +=
                "Pressure PPS: " + lastPPS + "\n\n";
            }
        }
        else
        {
            Debug.Log(string.Format("Lost Packet >>> {0}", currentPacket));
            lostPacketCounter++;
        }

    }

    public void SetWaitFlag(bool waitA) // Wait until the current played file or sample is done
    {
        SendCommand("AAW" + (waitA ? "1" : "0"));
    }


    public void Play(int audioIndex, int volume)
    {
        SendCommand("AVP" + audioIndex + "," + volume);
    }

    public void Stop(byte audioIndex)
    {
        SendCommand("AAS");
    }
}
