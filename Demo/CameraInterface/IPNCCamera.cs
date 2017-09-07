using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace ETCF
{
    class IPNCCamera
    {
        #region 全局变量
        public FormDemo MF = null;

        public static string GetPlateNo = "未检测";
        public static string imagepath = "未知";
        public static string PlateColor = "未知";
        public static string GetVehicleLogoRecog = "";
        public static bool IPNCConnState = false;

        public IPNCSDK IPNC_Camera;
        #endregion

        public IPNCCamera(FormDemo mf)
        {
            if (MF == null)
            {
                MF = mf;
            }
        }

        #region ******摄像机处理流程******
        public void initCamera(string IPNCCameraip)
        {
            int res = 0;
            if (IPNC_Camera == null)
            {
                IPNC_Camera = new IPNCSDK();
                IPNC_Camera.pCaremaCallBack = CaremaCall;//传人mainform中的回调函数
            }
            bool m_bInitSDK = IPNC_Camera.Initialize();
            if (m_bInitSDK == true)
            {
                try
                {
                    res = IPNC_Camera.ConnectCamera(IPNCCameraip);
                }
                catch (Exception e)
                {
                    MF.AddOperLogCacheStr(e.ToString());
                }
                if (res == 0)
                {
                    MF.AddOperLogCacheStr("摄像头连接成功！");
                    IPNCConnState = true;
                }
                else
                {
                    MF.AddOperLogCacheStr("摄像头连接失败！");
                    IPNCConnState = false;
                }
            }
            else
            {
                MF.AddOperLogCacheStr("Initialize返回-1,摄像头连接失败！");
                IPNCConnState = false;
            }
        }
        //摄像机回调函数
        public void CaremaCall(IntPtr buf, int len, IntPtr PlateNo, ref byte PlateColor, IntPtr PlateBrand, int vehtype, int res)
        {
            try
            {
                MF.CameraCanpost.WaitOne(1000);
                MF.AddOperLogCacheStr("摄像机进入回调");
                FlieClass fc = new FlieClass();
                byte[] VehImage = new byte[len];
                string sPlate = "";
                string sPlateBrand = "";
                Marshal.Copy(buf, VehImage, 0, len);
                byte[] l_PlateNo = new byte[16];
                Marshal.Copy(PlateNo, l_PlateNo, 0, 16);
                if (res == 0)
                {
                    sPlate = Encoding.Default.GetString(l_PlateNo);
                    sPlate = sPlate.Substring(0, sPlate.IndexOf("\0"));
                }
                else
                {
                    sPlate = "无车牌";
                }
                if (sPlate == "")
                {
                    sPlate = "无牌车";
                }
                byte[] l_PlateBrand = new byte[12];
                Marshal.Copy(PlateBrand, l_PlateBrand, 0, 12);
                sPlateBrand = Encoding.Default.GetString(l_PlateBrand);
                sPlateBrand = sPlateBrand.Substring(0, sPlateBrand.IndexOf("\0"));
                GetVehicleLogoRecog = sPlateBrand;
                GetPlateNo = sPlate;
                MF.AddOperLogCacheStr("车牌;" + GetPlateNo);
                string dirpath = ".\\plateimage\\";
                DateTime forcetimedt = DateTime.Now;
                string forcetime = forcetimedt.ToString("yyyyMMddHHmmss");
                string imagename = forcetime + sPlate + ".jpg";
                dirpath += DateTime.Now.Year.ToString();
                dirpath += "年\\";
                dirpath += DateTime.Now.Month.ToString();
                dirpath += "月\\";
                dirpath += DateTime.Now.Day.ToString();
                dirpath += "日\\";
                imagepath = dirpath + imagename;
                //string imagepath = dirpath + imagename + "车型" + vehtype.ToString();
                if (true == fc.WriteFileImage(dirpath, imagename, VehImage, 0, len))
                {
                    MF.CameraPicture.Set();
                    MF.AddOperLogCacheStr("保存车牌图片成功!");
                }
                else
                {
                    MF.AddOperLogCacheStr("保存车牌图片失败!");
                }
            }
            catch (Exception ex)
            {
                MF.AddOperLogCacheStr("保存车牌图片失败!");
            }
        }
        #endregion
    }
}
