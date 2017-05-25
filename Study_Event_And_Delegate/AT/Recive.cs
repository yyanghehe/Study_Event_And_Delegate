using System;
using System.Windows.Forms;

namespace Study_Event_And_Delegate.AT
{
    class Recive
    {
        string[] strs;
        public Recive(AtComm atComm)
        {
            //解析字符串
            /*
             * 
             * 
             * */
            string souceStr = atComm.ReceviceStr;
            //if (!souceStr.EndsWith("\r\n"))
                //return;
            //截取字符串并放入字符串数组
            //int lines = souceStr.Split(new string[] { "\r\n"},StringSplitOptions.RemoveEmptyEntries).Length;//获取字符串的总行数
            //strs = new string[lines];
            strs = souceStr.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            
            //for (int i = 0; i < lines; i++)
            //{
            //    int sLength = souceStr.IndexOf("\n") + 1;
            //    string temp = souceStr.Substring(0, sLength);
            //    strs[i] = temp;
            //    souceStr = souceStr.Remove(0, sLength);
            //}
            //解析的字符串与期望值对比
            if (!atComm.CommState)
            {
                Application.DoEvents();
                if (!atComm.IsTimeOut)
                {
                    Commpare(atComm);
                }
                else
                {
                    atComm.CommState = false;
                    return;
                }
            }
            
        }
        enum expectState
        {
            S,
            C,
            E
        }
        private void Commpare(AtComm atComm)
        {
           
            string expectStr = atComm.ExpectStr.Substring(1, atComm.ExpectStr.Length - 1);
            char expectHead = atComm.ExpectStr.Substring(0, 1).ToCharArray()[0];
            switch (expectHead)
            {
                case (char)'S':
                    if (strs[0].StartsWith(expectStr))
                    {
                        atComm.CommState = true;
                    }
                    else
                    {
                        if(atComm.IsTimeOut)
                            atComm.CommState = false;
                    }
                    break;
                case (char)'C':
                    string afterS="";
                    for(int i = 1; i < strs.Length-1; i++)
                    {
                        afterS += strs[i];
                    }
                    if(afterS.Contains(expectStr))
                    {
                        atComm.CommState = true;
                    }
                    else
                    {
                        if (atComm.IsTimeOut)
                            atComm.CommState = false;
                    }
                    break;
                case (char)'E':
                    if (strs[strs.Length - 1].StartsWith(expectStr))
                    {
                        atComm.CommState = true;
                    }
                    else
                    {
                        if (atComm.IsTimeOut)
                            atComm.CommState = false;
                    }
                    break;
                default:
                    if (atComm.IsTimeOut)
                        atComm.CommState = false;
                    break;
            }
            

        }
    }
}
