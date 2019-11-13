// XLS Helper - only for reading
// David Piao
using ExcelDataReader;
using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PCKLIB
{
    class XLSUtil
    {
        string[] column_names(DataTable dt)
        {
            string[] columnNames = (from dc in dt.Columns.Cast<DataColumn>() select dc.ColumnName).ToArray();
            return columnNames;
        }

        public static DataSet ReadDataExcel(string filepath, bool use_header_row = true)
        {
            try
            {
                FileStream stream = File.Open(filepath, FileMode.Open, FileAccess.Read);
                IExcelDataReader reader;

                string extension = System.IO.Path.GetExtension(filepath).ToLower();

                if (extension.Equals(".csv"))
                {
                    reader = ExcelReaderFactory.CreateCsvReader(stream);
                }
                else if (extension.Equals(".xls"))
                {
                    reader = ExcelReaderFactory.CreateBinaryReader(stream);
                }
                else
                {
                    reader = ExcelReaderFactory.CreateOpenXmlReader(stream);
                }

                var conf = new ExcelDataSetConfiguration
                {
                    ConfigureDataTable = _ => new ExcelDataTableConfiguration
                    {
                        UseHeaderRow = use_header_row,
                        FilterRow = rowReader =>
                        {
                            var hasData = false;
                            for (var i = 0; i < rowReader.FieldCount; i++)
                            {
                                if (rowReader[i] == null || string.IsNullOrEmpty(rowReader[i].ToString()))
                                {
                                    continue;
                                }

                                hasData = true;
                                break;
                            }

                            return hasData;
                        },
                        EmptyColumnNamePrefix = "Col "
                    }
                };

                conf.UseColumnDataType = false;
                var dataSet = reader.AsDataSet(conf);
                reader.Close();
                return dataSet;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
        }
    }
}
