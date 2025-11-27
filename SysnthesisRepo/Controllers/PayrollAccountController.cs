using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EntityModels.Models;
using NLog;
using Repository;
using Repository.IRepository;
using SynthesisViewModal;

namespace SynthesisRepo.Controllers
{
    [Authorize(Roles = "Administrator,PayrollAccountSettings")]
    public class PayrollAccountController : Controller
    {
        private readonly ICommonRepository _commonRepository;
        private readonly IUserActivityLogRepository _ActivityLogRepository;
        private readonly IPayrollAccountRepository _payrollAccountRepository;
        private readonly IQBRepository _QBRepository;
        private readonly IMastersBindRepository _mastersBindRepository;
        protected static Array Arr;
        protected static bool IsArray;

        protected static bool IsEdit = false;
        protected static int TotalDataCount;
        protected static string StatusMessage = "";
        protected static string InsertMessage = "";
        protected static string Editmessage = "";
        private static Logger logger = LogManager.GetCurrentClassLogger();
        public PayrollAccountController()
        {
            this._ActivityLogRepository = new UserActivityLogRepository(new DBContext());
            this._payrollAccountRepository = new PayrollAccountRepository(new DBContext());
            this._QBRepository = new QBRepository(new DBContext());
            this._commonRepository = new CommonRepository(new DBContext());
            this._mastersBindRepository = new MastersBindRepository(new DBContext());
        }
        // GET: Admin/PayrollAccount
        /// <summary>
        /// This method is return Index view With Payroll Account.
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            ViewBag.Title = "Payroll Account - Synthesis";
            try
            {
                int storeid = 0;
                if (Convert.ToString(Session["storeid"]) != "0")
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
                logger.Error("PayrollAccountController - Index - " + DateTime.Now + " - " + ex.Message.ToString());
            }

            
            return View();
        }
        /// <summary>
        /// This method is return Grid view.
        /// </summary>
        /// <param name="startIndex"></param>
        /// <param name="endIndex"></param>
        /// <param name="IsBindData"></param>
        /// <param name="currentPageIndex"></param>
        /// <param name="orderby"></param>
        /// <param name="IsAsc"></param>
        /// <param name="PageSize"></param>
        /// <param name="SearchRecords"></param>
        /// <param name="Alpha"></param>
        /// <param name="SearchTitle"></param>
        /// <returns></returns>
        public ActionResult Grid(int startIndex = 0, int endIndex = 0, int IsBindData = 1, int currentPageIndex = 1, string orderby = "id", int IsAsc = 0, int PageSize = 100, int SearchRecords = 1, string Alpha = "", string SearchTitle = "")
        {
            IEnumerable BindData = null;
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
            catch (Exception ex)
            {
                logger.Error("PayrollAccountController - Grid - " + DateTime.Now + " - " + ex.Message.ToString());
            }

            IsArray = false;
            Arr = new string[]
            {  "IsBindData:" + IsBindData
               ,"currentPageIndex:" + currentPageIndex
               ,"orderby:" + orderby
               ,"IsAsc:" + IsAsc
               ,"PageSize:" + PageSize
               ,"SearchRecords:" + SearchRecords
               ,"Alpha:" + Alpha
               ,"SearchTitle:" + SearchTitle

            };
            #endregion

            #region MyRegion_BindData
            IEnumerable Data = null;
            try
            {


                startIndex = ((currentPageIndex - 1) * PageSize) + 1;
                endIndex = startIndex + PageSize - 1;
                int StoreId = 0;
                if (IsBindData == 1 || IsEdit == true)
                {
                    StoreId = Convert.ToInt32(Session["storeid"]);
                    BindData = _payrollAccountRepository.GetBindData(SearchTitle, StoreId).ToList();
                    TotalDataCount = BindData.OfType<PayrollAccount_Select>().ToList().Count();
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
                ViewBag.Insert = InsertMessage;
                ViewBag.Edit = Editmessage;
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

                // Add this after getting BindData and before Skip/Take
                var allData = BindData.OfType<PayrollAccount_Select>().ToList();

                // Check if any record has sorting number
                bool hasSortingNumbers = allData.Any(x => x.NewSortingNo != null && x.NewSortingNo != 0);
                ViewBag.HasSortingNumbers = hasSortingNumbers;

                // If has sorting numbers, calculate counts for each section
                if (hasSortingNumbers)
                {
                    ViewBag.ActiveWithSortingCount = allData.Count(x => x.IsActive == true && x.NewSortingNo != null && x.NewSortingNo != 0);
                    ViewBag.ActiveWithoutSortingCount = allData.Count(x => x.IsActive == true && (x.NewSortingNo == null || x.NewSortingNo == 0));
                    ViewBag.InactiveCount = allData.Count(x => x.IsActive == false);
                }

                Data = allData.Skip(startIndex - 1).Take(PageSize);
                StatusMessage = "";

                if (Session["storeid"] == null)
                {
                    Session["storeid"] = "0";
                }

                int[] ACTypeArr = new int[] { 1, 6 };
                var QbDepartmentList = _mastersBindRepository.GetDepartmentMasters(StoreId).ToList();
                string Stores = _QBRepository.GetStoreOnlineDesktop(StoreId);
                if (Stores == "Online")
                {
                    ViewBag.QBAccount = (QbDepartmentList.Select(s => new ddllist
                    {
                        Value = s.DepartmentId.ToString(),
                        Text = s.DepartmentName,
                        selectedvalue = s.DepartmentId,
                        ListID = s.ListId
                    }).ToList()).Where(a => !a.ListID.Contains("-"))
                    .OrderBy(a => a.Text)
                    .ToList();
                }
                else if (Stores == "Desktop")
                {
                    ViewBag.QBAccount = (QbDepartmentList.Select(s => new ddllist
                    {
                        Value = s.DepartmentId.ToString(),
                        Text = s.DepartmentName,
                        selectedvalue = s.DepartmentId,
                        ListID = s.ListId
                    }).ToList()).Where(a => !a.ListID.Contains("-"))
                    .OrderBy(a => a.Text)
                    .ToList();
                }

                List<ddllist> ddlValueIn = new List<ddllist>();
                ddlValueIn.Add(new ddllist() { Value = "1", Text = "Plus (+)", selectedvalue = 1 });
                ddlValueIn.Add(new ddllist() { Value = "2", Text = "Minus (-)", selectedvalue = 2 });
                ViewBag.ValueInList = ddlValueIn;
            }
            catch (Exception ex)
            {
                logger.Error("PayrollAccountController - Grid - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return View(Data);
            #endregion
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
        /// This method is Update Payroll Account.
        /// </summary>
        /// <param name="updatePRAccount"></param>
        /// <returns></returns>
        public JsonResult UpdatePayrollAccount(UpdatePRAccount updatePRAccount)
        {

            string message = "";
            try
            {
                _payrollAccountRepository.UpdatePRAccount(updatePRAccount);
                //new PayrollCashAnalysisBAL().PayrollCashAnalysisUpdate(objPayDept);
                message = "Edit";
            }
            catch (Exception ex)
            {
                logger.Error("PayrollAccountController - UpdatePayrollAccount - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return Json(message, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// This method is get Payroll bank Account.
        /// </summary>
        /// <returns></returns>
        public ActionResult PayrollBankAccount()
        {
            IEnumerable RtnData = null;
            try
            {
                int storeid = 0;
                storeid = Convert.ToInt32(Session["storeid"]);

                //Db class is used for Get DepartmentMasters with selection.
                ViewBag.DepartmentList = _mastersBindRepository.GetDepartmentMasters(storeid).Select(s => new ddllist
                {
                    Value = s.DepartmentId.ToString(),
                    Text = s.DepartmentName,
                    Store = s.StoreId.ToString()
                });
                //Db class is used for Get Vendor master.

                ViewBag.VendorList = (_mastersBindRepository.GetVendorMaster(storeid).Where(s=>s.ListId!=null).OrderBy(o => o.VendorName)
                                      .Select(s => new ddllist
                                      {
                                          Value = s.VendorId.ToString(),
                                          Text = s.VendorName,
                                          ListID = s.ListId
                                      }).ToList()).Where(a => !a.ListID.Contains("-")).ToList();

                if (storeid > 0)
                {
                    CheckBankAccount_Exist(storeid);
                }
                //This Db class is used for get all payroll bank accounts with selection.
                RtnData = _payrollAccountRepository.GetPayrollBankAccounts(storeid).Select(s => new Payroll_Setting_Select
                {
                    id = s.PayrollBankAccountId,
                    BankAccountID = (s.BankAccountId == null ? 0 : s.BankAccountId.Value),
                    BankAccountName = s.DepartmentMasters == null ? "" : s.DepartmentMasters.DepartmentName.ToString(),
                    VendorID = (s.VendorId == null ? 0 : s.VendorId.Value),
                    VendorName = s.VendorMasters == null ? "" : s.VendorMasters.VendorName.ToString(),
                }).ToList();

            }
            catch (Exception ex) {
                logger.Error("PayrollAccountController - PayrollBankAccount - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            ViewBag.StatusMessage = "";
            return View(RtnData);
        }

        /// <summary>
        /// This class is used to check back account Exits.
        /// </summary>
        /// <param name="StoreID"></param>
        public void CheckBankAccount_Exist(int StoreID)
        {
            //This Db class is used for get all payroll bank accounts.
            _payrollAccountRepository.GetPayrollBankAccounts(StoreID);
        }
        /// <summary>
        /// This method is used to update back account setting.
        /// </summary>
        /// <param name="updateBankAccount_Setting"></param>
        /// <param name="storeid"></param>
        /// <returns></returns>
        public JsonResult UpdateBankAccount_Setting(UpdateBankAccount_Setting updateBankAccount_Setting, int storeid)
        {
            string message = "";
            try
            {
                //Db class is used for Update Payroll bank Account.
                if (updateBankAccount_Setting.ID != 0)
                {
                    message = "Edit";
                }
                else
                {
                    message = "Save";
                }
                storeid = Convert.ToInt32(Session["storeid"]);
                try
                {
                    _payrollAccountRepository.UpdatePayrollBankAccounts(updateBankAccount_Setting, storeid);
                }
                catch (Exception ex)
                {
                    logger.Error("PayrollAccount:UpdateBankAccount_Setting :" + ex.Message.ToString());
                }

            }
            catch (Exception ex)
            {
                logger.Error("PayrollAccountController - UpdateBankAccount_Setting - " + DateTime.Now + " - " + ex.Message.ToString());
            }
          
            return Json(message, JsonRequestBehavior.AllowGet);
        }
    }
}