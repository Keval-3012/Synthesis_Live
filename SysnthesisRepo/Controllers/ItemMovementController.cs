using Aspose.Cells;
using Aspose.Pdf;
using Aspose.Pdf.Operators;
using EntityModels.Models;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using NLog;
using Repository;
using Repository.IRepository;
using Syncfusion.EJ2.Base;
using Syncfusion.EJ2.Spreadsheet;
using SynthesisQBOnline;
using SynthesisViewModal;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace SysnthesisRepo.Controllers
{
    public class ItemMovementController : Controller
    {
        private readonly IItemMovementRepository _itemMovementRepository;
        private readonly ICommonRepository _commonRepository;
        private readonly IUserActivityLogRepository _activityLogRepository;
        private readonly ISynthesisApiRepository _synthesisApiRepository;
        protected static string StatusMessageString = "";
        protected static string DeleteMessage = "";
        Logger logger = LogManager.GetCurrentClassLogger();
        public string Message { get; set; }
        public ItemMovementController()
        {
            this._itemMovementRepository = new ItemMovementRepository(new DBContext());
            this._commonRepository = new CommonRepository(new DBContext());
            this._commonRepository = new CommonRepository(new DBContext());
            this._synthesisApiRepository = new SynthesisApiRepository();
        }
        // GET: ItemMovement
        /// <summary>
        /// This method is return Index view.
        /// </summary>
        /// <param name="StoreId"></param>
        /// <returns></returns>
        public ActionResult Index(int StoreId = 0)
        {
            ViewBag.Title = "Item Movement - Synthesis";
            _commonRepository.LogEntries();     //Harsh's code
            string storeid = "0";
            try
            {
                if (StoreId == 0)
                {
                    storeid = Convert.ToString(Session["storeid"]);
                    ViewBag.storeid = storeid;

                    int StoreID = Convert.ToInt32(storeid);
                }
            }
            catch (Exception ex)
            {
                logger.Error("ItemMovementController - Index - " + DateTime.Now + " - " + ex.Message.ToString());
            }

            return View();
        }
        /// <summary>
        /// This method is return get  List of item.
        /// </summary>
        /// <returns></returns>
        public ActionResult GetList()
        {
            return PartialView("_ItemMovement");
        }
        /// <summary>
        /// This method is used for Url Datasource with Child.
        /// </summary>
        /// <param name="dm"></param>
        /// <returns></returns>
        public ActionResult UrlDatasourceChild(DataManagerRequest dm)
        {
            List<ItemMovementBySupplierSelect> itemMovement = new List<ItemMovementBySupplierSelect>();
            if (dm.Where != null && dm.Where.Count > 0) //Filtering
                //This Db class is  used to Select item Movement. 
                itemMovement = _itemMovementRepository.SelectItemMovement(dm.Where[0].value).ToList();
            IEnumerable DataSource = itemMovement;
            DataOperations operation = new DataOperations();
            int count = 0;
            try
            {
                if (dm.Search != null && dm.Search.Count > 0)
                {
                    DataSource = operation.PerformSearching(DataSource, dm.Search);  //Search
                }
                if (dm.Sorted != null && dm.Sorted.Count > 0) //Sorting
                {
                    DataSource = operation.PerformSorting(DataSource, dm.Sorted);
                }
                count = DataSource.Cast<ItemMovementBySupplierSelect>().Count();
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
                logger.Error("ItemMovementController - UrlDatasourceChild - " + DateTime.Now + " - " + ex.Message.ToString());
            }


            return dm.RequiresCounts ? Json(new { result = DataSource, count = count }) : Json(DataSource);
        }
        /// <summary>
        /// This method is used for Url Datasource with Item movement date.
        /// </summary>
        /// <param name="dm"></param>
        /// <returns></returns>
        public ActionResult UrlDatasource(DataManagerRequest dm)
        {
            int StoreIds = 0;
            if (Session["storeid"] != null)
            {
                StoreIds = Convert.ToInt32(Convert.ToString(Session["storeid"]));
            }
            ViewBag.StoreId = StoreIds;
            List<ItemMovementdatehistorySelect> itemMovement = new List<ItemMovementdatehistorySelect>();
            //This Db class is  used to Get all item Movement with History selects. 
            itemMovement = _itemMovementRepository.GetItemMovementdatehistorySelects(StoreIds);
            IEnumerable DataSource = itemMovement;
            DataOperations operation = new DataOperations();
            if (dm.Search != null && dm.Search.Count > 0)
            {
                DataSource = operation.PerformSearching(DataSource, dm.Search);  //Search
            }
            if (dm.Sorted != null && dm.Sorted.Count > 0) //Sorting
            {
                DataSource = operation.PerformSorting(DataSource, dm.Sorted);
            }
            if (dm.Where != null && dm.Where.Count > 0) //Filtering
            {
                DataSource = operation.PerformFiltering(DataSource, dm.Where, dm.Where[0].Operator);
            }
            int count = DataSource.Cast<ItemMovementdatehistorySelect>().Count();
            if (dm.Skip != 0)
            {
                DataSource = operation.PerformSkip(DataSource, dm.Skip);   //Paging
            }
            if (dm.Take != 0)
            {
                DataSource = operation.PerformTake(DataSource, dm.Take);
            }

            return dm.RequiresCounts ? Json(new { result = DataSource, count = count }) : Json(DataSource);
        }
        bool IsItemReading = false;
        string SupplierName = "";
        string DepartmentName = "";
        string Data = "";
        string[] Values = null;
        int Indexs = 0;
        DataRow dr = null;
        string StartDate = "";
        string EndDate = "";
        string StoreName = "";
        /// <summary>
        /// This method is used for Import Supplierd Pdf.
        /// </summary>
        /// <param name="postedFile"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ImportSuppliersPdf(HttpPostedFileBase postedFile)
        {
            try
            {
                string storeid = "";
                int StoreId = 0;
                if (!string.IsNullOrEmpty(Convert.ToString(Session["storeid"])))
                {
                    storeid = Convert.ToString(Session["storeid"]);
                    ViewBag.storeid = storeid;
                    StoreId = Convert.ToInt32(storeid);
                }
                string filename = System.IO.Path.GetFileNameWithoutExtension(postedFile.FileName);
                string extension = System.IO.Path.GetExtension(postedFile.FileName);
                filename = filename + extension;
                var ExtractFileName = filename;

                filename = System.IO.Path.Combine(Server.MapPath("~/UserFiles/ItemMovementSupplier"), filename);
                postedFile.SaveAs(filename);
                ReadPDF(filename);
                //ExtractPDfData(StoreId);
                // DB Save code
                ItemMovementdatehistory itemMovementdatehistory = new ItemMovementdatehistory();
                DateTime sDate = DateTime.ParseExact(StartDate, "MM/dd/yyyy hh:mm tt", null);
                DateTime eDate = DateTime.ParseExact(EndDate, "MM/dd/yyyy hh:mm tt", null);

                itemMovementdatehistory.Startdate = sDate;
                itemMovementdatehistory.Enddate = eDate;
                itemMovementdatehistory.StoreID = StoreId;
                itemMovementdatehistory.FileName = ExtractFileName;
                //This Db class is  used to Get all Move item Movement. 
                _itemMovementRepository.GetMoveItemMovement(itemMovementdatehistory);
                int itemMovementdatehistoryID = itemMovementdatehistory.ItemMovementdatehistoryID;
                foreach (DataRow row in dtItems.Rows)
                {
                    row[10] = itemMovementdatehistoryID;
                }
                dtItems.Columns.Remove("PDevider");
                BulkInsertToDataBase();

                string path = Server.MapPath("~/Userfiles/ItemPdfToExcelFile/");
                string FileName = "PdfToExcel.xlsx";
                if (System.IO.File.Exists(path + System.IO.Path.GetFileName(FileName)))
                {
                    System.IO.File.Delete(path + System.IO.Path.GetFileName(FileName));
                }
            }
            catch (Exception ex)
            {
                string path = Server.MapPath("~/Userfiles/ItemPdfToExcelFile/");
                string FileName = "PdfToExcel.xlsx";
                if (System.IO.File.Exists(path + System.IO.Path.GetFileName(FileName)))
                {
                    System.IO.File.Delete(path + System.IO.Path.GetFileName(FileName));
                }
                logger.Error("ItemMovementController - ImportSuppliersPdf - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return RedirectToAction("Index", "ItemMovement");
        }
        /// <summary>
        /// This Class is used to Bulk insert to database.
        /// </summary>
        private void BulkInsertToDataBase()
        {
            try
            {
                string con = ConfigurationManager.ConnectionStrings["DBConn"].ConnectionString;
                //creating object of SqlBulkCopy  
                SqlBulkCopy objbulk = new SqlBulkCopy(con, SqlBulkCopyOptions.KeepNulls);
                objbulk.BatchSize = 500;
                //assigning Destination table name  
                objbulk.DestinationTableName = "ItemMovementBySupplier";
                //Mapping Table column  


                objbulk.ColumnMappings.Add("SupplierName", "SupplierName");
                objbulk.ColumnMappings.Add("Department", "Department");
                objbulk.ColumnMappings.Add("ItemCode", "ItemCode");
                objbulk.ColumnMappings.Add("ItemName", "ItemName");
                objbulk.ColumnMappings.Add("QtySold", "QtySold");
                objbulk.ColumnMappings.Add("QtyOnHand", "QtyOnHand");
                objbulk.ColumnMappings.Add("LastCOst", "LastCOst");
                objbulk.ColumnMappings.Add("BasePrice", "BasePrice");
                objbulk.ColumnMappings.Add("ProjMargin", "ProjMargin");
                objbulk.ColumnMappings.Add("ItemMovementHistoryID", "ItemMovementHistoryID");
                //inserting bulk Records into DataBase   
                objbulk.WriteToServer(dtItems);
            }
            catch (Exception ex)
            {
                logger.Error("ItemMovementController - BulkInsertToDataBase - " + DateTime.Now + " - " + ex.Message.ToString());
            }
        }
        public ArrayList arLines = new ArrayList();
        public ArrayList arItemLines = new ArrayList();
        public DataTable dtItems = new DataTable();   
        /// <summary>
        /// This class is used to read pdf  for ImportSuppliersPdf.
        /// </summary>
        /// <param name="FileName"></param>
        public void ReadPDF(string FileName)
        {
            try
            {
                if (FileName != string.Empty)
                {
                    Aspose.Pdf.Document pdfDocument = new Aspose.Pdf.Document(FileName);
                    ExcelSaveOptions options = new ExcelSaveOptions();
                    options.Format = ExcelSaveOptions.ExcelFormat.XLSX;
                    string NewFilePath = Server.MapPath("~/userfiles/ItemPdfToExcelFile/PdfToExcel.xlsx");
                    pdfDocument.Save(NewFilePath, options);
                    ReadExcelAndBulkInsert(NewFilePath);
                }
            }
            catch (Exception ex)
            {
                logger.Error("ItemMovementController - ReadPDF - " + DateTime.Now + " - " + ex.Message.ToString());
            }
        }
        private void ReadExcelAndBulkInsert(string excelFilePath)
        {
            try
            {
                InitializeTable();
                Aspose.Cells.Workbook wb = new Aspose.Cells.Workbook(excelFilePath);
                WorksheetCollection collection = wb.Worksheets;
                DataTable dataTable = new DataTable();
                int rows = 0;
                int cols = 0;
                for (int worksheetIndex = 0; worksheetIndex < collection.Count; worksheetIndex++)
                {
                    Aspose.Cells.Worksheet worksheet = collection[worksheetIndex];
                    rows = worksheet.Cells.MaxDataRow;
                    cols = worksheet.Cells.MaxDataColumn + 1;
                    dataTable = worksheet.Cells.ExportDataTable(0, 0, rows, cols, false);

                    string SupplierName = "";
                    string DepartmentName = "";
                    string ItemID = "";
                    string ReceiptAlias = "";
                    decimal QtySold = 0.00m;
                    decimal LastCost = 0.00m;
                    decimal QtyOnHand = 0.00m;
                    decimal BasePrice = 0.00m;
                    string ProjMargin = "";
                    int isvalidloop = 0;
                    int containsupplier = 0;
                    int supplierCol = -1;
                    int itemIDCol = -1;
                    int aliasNameCol = -1;
                    int brandCol = -1;
                    int qtySoldCol = -1;
                    int lastCostCol = -1;
                    int priceDeviderCol = -1;
                    int qtyOnHandCol = -1;
                    int basePriceCol = -1;
                    int projMarginCol = -1;

                    for (int rowIndex = 0; rowIndex < rows; rowIndex++)
                    {
                        for (int colIndex = 0; colIndex < cols; colIndex++)
                        {
                            string cellValue = worksheet.Cells[rowIndex, colIndex].Value?.ToString().Trim();

                            if (string.IsNullOrEmpty(cellValue)) continue;

                            if (cellValue.Contains("Supplier ID"))
                                supplierCol = colIndex;
                            else if (cellValue.Contains("Item ID"))
                                itemIDCol = colIndex;
                            else if (cellValue.Contains("Receipt Alias"))
                                aliasNameCol = colIndex;
                            else if (cellValue.Contains("Brand"))
                                brandCol = colIndex;
                            else if (cellValue.Contains("Sold"))
                                qtySoldCol = colIndex;
                            else if (cellValue.Contains("On Hand"))
                                qtyOnHandCol = colIndex;
                            else if (cellValue.Contains("Last Cost"))
                                lastCostCol = colIndex;
                            else if (cellValue.Contains("Divider"))
                                priceDeviderCol = colIndex;
                            else if (cellValue.Contains("Base Price"))
                                basePriceCol = colIndex;
                            else if (cellValue.Contains("Margin"))
                                projMarginCol = colIndex;
                        }

                        // Break once the columns are identified
                        if (itemIDCol != -1 && aliasNameCol != -1 && qtySoldCol != -1 && lastCostCol != -1 && qtyOnHandCol != -1 && basePriceCol != -1 && projMarginCol != -1)
                        {
                            break;
                        }
                    }

                    foreach (DataRow r in dataTable.Rows)
                    {
                        if (r.Field<string>("Column1")?.Contains("Reporting Range:") == true)
                        {
                            string notfiltereddate = r.Field<string>("Column2").Trim();
                            if (notfiltereddate.Contains("Date-Time Range between"))
                            {
                                string arraydatedata = notfiltereddate.Replace("Date-Time Range between", "").Replace("[", "");
                                Values = arraydatedata.Split(']')[0].Split(',');
                                StartDate = Values[0].ToString().Trim();
                                EndDate = Values[1].ToString().Trim();
                            }
                            else
                            {
                                string arraydatedata = notfiltereddate.Replace("Date-Time Range: Yesterday{between ", "").Replace("}", "");
                                Values = arraydatedata.Split(',');
                                StartDate = Values[0].ToString().Trim();
                                EndDate = Values[1].ToString().Trim();
                            }
                        }
                        else if (r.Field<string>("Column1")?.Contains("Supplier:") == true)
                        {
                            var Supplier = r.Field<string>("Column1").Split(':')[1];
                            if (String.IsNullOrEmpty(Supplier))
                            {
                                SupplierName = r.Field<string>("Column2").Trim();
                                if (SupplierName == "(none defined)")
                                {
                                    SupplierName = "";
                                }
                            }
                            else
                            {
                                SupplierName = Supplier.Trim();
                            }
                            containsupplier = 1;
                        }
                        else if (containsupplier == 1)
                        {
                            if (String.IsNullOrEmpty(r.Field<string>("Column2")))
                            {

                                if (!String.IsNullOrEmpty(r.Field<string>("Column1")))
                                {
                                    DepartmentName = r.Field<string>("Column1").Trim();
                                    isvalidloop = 0;
                                }
                            }
                            else if (r.Field<string>("Column2")?.Contains("Item ID") == true)
                            {
                                isvalidloop = 1;
                            }
                            else if (isvalidloop == 1)
                            {
                                if (cols > 11)
                                {
                                    try
                                    {
                                        ItemID = (itemIDCol + 1) == aliasNameCol ? r.Field<string>(itemIDCol)?.Replace("(", "").Replace(")", "").Trim() : (r.Field<string>(itemIDCol)?.Replace("(", "").Replace(")", "").Trim() + " " + r.Field<string>(itemIDCol + 1)?.Replace("(", "").Replace(")", "").Trim()).Trim();
                                        ReceiptAlias = (aliasNameCol + 1) == brandCol ? r.Field<string>(aliasNameCol)?.Replace("(", "").Replace(")", "").Trim() : (r.Field<string>(aliasNameCol)?.Replace("(", "").Replace(")", "").Trim() + " " + r.Field<string>(aliasNameCol + 1)?.Replace("(", "").Replace(")", "").Trim()).Trim();
                                        string qtySoldString = (r.Field<string>(qtySoldCol) == null ? "0" : r.Field<string>(qtySoldCol).Trim());
                                        if (qtySoldString.StartsWith("(") || qtySoldString.EndsWith(")"))
                                        {
                                            qtySoldString = qtySoldString.Replace("(", "").Replace(")", "").Trim();
                                            QtySold = -decimal.Parse(qtySoldString);
                                        }
                                        else
                                        {
                                            QtySold = decimal.Parse(qtySoldString);
                                        }
                                        string LastCostString = (r.Field<string>(lastCostCol) == null ? "0" : r.Field<string>(lastCostCol).Trim());
                                        if (LastCostString.StartsWith("(") || LastCostString.EndsWith(")"))
                                        {
                                            LastCostString = LastCostString.Replace("(", "").Replace(")", "").Trim();
                                            LastCost = -decimal.Parse(LastCostString);
                                        }
                                        else
                                        {
                                            LastCost = decimal.Parse(LastCostString);
                                        }
                                        string qtyOnHandString = (r.Field<string>(qtyOnHandCol) == null ? "0" : r.Field<string>(qtyOnHandCol).Trim());
                                        qtyOnHandString = qtyOnHandString.Replace(",", "").Replace("*", "").Trim();
                                        if (qtyOnHandString.StartsWith("(") || qtyOnHandString.EndsWith(")"))
                                        {
                                            qtyOnHandString = qtyOnHandString.Replace("(", "").Replace(")", "").Trim();
                                            QtyOnHand = -decimal.Parse(qtyOnHandString);
                                        }
                                        else
                                        {
                                            QtyOnHand = decimal.Parse(qtyOnHandString);
                                        }
                                        string BasePriceString = (r.Field<string>(basePriceCol) == null ? "0" : r.Field<string>(basePriceCol).Trim());
                                        if (BasePriceString.StartsWith("(") || BasePriceString.EndsWith(")"))
                                        {
                                            BasePriceString = BasePriceString.Replace("(", "").Replace(")", "").Trim();
                                            BasePrice = -decimal.Parse(BasePriceString);
                                        }
                                        else
                                        {
                                            BasePrice = decimal.Parse(BasePriceString);
                                        }
                                        string ProjMarginstring = (r.Field<string>(projMarginCol) == null ? "0" : r.Field<string>(projMarginCol).Trim());
                                        if (ProjMarginstring.StartsWith("(") || ProjMarginstring.EndsWith(")"))
                                        {
                                            ProjMarginstring = ProjMarginstring.Replace("(", "").Replace(")", "").Trim();
                                            ProjMargin = "-" + ProjMarginstring;
                                        }
                                        else
                                        {
                                            ProjMargin = ProjMarginstring;
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        logger.Error("ItemMovementController - ReadExcelAndBulkInsert - " + DateTime.Now + " - " + ex.Message.ToString());
                                    }
                                }
                                else
                                {
                                    try
                                    {
                                        ItemID = r.Field<string>("Column2")?.Replace("(", "").Replace(")", "").Trim();
                                        ReceiptAlias = r.Field<string>("Column3")?.Replace("(", "").Replace(")", "").Trim();
                                        string qtySoldString = (r.Field<string>("Column5") == null ? "0" : r.Field<string>("Column5")?.Trim());
                                        if (qtySoldString.StartsWith("(") || qtySoldString.EndsWith(")"))
                                        {
                                            qtySoldString = qtySoldString.Replace("(", "").Replace(")", "").Trim();
                                            QtySold = -decimal.Parse(qtySoldString);
                                        }
                                        else
                                        {
                                            QtySold = decimal.Parse(qtySoldString);
                                        }
                                        string LastCostString = (r.Field<string>("Column7") == null ? "0" : r.Field<string>("Column7")?.Trim());
                                        if (LastCostString.StartsWith("(") || LastCostString.EndsWith(")"))
                                        {
                                            LastCostString = LastCostString.Replace("(", "").Replace(")", "").Trim();
                                            LastCost = -decimal.Parse(LastCostString);
                                        }
                                        else
                                        {
                                            LastCost = decimal.Parse(LastCostString);
                                        }
                                        string qtyOnHandString = (r.Field<string>("Column6") == null ? "0" : r.Field<string>("Column6")?.Trim());
                                        qtyOnHandString = qtyOnHandString.Replace(",", "").Replace("*", "").Trim();
                                        if (qtyOnHandString.StartsWith("(") || qtyOnHandString.EndsWith(")"))
                                        {
                                            qtyOnHandString = qtyOnHandString.Replace("(", "").Replace(")", "").Trim();
                                            QtyOnHand = -decimal.Parse(qtyOnHandString);
                                        }
                                        else
                                        {
                                            QtyOnHand = decimal.Parse(qtyOnHandString);
                                        }
                                        string BasePriceString = (r.Field<string>("Column10") == null ? "0" : r.Field<string>("Column10")?.Trim());
                                        if (BasePriceString.StartsWith("(") || BasePriceString.EndsWith(")"))
                                        {
                                            BasePriceString = BasePriceString.Replace("(", "").Replace(")", "").Trim();
                                            BasePrice = -decimal.Parse(BasePriceString);
                                        }
                                        else
                                        {
                                            BasePrice = decimal.Parse(BasePriceString);
                                        }
                                        string ProjMarginstring = (r.Field<string>("Column11") == null ? "0" : r.Field<string>("Column11")?.Trim());
                                        if (ProjMarginstring.StartsWith("(") || ProjMarginstring.EndsWith(")"))
                                        {
                                            ProjMarginstring = ProjMarginstring.Replace("(", "").Replace(")", "").Trim();
                                            ProjMargin = "-" + ProjMarginstring;
                                        }
                                        else
                                        {
                                            ProjMargin = ProjMarginstring;
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        logger.Error("ItemMovementController - ReadExcelAndBulkInsert - " + DateTime.Now + " - " + ex.Message.ToString());
                                    }
                                }
                                dr = dtItems.NewRow();
                                dr[0] = SupplierName;
                                dr[1] = DepartmentName;
                                dr[2] = ItemID;
                                dr[3] = ReceiptAlias;
                                dr[4] = QtySold;
                                dr[5] = QtyOnHand;
                                dr[6] = LastCost;
                                dr[8] = BasePrice;
                                dr[9] = ProjMargin;
                                dtItems.Rows.Add(dr);

                            }
                        }
                    }
                    //BulkInsertToDataBase(dt);
                }
            }
            catch (Exception ex)
            {
                logger.Error("ItemMovementController - ReadExcelAndBulkInsert - " + DateTime.Now + " - " + ex.Message.ToString());
            }
        }
        /// <summary>
        /// This class is used to Initialize table for ExtractPdf.
        /// </summary>
        private void InitializeTable()
        {
            dtItems = new DataTable();
            dtItems.Columns.Add(new DataColumn("SupplierName", typeof(string)));
            dtItems.Columns.Add(new DataColumn("Department", typeof(string)));
            dtItems.Columns.Add(new DataColumn("ItemCode", typeof(string)));
            dtItems.Columns.Add(new DataColumn("ItemName", typeof(string)));
            dtItems.Columns.Add(new DataColumn("QtySold", typeof(decimal)));
            dtItems.Columns.Add(new DataColumn("QtyOnHand", typeof(decimal)));
            dtItems.Columns.Add(new DataColumn("LastCOst", typeof(decimal)));
            dtItems.Columns.Add(new DataColumn("PDevider", typeof(string)));
            dtItems.Columns.Add(new DataColumn("BasePrice", typeof(decimal)));
            dtItems.Columns.Add(new DataColumn("ProjMargin", typeof(string)));
            dtItems.Columns.Add(new DataColumn("ItemMovementHistoryID", typeof(int)));
        }
        /// <summary>
        /// This class is used to Extract PDF data with Initialize table.
        /// </summary>
        /// <param name="StoreID"></param>
        public void ExtractPDfData(Int32 StoreID)
        {
            InitializeTable();

            for (int i = 0; i < arLines.Count; i++)
            {
                try
                {
                    if (arLines[i].ToString().Contains("Reporting Range:"))
                    {
                        Data = arLines[i].ToString().Replace("Reporting Range:", "").Replace("[", "").Replace("]", "").Replace("Date-Time Range between", "");
                        if (Data != "")
                        {
                            Values = Data.Split(',');
                            StartDate = Values[0].ToString().Trim();
                            EndDate = Values[1].ToString().Trim();
                        }
                    }
                    else if (arLines[i].ToString().Contains("Date-Time Range between"))
                    {
                        Data = arLines[i].ToString().Replace("Reporting Range:", "").Replace("[", "").Replace("]", "").Replace("Date-Time Range between", "");
                        if (Data != "")
                        {
                            Values = Data.Split(',');
                            StartDate = Values[0].ToString().Trim();
                            EndDate = Values[1].ToString().Trim();
                        }
                    }
                    else if (arLines[i].ToString().Contains("Store:"))
                    {
                        StoreName = arLines[i].ToString().Replace("Store:", "").Trim();
                    }
                    else if (arLines[i].ToString().Contains("Supplier ID"))
                    {
                        IsItemReading = true;
                    }

                    if (IsItemReading)
                    {
                        if (arLines[i].ToString().Contains("Supplier:") == true && arLines[i].ToString() != "Supplier: (none defined)")
                        {
                            SupplierName = arLines[i].ToString().Replace("Supplier:", "").Trim();
                        }
                        else if (arLines[i].ToString().Contains("$") == false && arLines[i].ToString().Contains("%") == false)
                        {
                            DepartmentName = arLines[i].ToString().Trim();
                        }
                        else if (arLines[i].ToString().Contains("$"))
                        {
                            dr = dtItems.NewRow();
                            dr[0] = SupplierName;
                            dr[1] = DepartmentName;

                            Indexs = 9;
                            Data = arLines[i].ToString().Trim();
                            Values = Data.Split(' ');
                            for (int j = Values.Length - 1; j >= 0; j--)
                            {
                                try
                                {
                                    switch (Indexs)
                                    {
                                        case 9:
                                            if (Values[j].Trim() != "")
                                            {
                                                if (Values[j].ToString().Contains("%"))
                                                {
                                                    dr[Indexs] = Values[j].Trim();
                                                }
                                                else
                                                    j += 1;
                                                Indexs -= 1;
                                            }
                                            break;
                                        case 8:
                                        case 7:
                                        case 6:
                                        case 5:
                                        case 4:
                                            if (Values[j].Trim() != "/")
                                            {
                                                if (Values[j].Trim() != "" && Values[j].Trim() != "*")
                                                {
                                                    dr[Indexs] = Values[j].Replace("(", "").Replace(")", "").Replace("$", "").Replace(",", "").Replace("*", "").Trim();
                                                    Indexs -= 1;
                                                }
                                            }
                                            break;
                                        case 3:
                                            if (Values[j].Trim() != "")
                                            {
                                                dr[Indexs] = Values[j].Trim() + ' ' + dr[Indexs];
                                            }
                                            else if (dr[Indexs].ToString() != "")
                                                Indexs -= 1;
                                            break;
                                        case 2:
                                            if (Values[j].Trim() != "")
                                            {
                                                dr[Indexs] = Values[j].Trim();
                                            }
                                            break;
                                    }
                                }
                                catch (Exception ex)
                                {
                                    logger.Error("ItemMovementController - ExtractPDfData - " + DateTime.Now + " - " + ex.Message.ToString());
                                }


                            }
                            dtItems.Rows.Add(dr);
                        }
                    }

                }
                catch (Exception ex)
                {
                    logger.Error("ItemMovementController - ExtractPDfData - " + DateTime.Now + " - " + ex.Message.ToString());
                }
            }
        }
        /// <summary>
        /// This method is used for delete item movement.
        /// </summary>
        /// <param name="itemMovementViewModal"></param>
        /// <returns></returns>
        public string DeleteItemMovement(int? ItemMovementID)
        {
            ItemMovementViewModal itemMovementViewModal = new ItemMovementViewModal();
            itemMovementViewModal.ItemMovementID = ItemMovementID;
            try
            {
                //This Db class is  used to Get all Delete item Movement. 
                _itemMovementRepository.GetDeleteItemMovement(itemMovementViewModal);
                StatusMessageString = "Delete";
                //DeleteMessage = "Item Movement deleted successfully.";
                DeleteMessage = _commonRepository.GetMessageValue("IMRD", "Item Movement deleted successfully.");
                ViewBag.Delete = DeleteMessage;
            }
            catch (Exception ex)
            {
                DeleteMessage = "";
                ViewBag.Delete = DeleteMessage;
                logger.Error("ItemMovementController - DeleteItemMovement - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return StatusMessageString;
        }
    }
}