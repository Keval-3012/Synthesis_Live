using EntityModels.HRModels;
using EntityModels.Models;
using NLog;
using Repository;
using Repository.IRepository;
using Syncfusion.EJ2.Base;
using SynthesisQBOnline.BAL;
using SynthesisQBOnline.QBClass;
using SynthesisViewModal.HRViewModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Utility;

namespace SysnthesisRepo.Controllers
{
    public class JournalEntriesController : Controller
    {
        private readonly IJournalEntriesRepository _journalEntriesRepository;
        private readonly ISynthesisApiRepository _SynthesisApiRepository;
        private readonly IQBRepository _IQBRepositoryRepository;
        private readonly ICommonRepository _ICommonRepositoryRepository;
        Logger logger = LogManager.GetCurrentClassLogger();
        public JournalEntriesController()
        {
            this._journalEntriesRepository = new JournalEntriesRepository(new DBContext());
            this._SynthesisApiRepository = new SynthesisApiRepository();
            this._IQBRepositoryRepository = new QBRepository(new DBContext());
            this._ICommonRepositoryRepository = new CommonRepository(new DBContext());
        }
        protected static string StatusMessage = "";
        private const int FirstPageIndex = 1;
        protected static Array Arr;
        protected static bool IsArray;
        protected static int TotalDataCount;
        protected static bool IsEdit = false;
        protected static IEnumerable BindData;


        // GET: Admin/GeneralEntries
        /// <summary>
        /// This method is return view of Journal Entries.
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "Administrator,ViewJournalEntries")]
        public ActionResult Index()
        {
            ViewBag.Title = "Journal Entries - Synthesis";
            _ICommonRepositoryRepository.LogEntries();      //Harsh's code
            //QBSync_PaymentType(1);
            return View();
        }
        /// <summary>
        /// This method is return grid view.
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
        [Authorize(Roles = "Administrator,ViewJournalEntries")]
        public ActionResult Grid(int IsBindData = 1, int currentPageIndex = 1, string orderby = "SalesDate", int IsAsc = 0, int PageSize = 100, int SearchRecords = 1, string Alpha = "", string SearchTitle = "")
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
            catch (Exception ex)
            {
                logger.Error("JournalEntriesController - grid - " + DateTime.Now + " - " + ex.Message.ToString());
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
            int startIndex = ((currentPageIndex - 1) * PageSize) + 1;
            int endIndex = startIndex + PageSize - 1;

            IEnumerable Data = null;
            try
            {
                if (IsBindData == 1 || IsEdit == true)
                {
                    BindData = GetData(SearchRecords, SearchTitle).OfType<SalesGeneralEntries>().ToList();
                    TotalDataCount = BindData.OfType<SalesGeneralEntries>().ToList().Count();

                }


                if (TotalDataCount == 0)
                {
                    StatusMessage = "NoItem";
                }
                if (string.IsNullOrEmpty(SearchTitle.Trim()))
                {
                    int storeid = 0;
                    DateTime? salesdate = null;
                    if (!string.IsNullOrEmpty(Convert.ToString(Session["storeid"])))
                    {
                        storeid = Convert.ToInt32(Session["storeid"]);
                        ViewBag.storeid = storeid;
                    }
                    //Get All Sales General Entries using storeid.
                    var data = _journalEntriesRepository.salesGeneralEntries(storeid);
                    if (data != null || (!string.IsNullOrEmpty(Convert.ToString(data))))
                    {
                        SearchTitle = AdminSiteConfiguration.GetMonthDateFormat(Convert.ToString(data));
                    }
                    else
                    {
                        SearchTitle = AdminSiteConfiguration.GetMonthDateFormat(Convert.ToString(AdminSiteConfiguration.GetEasternTime(DateTime.Now)));
                    }
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
                var ColumnName = typeof(SalesGeneralEntries).GetProperties().Where(p => p.Name == orderby).FirstOrDefault();

                if (IsAsc == 1)
                {
                    ViewBag.AscVal = 0;
                    Data = BindData.OfType<SalesGeneralEntries>().ToList().OrderBy(n => ColumnName.GetValue(n, null)).Skip(startIndex - 1).Take(PageSize);
                }
                else
                {

                    ViewBag.AscVal = 1;

                    Data = BindData.OfType<SalesGeneralEntries>().ToList().OrderByDescending(n => ColumnName.GetValue(n, null)).Skip(startIndex - 1).Take(PageSize);
                }
                StatusMessage = "";
                ViewBag.Delete = _ICommonRepositoryRepository.GetMessageValue("JEEDSF", "Entry deleted Successfully");
                ViewBag.Approve = _ICommonRepositoryRepository.GetMessageValue("JEAS", "Approved Successfully");
            }
            catch (Exception ex)
            {
                logger.Error("JournalEntriesController - grid - " + DateTime.Now + " - " + ex.Message.ToString());
            }

            return View(Data);
            #endregion
        }

        /// <summary>
        /// This class use to get last page index with pagesize.
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
        /// This method is get data of general Entities.
        /// </summary>
        /// <param name="SearchRecords"></param>
        /// <param name="SearchTitle"></param>
        /// <returns></returns>
        private IEnumerable GetData(int SearchRecords = 0, string SearchTitle = "")
        {
            IEnumerable RtnData = null;

            int storeid = 0;
            DateTime? salesdate = null;
            try
            {
                if (!string.IsNullOrEmpty(SearchTitle.Trim()))
                {
                    try
                    {
                        salesdate = Convert.ToDateTime(SearchTitle.Trim());
                    }
                    catch (Exception ex)
                    {
                        logger.Error("JournalEntriesController - GetData - " + DateTime.Now + " - " + ex.Message.ToString());
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(Convert.ToString(Session["storeid"])))
                    {
                        storeid = Convert.ToInt32(Session["storeid"]);
                        ViewBag.storeid = storeid;
                    }
                    //Get All Sales General Entries using storeid.
                    var data = _journalEntriesRepository.salesGeneralEntries(storeid);
                    if (data != null || (!string.IsNullOrEmpty(Convert.ToString(data))))
                    {
                        SearchTitle = Convert.ToString(data);
                        ViewBag.SearchTitle = SearchTitle;
                        salesdate = data;
                    }
                    else
                    {
                        salesdate = DateTime.Today;
                    }
                }

                if (!string.IsNullOrEmpty(Convert.ToString(Session["storeid"])))
                {
                    storeid = Convert.ToInt32(Session["storeid"]);
                    ViewBag.storeid = storeid;
                }

                var strStore = GetJournalEntry_StoreList(storeid);
                //Get All Sales General Entries Deatils.
                RtnData = _journalEntriesRepository.GetSalesGeneralEntries_Detail(strStore, salesdate);
                if (salesdate != null)
                {
                    clsActivityLog clsActivityLog = new clsActivityLog();
                    clsActivityLog.Action = "Click";
                    clsActivityLog.ModuleName = "Report";
                    clsActivityLog.PageName = "Journal Entries";
                    clsActivityLog.Message = "Journal Entries Generated for " + (salesdate == null ? "" : " Month: ") + Convert.ToDateTime(salesdate).ToString("MMM-yyyy");
                    clsActivityLog.CreatedBy = Convert.ToInt32(Session["UserID"]);
                    //Using this db class Create log.
                    _SynthesisApiRepository.CreateLog(clsActivityLog);
                }
            }
            catch (Exception ex)
            {
                logger.Error("JournalEntriesController - GetData - " + DateTime.Now + " - " + ex.Message.ToString());
            }

            return RtnData;
        }

        /// <summary>
        /// This method is Marks As Aporove Entry using id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        //public ActionResult MarkAsApprove(int Id)
        //{
        //    //Get Sales Entry By Id.
        //    SalesGeneralEntries Data = _journalEntriesRepository.SalesEntryFindById(Id);
        //    // //Using this db class get stores on Line desktop.
        //    string Store = _IQBRepositoryRepository.GetStoreOnlineDesktop(Convert.ToInt32(Data.StoreId));
        //    //Using this db class get stores on Line desktop flag.
        //    int StoreFlag = _IQBRepositoryRepository.GetStoreOnlineDesktopFlag(Convert.ToInt32(Data.StoreId));
        //    try
        //    {
        //        if (Store != "")
        //        {
        //            if (Store != "")
        //            {
        //                bool flg = false;
        //                if (Store == "Online" && StoreFlag == 1)
        //                {
        //                    if (Data.Status == null || Data.Status == 0)
        //                    {
        //                        JournalEntry MainDetail = new JournalEntry();
        //                        MainDetail.ID = Data.SalesGeneralId.ToString();
        //                        MainDetail.storeid = Convert.ToInt32(Data.StoreId);
        //                        MainDetail.userid = Convert.ToInt32(Data.UserId);
        //                        MainDetail.salesdate = Data.SalesDate;
        //                        MainDetail.Createddate = Data.CreatedDate;
        //                        MainDetail.status = Convert.ToInt32(Data.Status);
        //                        MainDetail.syncstatus = Convert.ToInt32(Data.SyncStatus);
        //                        MainDetail.totalamount = Data.TotalAmount;
        //                        MainDetail.noofpos = Convert.ToInt32(Data.NoOfPos);

        //                        List<JournalDetail> Detail = new List<JournalDetail>();
        //                        //get all general Entry details
        //                        var Det = _journalEntriesRepository.GeneralEntryDetail(Data.SalesGeneralId);
        //                        if (Det != null)
        //                        {
        //                            foreach (var item in Det)
        //                            {
        //                                JournalDetail obj = new JournalDetail();
        //                                obj.id = item.SalesChildId.ToString();
        //                                obj.Gid = Data.SalesGeneralId;
        //                                obj.storeid = Data.StoreId;
        //                                obj.groupid = Convert.ToInt32(item.GroupAccountId);
        //                                obj.Groupname = item.DepartmentName;
        //                                obj.Amount = (item.Amount < 0 ? item.Amount * -1 : item.Amount);
        //                                obj.Typeid = (item.Amount < 0 ? (Convert.ToInt32(item.TypeId) == 1 ? 2 : 1) : Convert.ToInt32(item.TypeId));
        //                                obj.Memo = item.Memo;
        //                                obj.Title = item.Title;
        //                                obj.ListID = item.DepartmentListID;
        //                                obj.EntityID = item.EntityID;
        //                                obj.EntityType = item.EntityType;
        //                                Detail.Add(obj);
        //                                obj = null;
        //                            }
        //                        }

        //                        QBResponse objResponse = new QBResponse();
        //                        //This class is Get Config Details
        //                        QBOnlineconfiguration objOnlieDetail = _IQBRepositoryRepository.GetConfigDetail(Convert.ToInt32(Data.StoreId));
        //                        QBJournalEntry.CreateJournalEntry(MainDetail, Detail, ref objResponse, objOnlieDetail);
        //                        if (objResponse.ID != "0" || objResponse.Status == "Done")
        //                        {
        //                            //Entry Sales general data.
        //                            _journalEntriesRepository.Entry_SalesgeneralEntry(Data, objResponse.ID);
        //                            AdminSiteConfiguration.WriteErrorLogs("Journal Entry Created : " + objResponse.ID);
        //                            flg = true;
        //                        }
        //                        else
        //                        {
        //                            AdminSiteConfiguration.WriteErrorLogs(objResponse.Status + ": " + objResponse.Message);
        //                        }
        //                        //Get child Other Deposite using id.
        //                        var datas = _journalEntriesRepository.GetChildOtherDeposite(Id);
        //                        if (datas != null)
        //                        {
        //                            foreach (var item in datas)
        //                            {
        //                                Deposit objDep = new Deposit();
        //                                objDep.TxnDate = item.OtherDepositDate;
        //                                objDep.Memo = item.Memo;
        //                                objDep.DepositAccID = item.BankAccountID;

        //                                List<DepositDetail> objDetailList = new List<DepositDetail>();
        //                                DepositDetail objDetail = new DepositDetail();
        //                                objDetail.Amount = item.Amount;
        //                                objDetail.Description = item.Memo;
        //                                objDetail.EntityID = item.VendorListID;
        //                                objDetail.AccountID = item.DepartmentListId;
        //                                objDetail.PaymentMethod = item.PaymentTypeID;
        //                                objDetail.EntityType = "Vendor";
        //                                objDetailList.Add(objDetail);
        //                                QBResponse objResponse1 = new QBResponse();
        //                                QBDeposit.CreateDepositeEntry(objDep, objDetailList, ref objResponse1, objOnlieDetail);
        //                                objDetailList = null;
        //                                if (objResponse1.ID != "0" || objResponse1.Status == "Done" || objResponse1.ID != "")
        //                                {
        //                                    //Sales Other Deposites by Id.
        //                                    _journalEntriesRepository.SalesOtherDepositesbyId(item.SalesOtherDepositeId, objResponse1.ID);
        //                                    flg = true;
        //                                    AdminSiteConfiguration.WriteErrorLogs("Deposite Entry Created : " + objResponse1.ID);
        //                                }
        //                                else
        //                                {
        //                                    AdminSiteConfiguration.WriteErrorLogs(objResponse.Status + ": " + objResponse1.Message);
        //                                }
        //                            }
        //                        }

        //                        if (flg == true)
        //                        {
        //                            return Json("success", JsonRequestBehavior.AllowGet);
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        logger.Error("JournalEntriesController - MarkAsApprove - " + DateTime.Now + " - " + ex.Message.ToString());
        //    }


        //    return Json("Notsuccess", JsonRequestBehavior.AllowGet);
        //}


        #region New Code Updated for Handling the Error Exception by Dani on 10-11-2025
        public ActionResult MarkAsApprove(int Id)
        {
            //Get Sales Entry By Id.
            SalesGeneralEntries Data = _journalEntriesRepository.SalesEntryFindById(Id);
            // //Using this db class get stores on Line desktop.
            string Store = _IQBRepositoryRepository.GetStoreOnlineDesktop(Convert.ToInt32(Data.StoreId));
            //Using this db class get stores on Line desktop flag.
            int StoreFlag = _IQBRepositoryRepository.GetStoreOnlineDesktopFlag(Convert.ToInt32(Data.StoreId));
            try
            {
                if (Store != "")
                {
                    if (Store != "")
                    {
                        bool flg = false;
                        if (Store == "Online" && StoreFlag == 1)
                        {
                            if (Data.Status == null || Data.Status == 0)
                            {
                                JournalEntry MainDetail = new JournalEntry();
                                MainDetail.ID = Data.SalesGeneralId.ToString();
                                MainDetail.storeid = Convert.ToInt32(Data.StoreId);
                                MainDetail.userid = Convert.ToInt32(Data.UserId);
                                MainDetail.salesdate = Data.SalesDate;
                                MainDetail.Createddate = Data.CreatedDate;
                                MainDetail.status = Convert.ToInt32(Data.Status);
                                MainDetail.syncstatus = Convert.ToInt32(Data.SyncStatus);
                                MainDetail.totalamount = Data.TotalAmount;
                                MainDetail.noofpos = Convert.ToInt32(Data.NoOfPos);

                                List<JournalDetail> Detail = new List<JournalDetail>();
                                //get all general Entry details
                                var Det = _journalEntriesRepository.GeneralEntryDetail(Data.SalesGeneralId);
                                if (Det != null)
                                {
                                    foreach (var item in Det)
                                    {
                                        JournalDetail obj = new JournalDetail();
                                        obj.id = item.SalesChildId.ToString();
                                        obj.Gid = Data.SalesGeneralId;
                                        obj.storeid = Data.StoreId;
                                        obj.groupid = Convert.ToInt32(item.GroupAccountId);
                                        obj.Groupname = item.DepartmentName;
                                        obj.Amount = (item.Amount < 0 ? item.Amount * -1 : item.Amount);
                                        obj.Typeid = (item.Amount < 0 ? (Convert.ToInt32(item.TypeId) == 1 ? 2 : 1) : Convert.ToInt32(item.TypeId));
                                        obj.Memo = item.Memo;
                                        obj.Title = item.Title;
                                        obj.ListID = item.DepartmentListID;
                                        obj.EntityID = item.EntityID;
                                        obj.EntityType = item.EntityType;
                                        Detail.Add(obj);
                                        obj = null;
                                    }
                                }

                                QBResponse objResponse = new QBResponse();
                                //This class is Get Config Details
                                QBOnlineconfiguration objOnlieDetail = _IQBRepositoryRepository.GetConfigDetail(Convert.ToInt32(Data.StoreId));
                                QBJournalEntry.CreateJournalEntry(MainDetail, Detail, ref objResponse, objOnlieDetail);

                                // ✅ ONLY UPDATE IF WE GOT VALID TXNID FROM QB
                                if (!string.IsNullOrEmpty(objResponse.ID) && objResponse.ID != "0" && objResponse.Status == "Done")
                                {
                                    //Entry Sales general data - Update Status=3 & IsSync=1
                                    _journalEntriesRepository.Entry_SalesgeneralEntry(Data, objResponse.ID);
                                    AdminSiteConfiguration.WriteErrorLogs("Journal Entry Created Successfully : " + objResponse.ID);
                                    flg = true;
                                }
                                else
                                {
                                    // ❌ QB ENTRY FAILED - Don't update status, just log error
                                    AdminSiteConfiguration.WriteErrorLogs("Journal Entry Failed - Status: " + objResponse.Status + " | Message: " + objResponse.Message);
                                    flg = false;
                                    // Status remains 0 and IsSync remains NULL in database
                                }
                                //Get child Other Deposite using id.
                                var datas = _journalEntriesRepository.GetChildOtherDeposite(Id);
                                if (datas != null)
                                {
                                    foreach (var item in datas)
                                    {
                                        Deposit objDep = new Deposit();
                                        objDep.TxnDate = item.OtherDepositDate;
                                        objDep.Memo = item.Memo;
                                        objDep.DepositAccID = item.BankAccountID;

                                        List<DepositDetail> objDetailList = new List<DepositDetail>();
                                        DepositDetail objDetail = new DepositDetail();
                                        objDetail.Amount = item.Amount;
                                        objDetail.Description = item.Memo;
                                        objDetail.EntityID = item.VendorListID;
                                        objDetail.AccountID = item.DepartmentListId;
                                        objDetail.PaymentMethod = item.PaymentTypeID;
                                        objDetail.EntityType = "Vendor";
                                        objDetailList.Add(objDetail);
                                        QBResponse objResponse1 = new QBResponse();
                                        QBDeposit.CreateDepositeEntry(objDep, objDetailList, ref objResponse1, objOnlieDetail);
                                        objDetailList = null;

                                        // ✅ ONLY UPDATE IF WE GOT VALID TXNID FROM QB
                                        if (!string.IsNullOrEmpty(objResponse1.ID) && objResponse1.ID != "0" && objResponse1.Status == "Done")
                                        {
                                            //Sales Other Deposites by Id - Update with TxnID
                                            _journalEntriesRepository.SalesOtherDepositesbyId(item.SalesOtherDepositeId, objResponse1.ID);
                                            flg = true;
                                            AdminSiteConfiguration.WriteErrorLogs("Deposit Entry Created Successfully : " + objResponse1.ID);
                                        }
                                        else
                                        {
                                            // ❌ DEPOSIT ENTRY FAILED - Don't update, just log error
                                            AdminSiteConfiguration.WriteErrorLogs("Deposit Entry Failed - Status: " + objResponse1.Status + " | Message: " + objResponse1.Message);
                                            flg = false;
                                        }
                                    }
                                }

                                if (flg == true)
                                {
                                    return Json("success", JsonRequestBehavior.AllowGet);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("JournalEntriesController - MarkAsApprove - " + DateTime.Now + " - " + ex.Message.ToString());
            }


            return Json("Notsuccess", JsonRequestBehavior.AllowGet);
        }
        #endregion

        /// <summary>
        /// This method Get details using id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Detail(int id)
        {
            GeneralEntriesDetail Detail = new GeneralEntriesDetail();
            //Get details list of general entries.
            Detail = _journalEntriesRepository.DetailList_generalEntries(id);
            ViewBag.SureApprove = _ICommonRepositoryRepository.GetMessageValue("JEA", "Are you Sure you want to Approve?");
            ViewBag.AlreadyUpdated = _ICommonRepositoryRepository.GetMessageValue("JEURA", "This Entry Is Updated, Are You Sure You Want To Re-Approve?");
            ViewBag.Approve = _ICommonRepositoryRepository.GetMessageValue("JEAS", "Approved Successfully");
            ViewBag.ReApprove = _ICommonRepositoryRepository.GetMessageValue("JERAS", "ReApproved Successfully");
            return View(Detail);
        }

        /// <summary>
        /// This methos is Add general Entries.
        /// </summary>
        /// <param name="posteddata"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Detail(GeneralEntriesDetail posteddata)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    //add details list of general entries.
                    _journalEntriesRepository.Detail_generalEntries(posteddata);
                }
            }
            catch (Exception ex)
            {
                logger.Error("JournalEntriesController - Details - " + DateTime.Now + " - " + ex.Message.ToString());
            }

            GeneralEntriesDetail Detail = new GeneralEntriesDetail();
            //Get details HttpPost.
            Detail = _journalEntriesRepository.DetailsHttpPost(posteddata);
            return RedirectToAction("Detail", Detail);
        }
        /// <summary>
        /// This method delte details using id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Delete(int id = 0)
        {
            StatusMessage = "Error";
            try
            {
                if (id > 0)
                {
                    //Delete data using Id.
                    _journalEntriesRepository.delete(id);
                    StatusMessage = "Delete";
                }
            }
            catch (Exception ex)
            {
                logger.Error("JournalEntriesController - delete - " + DateTime.Now + " - " + ex.Message.ToString());
            }

            return null;
        }

        /// <summary>
        /// This class is get QB Sync Payment Type
        /// </summary>
        /// <param name="StoreId"></param>
        public void QBSync_PaymentType(int StoreId)
        {
            try
            {
                QBResponse objResponse = new QBResponse();
                //This class is Get Config Details
                QBOnlineconfiguration objOnlieDetail = _IQBRepositoryRepository.GetConfigDetail(Convert.ToInt32(StoreId));

                List<PaymentMethod> dtPayment = SynthesisQBOnline.QBClass.QBPaymentMethod.GetPaymentMethod_All(objOnlieDetail, ref objResponse);
                if (dtPayment.Count > 0)
                {
                    //This db class is Qb Sync Payment Type.
                    _journalEntriesRepository.QBSync_PaymentType(dtPayment, StoreId);
                }
            }
            catch (Exception ex)
            {
                //This db class is Qb Sync Payment Type.
                logger.Error("JournalEntriesController - QBSync_PaymentType - " + DateTime.Now + " - " + ex.Message.ToString());
            }

        }

        /// <summary>
        /// This methos is Get all Journal Entry using storeid
        /// </summary>
        /// <param name="StoreId"></param>
        /// <returns></returns>
        private string GetJournalEntry_StoreList(int StoreId)
        {
            string strList = "";
            try
            {
                if (!Roles.IsUserInRole("Administrator"))
                {
                    if (StoreId == 0)
                    {
                        var UserName = System.Web.HttpContext.Current.User.Identity.Name;
                        //Using this db class get StoreList with role Wise.
                        var list = _ICommonRepositoryRepository.GetStoreList_RoleWise(9, "ViewJournalEntries", UserName);
                        if (list != null) { strList = String.Join(",", list); }
                        list = null;
                    }
                    else
                    {
                        strList = StoreId.ToString();
                    }
                }
                else
                {
                    strList = StoreId.ToString();
                }
            }
            catch (Exception ex)
            {
                logger.Error("JournalEntriesController - GetJournalEntry_StoreList - " + DateTime.Now + " - " + ex.Message.ToString());
            }

            return strList;
        }

        //Syncfusion grid journal entries
        public ActionResult JournalEntriesIndex()
        {
            int storeid = Convert.ToInt32(Session["storeid"]);
            ViewBag.Storeidvalue = storeid;
            return View();
        }

        public ActionResult UrlDatasource(DataManagerRequest dm,string saledate)
        {
            List<SalesGeneralEntriesSyncfusion> HrDeVm = new List<SalesGeneralEntriesSyncfusion>();
            IEnumerable<SalesGeneralEntriesSyncfusion> DataSource = new List<SalesGeneralEntriesSyncfusion>();
            int Count = 0;
            try
            {
                logger.Info("JournalEntriesController - UrlDatasource - " + DateTime.Now);

                if (string.IsNullOrEmpty(saledate))
                {
                    DateTime now = DateTime.Now;
                    saledate = now.ToString("MMM-yyyy");
                }
                DataSource = GetData(1, saledate).OfType<SalesGeneralEntriesSyncfusion>().ToList();
                //DataSource = HrDeVm;

                DataOperations operation = new DataOperations();
                if (dm.Search != null && dm.Search.Count > 0)
                {
                    string search = dm.Search[0].Key.ToString().Trim();
                    decimal searchAmount;
                    bool isNumericSearch = decimal.TryParse(search, out searchAmount);
                    if (isNumericSearch)
                    {
                        DataSource = DataSource.Where(x => x.TotalAmount == searchAmount || x.StoreName.Contains(search)).ToList();
                    }
                    else
                    {
                        DataSource = DataSource.Where(x => x.StoreName.Contains(search) || x.NoOfPos.ToString().Contains(search)).ToList();
                    }
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
                Count = DataSource.Cast<SalesGeneralEntriesSyncfusion>().Count();
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
                logger.Error("JournalEntriesController - UrlDatasource - " + DateTime.Now + " - " + ex.Message.ToString());
            }

            return dm.RequiresCounts ? Json(new { result = DataSource, count = Count }) : Json(DataSource);
        }
    }
}