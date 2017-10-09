using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ETCF
{
    public class Veh_Type
    {
        public static int Veh_Type_Last(UInt16 Veh_Lenth, UInt16 Veh_Height, string Veh_Logo)
        {
            UInt16 u16FirstBusThres_H = GlobalMember.DisFirst_H;       //第一类客车高度阈值
            UInt16 u16FirstBusThres_L = GlobalMember.DisFirst_L;       //第一类客车长度阈值
            UInt16 u16SecBusThres_H = GlobalMember.DisSecond_H;         //第二类客车高度阈值
            UInt16 u16SecBusThres_L = GlobalMember.DisSecond_L;         //第二类客车长度阈值
            UInt16 u16ThirdBusThres_H = GlobalMember.DisThird_H;       //第三类客车高度阈值
            UInt16 u16ThirdBusThres_L = GlobalMember.DisThird_L;       //第三类客车长度阈值

            int lVeh_Type = 0;
            if (Veh_Logo != null)
            {
                switch (Veh_Logo)
                {
                    case "依维柯":
                        u16FirstBusThres_H = GlobalMember.DisLargeTypeOne_H;
                        u16FirstBusThres_L = GlobalMember.DisLargeTypeOne_L;
                        break;
                    case "全顺":
                        u16FirstBusThres_H = GlobalMember.DisLargeTypeOne_H;
                        u16FirstBusThres_L = GlobalMember.DisLargeTypeOne_L;
                        break;
                    default:
                        break;
                }
            }
            if (Veh_Lenth == 0 || Veh_Height == 0)
            {
                lVeh_Type = 0;
            }
            else
            {
                if (Veh_Height > GlobalMember.DisFourHigherthan_H || (Veh_Lenth > GlobalMember.DisFourHighandLength_L && Veh_Height > GlobalMember.DisFourHighandLength_H))
                {
                    lVeh_Type = 4;
                }
                else if (Veh_Height > u16ThirdBusThres_H && Veh_Lenth > u16ThirdBusThres_L)
                {
                    lVeh_Type = 4;
                }
                else if (Veh_Height > u16SecBusThres_H && Veh_Lenth > u16SecBusThres_L)
                {
                    lVeh_Type = 3;
                }
                else if (Veh_Height > u16FirstBusThres_H && Veh_Lenth > u16FirstBusThres_L)
                {
                    lVeh_Type = 2;
                }
                else
                {
                    lVeh_Type = 1;
                }
            }
            

            return lVeh_Type;
        }
    }
}
