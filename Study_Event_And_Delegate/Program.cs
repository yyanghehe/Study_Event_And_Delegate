using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Study_Event_And_Delegate
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            try {
            Application.Run(new AtTestForm());
            }
            catch(Exception e)
            {
                Console.WriteLine(e.TargetSite.DeclaringType.FullName);
            }
        }
    }
}
