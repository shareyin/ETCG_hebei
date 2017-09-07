using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ETCF
{
    public class QueueJGData
    {
        public string qJGLength;
        public string qJGWide;
        public string qJGCarType;
        public string qJGId;
        public string qCamPlateNum;
        public string qCamPlateColor;
        public string qCamPicPath;
        public string qCambiao;
        public string qJGDateTime;
        public long qJGRandCode;
    }
    //public class QueueCam
    //{
    //    public string qCamPlateNum;
    //    public string qCamPlateColor;
    //    public string qCamPicPath;
    //    public string qCambiao;
    //    //public string qCam
    //}
    public class QueueRSUData
    {
        public string qCount;
        public string qOBUMac;
        public string qOBUY;
        public string qOBUBiao;
        public string qOBUPlateNum;
        public string qOBUPlateColor;
        public string qOBUCarType;
        public string qOBUDateTime;
        public string qOBUCarLength;
        public string qOBUCarhigh;
        public long qRSURandCode;

    }

    public class OBUList
    {
        public string sOBUPlateNumList { get; set; }
        public string sRSURandCodeList { get; set; }
        public string sOBUDateTime { get; set; }

        public OBUList(string s_PlateNum,string s_RandCode,string s_DateTime)
        {
            sOBUPlateNumList = s_PlateNum;
            sRSURandCodeList = s_RandCode;
            sOBUDateTime = s_DateTime;
        }
    }
    public class CarFullInfo
    {
        public string sOBUPlateNum { get; set; }
        public string sOBUCartype { get; set; }
        public string sRSURandCode { get; set; }
        public string sOBUDateTime { get; set; }
        public string sOBUCarLength { get; set; }
        public string sOBUCarHigh { get; set; }
        public string sOBUPlateColor { get; set; }
        public string sOBUMac { get; set; }
        public string sOBUY { get; set; }
        public string sOBUBiao { get; set; }

        public string sJGCarType { get; set; }
        public string sJGCarHigh { get; set; }
        public string sJGCarLength { get; set; }
        public string sJGDateTime { get; set; }
        public string sJGId { get; set; }
        public string sJGRandCode { get; set; }

        public string sCamPlateNum { get; set; }
        public string sCamPlateColor { get; set; }
        public string sCamBiao { get; set; }
        public string sCamPicPath { get; set; }

        public string sCount { get; set; }

        public CarFullInfo(string s_OBUPlateNum, string s_OBUcarType, string s_RSURandCode,
            string s_OBUDateTime, string s_OBUCarLength, string s_OBUCarHigh,string s_OBUY,string s_OBUBiao,
            string s_OBUPlateColor, string s_OBUMac,string s_JGCarType,
            string s_JGCarHigh, string s_JGCarLength, string s_JGDateTime,
            string s_JGId, string s_CamPlateNum, string s_CamPlateColor,
            string s_CamBiao, string s_CamPicPath,string s_JGRandCode,string s_Count)
        {
            sOBUPlateNum = s_OBUPlateNum;
            sOBUCartype = s_OBUcarType;
            sRSURandCode = s_RSURandCode;
            sOBUDateTime = s_OBUDateTime;
            sOBUCarLength = s_OBUCarLength;
            sOBUCarHigh = s_OBUCarHigh;
            sOBUY = s_OBUY;
            sOBUBiao = s_OBUBiao;
            sOBUPlateColor = s_OBUPlateColor;
            sOBUMac = s_OBUMac; 
            sJGCarType = s_JGCarType;
            sJGCarHigh = s_JGCarHigh;
            sJGCarLength = s_JGCarLength;
            sJGDateTime = s_JGDateTime;
            sJGId = s_JGId;
            sCamPlateNum = s_CamPlateNum;
            sCamPlateColor = s_CamPlateColor;
            sCamBiao = s_CamBiao;
            sCamPicPath = s_CamPicPath;
            sJGRandCode = s_JGRandCode;
            sCount = s_Count;


        }

    }
    public class CamList
    {
        public string qJGID { get; set; }
        public string qCamPlateNum { get; set; }
        public string qCamPlateColor { get; set; }
        public string qCamPicPath { get; set; }
        public string qCambiao { get; set; }

         public CamList(string s_JgId, string s_CamPlateNum, string s_CamPlateColor, string s_CamPicPath, string s_Cambiao)
        {
            qJGID = s_JgId;
            qCamPlateNum = s_CamPlateNum;
            qCamPlateColor = s_CamPlateColor;
            qCamPicPath = s_CamPicPath;
            qCambiao = s_Cambiao;

        }
    }

}
