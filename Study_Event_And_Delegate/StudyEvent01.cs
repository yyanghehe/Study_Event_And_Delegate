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
        private int temperature;
        public delegate void BoilHandler(int param);
        public event BoilHandler BoilEvent;

        public void BoilWater()
        {
            for (int i=0; i < 100; i++)
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

        public StudyEvent01()
        {
            InitializeComponent();
        }
    }
}
