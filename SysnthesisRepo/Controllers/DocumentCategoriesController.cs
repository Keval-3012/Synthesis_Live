using EntityModels.Models;
using NLog;
using Repository;
using Repository.IRepository;
using SynthesisQBOnline;
using SynthesisViewModal;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Utility;

namespace SysnthesisRepo.Controllers
{
    public class DocumentCategoriesController : Controller
    {
        private readonly IDocumentCategoriesRepository _DocumentCategoriesRepository;
        private readonly ICommonRepository _CommonRepository;
        private readonly IUserActivityLogRepository _ActivityLogRepository;
        Logger logger = LogManager.GetCurrentClassLogger();
        #region Static Variables
        protected static string InsertMessage = "";
        protected static string DeleteMessage = "";
        protected static string Editmessage = "";
        protected static string ActivityLogMessage = "";

        protected static Array Arr;
        protected static bool IsArray;
        protected static IEnumerable BindData;
        protected static bool IsEdit = false;
        protected static string StatusMessage = "";
        protected static int TotalDataCount;
        protected static string StatusMessageString = "";
        #endregion
        public DocumentCategoriesController()
        {
            this._DocumentCategoriesRepository = new DocumentCategoriesRepository(new DBContext());
            this._CommonRepository = new CommonRepository(new DBContext());
            this._ActivityLogRepository = new UserActivityLogRepository(new DBContext());
        }
        // GET: DocumentCategories
        /// <summary>
        /// This Method return All The Documentcategory.
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> Index()
        {
            ViewBag.Title = "Document Categories - Synthesis";
            //db class for get all document category.
            return View(_DocumentCategoriesRepository.GetAllDocumentCategory());
        }
        /// <summary>
        /// This Method return Grid data.
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
        public ActionResult Grid(int IsBindData = 1, int currentPageIndex = 1, string orderby = "DocumentCategoryId", int IsAsc = 0, int PageSize = 100, int SearchRecords = 1, string Alpha = "", string SearchTitle = "")
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
            catch(Exception ex) {
                logger.Error("DocumentCategoriesController - Grid - " + DateTime.Now + " - "  + ex.Message.ToString());
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
                    //db class use for get all Document categoty with selection.
                    BindData = _DocumentCategoriesRepository.GetAllDocumentCategory().Select(s => new { DocumentCategoryId = s.DocumentCategoryId, Name = s.Name, IsActive = s.IsActive, DocumentCount = s.Documents.Count() }).ToList().Select(k => new DocumentCategory { DocumentCategoryId = k.DocumentCategoryId, Name = k.Name, IsActive = k.IsActive, DocumentCount = k.DocumentCount }).ToList();
                    if (SearchTitle != "")
                    {
                        BindData = BindData.OfType<DocumentCategory>().ToList().Where(a => a.Name.ToLower().Contains(SearchTitle.ToLower())).ToList();
                    }
                    TotalDataCount = BindData.OfType<DocumentCategory>().ToList().Count();

                }

                if (TotalDataCount == 0)
                {
                    StatusMessage = "NoItem";
                    StatusMessageString = "No Records Found..";
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
            ViewBag.DeleteDoc = _CommonRepository.GetMessageValue("DCD", "Document Category Deleted Successfully..");

                if (TotalDataCount < endIndex)
                {
                    ViewBag.endIndex = TotalDataCount;
                }
                else
                {
                    ViewBag.endIndex = endIndex;
                }
                ViewBag.TotalDataCount = TotalDataCount;
                var ColumnName = typeof(DocumentCategory).GetProperties().Where(p => p.Name == orderby).FirstOrDefault();


                if (IsAsc == 1)
                {
                    ViewBag.AscVal = 0;
                    Data = BindData.OfType<DocumentCategory>().ToList().OrderBy(n => ColumnName.GetValue(n, null)).Skip(startIndex - 1).Take(PageSize);
                }
                else
                {

                    ViewBag.AscVal = 1;
                    Data = BindData.OfType<DocumentCategory>().ToList().OrderByDescending(n => ColumnName.GetValue(n, null)).Skip(startIndex - 1).Take(PageSize);
                }
                StatusMessage = "";
                StatusMessageString = "";

            }
            catch (Exception ex)
            {
                logger.Error("DocumentCategoriesController - Grid - " + DateTime.Now + " - " + ex.Message.ToString());
            }
          
            return View(Data);
            #endregion
        }


        // GET: DocumentCategories/Create
        /// <summary>
        /// This Method return create view.
        /// </summary>
        /// <returns></returns>
        public ActionResult Create()
        {
            return View();
        }

        // POST: DocumentCategories/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        /// <summary>
        /// This Method Add the Documnet Category.
        /// </summary>
        /// <param name="documentCategory"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(DocumentCategory documentCategory)
        {
            StatusMessageString = "";
            try
            {
                if (ModelState.IsValid)
                {
                    if (CheckExist(documentCategory))
                    {
                        var UserName = System.Web.HttpContext.Current.User.Identity.Name;
                        //This  class use for Get Use type by Id.
                        documentCategory.CreatedBy = _CommonRepository.getUserTypeId(UserName);
                        documentCategory.CreatedOn = DateTime.Now;
                        //This db class use for set the document categoty.
                        _DocumentCategoriesRepository.AddDocumentCategory(documentCategory);

                        ActivityLog ActLog = new ActivityLog();
                        ActLog.Action = 1;
                        ActLog.Comment = "Document Category " + "<a href='/DocumentCategories/Details/" + documentCategory.DocumentCategoryId + "'>" + documentCategory.Name + "</a> created by "
                            //This Db class is used for get user firstname.
                            + _CommonRepository.getUserFirstName(UserName) + " on " + AdminSiteConfiguration.GetDate(AdminSiteConfiguration.GetEasternTime(DateTime.Now).ToString());
                        //This  class is used for Activity Log Insert.
                        _ActivityLogRepository.ActivityLogInsert(ActLog);

                        //StatusMessageString = "Document Category Created successfully..";
						StatusMessageString = _CommonRepository.GetMessageValue("DCC", "Document Category Created successfully..");

                        ViewBag.StatusMessageString = StatusMessageString;
                        return RedirectToAction("Index");
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("DocumentCategoriesController - Create - " + DateTime.Now + " - " + ex.Message.ToString());
            }
          
            return View("Create");
        }

        // GET: DocumentCategories/Edit/5
        /// <summary>
        /// This method return Edit data with Id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<ActionResult> Edit(string id)
        {
            StatusMessageString = "";
            if (id.ToString() == "")
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            // Db class use to get all the document category by id.
            DocumentCategory documentCategory = _DocumentCategoriesRepository.GetAllDocumentCategorybyID(Convert.ToInt32(id));
            if (documentCategory == null)
            {
                return HttpNotFound();
            }
            return View(documentCategory);
        }

        // POST: DocumentCategories/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        /// <summary>
        /// This method update DocumentCategory.
        /// </summary>
        /// <param name="documentCategory"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(DocumentCategory documentCategory)
        {
            StatusMessageString = "";
            try
            {
                if (ModelState.IsValid)
                {
                    if (CheckExist(documentCategory))
                    {
                        var UserName = System.Web.HttpContext.Current.User.Identity.Name;
                        //This  class use for Get Use type by Id.
                        documentCategory.ModifiedBy = _CommonRepository.getUserId(UserName);
                        documentCategory.ModifiedOn = DateTime.Now;

                        //This Db class update the document Category.
                        _DocumentCategoriesRepository.UpdateDocumentCategory(documentCategory);

                        ActivityLog ActLog = new ActivityLog();
                        ActLog.Action = 2;
                        ActLog.Comment = "Document Category " + "<a href='/DocumentCategories/Details/" + documentCategory.DocumentCategoryId + "'>" + documentCategory.Name + "</a> Edited by "
                            //This Db class is used for get user firstname
                            + _CommonRepository.getUserFirstName(UserName) + " on " + AdminSiteConfiguration.GetDate(AdminSiteConfiguration.GetEasternTime(DateTime.Now).ToString());
                        //This  class is used for Activity Log Insert
                        _ActivityLogRepository.ActivityLogInsert(ActLog);

                        //StatusMessageString = "Document  Category updated successfully..";
                        StatusMessageString = _CommonRepository.GetMessageValue("DCE", "Document  Category updated successfully..");
                        ViewBag.StatusMessageString = StatusMessageString;
                        return RedirectToAction("Index");
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("DocumentCategoriesController - Edit - " + DateTime.Now + " - " + ex.Message.ToString());
                
            }
         
            return View(documentCategory);
        }

        // POST: DocumentCategories/Delete/5
        /// <summary>
        /// This method use to delete DocumentCategory.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                StatusMessageString = "";
                StatusMessage = "Error";
                var UserName = System.Web.HttpContext.Current.User.Identity.Name;
                //db.Entry(Data).State = System.Data.Entity.EntityState.Modified;
                string name = "";
                //string name = Convert.ToString((from Data1 in db.tbl_DocumentCategory_byid(Data.Id)
                //                                select Data1.Name).FirstOrDefault());

                // Db class use to get all the document category by id.
                DocumentCategory documentCategory = _DocumentCategoriesRepository.GetAllDocumentCategorybyID(id);
                //this Db Class delete Document Category
                _DocumentCategoriesRepository.RemoveDocumentCategory(documentCategory);

                ActivityLog ActLog = new ActivityLog();
                ActLog.Action = 3;
                //This Db class is used for get user firstname
                ActLog.Comment = "Document Category " + documentCategory.Name + "</a> Deleted by " + _CommonRepository.getUserFirstName(UserName) + " on " + AdminSiteConfiguration.GetDate(AdminSiteConfiguration.GetEasternTime(DateTime.Now).ToString());
               // This Db class is used for Activity Log Insert
                 _ActivityLogRepository.ActivityLogInsert(ActLog);

                //int uid = WebSecurity.CurrentUserId;
                //string fullname = db.tbl_user.Where(x => x.userid == uid).Select(x => x.Name).FirstOrDefault();
                //ActivityLogMessage = "Category " + name + " deleted by " + fullname + " on " + wwwroot.Class.AdminSiteConfiguration.GetDate(wwwroot.Class.AdminSiteConfiguration.GetEasternTime(DateTime.Now).ToString());
                //var successActivity = db.tbl_Activity_log_Insert(userid, ActivityLogMessage, username, 3);
                StatusMessage = "Delete";
                //DeleteMessage = name + " deleted successfully.";
                DeleteMessage = _CommonRepository.GetMessageValue("DCDD", "##name## deleted successfully.").Replace("##name##",name);
                ViewBag.Delete = DeleteMessage;
                //StatusMessageString = "Document Category Deleted Successfully..";
                StatusMessageString = _CommonRepository.GetMessageValue("DCD", "Document Category Deleted Successfully..");
                ViewBag.StatusMessageString = StatusMessageString;
                return Json(StatusMessageString, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error("DocumentCategoriesController - DeleteConfirmed - " + DateTime.Now + " - " + ex.Message.ToString());
                string Msg = "";
                if (ex.InnerException.ToString().Contains("The DELETE statement conflicted with the REFERENCE constraint"))
                {
                    //Msg = "This Document Category can not deleted. because it is used in my document..";
                    Msg = _CommonRepository.GetMessageValue("DCD_E", "This Document Category can not deleted. because it is used in my document..");
                }
                return Json(Msg, JsonRequestBehavior.AllowGet);
            }
        }
        /// <summary>
        /// This method Get DocumentCategory With ID.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<ActionResult> Details(int id)
        {
            // Db class use to get all the document category by id.
            DocumentCategory documentCategory = _DocumentCategoriesRepository.GetAllDocumentCategorybyID(id);
            if (documentCategory == null)
            {
                return HttpNotFound();
            }
            return View(documentCategory);
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
        /// <param name="DocumentCategories"></param>
        /// <returns></returns>
        public bool CheckExist(DocumentCategory DocumentCategories)
        {
            bool status = false;
            try
            {
                if (DocumentCategories.Name != null)
                {
                    //db class for get all document category.
                    if (_DocumentCategoriesRepository.GetAllDocumentCategory().Where(s => s.DocumentCategoryId != DocumentCategories.DocumentCategoryId && s.Name == DocumentCategories.Name).Count() > 0)
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
                logger.Error("DocumentCategoriesController - CheckExist - " + DateTime.Now + " - " + ex.Message.ToString());
            }
           
            return status;
        }
    }
}