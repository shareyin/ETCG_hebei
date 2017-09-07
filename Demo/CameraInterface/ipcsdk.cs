using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace ETCF
{
    [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential, CharSet = System.Runtime.InteropServices.CharSet.Ansi)]
    //osd叠加信息结构体
    public struct ICE_OSDAttr_S
    {
        //video
        public uint u32OSDLocationVideo;    //叠加位置(0左上，1右上，2左下，3右下，4上居中，5下居中)
        public uint u32ColorVideo;          //颜色(十六进制RGB颜色值，格式为0x00bbggrr)
        public uint u32DateVideo;           //是否叠加日期时间(0不叠加 1叠加)
        public uint u32License;             //是否叠加授权信息(0不叠加 1叠加)
        public uint u32CustomVideo;         //是否叠加自定义信息(0不叠加 1叠加)
        [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValTStr, SizeConst = 64)]
        public string szCustomVideo;        //自定义信息(预留，暂未使用)

        //jpeg
        public uint u32OSDLocationJpeg;     //叠加位置(0左上，1右上，2左下，3右下，4上居中，5下居中)
        public uint u32ColorJpeg;           //颜色(十六进制RGB颜色值，格式为0x00bbggrr)
        public uint u32DateJpeg;            //是否叠加日期时间(0不叠加 1叠加)
        public uint u32Algo;                //是否叠加算法信息(0不叠加 1叠加)
        public uint u32DeviceID;            //是否叠加设备ID(预留，暂未使用)
        [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValTStr, SizeConst = 64)]
        public string szDeviceID;           //设备ID(预留，暂未使用)
        public uint u32DeviceName;          //是否叠加设备名称(预留，暂未使用)	
        [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValTStr, SizeConst = 64)]
        public string szDeviceName;         //设备名称(预留，暂未使用)
        public uint u32CamreaLocation;      //是否叠加地点信息(预留，暂未使用)
        [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValTStr, SizeConst = 64)]
        public string szCamreaLocation;     //地点信息(预留，暂未使用)
        public uint u32SubLocation;         //是否叠加子地点信息(预留，暂未使用)
        [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValTStr, SizeConst = 64)]
        public string szSubLocation;        //子地点信息(预留，暂未使用)
        public uint u32ShowDirection;       //是否叠加相机方向(预留，暂未使用)
        public uint u32CameraDirection;     //相机方向(预留，暂未使用)
        public uint u32CustomJpeg;          //是否叠加自定义信息(1叠加 0不叠加)	

        public uint u32ItemDisplayMode;     //图片信息显示模式（0，多行显示，1，单行显示,默认0）
        public uint u32DateMode;            //日期显示模式（0，xxxx/xx/xx   1，xxxx年xx月xx日，默认0）
        public uint u32BgColor;             //OSD 背景色（0背景全透明，1，背景黑色，默认0）
        public uint u32FontSize;            //字体大小（0:小，1:中 2:大,默认为中，在540P 以下，中和大会转换为小）
        [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValTStr, SizeConst = 48)]
        public string szCustomJpeg;         //自定义信息(预留，暂未使用)
        [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValTStr, SizeConst = 384)]
        public string szCustomVideo6;       //视频自定义信息，每行最多64字节（包含换行符），最多6行，数组长度为64*6
        [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValTStr, SizeConst = 384)]
        public string szCustomJpeg6;        //抓拍图自定义信息，每行最多64字节（包含换行符），最多6行，数组长度为64*6
    }

    [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential, CharSet = System.Runtime.InteropServices.CharSet.Ansi)]
    //车辆特征码结构体   
    public struct ICE_VBR_RESULT_S
    {
        /// ICE_S8[20]
        [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValTStr, SizeConst = 20)]
        public string szLogName;        //主品牌

        /// ICE_S8[20]
        [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValTStr, SizeConst = 20)]
        public string szSubLogName;     //预留

        /// ICE_S8[20]
        [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValTStr, SizeConst = 20)]
        public string szProductYearName;//预留

        /// ICE_S8[40]
        [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValTStr, SizeConst = 40)]
        public string szLabelTotal;     //预留

        /// ICE_U32->unsigned int
        public uint iLabelIndex;        //预留

        /// ICE_FLOAT->float
        public float fScore;            //预留

        /// ICE_FLOAT[20]
        [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 20, ArraySubType = System.Runtime.InteropServices.UnmanagedType.R4)]
        public float[] fResFeature;     //特征码，数组长度为20

        /// ICE_U32[4]
        [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 4, ArraySubType = System.Runtime.InteropServices.UnmanagedType.U4)]
        public uint[] iReserved;        //预留
    }

    [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential, CharSet = System.Runtime.InteropServices.CharSet.Ansi)]
    //车牌识别数据结构体，由于只使用车辆特征码结构体指针，其余内容都用数组代替
    public struct ICE_VDC_PICTRUE_INFO_S
    {
#if VERSION32
        [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValTStr, SizeConst = 132)]
        public string cFileName;            //预留（考虑到64位指针长度比32位时指针长度大4，所以32位的预留数组长度比64位大4）
#else
         [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValTStr, SizeConst = 128)]
        public string cFileName;            //预留（考虑到64位指针长度比32位时指针长度大4，所以32位的预留数组长度比64位大4）
#endif


        /// ICE_VBR_RESULT_S*
        public System.IntPtr pstVbrResult; //车辆特征码结构体指针

#if VERSION32
        [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValTStr, SizeConst = 404)]
        public string data;
#else
         [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValTStr, SizeConst = 408)]
        public string data;                 //预留
#endif

    }


    public partial class ipcsdk
    {
        [System.Runtime.InteropServices.UnmanagedFunctionPointerAttribute(System.Runtime.InteropServices.CallingConvention.StdCall)]
        /**
         * @brief  车牌类型枚举值
         * typedef enum 
            {
	            ICE_PLATE_UNCERTAIN	    = 0,	        不确定的
	            ICE_PLATE_BLUE	        = 1,	        蓝牌车
	            ICE_PLATE_YELLOW	    = 2,	        单层黄牌车
	            ICE_PLATE_POL	        = 4,	        警车
	            ICE_PLATE_WUJING	    = 8,	        武警车辆
	            ICE_PLATE_DBYELLOW	    = 16,	        双层黄牌
	            ICE_PLATE_MOTOR	        = 32,	        摩托车
	            ICE_PLATE_INSTRUCTIONCAR= 64,	        教练车
	            ICE_PLATE_MILITARY	    = 128,	        军车
	            ICE_PLATE_PERSONAL	    = 256,	        个性化车
	            ICE_PLATE_GANGAO	    = 512,	        港澳车
	            ICE_PLATE_EMBASSY	    = 1024,	        使馆车
	            ICE_PLATE_NONGLARE	    = 2048,	        老式车牌(不反光)

	            ICE_PLATE_WHITE_TWOTWO	= 0x10000001,	2+2模型；
	            ICE_PLATE_WHITE_TWOTHR	= 0x10000002,	2+3模型；
	            ICE_PLATE_WHITE_THRTWO	= 0x10000004,	3+2模型；
	            ICE_PLATE_WHITE_THRTHR	= 0x10000008,	 3+3模型；
	            ICE_PLATE_WHITE_THRFOR	= 0x10000010,	3+4模型；

	            ICE_PLATE_BLACK_THRTWO	= 0x10000020,	3+2模型；
	            ICE_PLATE_BLACK_TWOTHR	= 0x10000040,	2+3模型；
	            ICE_PLATE_BLACK_THRTHR	= 0x10000080,	3+3模型；
	            ICE_PLATE_BLACK_TWOFOR	= 0x10000100,	2+4模型；
	            ICE_PLATE_BLACK_FORTWO	= 0x10000200,	4+2模型；
	            ICE_PLATE_BLACK_THRFOR	= 0x10000400,	3+4模型；
            }ICE_PLATETYPE_E;
         */
        /*
         * @brief   报警类型枚举值
            typedef enum{
	            ICE_VDC_HD_TRIGER,						//实时_硬触发+临时车辆(0),
	            ICE_VDC_VIDEO_TRIGER, 					//实时_视频触发+临时车辆（1），
	            ICE_VDC_SOFT_TRIGER,					//实时_软触发+临时车辆（2），
	            ICE_VDC_HD_TRIGER_AND_WHITELIST,		//实时_硬触发+有效白名单(3),
	            ICE_VDC_VIDEO_TRIGER_AND_WHITELIST,		//实时_视频触发+有效白名单（4），
	            ICE_VDC_SOFT_TRIGER_AND_WHITELIST,  	//实时_软触发+有效白名单（5），
	            ICE_VDC_HD_TRIGER_AND_BLACKLIST,		//实时_硬触发+黑名单(6),
	            ICE_VDC_VIDEO_TRIGER_AND_BLACKLIST,		//实时_视频触发+黑名单（7），
	            ICE_VDC_SOFT_TRIGER_AND_BLACKLIST,  	//实时_软触发+黑名单（8），
	
	            ICE_VDC_NC_HD_TRIGER,					//脱机_硬触发+临时车辆(9),
	            ICE_VDC_NC_VIDEO_TRIGER, 				//脱机_视频触发+临时车辆（10），
	            ICE_VDC_NC_SOFT_TRIGER,					//脱机_软触发+临时车辆（11），
	            ICE_VDC_NC_HD_TRIGER_AND_WHITELIST,		//脱机_硬触发+有效白名单(12),
	            ICE_VDC_NC_VIDEO_TRIGER_AND_WHITELIST,	//脱机_视频触发+有效白名单（13），
	            ICE_VDC_NC_SOFT_TRIGER_AND_WHITELIST,  	//脱机_软触发+有效白名单（14），
	            ICE_VDC_NC_HD_TRIGER_AND_BLACKLIST,		//脱机_硬触发+黑名单(15),
	            ICE_VDC_NC_VIDEO_TRIGER_AND_BLACKLIST,	//脱机_视频触发+黑名单（16），
	            ICE_VDC_NC_SOFT_TRIGER_AND_BLACKLIST,  	//脱机_软触发+黑名单（17），
	
	            ICE_VDC_HD_TRIGER_AND_OVERDUE_WHITELIST,		//实时_硬触发+过期白名单(18),
	            ICE_VDC_VIDEO_TRIGER_AND_OVERDUE_WHITELIST,		//实时_视频触发+过期白名单（19），
	            ICE_VDC_SOFT_TRIGER_AND_OVERDUE_WHITELIST,  	//实时_软触发+过期白名单（20），
	            ICE_VDC_NC_HD_TRIGER_AND_OVERDUE_WHITELIST,		//脱机_硬触发+过期白名单(21),
	            ICE_VDC_NC_VIDEO_TRIGER_AND_OVERDUE_WHITELIST,	//脱机_视频触发+过期白名单（22），
	            ICE_VDC_NC_SOFT_TRIGER_AND_OVERDUE_WHITELIST,  	//脱机_软触发+过期白名单（23），
	
	            ICE_VDC_ALARM_UNKOWN,
            }ICE_VDC_ALARM_TYPE;
         * /
        /**
         *  @brief  通过该回调函数获得实时识别数据
         *  @param  [OUT] pvParam	         用户自定义参数，用来区分不同的sdk使用者，类似于线程入口函数的参数（与设置此回调接口的最后一个参数相同）
         *  @param  [OUT] pcIP	             相机ip
         *  @param  [OUT] pcNumber           车牌号	
         *  @param  [OUT] pcColor            车牌颜色（"蓝色","黄色","白色","黑色",“绿色”）
         *  @param  [OUT] pcPicData          全景数据
         *  @param  [OUT] u32PicLen          全景数据长度
         *  @param  [OUT] pcCloseUpPicData   车牌数据
         *  @param  [OUT] u32CloseUpPicLen   车牌数据长度
         *  @param  [OUT] nSpeed             车辆速度
         *  @param  [OUT] nVehicleType       车辆类型（0:未知,1:普通汽车,2:面包车,3:大客车,4:箱式货车,5:大货车,6:非机动车)
         *  @param  [OUT] nReserved1         预留参数1
         *  @param  [OUT] nReserved2         预留参数2
         *  @param  [OUT] fPlateConfidence   车牌打分值（SDK输出的范围大于IE界面设置的车牌阈值，上限是28，例如：IE设置的是10，范围：10-28）
         *  @param  [OUT] u32VehicleColor    车身颜色（车辆特征码相机版本：(-1:未知,0:黑色,1:蓝色,2:灰色,3:棕色,4:绿色,5:夜间深色,6:紫色,7:红色,8:白色,9:黄色)
         *                                           其它相机版本：(0:未知,1:红色,2:绿色,3:蓝色,4:黄色,5:白色,6:灰色,7:黑色,8:紫色,9:棕色,10:粉色)）
         *  @param  [OUT] u32PlateType       车牌类型，详见车牌类型ICE_PLATETYPE_E枚举值
         *  @param  [OUT] u32VehicleDir      车辆方向（0:车头方向,1:车尾方向,2:车头和车尾方向）
         *  @param  [OUT] u32AlarmType       报警输出，详见报警输出ICE_VDC_ALARM_TYPE枚举值
         *  @param  [OUT] u32SerialNum       抓拍的序号（从相机第一次抓拍开始计数，相机重启后才清零）
         *  @param  [OUT] uCapTime           实时抓拍时间，从1970年1月1日零点开始的秒数
         *  @param  [OUT] u32ResultHigh      车牌识别数据结构体（ICE_VDC_PICTRUE_INFO_S）指针高8位（64位sdk时需要使用）
         *  @param  [OUT] u32ResultLow       车牌识别数据结构体（ICE_VDC_PICTRUE_INFO_S）指针低8位
         *  @return void
         */
        public delegate void ICE_IPCSDK_OnPlate(
                    System.IntPtr pvParam,
                    [System.Runtime.InteropServices.InAttribute()] [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.LPStr)] string pcIP,
                    [System.Runtime.InteropServices.InAttribute()] [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.LPStr)] string pcNumber,
                    [System.Runtime.InteropServices.InAttribute()] [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.LPStr)] string pcColor,
                    System.IntPtr pcPicData,
                    uint u32PicLen,
                    System.IntPtr pcCloseUpPicData,
                    uint u32CloseUpPicLen,
                    short nSpeed,
                    short nVehicleType,
                    short nReserved1,
                    short nReserved2,
                    float fPlateConfidence,
                    uint u32VehicleColor,
                    uint u32PlateType,
                    uint u32VehicleDir,
                    uint u32AlarmType,
                    uint u32SerialNum,
                    uint uCapTime,
                    uint u32ResultHigh,
                    uint u32ResultLow);

        /**
         *  @brief  设置获得实时识别数据的相关回调函数
         *  @param  [IN] hSDK       连接相机时返回的sdk句柄
         *  @param  [IN] pfOnPlate  实时识别数据，通过该回调获得
         *  @param  [IN] pvParam    回调函数中的参数，能区分开不同的使用者即可
         *  @return void
         */
        [System.Runtime.InteropServices.DllImportAttribute("ice_ipcsdk.dll", EntryPoint = "ICE_IPCSDK_SetPlateCallback", CallingConvention = CallingConvention.Cdecl)]
        public static extern void ICE_IPCSDK_SetPlateCallback(System.IntPtr hSDK, ICE_IPCSDK_OnPlate pfOnPlate, System.IntPtr pvParam);

        /**
         *  @brief  通过该回调函数获得断网识别数据
         *  @param  [OUT] pvParam	         用户自定义参数，用来区分不同的sdk使用者，类似于线程入口函数的参数（即ICE_IPCSDK_SetPastPlateCallBack传入的最后一个参数）
         *  @param  [OUT] pcIP	             相机ip
         *  @param  [OUT] u32CapTime         抓拍时间   
         *  @param  [OUT] pcNumber           车牌号	
         *  @param  [OUT] pcColor            车牌颜色（"蓝色","黄色","白色","黑色",“绿色”）
         *  @param  [OUT] pcPicData          全景数据
         *  @param  [OUT] u32PicLen          全景数据长度
         *  @param  [OUT] pcCloseUpPicData   车牌数据
         *  @param  [OUT] u32CloseUpPicLen   车牌数据长度
         *  @param  [OUT] s16PlatePosLeft    车牌区域左上角横坐标
         *  @param  [OUT] s16PlatePosTop     车牌区域左上角纵坐标
         *  @param  [OUT] s16PlatePosRight   车牌区域右下角横坐标
         *  @param  [OUT] s16PlatePosBottom  车牌区域右下角纵坐标
         *  @param  [OUT] fPlateConfidence   车牌打分值（SDK输出的范围大于IE界面设置的车牌阈值，上限是28，例如：IE设置的是10，范围：10-28）
         *  @param  [OUT] u32VehicleColor    车身颜色（0:未知,1:红色,2:绿色,3:蓝色,4:黄色,5:白色,6:灰色,7:黑色,8:紫色,9:棕色,10:粉色）
         *  @param  [OUT] u32PlateType       车牌类型，详见车牌类型ICE_PLATETYPE_E枚举值
         *  @param  [OUT] u32VehicleDir      车辆方向（0:车头方向,1:车尾方向,2:车头和车尾方向）
         *  @param  [OUT] u32AlarmType       报警输出，详见报警输出ICE_VDC_ALARM_TYPE枚举值
         *  @param  [OUT] u32Reserved1       预留参数1
         *  @param  [OUT] u32Reserved2       预留参数2
         *  @param  [OUT] u32Reserved3       预留参数3
         *  @param  [OUT] u32Reserved4       预留参数4
         *  @return void
         */
        public delegate void ICE_IPCSDK_OnPastPlate(
            System.IntPtr pvParam,
            [System.Runtime.InteropServices.InAttribute()] [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.LPStr)] string pcIP,
            uint u32CapTime,
            [System.Runtime.InteropServices.InAttribute()] [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.LPStr)] string pcNumber,
            [System.Runtime.InteropServices.InAttribute()] [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.LPStr)] string pcColor,
            System.IntPtr pcPicData,
            uint u32PicLen,
            System.IntPtr pcCloseUpPicData,
            uint u32CloseUpPicLen,
            short s16PlatePosLeft,
            short s16PlatePosTop,
            short s16PlatePosRight,
            short s16PlatePosBottom,
            float fPlateConfidence,
            uint u32VehicleColor,
            uint u32PlateType,
            uint u32VehicleDir,
            uint u32AlarmType,
            uint u32Reserved1,
            uint u32Reserved2,
            uint u32Reserved3,
            uint u32Reserved4);

        /**
         *  @brief  设置获得断网识别数据的相关回调函数
         *  @param  [IN] hSDK               连接相机时返回的sdk句柄
         *  @param  [IN] pfOnPastPlate      断网识别数据，通过该回调获得
         *  @param  [IN] pvPastPlateParam   回调函数中的参数，能区分开不同的使用者即可
         *  @return void
         */
        [System.Runtime.InteropServices.DllImportAttribute("ice_ipcsdk.dll", EntryPoint = "ICE_IPCSDK_SetPastPlateCallBack", CallingConvention = CallingConvention.Cdecl)]
        public static extern void ICE_IPCSDK_SetPastPlateCallBack(System.IntPtr hSDK, ICE_IPCSDK_OnPastPlate pfOnPastPlate,
                                                                  System.IntPtr pvPastPlateParam);

        /**
         *  @brief  通过该回调函数获得RS485数据
         *  @param  [OUT] pvParam   用户自定义参数，用来区分不同的sdk使用者，类似于线程入口函数的参数（即ICE_IPCSDK_SetSerialPortCallBack传入的最后一个参数）
         *  @param  [OUT] pcIP      相机ip
         *  @param  [OUT] pcData    串口数据首地址
         *  @param  [OUT] u32Len    串口数据长度
         *  @return void
         */
        public delegate void ICE_IPCSDK_OnSerialPort(System.IntPtr pvParam, 
            [System.Runtime.InteropServices.InAttribute()] [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.LPStr)] string pcIP, 
            System.IntPtr pcData, uint u32Len);

        /**
         *  @brief  设置获得RS485数据的相关回调函数
         *  @param  [IN] hSDK               连接相机时返回的sdk句柄
         *  @param  [IN] pfOnSerialPort     相机发送的RS485数据，通过该回调获得
         *  @param  [IN] pvSerialPortParam  回调函数中的参数，能区分开不同的使用者即可
         */
        [System.Runtime.InteropServices.DllImportAttribute("ice_ipcsdk.dll", EntryPoint = "ICE_IPCSDK_SetSerialPortCallBack", CallingConvention = CallingConvention.Cdecl)]
        public static extern void ICE_IPCSDK_SetSerialPortCallBack(System.IntPtr hSDK, ICE_IPCSDK_OnSerialPort pfOnSerialPort, 
                                                                   System.IntPtr pvSerialPortParam);

        /**
         *  @brief  通过该回调函数获得RS232数据
         *  @param  [OUT] pvParam   用户自定义参数，用来区分不同的sdk使用者，类似于线程入口函数的参数（即ICE_IPCSDK_SetSerialPortCallBack_RS232传入的最后一个参数）
         *  @param  [OUT] pcIP      相机ip
         *  @param  [OUT] pcData    串口数据首地址
         *  @param  [OUT] u32Len    串口数据长度
         *  @return void
         */
        public delegate void ICE_IPCSDK_OnSerialPort_RS232(System.IntPtr pvParam, 
            [System.Runtime.InteropServices.InAttribute()] [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.LPStr)] string pcIP, 
            System.IntPtr pcData, uint u32Len);

        /**
         *  @brief  设置获得RS232数据的相关回调函数
         *  @param  [IN] hSDK               连接相机时返回的sdk句柄
         *  @param  [IN] pfOnSerialPort     相机发送的RS232数据，通过该回调获得
         *  @param  [IN] pvSerialPortParam  回调函数中的参数，能区分开不同的使用者即可
         *  @return void
         */
        [System.Runtime.InteropServices.DllImportAttribute("ice_ipcsdk.dll", EntryPoint = "ICE_IPCSDK_SetSerialPortCallBack_RS232", CallingConvention = CallingConvention.Cdecl)]
        public static extern void ICE_IPCSDK_SetSerialPortCallBack_RS232(System.IntPtr hSDK, ICE_IPCSDK_OnSerialPort_RS232 pfOnSerialPort,
                                                                         System.IntPtr pvSerialPortParam);


        /**
         *  @brief  通过该回调函数获得解码出的一帧图像
         *  @param  [OUT] pvParam       用户自定义参数，用来区分不同的sdk使用者，类似于线程入口函数的参数（即ICE_IPCSDK_SetFrameCallback传入的最后一个参数）
         *  @param  [OUT] u32Timestamp  时间戳
         *  @param  [OUT] pu8DataY      y帧数据地址
         *  @param  [OUT] pu8DataU      U帧数据地址
         *  @param  [OUT] pu8DataV      V帧数据地址
         *  @param  [OUT] s32LinesizeY  y帧数据每扫描行长度
         *  @param  [OUT] s32LinesizeU  U帧数据每扫描行长度
         *  @param  [OUT] s32LinesizeV  V帧数据每扫描行长度
         *  @param  [OUT] s32Width      图像宽度
         *  @param  [OUT] s32Height     图像高度
         *  @return void
         */
        public delegate void ICE_IPCSDK_OnFrame_Planar(System.IntPtr pvParam, uint u32Timestamp,
                                                        System.IntPtr pu8DataY, System.IntPtr pu8DataU,
                                                        System.IntPtr pu8DataV, int s32LinesizeY,
                                                        int s32LinesizeU, int s32LinesizeV, int s32Width, int s32Height);
        /**
         *  @brief  设置获得解码出的一帧图像的相关回调函数
         *  @param  [IN] hSDK       连接相机时返回的sdk句柄
         *  @param  [IN] pfOnFrame  解码出的一帧图像，通过该回调获得
         *  @param  [IN] pvParam    回调函数中的参数，能区分开不同的使用者即可
         *  @return void
         */
        [System.Runtime.InteropServices.DllImportAttribute("ice_ipcsdk.dll", EntryPoint = "ICE_IPCSDK_SetFrameCallback", CallingConvention = CallingConvention.Cdecl)]
        public static extern void ICE_IPCSDK_SetFrameCallback(System.IntPtr hSDK, ICE_IPCSDK_OnFrame_Planar pfOnFrame, System.IntPtr pvParam);

        /**
         *  @brief  全局初始化
         *  @return void
         */
        [System.Runtime.InteropServices.DllImportAttribute("ice_ipcsdk.dll", EntryPoint = "ICE_IPCSDK_Init", CallingConvention = CallingConvention.Cdecl)]
        public static extern void ICE_IPCSDK_Init();

        /**
         *  @brief  全局释放
         *  @return void
         */
        [System.Runtime.InteropServices.DllImportAttribute("ice_ipcsdk.dll", EntryPoint = "ICE_IPCSDK_Fini", CallingConvention = CallingConvention.Cdecl)]
        public static extern void ICE_IPCSDK_Fini();

        /**
         *  @brief  连接相机并接入视频（推荐使用）
         *  @param  [IN] pcIP          相机ip
         *  @param  [IN] u8OverTCP     是否使用tcp模式（1：使用tcp 0：不使用tcp，使用udp）    
         *  @param  [IN] u8MainStream  是否请求主码流（1：主码流 0：子码流） 
         *  @param  [IN] hWnd          预览视频的窗口句柄
         *  @param  [IN] pfOnPlate     车牌识别数据通过该回调获得
         *  @param  [IN] pvPlateParam  车牌回调参数，能区分开不同的使用者即可
         *  @return sdk句柄(连接不成功时，返回值为null）
         */
        [System.Runtime.InteropServices.DllImportAttribute("ice_ipcsdk.dll", EntryPoint = "ICE_IPCSDK_OpenPreview",CallingConvention = CallingConvention.Cdecl)]
        public static extern System.IntPtr ICE_IPCSDK_OpenPreview(
            [System.Runtime.InteropServices.InAttribute()] [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.LPStr)] string pcIP, 
            byte u8OverTCP, byte u8MainStream, uint hWnd, ICE_IPCSDK_OnPlate pfOnPlate, System.IntPtr pvPlateParam);

        /**
         *  @brief  使用密码连接相机并接入视频
         *  @param  [IN] pcIP          相机ip
         *  @param  [IN] pcPasswd      连接密码
         *  @param  [IN] u8OverTCP     是否使用tcp模式（1：使用tcp 0：不使用tcp，使用udp）    
         *  @param  [IN] u8MainStream  是否请求主码流（1：主码流 0：子码流） 
         *  @param  [IN] hWnd          预览视频的窗口句柄
         *  @param  [IN] pfOnPlate     车牌识别数据通过该回调获得
         *  @param  [IN] pvPlateParam  车牌回调参数，能区分开不同的使用者即可
         *  @return sdk句柄(连接不成功时，返回值为null）
         */
        [System.Runtime.InteropServices.DllImportAttribute("ice_ipcsdk.dll", EntryPoint = "ICE_IPCSDK_OpenPreview_Passwd", CallingConvention = CallingConvention.Cdecl)]
        public static extern System.IntPtr ICE_IPCSDK_OpenPreview_Passwd(
            [System.Runtime.InteropServices.InAttribute()] [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.LPStr)] string pcIP,
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.LPStr)] string pcPasswd,
            byte u8OverTCP, byte u8MainStream, uint hWnd, ICE_IPCSDK_OnPlate pfOnPlate, System.IntPtr pvPlateParam);

        /**
         *  @brief  连接相机，不带视频流
         *  @param  [IN] pcIP   相机ip
         *  @return sdk句柄(连接不成功时，返回值为null）
         */
        [System.Runtime.InteropServices.DllImportAttribute("ice_ipcsdk.dll", EntryPoint = "ICE_IPCSDK_OpenDevice", CallingConvention = CallingConvention.Cdecl)]
        public static extern System.IntPtr ICE_IPCSDK_OpenDevice([System.Runtime.InteropServices.InAttribute()] [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.LPStr)] string pcIP);

        /**
         *  @brief  使用密码连接相机，不带视频流
         *  @param  [IN] pcIP          相机ip
         *  @param  [IN] pcPasswd      连接密码
         *  @return sdk句柄(连接不成功时，返回值为null）
         */
        [System.Runtime.InteropServices.DllImportAttribute("ice_ipcsdk.dll", EntryPoint = "ICE_IPCSDK_OpenDevice_Passwd", CallingConvention = CallingConvention.Cdecl)]
        public static extern System.IntPtr ICE_IPCSDK_OpenDevice_Passwd([System.Runtime.InteropServices.InAttribute()] [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.LPStr)] string pcIP,
            [System.Runtime.InteropServices.InAttribute()] [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.LPStr)] string pcPasswd);

        /**
         *  @brief  连接相机
         *  @param  [IN] pcIP          相机ip
         *  @param  [IN] u8OverTCP     是否使用tcp模式（1：使用tcp 0：不使用tcp，使用udp）
         *  @param  [IN] u16RTSPPort   rtsp端口（554）
         *  @param  [IN] u16ICEPort    私有协议对应的端口（8117）
         *  @param  [IN] u16OnvifPort  onvif端口（8080）
         *  @param  [IN] u8MainStream  是否请求主码流（1：主码流 0：子码流） 
         *  @param  [IN] pfOnStream    网络流回调地址，可以为NULL(demo中没有使用)
         *  @param  [IN] pvStreamParam 网络流回调参数，能区分开不同的使用者即可
         *  @param  [IN] pfOnFrame     图像帧回调地址，可以为NULL，只有当u8ReqType包含了REQ_TYPE_H264时才有意义(demo中没有使用)
         *  @param  [IN] pvFrameParam  图像帧回调参数，能区分开不同的使用者即可
         *  @return sdk句柄(连接不成功时，返回值为null）
         */
        [System.Runtime.InteropServices.DllImportAttribute("ice_ipcsdk.dll", EntryPoint = "ICE_IPCSDK_Open", CallingConvention = CallingConvention.Cdecl)]
        public static extern System.IntPtr ICE_IPCSDK_Open(
            [System.Runtime.InteropServices.InAttribute()] [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.LPStr)] string pcIP,
            byte u8OverTCP, ushort u16RTSPPort, ushort u16ICEPort,
            ushort u16OnvifPort, byte u8MainStream, uint pfOnStream,
            System.IntPtr pvStreamParam, uint pfOnFrame, System.IntPtr pvFrameParam);

        /**
         *  @brief  使用密码连接相机
         *  @param  [IN] pcIP          相机ip
         *  @param  [IN] pcPasswd      连接密码
         *  @param  [IN] u8OverTCP     是否使用tcp模式（1：使用tcp 0：不使用tcp，使用udp）
         *  @param  [IN] u16RTSPPort   rtsp端口（554）
         *  @param  [IN] u16ICEPort    私有协议对应的端口（8117）
         *  @param  [IN] u16OnvifPort  onvif端口（8080）
         *  @param  [IN] u8MainStream  是否请求主码流（1：主码流 0：子码流） 
         *  @param  [IN] pfOnStream    网络流回调地址，可以为NULL(demo中没有使用)
         *  @param  [IN] pvStreamParam 网络流回调参数，能区分开不同的使用者即可
         *  @param  [IN] pfOnFrame     图像帧回调地址，可以为NULL，只有当u8ReqType包含了REQ_TYPE_H264时才有意义(demo中没有使用)
         *  @param  [IN] pvFrameParam  图像帧回调参数，能区分开不同的使用者即可
         *  @return sdk句柄(连接不成功时，返回值为null）
         */
        [System.Runtime.InteropServices.DllImportAttribute("ice_ipcsdk.dll", EntryPoint = "ICE_IPCSDK_Open_Passwd", CallingConvention = CallingConvention.Cdecl)]
        public static extern System.IntPtr ICE_IPCSDK_Open_Passwd(
            [System.Runtime.InteropServices.InAttribute()] [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.LPStr)] string pcIP,
            [System.Runtime.InteropServices.InAttribute()] [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.LPStr)] string pcPasswd,
            byte u8OverTCP, ushort u16RTSPPort, ushort u16ICEPort, ushort u16OnvifPort, byte u8MainStream, 
            uint pfOnStream, System.IntPtr pvStreamParam, uint pfOnFrame, System.IntPtr pvFrameParam);

        /**
         *  @brief  断开连接
         *  @param  [IN] hSDK   连接相机时返回的句柄值
         *  @return void
         */
        [System.Runtime.InteropServices.DllImportAttribute("ice_ipcsdk.dll", EntryPoint = "ICE_IPCSDK_Close", CallingConvention = CallingConvention.Cdecl)]
        public static extern void ICE_IPCSDK_Close(System.IntPtr hSDK);

        /**
         *  @brief  连接视频
         *  @param  [IN] hSDK           连接相机时返回的句柄值
         *  @param  [IN] u8MainStream   是否请求主码流（1：主码流 0：子码流）
         *  @param  [IN] hWnd           预览视频的窗口句柄
         *  @return 1表示成功，0表示失败
         */
        [System.Runtime.InteropServices.DllImportAttribute("ice_ipcsdk.dll", EntryPoint = "ICE_IPCSDK_StartStream", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint ICE_IPCSDK_StartStream(System.IntPtr hSDK, byte u8MainStream, uint hWnd);

        /**
         *  @brief  断开视频
         *  @param  [IN] hSDK   连接相机时返回的句柄值
         *  @return void
         */
        [System.Runtime.InteropServices.DllImportAttribute("ice_ipcsdk.dll", EntryPoint = "ICE_IPCSDK_StopStream", CallingConvention = CallingConvention.Cdecl)]
        public static extern void ICE_IPCSDK_StopStream(System.IntPtr hSDK);

        /**
         *  @brief   打开道闸
         *  @param   [IN] hSDK 由连接相机接口获得的句柄
         *  @return  1表示成功，0表示失败
         */
        [System.Runtime.InteropServices.DllImportAttribute("ice_ipcsdk.dll", EntryPoint = "ICE_IPCSDK_OpenGate", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint ICE_IPCSDK_OpenGate(System.IntPtr hSDK);

        /**
         *  @brief   控制开关量输出
         *  @param   [IN] hSDK      由连接相机接口获得的句柄
         *  @param   [IN] u32Index  控制的IO口(0:表示IO1 1:表示IO2)
         *  @return  1表示成功，0表示失败
         */
        [System.Runtime.InteropServices.DllImportAttribute("ice_ipcsdk.dll", EntryPoint = "ICE_IPCSDK_ControlAlarmOut", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint ICE_IPCSDK_ControlAlarmOut(System.IntPtr hSDK, uint u32Index);

        /**
         *  @brief  获取开关量输出配置
         *  @param  [IN] hSDK             由连接相机接口获得的句柄
         *  @parame [IN] u32Index         IO口（0：IO1 1：IO2）
         *  @param  [OUT] pu32IdleState   常开常闭状态的配置（1 是常开，0是常闭）
         *  @param  [OUT] pu32DelayTime   切换状态的时间（-1表示不恢复 单位：s）
         *  @param  [OUT] pu32Reserve     预留参数
         *  @return 1表示成功，0表示失败
         */
        [System.Runtime.InteropServices.DllImportAttribute("ice_ipcsdk.dll", EntryPoint = "ICE_IPCSDK_GetAlarmOutConfig", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint ICE_IPCSDK_GetAlarmOutConfig(System.IntPtr hSDK, uint u32Index, ref uint pu32IdleState, 
                                                                ref uint pu32DelayTime, ref uint pu32Reserve);

        /**
         *  @brief  设置开关量输出配置
         *  @param  [IN] hSDK             由连接相机接口获得的句柄
         *  @parame [IN] u32Index         IO口（0：IO1 1：IO2）
         *  @param  [IN] pu32IdleState    常开常闭状态的配置（1 是常开，0是常闭）
         *  @param  [IN] pu32DelayTime    切换状态的时间（-1表示不恢复 单位：s）
         *  @param  [IN] pu32Reserve      预留参数
         *  @return 1表示成功，0表示失败
         */
        [System.Runtime.InteropServices.DllImportAttribute("ice_ipcsdk.dll", EntryPoint = "ICE_IPCSDK_SetAlarmOutConfig", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint ICE_IPCSDK_SetAlarmOutConfig(System.IntPtr hSDK, uint u32Index, uint u32IdleState,
                                                                uint u32DelayTime, uint u32Reserve);
        /**
         *  @brief  开始对讲
         *  @param  [IN] hSDK 由连接相机接口获得的句柄
         *  @return 1表示成功，0表示失败
         */
        [System.Runtime.InteropServices.DllImportAttribute("ice_ipcsdk.dll", EntryPoint = "ICE_IPCSDK_BeginTalk", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint ICE_IPCSDK_BeginTalk(System.IntPtr hSDK);

        /**
         *  @brief  结束对讲
         *  @param  [IN] hSDK 由连接相机接口获得的句柄
         *  @return 1表示成功，0表示失败
         */
        [System.Runtime.InteropServices.DllImportAttribute("ice_ipcsdk.dll", EntryPoint = "ICE_IPCSDK_EndTalk", CallingConvention = CallingConvention.Cdecl)]
        public static extern void ICE_IPCSDK_EndTalk(System.IntPtr hSDK);

        /**
         *  @brief  软触发
         *  @param  [IN]  hSDK          由连接相机接口获得的句柄
         *  @param  [OUT] pcNumber      车牌号
         *  @param  [OUT] pcColor       车牌颜色（"蓝色","黄色","白色","黑色",“绿色”）
         *  @param  [OUT] pcPicData     抓拍图片数据
         *  @param  [OUT] u32PicSize    抓拍图片缓冲区大小
         *  @param  [OUT] pu32PicLen    抓拍图片实际长度
         *  @return 1表示成功，0表示失败
         */
        [System.Runtime.InteropServices.DllImportAttribute("ice_ipcsdk.dll", EntryPoint = "ICE_IPCSDK_Trigger", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint ICE_IPCSDK_Trigger(System.IntPtr hSDK, StringBuilder pcNumber, StringBuilder pcColor, 
                                                    byte[] pcPicData, uint u32PicSize, ref uint pu32PicLen);

        /**
         *  @brief  软触发扩展接口
         *  @param  [IN]  hSDK          由连接相机接口获得的句柄
         *  @return 1表示成功，0表示失败
         */
        [System.Runtime.InteropServices.DllImportAttribute("ice_ipcsdk.dll", EntryPoint = "ICE_IPCSDK_TriggerExt", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint ICE_IPCSDK_TriggerExt(System.IntPtr hSDK);

        /**
         *  @brief  手动抓拍，不做识别，直接抓拍一张码流的图片
         *  @param  [IN]  hSDK          由连接相机接口获得的句柄
         *  @param  [OUT] pcPicData     抓拍图片数据
         *  @param  [OUT] u32PicSize    抓拍图片缓冲区大小
         *  @param  [OUT] pu32PicLen    抓拍图片实际长度
         *  @return 1表示成功，0表示失败
         */
        [System.Runtime.InteropServices.DllImportAttribute("ice_ipcsdk.dll", EntryPoint = "ICE_IPCSDK_Capture", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint ICE_IPCSDK_Capture(System.IntPtr hSDK, byte[] pcPicData, uint u32PicSize, ref uint pu32PicLen);

        /**
         *  @brief  获取相机连接状态
         *  @param  [IN] hSDK   由连接相机接口获得的句柄
         *  @return 1表示在线，0表示离线
         */
        [System.Runtime.InteropServices.DllImportAttribute("ice_ipcsdk.dll", EntryPoint = "ICE_IPCSDK_GetStatus", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint ICE_IPCSDK_GetStatus(System.IntPtr hSDK);

        /**
         *  @brief  重启相机
         *  @param  [IN] hSDK   由连接相机接口获得的句柄
         *  @return 1表示成功，0表示失败
         */
        [System.Runtime.InteropServices.DllImportAttribute("ice_ipcsdk.dll", EntryPoint = "ICE_IPCSDK_Reboot", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint ICE_IPCSDK_Reboot(System.IntPtr hSDK);

        /**
         *  @brief  时间同步
         *  @param  [IN] hSDK       由连接相机接口获得的句柄
         *  @param  [IN] u16Year    年
         *  @param  [IN] u8Month    月
         *  @param  [IN] u8Day      日
         *  @param  [IN] u8Hour     时
         *  @param  [IN] u8Min      分
         *  @param  [IN] u8Sec      秒
         *  @return 1表示成功，0表示失败
         */
        [System.Runtime.InteropServices.DllImportAttribute("ice_ipcsdk.dll", EntryPoint = "ICE_IPCSDK_SyncTime", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint ICE_IPCSDK_SyncTime(System.IntPtr hSDK, ushort u16Year, byte u8Month, byte u8Day, 
                                                        byte u8Hour, byte u8Min, byte u8Sec);

        /**
         *  @brief  发送RS485串口数据
         *  @param  [IN] hSDK      由连接相机接口获得的句柄
         *  @param  [IN] pcData    RS485串口数据
         *  @param  [IN] u32Len    串口数据长度
         *  @return 1表示成功，0表示失败
         */
        [System.Runtime.InteropServices.DllImportAttribute("ice_ipcsdk.dll", EntryPoint = "ICE_IPCSDK_TransSerialPort", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint ICE_IPCSDK_TransSerialPort(System.IntPtr hSDK, string pcData, uint u32Len);

        /**
         *  @brief  发送RS232串口数据
         *  @param  [IN] hSDK      由连接相机接口获得的句柄
         *  @param  [IN] pcData    RS232串口数据
         *  @param  [IN] u32Len    串口数据长度
         *  @return 1表示成功，0表示失败
         */
        [System.Runtime.InteropServices.DllImportAttribute("ice_ipcsdk.dll", EntryPoint = "ICE_IPCSDK_TransSerialPort_RS232", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint ICE_IPCSDK_TransSerialPort_RS232(System.IntPtr hSDK, string pcData, uint u32Len);

        /**
         *  @brief  获取相机mac地址
         *  @param  [IN] hSDK      由连接相机接口获得的句柄
         *  @param  [OUT] szDevID  相机mac地址
         *  @return 1表示成功，0表示失败
         */
        [System.Runtime.InteropServices.DllImportAttribute("ice_ipcsdk.dll", EntryPoint = "ICE_IPCSDK_GetDevID", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint ICE_IPCSDK_GetDevID(System.IntPtr hSDK, StringBuilder szDevID);

        /**
         *  @brief  开始录像
         *  @param  [IN] hSDK        由连接相机接口获得的句柄
         *  @param  [IN] pcFileName  保存录像的文件名
         *  @return 1表示成功，0表示失败
         */
        [System.Runtime.InteropServices.DllImportAttribute("ice_ipcsdk.dll", EntryPoint = "ICE_IPCSDK_StartRecord", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint ICE_IPCSDK_StartRecord(System.IntPtr hSDK, [System.Runtime.InteropServices.InAttribute()] [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.LPStr)] string pcFileName);

        /**
         *  @brief  结束录像
         *  @param  [IN] hSDK   由连接相机接口获得的句柄
         *  @return void
         */
        [System.Runtime.InteropServices.DllImportAttribute("ice_ipcsdk.dll", EntryPoint = "ICE_IPCSDK_StopRecord", CallingConvention = CallingConvention.Cdecl)]
        public static extern void ICE_IPCSDK_StopRecord(System.IntPtr hSDK);

        /**
         *  @brief  设置OSD参数
         *  @param  [IN] hSDK       由连接相机接口获得的句柄
         *  @parame [IN] pstOSDAttr OSD参数结构体地址，详见ICE_OSDAttr_S
         *  @return 1表示成功，0表示失败
         */
        [System.Runtime.InteropServices.DllImportAttribute("ice_ipcsdk.dll", EntryPoint = "ICE_IPCSDK_SetOSDCfg", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint ICE_IPCSDK_SetOSDCfg(System.IntPtr hSDK, ref ICE_OSDAttr_S pstOSDAttr);

        /**
         *  @brief  写入用户数据
         *  @param  [IN] hSDK       由连接相机接口获得的句柄
         *  @parame [IN] pcData     需要写入的用户数据
         *  @return 1表示成功，0表示失败
         */
        [System.Runtime.InteropServices.DllImportAttribute("ice_ipcsdk.dll", EntryPoint = "ICE_IPCSDK_WriteUserData", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint ICE_IPCSDK_WriteUserData(System.IntPtr hSDK, 
            [System.Runtime.InteropServices.InAttribute()] [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.LPStr)] string pcData);

        /**
         *  @brief  读取用户数据
         *  @param  [IN] hSDK       由连接相机接口获得的句柄
         *  @parame [OUT] pcData    读取的用户数据
         *  @param  [IN] nSize      读出的数据的最大长度，即缓冲区大小
         *  @return 1表示成功，0表示失败
         */
        [System.Runtime.InteropServices.DllImportAttribute("ice_ipcsdk.dll", EntryPoint = "ICE_IPCSDK_ReadUserData", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint ICE_IPCSDK_ReadUserData(System.IntPtr hSDK, byte[] pcData, int nSize);

        /**
         *  @brief  写入二进制用户数据
         *  @param  [IN] hSDK       由连接相机接口获得的句柄
         *  @parame [IN] pcData     需要写入的用户数据
         *  @parame [IN] nOffset    偏移量
         *  @parame [IN] nLen       写入数据的长度
         *  @return 1表示成功，0表示失败
         */
        [System.Runtime.InteropServices.DllImportAttribute("ice_ipcsdk.dll", EntryPoint = "ICE_IPCSDK_WriteUserData_Binary", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint ICE_IPCSDK_WriteUserData_Binary(System.IntPtr hSDK,
            [System.Runtime.InteropServices.InAttribute()] [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.LPStr)] string pcData,
            uint nOffset, uint nLen);

        /**
         *  @brief  读取二进制用户数据
         *  @param  [IN] hSDK       由连接相机接口获得的句柄
         *  @parame [OUT] pcData    读取的用户数据
         *  @param  [IN] nSize      读出的数据的最大长度，即缓冲区大小
         *  @param  [IN] nOffset    读数据的偏移量
         *  @param  [IN] nLen       需要读出的数据的大小
         *  @return 1表示成功，0表示失败
         */
        [System.Runtime.InteropServices.DllImportAttribute("ice_ipcsdk.dll", EntryPoint = "ICE_IPCSDK_ReadUserData_Binary", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint ICE_IPCSDK_ReadUserData_Binary(System.IntPtr hSDK, byte[] pcData,
                                                                 uint nSize, uint nOffset, uint nLen);
        /**
         *  @brief  获取相机网络参数
         *  @param  [IN] hSDK       由连接相机接口获得的句柄
         *  @parame [OUT] pu32IP    相机ip
         *  @param  [OUT] nSize     相机掩码
         *  @param  [OUT] nOffset   相机网关
         *  @return 1表示成功，0表示失败
         */
        [System.Runtime.InteropServices.DllImportAttribute("ice_ipcsdk.dll", EntryPoint = "ICE_IPCSDK_GetIPAddr", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint ICE_IPCSDK_GetIPAddr(System.IntPtr hSDK, ref uint pu32IP, ref uint pu32Mask, ref uint pu32Gateway);

        /**
         *  @brief  设置相机网络参数
         *  @param  [IN] hSDK      由连接相机接口获得的句柄
         *  @parame [IN] pu32IP    相机ip
         *  @param  [IN] nSize     相机掩码
         *  @param  [IN] nOffset   相机网关
         *  @return 1表示成功，0表示失败
         */
        [System.Runtime.InteropServices.DllImportAttribute("ice_ipcsdk.dll", EntryPoint = "ICE_IPCSDK_SetIPAddr", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint ICE_IPCSDK_SetIPAddr(System.IntPtr hSDK, uint u32IP, uint u32Mask, uint u32Gateway);

        /**
         *  @brief  搜索区域网内相机
         *  @param  [OUT] szDevs   设备mac地址和ip地址的字符串
         *                         设备mac地址和ip地址的字符串，格式为：mac地址 ip地址 例如：00-00-00-00-00-00 192.168.55.150\r\n
         *  @return void
         */
        [System.Runtime.InteropServices.DllImportAttribute("ice_ipcsdk.dll", EntryPoint = "ICE_IPCSDK_SearchDev", CallingConvention = CallingConvention.Cdecl)]
        public static extern void ICE_IPCSDK_SearchDev(StringBuilder szDevs);

        /**
         *  @brief  记录日志配置
         *  @param  [IN] openLog    是否开启日志，0：不开启 1：开启
         *  @parame [IN] logPath    日志路径，默认为D:\
         *  @return void
         */
        [System.Runtime.InteropServices.DllImportAttribute("ice_ipcsdk.dll", EntryPoint = "ICE_IPCSDK_LogConfig", CallingConvention = CallingConvention.Cdecl)]
        public static extern void ICE_IPCSDK_LogConfig(int openLog, string logPath);

        /**
         *  @brief  语音播放，单条语音
         *  @param  [IN] hSDK      由连接相机接口获得的句柄
         *  @parame [IN] nIndex    语音文件索引号，详见《语音列表.txt》
         *  @return 1表示成功，0表示失败
         */
        [System.Runtime.InteropServices.DllImportAttribute("ice_ipcsdk.dll", EntryPoint = "ICE_IPCSDK_Broadcast", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint ICE_IPCSDK_Broadcast(System.IntPtr hSDK, ushort nIndex);

        /**
         *  @brief  语音组播
         *  @param  [IN] hSDK      由连接相机接口获得的句柄
         *  @parame [IN] nIndex    包含语音序号的字符串  注：中间可以用, ; \t \n或者空格分开；如：1 2 3 4或者1,2,3,4
         *                         语音文件索引号，详见《语音列表.txt》
         *  @return 1表示成功，0表示失败
         */
        [System.Runtime.InteropServices.DllImportAttribute("ice_ipcsdk.dll", EntryPoint = "ICE_IPCSDK_BroadcastGroup", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint ICE_IPCSDK_BroadcastGroup(System.IntPtr hSDK, 
            [System.Runtime.InteropServices.InAttribute()] [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.LPStr)] string pcIndex);

        /**
         *  @brief  设置优先城市
         *  @param  [IN] hSDK      由连接相机接口获得的句柄
         *  @parame [IN] u32Index  优先城市的索引号
         *  优先城市列表：（0  全国；1  北京；2  上海；3  天津；4  重庆；5  黑龙江；6  吉林；7  辽宁；8  内蒙古；9  河北；10 山东
                         11 山西；12 安徽；13 江苏；14 浙江；15 福建；16 广东；17 河南；18 江西；19 湖南；20 湖北；21 广西
                         22 海南；23 云南；24 贵州；25 四川；26 陕西；27 甘肃；28 宁夏；29 青海；30 西藏；31 新疆）

         *  @return 1表示成功，0表示失败
         */
        [System.Runtime.InteropServices.DllImportAttribute("ice_ipcsdk.dll", EntryPoint = "ICE_IPCSDK_SetCity", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint ICE_IPCSDK_SetCity(System.IntPtr hSDK, uint u32Index);

        /**
         *  @brief  特征码比较
         *  @param  [IN] _pfResFeat1    需要比较的特征码1
         *  @param  [IN] _iFeat1Len     特征码1的长度，目前需输入20
         *  @param  [IN] _pfReaFeat2    需要比较的特征码2
         *  @param  [IN] _iFeat2Len     特征码2的长度，目前需输入20
         *  @return  匹配度，范围：0-1，值越大越匹配
         */
        [System.Runtime.InteropServices.DllImportAttribute("ice_ipcsdk.dll", EntryPoint = "ICE_IPCSDK_VBR_CompareFeat", CallingConvention = CallingConvention.Cdecl)]
        public static extern float ICE_IPCSDK_VBR_CompareFeat(float[] _pfResFeat1, uint _iFeat1Len,
                                                              float[] _pfReaFeat2, uint _iFeat2Len);
    }
}
