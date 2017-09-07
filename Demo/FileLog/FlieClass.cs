using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace ETCF
{
    public class FlieClass
    {
        private void CreateDir(string dirPath)
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

        private int CheckWawWj(string dirPathWaw, string dirPathWj)
        {
            if (File.Exists(dirPathWaw))
            {
                return 1;
            }
            else if (File.Exists(dirPathWj))
            {
                return 2;
            }
            return 0;
        }

        //--------------------------------------
        // 写图片函数
        // INT sDirPath：路径
        // INT FlieName：文件名，带后缀，如pic.png
        // INT Msg：文件缓存
        // INT startindex：文件缓存起始位置
        // INT len：存入总长度
        // OUT true：成功
        // OUT false：失败
        //---------------------------------------
        public bool WriteFileImage(string sDirPath, string FlieName, byte[] Msg, int startindex, Int32 len)
        {
            CreateDir(sDirPath);
            string sFilePath = sDirPath + FlieName;
            if (File.Exists(sFilePath))
            {
                try
                {
                    File.Delete(sFilePath);
                }
                catch (System.Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }


            try
            {
                byte[] Msg2 = new byte[len - startindex];
                int index = 0;
                for (int i = startindex; i < len; i++)
                {
                    Msg2[index++] = Msg[i];
                }

                FileStream sw = null;
                using (sw = File.OpenWrite(sFilePath))
                {
                    sw.Position = sw.Length;

                    sw.Write(Msg2, 0, Msg2.Length);
                    sw.Flush();
                    sw.Dispose();
                }

            }
            catch
            {
                return false;
            }
            return true;
        }

        
    }
}
