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
       
        public void ExecuteNonQuery(string db_file, string txtQuery)
        {
            string db_conn = $"Data Source={db_file};Version=3;New=True;Compress=True;";
            try
            {
                using (var sql_con = new SQLiteConnection(db_file))
                {
                    sql_con.Open();
                    using (var cmd = new SQLiteCommand(sql_con))
                    {
                        using (var transaction = sql_con.BeginTransaction())
                        {
                            cmd.CommandText = txtQuery;
                            cmd.ExecuteNonQuery();
                            transaction.Commit();
                        }
                    }
                    sql_con.Close();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to execute SQLite query.\n{txtQuery}\n {ex.Message}");
            }
        }

        public DataTable ExecuteQuery(string db_file, string txtQuery)
        {
            try
            {
                string db_conn = $"Data Source={db_file};Version=3;New=True;Compress=True;";
                DataTable dt = new DataTable();
                using (var sql_con = new SQLiteConnection(db_conn))
                {
                    sql_con.Open();
                    using (var cmd = new SQLiteCommand(sql_con))
                    {
                        SQLiteDataAdapter DB = new SQLiteDataAdapter(txtQuery, sql_con);
                        DB.SelectCommand.CommandType = CommandType.Text;
                        DB.Fill(dt);
                    }
                    sql_con.Close();
                }

                System.Diagnostics.Debug.WriteLine("Execute SQLite Query: " + txtQuery + " -> " + dt.Rows.Count.ToString());
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