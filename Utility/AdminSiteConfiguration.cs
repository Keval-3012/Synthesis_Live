using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Security;


namespace Utility
{
    public static class AdminSiteConfiguration
    {

        #region Get URL Code Start
        public static string GetURL()
        {
            return HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority) + "/";

        }
        #endregion

        #region Viewing Record
        public static string GetArrayForRecord()
        {
            return "100:100,200:200,500:500";//,All:0";
        }
        #endregion

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

        public static string GetDateTime(DateTime date)
        {
            string retval = "";
            try
            {
                retval = string.Format("{0:MMM dd, yyyy}", date);
            }
            catch
            {
            }

            return retval;
        }

        public static DateTime GetEasternTime(DateTime dt)
        {
            var timeUtc = DateTime.UtcNow;
            TimeZoneInfo easternZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
            DateTime easternTime = TimeZoneInfo.ConvertTimeFromUtc(timeUtc, easternZone);
            return easternTime;
        }

        #region Get Remove Special Character Code Start
        public static string RemoveSpecialCharacter(String name)
        {
            name = name.Trim();

            StringBuilder sb = new StringBuilder();
            foreach (char c in name)
            {
                if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || c == '.' || c == '_')
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }

        public static string RemoveSpecialCharacterInvoice(String name)
        {
            name = name.Trim();

            StringBuilder sb = new StringBuilder();
            foreach (char c in name)
            {
                if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || c == '_')
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }
        #endregion

        #region Get Random No
        public static int GetRandomNo()
        {
            Random rn = new Random();
            int i = 0;
            i = rn.Next(1, 10000);
            return i;
        }
        public static int GetRandomNo(int noofcount)
        {
            Random rn = new Random();
            int i = 0;
            i = rn.Next(1, noofcount);
            return i;
        }
        #endregion
        public static string GetDatestatusmsg(string date)
        {
            string retval = "";
            try
            {
                retval = string.Format("{0:MMM dd, yyyy hh:mm tt}", Convert.ToDateTime(date));
            }
            catch
            {
            }

            return retval;
        }

        #region Get Date Code Start

        public static string GetDateformat(string date)
        {
            string retval = "";
            try
            {
                DateTime currDate = Convert.ToDateTime(date);
                //TimeSpan time = new TimeSpan(0, -1, -30, 0);
                //DateTime combined = currDate.Add(time);
                DateTime octoberUtc = new DateTime(2012, 10, 1, 0, 0, 0, DateTimeKind.Utc);
                retval = string.Format("{0:MM/dd/yyyy}", Convert.ToDateTime(currDate));
            }
            catch { }
            return retval;
        }
        public static string GetDatetimeformatwithAMPM(string date)
        {
            string retval = "";
            try
            {
                DateTime currDate = Convert.ToDateTime(date);
                //TimeSpan time = new TimeSpan(0, -1, -30, 0);
                //DateTime combined = currDate.Add(time);
                DateTime octoberUtc = new DateTime(2012, 10, 1, 0, 0, 0, DateTimeKind.Utc);
                retval = string.Format("{0:MM/dd/yyyy hh:mm:ss tt}", Convert.ToDateTime(currDate));
            }
            catch { }
            return retval;
        }
        public static string GetDate1(string date)
        {
            string retval = "";
            try
            {
                DateTime currDate = Convert.ToDateTime(date);
                DateTime octoberUtc = new DateTime(2012, 10, 1, 0, 0, 0, DateTimeKind.Utc);
                retval = string.Format("{0:MMM dd, yyyy}", Convert.ToDateTime(currDate));
            }
            catch
            {
            }

            return retval;
        }
        #endregion
        public enum QBConnectionType : int
        {
            Desktop = 1,
            Online = 2,
            None = 0
        }

        public enum HomeExpenseSettingType : int
        {
            Payment_Fees = 1,
            Delivery_Cost = 2,
            Tips_Paid = 3
        }
        public static string PriceWithComma(string number)
        {
            string returnval = "";
            try
            {
                returnval = Convert.ToDecimal(number).ToString("#,##0.00");
            }
            catch
            {
            }
            return returnval;
        }
        public static string RoundPriceWithComma(string number)
        {
            string returnval = "";
            try
            {
                returnval = Convert.ToInt32(number).ToString("#,##");
            }
            catch
            {
            }
            return returnval;
        }

        public static string GetMonthDateFormat(string date)
        {
            string retval = "";
            try
            {
                DateTime currDate = Convert.ToDateTime(date);
                //TimeSpan time = new TimeSpan(0, -1, -30, 0);
                //DateTime combined = currDate.Add(time);
                DateTime octoberUtc = new DateTime(2012, 10, 1, 0, 0, 0, DateTimeKind.Utc);
                retval = string.Format("{0:MMM - yyyy}", Convert.ToDateTime(currDate));
            }
            catch
            {
            }

            return retval;
        }

        public static string GetDateDisplay(string date)
        {
            string retval = "";
            try
            {
                //retval = string.Format("{0:dd MMM yyyy}", Convert.ToDateTime(date));
                retval = string.Format("{0:dd MMM yyyy}", Convert.ToDateTime(date));
            }
            catch
            {
            }

            return retval;
        }

        #region Viewsing 100 Record for report only
        public static string GetArrayForRecordReport()
        {
            return "100:100,200:200,500:500";//,All:0";
        }
        #endregion

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
            catch (Exception ex)
            {
            }
        }
        public enum DateFormats
        {
            ddMMyyyy = 0,
            MMddyyyy = 1,
            ddMMyyyyHHmmtt = 2,
            MMddyyyyHHmmtt = 3,

            ddMMyyyyhhmm = 4,
            MMddyyyyhhmm = 5,
            yyyyddMMhhmm = 6,
            yyyyMMddhhmm = 7
        }

        public static DateTime stringToDate(string date, DateFormats sourceFormat, DateFormats destinationFormat)
        {
            if (!String.IsNullOrEmpty(date))
            {
                if (destinationFormat == DateFormats.MMddyyyy)
                {
                    if (sourceFormat == DateFormats.ddMMyyyy)
                        return DateTime.ParseExact(DateTime.ParseExact(date.Trim(), "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("MM/dd/yyyy"), "MM/dd/yyyy", null);
                    else
                        return DateTime.ParseExact(date.Trim(), "MM/dd/yyyy", null);
                }
                else if (destinationFormat == DateFormats.ddMMyyyy)
                {
                    if (sourceFormat == DateFormats.ddMMyyyy)
                        return DateTime.ParseExact(date.Trim(), "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    else
                        return DateTime.ParseExact(DateTime.ParseExact(date.Trim(), "MM/dd/yyyy", null).ToString("dd/MM/yyyy"), "dd/MM/yyyy", CultureInfo.InvariantCulture);
                }
                else if (destinationFormat == DateFormats.MMddyyyyHHmmtt)
                {
                    if (sourceFormat == DateFormats.ddMMyyyyHHmmtt)
                        return DateTime.ParseExact(DateTime.ParseExact(date.Trim(), "dd/MM/yyyy hh:mm tt", CultureInfo.InvariantCulture).ToString("MM/dd/yyyy hh:mm tt"), "MM/dd/yyyy hh:mm tt", null);
                    else
                        return DateTime.ParseExact(date.Trim(), "MM/dd/yyyy hh:mm tt", null);
                }
                else if (destinationFormat == DateFormats.ddMMyyyyHHmmtt)
                {
                    if (sourceFormat == DateFormats.ddMMyyyyHHmmtt)
                        return DateTime.ParseExact(date.Trim(), "dd/MM/yyyy hh:mm tt", CultureInfo.InvariantCulture);
                    else
                        return DateTime.ParseExact(DateTime.ParseExact(date.Trim(), "MM/dd/yyyy hh:mm tt", null).ToString("dd/MM/yyyy hh:mm tt"), "dd/MM/yyyy hh:mm tt", CultureInfo.InvariantCulture);
                }
                else if (destinationFormat == DateFormats.MMddyyyyhhmm)
                {
                    if (sourceFormat == DateFormats.ddMMyyyyhhmm)
                        return DateTime.ParseExact(DateTime.ParseExact(date.Trim(), "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture).ToString("MM/dd/yyyy HH:mm"), "MM/dd/yyyy HH:mm", null);
                    else
                        return DateTime.ParseExact(date.Trim(), "MM/dd/yyyy HH:mm", null);
                }
                else if (destinationFormat == DateFormats.ddMMyyyyhhmm)
                {
                    if (sourceFormat == DateFormats.ddMMyyyyhhmm)
                        return DateTime.ParseExact(date.Trim(), "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture);
                    else
                        return DateTime.ParseExact(DateTime.ParseExact(date.Trim(), "MM/dd/yyyy HH:mm", null).ToString("dd/MM/yyyy HH:mm"), "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture);
                }
                else if (destinationFormat == DateFormats.yyyyddMMhhmm)
                {
                    if (sourceFormat == DateFormats.yyyyddMMhhmm)
                        return DateTime.ParseExact(DateTime.ParseExact(date.Trim(), "yyyy-dd-MM HH:mm", CultureInfo.InvariantCulture).ToString("yyyy-dd-MM HH:mm"), "yyyy-dd-MM HH:mm", null);
                    else
                        return DateTime.ParseExact(date.Trim(), "yyyy-dd-MM HH:mm", null);
                }
                else if (destinationFormat == DateFormats.yyyyMMddhhmm)
                {
                    if (sourceFormat == DateFormats.yyyyMMddhhmm)
                        return DateTime.ParseExact(date.Trim(), "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture);
                    else
                        return DateTime.ParseExact(DateTime.ParseExact(date.Trim(), "yyyy-MM-dd HH:mm", null).ToString("yyyy-MM-dd HH:mm"), "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture);
                }
                else
                {
                    DateTime temp = new DateTime();
                    return temp;
                }
            }
            else
            {
                DateTime temp = DateTime.Now;
                return temp;
            }
        }
        }
    }
