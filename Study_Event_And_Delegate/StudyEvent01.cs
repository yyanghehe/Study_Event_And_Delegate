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
    public partial class StudyEvent01 : Form
    {
        public class Heater
        {
            private int temperature;
            public string type = "fasthot 001";//产品型号
            public string area = "Chian ChengDu";//产地
            public event BoiledEventHandler Boiled;//声明事件
            public delegate void BoiledEventHandler(object sender, BoiledEventArgs e);

            //定义 BoiledEventArgs 类,传递给 Observer 所感兴趣的信息
            public class BoiledEventArgs : EventArgs
            {
                public readonly int temperature;
                public BoiledEventArgs(int temperature)
                {
                    this.temperature = temperature;
                }
            }
            protected virtual void OnBoiled(BoiledEventArgs e)
            {
                //if(Boiled != null)
                //{
                //    Boiled(this,e);
                //}
                Boiled?.Invoke(this,e);
            }
            public void BoilWater()
            {
                for (int i = 0; i < 100; i++)
                {
                    temperature = i;
                    if (temperature > 95)
                    {
                        BoiledEventArgs e = new BoiledEventArgs(temperature);
                        OnBoiled(e);
                    }
                }
            }
        }
        public class Alarm
        {
            public void MakeAlert(object sender,Heater.BoiledEventArgs e)
            {
                Heater heater = (Heater)sender;
                Console.WriteLine("Alarm:{0}-{1}:", heater.area, heater.type);
                Console.WriteLine("Alarm:嘀嘀嘀,水已经{0}度了", e.temperature);
                Console.WriteLine();
            }
        }
        public class Display
        {
            public static void ShowMsg(object sender,Heater.BoiledEventArgs e)
            {
                Heater heater = (Heater)sender;
                Console.WriteLine("DisPlay:{0} - {1}:", heater.area, heater.type);
                Console.WriteLine("Display:水快烧开了,当前温度:{0}度", e.temperature);
                Console.WriteLine();

            }
        }
        public StudyEvent01()
        {
            InitializeComponent();
            Heater heater = new Heater();
            Alarm alarm = new Alarm();
            heater.Boiled += alarm.MakeAlert;
            //heater.Boiled += (new Alarm()).MakeAlert;
            //heater.Boiled += new Heater.BoiledEventHandler(alarm.MakeAlert);
            heater.Boiled += Display.ShowMsg;
            heater.BoilWater();
        }
    }
}
