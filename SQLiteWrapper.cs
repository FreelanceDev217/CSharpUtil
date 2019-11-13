// Wraper for SQLite
// David Piao
using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Data;
using System.Collections;
using System.Windows.Forms;
using System.Diagnostics;
using System.Data.SQLite;

namespace DBConnector
{
    public class SQLiteWrapper
    {
        public SQLiteConnection     sql_con;
        public SQLiteCommand        sql_cmd;
        private System.Object locker = new System.Object();

        public SQLiteWrapper() { }
        public void CreateFile(string file_name)
        {
            SQLiteConnection.CreateFile(file_name);
        }
        public void CreatConnection(string file_name)
        {
            try
            {
                string conn = "Data Source=" + file_name + ";Version=3;New=True;Compress=True;";
                sql_con = new SQLiteConnection(conn);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to create SQLite connection. {ex.Message}");
            }
        }

        public void Open()
        {
            try
            {
                sql_con.Open();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to open SQLite connection.  {ex.Message}");
            }
        }

        public void Close()
        {
            try
            {
                sql_con.Close();
                sql_con.Dispose();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to close SQLite connection.  {ex.Message}");
            }
        }

        public ArrayList GetTables()
        {
            try
            {
                string query = "SELECT name FROM sqlite_master " +
                                        "WHERE type IN ('table','view') AND name NOT LIKE 'sqlite_%'" +
                                        "UNION ALL " +
                                        "SELECT name FROM sqlite_temp_master " +
                                        "WHERE type IN ('table','view') " +
                                        "ORDER BY 1";
                DataTable table = ExecuteQuery(query);

                ArrayList list = new ArrayList();
                foreach (DataRow row in table.Rows)
                {
                    list.Add(row.ItemArray[0].ToString());
                }
                return list;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to get tables from SQLite connection.  {ex.Message}");
            }
            return null;
        }

        public void ExecuteNonQuery(string txtQuery)
        {
            try
            {
                lock (locker)
                {
                    sql_cmd = sql_con.CreateCommand();
                    sql_cmd.CommandText = txtQuery;
                    sql_cmd.ExecuteNonQuery();
                    sql_cmd.Dispose();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to execute SQLite query.\n{txtQuery}\n {ex.Message}");
            }
        }

        public DataTable ExecuteQuery(string txtQuery)
        {
            try
            {
                lock (locker)
                {
                    DataTable dt = new DataTable();
                    sql_cmd = sql_con.CreateCommand();
                    SQLiteDataAdapter DB = new SQLiteDataAdapter(txtQuery, sql_con);
                    DB.SelectCommand.CommandType = CommandType.Text;
                    DB.Fill(dt);
                    sql_cmd.Dispose();
                    System.Diagnostics.Debug.WriteLine("Execute SQLite Query: " + txtQuery + " -> " + dt.Rows.Count.ToString());
                    return dt;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to execute SQLite query.\n{txtQuery}\n {ex.Message}");
            }
            return null;
        }

        public DataTable GetDataTable(string tablename)
        {
            try
            {
                DataTable DT = new DataTable();
                sql_cmd = sql_con.CreateCommand();
                sql_cmd.CommandText = string.Format("SELECT * FROM {0}", tablename);
                var adapter = new SQLiteDataAdapter(sql_cmd);
                adapter.AcceptChangesDuringFill = false;
                adapter.Fill(DT);

                DT.TableName = tablename;
                foreach (DataRow row in DT.Rows)
                {
                    row.AcceptChanges();
                }
                return DT;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to execute SQLite query.\n {ex.Message}");
            }
            return null;
        }
        public void SaveDataTable(DataTable DT, string tbl_name = "")
        {
            try
            {
                sql_cmd = sql_con.CreateCommand();
                if (tbl_name == "")
                    tbl_name = DT.TableName;
                if (tbl_name == "")
                    return;

                sql_cmd.CommandText = string.Format("SELECT * FROM {0}", tbl_name);
                var adapter = new SQLiteDataAdapter(sql_cmd);
                adapter.AcceptChangesDuringFill = true;
                SQLiteCommandBuilder builder = new SQLiteCommandBuilder(adapter);
                adapter.Update(DT);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to execute SQLite query.{ex.Message}");
            }
        }
    }
}