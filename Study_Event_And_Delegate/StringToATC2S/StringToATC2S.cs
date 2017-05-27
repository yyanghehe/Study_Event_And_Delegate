using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO.Ports;
using System.Windows.Forms;

namespace Study_Event_And_Delegate
{
     class StringToATC2S
    {
        //AutoResetEvent myCommandEvent = new AutoResetEvent(false);
        //SerialPort myPort;
        public string RequestATC2;
        internal StringToATC2S(string str, SerialPort port,RichTextBox rbox)
        {
            string[] temps = str.Split(new string[] { "\r\n"},StringSplitOptions.RemoveEmptyEntries);
            //同步执行AT指令
            foreach(string temp in temps)
            {

                if (temp.StartsWith("AT") || temp.StartsWith("A\\"))
                {
                    //此为AT指令,对其解析并执行
                }
                if (temp.StartsWith("SLEEP"))
                {
                    //线程需要睡眠
                    int time = 0;
                    if (temp.Contains("(") && temp.Contains(")"))
                    {
                        time = int.Parse(temp.Replace("SLEEP", "").Replace("(", "").Replace(")", ""));
                    }
                    else
                    {
                        return;
                    }
                    Thread.Sleep(Int32.Parse("") * 1000);
                }
            }

        }
    }
}
