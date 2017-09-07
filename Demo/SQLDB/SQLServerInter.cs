using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Threading;

namespace ETCF
{
    class SQLServerInter
    {
        public FormDemo MF = null;
        private static string connStr = @"";
        public SqlConnection SQLconnection = null;
        Thread DataBaseConThread;
        private static string sql_ip;//SQLServer的ip
        private static string sql_dbname;//SQLServer数据库名称
        private static string sql_username;//用户名
        private static string sql_password;//密码
        private static string sql_port;//端口
        

        #region ******数据库连接******
        public SQLServerInter(FormDemo mf,string sqlname,string sqlhost,string sqlusername,string sqlpassword,string connstr)
        {
            if (MF == null)
            {
                MF = mf;
            }
            sql_dbname = sqlname;
            sql_ip = sqlhost;
            sql_username = sqlusername;
            sql_password = sqlpassword;
            connStr = connstr;

        }
        public static SqlConnection Conn
        {
            get
            {
                return new SqlConnection(connStr);
            }
        }
        //执行SQL语句
        public static SqlDataReader ExecuteQuery(string sqlStr)
        {
            SqlCommand cmd = new SqlCommand(sqlStr, Conn);
            cmd.Connection.Open();
            try
            {
                return cmd.ExecuteReader();
                //return cmd.ExecuteReader(System.Data.CommandBehavior.CloseConnection);
            }
            catch (Exception)
            {
                cmd.Connection.Close();
                throw;
            }
        }

        public void SQLInit()
        {
            ////初始化数据库
            if (false == InitSqlserver())
            {
                MF.AddOperLogCacheStr("数据库连接失败！");

            }
            else
            {
                MF.AddOperLogCacheStr("数据库连接成功！");

            }
        }

        public bool InitSqlserver()
        {
            try
            {
                if (SQLconnection == null)
                {
                    string connectionString = @"Persist Security Info=True;User ID=" + sql_username + ";Password =" + sql_password + ";Initial Catalog=" + sql_dbname + ";Data Source=" + sql_ip;
                    SQLconnection = new SqlConnection(connectionString);

                    SQLconnection.Open();
                }
                else if (SQLconnection.State == System.Data.ConnectionState.Closed)
                {
                    SQLconnection.Open();
                }
                else if (SQLconnection.State == System.Data.ConnectionState.Broken)
                {
                    SQLconnection.Close();
                    SQLconnection.Open();
                }

                //开数据库连接维护线程
                DataBaseConThread = new Thread(DataBaseConThr);  //数据库连接维护线程
                DataBaseConThread.IsBackground = true;//程序结束自动退出
                DataBaseConThread.Priority = ThreadPriority.BelowNormal;//Highest，AboveNormal，Normal，BelowNormal，Lowest
                DataBaseConThread.Start();

            }
            catch (System.Exception ex)
            {
                //MessageBox.Show(ex.ToString());
                Log.WriteLog(DateTime.Now + " 数据库初始化异常\r\n" + ex.ToString() + "\r\n");
                return false;
            }

            return true;

        }

        public void DataBaseConThr(object statetemp)          //数据库连接维护线程
        {
            while (true)
            {
                if (SQLconnection.State == System.Data.ConnectionState.Closed)
                {
                    SQLconnection.Open();
                }
                else if (SQLconnection.State == System.Data.ConnectionState.Broken)
                {
                    SQLconnection.Close();
                    SQLconnection.Open();
                    MF.AddOperLogCacheStr("数据库重连成功！");

                }
                Thread.Sleep(7000);
            }

        }
        #endregion

        #region ******数据库操作******
        //插入激光数据
        public bool InsertJGData(string s_JGCarLength, string s_JGCarWide, string s_JGCarType, string s_JGId, string s_CamPlateNum, string s_CamPicPath, string s_CamForceTime, string s_Cambiao, string s_CamPlateColor, string s_RandCode)
        {
            string InsertString = @"Insert into " + sql_dbname + ".dbo.JGInfo(JGLength,JGWide,JGCarType,CamPlateNum,ForceTime,Cambiao,CamPicPath,JGId,CamPlateColor,RandCode) values('" + s_JGCarLength + "','" + s_JGCarWide + "','" + s_JGCarType + "','" + s_CamPlateNum + "','" + s_CamForceTime + "','" + s_Cambiao + "','" + s_CamPicPath + "','" + s_JGId + "','" + s_CamPlateColor + "','" + s_RandCode + "')";
            try
            {
                if (SQLconnection.State != System.Data.ConnectionState.Open)
                {
                    MF.AddOperLogCacheStr("激光数据插入失败！");

                    return false;
                }
                SqlCommand cmd = new SqlCommand(InsertString, SQLconnection);
                cmd.ExecuteNonQuery();
                MF.AddOperLogCacheStr("激光数据插入成功！");
                return true;
            }
            catch (Exception ex)
            {
                MF.AddOperLogCacheStr("激光数据插入失败" + ex.ToString());
                Log.WriteLog(DateTime.Now + " 激光数据入库异常\r\n" + ex.ToString() + "\r\n");
                return false;
            }
        }
        //插入RSU数据
        public bool InsertRSUData(string s_OBUPlateColor, string s_OBUPlateNum, string s_OBUMac, string s_OBUY, string s_OBUBiao,string s_OBUCarLength, string s_OBUCarHigh, string s_OBUCarType, string s_TradeTime, string s_RandCode)
        {
            string InsertString = @"Insert into " + sql_dbname + ".dbo.OBUInfo(OBUPlateColor,OBUPlateNum,OBUMac,OBUY,OBUCarLength,OBUCarHigh,OBUCarType,TradeTime,RandCode) values('" + s_OBUPlateColor + "','" + s_OBUPlateNum + "','" + s_OBUMac + "','" + s_OBUY + "','" + s_OBUCarLength + "','" + s_OBUCarHigh + "','" + s_OBUCarType + "','" + s_TradeTime + "','" + s_RandCode + "')";
            try
            {
                if (SQLconnection.State != System.Data.ConnectionState.Open)
                {
                    MF.AddOperLogCacheStr("天线数据插入失败");
                    return false;
                }
                SqlCommand cmd = new SqlCommand(InsertString, SQLconnection);
                cmd.ExecuteNonQuery();
                MF.AddOperLogCacheStr("天线数据插入成功");
                return true;
            }
            catch (Exception ex)
            {
                MF.AddOperLogCacheStr("天线数据插入失败" + ex.ToString());
                Log.WriteLog(DateTime.Now + " 天线数据入库异常\r\n" + ex.ToString() + "\r\n");
                //MessageBox.Show(ex.ToString());
                return false;
            }
        }
        //数据更新通用函数
        public bool UpdateSQLData(string SQLString)
        {
            try
            {
                if (SQLconnection.State != System.Data.ConnectionState.Open)
                {
                    MF.AddOperLogCacheStr("数据更新失败");
                    return false;
                }
                SqlCommand cmd = new SqlCommand(SQLString, SQLconnection);
                cmd.ExecuteNonQuery();
                MF.AddOperLogCacheStr("数据更新成功");
                return true;
            }
            catch (Exception ex)
            {
                MF.AddOperLogCacheStr("数据更新失败" + ex.ToString());
                Log.WriteLog(DateTime.Now + " 数据库更新异常\r\n" + ex.ToString() + "\r\n");
                return false;
            }
        }
        #endregion
    }
}
