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
            this.cboLogLevel = new System.Windows.Forms.ComboBox();
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
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.con_Button_Reset_Flags = new System.Windows.Forms.Button();
            this.label13 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.con_img_logo = new System.Windows.Forms.PictureBox();
            this.con_img_lotATC = new System.Windows.Forms.PictureBox();
            this.con_img_srs = new System.Windows.Forms.PictureBox();
            this.con_img_dcs = new System.Windows.Forms.PictureBox();
            this.con_img_db = new System.Windows.Forms.PictureBox();
            this.TIM_Autostart = new System.Windows.Forms.Timer(this.components);
            this.chkCloseToTray = new System.Windows.Forms.CheckBox();
            this.con_GroupBox_1.SuspendLayout();
            this.con_GroupBox_2.SuspendLayout();
            this.con_GroupBox_3.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.con_img_logo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.con_img_lotATC)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.con_img_srs)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.con_img_dcs)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.con_img_db)).BeginInit();
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
            this.con_Button_Listen_ON.Location = new System.Drawing.Point(12, 602);
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
            this.con_Button_Listen_OFF.Location = new System.Drawing.Point(104, 602);
            this.con_Button_Listen_OFF.Name = "con_Button_Listen_OFF";
            this.con_Button_Listen_OFF.Size = new System.Drawing.Size(86, 39);
            this.con_Button_Listen_OFF.TabIndex = 3;
            this.con_Button_Listen_OFF.Text = "Stop";
            this.con_Button_Listen_OFF.UseVisualStyleBackColor = true;
            this.con_Button_Listen_OFF.Click += new System.EventHandler(this.con_Button_Listen_OFF_Click);
            // 
            // con_GroupBox_1
            // 
            this.con_GroupBox_1.Controls.Add(this.label15);
            this.con_GroupBox_1.Controls.Add(this.cboLogLevel);
            this.con_GroupBox_1.Controls.Add(this.con_List_Received);
            this.con_GroupBox_1.Controls.Add(this.con_Button_Add_Marker);
            this.con_GroupBox_1.Location = new System.Drawing.Point(12, 412);
            this.con_GroupBox_1.Name = "con_GroupBox_1";
            this.con_GroupBox_1.Size = new System.Drawing.Size(324, 184);
            this.con_GroupBox_1.TabIndex = 4;
            this.con_GroupBox_1.TabStop = false;
            this.con_GroupBox_1.Text = "Data log";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.label15.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label15.Location = new System.Drawing.Point(9, 21);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(50, 13);
            this.label15.TabIndex = 22;
            this.label15.Text = "Log level";
            // 
            // cboLogLevel
            // 
            this.cboLogLevel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboLogLevel.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F);
            this.cboLogLevel.FormattingEnabled = true;
            this.cboLogLevel.Items.AddRange(new object[] {
            "Error",
            "Warning",
            "Info",
            "Debug"});
            this.cboLogLevel.Location = new System.Drawing.Point(65, 18);
            this.cboLogLevel.Name = "cboLogLevel";
            this.cboLogLevel.Size = new System.Drawing.Size(102, 20);
            this.cboLogLevel.TabIndex = 23;
            this.cboLogLevel.SelectedIndexChanged += new System.EventHandler(this.cboLogLevel_SelectedIndexChanged);
            // 
            // con_Button_Add_Marker
            // 
            this.con_Button_Add_Marker.Enabled = false;
            this.con_Button_Add_Marker.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.con_Button_Add_Marker.Location = new System.Drawing.Point(229, 17);
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
            this.con_GroupBox_2.Location = new System.Drawing.Point(14, 54);
            this.con_GroupBox_2.Name = "con_GroupBox_2";
            this.con_GroupBox_2.Size = new System.Drawing.Size(324, 154);
            this.con_GroupBox_2.TabIndex = 5;
            this.con_GroupBox_2.TabStop = false;
            this.con_GroupBox_2.Text = "MySQL";
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
            this.con_GroupBox_3.Location = new System.Drawing.Point(14, 288);
            this.con_GroupBox_3.Name = "con_GroupBox_3";
            this.con_GroupBox_3.Size = new System.Drawing.Size(324, 68);
            this.con_GroupBox_3.TabIndex = 6;
            this.con_GroupBox_3.TabStop = false;
            this.con_GroupBox_3.Text = "3rd party";
            // 
            // con_txt_3rd_lotatc
            // 
            this.con_txt_3rd_lotatc.Location = new System.Drawing.Point(123, 39);
            this.con_txt_3rd_lotatc.Name = "con_txt_3rd_lotatc";
            this.con_txt_3rd_lotatc.Size = new System.Drawing.Size(190, 20);
            this.con_txt_3rd_lotatc.TabIndex = 4;
            this.con_txt_3rd_lotatc.MouseClick += new System.Windows.Forms.MouseEventHandler(this.con_txt_3rd_lotatc_Click);
            // 
            // con_txt_3rd_srs
            // 
            this.con_txt_3rd_srs.Location = new System.Drawing.Point(123, 16);
            this.con_txt_3rd_srs.Name = "con_txt_3rd_srs";
            this.con_txt_3rd_srs.Size = new System.Drawing.Size(190, 20);
            this.con_txt_3rd_srs.TabIndex = 3;
            this.con_txt_3rd_srs.MouseClick += new System.Windows.Forms.MouseEventHandler(this.con_txt_3rd_srs_Click);
            // 
            // con_check_3rd_lotatc
            // 
            this.con_check_3rd_lotatc.AutoSize = true;
            this.con_check_3rd_lotatc.Location = new System.Drawing.Point(6, 42);
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
            this.con_Button_Quit.Location = new System.Drawing.Point(251, 602);
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
            this.con_lab_github.Location = new System.Drawing.Point(183, 39);
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
            this.label5.Location = new System.Drawing.Point(183, 27);
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
            this.groupBox1.Location = new System.Drawing.Point(14, 214);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(324, 68);
            this.groupBox1.TabIndex = 7;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "DCS Connection";
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
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.Location = new System.Drawing.Point(20, 393);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(61, 13);
            this.label9.TabIndex = 1;
            this.label9.Text = "Database";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.label10.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.Location = new System.Drawing.Point(88, 393);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(39, 13);
            this.label10.TabIndex = 14;
            this.label10.Text = "Game";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.label11.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label11.Location = new System.Drawing.Point(149, 393);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(32, 13);
            this.label11.TabIndex = 15;
            this.label11.Text = "SRS";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.label12.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label12.Location = new System.Drawing.Point(198, 393);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(49, 13);
            this.label12.TabIndex = 16;
            this.label12.Text = "LotATC";
            // 
            // con_Button_Reset_Flags
            // 
            this.con_Button_Reset_Flags.Enabled = false;
            this.con_Button_Reset_Flags.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.con_Button_Reset_Flags.Location = new System.Drawing.Point(266, 362);
            this.con_Button_Reset_Flags.Name = "con_Button_Reset_Flags";
            this.con_Button_Reset_Flags.Size = new System.Drawing.Size(61, 36);
            this.con_Button_Reset_Flags.TabIndex = 17;
            this.con_Button_Reset_Flags.Text = "Reset error flags";
            this.con_Button_Reset_Flags.UseVisualStyleBackColor = true;
            this.con_Button_Reset_Flags.Click += new System.EventHandler(this.con_Button_Reset_Flags_Click);
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label13.Location = new System.Drawing.Point(63, 7);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(74, 25);
            this.label13.TabIndex = 19;
            this.label13.Text = "Perun";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label14.Location = new System.Drawing.Point(65, 32);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(88, 13);
            this.label14.TabIndex = 20;
            this.label14.Text = "for DCS World";
            // 
            // con_img_logo
            // 
            this.con_img_logo.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.con_img_logo.Image = global::Perun_v1.Properties.Resources.perun_logo;
            this.con_img_logo.Location = new System.Drawing.Point(12, 7);
            this.con_img_logo.Name = "con_img_logo";
            this.con_img_logo.Size = new System.Drawing.Size(45, 41);
            this.con_img_logo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.con_img_logo.TabIndex = 18;
            this.con_img_logo.TabStop = false;
            // 
            // con_img_lotATC
            // 
            this.con_img_lotATC.Location = new System.Drawing.Point(207, 362);
            this.con_img_lotATC.Name = "con_img_lotATC";
            this.con_img_lotATC.Size = new System.Drawing.Size(29, 28);
            this.con_img_lotATC.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.con_img_lotATC.TabIndex = 13;
            this.con_img_lotATC.TabStop = false;
            // 
            // con_img_srs
            // 
            this.con_img_srs.Location = new System.Drawing.Point(150, 362);
            this.con_img_srs.Name = "con_img_srs";
            this.con_img_srs.Size = new System.Drawing.Size(29, 28);
            this.con_img_srs.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.con_img_srs.TabIndex = 12;
            this.con_img_srs.TabStop = false;
            // 
            // con_img_dcs
            // 
            this.con_img_dcs.Location = new System.Drawing.Point(93, 362);
            this.con_img_dcs.Name = "con_img_dcs";
            this.con_img_dcs.Size = new System.Drawing.Size(29, 28);
            this.con_img_dcs.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.con_img_dcs.TabIndex = 11;
            this.con_img_dcs.TabStop = false;
            // 
            // con_img_db
            // 
            this.con_img_db.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.con_img_db.Location = new System.Drawing.Point(36, 362);
            this.con_img_db.Name = "con_img_db";
            this.con_img_db.Size = new System.Drawing.Size(29, 28);
            this.con_img_db.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.con_img_db.TabIndex = 10;
            this.con_img_db.TabStop = false;
            // 
            // TIM_Autostart
            // 
            this.TIM_Autostart.Interval = 500;
            this.TIM_Autostart.Tick += new System.EventHandler(this.TIM_Autostart_Tick);
            // 
            // chkCloseToTray
            // 
            this.chkCloseToTray.AutoSize = true;
            this.chkCloseToTray.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.chkCloseToTray.Checked = true;
            this.chkCloseToTray.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkCloseToTray.Location = new System.Drawing.Point(236, 7);
            this.chkCloseToTray.Name = "chkCloseToTray";
            this.chkCloseToTray.Size = new System.Drawing.Size(98, 17);
            this.chkCloseToTray.TabIndex = 24;
            this.chkCloseToTray.Text = "Minimize to tray";
            this.chkCloseToTray.UseVisualStyleBackColor = true;
            this.chkCloseToTray.Validated += new System.EventHandler(this.chkCloseToTray_Validated);
            // 
            // form_Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(346, 652);
            this.Controls.Add(this.chkCloseToTray);
            this.Controls.Add(this.con_Button_Reset_Flags);
            this.Controls.Add(this.label14);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.con_img_logo);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.con_img_lotATC);
            this.Controls.Add(this.con_img_srs);
            this.Controls.Add(this.con_img_dcs);
            this.Controls.Add(this.con_img_db);
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
            this.Text = "Perun for DCS";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.form_Main_FormClosing);
            this.Load += new System.EventHandler(this.form_Main_Load);
            this.con_GroupBox_1.ResumeLayout(false);
            this.con_GroupBox_1.PerformLayout();
            this.con_GroupBox_2.ResumeLayout(false);
            this.con_GroupBox_2.PerformLayout();
            this.con_GroupBox_3.ResumeLayout(false);
            this.con_GroupBox_3.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.con_img_logo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.con_img_lotATC)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.con_img_srs)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.con_img_dcs)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.con_img_db)).EndInit();
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
        private System.Windows.Forms.PictureBox con_img_db;
        private System.Windows.Forms.PictureBox con_img_dcs;
        private System.Windows.Forms.PictureBox con_img_srs;
        private System.Windows.Forms.PictureBox con_img_lotATC;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Button con_Button_Reset_Flags;
        private System.Windows.Forms.PictureBox con_img_logo;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Button con_Button_Add_Marker;
        private System.Windows.Forms.Timer TIM_Autostart;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.ComboBox cboLogLevel;
        private System.Windows.Forms.CheckBox chkCloseToTray;
    }
}

