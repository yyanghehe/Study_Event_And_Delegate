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
    public partial class Form1 : Form
    {
        public Form1()
        {
            //InitializeComponent();
            GreetDelegate delegate1 =new GreetDelegate(EnglishGreet);
            GreetPeople("xX", EnglishGreet);
            GreetPeople("xx", delegate1);
        }

        public delegate void GreetDelegate(string name);
        public void GreetPeople(string name, GreetDelegate MakeGreet) {
             MakeGreet(name);
        }
        public void EnglishGreet(string name) {
            Console.WriteLine("Good Moring " + name);
        }
        public void ChineseGreet(string name) {
            Console.WriteLine("早上好" + name);
        }
    }
}
