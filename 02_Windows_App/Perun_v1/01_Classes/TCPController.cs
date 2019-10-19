// This class handles TCP communication
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;

public class TCPController
{
    // Main class for TCP listener
    public int intListenPort;        // Port to connect to
    public bool bDone;            // Helper to exit main loop without killing thread
    public string[] arrLogHistory;   // Log history for GUI
    public string[] arrSendBuffer;   // Mysql send buffer
    public Thread thrTCPListener;    // Seperate thread for TCP
    public bool bStatus;             // TCP connecion status
    
    public void Create(int par_intListenPort, ref string[] par_arrLogHistory, ref string[] par_arrSendBuffer)
    {
        // Create class
        intListenPort = par_intListenPort;
        arrLogHistory = par_arrLogHistory;
        arrSendBuffer = par_arrSendBuffer;
        bDone = false;
        bStatus = false;
    }

    public void StopListen()
    {
        // Finish listening
        bDone = true;
        bStatus = false;

        for (int i = 0; i < arrSendBuffer.Length - 1; i++)
        {
            arrSendBuffer[i] = null;
        }
    }

    public void StartListen()
    {
        NetworkStream ns=null;
        TcpClient tcpClient=null;

        while (!bDone)
        {
            Console.WriteLine("TCP Listen start");
            TcpListener tcpServer = new TcpListener(IPAddress.Any, intListenPort); ;    // Listener object 

            try
            {
                // Start listening to TCP
                string strReceivedData;

                // Start the main loop
                tcpServer.Start();
                bStatus = true;
                while (!bDone)
                {
                    // Start listening

                    // Wait for pending connection
                    if (tcpServer.Pending())
                    {
                        Console.WriteLine("TCP: Waiting for packet");
                        tcpClient = tcpServer.AcceptTcpClient();  //if a connection exists, the server will accept it
                        ns = tcpClient.GetStream(); //networkstream is used to send/receive messages
                                                    //ns.ReadTimeout = 100;
                        Globals.bClientConnected = true;
                        while (tcpClient.Connected && !bDone)  //while the client is connected, we look for incoming messages
                        {
                            StringBuilder CompleteMessage = new StringBuilder();
                            Globals.bClientConnected = true;

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
                                while (ns.DataAvailable && !bDone);
                            }
                            strReceivedData = CompleteMessage.ToString();
                            Console.WriteLine("Sender: {0} Payload: {1}", null, strReceivedData);

                            try
                            {
                                dynamic dynamicRawTCPFrame = JsonConvert.DeserializeObject(strReceivedData); // Deserialize received frame
                                string strRawTCPFrameType = dynamicRawTCPFrame.type;

                                PerunHelper.LogHistoryAdd(ref arrLogHistory, "#" + Globals.intInstanceId + "> TCP packet received, type: " + strRawTCPFrameType);

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
                            catch (Exception e)
                            {
                                Globals.intGameErros++;
                                Console.WriteLine(e.ToString());
                                PerunHelper.LogHistoryAdd(ref arrLogHistory, "#" + Globals.intInstanceId + " > TCP ERROR incorrect JSON > " + e.Message);
                            }
                        }
                    }
                }
                tcpServer.Stop();
                bStatus = false;
            }
            catch (Exception e)
            {
                // General exception found
                if (e.HResult != -2147467259)
                {
                    Globals.intGameErros++;
                    Console.WriteLine(e.ToString());
                    PerunHelper.LogHistoryAdd(ref arrLogHistory, "#" + Globals.intInstanceId + " > TCP error - connection closed or port in use > " + e.Message);
                }
              
            }

            if (!(ns is null))
            {
                ns.Flush();
                ns.Close();
            }
            if (!(tcpClient is null))
            {
                tcpClient.Close();
            }
            if (!(tcpServer is null))
            {
                tcpServer.Stop();
            }
            bStatus = false;
            Console.WriteLine("TCP listen stop");
        }
    }
}
