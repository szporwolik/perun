// This class handles TCP communication
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;

internal class TCPController
{
    // Main class for TCP listener
    public static int intListenPort;        // Port to connect to
    public static bool boolDone;            // Helper to exit main loop without killing thread
    public static TcpListener tcpServer;    // Listener object 
    public static string[] arrLogHistory;   // Log history for GUI
    public static string[] arrSendBuffer;   // Mysql send buffer
    public static Thread thrTCPListener;    // Seperate thread for UDP

    public static void Create(int intListenPort, ref string[] arrLogHistory, ref string[] arrSendBuffer)
    {
        // Create class
        TCPController.intListenPort = intListenPort;
        TCPController.arrLogHistory = arrLogHistory;
        TCPController.arrSendBuffer = arrSendBuffer;
    }

    public static void StopListen()
    {
        // FInish listening
        TCPController.boolDone = true;
        TCPController.tcpServer.Close();
        TCPController.tcpServer = null;

        for (int i = 0; i < arrSendBuffer.Length - 1; i++)
        {
            arrSendBuffer[i] = null;
        }

    }

    public static void StartListen()
    {
        Console.WriteLine("UDP Listen start");
        try
        {
            // Start listening to UDP

            tcpServer = new TcpListener(IPAddress.Any, intListenPort);

            string strReceivedData;
            byte[] arrReceiveByteArray;

            // Start the main loop
            TCPController.boolDone = false;
            tcpServer.Start();
            while (!TCPController.boolDone)
            {
                // Start listening
                Console.WriteLine("TCP: Waiting for packet");
                arrReceiveByteArray = udpListener.Receive(ref ipendpointGroupEP);
                strReceivedData = Encoding.ASCII.GetString(arrReceiveByteArray, 0, arrReceiveByteArray.Length);
                Console.WriteLine("Sender: {0} Payload: {1}", ipendpointGroupEP.ToString(), strReceivedData);

                // Add to log history and rotate
                dynamic dynamicRawUDPFrame = JsonConvert.DeserializeObject(strReceivedData); // Deserialize received frame
                string strRawUDPFrameType = dynamicRawUDPFrame.type;
                PerunHelper.LogHistoryAdd(ref arrLogHistory, "UDP packet received, type: " + strRawUDPFrameType);

                // Add to mySQL send buffer (find first empty slot)
                for (int i = 0; i < arrSendBuffer.Length - 1; i++)
                {
                    if (arrSendBuffer[i] == null)
                    {
                        arrSendBuffer[i] = strReceivedData;
                        break;
                    }
                }
            }
            udpListener.Close(); // Close port
        }
        catch (Exception e)
        {
            // General exception found
            if (e.HResult != -2147467259)
            {
                Console.WriteLine(e.ToString());
                PerunHelper.LogHistoryAdd(ref arrLogHistory, "ERROR UDP - port may be in use");
            }
        }
        Console.WriteLine("UDP listen stop");
    }
}
