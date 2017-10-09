using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ETCF
{
    class HanderJGDataToQueue
    {
        public QueueJGData HanderJGDataIn(byte[] databuff, int bufflen)
        {
            QueueJGData m_qJG = new QueueJGData();
            int temp = 0;
            //激光ID
            m_qJG.qJGId = ((ushort)(databuff[temp] << 8 | databuff[temp+1])).ToString();
            temp += 2;
            //激光位置
            m_qJG.qJGLocation = (long)(databuff[temp] << 24 | databuff[temp + 1] << 16 | databuff[temp + 2] << 8 | databuff[temp+3]);
            temp += 4;
            //车长
            m_qJG.qJGLength = ((ushort)(databuff[temp] << 8 | databuff[temp+1])).ToString();
            temp += 2;
            //车高
            m_qJG.qJGHigh = ((ushort)(databuff[temp] << 8 | databuff[temp+1])).ToString();
            temp += 2;
            //是否已经拍照
            m_qJG.qJGGetPic = databuff[temp].ToString();
            temp += 1;
            //时间
            m_qJG.qJGDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff");
            
            return m_qJG;
        }
    }
}
