using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Study_Event_And_Delegate
{
    public partial class AtTestForm2 : Form
    {
        public AtTestForm2()
        {
            InitializeComponent();
            serialPort1.NewLine = "\r\n";
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try {
                if (serialPort1.IsOpen)
                {
                    serialPort1.Close();
                    button3.Text = "打开";
                }
                else
                {
                    serialPort1.Open();
                    button3.Text = "关闭";
                }
            }
            catch
            {
                if (serialPort1.IsOpen)
                {
                    richTextBox1.AppendText("关闭串口失败");
                }
                else
                {
                    richTextBox1.AppendText("打开串口失败");
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            serialPort1.WriteLine(textBox3.Text);
        }

        private void button1_Click(object sender, EventArgs e)
        {

            StringToATC2S stoATC2 = new StringToATC2S(textBox2.Text, serialPort1,richTextBox1);
            richTextBox1.AppendText(stoATC2.RequestATC2);
        }
    }
}
