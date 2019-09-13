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
        TCPController.tcpServer.Stop();
        TCPController.tcpServer = null;

        for (int i = 0; i < arrSendBuffer.Length - 1; i++)
        {
            arrSendBuffer[i] = null;
        }

    }

    public static void StartListen()
    {
        Console.WriteLine("TCP Listen start");
        while (!TCPController.boolDone)
        {
            try
            {
                // Start listening to TCP
                tcpServer = new TcpListener(IPAddress.Any, intListenPort);
                string strReceivedData;

                // Start the main loop
                TCPController.boolDone = false;
                tcpServer.Start();
                while (!TCPController.boolDone)
                {
                    // Start listening

                    Console.WriteLine("TCP: Waiting for packet");
                    TcpClient tcpClient = tcpServer.AcceptTcpClient();  //if a connection exists, the server will accept it
                    NetworkStream ns = tcpClient.GetStream(); //networkstream is used to send/receive messages
                    //tcpClient.ReceiveBufferSize = 65534;
                    
                    while (tcpClient.Connected && !TCPController.boolDone)  //while the client is connected, we look for incoming messages
                    {
                        //arrReceiveByteArray = new byte[tcpClient.ReceiveBufferSize];     //the messages arrive as byte array
                        //ns.Read(arrReceiveByteArray, 0, arrReceiveByteArray.Length);   //the same networkstream reads the message sent by the client
                        //strReceivedData = Encoding.ASCII.GetString(arrReceiveByteArray, 0, arrReceiveByteArray.Length);

                        StringBuilder CompleteMessage = new StringBuilder();

                        if (ns.CanRead)
                        {
                            byte[] ReadBuffer = new byte[1024];
                            CompleteMessage = new StringBuilder();
                            int numberOfBytesRead = 0;

                            // Incoming message may be larger than the buffer size.
                            do
                            {
                                numberOfBytesRead = ns.Read(ReadBuffer, 0, ReadBuffer.Length);
                                CompleteMessage.AppendFormat("{0}", Encoding.ASCII.GetString(ReadBuffer, 0, numberOfBytesRead));
                            }
                            while (ns.DataAvailable);
                        }
                        strReceivedData = CompleteMessage.ToString();
                        Console.WriteLine("Sender: {0} Payload: {1}", null, strReceivedData);

                        dynamic dynamicRawTCPFrame = JsonConvert.DeserializeObject(strReceivedData); // Deserialize received frame
                        string strRawTCPFrameType = dynamicRawTCPFrame.type;

                        PerunHelper.LogHistoryAdd(ref arrLogHistory, "TCP packet received, type: " + strRawTCPFrameType);

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

                }
                tcpServer.Stop();
            }
            catch (Exception e)
            {
                // General exception found
                if (e.HResult != -2147467259)
                {
                    Console.WriteLine(e.ToString());
                    PerunHelper.LogHistoryAdd(ref arrLogHistory, "TCP error - connection closed or port in use");
                }
            }
            //Console.WriteLine("TCP listen stop");
        }
    }
}
