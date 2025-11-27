using EntityModels.Models;
using NLog;
using Repository;
using Repository.IRepository;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using Utility;
namespace SysnthesisRepo.Controllers
{
    public class CompaniesController : Controller
    {
        private readonly ICompaniesRepository _CompaniesRepository;
        private readonly ICommonRepository _CommonRepository;
        private readonly IUserActivityLogRepository _ActivityLogRepository;
        Logger logger = LogManager.GetCurrentClassLogger();
        #region Common static variables
        protected static string InsertMessage = "";
        protected static string DeleteMessage = "";
        protected static string Editmessage = "";
        protected static string ActivityLogMessage = "";

        protected static Array Arr;
        protected static bool IsArray;
        protected static IEnumerable BindData;
        protected static IQueryable BindQData;
        protected static bool IsEdit = false;
        protected static string StatusMessage = "";
        protected static int TotalDataCount;
        protected static string StatusMessageString = "";
        #endregion

        public CompaniesController()
        {
            this._CompaniesRepository = new CompaniesRepository(new DBContext());
            this._CommonRepository = new CommonRepository(new DBContext());
            this._ActivityLogRepository = new UserActivityLogRepository(new DBContext());
        }
        // GET: StoreMasters
        /// <summary>
        /// This method is Get store master by ID.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<ActionResult> Details(int id)
        {
            //This  class is Get Store by id.
            StoreMaster storemaster = _CompaniesRepository.GetStoreMastersbyID(id);
            try
            {
                if (storemaster == null)
                {
                    return HttpNotFound();
                }
            }
            catch (Exception ex)
            {
                logger.Error("StoreMastersController - Details - " + DateTime.Now + " - " + ex.Message.ToString());
            }
          
            return View(storemaster);
        }
        // GET: StoreMasters
        /// <summary>
        /// This method is return for Store.
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> Index()
        {
            ViewBag.Title = "Companies - Synthesis";

            ViewBag.StatusMessage = StatusMessage;
            try
            {
                logger.Info("CompaniesController - Index - " + DateTime.Now);

                // Get all groups from repository
                var groupList = _CompaniesRepository.GetGroupMasters();
                if (groupList == null || !groupList.Any())
                {
                    groupList = new List<GroupMaster>();
                }

                // Create SelectList with "All Groups" option
                var groupDropdownList = groupList.Select(g => new SelectListItem
                {
                    Value = g.GroupId.ToString(),
                    Text = g.Name
                }).ToList();

                // Add "All Groups" option at the beginning
                groupDropdownList.Insert(0, new SelectListItem { Value = "0", Text = "All Groups" });

                ViewBag.GroupList = groupDropdownList;
            }
            catch (Exception ex)
            {
                logger.Error("CompaniesController - Index - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return View();
        }
        //[Authorize(Roles ="Admin")]
        /// <summary>
        /// This Grid method return Store list.
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
        public ActionResult Grid(int IsBindData = 1, int currentPageIndex = 1, string orderby = "StoreId", int IsAsc = 0, int PageSize = 100, int SearchRecords = 1, string Alpha = "", string SearchTitle = "", int? GroupId = null)
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
            catch(Exception ex)
            {
                logger.Error("StoreMastersController - Grid - " + DateTime.Now + " - " + ex.Message.ToString());
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
                    //This  class is Get All Store data.
                    // Apply GroupId filter
                    int? filterGroupId = (GroupId.HasValue && GroupId.Value > 0) ? GroupId : null;

                    BindData = _CompaniesRepository.GetAllStoreMasters(filterGroupId).Select(s => new { StoreId = s.StoreId, StoreNo = s.StoreNo, Group = s.GroupMasters.Name, Address1 = s.Address1, FaxNo = s.FaxNo, NickName = s.NickName, EmailId = s.EmailId, Name = s.Name }).ToList().Select(s => new StoreMaster { StoreId = s.StoreId, Name = s.Name, StoreNo = s.StoreNo, Group = s.Group, Address1 = s.Address1, FaxNo = s.FaxNo, NickName = s.NickName, EmailId = s.EmailId });
                    if (SearchTitle != "")
                    {
                        BindData = BindData.OfType<StoreMaster>().ToList().Where(a => a.Group.ToString().ToLower().Contains(SearchTitle.ToLower()) || a.Name.ToString().ToLower().Contains(SearchTitle.ToLower()) ||
                        a.StoreNo.ToString().ToLower().Contains(SearchTitle.ToLower()) || a.EmailId.ToString().ToLower().Contains(SearchTitle.ToLower()) || a.NickName.ToString().ToLower().Contains(SearchTitle.ToLower())).ToList();
                    }
                    TotalDataCount = BindData.OfType<StoreMaster>().ToList().Count();
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
                ViewBag.StatusMessageString = StatusMessageString;
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
                var ColumnName = typeof(StoreMaster).GetProperties().Where(p => p.Name == orderby).FirstOrDefault();

                if (IsAsc == 1)
                {
                    ViewBag.AscVal = 0;
                    Data = BindData.OfType<StoreMaster>().ToList().OrderBy(n => ColumnName.GetValue(n, null)).Skip(startIndex - 1).Take(PageSize);
                }
                else
                {
                    ViewBag.AscVal = 1;
                    Data = BindData.OfType<StoreMaster>().ToList().OrderByDescending(n => ColumnName.GetValue(n, null)).Skip(startIndex - 1).Take(PageSize);
                }
                StatusMessage = "";
                StatusMessageString = "";

            }
            catch (Exception ex)
            {
                logger.Error("StoreMastersController - Grid - " + DateTime.Now + " - " + ex.Message.ToString());
            }
           ViewBag.Deletesucc = _CommonRepository.GetMessageValue("SMD", "Store Deleted successfully..");
            return View(Data);
            #endregion
        }

        // GET: StoreMasters/Create
        /// <summary>
        /// This method return create view.
        /// </summary>
        /// <returns></returns>
        public ActionResult Create()
        {
            ViewBag.StatusMessage = StatusMessage;
            ViewBag.Insert = InsertMessage;
            StatusMessage = "";
            ViewBag.StatusMessageString = StatusMessageString;
            ViewBag.GroupId = new SelectList(_CompaniesRepository.GetGroupMasters(), "GroupId", "Name");
            //Get All State.
            ViewBag.StateId = new SelectList(_CompaniesRepository.GetStateMasters(), "StateId", "StateName");
            return View();
        }

        // POST: StoreMasters/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(StoreMaster storeMaster)
        {
            if (storeMaster.NickName == null || storeMaster.NickName == "")
            {
                ModelState.AddModelError("NickName", " ");
            }
            if (ModelState.IsValid)
            {
                try
                {
                    if (CheckExist(storeMaster))
                    {
                        var UserName = System.Web.HttpContext.Current.User.Identity.Name;
                        //This  class is Get user id.
                        storeMaster.CreatedBy = _CommonRepository.getUserId(UserName);
                        storeMaster.CreatedOn = DateTime.Now;
                        //This  class is Add Store details.
                        _CompaniesRepository.AddStoreMasters(storeMaster);

                        ActivityLog ActLog = new ActivityLog();
                        ActLog.Action = 1;
                        //This Db class is used for get user firstname.
                        ActLog.Comment = "Store " + "<a href='/StoreMasters/Details/" + storeMaster.StoreId + "'>" + storeMaster.Name + "</a> Created by " + _CommonRepository.getUserFirstName(UserName) + " on " + AdminSiteConfiguration.GetDate(AdminSiteConfiguration.GetEasternTime(DateTime.Now).ToString());
                        _ActivityLogRepository.ActivityLogInsert(ActLog);
                        //This  class is used for Activity Log Insert.
                        //StatusMessageString = "Store created successfully..";
                        StatusMessageString = _CommonRepository.GetMessageValue("SMC", "Store created successfully..");
                        ViewBag.StatusMessageString = StatusMessageString;
                        return RedirectToAction("Index");
                    }
                }
                catch (Exception ex)
                {
                    logger.Error("StoreMastersController - Create - " + DateTime.Now + " - " + ex.Message.ToString());
                }
                
            }
            else
            {
                ViewBag.StatusMessage = "Error1";
            }
            //This  class is Get All group Master.
            ViewBag.GroupId = new SelectList(_CompaniesRepository.GetGroupMasters(), "GroupId", "Name", storeMaster.GroupId);
            //Get all State.
            ViewBag.StateId = new SelectList(_CompaniesRepository.GetStateMasters(), "StateId", "StateName");
            return View("Create");
        }

        // GET: StoreMasters/Edit/5
        /// <summary>
        /// This method is get store details with Id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<ActionResult> Edit(int id = 0)
        {
            StatusMessageString = "";
            if (id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            // //This  class is get store by id.
            StoreMaster storeMaster = _CompaniesRepository.GetStoreMastersbyID(id);
            try
            {
                if (storeMaster == null)
                {
                    return HttpNotFound();
                }
                IsArray = true;
                ViewBag.StatusMessage = StatusMessage;
                ViewBag.Edit = Editmessage;
                ViewBag.StatusMessageString = StatusMessageString;
                //This  class is Get All group Master.
                ViewBag.GroupId = new SelectList(_CompaniesRepository.GetGroupMasters(), "GroupId", "Name", storeMaster.GroupId);
                //Get all State.
                ViewBag.StateId = new SelectList(_CompaniesRepository.GetStateMasters(), "StateId", "StateName", storeMaster.StateID);
            }
            catch (Exception ex)
            {
                logger.Error("StoreMastersController - Edit - " + DateTime.Now + " - " + ex.Message.ToString());
            }
           
            return View(storeMaster);
        }

        // POST: StoreMasters/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        /// <summary>
        /// This method is Update store master details.
        /// </summary>
        /// <param name="storeMaster"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(StoreMaster storeMaster)
        {
            if (storeMaster.NickName == null || storeMaster.NickName == "")
            {
                ModelState.AddModelError("NickName", " ");
            }
            if (ModelState.IsValid)
            {
                try
                {
                    if (CheckExist(storeMaster))
                    {
                        //This  class is update store details.
                        var UserName = System.Web.HttpContext.Current.User.Identity.Name;
                        int UserId = _CommonRepository.getUserId(UserName);
                        storeMaster.ModifiedBy = UserId;
                        storeMaster.ModifiedOn = DateTime.Today;
                        _CompaniesRepository.UpdatestoreMaster(storeMaster);                        
                        ActivityLog ActLog = new ActivityLog();
                        ActLog.Action = 2;
                        //This Db class is used for get user firstname.
                        ActLog.Comment = "Store " + "<a href='/StoreMasters/Details/" + storeMaster.StoreId + "'>" + storeMaster.Name + "</a> Edited by " + _CommonRepository.getUserFirstName(UserName) + " on " + AdminSiteConfiguration.GetDate(AdminSiteConfiguration.GetEasternTime(DateTime.Now).ToString());
                        //This  class is used for Activity Log Insert.
                        _ActivityLogRepository.ActivityLogInsert(ActLog);

                        //StatusMessageString = "Store updated successfully..";
                        StatusMessageString = _CommonRepository.GetMessageValue("SME", "Store Details Updated Successfully..");
                        ViewBag.StatusMessageString = StatusMessageString;
                        return RedirectToAction("Index");
                    }
                }
                catch (Exception ex)
                {
                    logger.Error("StoreMastersController - Edit - " + DateTime.Now + " - " + ex.Message.ToString());
                }
              
            }
            //This  class is Get All group Master.
            ViewBag.GroupId = new SelectList(_CompaniesRepository.GetGroupMasters(), "GroupId", "Name", storeMaster.GroupId);
            //Get all State.
            ViewBag.StateId = new SelectList(_CompaniesRepository.GetStateMasters(), "StateId", "StateName");
            return View();
        }

        // POST: StoreMasters/Delete/5
        /// <summary>
        /// This method is Delete store details.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult DeleteConfirmed(int id)
        {
            StatusMessageString = "";
            StatusMessage = "Error";
            string name = "";

            try
            {
                //Delete Store Master
                _CompaniesRepository.DeleteStoreMaster(id);
                var UserName = System.Web.HttpContext.Current.User.Identity.Name;
                ActivityLog ActLog = new ActivityLog();
                ActLog.Action = 3;
                //This Db class is used for get user firstname.
                //Get store name by id.
                ActLog.Comment = "Store " + _CompaniesRepository.GetStoreName(id) + " Deleted by " + _CommonRepository.getUserFirstName(UserName) + " on " + AdminSiteConfiguration.GetDate(AdminSiteConfiguration.GetEasternTime(DateTime.Now).ToString());
                //This  class is used for Activity Log Insert.
                _ActivityLogRepository.ActivityLogInsert(ActLog);
            }
            catch (Exception ex)
            {
                logger.Error("StoreMastersController - Delete - " + DateTime.Now + " - " + ex.Message.ToString());
                string Msg = "";
                if (ex.InnerException.ToString().Contains("The DELETE statement conflicted with the REFERENCE constraint"))
                {
                    //Msg = "This store can not deleted. because it is used some where.";
                    Msg = _CommonRepository.GetMessageValue("SMD_E", "This store can not deleted. because it is used some where.");
                }
                return Json(Msg, JsonRequestBehavior.AllowGet);
            }

            StatusMessageString = _CommonRepository.GetMessageValue("SMD", "Store Deleted successfully..");
            ViewBag.StatusMessageString = StatusMessageString;

            return Json(StatusMessageString, JsonRequestBehavior.AllowGet);
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
        ///  This class is used to Check Exist for store Categories.
        /// </summary>
        /// <param name="stores"></param>
        /// <returns></returns>
        public bool CheckExist(StoreMaster stores)
        {
            bool status = false;
            try
            {
                if (stores.Name != null)
                {
                    //Get store for count.
                    if (_CompaniesRepository.GetStoreMastersforCount(stores).Count() > 0)
                    {
                        ModelState.AddModelError("Name", "Already Exist");
                        status = false;
                    }
                    else
                    {
                        status = true;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("StoreMastersController - CheckExist - " + DateTime.Now + " - " + ex.Message.ToString());
            }
           
            return status;
        }
    }
}