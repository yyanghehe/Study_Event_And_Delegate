using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

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
        bool isTimeOut=false; //超时标识
        int timeOut = 3; //超时时间
        double startTime; //开始时间
        bool commState=false; //命令执行结果
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
                PortBeforSendEvent?.Invoke(this, e);
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
                if (!isTimeOut)
                {
                    cancleaction.Cancel();
                    receviceStr = value;
                    new Recive(this);
                    AtCommEventArgs e = new AtCommEventArgs(value);
                    PortReceviceEvent?.Invoke(this, e);
                }
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
                if (value)
                {
                    AtCommEventArgs e = new AtCommEventArgs(value);
                    PortTimeOutEvent?.Invoke(this,e);
                }
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
        public double StartTime
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
                if (!value)
                {
                    AtCommEventArgs e = new AtCommEventArgs(value);
                    PortCommFailEvent?.Invoke(this, e);
                }
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
                AtCommEventArgs e = new AtCommEventArgs(value);
                PortErrorEvent?.Invoke(this, e);
            }
        }

        //事件
        //public delegate void PortReceviceEventHandler(object sender,PortReceviceEventArgs e);
        //public event PortReceviceEventHandler PortReceviceEvent;//接收数据事件
        public event EventHandler<AtCommEventArgs> PortSendEvent;//发送数据事件
        public event EventHandler<AtCommEventArgs> PortTimeOutEvent;//超时事件
        public event EventHandler<AtCommEventArgs> PortReceviceEvent;//接收数据事件
        public event EventHandler<AtCommEventArgs> PortBeforSendEvent;//发送数据前
        public event EventHandler<AtCommEventArgs> PortErrorEvent;//串口发生错误
        /// <summary>
        /// 命令执行错误
        /// </summary>
        public event EventHandler<AtCommEventArgs> PortCommFailEvent;

        //激活超时事件
        public void RaisePortTimeOutEvent()
        {
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
            public string str;
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
        private void setStrs(string[] strs, longToShort DoLongToShort)
        {
            DoLongToShort(strs);
        }
        CancellationTokenSource cancleaction = new CancellationTokenSource();
        public AtComm(string longStr) {
            string[] strs = longStr.Split(new string[] { " " }, StringSplitOptions.None);
            if (strs.Length == 3)
            {
                setStrs(strs, If3Count);
            }
            if (strs.Length == 4)
            {
                setStrs(strs, If4Count);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="longStr">需要解析的字符串</param>
        /// <param name="sp">端口</param>
        /// <param name="endLine">是否需要加上结束符发送</param>
        public void Run(SerialPort sp, bool endLine)
        {
            try
            {
                sp.WriteLine(SendStr + (endLine ? "\r\n" : ""));
                StartTime = DateTime.Now.Ticks;
            }
            catch (Exception E)
            {
                this.PortERROR1 = E.Message;
                return;
            }

            AtCommEventArgs e = new AtCommEventArgs(true);
            PortSendEvent?.Invoke(this, e);
            //添加超时机制

            Task.Factory.StartNew(() =>
            {
                while (!cancleaction.IsCancellationRequested)
                {
                    if ((DateTime.Now.Ticks - startTime) / 10000000.0 > timeOut)
                    {
                        this.IsTimeOut = true;
                        cancleaction.Cancel();
                       
                    }
                }
            });
        }
        private void If3Count(string[] strs)
        {
            this.SendStr = strs[0];
            this.ExpectStr = strs[1];
            this.NextStr = strs[2];
        }
        private void If4Count(string[] strs)
        {
            If3Count(strs);
            this.TimeOut = int.Parse(strs[3]);
        }
        public void runComm(SerialPort sp, bool endLine)
        {
            try
            {
                sp.WriteLine(SendStr);
            }
            catch (Exception E)
            {
                PortERROR1 = E.ToString();
            }
            RaisePortSendEvent(true);
        }
    }
}
