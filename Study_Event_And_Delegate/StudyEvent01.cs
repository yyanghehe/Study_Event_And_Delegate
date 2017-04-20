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
            //public string type = "fasthot 001";
           // public string area = "Chian ChengDu";
            
            public delegate void BoilHandler(int param);
            public event BoilHandler BoilEvent;
            
            public void BoilWater()
            {
                for (int i = 0; i < 100; i++)
                {
                    temperature = i;
                    if (temperature > 95)
                    {
                        if (BoilEvent != null)
                        {
                            BoilEvent(temperature);
                        }
                    }
                }
            }
        }
        public class Alarm
        {
            public void MakeAlert(int param)
            {
                Console.WriteLine("Alarm:嘀嘀嘀,水已经{0}度了:", param);
            }
        }
        public class Display
        {
            public static void ShowMsg(int param)
            {
                Console.WriteLine("Display:水快烧开了,当前温度:{0}度", param);
            }
        }
        public StudyEvent01()
        {
            InitializeComponent();
            Heater heater = new Heater();
            Alarm alarm = new Alarm();
            heater.BoilEvent += alarm.MakeAlert;
            heater.BoilEvent += Display.ShowMsg;
            heater.BoilWater();
        }
    }
}
