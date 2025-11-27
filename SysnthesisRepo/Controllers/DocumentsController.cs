using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Collections;
using System.Web.Security;
using EntityModels.Models;
using SynthesisQBOnline;
using Utility;
using NLog;
using Repository.IRepository;
using SynthesisViewModal;
using Repository;
using Aspose.Pdf.Operators;
using Syncfusion.EJ2.Base;

namespace SynthesisRepo.Controllers
{
    public class DocumentsController : Controller
    {
        private DBContext db = new DBContext();
        private readonly IDocumentsRepository _documentsRepository;
        Logger logger = LogManager.GetCurrentClassLogger();
        private readonly ICommonRepository _commonRepository;
        private readonly IUserActivityLogRepository _activityLogRepository;
        private readonly ISynthesisApiRepository _synthesisApiRepository;
        DocumentsViewModal documentsViewModal = new DocumentsViewModal();
        public DocumentsController()
        {
            this._commonRepository = new CommonRepository(new DBContext());
            this._activityLogRepository = new UserActivityLogRepository(new DBContext());
            this._synthesisApiRepository = new SynthesisApiRepository();
            this._documentsRepository = new DocumentsRepository(new DBContext());
        }
        /// <summary>
        /// This method return View of Document.
        /// </summary>
        /// <param name="dashbordsuccess"></param>
        /// <returns></returns>
        [Authorize(Roles = "Administrator,ViewDocument")]
        // GET: Documents
        public async Task<ActionResult> Index(string dashbordsuccess)
        {
            ViewBag.Title = "My Documents - Synthesis";
            _commonRepository.LogEntries();     //Harsh's code
            try
            {
                var UserName = System.Web.HttpContext.Current.User.Identity.Name;
                //Db class is  get user Id by username.
                int userid = Convert.ToInt32(_commonRepository.getUserId(UserName));
                int StoreId = 0;
                if (TempData["msg"] != null)
                {
                    ViewBag.StatusMessage = TempData["msg"].ToString();
                    documentsViewModal.strdashbordsuccess = TempData["msg"].ToString();
                }
                ViewBag.CloudURL = documentsViewModal.Cloudurl;
                if (Session["storeid"] == null)
                {
                    Session["storeid"] = "0";
                }
                if (Convert.ToString(Session["storeid"]) != "0" && Convert.ToString(Session["storeid"]) != "")
                {
                    string store_idval = Session["storeid"].ToString();
                    ViewBag.Storeidvalue = store_idval;
                    StoreId = Convert.ToInt16(Session["storeid"]);
                }
                else
                {
                    string store_idval = Session["storeid"].ToString();
                    ViewBag.Storeidvalue = store_idval;
                    StoreId = Convert.ToInt16(Session["storeid"]);
                }
            }
            catch (Exception ex)
            {
                logger.Error("DocumentsController - Index - " + DateTime.Now + " - " + ex.Message.ToString());
            }

            return View();
        }
        /// <summary>
        /// This methos is grid of list data.
        /// </summary>
        /// <param name="IsBindData"></param>
        /// <param name="currentPageIndex"></param>
        /// <param name="orderby"></param>
        /// <param name="IsAsc"></param>
        /// <param name="PageSize"></param>
        /// <param name="SearchRecords"></param>
        /// <param name="Alpha"></param>
        /// <param name="SearchTitle"></param>
        /// <param name="CategoryId"></param>
        /// <param name="startdate"></param>
        /// <param name="enddate"></param>
        /// <param name="chkImages"></param>
        /// <param name="chkEmail"></param>
        /// <param name="chkDoc"></param>
        /// <param name="chkSheet"></param>
        /// <param name="chkOther"></param>
        /// <param name="IsPrivate"></param>
        /// <param name="IsFavorite"></param>
        /// <param name="tabListing"></param>
        /// <returns></returns>
        public ActionResult Grid(int IsBindData = 1, int currentPageIndex = 1, string orderby = "DocumentId", int IsAsc = 0, int PageSize = 20000, int SearchRecords = 1, string Alpha = "", string SearchTitle = "", int CategoryId = 0, string startdate = "", string enddate = "", bool chkImages = false, bool chkEmail = false, bool chkDoc = false, bool chkSheet = false, bool chkOther = false, bool IsPrivate = false, bool IsFavorite = false, int tabListing = 0)
        {
            List<Document> BindData = new List<Document>();
            DateTime d = AdminSiteConfiguration.GetEasternTime(DateTime.Now);
            DateTime? stDate = null;
            DateTime? enDate = null;

            try
            {
                if (!string.IsNullOrEmpty(startdate))
                    stDate = DateTime.ParseExact(startdate, "MM/dd/yyyy", System.Globalization.CultureInfo.InvariantCulture);
            }
            catch (Exception ex)
            {
                logger.Error("DocumentsController - Grid(startdate) - " + DateTime.Now + " - " + ex.Message.ToString());
            }

            try
            {
                if (!string.IsNullOrEmpty(enddate))
                    enDate = DateTime.ParseExact(enddate, "MM/dd/yyyy", System.Globalization.CultureInfo.InvariantCulture);
            }
            catch (Exception ex)
            {
                logger.Error("DocumentsController - Grid(enddate) - " + DateTime.Now + " - " + ex.Message.ToString());
            }

            #region QueryText
            //,.eml,.msg,.rtf,.csv,
            string strFileType = ",.pdf,.txt,.doc,.docx,.jpg,.jpeg,.gif,.png,.xls,.xlsx,.mp4,.avi,.wmv,.mov,.mkv";
            if (chkDoc == true || chkImages == true || chkSheet == true || chkEmail == true || chkOther == true)
            {
                strFileType = "";

                if (chkDoc == true)
                {
                    strFileType += ",.doc,.docx,.pdf";
                }
                if (chkImages == true)
                {
                    strFileType += ",.jpg,.jpeg,.gif,.png";
                }
                if (chkSheet == true)
                {
                    strFileType += ",.xls,.xlsx,.csv";
                }
                if (chkEmail == true)
                {
                    strFileType += ",.eml,.msg";
                }
                if (chkOther == true)
                {
                    strFileType += ",.txt,.rtf";
                }

            }


            if (strFileType.Length > 0)
            {
                strFileType.Remove(0, 1);
            }

            #endregion

            List<Document> docGridList = new List<Document>();
            List<Document> PrivateGridList = new List<Document>();
            int StoreId = 0;
            var UserName = System.Web.HttpContext.Current.User.Identity.Name;
            //Db class for get user Id using username.
            var UserId = Convert.ToInt32(_commonRepository.getUserId(UserName));
            Boolean isadmin = Roles.IsUserInRole("Administrator");
            try
            {
                //Db class for get user Id using username.
                int CurrentUserId = Convert.ToInt32(_commonRepository.getUserId(UserName));

                if ((Convert.ToString(Session["storeid"]) != "0"))
                {
                    StoreId = Convert.ToInt16(Session["storeid"]);
                }

                ViewBag.storeid = StoreId;

                int startIndex = ((currentPageIndex - 1) * PageSize) + 1;
                int endIndex = startIndex + PageSize - 1;
                //This db class is get Document List.
                var documentlist = _documentsRepository.DocumentsList();
                if (SearchTitle != "")
                {
                    docGridList = documentlist.Where(s => s.IsDelete == false && s.StoreId == StoreId && s.IsPrivate == false && (s.Title.Trim().ToLower().Contains(SearchTitle.Trim().ToLower()) || s.DocumentKeywords.Select(k => k.Title.Trim().ToLower()).Contains(SearchTitle.Trim().ToLower()))).Select(s => new
                    {
                        DocumentId = s.DocumentId,
                        Type = "Doc",
                        Title = s.Title,
                        FilePath = s.FilePath,
                        AttachFile = s.AttachFile + s.AttachExtention,
                        AttachLink = "",
                        StoreName = s.StoreMasters.NickName,
                        CategoryName = s.DocumentCategories.Name,
                        CreatedOn = s.CreatedOn,
                        Notes = s.Notes,
                        Description = s.Description,
                        IsFavorite = (s.DocumentFavorites.Where(k => k.IsFavorite == true && (isadmin == true ? (k.UserId == k.UserId) : k.UserId == UserId)).Count() > 0 ? true : false),
                        IsDelete = s.IsDelete,
                        AttachExtention = s.AttachExtention,
                        IsStatus_id = s.CreatedBy,
                        FavId = s.DocumentFavorites.Where(k => k.IsFavorite == true && (isadmin == true ? (k.UserId == k.UserId) : k.UserId == UserId)).Count() > 0 ? s.DocumentFavorites.Where(k => k.IsFavorite == true && (isadmin == true ? (k.UserId == k.UserId) : k.UserId == UserId)).FirstOrDefault().DocumentFavoriteId : 0,
                        IsPrivate = s.IsPrivate,
                        DocumentCategoryId = s.DocumentCategoryId,
                        FavUserId = s.DocumentFavorites.Where(k => k.IsFavorite == true && (isadmin == true ? (k.UserId == k.UserId) : k.UserId == UserId)).Count() > 0 ? s.DocumentFavorites.Where(k => k.IsFavorite == true && (isadmin == true ? (k.UserId == k.UserId) : k.UserId == UserId)).FirstOrDefault().UserId : 0,
                    }).ToList()
              .Select(s => new Document
              {
                  DocumentId = s.DocumentId,
                  Type = s.Type,
                  Title = s.Title,
                  FilePath = s.FilePath,
                  AttachFile = s.AttachFile,
                  AttachLink = AdminSiteConfiguration.GetURL() + "\\UserFiles\\DocFiles\\" + s.FilePath + "\\" + s.AttachFile,
                  StoreName = s.StoreName,
                  CategoryName = s.CategoryName,
                  CreatedOn = s.CreatedOn,
                  CreatedDateFormated = AdminSiteConfiguration.GetDateformat(Convert.ToString(s.CreatedOn)),
                  Notes = s.Notes,
                  Description = s.Description,
                  IsFavorite = s.IsFavorite,
                  IsDelete = s.IsDelete,
                  AttachExtention = s.AttachExtention,
                  IsStatus_id = s.IsStatus_id.Value,
                  FavId = s.FavId,
                  IsPrivate = s.IsPrivate,
                  DocumentCategoryId = s.DocumentCategoryId,
                  FavUserId = s.FavUserId
              }).ToList();

                    PrivateGridList = documentlist.Where(s => s.IsDelete == false && s.StoreId == StoreId && s.IsPrivate == true && s.CreatedBy == CurrentUserId && (s.Title.Trim().ToLower().Contains(SearchTitle.Trim().ToLower()) || s.DocumentKeywords.Select(k => k.Title.Trim().ToLower()).Contains(SearchTitle.Trim().ToLower()))).Select(s => new
                    {
                        DocumentId = s.DocumentId,
                        Type = "Doc",
                        Title = s.Title,
                        FilePath = s.FilePath,
                        AttachFile = s.AttachFile + s.AttachExtention,
                        AttachLink = "",
                        StoreName = s.StoreMasters.NickName,
                        CategoryName = s.DocumentCategories.Name,
                        CreatedOn = s.CreatedOn,
                        Notes = s.Notes,
                        Description = s.Description,
                        IsFavorite = (s.DocumentFavorites.Where(k => k.IsFavorite == true && (isadmin == true ? (k.UserId == k.UserId) : k.UserId == UserId)).Count() > 0 ? true : false),
                        IsDelete = s.IsDelete,
                        AttachExtention = s.AttachExtention,
                        IsStatus_id = s.CreatedBy,
                        FavId = s.DocumentFavorites.Where(k => k.IsFavorite == true && (isadmin == true ? (k.UserId == k.UserId) : k.UserId == UserId)).Count() > 0 ? s.DocumentFavorites.Where(k => k.IsFavorite == true && (isadmin == true ? (k.UserId == k.UserId) : k.UserId == UserId)).FirstOrDefault().DocumentFavoriteId : 0,
                        IsPrivate = s.IsPrivate,
                        DocumentCategoryId = s.DocumentCategoryId,
                        FavUserId = s.DocumentFavorites.Where(k => k.IsFavorite == true && (isadmin == true ? (k.UserId == k.UserId) : k.UserId == UserId)).Count() > 0 ? s.DocumentFavorites.Where(k => k.IsFavorite == true && (isadmin == true ? (k.UserId == k.UserId) : k.UserId == UserId)).FirstOrDefault().UserId : 0,
                    }).ToList()
             .Select(s => new Document
             {
                 DocumentId = s.DocumentId,
                 Type = s.Type,
                 Title = s.Title,
                 FilePath = s.FilePath,
                 AttachFile = s.AttachFile,
                 AttachLink = AdminSiteConfiguration.GetURL() + "\\UserFiles\\DocFiles\\" + s.FilePath + "\\" + s.AttachFile,
                 StoreName = s.StoreName,
                 CategoryName = s.CategoryName,
                 CreatedOn = s.CreatedOn,
                 CreatedDateFormated = AdminSiteConfiguration.GetDateformat(Convert.ToString(s.CreatedOn)),
                 Notes = s.Notes,
                 Description = s.Description,
                 IsFavorite = s.IsFavorite,
                 IsDelete = s.IsDelete,
                 AttachExtention = s.AttachExtention,
                 IsStatus_id = s.IsStatus_id.Value,
                 FavId = s.FavId,
                 IsPrivate = s.IsPrivate,
                 DocumentCategoryId = s.DocumentCategoryId,
                 FavUserId = s.FavUserId
             }).ToList();
                    docGridList.AddRange(PrivateGridList);
                    docGridList = docGridList.OrderByDescending(o => o.DocumentId).ToList();
                }
                else
                {
                    docGridList = documentlist.Where(s => s.IsDelete == false && s.StoreId == StoreId && s.IsPrivate == false).Select(s => new
                    {
                        DocumentId = s.DocumentId,
                        Type = "Doc",
                        Title = s.Title,
                        FilePath = s.FilePath,
                        AttachFile = s.AttachFile + s.AttachExtention,
                        AttachLink = "",
                        StoreName = s.StoreMasters.NickName,
                        CategoryName = s.DocumentCategories.Name,
                        CreatedOn = s.CreatedOn,
                        Notes = s.Notes,
                        Description = s.Description,
                        IsFavorite = (s.DocumentFavorites.Where(k => k.IsFavorite == true && (isadmin == true ? (k.UserId == k.UserId) : k.UserId == UserId)).Count() > 0 ? true : false),
                        IsDelete = s.IsDelete,
                        AttachExtention = s.AttachExtention,
                        IsStatus_id = s.CreatedBy,
                        FavId = s.DocumentFavorites.Where(k => k.IsFavorite == true && (isadmin == true ? (k.UserId == k.UserId) : k.UserId == UserId)).Count() > 0 ? s.DocumentFavorites.Where(k => k.IsFavorite == true && (isadmin == true ? (k.UserId == k.UserId) : k.UserId == UserId)).FirstOrDefault().DocumentFavoriteId : 0,
                        IsPrivate = s.IsPrivate,
                        DocumentCategoryId = s.DocumentCategoryId,
                        FavUserId = s.DocumentFavorites.Where(k => k.IsFavorite == true && (isadmin == true ? (k.UserId == k.UserId) : k.UserId == UserId)).Count() > 0 ? s.DocumentFavorites.Where(k => k.IsFavorite == true && (isadmin == true ? (k.UserId == k.UserId) : k.UserId == UserId)).FirstOrDefault().UserId : 0,
                    }).ToList()
                                .Select(s => new Document
                                {
                                    DocumentId = s.DocumentId,
                                    Type = s.Type,
                                    Title = s.Title,
                                    FilePath = s.FilePath,
                                    AttachFile = s.AttachFile,
                                    AttachLink = AdminSiteConfiguration.GetURL() + "\\UserFiles\\DocFiles\\" + s.FilePath + "\\" + s.AttachFile,
                                    StoreName = s.StoreName,
                                    CategoryName = s.CategoryName,
                                    CreatedOn = s.CreatedOn,
                                    CreatedDateFormated = AdminSiteConfiguration.GetDateformat(Convert.ToString(s.CreatedOn)),
                                    Notes = s.Notes,
                                    Description = s.Description,
                                    IsFavorite = s.IsFavorite,
                                    IsDelete = s.IsDelete,
                                    AttachExtention = s.AttachExtention,
                                    IsStatus_id = s.IsStatus_id.Value,
                                    FavId = s.FavId,
                                    IsPrivate = s.IsPrivate,
                                    DocumentCategoryId = s.DocumentCategoryId,
                                    FavUserId = s.FavUserId
                                }).ToList();

                    PrivateGridList = documentlist.Where(s => s.IsDelete == false && s.StoreId == StoreId && s.IsPrivate == true && s.CreatedBy == CurrentUserId).Select(s => new
                    {
                        DocumentId = s.DocumentId,
                        Type = "Doc",
                        Title = s.Title,
                        FilePath = s.FilePath,
                        AttachFile = s.AttachFile + s.AttachExtention,
                        AttachLink = "",
                        StoreName = s.StoreMasters.NickName,
                        CategoryName = s.DocumentCategories.Name,
                        CreatedOn = s.CreatedOn,
                        Notes = s.Notes,
                        Description = s.Description,
                        IsFavorite = (s.DocumentFavorites.Where(k => k.IsFavorite == true && (isadmin == true ? (k.UserId == k.UserId) : k.UserId == UserId)).Count() > 0 ? true : false),
                        IsDelete = s.IsDelete,
                        AttachExtention = s.AttachExtention,
                        IsStatus_id = s.CreatedBy,
                        FavId = s.DocumentFavorites.Where(k => k.IsFavorite == true && (isadmin == true ? (k.UserId == k.UserId) : k.UserId == UserId)).Count() > 0 ? s.DocumentFavorites.Where(k => k.IsFavorite == true && (isadmin == true ? (k.UserId == k.UserId) : k.UserId == UserId)).FirstOrDefault().DocumentFavoriteId : 0,
                        IsPrivate = s.IsPrivate,
                        DocumentCategoryId = s.DocumentCategoryId,
                        FavUserId = s.DocumentFavorites.Where(k => k.IsFavorite == true && (isadmin == true ? (k.UserId == k.UserId) : k.UserId == UserId)).Count() > 0 ? s.DocumentFavorites.Where(k => k.IsFavorite == true && (isadmin == true ? (k.UserId == k.UserId) : k.UserId == UserId)).FirstOrDefault().UserId : 0,
                    }).ToList()
                               .Select(s => new Document
                               {
                                   DocumentId = s.DocumentId,
                                   Type = s.Type,
                                   Title = s.Title,
                                   FilePath = s.FilePath,
                                   AttachFile = s.AttachFile,
                                   AttachLink = AdminSiteConfiguration.GetURL() + "\\UserFiles\\DocFiles\\" + s.FilePath + "\\" + s.AttachFile,
                                   StoreName = s.StoreName,
                                   CategoryName = s.CategoryName,
                                   CreatedOn = s.CreatedOn,
                                   CreatedDateFormated = AdminSiteConfiguration.GetDateformat(Convert.ToString(s.CreatedOn)),
                                   Notes = s.Notes,
                                   Description = s.Description,
                                   IsFavorite = s.IsFavorite,
                                   IsDelete = s.IsDelete,
                                   AttachExtention = s.AttachExtention,
                                   IsStatus_id = s.IsStatus_id.Value,
                                   FavId = s.FavId,
                                   IsPrivate = s.IsPrivate,
                                   DocumentCategoryId = s.DocumentCategoryId,
                                   FavUserId = s.FavUserId
                               }).ToList();
                    docGridList.AddRange(PrivateGridList);
                    docGridList = docGridList.OrderByDescending(o => o.DocumentId).ToList();
                }

                if (stDate != null || enDate != null)
                {
                    stDate = stDate.Value.AddDays(-1);
                    enDate = enDate.Value.AddDays(1);
                    docGridList = docGridList.Where(s => s.CreatedOn.Value.Date > stDate.Value.Date && s.CreatedOn.Value.Date < enDate.Value.Date).ToList();
                }
                if (CategoryId > 0)
                {
                    docGridList = docGridList.Where(s => s.DocumentCategoryId == CategoryId).ToList();
                }
                if (strFileType != "")
                {
                    string[] TypeArr = strFileType.Split(',').ToArray();
                    docGridList = docGridList.Where(s => TypeArr.Contains(s.AttachExtention)).ToList();
                }


                ViewBag.TotalDoc = docGridList.Count();
                ViewBag.TotalPrivate = docGridList.Where(x => x.IsPrivate == true).Count();
                ViewBag.TotalFav = docGridList.Where(x => x.FavId > 0).Count();

                ViewBag.totalcount = docGridList.Count();
                if (tabListing == 1)
                {
                    docGridList = docGridList.Where(x => x.IsFavorite == true).ToList();
                    ViewBag.totalcount = docGridList.Count();
                }
                else if (tabListing == 2)
                {
                    docGridList = docGridList.Where(x => x.IsPrivate == true).ToList();
                    ViewBag.totalcount = docGridList.Count();
                }
                var ColumnName = typeof(Document).GetProperties().Where(p => p.Name == orderby).FirstOrDefault();

                IEnumerable Data = null;
                if (IsAsc == 1)
                {
                    ViewBag.IsAscVal = 0;
                    docGridList = docGridList.OfType<Document>().ToList().OrderBy(n => ColumnName.GetValue(n, null)).Skip(startIndex - 1).Take(PageSize).ToList();
                }
                else
                {
                    ViewBag.IsAscVal = 1;

                    docGridList = docGridList.OfType<Document>().ToList().OrderByDescending(n => ColumnName.GetValue(n, null)).Skip(startIndex - 1).Take(PageSize).ToList();
                }
                documentsViewModal.StatusMessage = docGridList.Count() == 0 ? "NoItem" : "";
            }
            catch (Exception ex)
            {
                logger.Error("DocumentsController - Grid - " + DateTime.Now + " - " + ex.Message.ToString());
            }

            ViewBag.CategoryId = CategoryId;
            ViewBag.tabListing = tabListing;
            ViewBag.IsBindData = IsBindData;
            ViewBag.CurrentPageIndex = currentPageIndex;
            ViewBag.OrderByVal = orderby;
            ViewBag.PageSize = PageSize;
            ViewBag.Alpha = Alpha;
            ViewBag.chkImages = chkImages;
            ViewBag.chkEmail = chkEmail;
            ViewBag.chkDoc = chkDoc;
            ViewBag.chkSheet = chkSheet;
            ViewBag.chkOther = chkOther;
            ViewBag.SearchTitle = SearchTitle;

            ViewBag.startindex = 1;
            ViewBag.startdate = startdate;
            ViewBag.enddate = enddate;

            if (documentsViewModal.strdashbordsuccess != null && documentsViewModal.strdashbordsuccess.Length > 25)
            {
                ViewBag.StatusMessage = documentsViewModal.strdashbordsuccess;
                documentsViewModal.strdashbordsuccess = "";
            }
            else
            {
                ViewBag.StatusMessage = documentsViewModal.StatusMessage;
            }

            if (TempData["msg"] != null && TempData["msg"].ToString().Length > 25)
            {
                ViewBag.StatusMessage = TempData["msg"].ToString();
            }

            for (int i = 0; i < docGridList.Count; i++)
            {
                var FileExt = docGridList[i].AttachExtention.ToLower();

                if (FileExt.Contains("pdf"))
                {
                    docGridList[i].FileTypeImage = "icon-pdf.svg";
                }
                else if (FileExt.Contains("jpg") || FileExt.Contains("jpeg"))
                {
                    docGridList[i].FileTypeImage = "icon-jpg.svg";
                }
                else if (FileExt.Contains("gif"))
                {
                    docGridList[i].FileTypeImage = "icon-gif.svg";
                }
                else if (FileExt.Contains("png"))
                {
                    docGridList[i].FileTypeImage = "icon-png.svg";
                }
                else if (FileExt.Contains("doc") || FileExt.Contains("docx"))
                {
                    docGridList[i].FileTypeImage = "icon-doc.svg";
                }
                else if (FileExt.Contains("xls") || FileExt.Contains("xlsx") || FileExt.Contains("csv"))
                {
                    docGridList[i].FileTypeImage = "icon-xls.svg";
                }
                else if (FileExt.Contains("txt") || FileExt.Contains("rtf"))
                {
                    docGridList[i].FileTypeImage = "icon-txt.svg";
                }
                else if (FileExt.Contains("eml") || FileExt.Contains("msg"))
                {
                    docGridList[i].FileTypeImage = "icon-eml.svg";
                }
                else if (FileExt.Contains("mp4") || FileExt.Contains("avi") || FileExt.Contains("wmv") || FileExt.Contains("mov") || FileExt.Contains("mkv"))
                {
                    docGridList[i].FileTypeImage = "video.svg";
                }

            }

            ViewBag.UserId = UserId;
            //using this Db class Get Document Categories List.
            ViewBag.DrpCategory = new SelectList(_documentsRepository.DocumentCategoriesList().Where(s => s.IsActive == true).OrderBy(o => o.Name), "DocumentCategoryId", "Name");

            if (startdate != "" || enddate != "")
            {
                clsActivityLog clsActivityLog = new clsActivityLog();
                clsActivityLog.Action = "Click";
                clsActivityLog.ModuleName = "Report";
                clsActivityLog.PageName = "My Documents";
                clsActivityLog.Message = " My Documents Generated for " + (startdate == "" ? "" : "From Date : ") + startdate + (startdate == "" ? "" : " - To Date : ") + enddate;
                //This db class is create activity log
                clsActivityLog.CreatedBy = Convert.ToInt32(Session["UserID"]);
                _synthesisApiRepository.CreateLog(clsActivityLog);
            }

            return View(docGridList);
        }
        // GET: Documents/Create
        /// <summary>
        /// This method is return create view of document.
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "Administrator,CreateDocument")]
        public ActionResult Create()
        {
            try
            {
                Document documentCreate = new Document();
                if (TempData["PostedData"] != null)
                {
                    documentCreate = (Document)TempData["PostedData"];
                    TempData["PostedData"] = null;
                }
                if (TempData["msg"] != null)
                {
                    ViewBag.StatusMessage = TempData["msg"].ToString();
                    TempData["msg"] = null;
                }
                var StoreId = Convert.ToInt16(Session["storeid"]);
                //using this Db class Get Document Categories List.
                ViewBag.DocumentCategoryId = new SelectList(_documentsRepository.DocumentCategoriesList().Where(s => s.IsActive == true).OrderBy(o => o.Name), "DocumentCategoryId", "Name");
                //Get Store List data.
                ViewBag.StoreId = new SelectList(_commonRepository.GetStoreList(2), "StoreId", "NickName", StoreId);
            }
            catch (Exception ex)
            {
                logger.Error("DocumentsController - Create - " + DateTime.Now + " - " + ex.Message.ToString());
            }

            return View();
        }

        // POST: Documents/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        /// <summary>
        /// Using this Method Save documents.
        /// </summary>
        /// <param name="document"></param>
        /// <param name="AttachFile"></param>
        /// <param name="btnsubmit"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator,CreateDocument")]
        public async Task<ActionResult> Create(Document document, HttpPostedFileBase AttachFile, string btnsubmit = "")
        {
            string strFilePrefix = AdminSiteConfiguration.GetEasternTime(DateTime.Now).ToString("ddMMyyyyhhmmss") + "_";
            try
            {
                ModelState.Remove("DocumentKeywords");
                if (ModelState.IsValid)
                {
                    #region file
                    try
                    {
                        //Get store master List using stroeId.
                        var storeList = _documentsRepository.StoreMastersList().Where(x => x.StoreId == document.StoreId).FirstOrDefault();
                        var StoreName = "";
                        if (storeList != null)
                        {
                            StoreName = storeList.Name;
                        }

                        if (AttachFile != null)
                        {
                            if (AttachFile.ContentLength > 0)
                            {
                                // ".eml", ".msg", ".txt", ".csv", ".rtf", ".msg", 
                                string Sacn_Title;
                                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".pdf", ".doc", ".docx", ".xls", ".xlsx", ".mp4", ".avi", ".wmv", ".mov", ".mkv" };
                                document.AttachFile = AttachFile.FileName;
                                document.AttachExtention = Path.GetExtension(AttachFile.FileName);

                                if (!allowedExtensions.Contains(document.AttachExtention.ToLower()))
                                {
                                    ViewBag.StatusMessage = "Invalid File";
                                    document.strErrMessage = "Invalid File";
                                    TempData["document"] = document;
                                    return RedirectToAction("AddDocument", "Document");
                                }
                                else
                                {
                                    if (AttachFile.ContentLength > 20971520)
                                    {
                                        ViewBag.StatusMessage = "InvalidPDFSize";
                                        document.strErrMessage = "InvalidPDFSize";
                                        TempData["document"] = document;
                                        return RedirectToAction("AddDocument", "Document");
                                    }
                                    else
                                    {
                                        Sacn_Title = document.Title;
                                        Sacn_Title = AdminSiteConfiguration.RemoveSpecialCharacter(Sacn_Title);
                                        StoreName = AdminSiteConfiguration.RemoveSpecialCharacter(StoreName);

                                        DateTime curDate = AdminSiteConfiguration.GetEasternTime(DateTime.Now);
                                        string BasePath = "~\\UserFiles\\DocFiles";
                                        string ExtPath = "\\" + StoreName;
                                        ExtPath = ExtPath + "\\" + curDate.Year.ToString();
                                        ExtPath = ExtPath + "\\" + System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(curDate.Month); ;

                                        document.FilePath = ExtPath;
                                        if (!Directory.Exists(Server.MapPath(BasePath + ExtPath)))
                                        {
                                            Directory.CreateDirectory(Server.MapPath(BasePath + ExtPath));
                                        }

                                        var UserName = System.Web.HttpContext.Current.User.Identity.Name;
                                        //Db class for get user Id using username.
                                        document.CreatedBy = Convert.ToInt32(_commonRepository.getUserId(UserName));
                                        document.CreatedOn = AdminSiteConfiguration.GetEasternTime(DateTime.Now);

                                        document.IsPrivate = document.chkPrivate == "1" ? true : false;
                                        document.IsFavorite = document.chkFav == "1" ? true : false;
                                        document.AttachFile = strFilePrefix + (Sacn_Title.Replace(document.AttachExtention, "")) + "_" + StoreName + "_" + Convert.ToInt32(_commonRepository.getUserId(UserName));

                                        document.AttachExtention = document.AttachExtention.ToLower();
                                        //Db class is Save Documents
                                        _documentsRepository.SaveDocuments(document);
                                        //This db classs is Save Attch file.
                                        _documentsRepository.SaveAttachFile(document.AttachFile, document.DocumentId);

                                        var path1 = Server.MapPath(BasePath + ExtPath + "\\") + document.AttachFile + document.AttachExtention;

                                        AttachFile.SaveAs(path1);


                                        if (document.KeyWords != null)
                                        {
                                            var KeywordList = document.KeyWords.Split(',');
                                            foreach (string keyword in KeywordList)
                                            {
                                                DocumentKeyword DKObj = new DocumentKeyword();
                                                DKObj.DocumentId = document.DocumentId;
                                                DKObj.Title = keyword;
                                                DKObj.IsActive = true;
                                                //Save Document keyword.
                                                _documentsRepository.SaveDocumentKeyword(DKObj);
                                            }
                                        }

                                        DocumentFavorite DFObj = new DocumentFavorite();
                                        DFObj.DocumentId = document.DocumentId;
                                        //Db class for get user Id using username.
                                        DFObj.UserId = Convert.ToInt32(_commonRepository.getUserId(UserName));
                                        DFObj.CreatedOn = AdminSiteConfiguration.GetEasternTime(DateTime.Now);
                                        DFObj.IsFavorite = document.IsFavorite;
                                        //Using this db class save Document favourite.
                                        _documentsRepository.SaveDocumentFavorite(DFObj);
                                        ActivityLog ActLog = new ActivityLog();
                                        ActLog.Action = 1;
                                        //This Db class is used for get user firstname.
                                        ActLog.Comment = "Document " + "<a href='/Documents/DetailDocument/" + document.DocumentId + "'>" + document.Title + "</a> Uploaded by " + _commonRepository.getUserFirstName(UserName) + " on " + AdminSiteConfiguration.GetDate(AdminSiteConfiguration.GetEasternTime(DateTime.Now).ToString());
                                        //This  class is used for Activity Log Insert
                                        _activityLogRepository.ActivityLogInsert(ActLog);

                                        string message = "";
                                        ViewBag.StatusMessage = _commonRepository.GetMessageValue("DC", "Document added successfully.");
                                        TempData["msg"] = _commonRepository.GetMessageValue("DC", "Document added successfully."); if (btnsubmit == "Save & New")
                                        {
                                            return RedirectToAction("Create", "Documents", new { @dashbordsuccess = message });
                                        }
                                        else
                                        {
                                            return RedirectToAction("MyDocumentIndex", "Documents", new { @dashbordsuccess = message });
                                        }
                                    }
                                }
                            }
                        }

                    }
                    catch (Exception ex)
                    {
                        return RedirectToAction("MyDocumentIndex", "Document", new { @dashbordsuccess = "" });
                    }
                    #endregion

                    return RedirectToAction("MyDocumentIndex", "Document", new { @dashbordsuccess = "" });
                }
                //using this Db class Get Document Categories List.
                ViewBag.DocumentCategoryId = new SelectList(_documentsRepository.DocumentCategoriesList().Where(s => s.IsActive == true).OrderBy(o => o.Name), "DocumentCategoryId", "Name", document.DocumentCategoryId);
                //Get Store List data.
                ViewBag.StoreId = new SelectList(_commonRepository.GetStoreList(2), "StoreId", "NickName", document.StoreId);
            }
            catch (Exception ex)
            {
                logger.Error("DocumentsController - Create - " + DateTime.Now + " - " + ex.Message.ToString());
            }

            return View(document);
        }

        // GET: Documents/Edit/5
        /// <summary>
        /// This method is get Document by Id for Update Document.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<ActionResult> Edit(int? id)
        {
            //Get Document By Id.
            Document document = await _documentsRepository.GetDocumentsById(id);
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            try
            {

                if (document == null)
                {
                    return HttpNotFound();
                }
                var FullPath = document.FilePath + "\\" + document.AttachFile + document.AttachExtention;

                foreach (var dk in document.DocumentKeywords)
                {
                    document.KeyWords += dk.Title + ",";
                }
                if (document.KeyWords != null && document.KeyWords.Contains(","))
                {
                    document.KeyWords = document.KeyWords.Remove(document.KeyWords.LastIndexOf(","));
                }
                if (document.DocumentFavorites.Count() > 0 && document.DocumentFavorites != null && document.DocumentFavorites.FirstOrDefault().IsFavorite == true)
                {
                    document.chkFav = "1";
                }
                else
                {
                    document.chkFav = "0";
                }
                document.AttachLink = FullPath;
            }
            catch (Exception ex)
            {
                logger.Error("DocumentsController - Edit - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            //using this Db class Get Document Categories List.
            ViewBag.DocumentCategoryId = new SelectList(_documentsRepository.DocumentCategoriesList().Where(s => s.IsActive == true).OrderBy(o => o.Name), "DocumentCategoryId", "Name", document.DocumentCategoryId);
            //Get Store List data.
            ViewBag.StoreId = new SelectList(_commonRepository.GetStoreList(2), "StoreId", "NickName", document.StoreId);
            return View(document);
        }

        // POST: Documents/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        /// <summary>
        /// This method is update documents.
        /// </summary>
        /// <param name="document"></param>
        /// <param name="AttachFile"></param>
        /// <param name="btnsubmit"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator,UpdateDocument")]
        public async Task<ActionResult> Edit(Document document, HttpPostedFileBase AttachFile, string btnsubmit = "")
        {
            string strNewFile = "";

            string strFilePrefix = AdminSiteConfiguration.GetEasternTime(DateTime.Now).ToString("ddMMyyyyhhmmss") + "_";
            if (ModelState.IsValid)
            {
            }

            #region file
            try
            {
                //Get Store Master ID.
                var storeList = _documentsRepository.StoreMastersId(document.StoreId);
                var StoreName = "";
                if (storeList != null)
                {
                    StoreName = storeList.Name;
                }

                if (AttachFile != null)
                {
                    if (AttachFile.ContentLength > 0)
                    {
                        //  ".eml", ".msg", ".txt", ".csv", ".rtf", ".msg",
                        string Sacn_Title;
                        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".pdf", ".doc", ".docx", ".xls", ".xlsx", ".mp4", ".avi", ".wmv", ".mov", ".mkv" };

                        document.AttachFile = AttachFile.FileName;
                        document.AttachExtention = Path.GetExtension(AttachFile.FileName);

                        if (!allowedExtensions.Contains(document.AttachExtention.ToLower()))
                        {
                            ViewBag.StatusMessage = "Invalid File";
                            // return View(PostedData);
                            document.strErrMessage = "Invalid File";
                            TempData["PostedData"] = document;
                            return RedirectToAction("Edit", "Documents", new { id = document.DocumentId });
                        }
                        else
                        {
                            if (AttachFile.ContentLength > 20971520)
                            {
                                ViewBag.StatusMessage = "InvalidPDFSize";
                                document.strErrMessage = "InvalidPDFSize";
                                TempData["PostedData"] = document;
                                return RedirectToAction("Edit", "Documents", new { id = document.DocumentId });
                            }
                            else
                            {
                                Sacn_Title = document.Title;
                                Sacn_Title = AdminSiteConfiguration.RemoveSpecialCharacter(Sacn_Title);
                                StoreName = AdminSiteConfiguration.RemoveSpecialCharacter(StoreName);

                                DateTime curDate = AdminSiteConfiguration.GetEasternTime(DateTime.Now);
                                string BasePath = "~\\UserFiles\\DocFiles";
                                string ExtPath = "\\" + StoreName;
                                ExtPath = ExtPath + "\\" + curDate.Year.ToString();
                                ExtPath = ExtPath + "\\" + System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(curDate.Month); ;

                                document.FilePath = ExtPath;
                                if (!Directory.Exists(Server.MapPath(BasePath + ExtPath)))
                                {
                                    Directory.CreateDirectory(Server.MapPath(BasePath + ExtPath));
                                }

                                var UserName = System.Web.HttpContext.Current.User.Identity.Name;
                                Sacn_Title = (Sacn_Title.Replace(document.AttachExtention, "")) + "_" + StoreName + "_" + Convert.ToInt32(_commonRepository.getUserId(UserName)) + "_" + document.DocumentId.ToString();

                                var path1 = Server.MapPath(BasePath + ExtPath + "\\") + strFilePrefix + Sacn_Title + document.AttachExtention;
                                strNewFile = Server.MapPath(BasePath + ExtPath + "\\");
                                ////Request.PhysicalApplicationPath +
                                if (document.DocumentId > 0)
                                {
                                    //Get Documents Id.
                                    var dataDoc1 = _documentsRepository.DocumentsId(document.DocumentId);
                                    strNewFile += dataDoc1.AttachFile + dataDoc1.AttachExtention;
                                    if (System.IO.File.Exists(strNewFile))
                                    {
                                        System.IO.File.Delete(strNewFile);
                                    }
                                }

                                AttachFile.SaveAs(path1);
                                document.AttachFile = strFilePrefix + (Sacn_Title.Replace(document.AttachExtention, ""));
                                document.AttachExtention = document.AttachExtention.ToLower();
                            }
                        }
                    }
                    else
                    {
                        ViewBag.StatusMessage = "InvalidPDFSize";
                        //return RedirectToAction("EditDocument", "Document", new { id = PostedData.Id });
                    }
                }
                else
                {
                    ViewBag.StatusMessage = "InvalidPDFSize";
                    //return RedirectToAction("EditDocument", "Document", new { id = PostedData.Id });
                }

            }
            catch (Exception ex)
            {
                logger.Error("DocumentsController - Edit - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            #endregion

            try
            {
                int DocId = 0;
                var UserName = System.Web.HttpContext.Current.User.Identity.Name;
                //Db class for get user Id using username.
                document.ModifiedBy = Convert.ToInt32(_commonRepository.getUserId(UserName));
                document.ModifiedOn = AdminSiteConfiguration.GetEasternTime(DateTime.Now);

                Document dataDoc;
                if (document.DocumentId > 0)
                {
                    //Db class is update Documents
                    _documentsRepository.UpdateDocuments(document);

                    ActivityLog ActLog = new ActivityLog();
                    ActLog.Action = 2;
                    //This Db class is used for get user firstname.
                    ActLog.Comment = "Document " + "<a href='/Documents/DetailDocument/" + document.DocumentId + "'>" + document.Title + "</a> Updated by " + _commonRepository.getUserFirstName(UserName) + " on " + AdminSiteConfiguration.GetDate(AdminSiteConfiguration.GetEasternTime(DateTime.Now).ToString());
                    //This  class is used for Activity Log Insert
                    _activityLogRepository.ActivityLogInsert(ActLog);

                    //using this Db class is Document Keyword remove
                    _documentsRepository.DocumentKeywordRemove(document.DocumentId);
                    if (document.KeyWords != null)
                    {
                        var KeywordList = document.KeyWords.Split(',');
                        foreach (string keyword in KeywordList)
                        {
                            DocumentKeyword DKObj = new DocumentKeyword();
                            DKObj.DocumentId = document.DocumentId;
                            DKObj.Title = keyword;
                            DKObj.IsActive = true;
                            //Save Document keyword.
                            _documentsRepository.SaveDocumentKeyword(DKObj);
                        }
                    }
                    //Get Document Favourites List.
                    DocumentFavorite docFav = _documentsRepository.DocumentFavoritesList().Where(x => x.DocumentId == document.DocumentId).FirstOrDefault();
                    if (docFav != null)
                    {
                        docFav.IsFavorite = document.IsFavorite;
                    }
                }

                string message = "";

                //TempData["msg"] = "Document edited successfully.";
                TempData["msg"] = _commonRepository.GetMessageValue("DE", "Document edited successfully.");

                if (btnsubmit == "Save & New")
                {
                    return RedirectToAction("Create", "Document", new { @dashbordsuccess = message });
                }
                else
                {
                    return RedirectToAction("MyDocumentIndex", "Documents", new { @dashbordsuccess = message });
                }
            }

            catch (Exception ex)
            {
                logger.Error("DocumentsController - Edit - " + DateTime.Now + " - " + ex.Message.ToString());
                return RedirectToAction("MyDocumentIndex", "Documents", new { @dashbordsuccess = "" });
            }
        }
        /// <summary>
        /// This method get details of documents.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="RedairectFrom"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult DetailDocument(int id = 0, string RedairectFrom = "")
        {
            var UserName = System.Web.HttpContext.Current.User.Identity.Name;
            //Db class for get user Id using username.
            int CurrentUserId = Convert.ToInt32(_commonRepository.getUserId(UserName));

            ViewBag.id = id;
            Document document = new Document();
            try
            {
                //Get documents Id.
                document = _documentsRepository.GetDocumentsId(id);
                document.StoreName = document.StoreMasters.NickName;
                document.CategoryName = document.DocumentCategories.Name;
                //Get User Firstname By id.
                document.CreatedByName = _commonRepository.getUserFirstNameById((int)document.CreatedBy);
                document.CreatedDateFormated = AdminSiteConfiguration.GetDateformat(Convert.ToString(document.CreatedOn));
                if (document.ModifiedBy != null)
                {
                    //Get User Firstname By id.
                    document.ModifyByName = _commonRepository.getUserFirstNameById((int)document.ModifiedBy);
                }
                var FullPath = document.FilePath + "\\" + document.AttachFile + document.AttachExtention;

                foreach (var dk in document.DocumentKeywords)
                {
                    document.KeyWords += dk.Title + ",";
                }
                if (document.KeyWords != null && document.KeyWords.Contains(","))
                {
                    document.KeyWords = document.KeyWords.Remove(document.KeyWords.LastIndexOf(","));
                }
                if (document.DocumentFavorites != null && document.DocumentFavorites.FirstOrDefault().IsFavorite == true)
                {
                    document.chkFav = "1";
                }
                else
                {
                    document.chkFav = "0";
                }
                document.AttachLink = FullPath;


                var FileExt = document.AttachExtention.ToLower();

                if (FileExt.Contains("pdf"))
                {
                    document.FileTypeImage = "icon-pdf.svg";
                }
                else if (FileExt.Contains("jpg") || FileExt.Contains("jpeg"))
                {
                    document.FileTypeImage = "icon-jpg.svg";
                }
                else if (FileExt.Contains("gif"))
                {
                    document.FileTypeImage = "icon-gif.svg";
                }
                else if (FileExt.Contains("png"))
                {
                    document.FileTypeImage = "icon-png.svg";
                }
                else if (FileExt.Contains("doc") || FileExt.Contains("docx"))
                {
                    document.FileTypeImage = "icon-doc.svg";
                }
                else if (FileExt.Contains("xls") || FileExt.Contains("xlsx") || FileExt.Contains("csv"))
                {
                    document.FileTypeImage = "icon-xls.svg";
                }
                else if (FileExt.Contains("txt") || FileExt.Contains("rtf"))
                {
                    document.FileTypeImage = "icon-txt.svg";
                }
                else if (FileExt.Contains("eml") || FileExt.Contains("msg"))
                {
                    document.FileTypeImage = "icon-eml.svg";
                }
            }
            catch (Exception ex)
            {
                logger.Error("DocumentsController - DetailDocument - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return View(document);
        }

        /// <summary>
        /// This method get document id for delete details of documents.
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DetailDocument(int Id = 0)
        {
            //get Delete Details Document.
            _documentsRepository.DeleteDetailDocument(Id);
            return RedirectToAction("index", "Documents", new { @dashbordsuccess = "Delete" });
        }
        /// <summary>
        /// This method is delete documents.
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [Authorize(Roles = "Administrator,DeleteDocument")]
        public ActionResult Delete(int Id = 0)
        {
            try
            {
                //Delete details Document By id.
                var data = _documentsRepository.DeleteDetailDocument(Id);
                var UserName = System.Web.HttpContext.Current.User.Identity.Name;
                ActivityLog ActLog = new ActivityLog();
                ActLog.Action = 3;
                //This Db class is used for get user firstname.
                ActLog.Comment = "Document " + data.Title + " Deleted by " + _commonRepository.getUserFirstName(UserName) + " on " + AdminSiteConfiguration.GetDate(AdminSiteConfiguration.GetEasternTime(DateTime.Now).ToString());
                //This  class is used for Activity Log Insert
                _activityLogRepository.ActivityLogInsert(ActLog);

                //TempData["msg"] = "Document deleted successfully.";
                TempData["msg"] = _commonRepository.GetMessageValue("DDD", "Document deleted successfully.");

                return Json(new { sucess = 1 }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error("DocumentsController - Delete - " + DateTime.Now + " - " + ex.Message.ToString());
                return Json(new { sucess = 0 }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// This method use for add to favourite document.
        /// </summary>
        /// <param name="DocId"></param>
        /// <returns></returns>
        public ActionResult AddToFavorite(int DocId = 0)
        {
            try
            {
                var UserName = System.Web.HttpContext.Current.User.Identity.Name;
                //Db class for get user Id.
                int userid = Convert.ToInt32(_commonRepository.getUserId(UserName));
                //get all Document favoritelist
                DocumentFavorite data = _documentsRepository.GetDocumentFavoritesobj(DocId, userid);
                if (data != null)
                {
                    data.IsFavorite = data.IsFavorite == true ? false : true; //reverse favorite flag
                    //update Document favorite.
                    _documentsRepository.UpdateDocumentFavorite(data);
                }
                else if (DocId > 0)
                {
                    DocumentFavorite DFObj = new DocumentFavorite();
                    DFObj.DocumentId = DocId;
                    //Db class for get user Id.
                    DFObj.UserId = Convert.ToInt32(_commonRepository.getUserId(UserName));
                    DFObj.CreatedOn = AdminSiteConfiguration.GetEasternTime(DateTime.Now);
                    DFObj.IsFavorite = true;
                    //Using this db class save docuemnt.
                    _documentsRepository.SaveDocumentFavorite(DFObj);
                    //get all Document favoritelist
                    data = _documentsRepository.DocumentFavoritesList().Where(x => x.DocumentId == DocId).FirstOrDefault();
                }


                string message = data.IsFavorite == true ? _commonRepository.GetMessageValue("DF", "Document added to favorite.") : _commonRepository.GetMessageValue("DUF", "Document added to Unfavorite.");

                return Json(new { sucess = 1, msg = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error("DocumentsController - AddToFavorite - " + DateTime.Now + " - " + ex.Message.ToString());
                return Json(new { sucess = 0, msg = "" }, JsonRequestBehavior.AllowGet);
            }
        }
        /// <summary>
        /// This method is  Edit documents.
        /// </summary>
        /// <param name="Ids"></param>
        /// <returns></returns>
        public ActionResult EditDocument(int? Ids)
        {
            Document document = new Document();
            ////Get Documents Id.
            document = _documentsRepository.GetDocumentsId(Ids);
            ViewBag.filePath = document.AttachFile + document.AttachExtention;
            ViewBag.Ids = Ids;
            ViewBag.FailLoad = _commonRepository.GetMessageValue("DFL", "Fail to load the document");
            ViewBag.SavedWord = _commonRepository.GetMessageValue("DSSW", "Successfully Saved Word File.");
            ViewBag.FailTosave = _commonRepository.GetMessageValue("DFSD", "Fail to save the document");

            return View();
        }


        /// <summary>
        /// This method is  Download documents.
        /// </summary>
        /// <param name="Ids"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Download(int? Ids)
        {
            Session["FileName"] = null;
            Document document = new Document();
            //Get Documents Id.
            document = _documentsRepository.GetDocumentsId(Ids);
            var FullPath = Server.MapPath("~\\UserFiles\\DocFiles" + document.FilePath);
            var filePath = FullPath + @"\" + document.AttachFile + document.AttachExtention;
            if (document.AttachExtention == ".xlsx" || document.AttachExtention == ".xls" || document.AttachExtention == ".csv")
            {
                return File(filePath, "application/vnd.ms-excel", document.AttachFile + document.AttachExtention);
            }
            else if (document.AttachExtention == ".pdf")
            {
                return File(filePath, "application/pdf", document.AttachFile + document.AttachExtention);
            }
            else
            {
                return File(filePath, "application/octet-stream", document.AttachFile + document.AttachExtention);
            }
        }

        /// <summary>
        /// This method is  Get report by id.
        /// </summary>
        /// <param name="Ids"></param>
        /// <returns></returns>
        public FileResult GetReport(int? Ids)
        {
            Document document = new Document();
            //Get Documents Id.
            document = _documentsRepository.GetDocumentsId(Ids);
            var FullPath = Server.MapPath("~\\UserFiles\\DocFiles" + document.FilePath);
            var filePath = FullPath + @"\" + document.AttachFile + document.AttachExtention;
            byte[] FileBytes = System.IO.File.ReadAllBytes(filePath);
            return File(FileBytes, "application/pdf");
        }

        /// <summary>
        /// This method is Pdf Viewe data.
        /// </summary>
        /// <param name="Ids"></param>
        /// <returns></returns>
        public ActionResult PdfViewerData(int? Ids)
        {
            Document document = new Document();
            try
            {
                //Get Documents Id.
                ViewBag.Editsuccess = _commonRepository.GetMessageValue("DSEDD", "Successfully Edit Document Data.");
                document = _documentsRepository.GetDocumentsId(Ids);
                if (document != null)
                {
                    var FullPath = Server.MapPath("~\\UserFiles\\DocFiles" + document.FilePath);
                    var filePath = FullPath + @"\" + document.AttachFile + document.AttachExtention;
                    var rep = filePath.Replace("\\", "/");

                    ViewBag.PdfPath = rep;
                    ViewBag.PdfName = document.AttachFile;
                    ViewBag.Ids = Ids;
                    ViewBag.EditDoc = _commonRepository.GetMessageValue("DSE", "Successfully Edit Document Data.");

                }
            }
            catch (Exception ex)
            {
                logger.Error("DocumentsController - PdfViewerData - " + DateTime.Now + " - " + ex.Message.ToString());
            }

            return View();
        }

        /// <summary>
        /// In this method get Edit Document details by Id.
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public ActionResult EditDocumentsDetali(int? ids)
        {
            //Get Documents Id.
            Document document = _documentsRepository.GetDocumentsId(ids);
            if (ids == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            try
            {

                if (document == null)
                {
                    return HttpNotFound();
                }
                var FullPath = document.FilePath + "\\" + document.AttachFile + document.AttachExtention;

                foreach (var dk in document.DocumentKeywords)
                {
                    document.KeyWords += dk.Title + ",";
                }
                if (document.KeyWords != null && document.KeyWords.Contains(","))
                {
                    document.KeyWords = document.KeyWords.Remove(document.KeyWords.LastIndexOf(","));
                }
                if (document.DocumentFavorites.Count() > 0 && document.DocumentFavorites.FirstOrDefault().IsFavorite == true)
                {
                    document.chkFav = "1";
                }
                else
                {
                    document.chkFav = "0";
                }
                document.AttachLink = FullPath;
                ViewBag.Ids = ids;
            }
            catch (Exception ex)
            {
                logger.Error("DocumentsController - EditDocumentsDetali - " + DateTime.Now + " - " + ex.Message.ToString());
            }

            //using this Db class Get Document Categories List.
            ViewBag.DocumentCategoryId = new SelectList(_documentsRepository.DocumentCategoriesList().Where(s => s.IsActive == true).OrderBy(o => o.Name), "DocumentCategoryId", "Name", document.DocumentCategoryId);
            return PartialView("_EditDocument", document);
        }
        /// <summary>
        /// This method is Edit Document details values.
        /// </summary>
        /// <param name="DocumentId"></param>
        /// <param name="DocumentCategoryId"></param>
        /// <param name="KeyWords"></param>
        /// <param name="Notes"></param>
        /// <param name="chkFav"></param>
        /// <param name="Title"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> EditDocumentsDetaliValue(string DocumentId, string DocumentCategoryId, string KeyWords, string Notes, string chkFav, string Title)
        {
            string d = "";
            var UserName = System.Web.HttpContext.Current.User.Identity.Name;
            try
            {
                //Get Documents Id string.
                Document document = _documentsRepository.GetDocumentsIdString(DocumentId);
                //Db class for get user Id.
                document.ModifiedBy = Convert.ToInt32(_commonRepository.getUserId(UserName));
                document.ModifiedOn = AdminSiteConfiguration.GetEasternTime(DateTime.Now);
                document.DocumentCategoryId = Convert.ToInt32(DocumentCategoryId);
                document.Notes = Notes;
                document.IsFavorite = chkFav == "1" ? true : false;
                //using thid db class Update document details.
                _documentsRepository.UpdateDocuments(document);

                ActivityLog ActLog = new ActivityLog();
                ActLog.Action = 2;
                //This Db class is used for get user firstname.
                ActLog.Comment = "Document " + "<a href='/Documents/DetailDocument/" + document.DocumentId + "'>" + Title + "</a> Updated by " + _commonRepository.getUserFirstName(UserName) + " on " + AdminSiteConfiguration.GetDate(AdminSiteConfiguration.GetEasternTime(DateTime.Now).ToString());
                //This  class is used for Activity Log Insert
                _activityLogRepository.ActivityLogInsert(ActLog);

                //Remove document Keyword.
                _documentsRepository.DocumentKeywordRemove(document.DocumentId);
                if (KeyWords != null)
                {
                    var KeywordList = KeyWords.Split(',');
                    foreach (string keyword in KeywordList)
                    {
                        DocumentKeyword DKObj = new DocumentKeyword();
                        DKObj.DocumentId = document.DocumentId;
                        DKObj.Title = keyword;
                        DKObj.IsActive = true;
                        //Save Dcoument Keyword.
                        _documentsRepository.SaveDocumentKeyword(DKObj);
                    }
                }
                //get all Document favoritelist
                DocumentFavorite docFav = _documentsRepository.DocumentFavoritesList().Where(x => x.DocumentId == document.DocumentId).FirstOrDefault();
                _documentsRepository.SaveDocumentFavorite(docFav);
                d = "Completed";
            }
            catch (Exception ex)
            {
                logger.Error("DocumentsController - EditDocumentsDetaliValue - " + DateTime.Now + " - " + ex.Message.ToString());
            }

            return Json(d, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// This method is save documents details.
        /// </summary>
        /// <param name="objDocDetail"></param>
        /// <returns></returns>
        public ActionResult SaveDocumentsDetail(DocumentBulk objDocDetail)
        {
            string strFilePrefix = AdminSiteConfiguration.GetEasternTime(DateTime.Now).ToString("ddMMyyyy") + "_";

            var StoreId = Convert.ToInt32(Session["storeid"]);
            //Get Store Masters list using store Id.
            var storeList = _documentsRepository.StoreMastersList().Where(x => x.StoreId == StoreId).FirstOrDefault();
            var StoreName = "";
            if (storeList != null)
            {
                StoreName = storeList.Name;
            }
            //using this Db class Get Document Categories List.
            var DocCategoryId = _documentsRepository.DocumentCategoriesList().Where(s => s.Name == "UnAssigned").Count() > 0 ? _documentsRepository.DocumentCategoriesList().Where(s => s.Name == "UnAssigned").FirstOrDefault().DocumentCategoryId : 0;
            try
            {
                if (ModelState.IsValid)
                {
                    if (DocCategoryId > 0)
                    {
                        if (objDocDetail != null)
                        {
                            int i = 1;
                            foreach (var AttachFile in objDocDetail.File)
                            {
                                if (AttachFile.ContentLength > 0)
                                {
                                    var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".pdf", ".doc", ".docx", ".xls", ".xlsx", ".mp4", ".avi", ".wmv", ".mov", ".mkv" };
                                    Document document = new Document();
                                    document.AttachFile = AttachFile.FileName;
                                    document.AttachExtention = Path.GetExtension(AttachFile.FileName);

                                    if (!allowedExtensions.Contains(document.AttachExtention.ToLower()))
                                    {
                                        ViewBag.StatusMessage = "Invalid File";
                                        document.strErrMessage = "Invalid File";
                                        TempData["document"] = document;
                                        return RedirectToAction("AddDocument", "Document");
                                    }
                                    else
                                    {
                                        if (AttachFile.ContentLength > 20971520)
                                        {
                                            ViewBag.StatusMessage = "InvalidPDFSize";
                                            document.strErrMessage = "InvalidPDFSize";
                                            TempData["document"] = document;
                                            return RedirectToAction("AddDocument", "Document");
                                        }
                                        else
                                        {
                                            StoreName = AdminSiteConfiguration.RemoveSpecialCharacter(StoreName);

                                            DateTime curDate = AdminSiteConfiguration.GetEasternTime(DateTime.Now);
                                            string BasePath = "~\\UserFiles\\DocFiles";
                                            string ExtPath = "\\" + StoreName;
                                            ExtPath = ExtPath + "\\" + curDate.Year.ToString();
                                            ExtPath = ExtPath + "\\" + System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(curDate.Month); ;

                                            document.FilePath = ExtPath;
                                            if (!Directory.Exists(Server.MapPath(BasePath + ExtPath)))
                                            {
                                                Directory.CreateDirectory(Server.MapPath(BasePath + ExtPath));
                                            }
                                            document.StoreId = StoreId;
                                            var Title = "BulkUnAssignedDoc_" + DateTime.Now.Ticks;
                                            var UserName = System.Web.HttpContext.Current.User.Identity.Name;
                                            document.Title = Title;
                                            document.DocumentCategoryId = DocCategoryId;
                                            //Db class for get user Id.
                                            document.CreatedBy = Convert.ToInt32(_commonRepository.getUserId(UserName));
                                            document.CreatedOn = AdminSiteConfiguration.GetEasternTime(DateTime.Now);

                                            document.IsPrivate = false;
                                            document.IsFavorite = false;
                                            document.AttachFile = strFilePrefix + Title;
                                            document.AttachExtention = document.AttachExtention.ToLower();


                                            try
                                            {
                                                foreach (var item in objDocDetail.item)
                                                {
                                                    string[] data = item.Split('^');
                                                    if (AttachFile.FileName == data[0].ToString())
                                                    {
                                                        document.IsFavorite = Convert.ToBoolean(data[1].ToString());
                                                        document.IsPrivate = Convert.ToBoolean(data[2].ToString());
                                                        break;
                                                    }
                                                }
                                            }
                                            catch (Exception ex)
                                            {
                                                logger.Error("DocumentsController - SaveDocumentsDetail - " + DateTime.Now + " - " + ex.Message.ToString());
                                            }
                                            //Db class is Save Documents
                                            _documentsRepository.SaveDocuments(document);
                                            //This db classs is Save Attch file.
                                            _documentsRepository.SaveAttachFile(document.AttachFile, document.DocumentId);

                                            var path1 = Server.MapPath(BasePath + ExtPath + "\\") + document.AttachFile + document.AttachExtention;

                                            AttachFile.SaveAs(path1);
                                            i += 1;


                                            DocumentFavorite DFObj = new DocumentFavorite();
                                            DFObj.DocumentId = document.DocumentId;
                                            //Db class for get user Id.
                                            DFObj.UserId = Convert.ToInt32(_commonRepository.getUserId(UserName));
                                            DFObj.CreatedOn = AdminSiteConfiguration.GetEasternTime(DateTime.Now);
                                            DFObj.IsFavorite = document.IsFavorite;
                                            //Save Document keyword.
                                            _documentsRepository.SaveDocumentFavorite(DFObj);


                                            ActivityLog ActLog = new ActivityLog();
                                            ActLog.Action = 1;
                                            //This Db class is used for get user firstname.
                                            ActLog.Comment = "Document " + "<a href='/Documents/DetailDocument/" + document.DocumentId + "'>" + document.Title + "</a> Uploaded by " + _commonRepository.getUserFirstName(UserName) + " on " + AdminSiteConfiguration.GetDate(AdminSiteConfiguration.GetEasternTime(DateTime.Now).ToString());
                                            //This  class is used for Activity Log Insert
                                            _activityLogRepository.ActivityLogInsert(ActLog);




                                        }
                                    }
                                }
                            }
                            ViewBag.StatusMessage = _commonRepository.GetMessageValue("DC", "Document added successfully.");

                            TempData["msg"] = _commonRepository.GetMessageValue("DC", "Document added successfully.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("DocumentsController - SaveDocumentsDetail - " + DateTime.Now + " - " + ex.Message.ToString());
            }


            return Json(new { Result = "success" }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GridScroll(int IsBindData = 1, int currentPageIndex = 1, string orderby = "id", int IsAsc = 0, int PageSize = 20, int SearchRecords = 1, string Alpha = "", string SearchTitle = "", int CategoryId = 0, string startdate = "", string enddate = "", bool chkImages = false, bool chkEmail = false, bool chkDoc = false, bool chkSheet = false, bool chkOther = false, bool IsPrivate = false, bool IsFavorite = false, int tabListing = 0)
        {
            documentsViewModal.strdashbordsuccess = "";
            DateTime d = AdminSiteConfiguration.GetEasternTime(DateTime.Now);

            DateTime? stDate = null;
            DateTime? enDate = null;

            try
            {
                if (!string.IsNullOrEmpty(startdate))
                    stDate = DateTime.ParseExact(startdate, "MM/dd/yyyy", System.Globalization.CultureInfo.InvariantCulture);
            }
            catch (Exception ex)
            {
                logger.Error("DocumentsController - GridScroll(StDate) - " + DateTime.Now + " - " + ex.Message.ToString());
            }

            try
            {
                if (!string.IsNullOrEmpty(enddate))
                    enDate = DateTime.ParseExact(enddate, "MM/dd/yyyy", System.Globalization.CultureInfo.InvariantCulture);
            }
            catch (Exception ex)
            {
                logger.Error("DocumentsController - GridScroll(enDate) - " + DateTime.Now + " - " + ex.Message.ToString());
            }

            #region QueryText
            //,.eml,.msg,.rtf,.csv,
            string strFileType = ",.pdf,.txt,.doc,.docx,.jpg,.jpeg,.gif,.png,.xls,.xlsx,.mp4,.avi,.wmv,.mov,.mkv";
            if (chkDoc == true || chkImages == true || chkSheet == true || chkEmail == true || chkOther == true)
            {
                strFileType = "";

                if (chkDoc == true)
                {
                    strFileType += ",.doc,.docx,.pdf";
                }
                if (chkImages == true)
                {
                    strFileType += ",.jpg,.jpeg,.gif,.png";
                }
                if (chkSheet == true)
                {
                    strFileType += ",.xls,.xlsx,.csv";
                }
                if (chkEmail == true)
                {
                    strFileType += ",.eml,.msg";
                }
                if (chkOther == true)
                {
                    strFileType += ",.txt,.rtf";
                }

            }


            if (strFileType.Length > 0)
            {
                strFileType.Remove(0, 1);
            }

            #endregion

            List<Document> docGridList = new List<Document>();
            List<Document> PrivateGridList = new List<Document>();
            int StoreId = 0;
            var UserName = System.Web.HttpContext.Current.User.Identity.Name;
            //Db class for get user Id using username.
            var UserId = Convert.ToInt32(_commonRepository.getUserId(UserName));
            Boolean isadmin = Roles.IsUserInRole("Administrator");
            try
            {
                //Db class for get user Id using username.
                int CurrentUserId = Convert.ToInt32(_commonRepository.getUserId(UserName));

                if ((Convert.ToString(Session["storeid"]) != "0"))
                {
                    StoreId = Convert.ToInt16(Session["storeid"]);
                }

                ViewBag.storeid = StoreId;

                int startIndex = ((currentPageIndex - 1) * PageSize) + 1;
                int endIndex = startIndex + PageSize - 1;
                //This db class is get Document List.
                var documentlist = _documentsRepository.DocumentsList();
                if (SearchTitle != "")
                {
                    docGridList = documentlist.Where(s => s.IsDelete == false && s.StoreId == StoreId && s.IsPrivate == false && (s.Title.Trim().ToLower().Contains(SearchTitle.Trim().ToLower()) || s.DocumentKeywords.Select(k => k.Title.Trim().ToLower()).Contains(SearchTitle.Trim().ToLower()))).Select(s => new
                    {
                        DocumentId = s.DocumentId,
                        Type = "Doc",
                        Title = s.Title,
                        FilePath = s.FilePath,
                        AttachFile = s.AttachFile + s.AttachExtention,
                        AttachLink = "",
                        StoreName = s.StoreMasters.NickName,
                        CategoryName = s.DocumentCategories.Name,
                        CreatedOn = s.CreatedOn,
                        Notes = s.Notes,
                        Description = s.Description,
                        IsFavorite = (s.DocumentFavorites.Where(k => k.IsFavorite == true && (isadmin == true ? (k.UserId == k.UserId) : k.UserId == UserId)).Count() > 0 ? true : false),
                        IsDelete = s.IsDelete,
                        AttachExtention = s.AttachExtention,
                        IsStatus_id = s.CreatedBy,
                        FavId = s.DocumentFavorites.Where(k => k.IsFavorite == true && (isadmin == true ? (k.UserId == k.UserId) : k.UserId == UserId)).Count() > 0 ? s.DocumentFavorites.Where(k => k.IsFavorite == true && (isadmin == true ? (k.UserId == k.UserId) : k.UserId == UserId)).FirstOrDefault().DocumentFavoriteId : 0,
                        IsPrivate = s.IsPrivate,
                        DocumentCategoryId = s.DocumentCategoryId,
                        FavUserId = s.DocumentFavorites.Where(k => k.IsFavorite == true && (isadmin == true ? (k.UserId == k.UserId) : k.UserId == UserId)).Count() > 0 ? s.DocumentFavorites.Where(k => k.IsFavorite == true && (isadmin == true ? (k.UserId == k.UserId) : k.UserId == UserId)).FirstOrDefault().UserId : 0,
                    }).ToList()
              .Select(s => new Document
              {
                  DocumentId = s.DocumentId,
                  Type = s.Type,
                  Title = s.Title,
                  FilePath = s.FilePath,
                  AttachFile = s.AttachFile,
                  AttachLink = AdminSiteConfiguration.GetURL() + "\\UserFiles\\DocFiles\\" + s.FilePath + "\\" + s.AttachFile,
                  StoreName = s.StoreName,
                  CategoryName = s.CategoryName,
                  CreatedOn = s.CreatedOn,
                  CreatedDateFormated = AdminSiteConfiguration.GetDateformat(Convert.ToString(s.CreatedOn)),
                  Notes = s.Notes,
                  Description = s.Description,
                  IsFavorite = s.IsFavorite,
                  IsDelete = s.IsDelete,
                  AttachExtention = s.AttachExtention,
                  IsStatus_id = s.IsStatus_id.Value,
                  FavId = s.FavId,
                  IsPrivate = s.IsPrivate,
                  DocumentCategoryId = s.DocumentCategoryId,
                  FavUserId = s.FavUserId
              }).ToList();

                    PrivateGridList = documentlist.Where(s => s.IsDelete == false && s.StoreId == StoreId && s.IsPrivate == true && s.CreatedBy == CurrentUserId && (s.Title.Trim().ToLower().Contains(SearchTitle.Trim().ToLower()) || s.DocumentKeywords.Select(k => k.Title.Trim().ToLower()).Contains(SearchTitle.Trim().ToLower()))).Select(s => new
                    {
                        DocumentId = s.DocumentId,
                        Type = "Doc",
                        Title = s.Title,
                        FilePath = s.FilePath,
                        AttachFile = s.AttachFile + s.AttachExtention,
                        AttachLink = "",
                        StoreName = s.StoreMasters.NickName,
                        CategoryName = s.DocumentCategories.Name,
                        CreatedOn = s.CreatedOn,
                        Notes = s.Notes,
                        Description = s.Description,
                        IsFavorite = (s.DocumentFavorites.Where(k => k.IsFavorite == true && (isadmin == true ? (k.UserId == k.UserId) : k.UserId == UserId)).Count() > 0 ? true : false),
                        IsDelete = s.IsDelete,
                        AttachExtention = s.AttachExtention,
                        IsStatus_id = s.CreatedBy,
                        FavId = s.DocumentFavorites.Where(k => k.IsFavorite == true && (isadmin == true ? (k.UserId == k.UserId) : k.UserId == UserId)).Count() > 0 ? s.DocumentFavorites.Where(k => k.IsFavorite == true && (isadmin == true ? (k.UserId == k.UserId) : k.UserId == UserId)).FirstOrDefault().DocumentFavoriteId : 0,
                        IsPrivate = s.IsPrivate,
                        DocumentCategoryId = s.DocumentCategoryId,
                        FavUserId = s.DocumentFavorites.Where(k => k.IsFavorite == true && (isadmin == true ? (k.UserId == k.UserId) : k.UserId == UserId)).Count() > 0 ? s.DocumentFavorites.Where(k => k.IsFavorite == true && (isadmin == true ? (k.UserId == k.UserId) : k.UserId == UserId)).FirstOrDefault().UserId : 0,
                    }).ToList()
             .Select(s => new Document
             {
                 DocumentId = s.DocumentId,
                 Type = s.Type,
                 Title = s.Title,
                 FilePath = s.FilePath,
                 AttachFile = s.AttachFile,
                 AttachLink = AdminSiteConfiguration.GetURL() + "\\UserFiles\\DocFiles\\" + s.FilePath + "\\" + s.AttachFile,
                 StoreName = s.StoreName,
                 CategoryName = s.CategoryName,
                 CreatedOn = s.CreatedOn,
                 CreatedDateFormated = AdminSiteConfiguration.GetDateformat(Convert.ToString(s.CreatedOn)),
                 Notes = s.Notes,
                 Description = s.Description,
                 IsFavorite = s.IsFavorite,
                 IsDelete = s.IsDelete,
                 AttachExtention = s.AttachExtention,
                 IsStatus_id = s.IsStatus_id.Value,
                 FavId = s.FavId,
                 IsPrivate = s.IsPrivate,
                 DocumentCategoryId = s.DocumentCategoryId,
                 FavUserId = s.FavUserId
             }).ToList();
                    docGridList.AddRange(PrivateGridList);
                    docGridList = docGridList.OrderByDescending(o => o.DocumentId).ToList();
                }
                else
                {
                    docGridList = documentlist.Where(s => s.IsDelete == false && s.StoreId == StoreId && s.IsPrivate == false).Select(s => new
                    {
                        DocumentId = s.DocumentId,
                        Type = "Doc",
                        Title = s.Title,
                        FilePath = s.FilePath,
                        AttachFile = s.AttachFile + s.AttachExtention,
                        AttachLink = "",
                        StoreName = s.StoreMasters.NickName,
                        CategoryName = s.DocumentCategories.Name,
                        CreatedOn = s.CreatedOn,
                        Notes = s.Notes,
                        Description = s.Description,
                        IsFavorite = (s.DocumentFavorites.Where(k => k.IsFavorite == true && (isadmin == true ? (k.UserId == k.UserId) : k.UserId == UserId)).Count() > 0 ? true : false),
                        IsDelete = s.IsDelete,
                        AttachExtention = s.AttachExtention,
                        IsStatus_id = s.CreatedBy,
                        FavId = s.DocumentFavorites.Where(k => k.IsFavorite == true && (isadmin == true ? (k.UserId == k.UserId) : k.UserId == UserId)).Count() > 0 ? s.DocumentFavorites.Where(k => k.IsFavorite == true && (isadmin == true ? (k.UserId == k.UserId) : k.UserId == UserId)).FirstOrDefault().DocumentFavoriteId : 0,
                        IsPrivate = s.IsPrivate,
                        DocumentCategoryId = s.DocumentCategoryId,
                        FavUserId = s.DocumentFavorites.Where(k => k.IsFavorite == true && (isadmin == true ? (k.UserId == k.UserId) : k.UserId == UserId)).Count() > 0 ? s.DocumentFavorites.Where(k => k.IsFavorite == true && (isadmin == true ? (k.UserId == k.UserId) : k.UserId == UserId)).FirstOrDefault().UserId : 0,
                    }).ToList()
                                .Select(s => new Document
                                {
                                    DocumentId = s.DocumentId,
                                    Type = s.Type,
                                    Title = s.Title,
                                    FilePath = s.FilePath,
                                    AttachFile = s.AttachFile,
                                    AttachLink = AdminSiteConfiguration.GetURL() + "\\UserFiles\\DocFiles\\" + s.FilePath + "\\" + s.AttachFile,
                                    StoreName = s.StoreName,
                                    CategoryName = s.CategoryName,
                                    CreatedOn = s.CreatedOn,
                                    CreatedDateFormated = AdminSiteConfiguration.GetDateformat(Convert.ToString(s.CreatedOn)),
                                    Notes = s.Notes,
                                    Description = s.Description,
                                    IsFavorite = s.IsFavorite,
                                    IsDelete = s.IsDelete,
                                    AttachExtention = s.AttachExtention,
                                    IsStatus_id = s.IsStatus_id.Value,
                                    FavId = s.FavId,
                                    IsPrivate = s.IsPrivate,
                                    DocumentCategoryId = s.DocumentCategoryId,
                                    FavUserId = s.FavUserId
                                }).ToList();

                    PrivateGridList = documentlist.Where(s => s.IsDelete == false && s.StoreId == StoreId && s.IsPrivate == true && s.CreatedBy == CurrentUserId).Select(s => new
                    {
                        DocumentId = s.DocumentId,
                        Type = "Doc",
                        Title = s.Title,
                        FilePath = s.FilePath,
                        AttachFile = s.AttachFile + s.AttachExtention,
                        AttachLink = "",
                        StoreName = s.StoreMasters.NickName,
                        CategoryName = s.DocumentCategories.Name,
                        CreatedOn = s.CreatedOn,
                        Notes = s.Notes,
                        Description = s.Description,
                        IsFavorite = (s.DocumentFavorites.Where(k => k.IsFavorite == true && (isadmin == true ? (k.UserId == k.UserId) : k.UserId == UserId)).Count() > 0 ? true : false),
                        IsDelete = s.IsDelete,
                        AttachExtention = s.AttachExtention,
                        IsStatus_id = s.CreatedBy,
                        FavId = s.DocumentFavorites.Where(k => k.IsFavorite == true && (isadmin == true ? (k.UserId == k.UserId) : k.UserId == UserId)).Count() > 0 ? s.DocumentFavorites.Where(k => k.IsFavorite == true && (isadmin == true ? (k.UserId == k.UserId) : k.UserId == UserId)).FirstOrDefault().DocumentFavoriteId : 0,
                        IsPrivate = s.IsPrivate,
                        DocumentCategoryId = s.DocumentCategoryId,
                        FavUserId = s.DocumentFavorites.Where(k => k.IsFavorite == true && (isadmin == true ? (k.UserId == k.UserId) : k.UserId == UserId)).Count() > 0 ? s.DocumentFavorites.Where(k => k.IsFavorite == true && (isadmin == true ? (k.UserId == k.UserId) : k.UserId == UserId)).FirstOrDefault().UserId : 0,
                    }).ToList()
                               .Select(s => new Document
                               {
                                   DocumentId = s.DocumentId,
                                   Type = s.Type,
                                   Title = s.Title,
                                   FilePath = s.FilePath,
                                   AttachFile = s.AttachFile,
                                   AttachLink = AdminSiteConfiguration.GetURL() + "\\UserFiles\\DocFiles\\" + s.FilePath + "\\" + s.AttachFile,
                                   StoreName = s.StoreName,
                                   CategoryName = s.CategoryName,
                                   CreatedOn = s.CreatedOn,
                                   CreatedDateFormated = AdminSiteConfiguration.GetDateformat(Convert.ToString(s.CreatedOn)),
                                   Notes = s.Notes,
                                   Description = s.Description,
                                   IsFavorite = s.IsFavorite,
                                   IsDelete = s.IsDelete,
                                   AttachExtention = s.AttachExtention,
                                   IsStatus_id = s.IsStatus_id.Value,
                                   FavId = s.FavId,
                                   IsPrivate = s.IsPrivate,
                                   DocumentCategoryId = s.DocumentCategoryId,
                                   FavUserId = s.FavUserId
                               }).ToList();
                    docGridList.AddRange(PrivateGridList);
                    docGridList = docGridList.OrderByDescending(o => o.DocumentId).ToList();
                }

                if (stDate != null || enDate != null)
                {
                    stDate = stDate.Value.AddDays(-1);
                    enDate = enDate.Value.AddDays(1);
                    docGridList = docGridList.Where(s => s.CreatedOn.Value.Date > stDate.Value.Date && s.CreatedOn.Value.Date < enDate.Value.Date).ToList();
                }
                if (CategoryId > 0)
                {
                    docGridList = docGridList.Where(s => s.DocumentCategoryId == CategoryId).ToList();
                }
                if (strFileType != "")
                {
                    string[] TypeArr = strFileType.Split(',').ToArray();
                    docGridList = docGridList.Where(s => TypeArr.Contains(s.AttachExtention)).ToList();
                }

                ViewBag.TotalDoc = docGridList.Count();
                ViewBag.TotalPrivate = docGridList.Where(x => x.IsPrivate == true).Count();
                ViewBag.TotalFav = docGridList.Where(x => x.FavId > 0).Count();

                ViewBag.totalcount = docGridList.Count();
                if (tabListing == 1)
                {
                    docGridList = docGridList.Where(x => x.IsFavorite == true).ToList();
                    ViewBag.totalcount = docGridList.Count();
                }
                else if (tabListing == 2)
                {
                    docGridList = docGridList.Where(x => x.IsPrivate == true).ToList();
                    ViewBag.totalcount = docGridList.Count();
                }
                var ColumnName = typeof(Document).GetProperties().Where(p => p.Name == orderby).FirstOrDefault();

                if (IsAsc == 1)
                {
                    ViewBag.IsAscVal = 0;
                    docGridList = docGridList.OfType<Document>().ToList().OrderBy(n => ColumnName.GetValue(n, null)).Skip(startIndex - 1).Take(PageSize).ToList();
                }
                else
                {

                    ViewBag.IsAscVal = 1;

                    docGridList = docGridList.OfType<Document>().ToList().OrderByDescending(n => ColumnName.GetValue(n, null)).Skip(startIndex - 1).Take(PageSize).ToList();
                }
                documentsViewModal.StatusMessage = docGridList.Count() == 0 ? "NoItem" : "";
            }
            catch (Exception ex)
            {
                logger.Error("DocumentsController - PrivateGridList - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            //  List<DocumentGrid> docgridList = GetData(SearchRecords, CategoryId, SearchTitle, startdate, enddate, 1); //.OfType<DocumentGrid>().ToList();
            // TotalDataCount = docGrid.Count();
            //ViewBag.CategoryList = FillCategoryList();
            ViewBag.CategoryId = CategoryId;
            ViewBag.tabListing = tabListing;
            ViewBag.IsBindData = IsBindData;
            ViewBag.CurrentPageIndex = currentPageIndex;
            // ViewBag.LastPageIndex = this.getLastPageIndex(PageSize);
            ViewBag.OrderByVal = orderby;
            //ViewBag.IsAscVal = IsAsc;
            ViewBag.PageSize = PageSize;
            ViewBag.Alpha = Alpha;
            ViewBag.chkImages = chkImages;
            ViewBag.chkEmail = chkEmail;
            ViewBag.chkDoc = chkDoc;
            ViewBag.chkSheet = chkSheet;
            ViewBag.chkOther = chkOther;
            ViewBag.SearchTitle = SearchTitle;
            ViewBag.startindex = 1;
            ViewBag.startdate = startdate;
            ViewBag.enddate = enddate;

            if (documentsViewModal.strdashbordsuccess != null && documentsViewModal.strdashbordsuccess.Length > 25)
            {
                ViewBag.StatusMessage = documentsViewModal.strdashbordsuccess;
                documentsViewModal.strdashbordsuccess = "";
            }
            else
            {
                ViewBag.StatusMessage = documentsViewModal.StatusMessage;
            }

            if (TempData["msg"] != null && TempData["msg"].ToString().Length > 25)
            {
                ViewBag.StatusMessage = TempData["msg"].ToString();
            }

            for (int i = 0; i < docGridList.Count; i++)
            {
                var FileExt = docGridList[i].AttachExtention.ToLower();

                if (FileExt.Contains("pdf"))
                {
                    docGridList[i].FileTypeImage = "icon-pdf.svg";
                }
                else if (FileExt.Contains("jpg") || FileExt.Contains("jpeg"))
                {
                    docGridList[i].FileTypeImage = "icon-jpg.svg";
                }
                else if (FileExt.Contains("gif"))
                {
                    docGridList[i].FileTypeImage = "icon-gif.svg";
                }
                else if (FileExt.Contains("png"))
                {
                    docGridList[i].FileTypeImage = "icon-png.svg";
                }
                else if (FileExt.Contains("doc") || FileExt.Contains("docx"))
                {
                    docGridList[i].FileTypeImage = "icon-doc.svg";
                }
                else if (FileExt.Contains("xls") || FileExt.Contains("xlsx") || FileExt.Contains("csv"))
                {
                    docGridList[i].FileTypeImage = "icon-xls.svg";
                }
                else if (FileExt.Contains("txt") || FileExt.Contains("rtf"))
                {
                    docGridList[i].FileTypeImage = "icon-txt.svg";
                }
                else if (FileExt.Contains("eml") || FileExt.Contains("msg"))
                {
                    docGridList[i].FileTypeImage = "icon-eml.svg";
                }

            }

            docGridList = docGridList.Take(20).ToList();
            ViewBag.DrpCategory = new SelectList(db.DocumentCategories.Where(s => s.IsActive == true).OrderBy(o => o.Name), "DocumentCategoryId", "Name");
            return Json(docGridList, JsonRequestBehavior.AllowGet);
        }


        //Syncfusion grid Mydocument
        [Authorize(Roles = "Administrator,ViewDocument")]
        public ActionResult MyDocumentIndex()
        {
            ViewBag.Title = "My Documents - Synthesis";
            int storeid = Convert.ToInt32(Session["storeid"]);
            ViewBag.Storeidvalue = storeid;
            return View();
        }

        public ActionResult UrlDatasource(DataManagerRequest dm,DateTime? startdate,DateTime? enddate)
        {
            List<Document> HrDeVm = new List<Document>();
            List<Document> PrivateGridList = new List<Document>();
            IEnumerable<Document> DataSource = new List<Document>();
            int Count = 0;
            int StoreId = 0;
            if ((Convert.ToString(Session["storeid"]) != "0"))
            {
                StoreId = Convert.ToInt16(Session["storeid"]);
            }
            var UserName = System.Web.HttpContext.Current.User.Identity.Name;
            int CurrentUserId = Convert.ToInt32(_commonRepository.getUserId(UserName));
            var UserId = Convert.ToInt32(_commonRepository.getUserId(UserName));
            Boolean isadmin = Roles.IsUserInRole("Administrator");
            try
            {
                logger.Info("DocumentsController - UrlDatasource - " + DateTime.Now);
                var documentlist = _documentsRepository.DocumentsList();
                HrDeVm = documentlist.Where(s => s.IsDelete == false && s.StoreId == StoreId && s.IsPrivate == false).Select(s => new
                {
                    DocumentId = s.DocumentId,
                    Type = "Doc",
                    Title = s.Title,
                    FilePath = s.FilePath,
                    AttachFile = s.AttachFile + s.AttachExtention,
                    AttachLink = "",
                    StoreName = s.StoreMasters.NickName,
                    CategoryName = s.DocumentCategories.Name,
                    CreatedOn = s.CreatedOn,
                    Notes = s.Notes,
                    Description = s.Description,
                    IsFavorite = (s.DocumentFavorites.Where(k => k.IsFavorite == true && (isadmin == true ? (k.UserId == k.UserId) : k.UserId == UserId)).Count() > 0 ? true : false),
                    IsDelete = s.IsDelete,
                    AttachExtention = s.AttachExtention,
                    IsStatus_id = s.CreatedBy,
                    FavId = s.DocumentFavorites.Where(k => k.IsFavorite == true && (isadmin == true ? (k.UserId == k.UserId) : k.UserId == UserId)).Count() > 0 ? s.DocumentFavorites.Where(k => k.IsFavorite == true && (isadmin == true ? (k.UserId == k.UserId) : k.UserId == UserId)).FirstOrDefault().DocumentFavoriteId : 0,
                    IsPrivate = s.IsPrivate,
                    DocumentCategoryId = s.DocumentCategoryId,
                    FavUserId = s.DocumentFavorites.Where(k => k.IsFavorite == true && (isadmin == true ? (k.UserId == k.UserId) : k.UserId == UserId)).Count() > 0 ? s.DocumentFavorites.Where(k => k.IsFavorite == true && (isadmin == true ? (k.UserId == k.UserId) : k.UserId == UserId)).FirstOrDefault().UserId : 0,
                }).ToList()
                                .Select(s => new Document
                                {
                                    DocumentId = s.DocumentId,
                                    Type = s.Type,
                                    Title = s.Title,
                                    FilePath = s.FilePath,
                                    AttachFile = s.AttachFile,
                                    AttachLink = AdminSiteConfiguration.GetURL() + "\\UserFiles\\DocFiles\\" + s.FilePath + "\\" + s.AttachFile,
                                    StoreName = s.StoreName,
                                    CategoryName = s.CategoryName,
                                    CreatedOn = s.CreatedOn,
                                    CreatedDateFormated = AdminSiteConfiguration.GetDateformat(Convert.ToString(s.CreatedOn)),
                                    Notes = s.Notes,
                                    Description = s.Description,
                                    IsFavorite = s.IsFavorite,
                                    IsDelete = s.IsDelete,
                                    AttachExtention = s.AttachExtention,
                                    IsStatus_id = s.IsStatus_id.Value,
                                    FavId = s.FavId,
                                    IsPrivate = s.IsPrivate,
                                    DocumentCategoryId = s.DocumentCategoryId,
                                    FavUserId = s.FavUserId
                                }).ToList();

                PrivateGridList = documentlist.Where(s => s.IsDelete == false && s.StoreId == StoreId && s.IsPrivate == true && s.CreatedBy == CurrentUserId).Select(s => new
                {
                    DocumentId = s.DocumentId,
                    Type = "Doc",
                    Title = s.Title,
                    FilePath = s.FilePath,
                    AttachFile = s.AttachFile + s.AttachExtention,
                    AttachLink = "",
                    StoreName = s.StoreMasters.NickName,
                    CategoryName = s.DocumentCategories.Name,
                    CreatedOn = s.CreatedOn,
                    Notes = s.Notes,
                    Description = s.Description,
                    IsFavorite = (s.DocumentFavorites.Where(k => k.IsFavorite == true && (isadmin == true ? (k.UserId == k.UserId) : k.UserId == UserId)).Count() > 0 ? true : false),
                    IsDelete = s.IsDelete,
                    AttachExtention = s.AttachExtention,
                    IsStatus_id = s.CreatedBy,
                    FavId = s.DocumentFavorites.Where(k => k.IsFavorite == true && (isadmin == true ? (k.UserId == k.UserId) : k.UserId == UserId)).Count() > 0 ? s.DocumentFavorites.Where(k => k.IsFavorite == true && (isadmin == true ? (k.UserId == k.UserId) : k.UserId == UserId)).FirstOrDefault().DocumentFavoriteId : 0,
                    IsPrivate = s.IsPrivate,
                    DocumentCategoryId = s.DocumentCategoryId,
                    FavUserId = s.DocumentFavorites.Where(k => k.IsFavorite == true && (isadmin == true ? (k.UserId == k.UserId) : k.UserId == UserId)).Count() > 0 ? s.DocumentFavorites.Where(k => k.IsFavorite == true && (isadmin == true ? (k.UserId == k.UserId) : k.UserId == UserId)).FirstOrDefault().UserId : 0,
                }).ToList()
                           .Select(s => new Document
                           {
                               DocumentId = s.DocumentId,
                               Type = s.Type,
                               Title = s.Title,
                               FilePath = s.FilePath,
                               AttachFile = s.AttachFile,
                               AttachLink = AdminSiteConfiguration.GetURL() + "\\UserFiles\\DocFiles\\" + s.FilePath + "\\" + s.AttachFile,
                               StoreName = s.StoreName,
                               CategoryName = s.CategoryName,
                               CreatedOn = s.CreatedOn,
                               CreatedDateFormated = AdminSiteConfiguration.GetDateformat(Convert.ToString(s.CreatedOn)),
                               Notes = s.Notes,
                               Description = s.Description,
                               IsFavorite = s.IsFavorite,
                               IsDelete = s.IsDelete,
                               AttachExtention = s.AttachExtention,
                               IsStatus_id = s.IsStatus_id.Value,
                               FavId = s.FavId,
                               IsPrivate = s.IsPrivate,
                               DocumentCategoryId = s.DocumentCategoryId,
                               FavUserId = s.FavUserId
                           }).ToList();
                HrDeVm.AddRange(PrivateGridList);
                HrDeVm = HrDeVm.OrderByDescending(o => o.DocumentId).ToList();

                if (startdate != null || enddate != null)
                {
                    startdate = startdate.Value.AddDays(-1);
                    enddate = enddate.Value.AddDays(1);
                    HrDeVm = HrDeVm.Where(s => s.CreatedOn.Value.Date > startdate.Value.Date && s.CreatedOn.Value.Date < enddate.Value.Date).ToList();
                }

                for (int i = 0; i < HrDeVm.Count; i++)
                {
                    var FileExt = HrDeVm[i].AttachExtention.ToLower();

                    if (FileExt.Contains("pdf"))
                    {
                        HrDeVm[i].FileTypeImage = "icon-pdf.svg";
                    }
                    else if (FileExt.Contains("jpg") || FileExt.Contains("jpeg"))
                    {
                        HrDeVm[i].FileTypeImage = "icon-jpg.svg";
                    }
                    else if (FileExt.Contains("gif"))
                    {
                        HrDeVm[i].FileTypeImage = "icon-gif.svg";
                    }
                    else if (FileExt.Contains("png"))
                    {
                        HrDeVm[i].FileTypeImage = "icon-png.svg";
                    }
                    else if (FileExt.Contains("doc") || FileExt.Contains("docx"))
                    {
                        HrDeVm[i].FileTypeImage = "icon-doc.svg";
                    }
                    else if (FileExt.Contains("xls") || FileExt.Contains("xlsx") || FileExt.Contains("csv"))
                    {
                        HrDeVm[i].FileTypeImage = "icon-xls.svg";
                    }
                    else if (FileExt.Contains("txt") || FileExt.Contains("rtf"))
                    {
                        HrDeVm[i].FileTypeImage = "icon-txt.svg";
                    }
                    else if (FileExt.Contains("eml") || FileExt.Contains("msg"))
                    {
                        HrDeVm[i].FileTypeImage = "icon-eml.svg";
                    }
                    else if (FileExt.Contains("mp4") || FileExt.Contains("avi") || FileExt.Contains("wmv") || FileExt.Contains("mov") || FileExt.Contains("mkv"))
                    {
                        HrDeVm[i].FileTypeImage = "video.svg";
                    }

                }

                DataSource = HrDeVm;

                DataOperations operation = new DataOperations();
                if (dm.Search != null && dm.Search.Count > 0)
                {
                    string search = dm.Search[0].Key.ToString().Trim();
                    DataSource = DataSource.Where(x => x.Title.Contains(search) || x.StoreName.Contains(search) || x.CategoryName.Contains(search) || x.CreatedDateFormated.Contains(search)).ToList();
                    //DataSource = operation.PerformSearching(DataSource, dm.Search);  //Search
                }
                if (dm.Sorted != null && dm.Sorted.Count > 0) //Sorting
                {
                    foreach (var sort in dm.Sorted)
                    {
                        if (sort.Name == "CreatedDateFormated")
                        {
                            if (sort.Direction == "ascending")
                            {
                                DataSource = DataSource.OrderBy(x => x.CreatedOn).ToList();
                            }
                            else
                            {
                                DataSource = DataSource.OrderByDescending(x => x.CreatedOn).ToList();
                            }
                        }
                        else
                        {
                            DataSource = operation.PerformSorting(DataSource, dm.Sorted);
                        }
                    }
                }
                if (dm.Where != null && dm.Where.Count > 0) //Filtering
                {
                    DataSource = operation.PerformFiltering(DataSource, dm.Where, dm.Where[0].Operator);
                }
                Count = DataSource.Cast<Document>().Count();
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
                logger.Error("DocumentsController - UrlDatasource - " + DateTime.Now + " - " + ex.Message.ToString());
            }

            return dm.RequiresCounts ? Json(new { result = DataSource, count = Count }) : Json(DataSource);
        }
    }
}
