using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
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
    public class BankAccountsController : Controller
    {
        private readonly IBankAccountsRepository _BankAccountSettingRepository;
        private readonly IUserActivityLogRepository _ActivityLogRepository;
        private readonly ICommonRepository _CommonRepository;
        Logger logger = LogManager.GetCurrentClassLogger();
        public BankAccountsController()
        {
            this._BankAccountSettingRepository = new BankAccountsRepository(new DBContext());
            this._ActivityLogRepository = new UserActivityLogRepository(new DBContext());
            this._CommonRepository = new CommonRepository(new DBContext());
        }

        // GET: BankAccountSetting
        /// <summary>
        /// This the default view page
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            ViewBag.Title = "Bank Accounts - Synthesis";
            ViewBag.delete = _CommonRepository.GetMessageValue("BASD", "Bank Account Setting deleted successfully.");
            ViewBag.ItemRequred = _CommonRepository.GetMessageValue("BAIR", "Item ID Is Required");
            ViewBag.AccessRequired = _CommonRepository.GetMessageValue("BAATR", "Access Token Is Required");
            int StoreID = 0;
            if (Session["storeid"] != null)
            {
                StoreID = Convert.ToInt32(Convert.ToString(Session["storeid"]));
            }

            ViewBag.StoreIDValue = StoreID;
            List<BankAccountSettingDetail> banks = new List<BankAccountSettingDetail>();
            try
            {
                banks = _BankAccountSettingRepository.GetBankDetail(StoreID);
            }
            catch (Exception ex)
            {
                logger.Error("BankAccountSettingController - Index - "  + DateTime.Now + " - "  + ex.Message.ToString());
            }
            return View(banks);
        }

        /// <summary>
        /// This method return partial view of Bank account
        /// </summary>
        /// <returns></returns>
        public ActionResult BankAccount()
        {
            return PartialView("_CreateBankAccount");
        }

        /// <summary>
        /// This method add Bank Account details
        /// </summary>
        /// <param name="ItemID"></param>
        /// <param name="AccessToken"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AddBankAccountDetail(string ItemID, string AccessToken)
        {
            try
            {
                int StoreID = 0;
                if (Session["storeid"] != null)
                {
                    StoreID = Convert.ToInt32(Convert.ToString(Session["storeid"]));
                }
                if (StoreID != 0)
                {
                    BankAccountSettingModel bank = new BankAccountSettingModel();
                    bank.ItemID = ItemID.Trim();
                    bank.AccessToken = AccessToken.Trim();
                    bank.StoreID = StoreID;
                    var UserName = System.Web.HttpContext.Current.User.Identity.Name;
                    bank.CreatedBy = _CommonRepository.getUserId(UserName);
                    bank.CreatedOn = DateTime.Now;
                    //Save Bank Details
                    _BankAccountSettingRepository.SaveBankDetails(bank);


                    ActivityLog ActLog = new ActivityLog();
                    ActLog.Action = 1;
                    //This Db class is used for get user firstname.
                    ActLog.Comment = "Bank Account Setting Created by " + _CommonRepository.getUserFirstName(UserName) + " on " + AdminSiteConfiguration.GetDate(AdminSiteConfiguration.GetEasternTime(DateTime.Now).ToString());
                    //This  class is used for Activity Log Insert
                    _ActivityLogRepository.ActivityLogInsert(ActLog);
                    return Json("Success", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json("ErrorStore", JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                logger.Error("BankAccountSettingController - AddBankAccountDetail - "  + DateTime.Now + " - "  + ex.Message.ToString());
                return Json("Error", JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// This method is delete Bank Account
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        ///
        public async Task<ActionResult> Delete(int? id)
        {
            var DeleteMessage = "";
            try
            {
                //Get bank details by Id
                BankAccountSettingModel bank = _BankAccountSettingRepository.GetBankDetailsByID(id);
                //Get Store Masters data
                var store = _BankAccountSettingRepository.GetStoreMastersdata(bank);
                //Delete bank details
                _BankAccountSettingRepository.DeleteBankDetails(bank);

                var UserName = System.Web.HttpContext.Current.User.Identity.Name;
                ActivityLog ActLog = new ActivityLog();
                ActLog.Action = 3;
                //This Db class is used for get user firstname.
                ActLog.Comment = " Bank Account Setting " + store.NickName + " Deleted by " + _CommonRepository.getUserFirstName(UserName) + " on " + AdminSiteConfiguration.GetDate(AdminSiteConfiguration.GetEasternTime(DateTime.Now).ToString());
                //This  class is used for Activity Log Insert
                _ActivityLogRepository.ActivityLogInsert(ActLog);

                //DeleteMessage = "Bank Account Setting deleted successfully.";
                DeleteMessage = _CommonRepository.GetMessageValue("BASD", "Bank Account Setting deleted successfully.");
                ViewBag.Delete = DeleteMessage;
                return Json(DeleteMessage, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error("BankAccountSettingController:Delete :" + ex.Message.ToString());
                //DeleteMessage = "Bank Account Setting Already Used in Error Log";
                DeleteMessage = _CommonRepository.GetMessageValue("BAAD", "Bank Account Setting Already Used in Error Log");
                ViewBag.Delete = DeleteMessage;
                return Json(DeleteMessage, JsonRequestBehavior.AllowGet);
            }
        }
    }
}