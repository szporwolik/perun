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
            this.con_List_Received = new System.Windows.Forms.ListBox();
            this.con_Button_DEBUG = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // con_List_Received
            // 
            this.con_List_Received.FormattingEnabled = true;
            this.con_List_Received.Location = new System.Drawing.Point(12, 12);
            this.con_List_Received.Name = "con_List_Received";
            this.con_List_Received.Size = new System.Drawing.Size(594, 550);
            this.con_List_Received.TabIndex = 0;
            // 
            // con_Button_DEBUG
            // 
            this.con_Button_DEBUG.Location = new System.Drawing.Point(612, 21);
            this.con_Button_DEBUG.Name = "con_Button_DEBUG";
            this.con_Button_DEBUG.Size = new System.Drawing.Size(80, 48);
            this.con_Button_DEBUG.TabIndex = 1;
            this.con_Button_DEBUG.Text = "Debug";
            this.con_Button_DEBUG.UseVisualStyleBackColor = true;
            this.con_Button_DEBUG.Click += new System.EventHandler(this.con_Button_DEBUG_Click);
            // 
            // form_Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(738, 567);
            this.Controls.Add(this.con_Button_DEBUG);
            this.Controls.Add(this.con_List_Received);
            this.Name = "form_Main";
            this.Text = "Perun for DCS World";
            this.Load += new System.EventHandler(this.form_Main_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox con_List_Received;
        private System.Windows.Forms.Button con_Button_DEBUG;
    }
}

