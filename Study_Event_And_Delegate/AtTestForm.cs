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
        AtComm[] atComms;
        double ST;
        bool listRun = false;
        int RunTimes;
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
                listRun = true;
                button2.Enabled = false;
                ST = DateTime.Now.Ticks / 10000000.0;
                string[] longstrs = textBox3.Text.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                atComms = new AtComm[longstrs.Length];
                for (int i = 0; i < atComms.Count(); i++)
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
        }
        int failTime = 0;
        private void AtTestForm_PortCommFailEvent(object sender, AtComm.AtCommEventArgs e)
        {
            //AtComm AT = (AtComm)sender;
            //if (failTime < 2)
            //{
            //    AT.Run(serialPort1, true);
            //    failTime += 1;
            //}
            //else
            //{
            //    failTime = 0;
            //    if (nowIndex+1 < atComms.Count())
            //    {
            //        nowIndex += 1;
            //        atComms[nowIndex].Run(serialPort1, true);
            //    }
            //}
        }

        private void AtComm_PortTimeOutEvent(object sender, AtComm.AtCommEventArgs e)
        {
            Console.Write("超时");
            AtComm AT = (AtComm)sender;
            //AT.Run(serialPort1, true);
            this.BeginInvoke(new printToWindows(DoPrintToWindows), sender);
            //if (AT.IsTimeOut && nowIndex + 1 < atComms.Count())
            //{
            //    Console.WriteLine("超时,执行下一条");
            //    nowIndex += 1;
            //    atComms[nowIndex].Run(serialPort1, true);
            //}
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
            this.BeginInvoke(new printToWindows(DoPrintToWindows), sender);

        }
        private delegate void printToWindows(AtComm at);
        void DoPrintToWindows(AtComm at)
        {
            if (at.HaveResult)
            {
                return;
            }
            //textBox2.AppendText(at.ReceviceStr);
            if (at.CommState || at.IsTimeOut)
            {
                Application.DoEvents();
                textBox2.AppendText("==================>" + (at.CommState ? "PASS" : "FAIL") + "\r\n");
                at.HaveResult = true;
                if (nowIndex + 1 < atComms.Count())
                {
                    Console.WriteLine("执行成功,执行下一条");
                    nowIndex += 1;
                    atComms[nowIndex].Run(serialPort1, true);
                }
                else
                {
                    if (RunTimes > 0)
                    {
                        RunTimes--;
                        textBox4.Text = RunTimes.ToString();
                        button2_Click(button2, new EventArgs());
                    }
                    else
                    {
                        saveLog();
                        listRun = false;
                        button2.Enabled = true;
                        double ET = DateTime.Now.Ticks / 10000000.0;
                        textBox2.AppendText("用例完成,耗时" + (ET - ST).ToString("f2") + "S\r\n");
                    }
                }
            }
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
                Console.WriteLine(Encoding.Default.GetString(bytes));
                this.BeginInvoke(new addTextToWindow(doAddTextToWindow), Encoding.Default.GetString(bytes));
                if (listRun)
                {
                    atComms[nowIndex].ReceviceStr = Encoding.Default.GetString(bytes);
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
