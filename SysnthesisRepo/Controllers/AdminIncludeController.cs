using Aspose.Pdf;
using DocumentFormat.OpenXml.Drawing.Diagrams;
using DocumentFormat.OpenXml.EMMA;
using DocumentFormat.OpenXml.ExtendedProperties;
using EntityModels.Models;
using NLog;
using Repository;
using Repository.IRepository;
using Syncfusion.XlsIO.Implementation;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Security.Policy;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Security;
using Utility;

namespace SysnthesisRepo.Controllers
{
    public class AdminIncludeController : Controller
    {
        private DBContext db = new DBContext();
        private readonly ICommonRepository _CommonRepository;
        private readonly IMastersBindRepository _mastersBindRepository;
        private readonly ISynthesisApiRepository _synthesisApiRepository;
        private static Logger logger = LogManager.GetCurrentClassLogger();
        string UserName = System.Web.HttpContext.Current.User.Identity.Name;
        public AdminIncludeController()
        {
            this._CommonRepository = new CommonRepository(new DBContext());
            this._mastersBindRepository = new MastersBindRepository(new DBContext());
            this._synthesisApiRepository = new SynthesisApiRepository();
        }

        /// <summary>
        /// This is GetIPAddress
        /// </summary>
        /// <returns></returns>
        public string GetIPAddress()
        {
            string IPAddress = "";
            try
            {
                if (ConfigurationManager.AppSettings["IPAddress"].ToString() == "")
                {
                    IPAddress = Request.UserHostAddress;
                }
                else
                {
                    IPAddress = "67.207.86.163";
                }
            }
            catch (Exception ex)
            {
                logger.Error("AdminIncludeController - GetIpAddress - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return IPAddress;
        }
        // GET: Header
        /// <summary>
        /// This method return Header 
        /// </summary>
        /// <returns></returns>
        public ActionResult Header()
        {

            //
            Header obj = new Header();
            try
            {
                obj.FirstName = db.UserMasters.Where(s => s.UserName == User.Identity.Name).Count() > 0 ? db.UserMasters.Where(s => s.UserName == User.Identity.Name).FirstOrDefault().FirstName : "";
                obj.UserRole = db.UserMasters.Where(s => s.UserName == User.Identity.Name).Count() > 0 ? db.UserMasters.Where(s => s.UserName == User.Identity.Name).FirstOrDefault().UserTypeMasters.UserType : "";
                if (Roles.IsUserInRole("Administrator"))
                {
                    var StoreData = db.StoreMasters.Where(s => s.IsActive == true).Select(s => new { s.StoreId, s.NickName }).OrderBy(o => o.NickName).ToList();
                    if (StoreData.Count() > 1)
                    {
                        ViewBag.HeaderStoreId = new SelectList(StoreData, "StoreId", "NickName", Session["StoreId"]);
                    }
                    else
                    {
                        var SID = StoreData.Count() > 0 ? StoreData.FirstOrDefault().StoreId : 0;
                        Session["StoreId"] = SID;
                        ViewBag.HeaderStoreId = new SelectList(StoreData, "StoreId", "NickName", SID);
                    }
                    ViewBag.StoreCount = StoreData.Count();
                }
                else
                {
                    var StoreList = _CommonRepository.GetHeaderStoreList(1);
                    if (StoreList.Count() > 1)
                    {
                        if (Convert.ToInt32(Session["StoreId"]) > 0)
                        {
                            ViewBag.HeaderStoreId = new SelectList(StoreList, "StoreId", "NickName", Session["StoreId"]);
                        }
                        else
                        {
                            ViewBag.HeaderStoreId = new SelectList(StoreList, "StoreId", "NickName");
                        }
                    }
                    else
                    {
                        //var SID = StoreList.Count() > 0 ? StoreList.FirstOrDefault().StoreId : 0;
                        //Session["StoreId"] = SID;
                        ViewBag.HeaderStoreId = new SelectList(StoreList, "StoreId", "NickName");
                    }
                    ViewBag.StoreCount = StoreList.Count();

                }
                string myIP = GetIPAddress();
                IpAdressInfo isIpContain = null;
                isIpContain = db.Database.SqlQuery<IpAdressInfo>("SP_IpAddressInfo @Mode = {0},@IpAddress = {1}", "SelectCheckIP", myIP).FirstOrDefault();
                var UserName = System.Web.HttpContext.Current.User.Identity.Name;
                var Userid = _CommonRepository.getUserId(UserName);
                UserMaster user = db.UserMasters.Where(s => s.UserId == Userid).FirstOrDefault();
                if (user.TrackHours == true)
                //Db class for get user Id.
                {
                    Session["CheckInOutTrack"] = 1;
                }
                else
                {
                    Session["CheckInOutTrack"] = 0;
                }
                if (isIpContain != null)
                {


                    UserIpInfo isuserAllow = db.UserIpInfos.Where(s => s.UserID == Userid && s.IpAdressInfoID == isIpContain.IpAdressInfoID).FirstOrDefault();

                    if (isuserAllow != null)
                    {
                        Session["CheckInOut"] = 1;
                    }
                    else
                    {
                        Session["CheckInOut"] = 0;
                    }
                    UserTimeTrackInfo LastStatus = db.UserTimeTrackInfos.Where(a => a.UserId == Userid).ToArray().OrderByDescending(a => a.StartTime).FirstOrDefault();
                    if (LastStatus == null)
                    {
                        Session["CheckInOutStatus"] = (long)CheckInOutType.CheckIn;
                        ViewBag.CheckInOutStatusForClass = (long)CheckInOutType.CheckIn;

                    }
                    else if (LastStatus.InOutType == CheckInOutType.CheckIn)
                    {
                        Session["CheckInOutStatus"] = (long)CheckInOutType.CheckOut;
                        ViewBag.CheckInOutStatusForClass = (long)CheckInOutType.CheckIn;
                    }
                    else if (LastStatus.InOutType == CheckInOutType.CheckOut)
                    {
                        Session["CheckInOutStatus"] = (long)CheckInOutType.CheckIn;
                        ViewBag.CheckInOutStatusForClass = (long)CheckInOutType.CheckOut;
                    }
                    Session["IpAdressInfoID"] = isIpContain.IpAdressInfoID;
                }

                //code for NoteModule Data get
                int NoteUserId = UserModule.getUserId();
                obj.UserNoteList = db.UserWiseNoteManage.Where(n => n.Isdeleted != true && n.Createdby == NoteUserId).Select(note => new UserNotListData
                {
                    NoteId = note.NoteId,
                    NoteName = note.NoteName,
                    NoteDescription = note.NoteDescription,
                    CreatedOn = note.CreatedOn,
                    InvoiceId = note.InvoiceId
                }).OrderByDescending(n => n.CreatedOn).ToList();

                ViewBag.UserNoteCount = db.UserWiseNoteManage.Where(n => n.Isdeleted != true && n.Createdby == NoteUserId).Count();

                obj.UserTaskList = (from task in db.UserWiseTaskManage
                                    join usermst in db.UserMasters on task.Createdby equals usermst.UserId
                                    join usermstto in db.UserMasters on task.AssignTo equals usermstto.UserId
                                    where (task.Createdby == NoteUserId || task.AssignTo == NoteUserId) && task.Isdeleted != true
                                    orderby task.DueDate descending
                                    select new UsertaskListData
                                    {
                                        TaskId = task.TaskId,
                                        TaskName = task.TaskName,
                                        TaskDescription = task.TaskDescription,
                                        PriorityId = task.PriorityId,
                                        AssignTo = task.AssignTo,
                                        AssignByName = usermst.FirstName,
                                        AssignToName = usermstto.FirstName,
                                        DueDate = task.DueDate,
                                        Createdby = task.Createdby,
                                        CreatedOn = task.CreatedOn,
                                        IsCompleted = task.IsCompleted,
                                        InvoiceId = task.InvoiceId
                                    }).ToList();

                var UserDroplist = db.UserMasters.Where(n => n.IsActive == true).Select(n => new { n.UserId, n.FirstName }).OrderBy(n => n.FirstName).ToList();
                ViewBag.UserDropData = new SelectList(UserDroplist, "UserId", "FirstName");
                var priorityList = new List<SelectListItem>
            {
                new SelectListItem { Text = "Low", Value = "1" },
                new SelectListItem { Text = "Medium", Value = "2" },
                new SelectListItem { Text = "High", Value = "3" }
            };
                ViewBag.PriorityDropData = new SelectList(priorityList, "Value", "Text");
                ViewBag.UserTaskCount = db.UserWiseTaskManage.Where(n => (n.Createdby == NoteUserId || n.AssignTo == NoteUserId) && n.Isdeleted != true).Count();

                obj.UserReminderList = db.UserWiseReminderManage.Where(n => n.Isdeleted != true && n.Createdby == NoteUserId).Select(remind => new UserReminderListData
                {
                    ReminderId = remind.ReminderId,
                    ReminderName = remind.ReminderName,
                    ReminderDescription = remind.ReminderDescription,
                    ReminderDate = remind.ReminderDate,
                    CreatedOn = remind.CreatedOn
                }).OrderByDescending(n => n.CreatedOn).ToList();

                ViewBag.UserReminderCount = db.UserWiseReminderManage.Where(n => n.Isdeleted != true && n.Createdby == NoteUserId).Count();
            }
            catch (Exception ex)
            {
                logger.Error("AdminIncludeController - Header - " + DateTime.Now + " - " + ex.Message.ToString());
            }

            return PartialView(obj);
        }


        /// <summary>
        /// This method return with curerent header 
        /// </summary>
        /// <param name="header"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Header(Header header)
        {
            Session["AmtMaximum"] = "";
            Session["AmtMinimum"] = "";
            Session["deptname"] = "";
            Session["startdate"] = "";
            Session["enddate"] = "";
            Session["payment"] = "";
            Session["searchdashbord"] = "";
            Session["DrpLstStore"] = "";
            Session["DrpLstVendor"] = "";
            Session["IPsearchdashbord"] = "";
            Session["IPstartdate"] = "";
            Session["IPenddate"] = "";
            Session["ECstartdate"] = "";
            Session["ECenddate"] = "";
            Session["ECLstStore"] = "";
            Session["ECsearchdashbord"] = "";
            try
            {
                if (header.Flg == 1)
                {
                    Session["StoreId"] = header.HeaderStoreId;
                    //var UserName = User.Identity.Name;
                    string URL = Request.UrlReferrer.AbsolutePath;
                    URL = URL.TrimStart('/');
                    var StoreData = db.StoreMasters.Where(s => s.IsActive == true).Select(s => new { s.StoreId, s.NickName }).OrderBy(o => o.NickName).ToList();
                    ViewBag.HeaderStoreId = new SelectList(StoreData, "StoreId", "NickName");
                    ViewBag.StoreCount = StoreData.Count();
                    //Session["FromInvoicePage"] = "";
                    if (URL != null)
                    {
                        string[] strCurrent = URL.ToString().Split('/');
                        if (strCurrent.Count() > 1)
                        {
                            if (strCurrent[0] == "invoices" && strCurrent[1] == "edit")
                            {
                                return RedirectToAction("IndexBeta", "Invoices");
                            }
                            else if (strCurrent[0] == "journalentries" && strCurrent[1] == "detail")
                            {
                                return RedirectToAction("index", "JournalEntries");
                            }
                            else if (strCurrent[0] == "report" && strCurrent[1] == "payrollexpensedetail")
                            {
                                return RedirectToAction("PayrollExpense", "Report");
                            }
                            else if (strCurrent.Length > 2)
                            {
                                return RedirectToAction(strCurrent[1], strCurrent[0], new { id = strCurrent[2] });
                            }
                            else
                            {
                                return RedirectToAction(strCurrent[1], strCurrent[0]);
                            }
                        }
                        else
                        {
                            return RedirectToAction("index", strCurrent[0]);
                        }
                    }
                    else
                    {
                        return RedirectToAction("IndexBeta", "Invoices");
                    }
                }
                else
                {
                    if (header.HeaderStoreId != 0)
                    {
                        Session["StoreId"] = header.HeaderStoreId;
                        string URL = Request.UrlReferrer.AbsolutePath;
                        URL = URL.TrimStart('/');
                        var StoreData = db.StoreMasters.Where(s => s.IsActive == true).Select(s => new { s.StoreId, s.NickName }).OrderBy(o => o.NickName).ToList();
                        ViewBag.HeaderStoreId = new SelectList(StoreData, "StoreId", "NickName");
                        ViewBag.StoreCount = StoreData.Count();
                        //if (Session["CurrentPageName"] != null)
                        //{
                        //    string[] strCurrent = Session["CurrentPageName"].ToString().Split('/');
                        //    if (strCurrent.Count() > 1)
                        //    {
                        //        if (strCurrent[0] == "invoices" && strCurrent[1] == "edit")
                        //        {
                        //            return RedirectToAction("index", "Invoices");
                        //        }
                        //        else if (strCurrent[0] == "journalentries" && strCurrent[1] == "detail")
                        //        {
                        //            return RedirectToAction("index", "JournalEntries");
                        //        }
                        //        else if (strCurrent[0] == "report" && strCurrent[1] == "payrollexpensedetail")
                        //        {
                        //            return RedirectToAction("PayrollExpense", "Report");
                        //        }
                        //        else if(strCurrent.Length >2)
                        //        {
                        //            return RedirectToAction(strCurrent[1], strCurrent[0], new { id = strCurrent[2] });
                        //        }
                        //        else
                        //        {
                        //            return RedirectToAction(strCurrent[1], strCurrent[0]);
                        //        }
                        //    }
                        //    else
                        //    {
                        //        return RedirectToAction("index", "Invoices");
                        //    }
                        //}
                        //else
                        //{
                        //    return RedirectToAction("index", "Invoices");
                        //}
                        if (URL != null)
                        {
                            string[] strCurrent = URL.ToString().Split('/');
                            if (strCurrent.Count() > 1)
                            {
                                if (strCurrent[0] == "invoices" && strCurrent[1] == "edit")
                                {
                                    return RedirectToAction("IndexBeta", "Invoices");
                                }
                                else if (strCurrent[0] == "journalentries" && strCurrent[1] == "detail")
                                {
                                    return RedirectToAction("index", "JournalEntries");
                                }
                                else if (strCurrent[0] == "report" && strCurrent[1] == "payrollexpensedetail")
                                {
                                    return RedirectToAction("PayrollExpense", "Report");
                                }
                                else if (strCurrent.Length > 2)
                                {
                                    return RedirectToAction(strCurrent[1], strCurrent[0], new { id = strCurrent[2] });
                                }
                                else
                                {
                                    return RedirectToAction(strCurrent[1], strCurrent[0]);
                                }
                            }
                            else
                            {
                                return RedirectToAction("index", strCurrent[0]);
                            }
                        }
                        else
                        {
                            return RedirectToAction("IndexBeta", "Invoices");
                        }
                    }
                    else
                    {
                        Session["StoreId"] = header.HeaderStoreId;

                        header.FirstName = db.UserMasters.Where(s => s.UserName == User.Identity.Name).Count() > 0 ? db.UserMasters.Where(s => s.UserName == User.Identity.Name).FirstOrDefault().FirstName : "";
                        var StoreData = db.StoreMasters.Where(s => s.IsActive == true).Select(s => new { s.StoreId, s.NickName }).OrderBy(o => o.NickName).ToList();
                        ViewBag.HeaderStoreId = new SelectList(StoreData, "StoreId", "NickName");
                        ViewBag.StoreCount = StoreData.Count();
                        Session["Form"] = "";
                        return View(header);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("AdminIncludeController - Header - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return View(header);
        }

        /// <summary>
        /// This is get store Wise list using storeid.
        /// </summary>
        /// <param name="StoreId"></param>
        /// <returns></returns>
        public ActionResult GetStoreWiseList(int StoreId)
        {
            try
            {
                Session["StoreId"] = StoreId;
                Session["Form"] = "YES";
                ViewBag.HeaderStoreId = new SelectList(db.StoreMasters.Where(s => s.IsActive == true).Select(s => new { s.StoreId, s.NickName }).OrderBy(o => o.NickName).ToList(), "StoreId", "NickName");
            }
            catch (Exception ex)
            {
                logger.Error("AdminIncludeController - GetStoreWiseList - " + DateTime.Now + " - " + ex.Message.ToString());
            }

            return RedirectToAction("IndexBeta", "Invoices");
        }

        /// <summary>
        /// This Method is Manage store. 
        /// </summary>
        /// <param name="Store"></param>
        /// <returns></returns>
        public ActionResult ManageStore(int Store)
        {
            Session["StoreId"] = Store;
            //WebRoleProvider obj = new WebRoleProvider();
            //obj.GetRolesForUser(User.Identity.Name);
            return RedirectToAction("SideMenu");
        }

        // GET: Footer
        /// <summary>
        /// This method is Footer.
        /// </summary>
        /// <returns></returns>
        public ActionResult Footer()
        {
            return View();
        }
        /// <summary>
        /// This is Side Menu.
        /// </summary>
        /// <returns></returns>
        [Authorize]
        // GET: SideMenu
        public ActionResult SideMenu()
        {
            ViewBag.HRAdmin = _mastersBindRepository.IsHRAdmin(UserName);
            ViewBag.HRSuperAdmin = _mastersBindRepository.IsHRSuperAdmin(UserName);
            return View();
        }
        // GET: SideMenu
        /// <summary>
        /// This method is Error for AdminInclude.
        /// </summary>
        /// <returns></returns>
        public ActionResult Error()
        {
            return View();
        }

        /// <summary>
        /// This method return the User Wise sticky Notes.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult GetUserWiseStickyNote()
        {
            int UserId = UserModule.getUserId();
            var StickNote = (from dr in db.userWiseStickyNotes where dr.UserId == UserId select dr.Notes).FirstOrDefault();
            return Json(new { notes = StickNote == null ? "" : StickNote, data = false }, JsonRequestBehavior.AllowGet);
        }
        //public string GetIPAddress()
        //{
        //    string IPAddress = Request.UserHostAddress;
        //    return IPAddress;
        //}
        /// <summary>
        /// This methos use for set user Stickynotes.
        /// </summary>
        /// <param name="notes"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SetUserWiseStickyNote(string notes)
        {
            try
            {
                int UserId = UserModule.getUserId();
                UserWiseStickyNote getdata = (from dr in db.userWiseStickyNotes where dr.UserId == UserId select dr).FirstOrDefault();
                if (getdata != null)
                {
                    getdata.Notes = notes;
                    db.Entry(getdata).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    return Json(new { notes = getdata.Notes, msg = "Note updated successfully.", msgstatus = 1, excepetion = "" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    getdata = new UserWiseStickyNote();
                    getdata.Notes = notes;
                    getdata.UserId = UserId;
                    getdata.CreatedOn = DateTime.Now;
                    db.userWiseStickyNotes.Add(getdata);
                    db.SaveChanges();
                    return Json(new { notes = getdata.Notes, msg = "Note added successfully.", msgstatus = 1, excepetion = "" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                logger.Error("AdminIncludeController - SetUserWiseStickyNote - " + DateTime.Now + " - " + ex.Message.ToString());
                return Json(new { notes = "", msg = "An error while updating entry.", msgstatus = 0, excepetion = ex.Message }, JsonRequestBehavior.AllowGet);
            }

        }
        /// <summary>
        /// This method is get All the data.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult GetAllData()
        {
            IEnumerable<object> Result = null;
            try
            {
                int userid = UserModule.getUserId();

                string storeid = "";
                if (!string.IsNullOrEmpty(Convert.ToString(Session["storeid"])))
                {
                    storeid = Convert.ToString((Session["storeid"]));
                }
                else
                {
                    RedirectToAction("index", "login");
                }

                var CurrentUserTypeLevel = db.UserMasters.Where(s => s.UserId == userid).Select(s => s.UserTypeMasters.LevelsApproverId).FirstOrDefault();
                var UserTypeList = db.UserTypeModuleApprovers.Where(s => s.LevelsApproverId == CurrentUserTypeLevel).Select(s => s.UserTypeId).ToList();
                var UserList = db.UserMasters.Where(s => UserTypeList.Contains(s.UserTypeId)).Select(s => new { s.UserId, s.UserName }).ToList();
                var ContainUserList = UserList.Select(s => s.UserId).ToList();
                var store = db.Database.SqlQuery<int>("SP_GetStore_ForDashboard @Mode={0},@UserTypeId={1}", "GetStore_ForNotification", UserModule.getUserTypeId());

                Result = db.Invoices.Where(s => s.StatusValue == InvoiceStatusEnm.Pending && store.Contains(s.StoreId) && ContainUserList.Contains(s.CreatedBy.Value)).Select(s => new { InvoiceId = s.InvoiceId, StoreName = s.StoreMasters.Name, StoreId = s.StoreId, s.CreatedBy, s.NotificationColor, s.InvoiceNumber, s.InvoiceDate }).ToList().Select(k => new Invoice_Notification
                {
                    StoreName = k.StoreName,
                    StoreId = k.StoreId,
                    InvoiceId = k.InvoiceId,
                    NotificationColor = k.NotificationColor,
                    InvoiceNumber = k.InvoiceNumber,
                    InvoiceDate = k.InvoiceDate,
                    UserName = UserList.Where(n => n.UserId == k.CreatedBy).FirstOrDefault().UserName
                }).ToList();
                ViewBag.Modalcount = Result.Count();
                Result = Result.Take(5);
            }
            catch (Exception ex)
            {
                logger.Error("AdminIncludeController - GetAllData - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return PartialView("~/Views/AdminInclude/_InvoiceNotification.cshtml", Result);
        }
        /// <summary>
        /// This method get CheckIn data.
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetCheckInData()
        {
            string Clockintime = null;
            CheckInButtonValues TotalHoursWorked = new CheckInButtonValues();
            try
            {
                int UserId = UserModule.getUserId();
                Clockintime = db.Database.SqlQuery<string>("SP_Attandanace @Mode = {0}, @UserId = {1}, @startDate = {2}, @endDate = {3}", "GetCheckInButtonValue", UserId, null, null).FirstOrDefault();
                TotalHoursWorked = db.Database.SqlQuery<CheckInButtonValues>("SP_Attandanace @Mode = {0}, @UserId = {1}, @startDate = {2}, @endDate = {3}", "GetCheckInTotalHours", UserId, null, null).FirstOrDefault();
            }
            catch (Exception ex)
            {
                logger.Error("AdminIncludeController - GetCheckInData - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return Json(new { Clockintime = Clockintime == null ? "" : Clockintime, TotalHoursWorked = TotalHoursWorked == null ? 0 : TotalHoursWorked.TotalHoursWorked }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SomethingWentWrong()
        {
            return View();
        }

        #region usernote 
        public async Task<ActionResult> CreateUserNote(int? noteId, string noteTitle, string editorContent)
        {
            string message = "";
            string notesHtml = "";
            int usernotecount = 0;
            try
            {
                if (noteId == null)
                {
                    string TitleName = await _synthesisApiRepository.GetNoteTitleFromGPTAsync(editorContent);
                    int UserId = UserModule.getUserId();
                    UserWiseNoteManage obj = new UserWiseNoteManage();
                    obj.NoteName = TitleName;
                    obj.NoteDescription = editorContent;
                    obj.Createdby = UserId;
                    db.UserWiseNoteManage.Add(obj);
                    db.SaveChanges();


                }
                else
                {
                    int id = noteId.Value;
                    var existingNote = db.UserWiseNoteManage.Find(id);
                    if (existingNote != null)
                    {
                        existingNote.NoteName = noteTitle;
                        existingNote.NoteDescription = editorContent;
                        existingNote.ModifiedOn = DateTime.Now;
                        db.Entry(existingNote).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                    }
                    else
                    {
                        message = "Note not found";
                    }
                }

                int NoteUserId = UserModule.getUserId();
                var notes = db.UserWiseNoteManage.Where(n => n.Isdeleted != true && n.Createdby == NoteUserId).OrderByDescending(n => n.CreatedOn).ToList();
                foreach (var note in notes)
                {
                    //var noteDescription = note.NoteDescription.Length > 20 ? note.NoteDescription.Substring(0, 20) + "..." : note.NoteDescription;
                    var deleteIconPath = Url.Content("~/Content/Admin/images/icons/delete.svg");
                    var editIconPath = Url.Content("~/Content/Admin/images/icons/edit-05.svg");
                    //notesHtml += $"<div class='note'>" +
                    //             $"<input type='hidden' class='noteId' value='{note.NoteId}' />" +
                    //             $"<div class='note-header'>" +
                    //             $"<h1>{note.NoteName}</h1>" +
                    //             $"<div class='actionBar'>" +
                    //             $"<div id='deleteNote' class='deleteNs'><img src='{deleteIconPath}' alt='Delete'/></div>" +
                    //             $"<div id='editNote' class='editNs'><img src='{editIconPath}' alt='Edit'/></div>" +
                    //             $"</div></div>" +
                    //             $"<div class='note-body'><span>{note.NoteDescription}</span></div></div>";

                    //notesHtml += $"<div class='note'>" +
                    //             $"<input type='hidden' class='noteId' value='{note.NoteId}' />" +
                    //             $"<div class='note-header'>" +
                    //             $"<h1>{(!string.IsNullOrWhiteSpace(note.NoteName) ? note.NoteName : noteDescription)}</h1>" +
                    //             $"<div class='actionBar'>" +
                    //             $"<div id='deleteNote' class='deleteNs'><img src='{deleteIconPath}' alt='Delete'/></div>" +
                    //             $"<div id='editNote' class='editNs'><img src='{editIconPath}' alt='Edit'/></div>" +
                    //             $"</div></div>" +
                    //             $"<div class='note-body'><span>{note.NoteDescription}</span></div></div>";

                    notesHtml += $"<div class='note'>" +
                                 $"<input type='hidden' class='noteId' value='{note.NoteId}' />" +
                                 $"<div class='note-header'>";

                    if (note.InvoiceId != null)
                    {
                        notesHtml += $"<h2><a href='/Invoices/Details/{note.InvoiceId}' style='color: #385585 !important;cursor:pointer;'>Invoice # - {note.NoteName}</a></h2>";
                    }
                    else
                    {
                        notesHtml += $"<h2 style='color: #000 !important;'>{note.NoteName}</h2>";
                    }

                    notesHtml += $"<div class='actionBar'>" +
                                 $"<div id='deleteNote' class='deleteNs'><img src='{deleteIconPath}' alt='Delete'/></div>" +
                                 $"<div id='editNote' class='editNs'><img src='{editIconPath}' alt='Edit'/></div>" +
                                 
                                 $"</div>" +
                                 $"</div>" +
                                 $"<div class='note-body'>{note.NoteDescription}</div>" +
                                 
                                 $"</div>";
                }

                usernotecount = db.UserWiseNoteManage.Where(n => n.Isdeleted != true && n.Createdby == NoteUserId).Count();

                message = noteId == null ? "Create" : "Update";
            }
            catch (Exception ex)
            {
                logger.Error("AdminIncludeController - Create - " + DateTime.Now + " - " + ex.Message.ToString());
                message = "Error";
            }
            return Json(new { success = message, notesHtml = notesHtml, usernotecount = usernotecount });
        }

        public ActionResult EditUserNote(int id)
        {
            try
            {
                var note = db.UserWiseNoteManage
                             .Where(n => n.NoteId == id)
                             .Select(n => new
                             {
                                 n.NoteId,
                                 n.NoteName,
                                 n.NoteDescription
                             })
                             .FirstOrDefault();

                if (note != null)
                {
                    return Json(new { success = true, note }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { success = false, message = "Note not found" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                logger.Error("Error in EditUserNote: " + ex.Message);
                return Json(new { success = false, message = "Error occurred" }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult DeleteUserNote(int id)
        {
            string notesHtml = "";
            int usernotecount = 0;
            try
            {
                var existingNote = db.UserWiseNoteManage.Find(id);
                if (existingNote != null)
                {
                    existingNote.ModifiedOn = DateTime.Now;
                    existingNote.Isdeleted = true;
                    db.Entry(existingNote).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();

                    int NoteUserId = UserModule.getUserId();
                    var notes = db.UserWiseNoteManage.Where(n => n.Isdeleted != true && n.Createdby == NoteUserId).OrderByDescending(n => n.CreatedOn).ToList();
                    foreach (var note in notes)
                    {
                        //var noteDescription = note.NoteDescription.Length > 20 ? note.NoteDescription.Substring(0, 20) + "..." : note.NoteDescription;
                        var deleteIconPath = Url.Content("~/Content/Admin/images/icons/delete.svg");
                        var editIconPath = Url.Content("~/Content/Admin/images/icons/edit-05.svg");
                        //notesHtml += $"<div class='note'>" +
                        //             $"<input type='hidden' class='noteId' value='{note.NoteId}' />" +
                        //             $"<div class='note-header'>" +
                        //             $"<h1>{note.NoteName}</h1>" +
                        //             $"<div class='actionBar'>" +
                        //             $"<div id='deleteNote' class='deleteNs'><img src='{deleteIconPath}' alt='Delete'/></div>" +
                        //             $"<div id='editNote' class='editNs'><img src='{editIconPath}' alt='Edit'/></div>" +
                        //             $"</div></div>" +
                        //             $"<div class='note-body'><span>{note.NoteDescription}</span></div></div>";
                        //notesHtml += $"<div class='note'>" +
                        //         $"<input type='hidden' class='noteId' value='{note.NoteId}' />" +
                        //         $"<div class='note-header'>" +
                        //         $"<h1>{(!string.IsNullOrWhiteSpace(note.NoteName) ? note.NoteName : noteDescription)}</h1>" +
                        //         $"<div class='actionBar'>" +
                        //         $"<div id='deleteNote' class='deleteNs'><img src='{deleteIconPath}' alt='Delete'/></div>" +
                        //         $"<div id='editNote' class='editNs'><img src='{editIconPath}' alt='Edit'/></div>" +
                        //         $"</div></div>" +
                        //         $"<div class='note-body'><span>{note.NoteDescription}</span></div></div>";

                        notesHtml += $"<div class='note'>" +
                                 $"<input type='hidden' class='noteId' value='{note.NoteId}' />" +
                                 $"<div class='note-header'>";

                        if (note.InvoiceId != null)
                        {
                            notesHtml += $"<h2><a href='/Invoices/Details/{note.InvoiceId}' style='color: #385585 !important;cursor:pointer;'>Invoice # - {note.NoteName}</a></h2>";
                        }
                        else
                        {
                            notesHtml += $"<h2 style='color: #000 !important;'>{note.NoteName}</h2>";
                        }

                        notesHtml += $"<div class='actionBar'>" +
                                     $"<div id='deleteNote' class='deleteNs'><img src='{deleteIconPath}' alt='Delete'/></div>" +
                                     $"<div id='editNote' class='editNs'><img src='{editIconPath}' alt='Edit'/></div>" +
                                     $"</div></div>" +
                                     $"<div class='note-body'>{note.NoteDescription}</div></div>";
                    }
                    usernotecount = db.UserWiseNoteManage.Where(n => n.Isdeleted != true && n.Createdby == NoteUserId).Count();

                    return Json(new { success = true, notesHtml = notesHtml, usernotecount = usernotecount }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { success = false, notesHtml = notesHtml, usernotecount = usernotecount }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                logger.Error("Error in EditUserNote: " + ex.Message);
                return Json(new { success = false, message = "Error occurred" }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion usernote 

        #region usertask
        public ActionResult CreateUserTask(int? taskId, string taskName, string taskDescription, int priorityId, int assignTo, DateTime dueDate)
        {
            string message = "";
            string taskHtml = "";
            int usertaskcount = 0;
            try
            {
                if (taskId == null)
                {
                    int UserId = UserModule.getUserId();
                    UserWiseTaskManage obj = new UserWiseTaskManage();
                    obj.TaskName = taskName;
                    obj.TaskDescription = taskDescription;
                    obj.PriorityId = priorityId;
                    obj.AssignTo = assignTo;
                    obj.DueDate = dueDate;
                    obj.IsCompleted = false;
                    obj.Createdby = UserId;
                    db.UserWiseTaskManage.Add(obj);
                    db.SaveChanges();
                }
                else
                {
                    int id = taskId.Value;
                    var existingTask = db.UserWiseTaskManage.Find(id);
                    if (existingTask != null)
                    {
                        existingTask.TaskName = taskName;
                        existingTask.TaskDescription = taskDescription;
                        existingTask.PriorityId = priorityId;
                        existingTask.AssignTo = assignTo;
                        existingTask.DueDate = dueDate;
                        existingTask.IsCompleted = false;
                        existingTask.ModifiedOn = DateTime.Now;
                        db.Entry(existingTask).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                    }
                    else
                    {
                        message = "Task not found";
                    }
                }

                int TaskUserId = UserModule.getUserId();
                var tasks = (from task in db.UserWiseTaskManage
                             join user in db.UserMasters on task.Createdby equals user.UserId
                             join usermstto in db.UserMasters on task.AssignTo equals usermstto.UserId
                             where (task.Createdby == TaskUserId || task.AssignTo == TaskUserId) && task.Isdeleted != true
                             //orderby (DbFunctions.TruncateTime(task.DueDate) == DateTime.Today && task.IsCompleted ? 0 : 1), (DbFunctions.TruncateTime(task.DueDate) != DateTime.Today ? (task.IsCompleted ? 1 : 0) : 0), task.DueDate descending
                             orderby task.DueDate descending
                             select new
                             {
                                 task.TaskId,
                                 task.TaskName,
                                 task.TaskDescription,
                                 task.DueDate,
                                 task.AssignTo,
                                 task.Createdby,
                                 task.IsCompleted,
                                 task.InvoiceId,
                                 UserFirstName = user.FirstName,
                                 AssignToFirstName = usermstto.FirstName,

                             }).ToList();
                foreach (var task in tasks)
                {
                    var completedIcon = Url.Content("~/Content/Admin/images/icons/completed-note.svg");
                    var markCompletedIcon = Url.Content("~/Content/Admin/images/icons/mark-as-completed.svg");
                    var editIconPath = Url.Content("~/Content/Admin/images/icons/edit-05.svg");
                    var deleteIconPath = Url.Content("~/Content/Admin/images/icons/delete.svg");

                    //string taskDiv = $@"
                    //    <div class='task {(task.DueDate.Date == DateTime.Today ? "dueToday" : "")}'>
                    //    <input type='hidden' class='taskId' value='{task.TaskId}' />
                    //        <div class='taskHeader'>";
                    string taskDiv = $@"
                                <div class='task {(task.DueDate.Date == DateTime.Today && !task.IsCompleted ? "dueToday" : (task.DueDate.Date < DateTime.Today && !task.IsCompleted ? "overDue" : ""))}'>
                                <input type='hidden' class='taskId' value='{task.TaskId}' />
                                    <div class='taskHeader'>";

                    if (task.DueDate.Date != DateTime.Today || task.IsCompleted)
                    {
                        taskDiv += $@"
                                    <span class='dueDate'>{task.DueDate.ToString("dd-MMM-yyyy")}</span>";
                    }
                    else
                    {
                        taskDiv += "<span class=''></span>";
                    }
                    if (task.IsCompleted)
                    {
                        taskDiv += $@"
                                <button id='markAsCompletedBtn' class='mark-completed completedts' data-status='completed'>
                                    <span class='completed-content'>
                                        <img src='{markCompletedIcon}' class='completed-icon' />
                                        <span class='button-textts'>Completed</span>
                                    </span>
                                </button>";
                    }
                    else
                    {
                        //taskDiv += $@"
                        //        <div class='task-row'>
                        //            <div class='editTaskTs'>
                        //                <img src='{editIconPath}' alt='Edit' />
                        //            </div>
                        //            <button id='markAsCompletedBtn' class='mark-completedts' data-status='mark-as-completed'>
                        //                <span class='button-contentts'>
                        //                    <img src='{completedIcon}' class='default-iconts' />
                        //                    <span class='button-textts'>Mark as Completed</span>
                        //                </span>
                        //            </button>
                        //        </div>";

                        taskDiv += $@"
                                <div class='task-row'>";

                        if (task.Createdby == Convert.ToInt32(Session["UserId"]))
                        {
                            taskDiv += $@"
                                    <div class='deleteTaskTs'>
                                        <img src='{deleteIconPath}' alt='Delete' />
                                    </div>
                                    <div class='editTaskTs'>
                                        <img src='{editIconPath}' alt='Edit' />
                                    </div>";
                        }

                        taskDiv += $@"
                                <button id='markAsCompletedBtn' class='mark-completedts' data-status='mark-as-completed'>
                                    <span class='button-contentts'>
                                        <img src='{completedIcon}' class='default-iconts' />
                                        <span class='button-textts'>Mark as Completed</span>
                                    </span>
                                </button>
                            </div>";
                    }

                    //taskDiv += $@"
                    //        </div>
                    //        <h1>{task.TaskName}</h1>
                    //        <h2>{task.TaskDescription}</h2>
                    //        <span class='assignedBy'>Assigned by <a href='#'>{task.UserFirstName}</a></span>
                    //    </div>";

                    taskDiv += $@"
                            </div>";

                    if (task.InvoiceId != null)
                    {
                        taskDiv += $@"
                                <h1>
                                    <a href='/Invoices/Details/{task.InvoiceId}' style='color: #385585;cursor:pointer;'>Invoice # - {task.TaskName}</a>
                                </h1>";
                    }
                    else
                    {
                        taskDiv += $@"
                                <h1>{task.TaskName}</h1>";
                    }

                    taskDiv += $@"
                                <h2>{task.TaskDescription}</h2>";

                    if (task.AssignTo == Convert.ToInt32(Session["UserId"].ToString()) && task.Createdby == Convert.ToInt32(Session["UserId"].ToString()))
                    {
                        taskDiv += $@"
                                <span class='assignedBy'>Assigned by <a href='#'>Me</a></span>
                              </div>";
                    }
                    else if (task.AssignTo == Convert.ToInt32(Session["UserId"].ToString()) && task.Createdby != Convert.ToInt32(Session["UserId"].ToString()))
                    {
                        taskDiv += $@"
                                <span class='assignedBy'>Assigned by <a href='#'>{task.UserFirstName}</a></span>
                              </div>";
                    }
                    else
                    {
                        taskDiv += $@"
                                <span class='assignedBy'>Assigned to <a href='#'>{task.AssignToFirstName}</a></span>
                              </div>";
                    }

                    //taskDiv += $@"
                    //        <h2>{task.TaskDescription}</h2>
                    //        <span class='assignedBy'>Assigned by <a href='#'>{task.UserFirstName}</a></span>
                    //        </div>";

                    taskHtml += taskDiv;
                }

                usertaskcount = db.UserWiseTaskManage.Where(n => (n.Createdby == TaskUserId || n.AssignTo == TaskUserId) && n.Isdeleted != true).OrderByDescending(n => n.DueDate).Count();

                message = taskId == null ? "Create" : "Update";
            }
            catch (Exception ex)
            {
                logger.Error("AdminIncludeController - CreateUserTask - " + DateTime.Now + " - " + ex.Message.ToString());
                message = "Error";
            }
            return Json(new { success = message, taskHtml = taskHtml, usertaskcount = usertaskcount });
        }

        public ActionResult EditUserTask(int id)
        {
            try
            {
                var task = db.UserWiseTaskManage
                             .Where(n => n.TaskId == id)
                             .Select(n => new
                             {
                                 n.TaskId,
                                 n.TaskName,
                                 n.TaskDescription,
                                 n.PriorityId,
                                 n.AssignTo,
                                 n.DueDate,
                             })
                             .FirstOrDefault();

                if (task != null)
                {
                    return Json(new { success = true, task }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { success = false, message = "Task not found" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                logger.Error("Error in EditUserTask: " + ex.Message);
                return Json(new { success = false, message = "Error occurred" }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult UpdateUserTask(int? taskId)
        {
            string message = "";
            string taskHtml = "";
            try
            {
                if (taskId != null)
                {
                    int id = taskId.Value;
                    var existingTask = db.UserWiseTaskManage.Find(id);
                    if (existingTask != null)
                    {
                        existingTask.ModifiedOn = DateTime.Now;
                        existingTask.IsCompleted = true;
                        db.Entry(existingTask).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                    }
                    else
                    {
                        message = "Task not found";
                    }
                }

                int TaskUserId = UserModule.getUserId();
                var tasks = (from task in db.UserWiseTaskManage
                             join user in db.UserMasters on task.Createdby equals user.UserId
                             join usermstto in db.UserMasters on task.AssignTo equals usermstto.UserId
                             where (task.Createdby == TaskUserId || task.AssignTo == TaskUserId) && task.Isdeleted != true
                             //orderby (DbFunctions.TruncateTime(task.DueDate) == DateTime.Today && task.IsCompleted ? 0 : 1), (DbFunctions.TruncateTime(task.DueDate) != DateTime.Today ? (task.IsCompleted ? 1 : 0) : 0), task.DueDate descending
                             orderby task.DueDate descending
                             select new
                             {
                                 task.TaskId,
                                 task.TaskName,
                                 task.TaskDescription,
                                 task.DueDate,
                                 task.AssignTo,
                                 task.Createdby,
                                 task.IsCompleted,
                                 task.InvoiceId,
                                 UserFirstName = user.FirstName,
                                 AssignToFirstName = usermstto.FirstName,
                             }).ToList();
                foreach (var task in tasks)
                {
                    var completedIcon = Url.Content("~/Content/Admin/images/icons/completed-note.svg");
                    var markCompletedIcon = Url.Content("~/Content/Admin/images/icons/mark-as-completed.svg");
                    var editIconPath = Url.Content("~/Content/Admin/images/icons/edit-05.svg");
                    var deleteIconPath = Url.Content("~/Content/Admin/images/icons/delete.svg");

                    string taskDiv = $@"
                                <div class='task {(task.DueDate.Date == DateTime.Today && !task.IsCompleted ? "dueToday" : (task.DueDate.Date < DateTime.Today && !task.IsCompleted ? "overDue" : ""))}'>
                                <input type='hidden' class='taskId' value='{task.TaskId}' />
                                    <div class='taskHeader'>";
                    if (task.DueDate.Date != DateTime.Today || task.IsCompleted)
                    {
                        taskDiv += $@"
                                    <span class='dueDate'>{task.DueDate.ToString("dd-MMM-yyyy")}</span>";
                    }
                    else
                    {
                        taskDiv += "<span class=''></span>";
                    }
                    if (task.IsCompleted)
                    {
                        taskDiv += $@"
                                <button id='markAsCompletedBtn' class='mark-completed completedts' data-status='completed'>
                                    <span class='completed-content'>
                                        <img src='{markCompletedIcon}' class='completed-icon' />
                                        <span class='button-textts'>Completed</span>
                                    </span>
                                </button>";
                    }
                    else
                    {
                        taskDiv += $@"
                                <div class='task-row'>";

                        if (task.Createdby == Convert.ToInt32(Session["UserId"]))
                        {
                            taskDiv += $@"
                                    <div class='deleteTaskTs'>
                                        <img src='{deleteIconPath}' alt='Delete' />
                                    </div>
                                    <div class='editTaskTs'>
                                        <img src='{editIconPath}' alt='Edit' />
                                    </div>";
                        }

                        taskDiv += $@"
                                <button id='markAsCompletedBtn' class='mark-completedts' data-status='mark-as-completed'>
                                    <span class='button-contentts'>
                                        <img src='{completedIcon}' class='default-iconts' />
                                        <span class='button-textts'>Mark as Completed</span>
                                    </span>
                                </button>
                            </div>";
                    }

                    //taskDiv += $@"
                    //        </div>
                    //        <h1>{task.TaskName}</h1>
                    //        <h2>{task.TaskDescription}</h2>
                    //        <span class='assignedBy'>Assigned by <a href='#'>{task.UserFirstName}</a></span>
                    //    </div>";

                    taskDiv += $@"
                            </div>";

                    if (task.InvoiceId != null)
                    {
                        taskDiv += $@"
                                <h1>
                                    <a href='/Invoices/Details/{task.InvoiceId}' style='color: #385585;cursor:pointer;'>Invoice # - {task.TaskName}</a>
                                </h1>";
                    }
                    else
                    {
                        taskDiv += $@"
                                <h1>{task.TaskName}</h1>";
                    }

                    taskDiv += $@"
                                <h2>{task.TaskDescription}</h2>";

                    if (task.AssignTo == Convert.ToInt32(Session["UserId"].ToString()) && task.Createdby == Convert.ToInt32(Session["UserId"].ToString()))
                    {
                        taskDiv += $@"
                                <span class='assignedBy'>Assigned by <a href='#'>Me</a></span>
                              </div>";
                    }
                    else if (task.AssignTo == Convert.ToInt32(Session["UserId"].ToString()) && task.Createdby != Convert.ToInt32(Session["UserId"].ToString()))
                    {
                        taskDiv += $@"
                                <span class='assignedBy'>Assigned by <a href='#'>{task.UserFirstName}</a></span>
                              </div>";
                    }
                    else
                    {
                        taskDiv += $@"
                                <span class='assignedBy'>Assigned to <a href='#'>{task.AssignToFirstName}</a></span>
                              </div>";
                    }

                    //taskDiv += $@"
                    //        <h2>{task.TaskDescription}</h2>
                    //        <span class='assignedBy'>Assigned by <a href='#'>{task.UserFirstName}</a></span>
                    //        </div>";

                    taskHtml += taskDiv;
                }

                message = "Update";
            }
            catch (Exception ex)
            {
                logger.Error("AdminIncludeController - UpdateUserTask - " + DateTime.Now + " - " + ex.Message.ToString());
                message = "Error";
            }
            return Json(new { success = message, taskHtml = taskHtml });
        }

        public ActionResult DeleteUserTask(int id)
        {
            string taskHtml = "";
            int usertaskcount = 0;
            try
            {
                var existingTask = db.UserWiseTaskManage.Find(id);
                if (existingTask != null)
                {
                    existingTask.ModifiedOn = DateTime.Now;
                    existingTask.Isdeleted = true;
                    db.Entry(existingTask).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();

                    int TaskUserId = UserModule.getUserId();
                    var tasks = (from task in db.UserWiseTaskManage
                                 join user in db.UserMasters on task.Createdby equals user.UserId
                                 join usermstto in db.UserMasters on task.AssignTo equals usermstto.UserId
                                 where (task.Createdby == TaskUserId || task.AssignTo == TaskUserId) && task.Isdeleted != true
                                 //orderby (DbFunctions.TruncateTime(task.DueDate) == DateTime.Today && task.IsCompleted ? 0 : 1), (DbFunctions.TruncateTime(task.DueDate) != DateTime.Today ? (task.IsCompleted ? 1 : 0) : 0), task.DueDate descending
                                 orderby task.DueDate descending
                                 select new
                                 {
                                     task.TaskId,
                                     task.TaskName,
                                     task.TaskDescription,
                                     task.DueDate,
                                     task.AssignTo,
                                     task.Createdby,
                                     task.IsCompleted,
                                     task.InvoiceId,
                                     UserFirstName = user.FirstName,
                                     AssignToFirstName = usermstto.FirstName,
                                 }).ToList();
                    foreach (var task in tasks)
                    {
                        var completedIcon = Url.Content("~/Content/Admin/images/icons/completed-note.svg");
                        var markCompletedIcon = Url.Content("~/Content/Admin/images/icons/mark-as-completed.svg");
                        var editIconPath = Url.Content("~/Content/Admin/images/icons/edit-05.svg");
                        var deleteIconPath = Url.Content("~/Content/Admin/images/icons/delete.svg");

                        string taskDiv = $@"
                                <div class='task {(task.DueDate.Date == DateTime.Today && !task.IsCompleted ? "dueToday" : (task.DueDate.Date < DateTime.Today && !task.IsCompleted ? "overDue" : ""))}'>
                                <input type='hidden' class='taskId' value='{task.TaskId}' />
                                    <div class='taskHeader'>";
                        if (task.DueDate.Date != DateTime.Today || task.IsCompleted)
                        {
                            taskDiv += $@"
                                    <span class='dueDate'>{task.DueDate.ToString("dd-MMM-yyyy")}</span>";
                        }
                        else
                        {
                            taskDiv += "<span class=''></span>";
                        }
                        if (task.IsCompleted)
                        {
                            taskDiv += $@"
                                <button id='markAsCompletedBtn' class='mark-completed completedts' data-status='completed'>
                                    <span class='completed-content'>
                                        <img src='{markCompletedIcon}' class='completed-icon' />
                                        <span class='button-textts'>Completed</span>
                                    </span>
                                </button>";
                        }
                        else
                        {
                            taskDiv += $@"
                                <div class='task-row'>";

                            if (task.Createdby == Convert.ToInt32(Session["UserId"]))
                            {
                                taskDiv += $@"
                                    <div class='deleteTaskTs'>
                                        <img src='{deleteIconPath}' alt='Delete' />
                                    </div>
                                    <div class='editTaskTs'>
                                        <img src='{editIconPath}' alt='Edit' />
                                    </div>";
                            }

                            taskDiv += $@"
                                <button id='markAsCompletedBtn' class='mark-completedts' data-status='mark-as-completed'>
                                    <span class='button-contentts'>
                                        <img src='{completedIcon}' class='default-iconts' />
                                        <span class='button-textts'>Mark as Completed</span>
                                    </span>
                                </button>
                            </div>";
                        }

                        taskDiv += $@"
                            </div>";

                        if (task.InvoiceId != null)
                        {
                            taskDiv += $@"
                                <h1>
                                    <a href='/Invoices/Details/{task.InvoiceId}' style='color: #385585;cursor:pointer;'>Invoice # - {task.TaskName}</a>
                                </h1>";
                        }
                        else
                        {
                            taskDiv += $@"
                                <h1>{task.TaskName}</h1>";
                        }

                        taskDiv += $@"
                                <h2>{task.TaskDescription}</h2>";

                        if (task.AssignTo == Convert.ToInt32(Session["UserId"].ToString()) && task.Createdby == Convert.ToInt32(Session["UserId"].ToString()))
                        {
                            taskDiv += $@"
                                <span class='assignedBy'>Assigned by <a href='#'>Me</a></span>
                              </div>";
                        }
                        else if (task.AssignTo == Convert.ToInt32(Session["UserId"].ToString()) && task.Createdby != Convert.ToInt32(Session["UserId"].ToString()))
                        {
                            taskDiv += $@"
                                    <span class='assignedBy'>Assigned by <a href='#'>{task.UserFirstName}</a></span>
                                  </div>";
                        }
                        else
                        {
                            taskDiv += $@"
                                <span class='assignedBy'>Assigned to <a href='#'>{task.AssignToFirstName}</a></span>
                              </div>";
                        }

                        taskHtml += taskDiv;
                    }
                    usertaskcount = db.UserWiseTaskManage.Where(n => (n.Createdby == TaskUserId || n.AssignTo == TaskUserId) && n.Isdeleted != true).OrderByDescending(n => n.DueDate).Count();

                    return Json(new { success = true, taskHtml = taskHtml, usertaskcount = usertaskcount }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { success = false, taskHtml = taskHtml, usertaskcount = usertaskcount }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                logger.Error("Error in DeleteUserTask: " + ex.Message);
                return Json(new { success = false, message = "Error occurred" }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion usertask

        #region userreminder
        public ActionResult CreateUserReminder(int? reminderId, string reminderName, string reminderDescription, DateTime reminderDate)
        {
            string message = "";
            string reminderHtml = "";
            int userremindercount = 0;
            try
            {
                if (reminderId == null)
                {
                    int UserId = UserModule.getUserId();
                    UserWiseReminderManage obj = new UserWiseReminderManage();
                    obj.ReminderName = reminderName;
                    obj.ReminderDescription = reminderDescription;
                    obj.ReminderDate = reminderDate;
                    obj.Createdby = UserId;
                    db.UserWiseReminderManage.Add(obj);
                    db.SaveChanges();


                }
                else
                {
                    int id = reminderId.Value;
                    var existingremind = db.UserWiseReminderManage.Find(id);
                    if (existingremind != null)
                    {
                        existingremind.ReminderName = reminderName;
                        existingremind.ReminderDescription = reminderDescription;
                        existingremind.ReminderDate = reminderDate;
                        existingremind.ModifiedOn = DateTime.Now;
                        db.Entry(existingremind).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                    }
                    else
                    {
                        message = "Reminder not found";
                    }
                }

                int ReminderUserId = UserModule.getUserId();
                var reminders = db.UserWiseReminderManage.Where(n => n.Isdeleted != true && n.Createdby == ReminderUserId).OrderByDescending(n => n.CreatedOn).ToList();
                foreach (var reminder in reminders)
                {
                    var deleteIconPath = Url.Content("~/Content/Admin/images/icons/delete.svg");
                    var editIconPath = Url.Content("~/Content/Admin/images/icons/edit-05.svg");
                    var remindericon = Url.Content("~/Content/Admin/images/icons/reminder-listing-icon.svg");
                    //reminderHtml += $"<div class='reminder'>" +
                    //             $"<input type='hidden' class='reminderId' value='{reminder.ReminderId}' />" +
                    //             $"<div class='reminder-section-01'>" +
                    //             $"<img src='{remindericon}' />" +
                    //             $"<span>{reminder.ReminderName} :- {reminder.ReminderDescription}</span>" +
                    //             $"</div>" +
                    //             $"<div class='actionBar'>" +
                    //             $"<div class='deleteReminderTs'><img src='{deleteIconPath}' alt='Delete' /></div>" +
                    //             $"<div class='editReminderTs'><img src='{editIconPath}' alt='Edit' /></div>" +
                    //             $"</div></div>";
                    reminderHtml += $"<div class='reminder'>" +
                                    $"<input type='hidden' class='reminderId' value='{reminder.ReminderId}' />" +
                                    $"<div class='reminder-section-01'>" +
                                    $"<img src='{remindericon}' />" +
                                    $"<h1>{reminder.ReminderName}</h1>" +
                                    $"</div>" +
                                    $"<div class='actionBar'>" +
                                    $"<div class='deleteReminderTs'>" +
                                    $"<img src='{deleteIconPath}' alt='Delete' />" +
                                    $"</div>" +
                                    $"<div class='editReminderTs'>" +
                                    $"<img src='{editIconPath}' alt='Edit' />" +
                                    $"</div>" +
                                    $"</div>" +
                                    $"<div class='reminderContent'>" +
                                    $"<span>{reminder.ReminderDescription}</span>" +
                                    $"</div>"+
                                    $"</div>";
                }

                userremindercount = db.UserWiseReminderManage.Where(n => n.Isdeleted != true && n.Createdby == ReminderUserId).Count();

                message = reminderId == null ? "Create" : "Update";
            }
            catch (Exception ex)
            {
                logger.Error("AdminIncludeController - CreateUserReminder - " + DateTime.Now + " - " + ex.Message.ToString());
                message = "Error";
            }
            return Json(new { success = message, reminderHtml = reminderHtml, userremindercount = userremindercount });
        }

        public ActionResult EditUserReminder(int id)
        {
            try
            {
                var reminder = db.UserWiseReminderManage
                             .Where(n => n.ReminderId == id)
                             .Select(n => new
                             {
                                 n.ReminderId,
                                 n.ReminderName,
                                 n.ReminderDescription,
                                 n.ReminderDate
                             })
                             .FirstOrDefault();

                if (reminder != null)
                {
                    return Json(new { success = true, reminder }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { success = false, message = "Reminder not found" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                logger.Error("Error in EditUserReminder: " + ex.Message);
                return Json(new { success = false, message = "Error occurred" }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult DeleteUserReminder(int id)
        {
            string reminderHtml = "";
            int userremindercount = 0;
            try
            {
                var existingReminder = db.UserWiseReminderManage.Find(id);
                if (existingReminder != null)
                {
                    existingReminder.ModifiedOn = DateTime.Now;
                    existingReminder.Isdeleted = true;
                    db.Entry(existingReminder).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();

                    int ReminderUserId = UserModule.getUserId();
                    var reminders = db.UserWiseReminderManage.Where(n => n.Isdeleted != true && n.Createdby == ReminderUserId).OrderByDescending(n => n.CreatedOn).ToList();
                    foreach (var reminder in reminders)
                    {
                        var deleteIconPath = Url.Content("~/Content/Admin/images/icons/delete.svg");
                        var editIconPath = Url.Content("~/Content/Admin/images/icons/edit-05.svg");
                        var remindericon = Url.Content("~/Content/Admin/images/icons/reminder-listing-icon.svg");
                        //reminderHtml += $"<div class='reminder'>" +
                        //         $"<input type='hidden' class='reminderId' value='{reminder.ReminderId}' />" +
                        //         $"<div class='reminder-section-01'>" +
                        //         $"<img src='{remindericon}' />" +
                        //         $"<span>{reminder.ReminderName} :- {reminder.ReminderDescription}</span>" +
                        //         $"</div>" +
                        //         $"<div class='actionBar'>" +
                        //         $"<div class='deleteReminderTs'><img src='{deleteIconPath}' alt='Delete' /></div>" +
                        //         $"<div class='editReminderTs'><img src='{editIconPath}' alt='Edit' /></div>" +
                        //         $"</div></div>";
                        reminderHtml += $"<div class='reminder'>" +
                                    $"<input type='hidden' class='reminderId' value='{reminder.ReminderId}' />" +
                                    $"<div class='reminder-section-01'>" +
                                    $"<img src='{remindericon}' />" +
                                    $"<h1>{reminder.ReminderName}</h1>" +
                                    $"</div>" +
                                    $"<div class='actionBar'>" +
                                    $"<div class='deleteReminderTs'>" +
                                    $"<img src='{deleteIconPath}' alt='Delete' />" +
                                    $"</div>" +
                                    $"<div class='editReminderTs'>" +
                                    $"<img src='{editIconPath}' alt='Edit' />" +
                                    $"</div>" +
                                    $"</div>" +
                                    $"<div class='reminderContent'>" +
                                    $"<span>{reminder.ReminderDescription}</span>" +
                                    $"</div>" +
                                    $"</div>";
                    }
                    userremindercount = db.UserWiseReminderManage.Where(n => n.Isdeleted != true && n.Createdby == ReminderUserId).Count();

                    return Json(new { success = true, reminderHtml = reminderHtml, userremindercount = userremindercount }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { success = false, reminderHtml = reminderHtml, userremindercount = userremindercount }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                logger.Error("Error in DeleteUserReminder: " + ex.Message);
                return Json(new { success = false, message = "Error occurred" }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion userreminder
    }
}