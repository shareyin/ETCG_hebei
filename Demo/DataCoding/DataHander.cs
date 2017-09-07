using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ETCF
{
    class DataHander
    {
        public byte RSCTL = 0x80;
        public void DataCoding(ref byte[] buf, ref int alen)
        {
            int i, l_codelen;
            int l_netsendlen = 0;
            byte chk;
            byte[] code_buf = new byte[1024];

            /* ****************编码，加上起始标志、校验码和结束标志**************** */
            chk = 0;
            l_codelen = 0;
            code_buf[l_codelen++] = 0xff;
            code_buf[l_codelen++] = 0xff;
            //code_buf[l_codelen++] = (RSCTL >> 4) | (RSCTL << 4); // 帧序号;
            code_buf[l_codelen++] = RSCTL;
            if (RSCTL < 0xfd)
            {
                RSCTL++;
            }
            else
            {
                RSCTL = 0;
            }
            chk ^= code_buf[2];
            for (i = 0; i < alen; i++)
            {
                //计算校验码
                chk ^= buf[i];
                //处理0xff特殊情况
                if (buf[i] == 0xff)
                {
                    code_buf[l_codelen++] = 0xfe;
                    code_buf[l_codelen++] = 0x01;
                }
                else if (buf[i] == 0xfe)
                {
                    code_buf[l_codelen++] = 0xfe;
                    code_buf[l_codelen++] = 0x00;
                }
                else
                    code_buf[l_codelen++] = buf[i];
            }

            if (chk == 0xff)
            {
                code_buf[l_codelen++] = 0xfe;
                code_buf[l_codelen++] = 0x01;
            }
            else if (chk == 0xfe)
            {
                code_buf[l_codelen++] = 0xfe;
                code_buf[l_codelen++] = 0x00;
            }
            else
                code_buf[l_codelen++] = chk;//校验码
            code_buf[l_codelen++] = 0xff;//结束标志

            for (i = 0; i < l_codelen; i++)
            {
                buf[l_netsendlen++] = code_buf[i];
            }
            alen = l_codelen;
        }

        //解包
        public int DataEncoding(ref byte[] buf, ref int len)
        {
            int i, j;
            int ret = 0;
            byte chk = 0;
            int start;

            //分析包头的正确性
            if (buf[0] != 0xff)
                return -1;

            if (buf[1] != 0xFF)
            {
                for (i = len - 1; i > 0; i--)
                {
                    buf[i + 1] = buf[i];
                }
                buf[0] = 0xFF;
            }
            start = 2;

            j = 0;
            buf[j++] = 0xff;
            buf[j++] = 0xff;

            //去掉了停止位
            for (i = start; i < len - 1; i++)
            {
                if (buf[i] == 0xff)
                    return -1;

                buf[j] = buf[i];
                //还原为0xff的情况
                if (buf[i] == 0xfe)
                {
                    buf[j] |= buf[i + 1];
                    i++;
                }
                j++;
            }
            buf[j++] = 0xff;
            len = j;

            //检验异或校验值
            for (i = 2; i < len - 2; i++)
                chk ^= buf[i];
            if (chk != buf[len - 2])
                return -1;

            return ret;
        }
    }
}
