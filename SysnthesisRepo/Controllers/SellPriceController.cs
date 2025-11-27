using EntityModels.Models;
using Newtonsoft.Json;
using NLog;
using Repository;
using Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SysnthesisRepo.Controllers
{
    public class SellPriceController : Controller
    {
        private readonly ISellPriceRepository _SellPriceRepository;
        private readonly ISynthesisApiRepository _SynthesisApiRepository;
        private readonly ICommonRepository _CommonRepository;
        Logger logger = LogManager.GetCurrentClassLogger();
        public SellPriceController()
        {
            this._SellPriceRepository = new SellPriceRepository(new DBContext());
            this._SynthesisApiRepository = new SynthesisApiRepository();
            this._CommonRepository = new CommonRepository(new DBContext());
        }

        // GET: SellPrice
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// This method is return Sell price view.
        /// </summary>
        /// <returns></returns>
        public ActionResult SellPrice()
        {
            ViewBag.Title = "Sales Price Comparison - Synthesis";
            _CommonRepository.LogEntries();     //Harsh's code
            try
            {
                //Get Sales Price list.
                List<VendorNameList> list = _SellPriceRepository.GetsalePriceList();
                ViewBag.Vendor = list;
                //Get All Department list.
                ViewBag.Department = _SellPriceRepository.ViewbagDepartment();
                ViewBag.EndDate = DateTime.Now;
                ViewBag.StartDate = DateTime.Now.AddDays(-15);
            }
            catch (Exception ex)
            {
                logger.Error("SellPriceController - SellPrice - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            
            return View();
        }

        /// <summary>
        /// This method return sell price grid voew.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="Upccode"></param>
        /// <param name="Departmentname"></param>
        /// <param name="ColorFilter"></param>
        /// <returns></returns>
        public ActionResult SellPriceGrid(string[] value, string Upccode, string Departmentname, string ColorFilter)
        {
            try
            {
                DataTable dt = new DataTable();
                //Get Sell Price Grid.
                dt = _SellPriceRepository.SellPriceGrid(value, Upccode, Departmentname, ColorFilter);
                var jsonResult = Json(JsonConvert.SerializeObject(dt), JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;
            }
            catch (Exception ex)
            {
                logger.Error("SellPriceController - SellPrice - " + DateTime.Now + " - " + ex.Message.ToString());
                return Json("", JsonRequestBehavior.AllowGet);
            }
            
        }
    }
}