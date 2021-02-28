using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;

namespace Perun_v1
{
    public partial class form_Main : Form
    {
        // Variable definitions
        
        public bool AppCanClose = false;                                            // Helper to handle system tray - true needed to quit the app
        public bool ExtSRSStatus;                                                   // True if to use empty/default SRS status
        public bool ExtLotATCStatus;                                                // True if to use empty/default LotATC status

        // ################################ Main ################################
        private void form_Main_Load(object sender, EventArgs e)
        {
            // Load settings from registry
            form_Main_LoadSettings();

            // Form loaded 
            PerunHelper.LogInfo(ref Globals.AppLogHistory, PerunHelper.GetAppVersion("Perun started - "));
              
            // Display build version in title bar
            Globals.AppTitle = PerunHelper.GetAppVersion(this.Text + " - ");
            this.Text = Globals.AppTitle;

            // Use command line parameters
            string[] args = Environment.GetCommandLineArgs();
            if (args.Length > 1)
            {
                // Get argument server port
                if (args[1] != null)
                {
                    if (args[1] != "-1")
                    {
                        con_txt_dcs_server_port.Text = args[1];
                    }
                }
            }
            if (args.Length > 2)
            {
                // Get argument instance id
                if (args[2] != null)
                {
                    if (args[2] != "-1")
                    {
                        con_txt_dcs_instance.Text = args[2];
                    }
                }
            }

            if (args.Length > 3)
            {
                // Get argument DCS SRS file path
                if (args[3] != null)
                {
                    if (args[3] != "-1")
                    {
                        con_txt_3rd_srs.Text = args[3];
                        con_check_3rd_srs.Checked = true;
                    }
                }
            }
            if (args.Length > 4)
            {
                // Get argument lotATC file path
                if (args[4] != null)
                {
                    if (args[4] != "-1")
                    {
                        con_txt_3rd_lotatc.Text = args[4];
                        con_check_3rd_lotatc.Checked = true;
                    }
                }
            }

            if (args.Length > 5)
            {
                // Get argument lotATC file path
                if (args[5] != null)
                {
                    if (args[5] == "1")
                    {
                        if (args[5] != "-1")
                        {
                            TIM_Autostart.Enabled = true;
                            PerunHelper.LogInfo(ref Globals.AppLogHistory, "Autostart parameter provided", 0, 1);
                        }
                    }
                }
            }

            // Initialize controls
            con_img_db.Image = (Image)Properties.Resources.ResourceManager.GetObject("status_disconnected");
            con_img_dcs.Image = (Image)Properties.Resources.ResourceManager.GetObject("status_disconnected");
            con_img_lotATC.Image = (Image)Properties.Resources.ResourceManager.GetObject("status_disconnected");
            con_img_srs.Image = (Image)Properties.Resources.ResourceManager.GetObject("status_disconnected");

            this.form_Main_SetBlankMissionInfo();
        }

        public form_Main()
        {
            // Form init
            InitializeComponent();
        }

        // ################################ Helpers ################################
        private void form_Main_LoadSettings()
        {
            // Loads registry settings
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
            con_com_loglevel.SelectedIndex = Properties.Settings.Default.OTHER_Log_Level;
            con_check_minimize_to_tray.Checked = Properties.Settings.Default.OTHER_Minimize_to_Tray;
            con_check_delete_logs.Checked = Properties.Settings.Default.OTHER_Log_Rotate;
        }

        private void form_Main_SaveSettings()
        {
            // Saves registry settings
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
            Properties.Settings.Default.OTHER_Log_Level = con_com_loglevel.SelectedIndex;
            Properties.Settings.Default.OTHER_Minimize_to_Tray = con_check_minimize_to_tray.Checked;
            Properties.Settings.Default.OTHER_Log_Rotate = con_check_delete_logs.Checked;

            Properties.Settings.Default.Save();
        }

        private void form_Main_SetControlsToConnected()
        {
            // Disables controls
            con_Button_Add_Marker.Enabled = true;
            con_Button_Listen_OFF.Enabled = true;
            con_Button_Listen_ON.Enabled = false;
            con_Button_Quit.Enabled = false;
            con_Button_Reset_Flags.Enabled = true;
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
            con_com_loglevel.Enabled = true;
        }

        private void form_Main_SetControlsToDisconnected()
        {
            // Enables controls
            con_Button_Reset_Flags.Enabled = false;
            con_Button_Listen_OFF.Enabled = false;
            con_Button_Listen_ON.Enabled = true;
            con_Button_Quit.Enabled = true;
            con_Button_Add_Marker.Enabled = false;
            con_txt_mysql_database.Enabled = true;
            con_txt_mysql_username.Enabled = true;
            con_txt_mysql_password.Enabled = true;
            con_txt_mysql_port.Enabled = true;
            con_txt_mysql_server.Enabled = true;
            con_txt_3rd_lotatc.Enabled = true;
            con_txt_3rd_srs.Enabled = true;
            con_check_3rd_lotatc.Enabled = true;
            con_check_3rd_srs.Enabled = true;
            con_txt_dcs_server_port.Enabled = true;
            con_txt_dcs_instance.Enabled = true;
            con_com_loglevel.Enabled = true;

            this.form_Main_SetBlankMissionInfo();
        }

        private void form_Main_SetBlankMissionInfo()
        {
            // Init values for the controls

            // Mission information
            label16.Text = "[unknown]";
            label17.Text = "[unknown]";
            label18.Text = "[unknown]";
            label19.Text = "?";
            label21.Text = "[unknown]";
        }

        // ################################ User input ################################
        private void con_Button_Listen_ON_Click(object sender, EventArgs e)
        {
            // Start listening
            // Set globals
            Globals.AppInstanceID = Int32.Parse(con_txt_dcs_instance.Text);
            Globals.AppForceIconReload = true;
            Globals.ErrorsDatabase = 0;             // Reset error counter
            Globals.ErrorsGame = 0;                 // Reset error counter
            Globals.ErrorsSRS = 0;                  // Reset error counter
            Globals.ErrorsLotATC = 0;               // Reset error counter
            Globals.StatusConnection = false;       // Reset connection status

            // Prepare GUI
            form_Main_SetControlsToConnected();
            form_Main_SaveSettings();
            this.Text = $"[#{con_txt_dcs_instance.Text}] {Globals.AppTitle}"; // Set title bar
            trayIconMain.Text = this.Text; // Set notification icon text

            // Prepare MySQL connection string
            Globals.DatabaseConnection.DatabaseConnectionString = $"server={con_txt_mysql_server.Text};user={con_txt_mysql_username.Text};database={con_txt_mysql_database.Text};port={con_txt_mysql_port.Text};password={con_txt_mysql_password.Text}";

            // Start listening
            PerunHelper.LogInfo(ref Globals.AppLogHistory, "Opening connections", 0, 1);
            Globals.TCPServer.Create(Int32.Parse(con_txt_dcs_server_port.Text), ref Globals.AppLogHistory, ref Globals.DatabaseSendBuffer);
            Globals.TCPServer.thrTCPListener = new Thread(Globals.TCPServer.StartListen);
            Globals.TCPServer.thrTCPListener.Start();
            Globals.TCPServer.thrTCPListener.Name = "TCPThread";

            // Start timmers
            tim_MySQL.Enabled = true;
            tim_GUI.Enabled = true;
            tim_3rdparties.Enabled = true;

            // Send initial data
            Tim_MySQL_Tick(null, null);
            tim_3rdparties_Tick(null, null);
            TIM_Autostart.Enabled = false;
        }

        private void con_Button_Listen_OFF_Click(object sender, EventArgs e)
        {
            // Stop listening
            // Prepare GUI
            PerunHelper.LogInfo(ref Globals.AppLogHistory, "Closing connections", 0, 1);
            con_Button_Listen_OFF.Enabled = false;
            Tim_GUI_Tick(null, null);
            this.Refresh();
            Application.DoEvents();

            // Stop timmers
            tim_GUI.Enabled = false;
            tim_3rdparties.Enabled = false;
            tim_MySQL.Enabled = false;

            try
            {
                // Stop the server
                Globals.TCPServer.StopListen();
            }
            catch (Exception ex)
            {
                PerunHelper.LogError(ref Globals.AppLogHistory, $"ERROR TCP, error: {ex.Message}", 2, 1, "?");
                Console.WriteLine(ex.ToString());
            }

            while (Globals.TCPServer.thrTCPListener.IsAlive)
            {
                // Wait untill TCP server closed connection
                Thread.Sleep(200); //ms
            }
            form_Main_SetControlsToDisconnected(); // Enable controls

            // Load status images
            con_img_db.Image = (Image)Properties.Resources.ResourceManager.GetObject("status_disconnected");
            con_img_dcs.Image = (Image)Properties.Resources.ResourceManager.GetObject("status_disconnected");
            con_img_lotATC.Image = (Image)Properties.Resources.ResourceManager.GetObject("status_disconnected");
            con_img_srs.Image = (Image)Properties.Resources.ResourceManager.GetObject("status_disconnected");

            // Display information about closed connections
            PerunHelper.LogInfo(ref Globals.AppLogHistory, "Connections closed", 0, 1);
            Tim_GUI_Tick(null, null);

            // Set title bar
            this.Text = Globals.AppTitle;
            trayIconMain.Text = this.Text;

            // Set globals
            Globals.AppInstanceID = 0;
            Globals.AppUpdateGUI = false;
            Globals.StatusDatabase = false;
            Globals.StatusHistoryConnection = false;
            Globals.StatusSRS = false;
            Globals.StatusLotATC = false;
            Globals.StatusConnection = false;
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

        private void con_com_loglevell_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Changed log level state
            LogController.instance.level = con_com_loglevel.SelectedIndex;
        }

        private void con_check_minimize_to_tray_Validated(object sender, EventArgs e)
        {
            // Changed close to try state
            Properties.Settings.Default.OTHER_Minimize_to_Tray = con_check_minimize_to_tray.Checked;
        }

        private void con_check_delete_logs_Validated(object sender, EventArgs e)
        {
            // Changed rotate logs state
            Properties.Settings.Default.OTHER_Log_Rotate = con_check_delete_logs.Checked;
        }

        private void con_Button_Reset_Flags_Click(object sender, EventArgs e)
        {
            // Reset error flags
            DialogResult dialogResult = MessageBox.Show("Are you sure to reset error flags?", "Question", MessageBoxButtons.YesNo, System.Windows.Forms.MessageBoxIcon.Question);
            if (dialogResult == DialogResult.Yes)
            {
                // Reset errors counter
                Globals.ErrorsDatabase = 0;                         // MySQL - Error counter
                Globals.ErrorsGame = 0;                             // TCP connection - Error counter
                Globals.ErrorsHistoryGame = 0;                      // TCP connection - historic value of Error counter
                Globals.ErrorsSRS = 0;                              // DCS SRS - error counter
                Globals.ErrorsLotATC = 0;                           // LotATC - error counter

                // Force icons reload
                Globals.AppForceIconReload = true;

                // Add information
                PerunHelper.LogInfo(ref Globals.AppLogHistory, "Resetted error counter", 0, 1);
            }
            else if (dialogResult == DialogResult.No)
            {
                // Do nothing
            }

        }

        private void con_Button_Add_Marker_Click(object sender, EventArgs e)
        {
            // Added user marker
            PerunHelper.LogError(ref Globals.AppLogHistory, "User Marker", 0, 1);
        }

        // ################################ Form state ################################
        private void con_Button_Quit_Click(object sender, EventArgs e)
        {
            // Try to close app
            DialogResult dialogResult = MessageBox.Show("Are you sure to exit Perun?", "Question", MessageBoxButtons.YesNo, System.Windows.Forms.MessageBoxIcon.Question);
            if (dialogResult == DialogResult.Yes)
            {
                form_Main_SaveSettings();

                AppCanClose = true; // Save settings on exit
                Globals.TCPServer.StopListen();
                Application.Exit(); // Quit
            }
            else if (dialogResult == DialogResult.No)
            {
                //do nothing
            }
        }

        private void form_Main_SendToTray()
        {
            // Sends app to system tray
            trayIconMain.Visible = true;

            // For instances 1-5 we have prepared special icons
            switch (Globals.AppInstanceID)
            {
                case 1:
                    trayIconMain.Icon = Perun_v1.Properties.Resources.perun_logo_1;
                    break;
                case 2:
                    trayIconMain.Icon = Perun_v1.Properties.Resources.perun_logo_2;
                    break;
                case 3:
                    trayIconMain.Icon = Perun_v1.Properties.Resources.perun_logo_3;
                    break;
                case 4:
                    trayIconMain.Icon = Perun_v1.Properties.Resources.perun_logo_4;
                    break;
                case 5:
                    trayIconMain.Icon = Perun_v1.Properties.Resources.perun_logo_5;
                    break;
                default:
                    break;
            }
            

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
            
            bool top = TopMost; // Get our current "TopMost" value (ours will always be false though)
            TopMost = true; // Make our form jump to the top of everything
            TopMost = top; // Set it back to whatever it was
        }

        private void form_Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Minimize to try on clicking "X"
            if (e.CloseReason == CloseReason.UserClosing && !AppCanClose)
            {
                e.Cancel = true;
                con_Button_Quit_Click(sender, e);
            }
        }

        private void trayIconMain_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            // Maximize from tray
            form_Main_BringFromTray(); // Bring app from system tray
        }

        private void form_Main_Resize(object sender, EventArgs e)
        {
            // Form was minimised to try
            if (WindowState == FormWindowState.Minimized)
            {
                if (con_check_minimize_to_tray.Checked) // Check if user wants to minimize to tray icon
                {
                    form_Main_SendToTray();
                }
            }
        }
        // ################################ Timers ################################
        private void Tim_GUI_Tick(object sender, EventArgs e)
        {
            // Main timer to sync GUI with background tasks and flush buffers

            // Refresh Log Window
            if (Globals.AppUpdateGUI)
            {
                con_List_Received.Items.Clear();
                foreach (string i in Globals.AppLogHistory)
                {
                    if (i != null)
                    {
                        con_List_Received.Items.Add(i);
                    }
                }
                Globals.AppUpdateGUI = false;
            }
            else
            {
                // Do nothing , control does not require update
            }

            // Enable see log button
            if(Globals.LastLogLocation != "")
            {
                con_Button_See_log.Enabled = true;
            }

            // Update mission status
            if (Globals.CurrentMission.Mission != "")
            {
                
                label17.Text = Globals.CurrentMission.Mission;
                label18.Text = Globals.CurrentMission.Theatre;
                label19.Text = Globals.CurrentMission.PlayerCount.ToString();

                label21.Text = PerunHelper.SecondsToString(Globals.CurrentMission.ModelTime);
                label16.Text = PerunHelper.SecondsToString(Globals.CurrentMission.RealTime);

                if (Globals.CurrentMission.Pause == "True")
                {
                    label21.Text = $"{label21.Text} (paused)";
                }
            }
            else
            {
                this.form_Main_SetBlankMissionInfo();
            }

            // Update status icons at main form - MySQL
            if ((Globals.DatabaseConnection.DatabaseStatus != Globals.StatusDatabase) || Globals.AppForceIconReload)
            {
                if (Globals.DatabaseConnection.DatabaseStatus)
                {
                    if (Globals.ErrorsDatabase == 0)
                    {
                        con_img_db.Image = (Image)Properties.Resources.ResourceManager.GetObject("status_connected");
                    }
                    else
                    {
                        con_img_db.Image = (Image)Properties.Resources.ResourceManager.GetObject("status_connected_error");
                    }
                }
                else
                {
                    con_img_db.Image = (Image)Properties.Resources.ResourceManager.GetObject("status_disconnected_error");
                }
                Globals.StatusDatabase = Globals.DatabaseConnection.DatabaseStatus;
            }
            // Update status icons at main form - DCS
            if ((Globals.StatusConnection != Globals.StatusHistoryConnection) || Globals.AppForceIconReload || Globals.ErrorsGame != Globals.ErrorsHistoryGame)
            {
                if (Globals.StatusConnection)
                {
                    if (Globals.ErrorsGame == 0)
                    {
                        con_img_dcs.Image = (Image)Properties.Resources.ResourceManager.GetObject("status_connected");
                    }
                    else
                    {
                        con_img_dcs.Image = (Image)Properties.Resources.ResourceManager.GetObject("status_connected_error");
                    }
                }
                else
                {
                    con_img_dcs.Image = (Image)Properties.Resources.ResourceManager.GetObject("status_disconnected_error");
                }
                Globals.StatusHistoryConnection = Globals.StatusConnection;
                Globals.ErrorsHistoryGame = Globals.ErrorsGame;
            }

            // Frame times
            label31.Text = $"{Globals.LastFrameTime.ToString("0.")}u";
            label33.Text = $"{Globals.LastFrameDelay.ToString("0.")}u";

            // Check if we shall rate logs
            Globals.RotateLogs = con_check_minimize_to_tray.Checked;

            // Update status icons at main form - SRS
            if ((ExtSRSStatus != Globals.StatusSRS) || Globals.AppForceIconReload)
            {
                if (ExtSRSStatus && con_check_3rd_srs.Checked)
                {
                    if (Globals.ErrorsSRS == 0)
                    {
                        con_img_srs.Image = (Image)Properties.Resources.ResourceManager.GetObject("status_connected");
                    }
                    else
                    {
                        con_img_srs.Image = (Image)Properties.Resources.ResourceManager.GetObject("status_connected_error");
                    }
                }
                else
                {
                    con_img_srs.Image = (Image)Properties.Resources.ResourceManager.GetObject("status_disconnected_error");
                }
                Globals.StatusSRS = ExtSRSStatus;
            }
            // Update status icons at main form - LotATC
            if ((ExtLotATCStatus != Globals.StatusLotATC) || Globals.AppForceIconReload)
            {
                if (ExtLotATCStatus && con_check_3rd_lotatc.Checked)
                {
                    if (Globals.ErrorsLotATC == 0)
                    {
                        con_img_lotATC.Image = (Image)Properties.Resources.ResourceManager.GetObject("status_connected");
                    }
                    else
                    {
                        con_img_lotATC.Image = (Image)Properties.Resources.ResourceManager.GetObject("status_connected_error");
                    }
                }
                else
                {
                    con_img_lotATC.Image = (Image)Properties.Resources.ResourceManager.GetObject("status_disconnected_error");
                }
                Globals.StatusLotATC = ExtLotATCStatus;
            }
            Globals.AppForceIconReload = false;
        }

        private void Tim_MySQL_Tick(object sender, EventArgs e)
        {
            // Send buffer to MySQL
            for (int i = 0; i < Globals.DatabaseSendBuffer.Length - 1; i++)
            {
                if (Globals.DatabaseSendBuffer[i] != null)
                {
                    if (Globals.DatabaseConnection.SendToMySql(Globals.DatabaseSendBuffer[i]) > 0)
                    {
                        Globals.DatabaseSendBuffer[i] = null; // If packet was send then delete it from send buffer
                    }
                }
            }
        }

        private void tim_3rdparties_Tick(object sender, EventArgs e)
        {
            // Main timer to check MySQL connection and send JSON files to MySQL

            // Send ping to check for possible connection issues
            Globals.DatabaseConnection.SendToMySql("", true);

            if (PerunHelper.CheckVersions() == 0)
            {
                MessageBox.Show("Version mismatch detected - please check log files and update.\n\nPerun will now terminate.", "Perun ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                con_Button_Listen_OFF_Click(null, null);
            }

            // Take care of 3rd party stuff
            string ExtSRSJson = "";
            string ExtLotATCJson = "";
            string ClientName = "";

            bool ExtSRSUseDefault = true;
            bool ExtLotATCUseDefault = true;

            // Handle SRS 
            if (Globals.StatusConnection)
            {
                if (con_check_3rd_srs.Checked)
                {
                    try
                    {
                        ExtSRSJson = System.IO.File.ReadAllText(con_txt_3rd_srs.Text);
                        dynamic raw_dcssrs = JsonConvert.DeserializeObject(ExtSRSJson);

                        for (int i = 0; i < raw_dcssrs.Clients.Count; i++)
                        {

                            if (raw_dcssrs.Clients[i].RadioInfo != null)
                            {

                                int temp = raw_dcssrs.Clients[i].RadioInfo.radios.Count - 1;
                                for (int j = temp; j >= 0; j--)
                                {
                                    if (raw_dcssrs.Clients[i].RadioInfo.radios[j].name == "No Radio")
                                    {
                                        raw_dcssrs.Clients[i].RadioInfo.radios[j].Remove();
                                    }
                                }
                            }
                            if (raw_dcssrs.Clients[i].Name != null)
                            {
                                // Clean the user names from non alphanumeric characters
                                ClientName = raw_dcssrs.Clients[i].Name;
                                Regex rgx = new Regex("[^a-zA-Z0-9 -]");
                                ClientName = rgx.Replace(ClientName, "");

                                raw_dcssrs.Clients[i].Name = ClientName;
                            }
                        }

                        ExtSRSJson = JsonConvert.SerializeObject(raw_dcssrs);

                        ExtSRSJson = $"{{'type':'100','instance':'{Int32.Parse(con_txt_dcs_instance.Text)}','payload':{ExtSRSJson}}}";

                        ExtSRSUseDefault = false;
                        PerunHelper.LogInfo(ref Globals.AppLogHistory, "SRS data loaded", 3, 0, "100", true);
                        ExtSRSStatus = true;
                    }
                    catch (Exception exc_srs)
                    {
                        PerunHelper.LogError(ref Globals.AppLogHistory, $"ERROR SRS Data, error: {exc_srs.Message}", 3, 1, "100");
                        ExtSRSStatus = false;
                        Globals.ErrorsSRS++;
                    }


                }
                if (ExtSRSUseDefault)
                {
                    ExtSRSJson = $"{{'type':'100','instance':'{Int32.Parse(con_txt_dcs_instance.Text)}','payload':{{'ignore':'true'}}";
                }
                if (ExtSRSStatus)
                {
                    Globals.DatabaseConnection.SendToMySql(ExtSRSJson);
                }
            }

            // Handle LotATC 
            if (Globals.StatusConnection)
            {
                if (con_check_3rd_lotatc.Checked)
                {
                    try
                    {
                        ExtLotATCJson = System.IO.File.ReadAllText(con_txt_3rd_lotatc.Text);

                        ExtLotATCJson = $"{{'type':'101','instance':'{Int32.Parse(con_txt_dcs_instance.Text)}','payload':{ExtLotATCJson}}}";
                        ExtLotATCUseDefault = false;
                        PerunHelper.LogInfo(ref Globals.AppLogHistory, "LotATC data loaded", 3, 0, "101", true);
                        ExtLotATCStatus = true;
                    }
                    catch (Exception exc_lotatc)
                    {
                        PerunHelper.LogError(ref Globals.AppLogHistory, $"ERROR LotATC Data, error: {exc_lotatc.Message}", 3, 1, "101");
                        ExtLotATCStatus = false;
                        Globals.ErrorsLotATC++;
                    }


                }
                if (ExtLotATCUseDefault)
                {
                    ExtLotATCJson = $"{{'type':'101','instance':'{Int32.Parse(con_txt_dcs_instance.Text)}','payload':{{'ignore':'true'}}}}";   // No LotATC controller connected
                }
                if (ExtLotATCStatus)
                {
                    Globals.DatabaseConnection.SendToMySql(ExtLotATCJson);
                }
            }

            // Let's do not risk int overload
            Globals.ErrorsDatabase = (Globals.ErrorsDatabase > 999) ? 999 : Globals.ErrorsDatabase;
            Globals.ErrorsGame = (Globals.ErrorsGame > 999) ? 999 : Globals.ErrorsGame;
            Globals.ErrorsSRS = (Globals.ErrorsSRS > 999) ? 999 : Globals.ErrorsSRS;
            Globals.ErrorsLotATC = (Globals.ErrorsLotATC > 999) ? 999 : Globals.ErrorsLotATC;

        }

        private void tim_HW_status_Tick(object sender, EventArgs e)
        {
            //Handle getting of system status
            label28.Text = $"{Globals.HardwareMonitor.getCurrentCpuUsage()}%";
            label29.Text = $"{Globals.HardwareMonitor.getCurrentRamUsage()}%";
        }
       
        private void TIM_Autostart_Tick(object sender, EventArgs e)
        {
            // Handle autostart parameter from command line
            TIM_Autostart.Enabled = false; // Disable timer
            con_Button_Listen_ON_Click(sender, e); // Simulate button click
        }

        private void con_Button_See_log_Click(object sender, EventArgs e)
        {
            if (Globals.LastLogLocation != "")
            {
                Process.Start(Globals.LastLogLocation);
            }
        }

    }
}
