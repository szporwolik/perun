using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows.Forms;

using MySql.Data.MySqlClient;
using Newtonsoft.Json;

namespace Perun_v1
{
    public partial class form_Main : Form
    {

        // Variable definitions
            public Thread thread_UDPListener;               // Seperate thread for UDP
            public class_UDPListener UDPListener;           // Helper class for UDP comunication
            public string[] LogHistory=new string[10];      // Log history for GUI
            public string[] SendBuffer=new string[10];      // Mysql send buffer
            public bool LetMeOut=false;                     // Helper to handle system tray
            public string MySql_connStr;             // MySQL connection string

        public static void LogHistoryAdd(ref string[] LogHistory, string Comment)
        {
            // Add to log history and rotate
            for (int i = 0; i < LogHistory.Length - 1; i++)
            {
                LogHistory[i] = LogHistory[i + 1];
            }
            LogHistory[9] = DateTime.Now.ToString("HH:mm:ss") + " > " + Comment;
        }

        public void SendToMySql(string raw_udp_frame)
        {
            // Main function to send data to mysql
                dynamic udp_frame = JsonConvert.DeserializeObject(raw_udp_frame);

            // Cut raw data to type and paylod for proper mysql insert
                string type = udp_frame.type;
                string payload = "";
                string sql = "";

                // Modify specific
            if (type == "1") // Inject app version information
                    {
                        Assembly assembly = Assembly.GetExecutingAssembly();
                        FileVersionInfo fileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
                        string version = fileVersionInfo.ProductVersion;
                        udp_frame.payload["v_win"] = "v"+version;
                    }

                // Specific SQL 
                    if(type == "50")
                    {
                        sql = "INSERT INTO `pe_LogChat` (`pe_LogChat_id`, `pe_LogChat_playerid`, `pe_LogChat_msg`, `pe_LogChat_all`) VALUES (NULL, '"+udp_frame.payload.player+ "', '" + udp_frame.payload.msg + "', '" + udp_frame.payload.all + "');";
                    }
                    else if(type == "51")
                    {
                        sql = "INSERT INTO `pe_LogEvent` (`pe_LogEvent_id`, `pe_LogEvent_datetime`, `pe_LogEvent_type`, `pe_LogEvent_content`) VALUES (NULL, CURRENT_TIMESTAMP, '" + udp_frame.payload.log_type + "', '" + udp_frame.payload.log_content + "');";
                    }
                    else
                    {
                        payload = JsonConvert.SerializeObject(udp_frame.payload);
                        sql = "INSERT INTO pe_DataRaw(pe_dataraw_type,pe_dataraw_payload) VALUES (" + type + ",JSON_QUOTE('" + payload + "')) ON DUPLICATE KEY UPDATE pe_dataraw_payload = JSON_QUOTE('" + payload + "')";
                    }
               
            // Connect to mysql
            MySqlConnection conn = new MySqlConnection(MySql_connStr);
                try
                {
                    Console.WriteLine("Sending data to MySQL - Begin");
                    conn.Open();

                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    MySqlDataReader rdr = cmd.ExecuteReader();

                    while (rdr.Read())
                    {
                        Console.WriteLine(rdr[0] + " -- " + rdr[1]);
                    }
                    rdr.Close();
                    LogHistoryAdd(ref LogHistory, "MySQL updated, package type: "+ type);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                LogHistoryAdd(ref LogHistory, "ERROR: MySQL");
            }

                conn.Close();
                Console.WriteLine("Sending data to MySQL - Done");
        }

     

        public class class_UDPListener
        {
            // Main class for UDP listener
                int listenPort;                 // Port to listen at
                public bool done;               // Helper to exit main loop without killing thread
                public UdpClient listener;      // Listener object 
                public string[] LogHistory;     // Log history for GUI
                public string[] SendBuffer;     // Mysql send buffer

                public class_UDPListener(int port, ref string[] LogHistory, ref string[] SendBuffer)
                {
                    // Create clas - NOTE that there is reference passing
                        this.listenPort = port;
                        this.LogHistory = LogHistory;
                        this.SendBuffer = SendBuffer;
                }

                public void StartListen()
                {
                    // Start listening to UDP
                    this.done = false;
                    listener = new UdpClient(listenPort);
                    IPEndPoint groupEP = new IPEndPoint(IPAddress.Loopback, listenPort);
                    string received_data;
                    byte[] receive_byte_array;

                    Console.WriteLine("UDP Listen start");
                    try
                    {
                        while (!done)
                        {
                            Console.WriteLine("UDP: Waiting for packet");
                            receive_byte_array = listener.Receive(ref groupEP);
                            received_data = Encoding.ASCII.GetString(receive_byte_array, 0, receive_byte_array.Length);

                            Console.WriteLine("Sender: {0} Payload: {1}", groupEP.ToString(), received_data);

                        // Add to log history and rotate
                            dynamic udp_frame = JsonConvert.DeserializeObject(received_data);
                            string type = udp_frame.type;
                            LogHistoryAdd(ref LogHistory, "UDP packet received, type: "+ type);
                            
                        // Add to mySQL send buffer and rotate
                             for (int i = 0; i < SendBuffer.Length - 1; i++)
                             {
                                SendBuffer[i] = SendBuffer[i + 1];
                             }
                             SendBuffer[9] = received_data;

                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.ToString());
                    }
                    Console.WriteLine("UDP listen stop");
                    listener.Close();
                }
            }


        public form_Main()
        {
            // Form init
                InitializeComponent();
        }

        private void form_Main_Load(object sender, EventArgs e)
        {
            // Form loaded - fill controls with default values
                LogHistory[0] = DateTime.Now.ToString("HH:mm:ss") + " > " + "Perun loaded...";

            // Load settings
            con_txt_mysql_database.Text = Properties.Settings.Default.MYSQL_DB;
            con_txt_mysql_username.Text = Properties.Settings.Default.MYSQL_User;
            con_txt_mysql_password.Text = Properties.Settings.Default.MYSQL_Password;
            con_txt_mysql_server.Text = Properties.Settings.Default.MYSQL_Server;
            con_txt_mysql_port.Text = Properties.Settings.Default.MYSQL_Port;
            con_txt_3rd_lotatc.Text = Properties.Settings.Default.OTHER_LOTATC_FILE;
            con_txt_3rd_srs.Text = Properties.Settings.Default.OTHER_SRS_FILE;
            con_check_3rd_lotatc.Checked = Properties.Settings.Default.OTHER_LOTATC_USE;
            con_check_3rd_srs.Checked = Properties.Settings.Default.OTHER_SRS_USE;

            // Version to title header
            Assembly assembly = Assembly.GetExecutingAssembly();
            FileVersionInfo fileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
            this.Text = this.Text + " - v" + fileVersionInfo.ProductVersion;
        }

        private void con_Button_Listen_ON_Click(object sender, EventArgs e)
        {
            // Start listening
                UDPListener = new class_UDPListener(48620,ref LogHistory, ref SendBuffer);
                thread_UDPListener = new Thread(UDPListener.StartListen);
                thread_UDPListener.Start();
            
            // Update form controls
                con_Button_Listen_ON.Enabled = false;
                con_Button_Listen_OFF.Enabled = true;
                con_txt_mysql_database.Enabled = false;
                con_txt_mysql_username.Enabled = false;
                con_txt_mysql_password.Enabled = false;
                con_txt_mysql_server.Enabled = false;
                con_txt_mysql_port.Enabled = false;
                con_txt_3rd_lotatc.Enabled = false;
                con_txt_3rd_srs.Enabled = false;
                con_check_3rd_lotatc.Enabled = false;
                con_check_3rd_srs.Enabled = false;
            // Prepare connection string
                MySql_connStr = "server="+con_txt_mysql_server.Text+";user="+con_txt_mysql_username.Text+";database="+con_txt_mysql_database.Text+";port="+ con_txt_mysql_port.Text + ";password="+con_txt_mysql_password.Text;

            // Start timmers
                tim_1000ms.Enabled = true;
                tim_10000ms.Enabled = true;

        }

        private void con_Button_Listen_OFF_Click(object sender, EventArgs e)
        {
            // Stop listening
                UDPListener.listener.Close();

            // Update form controls
                con_Button_Listen_ON.Enabled = true;
                con_Button_Listen_OFF.Enabled = false;
                
                con_txt_mysql_database.Enabled = true;
                con_txt_mysql_username.Enabled = true;
                con_txt_mysql_password.Enabled = true;
                con_txt_mysql_port.Enabled = true;
                con_txt_mysql_server.Enabled = true;
                con_txt_3rd_lotatc.Enabled = true;
                con_txt_3rd_srs.Enabled = true;
                con_check_3rd_lotatc.Enabled = true;
                con_check_3rd_srs.Enabled = true;
            
            // Stop timmers
                tim_1000ms.Enabled = false;
                tim_10000ms.Enabled = false;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            // Main timer to sync GUI with background tasks and flush buffers
            // Refresh Log Window
                con_List_Received.Items.Clear();
                foreach (string i in LogHistory)
                {
                    if (i != null)
                    {
                        con_List_Received.Items.Add(i);
                    }
                }

            // Send buffer to MySQL
                for (int i = 0; i < SendBuffer.Length - 1; i++)
                {
                    if (SendBuffer[i] != null)
                    {
                        SendToMySql(SendBuffer[i]);
                        SendBuffer[i] = null;
                    }
                }
        }

        private void con_lab_github_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            // Open default browser with link to Perun repo
                ProcessStartInfo sInfo = new ProcessStartInfo("https://github.com/szporwolik/perun");
                Process.Start(sInfo);
        }

        private void con_Button_Quit_Click(object sender, EventArgs e)
        {
            // Close app
           
                DialogResult dialogResult = MessageBox.Show("Are you sure to exit Perun?", "Question", MessageBoxButtons.YesNo, System.Windows.Forms.MessageBoxIcon.Question);
                if (dialogResult == DialogResult.Yes)
                {
                    // Save settings on exit
                        Properties.Settings.Default.MYSQL_Server = con_txt_mysql_database.Text;
                        Properties.Settings.Default.MYSQL_DB = con_txt_mysql_database.Text;
                        Properties.Settings.Default.MYSQL_User = con_txt_mysql_username.Text;
                        Properties.Settings.Default.MYSQL_Password = con_txt_mysql_password.Text;
                        Properties.Settings.Default.MYSQL_Port = con_txt_mysql_port.Text;
                        Properties.Settings.Default.MYSQL_Server = con_txt_mysql_server.Text;
                        Properties.Settings.Default.MYSQL_Port = con_txt_mysql_port.Text;
                        Properties.Settings.Default.OTHER_LOTATC_FILE = con_txt_3rd_lotatc.Text;
                        Properties.Settings.Default.OTHER_SRS_FILE = con_txt_3rd_srs.Text;
                        Properties.Settings.Default.OTHER_LOTATC_USE = con_check_3rd_lotatc.Checked;
                        Properties.Settings.Default.OTHER_SRS_USE = con_check_3rd_srs.Checked;

                        Properties.Settings.Default.Save();

                    LetMeOut = true;
                    this.Close();
                }
                else if (dialogResult == DialogResult.No)
                {
                    //do nothing
                }
        }

        private void form_Main_FormClosing(object sender, FormClosingEventArgs e)
        {
           // Minimize to try
            if (e.CloseReason == CloseReason.UserClosing && !LetMeOut)
            {
                e.Cancel = true;
                trayIconMain.Visible = true;
                this.WindowState = FormWindowState.Minimized;
                this.ShowInTaskbar = false;
                //this.Hide();

            }
        }

        private void trayIconMain_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            // Maximize from try
            trayIconMain.Visible = false;
            this.WindowState = FormWindowState.Normal;
            this.ShowInTaskbar = true;

        }

        private void con_txt_3rd_srs_Click(object sender, MouseEventArgs e)
        {
            // Chose SRS Json file
            if (openFileDialog_SRS.ShowDialog() == DialogResult.OK)
            {
                con_txt_3rd_srs.Text = openFileDialog_SRS.FileName;
                con_check_3rd_srs.Checked = true;
            } else
            {
                con_txt_3rd_srs.Text = "";
                con_check_3rd_srs.Checked = false;
            }
        }

        private void con_txt_3rd_lotatc_Click(object sender, MouseEventArgs e)
        {
            // Chose LotATC Json file
            if (openFileDialog_LotATC.ShowDialog() == DialogResult.OK)
            {
                con_txt_3rd_lotatc.Text = openFileDialog_LotATC.FileName;
                con_check_3rd_lotatc.Checked = true;
            }
            else
            {
                con_txt_3rd_lotatc.Text = "";
                con_check_3rd_lotatc.Checked = false;
            }
        }

        private void tim_10000ms_Tick(object sender, EventArgs e)
        {
            // Main timer to send JSON files to MySQL
            string strSRSJson ="";
            string strLotATCJson="";

            bool SRSdefault = true;
            bool LotATCdefault = true;

            if (con_check_3rd_srs.Checked)
            {
                try
                {
                    strSRSJson = System.IO.File.ReadAllText(con_txt_3rd_srs.Text);
                    JsonConvert.DeserializeObject(strSRSJson);

                    LogHistoryAdd(ref LogHistory, "SRS data loaded");
                    strSRSJson = "{'type':'100','payload':'" + strSRSJson + "'}";

                    SRSdefault = false;
                }
                catch
                {
                   
                }
            } 
            if(SRSdefault)
            {
                strSRSJson = "{'type':'100','payload':{'ignore':'true'}}";
            }
            SendToMySql(strSRSJson);

            if (con_check_3rd_lotatc.Checked)
            {
                try
                {
                    strLotATCJson = System.IO.File.ReadAllText(con_txt_3rd_lotatc.Text);
                    JsonConvert.DeserializeObject(strLotATCJson);
                    LogHistoryAdd(ref LogHistory, "LotATC data loaded");
                    strLotATCJson = "{'type':'101','payload':'" + strLotATCJson + "'}";
                }
                catch
                {

                }
                
            }

            if (LotATCdefault)
            {
                strLotATCJson = "{'type':'101','payload':{'ignore':'true'}}";
            }
            SendToMySql(strLotATCJson);
    
        }
    }
}
