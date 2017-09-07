using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ETCF
{
    class GlobalMember
    {
        public static bool WriteLogSwitch = true;
        public static int OperLogFreshTime = 500;//单位ms
        //海康摄像机
        public static HKCamera HKCameraInter = null;
        //IPC摄像机
        public static IPCCamera IPCCameraInter = null;
        //数据库
        public static SQLServerInter SQLInter = null;
        //IPNC摄像机
        public static IPNCCamera IPNCCameraInter = null;
    }
}
