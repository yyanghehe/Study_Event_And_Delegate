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
            serialPort1.Open();
            serialPort1.DataReceived += SerialPort1_DataReceived;
            serialPort1.Write(atComm.send);
            atComm.PortRecevice += AtComm_PortRecevice;
        }

        private void SerialPort1_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            //atComm.
        }

        private void AtComm_PortRecevice(object sender, AtComm.PortReceviceEventArgs e)
        {
            if (e.ReceviceStr.Length > 0) {
                Console.WriteLine(e.ReceviceStr);
            }
        }

        public class AtComm{
            public string send = "AT";
            private string ReceviceStr;
            public delegate void PortReceviceEventHandler(object sender, PortReceviceEventArgs e);
            public event PortReceviceEventHandler PortRecevice;
            public class PortReceviceEventArgs : EventArgs
            {
                public readonly string ReceviceStr;
                public PortReceviceEventArgs(string ReceviceStr)
                {
                    this.ReceviceStr = ReceviceStr;
                }
            }
        }
    }
}
