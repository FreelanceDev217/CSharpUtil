// Read/Write to xls file 
// David Piao
using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeskBillCompiler
{
    class CXMLUtil
    {
        public static DataTable ImportExceltoDatatable(string filePath, string sheetName)
        {
            // Open the Excel file using ClosedXML.
            // Keep in mind the Excel file cannot be open when trying to read it
            using (XLWorkbook workBook = new XLWorkbook(filePath))
            {
                //Read the first Sheet from Excel file.
                IXLWorksheet workSheet = workBook.Worksheet(1);

                //Create a new DataTable.
                DataTable dt = new DataTable();

                //Loop through the Worksheet rows.
                bool firstRow = true;
                foreach (IXLRow row in workSheet.Rows())
                {
                    //Use the first row to add columns to DataTable.
                    if (firstRow)
                    {
                        foreach (IXLCell cell in row.Cells())
                        {
                            dt.Columns.Add(cell.Value.ToString());
                        }
                        firstRow = false;
                    }
                    else
                    {
                        //Add rows to DataTable.
                        dt.Rows.Add();
                        int i = 0;

                        foreach (IXLCell cell in row.Cells(row.FirstCellUsed().Address.ColumnNumber, row.LastCellUsed().Address.ColumnNumber))
                        {
                            dt.Rows[dt.Rows.Count - 1][i] = cell.Value.ToString();
                            i++;
                        }
                    }
                }

                return dt;
            }
        }

        public static DataSet ImportExceltoDataSet(string filePath, bool first_row_header = true)
        {
            DataSet ds = new DataSet();
            // Open the Excel file using ClosedXML.
            // Keep in mind the Excel file cannot be open when trying to read it
            using (XLWorkbook workBook = new XLWorkbook(filePath))
            {
                for(int idx = 1; idx <= workBook.Worksheets.Count; idx ++)
                {
                    //Read the first Sheet from Excel file.
                    IXLWorksheet workSheet = workBook.Worksheet(idx);

                    //Create a new DataTable.
                    DataTable dt = new DataTable();

                    if (first_row_header == false)
                    {
                        for (int col = 1; col < workSheet.Columns().Count(); col++)
                            dt.Columns.Add($"Column {col}");
                    }

                    //Loop through the Worksheet rows.
                    bool firstRow = true;
                    foreach (IXLRow row in workSheet.Rows())
                    {
                        //Use the first row to add columns to DataTable.
                        if (firstRow && first_row_header)
                        {
                            foreach (IXLCell cell in row.Cells())
                            {
                                dt.Columns.Add(cell.Value.ToString());
                            }
                            firstRow = false;
                        }
                        else
                        {
                            //Add rows to DataTable.
                            dt.Rows.Add();
                            int i = 0;

                            if(row.FirstCellUsed() != null &&
                                row.LastCellUsed() != null)
                            {
                                foreach (IXLCell cell in row.Cells(row.FirstCellUsed().Address.ColumnNumber, row.LastCellUsed().Address.ColumnNumber))
                                {
                                    dt.Rows[dt.Rows.Count - 1][i] = cell.Value.ToString();
                                    i++;
                                }
                            }
                        }
                    }

                    ds.Tables.Add(dt);
                }
            }

            return ds;
        }
    }
}
