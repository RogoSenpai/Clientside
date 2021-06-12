using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Client
{
    public partial class ChatEnter : Form
    {
        public ChatEnter()
        {
            InitializeComponent();
            button1.Enabled = false;
        }

        public String numeText()
        {
            return textBox1.Text;
        }

        private void ChatEnter_Load(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            button1.Enabled = true;
        }
    }
}
