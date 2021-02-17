namespace Perun_v1
{
    partial class form_Main
    {
        /// <summary>
        /// Wymagana zmienna projektanta.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Wyczyść wszystkie używane zasoby.
        /// </summary>
        /// <param name="disposing">prawda, jeżeli zarządzane zasoby powinny zostać zlikwidowane; Fałsz w przeciwnym wypadku.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Kod generowany przez Projektanta formularzy systemu Windows

        /// <summary>
        /// Metoda wymagana do obsługi projektanta — nie należy modyfikować
        /// jej zawartości w edytorze kodu.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(form_Main));
            this.con_List_Received = new System.Windows.Forms.ListBox();
            this.con_Button_Listen_ON = new System.Windows.Forms.Button();
            this.con_Button_Listen_OFF = new System.Windows.Forms.Button();
            this.con_GroupBox_1 = new System.Windows.Forms.GroupBox();
            this.label15 = new System.Windows.Forms.Label();
            this.con_com_loglevel = new System.Windows.Forms.ComboBox();
            this.con_Button_Add_Marker = new System.Windows.Forms.Button();
            this.tim_GUI = new System.Windows.Forms.Timer(this.components);
            this.con_GroupBox_2 = new System.Windows.Forms.GroupBox();
            this.label6 = new System.Windows.Forms.Label();
            this.con_txt_mysql_port = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.con_txt_mysql_username = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.con_txt_mysql_database = new System.Windows.Forms.TextBox();
            this.con_txt_mysql_server = new System.Windows.Forms.TextBox();
            this.con_txt_mysql_password = new System.Windows.Forms.MaskedTextBox();
            this.con_GroupBox_3 = new System.Windows.Forms.GroupBox();
            this.con_txt_3rd_lotatc = new System.Windows.Forms.MaskedTextBox();
            this.con_txt_3rd_srs = new System.Windows.Forms.MaskedTextBox();
            this.con_check_3rd_lotatc = new System.Windows.Forms.CheckBox();
            this.con_check_3rd_srs = new System.Windows.Forms.CheckBox();
            this.con_Button_Quit = new System.Windows.Forms.Button();
            this.con_lab_github = new System.Windows.Forms.LinkLabel();
            this.label5 = new System.Windows.Forms.Label();
            this.trayIconMain = new System.Windows.Forms.NotifyIcon(this.components);
            this.openFileDialog_SRS = new System.Windows.Forms.OpenFileDialog();
            this.openFileDialog_LotATC = new System.Windows.Forms.OpenFileDialog();
            this.tim_3rdparties = new System.Windows.Forms.Timer(this.components);
            this.tim_MySQL = new System.Windows.Forms.Timer(this.components);
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label8 = new System.Windows.Forms.Label();
            this.con_txt_dcs_instance = new System.Windows.Forms.MaskedTextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.con_txt_dcs_server_port = new System.Windows.Forms.MaskedTextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.con_img_logo = new System.Windows.Forms.PictureBox();
            this.TIM_Autostart = new System.Windows.Forms.Timer(this.components);
            this.con_check_minimize_to_tray = new System.Windows.Forms.CheckBox();
            this.con_GroupBox_4 = new System.Windows.Forms.GroupBox();
            this.con_Button_Reset_Flags = new System.Windows.Forms.Button();
            this.label12 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.con_img_lotATC = new System.Windows.Forms.PictureBox();
            this.con_img_srs = new System.Windows.Forms.PictureBox();
            this.con_img_dcs = new System.Windows.Forms.PictureBox();
            this.con_img_db = new System.Windows.Forms.PictureBox();
            this.con_GroupBox_5 = new System.Windows.Forms.GroupBox();
            this.con_GroupBox_6 = new System.Windows.Forms.GroupBox();
            this.label26 = new System.Windows.Forms.Label();
            this.label25 = new System.Windows.Forms.Label();
            this.label24 = new System.Windows.Forms.Label();
            this.label23 = new System.Windows.Forms.Label();
            this.label22 = new System.Windows.Forms.Label();
            this.label21 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.label19 = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.con_GroupBox_7 = new System.Windows.Forms.GroupBox();
            this.label29 = new System.Windows.Forms.Label();
            this.label28 = new System.Windows.Forms.Label();
            this.label27 = new System.Windows.Forms.Label();
            this.label20 = new System.Windows.Forms.Label();
            this.tim_HW_status = new System.Windows.Forms.Timer(this.components);
            this.con_Button_See_log = new System.Windows.Forms.Button();
            this.con_GroupBox_1.SuspendLayout();
            this.con_GroupBox_2.SuspendLayout();
            this.con_GroupBox_3.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.con_img_logo)).BeginInit();
            this.con_GroupBox_4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.con_img_lotATC)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.con_img_srs)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.con_img_dcs)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.con_img_db)).BeginInit();
            this.con_GroupBox_5.SuspendLayout();
            this.con_GroupBox_6.SuspendLayout();
            this.con_GroupBox_7.SuspendLayout();
            this.SuspendLayout();
            // 
            // con_List_Received
            // 
            this.con_List_Received.Enabled = false;
            this.con_List_Received.FormattingEnabled = true;
            this.con_List_Received.Items.AddRange(new object[] {
            "Not connected"});
            this.con_List_Received.Location = new System.Drawing.Point(8, 44);
            this.con_List_Received.Name = "con_List_Received";
            this.con_List_Received.Size = new System.Drawing.Size(307, 134);
            this.con_List_Received.TabIndex = 0;
            // 
            // con_Button_Listen_ON
            // 
            this.con_Button_Listen_ON.Location = new System.Drawing.Point(342, 441);
            this.con_Button_Listen_ON.Name = "con_Button_Listen_ON";
            this.con_Button_Listen_ON.Size = new System.Drawing.Size(86, 39);
            this.con_Button_Listen_ON.TabIndex = 2;
            this.con_Button_Listen_ON.Text = "Start";
            this.con_Button_Listen_ON.UseVisualStyleBackColor = true;
            this.con_Button_Listen_ON.Click += new System.EventHandler(this.con_Button_Listen_ON_Click);
            // 
            // con_Button_Listen_OFF
            // 
            this.con_Button_Listen_OFF.Enabled = false;
            this.con_Button_Listen_OFF.Location = new System.Drawing.Point(434, 441);
            this.con_Button_Listen_OFF.Name = "con_Button_Listen_OFF";
            this.con_Button_Listen_OFF.Size = new System.Drawing.Size(86, 39);
            this.con_Button_Listen_OFF.TabIndex = 3;
            this.con_Button_Listen_OFF.Text = "Stop";
            this.con_Button_Listen_OFF.UseVisualStyleBackColor = true;
            this.con_Button_Listen_OFF.Click += new System.EventHandler(this.con_Button_Listen_OFF_Click);
            // 
            // con_GroupBox_1
            // 
            this.con_GroupBox_1.Controls.Add(this.con_Button_See_log);
            this.con_GroupBox_1.Controls.Add(this.label15);
            this.con_GroupBox_1.Controls.Add(this.con_com_loglevel);
            this.con_GroupBox_1.Controls.Add(this.con_List_Received);
            this.con_GroupBox_1.Controls.Add(this.con_Button_Add_Marker);
            this.con_GroupBox_1.Location = new System.Drawing.Point(342, 248);
            this.con_GroupBox_1.Name = "con_GroupBox_1";
            this.con_GroupBox_1.Size = new System.Drawing.Size(324, 187);
            this.con_GroupBox_1.TabIndex = 4;
            this.con_GroupBox_1.TabStop = false;
            this.con_GroupBox_1.Text = "Data log";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.label15.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label15.Location = new System.Drawing.Point(6, 18);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(50, 13);
            this.label15.TabIndex = 22;
            this.label15.Text = "Log level";
            // 
            // con_com_loglevel
            // 
            this.con_com_loglevel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.con_com_loglevel.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F);
            this.con_com_loglevel.FormattingEnabled = true;
            this.con_com_loglevel.Items.AddRange(new object[] {
            "Error",
            "Warning",
            "Info",
            "Debug"});
            this.con_com_loglevel.Location = new System.Drawing.Point(62, 12);
            this.con_com_loglevel.Name = "con_com_loglevel";
            this.con_com_loglevel.Size = new System.Drawing.Size(102, 20);
            this.con_com_loglevel.TabIndex = 23;
            this.con_com_loglevel.SelectedIndexChanged += new System.EventHandler(this.con_com_loglevell_SelectedIndexChanged);
            // 
            // con_Button_Add_Marker
            // 
            this.con_Button_Add_Marker.Enabled = false;
            this.con_Button_Add_Marker.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.con_Button_Add_Marker.Location = new System.Drawing.Point(232, 12);
            this.con_Button_Add_Marker.Name = "con_Button_Add_Marker";
            this.con_Button_Add_Marker.Size = new System.Drawing.Size(86, 20);
            this.con_Button_Add_Marker.TabIndex = 21;
            this.con_Button_Add_Marker.Text = "Add log marker";
            this.con_Button_Add_Marker.UseVisualStyleBackColor = true;
            this.con_Button_Add_Marker.Click += new System.EventHandler(this.con_Button_Add_Marker_Click);
            // 
            // tim_GUI
            // 
            this.tim_GUI.Interval = 200;
            this.tim_GUI.Tick += new System.EventHandler(this.Tim_GUI_Tick);
            // 
            // con_GroupBox_2
            // 
            this.con_GroupBox_2.Controls.Add(this.label6);
            this.con_GroupBox_2.Controls.Add(this.con_txt_mysql_port);
            this.con_GroupBox_2.Controls.Add(this.label4);
            this.con_GroupBox_2.Controls.Add(this.con_txt_mysql_username);
            this.con_GroupBox_2.Controls.Add(this.label3);
            this.con_GroupBox_2.Controls.Add(this.label2);
            this.con_GroupBox_2.Controls.Add(this.label1);
            this.con_GroupBox_2.Controls.Add(this.con_txt_mysql_database);
            this.con_GroupBox_2.Controls.Add(this.con_txt_mysql_server);
            this.con_GroupBox_2.Controls.Add(this.con_txt_mysql_password);
            this.con_GroupBox_2.Location = new System.Drawing.Point(12, 12);
            this.con_GroupBox_2.Name = "con_GroupBox_2";
            this.con_GroupBox_2.Size = new System.Drawing.Size(324, 154);
            this.con_GroupBox_2.TabIndex = 5;
            this.con_GroupBox_2.TabStop = false;
            this.con_GroupBox_2.Text = "Database settings";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(19, 48);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(81, 13);
            this.label6.TabIndex = 9;
            this.label6.Text = "MySQL DB port";
            // 
            // con_txt_mysql_port
            // 
            this.con_txt_mysql_port.Location = new System.Drawing.Point(106, 45);
            this.con_txt_mysql_port.Name = "con_txt_mysql_port";
            this.con_txt_mysql_port.Size = new System.Drawing.Size(207, 20);
            this.con_txt_mysql_port.TabIndex = 8;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(45, 100);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(55, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "Username";
            // 
            // con_txt_mysql_username
            // 
            this.con_txt_mysql_username.Location = new System.Drawing.Point(106, 97);
            this.con_txt_mysql_username.Name = "con_txt_mysql_username";
            this.con_txt_mysql_username.Size = new System.Drawing.Size(207, 20);
            this.con_txt_mysql_username.TabIndex = 6;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(47, 126);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Password";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(18, 74);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(82, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Database name";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(100, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "MySQL DB address";
            // 
            // con_txt_mysql_database
            // 
            this.con_txt_mysql_database.Location = new System.Drawing.Point(106, 71);
            this.con_txt_mysql_database.Name = "con_txt_mysql_database";
            this.con_txt_mysql_database.Size = new System.Drawing.Size(207, 20);
            this.con_txt_mysql_database.TabIndex = 2;
            // 
            // con_txt_mysql_server
            // 
            this.con_txt_mysql_server.Location = new System.Drawing.Point(106, 19);
            this.con_txt_mysql_server.Name = "con_txt_mysql_server";
            this.con_txt_mysql_server.Size = new System.Drawing.Size(207, 20);
            this.con_txt_mysql_server.TabIndex = 1;
            // 
            // con_txt_mysql_password
            // 
            this.con_txt_mysql_password.Location = new System.Drawing.Point(106, 123);
            this.con_txt_mysql_password.Name = "con_txt_mysql_password";
            this.con_txt_mysql_password.PasswordChar = '*';
            this.con_txt_mysql_password.Size = new System.Drawing.Size(207, 20);
            this.con_txt_mysql_password.TabIndex = 0;
            // 
            // con_GroupBox_3
            // 
            this.con_GroupBox_3.Controls.Add(this.con_txt_3rd_lotatc);
            this.con_GroupBox_3.Controls.Add(this.con_txt_3rd_srs);
            this.con_GroupBox_3.Controls.Add(this.con_check_3rd_lotatc);
            this.con_GroupBox_3.Controls.Add(this.con_check_3rd_srs);
            this.con_GroupBox_3.Location = new System.Drawing.Point(12, 246);
            this.con_GroupBox_3.Name = "con_GroupBox_3";
            this.con_GroupBox_3.Size = new System.Drawing.Size(324, 118);
            this.con_GroupBox_3.TabIndex = 6;
            this.con_GroupBox_3.TabStop = false;
            this.con_GroupBox_3.Text = "Integration settings";
            // 
            // con_txt_3rd_lotatc
            // 
            this.con_txt_3rd_lotatc.Location = new System.Drawing.Point(6, 91);
            this.con_txt_3rd_lotatc.Name = "con_txt_3rd_lotatc";
            this.con_txt_3rd_lotatc.Size = new System.Drawing.Size(307, 20);
            this.con_txt_3rd_lotatc.TabIndex = 4;
            this.con_txt_3rd_lotatc.MouseClick += new System.Windows.Forms.MouseEventHandler(this.con_txt_3rd_lotatc_Click);
            // 
            // con_txt_3rd_srs
            // 
            this.con_txt_3rd_srs.Location = new System.Drawing.Point(6, 42);
            this.con_txt_3rd_srs.Name = "con_txt_3rd_srs";
            this.con_txt_3rd_srs.Size = new System.Drawing.Size(307, 20);
            this.con_txt_3rd_srs.TabIndex = 3;
            this.con_txt_3rd_srs.MouseClick += new System.Windows.Forms.MouseEventHandler(this.con_txt_3rd_srs_Click);
            // 
            // con_check_3rd_lotatc
            // 
            this.con_check_3rd_lotatc.AutoSize = true;
            this.con_check_3rd_lotatc.Location = new System.Drawing.Point(6, 72);
            this.con_check_3rd_lotatc.Name = "con_check_3rd_lotatc";
            this.con_check_3rd_lotatc.Size = new System.Drawing.Size(100, 17);
            this.con_check_3rd_lotatc.TabIndex = 1;
            this.con_check_3rd_lotatc.Text = "LotATC json file";
            this.con_check_3rd_lotatc.UseVisualStyleBackColor = true;
            // 
            // con_check_3rd_srs
            // 
            this.con_check_3rd_srs.AutoSize = true;
            this.con_check_3rd_srs.Location = new System.Drawing.Point(6, 19);
            this.con_check_3rd_srs.Name = "con_check_3rd_srs";
            this.con_check_3rd_srs.Size = new System.Drawing.Size(111, 17);
            this.con_check_3rd_srs.TabIndex = 0;
            this.con_check_3rd_srs.Text = "DCS SRS json file";
            this.con_check_3rd_srs.UseVisualStyleBackColor = true;
            // 
            // con_Button_Quit
            // 
            this.con_Button_Quit.Location = new System.Drawing.Point(580, 441);
            this.con_Button_Quit.Name = "con_Button_Quit";
            this.con_Button_Quit.Size = new System.Drawing.Size(86, 39);
            this.con_Button_Quit.TabIndex = 7;
            this.con_Button_Quit.Text = "Quit";
            this.con_Button_Quit.UseVisualStyleBackColor = true;
            this.con_Button_Quit.Click += new System.EventHandler(this.con_Button_Quit_Click);
            // 
            // con_lab_github
            // 
            this.con_lab_github.AutoSize = true;
            this.con_lab_github.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.con_lab_github.Location = new System.Drawing.Point(172, 456);
            this.con_lab_github.Name = "con_lab_github";
            this.con_lab_github.Size = new System.Drawing.Size(155, 12);
            this.con_lab_github.TabIndex = 8;
            this.con_lab_github.TabStop = true;
            this.con_lab_github.Text = "https://github.com/szporwolik/perun";
            this.con_lab_github.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.con_lab_github_LinkClicked);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(223, 444);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(102, 12);
            this.label5.TabIndex = 9;
            this.label5.Text = "Manual and bugtracker:";
            // 
            // trayIconMain
            // 
            this.trayIconMain.BalloonTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            this.trayIconMain.BalloonTipText = "Perun";
            this.trayIconMain.BalloonTipTitle = "Perun for DCS World";
            this.trayIconMain.Icon = ((System.Drawing.Icon)(resources.GetObject("trayIconMain.Icon")));
            this.trayIconMain.Text = "Perun for DCS World";
            this.trayIconMain.MouseClick += new System.Windows.Forms.MouseEventHandler(this.trayIconMain_MouseDoubleClick);
            this.trayIconMain.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.trayIconMain_MouseDoubleClick);
            // 
            // openFileDialog_SRS
            // 
            this.openFileDialog_SRS.FileName = "openFileDialog1";
            // 
            // openFileDialog_LotATC
            // 
            this.openFileDialog_LotATC.FileName = "openFileDialog1";
            // 
            // tim_3rdparties
            // 
            this.tim_3rdparties.Interval = 30000;
            this.tim_3rdparties.Tick += new System.EventHandler(this.tim_3rdparties_Tick);
            // 
            // tim_MySQL
            // 
            this.tim_MySQL.Tick += new System.EventHandler(this.Tim_MySQL_Tick);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.con_txt_dcs_instance);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.con_txt_dcs_server_port);
            this.groupBox1.Location = new System.Drawing.Point(12, 172);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(324, 68);
            this.groupBox1.TabIndex = 7;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "DCS World settings";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(7, 45);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(93, 13);
            this.label8.TabIndex = 12;
            this.label8.Text = "Perun Instance ID";
            // 
            // con_txt_dcs_instance
            // 
            this.con_txt_dcs_instance.Location = new System.Drawing.Point(106, 42);
            this.con_txt_dcs_instance.Name = "con_txt_dcs_instance";
            this.con_txt_dcs_instance.Size = new System.Drawing.Size(207, 20);
            this.con_txt_dcs_instance.TabIndex = 11;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(44, 19);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(56, 13);
            this.label7.TabIndex = 10;
            this.label7.Text = "Perun port";
            // 
            // con_txt_dcs_server_port
            // 
            this.con_txt_dcs_server_port.Location = new System.Drawing.Point(106, 16);
            this.con_txt_dcs_server_port.Name = "con_txt_dcs_server_port";
            this.con_txt_dcs_server_port.Size = new System.Drawing.Size(207, 20);
            this.con_txt_dcs_server_port.TabIndex = 3;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label13.Location = new System.Drawing.Point(64, 427);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(74, 25);
            this.label13.TabIndex = 19;
            this.label13.Text = "Perun";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label14.Location = new System.Drawing.Point(66, 452);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(88, 13);
            this.label14.TabIndex = 20;
            this.label14.Text = "for DCS World";
            // 
            // con_img_logo
            // 
            this.con_img_logo.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.con_img_logo.Image = global::Perun_v1.Properties.Resources.perun_logo;
            this.con_img_logo.Location = new System.Drawing.Point(13, 427);
            this.con_img_logo.Name = "con_img_logo";
            this.con_img_logo.Size = new System.Drawing.Size(45, 41);
            this.con_img_logo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.con_img_logo.TabIndex = 18;
            this.con_img_logo.TabStop = false;
            // 
            // TIM_Autostart
            // 
            this.TIM_Autostart.Interval = 500;
            this.TIM_Autostart.Tick += new System.EventHandler(this.TIM_Autostart_Tick);
            // 
            // con_check_minimize_to_tray
            // 
            this.con_check_minimize_to_tray.AutoSize = true;
            this.con_check_minimize_to_tray.Checked = true;
            this.con_check_minimize_to_tray.CheckState = System.Windows.Forms.CheckState.Checked;
            this.con_check_minimize_to_tray.Location = new System.Drawing.Point(6, 19);
            this.con_check_minimize_to_tray.Name = "con_check_minimize_to_tray";
            this.con_check_minimize_to_tray.Size = new System.Drawing.Size(98, 17);
            this.con_check_minimize_to_tray.TabIndex = 24;
            this.con_check_minimize_to_tray.Text = "Minimize to tray";
            this.con_check_minimize_to_tray.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.con_check_minimize_to_tray.UseVisualStyleBackColor = true;
            this.con_check_minimize_to_tray.Validated += new System.EventHandler(this.con_check_minimize_to_tray_Validated);
            // 
            // con_GroupBox_4
            // 
            this.con_GroupBox_4.Controls.Add(this.con_Button_Reset_Flags);
            this.con_GroupBox_4.Controls.Add(this.label12);
            this.con_GroupBox_4.Controls.Add(this.label11);
            this.con_GroupBox_4.Controls.Add(this.label10);
            this.con_GroupBox_4.Controls.Add(this.label9);
            this.con_GroupBox_4.Controls.Add(this.con_img_lotATC);
            this.con_GroupBox_4.Controls.Add(this.con_img_srs);
            this.con_GroupBox_4.Controls.Add(this.con_img_dcs);
            this.con_GroupBox_4.Controls.Add(this.con_img_db);
            this.con_GroupBox_4.Location = new System.Drawing.Point(342, 12);
            this.con_GroupBox_4.Name = "con_GroupBox_4";
            this.con_GroupBox_4.Size = new System.Drawing.Size(324, 87);
            this.con_GroupBox_4.TabIndex = 25;
            this.con_GroupBox_4.TabStop = false;
            this.con_GroupBox_4.Text = "Perun status";
            // 
            // con_Button_Reset_Flags
            // 
            this.con_Button_Reset_Flags.Enabled = false;
            this.con_Button_Reset_Flags.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.con_Button_Reset_Flags.Location = new System.Drawing.Point(254, 25);
            this.con_Button_Reset_Flags.Name = "con_Button_Reset_Flags";
            this.con_Button_Reset_Flags.Size = new System.Drawing.Size(61, 40);
            this.con_Button_Reset_Flags.TabIndex = 26;
            this.con_Button_Reset_Flags.Text = "Reset error flags";
            this.con_Button_Reset_Flags.UseVisualStyleBackColor = true;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.label12.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label12.Location = new System.Drawing.Point(187, 59);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(49, 13);
            this.label12.TabIndex = 25;
            this.label12.Text = "LotATC";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.label11.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label11.Location = new System.Drawing.Point(138, 59);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(32, 13);
            this.label11.TabIndex = 24;
            this.label11.Text = "SRS";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.label10.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.Location = new System.Drawing.Point(77, 59);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(39, 13);
            this.label10.TabIndex = 23;
            this.label10.Text = "Game";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.Location = new System.Drawing.Point(9, 59);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(61, 13);
            this.label9.TabIndex = 18;
            this.label9.Text = "Database";
            // 
            // con_img_lotATC
            // 
            this.con_img_lotATC.Location = new System.Drawing.Point(196, 28);
            this.con_img_lotATC.Name = "con_img_lotATC";
            this.con_img_lotATC.Size = new System.Drawing.Size(29, 28);
            this.con_img_lotATC.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.con_img_lotATC.TabIndex = 22;
            this.con_img_lotATC.TabStop = false;
            // 
            // con_img_srs
            // 
            this.con_img_srs.Location = new System.Drawing.Point(139, 28);
            this.con_img_srs.Name = "con_img_srs";
            this.con_img_srs.Size = new System.Drawing.Size(29, 28);
            this.con_img_srs.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.con_img_srs.TabIndex = 21;
            this.con_img_srs.TabStop = false;
            // 
            // con_img_dcs
            // 
            this.con_img_dcs.Location = new System.Drawing.Point(82, 28);
            this.con_img_dcs.Name = "con_img_dcs";
            this.con_img_dcs.Size = new System.Drawing.Size(29, 28);
            this.con_img_dcs.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.con_img_dcs.TabIndex = 20;
            this.con_img_dcs.TabStop = false;
            // 
            // con_img_db
            // 
            this.con_img_db.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.con_img_db.Location = new System.Drawing.Point(25, 28);
            this.con_img_db.Name = "con_img_db";
            this.con_img_db.Size = new System.Drawing.Size(29, 28);
            this.con_img_db.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.con_img_db.TabIndex = 19;
            this.con_img_db.TabStop = false;
            // 
            // con_GroupBox_5
            // 
            this.con_GroupBox_5.Controls.Add(this.con_check_minimize_to_tray);
            this.con_GroupBox_5.Location = new System.Drawing.Point(13, 370);
            this.con_GroupBox_5.Name = "con_GroupBox_5";
            this.con_GroupBox_5.Size = new System.Drawing.Size(323, 46);
            this.con_GroupBox_5.TabIndex = 26;
            this.con_GroupBox_5.TabStop = false;
            this.con_GroupBox_5.Text = "Other settings";
            // 
            // con_GroupBox_6
            // 
            this.con_GroupBox_6.Controls.Add(this.label26);
            this.con_GroupBox_6.Controls.Add(this.label25);
            this.con_GroupBox_6.Controls.Add(this.label24);
            this.con_GroupBox_6.Controls.Add(this.label23);
            this.con_GroupBox_6.Controls.Add(this.label22);
            this.con_GroupBox_6.Controls.Add(this.label21);
            this.con_GroupBox_6.Controls.Add(this.label16);
            this.con_GroupBox_6.Controls.Add(this.label19);
            this.con_GroupBox_6.Controls.Add(this.label18);
            this.con_GroupBox_6.Controls.Add(this.label17);
            this.con_GroupBox_6.Location = new System.Drawing.Point(343, 105);
            this.con_GroupBox_6.Name = "con_GroupBox_6";
            this.con_GroupBox_6.Size = new System.Drawing.Size(323, 76);
            this.con_GroupBox_6.TabIndex = 28;
            this.con_GroupBox_6.TabStop = false;
            this.con_GroupBox_6.Text = "DCS World status";
            // 
            // label26
            // 
            this.label26.AutoSize = true;
            this.label26.Location = new System.Drawing.Point(240, 53);
            this.label26.Name = "label26";
            this.label26.Size = new System.Drawing.Size(77, 13);
            this.label26.TabIndex = 33;
            this.label26.Text = "player(s) online";
            this.label26.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label25
            // 
            this.label25.AutoSize = true;
            this.label25.Location = new System.Drawing.Point(6, 55);
            this.label25.Name = "label25";
            this.label25.Size = new System.Drawing.Size(79, 13);
            this.label25.TabIndex = 32;
            this.label25.Text = "Mission uptime:";
            this.label25.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label24
            // 
            this.label24.AutoSize = true;
            this.label24.Location = new System.Drawing.Point(10, 42);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(75, 13);
            this.label24.TabIndex = 31;
            this.label24.Text = "Server uptime:";
            this.label24.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label23
            // 
            this.label23.AutoSize = true;
            this.label23.Location = new System.Drawing.Point(54, 30);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(31, 13);
            this.label23.TabIndex = 30;
            this.label23.Text = "Map:";
            this.label23.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.Location = new System.Drawing.Point(40, 17);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(45, 13);
            this.label22.TabIndex = 29;
            this.label22.Text = "Mission:";
            this.label22.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Location = new System.Drawing.Point(82, 55);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(69, 13);
            this.label21.TabIndex = 28;
            this.label21.Text = "[mission time]";
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(82, 42);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(64, 13);
            this.label16.TabIndex = 27;
            this.label16.Text = "[server time]";
            this.label16.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.label19.Location = new System.Drawing.Point(267, 31);
            this.label19.Name = "label19";
            this.label19.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.label19.Size = new System.Drawing.Size(50, 24);
            this.label19.TabIndex = 2;
            this.label19.Text = "[000]";
            this.label19.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(82, 29);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(70, 13);
            this.label18.TabIndex = 1;
            this.label18.Text = "[mission map]";
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(82, 16);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(76, 13);
            this.label17.TabIndex = 0;
            this.label17.Text = "[mission name]";
            // 
            // con_GroupBox_7
            // 
            this.con_GroupBox_7.Controls.Add(this.label29);
            this.con_GroupBox_7.Controls.Add(this.label28);
            this.con_GroupBox_7.Controls.Add(this.label27);
            this.con_GroupBox_7.Controls.Add(this.label20);
            this.con_GroupBox_7.Location = new System.Drawing.Point(343, 187);
            this.con_GroupBox_7.Name = "con_GroupBox_7";
            this.con_GroupBox_7.Size = new System.Drawing.Size(323, 55);
            this.con_GroupBox_7.TabIndex = 29;
            this.con_GroupBox_7.TabStop = false;
            this.con_GroupBox_7.Text = "System status";
            // 
            // label29
            // 
            this.label29.AutoSize = true;
            this.label29.Location = new System.Drawing.Point(82, 30);
            this.label29.Name = "label29";
            this.label29.Size = new System.Drawing.Size(37, 13);
            this.label29.TabIndex = 34;
            this.label29.Text = "[RAM]";
            // 
            // label28
            // 
            this.label28.AutoSize = true;
            this.label28.Location = new System.Drawing.Point(82, 16);
            this.label28.Name = "label28";
            this.label28.Size = new System.Drawing.Size(35, 13);
            this.label28.TabIndex = 34;
            this.label28.Text = "[CPU]";
            // 
            // label27
            // 
            this.label27.AutoSize = true;
            this.label27.Location = new System.Drawing.Point(51, 30);
            this.label27.Name = "label27";
            this.label27.Size = new System.Drawing.Size(34, 13);
            this.label27.TabIndex = 34;
            this.label27.Text = "RAM:";
            this.label27.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Location = new System.Drawing.Point(53, 16);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(32, 13);
            this.label20.TabIndex = 34;
            this.label20.Text = "CPU:";
            this.label20.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // tim_HW_status
            // 
            this.tim_HW_status.Enabled = true;
            this.tim_HW_status.Interval = 1000;
            this.tim_HW_status.Tick += new System.EventHandler(this.tim_HW_status_Tick);
            // 
            // con_Button_See_log
            // 
            this.con_Button_See_log.Enabled = false;
            this.con_Button_See_log.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.con_Button_See_log.Location = new System.Drawing.Point(170, 12);
            this.con_Button_See_log.Name = "con_Button_See_log";
            this.con_Button_See_log.Size = new System.Drawing.Size(56, 20);
            this.con_Button_See_log.TabIndex = 24;
            this.con_Button_See_log.Text = "See log";
            this.con_Button_See_log.UseVisualStyleBackColor = true;
            this.con_Button_See_log.Click += new System.EventHandler(this.con_Button_See_log_Click);
            // 
            // form_Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(675, 485);
            this.Controls.Add(this.con_GroupBox_7);
            this.Controls.Add(this.con_GroupBox_6);
            this.Controls.Add(this.con_GroupBox_5);
            this.Controls.Add(this.con_GroupBox_4);
            this.Controls.Add(this.label14);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.con_img_logo);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.con_GroupBox_3);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.con_lab_github);
            this.Controls.Add(this.con_Button_Quit);
            this.Controls.Add(this.con_GroupBox_2);
            this.Controls.Add(this.con_GroupBox_1);
            this.Controls.Add(this.con_Button_Listen_OFF);
            this.Controls.Add(this.con_Button_Listen_ON);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "form_Main";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Perun for DCS World";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.form_Main_FormClosing);
            this.Load += new System.EventHandler(this.form_Main_Load);
            this.Resize += new System.EventHandler(this.form_Main_Resize);
            this.con_GroupBox_1.ResumeLayout(false);
            this.con_GroupBox_1.PerformLayout();
            this.con_GroupBox_2.ResumeLayout(false);
            this.con_GroupBox_2.PerformLayout();
            this.con_GroupBox_3.ResumeLayout(false);
            this.con_GroupBox_3.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.con_img_logo)).EndInit();
            this.con_GroupBox_4.ResumeLayout(false);
            this.con_GroupBox_4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.con_img_lotATC)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.con_img_srs)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.con_img_dcs)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.con_img_db)).EndInit();
            this.con_GroupBox_5.ResumeLayout(false);
            this.con_GroupBox_5.PerformLayout();
            this.con_GroupBox_6.ResumeLayout(false);
            this.con_GroupBox_6.PerformLayout();
            this.con_GroupBox_7.ResumeLayout(false);
            this.con_GroupBox_7.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox con_List_Received;
        private System.Windows.Forms.Button con_Button_Listen_ON;
        private System.Windows.Forms.Button con_Button_Listen_OFF;
        private System.Windows.Forms.GroupBox con_GroupBox_1;
        private System.Windows.Forms.Timer tim_GUI;
        private System.Windows.Forms.GroupBox con_GroupBox_2;
        private System.Windows.Forms.GroupBox con_GroupBox_3;
        private System.Windows.Forms.TextBox con_txt_mysql_database;
        private System.Windows.Forms.TextBox con_txt_mysql_server;
        private System.Windows.Forms.MaskedTextBox con_txt_mysql_password;
        private System.Windows.Forms.Button con_Button_Quit;
        private System.Windows.Forms.MaskedTextBox con_txt_3rd_lotatc;
        private System.Windows.Forms.MaskedTextBox con_txt_3rd_srs;
        private System.Windows.Forms.CheckBox con_check_3rd_lotatc;
        private System.Windows.Forms.CheckBox con_check_3rd_srs;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox con_txt_mysql_username;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.LinkLabel con_lab_github;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.NotifyIcon trayIconMain;
        private System.Windows.Forms.OpenFileDialog openFileDialog_SRS;
        private System.Windows.Forms.OpenFileDialog openFileDialog_LotATC;
        private System.Windows.Forms.Timer tim_3rdparties;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox con_txt_mysql_port;
        private System.Windows.Forms.Timer tim_MySQL;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.MaskedTextBox con_txt_dcs_server_port;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.MaskedTextBox con_txt_dcs_instance;
        private System.Windows.Forms.PictureBox con_img_logo;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Button con_Button_Add_Marker;
        private System.Windows.Forms.Timer TIM_Autostart;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.ComboBox con_com_loglevel;
        private System.Windows.Forms.CheckBox con_check_minimize_to_tray;
        private System.Windows.Forms.GroupBox con_GroupBox_4;
        private System.Windows.Forms.Button con_Button_Reset_Flags;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.PictureBox con_img_lotATC;
        private System.Windows.Forms.PictureBox con_img_srs;
        private System.Windows.Forms.PictureBox con_img_dcs;
        private System.Windows.Forms.PictureBox con_img_db;
        private System.Windows.Forms.GroupBox con_GroupBox_5;
        private System.Windows.Forms.GroupBox con_GroupBox_6;
        private System.Windows.Forms.Label label26;
        private System.Windows.Forms.Label label25;
        private System.Windows.Forms.Label label24;
        private System.Windows.Forms.Label label23;
        private System.Windows.Forms.Label label22;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.GroupBox con_GroupBox_7;
        private System.Windows.Forms.Label label29;
        private System.Windows.Forms.Label label28;
        private System.Windows.Forms.Label label27;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.Timer tim_HW_status;
        private System.Windows.Forms.Button con_Button_See_log;
    }
}

