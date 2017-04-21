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
    public partial class MyPracticeForm : Form
    {
        AtComm atComm = new AtComm();
        public MyPracticeForm()
        {
            InitializeComponent();
            //AtComm atComm = new AtComm();
            //serialPort1.Open();
            serialPort1.DataReceived += SerialPort1_DataReceived;
            //serialPort1.Write(atComm.send);
            atComm.PortRecevice += AtComm_PortRecevice;
            atComm.PortRecevice += ReciveStr.readStr;
        }

        private void SerialPort1_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            int count = serialPort1.BytesToRead;
            if (count > 0)
            {
                Console.WriteLine("GETLENGTH:" + count);
                byte[] bufferBytes = new byte[count];
                serialPort1.Read(bufferBytes, 0, count);
                atComm.setReceviceStr(Encoding.Default.GetString(bufferBytes));
                ShowText toShow = new ShowText(showText);
                this.BeginInvoke(toShow, Encoding.Default.GetString(bufferBytes));
              }
        }
        delegate void ShowText(string str);
        private void showText(string str)
        {
            textBox2.AppendText(str);
        }
        private void AtComm_PortRecevice(object sender, AtComm.PortReceviceEventArgs e)
        {
            Console.WriteLine("atCommEvent:"+e.ReceviceStr);
        }

        public class AtComm{
            public string sendStr = "AT";
            public string hopeStr = "AT\r\r\nOK\r\n";
            public string tempStr = "";
            public string realStr = "";
            public delegate void PortReceviceEventHandler(object sender, PortReceviceEventArgs e);
            public event PortReceviceEventHandler PortRecevice;
            public event EventHandler<PortReceviceEventArgs> PortRecevice2;
            public class PortReceviceEventArgs : EventArgs
            {
                public readonly string ReceviceStr;
                public PortReceviceEventArgs(string ReceviceStr)
                {
                    this.ReceviceStr = ReceviceStr;
                }
            }
            public void setReceviceStr(string ReceviceStr)
            {
                if (PortRecevice != null)
                {
                    PortReceviceEventArgs e = new PortReceviceEventArgs(ReceviceStr);
                    PortRecevice(this,e);
                }
            }

        }
        public class ReciveStr {
            private string Temp;
            public static void readStr(object sender,AtComm.PortReceviceEventArgs e)
            {
                AtComm atComm = (AtComm)sender;
                if (e.ReceviceStr.Length > 0)
                {
                    
                    if (atComm.hopeStr.ToUpper() != e.ReceviceStr.ToUpper())
                    {
                        if (e.ReceviceStr.StartsWith(atComm.sendStr))
                        {
                            atComm.tempStr += e.ReceviceStr;
                        }
                    }
                    else
                    {
                        Console.WriteLine("True");
                    }
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            serialPort1.Open();
            if (serialPort1.IsOpen) { Console.WriteLine("串口打开成功"); }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            serialPort1.Write(textBox1.Text+"\r\n");
            atComm.sendStr = textBox1.Text;
            Console.WriteLine("数据发送成功");
        }
    }
}
