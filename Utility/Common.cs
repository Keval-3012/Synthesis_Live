using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Utility
{
    public static class Common
    {

        public static int UserID = 0;

        public static string ProjectDateFormat = ConfigurationManager.AppSettings["DateFormat"].ToString();

        public static DateTime GetEasternTime(DateTime dt)
        {
            var timeUtc = DateTime.UtcNow;
            TimeZoneInfo easternZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
            DateTime easternTime = TimeZoneInfo.ConvertTimeFromUtc(timeUtc, easternZone);
            return easternTime;
        }
        public static string GetDateformat(string date)
        {
            string retval = "";
            try
            {
                DateTime currDate = Convert.ToDateTime(date);
                DateTime octoberUtc = new DateTime(2012, 10, 1, 0, 0, 0, DateTimeKind.Utc);
                retval = string.Format(ProjectDateFormat, Convert.ToDateTime(currDate));
            }
            catch { }
            return retval;
        }
        public static string GetDate(string date)
        {
           string retval = "";
            try
            {
                retval = string.Format("{0:MMM dd, yyyy}", Convert.ToDateTime(date));
            }
            catch
            {
            }

            return retval;
        }
        public static void WriteErrorLogs(string Message)
        {
            try
            {
                string Date = DateTime.Today.Year + "-" + DateTime.Today.Month + "-" + DateTime.Today.Day;
                string logPath = "~/Log/";
                if (!System.IO.Directory.Exists(logPath))
                {
                    System.IO.Directory.CreateDirectory(logPath);
                }
                StreamWriter sw = File.AppendText(HttpContext.Current.Server.MapPath(logPath) + Date + "-Log.txt");
                sw.WriteLine(DateTime.Now + "  " + Message);
                sw.Close();
                sw.Dispose();
            }
            catch (Exception)
            {
            }
        }
        public static void WriteErrorLogsCheckInOut(string Message)
        {
            string Date = DateTime.Today.Year + "-" + DateTime.Today.Month + "-" + DateTime.Today.Day;
            string logPath = "~/UserFiles/CheckInOutFile/";
            if (!System.IO.Directory.Exists(HttpContext.Current.Server.MapPath(logPath)))
            {
                System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath(logPath));
            }
            StreamWriter sw = File.AppendText(HttpContext.Current.Server.MapPath(logPath) + Date + "-Log.txt");
            sw.WriteLine(DateTime.Now + "  " + Message);
            sw.Close();
            sw.Dispose();
        }
        public enum ManageMessageId
        {
            ChangePasswordSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
            ChangePasswordError,
        }
        public static DataTable LINQToDataTable<T>(IEnumerable<T> varlist)
        {
            DataTable dtReturn = new DataTable();

            // column names 
            PropertyInfo[] oProps = null;

            if (varlist == null)
            {
                return dtReturn;
            }

            foreach (T rec in varlist)
            {
                // Use reflection to get property names, to create table, Only first time, others 

                if (oProps == null)
                {
                    oProps = ((Type)rec.GetType()).GetProperties();
                    foreach (PropertyInfo pi in oProps)
                    {
                        Type colType = pi.PropertyType;

                        if ((colType.IsGenericType) && (colType.GetGenericTypeDefinition()
                        == typeof(Nullable<>)))
                        {
                            colType = colType.GetGenericArguments()[0];
                        }

                        dtReturn.Columns.Add(new DataColumn(pi.Name, colType));
                    }
                }

                DataRow dr = dtReturn.NewRow();

                foreach (PropertyInfo pi in oProps)
                {
                    dr[pi.Name] = pi.GetValue(rec, null) == null ? DBNull.Value : pi.GetValue
                    (rec, null);
                }

                dtReturn.Rows.Add(dr);
            }
            return dtReturn;
        }
        public static int GetRandomNo()
        {
            Random rn = new Random();
            int i = 0;
            i = rn.Next(1, 10000);
            return i;
        }
        public static byte[] GetFile(string s)
        {
            System.IO.FileStream fs = System.IO.File.OpenRead(s);
            byte[] data = new byte[fs.Length];
            int br = fs.Read(data, 0, data.Length);
            if (br != fs.Length)
            {
                throw new System.IO.IOException(s);
            }

            return data;
        }
        public static string GetDateFormat(DateTime dt, int duration)
        {
            if (duration == 0)
                return dt.Year.ToString() + "-" + (dt.Month.ToString().Length == 1 ? "0" + dt.Month.ToString() : dt.Month.ToString()) + "-" + (dt.Day.ToString().Length == 1 ? "0" + dt.Day.ToString() : dt.Day.ToString());
            else
            {
                dt = dt.AddMonths(duration);
                return dt.Year.ToString() + "-" + (dt.Month.ToString().Length == 1 ? "0" + dt.Month.ToString() : dt.Month.ToString()) + "-" + (dt.Day.ToString().Length == 1 ? "0" + dt.Day.ToString() : dt.Day.ToString());
            }
        }
        public static DateTime StartOfWeek(DateTime dt, DayOfWeek startOfWeek)
        {
            int diff = (7 + (dt.DayOfWeek - startOfWeek)) % 7;
            return dt.AddDays(-1 * diff).Date;
        }
        public static List<T> ConvertToList<T>(DataTable dt)
        {
            var columnNames = dt.Columns.Cast<DataColumn>().Select(c => c.ColumnName.ToLower()).ToList();
            var properties = typeof(T).GetProperties();
            return dt.AsEnumerable().Select(row =>
            {
                var objT = Activator.CreateInstance<T>();
                foreach (var pro in properties)
                {
                    if (columnNames.Contains(pro.Name.ToLower()))
                    {
                        try
                        {
                            pro.SetValue(objT, row[pro.Name]);
                        }
                        catch (Exception) { }
                    }
                }
                return objT;
            }).ToList();
        }
    }
}
