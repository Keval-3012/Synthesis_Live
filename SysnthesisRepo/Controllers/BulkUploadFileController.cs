using EntityModels.Models;
using NLog;
using Repository;
using Repository.IRepository;
using SynthesisViewModal;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Utility;

namespace SysnthesisRepo.Controllers
{
    public class BulkUploadFileController : Controller
    {
        protected static string Cloudurl;
        protected static string StatusMessage = "";
        protected static string ActivityLogMessage = "";
        protected static string DeleteMessage = "";
        private const int FirstPageIndex = 1;
        protected static int TotalDataCount;
        protected static int RtnDatalistcount;
        protected static Array Arr;
        protected static bool IsArray;
        protected static IEnumerable BindData;
        protected static bool IsEdit = false;
        protected static string IsFilter = "0";
        protected static string paymethod = "";
        protected static int currentPageIndex;
        protected static string strdashbordsuccess;
        protected static string ExistCode = "";
        protected static int SaveFile = 0;
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private readonly ISynthesisApiRepository _synthesisApiRepository;
        private readonly ICommonRepository _commonRepository;
        private readonly IBulkUploadRepository _bulkUploadRepository;
        private readonly ICompaniesRepository _CompaniesRepository;
        private readonly IUserMastersRepository _userMastersRepository;
        private readonly IUserActivityLogRepository _activityLogRepository;
        private readonly IInvoiceRepository _invoiceRepository;

        public BulkUploadFileController()
        {
            this._synthesisApiRepository = new SynthesisApiRepository();
            this._commonRepository = new CommonRepository(new DBContext());
            this._bulkUploadRepository = new BulkUploadRepository(new DBContext());
            this._CompaniesRepository = new CompaniesRepository(new DBContext());
            this._userMastersRepository = new UserMastersRepository(new DBContext());
            this._activityLogRepository = new UserActivityLogRepository(new DBContext());
            this._invoiceRepository = new InvoiceRepository(new DBContext());
        }
        /// <summary>
        /// This method is  the Index view page
        /// </summary>
        /// <param name="ShowMyInvoice"></param>
        /// <param name="MSG"></param>
        /// <returns></returns>
        [Authorize(Roles = "Administrator,ViewBulkUploadFile")]
        public ActionResult Index(bool ShowMyInvoice = true, string MSG = "")
        {
            ViewBag.Title = "Add Bulk Invoices - Synthesis";
            _commonRepository.LogEntries();     //Harsh's code
            ViewBag.ShowMyInvoice = ShowMyInvoice;
            if (StatusMessage == "Success1" || StatusMessage == "Success" || StatusMessage == "Delete")
            {
                ViewBag.StatusMessage = StatusMessage;
            }
            else
            {
                ViewBag.StatusMessage = "";
            }
            return View();
        }

        /// <summary>
        /// This class is used to save bulk file
        /// </summary>
        [AcceptVerbs("Post")]
        public void Save()
        {
            SaveFile++;
            AdminSiteConfiguration.WriteErrorLogs("Bulk File Upload Count:" + SaveFile);
            try
            {
                int? StoreID = null;
                string Sacn_Title = "";
                if (Session["StoreId"] != null && !string.IsNullOrEmpty(Session["StoreId"].ToString()))
                {
                    StoreID = Convert.ToInt32(Session["StoreId"]);
                }
                if (System.Web.HttpContext.Current.Request.Files.AllKeys.Length > 0)
                {
                    var httpPostedFile = System.Web.HttpContext.Current.Request.Files["UploadFiles"];
                    Sacn_Title = AdminSiteConfiguration.GetRandomNo() + Path.GetFileName(httpPostedFile.FileName);
                    Sacn_Title = AdminSiteConfiguration.RemoveSpecialCharacter(Sacn_Title);
                    if (httpPostedFile != null)
                    {
                        var fileSave = System.Web.HttpContext.Current.Server.MapPath("~/UserFiles/BulkUploadFile");
                        if (!System.IO.File.Exists(fileSave))
                        {
                            System.IO.Directory.CreateDirectory(fileSave);
                        }
                        var fileSavePath = Path.Combine(fileSave, Sacn_Title);
                        if (!System.IO.File.Exists(fileSavePath))
                        {
                            httpPostedFile.SaveAs(fileSavePath);
                            UploadPdf uploadPDF = new UploadPdf();
                            uploadPDF.FileName = Sacn_Title;
                            uploadPDF.IsProcess = 0;
                            uploadPDF.CreatedBy = UserModule.getUserId();
                            uploadPDF.CreatedDate = DateTime.Now;
                            uploadPDF.UpdatedBy = null;
                            uploadPDF.UpdatedDate = null;
                            uploadPDF.PageCount = 0;
                            uploadPDF.ReadyForProcess = 1;
                            uploadPDF.StoreId = StoreID == 0 ? null : StoreID;
                            //This class is save file changes for upload PDF
                            _bulkUploadRepository.SaveFileChanges(uploadPDF);
                            HttpResponse Response = System.Web.HttpContext.Current.Response;
                            Response.Clear();
                            Response.ContentType = "application/json; charset=utf-8";
                            Response.StatusDescription = "File uploaded succesfully";
                            Response.End();
                            AdminSiteConfiguration.WriteErrorLogs(Sacn_Title + " File uploaded succesfully");
                        }
                        else
                        {
                            HttpResponse Response = System.Web.HttpContext.Current.Response;
                            Response.Clear();
                            Response.Status = "204 File already exists";
                            Response.StatusCode = 204;
                            Response.StatusDescription = "File already exists";
                            Response.End();
                            AdminSiteConfiguration.WriteErrorLogs(Sacn_Title + " File already exists.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("BulkUploadFileController - Save - " + DateTime.Now + " - " + ex.Message.ToString());
                AdminSiteConfiguration.WriteErrorLogs(ex.Message);
                HttpResponse Response = System.Web.HttpContext.Current.Response;
                Response.Clear();
                Response.ContentType = "application/json; charset=utf-8";
                Response.StatusCode = 204;
                Response.Status = "204 No Content";
                Response.StatusDescription = ex.Message;
                Response.End();
            }
        }

        /// <summary>
        /// This method is return Upload PDF view
        /// </summary>
        /// <returns></returns>
        public ActionResult UploadPDF()
        {
            UploadPdf pdf = new UploadPdf();
            return View(pdf);
        }

        /// <summary>
        /// This method is Upload Invoice PDF
        /// </summary>
        /// <param name="invoice"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult UploadPDF(Invoice invoice)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    foreach (string file in Request.Files)
                    {
                        HttpPostedFileBase hpf = Request.Files[file] as HttpPostedFileBase;
                    }
                    var pic = System.Web.HttpContext.Current.Request.Files["DocFiles"];
                    HttpFileCollectionBase files = Request.Files;
                    //foreach (HttpFileCollectionBase postedFile in files)
                    for (int i = 0; i < files.Count; i++)
                    {
                        HttpPostedFileBase file = files[i];
                        if (file != null)
                        {
                            string FileExtension = Path.GetExtension(file.FileName);
                            string FileName = file.FileName;
                            string UploadPath = "~/userfiles/docfile/" + FileName;
                            file.SaveAs(Server.MapPath(UploadPath));
                            UploadPdf uploadPDF = new UploadPdf();
                            uploadPDF.FileName = FileName;
                            uploadPDF.IsProcess = 0;
                            uploadPDF.CreatedBy = UserModule.getUserId();
                            uploadPDF.CreatedDate = DateTime.Now;
                            uploadPDF.UpdatedBy = null;
                            uploadPDF.UpdatedDate = null;
                            uploadPDF.PageCount = 0;
                            uploadPDF.ReadyForProcess = 1;
                            uploadPDF.StoreId = invoice.StoreId;
                            //This class is save file changes for upload PDF
                            _bulkUploadRepository.SaveFileChanges(uploadPDF);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("BulkUploadFileController - UploadPDF - " + DateTime.Now + " - " + ex.Message.ToString());
            }
         
            return Json("True", JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// This method is Upload Files Process
        /// </summary>
        /// <param name="DocFiles"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult UploapFilesProcess(HttpPostedFileBase DocFiles)
        {
            try
            {
                int StoreID = 0;
                if (Session["StoreId"] != null && !string.IsNullOrEmpty(Session["StoreId"].ToString()))
                {
                    StoreID = Convert.ToInt32(Session["StoreId"]);
                }
                foreach (string file in Request.Files)
                {
                    HttpPostedFileBase hpf = Request.Files[file] as HttpPostedFileBase;
                }
                var pic = System.Web.HttpContext.Current.Request.Files["DocFiles"];

                HttpFileCollectionBase files = Request.Files;

                for (int i = 0; i < files.Count; i++)
                {
                    HttpPostedFileBase file = files[i];
                    if (file != null)
                    {
                        string FileExtension = Path.GetExtension(file.FileName);
                        string FileName = file.FileName;
                        string UploadPath = "~/userfiles/scanimage/" + FileName;
                        file.SaveAs(Server.MapPath(UploadPath));
                        UploadPdf uploadPDF = new UploadPdf();
                        uploadPDF.FileName = FileName;
                        uploadPDF.IsProcess = 0;
                        uploadPDF.CreatedBy = UserModule.getUserId();
                        uploadPDF.CreatedDate = DateTime.Now;
                        uploadPDF.UpdatedBy = null;
                        uploadPDF.UpdatedDate = null;
                        uploadPDF.PageCount = 0;
                        uploadPDF.ReadyForProcess = 1;
                        uploadPDF.StoreId = StoreID;
                        //This class is save file changes for upload PDF
                        _bulkUploadRepository.SaveFileChanges(uploadPDF);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("BulkUploadFileController - UploapFilesProcess - " + DateTime.Now + " - " + ex.Message.ToString());
                return Json(ex, JsonRequestBehavior.AllowGet);
            }
            return Json("True", JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// This method is Get Bulk Invoice List
        /// </summary>
        /// <param name="IsBindData"></param>
        /// <param name="currentPageIndex"></param>
        /// <param name="orderby"></param>
        /// <param name="IsAsc"></param>
        /// <param name="PageSize"></param>
        /// <param name="SearchRecords"></param>
        /// <param name="Alpha"></param>
        /// <param name="deptname"></param>
        /// <param name="startdate"></param>
        /// <param name="enddate"></param>
        /// <param name="payment"></param>
        /// <param name="Store_val"></param>
        /// <param name="searchdashbord"></param>
        /// <param name="AmtMaximum"></param>
        /// <param name="AmtMinimum"></param>
        /// <param name="ShowMyInvoice"></param>
        /// <returns></returns>
        public ActionResult BulkInvoiceList(int IsBindData = 1, int currentPageIndex = 1, string orderby = "UploadPdfId", int IsAsc = 0, int PageSize = 20, int SearchRecords = 1, string Alpha = "", int deptname = 0, string startdate = "", string enddate = "", string payment = "", string Store_val = "", string searchdashbord = "", string AmtMaximum = "0", string AmtMinimum = "0", bool ShowMyInvoice = false)
        {
            BulkUploadViewModal bulkUploadViewModal = new BulkUploadViewModal();
            bulkUploadViewModal.IsBindData = IsBindData;
            bulkUploadViewModal.currentPageIndex = currentPageIndex;
            bulkUploadViewModal.orderby = orderby;
            bulkUploadViewModal.IsAsc = IsAsc;
            bulkUploadViewModal.PageSize = PageSize;
            bulkUploadViewModal.SearchRecords = SearchRecords;
            bulkUploadViewModal.Alpha = Alpha;
            bulkUploadViewModal.deptname = deptname;
            bulkUploadViewModal.startdate = startdate;
            bulkUploadViewModal.enddate = enddate;
            bulkUploadViewModal.payment = payment;
            bulkUploadViewModal.Store_val = Store_val;
            bulkUploadViewModal.searchdashbord = searchdashbord;
            bulkUploadViewModal.AmtMaximum = AmtMaximum;
            bulkUploadViewModal.AmtMinimum = AmtMinimum;
            bulkUploadViewModal.ShowMyInvoice = ShowMyInvoice;

            if (Session["StoreId"] != null)
            {
                if (!string.IsNullOrEmpty(Session["StoreId"].ToString()))
                {

                    string store_idval = Session["StoreId"].ToString();
                    ViewBag.Storeidvalue = store_idval;
                }
            }
            int startcount = 0;
            int endcount = 0;
            #region MyRegion_Array
            try
            {

                if (IsArray == true)
                {
                    foreach (string a1 in Arr)
                    {
                        if (a1.Split(':')[0].ToString() == "IsBindData")
                        {
                            bulkUploadViewModal.IsBindData = Convert.ToInt32(a1.Split(':')[1].ToString());
                        }
                        if (a1.Split(':')[0].ToString() == "currentPageIndex")
                        {
                            bulkUploadViewModal.currentPageIndex = Convert.ToInt32(a1.Split(':')[1].ToString());
                        }
                        if (a1.Split(':')[0].ToString() == "orderby")
                        {
                            bulkUploadViewModal.orderby = Convert.ToString(a1.Split(':')[1].ToString());
                        }
                        if (a1.Split(':')[0].ToString() == "IsAsc")
                        {
                            bulkUploadViewModal.IsAsc = Convert.ToInt32(a1.Split(':')[1].ToString());
                        }
                        if (a1.Split(':')[0].ToString() == "PageSize")
                        {
                            bulkUploadViewModal.PageSize = Convert.ToInt32(a1.Split(':')[1].ToString());
                        }
                        if (a1.Split(':')[0].ToString() == "SearchRecords")
                        {
                            bulkUploadViewModal.SearchRecords = Convert.ToInt32(a1.Split(':')[1].ToString());
                        }
                        if (a1.Split(':')[0].ToString() == "Alpha")
                        {
                            bulkUploadViewModal.Alpha = Convert.ToString(a1.Split(':')[1].ToString());
                        }
                        if (a1.Split(':')[0].ToString() == "deptname")
                        {
                            bulkUploadViewModal.deptname = Convert.ToInt32(a1.Split(':')[1].ToString());
                        }
                        if (a1.Split(':')[0].ToString() == "startdate")
                        {
                            bulkUploadViewModal.startdate = Convert.ToString(a1.Split(':')[1].ToString());
                        }
                        if (a1.Split(':')[0].ToString() == "enddate")
                        {
                            bulkUploadViewModal.enddate = Convert.ToString(a1.Split(':')[1].ToString());
                        }
                        if (a1.Split(':')[0].ToString() == "payment")
                        {
                            bulkUploadViewModal.payment = Convert.ToString(a1.Split(':')[1].ToString());
                        }
                        if (a1.Split(':')[0].ToString() == "Store_val")
                        {
                            bulkUploadViewModal.Store_val = Convert.ToString(a1.Split(':')[1].ToString());
                        }
                        if (a1.Split(':')[0].ToString() == "searchdashbord")
                        {
                            bulkUploadViewModal.searchdashbord = Convert.ToString(a1.Split(':')[1].ToString());
                        }
                        if (a1.Split(':')[0].ToString() == "AmtMinimum")
                        {
                            bulkUploadViewModal.AmtMinimum = Convert.ToString(a1.Split(':')[1].ToString());
                        }
                        if (a1.Split(':')[0].ToString() == "AmtMaximum")
                        {
                            bulkUploadViewModal.AmtMaximum = Convert.ToString(a1.Split(':')[1].ToString());
                        }
                    }
                }
            }
            catch(Exception ex) 
            { 
            	logger.Error("BulkUploadFileController - BulkInvoiceList - " + DateTime.Now + " - " + ex.Message.ToString()); 
            }

            IsArray = false;
            Arr = new string[]
            {  "IsBindData:" + bulkUploadViewModal.IsBindData
                ,"currentPageIndex:" + bulkUploadViewModal.currentPageIndex
                ,"orderby:" + bulkUploadViewModal.orderby
                ,"IsAsc:" + bulkUploadViewModal.IsAsc
                ,"PageSize:" + bulkUploadViewModal.PageSize
                ,"SearchRecords:" + bulkUploadViewModal.SearchRecords
                ,"Alpha:" + bulkUploadViewModal.Alpha
               ,"deptname:" + bulkUploadViewModal.deptname
               ,"startdate:"+ bulkUploadViewModal.startdate
                ,"enddate:"+ bulkUploadViewModal.enddate
                ,"payment:"+ bulkUploadViewModal.payment
                ,"Store_val:"+ bulkUploadViewModal.Store_val
               ,"searchdashbord:"+ bulkUploadViewModal.searchdashbord
               ,"AmtMaximum:"+ bulkUploadViewModal.AmtMaximum
                ,"AmtMinimum:"+ bulkUploadViewModal.AmtMinimum
            };
            #endregion

            #region MyRegion_BindData
            IEnumerable Data = null;
            List<ddllist> Lstdept = new List<ddllist>();
            try
            {
                int startIndex = ((currentPageIndex - 1) * PageSize) + 1;
                int endIndex = startIndex + PageSize - 1;
                int ds = 0;
                if (IsBindData == 1 || IsEdit == true)
                {

                    BindData = GetData(bulkUploadViewModal).OfType<UploadFile>().ToList();
                    TotalDataCount = BindData.OfType<UploadFile>().ToList().Count();
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
                ViewBag.AmtMaximum = AmtMaximum;
                ViewBag.AmtMinimum = AmtMinimum;

                if (strdashbordsuccess == "Success1" || strdashbordsuccess == "Success")
                {
                    ViewBag.StatusMessage = strdashbordsuccess;
                    strdashbordsuccess = "";
                }
                else
                {
                    ViewBag.StatusMessage = StatusMessage;
                }
                ViewBag.TotalDataCount = TotalDataCount;
                var ColumnName = typeof(UploadFile).GetProperties().Where(p => p.Name == orderby).FirstOrDefault();

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
            ViewBag.AmtMaximum = AmtMaximum;
            ViewBag.AmtMinimum = AmtMinimum;

                if (IsAsc == 1)
                {
                    ViewBag.AscVal = 0;
                    Data = BindData.OfType<UploadFile>().ToList().OrderBy(n => ColumnName.GetValue(n, null));
                }
                else
                {
                    ViewBag.AscVal = 1;
                    Data = BindData.OfType<UploadFile>().ToList().OrderByDescending(n => ColumnName.GetValue(n, null));
                }
                StatusMessage = "";
                ViewBag.Datadept = Lstdept;

                ViewBag.Invoicecount = TotalDataCount;
                #endregion

                ViewBag.InvoicesRevi = TotalDataCount;
                ViewBag.ShowMyInvoice = ShowMyInvoice;
            }
            catch (Exception ex)
            {
                logger.Error("BulkUploadFileController - BulkInvoiceList - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            StatusMessage = "";
            ViewBag.Datadept = Lstdept;

            ViewBag.Invoicecount = TotalDataCount;

            ViewBag.InvoicesRevi = TotalDataCount;
            ViewBag.ShowMyInvoice = ShowMyInvoice;
            return View(Data);
        }

        /// <summary>
        /// This methid is Post Bulk Invoice List
        /// </summary>
        /// <param name="invoice"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult BulkInvoiceList(Invoice invoice)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    foreach (string file in Request.Files)
                    {
                        HttpPostedFileBase hpf = Request.Files[file] as HttpPostedFileBase;
                    }
                    var pic = System.Web.HttpContext.Current.Request.Files["DocFiles"];

                    HttpFileCollectionBase files = Request.Files;
                    for (int i = 0; i < files.Count; i++)
                    {
                        HttpPostedFileBase file = files[i];
                        if (file != null)
                        {
                            string FileExtension = Path.GetExtension(file.FileName);
                            string FileName = file.FileName;
                            string UploadPath = "~/UserFiles/DocFile/" + FileName;
                            file.SaveAs(Server.MapPath(UploadPath));
                            UploadPdf uploadPDF = new UploadPdf();
                            uploadPDF.FileName = FileName;
                            uploadPDF.IsProcess = 0;
                            uploadPDF.CreatedBy = UserModule.getUserId();
                            uploadPDF.CreatedDate = DateTime.Now;
                            uploadPDF.UpdatedBy = null;
                            uploadPDF.UpdatedDate = null;
                            uploadPDF.PageCount = 0;
                            uploadPDF.ReadyForProcess = 1;
                            uploadPDF.StoreId = invoice.StoreId;
                            //This class is save file changes for upload PDF
                            _bulkUploadRepository.SaveFileChanges(uploadPDF);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("BulkUploadFileController - BulkInvoiceList - " + DateTime.Now + " - " + ex.Message.ToString());
            }
         
            return Json("True", JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// This method is search File Upload List
        /// </summary>
        /// <param name="Search"></param>
        /// <returns></returns>
        public ActionResult FileuploadList(string Search)
        {
            int StoreId = Convert.ToInt32(Session["StoreId"]);
            List<UploadFile> list = new List<UploadFile>();
            try
            {
                if (Search != null && Search != "")
                {
                    if (StoreId != 0)
                    {
                        //This class is Search store process by storeid
                        list = _bulkUploadRepository.StoreProcess(Search, StoreId);
                    }
                    else
                    {
                        //This class is Without Search store process
                        list = _bulkUploadRepository.WithoutStoreProcess(Search);
                    }
                }
                else
                {
                    if (StoreId != 0)
                    {
                        //This class is With store process by StoreiD 
                        list = _bulkUploadRepository.WithStoreProcess(StoreId);
                    }
                    else
                    {
                        // This class is With store processing
                        list = _bulkUploadRepository.WithStoreProcessing();
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("BulkUploadFileController - FileuploadList - " + DateTime.Now + " - " + ex.Message.ToString());
            }
         
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        ///  This class use to get last page index with pagesize.
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
        /// This method is get Bulk Upload Model
        /// </summary>
        /// <param name="bulkUploadViewModal"></param>
        /// <returns></returns>
        private IEnumerable GetData(BulkUploadViewModal bulkUploadViewModal)
        {

            if (bulkUploadViewModal.AmtMaximum == "" || bulkUploadViewModal.AmtMaximum == " " || bulkUploadViewModal.AmtMaximum == null)
            {
                bulkUploadViewModal.AmtMaximum = "0";
            }
            if (bulkUploadViewModal.AmtMinimum == "" || bulkUploadViewModal.AmtMinimum == " " || bulkUploadViewModal.AmtMinimum == null)
            {
                bulkUploadViewModal.AmtMinimum = "0";
            }
            int userid = UserModule.getUserId();
            DateTime? start_date = null;
            DateTime? end_date = null;
            try
            {
                if (!string.IsNullOrEmpty(bulkUploadViewModal.startdate))
                    start_date = Convert.ToDateTime(bulkUploadViewModal.startdate);
            }
            catch (Exception ex) { logger.Error("BulkUploadFileController - GetData - " + DateTime.Now + " - " + ex.Message.ToString()); }

            try
            {
                if (!string.IsNullOrEmpty(bulkUploadViewModal.enddate))
                    end_date = Convert.ToDateTime(bulkUploadViewModal.enddate);
            }
            catch(Exception ex) { logger.Error("BulkUploadFileController - GetData - " + DateTime.Now + " - " + ex.Message.ToString()); }
            IEnumerable RtnData = null;

            #region dashbord grid and search
            string storeid = "";
            int StoreID = 0;
            try
            {
                if (Session["StoreId"] != null && !string.IsNullOrEmpty(Session["StoreId"].ToString()))
                {
                    storeid = Session["StoreId"].ToString();
                    StoreID = Convert.ToInt32(Session["StoreId"]);
                }
                else
                {
                    RedirectToAction("index", "login");
                }
                //Get all Store mAsters data
                var StoreList = _CompaniesRepository.GetAllStoreMasters();
                //Get all Users mAsters data

                var UserMaster = _userMastersRepository.GetAllUserMasters();
                if (bulkUploadViewModal.ShowMyInvoice == false)
                {
                    //this db class is Get Upload Files list
                    RtnData = _bulkUploadRepository.GetUploadFiles(StoreID, 0, StoreList, UserMaster).ToList();
                }
                else
                {
                    //this db class is Get Upload Files list

                    RtnData = _bulkUploadRepository.GetUploadFiles(StoreID, userid, StoreList, UserMaster).ToList();
                }
            }
            catch (Exception ex)
            {
                logger.Error("BulkUploadFileController - GetData - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            
            #endregion
            return RtnData;
        }
        /// <summary>
        /// This method is delete Upload File
        /// </summary>
        /// <param name="id"></param>
        /// <param name="From"></param>
        /// <returns></returns>
        public async Task<ActionResult> Delete(int? id, string From)
        {
            try
            {

                //UploadPdf uploadPDF = await db.UploadPdfs.FindAsync(id);
                UploadPdf uploadPDF = _invoiceRepository.CreateSplitInvoice(id.Value);

                var FileName = uploadPDF.FileName;
                var fileSave = "";
                var fileMove = "";
                fileSave = System.Web.HttpContext.Current.Server.MapPath("~/UserFiles/BulkUploadFile");
                fileMove = System.Web.HttpContext.Current.Server.MapPath("~/UserFiles/DeleteBulkUploadFile");
                if (!System.IO.File.Exists(fileMove))
                {
                    System.IO.Directory.CreateDirectory(fileMove);
                }
                var fileSavePath = Path.Combine(fileSave, uploadPDF.FileName);
                var fileMovePath = Path.Combine(fileMove, uploadPDF.FileName);
                if (System.IO.File.Exists(fileSavePath))
                {
                    System.IO.File.Move(fileSavePath, fileMovePath);
                    AdminSiteConfiguration.WriteErrorLogs("File successfully moved to another folder.");
                }
                //This class is save file changes for upload PDF
                _bulkUploadRepository.RemoveFileChanges(uploadPDF);

                ActivityLog ActLog = new ActivityLog();
                ActLog.Action = 3;
                ActLog.Comment = "Bulk Upload file Deleted by " + UserModule.getUserFirstName() + " on " + AdminSiteConfiguration.GetDate(AdminSiteConfiguration.GetEasternTime(DateTime.Now).ToString());
                //This  class is used for Activity Log Insert.
                _activityLogRepository.ActivityLogInsert(ActLog);
                StatusMessage = "Delete";
                DeleteMessage = "Deleted successfully.";
                ViewBag.Delete = DeleteMessage;
                AdminSiteConfiguration.WriteErrorLogs(FileName + " File removed successfully and move to another folder.");
            }
            catch (Exception ex)
            {
                logger.Error("BulkUploadFileController - Delete - " + DateTime.Now + " - " + ex.Message.ToString());
                DeleteMessage = "Something went rong..";
                ViewBag.Delete = DeleteMessage;
            }
            return null;
        }
    }
}