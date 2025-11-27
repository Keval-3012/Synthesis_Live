using EntityModels.Models;
using Repository;
using Repository.IRepository;
using Syncfusion.EJ2.Spreadsheet;
using Syncfusion.XlsIO;
//using Synthesis.Class;
//using SynthesisCF.Models;
using SynthesisQBOnline;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Utility;

namespace SynthesisRepo.Controllers
{
    public class SpreadsheetController : Controller
    {
        // GET: Spreadsheet
        private DBContext db = new DBContext();
        private readonly IUserActivityLogRepository _ActivityLogRepository;

        public SpreadsheetController()
        {
            this._ActivityLogRepository = new UserActivityLogRepository(new DBContext());
        }
        public ActionResult SpreadSheet(int Ids)
        {
            ViewBag.id = Ids;
            Document document = new Document();
            try
            {
                string value = null;
                if (Session["value"] != null)
                {
                    value = Session["value"].ToString();
                }
                if (String.IsNullOrEmpty(value)) {
                    document = db.Documents.Find(Ids);
                    var FullPath = "~/UserFiles/DocFiles" + document.FilePath.Replace("\\", "/") + "/" + document.AttachFile + document.AttachExtention;
                    ViewBag.AttachLink = FullPath;
                    ViewBag.FileName = document.AttachFile + document.AttachExtention;
                    ViewBag.AttachFile = document.AttachFile;
                }
                else
                {
                    document = db.Documents.Find(Ids);
                    //var FullPath = AdminSiteConfiguration.GetURL() + "UserFiles/DocFiles" + document.FilePath.Replace("\\", "/") + "/" + document.AttachFile + document.AttachExtention;

                    var FullPath = "~/UserFiles/DocFiles" + document.FilePath.Replace("\\", "/") + "/" + document.AttachFile + document.AttachExtention;
                    ViewBag.AttachLink = FullPath;
                    ViewBag.FileName = document.AttachFile + document.AttachExtention;
                    ViewBag.AttachFile = document.AttachFile;
                    ViewBag.Message = value;
                }
                if (Session["value"] != null)
                {
                    Session["value"] = null;
                }
                
            }
            catch (Exception ex)
            {
            }
            return View();
        }
        public string LoadExcel(FileOptions file)
        {
            ExcelEngine excelEngine = new ExcelEngine();
            IWorkbook workbook;

            FileStream fs = System.IO.File.Open(HttpContext.Server.MapPath(file.FilePath), FileMode.Open); // converting excel file to stream 
            workbook = excelEngine.Excel.Workbooks.Open(fs, ExcelOpenType.Automatic);
            MemoryStream outputStream = new MemoryStream();
            workbook.SaveAs(outputStream);
            HttpPostedFileBase fileBase = (HttpPostedFileBase)new HttpPostedFile(outputStream.ToArray(), file.FileName);
            HttpPostedFileBase[] files = new HttpPostedFileBase[1];
            files[0] = fileBase;
            OpenRequest open = new OpenRequest();
            open.File = files;
            fs.Close();
            return Workbook.Open(open);
        }
        //public ActionResult Open(OpenRequest openRequest)
        //{
        //    return Content(Workbook.Open(openRequest));
        //}
        public class FileOptions
        {
            public string FileName { get; set; }
            public string FilePath { get; set; }
        }

        public class HttpPostedFile : HttpPostedFileBase
        {
            private readonly byte[] fileBytes;
            public HttpPostedFile(byte[] fileBytes, string fileName)
            {
                this.fileBytes = fileBytes;
                this.InputStream = new MemoryStream(fileBytes);
                this.FileName = fileName + ".xlsx";
            }
            public override int ContentLength => fileBytes.Length;
            public override string FileName { get; }
            public override Stream InputStream { get; }
        }
        public ActionResult Save(SaveSettings saveSettings)
        {
            string value = "0";
            try
            {
                ExcelEngine excelEngine = new ExcelEngine();
                IApplication application = excelEngine.Excel;
                // Convert Spreadsheet data as Stream 
                Stream fileStream = Workbook.Save<Stream>(saveSettings);
                IWorkbook workbook = application.Workbooks.Open(fileStream);

                var Array = saveSettings.FileName.Split('_');
                Document document = new Document();
                document = db.Documents.Find(Convert.ToInt32(Array[2]));
                int documnet = Convert.ToInt32(Array[2]);
                if (saveSettings.FileName == document.AttachFile)
                {
                    var FullPath = Server.MapPath("~\\UserFiles\\DocFiles" + document.FilePath);
                    var filePath = FullPath + @"\" + saveSettings.FileName + document.AttachExtention;
                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }

                    FileStream outputStream = new FileStream(filePath, FileMode.Create);
                    workbook.SaveAs(outputStream);

                    var version = db.DocumentFileVersions.Where(w => w.DocumentId == documnet).Count() > 0 ? db.DocumentFileVersions.Where(w => w.DocumentId == documnet).Max(m => m.Version + 1).ToString() : "1";
                    if (String.IsNullOrEmpty(version))
                    {
                        version = "1";
                    }
                    DocumentFileVersion dfv = new DocumentFileVersion();
                    dfv.AttachFile = saveSettings.FileName + "_" + version + document.AttachExtention;
                    dfv.DocumentId = documnet;
                    dfv.Version = Convert.ToInt32(version);
                    dfv.CreatedOn = DateTime.Now;
                    dfv.CreatedBy = UserModule.getUserId().ToString();
                    db.DocumentFileVersions.Add(dfv);
                    db.SaveChangesAsync();
                    var Full = Server.MapPath("~\\UserFiles\\DocFilesVersion" + document.FilePath);
                    if (!Directory.Exists(Full))
                    {
                        Directory.CreateDirectory(Full);
                    }
                    var file = Full + @"\" + dfv.AttachFile;
                    FileStream outputStreams = new FileStream(file, FileMode.Create);
                    workbook.SaveAs(outputStreams);
                    workbook.Close();
                    outputStream.Dispose();
                    outputStreams.Dispose();
                }
                else
                {
                    value = "1";
                }
                Session["value"] = value;
                //Workbook.Save(saveSettings);
                return RedirectToAction("SpreadSheet", new { Ids = documnet });
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        public void SaveFile(SaveSettings saveSettings)
        {
            Workbook.Save(saveSettings);
        }

        public ActionResult EditDocumentsDetali(int? ids)
        {
            Document document = db.Documents.Find(ids);
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
                if (document.DocumentFavorites.Count >0 && document.DocumentFavorites.FirstOrDefault().IsFavorite == true)
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
            }


            ViewBag.DocumentCategoryId = new SelectList(db.DocumentCategories.Where(s => s.IsActive == true).OrderBy(o => o.Name), "DocumentCategoryId", "Name", document.DocumentCategoryId);
            return PartialView("_EditDocument", document);
        }

        [HttpPost]
        public async Task<ActionResult> EditDocumentsDetaliValue(string DocumentId, string DocumentCategoryId, string KeyWords, string Notes, string chkFav, string Title)
        {
            string d = "";
            try
            {
                Document document = db.Documents.Find(Convert.ToInt32(DocumentId));
                document.ModifiedBy = UserModule.getUserId();
                document.ModifiedOn = AdminSiteConfiguration.GetEasternTime(DateTime.Now);
                document.DocumentCategoryId = Convert.ToInt32(DocumentCategoryId);
                document.Notes = Notes;
                document.IsFavorite = chkFav == "1" ? true : false;

                db.Entry(document).State = EntityState.Modified;
                await db.SaveChangesAsync();

                ActivityLog ActLog = new ActivityLog();
                ActLog.Action = 2;
                ActLog.Comment = "Document " + "<a href='/Documents/DetailDocument/" + document.DocumentId + "'>" + Title + "</a> Updated by " + UserModule.getUserFirstName() + " on " + AdminSiteConfiguration.GetDate(AdminSiteConfiguration.GetEasternTime(DateTime.Now).ToString());
                _ActivityLogRepository.ActivityLogInsert(ActLog);


                db.DocumentKeywords.RemoveRange(db.DocumentKeywords.Where(s => s.DocumentId == document.DocumentId));
                if (KeyWords != null)
                {
                    var KeywordList = KeyWords.Split(',');
                    foreach (string keyword in KeywordList)
                    {
                        DocumentKeyword DKObj = new DocumentKeyword();
                        DKObj.DocumentId = document.DocumentId;
                        DKObj.Title = keyword;
                        DKObj.IsActive = true;
                        db.DocumentKeywords.Add(DKObj);
                        await db.SaveChangesAsync();
                    }
                }
                DocumentFavorite docFav = db.DocumentFavorites.Where(x => x.DocumentId == document.DocumentId).FirstOrDefault();
                docFav.IsFavorite = document.IsFavorite;
                db.SaveChanges();
                d = "Completed";
            }
            catch (Exception ex)
            {
                d = "Error";
            }

            return Json(d, JsonRequestBehavior.AllowGet);
        }
    }
}