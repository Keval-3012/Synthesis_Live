using EntityModels.Models;
using NLog;
using Repository;
using Repository.IRepository;
using Syncfusion.EJ2.Base;
using Syncfusion.EJ2.GridExport;
using Syncfusion.Pdf;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace SysnthesisRepo.Controllers
{
    public class ProductMappingLogController : Controller
    {
        private readonly IProductMappingLogRepository _ProductMappingLogRepository;
        private readonly ICommonRepository _CommonRepository;
        Logger logger = LogManager.GetCurrentClassLogger();
        public ProductMappingLogController()
        {
            this._ProductMappingLogRepository = new ProductMappingLogRepository(new DBContext());
            this._CommonRepository = new CommonRepository(new DBContext());
        }

        // GET: ProductMappingLog

        protected static string StatusMessage = "";
        protected static Array Arr;
        protected static bool IsArray;
        protected static IEnumerable BindData;
        protected static bool IsEdit = false;
        protected static int TotalDataCount;
        protected static string success = null;
        protected static string Error = null;
       


        /// <summary>
        /// This method return Prodcut mapping Log.
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "Administrator,ViewProductMappingLog")]
        public ActionResult ProductMappingLog()
        {
            ViewBag.Title = "Product Mapping Log - Synthesis";
            return View();
        }
        public ActionResult Grid()
        {
            return View();
        }
        /// <summary>
        /// This method is url Data Source.
        /// </summary>
        /// <param name="dm"></param>
        /// <returns></returns>
        public ActionResult UrlDatasource(DataManagerRequest dm)
        {
            //Db class is get Product Mapping Log.
            List<InvoiceUserProductMapLogList> lstInvoiceUserProductMapLog = _ProductMappingLogRepository.GetProductMappingLog();
            IEnumerable DataSource = lstInvoiceUserProductMapLog;
            DataOperations operation = new DataOperations();
            int count = 0;
            try
            {
                if (dm.Search != null && dm.Search.Count > 0)
                {
                    string search = dm.Search[0].Key.ToString().Trim().ToLower();
                    DataSource = lstInvoiceUserProductMapLog.ToList().Where(x => (x.ItemNoOrUPCCode != null && x.ItemNoOrUPCCode.ToLower().Contains(search)) || (x.CDateTime != null && x.CDateTime.ToLower().Contains(search)) || (x.Description != null && x.Description.ToLower().Contains(search)) || (x.Name != null && x.Name.ToLower().Contains(search))
                   || (x.Operation != null && x.Operation.ToLower().Contains(search))).ToList();
                }
                if (dm.Sorted != null && dm.Sorted.Count > 0) //Sorting
                {
                    DataSource = operation.PerformSorting(DataSource, dm.Sorted);
                }
                if (dm.Where != null && dm.Where.Count > 0) //Filtering
                {
                    DataSource = operation.PerformFiltering(DataSource, dm.Where, dm.Where[0].Operator);
                }
                count = DataSource.Cast<InvoiceUserProductMapLogList>().Count();
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
                logger.Error("ProductMappingLogController - UrlDatasource - " + DateTime.Now + " - " + ex.Message.ToString());
            }

       
            return dm.RequiresCounts ? Json(new { result = DataSource, count = count }, "application/json", Encoding.UTF8, JsonRequestBehavior.AllowGet) : Json(DataSource, "application/json", Encoding.UTF8, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> ExcelExport(string gridModel)
        {
            List<InvoiceUserProductMapLogList> lstProducts = new List<InvoiceUserProductMapLogList>();
            GridExcelExport exp = new GridExcelExport();

            try
            {
                lstProducts = _ProductMappingLogRepository.GetProductMappingLog();
                exp.FileName = "ProductMappingLog.xlsx";
            }
            catch (Exception ex)
            {
                logger.Error("ProductMappingLogController - ExcelExport - " + DateTime.Now + " - " + ex.Message.ToString());
            }


            Syncfusion.EJ2.Grids.Grid gridProperty = ConvertGridObject(gridModel, lstProducts);
            return exp.ExcelExport<InvoiceUserProductMapLogList>(gridProperty, lstProducts);

        }

        private Syncfusion.EJ2.Grids.Grid ConvertGridObject(string gridProperty, List<InvoiceUserProductMapLogList> data)
        {
            Syncfusion.EJ2.Grids.Grid GridModel = (Syncfusion.EJ2.Grids.Grid)Newtonsoft.Json.JsonConvert.DeserializeObject(gridProperty, typeof(Syncfusion.EJ2.Grids.Grid));

            try
            {
                GridColumnModel cols = (GridColumnModel)Newtonsoft.Json.JsonConvert.DeserializeObject("{\"allowGrouping\":false,\"allowPaging\":false,\"pageSettings\":{\"currentPage\":1,\"pageCount\":2,\"pageSize\":100},\"sortSettings\":{},\"allowPdfExport\":true,\"allowExcelExport\":true,\"aggregates\":[],\"filterSettings\":{\"immediateModeDelay\":1500,\"type\":\"CheckBox\"},\"groupSettings\":{\"columns\":[],\"enableLazyLoading\":false,\"disablePageWiseAggregates\":false},\"columns\":[],\"locale\":\"en-US\",\"searchSettings\":{\"key\":\"\",\"fields\":[]}}", typeof(GridColumnModel));
                cols.columns.Add(new Syncfusion.EJ2.Grids.GridColumn { Field = "CDateTime", HeaderText = "Date and Time" });
                cols.columns.Add(new Syncfusion.EJ2.Grids.GridColumn { Field = "Name", HeaderText = "Name" });
                cols.columns.Add(new Syncfusion.EJ2.Grids.GridColumn { Field = "Operation", HeaderText = "Operation" });
                cols.columns.Add(new Syncfusion.EJ2.Grids.GridColumn { Field = "InvoiceNumber", HeaderText = "Invoice #" });
                cols.columns.Add(new Syncfusion.EJ2.Grids.GridColumn { Field = "ItemNoOrUPCCode", HeaderText = "Item No / UPC" });
                cols.columns.Add(new Syncfusion.EJ2.Grids.GridColumn { Field = "Description", HeaderText = "Item Name / Description" });
                cols.columns.Add(new Syncfusion.EJ2.Grids.GridColumn { Field = "AffectedRows", HeaderText = "Affected Rows" });

                foreach (var item in cols.columns)
                {
                    item.AutoFit = true;
                    item.Width = "10%";
                }
                foreach (var item in data)
                {
                    foreach (var col in cols.columns)
                    {
                        if (col.Field == "CDateTime")
                        {
                            int maxLineLength = 20;

                            if (!string.IsNullOrEmpty(item.CDateTime))
                            {
                                StringBuilder memoBuilder = new StringBuilder();
                                for (int i = 0; i < item.CDateTime.Length; i += maxLineLength)
                                {
                                    int length = Math.Min(maxLineLength, item.CDateTime.Length - i);
                                    memoBuilder.AppendLine(item.CDateTime.Substring(i, length));
                                }
                                item.CDateTime = memoBuilder.ToString().Trim();
                            }
                        }
                        else if (col.Field == "Name")
                        {
                            int maxLineLength = 20;

                            if (!string.IsNullOrEmpty(item.Name))
                            {
                                StringBuilder memoBuilder = new StringBuilder();
                                for (int i = 0; i < item.Name.Length; i += maxLineLength)
                                {
                                    int length = Math.Min(maxLineLength, item.Name.Length - i);
                                    memoBuilder.AppendLine(item.Name.Substring(i, length));
                                }
                                item.Name = memoBuilder.ToString().Trim();
                            }
                        }
                        else if (col.Field == "Operation")
                        {
                            int maxLineLength = 7;

                            if (!string.IsNullOrEmpty(item.Operation))
                            {
                                StringBuilder memoBuilder = new StringBuilder();
                                for (int i = 0; i < item.Operation.Length; i += maxLineLength)
                                {
                                    int length = Math.Min(maxLineLength, item.Operation.Length - i);
                                    memoBuilder.AppendLine(item.Operation.Substring(i, length));
                                }
                                item.Operation = memoBuilder.ToString().Trim();
                            }
                        }
                        else if (col.Field == "InvoiceNumber")
                        {
                            int maxLineLength = 11;

                            if (!string.IsNullOrEmpty(item.InvoiceNumber))
                            {
                                StringBuilder memoBuilder = new StringBuilder();
                                for (int i = 0; i < item.InvoiceNumber.Length; i += maxLineLength)
                                {
                                    int length = Math.Min(maxLineLength, item.InvoiceNumber.Length - i);
                                    memoBuilder.AppendLine(item.InvoiceNumber.Substring(i, length));
                                }
                                item.InvoiceNumber = memoBuilder.ToString().Trim();
                            }
                        }
                        else if (col.Field == "ItemNoOrUPCCode")
                        {
                            int maxLineLength = 20;

                            if (!string.IsNullOrEmpty(item.ItemNoOrUPCCode))
                            {
                                StringBuilder memoBuilder = new StringBuilder();
                                for (int i = 0; i < item.ItemNoOrUPCCode.Length; i += maxLineLength)
                                {
                                    int length = Math.Min(maxLineLength, item.ItemNoOrUPCCode.Length - i);
                                    memoBuilder.AppendLine(item.ItemNoOrUPCCode.Substring(i, length));
                                }
                                item.ItemNoOrUPCCode = memoBuilder.ToString().Trim();
                            }
                        }
                        else if (col.Field == "Description")
                        {
                            int maxLineLength = 20;

                            if (!string.IsNullOrEmpty(item.Description))
                            {
                                StringBuilder memoBuilder = new StringBuilder();
                                for (int i = 0; i < item.Description.Length; i += maxLineLength)
                                {
                                    int length = Math.Min(maxLineLength, item.Description.Length - i);
                                    memoBuilder.AppendLine(item.Description.Substring(i, length));
                                }
                                item.Description = memoBuilder.ToString().Trim();
                            }
                        }
                    }
                }

                GridModel.Columns = cols.columns;
            }
            catch (Exception ex)
            {
                logger.Error("ProductMappingLogController - ConvertGridObject - " + DateTime.Now + " - " + ex.Message.ToString());
            }

            return GridModel;
        }

        public ActionResult PdfExport(string gridModel)
        {
            List<InvoiceUserProductMapLogList> lstProducts = new List<InvoiceUserProductMapLogList>();
            PdfDocument doc = new PdfDocument();
            GridPdfExport exp = new GridPdfExport();

            try
            {
                lstProducts = _ProductMappingLogRepository.GetProductMappingLog();

                doc.PageSettings.Orientation = PdfPageOrientation.Landscape;
                doc.PageSettings.Size = PdfPageSize.A3;

                exp.Theme = "flat-saffron";
                exp.FileName = "ProductMappingLog.pdf";
                exp.PdfDocument = doc;
            }
            catch (Exception ex)
            {
                logger.Error("ProductMappingLogController - PdfExport - " + DateTime.Now + " - " + ex.Message.ToString());
            }

            Syncfusion.EJ2.Grids.Grid gridProperty = ConvertGridObject(gridModel, lstProducts);
            return exp.PdfExport<InvoiceUserProductMapLogList>(gridProperty, lstProducts);
        }
    }
}