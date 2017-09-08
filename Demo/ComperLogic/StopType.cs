using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ETCF
{
    class StopType
    {
         #region 全局变量
        public FormDemo MF = null;
        bool isInRSUSql = false;
        bool isMarch = false;
        string sZuobistring = "";
        List<CarFullInfo> listAllCarInfo = new List<CarFullInfo>();
        Levenshtein LevenPercent = new Levenshtein();
        string InsertString = "";
        string MarchFunction = "";
        #endregion

        public StopType(FormDemo mf)
        {
            if (MF == null)
            {
                MF = mf;
            }
        }

        #region ******拦截模式匹配逻辑******
        //拦截模式
        public void StartStoptype(System.Collections.Concurrent.ConcurrentQueue<QueueRSUData> qRSUData, 
            System.Collections.Concurrent.ConcurrentQueue<QueueJGData> qJGData,
            string sql_dbname)
        {
            try
            {
                QueueRSUData qoutRSU = new QueueRSUData();
                QueueJGData qoutJG = new QueueJGData();
                //先取激光数据，放入ListAllCarInfo里面
                if (qJGData.TryDequeue(out qoutJG))
                {
                    Log.WritePlateLog(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "激光数据81帧出栈完成 " + "\r\n");
                    //入库激光数据表
                    GlobalMember.SQLInter.InsertJGData(qoutJG.qJGLength, qoutJG.qJGWide, qoutJG.qJGCarType,
                        qoutJG.qJGId, qoutJG.qCamPlateNum, qoutJG.qCamPicPath, qoutJG.qJGDateTime,
                        qoutJG.qCambiao, qoutJG.qCamPlateColor, qoutJG.qJGRandCode.ToString("X2"));
                   //查找能否匹配到RSU数据，一般来讲，RSU数据应该后到，为了防止RSU数据先到
                    for (int i = listAllCarInfo.Count - 1; i >= 0; i--)
                    {
                        if (listAllCarInfo[i].sOBUPlateNum == qoutJG.qCamPlateNum
                            || (MF.OpenMohu.Checked && (((int)(LevenPercent.LevenshteinDistancePercent(listAllCarInfo[i].sOBUPlateNum, qoutJG.qCamPlateNum) * 100)) > 70)))
                        {
                            if (listAllCarInfo[i].sOBUPlateNum == qoutJG.qCamPlateNum)
                            {
                                MarchFunction = "车牌匹配";
                            }
                            else if ((MF.OpenMohu.Checked && (((int)(LevenPercent.LevenshteinDistancePercent(listAllCarInfo[i].sOBUPlateNum, qoutJG.qCamPlateNum) * 100)) > 70)))
                            {
                                MarchFunction = "模糊匹配";
                            }
                            listAllCarInfo[i].sCamBiao = qoutJG.qCambiao;
                            listAllCarInfo[i].sCamPicPath = qoutJG.qCamPicPath;
                            listAllCarInfo[i].sCamPlateColor = qoutJG.qCamPlateColor;
                            listAllCarInfo[i].sCamPlateNum = qoutJG.qCamPlateNum;
                            listAllCarInfo[i].sJGCarHigh = qoutJG.qJGWide;
                            listAllCarInfo[i].sJGCarLength = qoutJG.qJGLength;
                            listAllCarInfo[i].sJGCarType = qoutJG.qJGCarType;
                            listAllCarInfo[i].sJGDateTime = qoutJG.qJGDateTime;
                            listAllCarInfo[i].sJGId = qoutJG.qJGId;
                            listAllCarInfo[i].sJGRandCode = qoutJG.qJGRandCode.ToString("X2");
                            isMarch = true;

                            //界面显示
                            sZuobistring = MF.MarchedShow(listAllCarInfo[i].sOBUCartype, listAllCarInfo[i].sOBUPlateNum, listAllCarInfo[i].sJGCarType, listAllCarInfo[i].sCamPlateNum, listAllCarInfo[i].sCamPicPath, listAllCarInfo[i].sCount, listAllCarInfo[i]);
                            //表格显示
                            MF.adddataGridViewRoll(listAllCarInfo[i].sCount, listAllCarInfo[i].sJGCarType, listAllCarInfo[i].sOBUCartype,
                                listAllCarInfo[i].sOBUDateTime, listAllCarInfo[i].sJGDateTime, listAllCarInfo[i].sOBUPlateNum,
                                listAllCarInfo[i].sCamPlateNum, listAllCarInfo[i].sOBUPlateColor, listAllCarInfo[i].sCamPlateColor,
                                listAllCarInfo[i].sCamBiao, listAllCarInfo[i].sJGId, listAllCarInfo[i].sOBUCarLength, listAllCarInfo[i].sOBUCarHigh, listAllCarInfo[i].sCamPicPath);
                            //写入总数据库
                            InsertString = @"Insert into " + sql_dbname
                                + ".dbo.CarInfo(JGLength,JGWide,JGCarType,ForceTime,CamPlateColor,CamPlateNum,Cambiao,CamPicPath,JGId,OBUPlateColor,OBUPlateNum,OBUMac,OBUY,OBUBiao,OBUCarLength,OBUCarHigh,OBUCarType,TradeTime,TradeState,RandCode,GetFunction) values('"
                                + listAllCarInfo[i].sJGCarLength + "','" + listAllCarInfo[i].sJGCarHigh + "','" + listAllCarInfo[i].sJGCarType + "','"
                                + listAllCarInfo[i].sJGDateTime + "','" + listAllCarInfo[i].sCamPlateColor + "','" + listAllCarInfo[i].sCamPlateNum + "','"
                                + listAllCarInfo[i].sCamBiao + "','" + listAllCarInfo[i].sCamPicPath + "','" + listAllCarInfo[i].sJGId + "','"
                                + listAllCarInfo[i].sOBUPlateColor + "','" + listAllCarInfo[i].sOBUPlateNum + "','" + listAllCarInfo[i].sOBUMac + "','"
                                + listAllCarInfo[i].sOBUY + "','" + listAllCarInfo[i].sOBUBiao + "','" + listAllCarInfo[i].sOBUCarLength + "','" + listAllCarInfo[i].sOBUCarHigh + "','"
                                + listAllCarInfo[i].sOBUCartype + "','" + listAllCarInfo[i].sOBUDateTime + "','" + sZuobistring + "','"
                                + listAllCarInfo[i].sRSURandCode + "','" + MarchFunction + "')";
                            GlobalMember.SQLInter.UpdateSQLData(InsertString);
                            listAllCarInfo.RemoveAt(i);
                            Log.WritePlateLog(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + MarchFunction + "成功" + "识别车牌：" + qoutJG.qCamPlateNum + "\r\n");

                            break;
                        }
                    }
                    if (!isMarch)
                    {
                        if (listAllCarInfo.Count >= 5)
                        {
                            //写入总数据库
                            InsertString = @"Insert into " + sql_dbname
                                + ".dbo.CarInfo(JGLength,JGWide,JGCarType,ForceTime,CamPlateColor,CamPlateNum,Cambiao,CamPicPath,JGId,OBUPlateColor,OBUPlateNum,OBUMac,OBUY,OBUBiao,OBUCarLength,OBUCarHigh,OBUCarType,TradeTime,TradeState,RandCode,GetFunction) values('"
                                + listAllCarInfo[0].sJGCarLength + "','" + listAllCarInfo[0].sJGCarHigh + "','" + listAllCarInfo[0].sJGCarType + "','"
                                + listAllCarInfo[0].sJGDateTime + "','" + listAllCarInfo[0].sCamPlateColor + "','" + listAllCarInfo[0].sCamPlateNum + "','"
                                + listAllCarInfo[0].sCamBiao + "','" + listAllCarInfo[0].sCamPicPath + "','" + listAllCarInfo[0].sJGId + "','"
                                + listAllCarInfo[0].sOBUPlateColor + "','" + listAllCarInfo[0].sOBUPlateNum + "','" + listAllCarInfo[0].sOBUMac + "','"
                                + listAllCarInfo[0].sOBUY + "','" + listAllCarInfo[0].sOBUBiao + "','" + listAllCarInfo[0].sOBUCarLength + "','" + listAllCarInfo[0].sOBUCarHigh + "','"
                                + listAllCarInfo[0].sOBUCartype + "','" + listAllCarInfo[0].sOBUDateTime + "','" + "未知" + "','"
                                + listAllCarInfo[0].sRSURandCode + "','" + "未能匹配" + "')";
                            GlobalMember.SQLInter.UpdateSQLData(InsertString);
                            listAllCarInfo.RemoveAt(0);
                            Log.WritePlateLog(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + " RSU触发队列已满，首位清空" + "OBU车牌：" + listAllCarInfo[0].sOBUPlateNum + "识别车牌：" + listAllCarInfo[0].sCamPlateNum + "\r\n");
                        }
                        listAllCarInfo.Add(new CarFullInfo("", "", "", "", "", "", "", "", "", "", qoutJG.qJGCarType,
                            qoutJG.qJGWide, qoutJG.qJGLength, qoutJG.qJGDateTime, qoutJG.qJGId, qoutJG.qCamPlateNum,
                            qoutJG.qCamPlateColor, qoutJG.qCambiao, qoutJG.qCamPicPath, qoutJG.qJGRandCode.ToString("X2"), ""));
                        Log.WritePlateLog(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + " JG入队列" + "车牌：" + qoutJG.qCamPlateNum + "\r\n");
                    }
                    else
                    {
                        isMarch = false;
                        Log.WritePlateLog(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + " JG匹配RSU车牌成功，跟随码" + qoutJG.qJGRandCode.ToString("X2") + "车牌：" + qoutJG.qCamPlateNum + "入库Car表\r\n");
                    }
                }
                //后取ETC的数据
                if (qRSUData.TryDequeue(out qoutRSU))
                {
                    Log.WritePlateLog(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + " 天线数据出栈完成 " + "\r\n");
                    for (int i = listAllCarInfo.Count - 1; i >= 0; i--)
                    {
                        //匹配规则
                        //1.车牌完全相同 2.位置相同且车牌是未识别 3.开启位置匹配且位置相同
                        //4.开启车牌模糊匹配且匹配度大于70%(由于车牌只有七位，三位不一致的时候相似度只有57%，)
                        //模糊匹配算法为Levenstein算法修改版，适用于字节丢失匹配
                        if (listAllCarInfo[i].sCamPlateNum == qoutRSU.qOBUPlateNum
                            || (MF.OpenMohu.Checked && (((int)(LevenPercent.LevenshteinDistancePercent(listAllCarInfo[i].sCamPlateNum, qoutRSU.qOBUPlateNum) * 100)) > 70)))
                        {
                            if (listAllCarInfo[i].sCamPlateNum == qoutRSU.qOBUPlateNum)
                            {
                                MarchFunction = "车牌匹配";
                            }
                            else if ((MF.OpenMohu.Checked && (((int)(LevenPercent.LevenshteinDistancePercent(listAllCarInfo[i].sCamPlateNum, qoutRSU.qOBUPlateNum) * 100)) > 70)))
                            {
                                MarchFunction = "模糊匹配";
                            }
                            listAllCarInfo[i].sOBUCarHigh = qoutRSU.qOBUCarhigh;
                            listAllCarInfo[i].sOBUCarLength = qoutRSU.qOBUCarLength;
                            listAllCarInfo[i].sOBUCartype = qoutRSU.qOBUCarType;
                            listAllCarInfo[i].sOBUDateTime = qoutRSU.qOBUDateTime;
                            listAllCarInfo[i].sOBUMac = qoutRSU.qOBUMac;
                            listAllCarInfo[i].sOBUPlateColor = qoutRSU.qOBUPlateColor;
                            listAllCarInfo[i].sOBUPlateNum = qoutRSU.qOBUPlateNum;
                            listAllCarInfo[i].sOBUY = qoutRSU.qOBUY;
                            listAllCarInfo[i].sOBUBiao = qoutRSU.qOBUBiao;
                            listAllCarInfo[i].sRSURandCode = qoutRSU.qRSURandCode.ToString("X2");
                            listAllCarInfo[i].sCount = qoutRSU.qCount;
                            isMarch = true;
                            
                            //界面显示
                            sZuobistring = MF.MarchedShow(listAllCarInfo[i].sOBUCartype, listAllCarInfo[i].sOBUPlateNum, listAllCarInfo[i].sJGCarType, listAllCarInfo[i].sCamPlateNum, listAllCarInfo[i].sCamPicPath, listAllCarInfo[i].sCount, listAllCarInfo[i]);
                            //表格显示
                            MF.adddataGridViewRoll(listAllCarInfo[i].sCount, listAllCarInfo[i].sJGCarType, listAllCarInfo[i].sOBUCartype,
                                listAllCarInfo[i].sOBUDateTime, listAllCarInfo[i].sJGDateTime, listAllCarInfo[i].sOBUPlateNum,
                                listAllCarInfo[i].sCamPlateNum, listAllCarInfo[i].sOBUPlateColor, listAllCarInfo[i].sCamPlateColor,
                                listAllCarInfo[i].sCamBiao, listAllCarInfo[i].sJGId, listAllCarInfo[i].sOBUCarLength, listAllCarInfo[i].sOBUCarHigh, listAllCarInfo[i].sCamPicPath);
                            //更新数据库
                            //写入总数据库
                            InsertString = @"Insert into " + sql_dbname
                                + ".dbo.CarInfo(JGLength,JGWide,JGCarType,ForceTime,CamPlateColor,CamPlateNum,Cambiao,CamPicPath,JGId,OBUPlateColor,OBUPlateNum,OBUMac,OBUY,OBUBiao,OBUCarLength,OBUCarHigh,OBUCarType,TradeTime,TradeState,RandCode,GetFunction) values('"
                                + listAllCarInfo[i].sJGCarLength + "','" + listAllCarInfo[i].sJGCarHigh + "','" + listAllCarInfo[i].sJGCarType + "','"
                                + listAllCarInfo[i].sJGDateTime + "','" + listAllCarInfo[i].sCamPlateColor + "','" + listAllCarInfo[i].sCamPlateNum + "','"
                                + listAllCarInfo[i].sCamBiao + "','" + listAllCarInfo[i].sCamPicPath + "','" + listAllCarInfo[i].sJGId + "','"
                                + listAllCarInfo[i].sOBUPlateColor + "','" + listAllCarInfo[i].sOBUPlateNum + "','" + listAllCarInfo[i].sOBUMac + "','"
                                + listAllCarInfo[i].sOBUY + "','" + listAllCarInfo[i].sOBUBiao + "','" + listAllCarInfo[i].sOBUCarLength + "','" + listAllCarInfo[i].sOBUCarHigh + "','"
                                + listAllCarInfo[i].sOBUCartype + "','" + listAllCarInfo[i].sOBUDateTime + "','" + sZuobistring + "','"
                                + listAllCarInfo[i].sRSURandCode + "','" + "车牌匹配" + "')";
                            GlobalMember.SQLInter.UpdateSQLData(InsertString);
                            listAllCarInfo.RemoveAt(i);
                            Log.WritePlateLog(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + MarchFunction + "成功" + qoutRSU.qRSURandCode.ToString("X2") + "OBU车牌：" + qoutRSU.qOBUPlateNum + "\r\n");
                            break;
                        }
                    }

                    if (!isMarch)
                    {
                        //MF.TcpReply(0xD6, 0x00, MF.RSUTcpClient);
                        if (listAllCarInfo.Count >= 5)
                        {
                            //写入总数据库
                            InsertString = @"Insert into " + sql_dbname
                                + ".dbo.CarInfo(JGLength,JGWide,JGCarType,ForceTime,CamPlateColor,CamPlateNum,Cambiao,CamPicPath,JGId,OBUPlateColor,OBUPlateNum,OBUMac,OBUY,OBUBiao,OBUCarLength,OBUCarHigh,OBUCarType,TradeTime,TradeState,RandCode,GetFunction) values('"
                                + listAllCarInfo[0].sJGCarLength + "','" + listAllCarInfo[0].sJGCarHigh + "','" + listAllCarInfo[0].sJGCarType + "','"
                                + listAllCarInfo[0].sJGDateTime + "','" + listAllCarInfo[0].sCamPlateColor + "','" + listAllCarInfo[0].sCamPlateNum + "','"
                                + listAllCarInfo[0].sCamBiao + "','" + listAllCarInfo[0].sCamPicPath + "','" + listAllCarInfo[0].sJGId + "','"
                                + listAllCarInfo[0].sOBUPlateColor + "','" + listAllCarInfo[0].sOBUPlateNum + "','" + listAllCarInfo[0].sOBUMac + "','"
                                + listAllCarInfo[0].sOBUY + "','" + listAllCarInfo[0].sOBUBiao + "','" + listAllCarInfo[0].sOBUCarLength + "','" + listAllCarInfo[0].sOBUCarHigh + "','"
                                + listAllCarInfo[0].sOBUCartype + "','" + listAllCarInfo[0].sOBUDateTime + "','" + "未知" + "','"
                                + listAllCarInfo[0].sRSURandCode + "','" + "未能匹配" + "')";
                            GlobalMember.SQLInter.UpdateSQLData(InsertString);
                            listAllCarInfo.RemoveAt(0);
                            Log.WritePlateLog(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + " RSU触发队列已满，首位清空" + "OBU车牌：" + listAllCarInfo[0].sOBUPlateNum + "识别车牌：" + listAllCarInfo[0].sCamPlateNum + "\r\n");
                        }
                        listAllCarInfo.Add(new CarFullInfo(qoutRSU.qOBUPlateNum, qoutRSU.qOBUCarType, qoutRSU.qRSURandCode.ToString("X2"), qoutRSU.qOBUDateTime,
                            qoutRSU.qOBUCarLength, qoutRSU.qOBUCarhigh, qoutRSU.qOBUY, qoutRSU.qOBUBiao, qoutRSU.qOBUPlateColor, qoutRSU.qOBUMac,
                            "", "", "", "", "", "", "", "", "", "", qoutRSU.qCount));
                        Log.WritePlateLog(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + " RSU入队列" + "车牌：" + qoutRSU.qOBUPlateNum + "\r\n");
                    }
                    else
                    {
                        isMarch = false;
                        Log.WritePlateLog(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + " RSU匹配JG车牌成功" + "车牌：" + qoutRSU.qOBUPlateNum + "入库Car表\r\n");

                    }

                    //为了保证时间，RSU数据最后入库
                    isInRSUSql = GlobalMember.SQLInter.InsertRSUData(qoutRSU.qOBUPlateColor, qoutRSU.qOBUPlateNum,
                        qoutRSU.qOBUMac, qoutRSU.qOBUY, qoutRSU.qOBUBiao, qoutRSU.qOBUCarLength, qoutRSU.qOBUCarhigh,
                        qoutRSU.qOBUCarType, qoutRSU.qOBUDateTime, qoutRSU.qRSURandCode.ToString("X2"));
                    if (!isInRSUSql)
                    {
                        //异常
                    }
                }
                


            }
            catch (Exception ex)
            {
                Log.WriteLog(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + " 数据匹配异常\r\n" + ex.ToString() + "\r\n");
            }
        }
        #endregion

        #region ******稽查模式匹配逻辑******
        //稽查模式
        public void StartJiChaType(System.Collections.Concurrent.ConcurrentQueue<QueueRSUData> qRSUData,
            System.Collections.Concurrent.ConcurrentQueue<QueueJGData> qJGData,
            string sql_dbname)
        {
            try
            {
                QueueRSUData qoutRSU = new QueueRSUData();
                QueueJGData qoutJG = new QueueJGData();
                //先取ETC的数据
                if (qRSUData.TryDequeue(out qoutRSU))
                {
                    //先进行RSU数据存储
                    isInRSUSql = GlobalMember.SQLInter.InsertRSUData(qoutRSU.qOBUPlateColor, qoutRSU.qOBUPlateNum,
                        qoutRSU.qOBUMac, qoutRSU.qOBUY, qoutRSU.qOBUBiao, qoutRSU.qOBUCarLength, qoutRSU.qOBUCarhigh,
                        qoutRSU.qOBUCarType, qoutRSU.qOBUDateTime, qoutRSU.qRSURandCode.ToString("X2"));
                    if (!isInRSUSql)
                    {
                        //异常
                    }
                    for (int i = listAllCarInfo.Count - 1; i >= 0; i--)
                    {
                        //匹配规则
                        //1.车牌完全相同 2.位置相同且车牌是未识别 3.开启位置匹配且位置相同
                        //4.开启车牌模糊匹配且匹配度大于70%(由于车牌只有七位，三位不一致的时候相似度只有57%，)
                        //模糊匹配算法为Levenstein算法修改版，适用于字节丢失匹配
                        if (listAllCarInfo[i].sCamPlateNum == qoutRSU.qOBUPlateNum
                            || ((listAllCarInfo[i].sCamPlateNum == "未知" || listAllCarInfo[i].sCamPlateNum == "未检测")
                            && listAllCarInfo[i].sJGRandCode == qoutRSU.qRSURandCode.ToString("X2"))
                            || (MF.OpenLocation.Checked && (listAllCarInfo[i].sJGRandCode == qoutRSU.qRSURandCode.ToString("X2")))
                            || (MF.OpenMohu.Checked && (((int)(LevenPercent.LevenshteinDistancePercent(listAllCarInfo[i].sCamPlateNum, qoutRSU.qOBUPlateNum) * 100)) > 70)))
                        {
                            if (listAllCarInfo[i].sCamPlateNum == qoutRSU.qOBUPlateNum)
                            {
                                MarchFunction = "车牌匹配";
                            }
                            else if (((listAllCarInfo[i].sCamPlateNum == "未知" || listAllCarInfo[i].sCamPlateNum == "未检测")
                            && listAllCarInfo[i].sJGRandCode == qoutRSU.qRSURandCode.ToString("X2")))
                            {
                                MarchFunction = "位置匹配1";
                            }
                            else if ((MF.OpenLocation.Checked && (listAllCarInfo[i].sJGRandCode == qoutRSU.qRSURandCode.ToString("X2"))))
                            {
                                MarchFunction = "位置匹配2";
                            }
                            else if ((MF.OpenMohu.Checked && (((int)(LevenPercent.LevenshteinDistancePercent(listAllCarInfo[i].sCamPlateNum, qoutRSU.qOBUPlateNum) * 100)) > 70)))
                            {
                                MarchFunction = "模糊匹配";
                            }
                            listAllCarInfo[i].sOBUCarHigh = qoutRSU.qOBUCarhigh;
                            listAllCarInfo[i].sOBUCarLength = qoutRSU.qOBUCarLength;
                            listAllCarInfo[i].sOBUCartype = qoutRSU.qOBUCarType;
                            listAllCarInfo[i].sOBUDateTime = qoutRSU.qOBUDateTime;
                            listAllCarInfo[i].sOBUMac = qoutRSU.qOBUMac;
                            listAllCarInfo[i].sOBUPlateColor = qoutRSU.qOBUPlateColor;
                            listAllCarInfo[i].sOBUPlateNum = qoutRSU.qOBUPlateNum;
                            listAllCarInfo[i].sOBUY = qoutRSU.qOBUY;
                            listAllCarInfo[i].sOBUBiao = qoutRSU.qOBUBiao;
                            listAllCarInfo[i].sRSURandCode = qoutRSU.qRSURandCode.ToString("X2");
                            listAllCarInfo[i].sCount = qoutRSU.qCount;
                            isMarch = true;
                            //界面显示
                            sZuobistring = MF.MarchedShow(listAllCarInfo[i].sOBUCartype, listAllCarInfo[i].sOBUPlateNum, listAllCarInfo[i].sJGCarType, listAllCarInfo[i].sCamPlateNum, listAllCarInfo[i].sCamPicPath, listAllCarInfo[i].sCount, listAllCarInfo[i]);
                            //表格显示
                            MF.adddataGridViewRoll(listAllCarInfo[i].sCount, listAllCarInfo[i].sJGCarType, listAllCarInfo[i].sOBUCartype,
                                listAllCarInfo[i].sOBUDateTime, listAllCarInfo[i].sJGDateTime, listAllCarInfo[i].sOBUPlateNum,
                                listAllCarInfo[i].sCamPlateNum, listAllCarInfo[i].sOBUPlateColor, listAllCarInfo[i].sCamPlateColor,
                                listAllCarInfo[i].sCamBiao, listAllCarInfo[i].sJGId, listAllCarInfo[i].sOBUCarLength, listAllCarInfo[i].sOBUCarHigh, listAllCarInfo[i].sCamPicPath);
                            //更新数据库
                            //写入总数据库
                            InsertString = @"Insert into " + sql_dbname
                                + ".dbo.CarInfo(JGLength,JGWide,JGCarType,ForceTime,CamPlateColor,CamPlateNum,Cambiao,CamPicPath,JGId,OBUPlateColor,OBUPlateNum,OBUMac,OBUY,OBUBiao,OBUCarLength,OBUCarHigh,OBUCarType,TradeTime,TradeState,RandCode,GetFunction) values('"
                                + listAllCarInfo[i].sJGCarLength + "','" + listAllCarInfo[i].sJGCarHigh + "','" + listAllCarInfo[i].sJGCarType + "','"
                                + listAllCarInfo[i].sJGDateTime + "','" + listAllCarInfo[i].sCamPlateColor + "','" + listAllCarInfo[i].sCamPlateNum + "','"
                                + listAllCarInfo[i].sCamBiao + "','" + listAllCarInfo[i].sCamPicPath + "','" + listAllCarInfo[i].sJGId + "','"
                                + listAllCarInfo[i].sOBUPlateColor + "','" + listAllCarInfo[i].sOBUPlateNum + "','" + listAllCarInfo[i].sOBUMac + "','"
                                + listAllCarInfo[i].sOBUY + "','" + listAllCarInfo[i].sOBUBiao + "','" + listAllCarInfo[i].sOBUCarLength + "','" + listAllCarInfo[i].sOBUCarHigh + "','"
                                + listAllCarInfo[i].sOBUCartype + "','" + listAllCarInfo[i].sOBUDateTime + "','" + sZuobistring + "','"
                                + listAllCarInfo[i].sRSURandCode + "','" + "车牌匹配" + "')";
                            GlobalMember.SQLInter.UpdateSQLData(InsertString);
                            listAllCarInfo.RemoveAt(i);
                            Log.WritePlateLog(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + MarchFunction + "成功" + qoutRSU.qRSURandCode.ToString("X2") + "OBU车牌：" + qoutRSU.qOBUPlateNum + "\r\n");
                            break;
                        }
                    }

                    if (!isMarch)
                    {
                        if (listAllCarInfo.Count >= 6)
                        {
                            //写入总数据库
                            InsertString = @"Insert into " + sql_dbname
                                + ".dbo.CarInfo(JGLength,JGWide,JGCarType,ForceTime,CamPlateColor,CamPlateNum,Cambiao,CamPicPath,JGId,OBUPlateColor,OBUPlateNum,OBUMac,OBUY,OBUBiao,OBUCarLength,OBUCarHigh,OBUCarType,TradeTime,TradeState,RandCode,GetFunction) values('"
                                + listAllCarInfo[0].sJGCarLength + "','" + listAllCarInfo[0].sJGCarHigh + "','" + listAllCarInfo[0].sJGCarType + "','"
                                + listAllCarInfo[0].sJGDateTime + "','" + listAllCarInfo[0].sCamPlateColor + "','" + listAllCarInfo[0].sCamPlateNum + "','"
                                + listAllCarInfo[0].sCamBiao + "','" + listAllCarInfo[0].sCamPicPath + "','" + listAllCarInfo[0].sJGId + "','"
                                + listAllCarInfo[0].sOBUPlateColor + "','" + listAllCarInfo[0].sOBUPlateNum + "','" + listAllCarInfo[0].sOBUMac + "','"
                                + listAllCarInfo[0].sOBUY + "','" + listAllCarInfo[0].sOBUBiao + "','" + listAllCarInfo[0].sOBUCarLength + "','" + listAllCarInfo[0].sOBUCarHigh + "','"
                                + listAllCarInfo[0].sOBUCartype + "','" + listAllCarInfo[0].sOBUDateTime + "','" + "未知" + "','"
                                + listAllCarInfo[0].sRSURandCode + "','" + "未能匹配" + "')";
                            GlobalMember.SQLInter.UpdateSQLData(InsertString);
                            listAllCarInfo.RemoveAt(0);
                            Log.WritePlateLog(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + " RSU触发队列已满，首位清空" + "OBU车牌：" + listAllCarInfo[0].sOBUPlateNum + "识别车牌：" + listAllCarInfo[0].sCamPlateNum + "\r\n");
                        }
                        listAllCarInfo.Add(new CarFullInfo(qoutRSU.qOBUPlateNum, qoutRSU.qOBUCarType, qoutRSU.qRSURandCode.ToString("X2"), qoutRSU.qOBUDateTime,
                            qoutRSU.qOBUCarLength, qoutRSU.qOBUCarhigh, qoutRSU.qOBUY, qoutRSU.qOBUBiao, qoutRSU.qOBUPlateColor, qoutRSU.qOBUMac,
                            "", "", "", "", "", "", "", "", "", "", qoutRSU.qCount));
                        Log.WritePlateLog(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + " RSU入队列" + "车牌：" + qoutRSU.qOBUPlateNum + "\r\n");
                    }
                    else
                    {
                        isMarch = false;
                        Log.WritePlateLog(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + " RSU匹配JG车牌成功" + "车牌：" + qoutRSU.qOBUPlateNum + "入库Car表\r\n");

                    }
                }
                if (qJGData.TryDequeue(out qoutJG))
                {
                    Log.WritePlateLog(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "激光数据81帧出栈完成 " + "\r\n");
                    //入库
                    GlobalMember.SQLInter.InsertJGData(qoutJG.qJGLength, qoutJG.qJGWide, qoutJG.qJGCarType,
                        qoutJG.qJGId, qoutJG.qCamPlateNum, qoutJG.qCamPicPath, qoutJG.qJGDateTime,
                        qoutJG.qCambiao, qoutJG.qCamPlateColor, qoutJG.qJGRandCode.ToString("X2"));
                    for (int i = listAllCarInfo.Count - 1; i >= 0; i--)
                    {
                        if (listAllCarInfo[i].sOBUPlateNum == qoutJG.qCamPlateNum
                            || ((listAllCarInfo[i].sRSURandCode == qoutJG.qJGRandCode.ToString("X2")) && qoutJG.qCamPlateNum == "未知")
                            || (MF.OpenLocation.Checked && (listAllCarInfo[i].sRSURandCode == qoutJG.qJGRandCode.ToString("X2")))
                            || (MF.OpenMohu.Checked && (((int)(LevenPercent.LevenshteinDistancePercent(listAllCarInfo[i].sOBUPlateNum, qoutJG.qCamPlateNum) * 100)) > 70)))
                        {
                            if (listAllCarInfo[i].sOBUPlateNum == qoutJG.qCamPlateNum)
                            {
                                MarchFunction = "车牌匹配";
                            }
                            else if (((listAllCarInfo[i].sRSURandCode == qoutJG.qJGRandCode.ToString("X2")) && qoutJG.qCamPlateNum == "未知")
                            || (MF.OpenLocation.Checked && (listAllCarInfo[i].sRSURandCode == qoutJG.qJGRandCode.ToString("X2"))))
                            {
                                MarchFunction = "位置匹配1";
                            }
                            else if ((MF.OpenLocation.Checked && (listAllCarInfo[i].sRSURandCode == qoutJG.qJGRandCode.ToString("X2"))))
                            {
                                MarchFunction = "位置匹配2";
                            }
                            else if ((MF.OpenMohu.Checked && (((int)(LevenPercent.LevenshteinDistancePercent(listAllCarInfo[i].sOBUPlateNum, qoutJG.qCamPlateNum) * 100)) > 70)))
                            {
                                MarchFunction = "模糊匹配";
                            }
                            listAllCarInfo[i].sCamBiao = qoutJG.qCambiao;
                            listAllCarInfo[i].sCamPicPath = qoutJG.qCamPicPath;
                            listAllCarInfo[i].sCamPlateColor = qoutJG.qCamPlateColor;
                            listAllCarInfo[i].sCamPlateNum = qoutJG.qCamPlateNum;
                            listAllCarInfo[i].sJGCarHigh = qoutJG.qJGWide;
                            listAllCarInfo[i].sJGCarLength = qoutJG.qJGLength;
                            listAllCarInfo[i].sJGCarType = qoutJG.qJGCarType;
                            listAllCarInfo[i].sJGDateTime = qoutJG.qJGDateTime;
                            listAllCarInfo[i].sJGId = qoutJG.qJGId;
                            listAllCarInfo[i].sJGRandCode = qoutJG.qJGRandCode.ToString("X2");
                            isMarch = true;
                            //界面显示
                            sZuobistring = MF.MarchedShow(listAllCarInfo[i].sOBUCartype, listAllCarInfo[i].sOBUPlateNum, listAllCarInfo[i].sJGCarType, listAllCarInfo[i].sCamPlateNum, listAllCarInfo[i].sCamPicPath, listAllCarInfo[i].sCount, listAllCarInfo[i]);
                            //表格显示
                            MF.adddataGridViewRoll(listAllCarInfo[i].sCount, listAllCarInfo[i].sJGCarType, listAllCarInfo[i].sOBUCartype,
                                listAllCarInfo[i].sOBUDateTime, listAllCarInfo[i].sJGDateTime, listAllCarInfo[i].sOBUPlateNum,
                                listAllCarInfo[i].sCamPlateNum, listAllCarInfo[i].sOBUPlateColor, listAllCarInfo[i].sCamPlateColor,
                                listAllCarInfo[i].sCamBiao, listAllCarInfo[i].sJGId, listAllCarInfo[i].sOBUCarLength, listAllCarInfo[i].sOBUCarHigh, listAllCarInfo[i].sCamPicPath);
                            //写入总数据库
                            InsertString = @"Insert into " + sql_dbname
                                + ".dbo.CarInfo(JGLength,JGWide,JGCarType,ForceTime,CamPlateColor,CamPlateNum,Cambiao,CamPicPath,JGId,OBUPlateColor,OBUPlateNum,OBUMac,OBUY,OBUBiao,OBUCarLength,OBUCarHigh,OBUCarType,TradeTime,TradeState,RandCode,GetFunction) values('"
                                + listAllCarInfo[i].sJGCarLength + "','" + listAllCarInfo[i].sJGCarHigh + "','" + listAllCarInfo[i].sJGCarType + "','"
                                + listAllCarInfo[i].sJGDateTime + "','" + listAllCarInfo[i].sCamPlateColor + "','" + listAllCarInfo[i].sCamPlateNum + "','"
                                + listAllCarInfo[i].sCamBiao + "','" + listAllCarInfo[i].sCamPicPath + "','" + listAllCarInfo[i].sJGId + "','"
                                + listAllCarInfo[i].sOBUPlateColor + "','" + listAllCarInfo[i].sOBUPlateNum + "','" + listAllCarInfo[i].sOBUMac + "','"
                                + listAllCarInfo[i].sOBUY + "','" + listAllCarInfo[i].sOBUBiao + "','" + listAllCarInfo[i].sOBUCarLength + "','" + listAllCarInfo[i].sOBUCarHigh + "','"
                                + listAllCarInfo[i].sOBUCartype + "','" + listAllCarInfo[i].sOBUDateTime + "','" + sZuobistring + "','"
                                + listAllCarInfo[i].sRSURandCode + "','" + MarchFunction + "')";
                            GlobalMember.SQLInter.UpdateSQLData(InsertString);
                            listAllCarInfo.RemoveAt(i);
                            Log.WritePlateLog(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + MarchFunction + "成功" + "识别车牌：" + qoutJG.qCamPlateNum + "\r\n");
                            //弱强制匹配
                            if (i >= 2)
                            {
                                if (listAllCarInfo[i - 2].sOBUPlateNum != "" && listAllCarInfo[i - 1].sCamPlateNum != "")
                                {
                                    MarchFunction = "强制匹配";
                                    //界面显示（暂时不更新吧？）
                                    //sZuobistring = MarchedShow(listAllCarInfo[i-2].sOBUCartype, listAllCarInfo[i-2].sOBUPlateNum, listAllCarInfo[i-1].sJGCarType, listAllCarInfo[i-1].sCamPlateNum, listAllCarInfo[i-1].sCamPicPath, listAllCarInfo[i-2].sCount);
                                    //表格显示
                                    MF.adddataGridViewRoll(listAllCarInfo[i - 2].sCount, listAllCarInfo[i - 1].sJGCarType, listAllCarInfo[i - 2].sOBUCartype,
                                        listAllCarInfo[i - 2].sOBUDateTime, listAllCarInfo[i - 1].sJGDateTime, listAllCarInfo[i - 2].sOBUPlateNum,
                                        listAllCarInfo[i - 1].sCamPlateNum, listAllCarInfo[i - 2].sOBUPlateColor, listAllCarInfo[i - 1].sCamPlateColor,
                                        listAllCarInfo[i - 1].sCamBiao, listAllCarInfo[i - 1].sJGId, listAllCarInfo[i - 2].sOBUCarLength, listAllCarInfo[i - 2].sOBUCarHigh, listAllCarInfo[i - 1].sCamPicPath);
                                    //更新数据库
                                    //写入总数据库
                                    InsertString = @"Insert into " + sql_dbname
                                        + ".dbo.CarInfo(JGLength,JGWide,JGCarType,ForceTime,CamPlateColor,CamPlateNum,Cambiao,CamPicPath,JGId,OBUPlateColor,OBUPlateNum,OBUMac,OBUY,OBUBiao,OBUCarLength,OBUCarHigh,OBUCarType,TradeTime,TradeState,RandCode,GetFunction) values('"
                                        + listAllCarInfo[i - 1].sJGCarLength + "','" + listAllCarInfo[i - 1].sJGCarHigh + "','" + listAllCarInfo[i - 1].sJGCarType + "','"
                                        + listAllCarInfo[i - 1].sJGDateTime + "','" + listAllCarInfo[i - 1].sCamPlateColor + "','" + listAllCarInfo[i - 1].sCamPlateNum + "','"
                                        + listAllCarInfo[i - 1].sCamBiao + "','" + listAllCarInfo[i - 1].sCamPicPath + "','" + listAllCarInfo[i - 1].sJGId + "','"
                                        + listAllCarInfo[i - 2].sOBUPlateColor + "','" + listAllCarInfo[i - 2].sOBUPlateNum + "','" + listAllCarInfo[i - 2].sOBUMac + "','"
                                        + listAllCarInfo[i - 2].sOBUY + "','" + listAllCarInfo[i - 2].sOBUBiao + "','" + listAllCarInfo[i - 2].sOBUCarLength + "','" + listAllCarInfo[i - 2].sOBUCarHigh + "','"
                                        + listAllCarInfo[i - 2].sOBUCartype + "','" + listAllCarInfo[i - 2].sOBUDateTime + "','" + "强制匹配作弊不详" + "','"
                                        + listAllCarInfo[i - 2].sRSURandCode + "','" + "强制匹配" + "')";
                                    GlobalMember.SQLInter.UpdateSQLData(InsertString);

                                    Log.WritePlateLog(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + MarchFunction + "成功" + listAllCarInfo[i - 2].sRSURandCode + "OBU车牌：" + listAllCarInfo[i - 2].sOBUPlateNum + "\r\n");
                                    listAllCarInfo.RemoveAt(i - 1);
                                    listAllCarInfo.RemoveAt(i - 2);
                                }
                            }

                            break;
                        }
                    }
                    if (!isMarch)
                    {
                        if (listAllCarInfo.Count >= 6)
                        {
                            //写入总数据库
                            InsertString = @"Insert into " + sql_dbname
                                + ".dbo.CarInfo(JGLength,JGWide,JGCarType,ForceTime,CamPlateColor,CamPlateNum,Cambiao,CamPicPath,JGId,OBUPlateColor,OBUPlateNum,OBUMac,OBUY,OBUBiao,OBUCarLength,OBUCarHigh,OBUCarType,TradeTime,TradeState,RandCode,GetFunction) values('"
                                + listAllCarInfo[0].sJGCarLength + "','" + listAllCarInfo[0].sJGCarHigh + "','" + listAllCarInfo[0].sJGCarType + "','"
                                + listAllCarInfo[0].sJGDateTime + "','" + listAllCarInfo[0].sCamPlateColor + "','" + listAllCarInfo[0].sCamPlateNum + "','"
                                + listAllCarInfo[0].sCamBiao + "','" + listAllCarInfo[0].sCamPicPath + "','" + listAllCarInfo[0].sJGId + "','"
                                + listAllCarInfo[0].sOBUPlateColor + "','" + listAllCarInfo[0].sOBUPlateNum + "','" + listAllCarInfo[0].sOBUMac + "','"
                                + listAllCarInfo[0].sOBUY + "','" + listAllCarInfo[0].sOBUBiao + "','" + listAllCarInfo[0].sOBUCarLength + "','" + listAllCarInfo[0].sOBUCarHigh + "','"
                                + listAllCarInfo[0].sOBUCartype + "','" + listAllCarInfo[0].sOBUDateTime + "','" + "未知" + "','"
                                + listAllCarInfo[0].sRSURandCode + "','" + "未能匹配" + "')";
                            GlobalMember.SQLInter.UpdateSQLData(InsertString);
                            listAllCarInfo.RemoveAt(0);
                            Log.WritePlateLog(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + " RSU触发队列已满，首位清空" + "OBU车牌：" + listAllCarInfo[0].sOBUPlateNum + "识别车牌：" + listAllCarInfo[0].sCamPlateNum + "\r\n");
                        }
                        listAllCarInfo.Add(new CarFullInfo("", "", "", "", "", "", "", "", "", "", qoutJG.qJGCarType,
                            qoutJG.qJGWide, qoutJG.qJGLength, qoutJG.qJGDateTime, qoutJG.qJGId, qoutJG.qCamPlateNum,
                            qoutJG.qCamPlateColor, qoutJG.qCambiao, qoutJG.qCamPicPath, qoutJG.qJGRandCode.ToString("X2"), ""));
                        Log.WritePlateLog(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + " JG入队列" + "车牌：" + qoutJG.qCamPlateNum + "\r\n");
                    }
                    else
                    {
                        isMarch = false;
                        Log.WritePlateLog(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + " JG匹配RSU车牌成功，跟随码" + qoutJG.qJGRandCode.ToString("X2") + "车牌：" + qoutJG.qCamPlateNum + "入库Car表\r\n");
                    }
                }


            }
            catch (Exception ex)
            {
                Log.WriteLog(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + " 数据匹配异常\r\n" + ex.ToString() + "\r\n");
            }
        }
        #endregion

        

    }
}