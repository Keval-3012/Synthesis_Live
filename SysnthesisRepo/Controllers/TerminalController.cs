using EntityModels.Models;
using NLog;
using Repository;
using Repository.IRepository;
using SynthesisQBOnline.BAL;
using SynthesisQBOnline.QBClass;
using SynthesisQBOnline;
using SynthesisViewModal;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Utility;
using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.Spreadsheet;

namespace SysnthesisRepo.Controllers
{
    public class TerminalController : Controller
    {
        private readonly ITerminalRepository _terminalRepository;
        private readonly IQBRepository _qBRepository;
        private readonly IUserActivityLogRepository _activityLogRepository;
        private readonly ICommonRepository _CommonRepository;
        Logger logger = LogManager.GetCurrentClassLogger();
        TerminalViewModel MainTerminalView = new TerminalViewModel();
        TerminalViewModel terminalViewData = new TerminalViewModel();
        public TerminalController()
        {
            this._terminalRepository = new TerminalRepository(new DBContext());
            this._qBRepository = new QBRepository(new DBContext());
            this._activityLogRepository = new UserActivityLogRepository(new DBContext());
            this._CommonRepository = new CommonRepository(new DBContext());
        }

        /// <summary>
        /// This  method Is get terminal view data..
        /// </summary>
        /// <param name="terminalViewData"></param>
        /// <returns></returns>
        [Authorize(Roles = "Administrator,ViewShiftWiseReport")]
        public ActionResult Index(int StoreId = 0, string TerminalId = "", string StartDate = "", string shiftid = "")
        {
            Terminal_Select terminal_Select = new Terminal_Select();
            terminalViewData = new TerminalViewModel();
            terminalViewData.StoreId = StoreId;
            terminalViewData.TerminalId = TerminalId;
            terminalViewData.StartDate = StartDate;
            terminalViewData.shiftid = shiftid;
            try
            {
                if (terminalViewData.StoreId == 0 || terminalViewData.StartDate == "" || terminalViewData.TerminalId == "")
                {
                    if (MainTerminalView.IsFirst == false)
                    {
                        if ((string.IsNullOrEmpty(terminalViewData.StartDate)) && (terminalViewData.StStoreId != "0"))
                        {
                            try
                            {
                                //Get transaction start time .
                                MainTerminalView.StartDate = Common.GetDateformat(Convert.ToString(_terminalRepository.GetTransactionStartTimeDesc()));
                            }
                            catch (Exception ex)
                            {
                                logger.Error("TerminalController - Index - " + DateTime.Now + " - " + ex.Message.ToString());
                            }
                            ViewBag.Startdate = MainTerminalView.StartDate;
                        }
                        MainTerminalView.IsFirst = true;
                    }

                    ViewBag.Startdate = MainTerminalView.StartDate;
                    ViewBag.terminalidval = MainTerminalView.Terminalid_val;
                    ViewBag.Shiftdataid = MainTerminalView.Shiftid_val;
                    MainTerminalView.Terminalid_val = "";
                    MainTerminalView.Shiftid_val = "";
                    if(StoreId == 0)
                    {
                        MainTerminalView.StStoreId = Convert.ToString(Session["storeid"]);
                    }
                    else
                    {
                        MainTerminalView.StStoreId = StoreId.ToString();
                        Session["storeid"] = StoreId;
                    }
                    terminalViewData.StoreId = Convert.ToInt32(MainTerminalView.StStoreId);
                    if (!string.IsNullOrEmpty(MainTerminalView.StStoreId))
                    {
                        ViewBag.storeid = MainTerminalView.StStoreId;
                        MainTerminalView.StoreId = Convert.ToInt32(MainTerminalView.StStoreId);
                        if ((string.IsNullOrEmpty(MainTerminalView.StartDate)) && (MainTerminalView.StStoreId != "0"))
                        {
                            try
                            {
                                //Get transaction start time .
                                MainTerminalView.StartDate = Common.GetDateformat(Convert.ToString(_terminalRepository.GetTransactionStartTimeDesc()));



                                //IF GET ANY ISSUE THEN REMOVE THIS CODE START HERE
                                if (StartDate != "01/01/0001 00:00:00")
                                {
                                    if (!string.IsNullOrEmpty(terminalViewData.StartDate))
                                    {
                                        DateTime parsedDate;
                                        if (DateTime.TryParse(StartDate, out parsedDate))
                                        {
                                            string formattedDate = parsedDate.ToString("MM-dd-yyyy");
                                            terminalViewData.StartDate = formattedDate;
                                            MainTerminalView.StartDate = terminalViewData.StartDate;
                                        }
                                    }
                                }
                                //END HERE
                            }
                            catch (Exception ex)
                            {
                                logger.Error("TerminalController - Index - " + DateTime.Now + " - " + ex.Message.ToString());
                            }
                            ViewBag.Startdate = MainTerminalView.StartDate;
                        }

                        if (!string.IsNullOrEmpty(MainTerminalView.StartDate) && MainTerminalView.StStoreId != "0")
                        {
                            DateTime dDate;
                            MainTerminalView.SDate = Convert.ToDateTime(Common.GetDateformat(MainTerminalView.StartDate));
                            if (DateTime.TryParse(MainTerminalView.StartDate, out dDate))
                            {
                                String.Format("{0:d/MM/yyyy}", dDate);
                            }
                            else
                            {
                                MainTerminalView.StartDate = DateTime.ParseExact(MainTerminalView.StartDate, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture).ToString("MM/dd/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                            }
                            //Get other Deposit Datas.
                            terminal_Select.OtherDepositList = _terminalRepository.GetOtherDepositDatas(MainTerminalView);
                            MainTerminalView.flag = 1;
                        }
                    }
                }
                else
                {
                    ViewBag.Startdate = terminalViewData.StartDate;
                    MainTerminalView.Terminalid_val = "";
                    MainTerminalView.Shiftid_val = "";
                    try
                    {
                        if (terminalViewData.StoreId != 0)
                        {
                            ViewBag.storeid = Convert.ToString(terminalViewData.StoreId);
                            Session["storeid"] = terminalViewData.StoreId;
                            ViewBag.terminalid_val = terminalViewData.TerminalId;
                            MainTerminalView.Terminalid_val = terminalViewData.TerminalId;
                            if (!string.IsNullOrEmpty(terminalViewData.StartDate))
                            {
                                terminalViewData.SDate = Convert.ToDateTime(terminalViewData.StartDate);
                                //Get other Deposit Datas.
                                terminal_Select.OtherDepositList = _terminalRepository.GetOtherDepositDatas(terminalViewData);
                                MainTerminalView.flag = 2;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        logger.Error("TerminalController - Index - " + DateTime.Now + " - " + ex.Message.ToString());
                    }

                }

                if (MainTerminalView.flag == 1)
                {
                    //Get Total Avrage.
                    _terminalRepository.GetTotalAvrage(ref terminal_Select, MainTerminalView, terminalViewData.StoreId);
                }
                else if (MainTerminalView.flag == 2)
                {
                    //Get Total Avrage.
                    _terminalRepository.GetTotalAvrage(ref terminal_Select, terminalViewData, terminalViewData.StoreId);
                }
            }
            catch (Exception ex)
            {
                logger.Error("TerminalController - Index - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            ViewBag.DepositUpdate = _CommonRepository.GetMessageValue("SRDUSF", "Deposits updated Successfully");
            ViewBag.DepositAdedSucc = _CommonRepository.GetMessageValue("SRA", "Deposits added Successfully");
            ViewBag.DepositDelSucc = _CommonRepository.GetMessageValue("SRD", "Deposits deleted successfully");
            ViewBag.DepFileDel = _CommonRepository.GetMessageValue("SRDF", "Deposits file deleted successfully");
            ViewBag.SettleEntry = _CommonRepository.GetMessageValue("SRSE", "Settlement entry saved Successfully");
            ViewBag.SettleEntryDel = _CommonRepository.GetMessageValue("SRSED", "Settlement entry deleted Successfully");
            ViewBag.DelCashIn = _CommonRepository.GetMessageValue("SRCID", "Cash Invoice deleted successfully");
            ViewBag.SureCloseout = _CommonRepository.GetMessageValue("SRY", "Are You Sure You Want To Close Out The Day?");
            ViewBag.AlGenerated = _CommonRepository.GetMessageValue("SRAGU", "Closing Report Is Already Generated, Are You Sure You Want To Update It?");
            ViewBag.AlApproved = _CommonRepository.GetMessageValue("SRAAU", "Closing Report Is Already Approved, Are You Sure You Want To Update It?");
            ViewBag.AlGenUpdated = _CommonRepository.GetMessageValue("SRAGUU", "Closing Report Is Already Generated & Updated, Are You Sure You Want To Update It?");
            ViewBag.AlAppUpdated = _CommonRepository.GetMessageValue("SRCRAU", "Closing Report Is Already Approved & Updated, Are You Sure You Want To Update It?");
            return View(terminal_Select);
        }

        /// <summary>
        /// This Grid method return terminal list.
        /// </summary>
        /// <param name="terminalViewData"></param>
        /// <returns></returns>
        public ActionResult TerminalGrid(string date = "")
        {
            Terminal_Select terminal_Select = new Terminal_Select();
            terminalViewData = new TerminalViewModel();
            terminalViewData.date = date;
            try
            {
                ViewBag.terminalid_val = MainTerminalView.Terminalid_val;

                if (Convert.ToInt32(Session["storeid"]) != 0)
                {
                    MainTerminalView.StoreId = Convert.ToInt32(Session["storeid"]);
                }
                if ((string.IsNullOrEmpty(terminalViewData.date)) && (MainTerminalView.StoreId != 0))
                {
                    //Get transaction start time .
                    terminalViewData.date = MainTerminalView.StartDate = Common.GetDateformat(Convert.ToString(_terminalRepository.GetTransactionStartTimeDesc()));
                }
                try
                {
                    DateTime parsedDate;
                    string format = "MM-dd-yyyy";
                    if (DateTime.TryParseExact(terminalViewData.date, format, null, System.Globalization.DateTimeStyles.None, out parsedDate))
                    {
                        MainTerminalView.CDate = parsedDate;
                    }

                    //MainTerminalView.CDate = Convert.ToDateTime(terminalViewData.date);
                    //Get Terminal data with main terminal view.
                    _terminalRepository.GetTerminalData(ref terminal_Select, MainTerminalView);

                    if (terminal_Select.TerminalData.Count > 0)
                    {
                        MainTerminalView.TerminalId = terminal_Select.TerminalData[0].TerminalId.ToString();
                        ViewBag.terminalidval = MainTerminalView.TerminalId;
                        MainTerminalView.Terminalid_val = MainTerminalView.TerminalId;
                        ViewBag.Shiftdataid = "";
                    }
                    else
                    {
                        ViewBag.DepositeCount = 0;
                    }

                    if (terminal_Select.TerminalData.Count == 0)
                    {
                        MainTerminalView.ErrorMessage = "NoItemFound";
                        ViewBag.ErrorMessage = MainTerminalView.ErrorMessage;
                        MainTerminalView.ErrorMessage = "";
                    }
                    //get All selected vendor List
                    _terminalRepository.GetSelectedVendorList(MainTerminalView.StoreId, ref terminal_Select);

                    //Get Un Assign Shift Column With storeid and date.
                    ViewBag.TotalCount = _terminalRepository.GetUnassignShiftCount(MainTerminalView.StoreId, terminalViewData.date);

                    DayCloseOutStatus DayCount = _terminalRepository.GetDayCloseOutStatus_Data(MainTerminalView.StoreId, terminalViewData.date);
                    if (DayCount == null)
                    {
                        ViewBag.DayCloseCount = 0;
                    }
                    else
                    {
                        ViewBag.DayCloseCount = DayCount.DayCloseOutCount;
                    }
                }
                catch (Exception ex)
                {
                    logger.Error("TerminalController - TerminalGrid - " + DateTime.Now + " - " + ex.Message.ToString());
                }
            }
            catch (Exception ex)
            {
                logger.Error("TerminalController - TerminalGrid - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return PartialView(terminal_Select);
        }

        /// <summary>
        /// This  method is get shift data  list.
        /// </summary>
        /// <param name="terminalViewData"></param>
        /// <returns></returns>
        public ActionResult ShiftDataGrid(string date = "", string terminalid = "")
        {
            Terminal_Select terminal_Select = new Terminal_Select();
            terminalViewData = new TerminalViewModel();
            terminalViewData.date = date;
            terminalViewData.TerminalId = terminalid;
            ViewBag.terminalidval = "";
            ViewBag.Shiftdataid = "";
            try
            {
                if (Convert.ToInt32(Session["storeid"]) != 0)
                {
                    terminalViewData.StoreId = Convert.ToInt32(Session["storeid"]);
                }
                if ((string.IsNullOrEmpty(terminalViewData.date)) && (MainTerminalView.StoreId != 0))
                {
                    ////Get transaction start time .
                    terminalViewData.date = MainTerminalView.StartDate = Common.GetDateformat(Convert.ToString(_terminalRepository.GetTransactionStartTimeDesc()));
                }
                terminalViewData.CDate = Convert.ToDateTime(terminalViewData.date);
                if (!string.IsNullOrEmpty(terminalViewData.TerminalId) && terminalViewData.StoreId != 0)
                {
                    //Set Shift data Grid with Value.
                    _terminalRepository.SetShiftDataGridValue(ref terminal_Select, terminalViewData);
                }
            }
            catch (Exception ex)
            {
                logger.Error("TerminalController - ShiftDataGrid - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return PartialView(terminal_Select);
        }

        /// <summary>
        /// This method is get shift Wise Tender Grid 
        /// </summary>
        /// <param name="terminalViewData"></param>
        /// <returns></returns>
        public ActionResult ShiftWiseTenderGrid(string date = "", string terminalid = "", string shiftid = "")
        {
            ViewBag.ErrorMessage = MainTerminalView.ErrorMessage;
            MainTerminalView.ErrorMessage = "";
            Terminal_Select terminal_Select = new Terminal_Select();
            terminalViewData = new TerminalViewModel();
            terminalViewData.date = date;            
			terminalViewData.TerminalId = terminalid;
            terminalViewData.shiftid = shiftid;
            try
            {
                if (Convert.ToInt32(Session["storeid"]) != 0)
                {
                    terminalViewData.StoreId = Convert.ToInt32(Session["storeid"]);
                }
                if ((string.IsNullOrEmpty(terminalViewData.date)) && (MainTerminalView.StoreId != 0))
                {
                    //Get transaction start time .
                    terminalViewData.date = Common.GetDateformat(Convert.ToString(_terminalRepository.GetTransactionStartTimeDesc()));
                }

                if (!string.IsNullOrEmpty(terminalViewData.TerminalId) && !string.IsNullOrEmpty(terminalViewData.shiftid))
                {
                    terminalViewData.terminal_Id = Convert.ToInt32(terminalViewData.TerminalId);
                    terminalViewData.shift_Id = Convert.ToInt32(terminalViewData.shiftid);
                    //Get all Store Terminal name.
                    ViewBag.TerminalName = _terminalRepository.GetStoreTerminalName(terminalViewData);

                    terminal_Select.terminal_id = terminalViewData.TerminalId;
                    //get Shift Wise Tender Data.
                    _terminalRepository.GetShiftWisetenderData(ref terminal_Select, terminalViewData);

                    if (terminal_Select.ShiftWisetenderData != null)
                    {
                        ViewBag.CustomerCount = terminal_Select.ShiftWisetenderData.Count() > 0 ? terminal_Select.ShiftWisetenderData[0].CustomerCount : 0;
                    }
                    else
                    {
                        ViewBag.CustomerCount = 0;
                    }

                    ViewBag.IsSettlementDone = 0;
                    if (terminal_Select.ShiftWisetenderData.Count > 0)
                    {
                        if (terminal_Select.ShiftWisetenderData[0].paidoutLists.Count == 0)
                        {
                            ViewBag.PaidOut_SettlementCount = 0;
                        }
                        else
                        {
                            if (terminal_Select.ShiftWisetenderData[0].paidoutLists[0].PaidOut_SettlementList == null)
                            {
                                ViewBag.PaidOut_SettlementCount = 0;
                            }
                            else
                            {
                                decimal sTotal = terminal_Select.ShiftWisetenderData[0].paidoutLists[0].BindCase_PaidOut_Invoice.Sum(a => a.Totalamount);
                                if (sTotal == Convert.ToDecimal(terminal_Select.ShiftWisetenderData[0].paidoutLists[0].Amount))
                                {
                                    ViewBag.IsSettlementDone = 1;
                                }
                                ViewBag.PaidOut_SettlementCount = terminal_Select.ShiftWisetenderData[0].paidoutLists[0].PaidOut_SettlementList.Count();
                            }
                        }
                    }
                    else
                    {
                        ViewBag.PaidOut_SettlementCount = 0;
                    }

                    string ss = terminal_Select.ShiftWisetenderData.Count() > 0 ? terminal_Select.ShiftWisetenderData[0].ShiftName.ToString() : "";
                    //Set cradit Crad details.
                    _terminalRepository.SetCraditCardDetail(ref terminal_Select, terminalViewData);
                    ViewBag.EmptyTender = 0;
                }
                else
                {
                    ViewBag.EmptyTender = 1;
                    ViewBag.CustomerCount = 0;
                    MainTerminalView.ErrorMessage = "NoItemFound";
                    ViewBag.ErrorMessage = MainTerminalView.ErrorMessage;
                    MainTerminalView.ErrorMessage = "";
                }

                if (string.IsNullOrEmpty(terminalViewData.TerminalId))
                {
                    terminalViewData.terminal_Id = 0;
                }
                else
                {
                    terminalViewData.terminal_Id = Convert.ToInt32(terminalViewData.TerminalId);
                }

                if (string.IsNullOrEmpty(terminalViewData.shiftid))
                {
                    terminalViewData.shift_Id = 0;
                }
                else
                {
                    terminalViewData.shift_Id = Convert.ToInt32(terminalViewData.shiftid);
                }
                //Set other deposite data.
                _terminalRepository.SetOtherDepositeData(ref terminal_Select, terminalViewData);
            }
            catch (Exception ex)
            {
                logger.Error("TerminalController - ShiftWisetenderGrid - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            ViewBag.CreditCard = _CommonRepository.GetMessageValue("SRCCMC", "Credit cards total values are not matching with the actual credit card values.");
            return PartialView(terminal_Select);
        }

        /// <summary>
        /// This method is shift wise tender.
        /// </summary>
        /// <param name="Posteddata"></param>
        /// <param name="terminalViewData"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ShiftWisetenderGrid(Terminal_Select Posteddata, string[] Title, string[] Amount)
        {
            terminalViewData = new TerminalViewModel();
            terminalViewData.Title = Title;
            terminalViewData.Amount = Amount;
            if (ModelState.IsValid)
            {
                Terminal_Select terminal_Select = new Terminal_Select();
                try
                {
                    if (!string.IsNullOrEmpty(Convert.ToString(Session["storeid"])))
                    {
                        terminalViewData.StoreId = Convert.ToInt32(Session["storeid"]);
                        //This class is check Sales Activiy Summaries Exist or not
                        terminalViewData.IsFirst = _terminalRepository.CheckSalesActivitySummariesExistOrNot(ref Posteddata, ref terminalViewData);
                        if (terminalViewData.IsFirst == false)
                        {
                            //Update Credit card details
                            _terminalRepository.UpdateCreditcardDetails(ref Posteddata, ref terminalViewData);
                            terminalViewData.ErrorMessage = "Sucess";
                        }
                        else
                        {
                            terminalViewData.ErrorMessage = "ExistShift";
                        }
                    }
                }
                catch (Exception ex)
                {
                    logger.Error("TerminalController - ShiftWisetenderGrid - " + DateTime.Now + " - " + ex.Message.ToString());
                }
                return RedirectToAction("Index", new { StartDate = MainTerminalView.SDate });
            }
            return RedirectToAction("Index", Posteddata);
        }

        /// <summary>
        /// This method save Other deposite data.
        /// </summary>
        /// <param name="sid"></param>
        /// <param name="date"></param>
        /// <param name="payment"></param>
        /// <param name="amount"></param>
        /// <param name="options"></param>
        /// <param name="vendor"></param>
        /// <param name="Department"></param>
        /// <param name="Other"></param>
        /// <param name="name"></param>
        /// <param name="Terminal"></param>
        /// <param name="Shift"></param>
        /// <param name="ActivitySalesSummuryId"></param>
        /// <param name="UploadFile"></param>
        /// <returns></returns>
        public JsonResult SaveOtherDepositData(int sid, string date, string payment, string amount, string options, string vendor, string Department, string Other = "", string name = "", int Terminal = 0, int? Shift = null, int ActivitySalesSummuryId = 0, HttpPostedFileBase UploadFile = null)
        {
            terminalViewData = new TerminalViewModel();
            terminalViewData.sid = sid;
            terminalViewData.date = date;
            terminalViewData.payment = payment;
            terminalViewData.amount = amount;
            terminalViewData.options = options;
            terminalViewData.vendor = vendor;
            terminalViewData.Department = Department;
            terminalViewData.Other = Other;
            terminalViewData.name = name;
            terminalViewData.Terminal = Terminal;
            terminalViewData.Shift = Shift;
            terminalViewData.ActivitySalesSummuryId = ActivitySalesSummuryId;
            terminalViewData.UploadFile = UploadFile;
            try
            {
                if ((!string.IsNullOrEmpty(terminalViewData.date)))
                {
                    if (terminalViewData.amount != "")
                    {
                        terminalViewData.AmountValue = Convert.ToDecimal(terminalViewData.amount);
                    }
                    if (terminalViewData.UploadFile != null)
                    {
                        if (terminalViewData.UploadFile.ContentLength > 0)
                        {
                            terminalViewData.Extensions = new[] { ".pdf", ".docx", ".xlsx" };
                            terminalViewData.Ext = Convert.ToString(Path.GetExtension(terminalViewData.UploadFile.FileName)).ToLower();
                            if (!terminalViewData.Extensions.Contains(terminalViewData.Ext))
                            {
                                ViewBag.StatusMessage = "InvalidImage";
                                return Json("Notsuccess", JsonRequestBehavior.AllowGet);
                            }
                            else
                            {
                                if (terminalViewData.UploadFile.ContentLength > 30971520)
                                {
                                    ViewBag.StatusMessage = "InvalidPDFSize";
                                    return Json("Notsuccess", JsonRequestBehavior.AllowGet);

                                }
                                else
                                {
                                    terminalViewData.UploadFile_Title = Common.GetRandomNo() + Path.GetFileName(terminalViewData.UploadFile.FileName);
                                    terminalViewData.UploadFile_Title = AdminSiteConfiguration.RemoveSpecialCharacter(terminalViewData.UploadFile_Title);
                                    if (!System.IO.Directory.Exists(Request.PhysicalApplicationPath + "UserFiles\\OtherDepositeDoc"))
                                    {
                                        System.IO.Directory.CreateDirectory(Request.PhysicalApplicationPath + "UserFiles\\OtherDepositeDoc");
                                    }
                                    var path1 = Request.PhysicalApplicationPath + "UserFiles\\OtherDepositeDoc" + "\\" + terminalViewData.UploadFile_Title;
                                    terminalViewData.UploadFile.SaveAs(path1);

                                }
                            }
                        }
                    }
                    //Save Other Deposite
                    _terminalRepository.SaveOtherDeposit(ref terminalViewData);
                    terminalViewData.ErrorMessage = "sucess";
                }
                return Json(terminalViewData.ErrorMessage, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error("TerminalController - SaveOtherDepositData - " + DateTime.Now + " - " + ex.Message.ToString());
                return Json("Notsuccess", JsonRequestBehavior.AllowGet);
            }

        }

        /// <summary>
        /// This method is Update Other deposit data.
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="payment"></param>
        /// <param name="amount"></param>
        /// <param name="options"></param>
        /// <param name="vendor"></param>
        /// <param name="Department"></param>
        /// <param name="Other"></param>
        /// <param name="name"></param>
        /// <param name="ActivitySalesSummuryId"></param>
        /// <param name="UploadFile"></param>
        /// <returns></returns>
        public JsonResult Updateotherdepositdata(int Id, string payment, string amount, string options, string vendor, string Department, string Other = "", string name = "", int ActivitySalesSummuryId = 0, HttpPostedFileBase UploadFile = null)
        {
            terminalViewData = new TerminalViewModel();
            terminalViewData.Id = Id;
            terminalViewData.payment = payment;
            terminalViewData.amount = amount;
            terminalViewData.options = options;
            terminalViewData.vendor = vendor;
            terminalViewData.Department = Department;
            terminalViewData.Other = Other;
            terminalViewData.name = name;
            terminalViewData.ActivitySalesSummuryId = ActivitySalesSummuryId;
            terminalViewData.UploadFile = UploadFile;
            try
            {
                if (terminalViewData.Id != 0)
                {
                    if (terminalViewData.amount != "")
                    {
                        terminalViewData.AmountValue = Convert.ToDecimal(terminalViewData.amount);
                    }
                    if (terminalViewData.UploadFile != null)
                    {
                        if (terminalViewData.UploadFile.ContentLength > 0)
                        {
                            var allowedExtensions = new[] { ".pdf", "docx", ".xlsx" };
                            var extension = Path.GetExtension(terminalViewData.UploadFile.FileName);
                            var Ext = Convert.ToString(extension).ToLower();
                            if (!allowedExtensions.Contains(Ext))
                            {
                                ViewBag.StatusMessage = "InvalidImage";
                                return Json("Notsuccess", JsonRequestBehavior.AllowGet);
                            }
                            else
                            {
                                if (terminalViewData.UploadFile.ContentLength > 30971520)
                                {
                                    ViewBag.StatusMessage = "InvalidPDFSize";
                                    return Json("Notsuccess", JsonRequestBehavior.AllowGet);

                                }
                                else
                                {
                                    terminalViewData.UploadFile_Title = AdminSiteConfiguration.GetRandomNo() + Path.GetFileName(terminalViewData.UploadFile.FileName);
                                    terminalViewData.UploadFile_Title = AdminSiteConfiguration.RemoveSpecialCharacter(terminalViewData.UploadFile_Title);
                                    if (!System.IO.Directory.Exists(Request.PhysicalApplicationPath + "userfiles\\Otherdepositedoc"))
                                    {
                                        System.IO.Directory.CreateDirectory(Request.PhysicalApplicationPath + "userfiles\\Otherdepositedoc");
                                    }
                                    var path1 = Request.PhysicalApplicationPath + "userfiles\\Otherdepositedoc" + "\\" + terminalViewData.UploadFile_Title;
                                    terminalViewData.UploadFile.SaveAs(path1);
                                    terminalViewData.flag = 1;
                                }
                            }
                        }
                    }
                    //Update Other Deposite
                    _terminalRepository.UpdateOtherDeposit(ref terminalViewData);
                    terminalViewData.ErrorMessage = "sucess";
                }
                return Json(terminalViewData.ErrorMessage, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error("TerminalController - Updateotherdepositdata - " + DateTime.Now + " - " + ex.Message.ToString());
                return Json("Notsuccess", JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// This method is GetOther Deposit Grid.
        /// </summary>
        /// <param name="date"></param>
        /// <param name="TerminalID"></param>
        /// <param name="SalesActivityId"></param>
        /// <returns></returns>
        public ActionResult OtherDepositGrid(string date = "", int TerminalID = 0, int SalesActivityId = 0)
        {
            Terminal_Select terminal_Select = new Terminal_Select();
            terminalViewData = new TerminalViewModel();
            terminalViewData.date = date;
            terminalViewData.TerminalId = TerminalID.ToString();
            terminalViewData.SalesActivityId = SalesActivityId;
            MainTerminalView.StoreId = Convert.ToInt32(Session["storeid"]);
            try
            {
                ViewBag.DepositeCount = 0;
                if (!string.IsNullOrEmpty(Convert.ToString(Session["storeid"])))
                {
                    ViewBag.storeid = terminalViewData.StStoreId = Convert.ToString(Session["storeid"]);
                    terminalViewData.StoreId = Convert.ToInt32(terminalViewData.StStoreId);
                    List<OtherDeposit> OtherDepositeData = new List<OtherDeposit>();
                    if (!string.IsNullOrEmpty(terminalViewData.date))
                    {
                        // shift id condition while get other depositedata, Check this sp tbl_otherdeposit_selectbystoreid_TerminalId
                        terminalViewData.terminal_Id = String.IsNullOrEmpty(terminalViewData.TerminalId) ? 0 : Convert.ToInt32(terminalViewData.TerminalId);
                        //This class is set deposite data.
                        ViewBag.DepositeCount = _terminalRepository.SetDepositeData(ref terminal_Select, terminalViewData);
                    }
                }
                if (terminalViewData.terminal_Id == 0)
                {
                    ViewBag.TerminalName = "";
                }
                else
                {
                    //Get all Store Terminal name
                    ViewBag.TerminalName = _terminalRepository.GetStoreTerminalName(terminalViewData);
                }
                //Set Option List
                _terminalRepository.SetOptionList(ref terminal_Select);
                //Set ShiftName List
                _terminalRepository.SetShiftNameList(ref terminal_Select);
                //Get Selected Vendor List
                _terminalRepository.GetSelectedVendorList(MainTerminalView.StoreId, ref terminal_Select);
            }
            catch (Exception ex)
            {
                logger.Error("TerminalController - OtherDepositGrid - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return View(terminal_Select);
        }

        /// <summary>
        /// This method is delete Other Deposit data.
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public JsonResult Deleteotherdepositdata(int Id)
        {
            terminalViewData = new TerminalViewModel();
            terminalViewData.Id = Id;
            if (terminalViewData.Id != 0)
            {
                try
                {
                    //Delete Other Depositdata
                    _terminalRepository.Deleteotherdepositdata(terminalViewData);
                    terminalViewData.ErrorMessage = "sucess";
                }
                catch (Exception ex)
                {
                    logger.Error("TerminalController - Deleteotherdepositdata - " + DateTime.Now + " - " + ex.Message.ToString());
                }
            }
            return Json(terminalViewData.ErrorMessage, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// This method is save settlement Entry
        /// </summary>
        /// <param name="Title"></param>
        /// <param name="SettlementID"></param>
        /// <param name="Amount"></param>
        /// <returns></returns>
        public JsonResult SaveSettlementEntry(string Title, int SettlementID, decimal Amount)
        {
            TerminalSettlement terminalSettlement = new TerminalSettlement();
            terminalSettlement.Title = Title;
            terminalSettlement.SettlementID = SettlementID;
            terminalSettlement.Amount = Amount;

            try
            {
                if (terminalSettlement.SettlementID != 0)
                {
                    //Save Paid Out Settelement
                    terminalSettlement.ErrorMessage = _terminalRepository.SavePaidOutSettlement(terminalSettlement);
                }
            }
            catch (Exception ex)
            {
                logger.Error("TerminalController - SaveSettlementEntry - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return Json(terminalSettlement.ErrorMessage, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// This method is Delete settlement Entry
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public ActionResult DeleteSettlementEntry(int Id = 0)
        {
            try
            {
                //Delete settelement ENtry
                terminalViewData.Id = Id;
                _terminalRepository.DeleteSettlementEntry(terminalViewData);
            }
            catch (Exception ex)
            {
                logger.Error("TerminalController - DeleteSettlementEntry - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return Json("Delete", JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// This method is Get payment List.
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public JsonResult GetPaymethodlist(string Id)
        {
            List<ddllist> paymethodlist = new List<ddllist>();
            try
            {
                //get Pay Details list by id
                paymethodlist = _terminalRepository.GetPayDetailList(Id);
            }
            catch (Exception ex)
            {
                logger.Error("TerminalController - Daycloseout - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return Json(new SelectList(paymethodlist.ToArray(), "Value", "Text"), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// This method get Day CloseOut.
        /// </summary>
        /// <param name="date_val"></param>
        /// <param name="IsSettlementDone"></param>
        /// <returns></returns>
        public JsonResult Daycloseout(string date_val = "", int IsSettlementDone = 0)
        {
            terminalViewData = new TerminalViewModel();
            terminalViewData.date_val = date_val;
            terminalViewData.IsSettlementDone = IsSettlementDone;
            try
            {
                if (!string.IsNullOrEmpty(Convert.ToString(terminalViewData.date_val)))
                {
                    terminalViewData.UserID = UserModule.getUserId();
                    if (!string.IsNullOrEmpty(Convert.ToString(Session["storeid"])))
                    {
                        terminalViewData.StoreId = Convert.ToInt32(Session["storeid"]);
                    }
                    //Set Day Close Out
                    terminalViewData.ErrorMessage = _terminalRepository.SetDayCloseOut(terminalViewData);
                    ViewBag.DayCloseCount = 1;
                }
                else
                {
                    //terminalViewData.ErrorMessage = "Please Select Some Date==" + terminalViewData.date_val + " DateVal " + terminalViewData.date_val;
                    terminalViewData.ErrorMessage = _CommonRepository.GetMessageValue("SRTD", "Please Select Some Date==## terminalViewData.date_val## DateVal ##terminalViewData.date_val##").Replace("## terminalViewData.date_val##", terminalViewData.date_val).Replace("##terminalViewData.date_val##", terminalViewData.date_val);
                }

            }
            catch (Exception ex)
            {
                logger.Error("TerminalController - Daycloseout - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return Json(terminalViewData.ErrorMessage, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// This method is download Other File.
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public ActionResult DownloadOtherFile(string filePath)
        {
            terminalViewData = new TerminalViewModel();
            terminalViewData.filePath = filePath;
            try
            {
                terminalViewData.fullName = Server.MapPath("~/UserFiles/OtherDepositeDoc/" + terminalViewData.filePath);
                terminalViewData.fileBytes = Common.GetFile(terminalViewData.fullName);
            }
            catch (Exception ex)
            {
                logger.Error("TerminalController - DownloadOtherFile - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return File(terminalViewData.fileBytes, System.Net.Mime.MediaTypeNames.Application.Pdf);
        }

        /// <summary>
        /// This method is get Not ConfigureAccount
        /// </summary>
        /// <param name="StoreID"></param>
        /// <returns></returns>
        public string GetNotConfigureAccount(int StoreID)
        {
            terminalViewData = new TerminalViewModel();
            terminalViewData.StoreId = StoreID;
            try
            {

                //this class is Get Tender Accounts Store Wise
                List<TenderAccountsStoreWise> AccList = _terminalRepository.getTenderAccountsStoreWise(terminalViewData);
                foreach (var item in AccList)
                {
                    terminalViewData.Acc = (terminalViewData.Acc == "" ? item.Title : terminalViewData.Acc + ", " + item.Title);
                }
            }
            catch (Exception ex)
            {
                logger.Error("TerminalController - GetNotConfigureAccount - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return terminalViewData.Acc;
        }

        /// <summary>
        /// This method is Delete Tender Entry
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public ActionResult DeleteTenderEntry(int Id = 0)
        {
            terminalViewData = new TerminalViewModel();
            terminalViewData.Id = Id;
            try
            {
                //This class is delete Tender Entry
                _terminalRepository.DeleteTenderEntry(terminalViewData);
            }
            catch (Exception ex)
            {
                logger.Error("TerminalController - DeleteTenderEntry - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return Json("Delete", JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        ///  This method is Delete other Postit File
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public ActionResult DeleteotherdepositFile(int Id = 0)
        {
            terminalViewData = new TerminalViewModel();
            terminalViewData.Id = Id;
            try
            {
                //Delete Other Deposit File
                _terminalRepository.DeleteotherdepositFile(terminalViewData);
            }
            catch (Exception ex)
            {
                logger.Error("TerminalController - DeleteotherdepositFile - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return Json("Delete", JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        ///  This method is Get Status.
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public JsonResult GetStatus(DateTime date)
        {
            terminalViewData = new TerminalViewModel();
            terminalViewData.SDate = date;
            try
            {
                if (!string.IsNullOrEmpty(Convert.ToString(Session["storeid"])))
                {
                    terminalViewData.StoreId = Convert.ToInt32(Session["storeid"]);
                }
                //get sales General Entries
                terminalViewData.ErrorMessage = _terminalRepository.GetSalesGeneralEntries_Data(terminalViewData);
            }
            catch (Exception ex)
            {
                logger.Error("TerminalController - GetStatus - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return Json(terminalViewData.ErrorMessage, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// This method is return Create cash Invoice.
        /// </summary>
        /// <returns></returns>
        public ActionResult CreateCashInvoice()
        {
            Invoice obj = new Invoice();
            try
            {
                ViewBag.CurrentDate = DateTime.Today.Date;
                TerminalViewModel terminalViewModel = new TerminalViewModel();
                if (Convert.ToString(Session["storeid"]) != "0")
                {
                    terminalViewModel.StoreId = Convert.ToInt32(Session["storeid"]);
                }
                //This class is Cash Invoice
                CreateInvoiceDetail detail = _terminalRepository.SetCaseInvoice(terminalViewModel);
                obj.DepartmentMasters = detail.DepartmentMasters;
                if (Convert.ToString(Session["storeid"]) != "0")
                {
                    obj.StoreId = Convert.ToInt32(Session["storeid"]);
                    ViewBag.StoreId = new SelectList(detail.StoreMasters, "StoreId", "NickName", obj.StoreId);
                    ViewBag.VendorId = new SelectList(detail.VendorMasters, "VendorId", "VendorName");
                    ViewBag.Disc_Dept_id = new SelectList(detail.DepartmentMasters_store, "DepartmentId", "DepartmentName");
                }
                else
                {
                    ViewBag.StoreId = new SelectList(detail.StoreMasters, "StoreId", "NickName");
                    ViewBag.VendorId = new SelectList("");
                    ViewBag.Disc_Dept_id = new SelectList("");
                }
                ViewBag.DiscountTypeId = new SelectList(detail.DiscountTypeMasters, "DiscountTypeId", "DiscountType", 1);
                ViewBag.InvoiceTypeId = new SelectList(detail.InvoiceTypeMasters, "InvoiceTypeId", "InvoiceType");
                ViewBag.PaymentTypeId = new SelectList(detail.PaymentTypeMasters, "PaymentTypeId", "PaymentType");
                if (MainTerminalView.StatusMessage == "Success1" || MainTerminalView.StatusMessage == "Success" || MainTerminalView.StatusMessage == "Exists" || MainTerminalView.StatusMessage == "Existence")
                {
                    ViewBag.StatusMessage = MainTerminalView.StatusMessage;
                    MainTerminalView.StatusMessage = "";
                }
                else
                {
                    ViewBag.StatusMessage = "";
                }
                ViewBag.closingyear = Convert.ToInt32(ConfigurationManager.AppSettings["ClosingYear"].ToString()) + 1;
            }
            catch (Exception ex)
            {
                logger.Error("TerminalController - CreateCashInvoice - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            ViewBag.ShiftAssign = _CommonRepository.GetMessageValue("SRCO", "After shift assign you can create invoice.");
            return PartialView("_CreateCashInvoice", obj);
        }

        /// <summary>
        /// This method is  Create cash Invoice.
        /// </summary>
        /// <param name="invoice"></param>
        /// <param name="UploadInvoice"></param>
        /// <param name="ChildDepartmentId"></param>
        /// <param name="ChildAmount"></param>
        /// <param name="addnew"></param>
        /// <param name="btnsubmit"></param>
        /// <param name="TerminalId"></param>
        /// <param name="ShiftID"></param>
        /// <param name="PaidOutID"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> CreateCashInvoice(Invoice invoice, HttpPostedFileBase UploadInvoice, string[] ChildDepartmentId, string[] ChildAmount, string addnew = "", string btnsubmit = "", int TerminalId = 0, int ShiftID = 0, int PaidOutID = 0)
        {
            TerminalViewModel terminalViewModel = new TerminalViewModel();
            terminalViewModel.invoice = invoice;
            terminalViewModel.UploadInvoice = UploadInvoice;
            terminalViewModel.ChildDepartmentId = ChildDepartmentId;
            terminalViewModel.ChildAmount = ChildAmount;
            terminalViewModel.addnew = addnew;
            terminalViewModel.btnsubmit = btnsubmit;
            terminalViewModel.TerminalId = TerminalId.ToString();
            terminalViewModel.ShiftID = ShiftID;
            terminalViewModel.PaidOutID = PaidOutID;
            try
            {

                if (terminalViewModel.invoice.InvoiceDate == null)
                {
                    if (terminalViewModel.invoice.strInvoiceDate == null || terminalViewModel.invoice.strInvoiceDate == "")
                    {
                        ModelState.AddModelError("strInvoiceDate", "Required");
                    }
                }

                // Ignore City from ModelState.
                terminalViewModel.invoice.QBTransfer = (terminalViewModel.invoice.QBtransferss == "1" ? true : false);
                if (terminalViewModel.invoice.DiscountTypeId == 1)
                {
                    ModelState.Remove("Disc_Dept_id");
                }
                else
                {
                    ModelState.AddModelError("Disc_Dept_id", "Required");
                }
                if (ModelState.IsValid)
                {
                    //Using this db class get stores on Line desktop.
                    terminalViewModel.Store = _qBRepository.GetStoreOnlineDesktop(terminalViewModel.invoice.StoreId);
                    //Using this db class get stores on Line desktop flag.
                    terminalViewModel.StoreFlag = _qBRepository.GetStoreOnlineDesktopFlag(terminalViewModel.invoice.StoreId);

                    char[] MyChar = { '0' };
                    string Inoicno = terminalViewModel.invoice.InvoiceNumber.TrimStart(MyChar);
                    terminalViewModel.invoice.InvoiceNumber = Inoicno;
                    if (terminalViewModel.invoice.strInvoiceDate != null)
                    {
                        terminalViewModel.invoice.InvoiceDate = Convert.ToDateTime(terminalViewModel.invoice.strInvoiceDate);
                    }
                    if (Roles.IsUserInRole("Administrator"))
                    {
                        terminalViewModel.StoreId = terminalViewModel.invoice.StoreId;
                    }
                    else
                    {
                        if (Convert.ToString(Session["storeid"]) != "0")
                        {
                            terminalViewModel.StoreId = Convert.ToInt32(Session["storeid"].ToString());
                        }
                        else
                        {
                            RedirectToAction("Index", "Login", new { area = "" });
                        }
                    }

                    #region AttachNote
                    if (terminalViewModel.UploadInvoice != null)
                    {
                        if (terminalViewModel.UploadInvoice.ContentLength > 0)
                        {
                            var allowedExtensions = new[] { ".pdf" };
                            var extension = Path.GetExtension(terminalViewModel.UploadInvoice.FileName);
                            var Ext = Convert.ToString(extension).ToLower();
                            if (!allowedExtensions.Contains(Ext))
                            {
                                ViewBag.StatusMessage = "InvalidImage";
                                //This class is Upload File Extantion Invalid
                                CreateInvoiceDetail detail = _terminalRepository.UploadFileExtantionInvalid(terminalViewModel);
                                terminalViewModel.invoice.DepartmentMasters = detail.DepartmentMasters;
                                ViewBag.Disc_Dept_id = new SelectList(detail.DepartmentMasters_store, "DepartmentId", "DepartmentName", terminalViewModel.invoice.Disc_Dept_id);
                                ViewBag.DiscountTypeId = new SelectList(detail.DiscountTypeMasters, "DiscountTypeId", "DiscountType", terminalViewModel.invoice.DiscountTypeId);
                                ViewBag.InvoiceTypeId = new SelectList(detail.InvoiceTypeMasters, "InvoiceTypeId", "InvoiceType", terminalViewModel.invoice.InvoiceTypeId);
                                ViewBag.PaymentTypeId = new SelectList(detail.PaymentTypeMasters, "PaymentTypeId", "PaymentType", terminalViewModel.invoice.PaymentTypeId);
                                ViewBag.StoreId = new SelectList(detail.StoreMasters, "StoreId", "NickName", terminalViewModel.invoice.StoreId);
                                ViewBag.VendorId = new SelectList(detail.VendorMasters, "VendorId", "VendorName", terminalViewModel.invoice.VendorId);
                                return View(terminalViewModel.invoice);
                            }
                            else
                            {
                                if (terminalViewModel.UploadInvoice.ContentLength > 50971520)
                                {
                                    ViewBag.StatusMessage = "InvalidPDFSize";
                                    return View(terminalViewModel.invoice);
                                }
                                else
                                {
                                    terminalViewModel.Sacn_Title = AdminSiteConfiguration.GetRandomNo() + Path.GetFileName(terminalViewModel.UploadInvoice.FileName);
                                    terminalViewModel.Sacn_Title = AdminSiteConfiguration.RemoveSpecialCharacter(terminalViewModel.Sacn_Title);
                                    terminalViewModel.fullName = terminalViewModel.invoice.InvoiceDate.ToString("MMddyyyy") + "-" + GetVendorName(terminalViewModel.invoice.VendorId) + "-" + AdminSiteConfiguration.RemoveSpecialCharacterInvoice(Inoicno) + "-" + (terminalViewModel.invoice.InvoiceTypeId == 1 ? "INV" : "CR") + ".pdf";
                                    terminalViewModel.PathRel = CreateDirectory(terminalViewModel.invoice.InvoiceDate.ToString("MMddyyyy"), terminalViewModel.invoice.VendorId, terminalViewModel.invoice.StoreId);
                                    terminalViewModel.filePath = terminalViewModel.PathRel + "\\" + terminalViewModel.fullName;
                                    terminalViewModel.UploadInvoice.SaveAs(terminalViewModel.filePath);
                                    terminalViewModel.invoice.UploadInvoice = terminalViewModel.filePath.Replace(Request.PhysicalApplicationPath + "UserFiles\\Invoices\\", "");
                                }
                            }
                        }
                    }
                    #endregion
                    terminalViewModel.name = User.Identity.Name;
                    if (terminalViewModel.invoice.QuickInvoice == "1" && (Roles.IsUserInRole("Administrator") || Roles.IsUserInRole("ApproveInvoice")))
                    {
                        if (terminalViewModel.Store != "")
                        {
                            //This class is create Invoice
                            terminalViewModel = await _terminalRepository.CreateInvoice(terminalViewModel);
                        }
                        else
                        {
                            //Upload File Exntantion Invalid
                            CreateInvoiceDetail detail = _terminalRepository.UploadFileExtantionInvalid(terminalViewModel);
                            terminalViewModel.invoice.DepartmentMasters = detail.DepartmentMasters;
                            ViewBag.Disc_Dept_id = new SelectList(detail.DepartmentMasters_store, "DepartmentId", "DepartmentName", terminalViewModel.invoice.Disc_Dept_id);
                            ViewBag.DiscountTypeId = new SelectList(detail.DiscountTypeMasters, "DiscountTypeId", "DiscountType", terminalViewModel.invoice.DiscountTypeId);
                            ViewBag.InvoiceTypeId = new SelectList(detail.InvoiceTypeMasters, "InvoiceTypeId", "InvoiceType", terminalViewModel.invoice.InvoiceTypeId);
                            ViewBag.PaymentTypeId = new SelectList(detail.PaymentTypeMasters, "PaymentTypeId", "PaymentType", terminalViewModel.invoice.PaymentTypeId);
                            ViewBag.StoreId = new SelectList(detail.StoreMasters, "StoreId", "NickName", terminalViewModel.invoice.StoreId);
                            ViewBag.VendorId = new SelectList(detail.VendorMasters, "VendorId", "VendorName", terminalViewModel.invoice.VendorId);
                            ViewBag.StatusMessage = "Error";
                            return Json("Notsuccess", JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                        //Create Invoice Without Quick Invoice
                        terminalViewModel = await _terminalRepository.CreateInvoiceWithoutQuickInvoice(terminalViewModel);
                    }

                    try
                    {
                        if (Convert.ToInt32(terminalViewModel.InvoiceId) > 0)
                        {
                            //Uisng this class is Updare Invoice
                            terminalViewModel = await _terminalRepository.UpdateInvoice(terminalViewModel);
                            if (terminalViewModel.StatusMessage == "InvalidImage")
                            {
                                return View("IndexBeta");
                            }
                            terminalViewModel.StatusMessage = "";
                            return Json("success", JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            return RedirectToAction("Index", "Login", new { area = "" });
                        }
                    }
                    catch (Exception ex) 
                    {
                        logger.Error("TerminalController - CreateCashInvoice - " + DateTime.Now + " - " + ex.Message.ToString());
                        return Json("Notsuccess", JsonRequestBehavior.AllowGet);
                    }

                }
                else
                {
                    CreateInvoiceDetail detail = _terminalRepository.UploadFileExtantionInvalid(terminalViewModel);
                    terminalViewModel.invoice.DepartmentMasters = detail.DepartmentMasters;
                    ViewBag.Disc_Dept_id = new SelectList(detail.DepartmentMasters_store, "DepartmentId", "DepartmentName", terminalViewModel.invoice.Disc_Dept_id);
                    ViewBag.DiscountTypeId = new SelectList(detail.DiscountTypeMasters, "DiscountTypeId", "DiscountType", terminalViewModel.invoice.DiscountTypeId);
                    ViewBag.InvoiceTypeId = new SelectList(detail.InvoiceTypeMasters, "InvoiceTypeId", "InvoiceType", terminalViewModel.invoice.InvoiceTypeId);
                    ViewBag.PaymentTypeId = new SelectList(detail.PaymentTypeMasters, "PaymentTypeId", "PaymentType", terminalViewModel.invoice.PaymentTypeId);
                    ViewBag.StoreId = new SelectList(detail.StoreMasters, "StoreId", "NickName", terminalViewModel.invoice.StoreId);
                    ViewBag.VendorId = new SelectList(detail.VendorMasters, "VendorId", "VendorName", terminalViewModel.invoice.VendorId);
                    return View("Create", terminalViewModel.invoice);
                }
            }
            catch (Exception ex)
            {
                logger.Error("TerminalController - CreateCashInvoice - " + DateTime.Now + " - " + ex.Message.ToString());
                return Json("Notsuccess", JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// This class is Craete Directory.
        /// </summary>
        /// <param name="Date"></param>
        /// <param name="VendorId"></param>
        /// <param name="StoreId"></param>
        /// <returns></returns>
        public string CreateDirectory(string Date, int VendorId, int StoreId)
        {
            var RootURL = Request.PhysicalApplicationPath + "UserFiles\\Invoices" + "\\";

            var RelationalURL = RootURL + Date.Substring(4, 4);
            try
            {
                if (!(Directory.Exists(RelationalURL)))
                {
                    Directory.CreateDirectory(RelationalURL);
                    RelationalURL = RelationalURL + "\\" + StoreId;
                    if (!(Directory.Exists(RelationalURL)))
                    {
                        Directory.CreateDirectory(RelationalURL);
                        RelationalURL = RelationalURL + "\\" + GetMonthName(Convert.ToInt32(Date.Substring(0, 2)));
                        if (!(Directory.Exists(RelationalURL)))
                        {
                            Directory.CreateDirectory(RelationalURL);
                            RelationalURL = RelationalURL + "\\" + VendorId;
                            if (!(Directory.Exists(RelationalURL)))
                            {
                                Directory.CreateDirectory(RelationalURL);
                            }
                        }
                    }
                }
                else
                {
                    RelationalURL = RelationalURL + "\\" + StoreId;
                    if (!(Directory.Exists(RelationalURL)))
                    {
                        Directory.CreateDirectory(RelationalURL);
                        RelationalURL = RelationalURL + "\\" + GetMonthName(Convert.ToInt32(Date.Substring(0, 2)));
                        if (!(Directory.Exists(RelationalURL)))
                        {
                            Directory.CreateDirectory(RelationalURL);
                            RelationalURL = RelationalURL + "\\" + VendorId;
                            if (!(Directory.Exists(RelationalURL)))
                            {
                                Directory.CreateDirectory(RelationalURL);
                            }
                        }
                    }
                    else
                    {
                        RelationalURL = RelationalURL + "\\" + GetMonthName(Convert.ToInt32(Date.Substring(0, 2)));
                        if (!(Directory.Exists(RelationalURL)))
                        {
                            Directory.CreateDirectory(RelationalURL);
                            RelationalURL = RelationalURL + "\\" + VendorId;
                            if (!(Directory.Exists(RelationalURL)))
                            {
                                Directory.CreateDirectory(RelationalURL);
                            }
                        }
                        else
                        {
                            RelationalURL = RelationalURL + "\\" + VendorId;
                            if (!(Directory.Exists(RelationalURL)))
                            {
                                Directory.CreateDirectory(RelationalURL);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("TerminalController - CreateDirectory - " + DateTime.Now + " - " + ex.Message.ToString());
            }
        
            return RelationalURL;
        }

        /// <summary>
        /// This class is GetMonth Name
        /// </summary>
        /// <param name="Month"></param>
        /// <returns></returns>
        public string GetMonthName(int Month)
        {
            string MonthName = "";
            switch (Month)
            {
                case 1:
                    MonthName = "January";
                    break;
                case 2:
                    MonthName = "February";
                    break;
                case 3:
                    MonthName = "March";
                    break;
                case 4:
                    MonthName = "April";
                    break;
                case 5:
                    MonthName = "May";
                    break;
                case 6:
                    MonthName = "June";
                    break;
                case 7:
                    MonthName = "July";
                    break;
                case 8:
                    MonthName = "August";
                    break;
                case 9:
                    MonthName = "September";
                    break;
                case 10:
                    MonthName = "October";
                    break;
                case 11:
                    MonthName = "November";
                    break;
                case 12:
                    MonthName = "December";
                    break;
                default:
                    break;
            }
            return MonthName;
        }
        /// <summary>
        ///  This class is Vendor Name
        /// </summary>
        /// <param name="VendorId"></param>
        /// <returns></returns>
        private string GetVendorName(int VendorId)
        {
            return _terminalRepository.GetVendorName(VendorId);
        }
        /// <summary>
        ///  This Method  is delete cash Invoice
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public JsonResult DeleteCashInvoice(int Id)
        {
            try
            {
                if (Id != 0)
                {
                    //THis class is delete Cash Invoice
                    MainTerminalView.ErrorMessage = _terminalRepository.DeleteCashInvoice(Id);
                }
            }
            catch (Exception ex)
            {
                logger.Error("TerminalController - DeleteCashInvoice - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return Json(MainTerminalView.ErrorMessage, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// This Method is check invoice used Any where 
        /// </summary>
        /// <param name="InvoiceID"></param>
        /// <returns></returns>
        public JsonResult CheckInvoiceUSedAnywhere(int InvoiceID)
        {
            var InvExist = false;
            try
            {
                //Using this class Get Invoice By Id
                var Inv = _terminalRepository.GetInvoiceByID(InvoiceID);
                if (Inv != null)
                {
                    if (Inv.StatusValue == InvoiceStatusEnm.Approved && Inv.TXNId != null)
                    {
                        //return Json("This Invoice already approved and synced with QB. <br/> Are you sure want to delete this Invoice? If Yes, you have to manual delete from QB.", JsonRequestBehavior.AllowGet);
                        return Json(_CommonRepository.GetMessageValue("INDQ", "This Invoice already approved and synced with QB. ##br## Are you sure want to delete this Invoice? If Yes, you have to manual delete from QB.").Replace("##br##", "<br/>"), JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        //return Json("Are you sure want to delete this Invoice?", JsonRequestBehavior.AllowGet);
                        return Json(_CommonRepository.GetMessageValue("INDN", "Are you sure want to delete this Invoice?"), JsonRequestBehavior.AllowGet);
                    }
                }
                return Json(InvExist, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error("TerminalController - CheckInvoiceUSedAnywhere - " + DateTime.Now + " - " + ex.Message.ToString());
                AdminSiteConfiguration.WriteErrorLogs("Controller : Invoice Method : GetroleForJSApproval Message:" + ex.Message + "Internal Message:" + ex.InnerException);
                return Json(InvExist, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        ///  This Method is check sales other deposite used Any where 
        /// </summary>
        /// <param name="OtherDepositeID"></param>
        /// <returns></returns>
        public JsonResult CheckSalesOtherDeposite_UsedAnywhere(int OtherDepositeID)
        {
            var InvExist = false;
            try
            {
                //Get sales Other Deposite by IDs
                var CPInvoice = _terminalRepository.GetSalesOtherDepositeByID(OtherDepositeID);
                if (CPInvoice != null)
                {
                    if (CPInvoice.Status == 3 && CPInvoice.TxnId != null)
                    {
                        //return Json("This record already sync in QB. Are you sure want to delete this record?", JsonRequestBehavior.AllowGet);
                        return Json(_CommonRepository.GetMessageValue("TCD", "This record already sync in QB. Are you sure want to delete this record?"), JsonRequestBehavior.AllowGet);
                    }

                    else
                    {
                        //return Json("Are you sure want to delete this record?", JsonRequestBehavior.AllowGet);
                        return Json(_CommonRepository.GetMessageValue("SRDTR", "Are you sure want to delete this record?"), JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    return Json(_CommonRepository.GetMessageValue("SRDTR", "Are you sure want to delete this record?"), JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                logger.Error("TerminalController - CheckSalesOtherDeposite_UsedAnywhere - " + DateTime.Now + " - " + ex.Message.ToString());
                AdminSiteConfiguration.WriteErrorLogs("Controller : Invoice Method : GetroleForJSApproval Message:" + ex.Message + "Internal Message:" + ex.InnerException);
                return Json(InvExist, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult CheckPreviousDayClosedOut(DateTime date_val)
        {
            terminalViewData = new TerminalViewModel();
            terminalViewData.SDate = date_val;
            DateTime? previousdate = null;
            try
            {
                if (!string.IsNullOrEmpty(Convert.ToString(terminalViewData.SDate)))
                {
                    terminalViewData.UserID = UserModule.getUserId();
                    if (!string.IsNullOrEmpty(Convert.ToString(Session["storeid"])))
                    {
                        terminalViewData.StoreId = Convert.ToInt32(Session["storeid"]);
                    }
                    previousdate = _terminalRepository.GetPreviousDayCount(terminalViewData);
                }
                if (previousdate == null)
                {
                    previousdate = new DateTime(1, 1, 1);
                }

            }
            catch (Exception ex)
            {
                logger.Error("TerminalController - CheckPreviousDayClosedOut - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return Json(previousdate, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CheckPaidOutAmount(DateTime date_val,string terminalIds)
        {
            terminalViewData = new TerminalViewModel();
            terminalViewData.SDate = date_val;
            terminalViewData.Terminalid_val = terminalIds;
            string result = "";
            try
            {
                if (!string.IsNullOrEmpty(Convert.ToString(terminalViewData.SDate)))
                {
                    terminalViewData.UserID = UserModule.getUserId();
                    if (!string.IsNullOrEmpty(Convert.ToString(Session["storeid"])))
                    {
                        terminalViewData.StoreId = Convert.ToInt32(Session["storeid"]);
                    }
                    result = _terminalRepository.GetPaidOutAmountMessage(terminalViewData);
                }
            }
            catch (Exception ex)
            {
                logger.Error("TerminalController - CheckPreviousDayClosedOut - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CheckCCOfflineAmount(DateTime date_val, string terminalIds)
        {
            terminalViewData = new TerminalViewModel();
            terminalViewData.SDate = date_val;
            terminalViewData.Terminalid_val = terminalIds;
            string result = "";
            try
            {
                if (!string.IsNullOrEmpty(Convert.ToString(terminalViewData.SDate)))
                {
                    terminalViewData.UserID = UserModule.getUserId();
                    if (!string.IsNullOrEmpty(Convert.ToString(Session["storeid"])))
                    {
                        terminalViewData.StoreId = Convert.ToInt32(Session["storeid"]);
                    }
                    result = _terminalRepository.GetCCOfflineAmountMessage(terminalViewData);
                }
            }
            catch (Exception ex)
            {
                logger.Error("TerminalController - CheckCCOfflineAmount - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CheckLastThirtydaysClosedOut(DateTime date_val)
        {
            terminalViewData = new TerminalViewModel();
            terminalViewData.SDate = date_val;
            string thirtydaysdates = "";
            try
            {
                if (!string.IsNullOrEmpty(Convert.ToString(terminalViewData.SDate)))
                {
                    terminalViewData.UserID = UserModule.getUserId();
                    if (!string.IsNullOrEmpty(Convert.ToString(Session["storeid"])))
                    {
                        terminalViewData.StoreId = Convert.ToInt32(Session["storeid"]);
                    }
                    thirtydaysdates = _terminalRepository.GetLastThirtydaysClosedOut(terminalViewData);
                }

            }
            catch (Exception ex)
            {
                logger.Error("TerminalController - CheckLastThirtydaysClosedOut - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return Json(thirtydaysdates, JsonRequestBehavior.AllowGet);
        }
    }
}