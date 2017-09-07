using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace ETCF
{
    public class Log
    {
        public static void CreateDir(string dirPath)
        {
            if (!Directory.Exists(dirPath))
            {
                try
                {
                    Directory.CreateDirectory(dirPath);
                }
                catch { }
            }
        }
        public static void MainStartLog(string Msg)
        {
            string sDirPath = ".\\MainStartLog\\";
            CreateDir(sDirPath);
            string sFilePath = sDirPath + DateTime.Now.ToString("yyyy-MM-dd") + "MainStart.log.ini";
            StreamWriter sw = null;
            try
            {
                using (sw = File.AppendText(sFilePath))
                {
                    sw.WriteLine(Msg);
                    sw.Flush();

                }
            }
            catch { }
            finally
            {
                if (sw != null)
                {
                    sw.Dispose();
                }
            }
        }

        public static void WriteWhenStart()
        {
            string sDirPath = ".\\startlog\\";
            CreateDir(sDirPath);
            string sFilePath = sDirPath + "startlog.log.ini";
            try
            {
                using (StreamWriter sw = File.AppendText(sFilePath))
                {
                    sw.Write("soft start when "+DateTime.Now.ToString("yyyyMMddHHmmss"+"\r\n"));
                    sw.Flush();
                    sw.Dispose();
                }
            }
            catch { }    
        }

        public static void WriteLog(string Msg)
        {
            string sDirPath = ".\\log\\";
            CreateDir(sDirPath);
            string sFilePath = sDirPath + DateTime.Now.ToString("yyyy-MM-dd") + ".log";
            try
            {
                using (StreamWriter sw = File.AppendText(sFilePath))
                {
                    sw.Write(Msg);
                    sw.Flush();
                    sw.Dispose();
                }
            }
            catch { }
        }

        public static void WriteVehLog(string Msg, string sDirPath,string sname)
        {
            CreateDir(sDirPath);
            string sFilePath = sDirPath + sname;
            try
            {
                using (StreamWriter sw = File.AppendText(sFilePath))
                {
                    sw.Write(Msg);
                    sw.Flush();
                    sw.Dispose();
                }
            }
            catch { }
        }

        public static void WritePlateLog(string Msg)
        {
            string sDirPath = ".\\Platelog\\";
            CreateDir(sDirPath);
            string sFilePath = sDirPath + DateTime.Now.ToString("yyyy-MM-dd") + ".log.ini";
            //StreamWriter sw = null;
            try
            {
                using (StreamWriter sw = File.AppendText(sFilePath))
                {
                    sw.WriteLine(Msg);
                    sw.Flush();
                    sw.Dispose();

                }
            }
            catch { }
            //finally
            //{
            //    if (sw != null)
            //    {
            //        sw.Dispose();
            //    }
            //}
        }

        public void WriteLogUnStatic(string Msg)
        {
            string sDirPath = ".\\log\\";
            CreateDir(sDirPath);
            string sFilePath = sDirPath + DateTime.Now.ToString("yyyy-MM-dd") + ".log";
            try
            {
                using (StreamWriter sw = File.AppendText(sFilePath))
                {
                    sw.Write(Msg);
                    sw.Flush();
                    sw.Dispose();
                }
            }
            catch { }
        }

        public static void WriteLog2(string ModuleName, string ErrorType, string Msg)
        {
            string sDirPath = ".\\log\\";
            CreateDir(sDirPath);
            string sFilePath = sDirPath + DateTime.Now.ToString("yyyy-MM-dd") + ".log";
            try
            {
                using (StreamWriter sw = File.AppendText(sFilePath))
                {
                    sw.WriteLine(DateTime.Now.ToString() + "->" + ModuleName + "\t" + ErrorType + ":  " + Msg);
                    sw.Flush();
                    sw.Dispose();
                }
            }
            catch { }
        }

        public static void DeleteDir(string dir)
        {
            try
            {
                if (Directory.Exists(dir))
                {
                    foreach (string d in Directory.GetFileSystemEntries(dir))
                    {
                        if (File.Exists(d))
                            File.Delete(d);
                        else
                            DeleteDir(d);
                    }
                    Directory.Delete(dir);
                }
            }
            catch { }
        }
    }
}
