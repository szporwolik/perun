// This class handles UDP communication
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;

internal class UDPController
{
    // Main class for UDP listener
    public static int intListenPort;        // Port to listen at
    public static bool boolDone;            // Helper to exit main loop without killing thread
    public static UdpClient udpListener;    // Listener object 
    public static string[] arrLogHistory;   // Log history for GUI
    public static string[] arrSendBuffer;   // Mysql send buffer
    public static Thread thrUDPListener;    // Seperate thread for UDP

    public static void Create(int intListenPort, ref string[] arrLogHistory, ref string[] arrSendBuffer)
    {
        // Create class
        UDPController.intListenPort = intListenPort;
        UDPController.arrLogHistory = arrLogHistory;
        UDPController.arrSendBuffer = arrSendBuffer;
    }

    public static void StopListen()
    {
        // FInish listening
        UDPController.boolDone = true;
        UDPController.udpListener.Close();
        UDPController.udpListener = null;

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
           
            udpListener = new UdpClient(intListenPort);
            IPEndPoint ipendpointGroupEP = new IPEndPoint(IPAddress.Loopback, intListenPort);
            string strReceivedData;
            byte[] arrReceiveByteArray;

            // Start the main loop
            UDPController.boolDone = false;
            while (!UDPController.boolDone)
            {
                // Start listening
                Console.WriteLine("UDP: Waiting for packet");
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
            if (e.HResult != -2147467259) {
                Console.WriteLine(e.ToString());
                PerunHelper.LogHistoryAdd(ref arrLogHistory, "ERROR UDP - port may be in use");
            }
        }
        Console.WriteLine("UDP listen stop");
    }
}