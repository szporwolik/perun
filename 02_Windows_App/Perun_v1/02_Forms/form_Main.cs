using System;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace Perun_v1
{
    public partial class form_Main : Form
    {
        // Variable definitions
        public string[] arrSendBuffer = new string[100];   // Mysql send buffer
        public bool boolLetMeOut = false;                   // Helper to handle system tray

        // ################################ Main ################################
        private void form_Main_Load(object sender, EventArgs e)
        {
            // Form loaded - fill controls with default values

            Globals.arrLogHistory[0] = DateTime.Now.ToString("HH:mm:ss") + " > " + "Perun started";
            form_Main_LoadSettings(); // Load settings
            this.Text = PerunHelper.GetAppVersion(this.Text + " - "); // Display build version in title bar
        }

        public form_Main()
        {
            // Form init
            InitializeComponent();
        }

        // ################################ Helpers ################################
        private void form_Main_LoadSettings()
        {
            // Loads settings
            con_txt_mysql_database.Text = Properties.Settings.Default.MYSQL_DB;
            con_txt_mysql_username.Text = Properties.Settings.Default.MYSQL_User;
            con_txt_mysql_password.Text = Properties.Settings.Default.MYSQL_Password;
            con_txt_mysql_server.Text = Properties.Settings.Default.MYSQL_Server;
            con_txt_mysql_port.Text = Properties.Settings.Default.MYSQL_Port;
            con_txt_3rd_lotatc.Text = Properties.Settings.Default.OTHER_LOTATC_FILE;
            con_txt_3rd_srs.Text = Properties.Settings.Default.OTHER_SRS_FILE;
            con_check_3rd_lotatc.Checked = Properties.Settings.Default.OTHER_LOTATC_USE;
            con_check_3rd_srs.Checked = Properties.Settings.Default.OTHER_SRS_USE;
            con_txt_dcs_server_port.Text = Properties.Settings.Default.DCS_Server_Port.ToString();
            con_txt_dcs_instance.Text = Properties.Settings.Default.DCS_Instance.ToString();

        }

        private void form_Main_SaveSettings()
        {
            // Saves settings
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

            Properties.Settings.Default.DCS_Server_Port = Int32.Parse(con_txt_dcs_server_port.Text);
            Properties.Settings.Default.DCS_Instance = Int32.Parse(con_txt_dcs_instance.Text);

            Properties.Settings.Default.Save();
        }

        private void form_Main_DisableControls()
        {
            // Disables controls
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
            con_txt_dcs_server_port.Enabled = false;
            con_txt_dcs_instance.Enabled = false;
        }

        private void form_Main_EnableControls()
        {
            // Enables controls
            con_Button_Listen_ON.Enabled = true;
            con_Button_Listen_OFF.Enabled = false;

            con_txt_mysql_database.Enabled = true;
            con_txt_mysql_username.Enabled = true;
            con_txt_mysql_password.Enabled = true;
            //con_txt_mysql_port.Enabled = true;    // TBD - protect against writting text in the port field
            con_txt_mysql_server.Enabled = true;
            con_txt_3rd_lotatc.Enabled = true;
            con_txt_3rd_srs.Enabled = true;
            con_check_3rd_lotatc.Enabled = true;
            con_check_3rd_srs.Enabled = true;
            con_txt_dcs_server_port.Enabled = true;
            con_txt_dcs_instance.Enabled = true;

        }

        // ################################ User input ################################
        private void con_Button_Listen_ON_Click(object sender, EventArgs e)
        {
            // Start listening
            //UDPController.Create(48620, ref Globals.arrLogHistory, ref arrSendBuffer);
            //UDPController.thrUDPListener = new Thread(UDPController.StartListen);
            //UDPController.thrUDPListener.Start();
            //UDPController.thrUDPListener.Name = "UDPThread";

            TCPController.Create(48620, ref Globals.arrLogHistory, ref arrSendBuffer);
            TCPController.thrTCPListener = new Thread(TCPController.StartListen);
            TCPController.thrTCPListener.Start();
            TCPController.thrTCPListener.Name = "TCPThread";

            form_Main_DisableControls(); // Disable controlls
            form_Main_SaveSettings(); // Save settings

            // Prepare connection string
            DatabaseController.strMySQLConnectionString = "server=" + con_txt_mysql_server.Text + ";user=" + con_txt_mysql_username.Text + ";database=" + con_txt_mysql_database.Text + ";port=" + con_txt_mysql_port.Text + ";password=" + con_txt_mysql_password.Text;

            // Start timmers
            tim_200ms.Enabled = true;
            tim_1000ms.Enabled = true;
            tim_10000ms.Enabled = true;
        }

        private void con_Button_Listen_OFF_Click(object sender, EventArgs e)
        {
            // Stop listening
            try
            {
                //UDPController.StopListen();
                TCPController.StopListen();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            form_Main_EnableControls(); // Enable controls

            // Stop timmers
            tim_1000ms.Enabled = false;
            tim_10000ms.Enabled = false;
            tim_200ms.Enabled = false;
        }

        private void con_lab_github_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            // Open default browser with link to Perun repo
            ProcessStartInfo sInfo = new ProcessStartInfo("https://github.com/szporwolik/perun");
            Process.Start(sInfo);
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

        // ################################ Form state ################################
        private void con_Button_Quit_Click(object sender, EventArgs e)
        {
            // Close app
            DialogResult dialogResult = MessageBox.Show("Are you sure to exit Perun?", "Question", MessageBoxButtons.YesNo, System.Windows.Forms.MessageBoxIcon.Question);
            if (dialogResult == DialogResult.Yes)
            {
                form_Main_SaveSettings();

                boolLetMeOut = true; // Save settings on exit
                this.Close();        // Allow to exit application
            }
            else if (dialogResult == DialogResult.No)
            {
                //do nothing
            }
        }

        protected override void WndProc(ref Message m)
        {
            // Try to run of 2nd instance
            if (m.Msg == NativeMethods.WM_SHOWME)
            {
                form_Main_BringFromTray();
            }
            base.WndProc(ref m);
        }

        private void form_Main_SendToTray()
        {
            // Sends app to system tray
            trayIconMain.Visible = true;
            this.WindowState = FormWindowState.Minimized;
            this.ShowInTaskbar = false;
        }

        private void form_Main_BringFromTray()
        {
            // Sends app from system tray
            trayIconMain.Visible = false;
            this.WindowState = FormWindowState.Normal;
            this.ShowInTaskbar = true;

            if (WindowState == FormWindowState.Minimized)
            {
                WindowState = FormWindowState.Normal;
            }
            // get our current "TopMost" value (ours will always be false though)
            bool top = TopMost;
            // make our form jump to the top of everything
            TopMost = true;
            // set it back to whatever it was
            TopMost = top;
        }


        private void form_Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Minimize to try on clicking "X"
            if (e.CloseReason == CloseReason.UserClosing && !boolLetMeOut)
            {
                e.Cancel = true;
                form_Main_SendToTray(); // Send app to system tray
            }
        }

        private void trayIconMain_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            // Maximize from tray
            form_Main_BringFromTray(); // Bring app from system tray
        }

        // ################################ Timers ################################
        private void timer1_Tick(object sender, EventArgs e)
        {
            // Main timer to sync GUI with background tasks and flush buffers
            // Refresh Log Window
            con_List_Received.Items.Clear();
            foreach (string i in Globals.arrLogHistory)
            {
                if (i != null)
                {
                    con_List_Received.Items.Add(i);
                }
            }
        }

        private void tim_200ms_Tick(object sender, EventArgs e)
        {
            // Send buffer to MySQL
            for (int i = 0; i < arrSendBuffer.Length - 1; i++)
            {
                if (arrSendBuffer[i] != null)
                {
                    DatabaseController.SendToMySql(arrSendBuffer[i]);
                    arrSendBuffer[i] = null;
                }
            }
        }

        private void tim_10000ms_Tick(object sender, EventArgs e)
        {
            // Main timer to send JSON files to MySQL
            string strSRSJson = "";
            string strLotATCJson = "";

            bool boolSRSdefault = true;
            bool boolLotATCdefault = true;

            // Handle SRS - TBD clean code for multiinstance
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
                        strSRSJson = "{'type':'100','instance':'" + Int32.Parse(con_txt_dcs_instance.Text) + "','payload':'" + strSRSJson + "'}";
                    }
                    else
                    {
                        strSRSJson = "{'type':'100','instance':'" + Int32.Parse(con_txt_dcs_instance.Text) + "','payload':{'ignore':'false'}}"; // No SRS clients connected
                    }
                    boolSRSdefault = false;
                    PerunHelper.LogHistoryAdd(ref Globals.arrLogHistory, "#" + Int32.Parse(con_txt_dcs_instance.Text) + " > SRS data loaded");

                }
                catch
                {
                    PerunHelper.LogHistoryAdd(ref Globals.arrLogHistory, "#" + Int32.Parse(con_txt_dcs_instance.Text) + " > SRS data ERROR");
                }


            }
            if (boolSRSdefault)
            {
                strSRSJson = "{'type':'100','instance':'" + Int32.Parse(con_txt_dcs_instance.Text) + "','payload':{'ignore':'true'}}";
            }
            DatabaseController.SendToMySql(strSRSJson);

            // Handle LotATC - TBD clean code for multiinstance
            if (con_check_3rd_lotatc.Checked)
            {
                try
                {
                    strLotATCJson = System.IO.File.ReadAllText(con_txt_3rd_lotatc.Text);
                    dynamic raw_srs = JsonConvert.DeserializeObject(strLotATCJson);

                    strLotATCJson = "{'type':'101','instance':'" + Int32.Parse(con_txt_dcs_instance.Text) + "','payload':'" + strLotATCJson + "'}";
                    boolLotATCdefault = false;
                    PerunHelper.LogHistoryAdd(ref Globals.arrLogHistory, "#" + Int32.Parse(con_txt_dcs_instance.Text) + " > LotATC data loaded");
                }
                catch
                {
                    PerunHelper.LogHistoryAdd(ref Globals.arrLogHistory, "#" + Int32.Parse(con_txt_dcs_instance.Text) + " > LotATC data ERROR");
                }


            }
            if (boolLotATCdefault)
            {
                strLotATCJson = "{'type':'101','instance':'" + Int32.Parse(con_txt_dcs_instance.Text) + "','payload':{'ignore':'true'}}";   // No LotATC controller connected
            }
            DatabaseController.SendToMySql(strLotATCJson);
        }
    }
}
