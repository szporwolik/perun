using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Perun_v1
{
    public partial class form_Main : Form
    {
        public form_Main()
        {
            InitializeComponent();
        }

        private void con_Button_DEBUG_Click(object sender, EventArgs e)
        {
            con_List_Received.Items.Add("test");
        }

        private void form_Main_Load(object sender, EventArgs e)
        {

        }
    }
}
