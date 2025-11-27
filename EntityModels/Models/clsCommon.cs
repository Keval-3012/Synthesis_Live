using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;

namespace SynthesisCF.Models
{
    public class clsCommon
    {
        public static int ActivityLogInsert(ActivityLog obj)
        {

            try
            {
                int UserId= UserModule.getUserId();
                obj.UserId = UserId;
                obj.CreatedBy = UserId;
                obj.CreatedOn = DateTime.Now;
                DBContext db = new DBContext();
                db.ActivityLogs.Add(obj);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
            return obj.ActivityLogId;
        }

        public static void WriteErrorLogs(string Message)
        {
            string Date = DateTime.Today.Year + "-" + DateTime.Today.Month + "-" + DateTime.Today.Day;
            string logPath = "~/UserFiles/LogFile/";
            if (!System.IO.Directory.Exists(HttpContext.Current.Server.MapPath(logPath)))
            {
                System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath(logPath));
            }
            StreamWriter sw = File.AppendText(HttpContext.Current.Server.MapPath(logPath) + Date + "-Log.txt");
            sw.WriteLine(DateTime.Now + "  " + Message);
            sw.Close();
            sw.Dispose();
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

        public static void InvoiceUserProductMapLogInsert(InvoiceUserProductMapLog obj)
        {
            try
            {
                int UserId = UserModule.getUserId();
                obj.CreatedBy = UserId;
                obj.CreatedDate = DateTime.Now;
                DBContext db = new DBContext();
                db.InvoiceUserProductMapLog.Add(obj);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
        }
        
        public static int InsertProduct(Products obj)
        {
            try
            {
                int UserId = UserModule.getUserId();
                obj.CreatedBy = UserId;
                obj.DateCreated = DateTime.Now;
                DBContext db = new DBContext();
                db.products.Add(obj);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
            return obj.ProductId;
        }
        
        public static int InsertProductVendors(ProductVendor obj)
        {
            try
            {
                int UserId = UserModule.getUserId();
                obj.CreatedBy = UserId;
                obj.DateCreated = DateTime.Now;
                DBContext db = new DBContext();
                db.productVendors.Add(obj);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
            return obj.ProductVendorId;
        }
    }
}