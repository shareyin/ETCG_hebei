using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace ETCF
{
    class HKCamera
    {
        #region 全局变量
        public FormDemo MF = null;

        private Int32 m_lUserID = -1;
        private uint iLastErr = 0;
        private string strErr;
        private int iDeviceNumber = 0; //添加设备个数
        private Int32 m_lAlarmHandle;
        private Int32 iListenHandle = -1;

        private CHCNetSDK.MSGCallBack m_falarmData = null;

        public static string GetPlateNo = "未检测";
        public static string imagepath = "未知";
        public static string GetVehicleLogoRecog = "";
        public static bool HKConnState = false;
        #endregion

        public HKCamera(FormDemo mf)
        {
            if (MF == null)
            {
                MF = mf;
            }
        }

        #region ******摄像机处理流程******
        //摄像头初始化
        public void initHK(string HKCameraip, string HKCameraUsername, string HKCameraPassword)
        {
            int res = 0;
            bool m_bInitSDK = CHCNetSDK.NET_DVR_Init();
            if (m_bInitSDK == true)
            {
                m_falarmData = new CHCNetSDK.MSGCallBack(MsgCallback);
                bool btemp = CHCNetSDK.NET_DVR_SetDVRMessageCallBack_V30(m_falarmData, IntPtr.Zero);
                if (btemp != true)
                {
                    MF.AddOperLogCacheStr("SetDVRMessageCallBack_V30返回失败");
                    return;
                }
                res = camera_Login(HKCameraip,HKCameraUsername,HKCameraPassword);
                if (res != 0)
                {
                    MF.AddOperLogCacheStr("摄像头登录失败");
                    return;
                }
                res = camera_SetAlarm();
                if (res != 0)
                {
                    MF.AddOperLogCacheStr("摄像头布防失败");
                    return;
                }
                res = camera_StartListen();
                if (res != 0)
                {
                    MF.AddOperLogCacheStr("摄像头启动监听失败");
                    return;
                }
                HKConnState = true;
                MF.AddOperLogCacheStr("Initialize返回0,摄像头连接成功！");
            }
            else
            {
                MF.AddOperLogCacheStr("Initialize返回-1,摄像头连接失败！");
            }
        }

        public int camera_Login(string HKCameraip,string HKCameraUsername,string HKCameraPassword)
        {
            string DVRIPAddress = HKCameraip;//设备IP地址或者域名 Device IP
            Int16 DVRPortNumber = Int16.Parse("8000");//设备服务端口号 Device Port
            string DVRUserName = HKCameraUsername;//设备登录用户名 User name to login
            string DVRPassword = HKCameraPassword;//设备登录密码 Password to login
            CHCNetSDK.NET_DVR_DEVICEINFO_V30 DeviceInfo = new CHCNetSDK.NET_DVR_DEVICEINFO_V30();

            //登录设备 Login the device
            m_lUserID = CHCNetSDK.NET_DVR_Login_V30(DVRIPAddress, DVRPortNumber, DVRUserName, DVRPassword, ref DeviceInfo);
            if (m_lUserID < 0)
            {
                iLastErr = CHCNetSDK.NET_DVR_GetLastError();
                strErr = "NET_DVR_Login_V30 failed, error code= " + iLastErr; //登录失败，输出错误号 Failed to login and output the error code
                Log.WriteLog(DateTime.Now + " 摄像机登录失败\r\n" + iLastErr + "\r\n");
                return -3;
            }
            else
            {
                //登录成功
                iDeviceNumber++;
                string str1 = "" + m_lUserID;
                return 0;
            }
        }
        public int camera_SetAlarm()
        {
            CHCNetSDK.NET_DVR_SETUPALARM_PARAM struAlarmParam = new CHCNetSDK.NET_DVR_SETUPALARM_PARAM();
            struAlarmParam.dwSize = (uint)Marshal.SizeOf(struAlarmParam);
            struAlarmParam.byLevel = 1; //0- 一级布防,1- 二级布防
            struAlarmParam.byAlarmInfoType = 1;//智能交通设备有效，新报警信息类型
            struAlarmParam.byFaceAlarmDetection = 1;//1-人脸侦测

            for (int i = 0; i < iDeviceNumber; i++)
            {

                m_lAlarmHandle = CHCNetSDK.NET_DVR_SetupAlarmChan_V41(m_lUserID, ref struAlarmParam);
                if (m_lAlarmHandle < 0)
                {
                    iLastErr = CHCNetSDK.NET_DVR_GetLastError();
                    strErr = "布防失败，错误号：" + iLastErr; //布防失败，输出错误号
                    return -1;
                }
            }
            return 0;
        }

        public int camera_CloseAlarm()
        {
            for (int i = 0; i < iDeviceNumber; i++)
            {

                if (m_lAlarmHandle >= 0)
                {
                    if (!CHCNetSDK.NET_DVR_CloseAlarmChan_V30(m_lAlarmHandle))
                    {
                        iLastErr = CHCNetSDK.NET_DVR_GetLastError();
                        strErr = "撤防失败，错误号：" + iLastErr; //撤防失败，输出错误号                      
                        return -1;
                    }

                }
                else
                {
                    strErr = "未布防";
                }
            }
            return 0;
        }

        public int camera_Exit()
        {
            int res = 0;
            //撤防
            res = camera_CloseAlarm();
            if (res != 0) return res;

            //停止监听
            if (iListenHandle >= 0)
            {
                CHCNetSDK.NET_DVR_StopListen_V30(iListenHandle);
            }

            //注销登录
            for (int i = 0; i < iDeviceNumber; i++)
            {

                CHCNetSDK.NET_DVR_Logout(m_lUserID);
            }

            //释放SDK资源，在程序结束之前调用
            CHCNetSDK.NET_DVR_Cleanup();
            return 0;

        }

        public int camera_StartListen()
        {
            byte[] strIP = new byte[16 * 16];
            uint dwValidNum = 0;
            Boolean bEnableBind = false;
            string sLocalIP = "";
            string sLocalPort = "7200";
            ushort wLocalPort = ushort.Parse(sLocalPort);
            int res = 0;
            //获取本地PC网卡IP信息
            if (CHCNetSDK.NET_DVR_GetLocalIP(strIP, ref dwValidNum, ref bEnableBind))
            {
                if (dwValidNum > 0)
                {
                    //取第一张网卡的IP地址为默认监听端口
                    sLocalIP = System.Text.Encoding.UTF8.GetString(strIP, 0, 16);
                }

            }
            iListenHandle = CHCNetSDK.NET_DVR_StartListen_V30(sLocalIP, wLocalPort, m_falarmData, IntPtr.Zero);
            if (iListenHandle < 0)
            {
                iLastErr = CHCNetSDK.NET_DVR_GetLastError();
                strErr = "启动监听失败，错误号：" + iLastErr; //撤防失败，输出错误号
                //MessageBox.Show(strErr);
                return -1;
            }
            else
            {

                return 0;
            }
        }

        public int camera_StopListen()
        {

            if (!CHCNetSDK.NET_DVR_StopListen_V30(iListenHandle))
            {
                iLastErr = CHCNetSDK.NET_DVR_GetLastError();
                strErr = "停止监听失败，错误号：" + iLastErr; //撤防失败，输出错误号
                //MessageBox.Show(strErr);
                return -1;
            }
            else
            {
                //MessageBox.Show("停止监听！");
                return 0;
            }
        }
        //强制抓拍
        public int camera_ForceGetBigImage()
        {
            CHCNetSDK.NET_DVR_PLATE_RESULT struPlateResultInfo = new CHCNetSDK.NET_DVR_PLATE_RESULT();
            struPlateResultInfo.pBuffer1 = Marshal.AllocHGlobal(2 * 1024 * 1024);
            struPlateResultInfo.pBuffer2 = Marshal.AllocHGlobal(1024 * 1024);
            CHCNetSDK.NET_DVR_MANUALSNAP struInter = new CHCNetSDK.NET_DVR_MANUALSNAP();
            struInter.byLaneNo = 1;
            if (!CHCNetSDK.NET_DVR_ManualSnap(m_lUserID, ref struInter, ref struPlateResultInfo))
            {
                uint iLastErr = CHCNetSDK.NET_DVR_GetLastError();
                string strErr = "NET_DVR_ManualSnap failed, error code= " + iLastErr;
                MF.AddOperLogCacheStr(strErr);

                Marshal.FreeHGlobal(struPlateResultInfo.pBuffer1);
                Marshal.FreeHGlobal(struPlateResultInfo.pBuffer2);
                return -1;
            }
            else
            {
                int iLen = (int)struPlateResultInfo.dwPicLen; ;
                if (iLen > 0)
                {
                    byte[] by = new byte[iLen];
                    if (struPlateResultInfo.struPlateInfo.sLicense.Equals("无车牌"))
                    {
                        GetPlateNo = "未检测";
                    }
                    else
                    {
                        string temp = "";
                        switch (struPlateResultInfo.struPlateInfo.byColor)
                        {
                            case 0:
                                temp = "蓝";
                                break;
                            case 1:
                                temp = "黄";
                                break;
                            case 2:
                                temp = "白";
                                break;
                            case 3:
                                temp = "黑";
                                break;
                            case 4:
                                temp = "绿";
                                break;
                            default:
                                break;
                        }
                        GetPlateNo = struPlateResultInfo.struPlateInfo.sLicense;
                    }
                    GetVehicleLogoRecog = "";
                    GetVehicleLogoRecog = CHCNetSDK.VLR_VEHICLE_CLASS[struPlateResultInfo.struVehicleInfo.byVehicleLogoRecog];

                    FlieClass fc = new FlieClass();
                    string dirpath = ".\\image\\";
                    DateTime forcetimedt = DateTime.Now;
                    string forcetime = forcetimedt.ToString("yyyyMMddHHmmss");
                    string imagename = forcetime + GetPlateNo + ".bmp";
                    dirpath += DateTime.Now.Year.ToString();
                    dirpath += "年\\";
                    dirpath += DateTime.Now.Month.ToString();
                    dirpath += "月\\";
                    dirpath += DateTime.Now.Day.ToString();
                    dirpath += "日\\";
                    imagepath = dirpath + imagename;
                    Marshal.Copy(struPlateResultInfo.pBuffer1, by, 0, iLen);
                    try
                    {
                        if (true == fc.WriteFileImage(dirpath, imagename, by, 0, iLen))
                        {
                            Marshal.FreeHGlobal(struPlateResultInfo.pBuffer1);
                            Marshal.FreeHGlobal(struPlateResultInfo.pBuffer2);
                            return 0;
                        }
                        else
                        {
                            Marshal.FreeHGlobal(struPlateResultInfo.pBuffer1);
                            Marshal.FreeHGlobal(struPlateResultInfo.pBuffer2);
                            //AddOperLogCacheStr("保存车牌图片失败!");
                            return -1;
                        }
                    }
                    catch (Exception ex)
                    {
                        //AddOperLogCacheStr("保存车牌图片失败!");
                        Marshal.FreeHGlobal(struPlateResultInfo.pBuffer1);
                        Marshal.FreeHGlobal(struPlateResultInfo.pBuffer2);
                        return -1;
                    }
                }
            }
            return 0;
        }

        public void MsgCallback(int lCommand, ref CHCNetSDK.NET_DVR_ALARMER pAlarmer, IntPtr pAlarmInfo, uint dwBufLen, IntPtr pUser)
        {
            //通过lCommand来判断接收到的报警信息类型，不同的lCommand对应不同的pAlarmInfo内容
            switch (lCommand)
            {
                case CHCNetSDK.COMM_ITS_PLATE_RESULT://交通抓拍结果上传(新报警信息类型)
                    ProcessCommAlarm_ITSPlate(ref pAlarmer, pAlarmInfo, dwBufLen, pUser);
                    break;
                default:
                    break;
            }
        }

        public uint ProcessCommAlarm_ITSPlate(ref CHCNetSDK.NET_DVR_ALARMER pAlarmer, IntPtr pAlarmInfo, uint dwBufLen, IntPtr pUser)
        {
            
            DateTime dtS = DateTime.Now;
            CHCNetSDK.NET_ITS_PLATE_RESULT struITSPlateResult = new CHCNetSDK.NET_ITS_PLATE_RESULT();
            uint dwSize = (uint)Marshal.SizeOf(struITSPlateResult);
            struITSPlateResult = (CHCNetSDK.NET_ITS_PLATE_RESULT)Marshal.PtrToStructure(pAlarmInfo, typeof(CHCNetSDK.NET_ITS_PLATE_RESULT));
            TimeSpan ts = DateTime.Now - dtS;
            Log.WritePlateLog(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + " 摄像机抓拍完成时间1：" + ts.TotalMilliseconds + "\r\n");
            MF.CameraCanpost.WaitOne(1000);
            MF.AddOperLogCacheStr("进入报警布防回调函数,图片" + struITSPlateResult.dwPicNum.ToString() + "张..");
            string res = "成功";
            int iLen = (int)struITSPlateResult.struPicInfo[0].dwDataLen;
            byte[] by = new byte[iLen];
            if (iLen > 0) res = "成功";
            else res = "失败";
            MF.AddOperLogCacheStr("取图返回:" + res);
            if (struITSPlateResult.struPlateInfo.sLicense.Equals("无车牌"))
            {
                GetPlateNo = "未检测";
            }
            else
            {
                string temp = "";
                switch (struITSPlateResult.struPlateInfo.byColor)
                {
                    case 0:
                        temp = "蓝";
                        break;
                    case 1:
                        temp = "黄";
                        break;
                    case 2:
                        temp = "白";
                        break;
                    case 3:
                        temp = "黑";
                        break;
                    case 4:
                        temp = "绿";
                        break;
                    default:
                        break;
                }
                GetPlateNo = struITSPlateResult.struPlateInfo.sLicense;
                
            }
            if (GetPlateNo.Equals(""))
            {
                GetPlateNo = "无牌车";
            }
            MF.AddOperLogCacheStr("车牌： " + GetPlateNo);
            GetVehicleLogoRecog = "";
            GetVehicleLogoRecog = CHCNetSDK.VLR_VEHICLE_CLASS[struITSPlateResult.struVehicleInfo.byVehicleLogoRecog];
            FlieClass fc = new FlieClass();
            string dirpath = ".\\plateimage\\";
            DateTime forcetimedt = DateTime.Now;
            string forcetime = forcetimedt.ToString("yyyyMMddHHmmss");
            string imagename = forcetime + GetPlateNo + ".bmp";
            dirpath += DateTime.Now.Year.ToString();
            dirpath += "年\\";
            dirpath += DateTime.Now.Month.ToString();
            dirpath += "月\\";
            dirpath += DateTime.Now.Day.ToString();
            dirpath += "日\\";
            imagepath = dirpath + imagename;
            //暂时放这里
            ts = DateTime.Now - dtS;
            Log.WritePlateLog(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + " 摄像机抓拍完成时间2：" + ts.TotalMilliseconds + "\r\n");

            Marshal.Copy(struITSPlateResult.struPicInfo[0].pBuffer, by, 0, iLen);
            try
            {
                if (true == fc.WriteFileImage(dirpath, imagename, by, 0, iLen))
                {
                    //MF.CameraCanpost.WaitOne(300);
                    MF.CameraPicture.Set();
                    ts = DateTime.Now - dtS;
                    Log.WritePlateLog(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + " 摄像机抓拍完成时间3：" + ts.TotalMilliseconds + "\r\n");
                }
                else
                {
                    MF.AddOperLogCacheStr("保存车牌图片失败!");
                    return 1;
                }
            }
            catch (Exception ex)
            {
                MF.AddOperLogCacheStr("保存车牌图片失败!");
                return 1;
            }
            //MF.CameraCanpost.Reset();
            return 0;
        }

        #endregion
    }
}
