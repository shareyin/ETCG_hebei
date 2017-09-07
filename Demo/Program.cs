using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace ETCF
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.ThreadException +=
                    new System.Threading.ThreadExceptionEventHandler(Application_ThreadException);

            AppDomain.CurrentDomain.UnhandledException +=
                new System.UnhandledExceptionEventHandler(AppDomain_UnHandledException);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FormDemo());
        }
        private static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            Log.WriteLog(DateTime.Now + " 软件异常\r\n" + e.ToString() + "\r\n");
        }
        private static void AppDomain_UnHandledException(object sender, System.UnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject is System.Exception)
            {
                Log.WriteLog(DateTime.Now + " 系统异常\r\n" + e.ToString() + "\r\n");
            }
        }
    }
}
