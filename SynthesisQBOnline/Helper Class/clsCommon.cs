using SynthesisQBOnline.QBClass;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;


namespace SynthesisQBOnline
{
    public class clsCommon
    {
        public static DataTable dtQBChild = new DataTable();
        public static void Clear_Intialize_Table(string Type)
        {
            dtQBChild = new DataTable();
            switch (Type)
            {
                case "INV":

                    //Child Table Column

                    dtQBChild = new DataTable();
                    dtQBChild.Columns.Add(new DataColumn("TxnID"));
                    dtQBChild.Columns.Add(new DataColumn("LineId"));
                    dtQBChild.Columns.Add(new DataColumn("LineNum"));
                    dtQBChild.Columns.Add(new DataColumn("Description"));
                    dtQBChild.Columns.Add(new DataColumn("Amount"));
                    dtQBChild.Columns.Add(new DataColumn("DetailType"));
                    dtQBChild.Columns.Add(new DataColumn("DeptName"));
                    dtQBChild.Columns.Add(new DataColumn("DeptID"));
                    dtQBChild.Columns.Add(new DataColumn("UnitPrice"));
                    dtQBChild.Columns.Add(new DataColumn("Qty"));
                    dtQBChild.Columns.Add(new DataColumn("TaxCodeRef"));

                    break;
            }
        }

        public static string RemoveSpecialCharacters(string str, string Type)
        {
            if (Type == "Address")
                str = str.Length > 1000 ? str.Substring(0, 1000) : str;
            else if (Type == "City" || Type == "Country" || Type == "ZipCode")
                str = str.Length > 31 ? str.Substring(0, 31) : str;
            else if (Type == "SKU" || Type == "Item" || Type == "PaymentMethod" || Type == "Country")
                str = str.Length > 31 ? str.Substring(0, 31) : str;
            else if (Type == "Phone" || Type == "Ship")
                str = str.Length > 15 ? str.Substring(0, 15) : str;
            else if (Type == "Customer")
                str = str.Length > 100 ? str.Substring(0, 100) : str;
            else if (Type == "DisplayName")
                str = str.Length > 500 ? str.Substring(0, 500) : str;
            else if (Type == "MiddleName" || Type == "SalesRep")
                str = str.Length > 5 ? str.Substring(0, 5) : str;
            else if (Type == "FirstName" || Type == "State")
                str = str.Length > 25 ? str.Substring(0, 25) : str;
            else if (Type == "Addr1")
                str = str.Length > 41 ? str.Substring(0, 41) : str;
            else if (Type == "LastName")
                str = str.Length > 25 ? str.Substring(0, 25) : str;
            else if (Type == "RefNumber")
                str = str.Length > 20 ? str.Substring(0, 20) : str;
            else if (Type == "Name")
                str = str.Length > 41 ? str.Substring(0, 41) : str;
            else if (Type == "Email")
                str = str.Length > 100 ? str.Substring(0, 100) : str;
            else if (Type == "Country")
                str = str.Length > 31 ? str.Substring(0, 31) : str;
            else if (Type == "State")
                str = str.Length > 21 ? str.Substring(0, 21) : str;
            else if (Type == "PostalCode")
                str = str.Length > 13 ? str.Substring(0, 13) : str;
            else if (Type == "Fax" || Type == "Shipp")
                str = str.Length > 15 ? str.Substring(0, 15) : str;
            else if (Type == "Message")
                str = str.Length > 100 ? str.Substring(0, 100) : str;
            else if (Type == "Memo" || Type == "Description")
                str = str.Length > 4095 ? str.Substring(0, 4095) : str;
            else if (Type == "Mobile")
                str = str.Length > 30 ? str.Substring(0, 30) : str;
            else if (Type == "Amount")
                str = str.Length > 11 ? str.Substring(0, 11) : str;

            str = str.Trim();
            StringBuilder sb = new StringBuilder();

            //if (Operation == "C")
            //{
            //    for (int i = 0; i <= str.Length - 1; i++)
            //    {
            //        if ((str[i] >= '0' && str[i] <= '9') || (str[i] >= 'A' && str[i] <= 'z') || (str[i] >= 'a' && str[i] <= 'z') || str[i] == ' ' || str[i] == '_' || str[i] == '-' || str[i] == '@' || str[i] == '/' || str[i] == '·' || str[i] == ',' || str[i] == '.' || str[i] == '"' || str[i] == '(' || str[i] == ')' || str[i] == ':' || str[i] == '&' || str[i] == '\'')
            //            sb.Append(str[i]);
            //    }
            //}
            //else
            for (int i = 0; i <= str.Length - 1; i++)
            {
                if ((str[i] >= '0' && str[i] <= '9') || (str[i] >= 'A' && str[i] <= 'Z') || (str[i] >= 'a' && str[i] <= 'z') || str[i] == ' ' || str[i] == '_' || str[i] == '-' || str[i] == '@' || str[i] == '/' || str[i] == '·' || str[i] == ',' || str[i] == '.' || str[i] == '"' || str[i] == '(' || str[i] == ')' || str[i] == ':' || str[i] == '_' || str[i] == '%')
                    sb.Append(str[i]);
                else if (str[i] == '&' || str[i] == '\'')
                    sb.Append(str[i].ToString().Replace("&", "&amp;").Replace("'", "'"));
            }

            return sb.ToString();
        }

        public static void WriteErrorLogs(string Message)
        {
            string Date = DateTime.Today.Year + "-" + DateTime.Today.Month + "-" + DateTime.Today.Day;
            string logPath = "~/Log/";
            if (!System.IO.Directory.Exists(HttpContext.Current.Server.MapPath(logPath)))
            {
                System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath(logPath));
            }
            StreamWriter sw = File.AppendText(HttpContext.Current.Server.MapPath(logPath) + Date + "-Log.txt");
            sw.WriteLine(DateTime.Now + "  " + Message);
            sw.Close();
            sw.Dispose();
        }

        public static string ConvertDate_QbFormat(string Date)
        {
            string NewDate = "";
            try
            {
                DateTime dt = Convert.ToDateTime(Date);
                NewDate = dt.Year.ToString() + "-" + (dt.Month.ToString().Length == 1 ? "0" + dt.Month.ToString() : dt.Month.ToString()) + "-" + (dt.Day.ToString().Length == 1 ? "0" + dt.Day.ToString() : dt.Day.ToString());
            }
            catch (Exception ex)
            {

            }
            return NewDate;
        }

        public static string ConvertDate_OnlineQBFormat(string Date)
        {
            string strDate = "";
            try
            {
                DateTime dt = Convert.ToDateTime(Date);
                strDate = dt.Year.ToString() + "-" + (dt.Month.ToString().Length == 1 ? "0" + dt.Month.ToString() : dt.Month.ToString()) + "-" + (dt.Day.ToString().Length == 1 ? "0" + dt.Day.ToString() : dt.Day.ToString()) + "T" + (dt.Hour.ToString().Length == 1 ? "0" + dt.Hour.ToString() : dt.Hour.ToString()) + ":" + (dt.Minute.ToString().Length == 1 ? "0" + dt.Minute.ToString() : dt.Minute.ToString()) + ":" + (dt.Second.ToString().Length == 1 ? "0" + dt.Second.ToString() : dt.Second.ToString()) + "-07:00";
            }
            catch (Exception ex)
            {
                string[] Data = Date.Split('-');
                strDate = Data[2].ToString() + "-" + Data[0].ToString() + "-" + Data[1].ToString();

            }
            return strDate;
        }
    }
}
