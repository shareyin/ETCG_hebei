using System;
using System.Configuration;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading;

namespace ETCF
{
    public class MysqlInter
    {
        public FormDemo MF = null;
        private static string connStr = @"";
        public MySqlConnection MySqlconnection=null;
        Thread DataBaseConThread;
        private static string sql_dbname = string.Empty;
        private static string sql_ip = string.Empty;
        private static string sql_username = string.Empty;
        private static string sql_password = string.Empty;
        private static string sql_port = string.Empty;


        public MysqlInter(FormDemo mf, string Databasetemp, string DataSourcetemp, string UserIdtemp, string Passwordtemp, string porttemp, string connstrtemp)
        {
            if (MF == null)
            {
                MF = mf;
            }
            sql_dbname = Databasetemp;
            sql_ip = DataSourcetemp;
            sql_username = UserIdtemp;
            sql_password = Passwordtemp;
            sql_port = porttemp;
            connStr = connstrtemp;
        }

        public static MySqlConnection MysqlConn
        {
            get
            {
                return new MySqlConnection(connStr);
            }
        }
        //执行SQL语句
        public static MySqlDataReader MysqlExecuteQuery(string sqlStr)
        {
            MySqlCommand cmd = new MySqlCommand(sqlStr, MysqlConn);
            cmd.Connection.Open();
            try
            {
                return cmd.ExecuteReader();
            }
            catch (Exception)
            {
                cmd.Connection.Close();
                throw;
            }
        }

        public void MysqlInit()
        {
            ////初始化数据库
            if (InitMySql())
            {
                MF.AddOperLogCacheStr("数据库连接成功！");
            }
            else
            {
                MF.AddOperLogCacheStr("数据库连接失败！");
            }
        }
        //
        public bool InitMySql()
        {
            try
            {
                if (MySqlconnection == null)
                {
                    string connectionString = @"Persist Security Info=True;UserId=" + sql_username + ";Password =" + sql_password + ";Database=" + sql_dbname + ";DataSource=" + sql_ip + ";CharSet=utf8;port=" + sql_port ;
                    MySqlconnection = new MySqlConnection(connectionString);
                    MySqlconnection.Open();
                }
                else if (MySqlconnection.State == System.Data.ConnectionState.Closed)
                {
                    MySqlconnection.Open();
                }
                else if (MySqlconnection.State == System.Data.ConnectionState.Broken)
                {
                    MySqlconnection.Close();
                    MySqlconnection.Open();
                }
                //开数据库连接维护线程
                DataBaseConThread = new Thread(DataBaseConThr);  //数据库连接维护线程
                DataBaseConThread.IsBackground = true;//程序结束自动退出
                DataBaseConThread.Priority = ThreadPriority.BelowNormal;//Highest，AboveNormal，Normal，BelowNormal，Lowest
                DataBaseConThread.Start();
            }
            catch (System.Exception ex)
            {
                Log.WriteLog(DateTime.Now + " 数据库初始化异常\r\n" + ex.ToString() + "\r\n");
                return false;
            }
            return true;
        }
        public void DataBaseConThr(object statetemp)          //数据库连接维护线程
        {
            while (true)
            {
                if (MySqlconnection.State == System.Data.ConnectionState.Closed)
                {
                    MySqlconnection.Open();
                }
                else if (MySqlconnection.State == System.Data.ConnectionState.Broken)
                {
                    MySqlconnection.Close();
                    MySqlconnection.Open();
                    MF.AddOperLogCacheStr("数据库重连成功！");

                }
                Thread.Sleep(7000);
            }

        }

        #region ******数据库操作******
        //插入激光数据
        public bool InsertJGData(string s_JGCarLength, string s_JGCarWide, string s_JGCarType, string s_JGId, string s_CamPlateNum, string s_CamPicPath, string s_CamForceTime, string s_Cambiao, string s_CamPlateColor, string s_RandCode, string s_LaneNo)
        {
            string InsertString = @"Insert into JGInfo(JGLength,JGWide,JGCarType,CamPlateNum,ForceTime,Cambiao,CamPicPath,JGId,CamPlateColor,RandCode,LaneNo) values('" + s_JGCarLength + "','" + s_JGCarWide + "','" + s_JGCarType + "','" + s_CamPlateNum + "','" + s_CamForceTime + "','" + s_Cambiao + "','" + s_CamPicPath + "','" + s_JGId + "','" + s_CamPlateColor + "','" + s_RandCode + "','" + s_LaneNo + "')";
            try
            {
                if (MySqlconnection.State != System.Data.ConnectionState.Open)
                {
                    MF.AddOperLogCacheStr("激光数据插入失败！");

                    return false;
                }
                MySqlCommand cmd = new MySqlCommand(InsertString, MySqlconnection);
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
        public bool InsertRSUData(string s_OBUPlateColor, string s_OBUPlateNum, string s_OBUMac, string s_OBUY, string s_OBUBiao, string s_OBUCarLength, string s_OBUCarHigh, string s_OBUCarType, string s_TradeTime, string s_RandCode, string s_LaneNo)
        {
            string InsertString = @"Insert into OBUInfo(OBUPlateColor,OBUPlateNum,OBUMac,OBUY,OBUCarLength,OBUCarHigh,OBUCarType,TradeTime,RandCode,LaneNo) values('" + s_OBUPlateColor + "','" + s_OBUPlateNum + "','" + s_OBUMac + "','" + s_OBUY + "','" + s_OBUCarLength + "','" + s_OBUCarHigh + "','" + s_OBUCarType + "','" + s_TradeTime + "','" + s_RandCode + "','" + s_LaneNo + "')";
            
            try
            {
                if (MySqlconnection.State != System.Data.ConnectionState.Open)
                {
                    MF.AddOperLogCacheStr("天线数据插入失败");
                    return false;
                }
                MySqlCommand cmd = new MySqlCommand(InsertString, MySqlconnection);
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
        //检索黑白名单
        //0-为普通车辆 1为白名单车辆 -1为一次黑名单（可能因为误操作引起） -2为永久黑名单 2为不在黑白名单队列 -3异常
        public bool FindBlackOrWhiteCar(string CarNumber, ref int ShutType)
        {
            string SelectString = "select * from ShutTable where OBUPlateNum=" + "'" + CarNumber + "'";
            try
            {
                if (MySqlconnection.State != System.Data.ConnectionState.Open)
                {

                    MF.AddOperLogCacheStr("天线数据插入失败");
                    return false;
                }
                MySqlCommand cmd = new MySqlCommand(SelectString, MySqlconnection);
                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    if (reader.HasRows)
                    {
                        ShutType |= reader.GetInt32(10);
                    }
                    else
                        ShutType = 2;
                }
                if (!reader.HasRows)
                    ShutType = 2;
                reader.Dispose();
            }
            catch (Exception ex)
            {
                MF.AddOperLogCacheStr("黑白名单查询失败" + ex.ToString());
                Log.WriteLog(DateTime.Now + " 查询黑白名单出现异常\r\n" + ex.ToString() + "\r\n");
                ShutType = -3;
                //MessageBox.Show(ex.ToString());
                return false;
            }
            if (ShutType == -1 || ShutType == -2)
            {
                return true;
            }
            return false;
        }
        //更新黑白名单
        public int UpdateBlackOrWhiteCar(string OBUCarnumber, int BlackOrWhite)
        {
            try
            {
                if (MySqlconnection.State == ConnectionState.Closed)
                {
                    MySqlconnection.Open();
                }
                string UpdateString = "update ShutTable set CarFlag=" + "'" + BlackOrWhite + "'" + "where OBUPlateNum=" + "'" + OBUCarnumber + "'";
                MySqlCommand mySqlCommand = new MySqlCommand(UpdateString, MySqlconnection);
                mySqlCommand.ExecuteNonQuery();

                return 0;
            }
            catch (Exception ex)
            {
                Log.WriteLog(DateTime.Now + "\r\n" + "更新黑白名单\r\n" + ex.ToString() + "\r\n");
                MessageBox.Show("更新失败" + ex.ToString());
                MySqlconnection.Close();
                return -1;
            }
        }
        //数据更新通用函数
        public bool UpdateSQLData(string SQLString)
        {
            try
            {
                if (MySqlconnection.State != System.Data.ConnectionState.Open)
                {
                    MF.AddOperLogCacheStr("数据更新失败");
                    return false;
                }
                MySqlCommand cmd = new MySqlCommand(SQLString, MySqlconnection);
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
      

        /// <summary>
        /// 建立mysql数据库链接
        /// </summary>
        /// <returns></returns>
        public MySqlConnection getMySqlCon(String Database, String DataSource, String UserId, String Password, String port)
        {
            String mysqlStr = "Database=" + Database + ";DataSource=" + DataSource + ";UserId=" + UserId + ";Password=" + Password + ";pooling=false;CharSet=utf8;port=" + port;
            // String mySqlCon = ConfigurationManager.ConnectionStrings["MySqlCon"].ConnectionString;
            MySqlConnection mysql = new MySqlConnection(mysqlStr);
            return mysql;
        }

       

       

       
    }
}
