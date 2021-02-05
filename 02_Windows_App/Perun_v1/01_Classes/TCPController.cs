// This class handles TCP communication
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

public class TCPController
{
    // Main class for TCP listener
    public int intListenPort;           // Port to connect to
    public bool bCloseConnection;       // Helper to exit main loop without killing thread
    public string[] arrGUILogHistory;   // Log history for GUI
    public string[] arrMySQLSendBuffer; // MySQL send buffer
    public Thread thrTCPListener;       // Seperate thread for TCP

    public void Create(int par_intListenPort, ref string[] par_arrLogHistory, ref string[] par_arrSendBuffer)
    {
        // Create class and map creation arguments to class
        intListenPort = par_intListenPort;
        arrGUILogHistory = par_arrLogHistory;
        arrMySQLSendBuffer = par_arrSendBuffer;
        bCloseConnection = false;
    }

    public void StopListen()
    {
        // Stop listening
        bCloseConnection = true;

        // Clear send buffer
        for (int i = 0; i < arrMySQLSendBuffer.Length - 1; i++)
        {
            arrMySQLSendBuffer[i] = null;
        }
    }

    public void StartListen()
    {
        // Start listening
        NetworkStream nsReadStream = null;
        TcpClient tcpClient = null;
        bool bTCPConnectionOnline = false;
        string strReceiveBuffer;

        // Main loop - do until diconnect button is clicked
        while (!bCloseConnection)
        {
            Console.WriteLine("TCP Listen start");
            TcpListener tcpServer = new TcpListener(IPAddress.Any, intListenPort); ;    // Create listener object 

            try
            {
                // Start listening to TCP
                string strReceivedData;

                // Start the main loop
                tcpServer.Start();

                // While user do not clicked Disconnect button
                while (!bCloseConnection)
                {
                    // Start listening
                    Console.WriteLine("TCP: Waiting for connection");
                    Globals.StatusConnection = false;

                    // Wait for pending connection
                    if (tcpServer.Pending())
                    {
                        Console.WriteLine("TCP: Connected");
                        bTCPConnectionOnline = true;
                        tcpClient = tcpServer.AcceptTcpClient();  //if a connection exists, the server will accept it
                        nsReadStream = tcpClient.GetStream(); //networkstream is used to send/receive messages
                        nsReadStream.ReadTimeout = 10000;
                        tcpClient.ReceiveTimeout = 10000;
                       

                        while (tcpClient.Connected && !bCloseConnection && bTCPConnectionOnline)  //while the client is connected, we look for incoming messages
                        {
                            StringBuilder CompleteMessage = new StringBuilder();
                            Globals.StatusConnection = true;

                            if (nsReadStream.CanRead)
                            {
                                Console.WriteLine("TCP: Can read");
                                byte[] ReadBuffer = new byte[1024];
                                CompleteMessage = new StringBuilder();
                                int numberOfBytesRead = 0;

                                // Incoming message may be larger than the buffer size.
                                do
                                {
                                    Console.WriteLine("TCP: Read");
                                    numberOfBytesRead = nsReadStream.Read(ReadBuffer, 0, ReadBuffer.Length);
                                    CompleteMessage.AppendFormat("{0}", Encoding.ASCII.GetString(ReadBuffer, 0, numberOfBytesRead));
                                    if (numberOfBytesRead == 0)
                                    {
                                        break;
                                    }
                                }
                                while (nsReadStream.DataAvailable && nsReadStream.CanRead && tcpClient.Connected);
                            }
                            strReceivedData = CompleteMessage.ToString();
                            Console.WriteLine("Sender: {0} Payload: {1}", null, strReceivedData);

                            string pattern = @"(\<SOT\>)+?.*?(\<EOT\>)+?";

                            foreach (Match match in Regex.Matches(strReceivedData, pattern))
                            {
                                strReceiveBuffer = match.Value;

                                Console.WriteLine("Found '{0}' at position {1} end {2}",match.Value, match.Index,match.Length);
                                Console.WriteLine("Prepared {0}", strReceiveBuffer.Substring(5, match.Length-10));

                                strReceivedData = strReceiveBuffer.Substring(5, match.Length - 10);
                                try
                                {
                                    if (strReceivedData != "")
                                    {
                                        dynamic dynamicRawTCPFrame = JsonConvert.DeserializeObject(strReceivedData); // Deserialize received frame
                                        string strRawTCPFrameType = dynamicRawTCPFrame.type;

                                        // Check if this is NOT keep alive
                                        if (Int32.Parse(strRawTCPFrameType) != 0)
                                        {
                                            // Add to mySQL send buffer (find first empty slot)
                                            PerunHelper.LogDebug(ref arrGUILogHistory, "Packet received" , 2,0, strRawTCPFrameType,true);
                                            bool AddedDataToBuffer = false;
                                            for (int i = 0; i < arrMySQLSendBuffer.Length - 1; i++)
                                            {
                                                if (arrMySQLSendBuffer[i] == null)
                                                {
                                                    arrMySQLSendBuffer[i] = strReceivedData;
                                                    AddedDataToBuffer = true;
                                                    break;
                                                }
                                            }
                                            if (!AddedDataToBuffer)
                                            {
                                                PerunHelper.LogError(ref arrGUILogHistory, "ERROR package was dropped", 1, 1, strRawTCPFrameType);
                                            }
                                        } else
                                        {
                                            // Keep alive
                                            PerunHelper.LogDebug(ref arrGUILogHistory, "Keep-alive received", 2,0,"0",true);
                                        }
                                    }
                                    else
                                    {
                                        bTCPConnectionOnline = false;
                                    }
                                }
                                catch (Exception e)
                                {
                                    Globals.ErrorsGame++;
                                    Console.WriteLine(e.ToString());
                                    PerunHelper.LogError(ref arrGUILogHistory, "ERROR while message parsing , error: " + e.Message,2,1,"?");
                                    bTCPConnectionOnline = false;
                                }

                                // Send empty byte to check if connection is still alive
                                try
                                {
                                    tcpClient.Client.Send(new byte[] { 0 }, 1, 0);
                                }
                                catch (SocketException e)
                                {
                                    Console.WriteLine(e.ToString());
                                    PerunHelper.LogError(ref arrGUILogHistory, "TCP ERROR cannot check connection, error: " + e.Message,2,1,"?");
                                }

                            }
                        }
                    }
                    System.Threading.Thread.Sleep(500);
                }
                tcpServer.Stop();
            }
            catch (Exception e)
            {
                Globals.ErrorsGame++;
                Console.WriteLine(e.ToString());
                PerunHelper.LogError(ref arrGUILogHistory, "TCP error - connection closed or port in use, error: " + e.Message,1,1,"?");
                bTCPConnectionOnline = false;
            }

            // Clean up and prepare for next connection
            if (!(nsReadStream is null))
            {
                nsReadStream.Flush();
                nsReadStream.Close();
            }
            if (!(tcpClient is null))
            {
                tcpClient.Close();
            }
            if (!(tcpServer is null))
            {
                tcpServer.Stop();
            }
            Console.WriteLine("TCP listen stop");
        }
    }
}
