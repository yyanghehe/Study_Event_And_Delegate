using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Study_Event_And_Delegate
{
    public partial class TimeOutPractice : Form
    {
        bool isTimeOut;
        public TimeOutPractice()
        {
            var cancelAction = new CancellationTokenSource();
            InitializeComponent();
            //Task task = new Task(TaskMethod);
            //task.Start();

            Task.Factory.StartNew(() =>
            {
                var startTime = DateTime.Now.Ticks;
                while (!cancelAction.IsCancellationRequested)
                {
                    Console.WriteLine("Running in task method");
                    Thread.Sleep(100);
                    if (( DateTime.Now.Ticks- startTime ) / 10000000.0 > 3)
                    {
                        Console.WriteLine("task TimeOut");
                        cancelAction.Cancel();
                    }
                }
            });

            for (int i = 0; i < 10; i++)
            {
                if (!cancelAction.IsCancellationRequested) {
                    Console.WriteLine("Running in main thread..");
                    Thread.Sleep(500);
                }
                else
                {
                    Console.WriteLine("TimeOut");
                    return;
                }

            }

            cancelAction.Cancel();
        }
        void TaskMethod()
        {
            for (int i = 0; i < 10; i++)
            {
                Console.WriteLine("Running in task method,Task ID{0}", i);
                Thread.Sleep(500);
            }
        }
    }
}
