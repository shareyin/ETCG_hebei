using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

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
        AllInfo Allinfo;
        Levenshtein LevenPercent = new Levenshtein();
        string InsertString = "";
        string MarchFunction = "";
        string MarchFlag = "";
        int ShutType = 0;
        bool isZuobi = false;
        #endregion

        public StopType(FormDemo mf)
        {
            if (MF == null)
            {
                MF = mf;
            }
        }

        #region ******拦截模式匹配逻辑(与位置无关版本)******
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
                    if (GlobalMember.SqlType.Equals("SQLServer"))
                    {
                        GlobalMember.SQLInter.InsertJGData(qoutJG.qJGLength, qoutJG.qJGHigh, qoutJG.qJGCarType,
                        qoutJG.qJGId, qoutJG.qCamPlateNum, qoutJG.qCamPicPath, qoutJG.qJGDateTime,
                        qoutJG.qCambiao, qoutJG.qCamPlateColor, qoutJG.qJGRandCode.ToString("X2"),GlobalMember.g_sLaneNo);
                    }
                    else
                    {
                        GlobalMember.MysqlInter.InsertJGData(qoutJG.qJGLength, qoutJG.qJGHigh, qoutJG.qJGCarType,
                        qoutJG.qJGId, qoutJG.qCamPlateNum, qoutJG.qCamPicPath, qoutJG.qJGDateTime,
                        qoutJG.qCambiao, qoutJG.qCamPlateColor, qoutJG.qJGRandCode.ToString("X2"), GlobalMember.g_sLaneNo);
                    }
                    
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
                            listAllCarInfo[i].sJGCarHigh = qoutJG.qJGHigh;
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
                                + ".dbo.CarInfo(JGLength,JGWide,JGCarType,ForceTime,CamPlateColor,CamPlateNum,Cambiao,CamPicPath,JGId,OBUPlateColor,OBUPlateNum,OBUMac,OBUY,OBUBiao,OBUCarLength,OBUCarHigh,OBUCarType,TradeTime,TradeState,RandCode,GetFunction,LaneNo) values('"
                                + listAllCarInfo[i].sJGCarLength + "','" + listAllCarInfo[i].sJGCarHigh + "','" + listAllCarInfo[i].sJGCarType + "','"
                                + listAllCarInfo[i].sJGDateTime + "','" + listAllCarInfo[i].sCamPlateColor + "','" + listAllCarInfo[i].sCamPlateNum + "','"
                                + listAllCarInfo[i].sCamBiao + "','" + listAllCarInfo[i].sCamPicPath + "','" + listAllCarInfo[i].sJGId + "','"
                                + listAllCarInfo[i].sOBUPlateColor + "','" + listAllCarInfo[i].sOBUPlateNum + "','" + listAllCarInfo[i].sOBUMac + "','"
                                + listAllCarInfo[i].sOBUY + "','" + listAllCarInfo[i].sOBUBiao + "','" + listAllCarInfo[i].sOBUCarLength + "','" + listAllCarInfo[i].sOBUCarHigh + "','"
                                + listAllCarInfo[i].sOBUCartype + "','" + listAllCarInfo[i].sOBUDateTime + "','" + sZuobistring + "','"
                                + listAllCarInfo[i].sRSURandCode + "','" + MarchFunction + "','" + GlobalMember.g_sLaneNo + "')";
                            if (GlobalMember.SqlType.Equals("SQLServer"))
                            {
                                GlobalMember.SQLInter.UpdateSQLData(InsertString);
                            }
                            else
                            {
                                GlobalMember.MysqlInter.UpdateSQLData(InsertString);
                            }
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
                                + ".dbo.CarInfo(JGLength,JGWide,JGCarType,ForceTime,CamPlateColor,CamPlateNum,Cambiao,CamPicPath,JGId,OBUPlateColor,OBUPlateNum,OBUMac,OBUY,OBUBiao,OBUCarLength,OBUCarHigh,OBUCarType,TradeTime,TradeState,RandCode,GetFunction,LaneNo) values('"
                                + listAllCarInfo[0].sJGCarLength + "','" + listAllCarInfo[0].sJGCarHigh + "','" + listAllCarInfo[0].sJGCarType + "','"
                                + listAllCarInfo[0].sJGDateTime + "','" + listAllCarInfo[0].sCamPlateColor + "','" + listAllCarInfo[0].sCamPlateNum + "','"
                                + listAllCarInfo[0].sCamBiao + "','" + listAllCarInfo[0].sCamPicPath + "','" + listAllCarInfo[0].sJGId + "','"
                                + listAllCarInfo[0].sOBUPlateColor + "','" + listAllCarInfo[0].sOBUPlateNum + "','" + listAllCarInfo[0].sOBUMac + "','"
                                + listAllCarInfo[0].sOBUY + "','" + listAllCarInfo[0].sOBUBiao + "','" + listAllCarInfo[0].sOBUCarLength + "','" + listAllCarInfo[0].sOBUCarHigh + "','"
                                + listAllCarInfo[0].sOBUCartype + "','" + listAllCarInfo[0].sOBUDateTime + "','" + "未知" + "','"
                                + listAllCarInfo[0].sRSURandCode + "','" + "未能匹配" + "','" + GlobalMember.g_sLaneNo + "')";
                            if (GlobalMember.SqlType.Equals("SQLServer"))
                            {
                                GlobalMember.SQLInter.UpdateSQLData(InsertString);
                            }
                            else
                            {
                                GlobalMember.MysqlInter.UpdateSQLData(InsertString);
                            }
                            listAllCarInfo.RemoveAt(0);
                            Log.WritePlateLog(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + " RSU触发队列已满，首位清空" + "OBU车牌：" + listAllCarInfo[0].sOBUPlateNum + "识别车牌：" + listAllCarInfo[0].sCamPlateNum + "\r\n");
                        }
                        listAllCarInfo.Add(new CarFullInfo("", "", "", "", "", "", "", "", "", "", qoutJG.qJGCarType,
                            qoutJG.qJGHigh, qoutJG.qJGLength, qoutJG.qJGDateTime, qoutJG.qJGId, qoutJG.qCamPlateNum,
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
                                + ".dbo.CarInfo(JGLength,JGWide,JGCarType,ForceTime,CamPlateColor,CamPlateNum,Cambiao,CamPicPath,JGId,OBUPlateColor,OBUPlateNum,OBUMac,OBUY,OBUBiao,OBUCarLength,OBUCarHigh,OBUCarType,TradeTime,TradeState,RandCode,GetFunction,LaneNo) values('"
                                + listAllCarInfo[i].sJGCarLength + "','" + listAllCarInfo[i].sJGCarHigh + "','" + listAllCarInfo[i].sJGCarType + "','"
                                + listAllCarInfo[i].sJGDateTime + "','" + listAllCarInfo[i].sCamPlateColor + "','" + listAllCarInfo[i].sCamPlateNum + "','"
                                + listAllCarInfo[i].sCamBiao + "','" + listAllCarInfo[i].sCamPicPath + "','" + listAllCarInfo[i].sJGId + "','"
                                + listAllCarInfo[i].sOBUPlateColor + "','" + listAllCarInfo[i].sOBUPlateNum + "','" + listAllCarInfo[i].sOBUMac + "','"
                                + listAllCarInfo[i].sOBUY + "','" + listAllCarInfo[i].sOBUBiao + "','" + listAllCarInfo[i].sOBUCarLength + "','" + listAllCarInfo[i].sOBUCarHigh + "','"
                                + listAllCarInfo[i].sOBUCartype + "','" + listAllCarInfo[i].sOBUDateTime + "','" + sZuobistring + "','"
                                + listAllCarInfo[i].sRSURandCode + "','" + "车牌匹配" + "','" + GlobalMember.g_sLaneNo + "')";
                            if (GlobalMember.SqlType.Equals("SQLServer"))
                            {
                                GlobalMember.SQLInter.UpdateSQLData(InsertString);
                            }
                            else
                            {
                                GlobalMember.MysqlInter.UpdateSQLData(InsertString);
                            }
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
                                + ".dbo.CarInfo(JGLength,JGWide,JGCarType,ForceTime,CamPlateColor,CamPlateNum,Cambiao,CamPicPath,JGId,OBUPlateColor,OBUPlateNum,OBUMac,OBUY,OBUBiao,OBUCarLength,OBUCarHigh,OBUCarType,TradeTime,TradeState,RandCode,GetFunction,LaneNo) values('"
                                + listAllCarInfo[0].sJGCarLength + "','" + listAllCarInfo[0].sJGCarHigh + "','" + listAllCarInfo[0].sJGCarType + "','"
                                + listAllCarInfo[0].sJGDateTime + "','" + listAllCarInfo[0].sCamPlateColor + "','" + listAllCarInfo[0].sCamPlateNum + "','"
                                + listAllCarInfo[0].sCamBiao + "','" + listAllCarInfo[0].sCamPicPath + "','" + listAllCarInfo[0].sJGId + "','"
                                + listAllCarInfo[0].sOBUPlateColor + "','" + listAllCarInfo[0].sOBUPlateNum + "','" + listAllCarInfo[0].sOBUMac + "','"
                                + listAllCarInfo[0].sOBUY + "','" + listAllCarInfo[0].sOBUBiao + "','" + listAllCarInfo[0].sOBUCarLength + "','" + listAllCarInfo[0].sOBUCarHigh + "','"
                                + listAllCarInfo[0].sOBUCartype + "','" + listAllCarInfo[0].sOBUDateTime + "','" + "未知" + "','"
                                + listAllCarInfo[0].sRSURandCode + "','" + "未能匹配" + "','" + GlobalMember.g_sLaneNo + "')";
                            if (GlobalMember.SqlType.Equals("SQLServer"))
                            {
                                GlobalMember.SQLInter.UpdateSQLData(InsertString);
                            }
                            else
                            {
                                GlobalMember.MysqlInter.UpdateSQLData(InsertString);
                            }
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
                    if (GlobalMember.SqlType.Equals("SQLServer"))
                    {
                        GlobalMember.SQLInter.InsertRSUData(qoutRSU.qOBUPlateColor, qoutRSU.qOBUPlateNum,
                        qoutRSU.qOBUMac, qoutRSU.qOBUY, qoutRSU.qOBUBiao, qoutRSU.qOBUCarLength, qoutRSU.qOBUCarhigh,
                        qoutRSU.qOBUCarType, qoutRSU.qOBUDateTime, qoutRSU.qRSURandCode.ToString("X2"), GlobalMember.g_sLaneNo);
                    }
                    else
                    {
                        GlobalMember.MysqlInter.InsertRSUData(qoutRSU.qOBUPlateColor, qoutRSU.qOBUPlateNum,
                        qoutRSU.qOBUMac, qoutRSU.qOBUY, qoutRSU.qOBUBiao, qoutRSU.qOBUCarLength, qoutRSU.qOBUCarhigh,
                        qoutRSU.qOBUCarType, qoutRSU.qOBUDateTime, qoutRSU.qRSURandCode.ToString("X2"), GlobalMember.g_sLaneNo);
                    }
                    
                }
                


            }
            catch (Exception ex)
            {
                Log.WriteLog(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + " 数据匹配异常\r\n" + ex.ToString() + "\r\n");
            }
        }
        #endregion

        #region ******拦截模式匹配逻辑(需要位置版本)******
        //拦截模式
        /// <summary>
        /// 目前有五种匹配，
        /// 1、车牌位置均未能匹配 2、车牌匹配位置不匹配 3、车牌模糊匹配位置不匹配 4、位置匹配车牌不匹配 5、完全匹配
        /// 
        /// </summary>
        /// <param name="qRSUData"></param>
        /// <param name="qJGData"></param>
        /// <param name="sql_dbname"></param>
        public void StartStoptypeWithLocation(System.Collections.Concurrent.ConcurrentQueue<QueueRSUData> qRSUData,
            System.Collections.Concurrent.ConcurrentQueue<QueueJGData> qJGData,
            string sql_dbname)
        {
            try
            {
                MarchFlag = "";//初始化
                ShutType = 0;
                isZuobi = false;
                Allinfo = new AllInfo();
                QueueRSUData qoutRSU = new QueueRSUData();
                QueueJGData qoutJG = new QueueJGData();
                //取ETC的数据
                if (qRSUData.TryDequeue(out qoutRSU))
                {
                    Log.WritePlateLog(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + " 天线数据出栈完成 " + "\r\n");
                    //发送位置
                    TimeSpan ts1 = DateTime.Now-GlobalMember.g_TimeForTest;
                    MF.JGMainDone.Reset();
                    
                    MF.SendLocation(Convert.ToUInt16(qoutRSU.qOBUY));
                    Log.WritePlateLog(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + " 收到RSU数据到发送位置数据耗时 "+ts1.TotalMilliseconds +"毫秒"+ "\r\n");
                    //isZuobi=GlobalMember.SQLInter.FindBlackOrWhiteCar(qoutRSU.qOBUPlateNum, ref ShutType);
                    //获取OBU信息
                    Allinfo.sOBUCarHigh = qoutRSU.qOBUCarhigh;
                    Allinfo.sOBUCarLength = qoutRSU.qOBUCarLength;
                    Allinfo.sOBUCartype = qoutRSU.qOBUCarType;
                    Allinfo.sOBUDateTime = qoutRSU.qOBUDateTime;
                    Allinfo.sOBUMac = qoutRSU.qOBUMac;
                    Allinfo.sOBUPlateColor = qoutRSU.qOBUPlateColor;
                    Allinfo.sOBUPlateNum = qoutRSU.qOBUPlateNum;
                    Allinfo.sOBUY = qoutRSU.qOBUY;
                    Allinfo.sOBUBiao = qoutRSU.qOBUBiao;
                    Allinfo.sCount = qoutRSU.qCount;
                    //先进行车牌匹配
                    if (MF.listCamInfo != null)
                    {
                        foreach (var camlist in MF.listCamInfo)
                        {
                            if (qoutRSU.qOBUPlateNum.Equals(camlist.qCamPlateNum))
                            {
                                MarchFlag = GlobalMember.MarchByPlateNum;
                                Log.WritePlateLog(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + " 调试日志，车牌匹配成功 " + "\r\n");
                            }
                            else if (((int)(LevenPercent.LevenshteinDistancePercent(camlist.qCamPlateNum, qoutRSU.qOBUPlateNum) * 100)) > 70)
                            {
                                MarchFlag = GlobalMember.MarchByMohu;
                                Log.WritePlateLog(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + " 调试日志，模糊匹配成功 " + "\r\n");
                            }
                            if (MarchFlag != GlobalMember.MarchByDefault)
                            {
                                //获取摄像头信息
                                Allinfo.sCamBiao = camlist.qCambiao;
                                Allinfo.sCamPicPath = camlist.qCamPicPath;
                                Allinfo.sCamPlateColor = camlist.qCamPlateColor;
                                Allinfo.sCamPlateNum = camlist.qCamPlateNum;
                                Allinfo.sJGId = camlist.qJGID;
                                Allinfo.sJGCarHigh = camlist.qJGHigh;
                                Allinfo.sJGCarLength = camlist.qJGLength;
                                Allinfo.sJGCarType = camlist.qJGCarType;
                                Allinfo.sJGDateTime = camlist.qJGDateTime;

                                break;             
                            }
                        }
                    }
                    
                    if (MF.JGMainDone.WaitOne(80))
                    {
                        TimeSpan ts2 = DateTime.Now - GlobalMember.g_TimeForTest;
                        Log.WritePlateLog(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + " 收到RSU数据到收到激光返回耗时 " + ts2.TotalMilliseconds + "毫秒" + "\r\n");
                        //等到激光数据
                        //1、先进行位置匹配
                        if (MarchFlag != GlobalMember.MarchByDefault)
                        {
                            for (int i = MF.listJGMain.Count - 1; i >= 0; i--)
                            {

                                if (Allinfo.sJGId == MF.listJGMain[i].qJGID)
                                {
                                    //第1种情况，车配匹配或者模糊匹配完成,ID也匹配
                                    Allinfo.sGetPic = MF.listJGMain[i].qGetPicDone;
                                    Allinfo.sJGCarHigh = MF.listJGMain[i].qJGCarHigh;
                                    Allinfo.sJGCarLength = MF.listJGMain[i].qJGCarLength;
                                    Allinfo.sJGCarType = MF.listJGMain[i].qJGID;//暂时没修改，这里需要进行车型判断的
                                    Allinfo.sJGDateTime = MF.listJGMain[i].qJGDateTime;
                                    MarchFlag = GlobalMember.MarchByPerfact;
                                    Log.WritePlateLog(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + " 调试日志，完全匹配成功 " + "\r\n");
                                    //MF.listJGMain.RemoveRange(0,MF.listJGMain.Count-1);
                                    break;
                                }

                            }

                        }
                        if (MarchFlag != GlobalMember.MarchByPerfact&&MarchFlag != GlobalMember.MarchByDefault)
                        {
                            //非完全匹配，可能由于没有位置导致
                            MarchFlag = GlobalMember.MarchByReget;

                        }
                        if (MarchFlag == GlobalMember.MarchByDefault)
                        {

                            if (MF.listJGMain != null)
                            {
                                for (int i = MF.listJGMain.Count - 1; i >= 0; i--)
                                {
                                    //第2种情况，车牌匹配和模糊匹配均无结果。但是位置匹配
                                    if (LocationMarch(MF.listJGMain[i].qJGLocation, qoutRSU.qOBUY, 23500))
                                    {
                                        Allinfo.sGetPic = MF.listJGMain[i].qGetPicDone;
                                        Allinfo.sJGCarHigh = MF.listJGMain[i].qJGCarHigh;
                                        Allinfo.sJGCarLength = MF.listJGMain[i].qJGCarLength;
                                        Allinfo.sJGCarType = MF.listJGMain[i].qJGID;//暂时没修改，这里需要进行车型判断的
                                        Allinfo.sJGDateTime = MF.listJGMain[i].qJGDateTime;
                                        Allinfo.sJGId = MF.listJGMain[i].qJGID;
                                        MarchFlag = GlobalMember.MarchByLocation;
                                    }
                                    if (MarchFlag.Equals(GlobalMember.MarchByLocation))
                                    {
                                        if (MF.listCamInfo != null)
                                        {
                                            foreach (var camlist in MF.listCamInfo)
                                            {
                                                if (Allinfo.sJGId == camlist.qJGID)
                                                {
                                                    //获取摄像头信息
                                                    Allinfo.sCamBiao = camlist.qCambiao;
                                                    Allinfo.sCamPicPath = camlist.qCamPicPath;
                                                    Allinfo.sCamPlateColor = camlist.qCamPlateColor;
                                                    Allinfo.sCamPlateNum = camlist.qCamPlateNum;
                                                    Allinfo.sJGId = camlist.qJGID;
                                                    break;
                                                }
                                            }
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                        
                    }
                    else
                    {
                        //80毫秒超时
                        //
                        Log.WritePlateLog(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + " 调试日志，激光返回超时 " + "\r\n");
                        if (MarchFlag.Equals(GlobalMember.MarchByDefault))
                        {
                            //车牌位置均未匹配
                            //第3种情况，车牌位置均未匹配（我也很无奈啊）
                        }
                        else
                        { 
                            //第4种情况，车牌匹配，位置没有匹配.重新匹配
                            MarchFlag = GlobalMember.MarchByReget;
                        }
                    }
                    if (MarchFlag!=GlobalMember.MarchByReget)
                    {
                        if (Allinfo.sJGCarLength != null && Allinfo.sJGCarHigh != null)
                        {
                            Allinfo.sJGCarType = "客" + Veh_Type.Veh_Type_Last(Convert.ToUInt16(Allinfo.sJGCarLength), Convert.ToUInt16(Allinfo.sJGCarHigh), Allinfo.sCamBiao).ToString();
                        }
                        else
                        {
                            Allinfo.sJGCarType = "未知";
                        }
                    }
                    Log.WritePlateLog(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + " 调试日志，计算得到激光车型 " + Allinfo.sJGCarType + "\r\n");
                    //第3，第4种情况，均只查询数据库来判断
                    if (MarchFlag.Equals(GlobalMember.MarchByDefault) || MarchFlag.Equals(GlobalMember.MarchByPlateNum) || MarchFlag.Equals(GlobalMember.MarchByMohu))
                    {
                        //没有匹配的话，进行一次数据库查询。如果作弊就直接拦车
                        if (GlobalMember.SqlType.Equals("SQLServer"))
                        {
                            isZuobi = GlobalMember.SQLInter.FindBlackOrWhiteCar(qoutRSU.qOBUPlateNum, ref ShutType);
                        }
                        else
                        {
                            isZuobi = GlobalMember.MysqlInter.FindBlackOrWhiteCar(qoutRSU.qOBUPlateNum, ref ShutType);
                        }

                        //进行车型判别是否作弊
                        if (isZuobi)
                        {
                            ////先通知天线控制器终止交易
                            //MF.TcpReply(0xA4, 02, MF.RSUTcpClient);
                            ////启动报警器
                            //Thread thread1 = new Thread(new ThreadStart(MF.Alarm));
                            //thread1.Start();

                            Log.WritePlateLog(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + " "+MarchFlag + " 可能作弊" + " OBU车牌：" + qoutRSU.qOBUPlateNum + "\r\n");
                        }
                        else
                        {
                            MF.TcpReply(0xA4, 01, MF.RSUTcpClient);
                            Log.WritePlateLog(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + " " + MarchFlag + " 正常过车" + " OBU车牌：" + qoutRSU.qOBUPlateNum + "\r\n");
                        }
                        //先进行RSU数据存储
                        if (GlobalMember.SqlType.Equals("SQLServer"))
                        {
                            GlobalMember.SQLInter.InsertRSUData(qoutRSU.qOBUPlateColor, qoutRSU.qOBUPlateNum,
                            qoutRSU.qOBUMac, qoutRSU.qOBUY, qoutRSU.qOBUBiao, qoutRSU.qOBUCarLength, qoutRSU.qOBUCarhigh,
                            qoutRSU.qOBUCarType, qoutRSU.qOBUDateTime, qoutRSU.qRSURandCode.ToString("X2"), GlobalMember.g_sLaneNo);
                        }
                        else
                        {
                            GlobalMember.MysqlInter.InsertRSUData(qoutRSU.qOBUPlateColor, qoutRSU.qOBUPlateNum,
                            qoutRSU.qOBUMac, qoutRSU.qOBUY, qoutRSU.qOBUBiao, qoutRSU.qOBUCarLength, qoutRSU.qOBUCarhigh,
                            qoutRSU.qOBUCarType, qoutRSU.qOBUDateTime, qoutRSU.qRSURandCode.ToString("X2"), GlobalMember.g_sLaneNo);
                        }
                        if(MarchFlag!=GlobalMember.MarchByDefault)
                        {
                            //写入总数据 
                            if (GlobalMember.SqlType.Equals("SQLServer"))
                            {
                                InsertString = @"Insert into " + sql_dbname
                                + ".dbo.CarInfo(JGLength,JGWide,JGCarType,ForceTime,CamPlateColor,CamPlateNum,Cambiao,CamPicPath,JGId,OBUPlateColor,OBUPlateNum,OBUMac,OBUY,OBUBiao,OBUCarLength,OBUCarHigh,OBUCarType,TradeTime,TradeState,RandCode,GetFunction,LaneNo) values('"
                                + Allinfo.sJGCarLength + "','" + Allinfo.sJGCarHigh + "','" + Allinfo.sJGCarType + "','"
                                + Allinfo.sJGDateTime + "','" + Allinfo.sCamPlateColor + "','" + Allinfo.sCamPlateNum + "','"
                                + Allinfo.sCamBiao + "','" + Allinfo.sCamPicPath + "','" + Allinfo.sJGId + "','"
                                + Allinfo.sOBUPlateColor + "','" + Allinfo.sOBUPlateNum + "','" + Allinfo.sOBUMac + "','"
                                + Allinfo.sOBUY + "','" + Allinfo.sOBUBiao + "','" + Allinfo.sOBUCarLength + "','" + Allinfo.sOBUCarHigh + "','"
                                + Allinfo.sOBUCartype + "','" + Allinfo.sOBUDateTime + "','" + sZuobistring + "','"
                                + Allinfo.sRSURandCode + "','" + MarchFlag + "','" + GlobalMember.g_sLaneNo + "')";
                                GlobalMember.SQLInter.UpdateSQLData(InsertString);
                            }
                            else
                            {
                                InsertString = @"Insert into CarInfo(JGLength,JGWide,JGCarType,ForceTime,CamPlateColor,CamPlateNum,Cambiao,CamPicPath,JGId,OBUPlateColor,OBUPlateNum,OBUMac,OBUY,OBUBiao,OBUCarLength,OBUCarHigh,OBUCarType,TradeTime,TradeState,RandCode,GetFunction,LaneNo) values('"
                                + Allinfo.sJGCarLength + "','" + Allinfo.sJGCarHigh + "','" + Allinfo.sJGCarType + "','"
                                + Allinfo.sJGDateTime + "','" + Allinfo.sCamPlateColor + "','" + Allinfo.sCamPlateNum + "','"
                                + Allinfo.sCamBiao + "','" + Allinfo.sCamPicPath + "','" + Allinfo.sJGId + "','"
                                + Allinfo.sOBUPlateColor + "','" + Allinfo.sOBUPlateNum + "','" + Allinfo.sOBUMac + "','"
                                + Allinfo.sOBUY + "','" + Allinfo.sOBUBiao + "','" + Allinfo.sOBUCarLength + "','" + Allinfo.sOBUCarHigh + "','"
                                + Allinfo.sOBUCartype + "','" + Allinfo.sOBUDateTime + "','" + sZuobistring + "','"
                                + Allinfo.sRSURandCode + "','" + MarchFlag + "','" + GlobalMember.g_sLaneNo + "')";
                                GlobalMember.MysqlInter.UpdateSQLData(InsertString);
                            }
                        }
                        //界面显示
                        MF.MarchedShow(Allinfo.sOBUCartype, Allinfo.sOBUPlateNum, Allinfo.sJGCarType, Allinfo.sCamPlateNum, Allinfo.sCamPicPath, Allinfo.sCount);

                        //表格显示
                        MF.adddataGridViewRoll(Allinfo.sCount, Allinfo.sJGCarType, Allinfo.sOBUCartype,
                            Allinfo.sOBUDateTime, Allinfo.sJGDateTime, Allinfo.sOBUPlateNum,
                            Allinfo.sCamPlateNum, Allinfo.sOBUPlateColor, Allinfo.sCamPlateColor,
                            Allinfo.sCamBiao, Allinfo.sJGId, Allinfo.sJGCarLength, Allinfo.sJGCarHigh, Allinfo.sCamPicPath);
                    }
                    else
                    {
                        sZuobistring = MF.isZuobi(qoutRSU.qOBUCarType, qoutRSU.qOBUPlateNum, Allinfo.sJGCarType, Allinfo, ref ShutType);
                        TimeSpan ts3 = DateTime.Now - GlobalMember.g_TimeForTest;
                        Log.WritePlateLog(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + " 收到RSU数据到收到逻辑判断结束耗时 " + ts3.TotalMilliseconds + "毫秒" + "\r\n");
                        Log.WritePlateLog(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + " " + MarchFlag + " OBU车牌：" + qoutRSU.qOBUPlateNum+" "+ sZuobistring  + "\r\n");
                        //先进行RSU数据存储
                        if (GlobalMember.SqlType.Equals("SQLServer"))
                        {
                            GlobalMember.SQLInter.InsertRSUData(qoutRSU.qOBUPlateColor, qoutRSU.qOBUPlateNum,
                            qoutRSU.qOBUMac, qoutRSU.qOBUY, qoutRSU.qOBUBiao, qoutRSU.qOBUCarLength, qoutRSU.qOBUCarhigh,
                            qoutRSU.qOBUCarType, qoutRSU.qOBUDateTime, qoutRSU.qRSURandCode.ToString("X2"), GlobalMember.g_sLaneNo);
                            //再进行激光数据存储
                            GlobalMember.SQLInter.InsertJGData(Allinfo.sJGCarLength, Allinfo.sJGCarHigh, Allinfo.sJGCarType,
                           Allinfo.sJGId, Allinfo.sCamPlateNum, Allinfo.sCamPicPath, Allinfo.sJGDateTime,
                           Allinfo.sCamBiao, Allinfo.sCamPlateColor, Allinfo.sJGLocation.ToString(), GlobalMember.g_sLaneNo);
                        }
                        else
                        {
                            GlobalMember.MysqlInter.InsertRSUData(qoutRSU.qOBUPlateColor, qoutRSU.qOBUPlateNum,
                            qoutRSU.qOBUMac, qoutRSU.qOBUY, qoutRSU.qOBUBiao, qoutRSU.qOBUCarLength, qoutRSU.qOBUCarhigh,
                            qoutRSU.qOBUCarType, qoutRSU.qOBUDateTime, qoutRSU.qRSURandCode.ToString("X2"), GlobalMember.g_sLaneNo);
                            //再进行激光数据存储
                            GlobalMember.MysqlInter.InsertJGData(Allinfo.sJGCarLength, Allinfo.sJGCarHigh, Allinfo.sJGCarType,
                           Allinfo.sJGId, Allinfo.sCamPlateNum, Allinfo.sCamPicPath, Allinfo.sJGDateTime,
                           Allinfo.sCamBiao, Allinfo.sCamPlateColor, Allinfo.sJGLocation.ToString(), GlobalMember.g_sLaneNo);
                        }

                        //界面显示
                        MF.MarchedShow(Allinfo.sOBUCartype, Allinfo.sOBUPlateNum, Allinfo.sJGCarType, Allinfo.sCamPlateNum, Allinfo.sCamPicPath, Allinfo.sCount);
                        //表格显示
                        MF.adddataGridViewRoll(Allinfo.sCount, Allinfo.sJGCarType, Allinfo.sOBUCartype,
                            Allinfo.sOBUDateTime, Allinfo.sJGDateTime, Allinfo.sOBUPlateNum,
                            Allinfo.sCamPlateNum, Allinfo.sOBUPlateColor, Allinfo.sCamPlateColor,
                            Allinfo.sCamBiao, Allinfo.sJGId, Allinfo.sOBUCarLength, Allinfo.sOBUCarHigh, Allinfo.sCamPicPath);
                        //更新数据库
                        //写入总数据
                        
                        if (GlobalMember.SqlType.Equals("SQLServer"))
                        {
                            InsertString = @"Insert into " + sql_dbname
                            + ".dbo.CarInfo(JGLength,JGWide,JGCarType,ForceTime,CamPlateColor,CamPlateNum,Cambiao,CamPicPath,JGId,OBUPlateColor,OBUPlateNum,OBUMac,OBUY,OBUBiao,OBUCarLength,OBUCarHigh,OBUCarType,TradeTime,TradeState,RandCode,GetFunction,LaneNo) values('"
                            + Allinfo.sJGCarLength + "','" + Allinfo.sJGCarHigh + "','" + Allinfo.sJGCarType + "','"
                            + Allinfo.sJGDateTime + "','" + Allinfo.sCamPlateColor + "','" + Allinfo.sCamPlateNum + "','"
                            + Allinfo.sCamBiao + "','" + Allinfo.sCamPicPath + "','" + Allinfo.sJGId + "','"
                            + Allinfo.sOBUPlateColor + "','" + Allinfo.sOBUPlateNum + "','" + Allinfo.sOBUMac + "','"
                            + Allinfo.sOBUY + "','" + Allinfo.sOBUBiao + "','" + Allinfo.sOBUCarLength + "','" + Allinfo.sOBUCarHigh + "','"
                            + Allinfo.sOBUCartype + "','" + Allinfo.sOBUDateTime + "','" + sZuobistring + "','"
                            + Allinfo.sRSURandCode + "','" + MarchFlag + "','" + GlobalMember.g_sLaneNo + "')";
                            GlobalMember.SQLInter.UpdateSQLData(InsertString);
                        }
                        else
                        {
                            InsertString = @"Insert into CarInfo(JGLength,JGWide,JGCarType,ForceTime,CamPlateColor,CamPlateNum,Cambiao,CamPicPath,JGId,OBUPlateColor,OBUPlateNum,OBUMac,OBUY,OBUBiao,OBUCarLength,OBUCarHigh,OBUCarType,TradeTime,TradeState,RandCode,GetFunction,LaneNo) values('"
                            + Allinfo.sJGCarLength + "','" + Allinfo.sJGCarHigh + "','" + Allinfo.sJGCarType + "','"
                            + Allinfo.sJGDateTime + "','" + Allinfo.sCamPlateColor + "','" + Allinfo.sCamPlateNum + "','"
                            + Allinfo.sCamBiao + "','" + Allinfo.sCamPicPath + "','" + Allinfo.sJGId + "','"
                            + Allinfo.sOBUPlateColor + "','" + Allinfo.sOBUPlateNum + "','" + Allinfo.sOBUMac + "','"
                            + Allinfo.sOBUY + "','" + Allinfo.sOBUBiao + "','" + Allinfo.sOBUCarLength + "','" + Allinfo.sOBUCarHigh + "','"
                            + Allinfo.sOBUCartype + "','" + Allinfo.sOBUDateTime + "','" + sZuobistring + "','"
                            + Allinfo.sRSURandCode + "','" + MarchFlag + "','" + GlobalMember.g_sLaneNo + "')";
                            GlobalMember.MysqlInter.UpdateSQLData(InsertString);
                        }
                        Log.WritePlateLog(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + MarchFlag + "成功" + "OBU车牌：" + qoutRSU.qOBUPlateNum + "\r\n");
                    }
                    
                }
            }
            catch (Exception ex)
            {
                Log.WriteLog(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + " 数据匹配异常\r\n" + ex.ToString() + "\r\n");
            }
            finally
            {
                //清空JG主数据
                if (MF.listJGMain.Count > 0)
                {
                    MF.listJGMain.RemoveRange(0, MF.listJGMain.Count - 1);
                }
            }

        }
        //判断位置是否在范围内匹配（稽查模式）
        public bool LocationMarch(long m_lJGLocation, string m_sOBUY)
        {
            int ChetouDis = (int)((Math.Abs(m_lJGLocation)) / 10);
            int Temp = Convert.ToInt16(m_sOBUY) - ChetouDis;
            if ((Temp > 0 && Temp < GlobalMember.ZxTempValue) || (Temp < 0 && Math.Abs(Temp) < GlobalMember.FxTempValue))
            {
                return true;
            }
            else
            {
                return false;
            }
            
        }

        //判断位置是否在范围内匹配(拦截模式)
        public bool LocationMarch(long m_lJGLocation, string m_sOBUY,int m_iJGforeheadRSU)
        {
            int ChetouDis = (int)((Math.Abs(m_iJGforeheadRSU-m_lJGLocation)) / 10);
            int Temp = Convert.ToUInt16(m_sOBUY) - ChetouDis;
            if ((Temp > 0 && Temp < GlobalMember.ZxTempValue) || (Temp < 0 && Math.Abs(Temp) < GlobalMember.FxTempValue))
            {
                return true;
            }
            else
            {
                return false;
            }

        }
        #endregion

        #region ******稽查模式(激光器与天线同杆)匹配逻辑******
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
                    if (GlobalMember.SqlType.Equals("SQLServer"))
                    {
                        GlobalMember.SQLInter.InsertRSUData(qoutRSU.qOBUPlateColor, qoutRSU.qOBUPlateNum,
                        qoutRSU.qOBUMac, qoutRSU.qOBUY, qoutRSU.qOBUBiao, qoutRSU.qOBUCarLength, qoutRSU.qOBUCarhigh,
                        qoutRSU.qOBUCarType, qoutRSU.qOBUDateTime, qoutRSU.qRSURandCode.ToString("X2"), GlobalMember.g_sLaneNo);
                    }
                    else
                    {
                        GlobalMember.MysqlInter.InsertRSUData(qoutRSU.qOBUPlateColor, qoutRSU.qOBUPlateNum,
                        qoutRSU.qOBUMac, qoutRSU.qOBUY, qoutRSU.qOBUBiao, qoutRSU.qOBUCarLength, qoutRSU.qOBUCarhigh,
                        qoutRSU.qOBUCarType, qoutRSU.qOBUDateTime, qoutRSU.qRSURandCode.ToString("X2"), GlobalMember.g_sLaneNo);
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
                                + ".dbo.CarInfo(JGLength,JGWide,JGCarType,ForceTime,CamPlateColor,CamPlateNum,Cambiao,CamPicPath,JGId,OBUPlateColor,OBUPlateNum,OBUMac,OBUY,OBUBiao,OBUCarLength,OBUCarHigh,OBUCarType,TradeTime,TradeState,RandCode,GetFunction,LaneNo) values('"
                                + listAllCarInfo[i].sJGCarLength + "','" + listAllCarInfo[i].sJGCarHigh + "','" + listAllCarInfo[i].sJGCarType + "','"
                                + listAllCarInfo[i].sJGDateTime + "','" + listAllCarInfo[i].sCamPlateColor + "','" + listAllCarInfo[i].sCamPlateNum + "','"
                                + listAllCarInfo[i].sCamBiao + "','" + listAllCarInfo[i].sCamPicPath + "','" + listAllCarInfo[i].sJGId + "','"
                                + listAllCarInfo[i].sOBUPlateColor + "','" + listAllCarInfo[i].sOBUPlateNum + "','" + listAllCarInfo[i].sOBUMac + "','"
                                + listAllCarInfo[i].sOBUY + "','" + listAllCarInfo[i].sOBUBiao + "','" + listAllCarInfo[i].sOBUCarLength + "','" + listAllCarInfo[i].sOBUCarHigh + "','"
                                + listAllCarInfo[i].sOBUCartype + "','" + listAllCarInfo[i].sOBUDateTime + "','" + sZuobistring + "','"
                                + listAllCarInfo[i].sRSURandCode + "','" + "车牌匹配" + "','" + GlobalMember.g_sLaneNo + "')";
                            if (GlobalMember.SqlType.Equals("SQLServer"))
                            {
                                GlobalMember.SQLInter.UpdateSQLData(InsertString);
                            }
                            else
                            {
                                GlobalMember.MysqlInter.UpdateSQLData(InsertString);
                            }
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
                                + ".dbo.CarInfo(JGLength,JGWide,JGCarType,ForceTime,CamPlateColor,CamPlateNum,Cambiao,CamPicPath,JGId,OBUPlateColor,OBUPlateNum,OBUMac,OBUY,OBUBiao,OBUCarLength,OBUCarHigh,OBUCarType,TradeTime,TradeState,RandCode,GetFunction,LaneNo) values('"
                                + listAllCarInfo[0].sJGCarLength + "','" + listAllCarInfo[0].sJGCarHigh + "','" + listAllCarInfo[0].sJGCarType + "','"
                                + listAllCarInfo[0].sJGDateTime + "','" + listAllCarInfo[0].sCamPlateColor + "','" + listAllCarInfo[0].sCamPlateNum + "','"
                                + listAllCarInfo[0].sCamBiao + "','" + listAllCarInfo[0].sCamPicPath + "','" + listAllCarInfo[0].sJGId + "','"
                                + listAllCarInfo[0].sOBUPlateColor + "','" + listAllCarInfo[0].sOBUPlateNum + "','" + listAllCarInfo[0].sOBUMac + "','"
                                + listAllCarInfo[0].sOBUY + "','" + listAllCarInfo[0].sOBUBiao + "','" + listAllCarInfo[0].sOBUCarLength + "','" + listAllCarInfo[0].sOBUCarHigh + "','"
                                + listAllCarInfo[0].sOBUCartype + "','" + listAllCarInfo[0].sOBUDateTime + "','" + "未知" + "','"
                                + listAllCarInfo[0].sRSURandCode + "','" + "未能匹配" + "','" + GlobalMember.g_sLaneNo + "')";
                            if (GlobalMember.SqlType.Equals("SQLServer"))
                            {
                                GlobalMember.SQLInter.UpdateSQLData(InsertString);
                            }
                            else
                            {
                                GlobalMember.MysqlInter.UpdateSQLData(InsertString);
                            }
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
                    if (GlobalMember.SqlType.Equals("SQLServer"))
                    {
                        GlobalMember.SQLInter.InsertJGData(qoutJG.qJGLength, qoutJG.qJGHigh, qoutJG.qJGCarType,
                        qoutJG.qJGId, qoutJG.qCamPlateNum, qoutJG.qCamPicPath, qoutJG.qJGDateTime,
                        qoutJG.qCambiao, qoutJG.qCamPlateColor, qoutJG.qJGRandCode.ToString("X2"), GlobalMember.g_sLaneNo);
                    }
                    else
                    {
                        GlobalMember.MysqlInter.InsertJGData(qoutJG.qJGLength, qoutJG.qJGHigh, qoutJG.qJGCarType,
                        qoutJG.qJGId, qoutJG.qCamPlateNum, qoutJG.qCamPicPath, qoutJG.qJGDateTime,
                        qoutJG.qCambiao, qoutJG.qCamPlateColor, qoutJG.qJGRandCode.ToString("X2"), GlobalMember.g_sLaneNo);
                    }
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
                            listAllCarInfo[i].sJGCarHigh = qoutJG.qJGHigh;
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
                                + ".dbo.CarInfo(JGLength,JGWide,JGCarType,ForceTime,CamPlateColor,CamPlateNum,Cambiao,CamPicPath,JGId,OBUPlateColor,OBUPlateNum,OBUMac,OBUY,OBUBiao,OBUCarLength,OBUCarHigh,OBUCarType,TradeTime,TradeState,RandCode,GetFunction,LaneNo) values('"
                                + listAllCarInfo[i].sJGCarLength + "','" + listAllCarInfo[i].sJGCarHigh + "','" + listAllCarInfo[i].sJGCarType + "','"
                                + listAllCarInfo[i].sJGDateTime + "','" + listAllCarInfo[i].sCamPlateColor + "','" + listAllCarInfo[i].sCamPlateNum + "','"
                                + listAllCarInfo[i].sCamBiao + "','" + listAllCarInfo[i].sCamPicPath + "','" + listAllCarInfo[i].sJGId + "','"
                                + listAllCarInfo[i].sOBUPlateColor + "','" + listAllCarInfo[i].sOBUPlateNum + "','" + listAllCarInfo[i].sOBUMac + "','"
                                + listAllCarInfo[i].sOBUY + "','" + listAllCarInfo[i].sOBUBiao + "','" + listAllCarInfo[i].sOBUCarLength + "','" + listAllCarInfo[i].sOBUCarHigh + "','"
                                + listAllCarInfo[i].sOBUCartype + "','" + listAllCarInfo[i].sOBUDateTime + "','" + sZuobistring + "','"
                                + listAllCarInfo[i].sRSURandCode + "','" + MarchFunction + "','" + GlobalMember.g_sLaneNo + "')";
                            if (GlobalMember.SqlType.Equals("SQLServer"))
                            {
                                GlobalMember.SQLInter.UpdateSQLData(InsertString);
                            }
                            else
                            {
                                GlobalMember.MysqlInter.UpdateSQLData(InsertString);
                            }
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
                                        + ".dbo.CarInfo(JGLength,JGWide,JGCarType,ForceTime,CamPlateColor,CamPlateNum,Cambiao,CamPicPath,JGId,OBUPlateColor,OBUPlateNum,OBUMac,OBUY,OBUBiao,OBUCarLength,OBUCarHigh,OBUCarType,TradeTime,TradeState,RandCode,GetFunction,LaneNo) values('"
                                        + listAllCarInfo[i - 1].sJGCarLength + "','" + listAllCarInfo[i - 1].sJGCarHigh + "','" + listAllCarInfo[i - 1].sJGCarType + "','"
                                        + listAllCarInfo[i - 1].sJGDateTime + "','" + listAllCarInfo[i - 1].sCamPlateColor + "','" + listAllCarInfo[i - 1].sCamPlateNum + "','"
                                        + listAllCarInfo[i - 1].sCamBiao + "','" + listAllCarInfo[i - 1].sCamPicPath + "','" + listAllCarInfo[i - 1].sJGId + "','"
                                        + listAllCarInfo[i - 2].sOBUPlateColor + "','" + listAllCarInfo[i - 2].sOBUPlateNum + "','" + listAllCarInfo[i - 2].sOBUMac + "','"
                                        + listAllCarInfo[i - 2].sOBUY + "','" + listAllCarInfo[i - 2].sOBUBiao + "','" + listAllCarInfo[i - 2].sOBUCarLength + "','" + listAllCarInfo[i - 2].sOBUCarHigh + "','"
                                        + listAllCarInfo[i - 2].sOBUCartype + "','" + listAllCarInfo[i - 2].sOBUDateTime + "','" + "强制匹配作弊不详" + "','"
                                        + listAllCarInfo[i - 2].sRSURandCode + "','" + "强制匹配" + "','" + GlobalMember.g_sLaneNo + "')";
                                    if (GlobalMember.SqlType.Equals("SQLServer"))
                                    {
                                        GlobalMember.SQLInter.UpdateSQLData(InsertString);
                                    }
                                    else
                                    {
                                        GlobalMember.MysqlInter.UpdateSQLData(InsertString);
                                    }

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
                                + ".dbo.CarInfo(JGLength,JGWide,JGCarType,ForceTime,CamPlateColor,CamPlateNum,Cambiao,CamPicPath,JGId,OBUPlateColor,OBUPlateNum,OBUMac,OBUY,OBUBiao,OBUCarLength,OBUCarHigh,OBUCarType,TradeTime,TradeState,RandCode,GetFunction,LaneNo) values('"
                                + listAllCarInfo[0].sJGCarLength + "','" + listAllCarInfo[0].sJGCarHigh + "','" + listAllCarInfo[0].sJGCarType + "','"
                                + listAllCarInfo[0].sJGDateTime + "','" + listAllCarInfo[0].sCamPlateColor + "','" + listAllCarInfo[0].sCamPlateNum + "','"
                                + listAllCarInfo[0].sCamBiao + "','" + listAllCarInfo[0].sCamPicPath + "','" + listAllCarInfo[0].sJGId + "','"
                                + listAllCarInfo[0].sOBUPlateColor + "','" + listAllCarInfo[0].sOBUPlateNum + "','" + listAllCarInfo[0].sOBUMac + "','"
                                + listAllCarInfo[0].sOBUY + "','" + listAllCarInfo[0].sOBUBiao + "','" + listAllCarInfo[0].sOBUCarLength + "','" + listAllCarInfo[0].sOBUCarHigh + "','"
                                + listAllCarInfo[0].sOBUCartype + "','" + listAllCarInfo[0].sOBUDateTime + "','" + "未知" + "','"
                                + listAllCarInfo[0].sRSURandCode + "','" + "未能匹配" + "','" + GlobalMember.g_sLaneNo + "')";
                            if (GlobalMember.SqlType.Equals("SQLServer"))
                            {
                                GlobalMember.SQLInter.UpdateSQLData(InsertString);
                            }
                            else
                            {
                                GlobalMember.MysqlInter.UpdateSQLData(InsertString);
                            }
                            listAllCarInfo.RemoveAt(0);
                            Log.WritePlateLog(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + " RSU触发队列已满，首位清空" + "OBU车牌：" + listAllCarInfo[0].sOBUPlateNum + "识别车牌：" + listAllCarInfo[0].sCamPlateNum + "\r\n");
                        }
                        listAllCarInfo.Add(new CarFullInfo("", "", "", "", "", "", "", "", "", "", qoutJG.qJGCarType,
                            qoutJG.qJGHigh, qoutJG.qJGLength, qoutJG.qJGDateTime, qoutJG.qJGId, qoutJG.qCamPlateNum,
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

        #region ******稽查模式(激光器与天线不同杆)匹配逻辑******
        //稽查模式
        public void JiChaTypeWithLocation(System.Collections.Concurrent.ConcurrentQueue<QueueRSUData> qRSUData,
            System.Collections.Concurrent.ConcurrentQueue<QueueJGData> qJGData,
            string sql_dbname)
        {
            //try
            //{
            //    MarchFlag = "";//初始化
            //    ShutType = 0;
            //    isZuobi = false;
            //    Allinfo = new AllInfo();
            //    QueueRSUData qoutRSU = new QueueRSUData();
            //    QueueJGData qoutJG = new QueueJGData();
            //    //取ETC的数据
            //    if (qRSUData.TryDequeue(out qoutRSU))
            //    {
            //        Log.WritePlateLog(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + " 天线数据出栈完成 " + "\r\n");
            //        //发送位置
            //        TimeSpan ts1 = DateTime.Now - GlobalMember.g_TimeForTest;
            //        MF.JGMainDone.Reset();

            //        MF.SendLocation(Convert.ToUInt16(qoutRSU.qOBUY));
            //        Log.WritePlateLog(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + " 收到RSU数据到发送位置数据耗时 " + ts1.TotalMilliseconds + "毫秒" + "\r\n");
            //        //isZuobi=GlobalMember.SQLInter.FindBlackOrWhiteCar(qoutRSU.qOBUPlateNum, ref ShutType);
            //        //获取OBU信息
            //        Allinfo.sOBUCarHigh = qoutRSU.qOBUCarhigh;
            //        Allinfo.sOBUCarLength = qoutRSU.qOBUCarLength;
            //        Allinfo.sOBUCartype = qoutRSU.qOBUCarType;
            //        Allinfo.sOBUDateTime = qoutRSU.qOBUDateTime;
            //        Allinfo.sOBUMac = qoutRSU.qOBUMac;
            //        Allinfo.sOBUPlateColor = qoutRSU.qOBUPlateColor;
            //        Allinfo.sOBUPlateNum = qoutRSU.qOBUPlateNum;
            //        Allinfo.sOBUY = qoutRSU.qOBUY;
            //        Allinfo.sOBUBiao = qoutRSU.qOBUBiao;
            //        Allinfo.sCount = qoutRSU.qCount;
            //        //先进行车牌匹配
            //        if (MF.listCamInfo != null)
            //        {
            //            foreach (var camlist in MF.listCamInfo)
            //            {
            //                if (qoutRSU.qOBUPlateNum.Equals(camlist.qCamPlateNum))
            //                {
            //                    MarchFlag = GlobalMember.MarchByPlateNum;
            //                    Log.WritePlateLog(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + " 调试日志，车牌匹配成功 " + "\r\n");
            //                }
            //                else if (((int)(LevenPercent.LevenshteinDistancePercent(camlist.qCamPlateNum, qoutRSU.qOBUPlateNum) * 100)) > 70)
            //                {
            //                    MarchFlag = GlobalMember.MarchByMohu;
            //                    Log.WritePlateLog(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + " 调试日志，模糊匹配成功 " + "\r\n");
            //                }
            //                if (MarchFlag != GlobalMember.MarchByDefault)
            //                {
            //                    //获取摄像头信息
            //                    Allinfo.sCamBiao = camlist.qCambiao;
            //                    Allinfo.sCamPicPath = camlist.qCamPicPath;
            //                    Allinfo.sCamPlateColor = camlist.qCamPlateColor;
            //                    Allinfo.sCamPlateNum = camlist.qCamPlateNum;
            //                    Allinfo.sJGId = camlist.qJGID;
            //                    break;
            //                }
            //            }
            //        }

            //        if (MF.JGMainDone.WaitOne(80))
            //        {
            //            TimeSpan ts2 = DateTime.Now - GlobalMember.g_TimeForTest;
            //            Log.WritePlateLog(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + " 收到RSU数据到收到激光返回耗时 " + ts2.TotalMilliseconds + "毫秒" + "\r\n");
            //            //等到激光数据
            //            //1、先进行位置匹配
            //            if (MarchFlag != GlobalMember.MarchByDefault)
            //            {
            //                for (int i = MF.listJGMain.Count - 1; i >= 0; i--)
            //                {

            //                    if (Allinfo.sJGId == MF.listJGMain[i].qJGID)
            //                    {
            //                        //第1种情况，车配匹配或者模糊匹配完成,ID也匹配
            //                        Allinfo.sGetPic = MF.listJGMain[i].qGetPicDone;
            //                        Allinfo.sJGCarHigh = MF.listJGMain[i].qJGCarHigh;
            //                        Allinfo.sJGCarLength = MF.listJGMain[i].qJGCarLength;
            //                        Allinfo.sJGCarType = MF.listJGMain[i].qJGID;//暂时没修改，这里需要进行车型判断的
            //                        Allinfo.sJGDateTime = MF.listJGMain[i].qJGDateTime;
            //                        MarchFlag = GlobalMember.MarchByPerfact;
            //                        Log.WritePlateLog(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + " 调试日志，完全匹配成功 " + "\r\n");
            //                        //MF.listJGMain.RemoveRange(0,MF.listJGMain.Count-1);
            //                        break;
            //                    }

            //                }

            //            }
            //            else
            //            {
            //                Log.WritePlateLog(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + " 调试日志，激光返回超时 " + "\r\n");
            //                if (MF.listJGMain != null)
            //                {
            //                    for (int i = MF.listJGMain.Count - 1; i >= 0; i--)
            //                    {
            //                        //第2种情况，车牌匹配和模糊匹配均无结果。但是位置匹配
            //                        if (LocationMarch(MF.listJGMain[i].qJGLocation, qoutRSU.qOBUY))
            //                        {
            //                            Allinfo.sGetPic = MF.listJGMain[i].qGetPicDone;
            //                            Allinfo.sJGCarHigh = MF.listJGMain[i].qJGCarHigh;
            //                            Allinfo.sJGCarLength = MF.listJGMain[i].qJGCarLength;
            //                            Allinfo.sJGCarType = MF.listJGMain[i].qJGID;//暂时没修改，这里需要进行车型判断的
            //                            Allinfo.sJGDateTime = MF.listJGMain[i].qJGDateTime;
            //                            Allinfo.sJGId = MF.listJGMain[i].qJGID;
            //                            MarchFlag = GlobalMember.MarchByLocation;
            //                        }
            //                        if (MarchFlag.Equals(GlobalMember.MarchByLocation))
            //                        {
            //                            if (MF.listCamInfo != null)
            //                            {
            //                                foreach (var camlist in MF.listCamInfo)
            //                                {
            //                                    if (Allinfo.sJGId == camlist.qJGID)
            //                                    {
            //                                        //获取摄像头信息
            //                                        Allinfo.sCamBiao = camlist.qCambiao;
            //                                        Allinfo.sCamPicPath = camlist.qCamPicPath;
            //                                        Allinfo.sCamPlateColor = camlist.qCamPlateColor;
            //                                        Allinfo.sCamPlateNum = camlist.qCamPlateNum;
            //                                        Allinfo.sJGId = camlist.qJGID;
            //                                        break;
            //                                    }
            //                                }
            //                                break;
            //                            }
            //                        }
            //                    }
            //                }
            //            }
            //        }
            //        else
            //        {
            //            //50毫秒超时
            //            //
            //            if (MarchFlag.Equals(GlobalMember.MarchByDefault))
            //            {
            //                //车牌位置均未匹配
            //                //第3种情况，车牌位置均未匹配
            //            }
            //            else
            //            {
            //                //第4种情况，车牌匹配，位置没有匹配
            //            }
            //        }
            //        if (Allinfo.sJGCarLength != null && Allinfo.sJGCarHigh != null)
            //        {
            //            Allinfo.sJGCarType = "客" + Veh_Type.Veh_Type_Last(Convert.ToUInt16(Allinfo.sJGCarLength), Convert.ToUInt16(Allinfo.sJGCarHigh), Allinfo.sCamBiao).ToString();
            //        }
            //        else
            //        {
            //            Allinfo.sJGCarType = "未知";
            //        }
            //        Log.WritePlateLog(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + " 调试日志，计算得到激光车型 " + Allinfo.sJGCarType + "\r\n");
            //        //第3，第4种情况，均只查询数据库来判断
            //        if (MarchFlag.Equals(GlobalMember.MarchByDefault) || MarchFlag.Equals(GlobalMember.MarchByPlateNum) || MarchFlag.Equals(GlobalMember.MarchByMohu))
            //        {
            //            //没有匹配的话，进行一次数据库查询。如果作弊就直接拦车
            //            if (GlobalMember.SqlType.Equals("SQLServer"))
            //            {
            //                isZuobi = GlobalMember.SQLInter.FindBlackOrWhiteCar(qoutRSU.qOBUPlateNum, ref ShutType);
            //            }
            //            else
            //            {
            //                isZuobi = GlobalMember.MysqlInter.FindBlackOrWhiteCar(qoutRSU.qOBUPlateNum, ref ShutType);
            //            }

            //            //进行车型判别是否作弊
            //            if (isZuobi)
            //            {
            //                ////先通知天线控制器终止交易
            //                //MF.TcpReply(0xA4, 02, MF.RSUTcpClient);
            //                ////启动报警器
            //                //Thread thread1 = new Thread(new ThreadStart(MF.Alarm));
            //                //thread1.Start();

            //                Log.WritePlateLog(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + " " + MarchFlag + " 可能作弊" + " OBU车牌：" + qoutRSU.qOBUPlateNum + "\r\n");
            //            }
            //            else
            //            {
            //                MF.TcpReply(0xA4, 01, MF.RSUTcpClient);
            //                Log.WritePlateLog(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + " " + MarchFlag + " 正常过车" + " OBU车牌：" + qoutRSU.qOBUPlateNum + "\r\n");
            //            }
            //            //先进行RSU数据存储
            //            if (GlobalMember.SqlType.Equals("SQLServer"))
            //            {
            //                GlobalMember.SQLInter.InsertRSUData(qoutRSU.qOBUPlateColor, qoutRSU.qOBUPlateNum,
            //                qoutRSU.qOBUMac, qoutRSU.qOBUY, qoutRSU.qOBUBiao, qoutRSU.qOBUCarLength, qoutRSU.qOBUCarhigh,
            //                qoutRSU.qOBUCarType, qoutRSU.qOBUDateTime, qoutRSU.qRSURandCode.ToString("X2"), GlobalMember.g_sLaneNo);
            //            }
            //            else
            //            {
            //                GlobalMember.MysqlInter.InsertRSUData(qoutRSU.qOBUPlateColor, qoutRSU.qOBUPlateNum,
            //                qoutRSU.qOBUMac, qoutRSU.qOBUY, qoutRSU.qOBUBiao, qoutRSU.qOBUCarLength, qoutRSU.qOBUCarhigh,
            //                qoutRSU.qOBUCarType, qoutRSU.qOBUDateTime, qoutRSU.qRSURandCode.ToString("X2"), GlobalMember.g_sLaneNo);
            //            }
            //            if (MarchFlag != GlobalMember.MarchByDefault)
            //            {
            //                //写入总数据 
            //                if (GlobalMember.SqlType.Equals("SQLServer"))
            //                {
            //                    InsertString = @"Insert into " + sql_dbname
            //                    + ".dbo.CarInfo(JGLength,JGWide,JGCarType,ForceTime,CamPlateColor,CamPlateNum,Cambiao,CamPicPath,JGId,OBUPlateColor,OBUPlateNum,OBUMac,OBUY,OBUBiao,OBUCarLength,OBUCarHigh,OBUCarType,TradeTime,TradeState,RandCode,GetFunction,LaneNo) values('"
            //                    + Allinfo.sJGCarLength + "','" + Allinfo.sJGCarHigh + "','" + Allinfo.sJGCarType + "','"
            //                    + Allinfo.sJGDateTime + "','" + Allinfo.sCamPlateColor + "','" + Allinfo.sCamPlateNum + "','"
            //                    + Allinfo.sCamBiao + "','" + Allinfo.sCamPicPath + "','" + Allinfo.sJGId + "','"
            //                    + Allinfo.sOBUPlateColor + "','" + Allinfo.sOBUPlateNum + "','" + Allinfo.sOBUMac + "','"
            //                    + Allinfo.sOBUY + "','" + Allinfo.sOBUBiao + "','" + Allinfo.sOBUCarLength + "','" + Allinfo.sOBUCarHigh + "','"
            //                    + Allinfo.sOBUCartype + "','" + Allinfo.sOBUDateTime + "','" + sZuobistring + "','"
            //                    + Allinfo.sRSURandCode + "','" + MarchFlag + "','" + GlobalMember.g_sLaneNo + "')";
            //                    GlobalMember.SQLInter.UpdateSQLData(InsertString);
            //                }
            //                else
            //                {
            //                    InsertString = @"Insert into CarInfo(JGLength,JGWide,JGCarType,ForceTime,CamPlateColor,CamPlateNum,Cambiao,CamPicPath,JGId,OBUPlateColor,OBUPlateNum,OBUMac,OBUY,OBUBiao,OBUCarLength,OBUCarHigh,OBUCarType,TradeTime,TradeState,RandCode,GetFunction,LaneNo) values('"
            //                    + Allinfo.sJGCarLength + "','" + Allinfo.sJGCarHigh + "','" + Allinfo.sJGCarType + "','"
            //                    + Allinfo.sJGDateTime + "','" + Allinfo.sCamPlateColor + "','" + Allinfo.sCamPlateNum + "','"
            //                    + Allinfo.sCamBiao + "','" + Allinfo.sCamPicPath + "','" + Allinfo.sJGId + "','"
            //                    + Allinfo.sOBUPlateColor + "','" + Allinfo.sOBUPlateNum + "','" + Allinfo.sOBUMac + "','"
            //                    + Allinfo.sOBUY + "','" + Allinfo.sOBUBiao + "','" + Allinfo.sOBUCarLength + "','" + Allinfo.sOBUCarHigh + "','"
            //                    + Allinfo.sOBUCartype + "','" + Allinfo.sOBUDateTime + "','" + sZuobistring + "','"
            //                    + Allinfo.sRSURandCode + "','" + MarchFlag + "','" + GlobalMember.g_sLaneNo + "')";
            //                    GlobalMember.MysqlInter.UpdateSQLData(InsertString);
            //                }
            //            }
            //            //界面显示
            //            MF.MarchedShow(Allinfo.sOBUCartype, Allinfo.sOBUPlateNum, Allinfo.sJGCarType, Allinfo.sCamPlateNum, Allinfo.sCamPicPath, Allinfo.sCount);

            //            //表格显示
            //            MF.adddataGridViewRoll(Allinfo.sCount, Allinfo.sJGCarType, Allinfo.sOBUCartype,
            //                Allinfo.sOBUDateTime, Allinfo.sJGDateTime, Allinfo.sOBUPlateNum,
            //                Allinfo.sCamPlateNum, Allinfo.sOBUPlateColor, Allinfo.sCamPlateColor,
            //                Allinfo.sCamBiao, Allinfo.sJGId, Allinfo.sJGCarLength, Allinfo.sJGCarHigh, Allinfo.sCamPicPath);
            //        }
            //        else
            //        {

            //            sZuobistring = MF.isZuobi(qoutRSU.qOBUCarType, qoutRSU.qOBUPlateNum, Allinfo.sJGCarType, Allinfo, ref ShutType);
            //            TimeSpan ts3 = DateTime.Now - GlobalMember.g_TimeForTest;
            //            Log.WritePlateLog(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + " 收到RSU数据到收到逻辑判断结束耗时 " + ts3.TotalMilliseconds + "毫秒" + "\r\n");
            //            Log.WritePlateLog(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + " " + MarchFlag + " OBU车牌：" + qoutRSU.qOBUPlateNum + " " + sZuobistring + "\r\n");
            //            //先进行RSU数据存储
            //            if (GlobalMember.SqlType.Equals("SQLServer"))
            //            {
            //                GlobalMember.SQLInter.InsertRSUData(qoutRSU.qOBUPlateColor, qoutRSU.qOBUPlateNum,
            //                qoutRSU.qOBUMac, qoutRSU.qOBUY, qoutRSU.qOBUBiao, qoutRSU.qOBUCarLength, qoutRSU.qOBUCarhigh,
            //                qoutRSU.qOBUCarType, qoutRSU.qOBUDateTime, qoutRSU.qRSURandCode.ToString("X2"), GlobalMember.g_sLaneNo);
            //                //再进行激光数据存储
            //                GlobalMember.SQLInter.InsertJGData(Allinfo.sJGCarLength, Allinfo.sJGCarHigh, Allinfo.sJGCarType,
            //               Allinfo.sJGId, Allinfo.sCamPlateNum, Allinfo.sCamPicPath, Allinfo.sJGDateTime,
            //               Allinfo.sCamBiao, Allinfo.sCamPlateColor, Allinfo.sJGLocation.ToString(), GlobalMember.g_sLaneNo);
            //            }
            //            else
            //            {
            //                GlobalMember.MysqlInter.InsertRSUData(qoutRSU.qOBUPlateColor, qoutRSU.qOBUPlateNum,
            //                qoutRSU.qOBUMac, qoutRSU.qOBUY, qoutRSU.qOBUBiao, qoutRSU.qOBUCarLength, qoutRSU.qOBUCarhigh,
            //                qoutRSU.qOBUCarType, qoutRSU.qOBUDateTime, qoutRSU.qRSURandCode.ToString("X2"), GlobalMember.g_sLaneNo);
            //                //再进行激光数据存储
            //                GlobalMember.MysqlInter.InsertJGData(Allinfo.sJGCarLength, Allinfo.sJGCarHigh, Allinfo.sJGCarType,
            //               Allinfo.sJGId, Allinfo.sCamPlateNum, Allinfo.sCamPicPath, Allinfo.sJGDateTime,
            //               Allinfo.sCamBiao, Allinfo.sCamPlateColor, Allinfo.sJGLocation.ToString(), GlobalMember.g_sLaneNo);
            //            }

            //            //界面显示
            //            MF.MarchedShow(Allinfo.sOBUCartype, Allinfo.sOBUPlateNum, Allinfo.sJGCarType, Allinfo.sCamPlateNum, Allinfo.sCamPicPath, Allinfo.sCount);
            //            //表格显示
            //            MF.adddataGridViewRoll(Allinfo.sCount, Allinfo.sJGCarType, Allinfo.sOBUCartype,
            //                Allinfo.sOBUDateTime, Allinfo.sJGDateTime, Allinfo.sOBUPlateNum,
            //                Allinfo.sCamPlateNum, Allinfo.sOBUPlateColor, Allinfo.sCamPlateColor,
            //                Allinfo.sCamBiao, Allinfo.sJGId, Allinfo.sJGCarLength, Allinfo.sJGCarHigh, Allinfo.sCamPicPath);
            //            //更新数据库
            //            //写入总数据

            //            if (GlobalMember.SqlType.Equals("SQLServer"))
            //            {
            //                InsertString = @"Insert into " + sql_dbname
            //                + ".dbo.CarInfo(JGLength,JGWide,JGCarType,ForceTime,CamPlateColor,CamPlateNum,Cambiao,CamPicPath,JGId,OBUPlateColor,OBUPlateNum,OBUMac,OBUY,OBUBiao,OBUCarLength,OBUCarHigh,OBUCarType,TradeTime,TradeState,RandCode,GetFunction,LaneNo) values('"
            //                + Allinfo.sJGCarLength + "','" + Allinfo.sJGCarHigh + "','" + Allinfo.sJGCarType + "','"
            //                + Allinfo.sJGDateTime + "','" + Allinfo.sCamPlateColor + "','" + Allinfo.sCamPlateNum + "','"
            //                + Allinfo.sCamBiao + "','" + Allinfo.sCamPicPath + "','" + Allinfo.sJGId + "','"
            //                + Allinfo.sOBUPlateColor + "','" + Allinfo.sOBUPlateNum + "','" + Allinfo.sOBUMac + "','"
            //                + Allinfo.sOBUY + "','" + Allinfo.sOBUBiao + "','" + Allinfo.sOBUCarLength + "','" + Allinfo.sOBUCarHigh + "','"
            //                + Allinfo.sOBUCartype + "','" + Allinfo.sOBUDateTime + "','" + sZuobistring + "','"
            //                + Allinfo.sRSURandCode + "','" + MarchFlag + "','" + GlobalMember.g_sLaneNo + "')";
            //                GlobalMember.SQLInter.UpdateSQLData(InsertString);
            //            }
            //            else
            //            {
            //                InsertString = @"Insert into CarInfo(JGLength,JGWide,JGCarType,ForceTime,CamPlateColor,CamPlateNum,Cambiao,CamPicPath,JGId,OBUPlateColor,OBUPlateNum,OBUMac,OBUY,OBUBiao,OBUCarLength,OBUCarHigh,OBUCarType,TradeTime,TradeState,RandCode,GetFunction,LaneNo) values('"
            //                + Allinfo.sJGCarLength + "','" + Allinfo.sJGCarHigh + "','" + Allinfo.sJGCarType + "','"
            //                + Allinfo.sJGDateTime + "','" + Allinfo.sCamPlateColor + "','" + Allinfo.sCamPlateNum + "','"
            //                + Allinfo.sCamBiao + "','" + Allinfo.sCamPicPath + "','" + Allinfo.sJGId + "','"
            //                + Allinfo.sOBUPlateColor + "','" + Allinfo.sOBUPlateNum + "','" + Allinfo.sOBUMac + "','"
            //                + Allinfo.sOBUY + "','" + Allinfo.sOBUBiao + "','" + Allinfo.sOBUCarLength + "','" + Allinfo.sOBUCarHigh + "','"
            //                + Allinfo.sOBUCartype + "','" + Allinfo.sOBUDateTime + "','" + sZuobistring + "','"
            //                + Allinfo.sRSURandCode + "','" + MarchFlag + "','" + GlobalMember.g_sLaneNo + "')";
            //                GlobalMember.MysqlInter.UpdateSQLData(InsertString);
            //            }
            //            Log.WritePlateLog(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + MarchFlag + "成功" + "OBU车牌：" + qoutRSU.qOBUPlateNum + "\r\n");
            //        }

            //    }
            //}
            //catch (Exception ex)
            //{
            //    Log.WriteLog(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + " 数据匹配异常\r\n" + ex.ToString() + "\r\n");
            //}
            //finally
            //{
            //    //清空JG主数据
            //    if (MF.listJGMain.Count > 0)
            //    {
            //        MF.listJGMain.RemoveRange(0, MF.listJGMain.Count - 1);
            //    }
            //}
        }
        #endregion
        

    }
}