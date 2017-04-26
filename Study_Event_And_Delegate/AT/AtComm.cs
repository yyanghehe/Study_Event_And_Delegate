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
        public static bool ATE;
        /// <summary>
        /// 发送字符串
        /// </summary>
        public string SendStr
        {
            get
            {
                return sendStr;
            }

            set
            {
                sendStr = value;
                AtCommEventArgs e = new AtCommEventArgs(value);
                PortSendEvent?.Invoke(this, e);
            }
        }
        /// <summary>
        /// 接收到的字符串
        /// </summary>
        public string ReceviceStr
        {
            get
            {
                return receviceStr;
            }

            set
            {
                receviceStr = value;
                AtCommEventArgs e = new AtCommEventArgs(value);
                PortReceviceEvent?.Invoke(this,e);
            }
        }
        /// <summary>
        /// 期望字符串
        /// </summary>
        public string ExpectStr
        {
            get
            {
                return expectStr;
            }

            set
            {
                expectStr = value;
            }
        }
        /// <summary>
        /// 命令成功执行下次执行的字符串
        /// </summary>
        public string NextStr
        {
            get
            {
                return nextStr;
            }

            set
            {
                nextStr = value;
            }
        }
        /// <summary>
        /// 执行失败执行的字符串
        /// </summary>
        public string ElseStr
        {
            get
            {
                return elseStr;
            }

            set
            {
                elseStr = value;
            }
        }
        /// <summary>
        /// 发送标识
        /// </summary>
        public bool SendFlag
        {
            get
            {
                return sendFlag;
            }

            set
            {
                sendFlag = value;
            }
        }
        /// <summary>
        /// 超时标识
        /// </summary>
        public bool IsTimeOut
        {
            get
            {
                return isTimeOut;
            }

            set
            {
                isTimeOut = value;
            }
        }
        /// <summary>
        /// 超时时间
        /// </summary>
        public int TimeOut
        {
            get
            {
                return timeOut;
            }

            set
            {
                timeOut = value;
            }
        }
        /// <summary>
        /// 开始时间
        /// </summary>
        public int StartTime
        {
            get
            {
                return startTime;
            }

            set
            {
                startTime = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public bool CommState
        {
            get
            {
                return commState;
            }

            set
            {
                commState = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public string PortERROR1
        {
            get
            {
                return PortERROR;
            }

            set
            {
                PortERROR = value;
            }
        }

        //事件
        //public delegate void PortReceviceEventHandler(object sender,PortReceviceEventArgs e);
        //public event PortReceviceEventHandler PortReceviceEvent;//接收数据事件
        public event EventHandler<AtCommEventArgs> PortSendEvent;//发送数据事件
        public event EventHandler<AtCommEventArgs> PortTimeOutEvent;//超时事件
        public event EventHandler<AtCommEventArgs> PortReceviceEvent;

        //激活超时事件
        public void RaisePortTimeOutEvent(){
            AtCommEventArgs e = new AtCommEventArgs(IsTimeOut);
            PortTimeOutEvent?.Invoke(this, e);
        }
        //激活发送事件
        public void RaisePortSendEvent(bool sendFlag)
        {
            AtCommEventArgs e = new AtCommEventArgs(sendFlag);
            PortSendEvent?.Invoke(this, e); 
        }
        public void RaisePortReceviceEvent(byte[] bytes)
        {
        }
        //串口事件,赋值和处理
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
            this.SendStr = strs[0];
            this.ExpectStr = strs[1];
            this.NextStr = strs[2];
        }
        private void If4Count(string[] strs)
        {
            If3Count(strs);
            this.TimeOut =int.Parse( strs[3]);
        }

        public void runComm(SerialPort sp,bool endLine)
        {
            try { 
            sp.WriteLine(SendStr + (endLine ? "/r/n" : ""));
            }
            catch(Exception E)
            {
                PortERROR1 = E.ToString();
            }
            RaisePortSendEvent(true);
            
        }
    }
}
