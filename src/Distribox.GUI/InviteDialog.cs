using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Distribox.GUI
{
    public partial class InviteDialog : Form
    {
        public int Port { get; set; }

        public InviteDialog()
        {
            InitializeComponent();

            this.Port = -1;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                this.Port = int.Parse(this.textBox1.Text);
                this.Close();
            }
            catch
            {
                MessageBox.Show("Please input a valid number");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
