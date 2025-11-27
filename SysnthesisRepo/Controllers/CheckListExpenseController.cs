using Syncfusion.EJ2.Base;
using Syncfusion.EJ2.GridExport;
using Syncfusion.Pdf;
using EntityModels.Models;
using Repository;
using Repository.IRepository;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using NLog;

namespace SysnthesisRepo.Controllers
{
    public class CheckListExpenseController : Controller
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        #region Variables
        private readonly ICheckListExpenseRepository _CheckListExpenseRepository;
        private readonly ICommonRepository _CommonRepository;
        protected static int TotalDataCount;
        protected static string StatusMessage = "";
        #endregion
        // GET: CheckListExpense
        public CheckListExpenseController()
        {
            this._CheckListExpenseRepository = new CheckListExpenseRepository(new DBContext());
            this._CommonRepository = new CommonRepository(new DBContext());
        }
        public ActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// This method return Check List data.
        /// </summary>
        /// <param name="MSG"></param>
        /// <returns></returns>
        public async Task<ActionResult> CheckListIndexNew(string MSG)
        {
            ViewBag.Title = "Check List Details- Synthesis";

            string storeid = "";
            storeid = Convert.ToString(Session["storeid"]);
            ViewBag.storeid = storeid;
            return View();
        }
        /// <summary>
        /// This method use for Get All CheckListIndexNewGrid
        /// </summary>
        /// <returns></returns>
        public ActionResult CheckListIndexNewGrid()
        {
            ViewBag.StatusMessage = "";
            ViewBag.TotalDataCount = TotalDataCount;
            ViewBag.IsBindData = null; ;
            ViewBag.CurrentPageIndex = null;
            ViewBag.LastPageIndex = null;
            ViewBag.OrderByVal = null;
            ViewBag.IsAscVal = 1;
            ViewBag.PageSize = null;
            ViewBag.Alpha = "";
            ViewBag.SearchRecords = 0;
            ViewBag.startindex = null;
            ViewBag.SearchFlg = null;
            ViewBag.searchdashbord = null;
            ViewBag.AscVal = 0;
            ViewBag.IsFilter = "0";
            ViewBag.endIndex = 0;
            List<object> veg = new List<object>();

            veg.Insert(0, new { Text = "All", Value = "0" });
            veg.Insert(1, new { Text = "Mail out", Value = "1" });
            veg.Insert(2, new { Text = "Unmail out", Value = "2" });
            ViewBag.List = veg;

            List<object> All = new List<object>();
            All.Insert(0, new { Text = "None", Value = "0" });
            All.Insert(1, new { Text = "Select All", Value = "1" });
            All.Insert(2, new { Text = "Unselect All", Value = "2" });
            ViewBag.All = All;

            List<object> CheckFilter = new List<object>();
            CheckFilter.Insert(0, new { Text = "All", Value = "0" });
            CheckFilter.Insert(1, new { Text = "ACH", Value = "1" });
            CheckFilter.Insert(2, new { Text = "Amex", Value = "2" });
            CheckFilter.Insert(2, new { Text = "CHASEBP", Value = "3" });
            CheckFilter.Insert(2, new { Text = "EFT", Value = "4" });
            CheckFilter.Insert(2, new { Text = "To Print", Value = "5" });
            ViewBag.CheckFilter = CheckFilter;

            return View();
        }

        /// <summary>
        /// This is get all the Url DataSource Expense.
        /// </summary>
        /// <param name="dm"></param>
        /// <returns></returns>
        public ActionResult UrlDatasourceExpense(DataManagerRequest dm)
        {
            List<CheckIndexList> BindData = new List<CheckIndexList>();
            IEnumerable DataSource = new List<CheckIndexList>();
            var StoreId = Convert.ToInt32(Session["storeid"]);
            int count = 0;
            try
            {
                BindData = _CheckListExpenseRepository.Getchecklistbinddata(StoreId);
                DataSource = BindData;
                //This is DB class for get Check List data.
                DataOperations operation = new DataOperations();
                if (dm.Search != null && dm.Search.Count > 0)
                {
                    string search = dm.Search[0].Key.ToString().Trim().ToLower();
                }
                if (dm.Sorted != null && dm.Sorted.Count > 0) //Sorting
                {
                    DataSource = operation.PerformSorting(DataSource, dm.Sorted);
                }
                if (dm.Where != null && dm.Where.Count > 0) //Filtering
                {
                    DataSource = operation.PerformFiltering(DataSource, dm.Where, dm.Where[0].Operator);
                }
                count = DataSource.Cast<CheckIndexList>().Count();
                if (dm.Skip != 0)
                {
                    DataSource = operation.PerformSkip(DataSource, dm.Skip);   //Paging
                }
                if (dm.Take != 0)
                {
                    DataSource = operation.PerformTake(DataSource, dm.Take);
                }

                TotalDataCount = DataSource.OfType<CheckIndexList>().ToList().Count();
                if (TotalDataCount == 0)
                {
                    StatusMessage = "NoItem";
                }
                if (StatusMessage == null) { StatusMessage = ""; }
                if (StatusMessage == "")
                {
                    ViewBag.StatusMessage = "";
                }
                else
                {
                    ViewBag.StatusMessage = StatusMessage;
                }
                ViewBag.TotalDataCount = TotalDataCount;

                StatusMessage = "";
            }
            catch (Exception ex)
            {
                logger.Error("CheckListExpenseController - UrlDatasourceExpense - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return dm.RequiresCounts ? Json(new { result = DataSource, count = count }) : Json(DataSource);
        }

        /// <summary>
        /// This method retun ViewIn data.
        /// </summary>
        /// <param name="dm"></param>
        /// <param name="Mail"></param>
        /// <param name="startdate"></param>
        /// <param name="enddate"></param>
        /// <param name="Searchtext"></param>
        /// <returns></returns>
        public ActionResult ViewInv(DataManagerRequest dm, string Mail, string startdate, string enddate, string Searchtext,string CheckFilter)
        {
            List<CheckIndexList> BindData = new List<CheckIndexList>();

            if (startdate == "")
            {
                startdate = null;
            }
            if (enddate == "")
            {
                enddate = null;
            }
            var StoreId = Convert.ToInt32(Session["storeid"]);
            //This is Db class for List data witg Filter.
            BindData = _CheckListExpenseRepository.CheckListFilter_ALL(Mail.ToString(), startdate, enddate, StoreId, Searchtext, CheckFilter.ToString());
            IEnumerable DataSource = BindData;
            DataOperations operation = new DataOperations();
            
            if (dm.Sorted != null && dm.Sorted.Count > 0) //Sorting
            {
                DataSource = operation.PerformSorting(DataSource, dm.Sorted);
            }
            if (dm.Where != null && dm.Where.Count > 0) //Filtering
            {
                DataSource = operation.PerformFiltering(DataSource, dm.Where, dm.Where[0].Operator);
            }
            int count = DataSource.Cast<CheckIndexList>().Count();
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

        /// <summary>
        /// This method update status for Mail.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        /// 
        public ActionResult UpdateStatus(string id, string value)
        {
            string message = "";
            try
            {
                var UserName = System.Web.HttpContext.Current.User.Identity.Name;
                int UserID = _CommonRepository.getUserId(UserName);
                //This class use to update Mail Out status.
                _CheckListExpenseRepository.UpdateMailOutStatus(id, value, UserID);
                message = "success";
            }
            catch (Exception ex)
            {
                logger.Error("CheckListExpenseController - UpdateStatus - " + DateTime.Now + " - " + ex.Message.ToString());
                message = "Something went wrong..!!";
            }
            return Json(message);
        }

        [HttpPost]
        public ActionResult UpdateAllStatus(string StartDate,string EndDate, string value, string Searchtext, string Mail,string CheckFilter)
        {
            string message = "";
            try
            {
                var UserName = System.Web.HttpContext.Current.User.Identity.Name;
                int UserID = _CommonRepository.getUserId(UserName);
                var StoreId = Convert.ToInt32(Session["storeid"]);
                //This class use to update All Mail Out status.
                _CheckListExpenseRepository.UpdateMailOutAllStatus(StartDate, EndDate, value, UserID, Searchtext, Mail,StoreId, CheckFilter);
                message = "success";
            }
            catch (Exception ex)
            {
                logger.Error("CheckListExpenseController - UpdateAllStatus - " + DateTime.Now + " - " + ex.Message.ToString());
                message = "Something went wrong..!!";
            }
            return Json(message);
        }
        /// <summary>
        /// This method is use to Pdf Export.
        /// </summary>
        /// <param name="gridModel"></param>
        /// <param name="Mail"></param>
        /// <param name="startdate"></param>
        /// <param name="enddate"></param>
        /// <param name="Searchtext"></param>
        /// <returns></returns>
        public ActionResult PdfExport(string gridModel, string Mail, string startdate, string enddate, string Searchtext, string CheckFilter)
        {
            int? StoreIds = 0;
            if (Session["storeid"] != null)
            {
                StoreIds = Convert.ToInt32(Convert.ToString(Session["storeid"]));
            }
            List<CheckIndexList> BindData = new List<CheckIndexList>();
            //This is get all Check List data with filter.
            BindData = _CheckListExpenseRepository.CheckListFilter_ALL(Mail.ToString(), startdate, enddate, StoreIds, Searchtext, CheckFilter);
            foreach (var item in BindData)
            {
                if (item.MailSent == 1)
                {
                    item.MailsentBool = "true";
                }
                else
                {
                    item.MailsentBool = "false";
                }
            }
            PdfDocument doc = new PdfDocument();
            doc.PageSettings.Orientation = PdfPageOrientation.Landscape;
            doc.PageSettings.Size = PdfPageSize.A3;
            GridPdfExport exp = new GridPdfExport();
            exp.Theme = "flat-saffron";
            exp.FileName = "UnclearedCheckDetail.pdf";
            exp.PdfDocument = doc;
            Syncfusion.EJ2.Grids.Grid gridProperty = ConvertGridObject(gridModel);
            return exp.PdfExport<VenodrMasterSelect>(gridProperty, BindData);
        }

        /// <summary>
        /// This is class use for Convetgrid objects 
        /// </summary>
        /// <param name="gridProperty"></param>
        /// <returns></returns>
        private Syncfusion.EJ2.Grids.Grid ConvertGridObject(string gridProperty)
        {
            Syncfusion.EJ2.Grids.Grid GridModel = (Syncfusion.EJ2.Grids.Grid)Newtonsoft.Json.JsonConvert.DeserializeObject(gridProperty, typeof(Syncfusion.EJ2.Grids.Grid));

            GridColumnModel cols = (GridColumnModel)Newtonsoft.Json.JsonConvert.DeserializeObject("{\"allowGrouping\":false,\"allowPaging\":false,\"pageSettings\":{\"currentPage\":1,\"pageCount\":8,\"pageSize\":50},\"sortSettings\":{},\"allowPdfExport\":true,\"allowExcelExport\":true,\"aggregates\":[],\"filterSettings\":{\"immediateModeDelay\":1500,\"type\":\"CheckBox\"},\"groupSettings\":{\"columns\":[],\"enableLazyLoading\":false,\"disablePageWiseAggregates\":false},\"columns\":[{\"field\":\"Tx_Date\",\"format\":\"MM-dd-yyyy\",\"width\":\"60\",\"foreignKeyField\":\"Tx_Date\",\"visible\":true,\"index\":1,\"type\":\"date\",\"headerText\":\"Date\"}],\"locale\":\"en-US\",\"searchSettings\":{\"key\":\"\",\"fields\":[]}}", typeof(GridColumnModel));
            cols.columns.Add(new Syncfusion.EJ2.Grids.GridColumn { Field = "StoreName", HeaderText = "Store Name" });
            cols.columns.Add(new Syncfusion.EJ2.Grids.GridColumn { Field = "Doc_no", HeaderText = "Check #" });

            cols.columns.Add(new Syncfusion.EJ2.Grids.GridColumn { Field = "Account_name", HeaderText = "Bank Account" });
            cols.columns.Add(new Syncfusion.EJ2.Grids.GridColumn { Field = "Txn_type", HeaderText = "Transaction Type" });
            cols.columns.Add(new Syncfusion.EJ2.Grids.GridColumn { Field = "EntityName", HeaderText = "Name" });
            cols.columns.Add(new Syncfusion.EJ2.Grids.GridColumn { Field = "Amount", HeaderText = "Amount" });
            cols.columns.Add(new Syncfusion.EJ2.Grids.GridColumn { Field = "MailsentBool", HeaderText = "Mailed Out" });
            cols.columns.Add(new Syncfusion.EJ2.Grids.GridColumn { Field = "UpdateDate", HeaderText = "Update Date" });

            foreach (var item in cols.columns)
            {
                item.AutoFit = true;
            }
            GridModel.Columns = cols.columns;
            return GridModel;
        }

        /// <summary>
        /// This method is use to Excel Export.
        /// </summary>
        /// <param name="gridModel"></param>
        /// <param name="Mail"></param>
        /// <param name="startdate"></param>
        /// <param name="enddate"></param>
        /// <param name="Searchtext"></param>
        /// <returns></returns>
        public ActionResult ExcelExport(string gridModel, string Mail, string startdate, string enddate, string Searchtext, string CheckFilter)
        {
            int? StoreIds = 0;
            if (Session["storeid"] != null)
            {
                StoreIds = Convert.ToInt32(Convert.ToString(Session["storeid"]));
            }
            List<CheckIndexList> BindData = new List<CheckIndexList>();
            //This is get all Check List data with filter.
            BindData = _CheckListExpenseRepository.CheckListFilter_ALL(Mail.ToString(), startdate, enddate, StoreIds, Searchtext, CheckFilter);
            foreach (var item in BindData)
            {
                if (item.MailSent == 1)
                {
                    item.MailsentBool = "true";
                }
                else
                {
                    item.MailsentBool = "false";
                }
            }
            GridExcelExport exp = new GridExcelExport();
            exp.FileName = "UnclearCheckList.xlsx";
            Syncfusion.EJ2.Grids.Grid gridProperty = ConvertGridObject(gridModel);
            return exp.ExcelExport<VenodrMasterSelect>(gridProperty, BindData);
        }
    }
}