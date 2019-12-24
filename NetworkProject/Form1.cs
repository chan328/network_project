using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NetworkProject
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            this.button1.Click += new System.EventHandler(this.button1_Click);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // 메세지 전송
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            // 보낼 메세지를 입력하는 textbox
            String msg = this.textBox1.Text; 
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            // 주고받은 채팅을 보여주는 textbox
            this.textBox3.ReadOnly = true;

        }
    }
}
