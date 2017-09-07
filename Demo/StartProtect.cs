using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace ETCF
{
    class StartProtect
    {
        public static void StartPro()
        {
            //启动相互监控
            System.Timers.Timer ti = new System.Timers.Timer();
            int i_mTime = 1000 * 5;//5秒监测一次
            ti.Interval = i_mTime;
            ti.Elapsed += new System.Timers.ElapsedEventHandler(ThreadMethod);
            ti.AutoReset = true;
            ti.Enabled = true;
            Log.MainStartLog(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "主软件启动" + "\r\n");
        }

        public static void ThreadMethod(object souce, System.Timers.ElapsedEventArgs e)
        {
            string name = "ProtectProcess";
            int processCount = 65535;
            Process[] pro = Process.GetProcesses();
            foreach (Process pr in pro)
            {
                if (name == pr.ProcessName)
                {
                    processCount = 0;
                    return;
                }
            }
            if (processCount != 0)
            {
                //程序已经不在运行了
                try
                {
                    Process mypro = new Process();
                    mypro.StartInfo.FileName = string.Format("{0}\\{1}", AppDomain.CurrentDomain.BaseDirectory, "ProtectProcess.exe");//要启动程序位置
                    mypro.StartInfo.Verb = "Open";
                    mypro.StartInfo.CreateNoWindow = true;
                    mypro.Start();
                    Log.MainStartLog(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "监控软件进程丢失，已启动监控软件" + "\r\n");
                }
                catch (Exception)
                {
                    //do noting
                }
            }
            else
            {
                //程序在运行
            }
        }
    }
}
