using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;

namespace Study_Event_And_Delegate.AT
{
    class AtComm
    {
        //字段
        string sendStr; //发送字符串
        string receviceStr; //接收到的字符串
        string expectStr; //期望字符串
        string nextStr; //命令成功执行下次执行的字符串
        string elseStr; //执行失败执行的字符串
        bool sendFlag; //发送标识
        bool isTimeOut; //超时标识
        int timeOut=3; //超时时间
        int startTime; //开始时间
        bool commState; //命令执行结果
        string PortERROR; //串口出现的Error信息
        //事件
        public delegate void PortReceviceEventHandler(object sender,PortReceviceEventArgs e);
        public event PortReceviceEventHandler PortReceviceEvent;//接收数据事件
        public event EventHandler<AtCommEventArgs> PortSendEvent;//发送数据事件
        public event EventHandler<AtCommEventArgs> PortTimeOutEvent;

        public void RaisePortTimeOutEvent(){
            AtCommEventArgs e = new AtCommEventArgs(isTimeOut);
            PortTimeOutEvent?.Invoke(this, e);
        }
        public void RaisePoreSendEvent(bool sendFlag)
        {
            AtCommEventArgs e = new AtCommEventArgs(sendFlag);
            PortSendEvent?.Invoke(this, e); 
        }
        public class PortReceviceEventArgs : EventArgs
        {
            string receviceStr;
            public PortReceviceEventArgs(string receviceStr)
            {
                this.receviceStr = receviceStr;
            }
        }
        public class AtCommEventArgs : EventArgs
        {
            bool handle;
            string str;
            public AtCommEventArgs(string str)
            {
                this.str = str;
                handle = true;
            }
            public AtCommEventArgs(bool sendFlag)
            {
                this.handle = sendFlag;
            }

        }
        public void setReceviceStr(string str) {
            if (str.Length > 0)
            {
                PortReceviceEventArgs e = new PortReceviceEventArgs(str);
                PortReceviceEvent?.Invoke(this, e);
            }
        }
        private delegate void longToShort(string[] strs);
        private void setStrs(string[] strs, longToShort DoLongToShort) {
            DoLongToShort(strs);
        }
        public AtComm(string longStr)
        {
            string[] strs = longStr.Split(new string[] {" "},StringSplitOptions.None);
            if (strs.Length == 3)
            {
                setStrs(strs, If3Count);
            }
            if (strs.Length == 4)
            {
                setStrs(strs, If4Count);
            }

        }
        private void If3Count(string[] strs) {
            this.sendStr = strs[0];
            this.expectStr = strs[1];
            this.nextStr = strs[2];
        }
        private void If4Count(string[] strs)
        {
            If3Count(strs);
            this.timeOut =int.Parse( strs[3]);
        }

        public void runComm(SerialPort sp,bool endLine)
        {
            try { 
            sp.WriteLine(sendStr + (endLine ? "/r/n" : ""));
            }
            catch(Exception E)
            {
                ;
            }
            RaisePoreSendEvent(true);
        }
    }
}
