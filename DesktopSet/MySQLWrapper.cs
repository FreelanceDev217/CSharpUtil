using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using MySql.Data.MySqlClient;
using System.Data;

namespace PCK_LIB
{
    public class MySQLWrapper
    {
        private System.Object locker = new System.Object();
        public MySqlConnection sql_con;
        public MySqlCommand sql_cmd;

        public string server;
        public string database;
        public string uid;
        public string password;
        public string port;

        public bool log = false;

        public MySQLWrapper()
        {
        }

        public void OutLog(string str)
        {
            if (log == false)
                return;
            using (StreamWriter writer = new StreamWriter("mysql_log.txt", true))
            {
                writer.WriteLine(str);
            }
        }
        public void ExecuteNonQuery(string txtQuery)
        {
            try
            {
                lock (locker)
                {
                    CloseConnection();
                    OutLog("\t\t#CENTRAL# Execute MySQL NonQuery: " + txtQuery);
                    OpenConnection();
                    sql_cmd = sql_con.CreateCommand();
                    sql_cmd.CommandText = txtQuery;
                    sql_cmd.ExecuteNonQuery();
                    CloseConnection();
                }
            }
            catch (Exception ex)
            {
                OutLog("Database query failed : " + ex.Message);
            }
        }

        public DataTable ExecuteQuery(string txtQuery)
        {
            try
            {
                lock (locker)
                {
                    CloseConnection();
                    OpenConnection();
                    DataTable dt = new DataTable();
                    sql_cmd = sql_con.CreateCommand();
                    MySqlDataAdapter DB = new MySqlDataAdapter(txtQuery, sql_con);
                    DB.SelectCommand.CommandType = CommandType.Text;
                    DB.Fill(dt);
                    CloseConnection();

                    OutLog("\t\t#CENTRAL# Execute MySQL Query: " + txtQuery + " -> " + dt.Rows.Count.ToString());
                    return dt;
                }
            }
            catch (Exception ex)
            {
                OutLog("Database query failed : " + ex.Message);
                return null;
            }
        }

        public void CreateConnection()
        {
            OutLog(">>Create MySQL connection");
            string connectionString;
            if(port == "")
                connectionString = String.Format("server = {0}; user id = {1}; password ={2}; persistsecurityinfo = True; database ={3}; SslMode = none; Convert Zero Datetime=True;",
                                                server, uid, password, database);
            else
                connectionString = String.Format("server = {0}; user id = {1}; password ={2}; persistsecurityinfo = True; port ={3}; database ={4}; SslMode = none; Convert Zero Datetime=True;",
                                                server, uid, password, port, database);
            sql_con = new MySqlConnection(connectionString);
            OutLog("<<Create MySQL connection");
        }

        public static int GetExceptionNumber(MySqlException my)
        {
            if (my != null)
            {
                int number = my.Number;
                //if the number is zero, try to get the number of the inner exception
                if (number == 0 && (my = my.InnerException as MySqlException) != null)
                {
                    number = my.Number;
                }
                return number;
            }
            return -1;
        }
        public bool OpenConnection()
        {
            try
            {
                sql_con.Open();
                return true;
            }
            catch (MySqlException ex)
            {
                OutLog(sql_con.ConnectionString);
                switch (GetExceptionNumber(ex))
                {
                    case 0:
                        OutLog("Cannot connect to server.  Contact administrator");
                        break;

                    case 1045:
                        OutLog("Invalid username/password, please try again");
                        break;
                }
                OutLog(ex.Message);
                return false;
            }
        }
        public bool CloseConnection()
        {
            try
            {
                sql_con.Close();
                return true;
            }
            catch (MySqlException ex)
            {
                OutLog(ex.Message);
                return false;
            }
        }

        public bool is_connected()
        {
            lock (locker)
            {
                CloseConnection();
                bool ret = OpenConnection();
                if (ret)
                    CloseConnection();
                return ret;
            }
        }

        public static string date4sql(DateTime date)
        {
            return date.ToString("yyyy-MM-dd HH:mm:ss");
        }
    }
}
