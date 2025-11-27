using EntityModels.Models;
using NLog;
using Repository;
using Repository.IRepository;
using SynthesisViewModal;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SysnthesisRepo.Controllers
{
    public class ConfigurationController : Controller
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private readonly IConfigurationRepository _ConfigurationRepository;
        private readonly ICommonRepository _CommonRepository;
        // GET: Configuration
        protected static string StatusMessage = "";
        public ConfigurationController()
        {
            this._ConfigurationRepository = new ConfigurationRepository(new DBContext());
        }


        /// <summary>
        /// This Index method get all Configuration List.
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "Administrator,TenderAccountSettings")]
        // GET: Admin/Configuration
        public ActionResult Index()
        {
            ViewBag.Title = "Tender Account Master - Synthesis";

            Configuration configuration = new Configuration();
            try
            {
                int storeid = 0;
                if (Convert.ToString(Session["storeid"]) != "0")
                {
                    storeid = Convert.ToInt32(Session["storeid"]);
                    //Db class use for get Configration list.
                    configuration.Configrationlist = _ConfigurationRepository.getConfigrationlist(storeid);

                    ViewBag.StatusMessage = StatusMessage;
                    StatusMessage = "";
                }
                else
                {
                    ViewBag.StatusMessage = "NoStoreSelected";
                }
            }
            catch (Exception ex)
            {
                logger.Error("ConfigurationController - Index - " + DateTime.Now + " - " + ex.Message.ToString());
            }

            return View(configuration);
        }

        /// <summary>
        /// This method set the Department Configuration. 
        /// </summary>
        /// <param name="Deptid"></param>
        /// <param name="groupid"></param>
        /// <param name="Storeid"></param>
        /// <param name="typicalbalid"></param>
        /// <param name="memoidval"></param>
        /// <param name="tendername"></param>
        /// <param name="ggID"></param>
        /// <param name="Flag"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(int[] Deptid = null, int[] groupid = null, int[] Storeid = null, int[] typicalbalid = null, string[] memoidval = null, string[] tendername = null, int[] ggID = null, int[] Flag = null)
        {
            try
            {
                int IID = 0;
                string TenderName = "";
                int StoreID = 0;
                //var success = "";
                for (int i = 0; i < groupid.Count(); i++)
                {
                    IID = ggID[i];
                    TenderName = tendername[i];
                    StoreID = Storeid[i];
                    // db class use for Save department configurations
                    _ConfigurationRepository.SaveDepartmentconfigurations(IID, Deptid[i], Flag[i], tendername[i], Storeid[i], memoidval[i], typicalbalid[i]);
                    StatusMessage = "Success";
                }
            }
            catch (Exception ex)
            {
                logger.Error("ConfigurationController - Index - " + DateTime.Now + " - " + ex.Message.ToString());
            }

            return RedirectToAction("Index");
        }

        /// <summary>
        /// This method Bind Group data with ID.
        /// </summary>
        /// <param name="groupid"></param>
        /// <returns></returns>
        public JsonResult BindGroupData(string groupid = null)
        {
            try
            {
                if (groupid != "" && groupid != null)
                {
                    int groupval = Convert.ToInt32(groupid);
                    //This db class use for Get group data by group id.
                    var GroupData = _ConfigurationRepository.getGroupDatabygroupid(groupval);
                    return Json(new { DeptId = GroupData[0].DeptId, typicalBalId = GroupData[0].typicalBalId, Deptname = GroupData[0].Deptname, typicalbalance = GroupData[0].typicalbalance, memo = GroupData[0].memo }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { DeptId = 0, typicalBalId = 0, Deptname = "", typicalbalance = "", memo = "" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                logger.Error("CheckListExpenseController - BindGroupData - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return Json(new { DeptId = 0, typicalBalId = 0, Deptname = "", typicalbalance = "", memo = "" }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult ResetConfiguration(string tenderName, int storeId)
        {
            try
            {
                bool result = _ConfigurationRepository.ResetConfiguration(tenderName, storeId);
                return Json(new { success = result });
            }
            catch (Exception ex)
            {
                logger.Error("ConfigurationController - ResetConfiguration - " + DateTime.Now + " - " + ex.Message.ToString());
                return Json(new { success = false });
            }
        }
    }
}