using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using PublicNoticeScraper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PCKLIB
{
    public class GSHelper
    {
        private string[] scopes = { SheetsService.Scope.Spreadsheets };
        private string sheet_id;
        public SheetsService service;
        public GoogleCredential credential;
        public Spreadsheet book;

        public bool Authenticate(string auth_filename)
        {
            try
            {
                var credPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                credPath = Path.Combine(credPath, ".credentials/googleapis.com.json");

                credential =
                    Google.Apis.Auth.OAuth2.GoogleCredential.FromFile(auth_filename).CreateScoped(scopes);
                //Debug.WriteLine("Credential file saved to: " + credPath);

                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Google API authentication failed. {ex.Message}");
            }
            return false;
        }

        public async Task<bool> CreateTab(string name)
        {
            try
            {
                var addSheetRequest = new AddSheetRequest();
                addSheetRequest.Properties = new SheetProperties();
                addSheetRequest.Properties.Title = name;
                BatchUpdateSpreadsheetRequest batchUpdateSpreadsheetRequest = new BatchUpdateSpreadsheetRequest();
                batchUpdateSpreadsheetRequest.Requests = new List<Request>();
                batchUpdateSpreadsheetRequest.Requests.Add(new Request
                {
                    AddSheet = addSheetRequest
                });

                var batchUpdateRequest =
                    service.Spreadsheets.BatchUpdate(batchUpdateSpreadsheetRequest, this.sheet_id);

                batchUpdateRequest.Execute();
                return true;
            }   
            catch(Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            return false;
        }

        public async Task<bool> TabExist(string name)
        {
            var cnt = book.Sheets.Where(x => x.Properties.Title == name).Count();
            return cnt > 0;
        }

        public async Task<bool> ConnectSheet(string appname, string _sheet_id)
        {
            try
            {
                service = new SheetsService(
                        new BaseClientService.Initializer
                        {
                            HttpClientInitializer = credential,
                            ApplicationName = appname
                        });
                sheet_id = _sheet_id;
                                
                var ssRequest = service.Spreadsheets.Get(sheet_id);
                book = await ssRequest.ExecuteAsync();
                int cnt = book.Sheets.Count;

                App.log_error($"Sheet connection success. There are {cnt} sheets.");
                return true;
            }
            catch (Exception ex)
            {
                App.log_error($"Can't connect to the sheet. {ex.Message}");
            }
            return false;
        }

        // *****************************
        // Utility functions 
        // *****************************
        public static string Coord2Cell(int row, int column)
        {
            var columnSheet = new StringBuilder();

            while (column > 0)
            {
                var cm = column % 26;

                if (cm == 0)
                {
                    column--;
                    columnSheet.Insert(0, 'Z');
                }
                else
                {
                    columnSheet.Insert(0, (char)(cm + 'A' - 1));
                }

                column /= 26;
            }

            return $"{columnSheet}{row}";
        }

        public static Coord Cell2Coord(string sheetCellNumber)
        {
            var i = 0;

            for (; i < sheetCellNumber.Length; i++)
            {
                if (char.IsDigit(sheetCellNumber[i]))
                {
                    break;
                }
            }

            var column = 0;
            var row = sheetCellNumber.Substring(i);

            var cs = sheetCellNumber.Substring(0, i);
            var csl = cs.Length - 1;

            for (var j = 0; j < cs.Length; j++)
            {
                var cc = cs[j] - 'A' + 1;

                var multi = (int)Math.Pow(26, csl--);

                column += cc * multi;
            }

            return new Coord { row = int.Parse(row), column = column };
        }

        public async Task<ValueRange> GetAllData(Sheet sheet)
        {
            int? r_cnt = sheet.Properties.GridProperties.RowCount;
            int? c_cnt = sheet.Properties.GridProperties.ColumnCount;

            var req = service.Spreadsheets.Values.BatchGet(sheet_id);
            string startcell = GSHelper.Coord2Cell(1, 1);
            string endcell = GSHelper.Coord2Cell(r_cnt.Value, c_cnt.Value);
            req.Ranges = $"{sheet.Properties.Title}!{startcell}:{endcell}";
            var resp = await req.ExecuteAsync();
            var range = resp.ValueRanges[0];
            return range;
        }
        public async Task<IList<IList<object>>> ReadDataAsync(string sheetname, string cell)
        {
            try
            {
                var request = service.Spreadsheets.Values.Get(
                    sheet_id,
                    sheetname + "!" + cell);

                var response = await request.ExecuteAsync();
                return response.Values;
            }
            catch (Exception)
            {
                throw new Exception("Error in reading data");
            }
        }
        public async Task<UpdateValuesResponse> WriteToSheetAsync(string sheetname, string cell, Object valueToWrite)
        {
            try
            {
                var range = sheetname + "!" + cell; // "Basic!B111";
                var valueRange = new ValueRange { MajorDimension = "ROWS" };

                var objectList = new List<object> { valueToWrite };
                valueRange.Values = new List<IList<object>> { objectList };

                var update = service.Spreadsheets.Values.Update(valueRange, sheet_id, range);
                update.ValueInputOption =
                    SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.USERENTERED;

                return await update.ExecuteAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error in writing data");
            }
        }

        public async Task<UpdateValuesResponse> WriteMultipleToSheetAsync(string sheetname, string cell, List<List<object>> valueToWrite)
        {
            try
            {
                var range = sheetname + "!" + cell; // "Basic!B111";
                var valueRange = new ValueRange { MajorDimension = "ROWS" };

                valueRange.Values = new List<IList<object>>();
                foreach (var sublist in valueToWrite)
                    valueRange.Values.Add(sublist);

                var update = service.Spreadsheets.Values.Update(valueRange, sheet_id, range);
                update.ValueInputOption =
                    SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.USERENTERED;

                return await update.ExecuteAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error in writing data");
            }
        }

        public async Task<bool> InsertRows(string sheet_name, int start_index, int count)
        {
            try
            {
                // get sheet id from name
                var ids = book.Sheets.Where(x => x.Properties.Title == sheet_name).Select(x => x.Properties.SheetId).ToList();
                if (ids.Count < 0 || !ids[0].HasValue)
                    return false;
                int sheet_id = ids[0].Value;
                InsertDimensionRequest insertRow = new InsertDimensionRequest();
                insertRow.Range = new DimensionRange()
                {
                    SheetId = sheet_id,
                    Dimension = "ROWS",
                    StartIndex = start_index,
                    EndIndex = start_index + count
                };

                BatchUpdateSpreadsheetRequest r = new BatchUpdateSpreadsheetRequest()
                {
                    Requests = new List<Request>
                    {
                        new Request{ InsertDimension = insertRow }
                    }
                };

                BatchUpdateSpreadsheetResponse response = service.Spreadsheets.BatchUpdate(r, this.sheet_id).Execute();
                return true;
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            return false;
        }

        public async Task<UpdateValuesResponse> WriteDatatableToSheetAsync(string sheetname, string startcell, DataTable dt, bool include_header = true)
        {
            try
            {
                // parse start cell
                int start_col = ExcelColHelper.ColToIndex(Regex.Match(startcell, @"[a-zA-Z]+").Value);
                int start_row = int.Parse(Regex.Match(startcell, @"[0-9]+").Value);

                // calculate the cell range 
                int row_cnt = dt.Rows.Count;
                if(include_header)
                    row_cnt ++;
                int col_cnt = dt.Columns.Count;

                int end_col = start_col + col_cnt - 1;
                int end_row = start_row + row_cnt - 1;
                string endcell = ExcelColHelper.IndexToCol(end_col) + end_row.ToString();

                var range = sheetname + "!" + startcell + ":" + endcell; // "Basic!B11:F22";

                var values = dt.AsEnumerable().Select(row => row.ItemArray.ToList()).ToList();
                if(include_header)
                {
                    var columnNames = (from dc in dt.Columns.Cast<DataColumn>() select (object)dc.ColumnName).ToList();
                    values.Insert(0, columnNames);
                }

                // convert values to IList<IList<object>>
                var valueRange = new ValueRange { MajorDimension = "ROWS" };
                valueRange.Values = new List<IList<object>>();
                foreach (var sublist in values)
                    valueRange.Values.Add(sublist);

                var update = service.Spreadsheets.Values.Update(valueRange, sheet_id, range);
                update.ValueInputOption =
                    SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.USERENTERED;

                return await update.ExecuteAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error in writing data" + "\n" + ex.Message);
            }
        }

        public async Task<AppendValuesResponse> AppendToSheetAsync(string sheetname, string cell, Object valueToWrite)
        {
            try
            {
                var range = sheetname + "!" + cell; // "Basic!B111";
                var valueRange = new ValueRange { MajorDimension = "COLUMNS" };

                var objectList = new List<object> { valueToWrite };
                valueRange.Values = new List<IList<object>> { objectList };

                var append = service.Spreadsheets.Values.Append(valueRange, sheet_id, range);
                append.ValueInputOption =
                    SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.USERENTERED;

                return await append.ExecuteAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error in writing data");
            }
        }

        public class Coord
        {
            public int column;
            public int row;
        }
    }
}