using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

namespace ETCF
{
    public partial class GridDouclickShowPicDialog : Form
    {
        private string vehtype;
        private string getplateno;
        private string imagepath;
        private string forcetime;
        private string ovehtype;
        private string oplatenumber;
        private string vehlenth;
        private string vehheight;

        int Cartype = 0;


        FormDemo MF;

        public GridDouclickShowPicDialog(string veht, string plateno, string path, string forcetim, string otype, string onumber, string laser_vehlenth, string laser_vehheight, FormDemo fm)
        {
            InitializeComponent();

            vehtype = veht;//激光车型
            getplateno = plateno;//摄像机车牌
            imagepath = path;//图片路径
            forcetime = forcetim;//时间
            ovehtype = otype;//Obu车型
            oplatenumber = onumber;//OBU车牌
            vehlenth = laser_vehlenth;//车长
            vehheight = laser_vehheight;//车高
            MF = fm;
        }

        private void GridDouclickShowPicDialog_Load(object sender, EventArgs e)
        {
            int Shuttype=0;
            try
            {
                GlobalMember.MysqlInter.FindBlackOrWhiteCar(oplatenumber, ref Shuttype);
                if (Shuttype == -1 || Shuttype == -2)
                {
                    //恢复为正常车辆
                    skinLabel1.Text = "当前车辆：黑名单中  ";
                    //sbtnCheck.Text = "恢复为正常车辆";
                }
                else if (Shuttype == 1)
                {
                    //移除白名单
                    skinLabel1.Text = "当前车辆：白名单中  ";
                    //sbtnCheck.Text = "移除白名单";
                }
                else if (Shuttype == 0)
                {
                    skinLabel1.Text = "当前车辆：普通车辆  ";
                    //sbtnCheck.Text = "加入白名单";
                }
                else
                {
                    skinLabel1.Text = "当前车辆：未在名单中  ";
                    //sbtnCheck.Text = "列入正常车辆";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            try
            {


                this.pictureBoxVeh.Load(imagepath);
                this.pictureBoxVeh.SizeMode = PictureBoxSizeMode.StretchImage;
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("图片有误！" + ex.ToString(), "提示");
            }

            this.labelVeh.Text = "激光车型：" + vehtype + "            OBU车型：" + ovehtype + "\r\n摄像机车牌号：" + getplateno + "   OBU车牌号：" + oplatenumber + " \r\n时间:" + forcetime + "\r\n";

        }

        private void wihiteList_Click(object sender, EventArgs e)
        {
            int res = 0;
            int Shuttype = 0;
            GlobalMember.MysqlInter.FindBlackOrWhiteCar(oplatenumber, ref Shuttype);

            if (Shuttype == 2)//不在名单内
            {
                skinLabel1.Text = "当前车辆：不在名单内";
            }
            else if (Shuttype == 1) //白名单
            {
                skinLabel1.Text = "当前车辆：白名单";
            }
            else if (Shuttype == -1)
            {
                skinLabel1.Text = "当前车辆：临时黑名单";
            }
            else if (Shuttype == -2)
            {
                skinLabel1.Text = "当前车辆：黑名单";
            }
            if (Shuttype == 2)//不在名单内
            {
                res = GlobalMember.MysqlInter.UpdateBlackOrWhiteCar(oplatenumber, 1);
               
            }
            else
            {
                res = GlobalMember.MysqlInter.UpdateBlackOrWhiteCar(oplatenumber, 1);
                
            }

            if (res != 0)
            {
                MessageBox.Show("操作白名单失败");
            }
            else
            {
                skinLabel1.Text = "当前车辆：白名单";
            }
        }

        private void blackList_Click(object sender, EventArgs e)
        {
            int res = 0;
            int Shuttype = 0;
            GlobalMember.MysqlInter.FindBlackOrWhiteCar(oplatenumber, ref Shuttype);

            if (Shuttype == 2)//不在名单内
            {
                skinLabel1.Text = "当前车辆：不在名单内";
            }
            else if (Shuttype == 1) //白名单
            {
                skinLabel1.Text = "当前车辆：白名单";
            }
            else if (Shuttype == -1)
            {
                skinLabel1.Text = "当前车辆：临时黑名单";
            }
            else if (Shuttype == -2)
            {
                skinLabel1.Text = "当前车辆：黑名单";
            }

            if (Shuttype == 2)//不在名单内
            {
                res = GlobalMember.MysqlInter.UpdateBlackOrWhiteCar(oplatenumber, -2);
                
            }
            else
            {
                res = GlobalMember.MysqlInter.UpdateBlackOrWhiteCar(oplatenumber, -2);
               
            }

            if (res != 0)
            {
                MessageBox.Show("操作黑名单失败");
            }
            else
            {
                skinLabel1.Text = "当前车辆：黑名单";
            }
        }

        private void pictureBoxVeh_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            string PicPath = pictureBoxVeh.ImageLocation;
            Process.Start(PicPath);
        }
    }
}
