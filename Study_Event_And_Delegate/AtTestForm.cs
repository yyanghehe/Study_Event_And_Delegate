using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Study_Event_And_Delegate.AT;

namespace Study_Event_And_Delegate
{
    public partial class AtTestForm : Form
    {
        public AtTestForm()
        {

            InitializeComponent();
            serialPort1.NewLine = "\r\n";
        }

        private void SerialPort1_PinChanged(object sender, System.IO.Ports.SerialPinChangedEventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            serialPort1.Open();
        }
        AtComm atComm;
        int nowIndex = 0;
        AtComm[] atComms;
        private void button2_Click(object sender, EventArgs e)
        {

            string[] longstrs = textBox3.Text.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            atComms = new AtComm[longstrs.Length];
            for(int i =0;i<atComms.Count();i++)
            {
                atComms[i] = new AtComm(longstrs[i]);
                //atComms[i].SendStr = "AT";
                atComms[i].PortReceviceEvent += AtComm_PortReceviceEvent;
                atComms[i].PortErrorEvent += AtComm_PortErrorEvent;
                atComms[i].PortTimeOutEvent += AtComm_PortTimeOutEvent;
                atComms[i].PortCommFailEvent += AtTestForm_PortCommFailEvent;
            }
            nowIndex = 0;
            atComms[nowIndex].Run(serialPort1, true);
        }
        int failTime = 0;
        private void AtTestForm_PortCommFailEvent(object sender, AtComm.AtCommEventArgs e)
        {
            AtComm AT = (AtComm)sender;
            if (failTime < 2)
            {
                AT.Run(serialPort1, true);
                failTime += 1;
            }
            else
            {
                failTime = 0;
                if (nowIndex+1 < atComms.Count())
                {
                    nowIndex += 1;
                    atComms[nowIndex].Run(serialPort1, true);
                }
            }
        }

        private void AtComm_PortTimeOutEvent(object sender, AtComm.AtCommEventArgs e)
        {
            Console.Write("超时");
            AtComm AT = (AtComm)sender;
            AT.Run(serialPort1, true);
        }

        private void AtComm_PortErrorEvent(object sender, AtComm.AtCommEventArgs e)
        {
            Console.Write("ERROR:{0}", e.str);
        }

        private void AtComm_PortReceviceEvent(object sender, AtComm.AtCommEventArgs e)
        {
            AtComm AT = (AtComm)sender;
            Console.WriteLine("S:{0}", AT.SendStr);
            Console.WriteLine("R:{0}", e.str);
            this.BeginInvoke(new printToWindows(DoPrintToWindows),sender);
            if (AT.CommState&& nowIndex+1<atComms.Count())
            {
                nowIndex += 1;
                atComms[nowIndex].Run(serialPort1, true);
            }
            else {  }
        }
        private delegate void printToWindows(AtComm at);
        void DoPrintToWindows(AtComm at)
        {
            textBox2.AppendText(at.ReceviceStr);
            textBox2.AppendText("==================>" + (at.CommState ? "PASS" : "FAIL")+"\r\n");
        }
        private void serialPort1_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            if (serialPort1.BytesToRead > 0)
            {
                Thread.Sleep(500);
                int count = serialPort1.BytesToRead;
                byte[] bytes = new byte[count];
                serialPort1.Read(bytes, 0, count);
                Console.WriteLine(Encoding.Default.GetString(bytes));
                atComms[nowIndex].ReceviceStr = Encoding.Default.GetString(bytes);
            }
        }
    }
}
