using Syncfusion.EJ2.Base;
using EntityModels.Models;
using Repository;
using Repository.IRepository;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using NLog;
using System.Data;
using Syncfusion.EJ2.GridExport;
using Syncfusion.Pdf;

namespace SysnthesisRepo.Controllers
{
    public class ExpenseWeeklySettingController : Controller
    {
        private readonly IExpenseWeeklySettingRepository _ExpenseWeeklySettingRepository;
        private readonly IQBRepository _QBRepository;
        private readonly ICommonRepository _CommonRepository;
        protected static int TotalDataCount;
        protected static string StatusMessage = "";
        private static Logger logger = LogManager.GetCurrentClassLogger();
        // GET: ExpenseWeeklySetting
        public ExpenseWeeklySettingController()
        {
            this._ExpenseWeeklySettingRepository = new ExpenseWeeklySettingRepository(new DBContext());
            this._QBRepository = new QBRepository(new DBContext());
            this._CommonRepository = new CommonRepository(new DBContext());
        }
        /// <summary>
        /// This method is return Index view.
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            ViewBag.Title = "Expense Weekly Setting Master - Synthesis";
            try
            {
                int storeid = 0;
                string SID = Convert.ToString(Session["storeid"]);
                if (SID != "0" && SID != "")
                {
                    storeid = Convert.ToInt32(Session["storeid"]);
                    ViewBag.StatusMessage = "";
                }
                else
                {
                    ViewBag.StatusMessage = "NoStoreSelected";
                }
            }
            catch (Exception ex)
            {
                logger.Error("ExpenseWeeklySettingController - Index - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            
            return View();
        }
        /// <summary>
        /// This method is return view with Home Expense Weekly Sales Setting.
        /// </summary>
        /// <returns></returns>
        public ActionResult HomeExpenseWeeklySalesSetting()
        {
            IEnumerable RtnData = null;
            try
            {
                int storeid = 0;
                storeid = Convert.ToInt32(Session["storeid"]);
                List<DepartmentMaster> departmentMasters = new List<DepartmentMaster>();
                //Db class For Get departmentMasters.
                departmentMasters = _ExpenseWeeklySettingRepository.GetDepartmentMasters();
                var DeptBankList = departmentMasters.Where(a => (a.AccountTypeId == 3 || a.AccountTypeId == 19) && a.IsActive == true && a.StoreId.ToString() == storeid.ToString()).ToList();
                var DeptList = departmentMasters.Where(a => (a.AccountTypeId != 3 || a.AccountTypeId != 19) && a.IsActive == true && a.StoreId.ToString() == storeid.ToString()).ToList();
                //db class for get vendor masters.
                var VendorList = _ExpenseWeeklySettingRepository.GetVendorMasters().Where(a => a.IsActive == true && a.ListId != null && a.StoreId.ToString() == storeid.ToString()).ToList();
                //This db class use to get Store online desktop with Storeid.
                string Stores = _QBRepository.GetStoreOnlineDesktop(Convert.ToInt32(storeid));
                if (Stores == "Online")
                {
                    ViewBag.DepartmentBankList = (DeptBankList.Select(s => new ddllist
                    {
                        Value = s.DepartmentId.ToString(),
                        Text = s.DepartmentName,
                        selectedvalue = s.DepartmentId,
                        ListID = s.ListId
                    }).ToList()).Where(a => !a.ListID.Contains("-")).ToList();

                    ViewBag.DepartmentList = (DeptList.Select(s => new ddllist
                    {
                        Value = s.DepartmentId.ToString(),
                        Text = s.DepartmentName,
                        selectedvalue = s.DepartmentId,
                        ListID = s.ListId
                    }).ToList()).Where(a => !a.ListID.Contains("-")).ToList();

                    ViewBag.VendorList = (VendorList.Select(s => new ddllist
                    {
                        Value = s.VendorId.ToString(),
                        Text = s.VendorName,
                        selectedvalue = s.VendorId,
                        ListID = s.ListId
                    }).ToList()).Where(a => !a.ListID.Contains("-")).ToList();

                }
                else if (Stores == "Desktop")
                {
                    ViewBag.DepartmentBankList = (DeptBankList.Select(s => new ddllist
                    {
                        Value = s.DepartmentId.ToString(),
                        Text = s.DepartmentName,
                        selectedvalue = s.DepartmentId,
                        ListID = s.ListId
                    }).ToList()).Where(a => !a.ListID.Contains("-")).ToList();

                    ViewBag.DepartmentList = (DeptList.Select(s => new ddllist
                    {
                        Value = s.DepartmentId.ToString(),
                        Text = s.DepartmentName,
                        selectedvalue = s.DepartmentId,
                        ListID = s.ListId
                    }).ToList()).Where(a => !a.ListID.Contains("-")).ToList();

                    ViewBag.VendorList = (VendorList.Select(s => new ddllist
                    {
                        Value = s.VendorId.ToString(),
                        Text = s.VendorName,
                        selectedvalue = s.VendorId,
                        ListID = s.ListId
                    }).ToList()).Where(a => !a.ListID.Contains("-")).ToList();
                }

                CheckExpense_Exist(storeid);

                //db class for get home Expense Weekly sales by  storeid.
                RtnData = _ExpenseWeeklySettingRepository.HomeExpenseWeeklySalesSettingsByStoreID(storeid);
            }
            catch (Exception ex) {
                logger.Error("ExpenseWeeklySettingController - HomeExpenseWeeklySalesSetting - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            ViewBag.StatusMessage = "";
            return View(RtnData);
        }

        /// <summary>
        /// This Class is used for CheckExpense_Exist with store Id.
        /// </summary>
        /// <param name="StoreID"></param>
        public void CheckExpense_Exist(int StoreID)
        {

            //db class for get home Expense Weekly sales by  storeid.
            HomeExpenseWeeklySalesSetting obj = _ExpenseWeeklySettingRepository.HomeExpenseWeeklySalesSettingsByStoreID(StoreID).FirstOrDefault();
            try
            {
                if (obj == null)
                {
                    HomeExpenseWeeklySalesSetting objs = new HomeExpenseWeeklySalesSetting();
                    objs.StoreId = StoreID;
                    objs.DeliveryCostId = null;
                    objs.PaymentFeesId = null;
                    objs.TipsPaidId = null;
                    objs.VendorId = null;

                    //This db class use to  add home Expense Weekly sales settings.
                    _ExpenseWeeklySettingRepository.SaveHomeExpenseWeeklySalesSettings(objs);
                }
            }
            catch (Exception ex)
            {
                logger.Error("ExpenseWeeklySettingController - CheckExpense_Exist - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            
        }

        /// <summary>
        /// This method is update ExpenseWeekly setting.
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="DeliveryCostId"></param>
        /// <param name="PaymentFeesId"></param>
        /// <param name="TipsPaidId"></param>
        /// <param name="VendorId"></param>
        /// <param name="BankAccountID"></param>
        /// <returns></returns>
        public JsonResult UpdateExpenseWeekly_Setting(int ID, int DeliveryCostId, int PaymentFeesId, int TipsPaidId, int? VendorId, int? BankAccountID)
        {
            string message = "";
            int storeid = 0;
            storeid = Convert.ToInt32(Session["storeid"]);

            try
            {
          
                //db class used to get home Expense Weekly sales by  storeid.
                HomeExpenseWeeklySalesSetting data = _ExpenseWeeklySettingRepository.HomeExpenseWeeklySalesSettingsByID(ID);
                data.DeliveryCostId = Convert.ToInt32(DeliveryCostId);
                data.PaymentFeesId = Convert.ToInt32(PaymentFeesId);
                data.TipsPaidId = Convert.ToInt32(TipsPaidId);
                data.BankAccountId = BankAccountID;
                data.VendorId = VendorId;

                //This db class is used to update home Expense Weekly sales settings.
                _ExpenseWeeklySettingRepository.UpdateHomeExpenseWeeklySalesSettings(data);
            }
            catch (Exception ex)
            {
                logger.Error("ExpenseWeeklySettingController - UpdateExpenseWeekly_Setting - " + DateTime.Now + " - " + ex.Message.ToString());
            }


            message = "Edit";
            return Json(message, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// This method is return Home ExpenseIndex New.
        /// </summary>
        /// <param name="MSG"></param>
        /// <returns></returns>
        public async Task<ActionResult> HomeExpenseIndexNew(string MSG)
        {
            ViewBag.Title = "Homesome Expense Details- Synthesis";

            string storeid = "";
            storeid = Convert.ToString(Session["storeid"]);
            ViewBag.storeid = storeid;
            return View();
        }
        /// <summary>
        /// This method is return Home ExpenseIndex New Grid.
        /// </summary>
        /// <returns></returns>
        public ActionResult HomeExpenseIndexNewGrid()
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
            return View();
        }

        /// <summary>
        /// This method is return the Url Data Source Expense.
        /// </summary>
        /// <param name="dm"></param>
        /// <returns></returns>
        public ActionResult UrlDatasourceExpense(DataManagerRequest dm)
        {
            List<HomeExpensesDetailList> BindData = new List<HomeExpensesDetailList>();

            DBContext db1 = new DBContext();
            var StoreId = Convert.ToInt32(Session["storeid"]);
            BindData = db1.Database.SqlQuery<HomeExpensesDetailList>("[SP_HomeSome_Expenses_Proc] @Mode={0},@StoreID={1}", "HomeExpenses", StoreId).ToList();
            IEnumerable DataSource = BindData;
            int count = 0;
            try
            {
                db1.Dispose();
                db1 = null;

              
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
                count = DataSource.Cast<HomeExpensesDetailList>().Count();
                if (dm.Skip != 0)
                {
                    DataSource = operation.PerformSkip(DataSource, dm.Skip);   //Paging
                }
                if (dm.Take != 0)
                {
                    DataSource = operation.PerformTake(DataSource, dm.Take);
                }

                TotalDataCount = DataSource.OfType<HomeExpensesDetailList>().ToList().Count();
                // ViewBag.Invoicecount = TotalDataCount;
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
				logger.Error("ExpenseWeeklySettingController - UrlDatasourceExpense - " + DateTime.Now + " - " + ex.Message.ToString());
            }
           
            return dm.RequiresCounts ? Json(new { result = DataSource, count = count }) : Json(DataSource);
        }

        public ActionResult ViewWithFilter(DataManagerRequest dm, string startdate, string enddate)
        {
            List<HomeExpensesDetailList> BindData = new List<HomeExpensesDetailList>();

            DBContext db1 = new DBContext();
            var StoreId = Convert.ToInt32(Session["storeid"]);
            BindData = db1.Database.SqlQuery<HomeExpensesDetailList>("[SP_HomeSome_Expenses_Proc] @Mode={0},@StoreID={1},@ExpenseDate={2},@ExpenseEndDate={3}", "HomeExpensesFilter", StoreId, startdate == "" ? null : startdate, enddate == "" ? null : enddate).ToList();
            IEnumerable DataSource = BindData;
            int count = 0;
            try
            {
                db1.Dispose();
                db1 = null;


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
                count = DataSource.Cast<HomeExpensesDetailList>().Count();
                if (dm.Skip != 0)
                {
                    DataSource = operation.PerformSkip(DataSource, dm.Skip);   //Paging
                }
                if (dm.Take != 0)
                {
                    DataSource = operation.PerformTake(DataSource, dm.Take);
                }

                TotalDataCount = DataSource.OfType<HomeExpensesDetailList>().ToList().Count();
                // ViewBag.Invoicecount = TotalDataCount;
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
                logger.Error("ExpenseWeeklySettingController - UrlDatasourceExpense - " + DateTime.Now + " - " + ex.Message.ToString());
            }

            return dm.RequiresCounts ? Json(new { result = DataSource, count = count }) : Json(DataSource);
        }
        /// <summary>
        /// This method is use to change status using homeexpenseId.
        /// </summary>
        /// <param name="HomeexpenseID"></param>
        /// <returns></returns>
        public ActionResult ChangeStatus(int HomeexpenseID)
        {
            try
            {
                //This db class use to Change status for Home Expense.
                _ExpenseWeeklySettingRepository.ChangeStatus(HomeexpenseID);
                //return RedirectToAction("ExpenseCheckIndex", new { MSG = "Success" });
                return Json("OK", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error("ExpenseWeeklySettingController - ChangeStatus - " + DateTime.Now + " - " + ex.Message.ToString());
                return Json("Error", JsonRequestBehavior.AllowGet);

            }
        }

        public async Task<ActionResult> ExcelExport(string gridModel, string startdate, string enddate)
        {
            List<HomeExpensesDetailList> BindData = new List<HomeExpensesDetailList>();

            DBContext db1 = new DBContext();
            var StoreId = Convert.ToInt32(Session["storeid"]);
            BindData = db1.Database.SqlQuery<HomeExpensesDetailList>("[SP_HomeSome_Expenses_Proc] @Mode={0},@StoreID={1},@ExpenseDate={2},@ExpenseEndDate={3}", "HomeExpensesFilter", StoreId, startdate == "" ? null : startdate, enddate == "" ? null : enddate).ToList();
            


            GridExcelExport exp = new GridExcelExport();
            exp.FileName = "HomesomeExpensesDetails.xlsx";
            Syncfusion.EJ2.Grids.Grid gridProperty = ConvertGridObject(gridModel);
            return exp.ExcelExport<HomeExpensesDetailList>(gridProperty, BindData);

        }

        private Syncfusion.EJ2.Grids.Grid ConvertGridObject(string gridProperty)
        {
            Syncfusion.EJ2.Grids.Grid GridModel = (Syncfusion.EJ2.Grids.Grid)Newtonsoft.Json.JsonConvert.DeserializeObject(gridProperty, typeof(Syncfusion.EJ2.Grids.Grid));

            GridColumnModel cols = (GridColumnModel)Newtonsoft.Json.JsonConvert.DeserializeObject("{\"allowGrouping\":false,\"allowPaging\":false,\"pageSettings\":{\"currentPage\":1,\"pageCount\":2,\"pageSize\":100},\"sortSettings\":{},\"allowPdfExport\":true,\"allowExcelExport\":true,\"aggregates\":[],\"filterSettings\":{\"immediateModeDelay\":1500,\"type\":\"CheckBox\"},\"groupSettings\":{\"columns\":[],\"enableLazyLoading\":false,\"disablePageWiseAggregates\":false},\"columns\":[],\"locale\":\"en-US\",\"searchSettings\":{\"key\":\"\",\"fields\":[]}}", typeof(GridColumnModel));
            cols.columns.Add(new Syncfusion.EJ2.Grids.GridColumn { Field = "PaymentFees", HeaderText = "Payment Fees" });
            cols.columns.Add(new Syncfusion.EJ2.Grids.GridColumn { Field = "DeliveryCost", HeaderText = "Delivery Cost" });
            cols.columns.Add(new Syncfusion.EJ2.Grids.GridColumn { Field = "TipsPaid", HeaderText = "Tips Paid" });
            cols.columns.Add(new Syncfusion.EJ2.Grids.GridColumn { Field = "ExpenseDate", HeaderText = "Expense Date" });
            cols.columns.Add(new Syncfusion.EJ2.Grids.GridColumn { Field = "Status", HeaderText = "Status" });
            cols.columns.Add(new Syncfusion.EJ2.Grids.GridColumn { Field = "PaymentFeesStatus", HeaderText = "Payment Fees Status" });

            foreach (var item in cols.columns)
            {
                item.AutoFit = true;
            }
            GridModel.Columns = cols.columns;
            return GridModel;
        }

        public ActionResult PdfExport(string gridModel, string startdate, string enddate)
        {
            List<HomeExpensesDetailList> BindData = new List<HomeExpensesDetailList>();

            DBContext db1 = new DBContext();
            var StoreId = Convert.ToInt32(Session["storeid"]);
            BindData = db1.Database.SqlQuery<HomeExpensesDetailList>("[SP_HomeSome_Expenses_Proc] @Mode={0},@StoreID={1},@ExpenseDate={2},@ExpenseEndDate={3}", "HomeExpensesFilter", StoreId, startdate == "" ? null : startdate, enddate == "" ? null : enddate).ToList();


            PdfDocument doc = new PdfDocument();
            doc.PageSettings.Orientation = PdfPageOrientation.Landscape;
            doc.PageSettings.Size = PdfPageSize.A3;


            GridPdfExport exp = new GridPdfExport();
            exp.Theme = "flat-saffron";
            exp.FileName = "HomeSomeExpense.pdf";
            exp.PdfDocument = doc;

            Syncfusion.EJ2.Grids.Grid gridProperty = ConvertGridObject(gridModel);
            return exp.PdfExport<ExpenseCheckSelect>(gridProperty, BindData);
        }
    }
}