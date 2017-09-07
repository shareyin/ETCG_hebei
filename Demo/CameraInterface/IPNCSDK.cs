using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace ETCF
{
    public class IPNCSDK
    {
        public delegate void PFSnapPic(IntPtr buf, int len, IntPtr PlateNo, ref byte PlateColor, IntPtr PlateBrand, int veh_type, int res);

        [DllImport("WJIPNCSDK.dll", EntryPoint = "WVS_InitializeSDK", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool WVS_InitializeSDK();

        [DllImport("WJIPNCSDK.dll", EntryPoint = "WVS_CloseHv", CallingConvention = CallingConvention.Cdecl)]
        public static extern int WVS_CloseHv();

        [DllImport("WJIPNCSDK.dll", EntryPoint = "WVS_AddCamera", CallingConvention = CallingConvention.Cdecl)]
        public static extern int WVS_AddCamera(string CameraIP, PFSnapPic Callback);

        public PFSnapPic pCaremaCallBack = null;

        public bool Initialize()
        {
            bool val;
            val = WVS_InitializeSDK();
            return val;
        }

        public int ConnectCamera(string IP)
        {
            int val;
            val = WVS_AddCamera(IP, pCaremaCallBack);
            return val;
        }

        public void CloseCamera()
        {
            WVS_CloseHv();
        }

        
    }
}
