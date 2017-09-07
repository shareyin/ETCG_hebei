using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;

namespace ETCF
{
    class IPCCamera
    {
        public FormDemo MF = null;
        private ipcsdk.ICE_IPCSDK_OnPlate onPlate;
        private ipcsdk.ICE_IPCSDK_OnFrame_Planar onFrame;
        private IntPtr[] pUid = new IntPtr[4] { IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero };

        public static string GetPlateNo = "未检测";
        public static string imagepath = "未知";
        public static string PlateColor = "未知";
        public static string GetVehicleLogoRecog = "";
        public static bool IPCConnState = false;

         public IPCCamera(FormDemo mf)
        {
            if (MF == null)
            {
                MF = mf;
            }
        }

         public void initIPC(string IPCCameraip)
         {
             ipcsdk.ICE_IPCSDK_Init(); //调用全局初始化
             onFrame = new ipcsdk.ICE_IPCSDK_OnFrame_Planar(SDK_OnFrame);
             onPlate = new ipcsdk.ICE_IPCSDK_OnPlate(SDK_OnPlate);
             //调用不带密码的接口连接相机
             pUid[0] = ipcsdk.ICE_IPCSDK_Open(IPCCameraip, 1, 554, 8117, 8080, 1, 0, IntPtr.Zero, 0, IntPtr.Zero);
             if (pUid[0] == IntPtr.Zero)
             {
                 IPCConnState = false;
                 MF.AddOperLogCacheStr("相机连接失败"+IPCCameraip);
                 //MessageBox.Show("相机1连接失败！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                 return;
             }
             IPCConnState = true;
             MF.AddOperLogCacheStr("相机连接成功");
             ipcsdk.ICE_IPCSDK_SetFrameCallback(pUid[0], null, new IntPtr(0));//设置获得解码出的一帧图像的相关回调函数
             ipcsdk.ICE_IPCSDK_SetPlateCallback(pUid[0], onPlate, new IntPtr(0));//设置获取车牌识别数据的回调函数
         }
         public ICE_VDC_PICTRUE_INFO_S vdcInfo = new ICE_VDC_PICTRUE_INFO_S();
         public ICE_VBR_RESULT_S vbrResult = new ICE_VBR_RESULT_S();
         public void on_plate(string bstrIP, string bstrNumber, string bstrColor, IntPtr vPicData, UInt32 nPicLen,
              IntPtr vCloseUpPicData, UInt32 nCloseUpPicLen, short nSpeed, short nVehicleType, short nReserved1, short nReserved2, Single fPlateConfidence,
              UInt32 nVehicleColor, UInt32 nPlateType, UInt32 nVehicleDir, UInt32 nAlarmType, UInt32 nCapTime, Int32 index, uint u32ResultHigh, uint u32ResultLow)
         {
             MF.CameraCanpost.WaitOne(1000);

#if VERSION32
            IntPtr vdcPtr = (IntPtr)u32ResultLow;
#else
             ulong tmp = ((ulong)u32ResultHigh << 32) + (ulong)u32ResultLow;
             IntPtr vdcPtr = (IntPtr)tmp;
#endif

             if (vdcPtr != IntPtr.Zero)
             {
                 //将数据拷贝到ICE_VDC_PICTRUE_INFO_S结构体
                 vdcInfo = (ICE_VDC_PICTRUE_INFO_S)Marshal.PtrToStructure(vdcPtr, typeof(ICE_VDC_PICTRUE_INFO_S));

                 //获得车款结构体指针，并拷贝
                 if (vdcInfo.pstVbrResult != IntPtr.Zero)
                 {
                     vbrResult = (ICE_VBR_RESULT_S)Marshal.PtrToStructure(vdcInfo.pstVbrResult, typeof(ICE_VBR_RESULT_S));
                     if (vbrResult.szLogName.Length == 0)
                         vbrResult.szLogName = "未知";
                     //委托，用于显示识别数据(showCount)
                     //this.BeginInvoke(updatePlateInfo, bstrIP, bstrNumber, bstrColor,
                     //    nVehicleColor, nAlarmType, nVehicleType, nCapTime, index, vbrResult.szLogName);
                 }
                 //else
                 //this.BeginInvoke(updatePlateInfo, bstrIP, bstrNumber, bstrColor,
                 //    nVehicleColor, nAlarmType, nVehicleType, nCapTime, index, "");//委托，用于显示识别数据(showCount)
             }
             //else
             //this.BeginInvoke(updatePlateInfo, bstrIP, bstrNumber, bstrColor,
             //    nVehicleColor, nAlarmType, nVehicleType, nCapTime, index, "");//委托，用于显示识别数据(showCount)
             PlateColor = bstrColor;
             GetPlateNo = bstrNumber;
             if (bstrNumber.Equals(""))
             {
                 GetPlateNo = "无牌车";
             }
             if (nPicLen > 0)//全景图数据长度不为0
             {
                 IntPtr ptr2 = (IntPtr)vPicData;
                 byte[] datajpg2 = new byte[nPicLen];
                 Marshal.Copy(ptr2, datajpg2, 0, datajpg2.Length);//拷贝图片数据
                 //存图
                 if (vdcInfo.pstVbrResult != IntPtr.Zero)
                     storePic(datajpg2, bstrIP, bstrNumber, false, nCapTime, vbrResult.fResFeature, vbrResult.szLogName);
                 else
                     storePic(datajpg2, bstrIP, bstrNumber, false, nCapTime, null, "");
             }
             MF.AddOperLogCacheStr("摄像机车牌：" + GetPlateNo);
             MF.CameraPicture.Set();

             //if (nCloseUpPicLen > 0)//车牌图数据长度不为0
             //{
             //    IntPtr ptr = (IntPtr)vCloseUpPicData;
             //    byte[] datajpg = new byte[nCloseUpPicLen];
             //    Marshal.Copy(ptr, datajpg, 0, datajpg.Length);//拷贝图片数据
             //    //存图
             //    if (vdcInfo.pstVbrResult != IntPtr.Zero)
             //        storePic(datajpg, bstrIP, bstrNumber, true, nCapTime, null, vbrResult.szLogName);
             //    else
             //        storePic(datajpg, bstrIP, bstrNumber, true, nCapTime, null, "");
             //}
         }

         //存图
         public void storePic(byte[] picData, string strIP, string strNumber, bool bIsPlate, UInt32 nCapTime, float[] fResFuture, string strLogName)
         {
             DateTime dt = new DateTime();
             if (nCapTime == 0)
             {
                 dt = DateTime.Now;
             }
             else
             {
                 dt = DateTime.Parse("1970-01-01 08:00:00").AddSeconds(nCapTime);
             }

             string strDir = ".\\plateimage" + @"\" + dt.ToString("yyyyMMdd");
             if (!Directory.Exists(strDir))
             {
                 Directory.CreateDirectory(strDir);
             }

             string strPicName;
             if (strLogName.Length != 0)
                 strPicName = strDir + @"\" + dt.ToString("yyyyMMddHHmmss") + "_" + strLogName + "_" + strNumber;
             else
                 strPicName = strDir + @"\" + dt.ToString("yyyyMMddHHmmss") + "_" + strNumber;
             if (bIsPlate)//车牌图，图片名后缀加_plate
                 strPicName += "_plate";
             //string tmp = strPicName;
             strPicName += ".jpg";
             if (File.Exists(strPicName))//如果图片名存在，则在文件名末尾加数字以分辨，如XXX_1.jpg;XXX_2.jpg
             {
                 int count = 1;
                 while (count <= 10)
                 {
                     strPicName = strDir + @"\" + dt.ToString("yyyyMMddHHmmss") + "_" + strNumber;
                     if (bIsPlate)
                     {
                         strPicName += "_plate";
                     }
                     strPicName += "_" + count.ToString() + ".jpg";

                     if (!File.Exists(strPicName))
                     {
                         break;
                     }
                     count++;
                 }
             }
             //存图
             try
             {
                 imagepath = strPicName;
                 FileStream fs = new FileStream(strPicName, FileMode.Create, FileAccess.Write);
                 BinaryWriter bw = new BinaryWriter(fs);
                 bw.Write(picData);
                 bw.Close();
                 fs.Close();
             }
             catch (System.Exception ex)
             {

             }

             //保存特征码
             if (bIsPlate || null == fResFuture)
                 return;

             string strFileName = strDir + @"\" + "vbr_record.txt";

             string strContent = "";
             //将车辆特征码存到字符串中
             for (int i = 0; i < 20; i++)
             {
                 if (i != 0)
                     strContent += " ";
                 strContent += fResFuture[i].ToString("0.000000");
             }
             //将车辆特征码数据追加到文件中
             try
             {
                 StreamWriter sw = new StreamWriter(strFileName, true, Encoding.Unicode);
                 if (null != sw)
                 {
                     strContent = dt.ToString("yyyyMMddHHmmss") + "_" + strLogName + "_" + strNumber + ".jpg" + " " + strNumber + " " + strContent + "\r\n";
                     sw.Write(strContent);
                     sw.Close();
                 }
             }
             catch (System.Exception ex)
             {

             }

         }

         //实时抓拍
         public void SDK_OnPlate(System.IntPtr pvParam,
                     [System.Runtime.InteropServices.InAttribute()] [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.LPStr)] string pcIP,
                     [System.Runtime.InteropServices.InAttribute()] [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.LPStr)] string pcNumber,
                     [System.Runtime.InteropServices.InAttribute()] [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.LPStr)] string pcColor,
                     System.IntPtr pcPicData, uint u32PicLen, System.IntPtr pcCloseUpPicData, uint u32CloseUpPicLen,
                     short nSpeed, short nVehicleType, short nReserved1, short nReserved2,
                     float fPlateConfidence, uint u32VehicleColor, uint u32PlateType, uint u32VehicleDir, uint u32AlarmType,
                     uint u32SerialNum, uint uCapTime, uint u32ResultHigh, uint u32ResultLow)
         {
             int index = (int)pvParam;
             on_plate(pcIP, pcNumber, pcColor, pcPicData, u32PicLen, pcCloseUpPicData, u32CloseUpPicLen,
                 nSpeed, nVehicleType, nReserved1, nReserved2, fPlateConfidence,
                 u32VehicleColor, u32PlateType, u32VehicleDir, u32AlarmType, 0, index, u32ResultHigh, u32ResultLow);
         }

         private int[] frame_count = new int[4] { 0, 0, 0, 0 };
         private string m_strStorePath = "D:\\";
         public void on_frame(UInt32 u32Timestamp,
             IntPtr pu8DataY, IntPtr pu8DataU, IntPtr pu8DataV,
             Int32 s32LinesizeY, Int32 s32LinesizeU, Int32 s32LinesizeV,
             Int32 s32Width, Int32 s32Height, Int32 i)
         {
             string strDir = m_strStorePath + @"抓拍_C#Frame\" + i.ToString() + @"\" + DateTime.Now.ToString("yyyyMMdd");
             if (!Directory.Exists(strDir))
             {
                 Directory.CreateDirectory(strDir);
             }

             string strPicName = strDir + @"\" + "test.bmp";

             if (0 == (frame_count[i] % 30))
             {

                 try
                 {
                     //拷贝数据
                     byte[] datay = new byte[s32Width * s32Height];
                     for (int j = 0; j < s32Height; j++)
                         Marshal.Copy((IntPtr)pu8DataY + j * s32LinesizeY, datay, j * s32Width, s32Width);

                     byte[] datau = new byte[s32Width * s32Height / 4];
                     for (int j = 0; j < s32Height / 2; j++)
                         Marshal.Copy((IntPtr)pu8DataU + j * s32LinesizeU, datau, j * s32Width / 2, s32Width / 2);

                     byte[] datav = new byte[s32Width * s32Height / 4];
                     for (int j = 0; j < s32Height / 2; j++)
                         Marshal.Copy((IntPtr)pu8DataV + j * s32LinesizeV, datav, j * s32Width / 2, s32Width / 2);

                     byte[] rgb24 = new byte[s32Width * s32Height * 3];

                     util.Convert(s32Width, s32Height, datay, datau, datav, ref rgb24);
                     //存图
                     FileStream fs = new FileStream(strPicName, FileMode.Create, FileAccess.Write);
                     BinaryWriter bw = new BinaryWriter(fs);

                     bw.Write('B');
                     bw.Write('M');
                     bw.Write(rgb24.Length + 54);
                     bw.Write(0);
                     bw.Write(54);
                     bw.Write(40);
                     bw.Write(s32Width);
                     bw.Write(s32Height);
                     bw.Write((ushort)1);
                     bw.Write((ushort)24);
                     bw.Write(0);
                     bw.Write(rgb24.Length);
                     bw.Write(0);
                     bw.Write(0);
                     bw.Write(0);
                     bw.Write(0);

                     bw.Write(rgb24, 0, rgb24.Length);
                     bw.Close();
                     fs.Close();

                 }
                 catch (System.Exception ex)
                 {
                     //MessageBox.Show("frame" + ex.Message);
                 }

                 frame_count[i] = 0;
             }
             frame_count[i]++;
         }

         public class util
         {
             private static int width;
             private static int height;
             private static int length;
             private static int v;  //v值的起始位置
             private static int u;  //u值的起始位置
             private static int rdif, invgdif, bdif;
             private static int[] Table_fv1;
             private static int[] Table_fv2;
             private static int[] Table_fu1;
             private static int[] Table_fu2;
             private static int[] rgb = new int[3];
             private static int m, n, i, j, hfWidth;
             private static bool addHalf = true;
             private static int py;
             private static int pos, pos1;//dopod 595 图像调整
             private static byte temp;

             public static void YV12ToRGB(int iWidth, int iHeight)
             {
                 Table_fv1 = new int[256] { -180, -179, -177, -176, -174, -173, -172, -170, -169, -167, -166, -165, -163, -162, -160, -159, -158, -156, -155, -153, -152, -151, -149, -148, -146, -145, -144, -142, -141, -139, -138, -137, -135, -134, -132, -131, -130, -128, -127, -125, -124, -123, -121, -120, -118, -117, -115, -114, -113, -111, -110, -108, -107, -106, -104, -103, -101, -100, -99, -97, -96, -94, -93, -92, -90, -89, -87, -86, -85, -83, -82, -80, -79, -78, -76, -75, -73, -72, -71, -69, -68, -66, -65, -64, -62, -61, -59, -58, -57, -55, -54, -52, -51, -50, -48, -47, -45, -44, -43, -41, -40, -38, -37, -36, -34, -33, -31, -30, -29, -27, -26, -24, -23, -22, -20, -19, -17, -16, -15, -13, -12, -10, -9, -8, -6, -5, -3, -2, 0, 1, 2, 4, 5, 7, 8, 9, 11, 12, 14, 15, 16, 18, 19, 21, 22, 23, 25, 26, 28, 29, 30, 32, 33, 35, 36, 37, 39, 40, 42, 43, 44, 46, 47, 49, 50, 51, 53, 54, 56, 57, 58, 60, 61, 63, 64, 65, 67, 68, 70, 71, 72, 74, 75, 77, 78, 79, 81, 82, 84, 85, 86, 88, 89, 91, 92, 93, 95, 96, 98, 99, 100, 102, 103, 105, 106, 107, 109, 110, 112, 113, 114, 116, 117, 119, 120, 122, 123, 124, 126, 127, 129, 130, 131, 133, 134, 136, 137, 138, 140, 141, 143, 144, 145, 147, 148, 150, 151, 152, 154, 155, 157, 158, 159, 161, 162, 164, 165, 166, 168, 169, 171, 172, 173, 175, 176, 178 };
                 Table_fv2 = new int[256] { -92, -91, -91, -90, -89, -88, -88, -87, -86, -86, -85, -84, -83, -83, -82, -81, -81, -80, -79, -78, -78, -77, -76, -76, -75, -74, -73, -73, -72, -71, -71, -70, -69, -68, -68, -67, -66, -66, -65, -64, -63, -63, -62, -61, -61, -60, -59, -58, -58, -57, -56, -56, -55, -54, -53, -53, -52, -51, -51, -50, -49, -48, -48, -47, -46, -46, -45, -44, -43, -43, -42, -41, -41, -40, -39, -38, -38, -37, -36, -36, -35, -34, -33, -33, -32, -31, -31, -30, -29, -28, -28, -27, -26, -26, -25, -24, -23, -23, -22, -21, -21, -20, -19, -18, -18, -17, -16, -16, -15, -14, -13, -13, -12, -11, -11, -10, -9, -8, -8, -7, -6, -6, -5, -4, -3, -3, -2, -1, 0, 0, 1, 2, 2, 3, 4, 5, 5, 6, 7, 7, 8, 9, 10, 10, 11, 12, 12, 13, 14, 15, 15, 16, 17, 17, 18, 19, 20, 20, 21, 22, 22, 23, 24, 25, 25, 26, 27, 27, 28, 29, 30, 30, 31, 32, 32, 33, 34, 35, 35, 36, 37, 37, 38, 39, 40, 40, 41, 42, 42, 43, 44, 45, 45, 46, 47, 47, 48, 49, 50, 50, 51, 52, 52, 53, 54, 55, 55, 56, 57, 57, 58, 59, 60, 60, 61, 62, 62, 63, 64, 65, 65, 66, 67, 67, 68, 69, 70, 70, 71, 72, 72, 73, 74, 75, 75, 76, 77, 77, 78, 79, 80, 80, 81, 82, 82, 83, 84, 85, 85, 86, 87, 87, 88, 89, 90, 90 };
                 Table_fu1 = new int[256] { -44, -44, -44, -43, -43, -43, -42, -42, -42, -41, -41, -41, -40, -40, -40, -39, -39, -39, -38, -38, -38, -37, -37, -37, -36, -36, -36, -35, -35, -35, -34, -34, -33, -33, -33, -32, -32, -32, -31, -31, -31, -30, -30, -30, -29, -29, -29, -28, -28, -28, -27, -27, -27, -26, -26, -26, -25, -25, -25, -24, -24, -24, -23, -23, -22, -22, -22, -21, -21, -21, -20, -20, -20, -19, -19, -19, -18, -18, -18, -17, -17, -17, -16, -16, -16, -15, -15, -15, -14, -14, -14, -13, -13, -13, -12, -12, -11, -11, -11, -10, -10, -10, -9, -9, -9, -8, -8, -8, -7, -7, -7, -6, -6, -6, -5, -5, -5, -4, -4, -4, -3, -3, -3, -2, -2, -2, -1, -1, 0, 0, 0, 1, 1, 1, 2, 2, 2, 3, 3, 3, 4, 4, 4, 5, 5, 5, 6, 6, 6, 7, 7, 7, 8, 8, 8, 9, 9, 9, 10, 10, 11, 11, 11, 12, 12, 12, 13, 13, 13, 14, 14, 14, 15, 15, 15, 16, 16, 16, 17, 17, 17, 18, 18, 18, 19, 19, 19, 20, 20, 20, 21, 21, 22, 22, 22, 23, 23, 23, 24, 24, 24, 25, 25, 25, 26, 26, 26, 27, 27, 27, 28, 28, 28, 29, 29, 29, 30, 30, 30, 31, 31, 31, 32, 32, 33, 33, 33, 34, 34, 34, 35, 35, 35, 36, 36, 36, 37, 37, 37, 38, 38, 38, 39, 39, 39, 40, 40, 40, 41, 41, 41, 42, 42, 42, 43, 43 };
                 Table_fu2 = new int[256] { -227, -226, -224, -222, -220, -219, -217, -215, -213, -212, -210, -208, -206, -204, -203, -201, -199, -197, -196, -194, -192, -190, -188, -187, -185, -183, -181, -180, -178, -176, -174, -173, -171, -169, -167, -165, -164, -162, -160, -158, -157, -155, -153, -151, -149, -148, -146, -144, -142, -141, -139, -137, -135, -134, -132, -130, -128, -126, -125, -123, -121, -119, -118, -116, -114, -112, -110, -109, -107, -105, -103, -102, -100, -98, -96, -94, -93, -91, -89, -87, -86, -84, -82, -80, -79, -77, -75, -73, -71, -70, -68, -66, -64, -63, -61, -59, -57, -55, -54, -52, -50, -48, -47, -45, -43, -41, -40, -38, -36, -34, -32, -31, -29, -27, -25, -24, -22, -20, -18, -16, -15, -13, -11, -9, -8, -6, -4, -2, 0, 1, 3, 5, 7, 8, 10, 12, 14, 15, 17, 19, 21, 23, 24, 26, 28, 30, 31, 33, 35, 37, 39, 40, 42, 44, 46, 47, 49, 51, 53, 54, 56, 58, 60, 62, 63, 65, 67, 69, 70, 72, 74, 76, 78, 79, 81, 83, 85, 86, 88, 90, 92, 93, 95, 97, 99, 101, 102, 104, 106, 108, 109, 111, 113, 115, 117, 118, 120, 122, 124, 125, 127, 129, 131, 133, 134, 136, 138, 140, 141, 143, 145, 147, 148, 150, 152, 154, 156, 157, 159, 161, 163, 164, 166, 168, 170, 172, 173, 175, 177, 179, 180, 182, 184, 186, 187, 189, 191, 193, 195, 196, 198, 200, 202, 203, 205, 207, 209, 211, 212, 214, 216, 218, 219, 221, 223, 225 };
                 width = iWidth;
                 height = iHeight;
                 length = iWidth * iHeight;
                 v = length;//nYLen
                 u = v + (length >> 2);
                 hfWidth = iWidth >> 1;
                 addHalf = true;
             }

             public static bool Convert(int cwidth, int cheight, byte[] yv12y, byte[] yv12u, byte[] yv12v, ref  byte[] rgb24)
             {
                 try
                 {
                     YV12ToRGB(cwidth, cheight);
                     if (yv12y.Length == 0 || rgb24.Length == 0)
                         return false;
                     m = -width;
                     n = -hfWidth;
                     for (int y = 0; y < height; y++)
                     {
                         if (y == 139)
                         {
                         }
                         m += width;
                         if (addHalf)
                         {
                             n += hfWidth;
                             addHalf = false;
                         }
                         else
                         {
                             addHalf = true;
                         }
                         for (int x = 0; x < width; x++)
                         {
                             i = m + x;
                             j = n + (x >> 1);
                             py = (int)yv12y[i];
                             rdif = Table_fv1[(int)yv12v[j]];
                             invgdif = Table_fu1[(int)yv12u[j]] + Table_fv2[(int)yv12v[j]];
                             bdif = Table_fu2[(int)yv12u[j]];

                             rgb[2] = py + rdif;//R
                             rgb[1] = py - invgdif;//G
                             rgb[0] = py + bdif;//B

                             j = v - width - m + x;
                             i = (j << 1) + j;

                             // copy this pixel to rgb data
                             for (j = 0; j < 3; j++)
                             {

                                 if (rgb[j] >= 0 && rgb[j] <= 255)
                                 {
                                     rgb24[i + j] = (byte)rgb[j];
                                 }
                                 else
                                 {
                                     rgb24[i + j] = (byte)((rgb[j] < 0) ? 0 : 255);
                                 }

                             }
                             if (x % 4 == 3)
                             {
                                 pos = (m + x - 1) * 3;
                                 pos1 = (m + x) * 3;
                                 temp = rgb24[pos];
                                 rgb24[pos] = rgb24[pos1];
                                 rgb24[pos1] = temp;

                                 temp = rgb24[pos + 1];
                                 rgb24[pos + 1] = rgb24[pos1 + 1];
                                 rgb24[pos1 + 1] = temp;

                                 temp = rgb24[pos + 2];
                                 rgb24[pos + 2] = rgb24[pos1 + 2];
                                 rgb24[pos1 + 2] = temp;
                             }
                         }
                     }
                 }
                 catch (Exception e)
                 {

                     //MessageBox.Show(e.Message);
                 }
                 return true;
             }
         }

         public void SDK_OnFrame(System.IntPtr pvParam, uint u32Timestamp, System.IntPtr pu8DataY,
             System.IntPtr pu8DataU, System.IntPtr pu8DataV, int s32LinesizeY, int s32LinesizeU,
             int s32LinesizeV, int s32Width, int s32Height)
         {
             int index = (int)pvParam;
             //if (m_bExit || bClose[index])
             //    return;
             on_frame(u32Timestamp, pu8DataY, pu8DataU, pu8DataV, s32LinesizeY,
                 s32LinesizeU, s32LinesizeV, s32Width, s32Height, index);
         }
    }
}
