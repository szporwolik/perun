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
using Newtonsoft.Json.Linq;

namespace Perun_v1
{
    public partial class form_Main : Form
    {

        // Variable definitions
        public Thread thread_UDPListener;               // Seperate thread for UDP
        public class_UDPListener UDPListener;           // Helper class for UDP comunication
        public string[] LogHistory = new string[10];      // Log history for GUI
        public string[] SendBuffer = new string[100];      // Mysql send buffer
        public bool LetMeOut = false;                     // Helper to handle system tray
        public string MySql_connStr;                    // MySQL connection string
        public string publishVersion = "DEBUG";         // Helper for pulling version definition

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
            string timestamp = udp_frame.timestamp;
            string sql = "";

            if (timestamp != null)
            {
                timestamp = "'" + timestamp + "'";
            }
            else
            {
                timestamp = "CURRENT_TIMESTAMP()";
            }

            // Modify specific
            if (type == "1") // Inject app version information
            {
                udp_frame.payload["v_win"] = "v" + publishVersion;
            }

            // Specific SQL 
            if (type == "50")
            {
                // Add entry to chat log
                //sql = "INSERT IGNORE INTO `pe_DataPlayers` (`pe_DataPlayers_ucid`) VALUES ('" + udp_frame.payload.ucid + "');";
                sql = "INSERT INTO `pe_DataPlayers` (`pe_DataPlayers_ucid`) SELECT '" + udp_frame.payload.ucid + "' FROM DUAL WHERE NOT EXISTS (SELECT * FROM `pe_DataPlayers` where pe_DataPlayers_ucid = '" + udp_frame.payload.ucid + "' );";
                sql = sql + "UPDATE  `pe_DataPlayers` SET `pe_DataPlayers_updated` = " + timestamp + ",`pe_DataPlayers_lastname`='" + udp_frame.payload.player + "' WHERE `pe_DataPlayers_ucid`='"+ udp_frame.payload.ucid + "' ;";
                sql = sql + "INSERT INTO `pe_DataMissionHashes` (`pe_DataMissionHashes_hash`) SELECT '" + udp_frame.payload.missionhash + "' FROM DUAL WHERE NOT EXISTS (SELECT * FROM `pe_DataMissionHashes` where pe_DataMissionHashes_hash ='" + udp_frame.payload.missionhash + "' );";
                sql = sql + "UPDATE `pe_DataMissionHashes` SET `pe_DataMissionHashes_datetime` = " + timestamp + " WHERE `pe_DataMissionHashes_hash` = '" + udp_frame.payload.missionhash + "';";
                sql = sql + "INSERT INTO `pe_LogChat` (`pe_LogChat_id`,`pe_LogChat_datetime`, `pe_LogChat_playerid`, `pe_LogChat_msg`, `pe_LogChat_all`,`pe_LogChat_missionhash_id`) VALUES (NULL,'" + udp_frame.payload.datetime + "', (SELECT pe_DataPlayers_id from pe_DataPlayers WHERE pe_DataPlayers_ucid = '"+ udp_frame.payload.ucid + "'), '" + udp_frame.payload.msg + "', '" + udp_frame.payload.all + "',(SELECT pe_DataMissionHashes_id FROM pe_DataMissionHashes WHERE pe_DataMissionHashes_hash = '" + udp_frame.payload.missionhash + "'));";
            }
            else if (type == "51")
            {
                // Add entry to event log
                sql = "INSERT INTO `pe_DataMissionHashes` (`pe_DataMissionHashes_hash`) SELECT '" + udp_frame.payload.log_missionhash + "' FROM DUAL WHERE NOT EXISTS (SELECT * FROM `pe_DataMissionHashes` where pe_DataMissionHashes_hash = '" + udp_frame.payload.log_missionhash + "');";
                sql = sql + "UPDATE `pe_DataMissionHashes` SET `pe_DataMissionHashes_datetime` = " + timestamp + " WHERE `pe_DataMissionHashes_hash` = '" + udp_frame.payload.log_missionhash + "';";
                sql = sql + "INSERT INTO `pe_LogEvent` (`pe_LogEvent_id`, `pe_LogEvent_datetime`, `pe_LogEvent_type`, `pe_LogEvent_content`,`pe_LogEvent_missionhash_id`) VALUES ( NULL, '" + udp_frame.payload.log_datetime + "', '" + udp_frame.payload.log_type + "', '" + udp_frame.payload.log_content + "', (SELECT pe_DataMissionHashes_id FROM pe_DataMissionHashes WHERE pe_DataMissionHashes_hash = '" + udp_frame.payload.log_missionhash + "'));";
            }
            else if (type == "52")
            {
                // Update user stats
                payload = JsonConvert.SerializeObject(udp_frame.payload.stat_data);

                sql =  "INSERT INTO `pe_DataMissionHashes` (`pe_DataMissionHashes_hash`) SELECT '" + udp_frame.payload.stat_missionhash + "' FROM DUAL WHERE NOT EXISTS (SELECT * FROM `pe_DataMissionHashes` where pe_DataMissionHashes_hash = '" + udp_frame.payload.stat_missionhash + "');";
                sql = sql + "UPDATE `pe_DataMissionHashes` SET `pe_DataMissionHashes_datetime` = " + timestamp + " WHERE `pe_DataMissionHashes_hash` = '" + udp_frame.payload.stat_missionhash + "';";
                sql = sql + "INSERT INTO `pe_DataPlayers` (`pe_DataPlayers_ucid`) SELECT '" + udp_frame.payload.stat_ucid + "' FROM DUAL WHERE NOT EXISTS (SELECT * FROM `pe_DataPlayers` where pe_DataPlayers_ucid = '" + udp_frame.payload.stat_ucid + "');";
                sql = sql + "UPDATE `pe_DataPlayers` SET `pe_DataPlayers_updated`=" + timestamp + " WHERE `pe_DataPlayers_ucid`='" + udp_frame.payload.stat_ucid + "';";
                sql = sql + "INSERT INTO `pe_LogStats` (`pe_LogStats_playerid`,`pe_LogStats_missionhash_id`) SELECT (SELECT pe_DataPlayers_id from pe_DataPlayers WHERE pe_DataPlayers_ucid = '" + udp_frame.payload.stat_ucid + "'), (SELECT pe_DataMissionHashes_id FROM pe_DataMissionHashes WHERE pe_DataMissionHashes_hash = '" + udp_frame.payload.stat_missionhash + "') FROM DUAL WHERE NOT EXISTS (SELECT * FROM `pe_LogStats` WHERE `pe_LogStats_missionhash_id`=(SELECT pe_DataMissionHashes_id FROM pe_DataMissionHashes WHERE pe_DataMissionHashes_hash = '" + udp_frame.payload.stat_missionhash + "') AND `pe_LogStats_playerid` =  (SELECT pe_DataPlayers_id from pe_DataPlayers WHERE pe_DataPlayers_ucid = '" + udp_frame.payload.stat_ucid + "') );";
                sql = sql + "UPDATE `pe_LogStats` SET `pe_LogStats_datetime`='" + udp_frame.payload.stat_datetime + "',`pe_LogStats_playerid` =  (SELECT pe_DataPlayers_id from pe_DataPlayers WHERE pe_DataPlayers_ucid = '" + udp_frame.payload.stat_ucid + "'),`pe_LogStats_debug`=JSON_QUOTE('" + payload + "'),`pe_LogStats_missionhash_id`=(SELECT pe_DataMissionHashes_id FROM pe_DataMissionHashes WHERE pe_DataMissionHashes_hash = '" + udp_frame.payload.stat_missionhash + "') WHERE `pe_LogStats_missionhash_id`=(SELECT pe_DataMissionHashes_id FROM pe_DataMissionHashes WHERE pe_DataMissionHashes_hash = '" + udp_frame.payload.stat_missionhash + "') AND `pe_LogStats_playerid` =  (SELECT pe_DataPlayers_id from pe_DataPlayers WHERE pe_DataPlayers_ucid = '" + udp_frame.payload.stat_ucid + "');";
            }
            else if (type == "53")
            {
                // User logged in to DCS server
                payload = JsonConvert.SerializeObject(udp_frame.payload.stat_data);

                sql = "INSERT INTO `pe_DataPlayers` (`pe_DataPlayers_ucid`) SELECT '" + udp_frame.payload.login_ucid + "' FROM DUAL WHERE NOT EXISTS (SELECT * FROM `pe_DataPlayers` where pe_DataPlayers_ucid='" + udp_frame.payload.login_ucid + "');";
                sql = sql + "UPDATE `pe_DataPlayers` SET  pe_DataPlayers_lastip='" + udp_frame.payload.login_ipaddr + "', pe_DataPlayers_lastname='" + udp_frame.payload.login_name + "',pe_DataPlayers_updated='" + udp_frame.payload.login_datetime + "' WHERE `pe_DataPlayers_ucid`= '" + udp_frame.payload.login_ucid + "';";
                sql = sql + "INSERT INTO `pe_LogLogins` (`pe_LogLogins_datetime`, `pe_LogLogins_playerid`, `pe_LogLogins_name`, `pe_LogLogins_ip`) VALUES ('" + udp_frame.payload.login_datetime + "', (SELECT pe_DataPlayers_id from pe_DataPlayers WHERE pe_DataPlayers_ucid = '" + udp_frame.payload.login_ucid + "'), '" + udp_frame.payload.login_name + "', '" + udp_frame.payload.login_ipaddr + "');";
            }
            else
            {
                // General definition used for 1-10 packets
                payload = JsonConvert.SerializeObject(udp_frame.payload);
                sql = "INSERT INTO `pe_DataRaw` (`pe_dataraw_type`) SELECT '" + type + "' FROM DUAL WHERE NOT EXISTS (SELECT * FROM `pe_DataRaw` WHERE pe_dataraw_type = '" + type + "' );";
                sql = sql + "UPDATE `pe_DataRaw` SET `pe_dataraw_payload` = JSON_QUOTE('" + payload + "'), `pe_dataraw_updated`="+ timestamp + " WHERE `pe_dataraw_type`=" + type + ";";
            }

            // Connect to mysql and execute sql
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
                LogHistoryAdd(ref LogHistory, "MySQL updated, package type: " + type);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                LogHistoryAdd(ref LogHistory, "ERROR MySQL: type " + type);
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
                        LogHistoryAdd(ref LogHistory, "UDP packet received, type: " + type);

                        // Add to mySQL send buffer and rotate
                        for (int i = 0; i < SendBuffer.Length - 1; i++)
                        {
                            if (SendBuffer[i]==null)
                            {
                                SendBuffer[i] = received_data;
                                break;
                            }
                            
                        }

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
            LogHistory[0] = DateTime.Now.ToString("HH:mm:ss") + " > " + "Perun loaded";

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
            if (System.Deployment.Application.ApplicationDeployment.IsNetworkDeployed)
            {
                System.Deployment.Application.ApplicationDeployment cd = System.Deployment.Application.ApplicationDeployment.CurrentDeployment;
                publishVersion = cd.CurrentVersion.ToString();
            }
            this.Text = this.Text + " - v" + publishVersion;
        }

        private void con_Button_Listen_ON_Click(object sender, EventArgs e)
        {
            // Start listening
            UDPListener = new class_UDPListener(48620, ref LogHistory, ref SendBuffer);
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

            // Save settings
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

            // Prepare connection string
            MySql_connStr = "server=" + con_txt_mysql_server.Text + ";user=" + con_txt_mysql_username.Text + ";database=" + con_txt_mysql_database.Text + ";port=" + con_txt_mysql_port.Text + ";password=" + con_txt_mysql_password.Text;

            // Start timmers
            tim_1000ms.Enabled = true;
            tim_10000ms.Enabled = true;
            tim_200ms.Enabled = true;

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
            tim_200ms.Enabled = false;
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
            }
            else
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
            string strSRSJson = "";
            string strLotATCJson = "";

            bool SRSdefault = true;
            bool LotATCdefault = true;

            // Handle SRS
            if (con_check_3rd_srs.Checked)
            {
                try
                {
                    strSRSJson = System.IO.File.ReadAllText(con_txt_3rd_srs.Text);
                    dynamic raw_lotatc = JsonConvert.DeserializeObject(strSRSJson);

                    for (int i = 0; i < raw_lotatc.Count; i++)
                    {

                        if (raw_lotatc[i].RadioInfo != null)
                        {

                            int temp = raw_lotatc[i].RadioInfo.radios.Count - 1;
                            for (int j = temp; j >= 0; j--)
                            {
                                raw_lotatc[i].RadioInfo.radios[j].enc.Parent.Remove();
                                raw_lotatc[i].RadioInfo.radios[j].encKey.Parent.Remove();
                                raw_lotatc[i].RadioInfo.radios[j].encMode.Parent.Remove();
                                raw_lotatc[i].RadioInfo.radios[j].freqMax.Parent.Remove();
                                raw_lotatc[i].RadioInfo.radios[j].freqMin.Parent.Remove();
                                raw_lotatc[i].RadioInfo.radios[j].modulation.Parent.Remove();
                                raw_lotatc[i].RadioInfo.radios[j].freqMode.Parent.Remove();
                                raw_lotatc[i].RadioInfo.radios[j].volMode.Parent.Remove();
                                raw_lotatc[i].RadioInfo.radios[j].expansion.Parent.Remove();
                                raw_lotatc[i].RadioInfo.radios[j].channel.Parent.Remove();
                                raw_lotatc[i].RadioInfo.radios[j].simul.Parent.Remove();

                                if (raw_lotatc[i].RadioInfo.radios[j].name == "No Radio")
                                {
                                    raw_lotatc[i].RadioInfo.radios[j].Remove();
                                }
                            }
                            raw_lotatc[i].ClientChannelId.Parent.Remove();
                            raw_lotatc[i].RadioInfo.simultaneousTransmission.Parent.Remove();
                        }
                    }

                    if (raw_lotatc.Count > 0)
                    {
                        strSRSJson = JsonConvert.SerializeObject(raw_lotatc);
                        strSRSJson = "{'type':'100','payload':'" + strSRSJson + "'}";
                    }
                    else
                    {
                        strSRSJson = "{'type':'100','payload':{'ignore':'false'}}";
                    }
                    SRSdefault = false;
                    LogHistoryAdd(ref LogHistory, "SRS data loaded");

                }
                catch
                {
                    LogHistoryAdd(ref LogHistory, "SRS data ERROR");
                }


            }
            if (SRSdefault)
            {
                strSRSJson = "{'type':'100','payload':{'ignore':'true'}}";
            }
            SendToMySql(strSRSJson);

            // Handle LotATC
            if (con_check_3rd_lotatc.Checked)
            {
                try
                {
                    strLotATCJson = System.IO.File.ReadAllText(con_txt_3rd_lotatc.Text);
                    dynamic raw_srs = JsonConvert.DeserializeObject(strLotATCJson);

                    strLotATCJson = "{'type':'101','payload':'" + strLotATCJson + "'}";
                    LotATCdefault = false;
                    LogHistoryAdd(ref LogHistory, "LotATC data loaded");
                }
                catch
                {
                    LogHistoryAdd(ref LogHistory, "LotATC data ERROR");
                }


            }

            if (LotATCdefault)
            {
                strLotATCJson = "{'type':'101','payload':{'ignore':'true'}}";
            }
            SendToMySql(strLotATCJson);

        }

        private void tim_200ms_Tick(object sender, EventArgs e)
        {
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
    }
}
