using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;

namespace JSONParserIntoDB
{
    class ReadJson
    {
        private const string BinData = @"" + "\"" + "binary_data" + "\":" + "\".*" + "\",";
        private const string Io = @"(^(Input|Output)( = .*))";
        private const string RegId = @"""message_id"":null";
        private static readonly Dictionary<string, string> Mss = new Dictionary<string, string>();
        public static int CountRows;
        private static string _connectionString;


        private string path { get; set; }
        public static Form1 Component { get; set; }
        
        public ReadJson(string path, Form1 frm, string conn)
        {
            this.path = path;
            Component = frm;
            _connectionString = conn;
            OpenDocument(path);
        }

        public static void OpenDocument(string path)
        {
            CountRows = 0;
            var fl2 = Directory.GetFiles(path).Where(d => d.Contains("Server"));
            if (fl2.Any())
            {
                Component.textBox1.Text = "";
                Component.label4Status.Text = "";
                Component.label4WithoutPair.Text = "0";
                Component.label5CountInserted.Text = "0";
                if (MessageBox.Show("There are " + fl2.Count() + " elements. \r\nDo you want to parse this files?", "Information!", MessageBoxButtons.OKCancel, MessageBoxIcon.Information) == DialogResult.OK)
                {
                    var tb = new DataTable();
                    tb.Columns.Add(new DataColumn("RequestDate", typeof(DateTime)));
                    tb.Columns.Add(new DataColumn("Request", typeof(string)));
                    tb.Columns.Add(new DataColumn("Response", typeof(string)));
                    tb.Columns.Add(new DataColumn("ResponseDate", typeof(DateTime)));
                    Component.textBox1.AppendText("Reading files...");
                    foreach (var s in fl2.Where(s => !s.Contains("OK_")))
                    {
                        if (!ReadDocument(tb, s))
                        {
                            Component.textBox1.Text = "";
                            return;
                        }
                    }
                    if (Mss.Count > 0)
                    {
                        JsWhitOutPair(tb, Mss);
                        CountRows += Mss.Count;
                        Component.label4WithoutPair.Text = Mss.Count.ToString();
                    }
                    Component.label5CountInserted.Text = CountRows.ToString();
                    Component.label4Status.Text = @"Reading finished successfully!";
                }
            }
            else
            {
                Component.textBox1.Text = "";
                Component.label4Status.Text = "";
                Component.label4WithoutPair.Text = "0";
                Component.label5CountInserted.Text = "0";
                MessageBox.Show(@"There is nothing!");
            }
        }


        public static bool ReadDocument(DataTable tb, string path)
        {
            var newPath = ChangeNameFile(path);//set new name
            var st = new StringBuilder();
            const string regDate = @"(<=.*|=>.*)";
            const string regDt = @"([0-3]?[0-9]/[0-3]?[0-9]/(?:[0-9]{2})?[0-9]{2})\s(1[0-2]|[0-9]):(00|0[1-9]{1}|[1-5]{1}[0-9]):(00|0[1-9]{1}|[1-5]{1}[0-9])\s(PM|AM)";
            try
            {
                var i = 0;//count rows
                using (var read = new StreamReader(newPath))
                {
                    string input;//value in row
                    while (!String.IsNullOrEmpty(input = read.ReadLine()))
                    {
                        var rr = Regex.Match(input, regDate, RegexOptions.IgnoreCase);//row with date
                        if (rr.Success)
                        {
                            var time = Regex.Match(input, regDt, RegexOptions.IgnoreCase); // datetime
                            if (time.Success)
                            {
                                st.Clear();//clean stringbuilder 
                                st.Append(time.Value.Trim()); //add datetime into stringbuilder
                            }
                        }
                        var ss = Regex.Match(input, RegId, RegexOptions.IgnoreCase); //row where messageId = null
                        if (ss.Success)
                        {
                            var rezIo1 = Regex.Match(input, Io, RegexOptions.IgnoreCase); //add row input/output with messageId null
                            if (rezIo1.Success)
                            {
                                st.Append(" " + rezIo1);
                                CutInputOutput(tb, rezIo1.Value, st);
                                st.Clear();
                                i++;
                            }
                        }
                        else //where messageId is not null
                        {
                            var rezIo2 = Regex.Match(input, Io, RegexOptions.IgnoreCase);
                            if (rezIo2.Success)
                            {
                                var json = rezIo2.Value.Replace("'", "''");//Change sign on " for database understand
                                var bin = Regex.Match(json, BinData, RegexOptions.IgnoreCase);//get BinariData
                                var rez = bin.Success ? json.Remove(bin.Index, bin.Length) : json; //remove BinariData or no if isn't

                                st.Append(" " + rez.Trim());//add to stringbuilder where is datetime.

                                if (rez.Remove(0, 8) != "")// input/output is not empty
                                {
                                    dynamic msId = JObject.Parse(rez.Remove(0, 8));//parse JSON
                                    var id = msId.message_id.ToString();//get messageId
                                    if (!Mss.ContainsKey(id))
                                    {
                                        Mss.Add(id, st.ToString());//add to array
                                        st.Clear();
                                    }
                                    else
                                    {
                                        var req = Mss.Values.Last();//get last value from array 
                                        var res = st.ToString();//get current value
                                        var reqDate = req.Substring(0, req.IndexOf("M", StringComparison.Ordinal) + 1); //request datetime
                                        var resDate = res.Substring(0, res.IndexOf("M", StringComparison.Ordinal) + 1); //response datetime
                                        var ind = reqDate.Length + 9;//length date and Input/Output

                                        AddRows(tb, Convert.ToDateTime(reqDate), req.Remove(0, ind), res.Remove(0, ind), Convert.ToDateTime(resDate));// add to virtual table

                                        Mss.Remove(id);// delete from array
                                        st.Clear();
                                        i++;
                                    }
                                }
                                else
                                {
                                    CutInputOutput(tb, rez, st);//cut and add row with datas
                                    st.Clear(); //clean stringbuilder
                                    i++;
                                }
                            }
                        }
                    }
                    CountRows += i; //Count row inserted
                    read.Close();
                }
                if (!BulkInsert(tb))// add to database
                {
                    return false;
                }
                File.Delete(newPath);//delete processed files
                Component.textBox1.AppendText("\r\nDocument " + path + "completed successful! \r\nCount rows: "+i);
            }
            catch (Exception ex)
            {
                MessageBox.Show(@"Document " + newPath + @"completed failed! DetailMessage:" + ex.Message,@"ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return true;
        }


        //Cut Input/Output and add row to table without request or repsonse
        public static void CutInputOutput(DataTable tb, string value, StringBuilder st)
        {
            switch (value.Substring(0, 6).Trim())
            {
                case "Input":
                    var query1 = st.ToString();
                    AddRows(tb, Convert.ToDateTime(query1.Substring(0, query1.IndexOf("M", StringComparison.Ordinal) + 1)), string.Empty, "Not found response", default(DateTime));
                    break;
                case "Output":
                    var query2 = st.ToString();
                    AddRows(tb, default(DateTime), "Not found request", string.Empty, Convert.ToDateTime(query2.Substring(0, query2.IndexOf("M", StringComparison.Ordinal) + 1)));
                    break;
            }
        }

        //Treatment array where remaining documents without pair 
        public static void JsWhitOutPair(DataTable tb, Dictionary<string, string> doc)
        {
            foreach (var d in doc)
            {
                var query = d.Value;
                var date = query.Substring(0, query.IndexOf("M", StringComparison.Ordinal) + 1);
                var nquery = query.Remove(0, date.Length + 1);

                switch (nquery.Substring(0, 6).Trim())
                {
                    case "Input":
                        var query1 = d.Value;
                        AddRows(tb, Convert.ToDateTime(query1.Substring(0, query1.IndexOf("M", StringComparison.Ordinal) + 1)), nquery.Remove(0, 8), "Not found response", default(DateTime));
                        break;
                    case "Output":
                        var query2 = d.Value;
                        AddRows(tb, default(DateTime), "Not found request", nquery.Remove(0, 8), Convert.ToDateTime(query2.Substring(0, query2.IndexOf("M", StringComparison.Ordinal) + 1)));
                        break;
                }
            }
            BulkInsert(tb);
        }

        //Build virtual table
        public static void AddRows(DataTable tb, DateTime dtRequest, string request, string response, DateTime dtResponse)
        {
            var odata = tb.NewRow();
            odata["RequestDate"] = dtRequest;
            odata["Request"] = request;
            odata["Response"] = response;
            odata["ResponseDate"] = dtResponse;
            tb.Rows.Add(odata);
        }

        //Insert into table in database
        public static bool BulkInsert(DataTable tb)
        {
            try
            {
                using (var bulk = new SqlBulkCopy(_connectionString ?? ConfigurationManager.ConnectionStrings["LogsDB"].ConnectionString))
                {
                    bulk.BatchSize = 10000;
                    bulk.BulkCopyTimeout = 10000;
                    bulk.ColumnMappings.Add("RequestDate", "RequestDate");
                    bulk.ColumnMappings.Add("Request", "Request");
                    bulk.ColumnMappings.Add("Response", "Response");
                    bulk.ColumnMappings.Add("ResponseDate", "ResponseDate");
                    bulk.DestinationTableName = "Logs";
                    bulk.WriteToServer(tb);
                }
                tb.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, @"ERROR!!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                if (
                    MessageBox.Show("Do you want to create \"Logs\" table in your DB?", null, MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    CreateTable();
                }
                else
                {
                    return false;
                }
            }
            return true;
        }

        //Rename document name
        public static string ChangeNameFile(string path)
        {
            var newPath = Path.GetDirectoryName(path) + "\\OK_" + Path.GetFileName(path);
            File.Move(path, newPath);
            return newPath;
        }


        public static void CreateTable()
        {
            try
            {
                SqlConnection conn = new SqlConnection(_connectionString ?? ConfigurationManager.ConnectionStrings["LogsDB"].ConnectionString);
                SqlCommand cmd = new SqlCommand("CREATE TABLE [dbo].[Logs] ("
                                                    + "[Id]           INT           IDENTITY(1, 1) NOT NULL,"
                                                    + "[RequestDate]  DATETIME2(7) NULL,"
                                                    + "[Request]      VARCHAR(MAX) NULL,"
                                                    + "[Response]     VARCHAR(MAX) NULL,"
                                                    + "[ResponseDate] DATETIME2(7) NULL,"
                                                    + "CONSTRAINT[PK_Logs2] PRIMARY KEY CLUSTERED([Id] ASC))"
                                                , conn);
                cmd.Connection.Open();
                cmd.ExecuteNonQuery();
                cmd.Connection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

    }
}
