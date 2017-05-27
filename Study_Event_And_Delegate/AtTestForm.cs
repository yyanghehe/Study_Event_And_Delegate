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
using System.IO.Ports;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;

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

            try
            {
                if (serialPort1.IsOpen)
                {
                    serialPort1.Close();
                    button1.Text = "打开";
                }
                else
                {
                    if (comboBox1.SelectedItem != null)
                    {
                        serialPort1.PortName = comboBox1.SelectedItem.ToString();
                        serialPort1.Open();
                        button1.Text = "关闭";
                    }
                }
            }
            catch (Exception E)
            {
                textBox2.AppendText(E.Message);
            }
        }

        AtComm atComm;
        int nowIndex = 0;
        //AtComm[] atComms;
        double ST;
        bool listRun = false;
        int RunTimes;
        bool atSendOver = false;
        bool atReceviceOver = false;
        AutoResetEvent myEvent = new AutoResetEvent(false);
        //ManualResetEvent myEvent= new ManualResetEvent(false);
        private void button2_Click(object sender, EventArgs e)
        {
            //(DateTime.Now.Ticks - startTime) / 10000000.0
            RunTimes = int.Parse(textBox4.Text);
            if (!listRun)
            {
                textBox2.Text = "";
            }
            if (serialPort1.IsOpen)
            {
                startRun();
                string[] longstrs = textBox3.Text.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                myEvent.Set();
                //atComms = new AtComm[longstrs.Length];
                atSendOver = false;
                atReceviceOver = false;
                Thread myThread = new Thread(new ParameterizedThreadStart(runATCS));
                myThread.Start(longstrs);
            }
        }
        string failCaseName = "";
        void startRun()
        {
            listRun = true;
            button2.Enabled = false;
            textBox2.Text = "";
            ST = DateTime.Now.Ticks / 10000000.0;
            allCase = 0;
            failCase = 0;
            passCase = 0;
            failCaseName = "";
        }
        void runATCS(object obj)
        {
            string[] strs = (string[])obj;
            for (int i = 0; i < strs.Count(); i++)
            {
                myEvent.WaitOne();
                atComm = new AtComm(strs[i]);
                //atComms[i].SendStr = "AT";
                atComm.PortReceviceEvent += AtComm_PortReceviceEvent;
                atComm.PortErrorEvent += AtComm_PortErrorEvent;
                atComm.PortTimeOutEvent += AtComm_PortTimeOutEvent;
                atComm.PortSourceSuccessEvent += AtComm_PortSourceSuccessEvent;
                atComm.Index = i;
                atComm.Run(serialPort1, true);
            }
            atSendOver = true;
        }
        int failTime = 0;
        private void AtComm_PortSourceSuccessEvent(object sender, AtComm.AtCommEventArgs e)
        {
            AtComm AT = (AtComm)sender;
            Console.WriteLine("S:{0}", AT.SendStr);
            Console.WriteLine("R:{0}", AT.ReceviceStr);
            if (AT.Index == atComm.Index)
            {
                this.BeginInvoke(new printToWindows(DoPrintToWindows));
            }
        }

        private void AtComm_PortTimeOutEvent(object sender, AtComm.AtCommEventArgs e)
        {
            Console.Write("超时");
            AtComm AT = (AtComm)sender;
            if (AT.Index == atComm.Index) { 
            this.BeginInvoke(new printToWindows(DoPrintToWindows));
            }
        }

        private void AtComm_PortErrorEvent(object sender, AtComm.AtCommEventArgs e)
        {
            //myEvent.Set();
            Console.Write("ERROR:{0}", e.str);
        }

        private void AtComm_PortReceviceEvent(object sender, AtComm.AtCommEventArgs e)
        {
            
        }
        private delegate void printToWindows();
        int allCase = 0;
        int failCase = 0;
        int passCase = 0;
        void DoPrintToWindows()
        {
            //textBox2.AppendText(at.ReceviceStr);
            if (atComm.CommState || atComm.IsTimeOut)
            {
                
                textBox2.AppendText("==================>" + (atComm.CommState ? "PASS" : "FAIL") + "\r\n");
                if (atComm.CommState)
                {
                    passCase += 1;
                }
                else
                {
                    failCase += 1;
                    failCaseName +=(atComm.Index+1) +":"+ atComm.SendStr+"\r\n";
                }
                myEvent.Set();

                if (atSendOver)
                {
                    endRun();
                }
            }
        }
        void endRun()
        {
            atReceviceOver = true;
            double ET = DateTime.Now.Ticks / 10000000.0;
            textBox2.AppendText("用例完成,耗时" + (ET - ST).ToString("f2") + "S\r\n");
            string append = "总共执行"+(failCase+passCase)+"条用例"+"\r\n"+"fail:" + failCase + "  " + "pass:" + passCase + "  " + "\r\n" ;
            if (failCase > 0)
            {
                append += "失败用例:" + "\r\n" + failCaseName;
            }
            textBox2.AppendText(append);
            saveLog();
            button2.Enabled = true;
        }
        private void serialPort1_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            Console.WriteLine("COMEIN......");
            if (serialPort1.BytesToRead > 0)
            {
                Application.DoEvents();
                //Thread.Sleep(500);
                int count = serialPort1.BytesToRead;
                byte[] bytes = new byte[count];
                serialPort1.Read(bytes, 0, count);
                this.BeginInvoke(new addTextToWindow(doAddTextToWindow), Encoding.Default.GetString(bytes));
                if (listRun)
                {
                    atComm.ReceviceStr = Encoding.Default.GetString(bytes);
                }
            }
        }
        private delegate void addTextToWindow(string str);
        void doAddTextToWindow(string str)
        {
            textBox2.AppendText(str);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                serialPort1.WriteLine(textBox1.Text);
            }
            catch (Exception E)
            {
                textBox2.AppendText(E.Message);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            string[] ports = SerialPort.GetPortNames();
            comboBox1.Items.Clear();
            comboBox1.Items.AddRange(ports);
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        private void comboBox1_SelectedValueChanged(object sender, EventArgs e)
        {

        }
        void saveLog()
        {
            string filePath = System.Windows.Forms.Application.StartupPath + @"\log\" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".log";
            if (!File.Exists(filePath))
            {
                using (File.CreateText(filePath))
                {
                }
            }
            File.WriteAllText(filePath, textBox2.Text);
        }
    }
}
