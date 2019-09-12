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
            this.tim_1000ms = new System.Windows.Forms.Timer(this.components);
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
            this.tim_10000ms = new System.Windows.Forms.Timer(this.components);
            this.tim_200ms = new System.Windows.Forms.Timer(this.components);
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label8 = new System.Windows.Forms.Label();
            this.con_txt_dcs_instance = new System.Windows.Forms.MaskedTextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.con_txt_dcs_server_port = new System.Windows.Forms.MaskedTextBox();
            this.con_GroupBox_1.SuspendLayout();
            this.con_GroupBox_2.SuspendLayout();
            this.con_GroupBox_3.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // con_List_Received
            // 
            this.con_List_Received.Enabled = false;
            this.con_List_Received.FormattingEnabled = true;
            this.con_List_Received.Items.AddRange(new object[] {
            "Welcome to Perun for DCS World!"});
            this.con_List_Received.Location = new System.Drawing.Point(6, 19);
            this.con_List_Received.Name = "con_List_Received";
            this.con_List_Received.Size = new System.Drawing.Size(307, 147);
            this.con_List_Received.TabIndex = 0;
            // 
            // con_Button_Listen_ON
            // 
            this.con_Button_Listen_ON.Location = new System.Drawing.Point(12, 519);
            this.con_Button_Listen_ON.Name = "con_Button_Listen_ON";
            this.con_Button_Listen_ON.Size = new System.Drawing.Size(86, 39);
            this.con_Button_Listen_ON.TabIndex = 2;
            this.con_Button_Listen_ON.Text = "Connect";
            this.con_Button_Listen_ON.UseVisualStyleBackColor = true;
            this.con_Button_Listen_ON.Click += new System.EventHandler(this.con_Button_Listen_ON_Click);
            // 
            // con_Button_Listen_OFF
            // 
            this.con_Button_Listen_OFF.Enabled = false;
            this.con_Button_Listen_OFF.Location = new System.Drawing.Point(104, 519);
            this.con_Button_Listen_OFF.Name = "con_Button_Listen_OFF";
            this.con_Button_Listen_OFF.Size = new System.Drawing.Size(86, 39);
            this.con_Button_Listen_OFF.TabIndex = 3;
            this.con_Button_Listen_OFF.Text = "Disconnect";
            this.con_Button_Listen_OFF.UseVisualStyleBackColor = true;
            this.con_Button_Listen_OFF.Click += new System.EventHandler(this.con_Button_Listen_OFF_Click);
            // 
            // con_GroupBox_1
            // 
            this.con_GroupBox_1.Controls.Add(this.con_List_Received);
            this.con_GroupBox_1.Location = new System.Drawing.Point(12, 335);
            this.con_GroupBox_1.Name = "con_GroupBox_1";
            this.con_GroupBox_1.Size = new System.Drawing.Size(324, 176);
            this.con_GroupBox_1.TabIndex = 4;
            this.con_GroupBox_1.TabStop = false;
            this.con_GroupBox_1.Text = "Data log";
            // 
            // tim_1000ms
            // 
            this.tim_1000ms.Interval = 1000;
            this.tim_1000ms.Tick += new System.EventHandler(this.timer1_Tick);
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
            this.con_GroupBox_2.Location = new System.Drawing.Point(12, 27);
            this.con_GroupBox_2.Name = "con_GroupBox_2";
            this.con_GroupBox_2.Size = new System.Drawing.Size(324, 154);
            this.con_GroupBox_2.TabIndex = 5;
            this.con_GroupBox_2.TabStop = false;
            this.con_GroupBox_2.Text = "MySQL";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(41, 48);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(59, 13);
            this.label6.TabIndex = 9;
            this.label6.Text = "Server port";
            // 
            // con_txt_mysql_port
            // 
            this.con_txt_mysql_port.Enabled = false;
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
            this.label2.Location = new System.Drawing.Point(47, 74);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Database";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(22, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(78, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Server address";
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
            this.con_GroupBox_3.Location = new System.Drawing.Point(12, 261);
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
            this.con_Button_Quit.Location = new System.Drawing.Point(251, 519);
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
            this.con_lab_github.Location = new System.Drawing.Point(141, 9);
            this.con_lab_github.Name = "con_lab_github";
            this.con_lab_github.Size = new System.Drawing.Size(181, 13);
            this.con_lab_github.TabIndex = 8;
            this.con_lab_github.TabStop = true;
            this.con_lab_github.Text = "https://github.com/szporwolik/perun";
            this.con_lab_github.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.con_lab_github_LinkClicked);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(15, 9);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(120, 13);
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
            // tim_10000ms
            // 
            this.tim_10000ms.Interval = 30000;
            this.tim_10000ms.Tick += new System.EventHandler(this.tim_10000ms_Tick);
            // 
            // tim_200ms
            // 
            this.tim_200ms.Tick += new System.EventHandler(this.tim_200ms_Tick);
            // 
            // notifyIcon1
            // 
            this.notifyIcon1.Text = "notifyIcon1";
            this.notifyIcon1.Visible = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.con_txt_dcs_instance);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.con_txt_dcs_server_port);
            this.groupBox1.Location = new System.Drawing.Point(12, 187);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(324, 68);
            this.groupBox1.TabIndex = 7;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "DCS Connection";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(41, 45);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(62, 13);
            this.label8.TabIndex = 12;
            this.label8.Text = "Instance ID";
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
            this.label7.Location = new System.Drawing.Point(41, 19);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(59, 13);
            this.label7.TabIndex = 10;
            this.label7.Text = "Server port";
            // 
            // con_txt_dcs_server_port
            // 
            this.con_txt_dcs_server_port.Location = new System.Drawing.Point(106, 16);
            this.con_txt_dcs_server_port.Name = "con_txt_dcs_server_port";
            this.con_txt_dcs_server_port.Size = new System.Drawing.Size(207, 20);
            this.con_txt_dcs_server_port.TabIndex = 3;
            // 
            // form_Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(349, 570);
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
            this.MinimizeBox = false;
            this.Name = "form_Main";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Perun for DCS World";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.form_Main_FormClosing);
            this.Load += new System.EventHandler(this.form_Main_Load);
            this.con_GroupBox_1.ResumeLayout(false);
            this.con_GroupBox_2.ResumeLayout(false);
            this.con_GroupBox_2.PerformLayout();
            this.con_GroupBox_3.ResumeLayout(false);
            this.con_GroupBox_3.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox con_List_Received;
        private System.Windows.Forms.Button con_Button_Listen_ON;
        private System.Windows.Forms.Button con_Button_Listen_OFF;
        private System.Windows.Forms.GroupBox con_GroupBox_1;
        private System.Windows.Forms.Timer tim_1000ms;
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
        private System.Windows.Forms.Timer tim_10000ms;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox con_txt_mysql_port;
        private System.Windows.Forms.Timer tim_200ms;
        private System.Windows.Forms.NotifyIcon notifyIcon1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.MaskedTextBox con_txt_dcs_server_port;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.MaskedTextBox con_txt_dcs_instance;
    }
}

