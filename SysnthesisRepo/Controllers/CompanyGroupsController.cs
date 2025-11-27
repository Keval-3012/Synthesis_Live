using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using EntityModels.Models;
using NLog;
using Repository;
using Repository.IRepository;
using Utility;

namespace SysnthesisRepo.Controllers
{
    public class CompanyGroupsController : Controller
    {
        private readonly ICompanyGroupsRepository _groupMastersRepository;
        private readonly ICommonRepository _CommonRepository;
        private readonly IUserActivityLogRepository _ActivityLogRepository;
        Logger logger = LogManager.GetCurrentClassLogger();
        public CompanyGroupsController()
        {
            this._groupMastersRepository = new CompanyGroupsRepository(new DBContext());
            this._CommonRepository = new CommonRepository(new DBContext());
            this._ActivityLogRepository = new UserActivityLogRepository(new DBContext());
        }

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

        // GET: GroupMasters
        /// <summary>
        /// This method return view of Company Group
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> Index()
        {
            ViewBag.Title = "Company Groups - Synthesis";
            return View();
        }
        /// <summary>
        /// This method get details of Group Master by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<ActionResult> Details(int id)
        {
            //This db class get group master by id
            GroupMaster groupmaster = _groupMastersRepository.GetGroupMasterByID(id);
            if (groupmaster == null)
            {
                return HttpNotFound();
            }
            return View(groupmaster);
        }
        /// <summary>
        /// This method return view of grid Company Group
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
        public ActionResult Grid(int IsBindData = 1, int currentPageIndex = 1, string orderby = "GroupId", int IsAsc = 0, int PageSize = 100, int SearchRecords = 1, string Alpha = "", string SearchTitle = "")
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
            catch (Exception ex) {
                logger.Error("GroupMastersController - Grid - " + DateTime.Now + " - " + ex.Message.ToString());
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
                    //Get All group Master data
                    BindQData = _groupMastersRepository.GetGroupMasterData();
                    BindData = _groupMastersRepository.GetGroupDataList().Where(a => a.Name.Trim().ToLower().Contains(SearchTitle.Trim().ToLower()) || (SearchTitle ?? "") == "");
                    TotalDataCount = BindQData.OfType<GroupMaster>().ToList().Count();

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
                ViewBag.StatusMessageString = StatusMessageString;
                ViewBag.Insert = InsertMessage;
                ViewBag.Edit = Editmessage;

                ViewBag.startindex = startIndex;
            ViewBag.DeleteGrp = _CommonRepository.GetMessageValue("GRMD", "Group Deleted Successfully..");

                if (TotalDataCount < endIndex)
                {
                    ViewBag.endIndex = TotalDataCount;
                }
                else
                {
                    ViewBag.endIndex = endIndex;
                }
                ViewBag.TotalDataCount = TotalDataCount;
                var ColumnName = typeof(GroupMaster).GetProperties().Where(p => p.Name == orderby).FirstOrDefault();

                

                if (IsAsc == 1)
                {
                    ViewBag.AscVal = 0;
                    Data = BindData.OfType<GroupMaster>().ToList().OrderBy(n => ColumnName.GetValue(n, null)).Skip(startIndex - 1).Take(PageSize);
                }
                else
                {

                    ViewBag.AscVal = 1;

                    Data = BindData.OfType<GroupMaster>().ToList().OrderByDescending(n => ColumnName.GetValue(n, null)).Skip(startIndex - 1).Take(PageSize);
                }
                StatusMessage = "";
                if (StatusMessageString != "")
                {

                }
                StatusMessageString = "";

            }
            catch (Exception ex)
            {
                logger.Error("GroupMastersController - Grid - " + DateTime.Now + " - " + ex.Message.ToString());
            }
           
            return View(Data);
            #endregion
        }

        // GET: GroupMasters/Create
        /// <summary>
        /// This method is return Create view 
        /// </summary>
        /// <returns></returns>
        public ActionResult Create()
        {
            ViewBag.Message = _CommonRepository.GetMessageValue("");
            return View();
        }

        // POST: GroupMasters/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        /// <summary>
        /// This method is Save group Master details
        /// </summary>
        /// <param name="groupMaster"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(GroupMaster groupMaster)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (CheckExist(groupMaster))
                    {
                        var UserName = System.Web.HttpContext.Current.User.Identity.Name;
                        //Db class for get user Id.
                        groupMaster.CreatedBy = _CommonRepository.getUserId(UserName);
                        groupMaster.CreatedOn = DateTime.Now;
                        //This class is save Gropu master data
                        _groupMastersRepository.SaveGroupMaster(groupMaster);

                        ActivityLog ActLog = new ActivityLog();
                        ActLog.Action = 1;
                        //Db class for get user Id.
                        ActLog.Comment = "Group " + "<a href='/GroupMasters/Details/" + groupMaster.GroupId + "'>" + groupMaster.Name + "</a> created by " + _CommonRepository.getUserId(UserName) + " on " + AdminSiteConfiguration.GetDate(AdminSiteConfiguration.GetEasternTime(DateTime.Now).ToString());
                        //This  class is used for Activity Log Insert
                        _ActivityLogRepository.ActivityLogInsert(ActLog);

                        StatusMessageString = "Group Created successfully..";
                        ViewBag.StatusMessageString = StatusMessageString;
                        return RedirectToAction("Index");
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("GroupMastersController - Create - " + DateTime.Now + " - " + ex.Message.ToString());
            }
        

            return View("Create");
        }

        // GET: GroupMasters/Edit/5
        /// <summary>
        /// This method is get Group master by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<ActionResult> Edit(int? id)
        {
            StatusMessageString = "";
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //Get Group master by ID
            GroupMaster groupMaster = _groupMastersRepository.GetGroupMasterByID((int)id);
            if (groupMaster == null)
            {
                return HttpNotFound();
            }
            return View(groupMaster);
        }

        // POST: GroupMasters/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        /// <summary>
        /// This method is update Group master data
        /// </summary>
        /// <param name="groupMaster"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(GroupMaster groupMaster)
        {
            StatusMessageString = "";
            try
            {
                if (ModelState.IsValid)
                {
                    if (CheckExist(groupMaster))
                    {
                        var UserName = System.Web.HttpContext.Current.User.Identity.Name;
                        //Db class for get user Id.
                        groupMaster.ModifiedBy = _CommonRepository.getUserId(UserName);
                        groupMaster.ModifiedOn = DateTime.Now;
                        //Update group master data
                        _groupMastersRepository.UpdateGroupMaster(groupMaster);

                        ActivityLog ActLog = new ActivityLog();
                        ActLog.Action = 2;
                        //Db class for get user Id.
                        ActLog.Comment = "Group " + "<a href='/GroupMasters/Details/" + groupMaster.GroupId + "'>" + groupMaster.Name + "</a> Edited by " + _CommonRepository.getUserId(UserName) + " on " + AdminSiteConfiguration.GetDate(AdminSiteConfiguration.GetEasternTime(DateTime.Now).ToString());
                        //This  class is used for Activity Log Insert
                        _ActivityLogRepository.ActivityLogInsert(ActLog);

                        StatusMessageString = "Group Updated Successfully..";
                        ViewBag.StatusMessageString = StatusMessageString;
                        return RedirectToAction("Index");
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("GroupMastersController - Edit - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            
            return View(groupMaster);
        }

        /// <summary>
        /// This method is delete group master
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                var UserName = System.Web.HttpContext.Current.User.Identity.Name;
                StatusMessageString = "";
                StatusMessage = "Error";

                string name = "";
                //this db class is delete Group master data
                _groupMastersRepository.DeleteGroupMaster(id);

                ActivityLog ActLog = new ActivityLog();
                ActLog.Action = 3;
                //Db class for get user Id.
                ActLog.Comment = "Group " + _groupMastersRepository.GetGroupName(id) + "</a> Deleted by " + _CommonRepository.getUserId(UserName) + " on " + AdminSiteConfiguration.GetDate(AdminSiteConfiguration.GetEasternTime(DateTime.Now).ToString());

                //This  class is used for Activity Log Insert
                _ActivityLogRepository.ActivityLogInsert(ActLog);

                StatusMessage = "Delete";
                DeleteMessage = name + " deleted successfully.";
                ViewBag.Delete = DeleteMessage;
                StatusMessageString = "Group Deleted Successfully..";
                return Json(StatusMessageString, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error("GroupMastersController - DeleteConfirmed - " + DateTime.Now + " - " + ex.Message.ToString());
                var Msg = "";
                if (ex.InnerException.ToString().Contains("The DELETE statement conflicted with the REFERENCE constraint"))
                {
                    Msg = "This group can not deleted. because it is used some where.";
                }
                return Json(Msg, JsonRequestBehavior.AllowGet);
            }

            //return null;
        }

        /// <summary>
        /// This class is used to get last page index for totaldatacount.
        /// </summary>
        /// <param name="PageSize"></param>
        /// <returns></returns>
        private int getLastPageIndex(int PageSize)
        {
            int lastPageIndex = Convert.ToInt32(TotalDataCount) / PageSize;
            try
            {
                if (TotalDataCount % PageSize > 0)
                    lastPageIndex += 1;
            }
            catch (Exception ex)
            {
                logger.Error("GroupMastersController - getLastPageIndex - " + DateTime.Now + " - " + ex.Message.ToString());
            }
          
            return lastPageIndex;
        }

        /// <summary>
        /// This class is used to Check if exist or not
        /// </summary>
        /// <param name="groups"></param>
        /// <returns></returns>
        public bool CheckExist(GroupMaster groups)
        {
            bool status = false;
            try
            {
                if (groups.Name != null)
                {
                    //Get group Data for Count
                    if (_groupMastersRepository.GetGroupDataforCount(groups).Count() > 0)
                    {
                    	                    ModelState.AddModelError("Name", _CommonRepository.GetMessageValue("CGAE", "Already Exist"));

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
                logger.Error("GroupMastersController - CheckExist - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            
            return status;
        }
    }
}