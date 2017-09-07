using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ETCF
{
    public class DelegateState
    {
        //提示框信息显示
        public delegate void controllogtextCallBack(string msg);
        //提示框
        public static controllogtextCallBack controllogtext;

        //车辆图片显示
        public delegate void pictureBoxVehshowCallBack(string msg);

        //车辆号显示
        public delegate void plateNoshowCallBackplateNoshow(string s_OBUPlateNum, string s_OBUCarType, string s_CamPlateNum, string s_JGCarType, string s_Num);

        //车辆图片显示
        public static pictureBoxVehshowCallBack pictureBoxVehshow;

        //车辆号显示
        public static plateNoshowCallBackplateNoshow plateNoshow;

        //添加表格
        public delegate void adddataGridViewRollCallBack(string s_Id, string s_JgCarType, string s_RsuCarType, string s_RsuTradeTime, string s_JgTime, string s_RsuPlateNum, string s_CamPlateNum, string s_RsuPlateColor, string s_CamPlateColor, string s_Cambiao, string s_JgId, string s_JgLength, string s_JgWide, string s_CamPicPath);
        public delegate void InsertGridviewRollCallBack(string s_Id, string s_RsuTradeTime, string s_RsuPlateNum, string s_RsuCarType, string s_JgCarType, string s_IsZuobi,string s_JgLength, string s_JgWide, string s_CamPicPath);
        public delegate void updateataGridViewRollCallBack(string s_Id, string s_JgCarType, string s_JgTime, string s_CamPlateNum, string s_CamPlateColor, string s_Cambiao, string s_JgId, string s_JgLength, string s_JgWide, string s_CamPicPath);
        //添加表格
        public static adddataGridViewRollCallBack adddataGridViewRoll;
        public static InsertGridviewRollCallBack InsertGridview;
        //更新表格
        public static updateataGridViewRollCallBack updatedataGridViewRoll;
    }
}
