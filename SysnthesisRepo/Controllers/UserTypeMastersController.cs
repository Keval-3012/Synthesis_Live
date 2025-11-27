using EntityModels.Models;
using Repository;
using Repository.IRepository;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Utility;

namespace SysnthesisRepo.Controllers
{
    public class UserTypeMastersController : Controller
    {
        // GET: UserTypeMasters

        protected static string InsertMessage = "";
        protected static string DeleteMessage = "";
        protected static string Editmessage = "";
        protected static string ActivityLogMessage = "";

        protected static Array Arr;
        protected static bool IsArray;

        protected static bool IsEdit = false;
        protected static string StatusMessage = "";
        protected static int TotalDataCount;
        protected static string LockMessage = "";
        protected static string UnLockMessage = "";
        protected static string StatusMessageString = "";
        private readonly IUserTypeRepository _userTypeRepository;
        private readonly IUserActivityLogRepository _activityLogRepository;
        private readonly ICommonRepository _commonRepository;
        public UserTypeMastersController()
        {
            this._userTypeRepository = new UserTypeRepository(new DBContext());
            this._activityLogRepository = new UserActivityLogRepository(new DBContext());
            this._commonRepository = new CommonRepository(new DBContext());
        }
        public ActionResult Grid(int IsBindData = 1, int currentPageIndex = 1, string orderby = "UserTypeId", int IsAsc = 0, int PageSize = 50, int SearchRecords = 1, string Alpha = "", string SearchTitle = "")
        {
            IEnumerable BindData;
            IQueryable BindQData;
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
            catch { }

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
            var ColumnName = typeof(UserTypeMaster).GetProperties().Where(p => p.Name == orderby).FirstOrDefault();

            BindData = _userTypeRepository.UserTypeBindData();
            
            TotalDataCount = BindData.OfType<UserTypeMaster>().Count();
            if (SearchTitle != "")
            {
                BindData = BindData.OfType<UserTypeMaster>().ToList().Where(a => a.LevelSortName.ToString().ToLower().Contains(SearchTitle.ToLower()) || a.LevelName.ToString().ToLower().Contains(SearchTitle.ToLower()) || a.UserType.ToString().ToLower().Contains(SearchTitle.ToLower()) || a.Name.ToString().ToLower().Contains(SearchTitle.ToLower())).ToList();
                TotalDataCount = BindData.OfType<UserTypeMaster>().Count();
            }
            if (IsAsc == 1)
            {
                ViewBag.AscVal = 0;
                BindData = BindData.OfType<UserTypeMaster>().OrderBy(n => ColumnName.GetValue(n, null)).Skip(startIndex - 1).Take(PageSize).AsQueryable();

            }
            else
            {
                ViewBag.AscVal = 1;
                BindData = BindData.OfType<UserTypeMaster>().OrderByDescending(n => ColumnName.GetValue(n, null)).Skip(startIndex - 1).Take(PageSize).AsQueryable();
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
            StatusMessageString = "";
            StatusMessage = "";

            return View(BindData);
            #endregion
        }

        // GET: UserTypeMasters
        public async Task<ActionResult> Index()
        {
            return View();
        }

        // GET: UserTypeMasters/Create
        public ActionResult Create()
        {
            UserTypeMaster obj = new UserTypeMaster();
            obj.IsActive = true;
            return View(obj);
        }

        // POST: UserTypeMasters/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(UserTypeMaster userTypeMaster)
        {
            if (ModelState.IsValid)
            { 
                _userTypeRepository.SaveUserDetails(userTypeMaster);
                return RedirectToAction("Index");
            }
            return View(userTypeMaster);
        }
            
        // GET: UserTypeMasters/Edit/5
        public async Task<ActionResult> Edit(int? id)   
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserTypeMaster userTypeMaster = await _userTypeRepository.GetFindUserTypeMasters(id);
            if (userTypeMaster == null)
            {
                return HttpNotFound();
            }
            ViewBag.LevelsApproverId = new SelectList(_userTypeRepository.GetUserTypeMasters(userTypeMaster).Select(s => new LevelsApprover { LevelsApproverId = s.LevelsApproverId, LevelSortName = s.LevelSortName }).OrderBy(o => o.LevelSortName).ToList(), "LevelsApproverId", "LevelSortName", userTypeMaster.LevelsApproverId);
            return View(userTypeMaster);
        }

        // POST: UserTypeMasters/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(UserTypeMaster userTypeMaster)
        {

            if (ModelState.IsValid)
            {
                var UserName = System.Web.HttpContext.Current.User.Identity.Name;
                userTypeMaster.ModifiedOn = DateTime.Now;
                userTypeMaster.ModifiedBy = _commonRepository.getUserId(UserName);
                _userTypeRepository.UpdateUserTypeMasterDetails(userTypeMaster);
                ActivityLog ActLog = new ActivityLog();
                ActLog.Action = 2;
                ActLog.Comment = "User Type " + "<a href='/UserTypeMasters/Details/" + userTypeMaster.UserTypeId + "'>" + userTypeMaster.UserType + "</a> Edited by " + _commonRepository.getUserFirstName(UserName) + " on " + AdminSiteConfiguration.GetDate(AdminSiteConfiguration.GetEasternTime(DateTime.Now).ToString());
                _activityLogRepository.ActivityLogInsert(ActLog);

                StatusMessageString = "User Role Updated Successfully..";
                ViewBag.StatusMessageString = StatusMessageString;
                return RedirectToAction("Index");
            }
            ViewBag.LevelsApproverId = new SelectList(_userTypeRepository.GetUserTypeMasters(userTypeMaster), "LevelsApproverId", "LevelSortName", userTypeMaster.LevelsApproverId);
            return View(userTypeMaster);
        }

        // GET: UserTypeMasters/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserTypeMaster userTypeMaster = await _userTypeRepository.GetFindUserTypeMasters(id);
            if (userTypeMaster == null)
            {
                return HttpNotFound();
            }
            return View(userTypeMaster);
        }

        // POST: UserTypeMasters/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                var StatusMessageString = "";
                var UserName = System.Web.HttpContext.Current.User.Identity.Name;
                UserTypeMaster userType = _userTypeRepository.DeleteUserDetails(id);
                ActivityLog ActLog = new ActivityLog();
                ActLog.Action = 3;
                ActLog.Comment = "User Role " + userType.UserType + "</a> Deleted by " + _commonRepository.getUserFirstName(UserName) + " on " + AdminSiteConfiguration.GetDate(AdminSiteConfiguration.GetEasternTime(DateTime.Now).ToString());
                _activityLogRepository.ActivityLogInsert(ActLog);

                StatusMessageString = "User Role Deleted Successfully..";
                return Json(StatusMessageString, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                var Msg = "";
                if (ex.InnerException.ToString().Contains("The DELETE statement conflicted with the REFERENCE constraint"))
                {
                    Msg = "This user role can not deleted. because it is used some where.";
                }
                return Json(Msg, JsonRequestBehavior.AllowGet);
            }
        }

        private int getLastPageIndex(int PageSize)
        {
            int lastPageIndex = Convert.ToInt32(TotalDataCount) / PageSize;
            if (TotalDataCount % PageSize > 0)
                lastPageIndex += 1;
            return lastPageIndex;
        }
    }
}