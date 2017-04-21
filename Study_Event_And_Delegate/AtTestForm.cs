using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Study_Event_And_Delegate.AT;

namespace Study_Event_And_Delegate
{
    public partial class AtTestForm : Form
    {
        public AtTestForm()
        {
            serialPort1.NewLine = "/r/n";
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            serialPort1.Open();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            
            string[] longstrs = textBox3.Text.Split(new string[] { "/r/n" }, StringSplitOptions.None);
            AtComm[] atComms = new AtComm[longstrs.Length];
            serialPort1.WriteLine("");
        }
    }
}
