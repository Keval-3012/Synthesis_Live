using EntityModels.Models;
using Repository;
using Repository.IRepository;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using Utility;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using System.Text;
using static Utility.AdminSiteConfiguration;
using Aspose.Cells;
using Aspose.Pdf;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NLog;
using Syncfusion.EJ2.Base;
using Syncfusion.EJ2.Spreadsheet;
using iText.Kernel.XMP.Options;
using crypto;

namespace SysnthesisRepo.Controllers
{
    public class PDFReadController : Controller
    {
        #region variables
        private static Random random = new Random();
        private readonly IPDFReadRepository _PDFReadRepository;
        private readonly ICommonRepository _CommonRepository;
        private readonly IUserActivityLogRepository _ActivityLogRepository;
        Logger logger = LogManager.GetCurrentClassLogger();
        protected static string StatusMessage = string.Empty;
        protected static Array Arr;
        protected static bool IsArray;
        protected static IEnumerable BindData;
        protected static bool IsEdit = false;
        protected static int TotalDataCount;
        DBContext db = new DBContext();
        #endregion
        public PDFReadController()
        {
            this._PDFReadRepository = new PDFReadRepository(new DBContext());
            this._CommonRepository = new CommonRepository(new DBContext());
            this._ActivityLogRepository = new UserActivityLogRepository(new DBContext());

        }

        /// <summary>
        /// This function is for get Get Product Name list in array.
        /// </summary>
        /// <param name="ProductName"></param>
        /// <returns></returns>
        private string Get_ProductName(string ProductName)
        {
            string str = string.Empty;
            try
            {
                string[] str1 = null;
                ProductName = Regex.Replace(ProductName, @"[\d-]", "~");
                if (ProductName.Contains("-"))
                {
                    str1 = ProductName.Split('-');
                }
                else if (ProductName.Contains("~"))
                {
                    str1 = ProductName.Split('~');
                }
                else
                {
                    str1 = ProductName.Split(' ');
                }
                str = str1[str1.Length - 1].Trim();
                str = Regex.Replace(str, @"[\d-]", string.Empty).Trim();
            }
            catch (Exception ex)
            {
                logger.Error("PDFReadController - Get_ProductName - " + DateTime.Now + " - " + ex.Message.ToString());
            }

            return str;
        }

        /// <summary>
        /// This function is Read PDf.
        /// </summary>
        /// <param name="FileName"></param>
        /// <param name="PayrollReportId"></param>
        /// <returns></returns>
        public ActionResult ReadPDF(string FileName, int PayrollReportId)
        {
            try
            {
                if (FileName != string.Empty)
                {
                    int PayrollId = 0;

                    string FilePath = Server.MapPath("~/userfiles/PayrollFile/") + FileName;
                    List<PDFTable> TableList = new List<PDFTable>();
                    List<PDFTable> TempTable = new List<PDFTable>();

                    List<PDFTable> TableList1 = new List<PDFTable>();
                    List<PDFTable> TempTable1 = new List<PDFTable>();
                    List<PDFTable> TempTable2 = new List<PDFTable>();

                    PdfReader reader = new PdfReader(FilePath);
                    int intPageNum = reader.NumberOfPages;

                    string StartDate = string.Empty;
                    string EndDate = string.Empty;
                    string EndCheckDate = string.Empty;
                    string CheckDate = string.Empty;
                    string[] words;
                    StringBuilder Reader = new StringBuilder();
                    for (int i = 1; i <= intPageNum; i++)
                    {
                        string text = PdfTextExtractor.GetTextFromPage(reader, i, new LocationTextExtractionStrategy());
                        //if (StartDate == "")
                        //{
                        if (text.Contains("Period Sart:") ||
                            text.Contains("Period End:") ||
                            text.Contains("Period Start:") ||
                            text.Contains("End Check Date:") ||
                            text.Contains("Check Date:"))
                        {
                            words = text.Split('\n');

                            for (int j = 0, len = words.Length; j < len; j++)
                            {
                                var LineData = Encoding.UTF8.GetString(Encoding.UTF8.GetBytes(words[j]));
                                if ((LineData.Contains("Period Sart:") || LineData.Contains("Period Start:")) &&
                                    StartDate == string.Empty)
                                {
                                    string[] DateSplitData = LineData.Split(':');
                                    if (DateSplitData.Count() > 0)
                                    {
                                        StartDate = DateSplitData[1];
                                        string[] DateFormate = StartDate.Split('/');
                                        if (DateFormate.Count() == 3)
                                        {
                                            var Month = DateFormate[0].Trim().Length == 1
                                                ? "0" + DateFormate[0].Trim()
                                                : DateFormate[0].Trim();
                                            StartDate = DateFormate[2] + "-" + Month + "-" + DateFormate[1];
                                        }
                                    }
                                }
                                if (LineData.Contains("Period End:") && EndDate == string.Empty)
                                {
                                    string[] DateSplitData = LineData.Split(':');
                                    if (DateSplitData.Count() > 0)
                                    {
                                        EndDate = DateSplitData[1];
                                        string[] DateFormate = EndDate.Split('/');
                                        if (DateFormate.Count() == 3)
                                        {
                                            var Month = DateFormate[0].Trim().Length == 1
                                                ? "0" + DateFormate[0].Trim()
                                                : DateFormate[0].Trim();
                                            EndDate = DateFormate[2].Trim() + "-" + Month + "-" + DateFormate[1].Trim();
                                        }
                                    }
                                }
                                if ((LineData.Contains("End Check Date:") || LineData.Contains("Check Date:")) &&
                                    EndCheckDate == string.Empty)
                                {
                                    if (LineData.Contains("Start") == false)
                                    {
                                        if (LineData.Contains("End Check Date:") && EndCheckDate == string.Empty)
                                        {
                                            string[] DateSplitData = LineData.Split(':');
                                            if (DateSplitData.Count() > 0)
                                            {
                                                EndCheckDate = DateSplitData[1];
                                                string[] DateFormate = EndCheckDate.Split('/');
                                                if (DateFormate.Count() == 3)
                                                {
                                                    var Month = DateFormate[0].Trim().Length == 1
                                                        ? "0" + DateFormate[0].Trim()
                                                        : DateFormate[0].Trim();
                                                    var Day = DateFormate[1].Trim().Length == 1
                                                        ? "0" + DateFormate[1].Trim()
                                                        : DateFormate[1].Trim();
                                                    EndCheckDate = DateFormate[2].Trim() + "-" + Month + "-" + Day;
                                                }
                                            }
                                        }
                                        else if (LineData.Contains("Check Date:") && CheckDate == string.Empty)
                                        {
                                            string[] DateSplitData = LineData.Split(':');
                                            if (DateSplitData.Count() > 0)
                                            {
                                                CheckDate = DateSplitData[1];
                                                string[] DateFormate = CheckDate.Split('/');
                                                if (DateFormate.Count() == 3)
                                                {
                                                    var Month = DateFormate[0].Trim().Length == 1
                                                        ? "0" + DateFormate[0].Trim()
                                                        : DateFormate[0].Trim();
                                                    var Day = DateFormate[1].Trim().Length == 1
                                                        ? "0" + DateFormate[1].Trim()
                                                        : DateFormate[1].Trim();
                                                    CheckDate = DateFormate[2].Trim() + "-" + Month + "-" + Day;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        //}

                        if (text.Contains("Summary:"))
                        {
                            words = text.Split('\n');
                            //words = text;
                            int DVFlg = 0;
                            for (int j = 0, len = words.Length; j < len; j++)
                            {
                                try
                                {
                                    var LineData = Encoding.UTF8.GetString(Encoding.UTF8.GetBytes(words[j]));
                                    if (LineData.Contains("Summary:"))
                                    {
                                        DVFlg = 1;
                                    }
                                    if (DVFlg == 1)
                                    {
                                        LineData = LineData.Trim();
                                        //TableList.Add(new PDFTable { Name = LineData });
                                        if (LineData.Contains("Summary:"))
                                        {
                                            if (LineData.Contains("Summary: 85-SALAD B  - 85-SALAD"))
                                            {
                                            }

                                            if (LineData.Contains("Summary: 85 SALAD  - 85-SALAD BAR Net Pay:"))
                                            {
                                                var PreLine = Encoding.UTF8
                                                    .GetString(Encoding.UTF8.GetBytes(words[j + 1]));
                                                string[] authorsList = PreLine.Split(' ');

                                                //if (StringCount == 10)
                                                //{
                                                TableList.Add(new PDFTable { Name = string.Empty });
                                                TableList.Add(
                                                    new PDFTable
                                                    {
                                                        Name = LineData.Replace("Net Pay:", string.Empty).Trim(),
                                                        Name5 = "Net Pay",
                                                        Value5 = authorsList[0]
                                                    });
                                            }
                                            else if (LineData.Contains("Summary: 85 SALAD  - 85-SALAD BAR"))
                                            {
                                                var PreLine = Encoding.UTF8
                                                    .GetString(Encoding.UTF8.GetBytes(words[j + 1]));
                                                string[] authorsList = PreLine.Split(' ');

                                                //if (StringCount == 10)
                                                //{
                                                TableList.Add(new PDFTable { Name = string.Empty });
                                                TableList.Add(
                                                    new PDFTable
                                                    {
                                                        Name = LineData,
                                                        Name5 = authorsList[0] + " " + authorsList[1],
                                                        Value5 = authorsList[2]
                                                    });
                                            }
                                            else if (LineData.Contains("Summary:") && LineData.Contains("Net Pay:"))
                                            {
                                                string[] authorsList = LineData.Split(' ');
                                                int StringCount = authorsList.Count();
                                                int CountLmt = StringCount >= 8 ? 3 : StringCount == 7 ? 2 : 1;

                                                string MergeName = string.Empty;
                                                for (int x = 0; x < StringCount - CountLmt; x++)
                                                {
                                                    MergeName = MergeName == string.Empty ? authorsList[x]
                                                        : MergeName + " " + authorsList[x];
                                                }
                                                //if (StringCount == 10)
                                                //{
                                                TableList.Add(new PDFTable { Name = string.Empty });
                                                TableList.Add(
                                                    new PDFTable
                                                    {
                                                        Name = MergeName,
                                                        Name5 =
                                                            authorsList[StringCount - 3] +
                                                                    " " +
                                                                    authorsList[StringCount - 2],
                                                        Value5 = authorsList[StringCount - 1]
                                                    });
                                            }
                                            else
                                            {
                                                TableList.Add(new PDFTable { Name = string.Empty });
                                                TableList.Add(new PDFTable { Name = LineData });
                                            }
                                            //}
                                        }
                                        else
                                        {
                                            if (LineData.Contains("CHILD SUP"))
                                            {
                                                LineData = LineData.Replace("CHILD SUP", "CHILD_SUP");
                                            }
                                            if (LineData.Contains("PAID SICK"))
                                            {
                                                LineData = LineData.Replace("PAID SICK", "PAID_SICK");
                                            }
                                            if (LineData.Contains("CCFEE D"))
                                            {
                                                LineData = LineData.Replace("CCFEE D", "CCFEE_D");
                                            }
                                            if (LineData.Contains("CCTIP D"))
                                            {
                                                LineData = LineData.Replace("CCTIP D", "CCTIP_D");
                                            }
                                            if (LineData.Contains("LIFE INS."))
                                            {
                                                LineData = LineData.Replace("LIFE INS.", "LIFE_INS.");
                                            }


                                            if (LineData.Contains(" "))
                                            {
                                                string[] authorsList = LineData.Split(' ');
                                                int StringCount = authorsList.Count();
                                                if (StringCount > 10)
                                                {
                                                    TableList.Add(
                                                        new PDFTable
                                                        {
                                                            Name = authorsList[0],
                                                            Value = authorsList[2],
                                                            Name2 = authorsList[3],
                                                            Value2 = authorsList[4],
                                                            Name3 = authorsList[5],
                                                            Value3 = authorsList[6],
                                                            Name4 = authorsList[7],
                                                            Value4 = authorsList[8],
                                                            Name5 = authorsList[9],
                                                            Value5 = authorsList[10]
                                                        });
                                                }
                                                else if (LineData.Contains("RETRO PAY"))// || LineData.Contains("RETRO OT"))
                                                {
                                                    if (LineData.Contains("$"))
                                                    {
                                                        TableList.Add(
                                                            new PDFTable
                                                            {
                                                                Name = authorsList[0] + " " + authorsList[1],
                                                                Value = authorsList[2],
                                                                Name2 = authorsList[3],
                                                                Value2 = authorsList[4]
                                                            });
                                                    }
                                                    else
                                                    {
                                                        var TempData = TempTable;
                                                        if (TempTable.Count() > 0)
                                                        {
                                                            var TempValues = TempData.FirstOrDefault();
                                                            TableList.Add(
                                                                new PDFTable
                                                                {
                                                                    Name = authorsList[0] + " " + authorsList[1],
                                                                    Value = TempValues.Value,
                                                                    Name2 = authorsList[2],
                                                                    Value2 = TempValues.Value2
                                                                });
                                                            TempTable = new List<PDFTable>();
                                                        }
                                                    }
                                                }
                                                else if (LineData.Contains("OT PREMIUM") ||
                                                    LineData.Contains("OT PREMIU"))
                                                {
                                                    if (LineData.Contains("$"))
                                                    {
                                                        TableList.Add(
                                                            new PDFTable
                                                            {
                                                                Name = authorsList[0] + " " + authorsList[1],
                                                                Value = authorsList[2],
                                                                Name2 = authorsList[3],
                                                                Value2 = authorsList[4]
                                                            });
                                                    }
                                                    else
                                                    {
                                                        var TempData = TempTable;
                                                        if (TempTable.Count() > 0)
                                                        {
                                                            var TempValues = TempData.FirstOrDefault();
                                                            TableList.Add(
                                                                new PDFTable
                                                                {
                                                                    Name = authorsList[0] + " " + authorsList[1],
                                                                    Value = TempValues.Value,
                                                                    Name2 = authorsList[2],
                                                                    Value2 = TempValues.Value2
                                                                });
                                                            TempTable = new List<PDFTable>();
                                                        }
                                                    }
                                                }
                                                else if (StringCount == 10)
                                                {
                                                    TableList.Add(
                                                        new PDFTable
                                                        {
                                                            Name = authorsList[0],
                                                            Value = authorsList[1],
                                                            Name2 = authorsList[2],
                                                            Value2 = authorsList[3],
                                                            Name3 = authorsList[4],
                                                            Value3 = authorsList[5],
                                                            Name4 = authorsList[6],
                                                            Value4 = authorsList[7],
                                                            Name5 = authorsList[8],
                                                            Value5 = authorsList[9]
                                                        });
                                                }
                                                else if (StringCount == 8)
                                                {
                                                    TableList.Add(
                                                        new PDFTable
                                                        {
                                                            Name = authorsList[0],
                                                            Value = authorsList[1],
                                                            Name2 = authorsList[2],
                                                            Value2 = authorsList[3],
                                                            Name3 = authorsList[4],
                                                            Value3 = authorsList[5],
                                                            Name4 = authorsList[6],
                                                            Value4 = authorsList[7]
                                                        });
                                                }
                                                else if (StringCount == 6)
                                                {
                                                    if (authorsList[0].ToLower() == "covid")
                                                    {
                                                        var TempData = TempTable;
                                                        if (TempTable.Count() > 0)
                                                        {
                                                            var TempValues = TempData.FirstOrDefault();
                                                            TableList.Add(
                                                                new PDFTable
                                                                {
                                                                    Name = authorsList[0] + " " + authorsList[1],
                                                                    Value = TempValues.Value,
                                                                    Name2 = authorsList[2],
                                                                    Value2 = TempValues.Value2,
                                                                    Name3 = authorsList[3],
                                                                    Value3 = TempValues.Value3,
                                                                    Name4 = authorsList[4],
                                                                    Value4 = TempValues.Value4,
                                                                    Name5 = authorsList[5],
                                                                    Value5 = TempValues.Value5
                                                                });
                                                            TempTable = new List<PDFTable>();
                                                        }
                                                        // TableList.Add(new PDFTable { Name = authorsList[0] + " " + authorsList[1], Value = authorsList[2], Name2 = authorsList[3], Value2 = authorsList[4] });
                                                    }
                                                    else
                                                    {
                                                        TableList.Add(
                                                            new PDFTable
                                                            {
                                                                Name = authorsList[0],
                                                                Value = authorsList[1],
                                                                Name2 = authorsList[2],
                                                                Value2 = authorsList[3]
                                                            });
                                                    }
                                                }
                                                else if (StringCount == 5)
                                                {
                                                    try
                                                    {
                                                        if (LineData.Contains("$"))
                                                        {
                                                            TempTable.Add(
                                                                new PDFTable
                                                                {
                                                                    Name = string.Empty,
                                                                    Value = authorsList[0],
                                                                    Name2 = string.Empty,
                                                                    Value2 = authorsList[1],
                                                                    Name3 = string.Empty,
                                                                    Value3 = authorsList[2],
                                                                    Name4 = string.Empty,
                                                                    Value4 = authorsList[3],
                                                                    Name5 = string.Empty,
                                                                    Value5 = authorsList[4]
                                                                });
                                                        }
                                                        else
                                                        {
                                                            var TempData = TempTable;
                                                            if (TempTable.Count() > 0)
                                                            {
                                                                var TempValues = TempData.FirstOrDefault();
                                                                TableList.Add(
                                                                    new PDFTable
                                                                    {
                                                                        Name = authorsList[0],
                                                                        Value = TempValues.Value,
                                                                        Name2 = authorsList[1],
                                                                        Value2 = TempValues.Value2,
                                                                        Name3 = authorsList[2],
                                                                        Value3 = TempValues.Value3,
                                                                        Name4 = authorsList[3],
                                                                        Value4 = TempValues.Value4,
                                                                        Name5 = authorsList[4],
                                                                        Value5 = TempValues.Value5
                                                                    });
                                                                TempTable = new List<PDFTable>();
                                                            }
                                                        }
                                                    }
                                                    catch (Exception ex)
                                                    {
                                                        logger.Error("PDFReadController - ReadPDF(1) - " + DateTime.Now + " - " + ex.Message.ToString());
                                                    }
                                                }
                                                else if (StringCount == 4)
                                                {
                                                    try
                                                    {
                                                        if (LineData.Contains("$"))
                                                        {
                                                            var DigitLine = LineData.Replace("$", string.Empty)
                                                                .Replace(" ", string.Empty)
                                                                .Replace(".", string.Empty)
                                                                .Replace(",", string.Empty)
                                                                .Replace("(", string.Empty)
                                                                .Replace(")", string.Empty);
                                                            if (DigitLine.All(s => char.IsDigit(s)))
                                                            {
                                                                TempTable.Add(
                                                                    new PDFTable
                                                                    {
                                                                        Name = string.Empty,
                                                                        Value = authorsList[0],
                                                                        Name2 = string.Empty,
                                                                        Value2 = authorsList[1],
                                                                        Name3 = string.Empty,
                                                                        Value3 = authorsList[2],
                                                                        Name4 = string.Empty,
                                                                        Value4 = authorsList[3]
                                                                    });
                                                            }
                                                            else
                                                            {
                                                                if (LineData.Contains("SSEC") ||
                                                                    LineData.Contains("MEDI") ||
                                                                    LineData.Contains("FWT"))
                                                                {
                                                                    TableList.Add(
                                                                        new PDFTable
                                                                        {
                                                                            Name = string.Empty,
                                                                            Name3 = authorsList[0],
                                                                            Value3 = authorsList[1],
                                                                            Name4 = authorsList[2],
                                                                            Value4 = authorsList[3]
                                                                        });
                                                                }
                                                                else
                                                                {
                                                                    TableList.Add(
                                                                        new PDFTable
                                                                        {
                                                                            Name = authorsList[0],
                                                                            Value = authorsList[1],
                                                                            Name2 = authorsList[2],
                                                                            Value2 = authorsList[3]
                                                                        });
                                                                }
                                                            }
                                                        }
                                                        else
                                                        {
                                                            var TempData = TempTable;
                                                            if (TempTable.Count() > 0)
                                                            {
                                                                var TempValues = TempData.FirstOrDefault();
                                                                TableList.Add(
                                                                    new PDFTable
                                                                    {
                                                                        Name = authorsList[0],
                                                                        Value = TempValues.Value,
                                                                        Name2 = authorsList[1],
                                                                        Value2 = TempValues.Value2,
                                                                        Name3 = authorsList[2],
                                                                        Value3 = TempValues.Value3,
                                                                        Name4 = authorsList[3],
                                                                        Value4 = TempValues.Value4
                                                                    });
                                                                TempTable = new List<PDFTable>();
                                                            }
                                                        }
                                                    }
                                                    catch (Exception ex)
                                                    {
                                                    }
                                                }
                                                else if (StringCount == 3)
                                                {
                                                    try
                                                    {
                                                        if (LineData.Contains("$"))
                                                        {
                                                            var DigitLine = LineData.Replace("$", string.Empty)
                                                                .Replace(" ", string.Empty)
                                                                .Replace(".", string.Empty)
                                                                .Replace(",", string.Empty)
                                                                .Replace("(", string.Empty)
                                                                .Replace(")", string.Empty);
                                                            if (DigitLine.All(s => char.IsDigit(s)))
                                                            {
                                                                TempTable.Add(
                                                                    new PDFTable
                                                                    {
                                                                        Name = string.Empty,
                                                                        Value = authorsList[0],
                                                                        Name2 = string.Empty,
                                                                        Value2 = authorsList[1],
                                                                        Name3 = string.Empty,
                                                                        Value3 = authorsList[2]
                                                                    });
                                                            }
                                                            else
                                                            {
                                                                if (LineData.Contains("SSEC") ||
                                                                    LineData.Contains("MEDI") ||
                                                                    LineData.Contains("FWT"))
                                                                {
                                                                    TableList.Add(
                                                                        new PDFTable
                                                                        {
                                                                            Name = string.Empty,
                                                                            Name3 = authorsList[0],
                                                                            Value3 = authorsList[1],
                                                                            Name4 = authorsList[2],
                                                                        });
                                                                }
                                                                else
                                                                {
                                                                    TableList.Add(
                                                                        new PDFTable
                                                                        {
                                                                            Name = authorsList[0],
                                                                            Value = authorsList[1],
                                                                            Name2 = authorsList[2]
                                                                        });
                                                                }
                                                            }
                                                        }
                                                        else
                                                        {
                                                            var TempData = TempTable;
                                                            if (TempTable.Count() > 0)
                                                            {
                                                                var TempValues = TempData.FirstOrDefault();
                                                                TableList.Add(
                                                                    new PDFTable
                                                                    {
                                                                        Name = authorsList[0],
                                                                        Value = TempValues.Value,
                                                                        Name2 = authorsList[1],
                                                                        Value2 = TempValues.Value2,
                                                                        Name3 = authorsList[2]
                                                                    });
                                                                TempTable = new List<PDFTable>();
                                                            }
                                                        }
                                                    }
                                                    catch (Exception ex)
                                                    {
                                                    }
                                                }
                                                else if (StringCount == 2)
                                                {
                                                    if (LineData.Contains("$"))
                                                    {
                                                        try
                                                        {
                                                            var DigitLine = LineData.Replace("$", string.Empty)
                                                                .Replace(" ", string.Empty)
                                                                .Replace(".", string.Empty)
                                                                .Replace(",", string.Empty);
                                                            if (DigitLine.All(s => char.IsDigit(s)))
                                                            {
                                                                TempTable.Add(
                                                                    new PDFTable
                                                                    {
                                                                        Name = string.Empty,
                                                                        Value3 = authorsList[0],
                                                                        Name2 = string.Empty,
                                                                        Value4 = authorsList[1]
                                                                    });
                                                            }
                                                            else
                                                            {
                                                                if (LineData.Contains("LOCYONRES") ||
                                                                    LineData.Contains("LOCNYCRES"))
                                                                {
                                                                    TableList.Add(
                                                                        new PDFTable
                                                                        {
                                                                            Name = string.Empty,
                                                                            Value3 = authorsList[0],
                                                                            Value4 = authorsList[1]
                                                                        });
                                                                }
                                                            }
                                                        }
                                                        catch (Exception ex)
                                                        {
                                                        }
                                                    }
                                                    else
                                                    {
                                                        var TempData = TempTable;
                                                        if (TempTable.Count() > 0)
                                                        {
                                                            try
                                                            {
                                                                if (authorsList[1] == "HOURS")
                                                                {
                                                                    var TempValues = TempData.FirstOrDefault();
                                                                    TableList.Add(
                                                                        new PDFTable
                                                                        {
                                                                            Name = authorsList[0],
                                                                            Value = TempValues.Value3,
                                                                            Name2 = authorsList[1],
                                                                            Value2 = TempValues.Value4
                                                                        });
                                                                    TempTable = new List<PDFTable>();
                                                                }
                                                                else
                                                                {
                                                                    var TempValues = TempData.FirstOrDefault();
                                                                    TableList.Add(
                                                                        new PDFTable
                                                                        {
                                                                            Name = string.Empty,
                                                                            Name3 = authorsList[0],
                                                                            Value3 = TempValues.Value3,
                                                                            Name4 = authorsList[1],
                                                                            Value4 = TempValues.Value4
                                                                        });
                                                                    TempTable = new List<PDFTable>();
                                                                }
                                                            }
                                                            catch (Exception ex)
                                                            {
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                            if (LineData.Contains("Total"))
                                            {
                                                DVFlg = 0;
                                            }
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    logger.Error("PDFReadController - ReadPDF - " + DateTime.Now + " - " + ex.Message.ToString());
                                }
                            }
                        }

                        if (text.Contains("CASH ANALYSIS"))
                        {
                            TempTable1 = new List<PDFTable>();
                            if (TableList1.Count() == 0)
                            {
                                TableList1 = new List<PDFTable>();
                            }
                            words = text.Split('\n');
                            bool blnGPay = false;
                            for (int j = 0, len = words.Length; j < len; j++)
                            {
                                try
                                {
                                    var LineData = Encoding.UTF8.GetString(Encoding.UTF8.GetBytes(words[j]));
                                    if (LineData.Contains("GROSS PAY") ||
                                        LineData.Contains("DEDUCTION LIABILITY") ||
                                        LineData.Contains("VOID CHECKS") ||
                                        LineData == "MANUAL")
                                    {
                                        TempTable1.Add(new PDFTable { Name = LineData, Value2 = "0" });
                                        //TableList1.Add(new PDFTable { Name = LineData, Value2 = LineData });
                                        blnGPay = true;
                                    }
                                    if (blnGPay == true && LineData.Trim().Contains("TOTAL:"))
                                    {
                                        var TempData = TempTable1;
                                        var PreLine = Encoding.UTF8.GetString(Encoding.UTF8.GetBytes(words[j + 1]));
                                        if (TempTable1.Count() > 0)
                                        {
                                            var TempValues = TempData.FirstOrDefault();
                                            PreLine = PreLine.Trim().Replace("$", string.Empty).Replace(",", string.Empty);
                                            if (TempValues.Name != "VOID CHECKS" && TempValues.Name != "MANUAL")
                                            {
                                                TableList1.Add(
                                                    new PDFTable
                                                    {
                                                        Name = TempValues.Name,
                                                        Value = string.Empty,
                                                        Name2 = TempValues.Name,
                                                        Value2 = PreLine
                                                    });
                                            }
                                            TempTable1 = new List<PDFTable>();
                                        }
                                        blnGPay = false;
                                    }
                                    if (blnGPay == true &&
                                        (TempTable1[0].Name.Contains("DEDUCTION LIABILITY") ||
                                            TempTable1[0].Name.Contains("VOID CHECKS") ||
                                            TempTable1[0].Name == "MANUAL"))
                                    {
                                        var TempData = TempTable1;
                                        if (TempTable1.Count() > 0)
                                        {
                                            if (LineData.Contains("$"))
                                            {
                                                string[] authorsList = LineData.Trim().Replace("$", string.Empty).Split(' ');
                                                if (authorsList.Count() == 2)
                                                {
                                                    var TempValues = TempData.FirstOrDefault();
                                                    TableList1.Add(
                                                        new PDFTable
                                                        {
                                                            Name = TempValues.Name,
                                                            Value = string.Empty,
                                                            Name2 = authorsList[0],
                                                            Value2 = authorsList[1]
                                                        });
                                                }
                                                else if (authorsList.Count() == 3)
                                                {
                                                    var TempValues = TempData.FirstOrDefault();
                                                    TableList1.Add(
                                                        new PDFTable
                                                        {
                                                            Name = TempValues.Name,
                                                            Value = string.Empty,
                                                            Name2 = authorsList[0] + " " + authorsList[1],
                                                            Value2 = authorsList[2]
                                                        });
                                                }
                                                else if (authorsList.Count() == 1)
                                                {
                                                    var PreLine = Encoding.UTF8
                                                        .GetString(Encoding.UTF8.GetBytes(words[j + 1]));
                                                    var TempValues = TempData.FirstOrDefault();
                                                    if (TempValues.Name == "VOID CHECKS" || TempValues.Name == "MANUAL")
                                                    {
                                                        TableList1.Add(
                                                            new PDFTable
                                                            {
                                                                Name = TempValues.Name,
                                                                Value = string.Empty,
                                                                Name2 = TempValues.Name,
                                                                Value2 = authorsList[0]
                                                            });
                                                    }
                                                    else
                                                    {
                                                        TableList1.Add(
                                                            new PDFTable
                                                            {
                                                                Name = TempValues.Name,
                                                                Value = string.Empty,
                                                                Name2 = PreLine,
                                                                Value2 = authorsList[0]
                                                            });
                                                    }
                                                }
                                                else if (authorsList.Count() >= 8)
                                                {
                                                    string[] authorsList1 = LineData.Trim().Split(' ');
                                                    var TempValues = TempData.FirstOrDefault();
                                                    var res = authorsList1.Where(x => x.Contains("$")).FirstOrDefault();
                                                    if (TempValues.Name == "VOID CHECKS" || TempValues.Name == "MANUAL")
                                                    {
                                                        TableList1.Add(
                                                            new PDFTable
                                                            {
                                                                Name = TempValues.Name,
                                                                Value = string.Empty,
                                                                Name2 = TempValues.Name,
                                                                Value2 = res.Replace("$", string.Empty)
                                                            });
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    logger.Error("PDFReadController - ReadPDF - " + DateTime.Now + " - " + ex.Message.ToString());
                                }
                            }
                        }

                        if (text.Contains("Tax Summary"))
                        {
                            TempTable2 = new List<PDFTable>();
                            words = text.Split('\n');
                            bool blnGPay = false;
                            for (int j = 0, len = words.Length; j < len; j++)
                            {
                                try
                                {
                                    var LineData = Encoding.UTF8.GetString(Encoding.UTF8.GetBytes(words[j]));
                                    if ((LineData.Contains("Employee") && !LineData.Contains("Total Employee")) ||
                                        (LineData.Contains("Employer") && !LineData.Contains("Total Employer")))
                                    {
                                        if (!LineData.Contains("Employer Responsible:"))
                                        {
                                            TempTable1.Add(new PDFTable { Name = LineData, Value2 = "0" });
                                            //TableList1.Add(new PDFTable { Name = LineData, Value2 = LineData });
                                            blnGPay = true;
                                        }
                                    }
                                    if (blnGPay == true &&
                                        (LineData.Trim().Contains("Total Employee") ||
                                            LineData.Trim().Contains("Total Employer")))
                                    {
                                        var TempData = TempTable1;
                                        if (TempTable1.Count() > 0)
                                        {
                                            var TempValues = TempData.FirstOrDefault();
                                            if (LineData.Contains("$"))
                                            {
                                                string[] authorsList = LineData.Trim().Split(' ');
                                                if (authorsList.Count() > 2)
                                                {
                                                    string aa = GetFinalValue(authorsList);
                                                    TableList1.Add(
                                                        new PDFTable
                                                        {
                                                            Name = TempValues.Name,
                                                            Value = string.Empty,
                                                            Name2 = authorsList[0] + " " + authorsList[1],
                                                            Value2 = aa
                                                        });
                                                }
                                            }
                                            TempTable1 = new List<PDFTable>();
                                        }
                                        blnGPay = false;
                                    }
                                    if (blnGPay == true &&
                                        (TempTable1[0].Name.Contains("Employee") ||
                                            TempTable1[0].Name.Contains("Employer")))
                                    {
                                        string[] authorsList = LineData.Trim().Split(' ');
                                        int StringCount = authorsList.Count();

                                        if (LineData.Contains("$"))
                                        {
                                            if (StringCount == 2)
                                            {
                                                var DigitLine = LineData.Replace("$", string.Empty)
                                                    .Replace(" ", string.Empty)
                                                    .Replace(".", string.Empty)
                                                    .Replace(",", string.Empty)
                                                    .Replace("(", string.Empty)
                                                    .Replace(")", string.Empty);
                                                if (DigitLine.All(s => char.IsDigit(s)))
                                                {
                                                    TempTable2.Add(
                                                        new PDFTable
                                                        {
                                                            Name = TempTable1[0].Name,
                                                            Value = string.Empty,
                                                            Name2 = string.Empty,
                                                            Value2 = authorsList[0],
                                                            Name3 = string.Empty,
                                                            Value3 = authorsList[1]
                                                        });
                                                }
                                            }
                                            else if (StringCount > 2)
                                            {
                                                string aa = GetFinalValue(authorsList);
                                                TableList1.Add(
                                                    new PDFTable
                                                    {
                                                        Name = TempTable1[0].Name,
                                                        Value = string.Empty,
                                                        Name2 = authorsList[0],
                                                        Value2 = aa
                                                    });
                                            }
                                        }
                                        else
                                        {
                                            if (StringCount > 1)
                                            {
                                                var TempData = TempTable2;
                                                if (TempTable2.Count() > 0)
                                                {
                                                    var TempValues = TempData.FirstOrDefault();
                                                    TableList1.Add(
                                                        new PDFTable
                                                        {
                                                            Name = TempTable1[0].Name,
                                                            Value = string.Empty,
                                                            Name2 = authorsList[0],
                                                            Value2 = TempTable2[0].Value2
                                                        });
                                                    TempTable2 = new List<PDFTable>();
                                                }
                                            }
                                        }
                                    }
                                    if (LineData.Contains("Total due Tax Service:"))
                                    {
                                        var PreLine = Encoding.UTF8.GetString(Encoding.UTF8.GetBytes(words[j - 1]));
                                        TableList1.Add(
                                            new PDFTable
                                            {
                                                Name = "Total due Tax Service:",
                                                Value = PreLine.Trim().Replace(",", string.Empty),
                                                Name2 = "Total due Tax Service:",
                                                Value2 = PreLine.Trim().Replace(",", string.Empty)
                                            });
                                    }
                                }
                                catch (Exception ex)
                                {
                                    logger.Error("PDFReadController - ReadPDF - " + DateTime.Now + " - " + ex.Message.ToString());
                                }
                            }
                        }
                    }

                    List<PDFTable> PDFPrintList = new List<PDFTable>();
                    reader.Close();
                    reader.Dispose();
                    int flg = 0;
                    int DepartmentId = 0;
                    PayrollMaster obj = new PayrollMaster();
                    int StoreId = Convert.ToInt32(Session["storeid"].ToString());
                    obj.StoreId = StoreId;
                    if (StartDate != string.Empty)
                    {
                        obj.StartDate = Convert.ToDateTime(StartDate);
                    }
                    else
                    {
                        obj.StartDate = DateTime.Today;
                    }
                    if (EndDate != string.Empty)
                    {
                        obj.EndDate = Convert.ToDateTime(EndDate);
                    }
                    else
                    {
                        obj.EndDate = DateTime.Today;
                    }
                    if (EndCheckDate != string.Empty)
                    {
                        obj.EndCheckDate = Convert.ToDateTime(EndCheckDate);
                    }
                    else
                    {
                        if (CheckDate != string.Empty)
                            obj.EndCheckDate = Convert.ToDateTime(CheckDate);
                        else
                            obj.EndCheckDate = DateTime.Today;
                    }
                    obj.PayrollReportId = PayrollReportId;

                    //This Db class is used for Get Payroll Master.

                    var FileExist = _PDFReadRepository.GetPayrollMasters(obj);
                    if (FileExist.Count > 0 && 1 == 2)
                    {
                        _PDFReadRepository.ReomvePayrollReports(PayrollReportId);

                        ViewBag.Message = "File Already Exist.";
                        TempData["PDFFile"] = "File Already Exist";
                        return View("Create");
                    }
                    else
                    {
                        obj.PayrollId = _PDFReadRepository.AddPayrollMasters(obj);
                        PayrollId = obj.PayrollId;

                        foreach (var item in TableList)
                        {
                            if (item.Name.Contains("Summary:"))
                            {
                                flg = 4;
                            }
                            if (flg == 4)
                            {
                                PDFPrintList.Add(
                                    new PDFTable
                                    {
                                        Name = item.Name,
                                        Value = item.Value,
                                        Name2 = item.Name2,
                                        Value2 = item.Value2,
                                        Name3 = item.Name3,
                                        Value3 = item.Value3,
                                        Name4 = item.Name4,
                                        Value4 = item.Value4,
                                        Name5 = item.Name5,
                                        Value5 = item.Value5
                                    });
                                if (!item.Name.Contains("Summary:"))
                                {
                                    if (item.Name != string.Empty)
                                    {
                                        if (item.Name != "MEDIPLUS" &&
                                            item.Name != "SDINY" &&
                                            item.Name != "MEDI" &&
                                            item.Name != "EENYPFL" &&
                                            item.Name != "SWTNY" &&
                                            item.Name != "LOCNYCRES" &&
                                            item.Name != "SSEC" &&
                                            item.Name != "FWT")
                                        {
                                            PayrollDepartmentDetails objPayDeptDetails = new PayrollDepartmentDetails();
                                            objPayDeptDetails.Name = item.Name;
                                            objPayDeptDetails.PayrollDepartmentId = DepartmentId;
                                            objPayDeptDetails.PayrollId = PayrollId;
                                            if (item.Value2 != null)
                                            {
                                                try
                                                {
                                                    if (item.Value2.ToString().Contains("("))
                                                    {
                                                        string Valuestr = item.Value2
                                                            .Replace("$", string.Empty)
                                                            .Replace("(", string.Empty)
                                                            .Replace(")", string.Empty)
                                                            .Replace(",", string.Empty);
                                                        decimal Value = Convert.ToDecimal(Valuestr);
                                                        objPayDeptDetails.Value = -Value;
                                                    }
                                                    else
                                                    {
                                                        string Valuestr = item.Value2.Replace("$", string.Empty).Replace(",", string.Empty);
                                                        decimal Value = Convert.ToDecimal(Valuestr);
                                                        objPayDeptDetails.Value = Value;
                                                    }
                                                    _PDFReadRepository.AddPayrollDepartmentDetails(objPayDeptDetails);
                                                }
                                                catch (Exception ex)
                                                {
                                                    logger.Error("PDFReadController - ReadPDF - " + DateTime.Now + " - " + ex.Message.ToString());
                                                }
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    PayrollDepartment objPayDept = new PayrollDepartment();
                                    string[] DepartmentArr = item.Name.Split(':');
                                    if (DepartmentArr.Count() > 1)
                                    {
                                        string DepartmentName = DepartmentArr[1].Replace("Net Pay", string.Empty)
                                            .ToString()
                                            .Trim();
                                        string str = Get_ProductName(DepartmentName)
                                            .Replace("FROZEN FOODS", "FROZEN")
                                            .Replace("FROZEN FOOD", "FROZEN")
                                            .Replace("CHESSE", "CHEESE");
                                        //This Db class is  get payroll departments list.
                                        var DeptExist = _PDFReadRepository.GetPayrollDepartmentsList(StoreId, str);
                                        if (DeptExist.Count > 0)
                                        {
                                            DepartmentId = Convert.ToInt32(
                                                DeptExist.FirstOrDefault().PayrollDepartmentId);
                                            ;
                                        }
                                        else
                                        {
                                            try
                                            {
                                                objPayDept.StoreId = StoreId;
                                                objPayDept.DepartmentName = str;
                                                //This Db class is  Add payroll departments.
                                                _PDFReadRepository.AddPayrollDepartments(objPayDept);

                                                DepartmentId = objPayDept.PayrollDepartmentId;
                                            }
                                            catch (Exception ex)
                                            {
                                                logger.Error("PDFReadController - ReadPDF - " + DateTime.Now + " - " + ex.Message.ToString());
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        if (TableList.Count() > 0)
                        {
                            _PDFReadRepository.UpdatePayrollReportStatus(obj.PayrollReportId);
                        }

                        foreach (var item in TableList1)
                        {
                            if (item.Name2 != "DEDUCTION LIABILITY" &&
                                item.Name2 != "Total Employee" &&
                                item.Name2 != "Total Employer")
                            {
                                PayrollCashAnalysis objPayDept = new PayrollCashAnalysis();
                                var DeptExist = _PDFReadRepository.Getpayrollcashanalysis(StoreId, item.Name2.Replace(":", string.Empty).Trim());
                                if (DeptExist.Count > 0)
                                {
                                    DepartmentId = Convert.ToInt32(DeptExist.FirstOrDefault().PayrollCashAnalysisId);
                                }
                                else
                                {
                                    objPayDept.StoreId = StoreId;
                                    objPayDept.Name = item.Name2.Replace(":", string.Empty).Trim();
                                    _PDFReadRepository.AddpayrollCashAnalyse(objPayDept);
                                    DepartmentId = objPayDept.PayrollCashAnalysisId;
                                }

                                if (item.Name2 != null)
                                {
                                    PayrollCashAnalysisDetail objPayDetail = new PayrollCashAnalysisDetail();
                                    objPayDetail.PayrollCashAnalysisId = DepartmentId;
                                    objPayDetail.PayrollId = PayrollId;
                                    if (item.Value2 != null)
                                    {
                                        if (item.Value2.ToString().Contains("("))
                                        {
                                            string Valuestr = item.Value2
                                                .Replace("$", string.Empty)
                                                .Replace("(", string.Empty)
                                                .Replace(")", string.Empty)
                                                .Replace(",", string.Empty);
                                            decimal Value = Convert.ToDecimal(Valuestr);
                                            objPayDetail.Amount = -Value;
                                        }
                                        else
                                        {
                                            string Valuestr = item.Value2.Replace("$", string.Empty).Replace(",", string.Empty);
                                            decimal Value = Convert.ToDecimal(Valuestr);
                                            objPayDetail.Amount = Value;
                                        }
                                        _PDFReadRepository.AddPayrollCashAnalysisDetail(objPayDetail);
                                    }
                                    objPayDetail = null;
                                }
                            }
                        }
                        ViewBag.PDFData = TableList;
                        ViewBag.Message = "File uploaded successfully.";
                        TempData["PDFFile"] = "File uploaded successfully.";
                        return View("PDFRead", PDFPrintList.Where(s => s.Name != string.Empty));
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("PDFReadController - ReadPDF - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            ViewBag.ErrorMessage = "PDF File Path Not Found..";
            return View("Create");
        }

        /// <summary>
        /// Get Payroll Reports data
        /// </summary>
        /// <param name="SearchRecords"></param>
        /// <param name="SearchTitle"></param>
        /// <param name="StoreID"></param>
        /// <returns></returns>
        private IEnumerable GetData(int SearchRecords = 0, string SearchTitle = "", int StoreID = 0)
        {
            IEnumerable RtnData = null;
            //Get payroll report by store Id.
            RtnData = _PDFReadRepository.GetpayrollReportsByStoreID(StoreID);
            return RtnData;
        }

        /// <summary>
        /// This Function is for get file with byte[].
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        byte[] GetFile(string s)
        {
            System.IO.FileStream fs = System.IO.File.OpenRead(s);
            byte[] data = new byte[fs.Length];
            try
            {
                int br = fs.Read(data, 0, data.Length);
                if (br != fs.Length)
                    throw new System.IO.IOException(s);
            }
            catch (Exception ex)
            {
                logger.Error("PDFReadController - GetFile - " + DateTime.Now + " - " + ex.Message.ToString());
            }

            return data;
        }

        /// <summary>
        /// This Function is for getfinal Array.
        /// </summary>
        /// <param name="arr"></param>
        /// <returns></returns>
        private string[] GetFinalArr(string[] arr)
        {
            string[] arr1 = new string[arr.Count()];
            try
            {
                int iCount = 0;
                string str = string.Empty;
                bool First = true;
                for (int i = 0; i <= arr.Count() - 1; i++)
                {
                    var DigitLine = arr[i].Replace("$", string.Empty)
                        .Replace(" ", string.Empty)
                        .Replace(".", string.Empty)
                        .Replace(",", string.Empty)
                        .Replace("(", string.Empty)
                        .Replace(")", string.Empty)
                        .TrimStart(new Char[] { '0' });
                    if (DigitLine.Trim().All(s => char.IsDigit(s)) || arr[i].Trim().Contains("%"))
                    {
                        if (str != string.Empty)
                        {
                            arr1[iCount] = str;
                            iCount++;
                            First = false;
                            str = string.Empty;
                        }
                        arr1[iCount] = arr[i].Trim();
                        iCount++;
                    }
                    else if (DigitLine.Trim() == "-")
                    {
                        if (str != string.Empty)
                        {
                            arr1[iCount] = str;
                            iCount++;
                            First = false;
                            str = string.Empty;
                        }
                        arr1[iCount] = arr[i].Trim();
                        iCount++;
                    }
                    else
                    {
                        if (First)
                        {
                            str = (str == string.Empty ? arr[i].Trim() : str + " " + arr[i].Trim());
                        }
                        else
                        {
                            arr1[iCount] = arr[i].Trim();
                            iCount++;
                        }
                    }
                }
                arr1 = arr1.Where(a => !string.IsNullOrEmpty(a)).ToArray();
            }
            catch (Exception ex)
            {
                logger.Error("PDFReadController - GetFinalArr - " + DateTime.Now + " - " + ex.Message.ToString());
            }

            return arr1;
        }

        /// <summary>
        /// This Function is for GetFinalArrRev.
        /// </summary>
        /// <param name="arr"></param>
        /// <returns></returns>
        private string[] GetFinalArrRev(string[] arr)
        {
            string[] arr1 = new string[arr.Count()];
            try
            {
                int iCount = 0;
                string str = string.Empty;
                bool First = true;
                for (int i = 0; i <= arr.Count() - 1; i++)
                {
                    var DigitLine = arr[i].Replace("$", string.Empty)
                        .Replace(" ", string.Empty)
                        .Replace(".", string.Empty)
                        .Replace(",", string.Empty)
                        .Replace("(", string.Empty)
                        .Replace(")", string.Empty)
                        .TrimStart(new Char[] { '0' });
                    if (DigitLine.Length > 1)
                    {
                        if (DigitLine[0].ToString() == "-")
                        {
                            DigitLine = DigitLine.Replace("-", string.Empty);
                        }
                    }
                    if (DigitLine.Trim().All(s => char.IsDigit(s)) || arr[i].Trim().Contains("%"))
                    {
                        if (str != string.Empty)
                        {
                            arr1[iCount] = str;
                            iCount++;
                            First = false;
                            str = string.Empty;
                        }

                        arr1[iCount] = arr[i].Trim();
                        iCount++;
                    }
                    else
                    {
                        if (First)
                        {
                            str = (str == string.Empty ? arr[i].Trim() : arr[i].Trim() + " " + str);
                        }
                        else
                        {
                            arr1[iCount] = arr[i].Trim();
                            iCount++;
                        }
                    }
                }

                if (First == true && str != string.Empty)
                {
                    arr1[iCount] = str;
                }
                arr1 = arr1.Where(a => !string.IsNullOrEmpty(a)).ToArray();
            }
            catch (Exception ex)
            {
                logger.Error("PDFReadController - GetFinalArrRev - " + DateTime.Now + " - " + ex.Message.ToString());
            }

            return arr1;
        }

        /// <summary>
        /// This Function is for get final Value.
        /// </summary>
        /// <param name="arr"></param>
        /// <returns></returns>
        private string GetFinalValue(string[] arr)
        {
            string str = string.Empty;
            try
            {
                for (int i = 0; i <= arr.Count() - 1; i++)
                {
                    var DigitLine = arr[i].Replace("$", string.Empty)
                        .Replace(" ", string.Empty)
                        .Replace(".", string.Empty)
                        .Replace(",", string.Empty)
                        .Replace("(", string.Empty)
                        .Replace(")", string.Empty);
                    if (DigitLine.All(s => char.IsDigit(s)))
                    {
                        str = arr[i].Replace(" ", string.Empty);
                        return str;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("PDFReadController - GetFinalValue - " + DateTime.Now + " - " + ex.Message.ToString());
            }

            return str;
        }

        /// <summary>
        /// This class is used to get last page index for totaldatacount.
        /// </summary>
        /// <param name="PageSize"></param>
        /// <returns></returns>
        private int getLastPageIndex(int PageSize)
        {
            int lastPageIndex = Convert.ToInt32(TotalDataCount) / PageSize;
            if (TotalDataCount % PageSize > 0)
                lastPageIndex += 1;
            return lastPageIndex;
        }

        /// <summary>
        /// This class is used to ReadingFile2_13Column.
        /// </summary>
        /// <param name="r"></param>
        /// <param name="TableList"></param>
        /// <param name="flag"></param>
        /// <param name="name"></param>
        private void ReadingFile2_13Column(DataRow r, ref List<PDFTable> TableList, ref Boolean flag, ref string name)
        {
            if (flag == true)
            {
                try
                {
                    if (!String.IsNullOrEmpty(r.Field<string>("Column1")))
                    {
                        flag = false;
                    }
                    else if (!String.IsNullOrEmpty(r.Field<string>("Column4")))
                    {
                        flag = false;
                    }
                    else
                    {
                        if (!String.IsNullOrEmpty(r.Field<string>("Column7")))
                        {
                            TableList.Add(
                                new PDFTable
                                {
                                    Name = "REGULAR",
                                    Value =
                                        r.Field<string>("Column7")
                                                .Replace('(', '-')
                                                .Replace(')', ' ')
                                                .Replace(" ", string.Empty)
                                                .Replace(",", string.Empty)
                                                .Trim(),
                                    Name2 = "File2",
                                    Name3 = name
                                });
                        }
                        if (!String.IsNullOrEmpty(r.Field<string>("Column8")))
                        {
                            TableList.Add(
                                new PDFTable
                                {
                                    Name = "OVERTIME",
                                    Value =
                                        r.Field<string>("Column8")
                                                .Replace('(', '-')
                                                .Replace(')', ' ')
                                                .Replace(" ", string.Empty)
                                                .Replace(",", string.Empty)
                                                .Trim(),
                                    Name2 = "File2",
                                    Name3 = name
                                });
                        }
                        if (!String.IsNullOrEmpty(r.Field<string>("Column9")))
                        {
                            string inputString = r.Field<string>("Column9")
                                .Replace('(', '-')
                                .Replace(')', ' ')
                                .Replace(" ", string.Empty)
                                .Replace(",", string.Empty)
                                .Trim();
                            try
                            {
                                double Val = Convert.ToDouble(inputString);
                                if (!String.IsNullOrEmpty(r.Field<string>("Column10")))
                                    inputString = Val.ToString() +
                                        r.Field<string>("Column10")
                                            .Replace('(', '-')
                                            .Replace(')', ' ')
                                            .Replace(" ", string.Empty)
                                            .Replace(",", string.Empty)
                                            .Trim();
                                else
                                    inputString = Val.ToString();
                            }
                            catch (Exception ex)
                            {
                            }
                            string pattern = @"([0-9,.-]+)([A-Za-z]+)";
                            Match match = Regex.Match(inputString, pattern);

                            if (match.Success)
                            {
                                string numericPart = match.Groups[1].Value;
                                string textPart = match.Groups[2].Value;
                                TableList.Add(
                                    new PDFTable
                                    {
                                        Name = textPart.Replace(" ", "_").Trim(),
                                        Value = numericPart,
                                        Name2 = "File2",
                                        Name3 = name
                                    });
                            }
                        }
                        else if (!String.IsNullOrEmpty(r.Field<string>("Column10")))
                        {
                            string inputString = r.Field<string>("Column10")
                                .Replace('(', '-')
                                .Replace(')', ' ')
                                .Replace(" ", string.Empty)
                                .Replace(",", string.Empty)
                                .Trim();
                            string pattern = @"([0-9,.-]+)([A-Za-z]+)";
                            Match match = Regex.Match(inputString, pattern);

                            if (match.Success)
                            {
                                string numericPart = match.Groups[1].Value;
                                string textPart = match.Groups[2].Value;
                                TableList.Add(
                                    new PDFTable
                                    {
                                        Name = textPart.Replace(" ", "_").Trim(),
                                        Value = numericPart,
                                        Name2 = "File2",
                                        Name3 = name
                                    });
                            }
                            //else
                            //{
                            //    TableList.Add(new PDFTable { Name = r.Field<string>("Column9").Replace(" ", "_").Trim(), Value = r.Field<string>("Column9").Replace('(', '-').Replace(')', ' ').Replace(" ", "").Replace(",", "").Trim(), Name2 = "File2", Name3 = name });
                            //}
                        }
                    }
                }
                catch (Exception ex)
                {
                    logger.Error("PDFReadController - ReadingFile2_13Column - " + DateTime.Now + " - " + ex.Message.ToString());
                }

            }
            if (!String.IsNullOrEmpty(r.Field<string>("Column1")))
            {
                try
                {
                    if (r.Field<string>("Column1").ToLower().Contains("management"))
                    {
                        name = "MANAGEMENT";
                    }
                    else if (r.Field<string>("Column1").ToLower().Contains("general clerk"))
                    {
                        name = "GENERAL CLERK";
                    }
                    else if (r.Field<string>("Column1").ToLower().Contains("grocery"))
                    {
                        name = "GROCERY";
                    }
                    else if (r.Field<string>("Column1").ToLower().Contains("prepared foods"))
                    {
                        name = "PREPARED FOODS";
                    }
                    else if (r.Field<string>("Column1").ToLower().Contains("bakery"))
                    {
                        name = "BAKERY";
                    }
                    else if (r.Field<string>("Column1").ToLower().Contains("kitchen"))
                    {
                        name = "KITCHEN";
                    }
                    else if (r.Field<string>("Column1").ToLower().Contains("dairy"))
                    {
                        name = "DAIRY";
                    }
                    else if (r.Field<string>("Column1").ToLower().Contains("frozen foods"))
                    {
                        name = "FROZEN FOODS";
                    }
                    else if (r.Field<string>("Column1").ToLower().Contains("chesse"))
                    {
                        name = "CHESSE";
                    }
                    else if (r.Field<string>("Column1").ToLower().Contains("cashier"))
                    {
                        name = "CASHIER";
                    }
                    else if (r.Field<string>("Column1").ToLower().Contains("produce"))
                    {
                        name = "PRODUCE";
                    }
                    else if (r.Field<string>("Column1").ToLower().Contains("cheese"))
                    {
                        name = "CHEESE";
                    }
                    else if (r.Field<string>("Column1").ToLower().Contains("salad bar"))
                    {
                        name = "SALAD BAR";
                    }
                    else if (r.Field<string>("Column1").ToLower().Contains("maintenance"))
                    {
                        name = "MAINTENANCE";
                    }
                    else if (r.Field<string>("Column1").ToLower().Contains("meat"))
                    {
                        name = "MEAT";
                    }
                    else if (r.Field<string>("Column1").ToLower().Contains("delivery"))
                    {
                        name = "DELIVERY";
                    }
                    else if (r.Field<string>("Column1").ToLower().Contains("office"))
                    {
                        name = "OFFICE";
                    }
                    else if (r.Field<string>("Column1").ToLower().Contains("security"))
                    {
                        name = "SECURITY";
                    }
                    else if (r.Field<string>("Column1").ToLower().Contains("fish"))
                    {
                        name = "FISH";
                    }
                    else if (r.Field<string>("Column1").ToLower().Contains("office"))
                    {
                        name = "OFFICE";
                    }
                    else if (r.Field<string>("Column1").ToLower().Contains("deli"))
                    {
                        name = "DELI";
                    }
                }
                catch (Exception ex)
                {
                    logger.Error("PDFReadController - ReadingFile2_13Column - " + DateTime.Now + " - " + ex.Message.ToString());
                }

                try
                {
                    if (r.Field<string>("Column1").ToLower() == "department totals")
                    {
                        if (!String.IsNullOrEmpty(r.Field<string>("Column7")))
                        {
                            TableList.Add(
                                new PDFTable
                                {
                                    Name = "REGULAR",
                                    Value =
                                        r.Field<string>("Column7")
                                                .Replace('(', ' ')
                                                .Replace(')', ' ')
                                                .Replace(",", string.Empty)
                                                .Trim(),
                                    Name2 = "File2",
                                    Name3 = name
                                });
                        }
                        if (!String.IsNullOrEmpty(r.Field<string>("Column8")))
                        {
                            TableList.Add(
                                new PDFTable
                                {
                                    Name = "OVERTIME",
                                    Value =
                                        r.Field<string>("Column8")
                                                .Replace('(', ' ')
                                                .Replace(')', ' ')
                                                .Replace(",", string.Empty)
                                                .Trim(),
                                    Name2 = "File2",
                                    Name3 = name
                                });
                        }
                        if (!String.IsNullOrEmpty(r.Field<string>("Column9")))
                        {
                            string inputString = r.Field<string>("Column9")
                                .Replace('(', '-')
                                .Replace(')', ' ')
                                .Replace(" ", string.Empty)
                                .Replace(",", string.Empty)
                                .Trim();
                            try
                            {
                                double Val = Convert.ToDouble(inputString);
                                if (!String.IsNullOrEmpty(r.Field<string>("Column10")))
                                    inputString = Val.ToString() +
                                        r.Field<string>("Column10")
                                            .Replace('(', '-')
                                            .Replace(')', ' ')
                                            .Replace(" ", string.Empty)
                                            .Replace(",", string.Empty)
                                            .Trim();
                                else
                                    inputString = Val.ToString();
                            }
                            catch (Exception ex)
                            {
                            }
                            string pattern = @"([0-9,.-]+)([A-Za-z]+)";
                            Match match = Regex.Match(inputString, pattern);

                            if (match.Success)
                            {
                                string numericPart = match.Groups[1].Value;
                                string textPart = match.Groups[2].Value;
                                TableList.Add(
                                    new PDFTable
                                    {
                                        Name = textPart.Replace(" ", "_").Trim(),
                                        Value = numericPart,
                                        Name2 = "File2",
                                        Name3 = name
                                    });
                            }
                        }
                        else if (!String.IsNullOrEmpty(r.Field<string>("Column10")))
                        {
                            string inputString = r.Field<string>("Column10")
                                .Replace('(', '-')
                                .Replace(')', ' ')
                                .Replace(" ", string.Empty)
                                .Replace(",", string.Empty)
                                .Trim();
                            string pattern = @"([0-9,.-]+)([A-Za-z]+)";
                            Match match = Regex.Match(inputString, pattern);

                            if (match.Success)
                            {
                                string numericPart = match.Groups[1].Value;
                                string textPart = match.Groups[2].Value;
                                TableList.Add(
                                    new PDFTable
                                    {
                                        Name = textPart.Replace(" ", "_").Trim(),
                                        Value = numericPart,
                                        Name2 = "File2",
                                        Name3 = name
                                    });
                            }
                            //else
                            //{
                            //    TableList.Add(new PDFTable { Name = r.Field<string>("Column9").Replace(" ", "_").Trim(), Value = r.Field<string>("Column9").Replace('(', '-').Replace(')', ' ').Replace(" ", "").Replace(",", "").Trim(), Name2 = "File2", Name3 = name });
                            //}
                        }

                        flag = true;
                    }
                }
                catch (Exception ex)
                {
                    logger.Error("PDFReadController - ReadingFile2_13Column - " + DateTime.Now + " - " + ex.Message.ToString());
                }

            }
        }

        /// <summary>
        ///This function is used to ReadingFile2_14 Column.
        /// </summary>
        /// <param name="r"></param>
        /// <param name="TableList"></param>
        /// <param name="flag"></param>
        /// <param name="name"></param>
        private void ReadingFile2_14Column(DataRow r, ref List<PDFTable> TableList, ref Boolean flag, ref string name)
        {
            if (flag == true)
            {
                try
                {
                    if (!String.IsNullOrEmpty(r.Field<string>("Column1")))
                    {
                        flag = false;
                    }
                    else if (!String.IsNullOrEmpty(r.Field<string>("Column4")))
                    {
                        flag = false;
                    }
                    else
                    {
                        if (!String.IsNullOrEmpty(r.Field<string>("Column7")))
                        {
                            TableList.Add(
                                new PDFTable
                                {
                                    Name = "REGULAR",
                                    Value =
                                        r.Field<string>("Column7")
                                                .Replace('(', '-')
                                                .Replace(')', ' ')
                                                .Replace(" ", string.Empty)
                                                .Replace(",", string.Empty)
                                                .Trim(),
                                    Name2 = "File2",
                                    Name3 = name
                                });
                        }
                        if (!String.IsNullOrEmpty(r.Field<string>("Column8")))
                        {
                            TableList.Add(
                                new PDFTable
                                {
                                    Name = "OVERTIME",
                                    Value =
                                        r.Field<string>("Column8")
                                                .Replace('(', '-')
                                                .Replace(')', ' ')
                                                .Replace(" ", string.Empty)
                                                .Replace(",", string.Empty)
                                                .Trim(),
                                    Name2 = "File2",
                                    Name3 = name
                                });
                        }
                        if (!String.IsNullOrEmpty(r.Field<string>("Column9")))
                        {
                            string inputString = r.Field<string>("Column9")
                                .Replace('(', '-')
                                .Replace(')', ' ')
                                .Replace(" ", string.Empty)
                                .Replace(",", string.Empty)
                                .Trim();
                            try
                            {
                                double Val = Convert.ToDouble(inputString);
                                if (!String.IsNullOrEmpty(r.Field<string>("Column10")))
                                    inputString = Val.ToString() +
                                        r.Field<string>("Column10")
                                            .Replace('(', '-')
                                            .Replace(')', ' ')
                                            .Replace(" ", string.Empty)
                                            .Replace(",", string.Empty)
                                            .Trim();
                                else
                                    inputString = Val.ToString();
                            }
                            catch (Exception ex)
                            {
                            }
                            string pattern = @"([0-9,.-]+)([A-Za-z]+)";
                            Match match = Regex.Match(inputString, pattern);

                            if (match.Success)
                            {
                                string numericPart = match.Groups[1].Value;
                                string textPart = match.Groups[2].Value;
                                TableList.Add(
                                    new PDFTable
                                    {
                                        Name = textPart.Replace(" ", "_").Trim(),
                                        Value = numericPart,
                                        Name2 = "File2",
                                        Name3 = name
                                    });
                            }
                        }
                        else if (!String.IsNullOrEmpty(r.Field<string>("Column10")))
                        {
                            string inputString = r.Field<string>("Column10")
                                .Replace('(', '-')
                                .Replace(')', ' ')
                                .Replace(" ", string.Empty)
                                .Replace(",", string.Empty)
                                .Trim();
                            string pattern = @"([0-9,.-]+)([A-Za-z]+)";
                            Match match = Regex.Match(inputString, pattern);

                            if (match.Success)
                            {
                                string numericPart = match.Groups[1].Value;
                                string textPart = match.Groups[2].Value;
                                TableList.Add(
                                    new PDFTable
                                    {
                                        Name = textPart.Replace(" ", "_").Trim(),
                                        Value = numericPart,
                                        Name2 = "File2",
                                        Name3 = name
                                    });
                            }
                            //else
                            //{
                            //    TableList.Add(new PDFTable { Name = r.Field<string>("Column9").Replace(" ", "_").Trim(), Value = r.Field<string>("Column9").Replace('(', '-').Replace(')', ' ').Replace(" ", "").Replace(",", "").Trim(), Name2 = "File2", Name3 = name });
                            //}
                        }
                    }
                }
                catch (Exception ex)
                {
                    logger.Error("PDFReadController - ReadingFile2_14Column - " + DateTime.Now + " - " + ex.Message.ToString());
                }

            }
            if (!String.IsNullOrEmpty(r.Field<string>("Column1")))
            {
                try
                {
                    if (r.Field<string>("Column1").ToLower().Contains("management"))
                    {
                        name = "MANAGEMENT";
                    }
                    else if (r.Field<string>("Column1").ToLower().Contains("general clerk"))
                    {
                        name = "GENERAL CLERK";
                    }
                    else if (r.Field<string>("Column1").ToLower().Contains("grocery"))
                    {
                        name = "GROCERY";
                    }
                    else if (r.Field<string>("Column1").ToLower().Contains("prepared foods"))
                    {
                        name = "PREPARED FOODS";
                    }
                    else if (r.Field<string>("Column1").ToLower().Contains("bakery"))
                    {
                        name = "BAKERY";
                    }
                    else if (r.Field<string>("Column1").ToLower().Contains("kitchen"))
                    {
                        name = "KITCHEN";
                    }
                    else if (r.Field<string>("Column1").ToLower().Contains("dairy"))
                    {
                        name = "DAIRY";
                    }
                    else if (r.Field<string>("Column1").ToLower().Contains("frozen foods"))
                    {
                        name = "FROZEN FOODS";
                    }
                    else if (r.Field<string>("Column1").ToLower().Contains("chesse"))
                    {
                        name = "CHESSE";
                    }
                    else if (r.Field<string>("Column1").ToLower().Contains("cashier"))
                    {
                        name = "CASHIER";
                    }
                    else if (r.Field<string>("Column1").ToLower().Contains("produce"))
                    {
                        name = "PRODUCE";
                    }
                    else if (r.Field<string>("Column1").ToLower().Contains("cheese"))
                    {
                        name = "CHEESE";
                    }
                    else if (r.Field<string>("Column1").ToLower().Contains("salad bar"))
                    {
                        name = "SALAD BAR";
                    }
                    else if (r.Field<string>("Column1").ToLower().Contains("maintenance"))
                    {
                        name = "MAINTENANCE";
                    }
                    else if (r.Field<string>("Column1").ToLower().Contains("meat"))
                    {
                        name = "MEAT";
                    }
                    else if (r.Field<string>("Column1").ToLower().Contains("delivery"))
                    {
                        name = "DELIVERY";
                    }
                    else if (r.Field<string>("Column1").ToLower().Contains("office"))
                    {
                        name = "OFFICE";
                    }
                    else if (r.Field<string>("Column1").ToLower().Contains("security"))
                    {
                        name = "SECURITY";
                    }
                    else if (r.Field<string>("Column1").ToLower().Contains("fish"))
                    {
                        name = "FISH";
                    }
                    else if (r.Field<string>("Column1").ToLower().Contains("office"))
                    {
                        name = "OFFICE";
                    }
                    else if (r.Field<string>("Column1").ToLower().Contains("deli"))
                    {
                        name = "DELI";
                    }
                    else if (r.Field<string>("Column1").ToLower().Contains("online shoppers"))
                    {
                        name = "ONLINE SHOPPERS";
                    }
                }
                catch (Exception ex)
                {
                    logger.Error("PDFReadController - ReadingFile2_14Column - " + DateTime.Now + " - " + ex.Message.ToString());
                }


                if (r.Field<string>("Column1").ToLower() == "department totals")
                {
                    try
                    {
                        if (!String.IsNullOrEmpty(r.Field<string>("Column7")))
                        {
                            TableList.Add(
                                new PDFTable
                                {
                                    Name = "REGULAR",
                                    Value =
                                        r.Field<string>("Column7")
                                                .Replace('(', ' ')
                                                .Replace(')', ' ')
                                                .Replace(",", string.Empty)
                                                .Trim(),
                                    Name2 = "File2",
                                    Name3 = name
                                });
                        }
                        if (!String.IsNullOrEmpty(r.Field<string>("Column8")))
                        {
                            TableList.Add(
                                new PDFTable
                                {
                                    Name = "OVERTIME",
                                    Value =
                                        r.Field<string>("Column8")
                                                .Replace('(', ' ')
                                                .Replace(')', ' ')
                                                .Replace(",", string.Empty)
                                                .Trim(),
                                    Name2 = "File2",
                                    Name3 = name
                                });
                        }
                        if (!String.IsNullOrEmpty(r.Field<string>("Column9")))
                        {
                            string inputString = r.Field<string>("Column9")
                                .Replace('(', '-')
                                .Replace(')', ' ')
                                .Replace(" ", string.Empty)
                                .Replace(",", string.Empty)
                                .Trim();
                            try
                            {
                                double Val = Convert.ToDouble(inputString);
                                if (!String.IsNullOrEmpty(r.Field<string>("Column10")))
                                    inputString = Val.ToString() +
                                        r.Field<string>("Column10")
                                            .Replace('(', '-')
                                            .Replace(')', ' ')
                                            .Replace(" ", string.Empty)
                                            .Replace(",", string.Empty)
                                            .Trim();
                                else
                                    inputString = Val.ToString();
                            }
                            catch (Exception ex)
                            {
                            }
                            string pattern = @"([0-9,.-]+)([A-Za-z]+)";
                            Match match = Regex.Match(inputString, pattern);

                            if (match.Success)
                            {
                                string numericPart = match.Groups[1].Value;
                                string textPart = match.Groups[2].Value;
                                TableList.Add(
                                    new PDFTable
                                    {
                                        Name = textPart.Replace(" ", "_").Trim(),
                                        Value = numericPart,
                                        Name2 = "File2",
                                        Name3 = name
                                    });
                            }
                            else
                            {
                                string N = name;
                                if (TableList.AsEnumerable()
                                        .Where(s => s.Name == "OVERTIME" && s.Name3 == N && s.Name2 == "File2")
                                        .Count() ==
                                    0)
                                {
                                    TableList.Add(
                                        new PDFTable
                                        {
                                            Name = "OVERTIME",
                                            Value = inputString.ToString(),
                                            Name2 = "File2",
                                            Name3 = name
                                        });
                                }
                            }
                        }
                        else if (!String.IsNullOrEmpty(r.Field<string>("Column10")))
                        {
                            string inputString = r.Field<string>("Column10")
                                .Replace('(', '-')
                                .Replace(')', ' ')
                                .Replace(" ", string.Empty)
                                .Replace(",", string.Empty)
                                .Trim();
                            string pattern = @"([0-9,.-]+)([A-Za-z]+)";
                            Match match = Regex.Match(inputString, pattern);

                            if (match.Success)
                            {
                                string numericPart = match.Groups[1].Value;
                                string textPart = match.Groups[2].Value;
                                TableList.Add(
                                    new PDFTable
                                    {
                                        Name = textPart.Replace(" ", "_").Trim(),
                                        Value = numericPart,
                                        Name2 = "File2",
                                        Name3 = name
                                    });
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        logger.Error("PDFReadController - ReadingFile2_13Column - " + DateTime.Now + " - " + ex.Message.ToString());
                    }

                    flag = true;
                }
            }
        }

        /// <summary>
        /// This class is used to ReadingFile2_15 Column.
        /// </summary>
        /// <param name="r"></param>
        /// <param name="TableList"></param>
        /// <param name="flag"></param>
        /// <param name="name"></param>
        private void ReadingFile2_15Column(DataRow r, ref List<PDFTable> TableList, ref Boolean flag, ref string name)
        {
            if (flag == true)
            {
                try
                {
                    if (!String.IsNullOrEmpty(r.Field<string>("Column1")))
                    {
                        flag = false;
                    }
                    else if (!String.IsNullOrEmpty(r.Field<string>("Column4")))
                    {
                        flag = false;
                    }
                    else
                    {
                        if (!String.IsNullOrEmpty(r.Field<string>("Column7")))
                        {
                            TableList.Add(
                                new PDFTable
                                {
                                    Name = "REGULAR",
                                    Value =
                                        r.Field<string>("Column7")
                                                .Replace('(', '-')
                                                .Replace(')', ' ')
                                                .Replace(" ", string.Empty)
                                                .Replace(",", string.Empty)
                                                .Trim(),
                                    Name2 = "File2",
                                    Name3 = name
                                });
                        }

                        if (!String.IsNullOrEmpty(r.Field<string>("Column9")))
                        {
                            TableList.Add(
                                new PDFTable
                                {
                                    Name = "OVERTIME",
                                    Value =
                                        r.Field<string>("Column9")
                                                .Replace('(', '-')
                                                .Replace(')', ' ')
                                                .Replace(" ", string.Empty)
                                                .Replace(",", string.Empty)
                                                .Trim(),
                                    Name2 = "File2",
                                    Name3 = name
                                });
                        }
                        if (!String.IsNullOrEmpty(r.Field<string>("Column10")))
                        {
                            string inputString = r.Field<string>("Column10")
                                .Replace('(', '-')
                                .Replace(')', ' ')
                                .Replace(" ", string.Empty)
                                .Replace(",", string.Empty)
                                .Trim();
                            try
                            {
                                double Val = Convert.ToDouble(inputString);
                                if (!String.IsNullOrEmpty(r.Field<string>("Column11")))
                                    inputString = Val.ToString() +
                                        r.Field<string>("Column11")
                                            .Replace('(', '-')
                                            .Replace(')', ' ')
                                            .Replace(" ", string.Empty)
                                            .Replace(",", string.Empty)
                                            .Trim();
                                else
                                    inputString = Val.ToString();
                            }
                            catch (Exception ex)
                            {
                            }
                            string pattern = @"([0-9,.-]+)([A-Za-z]+)";
                            Match match = Regex.Match(inputString, pattern);

                            if (match.Success)
                            {
                                string numericPart = match.Groups[1].Value;
                                string textPart = match.Groups[2].Value;
                                TableList.Add(
                                    new PDFTable
                                    {
                                        Name = textPart.Replace(" ", "_").Trim(),
                                        Value = numericPart,
                                        Name2 = "File2",
                                        Name3 = name
                                    });
                            }
                        }
                        else if (!String.IsNullOrEmpty(r.Field<string>("Column11")))
                        {
                            string inputString = r.Field<string>("Column11")
                                .Replace('(', '-')
                                .Replace(')', ' ')
                                .Replace(" ", string.Empty)
                                .Replace(",", string.Empty)
                                .Trim();
                            string pattern = @"([0-9,.-]+)([A-Za-z]+)";
                            Match match = Regex.Match(inputString, pattern);

                            if (match.Success)
                            {
                                string numericPart = match.Groups[1].Value;
                                string textPart = match.Groups[2].Value;
                                TableList.Add(
                                    new PDFTable
                                    {
                                        Name = textPart.Replace(" ", "_").Trim(),
                                        Value = numericPart,
                                        Name2 = "File2",
                                        Name3 = name
                                    });
                            }
                            //else
                            //{
                            //    TableList.Add(new PDFTable { Name = r.Field<string>("Column9").Replace(" ", "_").Trim(), Value = r.Field<string>("Column9").Replace('(', '-').Replace(')', ' ').Replace(" ", "").Replace(",", "").Trim(), Name2 = "File2", Name3 = name });
                            //}
                        }
                    }
                }
                catch (Exception ex)
                {
                    logger.Error("PDFReadController - ReadingFile2_15Column - " + DateTime.Now + " - " + ex.Message.ToString());
                }

            }
            if (!String.IsNullOrEmpty(r.Field<string>("Column1")))
            {
                try
                {
                    if (r.Field<string>("Column1").ToLower().Contains("management"))
                    {
                        name = "MANAGEMENT";
                    }
                    else if (r.Field<string>("Column1").ToLower().Contains("general clerk"))
                    {
                        name = "GENERAL CLERK";
                    }
                    else if (r.Field<string>("Column1").ToLower().Contains("grocery"))
                    {
                        name = "GROCERY";
                    }
                    else if (r.Field<string>("Column1").ToLower().Contains("prepared foods"))
                    {
                        name = "PREPARED FOODS";
                    }
                    else if (r.Field<string>("Column1").ToLower().Contains("bakery"))
                    {
                        name = "BAKERY";
                    }
                    else if (r.Field<string>("Column1").ToLower().Contains("kitchen"))
                    {
                        name = "KITCHEN";
                    }
                    else if (r.Field<string>("Column1").ToLower().Contains("dairy"))
                    {
                        name = "DAIRY";
                    }
                    else if (r.Field<string>("Column1").ToLower().Contains("frozen foods"))
                    {
                        name = "FROZEN FOODS";
                    }
                    else if (r.Field<string>("Column1").ToLower().Contains("chesse"))
                    {
                        name = "CHESSE";
                    }
                    else if (r.Field<string>("Column1").ToLower().Contains("cashier"))
                    {
                        name = "CASHIER";
                    }
                    else if (r.Field<string>("Column1").ToLower().Contains("produce"))
                    {
                        name = "PRODUCE";
                    }
                    else if (r.Field<string>("Column1").ToLower().Contains("cheese"))
                    {
                        name = "CHEESE";
                    }
                    else if (r.Field<string>("Column1").ToLower().Contains("salad bar"))
                    {
                        name = "SALAD BAR";
                    }
                    else if (r.Field<string>("Column1").ToLower().Contains("maintenance"))
                    {
                        name = "MAINTENANCE";
                    }
                    else if (r.Field<string>("Column1").ToLower().Contains("meat"))
                    {
                        name = "MEAT";
                    }
                    else if (r.Field<string>("Column1").ToLower().Contains("delivery"))
                    {
                        name = "DELIVERY";
                    }
                    else if (r.Field<string>("Column1").ToLower().Contains("office"))
                    {
                        name = "OFFICE";
                    }
                    else if (r.Field<string>("Column1").ToLower().Contains("security"))
                    {
                        name = "SECURITY";
                    }
                    else if (r.Field<string>("Column1").ToLower().Contains("fish"))
                    {
                        name = "FISH";
                    }
                    else if (r.Field<string>("Column1").ToLower().Contains("office"))
                    {
                        name = "OFFICE";
                    }
                    else if (r.Field<string>("Column1").ToLower().Contains("deli"))
                    {
                        name = "DELI";
                    }
                    else if (r.Field<string>("Column1").ToLower().Contains("online shoppers"))
                    {
                        name = "ONLINE SHOPPERS";
                    }

                }
                catch (Exception ex)
                {

                    logger.Error("PDFReadController - ReadingFile2_15Column - " + DateTime.Now + " - " + ex.Message.ToString());
                }

                if (r.Field<string>("Column1").ToLower() == "department totals")
                {
                    try
                    {
                        if (!String.IsNullOrEmpty(r.Field<string>("Column7")))
                        {
                            TableList.Add(
                                new PDFTable
                                {
                                    Name = "REGULAR",
                                    Value =
                                        r.Field<string>("Column7")
                                                .Replace('(', ' ')
                                                .Replace(')', ' ')
                                                .Replace(",", string.Empty)
                                                .Trim(),
                                    Name2 = "File2",
                                    Name3 = name
                                });
                        }

                        if (!String.IsNullOrEmpty(r.Field<string>("Column9")))
                        {
                            TableList.Add(
                                new PDFTable
                                {
                                    Name = "OVERTIME",
                                    Value =
                                        r.Field<string>("Column9")
                                                .Replace('(', ' ')
                                                .Replace(')', ' ')
                                                .Replace(",", string.Empty)
                                                .Trim(),
                                    Name2 = "File2",
                                    Name3 = name
                                });
                        }
                        if (!String.IsNullOrEmpty(r.Field<string>("Column10")))
                        {
                            string inputString = r.Field<string>("Column10")
                                .Replace('(', '-')
                                .Replace(')', ' ')
                                .Replace(" ", string.Empty)
                                .Replace(",", string.Empty)
                                .Trim();
                            try
                            {
                                double Val = Convert.ToDouble(inputString);
                                if (!String.IsNullOrEmpty(r.Field<string>("Column11")))
                                    inputString = Val.ToString() +
                                        r.Field<string>("Column11")
                                            .Replace('(', '-')
                                            .Replace(')', ' ')
                                            .Replace(" ", string.Empty)
                                            .Replace(",", string.Empty)
                                            .Trim();
                                else
                                    inputString = Val.ToString();
                            }
                            catch (Exception ex)
                            {
                                logger.Error("PDFReadController - ReadingFile2_15Column - " + DateTime.Now + " - " + ex.Message.ToString());
                            }
                            string pattern = @"([0-9,.-]+)([A-Za-z]+)";
                            Match match = Regex.Match(inputString, pattern);

                            if (match.Success)
                            {
                                string numericPart = match.Groups[1].Value;
                                string textPart = match.Groups[2].Value;
                                TableList.Add(
                                    new PDFTable
                                    {
                                        Name = textPart.Replace(" ", "_").Trim(),
                                        Value = numericPart,
                                        Name2 = "File2",
                                        Name3 = name
                                    });
                            }
                        }
                        else if (!String.IsNullOrEmpty(r.Field<string>("Column11")))
                        {
                            string inputString = r.Field<string>("Column11")
                                .Replace('(', '-')
                                .Replace(')', ' ')
                                .Replace(" ", string.Empty)
                                .Replace(",", string.Empty)
                                .Trim();
                            string pattern = @"([0-9,.-]+)([A-Za-z]+)";
                            Match match = Regex.Match(inputString, pattern);

                            if (match.Success)
                            {
                                string numericPart = match.Groups[1].Value;
                                string textPart = match.Groups[2].Value;
                                TableList.Add(
                                    new PDFTable
                                    {
                                        Name = textPart.Replace(" ", "_").Trim(),
                                        Value = numericPart,
                                        Name2 = "File2",
                                        Name3 = name
                                    });
                            }
                            //else
                            //{
                            //    TableList.Add(new PDFTable { Name = r.Field<string>("Column9").Replace(" ", "_").Trim(), Value = r.Field<string>("Column9").Replace('(', '-').Replace(')', ' ').Replace(" ", "").Replace(",", "").Trim(), Name2 = "File2", Name3 = name });
                            //}
                        }

                    }
                    catch (Exception ex)
                    {
                        logger.Error("PDFReadController - ReadingFile2_15Column - " + DateTime.Now + " - " + ex.Message.ToString());
                    }

                    flag = true;
                }
            }
        }

        /// <summary>
        /// This class is used to ReadingFile2_15Column_2840.
        /// </summary>
        /// <param name="r"></param>
        /// <param name="TableList"></param>
        /// <param name="flag"></param>
        /// <param name="name"></param>
        private void ReadingFile2_15Column_2840(
            DataRow r,
            ref List<PDFTable> TableList,
            ref Boolean flag,
            ref string name)
        {
            if (flag == true)
            {
                try
                {
                    if (!String.IsNullOrEmpty(r.Field<string>("Column1")))
                    {
                        flag = false;
                    }
                    else if (!String.IsNullOrEmpty(r.Field<string>("Column4")))
                    {
                        flag = false;
                    }
                    else
                    {
                        if (!String.IsNullOrEmpty(r.Field<string>("Column8")))
                        {
                            TableList.Add(
                                new PDFTable
                                {
                                    Name = "REGULAR",
                                    Value =
                                        r.Field<string>("Column8")
                                                .Replace('(', '-')
                                                .Replace(')', ' ')
                                                .Replace(" ", string.Empty)
                                                .Replace(",", string.Empty)
                                                .Trim(),
                                    Name2 = "File2",
                                    Name3 = name
                                });
                        }

                        if (!String.IsNullOrEmpty(r.Field<string>("Column9")))
                        {
                            TableList.Add(
                                new PDFTable
                                {
                                    Name = "OVERTIME",
                                    Value =
                                        r.Field<string>("Column9")
                                                .Replace('(', '-')
                                                .Replace(')', ' ')
                                                .Replace(" ", string.Empty)
                                                .Replace(",", string.Empty)
                                                .Trim(),
                                    Name2 = "File2",
                                    Name3 = name
                                });
                        }
                        if (!String.IsNullOrEmpty(r.Field<string>("Column10")))
                        {
                            string inputString = r.Field<string>("Column10")
                                .Replace('(', '-')
                                .Replace(')', ' ')
                                .Replace(" ", string.Empty)
                                .Replace(",", string.Empty)
                                .Trim();
                            try
                            {
                                double Val = Convert.ToDouble(inputString);
                                if (!String.IsNullOrEmpty(r.Field<string>("Column11")))
                                    inputString = Val.ToString() +
                                        r.Field<string>("Column11")
                                            .Replace('(', '-')
                                            .Replace(')', ' ')
                                            .Replace(" ", string.Empty)
                                            .Replace(",", string.Empty)
                                            .Trim();
                                else
                                    inputString = Val.ToString();
                            }
                            catch (Exception ex)
                            {
                                logger.Error("PDFReadController - ReadingFile2_15Column_2840 - " + DateTime.Now + " - " + ex.Message.ToString());
                            }
                            string pattern = @"([0-9,.-]+)([A-Za-z]+)";
                            Match match = Regex.Match(inputString, pattern);

                            if (match.Success)
                            {
                                string numericPart = match.Groups[1].Value;
                                string textPart = match.Groups[2].Value;
                                TableList.Add(
                                    new PDFTable
                                    {
                                        Name = textPart.Replace(" ", "_").Trim(),
                                        Value = numericPart,
                                        Name2 = "File2",
                                        Name3 = name
                                    });
                            }
                        }
                        else if (!String.IsNullOrEmpty(r.Field<string>("Column11")))
                        {
                            string inputString = r.Field<string>("Column11")
                                .Replace('(', '-')
                                .Replace(')', ' ')
                                .Replace(" ", string.Empty)
                                .Replace(",", string.Empty)
                                .Trim();
                            string pattern = @"([0-9,.-]+)([A-Za-z]+)";
                            Match match = Regex.Match(inputString, pattern);

                            if (match.Success)
                            {
                                string numericPart = match.Groups[1].Value;
                                string textPart = match.Groups[2].Value;
                                TableList.Add(
                                    new PDFTable
                                    {
                                        Name = textPart.Replace(" ", "_").Trim(),
                                        Value = numericPart,
                                        Name2 = "File2",
                                        Name3 = name
                                    });
                            }
                            //else
                            //{
                            //    TableList.Add(new PDFTable { Name = r.Field<string>("Column9").Replace(" ", "_").Trim(), Value = r.Field<string>("Column9").Replace('(', '-').Replace(')', ' ').Replace(" ", "").Replace(",", "").Trim(), Name2 = "File2", Name3 = name });
                            //}
                        }
                    }
                }
                catch (Exception ex)
                {
                    logger.Error("PDFReadController - ReadingFile2_15Column_2840 - " + DateTime.Now + " - " + ex.Message.ToString());
                }

            }
            if (!String.IsNullOrEmpty(r.Field<string>("Column1")))
            {
                try
                {
                    if (r.Field<string>("Column1").ToLower().Contains("management"))
                    {
                        name = "MANAGEMENT";
                    }
                    else if (r.Field<string>("Column1").ToLower().Contains("general clerk"))
                    {
                        name = "GENERAL CLERK";
                    }
                    else if (r.Field<string>("Column1").ToLower().Contains("grocery"))
                    {
                        name = "GROCERY";
                    }
                    else if (r.Field<string>("Column1").ToLower().Contains("prepared foods"))
                    {
                        name = "PREPARED FOODS";
                    }
                    else if (r.Field<string>("Column1").ToLower().Contains("bakery"))
                    {
                        name = "BAKERY";
                    }
                    else if (r.Field<string>("Column1").ToLower().Contains("kitchen"))
                    {
                        name = "KITCHEN";
                    }
                    else if (r.Field<string>("Column1").ToLower().Contains("dairy"))
                    {
                        name = "DAIRY";
                    }
                    else if (r.Field<string>("Column1").ToLower().Contains("frozen foods"))
                    {
                        name = "FROZEN FOODS";
                    }
                    else if (r.Field<string>("Column1").ToLower().Contains("chesse"))
                    {
                        name = "CHESSE";
                    }
                    else if (r.Field<string>("Column1").ToLower().Contains("cashier"))
                    {
                        name = "CASHIER";
                    }
                    else if (r.Field<string>("Column1").ToLower().Contains("produce"))
                    {
                        name = "PRODUCE";
                    }
                    else if (r.Field<string>("Column1").ToLower().Contains("cheese"))
                    {
                        name = "CHEESE";
                    }
                    else if (r.Field<string>("Column1").ToLower().Contains("salad bar"))
                    {
                        name = "SALAD BAR";
                    }
                    else if (r.Field<string>("Column1").ToLower().Contains("maintenance"))
                    {
                        name = "MAINTENANCE";
                    }
                    else if (r.Field<string>("Column1").ToLower().Contains("meat"))
                    {
                        name = "MEAT";
                    }
                    else if (r.Field<string>("Column1").ToLower().Contains("delivery"))
                    {
                        name = "DELIVERY";
                    }
                    else if (r.Field<string>("Column1").ToLower().Contains("office"))
                    {
                        name = "OFFICE";
                    }
                    else if (r.Field<string>("Column1").ToLower().Contains("security"))
                    {
                        name = "SECURITY";
                    }
                    else if (r.Field<string>("Column1").ToLower().Contains("fish"))
                    {
                        name = "FISH";
                    }
                    else if (r.Field<string>("Column1").ToLower().Contains("office"))
                    {
                        name = "OFFICE";
                    }
                    else if (r.Field<string>("Column1").ToLower().Contains("deli"))
                    {
                        name = "DELI";
                    }
                    else if (r.Field<string>("Column1").ToLower().Contains("online shoppers"))
                    {
                        name = "ONLINE SHOPPERS";
                    }
                }
                catch (Exception ex)
                {
                    logger.Error("PDFReadController - ReadingFile2_15Column_2840 - " + DateTime.Now + " - " + ex.Message.ToString());
                }

                try
                {
                    if (r.Field<string>("Column1").ToLower() == "department totals")
                    {
                        if (!String.IsNullOrEmpty(r.Field<string>("Column8")))
                        {
                            TableList.Add(
                                new PDFTable
                                {
                                    Name = "REGULAR",
                                    Value =
                                        r.Field<string>("Column8")
                                                .Replace('(', ' ')
                                                .Replace(')', ' ')
                                                .Replace(",", string.Empty)
                                                .Trim(),
                                    Name2 = "File2",
                                    Name3 = name
                                });
                        }

                        if (!String.IsNullOrEmpty(r.Field<string>("Column9")))
                        {
                            TableList.Add(
                                new PDFTable
                                {
                                    Name = "OVERTIME",
                                    Value =
                                        r.Field<string>("Column9")
                                                .Replace('(', ' ')
                                                .Replace(')', ' ')
                                                .Replace(",", string.Empty)
                                                .Trim(),
                                    Name2 = "File2",
                                    Name3 = name
                                });
                        }
                        if (!String.IsNullOrEmpty(r.Field<string>("Column10")))
                        {
                            string inputString = r.Field<string>("Column10")
                                .Replace('(', '-')
                                .Replace(')', ' ')
                                .Replace(" ", string.Empty)
                                .Replace(",", string.Empty)
                                .Trim();
                            try
                            {
                                double Val = Convert.ToDouble(inputString);
                                if (!String.IsNullOrEmpty(r.Field<string>("Column11")))
                                    inputString = Val.ToString() +
                                        r.Field<string>("Column11")
                                            .Replace('(', '-')
                                            .Replace(')', ' ')
                                            .Replace(" ", string.Empty)
                                            .Replace(",", string.Empty)
                                            .Trim();
                                else
                                    inputString = Val.ToString();
                            }
                            catch (Exception ex)
                            {
                                logger.Error("PDFReadController - ReadingFile2_15Column_2840 - " + DateTime.Now + " - " + ex.Message.ToString());
                            }
                            string pattern = @"([0-9,.-]+)([A-Za-z]+)";
                            Match match = Regex.Match(inputString, pattern);

                            if (match.Success)
                            {
                                string numericPart = match.Groups[1].Value;
                                string textPart = match.Groups[2].Value;
                                TableList.Add(
                                    new PDFTable
                                    {
                                        Name = textPart.Replace(" ", "_").Trim(),
                                        Value = numericPart,
                                        Name2 = "File2",
                                        Name3 = name
                                    });
                            }
                        }
                        else if (!String.IsNullOrEmpty(r.Field<string>("Column11")))
                        {
                            string inputString = r.Field<string>("Column11")
                                .Replace('(', '-')
                                .Replace(')', ' ')
                                .Replace(" ", string.Empty)
                                .Replace(",", string.Empty)
                                .Trim();
                            string pattern = @"([0-9,.-]+)([A-Za-z]+)";
                            Match match = Regex.Match(inputString, pattern);

                            if (match.Success)
                            {
                                string numericPart = match.Groups[1].Value;
                                string textPart = match.Groups[2].Value;
                                TableList.Add(
                                    new PDFTable
                                    {
                                        Name = textPart.Replace(" ", "_").Trim(),
                                        Value = numericPart,
                                        Name2 = "File2",
                                        Name3 = name
                                    });
                            }
                            //else
                            //{
                            //    TableList.Add(new PDFTable { Name = r.Field<string>("Column9").Replace(" ", "_").Trim(), Value = r.Field<string>("Column9").Replace('(', '-').Replace(')', ' ').Replace(" ", "").Replace(",", "").Trim(), Name2 = "File2", Name3 = name });
                            //}
                        }

                        flag = true;
                    }
                }
                catch (Exception ex)
                {
                    logger.Error("PDFReadController - ReadingFile2_15Column_2840 - " + DateTime.Now + " - " + ex.Message.ToString());
                }

            }
        }

        /// <summary>
        /// This class is used to ReadingFile2_16 Column.
        /// </summary>
        /// <param name="r"></param>
        /// <param name="TableList"></param>
        /// <param name="flag"></param>
        /// <param name="name"></param>
        private void ReadingFile2_16Column(DataRow r, ref List<PDFTable> TableList, ref Boolean flag, ref string name)
        {
            if (flag == true)
            {
                try
                {
                    if (!String.IsNullOrEmpty(r.Field<string>("Column1")))
                    {
                        flag = false;
                    }
                    else if (!String.IsNullOrEmpty(r.Field<string>("Column4")))
                    {
                        flag = false;
                    }
                    else
                    {
                        if (!String.IsNullOrEmpty(r.Field<string>("Column8")))
                        {
                            TableList.Add(
                                new PDFTable
                                {
                                    Name = "REGULAR",
                                    Value =
                                        r.Field<string>("Column8")
                                                .Replace('(', '-')
                                                .Replace(')', ' ')
                                                .Replace(" ", string.Empty)
                                                .Replace(",", string.Empty)
                                                .Trim(),
                                    Name2 = "File2",
                                    Name3 = name
                                });
                        }
                        if (!String.IsNullOrEmpty(r.Field<string>("Column10")))
                        {
                            TableList.Add(
                                new PDFTable
                                {
                                    Name = "OVERTIME",
                                    Value =
                                        r.Field<string>("Column10")
                                                .Replace('(', '-')
                                                .Replace(')', ' ')
                                                .Replace(" ", string.Empty)
                                                .Replace(",", string.Empty)
                                                .Trim(),
                                    Name2 = "File2",
                                    Name3 = name
                                });
                        }
                        if (!String.IsNullOrEmpty(r.Field<string>("Column11")))
                        {
                            string inputString = r.Field<string>("Column11")
                                .Replace('(', '-')
                                .Replace(')', ' ')
                                .Replace(" ", string.Empty)
                                .Replace(",", string.Empty)
                                .Trim();
                            try
                            {
                                double Val = Convert.ToDouble(inputString);
                                if (!String.IsNullOrEmpty(r.Field<string>("Column12")))
                                    inputString = Val.ToString() +
                                        r.Field<string>("Column12")
                                            .Replace('(', '-')
                                            .Replace(')', ' ')
                                            .Replace(" ", string.Empty)
                                            .Replace(",", string.Empty)
                                            .Trim();
                                else
                                    inputString = Val.ToString();
                            }
                            catch (Exception ex)
                            {
                            }
                            string pattern = @"([0-9,.-]+)([A-Za-z]+)";
                            Match match = Regex.Match(inputString, pattern);

                            if (match.Success)
                            {
                                string numericPart = match.Groups[1].Value;
                                string textPart = match.Groups[2].Value;
                                TableList.Add(
                                    new PDFTable
                                    {
                                        Name = textPart.Replace(" ", "_").Trim(),
                                        Value = numericPart,
                                        Name2 = "File2",
                                        Name3 = name
                                    });
                            }
                        }
                        else if (!String.IsNullOrEmpty(r.Field<string>("Column12")))
                        {
                            string inputString = r.Field<string>("Column12")
                                .Replace('(', '-')
                                .Replace(')', ' ')
                                .Replace(" ", string.Empty)
                                .Replace(",", string.Empty)
                                .Trim();
                            string pattern = @"([0-9,.-]+)([A-Za-z]+)";
                            Match match = Regex.Match(inputString, pattern);

                            if (match.Success)
                            {
                                string numericPart = match.Groups[1].Value;
                                string textPart = match.Groups[2].Value;
                                TableList.Add(
                                    new PDFTable
                                    {
                                        Name = textPart.Replace(" ", "_").Trim(),
                                        Value = numericPart,
                                        Name2 = "File2",
                                        Name3 = name
                                    });
                            }
                            //else
                            //{
                            //    TableList.Add(new PDFTable { Name = r.Field<string>("Column9").Replace(" ", "_").Trim(), Value = r.Field<string>("Column9").Replace('(', '-').Replace(')', ' ').Replace(" ", "").Replace(",", "").Trim(), Name2 = "File2", Name3 = name });
                            //}
                        }
                    }
                }
                catch (Exception ex)
                {

                    logger.Error("PDFReadController - ReadingFile2_16Column - " + DateTime.Now + " - " + ex.Message.ToString());
                }

            }
            if (!String.IsNullOrEmpty(r.Field<string>("Column1")))
            {
                try
                {
                    if (r.Field<string>("Column1").ToLower().Contains("management"))
                    {
                        name = "MANAGEMENT";
                    }
                    else if (r.Field<string>("Column1").ToLower().Contains("general clerk"))
                    {
                        name = "GENERAL CLERK";
                    }
                    else if (r.Field<string>("Column1").ToLower().Contains("grocery"))
                    {
                        name = "GROCERY";
                    }
                    else if (r.Field<string>("Column1").ToLower().Contains("prepared foods"))
                    {
                        name = "PREPARED FOODS";
                    }
                    else if (r.Field<string>("Column1").ToLower().Contains("bakery"))
                    {
                        name = "BAKERY";
                    }
                    else if (r.Field<string>("Column1").ToLower().Contains("kitchen"))
                    {
                        name = "KITCHEN";
                    }
                    else if (r.Field<string>("Column1").ToLower().Contains("dairy"))
                    {
                        name = "DAIRY";
                    }
                    else if (r.Field<string>("Column1").ToLower().Contains("frozen foods"))
                    {
                        name = "FROZEN FOODS";
                    }
                    else if (r.Field<string>("Column1").ToLower().Contains("chesse"))
                    {
                        name = "CHESSE";
                    }
                    else if (r.Field<string>("Column1").ToLower().Contains("cashier"))
                    {
                        name = "CASHIER";
                    }
                    else if (r.Field<string>("Column1").ToLower().Contains("produce"))
                    {
                        name = "PRODUCE";
                    }
                    else if (r.Field<string>("Column1").ToLower().Contains("cheese"))
                    {
                        name = "CHEESE";
                    }
                    else if (r.Field<string>("Column1").ToLower().Contains("salad bar"))
                    {
                        name = "SALAD BAR";
                    }
                    else if (r.Field<string>("Column1").ToLower().Contains("maintenance"))
                    {
                        name = "MAINTENANCE";
                    }
                    else if (r.Field<string>("Column1").ToLower().Contains("meat"))
                    {
                        name = "MEAT";
                    }
                    else if (r.Field<string>("Column1").ToLower().Contains("delivery"))
                    {
                        name = "DELIVERY";
                    }
                    else if (r.Field<string>("Column1").ToLower().Contains("office"))
                    {
                        name = "OFFICE";
                    }
                    else if (r.Field<string>("Column1").ToLower().Contains("security"))
                    {
                        name = "SECURITY";
                    }
                    else if (r.Field<string>("Column1").ToLower().Contains("fish"))
                    {
                        name = "FISH";
                    }
                    else if (r.Field<string>("Column1").ToLower().Contains("office"))
                    {
                        name = "OFFICE";
                    }
                    else if (r.Field<string>("Column1").ToLower().Contains("deli"))
                    {
                        name = "DELI";
                    }
                    else if (r.Field<string>("Column1").ToLower().Contains("online shoppers"))
                    {
                        name = "ONLINE SHOPPERS";
                    }

                }
                catch (Exception ex)
                {
                    logger.Error("PDFReadController - ReadingFile2_16Column - " + DateTime.Now + " - " + ex.Message.ToString());
                }

                try
                {
                    if (r.Field<string>("Column1").ToLower() == "department totals")
                    {
                        if (!String.IsNullOrEmpty(r.Field<string>("Column8")))
                        {
                            TableList.Add(
                                new PDFTable
                                {
                                    Name = "REGULAR",
                                    Value =
                                        r.Field<string>("Column8")
                                                .Replace('(', ' ')
                                                .Replace(')', ' ')
                                                .Replace(",", string.Empty)
                                                .Trim(),
                                    Name2 = "File2",
                                    Name3 = name
                                });
                        }
                        if (!String.IsNullOrEmpty(r.Field<string>("Column10")))
                        {
                            TableList.Add(
                                new PDFTable
                                {
                                    Name = "OVERTIME",
                                    Value =
                                        r.Field<string>("Column10")
                                                .Replace('(', ' ')
                                                .Replace(')', ' ')
                                                .Replace(",", string.Empty)
                                                .Trim(),
                                    Name2 = "File2",
                                    Name3 = name
                                });
                        }
                        if (!String.IsNullOrEmpty(r.Field<string>("Column11")))
                        {
                            string inputString = r.Field<string>("Column11")
                                .Replace('(', '-')
                                .Replace(')', ' ')
                                .Replace(" ", string.Empty)
                                .Replace(",", string.Empty)
                                .Trim();
                            try
                            {
                                double Val = Convert.ToDouble(inputString);
                                if (!String.IsNullOrEmpty(r.Field<string>("Column12")))
                                    inputString = Val.ToString() +
                                        r.Field<string>("Column12")
                                            .Replace('(', '-')
                                            .Replace(')', ' ')
                                            .Replace(" ", string.Empty)
                                            .Replace(",", string.Empty)
                                            .Trim();
                                else
                                    inputString = Val.ToString();
                            }
                            catch (Exception ex)
                            {
                                logger.Error("PDFReadController - ReadingFile2_16Column - " + DateTime.Now + " - " + ex.Message.ToString());
                            }
                            string pattern = @"([0-9,.-]+)([A-Za-z]+)";
                            Match match = Regex.Match(inputString, pattern);

                            if (match.Success)
                            {
                                string numericPart = match.Groups[1].Value;
                                string textPart = match.Groups[2].Value;
                                TableList.Add(
                                    new PDFTable
                                    {
                                        Name = textPart.Replace(" ", "_").Trim(),
                                        Value = numericPart,
                                        Name2 = "File2",
                                        Name3 = name
                                    });
                            }
                        }
                        else if (!String.IsNullOrEmpty(r.Field<string>("Column12")))
                        {
                            string inputString = r.Field<string>("Column12")
                                .Replace('(', '-')
                                .Replace(')', ' ')
                                .Replace(" ", string.Empty)
                                .Replace(",", string.Empty)
                                .Trim();
                            string pattern = @"([0-9,.-]+)([A-Za-z]+)";
                            Match match = Regex.Match(inputString, pattern);

                            if (match.Success)
                            {
                                string numericPart = match.Groups[1].Value;
                                string textPart = match.Groups[2].Value;
                                TableList.Add(
                                    new PDFTable
                                    {
                                        Name = textPart.Replace(" ", "_").Trim(),
                                        Value = numericPart,
                                        Name2 = "File2",
                                        Name3 = name
                                    });
                            }
                            //else
                            //{
                            //    TableList.Add(new PDFTable { Name = r.Field<string>("Column9").Replace(" ", "_").Trim(), Value = r.Field<string>("Column9").Replace('(', '-').Replace(')', ' ').Replace(" ", "").Replace(",", "").Trim(), Name2 = "File2", Name3 = name });
                            //}
                        }

                        flag = true;
                    }
                }
                catch (Exception ex)
                {

                    logger.Error("PDFReadController - ReadingFile2_16Column - " + DateTime.Now + " - " + ex.Message.ToString());
                }


            }
        }

        /// <summary>
        /// This Method  is for check Exisrence.
        /// </summary>
        /// <param name="postedFile"></param>
        /// <param name="Redio"></param>
        /// <returns></returns>
        public ActionResult CheckExistence(HttpPostedFileBase postedFile, string Redio)
        {
            int Check = 0;
            if (postedFile != null)
            {
                try
                {
                    FileInfo fi = new FileInfo(postedFile.FileName);
                    if (fi.Extension.ToLower() == ".pdf")
                    {
                        string path = Server.MapPath("~/Userfiles/PayrollFile/");
                        if (!Directory.Exists(path))
                        {
                            Directory.CreateDirectory(path);
                        }
                        var FileName = DateTime.Now.Ticks.ToString() + "_" + postedFile.FileName; // DateTime.Now.Ticks.ToString() + "_" +
                        if (System.IO.File.Exists(path + System.IO.Path.GetFileName(FileName)))
                        {
                            System.IO.File.Delete(path + System.IO.Path.GetFileName(FileName));
                        }

                        postedFile.SaveAs(path + System.IO.Path.GetFileName(FileName));
                        Session["PDFFile"] = FileName;
                        //if (Redio == "Old")
                        //{
                        //    if (Convert.ToInt32(Session["storeid"].ToString()) == 9)
                        //    {
                        //        Check = ReadPayrollSummaryCheckFile(FileName);
                        //        //Check = ReadPayrollCheckFile_NewStructer(FileName);
                        //    }
                        //    else
                        //    {
                        //        //Check = ReadPayrollCheckFile_NewStructer(FileName);
                        //        Check = ReadPDFCheckFile(FileName);
                        //    }
                        //}
                        //else
                        //{
                        if (Convert.ToInt32(Session["storeid"].ToString()) == 9)
                        {
                            Check = ReadPayrollSummaryCheckFile(FileName);
                        }
                        else
                        {
                            Check = NewReadPayrollFile(FileName);
                        }
                        //}
                        if (System.IO.File.Exists(path + System.IO.Path.GetFileName(FileName)))
                        {
                            System.IO.File.Delete(path + System.IO.Path.GetFileName(FileName));
                        }
                    }
                    else
                    {
                        Check = 0;
                    }
                }
                catch (Exception ex)
                {
                    logger.Error("PDFReadController - CheckExistence - " + DateTime.Now + " - " + ex.Message.ToString());
                }

            }
            else
            {
                Check = 0;
            }
            return Json(Check);
        }

        // GET: PDFRead
        /// <summary>
        ///  This Method  is return with payroll filles.
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "Administrator,ViewPayrollReports")]
        public ActionResult Create()
        {
            ViewBag.Title = "Payroll Files - Synthesis";
            try
            {
                if (Session["storeid"] == null)
                {
                    Session["storeid"] = "0";
                }
                int StoreID = Convert.ToInt32(Session["storeid"].ToString());
                if (StoreID == 0)
                {
                    ViewBag.storeid = StoreID;
                    return View();
                }
            }
            catch (Exception ex)
            {
                logger.Error("PDFReadController - Create - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return View();
        }

        /// <summary>
        ///  This Method  is for create PayrollReport.
        /// </summary>
        /// <param name="postedFile"></param>
        /// <param name="FileFormat"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(HttpPostedFileBase postedFile, string FileFormat)
        {
            try
            {
                if (postedFile != null)
                {
                    FileInfo fi = new FileInfo(postedFile.FileName);
                    if (fi.Extension.ToLower() == ".pdf")
                    {
                        string path = Server.MapPath("~/Userfiles/PayrollFile/");
                        if (!Directory.Exists(path))
                        {
                            Directory.CreateDirectory(path);
                        }
                        var FileName = DateTime.Now.Ticks.ToString() + "_" + postedFile.FileName; // DateTime.Now.Ticks.ToString() + "_" +
                        if (System.IO.File.Exists(path + System.IO.Path.GetFileName(FileName)))
                        {
                            System.IO.File.Delete(path + System.IO.Path.GetFileName(FileName));
                        }

                        postedFile.SaveAs(path + System.IO.Path.GetFileName(FileName));
                        Session["PDFFile"] = FileName;
                        PayrollReport obj = new PayrollReport();
                        obj.FileName = FileName;
                        obj.StoreId = Convert.ToInt32(Session["storeid"].ToString());
                        obj.UploadDate = DateTime.Today;
                        obj.IsRead = false;
                        obj.IsSync = 0;
                        //if (Convert.ToInt32(Session["storeid"].ToString()) == 9)
                        //{
                        //    obj.FIleNo = 1;
                        //}
                        //obj.FIleNo = 1;
                        //This Db class is used for Add payroll reports.
                        _PDFReadRepository.AddPayrollReports(obj);
                        Session["PayrollReportId"] = obj.PayrollReportId;
                        //if (FileFormat == "Old")
                        //{
                        //    if (Convert.ToInt32(Session["storeid"].ToString()) == 9)
                        //    {
                        //        ReadPayrollSummary(FileName, obj.PayrollReportId);
                        //    }
                        //    else
                        //    {
                        //        ReadPDF(FileName, obj.PayrollReportId);
                        //    }
                        //}
                        //else
                        //{
                        //    if (Convert.ToInt32(Session["storeid"].ToString()) == 9)
                        //    {
                        //        ReadPayrollSummary(FileName, obj.PayrollReportId);
                        //    }
                        //    else
                        //    {
                        //        ReadPayroll_NewStructer(FileName, obj.PayrollReportId);
                        //    }
                        //}                                                                                                    
                        //if (Convert.ToInt32(Session["storeid"].ToString()) == 9)
                        //{
                        //    ReadPayrollSummary(FileName, obj.PayrollReportId);
                        //}
                        ViewBag.Message = "File uploaded successfully.";
                        TempData["PDFFile"] = "File uploaded successfully.";
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Please Upload PDF Format Files Only";
                    }
                }
                else
                {
                    TempData["ErrorMessage"] = "PDF Not Uploaded,Please Upload it First";
                }
            }
            catch (Exception ex)
            {
                logger.Error("PDFReadController - Create - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return RedirectToAction("Create", "PDFRead");
        }

        /// <summary>
        /// This Method  is for delete PayrollReports.

        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public ActionResult Delete(int Id = 0)
        {
            try
            {
                //This Db class is used for remove payroll reports.

                var FileName = _PDFReadRepository.ReomvePayrollReports(Id);
                ActivityLog ActLog = new ActivityLog();
                ActLog.Action = 3;

                var UserName = System.Web.HttpContext.Current.User.Identity.Name;

                //This Db class is used for get user firstname.
                ActLog.Comment = "Payroll PDF" + FileName + " Deleted by " + _CommonRepository.getUserFirstName(UserName) + " on " + AdminSiteConfiguration.GetDate(AdminSiteConfiguration.GetEasternTime(DateTime.Now).ToString());
                //This  class is used for Activity Log Insert.
                _ActivityLogRepository.ActivityLogInsert(ActLog);
                //new PayrollReportBAL().DeletePayrollDetail(Id);
                ViewBag.DeleteMessage = "Payroll data deleted successfully.";
            }
            catch (Exception ex)
            {
                logger.Error("PDFReadController - Delete - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return null;
        }

        /// <summary>
        /// This method is Download payroll report.
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public ActionResult DownloadFile(string filePath)
        {
            try
            {
                string fullName = Server.MapPath("~/userfiles/PayrollFile/" + filePath);
                if (System.IO.File.Exists(fullName))
                {
                    byte[] fileBytes = GetFile(fullName);
                    return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Pdf);
                }
                else
                {
                    ViewBag.ErrorMessage = "PDF File Path Not Found..";
                    return View("Create");
                }
            }
            catch (Exception ex)
            {
                logger.Error("PDFReadController - DownloadFile - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return View("Create");
        }

        /// <summary>
        /// This method is get details of payroll report.
        /// </summary>
        /// <param name="FileName"></param>
        /// <param name="PayrollReportId"></param>
        /// <returns></returns>
        public ActionResult GetDetail(string FileName, int PayrollReportId)
        {
            try
            {
                if (FileName != string.Empty)
                {
                    int PayrollId = 0;

                    string FilePath = Server.MapPath("~/userfiles/PayrollFile/" + FileName);
                    List<PDFTable> TableList = new List<PDFTable>();
                    List<PDFTable> TempTable = new List<PDFTable>();
                    List<PDFTable> TempTable1 = new List<PDFTable>();

                    PdfReader reader = new PdfReader(FilePath);
                    int intPageNum = reader.NumberOfPages;
                    string StartDate = string.Empty;
                    string EndDate = string.Empty;
                    string EndCheckDate = string.Empty;
                    string[] words;
                    int FileNo = 0;
                    string PeriodEndDate = string.Empty;
                    string CheckDate = string.Empty;
                    string Transaction = string.Empty;

                    StringBuilder Reader = new StringBuilder();
                    if (Convert.ToInt32(Session["storeid"].ToString()) == 9)
                    {
                        try
                        {
                            for (int i = 1; i <= intPageNum; i++)
                            {
                                string text = PdfTextExtractor.GetTextFromPage(
                                    reader,
                                    i,
                                    new LocationTextExtractionStrategy());
                                int DVFlg = 0;
                                bool blnTotal = false;
                                bool blnTotal1 = false;
                                if (text.Contains("Payroll Distributed Summary Report"))
                                {
                                    FileNo = 1;
                                }
                                else if (text.Contains("Cash Requirements Statement"))
                                {
                                    FileNo = 2;
                                }
                                if (PeriodEndDate == string.Empty)
                                {
                                    if (text.Contains("Period Ending:") ||
                                        text.Contains("Check Date:") ||
                                        text.Contains("Transaction:"))
                                    {
                                        words = text.Split('\n');
                                        for (int j = 0, len = words.Length; j < len; j++)
                                        {
                                            var LineData = Encoding.UTF8.GetString(Encoding.UTF8.GetBytes(words[j]));
                                            if (LineData.Contains("Period Ending:"))
                                            {
                                                string[] DateSplitData = LineData.Split(':');
                                                if (DateSplitData.Count() > 0)
                                                {
                                                    PeriodEndDate = DateSplitData[1];
                                                    string[] DateFormate = PeriodEndDate.Split('/');
                                                    if (DateFormate.Count() == 3)
                                                    {
                                                        var Month = DateFormate[0].Trim().Length == 1
                                                            ? "0" + DateFormate[0].Trim()
                                                            : DateFormate[0].Trim();
                                                        PeriodEndDate = DateFormate[2] + "-" + Month + "-" + DateFormate[1];
                                                    }
                                                }
                                            }
                                            if (LineData.Contains("Check Date:"))
                                            {
                                                string[] DateSplitData = LineData.Split(':');
                                                if (DateSplitData.Count() > 0)
                                                {
                                                    if (FileNo == 2)
                                                    {
                                                        CheckDate = DateSplitData[1];
                                                    }
                                                    else
                                                    {
                                                        CheckDate = DateSplitData[2];
                                                    }
                                                    string[] DateFormate = CheckDate.Split('/');
                                                    if (DateFormate.Count() == 3)
                                                    {
                                                        var Month = DateFormate[0].Trim().Length == 1
                                                            ? "0" + DateFormate[0].Trim()
                                                            : DateFormate[0].Trim();
                                                        CheckDate = DateFormate[2].Trim() +
                                                            "-" +
                                                            Month +
                                                            "-" +
                                                            DateFormate[1].Trim();
                                                    }
                                                }
                                            }
                                            if (LineData.Contains("Transaction"))
                                            {
                                                string[] DateSplitData = LineData.Split(':');
                                                if (DateSplitData.Count() > 0)
                                                {
                                                    Transaction = DateSplitData[1];
                                                }
                                            }
                                        }
                                    }
                                }

                                if (FileNo == 1)
                                {
                                    if (text.Contains("Dept :"))
                                    {
                                        DVFlg = 1;
                                    }
                                    if (DVFlg == 1)
                                    {
                                        words = text.Split('\n');
                                        for (int j = 0, len = words.Length; j < len; j++)
                                        {
                                            var LineData = Encoding.UTF8.GetString(Encoding.UTF8.GetBytes(words[j]));
                                            if (LineData.Contains("Dept :"))
                                            {
                                                TableList.Add(new PDFTable { Name = LineData });
                                                TempTable = new List<PDFTable>();
                                            }
                                            if (LineData.Contains("Gross Earnings"))
                                            {
                                                var PreLine = Encoding.UTF8.GetString(Encoding.UTF8.GetBytes(words[j - 1]));
                                                //TableList.Add(new PDFTable { Name = PreLine, Value = "", Name2 = "Gross Earnings", Value2 = "", Name3 = "Amount", Value3 = "", Name4 = "Voluntary Deductions", Value4 = "", Name5 = "Amount", Value5 = "" });
                                                TempTable.Add(
                                                    new PDFTable
                                                    {
                                                        Name = PreLine,
                                                        Value = string.Empty,
                                                        Name2 = "Gross Earnings",
                                                        Value2 = string.Empty,
                                                        Name3 = "Amount",
                                                        Value3 = string.Empty,
                                                        Name4 = "Voluntary Deductions",
                                                        Value4 = string.Empty,
                                                        Name5 = "Amount",
                                                        Value5 = string.Empty
                                                    });
                                                blnTotal = false;
                                            }
                                            else
                                            {
                                                var TempData = TempTable;
                                                if (TempTable.Count() > 0)
                                                {
                                                    string[] authorsList = LineData.Trim().Split(' ');
                                                    int StringCount = authorsList.Count();
                                                    var TempValues = TempData.FirstOrDefault();
                                                    string[] bb = GetFinalArr(authorsList);
                                                    if (bb.Length > 0)
                                                    {
                                                        if (!char.IsDigit(bb[0].Trim().ToArray()[0]) && blnTotal == false)
                                                        {
                                                            if (!bb[0].ToString().Contains("Medical Pre-Tax"))
                                                            {
                                                                TableList.Add(
                                                                    new PDFTable
                                                                    {
                                                                        Name = TempValues.Name,
                                                                        Value = string.Empty,
                                                                        Name2 = TempValues.Name2,
                                                                        Value2 = bb[0],
                                                                        Name3 = TempValues.Name3,
                                                                        Value3 = (bb.Length >= 3) ? bb[2] : "0"
                                                                    });
                                                            }
                                                        }
                                                        else
                                                        {
                                                            blnTotal = true;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        blnTotal = true;
                                                    }
                                                    if (LineData.Contains("Service Fee"))
                                                    {
                                                        TableList.Add(
                                                            new PDFTable
                                                            {
                                                                Name = TempValues.Name,
                                                                Value = string.Empty,
                                                                Name2 = TempValues.Name2,
                                                                Value2 = bb[0],
                                                                Name3 = TempValues.Name3,
                                                                Value3 = bb[1]
                                                            });
                                                        TempTable = new List<PDFTable>();
                                                        DVFlg = 0;
                                                    }
                                                }
                                            }
                                        }
                                    }

                                    if (text.Contains("Company Total"))
                                    {
                                        TempTable = new List<PDFTable>();
                                        words = text.Split('\n');
                                        decimal Amt = 0;
                                        decimal AmtVol = 0;
                                        for (int j = 0, len = words.Length; j < len; j++)
                                        {
                                            var LineData = Encoding.UTF8.GetString(Encoding.UTF8.GetBytes(words[j]));
                                            if (LineData.Contains("Company Total"))
                                            {
                                                TableList.Add(new PDFTable { Name = LineData });
                                            }
                                            if (LineData.Contains("Employee Taxes Amount"))
                                            {
                                                var PreLine = Encoding.UTF8.GetString(Encoding.UTF8.GetBytes(words[j - 1]));
                                                TempTable = new List<PDFTable>();
                                                TempTable1 = new List<PDFTable>();
                                                //TableList.Add(new PDFTable { Name = PreLine, Value = "", Name2 = "Employee Taxes", Value2 = "", Name3 = "Amount", Value3 = "", Name4 = "Employer Taxes", Value4 = "", Name5 = "Amount", Value5 = "" });
                                                TempTable.Add(
                                                    new PDFTable
                                                    {
                                                        Name = PreLine,
                                                        Value = string.Empty,
                                                        Name2 = "Employee Taxes",
                                                        Value2 = string.Empty,
                                                        Name3 = "Amount",
                                                        Value3 = string.Empty,
                                                        Name4 = "Employer Taxes",
                                                        Value4 = string.Empty,
                                                        Name5 = "Amount",
                                                        Value5 = string.Empty
                                                    });
                                                TempTable1.Add(
                                                    new PDFTable
                                                    {
                                                        Name = PreLine,
                                                        Value = string.Empty,
                                                        Name2 = "Voluntary Deductions",
                                                        Value2 = string.Empty,
                                                        Name3 = "Amount",
                                                        Value3 = string.Empty
                                                    });
                                                blnTotal = false;
                                                blnTotal1 = false;
                                            }
                                            else if (LineData.Contains("Net Pay"))
                                            {
                                                TempTable = new List<PDFTable>();
                                                TempTable1 = new List<PDFTable>();
                                                blnTotal = false;
                                                blnTotal1 = false;
                                            }
                                            else if (LineData.Contains("Employer Taxes Amount"))
                                            {
                                                var PreLine = Encoding.UTF8.GetString(Encoding.UTF8.GetBytes(words[j - 1]));
                                                TempTable = new List<PDFTable>();
                                                //TableList.Add(new PDFTable { Name = "Company Total", Value = "", Name2 = "Employer Taxes", Value2 = "", Name3 = "Amount", Value3 = "", Name4 = "Employer Taxes", Value4 = "", Name5 = "Amount", Value5 = "" });
                                                TempTable.Add(
                                                    new PDFTable
                                                    {
                                                        Name = "Company Total",
                                                        Value = string.Empty,
                                                        Name2 = "Employer Taxes",
                                                        Value2 = string.Empty,
                                                        Name3 = "Amount",
                                                        Value3 = string.Empty,
                                                        Name4 = "Employer Taxes",
                                                        Value4 = string.Empty,
                                                        Name5 = "Amount",
                                                        Value5 = string.Empty
                                                    });
                                                Amt = 0;


                                                //Voluntary Deduction
                                                string[] authorsList = LineData.Trim().Split(' ');
                                                string[] bb = GetFinalArr(authorsList);
                                                if (bb.Length > 2 && blnTotal1 == false)
                                                {
                                                    var DigitLine = bb[1].Replace("$", string.Empty)
                                                        .Replace(" ", string.Empty)
                                                        .Replace(".", string.Empty)
                                                        .Replace(",", string.Empty)
                                                        .Replace("(", string.Empty)
                                                        .Replace(")", string.Empty)
                                                        .TrimStart(new Char[] { '0' });
                                                    if (DigitLine.Trim().All(s => char.IsDigit(s)))
                                                    {
                                                        var TempData1 = TempTable1;
                                                        var TempValues1 = TempData1.FirstOrDefault();

                                                        if (bb[0].ToString().Contains("Spread of Hours"))
                                                        {
                                                            int index = Array.IndexOf(bb, "100.00%");
                                                            var list = new List<string>(bb);
                                                            list.RemoveRange(0, index + 1);
                                                            //for (int k=0;k<= index-1;k++)
                                                            //{
                                                            //    list.Remove(k);
                                                            //}
                                                            bb = list.ToArray();
                                                            bb = GetFinalArr(bb);
                                                        }
                                                        string aa = bb[0].Substring(bb[0].ToString().IndexOf("%") + 1)
                                                            .Trim();
                                                        TableList.Add(
                                                            new PDFTable
                                                            {
                                                                Name = TempValues1.Name,
                                                                Value = string.Empty,
                                                                Name2 = TempValues1.Name2,
                                                                Value2 = aa.Trim(),
                                                                Name3 = TempValues1.Name3,
                                                                Value3 = bb[1]
                                                            });
                                                        if (bb[1].ToString().Contains("("))
                                                        {
                                                            string Valuestr = bb[1].ToString()
                                                                .Replace("$", string.Empty)
                                                                .Replace("(", string.Empty)
                                                                .Replace(")", string.Empty)
                                                                .Replace(",", string.Empty);
                                                            decimal Value = Convert.ToDecimal(Valuestr);
                                                            AmtVol += (-Value);
                                                        }
                                                        else
                                                        {
                                                            string Valuestr = bb[1].ToString()
                                                                .Replace("$", string.Empty)
                                                                .Replace(",", string.Empty);
                                                            decimal Value = Convert.ToDecimal(Valuestr);
                                                            AmtVol += Value;
                                                        }
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                var TempData = TempTable;
                                                if (TempTable.Count() > 0 || TempTable1.Count() > 0)
                                                {
                                                    string[] authorsList = LineData.Trim().Split(' ');
                                                    int StringCount = authorsList.Count();
                                                    var TempValues = TempData.FirstOrDefault();
                                                    Array.Reverse(authorsList);
                                                    if (LineData.Trim().EndsWith("%"))
                                                    {
                                                        authorsList = authorsList.Where((item, index) => index != 0)
                                                            .ToArray();
                                                        blnTotal = false;
                                                    }
                                                    string[] bb = GetFinalArrRev(authorsList);
                                                    if (bb.Length > 0)
                                                    {
                                                        if (blnTotal == false)
                                                        {
                                                            if (bb.Length != 2 || LineData.Trim().EndsWith("%"))
                                                            {
                                                                string str1 = bb[0].ToString()
                                                                    .Replace("$", string.Empty)
                                                                    .Replace("(", string.Empty)
                                                                    .Replace(")", string.Empty)
                                                                    .Replace(",", string.Empty)
                                                                    .Trim();
                                                                decimal V1 = Convert.ToDecimal(str1);
                                                                var PreLine = Encoding.UTF8
                                                                    .GetString(Encoding.UTF8.GetBytes(words[j + 1]));
                                                                string[] bb1 = GetFinalArrRev(PreLine.Split(' '));
                                                                if (bb1.Length == 1)
                                                                {
                                                                    string str2 = bb1[0].ToString()
                                                                        .Replace("$", string.Empty)
                                                                        .Replace("(", string.Empty)
                                                                        .Replace(")", string.Empty)
                                                                        .Replace(",", string.Empty)
                                                                        .Trim();
                                                                    decimal V2 = Convert.ToDecimal(str2);
                                                                    if (Amt == V2)
                                                                    {
                                                                        TempTable = new List<PDFTable>();
                                                                        blnTotal = true;
                                                                    }
                                                                }
                                                                if (Amt == V1)
                                                                {
                                                                    TempTable = new List<PDFTable>();
                                                                    blnTotal = true;
                                                                }
                                                                if (blnTotal == false)
                                                                {
                                                                    if (bb.Length > 1)
                                                                    {
                                                                        TableList.Add(
                                                                            new PDFTable
                                                                            {
                                                                                Name = TempValues.Name,
                                                                                Value = string.Empty,
                                                                                Name2 = TempValues.Name2,
                                                                                Value2 = bb[1],
                                                                                Name3 = TempValues.Name3,
                                                                                Value3 = bb[0]
                                                                            });
                                                                        if (bb[0].ToString().Contains("("))
                                                                        {
                                                                            string Valuestr = bb[0].ToString()
                                                                                .Replace("$", string.Empty)
                                                                                .Replace("(", string.Empty)
                                                                                .Replace(")", string.Empty)
                                                                                .Replace(",", string.Empty);
                                                                            decimal Value = Convert.ToDecimal(Valuestr);
                                                                            Amt += (-Value);
                                                                        }
                                                                        else
                                                                        {
                                                                            string Valuestr = bb[0].ToString()
                                                                                .Replace("$", string.Empty)
                                                                                .Replace(",", string.Empty);
                                                                            decimal Value = Convert.ToDecimal(Valuestr);
                                                                            Amt += Value;
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }

                                                        if (blnTotal1 == false)
                                                        {
                                                            var TempData1 = TempTable1;
                                                            var TempValues1 = TempData1.FirstOrDefault();
                                                            var PreLine = Encoding.UTF8
                                                                .GetString(Encoding.UTF8.GetBytes(words[j + 1]));
                                                            string[] bb1 = GetFinalArrRev(PreLine.Split(' '));
                                                            if (bb1.Length == 1 && !bb1[0].Contains("%"))
                                                            {
                                                                string str2 = bb1[0].ToString()
                                                                    .Replace("$", string.Empty)
                                                                    .Replace("(", string.Empty)
                                                                    .Replace(")", string.Empty)
                                                                    .Replace(",", string.Empty)
                                                                    .Trim();
                                                                decimal V2 = Convert.ToDecimal(str2);
                                                                if (AmtVol == V2)
                                                                {
                                                                    TempTable1 = new List<PDFTable>();
                                                                    blnTotal1 = true;
                                                                }
                                                            }
                                                            else
                                                            {
                                                                if (bb.Length > 2)
                                                                {
                                                                    if (!bb[1].StartsWith("100.00%"))
                                                                    {
                                                                        bb = bb.Where(
                                                                            (item, index) => index != 0 && index != 1)
                                                                            .ToArray();
                                                                        bb = GetFinalArrRev(bb);
                                                                    }
                                                                }
                                                            }
                                                            string str1 = bb[0].ToString()
                                                                .Replace("$", string.Empty)
                                                                .Replace("(", string.Empty)
                                                                .Replace(")", string.Empty)
                                                                .Replace(",", string.Empty)
                                                                .Trim();
                                                            decimal V1 = Convert.ToDecimal(str1);
                                                            if (AmtVol == V1)
                                                            {
                                                                TempTable1 = new List<PDFTable>();
                                                                blnTotal1 = true;
                                                            }
                                                            if (blnTotal1 == false && bb.Length > 1)
                                                            {
                                                                string aa = bb[1].Substring(
                                                                    bb[1].ToString().IndexOf("%") + 1)
                                                                    .Trim();

                                                                string tvalue2 = TableList[TableList.Count - 1].Value2 == null ? "" : TableList[TableList.Count - 1].Value2.ToString();
                                                                string tvalue3 = TableList[TableList.Count - 1].Value3 == null ? "" : TableList[TableList.Count - 1].Value3.ToString();
                                                                if (tvalue2 != aa &&
                                                                    tvalue3 !=
                                                                    bb[0])
                                                                {
                                                                    TableList.Add(
                                                                        new PDFTable
                                                                        {
                                                                            Name = TempValues1.Name,
                                                                            Value = string.Empty,
                                                                            Name2 = TempValues1.Name2,
                                                                            Value2 = aa.Trim(),
                                                                            Name3 = TempValues1.Name3,
                                                                            Value3 = bb[0]
                                                                        });
                                                                    if (bb[0].ToString().Contains("("))
                                                                    {
                                                                        string Valuestr = bb[0].ToString()
                                                                            .Replace("$", string.Empty)
                                                                            .Replace("(", string.Empty)
                                                                            .Replace(")", string.Empty)
                                                                            .Replace(",", string.Empty);
                                                                        decimal Value = Convert.ToDecimal(Valuestr);
                                                                        AmtVol += (-Value);
                                                                    }
                                                                    else
                                                                    {
                                                                        string Valuestr = bb[0].ToString()
                                                                            .Replace("$", string.Empty)
                                                                            .Replace(",", string.Empty);
                                                                        decimal Value = Convert.ToDecimal(Valuestr);
                                                                        AmtVol += Value;
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                    else
                                                    {
                                                        blnTotal = true;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }

                                if (FileNo == 2)
                                {
                                    words = text.Split('\n');
                                    for (int j = 0, len = words.Length; j < len; j++)
                                    {
                                        var LineData = Encoding.UTF8.GetString(Encoding.UTF8.GetBytes(words[j]));
                                        if (LineData.Contains("Payroll Fee"))
                                        {
                                            string[] authorsList = LineData.Trim().Split(' ');
                                            string[] bb = GetFinalArr(authorsList);
                                            TableList.Add(new PDFTable { Name = bb[0], Value = bb[1], Name2 = "File2" });
                                        }
                                        else if (LineData.Contains("Manual Checks"))
                                        {
                                            string[] authorsList = LineData.Trim().Split(' ');
                                            string[] bb = GetFinalArrRev(authorsList);
                                            TableList.Add(
                                                new PDFTable { Name = "Manual Checks", Value = bb[1], Name2 = "File2" });
                                        }
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            logger.Error("PDFReadController - GetDetail - " + DateTime.Now + " - " + ex.Message.ToString());
                        }

                    }
                    else
                    {
                        for (int i = 1; i <= intPageNum; i++)
                        {
                            string text = PdfTextExtractor.GetTextFromPage(
                                reader,
                                i,
                                new LocationTextExtractionStrategy());
                            try
                            {
                                if (StartDate == string.Empty)
                                {
                                    if (text.Contains("Period Sart:") ||
                                        text.Contains("Period End:") ||
                                        text.Contains("Period Start:") ||
                                        text.Contains("End Check Date:"))
                                    {
                                        words = text.Split('\n');

                                        for (int j = 0, len = words.Length; j < len; j++)
                                        {
                                            var LineData = Encoding.UTF8.GetString(Encoding.UTF8.GetBytes(words[j]));
                                            if (LineData.Contains("Period Sart:") || LineData.Contains("Period Start:"))
                                            {
                                                string[] DateSplitData = LineData.Split(':');
                                                if (DateSplitData.Count() > 0)
                                                {
                                                    StartDate = DateSplitData[1];
                                                    string[] DateFormate = StartDate.Split('/');
                                                    if (DateFormate.Count() == 3)
                                                    {
                                                        var Month = DateFormate[0].Trim().Length == 1
                                                            ? "0" + DateFormate[0].Trim()
                                                            : DateFormate[0].Trim();
                                                        StartDate = DateFormate[2] + "-" + Month + "-" + DateFormate[1];
                                                    }
                                                }
                                            }
                                            if (LineData.Contains("Period End:"))
                                            {
                                                string[] DateSplitData = LineData.Split(':');
                                                if (DateSplitData.Count() > 0)
                                                {
                                                    EndDate = DateSplitData[1];
                                                    string[] DateFormate = EndDate.Split('/');
                                                    if (DateFormate.Count() == 3)
                                                    {
                                                        var Month = DateFormate[0].Trim().Length == 1
                                                            ? "0" + DateFormate[0].Trim()
                                                            : DateFormate[0].Trim();
                                                        EndDate = DateFormate[2].Trim() +
                                                            "-" +
                                                            Month +
                                                            "-" +
                                                            DateFormate[1].Trim();
                                                    }
                                                }
                                            }
                                            if (LineData.Contains("End Check Date:"))
                                            {
                                                string[] DateSplitData = LineData.Split(':');
                                                if (DateSplitData.Count() > 0)
                                                {
                                                    EndCheckDate = DateSplitData[1];
                                                    string[] DateFormate = EndCheckDate.Split('/');
                                                    if (DateFormate.Count() == 3)
                                                    {
                                                        var Month = DateFormate[0].Trim().Length == 1
                                                            ? "0" + DateFormate[0].Trim()
                                                            : DateFormate[0].Trim();
                                                        EndCheckDate = DateFormate[2].Trim() +
                                                            "-" +
                                                            Month +
                                                            "-" +
                                                            DateFormate[1].Trim();
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }

                                if (text.Contains("Summary:"))
                                {
                                    words = text.Split('\n');
                                    //words = text;
                                    int DVFlg = 0;
                                    for (int j = 0, len = words.Length; j < len; j++)
                                    {
                                        var LineData = Encoding.UTF8.GetString(Encoding.UTF8.GetBytes(words[j]));
                                        if (LineData.Contains("Summary:"))
                                        {
                                            DVFlg = 1;
                                        }
                                        if (DVFlg == 1)
                                        {
                                            LineData = LineData.Trim();
                                            //TableList.Add(new PDFTable { Name = LineData });
                                            if (LineData.Contains("Summary:"))
                                            {
                                                if (LineData.Contains("Summary: 85-SALAD B  - 85-SALAD"))
                                                {
                                                }

                                                if (LineData.Contains("Summary: 85 SALAD  - 85-SALAD BAR"))
                                                {
                                                    var PreLine = Encoding.UTF8
                                                        .GetString(Encoding.UTF8.GetBytes(words[j + 1]));
                                                    string[] authorsList = PreLine.Split(' ');

                                                    //if (StringCount == 10)
                                                    //{
                                                    TableList.Add(new PDFTable { Name = string.Empty });
                                                    TableList.Add(
                                                        new PDFTable
                                                        {
                                                            Name = LineData,
                                                            Name5 = authorsList[0] + " " + authorsList[1],
                                                            Value5 = authorsList[2]
                                                        });
                                                }
                                                else if (LineData.Contains("Summary:") && LineData.Contains("Net Pay:"))
                                                {
                                                    string[] authorsList = LineData.Split(' ');
                                                    int StringCount = authorsList.Count();
                                                    int CountLmt = StringCount >= 8 ? 3 : StringCount == 7 ? 2 : 1;

                                                    string MergeName = string.Empty;
                                                    for (int x = 0; x < StringCount - CountLmt; x++)
                                                    {
                                                        MergeName = MergeName == string.Empty ? authorsList[x]
                                                            : MergeName + " " + authorsList[x];
                                                    }
                                                    //if (StringCount == 10)
                                                    //{
                                                    TableList.Add(new PDFTable { Name = string.Empty });
                                                    TableList.Add(
                                                        new PDFTable
                                                        {
                                                            Name = MergeName,
                                                            Name5 =
                                                                authorsList[StringCount - 3] +
                                                                        " " +
                                                                        authorsList[StringCount - 2],
                                                            Value5 = authorsList[StringCount - 1]
                                                        });
                                                }
                                                else
                                                {
                                                    TableList.Add(new PDFTable { Name = string.Empty });
                                                    TableList.Add(new PDFTable { Name = LineData });
                                                }
                                                //}
                                            }
                                            else
                                            {
                                                if (LineData.Contains("CHILD SUP"))
                                                {
                                                    LineData = LineData.Replace("CHILD SUP", "CHILD_SUP");
                                                }
                                                if (LineData.Contains("PAID SICK"))
                                                {
                                                    LineData = LineData.Replace("PAID SICK", "PAID_SICK");
                                                }
                                                if (LineData.Contains("CCFEE D"))
                                                {
                                                    LineData = LineData.Replace("CCFEE D", "CCFEE_D");
                                                }
                                                if (LineData.Contains("CCTIP D"))
                                                {
                                                    LineData = LineData.Replace("CCTIP D", "CCTIP_D");
                                                }
                                                if (LineData.Contains("LIFE INS."))
                                                {
                                                    LineData = LineData.Replace("LIFE INS.", "LIFE_INS.");
                                                }


                                                if (LineData.Contains(" "))
                                                {
                                                    string[] authorsList = LineData.Split(' ');
                                                    int StringCount = authorsList.Count();
                                                    if (StringCount > 10)
                                                    {
                                                        TableList.Add(
                                                            new PDFTable
                                                            {
                                                                Name = authorsList[0],
                                                                Value = authorsList[2],
                                                                Name2 = authorsList[3],
                                                                Value2 = authorsList[4],
                                                                Name3 = authorsList[5],
                                                                Value3 = authorsList[6],
                                                                Name4 = authorsList[7],
                                                                Value4 = authorsList[8],
                                                                Name5 = authorsList[9],
                                                                Value5 = authorsList[10]
                                                            });
                                                    }
                                                    else if (StringCount == 10)
                                                    {
                                                        TableList.Add(
                                                            new PDFTable
                                                            {
                                                                Name = authorsList[0],
                                                                Value = authorsList[1],
                                                                Name2 = authorsList[2],
                                                                Value2 = authorsList[3],
                                                                Name3 = authorsList[4],
                                                                Value3 = authorsList[5],
                                                                Name4 = authorsList[6],
                                                                Value4 = authorsList[7],
                                                                Name5 = authorsList[8],
                                                                Value5 = authorsList[9]
                                                            });
                                                    }
                                                    else if (StringCount == 8)
                                                    {
                                                        TableList.Add(
                                                            new PDFTable
                                                            {
                                                                Name = authorsList[0],
                                                                Value = authorsList[1],
                                                                Name2 = authorsList[2],
                                                                Value2 = authorsList[3],
                                                                Name3 = authorsList[4],
                                                                Value3 = authorsList[5],
                                                                Name4 = authorsList[6],
                                                                Value4 = authorsList[7]
                                                            });
                                                    }
                                                    else if (StringCount == 6)
                                                    {
                                                        TableList.Add(
                                                            new PDFTable
                                                            {
                                                                Name = authorsList[0],
                                                                Value = authorsList[1],
                                                                Name2 = authorsList[2],
                                                                Value2 = authorsList[3]
                                                            });
                                                    }
                                                    else if (StringCount == 5)
                                                    {
                                                        if (LineData.Contains("$"))
                                                        {
                                                            TempTable.Add(
                                                                new PDFTable
                                                                {
                                                                    Name = string.Empty,
                                                                    Value = authorsList[0],
                                                                    Name2 = string.Empty,
                                                                    Value2 = authorsList[1],
                                                                    Name3 = string.Empty,
                                                                    Value3 = authorsList[2],
                                                                    Name4 = string.Empty,
                                                                    Value4 = authorsList[3],
                                                                    Name5 = string.Empty,
                                                                    Value5 = authorsList[4]
                                                                });
                                                        }
                                                        else
                                                        {
                                                            var TempData = TempTable;
                                                            if (TempTable.Count() > 0)
                                                            {
                                                                var TempValues = TempData.FirstOrDefault();
                                                                TableList.Add(
                                                                    new PDFTable
                                                                    {
                                                                        Name = authorsList[0],
                                                                        Value = TempValues.Value,
                                                                        Name2 = authorsList[1],
                                                                        Value2 = TempValues.Value2,
                                                                        Name3 = authorsList[2],
                                                                        Value3 = TempValues.Value3,
                                                                        Name4 = authorsList[3],
                                                                        Value4 = TempValues.Value4,
                                                                        Name5 = authorsList[4],
                                                                        Value5 = TempValues.Value5
                                                                    });
                                                                TempTable = new List<PDFTable>();
                                                            }
                                                        }
                                                    }
                                                    else if (StringCount == 4)
                                                    {
                                                        if (LineData.Contains("$"))
                                                        {
                                                            var DigitLine = LineData.Replace("$", string.Empty)
                                                                .Replace(" ", string.Empty)
                                                                .Replace(".", string.Empty)
                                                                .Replace(",", string.Empty)
                                                                .Replace("(", string.Empty)
                                                                .Replace(")", string.Empty);
                                                            if (DigitLine.All(s => char.IsDigit(s)))
                                                            {
                                                                TempTable.Add(
                                                                    new PDFTable
                                                                    {
                                                                        Name = string.Empty,
                                                                        Value = authorsList[0],
                                                                        Name2 = string.Empty,
                                                                        Value2 = authorsList[1],
                                                                        Name3 = string.Empty,
                                                                        Value3 = authorsList[2],
                                                                        Name4 = string.Empty,
                                                                        Value4 = authorsList[3]
                                                                    });
                                                            }
                                                            else
                                                            {
                                                                if (LineData.Contains("SSEC") ||
                                                                    LineData.Contains("MEDI") ||
                                                                    LineData.Contains("FWT"))
                                                                {
                                                                    TableList.Add(
                                                                        new PDFTable
                                                                        {
                                                                            Name = string.Empty,
                                                                            Name3 = authorsList[0],
                                                                            Value3 = authorsList[1],
                                                                            Name4 = authorsList[2],
                                                                            Value4 = authorsList[3]
                                                                        });
                                                                }
                                                                else
                                                                {
                                                                    TableList.Add(
                                                                        new PDFTable
                                                                        {
                                                                            Name = authorsList[0],
                                                                            Value = authorsList[1],
                                                                            Name2 = authorsList[2],
                                                                            Value2 = authorsList[3]
                                                                        });
                                                                }
                                                            }
                                                        }
                                                        else
                                                        {
                                                            var TempData = TempTable;
                                                            if (TempTable.Count() > 0)
                                                            {
                                                                var TempValues = TempData.FirstOrDefault();
                                                                TableList.Add(
                                                                    new PDFTable
                                                                    {
                                                                        Name = authorsList[0],
                                                                        Value = TempValues.Value,
                                                                        Name2 = authorsList[1],
                                                                        Value2 = TempValues.Value2,
                                                                        Name3 = authorsList[2],
                                                                        Value3 = TempValues.Value3,
                                                                        Name4 = authorsList[3],
                                                                        Value4 = TempValues.Value4
                                                                    });
                                                                TempTable = new List<PDFTable>();
                                                            }
                                                        }
                                                    }
                                                    else if (StringCount == 3)
                                                    {
                                                        try
                                                        {
                                                            if (LineData.Contains("$"))
                                                            {
                                                                var DigitLine = LineData.Replace("$", string.Empty)
                                                                    .Replace(" ", string.Empty)
                                                                    .Replace(".", string.Empty)
                                                                    .Replace(",", string.Empty)
                                                                    .Replace("(", string.Empty)
                                                                    .Replace(")", string.Empty);
                                                                if (DigitLine.All(s => char.IsDigit(s)))
                                                                {
                                                                    TempTable.Add(
                                                                        new PDFTable
                                                                        {
                                                                            Name = string.Empty,
                                                                            Value = authorsList[0],
                                                                            Name2 = string.Empty,
                                                                            Value2 = authorsList[1],
                                                                            Name3 = string.Empty,
                                                                            Value3 = authorsList[2]
                                                                        });
                                                                }
                                                                else
                                                                {
                                                                    if (LineData.Contains("SSEC") ||
                                                                        LineData.Contains("MEDI") ||
                                                                        LineData.Contains("FWT"))
                                                                    {
                                                                        TableList.Add(
                                                                            new PDFTable
                                                                            {
                                                                                Name = string.Empty,
                                                                                Name3 = authorsList[0],
                                                                                Value3 = authorsList[1],
                                                                                Name4 = authorsList[2],
                                                                            });
                                                                    }
                                                                    else
                                                                    {
                                                                        TableList.Add(
                                                                            new PDFTable
                                                                            {
                                                                                Name = authorsList[0],
                                                                                Value = authorsList[1],
                                                                                Name2 = authorsList[2]
                                                                            });
                                                                    }
                                                                }
                                                            }
                                                            else
                                                            {
                                                                var TempData = TempTable;
                                                                if (TempTable.Count() > 0)
                                                                {
                                                                    var TempValues = TempData.FirstOrDefault();
                                                                    TableList.Add(
                                                                        new PDFTable
                                                                        {
                                                                            Name = authorsList[0],
                                                                            Value = TempValues.Value,
                                                                            Name2 = authorsList[1],
                                                                            Value2 = TempValues.Value2,
                                                                            Name3 = authorsList[2]
                                                                        });
                                                                    TempTable = new List<PDFTable>();
                                                                }
                                                            }
                                                        }
                                                        catch (Exception ex)
                                                        {
                                                            logger.Error("PDFReadController - GetDetail - " + DateTime.Now + " - " + ex.Message.ToString());
                                                        }

                                                    }
                                                    else if (StringCount == 2)
                                                    {

                                                        if (LineData.Contains("$"))
                                                        {
                                                            var DigitLine = LineData.Replace("$", string.Empty)
                                                                .Replace(" ", string.Empty)
                                                                .Replace(".", string.Empty)
                                                                .Replace(",", string.Empty);
                                                            if (DigitLine.All(s => char.IsDigit(s)))
                                                            {
                                                                TempTable.Add(
                                                                    new PDFTable
                                                                    {
                                                                        Name = string.Empty,
                                                                        Value3 = authorsList[0],
                                                                        Name2 = string.Empty,
                                                                        Value4 = authorsList[1]
                                                                    });
                                                            }
                                                            else
                                                            {
                                                                if (LineData.Contains("LOCYONRES") ||
                                                                    LineData.Contains("LOCNYCRES"))
                                                                {
                                                                    TableList.Add(
                                                                        new PDFTable
                                                                        {
                                                                            Name = string.Empty,
                                                                            Value3 = authorsList[0],
                                                                            Value4 = authorsList[1]
                                                                        });
                                                                }
                                                            }
                                                        }
                                                        else
                                                        {
                                                            try
                                                            {
                                                                var TempData = TempTable;
                                                                if (TempTable.Count() > 0)
                                                                {
                                                                    if (authorsList[1] == "HOURS")
                                                                    {
                                                                        var TempValues = TempData.FirstOrDefault();
                                                                        TableList.Add(
                                                                            new PDFTable
                                                                            {
                                                                                Name = authorsList[0],
                                                                                Value = TempValues.Value3,
                                                                                Name2 = authorsList[1],
                                                                                Value2 = TempValues.Value4
                                                                            });
                                                                        TempTable = new List<PDFTable>();
                                                                    }
                                                                    else
                                                                    {
                                                                        var TempValues = TempData.FirstOrDefault();
                                                                        TableList.Add(
                                                                            new PDFTable
                                                                            {
                                                                                Name = string.Empty,
                                                                                Name3 = authorsList[0],
                                                                                Value3 = TempValues.Value3,
                                                                                Name4 = authorsList[1],
                                                                                Value4 = TempValues.Value4
                                                                            });
                                                                        TempTable = new List<PDFTable>();
                                                                    }
                                                                }
                                                            }
                                                            catch (Exception ex)
                                                            {
                                                                logger.Error("PDFReadController - GetDetail - " + DateTime.Now + " - " + ex.Message.ToString());
                                                            }

                                                        }
                                                    }
                                                }
                                                if (LineData.Contains("Total"))
                                                {
                                                    DVFlg = 0;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                logger.Error("PDFReadController - GetDetail - " + DateTime.Now + " - " + ex.Message.ToString());
                            }

                        }
                    }
                    List<PDFTable> PDFPrintList = new List<PDFTable>();
                    int flg = 0;
                    int DepartmentId = 0;

                    foreach (var item in TableList)
                    {
                        try
                        {
                            if (item.Name2 == null)
                            {
                                item.Name2 = string.Empty;
                            }
                            if (item.Name.Contains("Summary:"))
                            {
                                flg = 4;
                            }
                            else if (item.Name2.Contains("File2"))
                            {
                                flg = 4;
                                item.Value2 = item.Value;
                            }
                            else if (item.Name.Contains("Dept :"))
                            {
                                flg = 4;
                                if (item.Value2 != null)
                                {
                                    item.Name = item.Value2;
                                    item.Value2 = item.Value3;
                                }
                            }
                            else if (item.Name.Contains("Company Total"))
                            {
                                flg = 4;
                                if (item.Value2 != null)
                                {
                                    item.Name = item.Value2;
                                    item.Value2 = item.Value3;
                                }
                            }

                            if (flg == 4 && item.Name != null)
                            {
                                PDFPrintList.Add(
                                    new PDFTable
                                    {
                                        Name = item.Name,
                                        Value = item.Value,
                                        Name2 = item.Name2,
                                        Value2 = item.Value2,
                                        Name3 = item.Name3,
                                        Value3 = item.Value3,
                                        Name4 = item.Name4,
                                        Value4 = item.Value4,
                                        Name5 = item.Name5,
                                        Value5 = item.Value5
                                    });
                            }
                        }
                        catch (Exception ex)
                        {
                            logger.Error("PDFReadController - GetDetail - " + DateTime.Now + " - " + ex.Message.ToString());
                        }

                    }
                    ViewBag.PDFData = TableList;
                    return View("PDFRead", PDFPrintList.Where(s => s.Name != string.Empty));
                }
            }
            catch (Exception ex)
            {
                logger.Error("PDFReadController - GetDetail - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            ViewBag.ErrorMessage = "PDF File Path Not Found..";
            return View("Create");
        }

        /// <summary>
        /// This method is get grid payroll.
        /// </summary>
        /// <param name="IsBindData"></param>
        /// <param name="currentPageIndex"></param>
        /// <param name="orderby"></param>
        /// <param name="IsAsc"></param>
        /// <param name="PageSize"></param>
        /// <param name="SearchRecords"></param>
        /// <param name="Alpha"></param>
        /// <param name="SearchTitle"></param>
        /// <returns></returns>
        public ActionResult Grid(
            int IsBindData = 1,
            int currentPageIndex = 1,
            string orderby = "EndCheckDate",
            int IsAsc = 0,
            int PageSize = 100,
            int SearchRecords = 1,
            string Alpha = "",
            string SearchTitle = "")
        {
            #region MyRegion_Array
            try
            {
                if (IsArray == true)
                {
                    foreach (string a1 in Arr)
                    {
                        if (a1.Split(':')[0].ToString() == "IsBindData")
                        {
                            IsBindData = Convert.ToInt32(a1.Split(':')[1].ToString());
                        }
                        if (a1.Split(':')[0].ToString() == "currentPageIndex")
                        {
                            currentPageIndex = Convert.ToInt32(a1.Split(':')[1].ToString());
                        }
                        if (a1.Split(':')[0].ToString() == "orderby")
                        {
                            orderby = Convert.ToString(a1.Split(':')[1].ToString());
                        }
                        if (a1.Split(':')[0].ToString() == "IsAsc")
                        {
                            IsAsc = Convert.ToInt32(a1.Split(':')[1].ToString());
                        }
                        if (a1.Split(':')[0].ToString() == "PageSize")
                        {
                            PageSize = Convert.ToInt32(a1.Split(':')[1].ToString());
                        }
                        if (a1.Split(':')[0].ToString() == "SearchRecords")
                        {
                            SearchRecords = Convert.ToInt32(a1.Split(':')[1].ToString());
                        }
                        if (a1.Split(':')[0].ToString() == "Alpha")
                        {
                            Alpha = Convert.ToString(a1.Split(':')[1].ToString());
                        }
                        if (a1.Split(':')[0].ToString() == "SearchTitle")
                        {
                            SearchTitle = Convert.ToString(a1.Split(':')[1].ToString());
                        }
                    }
                }
            }
            catch
            {
            }

            IsArray = false;
            Arr = new string[]
            {
                "IsBindData:" + IsBindData,
                "currentPageIndex:" + currentPageIndex,
                "orderby:" + orderby,
                "IsAsc:" + IsAsc,
                "PageSize:" + PageSize,
                "SearchRecords:" + SearchRecords,
                "Alpha:" + Alpha,
                "SearchTitle:" + SearchTitle
            };
            #endregion

            #region MyRegion_BindData
            int startIndex = ((currentPageIndex - 1) * PageSize) + 1;
            int endIndex = startIndex + PageSize - 1;

            if (IsBindData == 1 || IsEdit == true)
            {
                int StoreID = Convert.ToInt32(Session["storeid"].ToString());
                BindData = GetData(SearchRecords, SearchTitle, StoreID).OfType<PayrollFileList>().ToList();
                TotalDataCount = BindData.OfType<PayrollFileList>().ToList().Count();
            }

            if (TotalDataCount == 0)
            {
                StatusMessage = "NoItem";
            }

            ViewBag.IsBindData = IsBindData;
            ViewBag.CurrentPageIndex = currentPageIndex;
            ViewBag.LastPageIndex = this.getLastPageIndex(PageSize);
            ViewBag.OrderByVal = orderby;
            ViewBag.IsAscVal = IsAsc;
            ViewBag.PageSize = PageSize;
            ViewBag.Alpha = Alpha;
            ViewBag.SearchRecords = SearchRecords;
            ViewBag.SearchTitle = SearchTitle;
            ViewBag.StatusMessage = StatusMessage;
            ViewBag.startindex = startIndex;

            if (TotalDataCount < endIndex)
            {
                ViewBag.endIndex = TotalDataCount;
            }
            else
            {
                ViewBag.endIndex = endIndex;
            }
            ViewBag.TotalDataCount = TotalDataCount;
            var ColumnName = typeof(PayrollFileList).GetProperties().Where(p => p.Name == orderby).FirstOrDefault();

            IEnumerable Data = null;
            if (IsAsc == 1)
            {
                ViewBag.AscVal = 0;
                if (orderby == "UploadDate")
                {
                    Data = BindData.OfType<PayrollFileList>()
                        .ToList()
                        .OrderBy(
                            n => AdminSiteConfiguration.stringToDate(
                                n.UploadDate,
                                DateFormats.MMddyyyy,
                                DateFormats.MMddyyyy))
                        .Skip(startIndex - 1)
                        .Take(PageSize);
                }
                else if (orderby == "EndCheckDate")
                {
                    Data = BindData.OfType<PayrollFileList>()
                        .ToList()
                        .OrderBy(
                            n => AdminSiteConfiguration.stringToDate(
                                n.EndCheckDate,
                                DateFormats.MMddyyyy,
                                DateFormats.MMddyyyy))
                        .Skip(startIndex - 1)
                        .Take(PageSize);
                }
                else
                {
                    Data = BindData.OfType<PayrollFileList>()
                        .ToList()
                        .OrderBy(n => ColumnName.GetValue(n, null))
                        .Skip(startIndex - 1)
                        .Take(PageSize);
                }
            }
            else
            {
                ViewBag.AscVal = 1;
                if (orderby == "UploadDate")
                {
                    Data = BindData.OfType<PayrollFileList>()
                        .ToList()
                        .OrderByDescending(
                            n => AdminSiteConfiguration.stringToDate(
                                n.UploadDate,
                                DateFormats.MMddyyyy,
                                DateFormats.MMddyyyy))
                        .Skip(startIndex - 1)
                        .Take(PageSize);
                }
                else if (orderby == "EndCheckDate")
                {
                    Data = BindData.OfType<PayrollFileList>()
                        .ToList()
                        .OrderByDescending(
                            n => AdminSiteConfiguration.stringToDate(
                                n.EndCheckDate,
                                DateFormats.MMddyyyy,
                                DateFormats.MMddyyyy))
                        .Skip(startIndex - 1)
                        .Take(PageSize);
                }
                else
                {
                    Data = BindData.OfType<PayrollFileList>()
                        .ToList()
                        .OrderByDescending(n => ColumnName.GetValue(n, null))
                        .Skip(startIndex - 1)
                        .Take(PageSize);
                }
            }
            StatusMessage = string.Empty;
            return View(Data);
            #endregion
        }

        // GET: PDFRead
        /// <summary>
        /// This function is Index for payroll report.
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            try
            {
                if (Session["PDFFile"] != null)
                {
                    int PayrollId = 0;

                    string FilePath = Server.MapPath("~/userfiles/PayrollFile/" + Session["PDFFile"]);
                    List<PDFTable> TableList = new List<PDFTable>();
                    List<PDFTable> TempTable = new List<PDFTable>();


                    PdfReader reader = new PdfReader(FilePath);
                    int intPageNum = reader.NumberOfPages;
                    string StartDate = string.Empty;
                    string EndDate = string.Empty;
                    string[] words;
                    StringBuilder Reader = new StringBuilder();
                    for (int i = 1; i <= intPageNum; i++)
                    {
                        string text = PdfTextExtractor.GetTextFromPage(reader, i, new LocationTextExtractionStrategy());
                        if (StartDate == string.Empty)
                        {
                            if (text.Contains("Period Sart:") || text.Contains("Period End:"))
                            {
                                words = text.Split('\n');

                                for (int j = 0, len = words.Length; j < len; j++)
                                {
                                    var LineData = Encoding.UTF8.GetString(Encoding.UTF8.GetBytes(words[j]));
                                    if (LineData.Contains("Period Sart:"))
                                    {
                                        string[] DateSplitData = LineData.Split(':');
                                        if (DateSplitData.Count() > 0)
                                        {
                                            StartDate = DateSplitData[1];
                                            string[] DateFormate = StartDate.Split('/');
                                            if (DateFormate.Count() == 3)
                                            {
                                                var Month = DateFormate[0].Trim().Length == 1
                                                    ? "0" + DateFormate[0].Trim()
                                                    : DateFormate[0].Trim();
                                                StartDate = DateFormate[2] + "-" + Month + "-" + DateFormate[1];
                                            }
                                        }
                                    }
                                    if (LineData.Contains("Period End:"))
                                    {
                                        string[] DateSplitData = LineData.Split(':');
                                        if (DateSplitData.Count() > 0)
                                        {
                                            EndDate = DateSplitData[1];
                                            string[] DateFormate = EndDate.Split('/');
                                            if (DateFormate.Count() == 3)
                                            {
                                                var Month = DateFormate[0].Trim().Length == 1
                                                    ? "0" + DateFormate[0].Trim()
                                                    : DateFormate[0].Trim();
                                                EndDate = DateFormate[2].Trim() +
                                                    "-" +
                                                    Month +
                                                    "-" +
                                                    DateFormate[1].Trim();
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        if (text.Contains("Summary:"))
                        {
                            words = text.Split('\n');
                            //words = text;
                            int DVFlg = 0;
                            for (int j = 0, len = words.Length; j < len; j++)
                            {
                                var LineData = Encoding.UTF8.GetString(Encoding.UTF8.GetBytes(words[j]));
                                if (LineData.Contains("Summary:"))
                                {
                                    DVFlg = 1;
                                }
                                if (DVFlg == 1)
                                {
                                    LineData = LineData.Trim();
                                    //TableList.Add(new PDFTable { Name = LineData });
                                    if (LineData.Contains("Summary:"))
                                    {
                                        if (LineData.Contains("Summary: 85-SALAD B  - 85-SALAD"))
                                        {
                                        }

                                        if (LineData.Contains("Summary: 85 SALAD  - 85-SALAD BAR"))
                                        {
                                            var PreLine = Encoding.UTF8.GetString(Encoding.UTF8.GetBytes(words[j + 1]));
                                            string[] authorsList = PreLine.Split(' ');

                                            //if (StringCount == 10)
                                            //{
                                            TableList.Add(new PDFTable { Name = string.Empty });
                                            TableList.Add(
                                                new PDFTable
                                                {
                                                    Name = LineData,
                                                    Name5 = authorsList[0] + " " + authorsList[1],
                                                    Value5 = authorsList[2]
                                                });
                                        }
                                        else if (LineData.Contains("Summary:") && LineData.Contains("Net Pay:"))
                                        {
                                            string[] authorsList = LineData.Split(' ');
                                            int StringCount = authorsList.Count();
                                            int CountLmt = StringCount >= 8 ? 3 : StringCount == 7 ? 2 : 1;

                                            string MergeName = string.Empty;
                                            for (int x = 0; x < StringCount - CountLmt; x++)
                                            {
                                                MergeName = MergeName == string.Empty ? authorsList[x]
                                                    : MergeName + " " + authorsList[x];
                                            }
                                            //if (StringCount == 10)
                                            //{
                                            TableList.Add(new PDFTable { Name = string.Empty });
                                            TableList.Add(
                                                new PDFTable
                                                {
                                                    Name = MergeName,
                                                    Name5 =
                                                        authorsList[StringCount - 3] +
                                                                " " +
                                                                authorsList[StringCount - 2],
                                                    Value5 = authorsList[StringCount - 1]
                                                });
                                        }
                                        else
                                        {
                                            TableList.Add(new PDFTable { Name = string.Empty });
                                            TableList.Add(new PDFTable { Name = LineData });
                                        }
                                        //}
                                    }
                                    else
                                    {
                                        if (LineData.Contains("CHILD SUP"))
                                        {
                                            LineData = LineData.Replace("CHILD SUP", "CHILD_SUP");
                                        }
                                        if (LineData.Contains("PAID SICK"))
                                        {
                                            LineData = LineData.Replace("PAID SICK", "PAID_SICK");
                                        }
                                        if (LineData.Contains("CCFEE D"))
                                        {
                                            LineData = LineData.Replace("CCFEE D", "CCFEE_D");
                                        }
                                        if (LineData.Contains("CCTIP D"))
                                        {
                                            LineData = LineData.Replace("CCTIP D", "CCTIP_D");
                                        }
                                        if (LineData.Contains("LIFE INS."))
                                        {
                                            LineData = LineData.Replace("LIFE INS.", "LIFE_INS.");
                                        }


                                        if (LineData.Contains(" "))
                                        {
                                            string[] authorsList = LineData.Split(' ');
                                            int StringCount = authorsList.Count();
                                            if (StringCount > 10)
                                            {
                                                TableList.Add(
                                                    new PDFTable
                                                    {
                                                        Name = authorsList[0],
                                                        Value = authorsList[2],
                                                        Name2 = authorsList[3],
                                                        Value2 = authorsList[4],
                                                        Name3 = authorsList[5],
                                                        Value3 = authorsList[6],
                                                        Name4 = authorsList[7],
                                                        Value4 = authorsList[8],
                                                        Name5 = authorsList[9],
                                                        Value5 = authorsList[10]
                                                    });
                                            }
                                            else if (StringCount == 10)
                                            {
                                                TableList.Add(
                                                    new PDFTable
                                                    {
                                                        Name = authorsList[0],
                                                        Value = authorsList[1],
                                                        Name2 = authorsList[2],
                                                        Value2 = authorsList[3],
                                                        Name3 = authorsList[4],
                                                        Value3 = authorsList[5],
                                                        Name4 = authorsList[6],
                                                        Value4 = authorsList[7],
                                                        Name5 = authorsList[8],
                                                        Value5 = authorsList[9]
                                                    });
                                            }
                                            else if (StringCount == 8)
                                            {
                                                TableList.Add(
                                                    new PDFTable
                                                    {
                                                        Name = authorsList[0],
                                                        Value = authorsList[1],
                                                        Name2 = authorsList[2],
                                                        Value2 = authorsList[3],
                                                        Name3 = authorsList[4],
                                                        Value3 = authorsList[5],
                                                        Name4 = authorsList[6],
                                                        Value4 = authorsList[7]
                                                    });
                                            }
                                            else if (StringCount == 6)
                                            {
                                                TableList.Add(
                                                    new PDFTable
                                                    {
                                                        Name = authorsList[0],
                                                        Value = authorsList[1],
                                                        Name2 = authorsList[2],
                                                        Value2 = authorsList[3]
                                                    });
                                            }
                                            else if (StringCount == 5)
                                            {
                                                if (LineData.Contains("$"))
                                                {
                                                    TempTable.Add(
                                                        new PDFTable
                                                        {
                                                            Name = string.Empty,
                                                            Value = authorsList[0],
                                                            Name2 = string.Empty,
                                                            Value2 = authorsList[1],
                                                            Name3 = string.Empty,
                                                            Value3 = authorsList[2],
                                                            Name4 = string.Empty,
                                                            Value4 = authorsList[3],
                                                            Name5 = string.Empty,
                                                            Value5 = authorsList[4]
                                                        });
                                                }
                                                else
                                                {
                                                    var TempData = TempTable;
                                                    if (TempTable.Count() > 0)
                                                    {
                                                        var TempValues = TempData.FirstOrDefault();
                                                        TableList.Add(
                                                            new PDFTable
                                                            {
                                                                Name = authorsList[0],
                                                                Value = TempValues.Value,
                                                                Name2 = authorsList[1],
                                                                Value2 = TempValues.Value2,
                                                                Name3 = authorsList[2],
                                                                Value3 = TempValues.Value3,
                                                                Name4 = authorsList[3],
                                                                Value4 = TempValues.Value4,
                                                                Name5 = authorsList[4],
                                                                Value5 = TempValues.Value5
                                                            });
                                                        TempTable = new List<PDFTable>();
                                                    }
                                                }
                                            }
                                            else if (StringCount == 4)
                                            {
                                                if (LineData.Contains("$"))
                                                {
                                                    var DigitLine = LineData.Replace("$", string.Empty)
                                                        .Replace(" ", string.Empty)
                                                        .Replace(".", string.Empty)
                                                        .Replace(",", string.Empty)
                                                        .Replace("(", string.Empty)
                                                        .Replace(")", string.Empty);
                                                    if (DigitLine.All(s => char.IsDigit(s)))
                                                    {
                                                        TempTable.Add(
                                                            new PDFTable
                                                            {
                                                                Name = string.Empty,
                                                                Value = authorsList[0],
                                                                Name2 = string.Empty,
                                                                Value2 = authorsList[1],
                                                                Name3 = string.Empty,
                                                                Value3 = authorsList[2],
                                                                Name4 = string.Empty,
                                                                Value4 = authorsList[3]
                                                            });
                                                    }
                                                    else
                                                    {
                                                        if (LineData.Contains("SSEC") ||
                                                            LineData.Contains("MEDI") ||
                                                            LineData.Contains("FWT"))
                                                        {
                                                            TableList.Add(
                                                                new PDFTable
                                                                {
                                                                    Name = string.Empty,
                                                                    Name3 = authorsList[0],
                                                                    Value3 = authorsList[1],
                                                                    Name4 = authorsList[2],
                                                                    Value4 = authorsList[3]
                                                                });
                                                        }
                                                        else
                                                        {
                                                            TableList.Add(
                                                                new PDFTable
                                                                {
                                                                    Name = authorsList[0],
                                                                    Value = authorsList[1],
                                                                    Name2 = authorsList[2],
                                                                    Value2 = authorsList[3]
                                                                });
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    var TempData = TempTable;
                                                    if (TempTable.Count() > 0)
                                                    {
                                                        var TempValues = TempData.FirstOrDefault();
                                                        TableList.Add(
                                                            new PDFTable
                                                            {
                                                                Name = authorsList[0],
                                                                Value = TempValues.Value,
                                                                Name2 = authorsList[1],
                                                                Value2 = TempValues.Value2,
                                                                Name3 = authorsList[2],
                                                                Value3 = TempValues.Value3,
                                                                Name4 = authorsList[3],
                                                                Value4 = TempValues.Value4
                                                            });
                                                        TempTable = new List<PDFTable>();
                                                    }
                                                }
                                            }
                                            else if (StringCount == 2)
                                            {
                                                if (LineData.Contains("$"))
                                                {
                                                    var DigitLine = LineData.Replace("$", string.Empty)
                                                        .Replace(" ", string.Empty)
                                                        .Replace(".", string.Empty)
                                                        .Replace(",", string.Empty);
                                                    if (DigitLine.All(s => char.IsDigit(s)))
                                                    {
                                                        TempTable.Add(
                                                            new PDFTable
                                                            {
                                                                Name = string.Empty,
                                                                Value3 = authorsList[0],
                                                                Name2 = string.Empty,
                                                                Value4 = authorsList[1]
                                                            });
                                                    }
                                                    else
                                                    {
                                                        if (LineData.Contains("LOCYONRES") ||
                                                            LineData.Contains("LOCNYCRES"))
                                                        {
                                                            TableList.Add(
                                                                new PDFTable
                                                                {
                                                                    Name = string.Empty,
                                                                    Value3 = authorsList[0],
                                                                    Value4 = authorsList[1]
                                                                });
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    var TempData = TempTable;
                                                    if (TempTable.Count() > 0)
                                                    {
                                                        if (authorsList[1] == "HOURS")
                                                        {
                                                            var TempValues = TempData.FirstOrDefault();
                                                            TableList.Add(
                                                                new PDFTable
                                                                {
                                                                    Name = authorsList[0],
                                                                    Value = TempValues.Value3,
                                                                    Name2 = authorsList[1],
                                                                    Value2 = TempValues.Value4
                                                                });
                                                            TempTable = new List<PDFTable>();
                                                        }
                                                        else
                                                        {
                                                            var TempValues = TempData.FirstOrDefault();
                                                            TableList.Add(
                                                                new PDFTable
                                                                {
                                                                    Name = string.Empty,
                                                                    Name3 = authorsList[0],
                                                                    Value3 = TempValues.Value3,
                                                                    Name4 = authorsList[1],
                                                                    Value4 = TempValues.Value4
                                                                });
                                                            TempTable = new List<PDFTable>();
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        if (LineData.Contains("Total"))
                                        {
                                            DVFlg = 0;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    List<PDFTable> PDFPrintList = new List<PDFTable>();

                    int flg = 0;
                    int DepartmentId = 0;
                    PayrollMaster obj = new PayrollMaster();
                    obj.StoreId = Convert.ToInt32(Session["storeid"].ToString());
                    if (StartDate != string.Empty)
                    {
                        obj.StartDate = Convert.ToDateTime(StartDate);
                    }
                    else
                    {
                        obj.StartDate = DateTime.Today;
                    }
                    if (EndDate != string.Empty)
                    {
                        obj.EndDate = Convert.ToDateTime(EndDate);
                    }
                    else
                    {
                        obj.EndDate = DateTime.Today;
                    }
                    obj.PayrollReportId = Convert.ToInt32(Session["PayrollReportId"].ToString());
                    //This Db class is used for Add payroll Masters.
                    _PDFReadRepository.AddPayrollMasters(obj);

                    PayrollId = obj.PayrollId;
                    foreach (var item in TableList)
                    {
                        if (item.Name.Contains("Summary:"))
                        {
                            flg = 4;
                        }

                        if (flg == 4)
                        {
                            PDFPrintList.Add(
                                new PDFTable
                                {
                                    Name = item.Name,
                                    Value = item.Value,
                                    Name2 = item.Name2,
                                    Value2 = item.Value2,
                                    Name3 = item.Name3,
                                    Value3 = item.Value3,
                                    Name4 = item.Name4,
                                    Value4 = item.Value4,
                                    Name5 = item.Name5,
                                    Value5 = item.Value5
                                });

                            if (!item.Name.Contains("Summary:"))
                            {
                                if (item.Name != string.Empty)
                                {
                                    if (item.Name != "MEDIPLUS")
                                    {
                                        PayrollDepartmentDetails objPayDeptDetails = new PayrollDepartmentDetails();
                                        objPayDeptDetails.Name = item.Name;
                                        objPayDeptDetails.PayrollDepartmentId = DepartmentId;
                                        objPayDeptDetails.PayrollId = PayrollId;
                                        if (item.Value2.Contains("("))
                                        {
                                            string Valuestr = item.Value2
                                                .Replace("$", string.Empty)
                                                .Replace("(", string.Empty)
                                                .Replace(")", string.Empty)
                                                .Replace(",", string.Empty);
                                            decimal Value = Convert.ToDecimal(Valuestr);
                                            objPayDeptDetails.Value = -Value;
                                        }
                                        else
                                        {
                                            string Valuestr = item.Value2.Replace("$", string.Empty).Replace(",", string.Empty);
                                            decimal Value = Convert.ToDecimal(Valuestr);
                                            objPayDeptDetails.Value = Value;
                                        }
                                        //This Db class is used for Add payroll Department Details.
                                        _PDFReadRepository.AddPayrollDepartmentDetails(objPayDeptDetails);
                                    }
                                }
                            }
                            else
                            {
                                PayrollDepartment objPayDept = new PayrollDepartment();
                                string[] DepartmentArr = item.Name.Split(':');
                                if (DepartmentArr.Count() > 1)
                                {
                                    string DepartmentName = DepartmentArr[1].Replace("Net Pay", string.Empty).ToString().Trim();
                                    int StoreId = Convert.ToInt32(Session["storeid"].ToString());
                                    var TrimDepartmentName = Get_ProductName(DepartmentName);
                                    //This Db class is used for Get payroll departments list.
                                    var DeptData = _PDFReadRepository.GetPayrollDepartmentsList(StoreId, TrimDepartmentName);
                                    if (DeptData.Count == 0)
                                    {
                                        objPayDept.StoreId = StoreId;
                                        objPayDept.DepartmentName = Get_ProductName(DepartmentName);
                                        //This Db class is used for Add payroll Departments.

                                        int PayrollDepartmentId = _PDFReadRepository.AddPayrollDepartments(objPayDept);
                                        objPayDept.PayrollDepartmentId = PayrollDepartmentId;
                                        DepartmentId = objPayDept.PayrollDepartmentId;
                                    }
                                    else
                                    {
                                        DepartmentId = DeptData.FirstOrDefault().PayrollDepartmentId;
                                    }
                                }
                            }
                        }
                    }
                    if (TableList.Count() > 0)
                    {
                        //This Db class is used for update payroll reports.

                        string PdfFileName = _PDFReadRepository.UpdatePayrollReports(obj);

                        var UserName = System.Web.HttpContext.Current.User.Identity.Name;
                        ActivityLog ActLog = new ActivityLog();
                        ActLog.Action = 1;
                        ActLog.Comment = "Payroll PDF " +
                            "<a href='/PDFRead/DownloadFile?filePath=" +
                            PdfFileName +
                            "'>" +
                            PdfFileName +
                            "</a> Uploaded by " +
                            //This Db class is used for get user firstname.
                            _CommonRepository.getUserFirstName(UserName) + " on " + AdminSiteConfiguration.GetDate(AdminSiteConfiguration.GetEasternTime(DateTime.Now).ToString());
                        //This  class is used for Activity Log Insert.
                        _ActivityLogRepository.ActivityLogInsert(ActLog);
                    }
                    ViewBag.PDFData = TableList;
                    return View("PDFRead", PDFPrintList.Where(s => s.Name != string.Empty));
                }
            }
            catch
            {
            }
            ViewBag.ErrorMessage = "PDF Not Uploaded, Upload it First";
            return View("Create");
        }

        /// <summary>
        /// This function is New ReadPayroll File .
        /// </summary>
        /// <param name="FileName"></param>
        /// <returns></returns>
        public int NewReadPayrollFile(string FileName)
        {
            int value = 0;
            try
            {
                if (FileName != string.Empty)
                {
                    int PayrollId = 0;
                    string FilePath = Server.MapPath("~/userfiles/PayrollFile/" + FileName);


                    Aspose.Pdf.Document pdfDocument = new Aspose.Pdf.Document(FilePath);
                    ExcelSaveOptions options = new ExcelSaveOptions();
                    options.Format = ExcelSaveOptions.ExcelFormat.XLSX;
                    string NewFilePath = Server.MapPath("~/userfiles/PdfToExcelFile/PdfToExcel.xlsx");
                    pdfDocument.Save(NewFilePath, options);


                    Aspose.Cells.Workbook wb = new Aspose.Cells.Workbook(NewFilePath);

                    WorksheetCollection collection = wb.Worksheets;
                    List<PDFTable> TableList = new List<PDFTable>();
                    bool flag = false;
                    string name = string.Empty;
                    DataTable dataTable = new DataTable();
                    int rows = 0;
                    int cols = 0;
                    string PeriodEndDate = string.Empty;
                    string PayDate = string.Empty;
                    int FileNo = 0;
                    int rownumber = 0;
                    for (int worksheetIndex = 0; worksheetIndex < collection.Count; worksheetIndex++)
                    {
                        Aspose.Cells.Worksheet worksheet = collection[worksheetIndex];

                        rows = worksheet.Cells.MaxDataRow;
                        cols = worksheet.Cells.MaxDataColumn;
                        dataTable = worksheet.Cells.ExportDataTable(0, 0, rows + 1, cols, false);
                        name = string.Empty;
                        flag = false;
                        foreach (DataRow r in dataTable.Rows)
                        {
                            if (!String.IsNullOrEmpty(r.Field<string>("Column1")))
                            {
                                if (r.Field<string>("Column1").ToLower().Contains("cash requirement summary"))
                                {
                                    FileNo = 1;
                                    break;
                                }
                                else
                                {
                                    FileNo = 2;
                                }
                            }
                            if (rownumber == 5)
                            {
                                break;
                            }
                            rownumber++;
                        }
                        foreach (DataRow r in dataTable.Rows)
                        {
                            if (FileNo == 2)
                            {
                                if (PeriodEndDate == string.Empty)
                                {
                                    if (!String.IsNullOrEmpty(r.Field<string>("Column2")))
                                    {
                                        if (r.Field<string>("Column2").Contains("Period Ending:"))
                                        {
                                            string[] DateSplitData = r.Field<string>("Column2").Split(':');
                                            if (DateSplitData.Count() > 0)
                                            {
                                                PeriodEndDate = DateSplitData[1];
                                                string[] DateFormate = PeriodEndDate.Split('/');
                                                if (DateFormate.Count() == 3)
                                                {
                                                    var Month = DateFormate[0].Trim().Length == 1
                                                        ? "0" + DateFormate[0].Trim()
                                                        : DateFormate[0].Trim();
                                                    PeriodEndDate = DateFormate[2].Split(' ')[0] +
                                                        "-" +
                                                        Month +
                                                        "-" +
                                                        DateFormate[1];
                                                }
                                            }
                                        }
                                    }
                                    if (!String.IsNullOrEmpty(r.Field<string>("Column3")))
                                    {
                                        if (r.Field<string>("Column3").Contains("Pay Date:"))
                                        {
                                            string[] DateSplitData = r.Field<string>("Column3").Split(':');
                                            if (DateSplitData.Count() > 0)
                                            {
                                                PayDate = DateSplitData[1];
                                                string[] DateFormate = PayDate.Split('/');
                                                if (DateFormate.Count() == 3)
                                                {
                                                    var Month = DateFormate[0].Trim().Length == 1
                                                        ? "0" + DateFormate[0].Trim()
                                                        : DateFormate[0].Trim();
                                                    PayDate = DateFormate[2].Trim() +
                                                        "-" +
                                                        Month +
                                                        "-" +
                                                        DateFormate[1].Trim();
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                Boolean IsCol3 = false;
                                if (PeriodEndDate == string.Empty)
                                {
                                    if (!String.IsNullOrEmpty(r.Field<string>("Column1")))
                                    {
                                        if (r.Field<string>("Column1").Contains("Period Ending"))
                                        {
                                            string[] DateSplitData = r.Field<string>("Column1").Split(':');
                                            if (DateSplitData.Length > 1)
                                            {
                                                if (DateSplitData[1].ToString() == "")
                                                {
                                                    IsCol3 = true;
                                                    DateSplitData = (r.Field<string>("Column1") + r.Field<string>("Column2")).Split(':');
                                                }
                                            }
                                            else if (DateSplitData.Length == 1)
                                            {
                                                IsCol3 = true;
                                                DateSplitData = (r.Field<string>("Column1") + ":" + r.Field<string>("Column2")).Split(':');
                                            }
                                            if (DateSplitData.Count() > 0)
                                            {
                                                PeriodEndDate = DateSplitData[1];
                                                string[] DateFormate = PeriodEndDate.Split('/');
                                                if (DateFormate.Count() == 3)
                                                {
                                                    var Month = DateFormate[0].Trim().Length == 1
                                                        ? "0" + DateFormate[0].Trim()
                                                        : DateFormate[0].Trim();
                                                    PeriodEndDate = DateFormate[2].Split(' ')[0] +
                                                        "-" +
                                                        Month +
                                                        "-" +
                                                        DateFormate[1];
                                                }
                                            }
                                        }
                                    }
                                    if (!String.IsNullOrEmpty(r.Field<string>("Column2")))
                                    {
                                        if (r.Field<string>("Column2").Contains("Pay Date:"))
                                        {
                                            string[] DateSplitData = r.Field<string>("Column2").Split(':');
                                            if (DateSplitData.Count() > 0)
                                            {
                                                PayDate = DateSplitData[1];
                                                string[] DateFormate = PayDate.Split('/');
                                                if (DateFormate.Count() == 3)
                                                {
                                                    var Month = DateFormate[0].Trim().Length == 1
                                                        ? "0" + DateFormate[0].Trim()
                                                        : DateFormate[0].Trim();
                                                    PayDate = DateFormate[2].Trim() +
                                                        "-" +
                                                        Month +
                                                        "-" +
                                                        DateFormate[1].Trim();
                                                }
                                            }
                                        }
                                    }
                                    if (!String.IsNullOrEmpty(r.Field<string>("Column3")) && IsCol3 == true)
                                    {
                                        if (r.Field<string>("Column3").Contains("Pay Date"))
                                        {
                                            string[] DateSplitData = r.Field<string>("Column3").Split(':');
                                            if (DateSplitData.Count() > 0)
                                            {
                                                PayDate = DateSplitData[1];
                                                string[] DateFormate = PayDate.Split('/');
                                                if (DateFormate.Count() == 3)
                                                {
                                                    var Month = DateFormate[0].Trim().Length == 1
                                                        ? "0" + DateFormate[0].Trim()
                                                        : DateFormate[0].Trim();
                                                    PayDate = DateFormate[2].Trim() +
                                                        "-" +
                                                        Month +
                                                        "-" +
                                                        DateFormate[1].Trim();
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        List<PDFTable> PDFPrintList = new List<PDFTable>();
                        PayrollMaster obj = new PayrollMaster();
                        int StoreId = Convert.ToInt32(Session["storeid"].ToString());
                        obj.StoreId = StoreId;
                        if (PeriodEndDate != string.Empty)
                        {
                            obj.StartDate = Convert.ToDateTime(PeriodEndDate).AddDays(-7);
                            obj.EndDate = Convert.ToDateTime(PeriodEndDate);
                        }
                        else
                        {
                            obj.StartDate = DateTime.Today;
                            obj.EndDate = DateTime.Today;
                        }
                        if (PayDate != string.Empty)
                        {
                            obj.EndCheckDate = Convert.ToDateTime(PayDate);
                        }
                        else
                        {
                            obj.EndCheckDate = DateTime.Today;
                        }
                        //This Db class is used for Get Payroll Master.

                        var FileExist = _PDFReadRepository.GetPayrollMasters(obj);
                        var PayReportIDList = FileExist.Select(s => s.PayrollReportId).ToArray();
                        //This Db class is used for Get Payroll Reports.

                        var CheckFileNo = _PDFReadRepository.GetPayrollReports(PayReportIDList, FileNo);

                        if (CheckFileNo.Count > 0)
                        {
                            value = 1;
                        }
                        break;
                    }
                    if (System.IO.File.Exists(NewFilePath))
                    {
                        System.IO.File.Delete(NewFilePath);
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return value;
        }

        private void ProcessExcelFile_Distributor(ref List<PDFTable> TableList, ref string PeriodEndDate, ref string PayDate)

        {
            string Columns = "";
            string NewFilePath = Server.MapPath("~/userfiles/PdfToExcelFile/PdfToExcel.xlsx");
            Aspose.Cells.Workbook wb = new Aspose.Cells.Workbook(NewFilePath);
            WorksheetCollection collection = wb.Worksheets;
            for (int worksheetIndex = 0; worksheetIndex < collection.Count; worksheetIndex++)
            {
                Aspose.Cells.Worksheet worksheet = collection[worksheetIndex];
                worksheet.Cells.DeleteRows(0, 3);
            }
            wb.Save(NewFilePath);
            bool flag = false;
            bool flag1 = false;
            string Name = "";
            string[] Col = null;
            int R = 0, O = 0, C = 0, G = 0, D = 0;
            int rows = 0;
            int cols = 0;
            int BRow = 0;
            int Gro = 0;
            int IsRemoved = 0;
            DataTable dataTable = new DataTable();
            for (int worksheetIndex = 0; worksheetIndex < collection.Count; worksheetIndex++)
            {
                Columns = "";
                Aspose.Cells.Worksheet worksheet = collection[worksheetIndex];
                rows = worksheet.Cells.MaxDataRow;
                cols = worksheet.Cells.MaxDataColumn;
                //ExportTableOptions etOpt = new ExportTableOptions();
                //options.ExportAsString = true;
                dataTable = worksheet.Cells.ExportDataTableAsString(0, 0, rows + 1, cols + 1, false);

                GetColumnNumber(ref dataTable, ref Columns, worksheetIndex);
                Col = Columns.Split(',');
                R = 0; O = 0; C = 0; G = 0; D = 0; BRow = 0;
                foreach (string st in Col)
                {
                    if (st.Contains("R"))
                    {
                        R = Convert.ToInt32(st.Split('^')[1].ToString()) + 1;
                    }
                    else if (st.Contains("O"))
                    {
                        O = Convert.ToInt32(st.Split('^')[1].ToString()) + 1;
                    }
                    else if (st.Contains("C"))
                    {
                        C = Convert.ToInt32(st.Split('^')[1].ToString()) + 1;
                    }
                    else if (st.Contains("G"))
                    {
                        G = Convert.ToInt32(st.Split('^')[1].ToString()) + 1;
                    }
                    else if (st.Contains("D"))
                    {
                        D = Convert.ToInt32(st.Split('^')[1].ToString()) + 1;
                    }
                }
                RemoveUnusedColumns(ref dataTable, ref G, ref IsRemoved);
                if (IsRemoved == 1)
                {
                    GetColumnNumber(ref dataTable, ref Columns, worksheetIndex);
                    Col = Columns.Split(',');
                    R = 0; O = 0; C = 0; G = 0; D = 0; BRow = 0;
                    foreach (string st in Col)
                    {
                        if (st.Contains("R"))
                        {
                            R = Convert.ToInt32(st.Split('^')[1].ToString()) + 1;
                        }
                        else if (st.Contains("O"))
                        {
                            O = Convert.ToInt32(st.Split('^')[1].ToString()) + 1;
                        }
                        else if (st.Contains("C"))
                        {
                            C = Convert.ToInt32(st.Split('^')[1].ToString()) + 1;
                        }
                        else if (st.Contains("G"))
                        {
                            G = Convert.ToInt32(st.Split('^')[1].ToString()) + 1;
                        }
                        else if (st.Contains("D"))
                        {
                            D = Convert.ToInt32(st.Split('^')[1].ToString()) + 1;
                        }
                    }
                }
                foreach (DataRow r in dataTable.Rows)
                {
                    if (PeriodEndDate == "")
                    {
                        ReadingPeriodEndDate(ref dataTable, ref PeriodEndDate, ref PayDate);
                    }

                    BRow += 1;

                    ReadingFile2(r, ref TableList, ref flag, ref Name, R, O, C, ref flag1, ref G, ref D);
                    if (flag1)
                        Gro = 1;
                    if (Gro == 1 && flag1 == false)
                    {
                        ReadNetAmounts(ref dataTable, BRow, ref TableList);
                        if (worksheetIndex == collection.Count - 1)
                            Gro = 0;
                        break;
                    }
                }
            }
        }

        private void RemoveUnusedColumns(ref DataTable dtMaster, ref int G, ref int IsRemoved)
        {
            foreach (DataRow r in dtMaster.Rows)
            {
                if (r.Field<string>("Column1").ToLower().Contains("company"))
                {
                    if (!String.IsNullOrEmpty(r.Field<string>("Column" + G.ToString())))
                    {
                        if (r.Field<string>("Column" + G.ToString()).Contains("CCFEE D") == true || r.Field<string>("Column" + G.ToString()).Contains("HOLIDAY") == true || r.Field<string>("Column" + G.ToString()).Contains("PAID SICK") == true || r.Field<string>("Column" + G.ToString()).Contains("SALARY") == true || r.Field<string>("Column" + G.ToString()).Contains("VACATION") == true || r.Field<string>("Column" + G.ToString()).Contains("CCTIP D") == true)
                        {
                            dtMaster.Columns.RemoveAt(G - 1);
                            //dtMaster.Columns.RemoveAt(G - 2);
                            dtMaster.AcceptChanges();
                            IsRemoved = 1;
                            int IsMatch = 0;
                            //int IsRename = 0;
                            foreach (DataColumn dc in dtMaster.Columns)
                            {
                                if (IsMatch == 1)
                                {
                                    dc.ColumnName = "Column" + G.ToString();
                                    //if (IsRename == 0)
                                    //{
                                    //    dtMaster.Rows[0]["Column" + G.ToString()] = "CODED";
                                    //    IsRename = 1;
                                    //}                                                                        
                                    G += 1;
                                    dtMaster.AcceptChanges();
                                }
                                if (dc.ColumnName == "Column" + (G - 1).ToString() && IsMatch == 0)
                                {
                                    IsMatch = 1;
                                    //G = G - 1;
                                }

                            }
                        }
                    }
                }
            }
        }

        private void ReadingPeriodEndDate(ref DataTable dt, ref string PeriodEndDate, ref string PayDate)
        {
            bool IsCol3 = false;
            for (int i = dt.Rows.Count - 1; i >= 0; i--)
            {
                if (PeriodEndDate == "")
                {
                    for (int j = 0; j < dt.Columns.Count; j++)
                    {
                        if (dt.Rows[i][j].ToString() != "")
                        {
                            if (dt.Rows[i][j].ToString().Contains("Period Ending"))
                            {
                                string[] DateSplitData = dt.Rows[i][j].ToString().Split(':');
                                if (DateSplitData.Length > 1)
                                {
                                    if (DateSplitData[1].ToString() == "")
                                    {
                                        IsCol3 = true;
                                        DateSplitData = (dt.Rows[i][j].ToString() + dt.Rows[i][j + 1].ToString()).Split(':');
                                    }
                                }
                                else if (DateSplitData.Length == 1)
                                {
                                    IsCol3 = true;
                                    DateSplitData = (dt.Rows[i][j].ToString() + ":" + dt.Rows[i][j + 1].ToString()).Split(':');
                                }
                                if (DateSplitData.Count() > 0)
                                {
                                    PeriodEndDate = DateSplitData[1];
                                    string[] DateFormate = PeriodEndDate.Split('/');
                                    if (DateFormate.Count() == 3)
                                    {
                                        var Month = DateFormate[0].Trim().Length == 1 ? "0" + DateFormate[0].Trim() : DateFormate[0].Trim();
                                        PeriodEndDate = DateFormate[2].Split(' ')[0] + "-" + Month + "-" + DateFormate[1];
                                    }
                                }
                            }
                            else if (dt.Rows[i][j].ToString().Contains("Pay Date"))
                            {
                                string[] DateSplitData = dt.Rows[i][j].ToString().Split(':');
                                if (DateSplitData.Count() > 0)
                                {
                                    PayDate = DateSplitData[1];
                                    string[] DateFormate = PayDate.Split('/');
                                    if (DateFormate.Count() == 3)
                                    {
                                        var Month = DateFormate[0].Trim().Length == 1 ? "0" + DateFormate[0].Trim() : DateFormate[0].Trim();
                                        PayDate = DateFormate[2].Trim() + "-" + Month + "-" + DateFormate[1].Trim();
                                    }
                                }
                            }
                        }
                    }
                }
                else
                    break;
            }
        }

        private bool CheckingGrossreading(ref DataRow dr)
        {
            bool IsEnd = false;
            for (int i = 0; i < dr.ItemArray.Length; i++)
            {
                if (dr[i].ToString().Contains("TOTALS"))
                {
                    IsEnd = true;
                    break;
                }
            }
            return IsEnd;
        }

        private void ProcessExcelFile_CASHREQUIREMENT(ref List<PDFTable> TableList, ref string PeriodEndDate, ref string PayDate)
        {
            try
            {
                bool flag = false;
                int rows = 0, cols = 0;
                string NewFilePath = Server.MapPath("~/userfiles/PdfToExcelFile/PdfToExcel.xlsx");
                Aspose.Cells.Workbook wb = new Aspose.Cells.Workbook(NewFilePath);
                WorksheetCollection collection = wb.Worksheets;
                for (int worksheetIndex = 0; worksheetIndex < collection.Count; worksheetIndex++)
                {
                    Aspose.Cells.Worksheet worksheet = collection[worksheetIndex];
                    worksheet.Cells.DeleteRows(0, 4);
                }
                wb.Save(NewFilePath);
                DataTable dataTable = new DataTable();
                DataTable dataTableRows = new DataTable();
                for (int worksheetIndex = 0; worksheetIndex < collection.Count; worksheetIndex++)
                {
                    Aspose.Cells.Worksheet worksheet = collection[worksheetIndex];
                    rows = worksheet.Cells.MaxDataRow;
                    cols = worksheet.Cells.MaxDataColumn;
                    try
                    {
                        dataTableRows = worksheet.Cells.ExportDataTable(0, 0, rows, cols, false);
                        dataTable = worksheet.Cells.ExportDataTable(0, 0, rows + 1, cols - 1, false);
                    }
                    catch (Exception ex)
                    {
                        try
                        {
                            dataTable = worksheet.Cells.ExportDataTable(0, 0, rows + 1, cols - 1, false);
                        }
                        catch (Exception ex1)
                        {
                            dataTable = worksheet.Cells.ExportDataTable(0, 0, rows + 1, cols - 2, false);
                        }
                    }
                    try
                    {
                        if (PeriodEndDate == "")
                        {
                            ReadingPeriodEndDate(ref dataTable, ref PeriodEndDate, ref PayDate);
                        }
                    }
                    catch (Exception ex)
                    {
                    }
                    foreach (DataRow r in dataTableRows.Rows)
                    {
                        try
                        {
                            if (!String.IsNullOrEmpty(r.Field<string>("Column1")))
                            {
                                if (r.Field<string>("Column1").ToLower() == "total paycor filing responsibility")
                                {
                                    flag = false;
                                }
                                if (flag)
                                {
                                    if (!String.IsNullOrEmpty(r.Field<string>("Column2")))
                                    {
                                        TableList.Add(new PDFTable { Name = r.Field<string>("Column1"), Value = r.Field<string>("Column2").Replace('(', '-').Replace(')', ' ').Replace(" ", "").Replace(",", "").Trim(), Name2 = "File1", Name3 = "EMPLOYER TAX LIABILITY" });
                                    }
                                }
                                if (r.Field<string>("Column1").ToLower() == "paycor filing responsibility")
                                {
                                    flag = true;
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                        }
                    }
                    if (flag)
                    {
                        foreach (DataRow r in dataTableRows.Rows)
                        {
                            try
                            {
                                if (r[3].ToString() != "")
                                {
                                    if (r[3].ToString().ToLower() == "total paycor filing responsibility")
                                    {
                                        flag = false;
                                        break;
                                    }
                                    if (flag)
                                    {
                                        if (r[4].ToString() != "")
                                        {
                                            TableList.Add(new PDFTable { Name = r[3].ToString(), Value = r[4].ToString().Replace('(', '-').Replace(')', ' ').Replace(" ", "").Replace(",", "").Trim(), Name2 = "File1", Name3 = "EMPLOYER TAX LIABILITY" });
                                        }
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {


            }

        }

        private void ReadingFile2(DataRow r, ref List<PDFTable> TableList, ref Boolean flag, ref string name, int Reg, int OT, int Coded, ref Boolean flag1, ref int G, ref int D)
        {
            int oCount = 0;
            if (!String.IsNullOrEmpty(r.Field<string>("Column1")))
            {
                flag = false;
            }
            if (CheckingGrossreading(ref r))
            {
                flag1 = false;
            }
            if (flag == true)
            {
                if (Reg > 0)
                    if (!String.IsNullOrEmpty(r.Field<string>("Column" + Reg.ToString())))
                    {
                        if (r.Field<string>("Column" + Reg.ToString()) != "REG")
                            TableList.Add(new PDFTable { Name = "REGULAR", Value = r.Field<string>("Column" + Reg.ToString()).Replace('(', '-').Replace(')', ' ').Replace(" ", "").Replace(",", "").Trim(), Name2 = "File2", Name3 = name });

                    }
                if (OT > 0)
                    if (!String.IsNullOrEmpty(r.Field<string>("Column" + OT.ToString())))
                    {
                        if (r.Field<string>("Column" + OT.ToString()) != "OT")
                            TableList.Add(new PDFTable { Name = "OVERTIME", Value = r.Field<string>("Column" + OT.ToString()).Replace('(', '-').Replace(')', ' ').Replace(" ", "").Replace(",", "").Trim(), Name2 = "File2", Name3 = name });
                    }
                if (Coded > 0)
                    if (!String.IsNullOrEmpty(r.Field<string>("Column" + Coded.ToString())))
                    {
                        if (r.Field<string>("Column" + Coded.ToString()) != "CODED")
                        {
                            string inputString = r.Field<string>("Column" + Coded.ToString()).Replace('(', '-').Replace(')', ' ').Replace(" ", "").Replace(",", "").Trim();
                            try
                            {
                                double Val = Convert.ToDouble(inputString);
                                if (!String.IsNullOrEmpty(r.Field<string>("Column" + (Coded + 1).ToString())))
                                    inputString = Val.ToString() + r.Field<string>("Column" + (Coded + 1).ToString()).Replace('(', '-').Replace(')', ' ').Replace(" ", "").Replace(",", "").Trim();
                                else
                                    inputString = Val.ToString();
                            }
                            catch (Exception ex)
                            {
                            }
                            string pattern = @"([0-9,.-]+)([A-Za-z]+)";
                            Match match = Regex.Match(inputString, pattern);

                            if (match.Success)
                            {
                                string numericPart = match.Groups[1].Value;
                                string textPart = match.Groups[2].Value;
                                TableList.Add(new PDFTable { Name = textPart.Replace(" ", "_").Trim(), Value = numericPart, Name2 = "File2", Name3 = name });

                            }
                        }
                    }
            }

            int ReCheck = 0;

            if (flag1 == true)
            {
                if (G > 0)
                {
                    if (!String.IsNullOrEmpty(r.Field<string>("Column" + G.ToString())))
                    {
                        string inputString = r.Field<string>("Column" + G.ToString()).Replace('(', '-').Replace(')', ' ').Replace(" ", "").Replace(",", "").Trim();
                        try
                        {
                            decimal Val = Convert.ToDecimal(inputString);
                            if (TableList.AsEnumerable().Where(s => s.Name == "GROSS PAY" && s.Name3 == "COMPANY TOTAL" && s.Name2 == "File2").Count() == 1)
                            {
                                var Val1 = TableList.AsEnumerable().Where(s => s.Name == "GROSS PAY" && s.Name3 == "COMPANY TOTAL" && s.Name2 == "File2").FirstOrDefault().Value;
                                decimal Amt = Convert.ToDecimal(Val1) + Val;
                                var f = TableList.AsEnumerable().Where(s => s.Name == "GROSS PAY" && s.Name3 == "COMPANY TOTAL" && s.Name2 == "File2").FirstOrDefault();
                                TableList.Remove(f);
                                TableList.Add(new PDFTable { Name = "GROSS PAY", Value = Amt.ToString(), Name2 = "File2", Name3 = "COMPANY TOTAL" });
                            }
                            else
                                TableList.Add(new PDFTable { Name = "GROSS PAY", Value = r.Field<string>("Column" + G.ToString()).Replace('(', '-').Replace(')', ' ').Replace(" ", "").Replace(",", "").Trim(), Name2 = "File2", Name3 = "COMPANY TOTAL" });
                        }
                        catch (Exception ex)
                        {
                        }
                    }
                }
                if (D > 0)
                {
                Retry:
                    if (!String.IsNullOrEmpty(r.Field<string>("Column" + D.ToString())))
                    {
                        string inputString = r.Field<string>("Column" + D.ToString()).Replace('(', '-').Replace(')', ' ').Replace(" ", "").Replace(",", "").Trim();
                        try
                        {
                            double Val = Convert.ToDouble(inputString);
                            if (!String.IsNullOrEmpty(r.Field<string>("Column" + (D + 1).ToString())))
                                inputString = Val.ToString() + r.Field<string>("Column" + (D + 1).ToString()).Replace('(', '-').Replace(')', ' ').Replace(" ", "").Replace(",", "").Trim();
                            else
                                inputString = Val.ToString();
                        }
                        catch (Exception ex)
                        {
                            if (inputString.Contains(".") == false)
                            {
                                if (!String.IsNullOrEmpty(r.Field<string>("Column" + (D - 1).ToString())))
                                    inputString = r.Field<string>("Column" + (D - 1).ToString()).Replace('(', '-').Replace(')', ' ').Replace(" ", "").Replace(",", "").Trim() + inputString;
                                else
                                    inputString = inputString.ToString();
                            }
                        }
                        string pattern = @"([0-9,.-]+)([A-Za-z]+)";
                        Match match = Regex.Match(inputString, pattern);

                        if (match.Success)
                        {
                            string numericPart = match.Groups[1].Value;
                            string textPart = match.Groups[2].Value;
                            oCount = TableList.Count;
                            AddDeductionInfo(textPart, numericPart, ref TableList);
                            if (oCount == TableList.Count)
                            {
                                D = D + 1;
                                goto Retry;
                            }
                        }
                    }
                    else if (!String.IsNullOrEmpty(r.Field<string>("Column" + (D + 1).ToString())))
                    {
                        string inputString = r.Field<string>("Column" + (D + 1).ToString()).Replace('(', '-').Replace(')', ' ').Replace(" ", "").Replace(",", "").Trim();
                        try
                        {
                            double Val = Convert.ToDouble(inputString);
                            if (!String.IsNullOrEmpty(r.Field<string>("Column" + (D + 2).ToString())))
                                inputString = Val.ToString() + r.Field<string>("Column" + (D + 2).ToString()).Replace('(', '-').Replace(')', ' ').Replace(" ", "").Replace(",", "").Trim();
                            else
                                inputString = Val.ToString();
                        }
                        catch (Exception ex)
                        {
                        }
                        string pattern = @"([0-9,.-]+)([A-Za-z]+)";
                        Match match = Regex.Match(inputString, pattern);

                        if (match.Success)
                        {
                            string numericPart = match.Groups[1].Value;
                            string textPart = match.Groups[2].Value;
                            AddDeductionInfo(textPart, numericPart, ref TableList);
                        }
                    }
                    else
                    {
                        int IsUpdateValue = 0;
                        for (int i = 0; i < r.ItemArray.Count() - 1; i++)
                        {
                            if (r[i].ToString().Contains("REG") == true || r[i].ToString().Contains("OT") == true || r[i].ToString().Contains("CODED") == true)
                            {
                                IsUpdateValue = 0;
                                break;
                            }
                            else
                            {
                                IsUpdateValue = 1;
                            }
                        }
                        if (IsUpdateValue == 1)
                        {
                            GetColumnPositionReverseOrder_D(r, ref D);
                            if (ReCheck == 0)
                            {
                                ReCheck = 1;
                                goto Retry;
                            }
                        }
                    }
                }
            }


            if (!String.IsNullOrEmpty(r.Field<string>("Column1")))
            {

                if (r.Field<string>("Column1").ToLower().Contains("management"))
                {
                    name = "MANAGEMENT";
                }
                else if (r.Field<string>("Column1").ToLower().Contains("general clerk"))
                {
                    name = "GENERAL CLERK";
                }
                else if (r.Field<string>("Column1").ToLower().Contains("grocery"))
                {
                    name = "GROCERY";
                }
                else if (r.Field<string>("Column1").ToLower().Contains("prepared foods"))
                {
                    name = "PREPARED FOODS";
                }
                else if (r.Field<string>("Column1").ToLower().Contains("bakery"))
                {
                    name = "BAKERY";
                }
                else if (r.Field<string>("Column1").ToLower().Contains("kitchen"))
                {
                    name = "KITCHEN";
                }
                else if (r.Field<string>("Column1").ToLower().Contains("dairy"))
                {
                    name = "DAIRY";
                }
                else if (r.Field<string>("Column1").ToLower().Contains("frozen foods"))
                {
                    name = "FROZEN FOODS";
                }
                else if (r.Field<string>("Column1").ToLower().Contains("chesse"))
                {
                    name = "CHESSE";
                }
                else if (r.Field<string>("Column1").ToLower().Contains("cashier"))
                {
                    name = "CASHIER";
                }
                else if (r.Field<string>("Column1").ToLower().Contains("produce"))
                {
                    name = "PRODUCE";
                }
                else if (r.Field<string>("Column1").ToLower().Contains("cheese"))
                {
                    name = "CHEESE";
                }
                else if (r.Field<string>("Column1").ToLower().Contains("salad bar"))
                {
                    name = "SALAD BAR";
                }
                else if (r.Field<string>("Column1").ToLower().Contains("maintenance"))
                {
                    name = "MAINTENANCE";
                }
                else if (r.Field<string>("Column1").ToLower().Contains("meat"))
                {
                    name = "MEAT";
                }
                else if (r.Field<string>("Column1").ToLower().Contains("delivery"))
                {
                    name = "DELIVERY";
                }
                else if (r.Field<string>("Column1").ToLower().Contains("office"))
                {
                    name = "OFFICE";
                }
                else if (r.Field<string>("Column1").ToLower().Contains("security"))
                {
                    name = "SECURITY";
                }
                else if (r.Field<string>("Column1").ToLower().Contains("fish"))
                {
                    name = "FISH";
                }
                else if (r.Field<string>("Column1").ToLower().Contains("office"))
                {
                    name = "OFFICE";
                }
                else if (r.Field<string>("Column1").ToLower().Contains("deli"))
                {
                    name = "DELI";
                }
                else if (r.Field<string>("Column1").ToLower().Contains("online shoppers"))
                {
                    name = "ONLINE SHOPPERS";
                }

                if (r.Field<string>("Column1").ToLower() == "department totals")
                {
                    if (Reg > 0)
                    {
                        if (!String.IsNullOrEmpty(r.Field<string>("Column" + Reg.ToString())))
                        {
                            TableList.Add(new PDFTable { Name = "REGULAR", Value = r.Field<string>("Column" + Reg.ToString()).Replace('(', '-').Replace(')', ' ').Replace(" ", "").Replace(",", "").Trim(), Name2 = "File2", Name3 = name });
                        }
                    }
                    if (OT > 0)
                    {
                        if (!String.IsNullOrEmpty(r.Field<string>("Column" + OT.ToString())))
                        {
                            TableList.Add(new PDFTable { Name = "OVERTIME", Value = r.Field<string>("Column" + OT.ToString()).Replace('(', '-').Replace(')', ' ').Replace(" ", "").Replace(",", "").Trim(), Name2 = "File2", Name3 = name });
                        }
                    }
                    if (Coded > 0)
                    {
                        if (!String.IsNullOrEmpty(r.Field<string>("Column" + Coded.ToString())))
                        {
                            string inputString = r.Field<string>("Column" + Coded.ToString()).Replace('(', '-').Replace(')', ' ').Replace(" ", "").Replace(",", "").Trim();
                            try
                            {
                                double Val = Convert.ToDouble(inputString);
                                if (!String.IsNullOrEmpty(r.Field<string>("Column" + (Coded + 1).ToString())))
                                    inputString = Val.ToString() + r.Field<string>("Column" + (Coded + 1).ToString()).Replace('(', '-').Replace(')', ' ').Replace(" ", "").Replace(",", "").Trim();
                                else
                                    inputString = Val.ToString();
                            }
                            catch (Exception ex)
                            {
                            }
                            string pattern = @"([0-9,.-]+)([A-Za-z]+)";
                            Match match = Regex.Match(inputString, pattern);

                            if (match.Success)
                            {
                                string numericPart = match.Groups[1].Value;
                                string textPart = match.Groups[2].Value;
                                TableList.Add(new PDFTable { Name = textPart.Replace(" ", "_").Trim(), Value = numericPart, Name2 = "File2", Name3 = name });

                            }
                        }
                    }
                    flag = true;
                }

                // Check Company Total
                if (r.Field<string>("Column1").ToLower().Contains("company"))
                {
                    if (G > 0)
                    {
                    ReTry:
                        if (!String.IsNullOrEmpty(r.Field<string>("Column" + G.ToString())))
                        {
                            string inputString = r.Field<string>("Column" + G.ToString()).Replace('(', '-').Replace(')', ' ').Replace(" ", "").Replace(",", "").Trim();
                            try
                            {
                                double Val = Convert.ToDouble(inputString);
                                TableList.Add(new PDFTable { Name = "GROSS PAY", Value = r.Field<string>("Column" + G.ToString()).Replace('(', '-').Replace(')', ' ').Replace(" ", "").Replace(",", "").Trim(), Name2 = "File2", Name3 = "COMPANY TOTAL" });
                            }
                            catch (Exception ex)
                            {
                                GetColumnPositionReverseOrder(r, ref G, ref D);
                                goto ReTry;
                                //TableList.Add(new PDFTable { Name = "GROSS PAY", Value = r.Field<string>("Column" + G.ToString()).Replace('(', '-').Replace(')', ' ').Replace(" ", "").Replace(",", "").Trim(), Name2 = "File2", Name3 = "COMPANY TOTAL" });
                            }
                        }
                        else
                        {
                            GetColumnPositionReverseOrder(r, ref G, ref D);
                            goto ReTry;
                        }
                    }
                    if (D > 0)
                    {
                        if (!String.IsNullOrEmpty(r.Field<string>("Column" + D.ToString())))
                        {
                            string inputString = r.Field<string>("Column" + D.ToString()).Replace('(', '-').Replace(')', ' ').Replace(" ", "").Replace(",", "").Trim();
                            try
                            {
                                Decimal Val = Convert.ToDecimal(inputString);
                                if (!String.IsNullOrEmpty(r.Field<string>("Column" + (D + 1).ToString())))
                                    inputString = Val.ToString() + r.Field<string>("Column" + (D + 1).ToString()).Replace('(', '-').Replace(')', ' ').Replace(" ", "").Replace(",", "").Trim();
                                else
                                    inputString = Val.ToString();
                            }
                            catch (Exception ex)
                            {
                                if (inputString.Contains(".") == false)
                                {
                                    if (!String.IsNullOrEmpty(r.Field<string>("Column" + (D - 1).ToString())))
                                        inputString = r.Field<string>("Column" + (D - 1).ToString()).Replace('(', '-').Replace(')', ' ').Replace(" ", "").Replace(",", "").Trim() + inputString;
                                    else
                                        inputString = inputString.ToString();
                                }
                            }
                            string pattern = @"([0-9,.-]+)([A-Za-z-]+)";
                            Match match = Regex.Match(inputString, pattern);

                            if (match.Success)
                            {
                                string numericPart = match.Groups[1].Value;
                                string textPart = match.Groups[2].Value;
                                if (textPart == "K-ROTH")
                                {
                                    numericPart = numericPart.Replace("401", "");
                                    textPart = "401" + textPart;
                                }


                                AddDeductionInfo(textPart, numericPart, ref TableList);
                            }
                        }
                        else if (!String.IsNullOrEmpty(r.Field<string>("Column" + (D + 1).ToString())))
                        {
                            string inputString = r.Field<string>("Column" + (D + 1).ToString()).Replace('(', '-').Replace(')', ' ').Replace(" ", "").Replace(",", "").Trim();
                            try
                            {
                                double Val = Convert.ToDouble(inputString);
                                if (!String.IsNullOrEmpty(r.Field<string>("Column" + (D + 2).ToString())))
                                    inputString = Val.ToString() + r.Field<string>("Column" + (D + 2).ToString()).Replace('(', '-').Replace(')', ' ').Replace(" ", "").Replace(",", "").Trim();
                                else
                                    inputString = Val.ToString();
                            }
                            catch (Exception ex)
                            {
                            }
                            string pattern = @"([0-9,.-]+)([A-Za-z]+)";
                            Match match = Regex.Match(inputString, pattern);

                            if (match.Success)
                            {
                                string numericPart = match.Groups[1].Value;
                                string textPart = match.Groups[2].Value;
                                AddDeductionInfo(textPart, numericPart, ref TableList);
                            }
                        }
                    }
                    flag1 = true;
                }

            }
        }

        private int CheckColumnValues(DataRow dr)
        {
            int IsRead = 0;
            if (dr[dr.ItemArray.Length - 1].ToString() != "")
            {
                for (int i = dr.ItemArray.Length - 2; i >= 0; i--)
                {
                    if (dr[i].ToString() == "")
                        IsRead += 1;
                    else
                        break;
                }
            }
            return IsRead;
        }

        private void GetColumnPositionReverseOrder(DataRow dr, ref int G, ref int D)
        {
            int k = 0;
            if (CheckColumnValues(dr) > 1)
            {
                for (int i = dr.ItemArray.Length - 1; i >= 0; i--)
                {
                    if (dr[i].ToString() != "")
                    {
                        if (k == 0)
                            k = i;
                        else
                        {
                            dr[i + 1] = dr[k].ToString();
                            dr[k] = "";
                            break;
                        }
                    }
                }
            }
            for (int i = dr.ItemArray.Length - 1; i >= 0; i--)
            {
                if (dr[i].ToString() != "")
                {
                    if (dr[i].ToString().Replace('(', ' ').Replace(')', ' ').Replace(" ", "").Replace(",", "").Replace(".", "").Trim().All(char.IsNumber))
                    {
                        if (dr[i - 1].ToString().Replace('(', ' ').Replace(')', ' ').Replace(" ", "").Replace(",", "").Replace(".", "").Trim().All(char.IsNumber) == false && dr[i - 1].ToString() != "")
                            D = i - 1 + 1;
                        if (dr[i - 2].ToString().Replace('(', ' ').Replace(')', ' ').Replace(" ", "").Replace(",", "").Replace(".", "").Trim().All(char.IsNumber) == true && dr[i - 2].ToString() != "")
                        {
                            G = i - 2 + 1;
                        }
                        else
                        {
                            G = i - 3 + 1;
                        }
                    }
                    break;
                }
            }
        }

        private void GetColumnPositionReverseOrder_D(DataRow dr, ref int D)
        {
            for (int i = dr.ItemArray.Length - 1; i >= 0; i--)
            {
                if (dr[i].ToString() != "")
                {

                    if (dr[i].ToString().Replace('(', ' ').Replace(')', ' ').Replace(" ", "").Replace(",", "").Replace(".", "").Trim().All(char.IsNumber) == false)
                    {
                        D = i + 1;
                        break;
                    }
                }
            }
        }

        private void AddDeductionInfo(string Name, string Amount, ref List<PDFTable> TableList)
        {
            try
            {
                if (Amount.Contains("."))
                {
                    string[] amt = Amount.Split('.');
                    if (amt[1].ToString().Length > 2)
                    {
                        Amount = amt[0].ToString() + "." + amt[1].ToString().Substring(0, 2);
                        Name = amt[1].ToString().Substring(2, amt[1].ToString().Length - 2) + Name;
                    }
                }
            }
            catch (Exception ex)
            {


            }



            if (Name.Contains("AFLAC"))
            {
                TableList.Add(new PDFTable { Name = "AFLAC", Value = Amount.Replace('(', '-').Replace(')', ' ').Replace("AFLAC", ""), Name2 = "File2", Name3 = "COMPANY TOTAL", Name4 = "EtaxCalc", Value4 = "2" });
            }
            else if (Name.Contains("LIFE INS"))
            {
                TableList.Add(new PDFTable { Name = "LIFE INS", Value = Amount.Replace('(', '-').Replace(')', ' ').Replace("LIFE INS._", "").Replace("LIFE INS.", "").Replace("LIFE INS", ""), Name2 = "File2", Name3 = "COMPANY TOTAL", Name4 = "EtaxCalc", Value4 = "2" });
            }
            else if (Name.Contains("LIFEINS"))
            {
                TableList.Add(new PDFTable { Name = "LIFE INS", Value = Amount.Replace('(', '-').Replace(')', ' ').Replace("LIFEINS._", "").Replace("LIFEINS.", "").Replace("LIFEINS", ""), Name2 = "File2", Name3 = "COMPANY TOTAL", Name4 = "EtaxCalc", Value4 = "2" });
            }
            else if (Name.Contains("MED"))
            {
                TableList.Add(new PDFTable { Name = "MED-125", Value = Amount.Replace('(', '-').Replace(')', ' ').Replace("MED-125", ""), Name2 = "File2", Name3 = "COMPANY TOTAL", Name4 = "EtaxCalc", Value4 = "2" });
            }
            else if (Name.Contains("MISC"))
            {
                TableList.Add(new PDFTable { Name = "MISC", Value = Amount.Replace('(', '-').Replace(')', ' ').Replace("MISC.", "").Replace("MISC", ""), Name2 = "File2", Name3 = "COMPANY TOTAL", Name4 = "EtaxCalc", Value4 = "2" });
            }
            else if (Name.Contains("NYPFL"))
            {
                if (Name.Contains("EENYPFL"))
                    TableList.Add(new PDFTable { Name = "EENYPFL", Value = Amount.Replace('(', '-').Replace(')', ' ').Replace("EENYPFL", ""), Name2 = "File2", Name3 = "COMPANY TOTAL", Name4 = "EtaxCalc", Value4 = "2" });
                else
                    TableList.Add(new PDFTable { Name = "NYPFL", Value = Amount.Replace('(', '-').Replace(')', ' ').Replace("NYPFL", ""), Name2 = "File2", Name3 = "COMPANY TOTAL", Name4 = "EtaxCalc", Value4 = "2" });
            }
            else if (Name.Contains("STDIS"))
            {
                TableList.Add(new PDFTable { Name = "STDIS", Value = Amount.Replace('(', '-').Replace(')', ' ').Replace("STDIS", ""), Name2 = "File2", Name3 = "COMPANY TOTAL", Name4 = "EtaxCalc", Value4 = "2" });
            }
            else if (Name.Contains("SDINY"))
            {
                TableList.Add(new PDFTable { Name = "SDINY", Value = Amount.Replace('(', '-').Replace(')', ' ').Replace("SDINY", ""), Name2 = "File2", Name3 = "COMPANY TOTAL", Name4 = "EtaxCalc", Value4 = "2" });
            }
            else if (Name.Contains("401K2") == true || Name.Contains("401K") == true)
            {
                TableList.Add(new PDFTable { Name = "401K2", Value = Amount.Replace('(', '-').Replace(')', ' ').Replace("401K2", "").Replace("401K", ""), Name2 = "File2", Name3 = "COMPANY TOTAL", Name4 = "EtaxCalc", Value4 = "2" });
            }
            else if (Name.Contains("CSDispNY"))
            {
                TableList.Add(new PDFTable { Name = "CSDispNY", Value = Amount.Replace('(', '-').Replace(')', ' ').Replace("CSDispNY", ""), Name2 = "File2", Name3 = "COMPANY TOTAL", Name4 = "EtaxCalc", Value4 = "2" });
            }
            else if (Name.Contains("DD1") || Name.Contains("DD"))
            {
                TableList.Add(new PDFTable { Name = "DD1", Value = Amount.Replace('(', '-').Replace(')', ' ').Replace("DD1", ""), Name2 = "File2", Name3 = "COMPANY TOTAL", Name4 = "EtaxCalc", Value4 = "2" });
            }
            else if (Name.Contains("ROTH401K%") == true || Name.Contains("ROTH") == true)
            {
                TableList.Add(new PDFTable { Name = "ROTH401K%", Value = Amount.Replace('(', '-').Replace(')', ' ').Replace("ROTH401K%", "").Replace("ROTH", ""), Name2 = "File2", Name3 = "COMPANY TOTAL", Name4 = "EtaxCalc", Value4 = "2" });
            }
            else if (Name.Contains("CSDispCT"))
            {
                TableList.Add(new PDFTable { Name = "CSDispCT", Value = Amount.Replace('(', '-').Replace(')', ' ').Replace("CSDispCT", ""), Name2 = "File2", Name3 = "COMPANY TOTAL", Name4 = "EtaxCalc", Value4 = "2" });
            }
        }

        private void GetColumnNumber(ref DataTable dt, ref string Columns, int PageNumber)
        {
            DataRow dr = dt.Rows[0];
            bool IsContains = false;
            string LastCode = "";
            int OT = 0;
            int REG = 0;
            int G = 0;
            int Coded = 0;
            for (int i = 1; i < dt.Columns.Count; i++)
            {
                switch (dr[i].ToString().ToLower())
                {
                    case "reg":
                        REG += 1;
                        if (REG > 1)
                        {
                            LastCode = "R";
                            IsContains = CheckColumnHasValue(ref dt, i, "REG");
                            if (IsContains)
                            {
                                if (Columns == "")
                                    Columns = "R" + "^" + i.ToString();
                                else
                                    Columns += "," + "R" + "^" + i.ToString();
                            }
                        }
                        break;
                    case "ot":
                        OT += 1;
                        if (OT > 1)
                        {
                            LastCode = "O";
                            IsContains = CheckColumnHasValue(ref dt, i, "OT");
                            if (IsContains)
                            {
                                if (Columns == "")
                                    Columns = "O" + "^" + i.ToString();
                                else
                                    Columns += "," + "O" + "^" + i.ToString();
                            }
                        }
                        break;
                    case "coded":
                        if (OT == 2)
                        {
                            LastCode = "C";
                            IsContains = CheckColumnHasValue(ref dt, i, "CODED");
                            if (IsContains)
                            {
                                Coded += 1;
                                if (Columns == "")
                                    Columns = "C" + "^" + i.ToString();
                                else
                                    Columns += "," + "C" + "^" + i.ToString();
                            }
                            else
                                Coded += 1;
                        }
                        break;
                    default:
                        if (IsContains == false && dr[i].ToString().ToLower() == "" && Columns != "" && LastCode != "C")
                        {
                            IsContains = CheckColumnHasValue(ref dt, i, "");
                            if (IsContains)
                            {
                                if (Columns == "")
                                    Columns = LastCode + "^" + i.ToString();
                                else
                                    Columns += "," + LastCode + "^" + i.ToString();
                                LastCode = "";
                            }
                        }
                        else if (Coded > 0)
                        {
                            Columns += ",G^" + i.ToString();
                            Coded = 0;
                            G += 1;
                        }
                        else if (G == 1)
                            G += 1;
                        else if (G > 1)
                        {
                            Columns += ",D^" + i.ToString();
                            G = 0;
                        }
                        break;
                }
            }
        }

        private bool CheckColumnHasValue(ref DataTable dt, int i, string Word)
        {
            bool IsContains = false;
            for (int j = 1; j < dt.Rows.Count - 1; j++)
            {
                if (dt.Rows[j][i].ToString().Trim() != "" && dt.Rows[j][i].ToString().Trim().ToLower() != Word.ToLower())
                {
                    IsContains = true;
                    break;
                }
            }
            return IsContains;
        }

        private void ReadNetAmounts(ref DataTable dt, int r, ref List<PDFTable> TableList)
        {
            string Name = "";
            int col = 0;
            col = GetColumnPosition(ref dt, ref r, ref Name);
            if (col >= 0)
            {
                r = r + 1;
                ReadBankAccounts(col, ref r, Name, ref TableList, ref dt);
            }

            col = GetColumnPosition(ref dt, ref r, ref Name);
            if (col >= 0)
            {
                r = r + 1;
                ReadBankAccounts(col, ref r, Name, ref TableList, ref dt);
            }

            col = GetColumnPosition(ref dt, ref r, ref Name);
            if (col >= 0)
            {
                r = r + 1;
                ReadBankAccounts(col, ref r, Name, ref TableList, ref dt);
            }

            col = GetColumnPosition(ref dt, ref r, ref Name);
            if (col >= 0)
            {
                r = r + 1;
                ReadBankAccounts(col, ref r, Name, ref TableList, ref dt);
            }

            col = GetColumnPosition(ref dt, ref r, ref Name);
            if (col >= 0)
            {
                r = r + 1;
                ReadBankAccounts(col, ref r, Name, ref TableList, ref dt);
            }

        }

        private int GetColumnPosition(ref DataTable dt, ref int r, ref string Name)
        {
            int col = 0;
            for (int i = r; i < dt.Rows.Count; i++)
            {
                for (int c = 0; c < dt.Columns.Count; c++)
                {
                    if (dt.Rows[i][c].ToString().Contains("Net Checks"))
                    {
                        col = c;
                        Name = "NET CHECKS";
                        r = i;
                        goto cont;
                    }
                    else if (dt.Rows[i][c].ToString().Contains("Net DD"))
                    {
                        col = c;
                        Name = "DIRECT DEPOSITS";
                        r = i;
                        goto cont;
                    }
                    else if (dt.Rows[i][c].ToString().Contains("E-Child Support") == true || dt.Rows[i][c].ToString().Contains("E - Child Support") == true)
                    {
                        col = c;
                        Name = "CHILD SUPPORT";
                        r = i;
                        goto cont;
                    }
                    else if (dt.Rows[i][c].ToString().Contains("Tax Impounds"))
                    {
                        col = c;
                        Name = "PAYROLL TAXES";
                        r = i;
                        goto cont;
                    }
                    else if (dt.Rows[i][c].ToString().Contains("Partial DD"))
                    {
                        col = c;
                        Name = "DIRECT DEPOSITS";
                        r = i;
                        goto cont;
                    }
                }
            }
        cont:
            return col;
        }

        private void ReadBankAccounts(int c, ref int r, string Name, ref List<PDFTable> TableList, ref DataTable dt)
        {
            decimal Amount = 0;
            int Flag = 0;
            for (int i = r; i < dt.Rows.Count; i++)
            {
                if (dt.Rows[i][c].ToString().ToLower().Contains("bank"))
                {
                    Flag = 1;
                    for (int j = c + 1; j < dt.Columns.Count; j++)
                    {
                        try
                        {
                            Amount += Convert.ToDecimal(dt.Rows[i][j].ToString().Replace('(', '-').Replace(')', ' ').Replace(" ", "").Replace(",", "").Trim());
                            break;
                        }
                        catch (Exception ex)
                        {
                        }
                    }
                }
                else if (dt.Rows[i][c].ToString() != "" && Flag == 1)
                {
                    for (int j = c + 1; j < dt.Columns.Count; j++)
                    {
                        try
                        {
                            Amount += Convert.ToDecimal(dt.Rows[i][j].ToString().Replace("_", "").Replace('(', '-').Replace(')', ' ').Replace(" ", "").Replace(",", "").Trim());
                            break;
                        }
                        catch (Exception ex)
                        {

                            //match = Regex.Match(dt.Rows[i][j].ToString(), pattern);
                            //if (match.Success)
                            //{
                            //    Amount += Convert.ToDecimal(match.Groups[1].Value);                                
                            //}
                        }
                    }
                }
                else if (dt.Rows[i][c].ToString() == "")
                {
                    r = i + 1;
                    goto Next;
                }
            }
        Next:
            if (Amount != 0)
            {
                if (Name == "DIRECT DEPOSITS")
                {
                    if (TableList.AsEnumerable().Where(s => s.Name == "DIRECT DEPOSITS" && s.Name3 == "TAXABLE WAGES" && s.Name2 == "File2").Count() == 1)
                    {
                        var Val = TableList.AsEnumerable().Where(s => s.Name == "DIRECT DEPOSITS" && s.Name3 == "TAXABLE WAGES" && s.Name2 == "File2").FirstOrDefault().Value;
                        decimal Amt = Convert.ToDecimal(Val) + Amount;
                        var f = TableList.AsEnumerable().Where(s => s.Name == "DIRECT DEPOSITS" && s.Name3 == "TAXABLE WAGES" && s.Name2 == "File2").FirstOrDefault();
                        TableList.Remove(f);
                        TableList.Add(new PDFTable { Name = "DIRECT DEPOSITS", Value = Amt.ToString(), Name2 = "File2", Name3 = "TAXABLE WAGES" });
                    }
                    else
                        TableList.Add(new PDFTable { Name = "DIRECT DEPOSITS", Value = Amount.ToString(), Name2 = "File2", Name3 = "TAXABLE WAGES" });
                }
                else
                    TableList.Add(new PDFTable { Name = Name, Value = Amount.ToString(), Name2 = "File2", Name3 = "TAXABLE WAGES" });
                Amount = 0;
            }


        }

        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        /// <summary>
        /// This function is ReadPayroll_NewStructer.
        /// </summary>
        /// <param name="FileName"></param>
        /// <param name="PayrollReportId"></param>
        /// <returns></returns>
        public ActionResult ReadPayroll_NewStructer(string FileName, int PayrollReportId)
        {
            try
            {
                if (FileName != "")
                {
                    int PayrollId = 0;
                    string FilePath = Server.MapPath("~/userfiles/PayrollFile/" + FileName);
                    PdfReader reader = new PdfReader(FilePath);
                    int intPageNum = reader.NumberOfPages;
                    int FileNo = 0;
                    for (int i = 1; i <= intPageNum; i++)
                    {
                        string text = PdfTextExtractor.GetTextFromPage(reader, i, new LocationTextExtractionStrategy());
                        if (text.Contains("CASH REQUIREMENT SUMMARY"))
                        {
                            FileNo = 1;
                            break;
                        }
                        else
                        {
                            FileNo = 2;
                            break;
                        }
                    }
                    reader.Close();
                    reader.Dispose();
                    Aspose.Pdf.Document pdfDocument = new Aspose.Pdf.Document(FilePath);
                    ExcelSaveOptions options = new ExcelSaveOptions();
                    options.Format = ExcelSaveOptions.ExcelFormat.XLSX;
                    string NewFilePath = Server.MapPath("~/userfiles/PdfToExcelFile/PdfToExcel.xlsx");
                    pdfDocument.Save(NewFilePath, options);
                    bool IsProcess = false;
                    List<PDFTable> TableList = new List<PDFTable>();
                    string PeriodEndDate = "";
                    string PayDate = "";
                    if (FileNo == 2)
                    {
                        try
                        {
                            ProcessExcelFile_Distributor(ref TableList, ref PeriodEndDate, ref PayDate);
                            IsProcess = true;
                        }
                        catch (Exception ex)
                        {
                            TempData["PDFFile"] = "File has reading issue. Message:" + ex.Message;
                        }

                    }
                    else
                    {
                        try
                        {
                            ProcessExcelFile_CASHREQUIREMENT(ref TableList, ref PeriodEndDate, ref PayDate);
                            IsProcess = true;
                        }
                        catch (Exception ex)
                        {
                            TempData["PDFFile"] = "File has reading issue. Message:" + ex.Message;
                        }
                    }

                    // Start Saving Data in Database                    
                    if (IsProcess)
                    {
                        string json = Newtonsoft.Json.JsonConvert.SerializeObject(TableList);
                        DataTable pDt = JsonConvert.DeserializeObject<DataTable>(json);

                        List<PDFTable> PDFPrintList = new List<PDFTable>();
                        int flg = 0;
                        int DepartmentId = 0;

                        PayrollMaster obj = new PayrollMaster();
                        int StoreId = Convert.ToInt32(Session["storeid"].ToString());
                        obj.StoreId = Convert.ToInt32(Session["storeid"].ToString());

                        if (PeriodEndDate != "")
                        {
                            obj.StartDate = Convert.ToDateTime(PeriodEndDate).AddDays(-7);
                            obj.EndDate = Convert.ToDateTime(PeriodEndDate);
                        }
                        else
                        {
                            obj.StartDate = DateTime.Today;
                            obj.EndDate = DateTime.Today;
                        }
                        if (PayDate != "")
                        {
                            obj.EndCheckDate = Convert.ToDateTime(PayDate);
                        }
                        else
                        {
                            obj.EndCheckDate = DateTime.Today;
                        }

                        obj.PayrollReportId = PayrollReportId;

                        var FileExist = (from PR in db.PayrollReports
                                         join PM in db.PayrollMasters on PR.PayrollReportId equals PM.PayrollReportId
                                         where PM.StartDate == obj.StartDate && PM.EndDate == obj.EndDate && PM.EndCheckDate == obj.EndCheckDate && PR.FIleNo == FileNo && PM.StoreId == obj.StoreId
                                         select new { PayrollReports = PR, PayrollMasters = PM }).ToList();

                        PayrollReport payroll = db.PayrollReports.Find(PayrollReportId);
                        if (payroll != null)
                        {
                            payroll.FIleNo = FileNo;
                        }
                        db.SaveChanges();

                        var TransactionNo = RandomString(8);
                        db.Database.ExecuteSqlCommand("SP_Payroll @Mode = {0},@TransactionNo = {1},@StartDate = {2},@EndDate = {3},@EndCheckDate = {4},@StoreID = {5},@PayrollReportId = {6},@FileNo = {7}", "CheckandUpdateTransationNo", TransactionNo, obj.StartDate, obj.EndDate, obj.EndCheckDate, obj.StoreId, PayrollReportId, FileNo);


                        if (FileExist.Count > 0 && 1 == 2)
                        {
                            PayrollReport vendorMaster = db.PayrollReports.Find(PayrollReportId);
                            db.PayrollReports.Remove(vendorMaster);
                            db.SaveChangesAsync();

                            ViewBag.Message = "File Already Exist.";
                            TempData["PDFFile"] = "File Already Exist";
                            return View("Create");
                        }
                        else
                        {
                            db.PayrollMasters.Add(obj);
                            db.SaveChanges();
                            PayrollId = obj.PayrollId;
                            string[] casetypr = { "COMPANY TOTAL", "TAXABLE WAGES", "EMPLOYER TAX LIABILITY" };
                            var dep = TableList.Where(s => !casetypr.Contains(s.Name3)).ToList();
                            foreach (var item in dep)
                            {
                                try
                                {
                                    PDFPrintList.Add(new PDFTable { Name = item.Name, Value = item.Value, Name2 = item.Name2, Value2 = item.Value2, Name3 = item.Name3, Value3 = item.Value3, Name4 = item.Name4, Value4 = item.Value4, Name5 = item.Name5, Value5 = item.Value5 });
                                    //Get_ProductName(item.Name3).Replace("FROZEN FOODS", "FROZEN").Replace("FROZEN FOOD", "FROZEN").Replace("CHESSE", "CHEESE");
                                    string str = item.Name3.Replace("FROZEN FOODS", "FROZEN").Replace("FROZEN FOOD", "FROZEN").Replace("CHESSE", "CHEESE");
                                    var DeptExist = db.PayrollDepartments.Where(a => a.StoreId == StoreId && a.DepartmentName == str).ToList();
                                    if (DeptExist.Count > 0)
                                    {
                                        DepartmentId = Convert.ToInt32(DeptExist.FirstOrDefault().PayrollDepartmentId);

                                    }
                                    else
                                    {
                                        PayrollDepartment objPayDept = new PayrollDepartment();
                                        try
                                        {
                                            objPayDept.StoreId = StoreId;
                                            objPayDept.DepartmentName = str;
                                            db.PayrollDepartments.Add(objPayDept);
                                            db.SaveChanges();

                                            DepartmentId = objPayDept.PayrollDepartmentId;
                                        }
                                        catch (Exception ex) { }
                                    }
                                    PayrollDepartmentDetails objPayDeptDetails = new PayrollDepartmentDetails();
                                    objPayDeptDetails.Name = item.Name;
                                    objPayDeptDetails.PayrollDepartmentId = DepartmentId;
                                    objPayDeptDetails.PayrollId = PayrollId;
                                    if (item.Value != null)
                                    {
                                        try
                                        {
                                            string Valuestr = item.Value.Replace("$", "").Replace(",", "");
                                            decimal dValue = Convert.ToDecimal(Valuestr);
                                            objPayDeptDetails.Value = dValue;
                                            db.payrollDepartmentDetails.Add(objPayDeptDetails);
                                            db.SaveChanges();
                                        }
                                        catch (Exception ex) { }
                                    }
                                    objPayDeptDetails = null;
                                }
                                catch (Exception ex)
                                {


                                }
                            }

                            if (TableList.Count() > 0)
                            {
                                db.Database.ExecuteSqlCommand("SP_Payroll @Mode = {0},@PayrollReportId = {1}", "UpdatePayrollReportStatus", obj.PayrollReportId);
                            }
                            var deps = TableList.Where(s => casetypr.Contains(s.Name3)).ToList();
                            foreach (var item in deps)
                            {
                                try
                                {
                                    PayrollCashAnalysis objPayCash = new PayrollCashAnalysis();
                                    var DeptExist = db.payrollCashAnalyses.Where(a => a.StoreId == StoreId && a.Name == item.Name).ToList();
                                    if (DeptExist.Count > 0)
                                    {
                                        DepartmentId = Convert.ToInt32(DeptExist.FirstOrDefault().PayrollCashAnalysisId);
                                    }
                                    else
                                    {
                                        objPayCash.StoreId = StoreId;
                                        objPayCash.Name = item.Name;
                                        if (item.Value4 != null)
                                            objPayCash.ETaxCalc = Convert.ToInt32(item.Value4);
                                        db.payrollCashAnalyses.Add(objPayCash);
                                        db.SaveChanges();
                                        DepartmentId = objPayCash.PayrollCashAnalysisId;
                                    }


                                    if (item.Name != null)
                                    {
                                        PayrollCashAnalysisDetail objPaycashDetail = new PayrollCashAnalysisDetail();
                                        objPaycashDetail.PayrollCashAnalysisId = DepartmentId;
                                        objPaycashDetail.PayrollId = PayrollId;
                                        if (item.Value != null)
                                        {
                                            string Valuestr = item.Value.Replace("$", "").Replace("(", "").Replace(")", "").Replace(",", "");
                                            decimal Value = Convert.ToDecimal(Valuestr);
                                            objPaycashDetail.Amount = Value;
                                            db.payrollCashAnalysisDetails.Add(objPaycashDetail);
                                            db.SaveChanges();
                                        }
                                        objPaycashDetail = null;
                                    }
                                }
                                catch (Exception ex)
                                {
                                }

                            }
                            ViewBag.PDFData = TableList;
                            ViewBag.Message = "File uploaded successfully.";
                            TempData["PDFFile"] = "File uploaded successfully.";
                        }
                        if (System.IO.File.Exists(NewFilePath))
                        {
                            System.IO.File.Delete(NewFilePath);
                        }
                    }

                }
                return View("Create");
            }
            catch (Exception ex)
            {
                TempData["PDFFile"] = "Some thing went to wrong.";
            }
            return View("Create");
        }

        /// <summary>
        /// This function is Read Payroll Summary.
        /// </summary>
        /// <param name="FileName"></param>
        /// <param name="PayrollReportId"></param>
        /// <returns></returns>
        public ActionResult ReadPayrollSummary(string FileName, int PayrollReportId)
        {
            try
            {
                if (FileName != "")
                {
                    int PayrollId = 0;
                    string FilePath = Server.MapPath("~/userfiles/PayrollFile/" + FileName);
                    PdfReader reader = new PdfReader(FilePath);
                    int intPageNum = reader.NumberOfPages;
                    int FileNo = 0;
                    for (int i = 1; i <= intPageNum; i++)
                    {
                        string text = PdfTextExtractor.GetTextFromPage(reader, i, new LocationTextExtractionStrategy());
                        if (text.Contains("Payroll Distributed Summary Report"))
                        {
                            FileNo = 1;
                            break;
                        }
                        else
                        {
                            FileNo = 2;
                            break;
                        }
                    }
                    reader.Close();
                    reader.Dispose();
                    Aspose.Pdf.Document pdfDocument = new Aspose.Pdf.Document(FilePath);
                    ExcelSaveOptions options = new ExcelSaveOptions();
                    options.Format = ExcelSaveOptions.ExcelFormat.XLSX;
                    string NewFilePath = Server.MapPath("~/userfiles/PdfToExcelFile/PdfToExcel.xlsx");
                    pdfDocument.Save(NewFilePath, options);
                    DataTable TableList = new DataTable();
                    string PeriodEndDate = string.Empty;
                    string CheckDate = string.Empty;
                    string Transaction = string.Empty;
                    string RowValue = "";
                    int rows = 0, cols = 0;
                    DataTable dataTable = null;
                    Aspose.Cells.Workbook wb = new Aspose.Cells.Workbook(NewFilePath);
                    WorksheetCollection collection = wb.Worksheets;
                    // Get Dates and TransactionID
                    // Initilize Table Columns
                    TableList = new DataTable();
                    TableList.Columns.Add(new DataColumn("Department"));
                    TableList.Columns.Add(new DataColumn("Header"));
                    TableList.Columns.Add(new DataColumn("Amount"));

                    if (FileNo == 1)
                    {
                        if (collection.Count > 0)
                        {
                            Aspose.Cells.Worksheet worksheet = collection[0];
                            rows = worksheet.Cells.MaxDataRow;
                            cols = worksheet.Cells.MaxDataColumn;
                            dataTable = worksheet.Cells.ExportDataTable(0, 0, rows + 1, cols + 1, false);

                            if (dataTable.Rows.Count > 0)
                            {
                                if (dataTable.Rows[0][2].ToString().Contains("Period Ending"))
                                {
                                    RowValue = dataTable.Rows[0][2].ToString();
                                    string[] words = RowValue.Split('\n');
                                    for (int j = 0, len = words.Length; j < len; j++)
                                    {
                                        var LineData = Encoding.UTF8.GetString(Encoding.UTF8.GetBytes(words[j]));
                                        if (LineData.Contains("Period Ending:"))
                                        {
                                            string[] DateSplitData = LineData.Split(':');
                                            if (DateSplitData.Count() > 0)
                                            {
                                                PeriodEndDate = DateSplitData[1];
                                                string[] DateFormate = PeriodEndDate.Split('/');
                                                if (DateFormate.Count() == 3)
                                                {
                                                    var Month = DateFormate[0].Trim().Length == 1
                                                        ? "0" + DateFormate[0].Trim()
                                                        : DateFormate[0].Trim();
                                                    PeriodEndDate = DateFormate[2] + "-" + Month + "-" + DateFormate[1];
                                                }
                                            }
                                        }
                                        if (LineData.Contains("Check Date:"))
                                        {
                                            string[] DateSplitData = LineData.Split(':');
                                            if (DateSplitData.Count() > 0)
                                            {
                                                if (FileNo == 2)
                                                {
                                                    CheckDate = DateSplitData[1];
                                                }
                                                else
                                                {
                                                    CheckDate = DateSplitData[1];
                                                }
                                                string[] DateFormate = CheckDate.Split('/');
                                                if (DateFormate.Count() == 3)
                                                {
                                                    var Month = DateFormate[0].Trim().Length == 1
                                                        ? "0" + DateFormate[0].Trim()
                                                        : DateFormate[0].Trim();
                                                    CheckDate = DateFormate[2].Trim() +
                                                        "-" +
                                                        Month +
                                                        "-" +
                                                        DateFormate[1].Trim();
                                                }
                                            }
                                        }
                                        if (LineData.Contains("Transaction"))
                                        {
                                            string[] DateSplitData = LineData.Split(':');
                                            if (DateSplitData.Count() > 0)
                                            {
                                                Transaction = DateSplitData[1];
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        for (int worksheetIndex = 0; worksheetIndex < collection.Count; worksheetIndex++)
                        {
                            Aspose.Cells.Worksheet worksheet = collection[worksheetIndex];
                            rows = worksheet.Cells.MaxDataRow;
                            cols = worksheet.Cells.MaxDataColumn;
                            dataTable = worksheet.Cells.ExportDataTable(0, 0, rows + 1, cols + 1, false);

                            if (FileNo == 1)
                            {
                                ReadDepartmentValues(ref dataTable, ref TableList);
                            }

                        }
                    }
                    else if (FileNo == 2)
                    {
                        DataRow dr = null;
                        bool IsReadBelow = false;
                        for (int worksheetIndex = 0; worksheetIndex < collection.Count; worksheetIndex++)
                        {
                            Aspose.Cells.Worksheet worksheet = collection[worksheetIndex];
                            rows = worksheet.Cells.MaxDataRow;
                            cols = worksheet.Cells.MaxDataColumn;
                            dataTable = worksheet.Cells.ExportDataTable(0, 0, rows + 1, cols + 1, false);

                            for (int i = 0; i < dataTable.Rows.Count; i++)
                            {
                                if (dataTable.Rows[i][2].ToString().Contains("Period Ending"))
                                {
                                    string[] DateSplitData = dataTable.Rows[i][2].ToString().Split(':');
                                    if (DateSplitData.Count() > 0)
                                    {
                                        PeriodEndDate = DateSplitData[1];
                                        string[] DateFormate = PeriodEndDate.Split('/');
                                        if (DateFormate.Count() == 3)
                                        {
                                            var Month = DateFormate[0].Trim().Length == 1
                                                ? "0" + DateFormate[0].Trim()
                                                : DateFormate[0].Trim();
                                            PeriodEndDate = DateFormate[2] + "-" + Month + "-" + DateFormate[1];
                                        }
                                    }
                                }
                                else if (dataTable.Rows[i][2].ToString().Contains("Check Date:"))
                                {
                                    string[] DateSplitData = dataTable.Rows[i][2].ToString().Split(':');
                                    if (DateSplitData.Count() > 0)
                                    {
                                        if (FileNo == 2)
                                        {
                                            CheckDate = DateSplitData[1];
                                        }
                                        else
                                        {
                                            CheckDate = DateSplitData[1];
                                        }
                                        string[] DateFormate = CheckDate.Split('/');
                                        if (DateFormate.Count() == 3)
                                        {
                                            var Month = DateFormate[0].Trim().Length == 1
                                                ? "0" + DateFormate[0].Trim()
                                                : DateFormate[0].Trim();
                                            CheckDate = DateFormate[2].Trim() +
                                                "-" +
                                                Month +
                                                "-" +
                                                DateFormate[1].Trim();
                                        }
                                    }
                                }
                                else if (dataTable.Rows[i][2].ToString().Contains("Transaction"))
                                {
                                    string[] DateSplitData = dataTable.Rows[i][2].ToString().Split(':');
                                    if (DateSplitData.Count() > 0)
                                    {
                                        Transaction = DateSplitData[1];
                                    }
                                }
                                else if (dataTable.Rows[i][0].ToString().Trim() == "Paycom Cash Requirements")
                                {
                                    IsReadBelow = true;
                                }
                                if (IsReadBelow)
                                {
                                    if (dataTable.Rows[i][0].ToString().Trim().ToLower().Contains("payroll fee"))
                                    {
                                        dr = TableList.NewRow();
                                        dr["Department"] = "";
                                        dr["Header"] = dataTable.Rows[i][0].ToString();
                                        dr["Amount"] = (dataTable.Rows[i][1].ToString() == "-" ? "0" : dataTable.Rows[i][1].ToString());
                                        TableList.Rows.Add(dr);
                                    }
                                    else if (dataTable.Rows[i][0].ToString().Trim().ToLower().Contains("new york ias"))
                                    {
                                        dr = TableList.NewRow();
                                        dr["Department"] = "";
                                        dr["Header"] = dataTable.Rows[i][0].ToString();
                                        dr["Amount"] = (dataTable.Rows[i][1].ToString() == "-" ? "0" : dataTable.Rows[i][1].ToString());
                                        TableList.Rows.Add(dr);
                                    }
                                    else if (dataTable.Rows[i][0].ToString().Trim().ToLower().Contains("manual checks"))
                                    {
                                        dr = TableList.NewRow();
                                        dr["Department"] = "";
                                        dr["Header"] = dataTable.Rows[i][0].ToString();
                                        dr["Amount"] = (dataTable.Rows[i][1].ToString() == "-" ? "0" : dataTable.Rows[i][1].ToString());
                                        TableList.Rows.Add(dr);
                                    }
                                }
                            }
                        }
                    }

                    List<PDFTable> PDFPrintList = new List<PDFTable>();
                    int DepartmentId = 0;
                    PayrollMaster obj = new PayrollMaster();
                    obj.StoreId = Convert.ToInt32(Session["storeid"].ToString());
                    obj.StartDate = DateTime.Today;
                    if (PeriodEndDate != string.Empty)
                    {
                        obj.EndDate = Convert.ToDateTime(PeriodEndDate);
                    }
                    else
                    {
                        obj.EndDate = DateTime.Today;
                    }
                    if (CheckDate != string.Empty)
                    {
                        obj.EndCheckDate = Convert.ToDateTime(CheckDate);
                    }
                    else
                    {
                        obj.EndCheckDate = DateTime.Today;
                    }
                    obj.PayrollReportId = PayrollReportId;

                    //var FileExist = from PR in db.PayrollReports
                    //                join PM in db.PayrollMasters on PR.PayrollReportId equals PM.PayrollReportId
                    //                where PR.TransactionNo == Transaction &&
                    //                    PM.EndCheckDate == obj.EndCheckDate &&
                    //                    PR.FIleNo == FileNo
                    //                select new { PayrollReports = PR, PayrollMasters = PM };
                    //if (FileExist.ToList().Count() > 0 && 1 == 2)
                    //{
                    //    _PDFReadRepository.ReomvePayrollReports(PayrollReportId);

                    //    ViewBag.Message = "File Already Exist.";
                    //    TempData["PDFFile"] = "File Already Exist";
                    //    return View("Create");
                    //}
                    //else
                    //{
                    _PDFReadRepository.AddPayrollMasters(obj);

                    PayrollId = obj.PayrollId;

                    PayrollReport objP = _PDFReadRepository.GetPayrollReportbyPayrollReportId(obj.PayrollReportId);
                    if (objP != null)
                    {
                        objP.TransactionNo = Transaction;
                        objP.FIleNo = FileNo;
                        _PDFReadRepository.SavePayrollReport(objP);
                    }
                    int StoreId = Convert.ToInt32(Session["storeid"].ToString());
                    _PDFReadRepository.InsertPayrollFile(TableList, StoreId, PayrollId);


                    if (TableList.Rows.Count > 0)
                    {
                        _PDFReadRepository.UpdatePayrollReportStatus(obj.PayrollReportId);
                    }
                    ViewBag.PDFData = TableList;

                    ViewBag.Message = "File uploaded successfully.";
                    TempData["PDFFile"] = "File uploaded successfully.";
                    return View("PDFRead", PDFPrintList.Where(s => s.Name != string.Empty));
                    //}
                }
            }
            catch (Exception ex)
            {

            }
            ViewBag.ErrorMessage = "PDF File Path Not Found..";
            return View("Create");
        }

        private void ReadDepartmentValues(ref DataTable dataTable, ref DataTable TableList)
        {
            Boolean IsEnd = false, CheckNext = false;
            DataRow dr = null;
            string Name = "", Name1 = "";

            for (int i = 0; i < dataTable.Rows.Count; i++)
            {
                if (IsEnd == false)
                {
                    if (dataTable.Rows[i][0].ToString().Contains("Dept :"))
                    {
                        Name = dataTable.Rows[i][0].ToString();
                        Name = Name.Replace("Dept : ", "");
                        Name = Name.Split('-')[1].ToString().Trim();
                    }
                    else if (Name != "")
                    {
                        if (dataTable.Rows[i][0].ToString() != "Gross Earnings" && dataTable.Rows[i][0].ToString() != "")
                        {
                            dr = TableList.NewRow();
                            dr["Department"] = Name;
                            dr["Header"] = dataTable.Rows[i][0].ToString();
                            dr["Amount"] = (dataTable.Rows[i][2].ToString() == "-" ? "0" : dataTable.Rows[i][2].ToString());
                            TableList.Rows.Add(dr);
                        }
                        else if (TableList.Rows.Count > 0)
                        {
                            if (dataTable.Rows[i][0].ToString() == "")
                                goto Done;
                        }
                    }
                    else if (dataTable.Rows[i][0].ToString().Contains("Company Total"))
                    {
                        IsEnd = true;
                    }
                }
                else
                {
                    if (dataTable.Rows[i][4].ToString() == "Voluntary Deductions")
                    {
                        Name = "Voluntary Deductions";
                    }
                    else if (Name != "" && dataTable.Rows[i][4].ToString() != "")
                    {
                        dr = TableList.NewRow();
                        dr["Department"] = Name;
                        dr["Header"] = dataTable.Rows[i][4].ToString();
                        dr["Amount"] = dataTable.Rows[i][5].ToString();
                        TableList.Rows.Add(dr);

                        if (dataTable.Rows[i][6].ToString() == "Employer Taxes")
                        {
                            Name1 = "Employer Taxes";
                            CheckNext = true;
                        }
                    }
                    if (CheckNext == true && (dataTable.Rows[i][6].ToString() != "Employer Taxes" && dataTable.Rows[i][6].ToString() != ""))
                    {
                        dr = TableList.NewRow();
                        dr["Department"] = Name1;
                        dr["Header"] = dataTable.Rows[i][6].ToString();
                        dr["Amount"] = dataTable.Rows[i][7].ToString();
                        TableList.Rows.Add(dr);
                    }
                }
            }
        Done:
            Name = "";
        }

        /// <summary>
        /// This method is Read Payroll Summary for check file.
        /// </summary>
        /// <param name="FileName"></param>
        /// <returns></returns>
        public int ReadPayrollSummaryCheckFile(string FileName)
        {
            int value = 0;
            try
            {
                if (FileName != string.Empty)
                {
                    int PayrollId = 0;
                    string FilePath = Server.MapPath("~/userfiles/PayrollFile/" + FileName);
                    PdfReader reader = new PdfReader(FilePath);
                    int intPageNum = reader.NumberOfPages;

                    List<PDFTable> TableList = new List<PDFTable>();
                    List<PDFTable> TempTable = new List<PDFTable>();
                    List<PDFTable> TempTable1 = new List<PDFTable>();

                    string PeriodEndDate = string.Empty;
                    string CheckDate = string.Empty;
                    string Transaction = string.Empty;
                    string[] words;
                    int FileNo = 0;

                    StringBuilder Reader = new StringBuilder();
                    for (int i = 1; i <= intPageNum; i++)
                    {
                        string text = PdfTextExtractor.GetTextFromPage(reader, i, new LocationTextExtractionStrategy());
                        int DVFlg = 0;
                        bool blnTotal = false;
                        bool blnTotal1 = false;
                        if (text.Contains("Payroll Distributed Summary Report"))
                        {
                            FileNo = 1;
                        }
                        else if (text.Contains("Cash Requirements Statement"))
                        {
                            FileNo = 2;
                        }
                        if (PeriodEndDate == string.Empty)
                        {
                            if (text.Contains("Period Ending:") ||
                                text.Contains("Check Date:") ||
                                text.Contains("Transaction:"))
                            {
                                words = text.Split('\n');
                                for (int j = 0, len = words.Length; j < len; j++)
                                {
                                    var LineData = Encoding.UTF8.GetString(Encoding.UTF8.GetBytes(words[j]));
                                    if (LineData.Contains("Period Ending:"))
                                    {
                                        string[] DateSplitData = LineData.Split(':');
                                        if (DateSplitData.Count() > 0)
                                        {
                                            PeriodEndDate = DateSplitData[1];
                                            string[] DateFormate = PeriodEndDate.Split('/');
                                            if (DateFormate.Count() == 3)
                                            {
                                                var Month = DateFormate[0].Trim().Length == 1
                                                    ? "0" + DateFormate[0].Trim()
                                                    : DateFormate[0].Trim();
                                                PeriodEndDate = DateFormate[2] + "-" + Month + "-" + DateFormate[1];
                                            }
                                        }
                                    }
                                    if (LineData.Contains("Check Date:"))
                                    {
                                        string[] DateSplitData = LineData.Split(':');
                                        if (DateSplitData.Count() > 0)
                                        {
                                            if (FileNo == 2)
                                            {
                                                CheckDate = DateSplitData[1];
                                            }
                                            else
                                            {
                                                CheckDate = DateSplitData[2];
                                            }
                                            string[] DateFormate = CheckDate.Split('/');
                                            if (DateFormate.Count() == 3)
                                            {
                                                var Month = DateFormate[0].Trim().Length == 1
                                                    ? "0" + DateFormate[0].Trim()
                                                    : DateFormate[0].Trim();
                                                CheckDate = DateFormate[2].Trim() +
                                                    "-" +
                                                    Month +
                                                    "-" +
                                                    DateFormate[1].Trim();
                                            }
                                        }
                                    }
                                    if (LineData.Contains("Transaction"))
                                    {
                                        string[] DateSplitData = LineData.Split(':');
                                        if (DateSplitData.Count() > 0)
                                        {
                                            Transaction = DateSplitData[1];
                                        }
                                    }
                                }
                            }
                        }

                        //if (FileNo == 1)
                        //{
                        //    if (text.Contains("Dept :"))
                        //    {
                        //        DVFlg = 1;
                        //    }
                        //    if (DVFlg == 1)
                        //    {
                        //        words = text.Split('\n');
                        //        for (int j = 0, len = words.Length; j < len; j++)
                        //        {
                        //            var LineData = Encoding.UTF8.GetString(Encoding.UTF8.GetBytes(words[j]));
                        //            if (LineData.Contains("Dept :"))
                        //            {
                        //                TableList.Add(new PDFTable { Name = LineData });
                        //                TempTable = new List<PDFTable>();
                        //            }
                        //            if (LineData.Contains("Gross Earnings"))
                        //            {
                        //                try
                        //                {
                        //                    var PreLine = Encoding.UTF8.GetString(Encoding.UTF8.GetBytes(words[j - 1]));
                        //                    //TableList.Add(new PDFTable { Name = PreLine, Value = "", Name2 = "Gross Earnings", Value2 = "", Name3 = "Amount", Value3 = "", Name4 = "Voluntary Deductions", Value4 = "", Name5 = "Amount", Value5 = "" });
                        //                    TempTable.Add(
                        //                        new PDFTable
                        //                        {
                        //                            Name = PreLine,
                        //                            Value = string.Empty,
                        //                            Name2 = "Gross Earnings",
                        //                            Value2 = string.Empty,
                        //                            Name3 = "Amount",
                        //                            Value3 = string.Empty,
                        //                            Name4 = "Voluntary Deductions",
                        //                            Value4 = string.Empty,
                        //                            Name5 = "Amount",
                        //                            Value5 = string.Empty
                        //                        });
                        //                    blnTotal = false;
                        //                }
                        //                catch (Exception ex)
                        //                {
                        //                    logger.Error("PDFReadController - ReadPayrollSummaryCheckFile1 - " + DateTime.Now + " - " + ex.Message.ToString());
                        //                }
                        //            }
                        //            else
                        //            {
                        //                var TempData = TempTable;
                        //                if (TempTable.Count() > 0)
                        //                {
                        //                    string[] authorsList = LineData.Trim().Split(' ');
                        //                    int StringCount = authorsList.Count();
                        //                    var TempValues = TempData.FirstOrDefault();
                        //                    string[] bb = GetFinalArr(authorsList);
                        //                    if (bb.Length > 0)
                        //                    {
                        //                        if (!char.IsDigit(bb[0].Trim().ToArray()[0]) && blnTotal == false)
                        //                        {
                        //                            if (!bb[0].ToString().Contains("Medical Pre-Tax"))
                        //                            {
                        //                                try
                        //                                {
                        //                                    TableList.Add(
                        //                                        new PDFTable
                        //                                        {
                        //                                            Name = TempValues.Name,
                        //                                            Value = string.Empty,
                        //                                            Name2 = TempValues.Name2,
                        //                                            Value2 = bb[0],
                        //                                            Name3 = TempValues.Name3,
                        //                                            Value3 = (bb.Length >= 3) ? bb[2] : string.Empty
                        //                                        });
                        //                                }
                        //                                catch (Exception ex)
                        //                                {
                        //                                }
                        //                            }
                        //                        }
                        //                        else
                        //                        {
                        //                            blnTotal = true;
                        //                            if (bb[0].ToString().Contains("Holiday Worked") ||
                        //                                bb[0].ToString().Contains("Holiday Hours"))
                        //                            {
                        //                                try
                        //                                {
                        //                                    TableList.Add(
                        //                                        new PDFTable
                        //                                        {
                        //                                            Name = TempValues.Name,
                        //                                            Value = string.Empty,
                        //                                            Name2 = TempValues.Name2,
                        //                                            Value2 = bb[0],
                        //                                            Name3 = TempValues.Name3,
                        //                                            Value3 = (bb.Length >= 3) ? bb[2] : "0"
                        //                                        });
                        //                                }
                        //                                catch (Exception ex)
                        //                                {
                        //                                }
                        //                            }
                        //                        }
                        //                    }
                        //                    else
                        //                    {
                        //                        blnTotal = true;
                        //                    }
                        //                    if (LineData.Contains("Service Fee"))
                        //                    {
                        //                        if (LineData.Contains("Retro Pay") && LineData.Contains("Retro Pay"))
                        //                        {
                        //                            try
                        //                            {
                        //                                TableList.Add(
                        //                                    new PDFTable
                        //                                    {
                        //                                        Name = TempValues.Name,
                        //                                        Value = string.Empty,
                        //                                        Name2 = TempValues.Name2,
                        //                                        Value2 = bb[0],
                        //                                        Name3 = TempValues.Name3,
                        //                                        Value3 = (bb.Length >= 3) ? bb[2] : "0"
                        //                                    });
                        //                                TableList.Add(
                        //                                    new PDFTable
                        //                                    {
                        //                                        Name = TempValues.Name,
                        //                                        Value = string.Empty,
                        //                                        Name2 = TempValues.Name2,
                        //                                        Value2 = "Service Fee",
                        //                                        Name3 = TempValues.Name3,
                        //                                        Value3 = bb[bb.Length - 1]
                        //                                    });
                        //                                TempTable = new List<PDFTable>();
                        //                                DVFlg = 0;
                        //                            }
                        //                            catch (Exception ex)
                        //                            {
                        //                            }
                        //                        }
                        //                        else
                        //                        {
                        //                            try
                        //                            {
                        //                                if (!LineData.Contains("Spread of Hours"))
                        //                                {
                        //                                    TableList.Add(
                        //                                        new PDFTable
                        //                                        {
                        //                                            Name = TempValues.Name,
                        //                                            Value = string.Empty,
                        //                                            Name2 = TempValues.Name2,
                        //                                            Value2 = bb[0],
                        //                                            Name3 = TempValues.Name3,
                        //                                            Value3 = bb[1]
                        //                                        });
                        //                                }
                        //                                TempTable = new List<PDFTable>();
                        //                                DVFlg = 0;
                        //                            }
                        //                            catch (Exception ex)
                        //                            {
                        //                            }
                        //                        }
                        //                    }
                        //                    else
                        //                    {
                        //                        if (LineData.Contains("Retro Pay"))
                        //                        {
                        //                            try
                        //                            {
                        //                                TableList.Add(
                        //                                    new PDFTable
                        //                                    {
                        //                                        Name = TempValues.Name,
                        //                                        Value = string.Empty,
                        //                                        Name2 = TempValues.Name2,
                        //                                        Value2 = bb[0],
                        //                                        Name3 = TempValues.Name3,
                        //                                        Value3 = (bb.Length >= 3) ? bb[2] : "0"
                        //                                    });
                        //                                TempTable = new List<PDFTable>();
                        //                                DVFlg = 0;
                        //                            }
                        //                            catch (Exception ex)
                        //                            {
                        //                            }
                        //                        }
                        //                    }
                        //                    if (LineData.Contains("Spread of Hours"))
                        //                    {
                        //                        int pos = Array.FindIndex(words, x => x.Contains("Service Fee"));
                        //                        if (pos > j)
                        //                        {
                        //                            try
                        //                            {
                        //                                TableList.Add(
                        //                                    new PDFTable
                        //                                    {
                        //                                        Name = TempValues.Name,
                        //                                        Value = string.Empty,
                        //                                        Name2 = TempValues.Name2,
                        //                                        Value2 = bb[0],
                        //                                        Name3 = TempValues.Name3,
                        //                                        Value3 = (bb.Length >= 3) ? bb[2] : "0"
                        //                                    });
                        //                            }
                        //                            catch (Exception ex)
                        //                            {
                        //                            }
                        //                        }
                        //                    }
                        //                }

                        //                if (TempTable.Count() == 0 && LineData.Contains("Spread of Hours"))
                        //                {
                        //                    try
                        //                    {
                        //                        var aa = TableList[TableList.Count() - 1];
                        //                        string[] authorsList = LineData.Trim().Split(' ');
                        //                        string[] bb = GetFinalArr(authorsList);
                        //                        TableList.Add(
                        //                            new PDFTable
                        //                            {
                        //                                Name = aa.Name,
                        //                                Value = string.Empty,
                        //                                Name2 = aa.Name2,
                        //                                Value2 = bb[0],
                        //                                Name3 = aa.Name3,
                        //                                Value3 = (bb.Length >= 3) ? bb[2] : "0"
                        //                            });
                        //                        TempTable = new List<PDFTable>();
                        //                        DVFlg = 0;
                        //                    }
                        //                    catch (Exception ex)
                        //                    {
                        //                    }
                        //                }
                        //            }
                        //        }
                        //    }

                        //    if (text.Contains("Company Total"))
                        //    {
                        //        TempTable = new List<PDFTable>();
                        //        words = text.Split('\n');
                        //        decimal Amt = 0;
                        //        decimal AmtVol = 0;
                        //        int cntblank = 0;
                        //        for (int j = 0, len = words.Length; j < len; j++)
                        //        {
                        //            if (j != 0 && string.IsNullOrWhiteSpace(Convert.ToString(words[j])) && cntblank == 0)
                        //            {
                        //                cntblank++;
                        //                continue;
                        //            }
                        //            var LineData = Encoding.UTF8.GetString(Encoding.UTF8.GetBytes(words[j]));
                        //            if (LineData.Contains("Company Total"))
                        //            {
                        //                TableList.Add(new PDFTable { Name = LineData });
                        //            }
                        //            if (LineData.Contains("Employee Taxes Amount"))
                        //            {
                        //                try
                        //                {
                        //                    var PreLine = Encoding.UTF8.GetString(Encoding.UTF8.GetBytes(words[j - 1]));
                        //                    TempTable = new List<PDFTable>();
                        //                    TempTable1 = new List<PDFTable>();
                        //                    //TableList.Add(new PDFTable { Name = PreLine, Value = "", Name2 = "Employee Taxes", Value2 = "", Name3 = "Amount", Value3 = "", Name4 = "Employer Taxes", Value4 = "", Name5 = "Amount", Value5 = "" });
                        //                    TempTable.Add(
                        //                        new PDFTable
                        //                        {
                        //                            Name = PreLine,
                        //                            Value = string.Empty,
                        //                            Name2 = "Employee Taxes",
                        //                            Value2 = string.Empty,
                        //                            Name3 = "Amount",
                        //                            Value3 = string.Empty,
                        //                            Name4 = "Employer Taxes",
                        //                            Value4 = string.Empty,
                        //                            Name5 = "Amount",
                        //                            Value5 = string.Empty
                        //                        });
                        //                    TempTable1.Add(
                        //                        new PDFTable
                        //                        {
                        //                            Name = PreLine,
                        //                            Value = string.Empty,
                        //                            Name2 = "Voluntary Deductions",
                        //                            Value2 = string.Empty,
                        //                            Name3 = "Amount",
                        //                            Value3 = string.Empty
                        //                        });
                        //                    blnTotal = false;
                        //                    blnTotal1 = false;
                        //                }
                        //                catch (Exception ex)
                        //                {
                        //                    logger.Error("PDFReadController - ReadPayrollSummaryCheckFile2 - " + DateTime.Now + " - " + ex.Message.ToString());
                        //                }
                        //            }
                        //            else if (LineData.Contains("Net Pay"))
                        //            {
                        //                TempTable = new List<PDFTable>();
                        //                TempTable1 = new List<PDFTable>();
                        //                blnTotal = false;
                        //                blnTotal1 = false;
                        //            }
                        //            else if (LineData.Contains("Employer Taxes Amount"))
                        //            {
                        //                try
                        //                {
                        //                    var PreLine = Encoding.UTF8.GetString(Encoding.UTF8.GetBytes(words[j - 1]));
                        //                    TempTable = new List<PDFTable>();
                        //                    //TableList.Add(new PDFTable { Name = "Company Total", Value = "", Name2 = "Employer Taxes", Value2 = "", Name3 = "Amount", Value3 = "", Name4 = "Employer Taxes", Value4 = "", Name5 = "Amount", Value5 = "" });
                        //                    TempTable.Add(
                        //                        new PDFTable
                        //                        {
                        //                            Name = "Company Total",
                        //                            Value = string.Empty,
                        //                            Name2 = "Employer Taxes",
                        //                            Value2 = string.Empty,
                        //                            Name3 = "Amount",
                        //                            Value3 = string.Empty,
                        //                            Name4 = "Employer Taxes",
                        //                            Value4 = string.Empty,
                        //                            Name5 = "Amount",
                        //                            Value5 = string.Empty
                        //                        });
                        //                    Amt = 0;
                        //                }
                        //                catch (Exception ex)
                        //                {
                        //                    logger.Error("PDFReadController - ReadPayrollSummaryCheckFile3 - " + DateTime.Now + " - " + ex.Message.ToString());
                        //                }

                        //                //Voluntary Deduction
                        //                string[] authorsList = LineData.Trim().Split(' ');
                        //                string[] bb = GetFinalArr(authorsList);
                        //                if (bb.Length > 2 && blnTotal1 == false)
                        //                {
                        //                    var DigitLine = bb[1].Replace("$", string.Empty)
                        //                        .Replace(" ", string.Empty)
                        //                        .Replace(".", string.Empty)
                        //                        .Replace(",", string.Empty)
                        //                        .Replace("(", string.Empty)
                        //                        .Replace(")", string.Empty)
                        //                        .TrimStart(new Char[] { '0' });
                        //                    if (DigitLine.Trim().All(s => char.IsDigit(s)))
                        //                    {
                        //                        var TempData1 = TempTable1;
                        //                        var TempValues1 = TempData1.FirstOrDefault();

                        //                        var PreLine = Encoding.UTF8
                        //                            .GetString(Encoding.UTF8.GetBytes(words[j + 1]));
                        //                        string[] bb1 = GetFinalArrRev(PreLine.Split(' '));

                        //                        if (bb[0].ToString().Contains("Spread of Hours"))
                        //                        {
                        //                            int index = Array.IndexOf(bb, "100.00%");
                        //                            var list = new List<string>(bb);
                        //                            list.RemoveRange(0, index + 1);
                        //                            //for (int k=0;k<= index-1;k++)
                        //                            //{
                        //                            //    list.Remove(k);
                        //                            //}
                        //                            bb = list.ToArray();
                        //                            bb = GetFinalArr(bb);
                        //                        }
                        //                        if (bb1[0].ToString().Contains("Hours of Spread"))
                        //                        {
                        //                            int index = Array.IndexOf(bb, "100.00%");
                        //                            var list = new List<string>(bb);
                        //                            list.RemoveRange(0, index + 1);
                        //                            //for (int k=0;k<= index-1;k++)
                        //                            //{
                        //                            //    list.Remove(k);
                        //                            //}
                        //                            bb = list.ToArray();
                        //                            bb = GetFinalArr(bb);
                        //                        }
                        //                        try
                        //                        {
                        //                            string aa = bb[0].Substring(bb[0].ToString().IndexOf("%") + 1)
                        //                                .Trim();
                        //                            TableList.Add(
                        //                                new PDFTable
                        //                                {
                        //                                    Name = TempValues1.Name,
                        //                                    Value = string.Empty,
                        //                                    Name2 = TempValues1.Name2,
                        //                                    Value2 = aa.Trim(),
                        //                                    Name3 = TempValues1.Name3,
                        //                                    Value3 = bb[1]
                        //                                });
                        //                            if (bb[1].ToString().Contains("("))
                        //                            {
                        //                                string Valuestr = bb[1].ToString()
                        //                                    .Replace("$", string.Empty)
                        //                                    .Replace("(", string.Empty)
                        //                                    .Replace(")", string.Empty)
                        //                                    .Replace(",", string.Empty);
                        //                                decimal Value = Convert.ToDecimal(Valuestr);
                        //                                AmtVol += (-Value);
                        //                            }
                        //                            else
                        //                            {
                        //                                string Valuestr = bb[1].ToString()
                        //                                    .Replace("$", string.Empty)
                        //                                    .Replace(",", string.Empty);
                        //                                decimal Value = Convert.ToDecimal(Valuestr);
                        //                                AmtVol += Value;
                        //                            }
                        //                        }
                        //                        catch (Exception ex)
                        //                        {
                        //                            logger.Error("PDFReadController - ReadPayrollSummaryCheckFile4 - " + DateTime.Now + " - " + ex.Message.ToString());
                        //                        }
                        //                    }
                        //                }
                        //            }
                        //            else
                        //            {
                        //                var TempData = TempTable;
                        //                if (TempTable.Count() > 0 || TempTable1.Count() > 0)
                        //                {
                        //                    string[] authorsList = LineData.Trim().Split(' ');
                        //                    int StringCount = authorsList.Count();
                        //                    var TempValues = TempData.FirstOrDefault();
                        //                    Array.Reverse(authorsList);
                        //                    if (LineData.Trim().EndsWith("%"))
                        //                    {
                        //                        authorsList = authorsList.Where((item, index) => index != 0).ToArray();
                        //                        blnTotal = false;
                        //                    }
                        //                    string[] bb = GetFinalArrRev(authorsList);
                        //                    if (bb.Length > 0)
                        //                    {
                        //                        if (blnTotal == false)
                        //                        {
                        //                            if (bb.Length != 2 || LineData.Trim().EndsWith("%"))
                        //                            {
                        //                                string str1 = bb[0].ToString()
                        //                                    .Replace("$", string.Empty)
                        //                                    .Replace("(", string.Empty)
                        //                                    .Replace(")", string.Empty)
                        //                                    .Replace(",", string.Empty)
                        //                                    .Trim();
                        //                                decimal V1 = Convert.ToDecimal(str1);
                        //                                var PreLine = Encoding.UTF8
                        //                                    .GetString(Encoding.UTF8.GetBytes(words[j + 1]));
                        //                                string[] bb1 = GetFinalArrRev(PreLine.Split(' '));
                        //                                if (bb1.Length == 1)
                        //                                {
                        //                                    try
                        //                                    {
                        //                                        string str2 = bb1[0].ToString()
                        //                                            .Replace("$", string.Empty)
                        //                                            .Replace("(", string.Empty)
                        //                                            .Replace(")", string.Empty)
                        //                                            .Replace(",", string.Empty)
                        //                                            .Trim();
                        //                                        if (str2.ToCharArray().Count(c => c == '.') > 2 &&
                        //                                            (str2.All(s => char.IsDigit(s)) == false))
                        //                                        {
                        //                                            //str2 = str2.Substring(0, str2.IndexOf('.') + 3);
                        //                                        }
                        //                                        else
                        //                                        {
                        //                                            decimal V2 = Convert.ToDecimal(str2);
                        //                                            if (Amt == V2)
                        //                                            {
                        //                                                TempTable = new List<PDFTable>();
                        //                                                blnTotal = true;
                        //                                            }
                        //                                        }
                        //                                    }
                        //                                    catch (Exception ex)
                        //                                    {
                        //                                        logger.Error("PDFReadController - ReadPayrollSummaryCheckFile5 - " + DateTime.Now + " - " + ex.Message.ToString());
                        //                                    }
                        //                                }
                        //                                if (Amt == V1)
                        //                                {
                        //                                    TempTable = new List<PDFTable>();
                        //                                    blnTotal = true;
                        //                                }
                        //                                if (blnTotal == false)
                        //                                {
                        //                                    if (bb.Length > 1)
                        //                                    {
                        //                                        if (TempValues != null)
                        //                                        {
                        //                                            TableList.Add(
                        //                                                new PDFTable
                        //                                                {
                        //                                                    Name = TempValues.Name,
                        //                                                    Value = string.Empty,
                        //                                                    Name2 = TempValues.Name2,
                        //                                                    Value2 = bb[1],
                        //                                                    Name3 = TempValues.Name3,
                        //                                                    Value3 = bb[0]
                        //                                                });
                        //                                        }
                        //                                        else
                        //                                        {
                        //                                            TableList.Add(
                        //                                                new PDFTable
                        //                                                {
                        //                                                    Name = string.Empty,
                        //                                                    Value = string.Empty,
                        //                                                    Name2 = string.Empty,
                        //                                                    Value2 = bb[1],
                        //                                                    Name3 = string.Empty,
                        //                                                    Value3 = bb[0]
                        //                                                });
                        //                                        }
                        //                                        if (bb[0].ToString().Contains("("))
                        //                                        {
                        //                                            try
                        //                                            {
                        //                                                string Valuestr = bb[0].ToString()
                        //                                                    .Replace("$", string.Empty)
                        //                                                    .Replace("(", string.Empty)
                        //                                                    .Replace(")", string.Empty)
                        //                                                    .Replace(",", string.Empty);
                        //                                                decimal Value = Convert.ToDecimal(Valuestr);
                        //                                                Amt += (-Value);
                        //                                            }
                        //                                            catch (Exception ex)
                        //                                            {
                        //                                                logger.Error("PDFReadController - ReadPayrollSummaryCheckFile6 - " + DateTime.Now + " - " + ex.Message.ToString());
                        //                                            }
                        //                                        }
                        //                                        else
                        //                                        {
                        //                                            try
                        //                                            {
                        //                                                string Valuestr = bb[0].ToString()
                        //                                                    .Replace("$", string.Empty)
                        //                                                    .Replace(",", string.Empty);
                        //                                                decimal Value = Convert.ToDecimal(Valuestr);
                        //                                                Amt += Value;
                        //                                            }
                        //                                            catch (Exception ex)
                        //                                            {
                        //                                                logger.Error("PDFReadController - ReadPayrollSummaryCheckFile7 - " + DateTime.Now + " - " + ex.Message.ToString());
                        //                                            }
                        //                                        }
                        //                                    }
                        //                                }
                        //                            }
                        //                        }

                        //                        if (blnTotal1 == false)
                        //                        {
                        //                            var TempData1 = TempTable1;
                        //                            var TempValues1 = TempData1.FirstOrDefault();
                        //                            var PreLine = Encoding.UTF8
                        //                                .GetString(Encoding.UTF8.GetBytes(words[j + 1]));
                        //                            string[] bb1 = GetFinalArrRev(PreLine.Split(' '));
                        //                            if (bb1.Length == 1 && !bb1[0].Contains("%"))
                        //                            {
                        //                                string str2 = bb1[0].ToString()
                        //                                    .Replace("$", string.Empty)
                        //                                    .Replace("(", string.Empty)
                        //                                    .Replace(")", string.Empty)
                        //                                    .Replace(",", string.Empty)
                        //                                    .Trim();
                        //                                decimal V2 = Convert.ToDecimal(str2);
                        //                                if (AmtVol == V2)
                        //                                {
                        //                                    TempTable1 = new List<PDFTable>();
                        //                                    blnTotal1 = true;
                        //                                }
                        //                            }
                        //                            else
                        //                            {
                        //                                if (bb.Length > 2)
                        //                                {
                        //                                    if (!bb[1].StartsWith("100.00%"))
                        //                                    {
                        //                                        bb = bb.Where((item, index) => index != 0 && index != 1)
                        //                                            .ToArray();
                        //                                        bb = GetFinalArrRev(bb);
                        //                                    }
                        //                                }
                        //                            }
                        //                            try
                        //                            {
                        //                                string str1 = bb[0].ToString()
                        //                                    .Replace("$", string.Empty)
                        //                                    .Replace("(", string.Empty)
                        //                                    .Replace(")", string.Empty)
                        //                                    .Replace(",", string.Empty)
                        //                                    .Trim();
                        //                                decimal V1 = Convert.ToDecimal(str1);
                        //                                if (AmtVol == V1)
                        //                                {
                        //                                    TempTable1 = new List<PDFTable>();
                        //                                    blnTotal1 = true;
                        //                                }
                        //                                if (blnTotal1 == false && bb.Length > 1)
                        //                                {
                        //                                    string aa = bb[1].Substring(
                        //                                        bb[1].ToString().IndexOf("%") + 1)
                        //                                        .Trim();
                        //                                    string tvalue2 = TableList[TableList.Count - 1].Value2 == null ? "" : TableList[TableList.Count - 1].Value2.ToString();
                        //                                    string tvalue3 = TableList[TableList.Count - 1].Value3 == null ? "" : TableList[TableList.Count - 1].Value3.ToString();

                        //                                    if (tvalue2 != aa &&
                        //                                        tvalue3 != bb[0])
                        //                                    {
                        //                                        TableList.Add(
                        //                                            new PDFTable
                        //                                            {
                        //                                                Name = TempValues1.Name,
                        //                                                Value = string.Empty,
                        //                                                Name2 = TempValues1.Name2,
                        //                                                Value2 = aa.Trim(),
                        //                                                Name3 = TempValues1.Name3,
                        //                                                Value3 = bb[0]
                        //                                            });
                        //                                        if (bb[0].ToString().Contains("("))
                        //                                        {
                        //                                            try
                        //                                            {
                        //                                                string Valuestr = bb[0].ToString()
                        //                                                    .Replace("$", string.Empty)
                        //                                                    .Replace("(", string.Empty)
                        //                                                    .Replace(")", string.Empty)
                        //                                                    .Replace(",", string.Empty);
                        //                                                decimal Value = Convert.ToDecimal(Valuestr);
                        //                                                AmtVol += (-Value);
                        //                                            }
                        //                                            catch (Exception ex)
                        //                                            {
                        //                                                logger.Error("PDFReadController - ReadPayrollSummaryCheckFile8 - " + DateTime.Now + " - " + ex.Message.ToString());
                        //                                            }
                        //                                        }
                        //                                        else
                        //                                        {
                        //                                            try
                        //                                            {
                        //                                                string Valuestr = bb[0].ToString()
                        //                                                    .Replace("$", string.Empty)
                        //                                                    .Replace(",", string.Empty);
                        //                                                decimal Value = Convert.ToDecimal(Valuestr);
                        //                                                AmtVol += Value;
                        //                                            }
                        //                                            catch (Exception ex)
                        //                                            {
                        //                                                logger.Error("PDFReadController - ReadPayrollSummaryCheckFile9 - " + DateTime.Now + " - " + ex.Message.ToString());
                        //                                            }
                        //                                        }
                        //                                    }
                        //                                }
                        //                            }
                        //                            catch (Exception ex)
                        //                            {
                        //                                logger.Error("PDFReadController - ReadPayrollSummaryCheckFile10 - " + DateTime.Now + " - " + ex.Message.ToString());
                        //                            }
                        //                        }
                        //                    }
                        //                    else
                        //                    {
                        //                        blnTotal = true;
                        //                    }
                        //                }
                        //            }
                        //        }
                        //    }
                        //}

                        //if (FileNo == 2)
                        //{
                        //    int T = 0;
                        //    words = text.Split('\n');
                        //    for (int j = 0, len = words.Length; j < len; j++)
                        //    {
                        //        var LineData = Encoding.UTF8.GetString(Encoding.UTF8.GetBytes(words[j]));
                        //        if (LineData.Contains("Payroll Fee"))
                        //        {
                        //            string[] authorsList = LineData.Trim().Split(' ');
                        //            string[] bb = GetFinalArr(authorsList);
                        //            TableList.Add(new PDFTable { Name = bb[0], Value = bb[1], Name2 = "File2" });
                        //        }
                        //        else if (LineData.Contains("New York IAS") && T == 0)
                        //        {
                        //            T = 1;
                        //            string[] authorsList = LineData.Trim().Split(' ');
                        //            string[] bb = GetFinalArr(authorsList);
                        //            TableList.Add(new PDFTable { Name = bb[0], Value = bb[1], Name2 = "File2" });
                        //        }
                        //        else if (LineData.Contains("Manual Checks"))
                        //        {
                        //            string[] authorsList = LineData.Trim().Split(' ');
                        //            string[] bb = GetFinalArrRev(authorsList);
                        //            TableList.Add(
                        //                new PDFTable { Name = "Manual Checks", Value = bb[1], Name2 = "File2" });
                        //        }
                        //    }
                        //}
                    }

                    List<PDFTable> PDFPrintList = new List<PDFTable>();
                    reader.Close();
                    reader.Dispose();
                    int flg = 0;
                    int DepartmentId = 0;
                    PayrollMaster obj = new PayrollMaster();
                    obj.StoreId = Convert.ToInt32(Session["storeid"].ToString());
                    obj.StartDate = DateTime.Today;
                    if (PeriodEndDate != string.Empty)
                    {
                        obj.EndDate = Convert.ToDateTime(PeriodEndDate);
                    }
                    else
                    {
                        obj.EndDate = DateTime.Today;
                    }
                    if (CheckDate != string.Empty)
                    {
                        obj.EndCheckDate = Convert.ToDateTime(CheckDate);
                    }
                    else
                    {
                        obj.EndCheckDate = DateTime.Today;
                    }
                    var FileExist = from PR in db.PayrollReports
                                    join PM in db.PayrollMasters on PR.PayrollReportId equals PM.PayrollReportId
                                    where PR.TransactionNo == Transaction &&
                                        PM.EndCheckDate == obj.EndCheckDate &&
                                        PR.FIleNo == FileNo
                                    select new { PayrollReports = PR, PayrollMasters = PM };
                    if (FileExist.ToList().Count() > 0)
                    {
                        value = 1;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("PDFReadController - ReadPayrollSummaryCheckFile11 - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return value;
        }

        /// <summary>
        /// This function is Read PDF for check File.
        /// </summary>
        /// <param name="FileName"></param>
        public int ReadPDFCheckFile(string FileName)
        {
            int value = 0;
            try
            {
                if (FileName != string.Empty)
                {
                    int PayrollId = 0;

                    string FilePath = Server.MapPath("~/userfiles/PayrollFile/") + FileName;
                    List<PDFTable> TableList = new List<PDFTable>();
                    List<PDFTable> TempTable = new List<PDFTable>();

                    List<PDFTable> TableList1 = new List<PDFTable>();
                    List<PDFTable> TempTable1 = new List<PDFTable>();
                    List<PDFTable> TempTable2 = new List<PDFTable>();

                    PdfReader reader = new PdfReader(FilePath);
                    int intPageNum = reader.NumberOfPages;

                    string StartDate = string.Empty;
                    string EndDate = string.Empty;
                    string EndCheckDate = string.Empty;
                    string CheckDate = string.Empty;
                    string[] words;
                    StringBuilder Reader = new StringBuilder();
                    for (int i = 1; i <= intPageNum; i++)
                    {
                        string text = PdfTextExtractor.GetTextFromPage(reader, i, new LocationTextExtractionStrategy());
                        //if (StartDate == "")
                        //{
                        if (text.Contains("Period Sart:") ||
                            text.Contains("Period End:") ||
                            text.Contains("Period Start:") ||
                            text.Contains("End Check Date:") ||
                            text.Contains("Check Date:"))
                        {
                            words = text.Split('\n');

                            for (int j = 0, len = words.Length; j < len; j++)
                            {
                                var LineData = Encoding.UTF8.GetString(Encoding.UTF8.GetBytes(words[j]));
                                if ((LineData.Contains("Period Sart:") || LineData.Contains("Period Start:")) &&
                                    StartDate == string.Empty)
                                {
                                    string[] DateSplitData = LineData.Split(':');
                                    if (DateSplitData.Count() > 0)
                                    {
                                        StartDate = DateSplitData[1];
                                        string[] DateFormate = StartDate.Split('/');
                                        if (DateFormate.Count() == 3)
                                        {
                                            var Month = DateFormate[0].Trim().Length == 1
                                                ? "0" + DateFormate[0].Trim()
                                                : DateFormate[0].Trim();
                                            StartDate = DateFormate[2] + "-" + Month + "-" + DateFormate[1];
                                        }
                                    }
                                }
                                if (LineData.Contains("Period End:") && EndDate == string.Empty)
                                {
                                    string[] DateSplitData = LineData.Split(':');
                                    if (DateSplitData.Count() > 0)
                                    {
                                        EndDate = DateSplitData[1];
                                        string[] DateFormate = EndDate.Split('/');
                                        if (DateFormate.Count() == 3)
                                        {
                                            var Month = DateFormate[0].Trim().Length == 1
                                                ? "0" + DateFormate[0].Trim()
                                                : DateFormate[0].Trim();
                                            EndDate = DateFormate[2].Trim() + "-" + Month + "-" + DateFormate[1].Trim();
                                        }
                                    }
                                }
                                if ((LineData.Contains("End Check Date:") || LineData.Contains("Check Date:")) &&
                                    EndCheckDate == string.Empty)
                                {
                                    if (LineData.Contains("Start") == false)
                                    {
                                        if (LineData.Contains("End Check Date:") && EndCheckDate == string.Empty)
                                        {
                                            string[] DateSplitData = LineData.Split(':');
                                            if (DateSplitData.Count() > 0)
                                            {
                                                EndCheckDate = DateSplitData[1];
                                                string[] DateFormate = EndCheckDate.Split('/');
                                                if (DateFormate.Count() == 3)
                                                {
                                                    var Month = DateFormate[0].Trim().Length == 1
                                                        ? "0" + DateFormate[0].Trim()
                                                        : DateFormate[0].Trim();
                                                    var Day = DateFormate[1].Trim().Length == 1
                                                        ? "0" + DateFormate[1].Trim()
                                                        : DateFormate[1].Trim();
                                                    EndCheckDate = DateFormate[2].Trim() + "-" + Month + "-" + Day;
                                                }
                                            }
                                        }
                                        else if (LineData.Contains("Check Date:") && CheckDate == string.Empty)
                                        {
                                            string[] DateSplitData = LineData.Split(':');
                                            if (DateSplitData.Count() > 0)
                                            {
                                                CheckDate = DateSplitData[1];
                                                string[] DateFormate = CheckDate.Split('/');
                                                if (DateFormate.Count() == 3)
                                                {
                                                    var Month = DateFormate[0].Trim().Length == 1
                                                        ? "0" + DateFormate[0].Trim()
                                                        : DateFormate[0].Trim();
                                                    var Day = DateFormate[1].Trim().Length == 1
                                                        ? "0" + DateFormate[1].Trim()
                                                        : DateFormate[1].Trim();
                                                    CheckDate = DateFormate[2].Trim() + "-" + Month + "-" + Day;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        //}

                        if (text.Contains("Summary:"))
                        {
                            words = text.Split('\n');
                            //words = text;
                            int DVFlg = 0;
                            for (int j = 0, len = words.Length; j < len; j++)
                            {
                                try
                                {
                                    var LineData = Encoding.UTF8.GetString(Encoding.UTF8.GetBytes(words[j]));
                                    if (LineData.Contains("Summary:"))
                                    {
                                        DVFlg = 1;
                                    }
                                    if (DVFlg == 1)
                                    {
                                        LineData = LineData.Trim();
                                        //TableList.Add(new PDFTable { Name = LineData });
                                        if (LineData.Contains("Summary:"))
                                        {
                                            if (LineData.Contains("Summary: 85-SALAD B  - 85-SALAD"))
                                            {
                                            }

                                            if (LineData.Contains("Summary: 85 SALAD  - 85-SALAD BAR Net Pay:"))
                                            {
                                                var PreLine = Encoding.UTF8
                                                    .GetString(Encoding.UTF8.GetBytes(words[j + 1]));
                                                string[] authorsList = PreLine.Split(' ');

                                                //if (StringCount == 10)
                                                //{
                                                TableList.Add(new PDFTable { Name = string.Empty });
                                                TableList.Add(
                                                    new PDFTable
                                                    {
                                                        Name = LineData.Replace("Net Pay:", string.Empty).Trim(),
                                                        Name5 = "Net Pay",
                                                        Value5 = authorsList[0]
                                                    });
                                            }
                                            else if (LineData.Contains("Summary: 85 SALAD  - 85-SALAD BAR"))
                                            {
                                                var PreLine = Encoding.UTF8
                                                    .GetString(Encoding.UTF8.GetBytes(words[j + 1]));
                                                string[] authorsList = PreLine.Split(' ');

                                                //if (StringCount == 10)
                                                //{
                                                TableList.Add(new PDFTable { Name = string.Empty });
                                                TableList.Add(
                                                    new PDFTable
                                                    {
                                                        Name = LineData,
                                                        Name5 = authorsList[0] + " " + authorsList[1],
                                                        Value5 = authorsList[2]
                                                    });
                                            }
                                            else if (LineData.Contains("Summary:") && LineData.Contains("Net Pay:"))
                                            {
                                                try
                                                {
                                                    string[] authorsList = LineData.Split(' ');
                                                    int StringCount = authorsList.Count();
                                                    int CountLmt = StringCount >= 8 ? 3 : StringCount == 7 ? 2 : 1;

                                                    string MergeName = string.Empty;
                                                    for (int x = 0; x < StringCount - CountLmt; x++)
                                                    {
                                                        MergeName = MergeName == string.Empty ? authorsList[x]
                                                            : MergeName + " " + authorsList[x];
                                                    }
                                                    //if (StringCount == 10)
                                                    //{
                                                    TableList.Add(new PDFTable { Name = string.Empty });
                                                    TableList.Add(
                                                        new PDFTable
                                                        {
                                                            Name = MergeName,
                                                            Name5 =
                                                                authorsList[StringCount - 3] +
                                                                        " " +
                                                                        authorsList[StringCount - 2],
                                                            Value5 = authorsList[StringCount - 1]
                                                        });
                                                }
                                                catch (Exception ex)
                                                {
                                                }
                                            }
                                            else
                                            {
                                                TableList.Add(new PDFTable { Name = string.Empty });
                                                TableList.Add(new PDFTable { Name = LineData });
                                            }
                                            //}
                                        }
                                        else
                                        {
                                            if (LineData.Contains("CHILD SUP"))
                                            {
                                                LineData = LineData.Replace("CHILD SUP", "CHILD_SUP");
                                            }
                                            if (LineData.Contains("PAID SICK"))
                                            {
                                                LineData = LineData.Replace("PAID SICK", "PAID_SICK");
                                            }
                                            if (LineData.Contains("CCFEE D"))
                                            {
                                                LineData = LineData.Replace("CCFEE D", "CCFEE_D");
                                            }
                                            if (LineData.Contains("CCTIP D"))
                                            {
                                                LineData = LineData.Replace("CCTIP D", "CCTIP_D");
                                            }
                                            if (LineData.Contains("LIFE INS."))
                                            {
                                                LineData = LineData.Replace("LIFE INS.", "LIFE_INS.");
                                            }


                                            if (LineData.Contains(" "))
                                            {
                                                string[] authorsList = LineData.Split(' ');
                                                int StringCount = authorsList.Count();
                                                if (StringCount > 10)
                                                {
                                                    TableList.Add(
                                                        new PDFTable
                                                        {
                                                            Name = authorsList[0],
                                                            Value = authorsList[2],
                                                            Name2 = authorsList[3],
                                                            Value2 = authorsList[4],
                                                            Name3 = authorsList[5],
                                                            Value3 = authorsList[6],
                                                            Name4 = authorsList[7],
                                                            Value4 = authorsList[8],
                                                            Name5 = authorsList[9],
                                                            Value5 = authorsList[10]
                                                        });
                                                }
                                                else if (LineData.Contains("RETRO PAY"))// || LineData.Contains("RETRO OT"))
                                                {
                                                    if (LineData.Contains("$"))
                                                    {
                                                        TableList.Add(
                                                            new PDFTable
                                                            {
                                                                Name = authorsList[0] + " " + authorsList[1],
                                                                Value = authorsList[2],
                                                                Name2 = authorsList[3],
                                                                Value2 = authorsList[4]
                                                            });
                                                    }
                                                    else
                                                    {
                                                        var TempData = TempTable;
                                                        if (TempTable.Count() > 0)
                                                        {
                                                            var TempValues = TempData.FirstOrDefault();
                                                            TableList.Add(
                                                                new PDFTable
                                                                {
                                                                    Name = authorsList[0] + " " + authorsList[1],
                                                                    Value = TempValues.Value,
                                                                    Name2 = authorsList[2],
                                                                    Value2 = TempValues.Value2
                                                                });
                                                            TempTable = new List<PDFTable>();
                                                        }
                                                    }
                                                }
                                                else if (LineData.Contains("OT PREMIUM") ||
                                                    LineData.Contains("OT PREMIU"))
                                                {
                                                    if (LineData.Contains("$"))
                                                    {
                                                        TableList.Add(
                                                            new PDFTable
                                                            {
                                                                Name = authorsList[0] + " " + authorsList[1],
                                                                Value = authorsList[2],
                                                                Name2 = authorsList[3],
                                                                Value2 = authorsList[4]
                                                            });
                                                    }
                                                    else
                                                    {
                                                        var TempData = TempTable;
                                                        if (TempTable.Count() > 0)
                                                        {
                                                            var TempValues = TempData.FirstOrDefault();
                                                            TableList.Add(
                                                                new PDFTable
                                                                {
                                                                    Name = authorsList[0] + " " + authorsList[1],
                                                                    Value = TempValues.Value,
                                                                    Name2 = authorsList[2],
                                                                    Value2 = TempValues.Value2
                                                                });
                                                            TempTable = new List<PDFTable>();
                                                        }
                                                    }
                                                }
                                                else if (StringCount == 10)
                                                {
                                                    TableList.Add(
                                                        new PDFTable
                                                        {
                                                            Name = authorsList[0],
                                                            Value = authorsList[1],
                                                            Name2 = authorsList[2],
                                                            Value2 = authorsList[3],
                                                            Name3 = authorsList[4],
                                                            Value3 = authorsList[5],
                                                            Name4 = authorsList[6],
                                                            Value4 = authorsList[7],
                                                            Name5 = authorsList[8],
                                                            Value5 = authorsList[9]
                                                        });
                                                }
                                                else if (StringCount == 8)
                                                {
                                                    TableList.Add(
                                                        new PDFTable
                                                        {
                                                            Name = authorsList[0],
                                                            Value = authorsList[1],
                                                            Name2 = authorsList[2],
                                                            Value2 = authorsList[3],
                                                            Name3 = authorsList[4],
                                                            Value3 = authorsList[5],
                                                            Name4 = authorsList[6],
                                                            Value4 = authorsList[7]
                                                        });
                                                }
                                                else if (StringCount == 6)
                                                {
                                                    if (authorsList[0].ToLower() == "covid")
                                                    {
                                                        TableList.Add(
                                                            new PDFTable
                                                            {
                                                                Name = authorsList[0] + " " + authorsList[1],
                                                                Value = authorsList[2],
                                                                Name2 = authorsList[3],
                                                                Value2 = authorsList[4]
                                                            });
                                                    }
                                                    else
                                                    {
                                                        TableList.Add(
                                                            new PDFTable
                                                            {
                                                                Name = authorsList[0],
                                                                Value = authorsList[1],
                                                                Name2 = authorsList[2],
                                                                Value2 = authorsList[3]
                                                            });
                                                    }
                                                }
                                                else if (StringCount == 5)
                                                {
                                                    if (LineData.Contains("$"))
                                                    {
                                                        TempTable.Add(
                                                            new PDFTable
                                                            {
                                                                Name = string.Empty,
                                                                Value = authorsList[0],
                                                                Name2 = string.Empty,
                                                                Value2 = authorsList[1],
                                                                Name3 = string.Empty,
                                                                Value3 = authorsList[2],
                                                                Name4 = string.Empty,
                                                                Value4 = authorsList[3],
                                                                Name5 = string.Empty,
                                                                Value5 = authorsList[4]
                                                            });
                                                    }
                                                    else
                                                    {
                                                        var TempData = TempTable;
                                                        if (TempTable.Count() > 0)
                                                        {
                                                            var TempValues = TempData.FirstOrDefault();
                                                            TableList.Add(
                                                                new PDFTable
                                                                {
                                                                    Name = authorsList[0],
                                                                    Value = TempValues.Value,
                                                                    Name2 = authorsList[1],
                                                                    Value2 = TempValues.Value2,
                                                                    Name3 = authorsList[2],
                                                                    Value3 = TempValues.Value3,
                                                                    Name4 = authorsList[3],
                                                                    Value4 = TempValues.Value4,
                                                                    Name5 = authorsList[4],
                                                                    Value5 = TempValues.Value5
                                                                });
                                                            TempTable = new List<PDFTable>();
                                                        }
                                                    }
                                                }
                                                else if (StringCount == 4)
                                                {
                                                    if (LineData.Contains("$"))
                                                    {
                                                        var DigitLine = LineData.Replace("$", string.Empty)
                                                            .Replace(" ", string.Empty)
                                                            .Replace(".", string.Empty)
                                                            .Replace(",", string.Empty)
                                                            .Replace("(", string.Empty)
                                                            .Replace(")", string.Empty);
                                                        if (DigitLine.All(s => char.IsDigit(s)))
                                                        {
                                                            TempTable.Add(
                                                                new PDFTable
                                                                {
                                                                    Name = string.Empty,
                                                                    Value = authorsList[0],
                                                                    Name2 = string.Empty,
                                                                    Value2 = authorsList[1],
                                                                    Name3 = string.Empty,
                                                                    Value3 = authorsList[2],
                                                                    Name4 = string.Empty,
                                                                    Value4 = authorsList[3]
                                                                });
                                                        }
                                                        else
                                                        {
                                                            if (LineData.Contains("SSEC") ||
                                                                LineData.Contains("MEDI") ||
                                                                LineData.Contains("FWT"))
                                                            {
                                                                TableList.Add(
                                                                    new PDFTable
                                                                    {
                                                                        Name = string.Empty,
                                                                        Name3 = authorsList[0],
                                                                        Value3 = authorsList[1],
                                                                        Name4 = authorsList[2],
                                                                        Value4 = authorsList[3]
                                                                    });
                                                            }
                                                            else
                                                            {
                                                                TableList.Add(
                                                                    new PDFTable
                                                                    {
                                                                        Name = authorsList[0],
                                                                        Value = authorsList[1],
                                                                        Name2 = authorsList[2],
                                                                        Value2 = authorsList[3]
                                                                    });
                                                            }
                                                        }
                                                    }
                                                    else
                                                    {
                                                        var TempData = TempTable;
                                                        if (TempTable.Count() > 0)
                                                        {
                                                            var TempValues = TempData.FirstOrDefault();
                                                            TableList.Add(
                                                                new PDFTable
                                                                {
                                                                    Name = authorsList[0],
                                                                    Value = TempValues.Value,
                                                                    Name2 = authorsList[1],
                                                                    Value2 = TempValues.Value2,
                                                                    Name3 = authorsList[2],
                                                                    Value3 = TempValues.Value3,
                                                                    Name4 = authorsList[3],
                                                                    Value4 = TempValues.Value4
                                                                });
                                                            TempTable = new List<PDFTable>();
                                                        }
                                                    }
                                                }
                                                else if (StringCount == 3)
                                                {
                                                    if (LineData.Contains("$"))
                                                    {
                                                        var DigitLine = LineData.Replace("$", string.Empty)
                                                            .Replace(" ", string.Empty)
                                                            .Replace(".", string.Empty)
                                                            .Replace(",", string.Empty)
                                                            .Replace("(", string.Empty)
                                                            .Replace(")", string.Empty);
                                                        if (DigitLine.All(s => char.IsDigit(s)))
                                                        {
                                                            TempTable.Add(
                                                                new PDFTable
                                                                {
                                                                    Name = string.Empty,
                                                                    Value = authorsList[0],
                                                                    Name2 = string.Empty,
                                                                    Value2 = authorsList[1],
                                                                    Name3 = string.Empty,
                                                                    Value3 = authorsList[2]
                                                                });
                                                        }
                                                        else
                                                        {
                                                            if (LineData.Contains("SSEC") ||
                                                                LineData.Contains("MEDI") ||
                                                                LineData.Contains("FWT"))
                                                            {
                                                                TableList.Add(
                                                                    new PDFTable
                                                                    {
                                                                        Name = string.Empty,
                                                                        Name3 = authorsList[0],
                                                                        Value3 = authorsList[1],
                                                                        Name4 = authorsList[2],
                                                                    });
                                                            }
                                                            else
                                                            {
                                                                TableList.Add(
                                                                    new PDFTable
                                                                    {
                                                                        Name = authorsList[0],
                                                                        Value = authorsList[1],
                                                                        Name2 = authorsList[2]
                                                                    });
                                                            }
                                                        }
                                                    }
                                                    else
                                                    {
                                                        var TempData = TempTable;
                                                        if (TempTable.Count() > 0)
                                                        {
                                                            var TempValues = TempData.FirstOrDefault();
                                                            TableList.Add(
                                                                new PDFTable
                                                                {
                                                                    Name = authorsList[0],
                                                                    Value = TempValues.Value,
                                                                    Name2 = authorsList[1],
                                                                    Value2 = TempValues.Value2,
                                                                    Name3 = authorsList[2]
                                                                });
                                                            TempTable = new List<PDFTable>();
                                                        }
                                                    }
                                                }
                                                else if (StringCount == 2)
                                                {
                                                    if (LineData.Contains("$"))
                                                    {
                                                        var DigitLine = LineData.Replace("$", string.Empty)
                                                            .Replace(" ", string.Empty)
                                                            .Replace(".", string.Empty)
                                                            .Replace(",", string.Empty);
                                                        if (DigitLine.All(s => char.IsDigit(s)))
                                                        {
                                                            TempTable.Add(
                                                                new PDFTable
                                                                {
                                                                    Name = string.Empty,
                                                                    Value3 = authorsList[0],
                                                                    Name2 = string.Empty,
                                                                    Value4 = authorsList[1]
                                                                });
                                                        }
                                                        else
                                                        {
                                                            if (LineData.Contains("LOCYONRES") ||
                                                                LineData.Contains("LOCNYCRES"))
                                                            {
                                                                TableList.Add(
                                                                    new PDFTable
                                                                    {
                                                                        Name = string.Empty,
                                                                        Value3 = authorsList[0],
                                                                        Value4 = authorsList[1]
                                                                    });
                                                            }
                                                        }
                                                    }
                                                    else
                                                    {
                                                        var TempData = TempTable;
                                                        if (TempTable.Count() > 0)
                                                        {
                                                            if (authorsList[1] == "HOURS")
                                                            {
                                                                var TempValues = TempData.FirstOrDefault();
                                                                TableList.Add(
                                                                    new PDFTable
                                                                    {
                                                                        Name = authorsList[0],
                                                                        Value = TempValues.Value3,
                                                                        Name2 = authorsList[1],
                                                                        Value2 = TempValues.Value4
                                                                    });
                                                                TempTable = new List<PDFTable>();
                                                            }
                                                            else
                                                            {
                                                                var TempValues = TempData.FirstOrDefault();
                                                                TableList.Add(
                                                                    new PDFTable
                                                                    {
                                                                        Name = string.Empty,
                                                                        Name3 = authorsList[0],
                                                                        Value3 = TempValues.Value3,
                                                                        Name4 = authorsList[1],
                                                                        Value4 = TempValues.Value4
                                                                    });
                                                                TempTable = new List<PDFTable>();
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                            if (LineData.Contains("Total"))
                                            {
                                                DVFlg = 0;
                                            }
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    logger.Error("PDFReadController - Grid - " + DateTime.Now + " - " + ex.Message.ToString());
                                }
                            }
                        }

                        if (text.Contains("CASH ANALYSIS"))
                        {
                            TempTable1 = new List<PDFTable>();
                            TableList1 = new List<PDFTable>();
                            words = text.Split('\n');
                            bool blnGPay = false;
                            for (int j = 0, len = words.Length; j < len; j++)
                            {
                                try
                                {
                                    var LineData = Encoding.UTF8.GetString(Encoding.UTF8.GetBytes(words[j]));
                                    if (LineData.Contains("GROSS PAY") ||
                                        LineData.Contains("DEDUCTION LIABILITY") ||
                                        LineData.Contains("VOID CHECKS") ||
                                        LineData == "MANUAL")
                                    {
                                        TempTable1.Add(new PDFTable { Name = LineData, Value2 = "0" });
                                        //TableList1.Add(new PDFTable { Name = LineData, Value2 = LineData });
                                        blnGPay = true;
                                    }
                                    if (blnGPay == true && LineData.Trim().Contains("TOTAL:"))
                                    {
                                        var TempData = TempTable1;
                                        var PreLine = Encoding.UTF8.GetString(Encoding.UTF8.GetBytes(words[j + 1]));
                                        if (TempTable1.Count() > 0)
                                        {
                                            var TempValues = TempData.FirstOrDefault();
                                            PreLine = PreLine.Trim().Replace("$", string.Empty).Replace(",", string.Empty);
                                            if (TempValues.Name != "VOID CHECKS" && TempValues.Name != "MANUAL")
                                            {
                                                TableList1.Add(
                                                    new PDFTable
                                                    {
                                                        Name = TempValues.Name,
                                                        Value = string.Empty,
                                                        Name2 = TempValues.Name,
                                                        Value2 = PreLine
                                                    });
                                            }
                                            TempTable1 = new List<PDFTable>();
                                        }
                                        blnGPay = false;
                                    }
                                    if (blnGPay == true &&
                                        (TempTable1[0].Name.Contains("DEDUCTION LIABILITY") ||
                                            TempTable1[0].Name.Contains("VOID CHECKS") ||
                                            TempTable1[0].Name == "MANUAL"))
                                    {
                                        try
                                        {
                                            var TempData = TempTable1;
                                            if (TempTable1.Count() > 0)
                                            {
                                                if (LineData.Contains("$"))
                                                {
                                                    string[] authorsList = LineData.Trim().Replace("$", string.Empty).Split(' ');
                                                    if (authorsList.Count() == 2)
                                                    {
                                                        var TempValues = TempData.FirstOrDefault();
                                                        TableList1.Add(
                                                            new PDFTable
                                                            {
                                                                Name = TempValues.Name,
                                                                Value = string.Empty,
                                                                Name2 = authorsList[0],
                                                                Value2 = authorsList[1]
                                                            });
                                                    }
                                                    else if (authorsList.Count() == 3)
                                                    {
                                                        var TempValues = TempData.FirstOrDefault();
                                                        TableList1.Add(
                                                            new PDFTable
                                                            {
                                                                Name = TempValues.Name,
                                                                Value = string.Empty,
                                                                Name2 = authorsList[0] + " " + authorsList[1],
                                                                Value2 = authorsList[2]
                                                            });
                                                    }
                                                    else if (authorsList.Count() == 1)
                                                    {
                                                        var PreLine = Encoding.UTF8
                                                            .GetString(Encoding.UTF8.GetBytes(words[j + 1]));
                                                        var TempValues = TempData.FirstOrDefault();
                                                        if (TempValues.Name == "VOID CHECKS" ||
                                                            TempValues.Name == "MANUAL")
                                                        {
                                                            TableList1.Add(
                                                                new PDFTable
                                                                {
                                                                    Name = TempValues.Name,
                                                                    Value = string.Empty,
                                                                    Name2 = TempValues.Name,
                                                                    Value2 = authorsList[0]
                                                                });
                                                        }
                                                        else
                                                        {
                                                            TableList1.Add(
                                                                new PDFTable
                                                                {
                                                                    Name = TempValues.Name,
                                                                    Value = string.Empty,
                                                                    Name2 = PreLine,
                                                                    Value2 = authorsList[0]
                                                                });
                                                        }
                                                    }
                                                    else if (authorsList.Count() >= 8)
                                                    {
                                                        string[] authorsList1 = LineData.Trim().Split(' ');
                                                        var TempValues = TempData.FirstOrDefault();
                                                        var res = authorsList1.Where(x => x.Contains("$"))
                                                            .FirstOrDefault();
                                                        if (TempValues.Name == "VOID CHECKS" ||
                                                            TempValues.Name == "MANUAL")
                                                        {
                                                            TableList1.Add(
                                                                new PDFTable
                                                                {
                                                                    Name = TempValues.Name,
                                                                    Value = string.Empty,
                                                                    Name2 = TempValues.Name,
                                                                    Value2 = res.Replace("$", string.Empty)
                                                                });
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            logger.Error("PDFReadController - Grid - " + DateTime.Now + " - " + ex.Message.ToString());
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    logger.Error("PDFReadController - Grid - " + DateTime.Now + " - " + ex.Message.ToString());
                                }
                            }
                        }

                        if (text.Contains("Tax Summary"))
                        {
                            TempTable2 = new List<PDFTable>();
                            words = text.Split('\n');
                            bool blnGPay = false;
                            for (int j = 0, len = words.Length; j < len; j++)
                            {
                                try
                                {
                                    var LineData = Encoding.UTF8.GetString(Encoding.UTF8.GetBytes(words[j]));
                                    if ((LineData.Contains("Employee") && !LineData.Contains("Total Employee")) ||
                                        (LineData.Contains("Employer") && !LineData.Contains("Total Employer")))
                                    {
                                        if (!LineData.Contains("Employer Responsible:"))
                                        {
                                            TempTable1.Add(new PDFTable { Name = LineData, Value2 = "0" });
                                            //TableList1.Add(new PDFTable { Name = LineData, Value2 = LineData });
                                            blnGPay = true;
                                        }
                                    }
                                    if (blnGPay == true &&
                                        (LineData.Trim().Contains("Total Employee") ||
                                            LineData.Trim().Contains("Total Employer")))
                                    {
                                        var TempData = TempTable1;
                                        if (TempTable1.Count() > 0)
                                        {
                                            var TempValues = TempData.FirstOrDefault();
                                            if (LineData.Contains("$"))
                                            {
                                                string[] authorsList = LineData.Trim().Split(' ');
                                                if (authorsList.Count() > 2)
                                                {
                                                    string aa = GetFinalValue(authorsList);
                                                    TableList1.Add(
                                                        new PDFTable
                                                        {
                                                            Name = TempValues.Name,
                                                            Value = string.Empty,
                                                            Name2 = authorsList[0] + " " + authorsList[1],
                                                            Value2 = aa
                                                        });
                                                }
                                            }
                                            TempTable1 = new List<PDFTable>();
                                        }
                                        blnGPay = false;
                                    }
                                    if (blnGPay == true &&
                                        (TempTable1[0].Name.Contains("Employee") ||
                                            TempTable1[0].Name.Contains("Employer")))
                                    {
                                        string[] authorsList = LineData.Trim().Split(' ');
                                        int StringCount = authorsList.Count();

                                        if (LineData.Contains("$"))
                                        {
                                            if (StringCount == 2)
                                            {
                                                var DigitLine = LineData.Replace("$", string.Empty)
                                                    .Replace(" ", string.Empty)
                                                    .Replace(".", string.Empty)
                                                    .Replace(",", string.Empty)
                                                    .Replace("(", string.Empty)
                                                    .Replace(")", string.Empty);
                                                if (DigitLine.All(s => char.IsDigit(s)))
                                                {
                                                    TempTable2.Add(
                                                        new PDFTable
                                                        {
                                                            Name = TempTable1[0].Name,
                                                            Value = string.Empty,
                                                            Name2 = string.Empty,
                                                            Value2 = authorsList[0],
                                                            Name3 = string.Empty,
                                                            Value3 = authorsList[1]
                                                        });
                                                }
                                            }
                                            else if (StringCount > 2)
                                            {
                                                string aa = GetFinalValue(authorsList);
                                                TableList1.Add(
                                                    new PDFTable
                                                    {
                                                        Name = TempTable1[0].Name,
                                                        Value = string.Empty,
                                                        Name2 = authorsList[0],
                                                        Value2 = aa
                                                    });
                                            }
                                        }
                                        else
                                        {
                                            if (StringCount > 1)
                                            {
                                                var TempData = TempTable2;
                                                if (TempTable2.Count() > 0)
                                                {
                                                    var TempValues = TempData.FirstOrDefault();
                                                    TableList1.Add(
                                                        new PDFTable
                                                        {
                                                            Name = TempTable1[0].Name,
                                                            Value = string.Empty,
                                                            Name2 = authorsList[0],
                                                            Value2 = TempTable2[0].Value2
                                                        });
                                                    TempTable2 = new List<PDFTable>();
                                                }
                                            }
                                        }
                                    }
                                    if (LineData.Contains("Total due Tax Service:"))
                                    {
                                        var PreLine = Encoding.UTF8.GetString(Encoding.UTF8.GetBytes(words[j - 1]));
                                        TableList1.Add(
                                            new PDFTable
                                            {
                                                Name = "Total due Tax Service:",
                                                Value = PreLine.Trim().Replace(",", string.Empty),
                                                Name2 = "Total due Tax Service:",
                                                Value2 = PreLine.Trim().Replace(",", string.Empty)
                                            });
                                    }
                                }
                                catch (Exception ex)
                                {
                                    logger.Error("PDFReadController - Grid - " + DateTime.Now + " - " + ex.Message.ToString());
                                }
                            }
                        }
                    }

                    List<PDFTable> PDFPrintList = new List<PDFTable>();
                    reader.Close();
                    reader.Dispose();
                    int flg = 0;
                    int DepartmentId = 0;
                    PayrollMaster obj = new PayrollMaster();
                    int StoreId = Convert.ToInt32(Session["storeid"].ToString());
                    obj.StoreId = StoreId;
                    if (StartDate != string.Empty)
                    {
                        obj.StartDate = Convert.ToDateTime(StartDate);
                    }
                    else
                    {
                        obj.StartDate = DateTime.Today;
                    }
                    if (EndDate != string.Empty)
                    {
                        obj.EndDate = Convert.ToDateTime(EndDate);
                    }
                    else
                    {
                        obj.EndDate = DateTime.Today;
                    }
                    if (EndCheckDate != string.Empty)
                    {
                        obj.EndCheckDate = Convert.ToDateTime(EndCheckDate);
                    }
                    else
                    {
                        if (CheckDate != string.Empty)
                            obj.EndCheckDate = Convert.ToDateTime(CheckDate);
                        else
                            obj.EndCheckDate = DateTime.Today;
                    }

                    var FileExist = _PDFReadRepository.GetPayrollMasters(obj);
                    if (FileExist.Count > 0)
                    {
                        value = 1;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("PDFReadController - Grid - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return value;
        }

        //Syncfusion grid Payroll files 
        //public ActionResult PayrollFilsSyncIndex()
        //{
        //    return View();
        //}
        public ActionResult UrlDatasource(DataManagerRequest dm)
        {
            List<PayrollFileList> HrDeVm = new List<PayrollFileList>();
            IEnumerable<PayrollFileList> DataSource = new List<PayrollFileList>();
            int Count = 0;
            try
            {
                logger.Info("PDFReadController - UrlDatasource - " + DateTime.Now);

                int StoreID = Convert.ToInt32(Session["storeid"].ToString());
                HrDeVm = GetData(1, "", StoreID).OfType<PayrollFileList>().ToList();
                DataSource = HrDeVm.OfType<PayrollFileList>().ToList().OrderByDescending(n => AdminSiteConfiguration.stringToDate(n.EndCheckDate, DateFormats.MMddyyyy, DateFormats.MMddyyyy));

                DataOperations operation = new DataOperations();
                if (dm.Search != null && dm.Search.Count > 0)
                {
                    string search = dm.Search[0].Key.ToString().Trim();
                    DataSource = DataSource.Where(x => x.UploadDate.Contains(search) || x.StartDate.Contains(search) || x.EndDate.Contains(search) || x.EndCheckDate.Contains(search)).ToList();
                    //DataSource = operation.PerformSearching(DataSource, dm.Search);  //Search
                }
                if (dm.Sorted != null && dm.Sorted.Count > 0) //Sorting
                {
                    DataSource = operation.PerformSorting(DataSource, dm.Sorted);
                }
                if (dm.Where != null && dm.Where.Count > 0) //Filtering
                {
                    DataSource = operation.PerformFiltering(DataSource, dm.Where, dm.Where[0].Operator);
                }
                Count = DataSource.Cast<PayrollFileList>().Count();
                if (dm.Skip != 0)
                {
                    DataSource = operation.PerformSkip(DataSource, dm.Skip);   //Paging
                }
                if (dm.Take != 0)
                {
                    DataSource = operation.PerformTake(DataSource, dm.Take);
                }


            }
            catch (Exception ex)
            {
                logger.Error("PDFReadController - UrlDatasource - " + DateTime.Now + " - " + ex.Message.ToString());
            }

            return dm.RequiresCounts ? Json(new { result = DataSource, count = Count }) : Json(DataSource);
        }

        //Himanshu 27-11-2024
        public ActionResult GetPayrollDetailView(int PayrollReportId)
        {
            List<PayrollDetails> details = new List<PayrollDetails>();
            try
            {
                details = _PDFReadRepository.GetPayrollDepartmentsList(PayrollReportId);
                if (details.Any() && details.FirstOrDefault().DepartmentName != null)
                {
                    ViewBag.HasDepartments = true;
                    ViewBag.DepartmentNameList = details.Select(m => m.DepartmentName).Distinct().ToList();
                }
                else
                {
                    ViewBag.HasDepartments = false;
                }
            }
            catch (Exception ex)
            {
                logger.Error("PDFReadController - GetPayrollDetailView - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return View(details);
        }


        //Himanshu's Code Start From Here 17-03-2025
        public ActionResult PayrollManualEntrie()
        {
            ViewBag.Title = "Add Manual Payroll - Synthesis";
            PayrollManualModel obj = new PayrollManualModel();
            try
            {
                int StoreId = 0;
                StoreId = Convert.ToInt32(Session["storeid"]);
                ViewBag.PayrollDepartment = _PDFReadRepository.GetPayrollDepartmentsStorewise(StoreId).Select(s => new DropdownViewModel { Text = s.DepartmentName, Value = s.PayrollDepartmentId }).OrderBy(o => o.Text).ToList();
                ViewBag.PayrollDepartment.Insert(0, new DropdownViewModel { Text = "-Select Department-", Value = 0 });
            }
            catch (Exception ex)
            {
                logger.Error("PDFReadController - PayrollManualEntrie - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return View(obj);
        }

        [HttpPost]
        public ActionResult PayrollManualEntrie(PayrollManualModel obj)
        {
            try
            {
                int StoreId = 0;
                StoreId = Convert.ToInt32(Session["storeid"]);

                //insert into PayrollReport
                PayrollReport payreport = new PayrollReport();
                payreport.StoreId = StoreId;
                payreport.FileName = obj.FileName + ".pdf";
                payreport.UploadDate = DateTime.Now;
                payreport.IsRead = true;
                payreport.IsSync = 0;
                payreport.FIleNo = obj.FileType;
                int PayrollReportId = _PDFReadRepository.AddmanualPayrollReports(payreport);

                if (PayrollReportId > 0)
                {
                    //insert into PayrollMaster
                    PayrollMaster payrollMaster = new PayrollMaster();
                    payrollMaster.StoreId = StoreId;
                    payrollMaster.StartDate = obj.StartDate;
                    payrollMaster.EndDate = obj.EndDate;
                    payrollMaster.PayrollReportId = PayrollReportId;
                    payrollMaster.EndCheckDate = obj.EndCheckDate;
                    int PayrollId = _PDFReadRepository.AddPayrollMasters(payrollMaster);
                    if (obj.FileType == 2)
                    {
                        var TransactionNo = RandomString(8);
                        _PDFReadRepository.AddTranscationNo(obj.StartDate, obj.EndDate, obj.EndCheckDate, StoreId, TransactionNo);
                    }

                    if (PayrollId > 0)
                    {
                        var cashanalysisfields = new Dictionary<string, decimal?>
                        {
                            { "CHILD SUPPORT", obj.CHILDSUPPORT },
                            { "STDIS", obj.STDIS },
                            { "CSDispNY", obj.CSDispNY },
                            { "EENYPFL", obj.EENYPFL },
                            { "GROSS PAY", obj.GROSSPAY },
                            { "AFLAC", obj.AFLAC },
                            { "401K2", obj.K2401 },
                            { "LIFEINS", obj.LIFEINS },
                            { "SDINY", obj.SDINY },
                            { "Federal Unemployment", obj.FederalUnemployment },
                            { "Medicare - Employer", obj.MedicareEmployer },
                            { "New York Unemployment", obj.NewYorkUnemployment },
                            { "PAYROLL TAXES", obj.PAYROLLTAXES },
                            { "Social Security - Employer", obj.SocialSecurityEmployer },
                            { "New York Metro Commut Zone 1", obj.NewYorkMetroCommutZone },
                            { "Employees Payments", obj.Employeepayments },
                            { "NY Re-empl Svc", obj.NYReemplSvc },
                            { "NJ ER Work Dev", obj.NJERWorkDev },
                            { "NY MCTMT TD MSC", obj.NYMCTMTTDMSC },
                            { "Payroll Fee", obj.PayrollFee },
                            { "New Jersey Unemployment", obj.NewJerseyUnemployment },
                            { "NJ Disability", obj.NJDisability },
                            { "Dental", obj.Dental },
                            { "Life Insurance", obj.LifeInsurance },
                            { "Employer Social Security", obj.EmployerSocialSecurity },
                            { "Gross Wages", obj.GrossWages },
                            { "Employer Medicare", obj.EmployerMedicare }
                        };

                        foreach (var field in cashanalysisfields)
                        {
                            if (field.Value.HasValue)
                            {
                                //insert into PayrollCashAnalysisDetail
                                var payrollCashAnalysis = _PDFReadRepository.Getpayrollcashanalysis(StoreId, field.Key).FirstOrDefault();
                                int payrollCashAnalysisId = payrollCashAnalysis != null ? payrollCashAnalysis.PayrollCashAnalysisId : 0;

                                PayrollCashAnalysisDetail payrollCashAnalysisDetail = new PayrollCashAnalysisDetail
                                {
                                    PayrollId = PayrollId,
                                    PayrollCashAnalysisId = payrollCashAnalysisId,
                                    Amount = field.Value.Value,
                                    ETaxCalc = 0
                                };

                                _PDFReadRepository.AddPayrollCashAnalysisDetail(payrollCashAnalysisDetail);
                            }
                        }

                        foreach (var department in obj.PayrollManualDetailsModel)
                        {
                            var payrollFields = new Dictionary<string, decimal?>
                        {
                            { "HOURLY", department.HOURLY },
                            { "REGULAR", department.REGULAR },
                            { "RETRO", department.RETRO },
                            { "SALARY", department.SALARY },
                            { "SPREAD OF HOURS", department.SPREADOFHOURS },
                            { "TRAINING", department.TRAINING },
                            { "HOLIDAY WORKED", department.HOLIDAYWORKED },
                            { "OVERTIME", department.OVERTIME },
                            { "BONUS", department.BONUS },
                            { "CREDITCARDTIPS", department.CREDITCARDTIPS },
                            { "HOLIDAY", department.HOLIDAY },
                            { "SICK", department.SICK },
                            { "VACATION", department.VACATION }
                        };

                            foreach (var field in payrollFields)
                            {
                                if (field.Value.HasValue)
                                {
                                    //insert into PayrollDepartmentDetails
                                    PayrollDepartmentDetails payrollDetail = new PayrollDepartmentDetails
                                    {
                                        PayrollId = PayrollId,
                                        PayrollDepartmentId = department.PayrollDepartmentId,
                                        Name = field.Key,
                                        Value = field.Value.Value
                                    };

                                    _PDFReadRepository.AddPayrollDepartmentDetails(payrollDetail);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("PDFReadController - PayrollManualEntrie - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return RedirectToAction("PayrollManualEntrie");
        }
    }
}