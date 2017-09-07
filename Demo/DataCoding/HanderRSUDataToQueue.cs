using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ETCF
{
    public class HanderRSUDataToQueue
    {
        public static int Count = 0;//接受ETC计数
        long g_lUnixTime = 0x00000000;

        #region******天线数据解析与加入队列******
        public QueueRSUData HanderRSUDataIn(byte[] databuff, int bufflen)
        {
            int st = 2;
            QueueRSUData m_qRSU = new QueueRSUData();
            m_qRSU.qOBUMac = databuff[2 + st].ToString("X2") + databuff[3 + st].ToString("X2") + databuff[4 + st].ToString("X2") + databuff[5 + st].ToString("X2");
            m_qRSU.qOBUY = ((ushort)(databuff[8 + st] << 8 | databuff[9 + st])).ToString();
            int sit = 0;
            m_qRSU.qOBUPlateNum = databuff[10 + st].ToString("X2") + databuff[11 + st].ToString("X2") + databuff[12 + st].ToString("X2") + databuff[13 + st].ToString("X2") + databuff[14 + st].ToString("X2") + databuff[15 + st].ToString("X2") + databuff[16 + st].ToString("X2");
            while (databuff[16 + sit + 1 + st] != 0x00)
            {
                m_qRSU.qOBUPlateNum += databuff[17 + sit + st].ToString("X2");
                sit++;
            }
            m_qRSU.qOBUPlateNum = Encoding.GetEncoding("GB2312").GetString(HexStringToByteArray(m_qRSU.qOBUPlateNum));
            switch (databuff[23 + st])
            {
                case 0:
                    m_qRSU.qOBUPlateColor = "蓝色";
                    break;
                case 1:
                    m_qRSU.qOBUPlateColor = "黄色";
                    break;
                case 2:
                    m_qRSU.qOBUPlateColor = "黑色";
                    break;
                case 3:
                    m_qRSU.qOBUPlateColor = "白色";
                    break;
                default:
                    m_qRSU.qOBUPlateColor = "未知";
                    break;
            }
            m_qRSU.qOBUCarLength = ((ushort)(databuff[26 + st] << 8 | databuff[27 + st])).ToString() + "00";
            m_qRSU.qOBUCarhigh = databuff[29 + st].ToString() + "00";
            m_qRSU.qOBUBiao = "";
            for (int i = 0; i < 16; i++)
            {
                m_qRSU.qOBUBiao += databuff[39 + i].ToString("X2");

            }
            m_qRSU.qOBUBiao = Encoding.GetEncoding("GB2312").GetString(HexStringToByteArray(m_qRSU.qOBUBiao));
            m_qRSU.qOBUDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff");
            g_lUnixTime = GetUnixTime();
            m_qRSU.qRSURandCode = g_lUnixTime;
            //SendLocation((ushort)(databuff[8 + st] << 8 | databuff[9 + st]), m_qRSU.qRSURandCode);
            //SendLocation(0xfdfd, m_qRSU.qRSURandCode);
            switch (databuff[24 + st])
            {
                case 1:
                    m_qRSU.qOBUCarType = "客1";
                    break;
                case 2:
                    m_qRSU.qOBUCarType = "客2";
                    break;
                case 3:
                    m_qRSU.qOBUCarType = "客3";
                    break;
                case 4:
                    m_qRSU.qOBUCarType = "客4";
                    break;
                case 5:
                    m_qRSU.qOBUCarType = "货1";
                    break;
                case 6:
                    m_qRSU.qOBUCarType = "货2";
                    break;
                case 7:
                    m_qRSU.qOBUCarType = "货3";
                    break;
                case 8:
                    m_qRSU.qOBUCarType = "货4";
                    break;
                case 9:
                    m_qRSU.qOBUCarType = "货5";
                    break;
                case 10:
                    m_qRSU.qOBUCarType = "货6";
                    break;
                case 11:
                    m_qRSU.qOBUCarType = "货7";
                    break;
                default:
                    m_qRSU.qOBUCarType = "未知";
                    break;
            }
            Count++;
            m_qRSU.qCount = Count.ToString();
            Log.WritePlateLog(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "  天线入栈:"
                + " 车牌：" + m_qRSU.qOBUPlateNum + " 车型：" + m_qRSU.qOBUCarType + " OBUY位置："
                + m_qRSU.qOBUY + " 随机码：" + m_qRSU.qRSURandCode.ToString("X2") + "\r\n");

            return m_qRSU;
        }
        public static byte[] HexStringToByteArray(string s)
        {
            s = s.Replace(" ", "").Trim().ToUpper();
            byte[] buffer = new byte[s.Length / 2];
            for (int i = 0; i < s.Length; i += 2)
                buffer[i / 2] = (byte)Convert.ToByte(s.Substring(i, 2), 16);
            return buffer;
        }

        public long GetUnixTime()
        {
            DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1, 0, 0, 0));
            DateTime nowTime = DateTime.Now;
            long unixTime = (long)Math.Round((nowTime - startTime).TotalSeconds, MidpointRounding.AwayFromZero);
            return unixTime;
        }
        #endregion
    }
}
