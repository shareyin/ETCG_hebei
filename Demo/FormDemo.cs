using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using ControlExs;
using System.Threading;
using System.Runtime.InteropServices;
using System.Net;
using System.Data.SqlClient;
using System.Net.Sockets;
using System.Drawing.Text;
using LayeredSkin.DirectUI;
using System.IO.Ports;
using MySql.Data.MySqlClient;

namespace ETCF
{
    public partial class FormDemo : FormEx
    {
        #region Constructor

        public FormDemo()//:base()
        {
            InitializeComponent();
            initDelegateState();
        }

        #endregion

        #region Override

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                if (!DesignMode)
                {
                    cp.ExStyle |= (int)WindowStyle.WS_CLIPCHILDREN;
                }
                return cp;
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            //DrawFromAlphaMainPart(this, e.Graphics);
        }

        #endregion

        #region Private

        /// <summary>
        /// 绘制窗体主体部分白色透明层
        /// </summary>
        /// <param name="form"></param>
        /// <param name="g"></param>
        public static void DrawFromAlphaMainPart(Form form, Graphics g)
        {
            Color[] colors = 
            {
                Color.FromArgb(5, Color.White),
                Color.FromArgb(30, Color.White),
                Color.FromArgb(145, Color.White),
                Color.FromArgb(150, Color.White),
                Color.FromArgb(30, Color.White),
                Color.FromArgb(5, Color.White)
            };

            float[] pos = 
            {
                0.0f,
                0.04f,
                0.10f,
                0.90f,
                0.97f,
                1.0f      
            };

            ColorBlend colorBlend = new ColorBlend(6);
            colorBlend.Colors = colors;
            colorBlend.Positions = pos;

            RectangleF destRect = new RectangleF(0, 0, form.Width, form.Height);
            using (LinearGradientBrush lBrush = new LinearGradientBrush(destRect, colors[0], colors[5], LinearGradientMode.Vertical))
            {
                lBrush.InterpolationColors = colorBlend;
                g.FillRectangle(lBrush, destRect);
            }
        }


        private void SetStyles()
        {
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            UpdateStyles();
        }

        #endregion

        string fontpath = string.Format(@"{0}\{1}", Application.StartupPath, "Digital2.ttf");
        #region******通用类******
        DataHander datah = new DataHander();
        
        int index = 0;//列表计数
        
        string RedicoPath = string.Format("{0}\\{1}", Application.StartupPath, "red.ico");
        string GreenicoPath = string.Format("{0}\\{1}", Application.StartupPath, "green.ico");
        System.Collections.Concurrent.ConcurrentQueue<StateObject> queue = new System.Collections.Concurrent.ConcurrentQueue<StateObject>();//用于缓存
        private static readonly object Locker1 = new object();
        private static readonly object Locker2 = new object();
        public List<CamList> listCamInfo = new List<CamList>();//摄像机数据的队列
        public List<JGMainList> listJGMain = new List<JGMainList>();//激光数据队列
        public ManualResetEvent JGMainDone = new ManualResetEvent(false);//激光主要数据帧入栈标识

        public ManualResetEvent JGHeartGo = new ManualResetEvent(false);//激光主要数据帧入栈标识
        public static int NumInJGList = 0; //激光数据帧中包含位置数量（一般情况下不超过3个，我觉得）
        //增加拦车判别标准
        public static bool NotStopNear = false;//车型相差1是否拦车
        public static bool NotStopBetween3And4 = false;//客3客4作弊是否拦车
        public static bool NotStopWhite = false;//白牌车是否拦车
        public static bool JGandRSUInSameLocation = false;//天线与激光是否同杆，通过是否同杆来选择匹配逻辑


        Thread workThread;
        Thread queueThread;
        Thread HeartAct;
        public StringBuilder OperLogCacheStr = new StringBuilder();//UI日志缓存
        Thread ProtectThread;
        public object UpdateOperLog_LockObj = new object();
        
        public int HeartJGCount = 0;//激光未收到数据心跳计数
        public int HeartRSUCount = 0;//天线未收到数据心跳计数

        //功能配置
        public string CameraType;
        public string OpenLocationPipei;

        #endregion

        #region ******数据库相关参数******
        

        private string sql_ip;//SQLServer的ip
        private string sql_dbname;//SQLServer数据库名称
        private string sql_username;//用户名
        private string sql_password;//密码
        private string sql_port;//端口
        private static string connStr = @"";
        //public SqlConnection SQLconnection = null;
        //Thread DataBaseConThread;
        #endregion

        #region ******RSU，JG，摄像机 相关参数******
        //连接天线相关
        private string RSUip;
        private string RSUport;
        public bool IsConnRSU = false;
        private static ManualResetEvent rsu_inQueueDone =
            new ManualResetEvent(false);
        public byte RSCTL = 0x80;
        System.Collections.Concurrent.ConcurrentQueue<QueueRSUData> qRSUData = new System.Collections.Concurrent.ConcurrentQueue<QueueRSUData>();//用于缓存
        //连接激光相关
        private string JGip;
        private string JGport;
        public bool IsConnJG = false;
        private static ManualResetEvent jg_inQueueDone =
            new ManualResetEvent(false);
        System.Collections.Concurrent.ConcurrentQueue<QueueJGData> qJGData = new System.Collections.Concurrent.ConcurrentQueue<QueueJGData>();//用于缓存
        //连接摄像机
        private string HKCameraip;
        private string HKCameraUsername;
        private string HKCameraPassword;

        private string ComCameraip;
        public ManualResetEvent CameraPicture = new ManualResetEvent(false);
        public ManualResetEvent CameraCanpost = new ManualResetEvent(false);
        #endregion

        #region ******配置文件******
        //读取配置信息
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);
        private void readconfig()
        {
            StringBuilder temp = new StringBuilder();
            GetPrivateProfileString("SQLServer", "sql_type", "0", temp, 255, AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "config.ini");
            GlobalMember.SqlType = temp.ToString().Equals("SQLServer") ? "SQLServer" : "MySql";
            GetPrivateProfileString("SQLServer", "sql_ip", "0", temp, 255, AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "config.ini");
            sql_ip = temp.ToString();
            GetPrivateProfileString("SQLServer", "sql_dbname", "0", temp, 255, AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "config.ini");
            sql_dbname = temp.ToString();
            GetPrivateProfileString("SQLServer", "sql_username", "0", temp, 255, AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "config.ini");
            sql_username = temp.ToString();
            GetPrivateProfileString("SQLServer", "sql_password", "0", temp, 255, AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "config.ini");
            sql_password = temp.ToString();
            GetPrivateProfileString("SQLServer", "sql_port", "0", temp, 255, AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "config.ini");
            sql_port = temp.ToString();

            GetPrivateProfileString("RSUconfig", "RSUIp", "异常", temp, 255, AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "config.ini");
            RSUip = temp.ToString();
            GetPrivateProfileString("RSUconfig", "RSUPort", "异常", temp, 255, AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "config.ini");
            RSUport = temp.ToString();

            GetPrivateProfileString("JGconfig", "JGIp", "异常", temp, 255, AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "config.ini");
            JGip = temp.ToString();
            GetPrivateProfileString("JGconfig", "JGPort", "异常", temp, 255, AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "config.ini");
            JGport = temp.ToString();

            GetPrivateProfileString("HKCameraconfig", "CameIP", "异常", temp, 255, AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "config.ini");
            HKCameraip = temp.ToString();
            GetPrivateProfileString("HKCameraconfig", "Username", "异常", temp, 255, AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "config.ini");
            HKCameraUsername = temp.ToString();
            GetPrivateProfileString("HKCameraconfig", "Password", "异常", temp, 255, AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "config.ini");
            HKCameraPassword = temp.ToString();

            GetPrivateProfileString("ComCameraconfig", "CameIP", "异常", temp, 255, AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "config.ini");
            ComCameraip = temp.ToString();
            GetPrivateProfileString("ComCameraconfig", "SavePicPathInDisk", "D:", temp, 255, AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "config.ini");
            GlobalMember.SavePicPath = temp.ToString();
            

            GetPrivateProfileString("SoftFunction", "CameraType", "异常", temp, 255, AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "config.ini");
            CameraType = temp.ToString();

            GetPrivateProfileString("SoftFunction", "OpenLocation", "异常", temp, 255, AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "config.ini");
            OpenLocationPipei = temp.ToString();

            GetPrivateProfileString("SoftFunction", "LaneNo", "异常", temp, 255, AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "config.ini");
            GlobalMember.g_sLaneNo = temp.ToString();

            GetPrivateProfileString("SoftFunction", "NotStopPlateColorIsWhite", "异常", temp, 255, AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "config.ini");
            NotStopWhite = temp.ToString().Equals("0") ? false : true;
            GetPrivateProfileString("SoftFunction", "NotStopCarTypeNear", "异常", temp, 255, AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "config.ini");
            NotStopNear = temp.ToString().Equals("0") ? false : true;
            GetPrivateProfileString("SoftFunction", "NotStopCarTypeBetween3And4", "异常", temp, 255, AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "config.ini");
            NotStopBetween3And4 = temp.ToString().Equals("0") ? false : true;
            GetPrivateProfileString("SoftFunction", "JGandRSUInSameLocation", "异常", temp, 255, AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "config.ini");
            JGandRSUInSameLocation = temp.ToString().Equals("0") ? false : true;

            GetPrivateProfileString("Alarmconfig", "AlarmCom", "异常", temp, 255, AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "config.ini");
            GlobalMember.g_sComofAlarm = temp.ToString();

            GetPrivateProfileString("Alarmconfig", "AlarmTime", "255", temp, 255, AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "config.ini");
            GlobalMember.g_iTime0fAlarm = Convert.ToInt16(temp.ToString());

            GetPrivateProfileString("JGLocationSet", "ZxDisWithOBY", "255", temp, 255, AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "config.ini");
            GlobalMember.ZxTempValue = Convert.ToInt16(temp.ToString());
            GetPrivateProfileString("JGLocationSet", "FxDisWithOBY", "255", temp, 255, AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "config.ini");
            GlobalMember.FxTempValue = Convert.ToInt16(temp.ToString());

            GetPrivateProfileString("DisCarTypeSet", "DisFirstTypeHigh", "255", temp, 255, AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "config.ini");
            GlobalMember.DisFirst_H = Convert.ToUInt16(temp.ToString());
            GetPrivateProfileString("DisCarTypeSet", "DisFirstTypeLength", "255", temp, 255, AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "config.ini");
            GlobalMember.DisFirst_L = Convert.ToUInt16(temp.ToString());
            GetPrivateProfileString("DisCarTypeSet", "DisSecondTypeHigh", "255", temp, 255, AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "config.ini");
            GlobalMember.DisSecond_H = Convert.ToUInt16(temp.ToString());
            GetPrivateProfileString("DisCarTypeSet", "DisSecondTypeLength", "255", temp, 255, AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "config.ini");
            GlobalMember.DisSecond_L = Convert.ToUInt16(temp.ToString());
            GetPrivateProfileString("DisCarTypeSet", "DisThirdTypeHigh", "255", temp, 255, AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "config.ini");
            GlobalMember.DisThird_H = Convert.ToUInt16(temp.ToString());
            GetPrivateProfileString("DisCarTypeSet", "DisThirdTypeLength", "255", temp, 255, AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "config.ini");
            GlobalMember.DisThird_L = Convert.ToUInt16(temp.ToString());

            GetPrivateProfileString("CarTypeFourExpSet", "DisFourTypeHigherThanH", "异常", temp, 255, AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "config.ini");
            GlobalMember.DisFourHigherthan_H = Convert.ToUInt16(temp.ToString());
            GetPrivateProfileString("CarTypeFourExpSet", "DisFourTypeHighandLengthH", "异常", temp, 255, AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "config.ini");
            GlobalMember.DisFourHighandLength_H = Convert.ToUInt16(temp.ToString());
            GetPrivateProfileString("CarTypeFourExpSet", "DisFourTypeHighandLengthL", "异常", temp, 255, AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "config.ini");
            GlobalMember.DisFourHighandLength_L = Convert.ToUInt16(temp.ToString());

            GetPrivateProfileString("LargeTypeOneSet", "DisLargeTypeOneHigh", "异常", temp, 255, AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "config.ini");
            GlobalMember.DisLargeTypeOne_H = Convert.ToUInt16(temp.ToString());
            GetPrivateProfileString("LargeTypeOneSet", "DisLargeTypeOneLength", "异常", temp, 255, AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "config.ini");
            GlobalMember.DisLargeTypeOne_L = Convert.ToUInt16(temp.ToString());



        }
        private void btnReadConfig_Click(object sender, EventArgs e)
        {
            StringBuilder temp = new StringBuilder();
            GetPrivateProfileString("SQLServer", "sql_ip", "0", temp, 255, AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "config.ini");
            tbSqlIP.Text = temp.ToString();
            GetPrivateProfileString("SQLServer", "sql_dbname", "0", temp, 255, AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "config.ini");
            tbDbName.Text = temp.ToString();
            GetPrivateProfileString("SQLServer", "sql_username", "0", temp, 255, AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "config.ini");
            tbUserName.Text = temp.ToString();
            GetPrivateProfileString("SQLServer", "sql_password", "0", temp, 255, AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "config.ini");
            tbPassword.Text = temp.ToString();
            //端口基本不变
            //GetPrivateProfileString("SQLServer", "sql_port", "0", temp, 255, AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "config.ini");
            //sql_port = temp.ToString();

            GetPrivateProfileString("RSUconfig", "RSUIp", "异常", temp, 255, AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "config.ini");
            tbRsuIP.Text = temp.ToString();
            GetPrivateProfileString("RSUconfig", "RSUPort", "异常", temp, 255, AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "config.ini");
            tbRsuPort.Text = temp.ToString();

            GetPrivateProfileString("JGconfig", "JGIp", "异常", temp, 255, AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "config.ini");
            tbJgIP.Text = temp.ToString();
            GetPrivateProfileString("JGconfig", "JGPort", "异常", temp, 255, AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "config.ini");
            tbJgPort.Text = temp.ToString();

            GetPrivateProfileString("HKCameraconfig", "CameIP", "异常", temp, 255, AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "config.ini");
            tbCamIP.Text = temp.ToString();
            GetPrivateProfileString("HKCameraconfig", "Username", "异常", temp, 255, AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "config.ini");
            tbCamName.Text = temp.ToString();
            GetPrivateProfileString("HKCameraconfig", "Password", "异常", temp, 255, AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "config.ini");
            tbCamPassword.Text = temp.ToString();
        }
        [DllImport("kernel32")]
        public static extern long WritePrivateProfileString(string section, string key,
            string val, string filePath);
        private void btnSaveConfig_Click(object sender, EventArgs e)
        {
            WritePrivateProfileString("SQLServer", "sql_ip", tbSqlIP.Text, AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "config.ini");
            WritePrivateProfileString("SQLServer", "sql_dbname", tbDbName.Text, AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "config.ini");
            WritePrivateProfileString("SQLServer", "sql_username", tbUserName.Text, AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "config.ini");
            WritePrivateProfileString("SQLServer", "sql_password", tbPassword.Text, AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "config.ini");

            WritePrivateProfileString("RSUconfig", "RSUIp", tbRsuIP.Text, AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "config.ini");
            WritePrivateProfileString("RSUconfig", "RSUPort", tbRsuPort.Text, AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "config.ini");

            WritePrivateProfileString("JGconfig", "JGIp", tbJgIP.Text, AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "config.ini");
            WritePrivateProfileString("JGconfig", "JGPort", tbJgPort.Text, AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "config.ini");

            WritePrivateProfileString("HKCameraconfig", "sql_ip", tbCamIP.Text, AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "config.ini");
            WritePrivateProfileString("HKCameraconfig", "sql_ip", tbCamName.Text, AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "config.ini");
            WritePrivateProfileString("HKCameraconfig", "sql_ip", tbCamPassword.Text, AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "config.ini");
        }
        #endregion
        //窗口启动，加载配置文件，连接数据库，摄像机，天线，激光器
        AutoSizeFormClass asc = new AutoSizeFormClass();
        private void FormDemo_Load(object sender, EventArgs e)
        {
            this.Left = (Screen.PrimaryScreen.WorkingArea.Width - Width) / 2;
            this.Top = (Screen.PrimaryScreen.WorkingArea.Height - Height) / 2;
            new Thread(() => { UpdateOperLogThread(); }).Start();

            PrivateFontCollection pfc = new PrivateFontCollection();
            pfc.AddFontFile(fontpath);
            Font Numfont = new Font(pfc.Families[0], 20);
            //labelNum.Font = Numfont;
            try
            {
                //读取配置文件
                readconfig();

            }
            catch (Exception ex)
            {
                Log.WriteLog(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + " 配置文件读取异常\r\n" + ex.ToString() + "\r\n");
                return;
            }
            try
            {
                //连接天线控制器
                RSUTcpClient = new SocketHelper.TcpClients();
                RSUConnect(RSUip, RSUport);
            }
            catch (Exception ex)
            {
                Log.WriteLog(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + " 连接天线异常\r\n" + ex.ToString() + "\r\n");
            }
            try
            {
                //连接激光控制器
                JGTcpClient = new SocketHelper.TcpClients();
                JGConnect(JGip, JGport);
            }
            catch (Exception ex)
            {
                Log.WriteLog(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + " 连接激光异常\r\n" + ex.ToString() + "\r\n");
                MessageBox.Show(ex.ToString());
            }
            try
            {
                if (CameraType == "HK")
                {
                    //摄像机连接
                    GlobalMember.HKCameraInter = new HKCamera(this);
                    GlobalMember.HKCameraInter.initHK(HKCameraip, HKCameraUsername, HKCameraPassword);
                }
                else if (CameraType == "IPC")
                {
                    GlobalMember.IPCCameraInter = new IPCCamera(this);
                    GlobalMember.IPCCameraInter.initIPC(ComCameraip);
                }
                else if (CameraType == "IPNC")
                {
                    GlobalMember.IPNCCameraInter = new IPNCCamera(this);
                    GlobalMember.IPNCCameraInter.initCamera(ComCameraip);
                }
                
               
            }
            catch (Exception ex)
            {
                Log.WriteLog(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + " 连接摄像机异常\r\n" + ex.ToString() + "\r\n");
            }
            try
            {
                connStr = @"Server=" + sql_ip + ";uid=" + sql_username + ";pwd=" + sql_password + ";database=" + sql_dbname;
                //数据库连接
                if (GlobalMember.SqlType.Equals("SQLServer"))
                { 
                    GlobalMember.SQLInter = new SQLServerInter(this, sql_dbname, sql_ip, sql_username, sql_password, connStr);
                    GlobalMember.SQLInter.SQLInit();
                }
                else
                {
                    GlobalMember.MysqlInter = new MysqlInter(this, sql_dbname, sql_ip, sql_username, sql_password, sql_port, connStr);
                    GlobalMember.MysqlInter.MysqlInit();
                }
                
                //SQLInit();
            }
            catch (Exception ex)
            {
                Log.WriteLog(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + " 数据库初始化异常\r\n" + ex.ToString() + "\r\n");
            }
            if (OpenLocationPipei == "1")
            {
                OpenLocation.Checked=true;
            }
            else if (OpenLocationPipei == "0")
            {
                OpenLocation.Checked = false;
            }
            asc.controllInitializeSize(this);//自适应屏幕
            //this.WindowState = System.Windows.Forms.FormWindowState.Maximized;//最大化启动
            //维护线程
            ProtectPro();
            //数据接收线程
            SocketHelper.pushSockets = new SocketHelper.PushSockets(Rec);

            //数据解析线程
            ResThread();
            //数据入库
            QueueHanderThread();
            //定时心跳
            timer1.Start();

            labelLaneNo.Text = "车道号："+GlobalMember.g_sLaneNo;
            //开启监控程序
            //StartProtect.StartPro();
            //开启激光心跳线程
            ActiveJGHeart();
            
        }

        #region******线程维护部分******
        //维护线程
        private void ProtectPro()
        {
            ProtectThread = new Thread(ProtectBase);
            ProtectThread.IsBackground = true;
            ProtectThread.Priority = ThreadPriority.BelowNormal;
            ProtectThread.Start();
        }
        public void ProtectBase(object statetemp)
        {
            while (true)
            {
                if (CameraType == "HK")
                {
                    //摄像头重连
                    if (HKCamera.HKConnState == false)
                    {
                        pictureBoxCam.BackgroundImage = Image.FromFile(@RedicoPath);
                        GlobalMember.HKCameraInter = new HKCamera(this);
                        GlobalMember.HKCameraInter.initHK(HKCameraip, HKCameraUsername, HKCameraPassword);
                        Log.WriteLog(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + " 摄像机已检测断开，正在重连\r\n");
                    }
                    else
                    {
                        pictureBoxCam.BackgroundImage = Image.FromFile(@GreenicoPath);
                    }
                }
                else if (CameraType == "IPC")
                {
                    //摄像头重连
                    if (IPCCamera.IPCConnState == false)
                    {
                        pictureBoxCam.BackgroundImage = Image.FromFile(@RedicoPath);
                        GlobalMember.IPCCameraInter = new IPCCamera(this);
                        GlobalMember.IPCCameraInter.initIPC(ComCameraip);
                        Log.WriteLog(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + " 摄像机已检测断开，正在重连\r\n");
                    }
                    else
                    {
                        pictureBoxCam.BackgroundImage = Image.FromFile(@GreenicoPath);
                    }
                }
                else if (CameraType == "IPNC")
                {
                    //摄像头重连
                    if (IPNCCamera.IPNCConnState == false)
                    {
                        pictureBoxCam.BackgroundImage = Image.FromFile(@RedicoPath);
                        GlobalMember.IPNCCameraInter = new IPNCCamera(this);
                        GlobalMember.IPNCCameraInter.initCamera(ComCameraip);
                        Log.WriteLog(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + " 摄像机已检测断开，正在重连\r\n");
                    }
                    else
                    {
                        pictureBoxCam.BackgroundImage = Image.FromFile(@GreenicoPath);
                    }
                }
                //RSU重连
                if (!IsConnRSU)
                {
                    pictureBoxRSU.BackgroundImage = Image.FromFile(@RedicoPath);
                    RSUConnect(RSUip, RSUport);
                    Log.WriteLog(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + " 天线控制器已检测断开，正在重连\r\n");
                }
                else
                {
                    pictureBoxRSU.BackgroundImage = Image.FromFile(@GreenicoPath);
                }
                //机关重连
                if (!IsConnJG)
                {
                    pictureBoxJG.BackgroundImage = Image.FromFile(@RedicoPath);
                    JGConnect(JGip, JGport);
                    Log.WriteLog(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + " 激光控制器已检测断开，正在重连\r\n");
                }
                else
                {
                    pictureBoxJG.BackgroundImage = Image.FromFile(@GreenicoPath);
                }
                Thread.Sleep(3000);
            }
        }
        #endregion

        #region******UI委托部分******
        //UI委托类初始化函数
        private void initDelegateState()
        {
            //AddOperLogCacheStr = controllogtext;

            DelegateState.pictureBoxVehshow = pictureBoxVehshow;
            DelegateState.plateNoshow = plateNoshow;
            DelegateState.adddataGridViewRoll = adddataGridViewRoll;
            DelegateState.updatedataGridViewRoll = updatedataGridViewRoll;

            DelegateState.InsertGridview = InsertGridview;
        }
        //添加表格文本
        public void adddataGridViewRoll(string s_Id, string s_JgCarType, string s_RsuCarType, string s_RsuTradeTime, string s_JgTime, string s_RsuPlateNum, string s_CamPlateNum, string s_RsuPlateColor, string s_CamPlateColor, string s_Cambiao, string s_JgId, string s_JgLength, string s_JgWide, string s_CamPicPath)
        {
            try
            {
                this.BeginInvoke(new ThreadStart(delegate
                {

                    if (dataGridViewRoll.Rows.Count > 30)
                    {
                        dataGridViewRoll.Rows.Clear();
                    }
                    //dataGridViewRoll.Rows.Add();
                    int index = dataGridViewRoll.Rows.Add();
                   
                    
                    //序号
                    this.dataGridViewRoll.Rows[index].Cells[0].Value = s_Id;
                    this.dataGridViewRoll.Rows[index].Cells[0].Style.ForeColor = Color.Black;
                    //检测车型
                    this.dataGridViewRoll.Rows[index].Cells[1].Value = s_JgCarType;
                    this.dataGridViewRoll.Rows[index].Cells[1].Style.ForeColor = Color.Green;
                    //Obu车型
                    this.dataGridViewRoll.Rows[index].Cells[2].Value = s_RsuCarType;
                    this.dataGridViewRoll.Rows[index].Cells[2].Style.ForeColor = Color.Green;
                    //交易时间
                    this.dataGridViewRoll.Rows[index].Cells[3].Value = s_RsuTradeTime;
                    this.dataGridViewRoll.Rows[index].Cells[3].Style.ForeColor = Color.Black;
                    //抓拍时间
                    this.dataGridViewRoll.Rows[index].Cells[4].Value = s_JgTime;
                    this.dataGridViewRoll.Rows[index].Cells[4].Style.ForeColor = Color.Black;
                    //OBU车牌
                    this.dataGridViewRoll.Rows[index].Cells[5].Value = s_RsuPlateNum;
                    this.dataGridViewRoll.Rows[index].Cells[5].Style.ForeColor = Color.Green;
                    //识别车牌
                    this.dataGridViewRoll.Rows[index].Cells[6].Value = s_CamPlateNum;
                    this.dataGridViewRoll.Rows[index].Cells[6].Style.ForeColor = Color.Green;
                    //OBU车牌颜色
                    this.dataGridViewRoll.Rows[index].Cells[7].Value = s_RsuPlateColor;
                    this.dataGridViewRoll.Rows[index].Cells[7].Style.ForeColor = Color.Black;
                    //识别车牌颜色
                    this.dataGridViewRoll.Rows[index].Cells[8].Value = s_CamPlateColor;
                    this.dataGridViewRoll.Rows[index].Cells[8].Style.ForeColor = Color.Black;
                    //识别车标
                    this.dataGridViewRoll.Rows[index].Cells[9].Value = s_Cambiao;
                    this.dataGridViewRoll.Rows[index].Cells[9].Style.ForeColor = Color.Black;
                    //激光序号
                    this.dataGridViewRoll.Rows[index].Cells[10].Value = s_JgId;
                    this.dataGridViewRoll.Rows[index].Cells[10].Style.ForeColor = Color.Black;
                    //车长
                    this.dataGridViewRoll.Rows[index].Cells[11].Value = s_JgLength;
                    this.dataGridViewRoll.Rows[index].Cells[11].Style.ForeColor = Color.Black;
                    //车宽
                    this.dataGridViewRoll.Rows[index].Cells[12].Value = s_JgWide;
                    this.dataGridViewRoll.Rows[index].Cells[12].Style.ForeColor = Color.Black;
                    //图片路劲
                    this.dataGridViewRoll.Rows[index].Cells[13].Value = s_CamPicPath;
                    this.dataGridViewRoll.Rows[index].Cells[13].Style.ForeColor = Color.Black;

                    this.dataGridViewRoll.FirstDisplayedScrollingRowIndex = dataGridViewRoll.RowCount - 1;//显示最新一行
                    

                }));
            }
            catch (System.Exception ex)
            {
                //MessageBox.Show(ex.ToString());
                Console.WriteLine(ex.ToString());
            }


        }
        //更新表格文本
        private void updatedataGridViewRoll(string s_Id, string s_JgCarType, string s_JgTime, string s_CamPlateNum, string s_CamPlateColor, string s_Cambiao, string s_JgId, string s_JgLength, string s_JgWide, string s_CamPicPath)
        {
            try
            {
                this.Invoke(new ThreadStart(delegate
                {
                    int i = 0;
                    for (i = 0; i < 4; i++)
                    {
                        if (s_CamPlateNum == this.dataGridViewRoll.Rows[i].Cells[5].Value.ToString())
                        {
                            //检测车型
                            this.dataGridViewRoll.Rows[0].Cells[1].Value = s_JgCarType;
                            this.dataGridViewRoll.Rows[0].Cells[1].Style.ForeColor = Color.Green;
                            //抓拍时间
                            this.dataGridViewRoll.Rows[0].Cells[4].Value = s_JgTime;
                            this.dataGridViewRoll.Rows[0].Cells[4].Style.ForeColor = Color.Black;
                            //识别车牌
                            this.dataGridViewRoll.Rows[0].Cells[6].Value = s_CamPlateNum;
                            this.dataGridViewRoll.Rows[0].Cells[6].Style.ForeColor = Color.Green;
                            //识别车牌颜色
                            this.dataGridViewRoll.Rows[0].Cells[8].Value = s_CamPlateColor;
                            this.dataGridViewRoll.Rows[0].Cells[8].Style.ForeColor = Color.Black;
                            //识别车标
                            this.dataGridViewRoll.Rows[0].Cells[9].Value = s_Cambiao;
                            this.dataGridViewRoll.Rows[0].Cells[9].Style.ForeColor = Color.Black;
                            //激光序号
                            this.dataGridViewRoll.Rows[0].Cells[10].Value = s_JgId;
                            this.dataGridViewRoll.Rows[0].Cells[10].Style.ForeColor = Color.Black;
                            //车长
                            this.dataGridViewRoll.Rows[0].Cells[11].Value = s_JgLength;
                            this.dataGridViewRoll.Rows[0].Cells[11].Style.ForeColor = Color.Black;
                            //车宽
                            this.dataGridViewRoll.Rows[0].Cells[12].Value = s_JgWide;
                            this.dataGridViewRoll.Rows[0].Cells[12].Style.ForeColor = Color.Black;
                            //图片路劲
                            this.dataGridViewRoll.Rows[0].Cells[13].Value = s_CamPicPath;
                            this.dataGridViewRoll.Rows[0].Cells[13].Style.ForeColor = Color.Black;
                            break;
                        }
                    }
                }));
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }


        }
        //图片显示
        private void pictureBoxVehshow(string msg)
        {
            try
            {
                this.Invoke(new ThreadStart(delegate
                {
                    this.pictureBoxVeh.Image = null;//待测
                    this.pictureBoxVeh.Load(msg);
                    this.pictureBoxVeh.SizeMode = PictureBoxSizeMode.StretchImage;
                }));
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
        //提示框显示（耗时太长，已取消）
        private void controllogtext(string msg)
        {
            try
            {
                this.Invoke(new ThreadStart(delegate
                {
                    this.controltext.AppendText(DateTime.Now + ":" + msg + Environment.NewLine);
                    
                }));
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
        /********************新添加*****************/
        //定时更新日志线程
        public void UpdateOperLogThread()
        {
            while (true)
            {
                Thread.Sleep(GlobalMember.OperLogFreshTime);
                lock (UpdateOperLog_LockObj)
                {
                    if ("".Equals(OperLogCacheStr.ToString()))
                    {
                        continue;
                    }
                    else
                    {
                        WriteOperLog(OperLogCacheStr.ToString());
                        if (GlobalMember.WriteLogSwitch)
                        {
                            Log.WritePlateLog(OperLogCacheStr.ToString());
                        }
                        OperLogCacheStr.Clear();
                    }
                }
            }
        }
        //定义委托
        public delegate void MyDelegate_WriteOperLogDeleFun(string log);

        public void WriteOperLog(string log)
        {
            if (this.InvokeRequired)
            {
                MyDelegate_WriteOperLogDeleFun md = new MyDelegate_WriteOperLogDeleFun(WriteOperLogDeleFun);
                this.BeginInvoke(md, log);
            }
            else
            {
                WriteOperLogDeleFun(log);
            }
        }
        //更新UI显示日志
        public void WriteOperLogDeleFun(string log)
        {
            if (this.controltext.Text.Length > 10240)
            {
                this.controltext.Text = log;
            }
            else
            {
                this.controltext.AppendText(log);
            }
        }
        //向UI日志缓存里面添加内容
        public void AddOperLogCacheStr(string log)
        {
            lock (UpdateOperLog_LockObj)
            {
                OperLogCacheStr.Append(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + " " + log + "\r\n\r\n");
            }
        }
        /********************新添加完事*****************/
        //前端界面显示车牌车型等信息
        private void plateNoshow(string s_OBUPlateNum, string s_OBUCarType, string s_CamPlateNum, string s_JGCarType, string s_Num)
        {
            try
            {
                this.BeginInvoke(new ThreadStart(delegate
                {
                    //OBU车牌号
                    this.labelOBUPlateNum.Text = s_OBUPlateNum;
                    this.labelOBUPlateNum.Text += "    ";

                    //OBU车型
                    this.labelOBUCarType.Text = s_OBUCarType;
                    this.labelOBUCarType.Text += "    ";
                    //识别车牌                    
                    this.labelCamPlateNum.Text = s_CamPlateNum;
                    this.labelCamPlateNum.Text += "    ";

                    //激光车型
                    this.labelJGCarType.Text = s_JGCarType;
                    this.labelJGCarType.Text += "    ";

                    //过车总数
                    this.labelNum.Text = s_Num;
                    this.labelNum.Text += "    ";
                }));
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
        //显示到表格上
        private void InsertGridview(string s_Id, string s_RsuTradeTime, string s_RsuPlateNum, string s_RsuCarType, string s_JgCarType, string s_IsZuobi, string s_JgLength, string s_JgWide, string s_CamPicPath)
        {
            try
            {
                this.Invoke(new ThreadStart(delegate
                {
                    int index = this.dataGridView1.Rows.Add();

                    //序号
                    this.dataGridView1.Rows[index].Cells[0].Value = s_Id;
                    //交易时间
                    this.dataGridView1.Rows[index].Cells[1].Value = s_RsuTradeTime;
                    //OBU车牌
                    this.dataGridView1.Rows[index].Cells[2].Value = s_RsuPlateNum;
                    this.dataGridView1.Rows[index].Cells[2].Style.ForeColor = Color.Blue;
                    //OBU车型
                    this.dataGridView1.Rows[index].Cells[3].Value = s_RsuCarType;
                    this.dataGridView1.Rows[index].Cells[3].Style.ForeColor = Color.Blue;
                    //激光车型
                    this.dataGridView1.Rows[index].Cells[4].Value = s_JgCarType;
                    this.dataGridView1.Rows[index].Cells[4].Style.ForeColor = Color.Blue;
                    //可能作弊
                    this.dataGridView1.Rows[index].Cells[5].Value = s_IsZuobi;
                    this.dataGridView1.Rows[index].Cells[5].Style.ForeColor = Color.Red;
                    //车长
                    this.dataGridView1.Rows[index].Cells[6].Value = s_JgLength;
                    this.dataGridView1.Rows[index].Cells[6].Style.ForeColor = Color.Black;
                    //车高
                    this.dataGridView1.Rows[index].Cells[7].Value = s_JgWide;
                    this.dataGridView1.Rows[index].Cells[7].Style.ForeColor = Color.Black;
                    //图片路径
                    this.dataGridView1.Rows[index].Cells[8].Value = s_CamPicPath;//路径

                    //this.dataGridView1.FirstDisplayedScrollingRowIndex = this.dataGridView1.RowCount - 1;//06-06,显示最新一行

                }));
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        #endregion

        #region    ******RSU建立连接******
        public SocketHelper.TcpClients RSUTcpClient;
        public void RSUConnect(string s_Rsuip, string s_Rsuport)
        {
            RSUTcpClient.InitSocket(s_Rsuip, Convert.ToInt32(s_Rsuport));
            RSUTcpClient.Start();
            //SocketHelper.pushSockets = new SocketHelper.PushSockets(Rec);
            //RSUTcpClient.ReceiveData += new SocketHelper.TcpClients.ReceiveEventHander();
                
            IsConnRSU = true;
        }
        
        #endregion

        #region******专用接收线程******
        

        //通用接收，接入缓存
        private void Rec(SocketHelper.Sockets sks)
        {
            
            this.Invoke(new ThreadStart(delegate
            {
                if (sks.ex != null)
                {
                    Log.WriteLog(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + " " + sks.Ip + "Socket异常\r\n" + sks.ex.ToString() + "\r\n");
                    string RSUIpPort = RSUip +":"+ RSUport;
                    string JGIpPort = JGip + ":"+JGport;
                    if (sks.Ip != null)
                    {
                        if (sks.Ip.ToString() == RSUIpPort)
                        {
                            RSUTcpClient.Stop();
                            HeartRSUCount = 0;
                            RSUConnect(RSUip, RSUport);
                            Log.WriteLog(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + " " + "天线Socket异常 " + "已自动恢复连接" + "\r\n");
                        }
                        else if (sks.Ip.ToString() == JGIpPort)
                        {
                            JGTcpClient.Stop();
                            HeartJGCount = 0;
                            JGConnect(JGip, JGport);
                            Log.WriteLog(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + " " + "激光Socket异常 " + "已自动恢复连接" + "\r\n");
                        }
                    }

                }
                else
                {
                    StateObject Stateque = new StateObject();
                    
                    
                    if (sks.Offset == 0)
                    {
                        //do nothing
                    }
                    else
                    {
                        if(sks.RecBuffer[3] == 0xA4)
                        {
                            TcpReply(0xA4, 0x00, RSUTcpClient);
                        }
                        //写日志放后面去
                        if (sks.RecBuffer[3] != 0xD9 && sks.RecBuffer[3] != 0x9D)
                        {
                            Log.WritePlateLog(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + " " + sks.Ip + " 接收数据帧命令:" + sks.RecBuffer[3].ToString("X2") + " 帧序号:" + sks.RecBuffer[2].ToString("X2") + " Offset长度：" + sks.Offset.ToString() + "\r\n");
                        }
                        Stateque.revLength = sks.Offset;
                        try
                        {
                            Array.Copy(sks.RecBuffer, Stateque.buffer, sks.Offset);
                        }
                        catch (Exception ex)
                        {
                            Log.WriteLog(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + " " + sks.Ip + " 接收异常 " + "Offset长度：" + sks.Offset.ToString() + " resBuffer长度：" + sks.RecBuffer.Length.ToString()+ "\r\n" + ex.ToString() + "\r\n");
                        }
                        lock (Locker1)
                        {
                            queue.Enqueue(Stateque);
                        }
                        
                    }                
                }

            }

                ));
        }
        #endregion

        #region    ******JG建立连接******
        public SocketHelper.TcpClients JGTcpClient;
        public void JGConnect(string s_Jgip, string s_Jgport)
        {
            JGTcpClient.InitSocket(s_Jgip, Convert.ToInt32(s_Jgport));
            JGTcpClient.Start();
            IsConnJG = true;
        }
        
        #endregion

        #region ******数据处理流程******
        //分包解包处理流程
        public void HanderOrgData(byte[] OrgBuff,int OrgBuffLen)
        {
            byte[] ReceiveBuf = new byte[OrgBuffLen];
            //数据处理，循环解析
            int gcnt = 0;
            //读取成功
            for (int i = 0; i < OrgBuffLen; i++)
            {
                //找到帧头
                if (gcnt == 0)
                {
                    if (OrgBuff[i] == 0xFF)
                    {
                        ReceiveBuf[0] = OrgBuff[i];
                        gcnt = 1;
                    }
                }
                else if (gcnt == 1)
                {
                    if (OrgBuff[i] == 0xFF)
                    {
                        ReceiveBuf[1] = OrgBuff[i];
                        gcnt = 2;
                    }
                    else
                    {
                        gcnt = 0;
                        continue;
                    }
                }
                else
                {
                    if (OrgBuff[i] == 0xFF)
                    {
                        ReceiveBuf[gcnt++] = OrgBuff[i];
                        if (gcnt > 3)
                        {
                            //数据处理流程
                            PreprocessRecvData(ReceiveBuf, gcnt);
                        }
                        gcnt = 0;
                    }
                    else
                    {
                        ReceiveBuf[gcnt++] = OrgBuff[i];
                        if (gcnt > 1024)//帧长超过1024，直接return
                        {
                            return;
                        }
                    }
                }
            }
        }
        //处理数据缓存的线程函数
        public void ResThread()
        {
            workThread = new Thread(ResHander);
            workThread.IsBackground = true;
            workThread.Priority = ThreadPriority.Highest;
            workThread.Start();
        }
        
        public void ResHander()
        {
            while (true)
            {
                StateObject Stateque = new StateObject();
                lock (Locker2)
                {
                    if (queue.TryDequeue(out Stateque))
                    {
                        byte[] ReceiveBuf = new byte[Stateque.revLength];
                        //数据处理，循环解析
                        int gcnt = 0;
                        //读取成功
                        for (int i = 0; i < Stateque.revLength; i++)
                        {
                            //找到帧头
                            if (gcnt == 0)
                            {
                                if (Stateque.buffer[i] == 0xFF)
                                {
                                    ReceiveBuf[0] = Stateque.buffer[i];
                                    gcnt = 1;
                                }
                            }
                            else if (gcnt == 1)
                            {
                                if (Stateque.buffer[i] == 0xFF)
                                {
                                    ReceiveBuf[1] = Stateque.buffer[i];
                                    gcnt = 2;
                                }
                                else
                                {
                                    gcnt = 0;
                                    continue;
                                }
                            }
                            else
                            {
                                if (Stateque.buffer[i] == 0xFF)
                                {
                                    ReceiveBuf[gcnt++] = Stateque.buffer[i];
                                    if (gcnt > 3)
                                    {
                                        //数据处理流程
                                        PreprocessRecvData(ReceiveBuf, gcnt);
                                    }
                                    gcnt = 0;
                                }
                                else
                                {
                                    ReceiveBuf[gcnt++] = Stateque.buffer[i];
                                    if (gcnt > 1024)//帧长超过1024，直接return
                                    {
                                        return;
                                    }
                                }
                            }
                        }
                    }
                }
                Thread.Sleep(10);
            }
        }
        //处理数据缓存的线程函数
        public void QueueHanderThread()
        {
            queueThread = new Thread(QueueDataHanderFun3);
            queueThread.IsBackground = true;
            queueThread.Priority = ThreadPriority.Normal;
            queueThread.Start();
        }
        //处理激光心跳的线程
        public void ActiveJGHeart()
        {
            HeartAct = new Thread(ActiveJGheart);
            HeartAct.IsBackground = true;
            HeartAct.Priority = ThreadPriority.Normal;
            HeartAct.Start();
        }

        public void ActiveJGheart()
        {
            //发送激光心跳
            while (true)
            {
                if (GlobalMember.isDeedJGHeart)
                {
                    TcpReply(0x9D, 0x00, JGTcpClient);
                    GlobalMember.isDeedJGHeart = false;
                }
                Thread.Sleep(1000);
            }
            
            
        }

        //第三版本数据匹配逻辑
        public void QueueDataHanderFun3()
        {
            StopType stp = new StopType(this);
            while (true)
            {
                //根据拦截模式是否开启，选择不同的匹配逻辑
                if (JGandRSUInSameLocation)
                {
                    //同杆只有稽查模式
                    stp.StartJiChaType(qRSUData, qJGData, sql_dbname);
                }
                else
                {
                    if (isOpenStoptype.Checked)
                    {
                        //带位置的拦截模式
                        stp.StartStoptypeWithLocation(qRSUData, qJGData, sql_dbname);
                    }
                    else
                    {
                        //带位置的稽查模式
                        stp.JiChaTypeWithLocation(qRSUData, qJGData, sql_dbname);
                    }
                }
                
                Thread.Sleep(10);
            }
        }
        //已匹配数据显示
        public string MarchedShow(string m_sOBUCarType, string m_sOBUPlateNum, string m_sJGCarType, string m_sCamPlateNum, string m_sPicPath, string m_sCarCount,CarFullInfo m_CarFullInfo)
        {
            int ShutType=0;
            string sZuobiString = "";
            int iJGCarType = 0;
            int iOBUCarType = 0; 
            if (m_sJGCarType == "未知"||m_sJGCarType=="未检测"||m_sJGCarType==""||m_sJGCarType==null)
            {
                sZuobiString = "作弊未知";
                iJGCarType = 0;
                iOBUCarType = Convert.ToInt16(m_sOBUCarType.Substring(1));
            }
            else
            {
                iJGCarType = Convert.ToInt16(m_sJGCarType.Substring(1));
                iOBUCarType = Convert.ToInt16(m_sOBUCarType.Substring(1));
                if (iJGCarType <= iOBUCarType)
                {
                    m_sJGCarType = m_sOBUCarType;
                    sZuobiString = "正常通车";
                    //检索黑白名单.以下为暂时测试用
                    //如果开启了拦截模式，进行黑白名单检索
                    if (isOpenStoptype.Checked)
                    {
                        Log.WritePlateLog(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + " 这个时候开始查询黑白名单 " + "\r\n");
                        //查询黑白名单
                        bool isZuobi=false;
                        if (GlobalMember.SqlType.Equals("SQLServer"))
                        {
                            isZuobi = GlobalMember.SQLInter.FindBlackOrWhiteCar(m_sOBUPlateNum, ref ShutType);
                        }
                        else
                        {
                            isZuobi = GlobalMember.MysqlInter.FindBlackOrWhiteCar(m_sOBUPlateNum, ref ShutType);
                        }
                         
                        Log.WritePlateLog(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + " 这个时候已经查完啦 " + "\r\n");
                        if (isZuobi)
                        {
                            switch (ShutType)
                            {
                                case 0:
                                    //普通车辆，不做任何操作，继续交易
                                    //TcpReply(0xA4, 01, RSUTcpClient);
                                    break;
                                case 1:
                                    //白名单 继续交易
                                    //TcpReply(0xA4, 01, RSUTcpClient);
                                    break;
                                case 2:
                                    //先通知天线终止交易
                                    Log.WritePlateLog(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + " 这个时候开始准备写数据库啦 " + "\r\n");
                                    //TcpReply(0xA4, 02, RSUTcpClient);
                                    //启动报警器
                                    //Alarm();
                                    //写入黑白名单
                                    string InsertString = @"Insert into " + sql_dbname
                                        + ".dbo.ShutTable(JGCarType,OBUCarType,ForceTime,CamPlateColor,CamPlateNum,CamPicPath,"
                                        + "OBUPlateColor,OBUPlateNum,TradeTime,InputTime,CarFlag) values('"
                                        + m_CarFullInfo.sJGCarType + "','" + m_CarFullInfo.sOBUCartype + "','" + m_CarFullInfo.sJGDateTime + "','"
                                        + m_CarFullInfo.sCamPlateColor + "','" + m_CarFullInfo.sCamPlateNum + "','"
                                        + m_CarFullInfo.sCamPicPath + "','" + m_CarFullInfo.sOBUPlateColor + "','"
                                        + m_CarFullInfo.sOBUPlateNum + "','" + m_CarFullInfo.sOBUDateTime + "','"
                                        + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "','" + (ShutType - 3).ToString() + "')";
                                    if (GlobalMember.SqlType.Equals("SQLServer"))
                                    {
                                        GlobalMember.SQLInter.UpdateSQLData(InsertString);
                                    }
                                    else
                                    {
                                        GlobalMember.MysqlInter.UpdateSQLData(InsertString);
                                    }
                                    
                                    Log.WritePlateLog(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + " 这个时候数据库写完啦 " + "\r\n");
                                    break;
                                case 3:
                                    //查询异常 继续交易
                                    //TcpReply(0xA4, 01, RSUTcpClient);
                                    break;
                                default:
                                    //-1,-2
                                    //先通知天线控制器终止交易
                                    //TcpReply(0xA4, 02, RSUTcpClient);
                                    //启动报警器
                                    //Alarm();
                                    break;
                            }
                        }
                        Log.WritePlateLog(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + " 返回得到结果，标识位： " + ShutType.ToString() + "\r\n");
                    }
                }
                else
                {
                    sZuobiString = "可能作弊";
                    //检索黑白名单
                    //如果开启了拦截模式，进行黑白名单检索
                    if (isOpenStoptype.Checked)
                    {
                        Log.WritePlateLog(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + " 已经完成匹配，开始查询黑白名单 "  + "\r\n");
                        //查询黑白名单
                        bool isZuobi=false;
                        if (GlobalMember.SqlType.Equals("SQLServer"))
                        {
                            isZuobi = GlobalMember.SQLInter.FindBlackOrWhiteCar(m_sOBUPlateNum, ref ShutType);
                        }
                        else
                        {
                            isZuobi = GlobalMember.MysqlInter.FindBlackOrWhiteCar(m_sOBUPlateNum, ref ShutType);
                        }
                        if (isZuobi)
                        {
                            switch (ShutType)
                            { 
                                case 0:
                                    //普通车辆，不做任何操作，继续交易
                                    TcpReply(0xA4, 01, RSUTcpClient);
                                    break;
                                case 1:
                                    //白名单 继续交易
                                    TcpReply(0xA4, 01, RSUTcpClient);
                                    break;
                                case 2:
                                    //先通知天线终止交易
                                    TcpReply(0xA4, 02, RSUTcpClient);
                                    //启动报警器
                                    //Alarm();
                                    //写入黑白名单
                                    string InsertString = @"Insert into " + sql_dbname
                                        + ".dbo.ShutTable(JGCarType,OBUCarType,ForceTime,CamPlateColor,CamPlateNum,CamPicPath,"
                                        +"OBUPlateColor,OBUPlateNum,TradeTime,InputTime,CarFlag) values('"
                                        + m_CarFullInfo.sJGCarType + "','" + m_CarFullInfo.sOBUCartype + "','" + m_CarFullInfo.sJGDateTime + "','"
                                        + m_CarFullInfo.sCamPlateColor + "','" + m_CarFullInfo.sCamPlateNum + "','"
                                        + m_CarFullInfo.sCamPicPath + "','" + m_CarFullInfo.sOBUPlateColor + "','" 
                                        + m_CarFullInfo.sOBUPlateNum + "','" + m_CarFullInfo.sOBUDateTime + "','"
                                        + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "','" + (ShutType-3).ToString() + "')";
                                    if (GlobalMember.SqlType.Equals("SQLServer"))
                                    {
                                        GlobalMember.SQLInter.UpdateSQLData(InsertString);
                                    }
                                    else
                                    {
                                        GlobalMember.MysqlInter.UpdateSQLData(InsertString);
                                    }
                                    break;
                                case 3:
                                    //查询异常 继续交易
                                    TcpReply(0xA4, 01, RSUTcpClient);
                                    break;
                                default:
                                    //-1,-2
                                    //先通知天线控制器终止交易
                                    TcpReply(0xA4, 02, RSUTcpClient);
                                    //启动报警器
                                    Thread thread1 = new Thread(new ThreadStart(Alarm));
                                    thread1.Start();
                                    break;
                            }
                        }
                        Log.WritePlateLog(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + " 返回得到结果，标识位： "+ShutType.ToString() + "\r\n");
                    }
                }
            }
            if (m_sPicPath != "未知"&&m_sPicPath!=""&&m_sPicPath!=null)
            {
                pictureBoxVehshow(m_sPicPath);//显示图片
            }
            plateNoshow(m_sOBUPlateNum, m_sOBUCarType, m_sCamPlateNum, m_sJGCarType, m_sCarCount);
            return sZuobiString;
        }
        //单纯的显示主界面，更新图片
        public void MarchedShow(string m_sOBUCarType, string m_sOBUPlateNum, string m_sJGCarType, string m_sCamPlateNum, string m_sPicPath, string m_sCarCount)
        {
            if (m_sPicPath != "未知" && m_sPicPath != "" && m_sPicPath != null)
            {
                pictureBoxVehshow(m_sPicPath);//显示图片
            }
            plateNoshow(m_sOBUPlateNum, m_sOBUCarType, m_sCamPlateNum, m_sJGCarType, m_sCarCount);
        }
        //判断是否作弊（用于拦截模式）
        public bool isZuobi(string m_OBUPlateNum)
        {
            int ShutType = 0;
            //如果开启了拦截模式，进行黑白名单检索
            if (isOpenStoptype.Checked)
            {
                Log.WritePlateLog(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + " 这个时候开始查询黑白名单 " + "\r\n");
                //查询黑白名单
                bool isZuobi = false;
                if (GlobalMember.SqlType.Equals("SQLServer"))
                {
                    isZuobi = GlobalMember.SQLInter.FindBlackOrWhiteCar(m_OBUPlateNum, ref ShutType);
                }
                else
                {
                    isZuobi = GlobalMember.MysqlInter.FindBlackOrWhiteCar(m_OBUPlateNum, ref ShutType);
                }
                Log.WritePlateLog(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + " 这个时候已经查完啦 " + "\r\n");
                if (isZuobi)
                {
                    return true;
                }   
            }
            return false;
        }
        //判断是否作弊，重载
        public string isZuobi(string m_sOBUCarType, string m_sOBUPlateNum, string m_sJGCarType, AllInfo m_allinfo,ref int ShutType)
        {
            ShutType = 0;
            string sZuobiString = "";
            int iJGCarType = 0;
            int iOBUCarType = 0;
            if (m_sJGCarType == "未知" || m_sJGCarType == "未检测" || m_sJGCarType == "" || m_sJGCarType == null)
            {
                sZuobiString = "作弊未知";
                iJGCarType = 0;
                iOBUCarType = Convert.ToInt16(m_sOBUCarType.Substring(1));
            }
            else
            {
                iJGCarType = Convert.ToInt16(m_sJGCarType.Substring(1));
                iOBUCarType = Convert.ToInt16(m_sOBUCarType.Substring(1));
                if (iJGCarType <= iOBUCarType)
                {
                    m_sJGCarType = m_sOBUCarType;
                    sZuobiString = "正常通车";
                    TcpReply(0xA4, 01, RSUTcpClient);
                }
                else
                {
                    if ((NotStopNear && (Math.Abs(iJGCarType - iOBUCarType) == 1))
                        || (iOBUCarType == 3 && iJGCarType == 4 && NotStopBetween3And4)
                        || (NotStopWhite && m_allinfo.sOBUPlateColor.Equals("白色")))
                    {
                        sZuobiString = "情况允许";
                        TcpReply(0xA4, 01, RSUTcpClient);
                        //TcpReply(0xD6, 00, RSUTcpClient);
                    }
                    else
                    {
                        sZuobiString = "可能作弊";
                        //检索黑白名单
                        //如果开启了拦截模式，进行黑白名单检索
                        if (isOpenStoptype.Checked)
                        {
                            Log.WritePlateLog(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + " 已经完成匹配，开始查询黑白名单 " + "\r\n");
                            //查询黑白名单
                            bool isZuobi = false;
                            if (GlobalMember.SqlType.Equals("SQLServer"))
                            {
                                isZuobi = GlobalMember.SQLInter.FindBlackOrWhiteCar(m_sOBUPlateNum, ref ShutType);
                            }
                            else
                            {
                                isZuobi = GlobalMember.MysqlInter.FindBlackOrWhiteCar(m_sOBUPlateNum, ref ShutType);
                            }
                            if (isZuobi)
                            {
                                switch (ShutType)
                                {
                                    case 0:
                                        //普通车辆，不做任何操作，继续交易
                                        TcpReply(0xA4, 01, RSUTcpClient);
                                        break;
                                    case 1:
                                        //白名单 继续交易
                                        TcpReply(0xA4, 01, RSUTcpClient);
                                        break;
                                    case 2:
                                        //先通知天线终止交易
                                        //TcpReply(0xA4, 02, RSUTcpClient);
                                        //启动报警器
                                        //Alarm();
                                        //写入黑白名单
                                        
                                     if (GlobalMember.SqlType.Equals("SQLServer"))
                                    {
                                        string InsertString = @"Insert into " + sql_dbname
                                        + ".dbo.ShutTable(JGCarType,ForceTime,CamPlateColor,CamPlateNum,CamPicPath,"
                                        + "OBUPlateColor,OBUPlateNum,TradeTime,InputTime,CarFlag) values('"
                                        + m_allinfo.sJGCarType + "','" + m_allinfo.sJGDateTime + "','"
                                        + m_allinfo.sCamPlateColor + "','" + m_allinfo.sCamPlateNum + "','"
                                        + m_allinfo.sCamPicPath + "','" + m_allinfo.sOBUPlateColor + "','"
                                        + m_allinfo.sOBUPlateNum + "','" + m_allinfo.sOBUDateTime + "','"
                                        + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "','" + (ShutType - 3).ToString() + "')";
                                        GlobalMember.SQLInter.UpdateSQLData(InsertString);
                                    }
                                    else
                                    {
                                        string InsertString = @"Insert into ShutTable(JGCarType,ForceTime,CamPlateColor,CamPlateNum,CamPicPath,"
                                        + "OBUPlateColor,OBUPlateNum,TradeTime,InputTime,CarFlag) values('"
                                        + m_allinfo.sJGCarType + "','" + m_allinfo.sJGDateTime + "','"
                                        + m_allinfo.sCamPlateColor + "','" + m_allinfo.sCamPlateNum + "','"
                                        + m_allinfo.sCamPicPath + "','" + m_allinfo.sOBUPlateColor + "','"
                                        + m_allinfo.sOBUPlateNum + "','" + m_allinfo.sOBUDateTime + "','"
                                        + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "','" + (ShutType - 3).ToString() + "')";
                                        GlobalMember.MysqlInter.UpdateSQLData(InsertString);
                                    }
                                        break;
                                    case 3:
                                        //查询异常 继续交易
                                        TcpReply(0xA4, 01, RSUTcpClient);
                                        break;
                                    default:
                                        //-1,-2
                                        //先通知天线控制器终止交易
                                        //TcpReply(0xA4, 02, RSUTcpClient);
                                        //启动报警器
                                        //Alarm();
                                        break;
                                }
                            }
                            Log.WritePlateLog(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + " 返回得到结果，标识位： " + ShutType.ToString() + "\r\n");
                        }
                    }
                }
            }
            return sZuobiString;
        }
        //数据处理函数
        Thread handerJGCam = null;
        //public delegate void ParameterizedThreadStart(byte[] databuff, int bufflen);
        private void PreprocessRecvData(byte[] p_pBuffer, int p_nLen)//
        {
            int ret = datah.DataEncoding(ref p_pBuffer, ref p_nLen);
            if (ret != 0)
            {
                return;
            }
            switch (p_pBuffer[3])
            {
                case 0x81:
                    //激光数据
                    HeartJGCount = 0;
                    Log.WritePlateLog(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + " 激光数据81帧解包完成 " + "\r\n");
                    TcpReply(0x18,0x00, JGTcpClient);
                    HanderJGData(p_pBuffer, p_nLen);
                    break;
                case 0x83:
                    //激光数据（拦截模式）
                    HeartJGCount = 0;
                    Log.WritePlateLog(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + " 激光数据83帧解包完成 " + "\r\n");
                    //TcpReply(0x83, 0x00, JGTcpClient);
                    HanderJGMainData(p_pBuffer, p_nLen);
                    break;
                case 0x9D:
                    //RSU的心跳
                    HeartRSUCount = 0;
                    TcpReply(0xD9,0x00, RSUTcpClient);
                    break;
                case 0xD9:
                    //激光心跳
                    HeartJGCount = 0;
                    break;
                case 0xD8:
                    //激光位置
                    HeartJGCount = 0;
                    SaveLocation(p_pBuffer, p_nLen);
                    break;
                case 0x7D:
                    //ETC数据
                    HeartRSUCount = 0;
                    TcpReply(0xD7,0x00, RSUTcpClient);
                    HanderRSUData(p_pBuffer, p_nLen);
                    break;
                case 0x4A:
                    //ETC数据(拦截模式下)
                    GlobalMember.g_TimeForTest = DateTime.Now;
                    HeartRSUCount = 0;
                    HanderRSUData(p_pBuffer, p_nLen);
                    break;
                case 0x82:
                    //通知摄像机即将抓拍
                    HeartJGCount = 0;
                    try
                    {
                        //使用线程防止82帧占用主线程时间
                        handerJGCam = new Thread(() => HanderJGStartCam(p_pBuffer, p_nLen));
                        handerJGCam.Start();
                        //HanderJGStartCam(p_pBuffer, p_nLen);
                    }
                    catch(Exception ex)
                    {
                        Log.WriteLog(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "通知摄像机抓拍触发的异常 "+ex.ToString() + "\r\n");
                    }
                    break;
                default:
                    break;
            }
        }
        //保存收到的位置数据，一般只用于测试，正常模式下，该功能不适用
        public void SaveLocation(byte[] buffer, int bufferlen)
        {
            string ss = "";
            string location = "";
            for (int i = 0; i < bufferlen; i++)
            {
                ss += buffer[i].ToString("X2");
                ss += " ";
            }
            Log.WritePlateLog(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + " 收到激光的位置数据" + ss + "\r\n");
            AddOperLogCacheStr("收到激光返回的位置数据" + ss);
            for (int i = 0; i < 8; i++)
            {
                location += ((long)(buffer[(i + 1) * 4] << 24 | buffer[(i + 1) * 4 + 1] << 16 | buffer[(i + 1) * 4 + 2] << 8 | buffer[(i + 1) * 4 + 3])).ToString();
                Log.WritePlateLog(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "第" + i + "个位置" + location + "\r\n");
                location = "";
            }
        }
        //回复帧，输入主命令号，副命令号，和对应要发送的Socket
        public void TcpReply(byte command,byte secondcom, SocketHelper.TcpClients sk)
        {

            int send_lenth = 0;
            byte[] send_buffer;
            send_buffer = new byte[100];
            send_buffer[send_lenth++] = command;
            send_buffer[send_lenth++] = secondcom;
            datah.DataCoding(ref send_buffer, ref send_lenth);
            if (sk == JGTcpClient)
            {
                try
                {
                    JGTcpClient.SendByteData(send_buffer, send_lenth);
                }
                catch (Exception ex)
                {
                    AddOperLogCacheStr("回复激光异常" + ex.ToString());
                }
            }
            else if (sk == RSUTcpClient)
            {
                try
                {
                    RSUTcpClient.SendByteData(send_buffer, send_lenth);
                }
                catch (Exception ex)
                {
                    AddOperLogCacheStr("回复RSU异常" + ex.ToString());
                }
            }

        }
        //适用于稽查模式下的发送位置数据
        public void SendLocation(ushort obuy, long randCode)
        {
            int send_lenth = 0;
            byte[] send_buffer;
            send_buffer = new byte[100];
            send_buffer[send_lenth++] = 0x8D;
            send_buffer[send_lenth++] = (byte)(obuy >> 8);
            send_buffer[send_lenth++] = (byte)(obuy);
            send_buffer[send_lenth++] = (byte)(randCode >> 24);
            send_buffer[send_lenth++] = (byte)(randCode >> 16);
            send_buffer[send_lenth++] = (byte)(randCode >> 8);
            send_buffer[send_lenth++] = (byte)(randCode);
            datah.DataCoding(ref send_buffer, ref send_lenth);
            if (IsConnJG == false)
                return;
            try
            {
                string sss = "";
                JGTcpClient.SendByteData(send_buffer, send_lenth);
                //jg_sock.Send(send_buffer, 0, send_lenth, 0);
                for (int i = 0; i < send_lenth; i++)
                {
                    sss += send_buffer[i].ToString("X2");
                    sss += " ";
                }
                Log.WritePlateLog(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + " 发送位置:" + sss + "\r\n");

            }
            catch (Exception ex)
            {
                AddOperLogCacheStr("发送位置A异常" + ex.ToString());
            }
        }
        //重载，发送位置，这里适用于拦截模式的发送位置数据
        public void SendLocation(ushort obuy)
        {
            int send_lenth = 0;
            byte[] send_buffer;
            send_buffer = new byte[100];
            send_buffer[send_lenth++] = 0x38;
            send_buffer[send_lenth++] = (byte)(obuy >> 8);
            send_buffer[send_lenth++] = (byte)(obuy);
            datah.DataCoding(ref send_buffer, ref send_lenth);
            if (IsConnJG == false)
                return;
            try
            {
                string sss = "";
                JGTcpClient.SendByteData(send_buffer, send_lenth);
                HeartRSUCount = 0;
                for (int i = 0; i < send_lenth; i++)
                {
                    sss += send_buffer[i].ToString("X2");
                    sss += " ";
                }
                Log.WritePlateLog(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + " 发送位置:" + sss + "\r\n");

            }
            catch (Exception ex)
            {
                AddOperLogCacheStr("发送位置B异常" + ex.ToString());
            }
        }
        #endregion

        #region******激光数据解析与加入队列******
        //一般稽查模式下的激光入栈数据
        public void HanderJGData(byte[] databuff, int bufflen)
        {
           
            int st = 2;
            string ss = "";
            for (int i = 0; i < bufflen; i++)
            {
                ss += databuff[i].ToString("X2");
                ss += " ";
            }
            AddOperLogCacheStr("收到激光数据" + ss);
            QueueJGData m_qJG = new QueueJGData();
            bool match_flag = false;//匹配标识
            m_qJG.qJGId = ((ushort)(databuff[3 + st] << 8 | databuff[4 + st])).ToString();
            foreach (var camlist in listCamInfo)
            {
                if (camlist.qJGID == m_qJG.qJGId)
                {
                    m_qJG.qCamPlateNum = camlist.qCamPlateNum;
                    m_qJG.qCamPlateColor = camlist.qCamPlateColor;
                    m_qJG.qCambiao = camlist.qCambiao;
                    m_qJG.qCamPicPath = camlist.qCamPicPath;
                    match_flag = true;
                    break;
                }
            }
            m_qJG.qJGCarType = databuff[5 + st].ToString();
            m_qJG.qJGLength = ((ushort)(databuff[9 + st] << 8 | databuff[10 + st])).ToString();
            m_qJG.qJGHigh = ((ushort)(databuff[11 + st] << 8 | databuff[12 + st])).ToString();
            m_qJG.qJGDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff");
            m_qJG.qJGRandCode = (long)(databuff[13 + st] << 24 | databuff[14 + st] << 16 | databuff[15 + st] << 8 | databuff[16 + st]);
            //1~4 客1~4；5~11 货1~5
            switch (databuff[5 + st])
            {
                case 1:
                    m_qJG.qJGCarType = "客1";
                    break;
                case 2:
                    m_qJG.qJGCarType = "客2";
                    break;
                case 3:
                    m_qJG.qJGCarType = "客3";
                    break;
                case 4:
                    m_qJG.qJGCarType = "客4";
                    break;
                case 5:
                    m_qJG.qJGCarType = "货1";
                    break;
                case 6:
                    m_qJG.qJGCarType = "货2";
                    break;
                case 7:
                    m_qJG.qJGCarType = "货3";
                    break;
                case 8:
                    m_qJG.qJGCarType = "货4";
                    break;
                case 9:
                    m_qJG.qJGCarType = "货5";
                    break;
                case 10:
                    m_qJG.qJGCarType = "货6";
                    break;
                case 11:
                    m_qJG.qJGCarType = "货7";
                    break;
                default:
                    m_qJG.qJGCarType = "未知";
                    break;
            }
            Log.WritePlateLog(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "  激光入栈" 
                +"车牌："+ m_qJG.qCamPlateNum +"车型"+m_qJG.qJGCarType+"车长："+m_qJG.qJGLength+"车高："
                +m_qJG.qJGHigh+"车标："+m_qJG.qCambiao+"激光ID"+m_qJG.qJGId+"随机码："+m_qJG.qJGRandCode.ToString("X2")+ "\r\n");
            //Log.WritePlateLog(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + " 激光入栈原始数据：" + ss + "\r\n");
            Log.WritePlateLog(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "激光数据81帧解析并入栈完成 " + "\r\n");
            lock (qJGData)
            {
                qJGData.Enqueue(m_qJG);//放入激光的缓存中
            }
            jg_inQueueDone.Set();
        }

        //拦截模式下，激光处理全位置数据并入栈
        HanderJGDataToQueue HJGDataToQ = new HanderJGDataToQueue();
        public void HanderJGMainData(byte[] databuff, int bufflen)
        {
            string ss = "";
            for (int i = 0; i < bufflen; i++)
            {
                ss += databuff[i].ToString("X2");
                ss += " ";
            }
            AddOperLogCacheStr("收到激光主数据" + ss);
            QueueJGData m_qJG = new QueueJGData();
            //车道内车的数量
            m_qJG.qJGListNum = databuff[4];
            NumInJGList = m_qJG.qJGListNum;
            byte[] JGList = new byte[11];
            for (int i = 0; i < NumInJGList; i++)
            {
                System.Buffer.BlockCopy(databuff, i*11+5, JGList, 0, 11);
                m_qJG = HJGDataToQ.HanderJGDataIn(JGList, 11);
                lock (listJGMain)
                {
                    listJGMain.Add(new JGMainList(m_qJG.qJGId, m_qJG.qJGLocation, m_qJG.qJGLength, m_qJG.qJGHigh, m_qJG.qJGGetPic, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff")));
                }
            }
            
            JGMainDone.Set();
        }


        #endregion

        #region******通知摄像机抓拍******
        public void HanderJGStartCam(byte[] databuff, int bufflen)
        {
            byte[] sb1 = new byte[2];
            sb1[0] = 0xc1;
            sb1[1] = 0x00;
            
            //JGTcpClient.SendByteData(sb1, 2);
            
            Log.WritePlateLog(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "  收到82帧摄像机抓拍命令"+ "\r\n");
            int st = 2;
            QueueJGData m_qJG = new QueueJGData();
            bool match_flag = false;//匹配标识
            m_qJG.qJGId = ((ushort)(databuff[3 + st] << 8 | databuff[4 + st])).ToString();
            m_qJG.qJGCarType = "客"+databuff[5 + st].ToString();
            m_qJG.qJGLength = ((ushort)(databuff[9 + st] << 8 | databuff[10 + st])).ToString();
            m_qJG.qJGHigh = ((ushort)(databuff[11 + st] << 8 | databuff[12 + st])).ToString();
            m_qJG.qJGDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff");
            
            //m_qJG.qJGRandCode = (long)(databuff[13 + st] << 24 | databuff[14 + st] << 16 | databuff[15 + st] << 8 | databuff[16 + st]);
            if (OpenForce.Checked && CameraType == "IPC")
            {
                //IPC启用强制抓拍
                //由于IPC软触发也是回调，所以不能放入下面流程去
                sb1[0] = 0xc2;
                JGTcpClient.SendByteData(sb1, 2);
                Log.WritePlateLog(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "  开始软触发" + "\r\n");
                GlobalMember.IPCCameraInter.ForceGetPic();
                //JGTcpClient.SendByteData(sb1, 2);
                Log.WritePlateLog(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "  开始等待摄像机完成信号量" + "\r\n");
                CameraPicture.Reset();
                CameraCanpost.Set();
                if (CameraPicture.WaitOne(1200))
                {
                    sb1[0] = 0xc3;
                    //JGTcpClient.SendByteData(sb1, 2);
                    Log.WritePlateLog(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "  摄像机信号量已完成" + "\r\n");
                    match_flag = true;//匹配成功 
                    if (CameraType == "IPC")
                    {
                        m_qJG.qCamPicPath = IPCCamera.imagepath;
                    }
                }
                else
                {
                    Log.WritePlateLog(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "  摄像机信号量已超时" + "\r\n");
                    match_flag = false;//匹配失败
                    m_qJG.qCamPicPath = "未知";
                }
                CameraCanpost.Reset();
            }
            if (OpenForce.Checked)
            {
                if (CameraType == "HK")
                {
                    int res = GlobalMember.HKCameraInter.camera_ForceGetBigImage();
                    if (res == 0)
                    {
                        match_flag = true;
                        m_qJG.qCamPicPath = HKCamera.imagepath;
                        AddOperLogCacheStr("强制抓拍成功");
                    }
                    else
                    {
                        match_flag = false;
                        m_qJG.qCamPicPath = "未知";
                        AddOperLogCacheStr("强制抓拍失败");
                    }
                }
                
                

            }
            else
            {
                Log.WritePlateLog(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "  开始等待摄像机完成信号量" + "\r\n");
                CameraPicture.Reset();
                CameraCanpost.Set();
                if (CameraPicture.WaitOne(1200))
                {
                    Log.WritePlateLog(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "  摄像机信号量已完成" + "\r\n");
                    match_flag = true;//匹配成功 
                    if (CameraType == "HK")
                    {
                        m_qJG.qCamPicPath = HKCamera.imagepath;
                    }
                    else if (CameraType == "IPC")
                    {
                        m_qJG.qCamPicPath = IPCCamera.imagepath;
                    }
                    else if (CameraType == "IPNC")
                    {
                        m_qJG.qCamPicPath = IPNCCamera.imagepath;
                    }
                }
                else
                {
                    Log.WritePlateLog(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "  摄像机信号量已超时" + "\r\n");
                    match_flag = false;//匹配失败
                    m_qJG.qCamPicPath = "未知";
                }
                CameraCanpost.Reset();
            }
            
            
            if (match_flag == true)
            {
                if (CameraType == "HK")
                {
                    if (HKCamera.GetPlateNo == "未检测" || HKCamera.GetPlateNo == "无车牌")
                    {
                        m_qJG.qCamPlateColor = "未检测";
                        m_qJG.qCamPlateNum = HKCamera.GetPlateNo;
                        m_qJG.qCambiao = "未知";
                    }
                    else if (HKCamera.GetPlateNo == "" || HKCamera.GetPlateNo == "无牌车")
                    {
                        m_qJG.qCamPlateColor = "无牌车";
                        m_qJG.qCamPlateNum = "无牌车";
                        m_qJG.qCambiao = HKCamera.GetVehicleLogoRecog;
                    }
                    else
                    {

                        if (HKCamera.GetPlateNo.Length > 3)
                        {
                            m_qJG.qCamPlateColor = HKCamera.GetPlateNo.Substring(0, 1);
                            m_qJG.qCamPlateNum = HKCamera.GetPlateNo.Substring(1);
                            m_qJG.qCambiao = HKCamera.GetVehicleLogoRecog;
                        }

                    }
                    m_qJG.qCamPicPath = HKCamera.imagepath;
                }
                else if (CameraType == "IPC")
                {
                    if (IPCCamera.GetPlateNo == "未检测" || IPCCamera.GetPlateNo == "无车牌")
                    {
                        m_qJG.qCamPlateColor = "未检测";
                        m_qJG.qCamPlateNum = IPCCamera.GetPlateNo;
                        m_qJG.qCambiao = "未知";
                    }
                    else if (IPCCamera.GetPlateNo == "" || IPCCamera.GetPlateNo == "无牌车")
                    {
                        m_qJG.qCamPlateColor = "无牌车";
                        m_qJG.qCamPlateNum = "无牌车";
                        m_qJG.qCambiao = "未知";
                    }
                    else
                    {
                        if (IPCCamera.GetPlateNo != null)
                        {
                            if (IPCCamera.GetPlateNo.Length > 3)
                            {
                                m_qJG.qCamPlateColor = IPCCamera.PlateColor;
                                m_qJG.qCamPlateNum = IPCCamera.GetPlateNo;
                                m_qJG.qCambiao = IPCCamera.GetVehicleLogoRecog;
                            }
                        }
                    }
                    m_qJG.qCamPicPath = IPCCamera.imagepath;
                }
                else if (CameraType == "IPNC")
                {
                    if (IPNCCamera.GetPlateNo == "未检测" || IPNCCamera.GetPlateNo == "无车牌")
                    {
                        m_qJG.qCamPlateColor = "未检测";
                        m_qJG.qCamPlateNum = IPNCCamera.GetPlateNo;
                        m_qJG.qCambiao = "未知";
                    }
                    else if (IPNCCamera.GetPlateNo == "" || IPNCCamera.GetPlateNo == "无牌车")
                    {
                        m_qJG.qCamPlateColor = "无牌车";
                        m_qJG.qCamPlateNum = "无牌车";
                        m_qJG.qCambiao = "未知";
                    }
                    else
                    {
                        if (IPNCCamera.GetPlateNo != null)
                        {
                            if (IPNCCamera.GetPlateNo.Length > 3)
                            {
                                m_qJG.qCamPlateColor = IPNCCamera.PlateColor;
                                m_qJG.qCamPlateNum = IPNCCamera.GetPlateNo;
                                m_qJG.qCambiao = IPNCCamera.GetVehicleLogoRecog;
                            }
                        }
                    }
                    m_qJG.qCamPicPath = IPNCCamera.imagepath;
                }
            }
            else
            {
                m_qJG.qCamPlateColor = "未知";
                m_qJG.qCamPlateNum = "未知";
                m_qJG.qCamPicPath = "未知";
                m_qJG.qCambiao = "未知";
                m_qJG.qCamPicPath = "未知";
                m_qJG.qCamPicPath = "未知";
            }
            if (string.Equals(m_qJG.qCamPlateColor.ToString(), "黄"))
            {
                if (databuff[5 + st] == 1)
                    databuff[5 + st] = 2;
            }
            Log.WritePlateLog(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "  摄像机抓拍"
                + "车牌：" + m_qJG.qCamPlateNum + "车标：" + m_qJG.qCambiao + "激光ID" + m_qJG.qJGId + "\r\n");
            lock (listCamInfo)
            {
                if (listCamInfo.Count >= 3)
                {
                    try
                    {
                        listCamInfo.RemoveRange(0, 1);
                    }
                    catch (Exception ex)
                    {
                        Log.WriteLog(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") +" 摄像机清栈异常"+ex.ToString()+ "\r\n");
                    }
                }
                listCamInfo.Add(new CamList(m_qJG.qJGId, m_qJG.qCamPlateNum, m_qJG.qCamPlateColor, m_qJG.qCamPicPath, m_qJG.qCambiao, m_qJG.qJGLength, m_qJG.qJGHigh, m_qJG.qJGCarType, m_qJG.qJGDateTime));
            }
        }
        #endregion

        #region******天线数据解析与加入队列******
        HanderRSUDataToQueue HRSUDataToQ = new HanderRSUDataToQueue();
        public void HanderRSUData(byte[] databuff, int bufflen)
        {
            string ss = "";
            for (int i = 0; i < bufflen; i++)
            {
                ss += databuff[i].ToString("X2");
                ss += " ";
            }
            AddOperLogCacheStr("收到天线数据" + ss);
            QueueRSUData m_qRSU = new QueueRSUData();
            m_qRSU = HRSUDataToQ.HanderRSUDataIn(databuff, bufflen);
            
            //SendLocation((ushort)(databuff[8 + 2] << 8 | databuff[9 + 2]), m_qRSU.qRSURandCode);
            //SendLocation(0xfdfd, m_qRSU.qRSURandCode);
            
            lock (qRSUData)
            {
                qRSUData.Enqueue(m_qRSU);//列入RSU数据缓存中
            }
            rsu_inQueueDone.Set();
        }
        #endregion

        private void qqButton3_Click(object sender, EventArgs e)
        {
            QQMessageBox.Show(
                this,
                "更改信息成功a！",
                "提示",
                QQMessageBoxIcon.OK,
                QQMessageBoxButtons.OK);
        }

        #region******数据结果导出******
        private void button1_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.FileName = DateTime.Now.ToString("yyyyMMdd HHmm");
            string PathExcel = "";
            saveFileDialog1.Filter = "Excel files(*.xls)|*.xls";
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {

                PathExcel = saveFileDialog1.FileName;
                ExcelCreate excre = new ExcelCreate();
                DataTable dt = new DataTable();
                dt.Columns.Add("序号");
                dt.Columns.Add("交易时间");
                dt.Columns.Add("车牌号码");
                dt.Columns.Add("OBU车型");
                dt.Columns.Add("检测车型");
                dt.Columns.Add("可能作弊");
                dt.Columns.Add("车长");
                dt.Columns.Add("车高");
                dt.Columns.Add("图片路径");

                for (int i = 0; i < dataGridView1.Rows.Count; i++)
                {
                    DataRow dr = dt.NewRow();
                    dr["序号"] = (dataGridView1.Rows[i].Cells[0].Value);
                    dr["交易时间"] = (dataGridView1.Rows[i].Cells[1].Value);
                    dr["车牌号码"] = (dataGridView1.Rows[i].Cells[2].Value);
                    dr["OBU车型"] = (dataGridView1.Rows[i].Cells[3].Value);
                    dr["检测车型"] = (dataGridView1.Rows[i].Cells[4].Value);
                    dr["可能作弊"] = (dataGridView1.Rows[i].Cells[5].Value);
                    dr["车长"] = (dataGridView1.Rows[i].Cells[6].Value);
                    dr["车高"] = (dataGridView1.Rows[i].Cells[7].Value);
                    dr["图片路径"] = (dataGridView1.Rows[i].Cells[8].Value);

                    dt.Rows.Add(dr);
                }

                string dt12 = DateTime.Now.ToString("yyyyMMddhhmmss");

                excre.OutFileToDisk(dt, "车辆信息数据表", PathExcel);
                MessageBox.Show("Execl导出成功，路劲为：" + PathExcel);
            }
        }

        private void querybutton_Click(object sender, EventArgs e)
        {
            string s_Id;
            string s_RsuTradeTime;
            string s_RsuPlateNum;
            string s_RsuCarType;
            string s_JgCarType;
            string s_IsZuobi;
            string s_JgLength;
            string s_JgWide;
            string s_CamPicPath;
            this.dataGridView1.Rows.Clear();
            try
            {
                if (this.dateEndTime.Value < this.dateStartTime.Value)
                {
                    MessageBox.Show("结束时间必须大于起始时间");
                    return;
                }
                string limit_select = " ";
                if (checkBox1.CheckState == CheckState.Checked)
                {
                    limit_select = " and TradeState='可能作弊'";
                }
                if (GlobalMember.SqlType.Equals("SQLServer"))
                {
                    string CarSerch = "select * from " + sql_dbname + ".dbo.CarInfo where TradeTime > '" + this.dateStartTime.Value.ToString("yyyy-MM-dd HH:mm:ss:fff") + "' and TradeTime < '" + this.dateEndTime.Value.ToString("yyyy-MM-dd HH:mm:ss:fff") + "'" + limit_select + "  order by ID";//车道号
                    SqlDataReader sdr = SQLServerInter.ExecuteQuery(CarSerch);
                    bool flag1 = false;
                    while (sdr.Read())
                    {
                        s_Id = sdr[0].ToString();
                        s_RsuTradeTime = sdr[18].ToString();
                        s_RsuPlateNum = sdr[11].ToString();
                        s_RsuCarType = sdr[17].ToString();
                        s_JgCarType = sdr[3].ToString();
                        s_IsZuobi = sdr[19].ToString();
                        s_JgLength = sdr[1].ToString();
                        s_JgWide = sdr[2].ToString();
                        s_CamPicPath = sdr[8].ToString();
                        DelegateState.InsertGridview(s_Id, s_RsuTradeTime, s_RsuPlateNum, s_RsuCarType, s_JgCarType, s_IsZuobi, s_JgLength, s_JgWide, s_CamPicPath);
                        flag1 = true;
                    }
                    if (flag1 == false)
                        MessageBox.Show("查询完成，没有数据");
                }
                else
                {
                    string CarSerch = "select * from CarInfo where TradeTime > '" + this.dateStartTime.Value.ToString("yyyy-MM-dd HH:mm:ss:fff") + "' and TradeTime < '" + this.dateEndTime.Value.ToString("yyyy-MM-dd HH:mm:ss:fff") + "'" + limit_select + "  order by ID";//车道号
                    MySqlDataReader sdr = MysqlInter.MysqlExecuteQuery(CarSerch);
                    bool flag1 = false;
                    while (sdr.Read())
                    {
                        s_Id = sdr[0].ToString();
                        s_RsuTradeTime = sdr[18].ToString();
                        s_RsuPlateNum = sdr[11].ToString();
                        s_RsuCarType = sdr[17].ToString();
                        s_JgCarType = sdr[3].ToString();
                        s_IsZuobi = sdr[19].ToString();
                        s_JgLength = sdr[1].ToString();
                        s_JgWide = sdr[2].ToString();
                        s_CamPicPath = sdr[8].ToString();
                        DelegateState.InsertGridview(s_Id, s_RsuTradeTime, s_RsuPlateNum, s_RsuCarType, s_JgCarType, s_IsZuobi, s_JgLength, s_JgWide, s_CamPicPath);
                        flag1 = true;
                    }
                    sdr.Dispose();
                    if (flag1 == false)
                        MessageBox.Show("查询完成，没有数据");
                }
                
            }
            catch (SystemException ex)
            {
                Log.WriteLog(DateTime.Now + "\r\n" + "数据库查询失败\r\n" + ex.ToString() + "\r\n");
                MessageBox.Show("查询失败" + ex.ToString());
            }
        }
        #endregion

        private void qqButton1_Click(object sender, EventArgs e)
        {
           // int a = isUsingPiPei.DUIControls.Count;
        }
        //心跳定时检测
        private void timer1_Tick(object sender, EventArgs e)
        {
            HeartJGCount++;
            HeartRSUCount++;
            //if (IsConnJG)
            //{
            
            //}
            if (HeartJGCount >= 5)
            {
                GlobalMember.isDeedJGHeart = true;
                //TcpReply(0x9D, 0x00, JGTcpClient);
            }
            if (HeartJGCount >= 25)
            {
                IsConnJG = false;
                //链接断开了
                JGTcpClient.Stop();
                HeartJGCount = 0;
                Log.WritePlateLog(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + " 激光控制器心跳检测超时，激光通信断开重连\r\n");
            }
            if (HeartRSUCount >= 10)
            {
                //IsConnRSU = false;
                //rsu_sock.Close();
                IsConnRSU = false;
                RSUTcpClient.Stop();
                HeartRSUCount = 0;
                Log.WritePlateLog(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + " 天线控制器心跳检测超时，天线通信断开重连\r\n");
            }
        }
        //窗口关闭退出环境，防止残留线程
        private void FormDemo_FormClosed(object sender, FormClosedEventArgs e)
        {
            JGTcpClient.Stop();
            RSUTcpClient.Stop();
            Environment.Exit(0);
            Application.Exit();
            
            
        }
        //打开串口
        private SerialPort comm = new SerialPort();//串口
        public void OpenCom()
        {
            comm.PortName =GlobalMember.g_sComofAlarm;
            comm.BaudRate = int.Parse("115200");

            try
            {
                if (comm.IsOpen)
                {
                    comm.Close();
                }
                else
                {
                    comm.Open();
                }
            }
            catch (Exception ex)
            {
                Log.WriteLog(DateTime.Now + "\r\n" + "串口打开异常\r\n" + ex.ToString());
                return;
            }

        }
        //关闭串口
        public void CloseCom()
        {
            try
            {
                comm.Close();
            }
            catch (System.Exception ex)
            {
                Log.WriteLog(DateTime.Now + "\r\n" + "串口关闭异常\r\n" + ex.ToString());
                return;
            }
        }
        //启动报警
        public void Alarm()
        {
            try
            {
                OpenCom();
                RTSEnable();//声光灯报警
                System.Threading.Thread.Sleep(GlobalMember.g_iTime0fAlarm);
                RTSDisable();
                CloseCom();
            }
            catch (System.Exception ex)
            {
                Log.WriteLog(DateTime.Now + "\r\n" + "相应报警灯异常\r\n" + ex.ToString());
                return;
            }
        }
        //RTS使能
        public void RTSEnable()
        {
            comm.RtsEnable = true;
        }
        //RTS取消
        public void RTSDisable()
        {
            comm.RtsEnable = false;
        }

        private void imageButtonNext_Click(object sender, EventArgs e)
        {
            Random rd = new Random();
            adddataGridViewRoll("1", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff"), "2",
                                           "3", rd.Next().ToString(), rd.Next(3).ToString(),
                                           "1", "", "",
                                           "", "","","","");
        }

        private void animatedTabs1_Selecting(object sender, TabControlCancelEventArgs e)
        {
            if (GlobalMember.isLogin)
            {
            }
            else
            {
                if (animatedTabs1.SelectedIndex != 0)
                {
                    animatedTabs1.SelectedIndex = 0;
                    frmLogin fml = new frmLogin();
                    fml.Show();
                }
            }

        }
        //更新为白名单
        private void btnInWhite_Click(object sender, EventArgs e)
        {
            SetBlackWhite(labelOBUPlateNum.Text, 1, "白名单");
        }
        //更新为黑名单
        private void btnInBlack_Click(object sender, EventArgs e)
        {
            SetBlackWhite(labelOBUPlateNum.Text, -2, "黑名单");
        }
        //设置黑白名单
        public void SetBlackWhite(string m_sOBUCarNum, int m_iCartye, string m_sBWString)
        {
            int UpdateOut = 1;
            if (m_sOBUCarNum.Equals(""))
            {
                QQMessageBox.Show(
                        this,
                        "当前无目标车辆！",
                        "提示",
                        QQMessageBoxIcon.Warning,
                        QQMessageBoxButtons.OK);
            }
            else
            {
                if (GlobalMember.SqlType.Equals("SQLServer"))
                {
                    UpdateOut = GlobalMember.SQLInter.UpdateBlackOrWhiteCar(m_sOBUCarNum, m_iCartye);
                }
                else
                {
                    UpdateOut = GlobalMember.MysqlInter.UpdateBlackOrWhiteCar(m_sOBUCarNum, m_iCartye);
                }
                if (UpdateOut == 0)
                {
                    QQMessageBox.Show(
                        this,
                        "修改为" + m_sBWString + "成功！",
                        "提示",
                        QQMessageBoxIcon.OK,
                        QQMessageBoxButtons.OK);
                }
                else
                {
                    QQMessageBox.Show(
                        this,
                        "修改为" + m_sBWString + "失败！",
                        "提示",
                        QQMessageBoxIcon.Error,
                        QQMessageBoxButtons.OK);
                }
            }
        }
        //设置按钮
        private void btnconfig_Click(object sender, EventArgs e)
        {
            if (GlobalMember.isLogin)
            {
                animatedTabs1.SelectedIndex = 3;
            }
            else
            {

                frmLogin fml = new frmLogin();
                fml.Show();
                fml.Focus();
                if (GlobalMember.isLogin)
                {
                    animatedTabs1.SelectedIndex = 3;
                }

            }
        }
        //实时数据表详细
        private void dataGridViewRoll_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                int index = this.dataGridViewRoll.CurrentRow.Index;
                string forcetime = (this.dataGridViewRoll.Rows[index].Cells[3].Value).ToString();//交易时间
                string oplatenumber = (this.dataGridViewRoll.Rows[index].Cells[5].Value).ToString();//OBU车牌
                string vehtype = (this.dataGridViewRoll.Rows[index].Cells[1].Value).ToString();//激光检测车辆型号
                string ovehtype = (this.dataGridViewRoll.Rows[index].Cells[2].Value).ToString();//OBU的车型
                string laser_vehlenth = (this.dataGridViewRoll.Rows[index].Cells[11].Value).ToString();//车长
                string laser_vehheight = (this.dataGridViewRoll.Rows[index].Cells[12].Value).ToString();//车高
                string imagepath = (this.dataGridViewRoll.Rows[index].Cells[13].Value).ToString();//图片路径
                string getplateno = (this.dataGridViewRoll.Rows[index].Cells[6].Value).ToString();//摄像机识别车牌
                GridDouclickShowPicDialog gdspd = new GridDouclickShowPicDialog(vehtype, getplateno, imagepath, forcetime, ovehtype, oplatenumber, laser_vehlenth, laser_vehheight, this);
                gdspd.ShowDialog();
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
        //查询表双击事件
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                int index = this.dataGridView1.CurrentRow.Index;
                string forcetime = (this.dataGridView1.Rows[index].Cells[1].Value).ToString();//交易时间
                string oplatenumber = (this.dataGridView1.Rows[index].Cells[2].Value).ToString();//OBU车牌
                string vehtype = (this.dataGridView1.Rows[index].Cells[4].Value).ToString();//激光检测车辆型号
                string ovehtype = (this.dataGridView1.Rows[index].Cells[3].Value).ToString();//OBU的车型
                string laser_vehlenth = (this.dataGridView1.Rows[index].Cells[6].Value).ToString();//车长
                string laser_vehheight = (this.dataGridView1.Rows[index].Cells[7].Value).ToString();//车高
                string imagepath = (this.dataGridView1.Rows[index].Cells[8].Value).ToString();//图片路径
                string getplateno = System.IO.Path.GetFileName((this.dataGridView1.Rows[index].Cells[8].Value).ToString());//摄像机识别车牌
                GridDouclickShowPicDialog gdspd = new GridDouclickShowPicDialog(vehtype, getplateno, imagepath, forcetime, ovehtype, oplatenumber, laser_vehlenth, laser_vehheight, this);
                gdspd.ShowDialog();
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
