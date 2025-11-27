using EntityModels.Models;
using Microsoft.Extensions.Logging;
using NLog;
using Repository;
using Repository.IRepository;
using SynthesisQBOnline;
using SynthesisViewModal;
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
    public class InvoiceApprovalLevelsController : Controller
    {
        private readonly IInvoiceApprovalLevelsRepository _levelApproversRepository;
        private readonly ICommonRepository _commonRepository;
        private readonly IUserActivityLogRepository _ActivityLogRepository;
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
        Logger logger = LogManager.GetCurrentClassLogger();

        public InvoiceApprovalLevelsController()
        {
            this._levelApproversRepository = new InvoiceApprovalLevelsRepository(new DBContext());
            this._commonRepository = new CommonRepository(new DBContext());
            this._ActivityLogRepository = new UserActivityLogRepository(new DBContext());
        }
        // GET: LevelsApprovers
        /// <summary>
        /// This method is return Level Approvers data.
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> Index()
        {
            //This db class is used to Bind data for Level Approvers.
            return View(_levelApproversRepository.GetBindData());
        }

        /// <summary>
        /// This method is return Leave Approvers data with serach filter.
        /// </summary>
        /// <param name="LevelApproversViewModal"></param>
        /// <returns></returns>
        public ActionResult Grid(LevelApproversViewModal LevelApproversViewModal)
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
                            LevelApproversViewModal.IsBindData = Convert.ToInt32(a1.Split(':')[1].ToString());
                        }
                        if (a1.Split(':')[0].ToString() == "currentPageIndex")
                        {
                            LevelApproversViewModal.currentPageIndex = Convert.ToInt32(a1.Split(':')[1].ToString());
                        }
                        if (a1.Split(':')[0].ToString() == "orderby")
                        {
                            LevelApproversViewModal.orderby = Convert.ToString(a1.Split(':')[1].ToString());
                        }
                        if (a1.Split(':')[0].ToString() == "IsAsc")
                        {
                            LevelApproversViewModal.IsAsc = Convert.ToInt32(a1.Split(':')[1].ToString());
                        }
                        if (a1.Split(':')[0].ToString() == "PageSize")
                        {
                            LevelApproversViewModal.PageSize = Convert.ToInt32(a1.Split(':')[1].ToString());
                        }
                        if (a1.Split(':')[0].ToString() == "SearchRecords")
                        {
                            LevelApproversViewModal.SearchRecords = Convert.ToInt32(a1.Split(':')[1].ToString());
                        }
                        if (a1.Split(':')[0].ToString() == "Alpha")
                        {
                            LevelApproversViewModal.Alpha = Convert.ToString(a1.Split(':')[1].ToString());
                        }
                        if (a1.Split(':')[0].ToString() == "SearchTitle")
                        {
                            LevelApproversViewModal.SearchTitle = Convert.ToString(a1.Split(':')[1].ToString());
                        }

                    }
                }
            }
            catch(Exception ex) {
                logger.Error("LevelsApproversController - grid - " + DateTime.Now + " - " + ex.Message.ToString());
            }

            IsArray = false;
            Arr = new string[]
            {  "IsBindData:" + LevelApproversViewModal.IsBindData
                ,"currentPageIndex:" + LevelApproversViewModal.currentPageIndex
                ,"orderby:" + LevelApproversViewModal.orderby
                ,"IsAsc:" + LevelApproversViewModal.IsAsc
                ,"PageSize:" + LevelApproversViewModal.PageSize
                ,"SearchRecords:" + LevelApproversViewModal.SearchRecords
                ,"Alpha:" + LevelApproversViewModal.Alpha
                ,"SearchTitle:" + LevelApproversViewModal.SearchTitle

            };
            #endregion

            #region MyRegion_BindData
            LevelApproversViewModal.startIndex = ((LevelApproversViewModal.currentPageIndex - 1) * LevelApproversViewModal.PageSize) + 1;
            LevelApproversViewModal.endIndex = LevelApproversViewModal.startIndex + LevelApproversViewModal.PageSize - 1;

            IEnumerable Data = null;
            try
            {
                if (LevelApproversViewModal.IsBindData == 1 || IsEdit == true)
                {
                    //BindQData = db.levelsApprovers.AsQueryable();
                    //BindData = BindQData.OfType<GroupMaster>().ToList();

                    //This db class is used to Bind data for Level Approvers.
                    BindData = _levelApproversRepository.GetBindData().Where(a => a.LevelName.Trim().ToLower().Contains(LevelApproversViewModal.SearchTitle == null ? "" : LevelApproversViewModal.SearchTitle.Trim().ToLower()) || (LevelApproversViewModal.SearchTitle ?? "") == "");
                    TotalDataCount = BindData.OfType<LevelsApprover>().ToList().Count();
                }


                if (TotalDataCount == 0)
                {
                    StatusMessage = "NoItem";
                }

                ViewBag.IsBindData = LevelApproversViewModal.IsBindData;
                ViewBag.CurrentPageIndex = LevelApproversViewModal.currentPageIndex;
                ViewBag.LastPageIndex = this.getLastPageIndex(LevelApproversViewModal.PageSize);
                ViewBag.OrderByVal = LevelApproversViewModal.orderby;
                ViewBag.IsAscVal = LevelApproversViewModal.IsAsc;
                ViewBag.PageSize = LevelApproversViewModal.PageSize;
                ViewBag.Alpha = LevelApproversViewModal.Alpha;
                ViewBag.SearchRecords = LevelApproversViewModal.SearchRecords;
                ViewBag.SearchTitle = LevelApproversViewModal.SearchTitle;
                ViewBag.StatusMessage = StatusMessage;
                ViewBag.StatusMessageString = StatusMessageString;
                ViewBag.Insert = InsertMessage;
                ViewBag.Edit = Editmessage;

                ViewBag.startindex = LevelApproversViewModal.startIndex;

                if (TotalDataCount < LevelApproversViewModal.endIndex)
                {
                    ViewBag.endIndex = TotalDataCount;
                }
                else
                {
                    ViewBag.endIndex = LevelApproversViewModal.endIndex;
                }
                ViewBag.TotalDataCount = TotalDataCount;
                var ColumnName = typeof(LevelsApprover).GetProperties().Where(p => p.Name == LevelApproversViewModal.orderby).FirstOrDefault();


                if (LevelApproversViewModal.IsAsc == 1)
                {
                    ViewBag.AscVal = 0;
                    Data = BindData.OfType<LevelsApprover>().ToList().OrderBy(n => ColumnName.GetValue(n, null)).Skip(LevelApproversViewModal.startIndex - 1).Take(LevelApproversViewModal.PageSize);
                }
                else
                {

                    ViewBag.AscVal = 1;

                    Data = BindData.OfType<LevelsApprover>().ToList().OrderByDescending(n => ColumnName.GetValue(n, null)).Skip(LevelApproversViewModal.startIndex - 1).Take(LevelApproversViewModal.PageSize);
                }
                StatusMessage = "";
                if (StatusMessageString != "")
                {

                }
                StatusMessageString = "";
                ViewBag.Delete = _commonRepository.GetMessageValue("LAD", "Level Approver Deleted Successfully..");
            }
            catch (Exception ex)
            {
                logger.Error("LevelsApproversController - grid - " + DateTime.Now + " - " + ex.Message.ToString());
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
        /// This class is used to Check Exist for document Categories.
        /// </summary>
        /// <param name="modelObj"></param>
        /// <returns></returns>
        public bool CheckExist(LevelsApprover modelObj)
        {
            bool status = false;
            try
            {
                if (modelObj.LevelName == null)
                {
                    ModelState.AddModelError("LevelName", "");
                    status = false;
                }
                // This db class is used to Check Exist for Level Approvers.
                if (_levelApproversRepository.CheckExist(modelObj))
                {
                    ModelState.AddModelError("LevelName", "Already Exist...");
                    return false;
                }
            }
            catch (Exception ex)
            {
                logger.Error("LevelsApproversController - CheckExist - " + DateTime.Now + " - " + ex.Message.ToString());
            }
           
            return true;
        }
        // GET: LevelsApprovers/Create
        /// <summary>
        /// This method return create view.
        /// </summary>
        /// <returns></returns>
        public ActionResult Create()
        {
            ViewBag.Title = "Invoice Approval Levels Entry - Synthesis";
            return View();
        }

        // POST: LevelsApprovers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        /// <summary>
        /// In this method Add Levels Approver for users.
        /// </summary>
        /// <param name="levelsApprover"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(LevelsApprover levelsApprover)
        {
            try
            {
                if (ModelState.IsValid) 
                {
                    if (CheckExist(levelsApprover))
                    {
                        // This db class is used to save user details for Level Approvers.
                        var UserName = System.Web.HttpContext.Current.User.Identity.Name;
                        //db class is used to get userid using  Username.
                        levelsApprover.CreatedBy = _commonRepository.getUserId(UserName);
                        levelsApprover.CreatedOn = DateTime.Now;
                        _levelApproversRepository.SaveUserDetails(levelsApprover);

                        ActivityLog ActLog = new ActivityLog();
                        ActLog.Action = 1;
                        //This Db class is used for get user firstname
                        ActLog.Comment = "Level Approver " + "<a href='/LevelsApprovers/Details/" + levelsApprover.LevelsApproverId + "'>" + levelsApprover.LevelName + "</a> created by " + _commonRepository.getUserFirstName(UserName) + " on " + Common.GetDate(Common.GetEasternTime(DateTime.Now).ToString());
                        //This DB class is used for Activity Log Insert
                        _ActivityLogRepository.ActivityLogInsert(ActLog);

                        //StatusMessageString = "Level Approver Created successfully..";
                        StatusMessageString = _commonRepository.GetMessageValue("LAC", "Level Approver Created successfully..");
                        ViewBag.StatusMessageString = StatusMessageString;
                        return RedirectToAction("Create");
                    }
                }

            }
            catch (Exception ex)
            {
                logger.Error("LevelsApproversController - Create - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            
            return View(levelsApprover);
        }

        // GET: LevelsApprovers/Edit/5
        /// <summary>
        /// This method is get level Approver Master with ID.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            // This db class is used to Get Level Approvers Master using id.
            LevelsApprover levelsApprover = await _levelApproversRepository.GetFindApproveMaster(id);
            if (levelsApprover == null)
            {
                return HttpNotFound();
            }
            return View(levelsApprover);
        }

        // POST: LevelsApprovers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        /// <summary>
        /// This method is Update level Approver Master.
        /// </summary>
        /// <param name="levelsApprover"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(LevelsApprover levelsApprover)
        {
            StatusMessageString = "";
            try
            {

                if (ModelState.IsValid)
                {
                    if (CheckExist(levelsApprover))
                    {
                        var UserName = System.Web.HttpContext.Current.User.Identity.Name;
                        levelsApprover.ModifiedBy = _commonRepository.getUserId(UserName);
                        levelsApprover.ModifiedOn = DateTime.Now;
                        // This db class is used to update user details for Level Approvers.
                        _levelApproversRepository.UpdateUserDetails(levelsApprover);
                        ActivityLog ActLog = new ActivityLog();
                        ActLog.Action = 2;
                        // This Db class is used for get user firstname
                        ActLog.Comment = "Level Approver " + "<a href='/LevelsApprovers/Details/" + levelsApprover.LevelsApproverId + "'>" + levelsApprover.LevelName + "</a> Edited by " + _commonRepository.getUserFirstName(UserName) + " on " + Common.GetDate(Common.GetEasternTime(DateTime.Now).ToString());

                        //StatusMessageString = "Level Approver Updated Successfully..";
                    StatusMessageString = _commonRepository.GetMessageValue("LAE", "Level Approver Updated Successfully..");
                        ViewBag.StatusMessageString = StatusMessageString;
                        return RedirectToAction("Create");
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("LevelsApproversController - Edit - " + DateTime.Now + " - " + ex.Message.ToString());
            }
        
            return View(levelsApprover);
        }

        // POST: LevelsApprovers/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        /// <summary>
        /// This method is delete level Approver Master.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            try
            {
                StatusMessageString = "";
                var UserName = System.Web.HttpContext.Current.User.Identity.Name;
                StatusMessage = "Error";

                string name = "";
                //This db class is used to delete user details for Level Approvers.
                LevelsApprover levelsApprover = _levelApproversRepository.DeleteUserDetails(id);

                ActivityLog ActLog = new ActivityLog();
                ActLog.Action = 3;
                //This Db class is used for get user firstname.
                ActLog.Comment = "Level Approver " + levelsApprover.LevelName + "</a> Deleted by " + _commonRepository.getUserFirstName(UserName) + " on " + Common.GetDate(Common.GetEasternTime(DateTime.Now).ToString());
                //This DB class is used for Activity Log Insert.
                _ActivityLogRepository.ActivityLogInsert(ActLog);

                StatusMessageString = _commonRepository.GetMessageValue("LAD", "Level Approver Deleted Successfully..");
                ViewBag.StatusMessageString = StatusMessageString;
                return Json(StatusMessageString, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error("LevelsApproversController - DeleteConfirmed - " + DateTime.Now + " - " + ex.Message.ToString());
                var Msg = "";
                if (ex.InnerException.ToString().Contains("The DELETE statement conflicted with the REFERENCE constraint"))
                {
                    Msg = _commonRepository.GetMessageValue("LAD_E", "This Level Approver can not deleted. because it is used some where.");
                }
                return Json(Msg, JsonRequestBehavior.AllowGet);
            }
        }
    }
}
