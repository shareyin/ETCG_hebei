using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ETCF
{
    class GlobalMember
    {
        //界面日志用
        public static bool WriteLogSwitch = true;//界面日志存储开关
        public static int OperLogFreshTime = 500;//单位ms 界面日志存储时间间隔
        //登陆用
        public static bool isLogin = true;//用于判断用户是否已登录

        //汽车分类阈值
        public static UInt16 DisFirst_H=2100;
        public static UInt16 DisFirst_L = 5500;
        public static UInt16 DisSecond_H = 3000;
        public static UInt16 DisSecond_L = 7000;
        public static UInt16 DisThird_H = 3600;
        public static UInt16 DisThird_L = 9000;
        //客4特别分类
        public static UInt16 DisFourHigherthan_H=3700;
        public static UInt16 DisFourHighandLength_H = 3500;
        public static UInt16 DisFourHighandLength_L = 11500;
        //大型客1分类
        public static UInt16 DisLargeTypeOne_H = 2300;
        public static UInt16 DisLargeTypeOne_L = 5700;


        public static string SavePicPath = "";

        //匹配方式
        public static string MarchByPlateNum = "车牌匹配";
        public static string MarchByMohu = "模糊匹配";
        public static string MarchByLocation = "位置匹配";
        public static string MarchByForce = "强制匹配";
        public static string MarchByDefault = "";
        public static string MarchByPerfact = "完全匹配";
        public static string MarchByReget = "补偿匹配";

        //
        public static DateTime g_TimeForTest;

        //车道号
        public static string g_sLaneNo = "";
        
        //位置匹配前后阈值
        public static int ZxTempValue = 400;
        public static int FxTempValue = 200;

        //海康摄像机
        public static HKCamera HKCameraInter = null;
        //IPC摄像机
        public static IPCCamera IPCCameraInter = null;
        //IPNC摄像机
        public static IPNCCamera IPNCCameraInter = null;

        //选择数据库类型
        public static string SqlType = "";//SQLServer表示使用SQLServer数据库 Mysql表示使用Mysql数据库
        //SQLserver数据库
        public static SQLServerInter SQLInter = null;
        //mysql数据库
        public static MysqlInter MysqlInter = null;

        //报警器串口号
        public static string g_sComofAlarm = "COM1";
        
        //报警时间
        public static int g_iTime0fAlarm = 3000;

        public static bool isDeedJGHeart = false;
    }
}
