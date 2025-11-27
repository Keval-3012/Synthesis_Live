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
    public class TopsellerPriceController : Controller
    {
        private readonly ITopsellerPriceRepository _TopsellerPriceRepository;
        private readonly ISynthesisApiRepository _SynthesisApiRepository;
        private readonly ICommonRepository _CommonRepository;
        Logger logger = LogManager.GetCurrentClassLogger();
        public TopsellerPriceController()
        {
            this._TopsellerPriceRepository = new TopsellerPriceRepository(new DBContext());
            this._SynthesisApiRepository = new SynthesisApiRepository();
            this._CommonRepository = new CommonRepository(new DBContext());
        }

        // GET: TopsellerPrice
        public ActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// This method return Top Seller Price view.
        /// </summary>
        /// <returns></returns>
        public ActionResult TopSellerPrice()
        {
            ViewBag.Title = "Top Seller Price Comparison - Synthesis";
            _CommonRepository.LogEntries();     //Harsh's code
            try
            {
                //Db class is get All vendor
                List<VendorNameList> list = _TopsellerPriceRepository.ViewBagVendor();
                ViewBag.Vendor = list;
                //Get All Department.
                ViewBag.Department = _TopsellerPriceRepository.ViewBagDepartment();
                ViewBag.EndDate = DateTime.Now;
                ViewBag.StartDate = DateTime.Now.AddDays(-15);
            }
            catch (Exception ex)
            {
                logger.Error("TopsellerPriceController - TopSellerPrice - " + DateTime.Now + " - " + ex.Message.ToString());
            }
          
            return View();
        }

        /// <summary>
        /// This method is get Top seller Price grid.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="Upccode"></param>
        /// <param name="Departmentname"></param>
        /// <param name="ColorFilter"></param>
        /// <returns></returns>
        public ActionResult TopSellerPriceGrid(string[] value, string Upccode, string Departmentname, string ColorFilter)
        {
            try
            {
                DataTable dt = new DataTable();
                //Get Top seller Price grid.
                dt = _TopsellerPriceRepository.TopSellerPriceGrid(value, Upccode, Departmentname, ColorFilter);
                var jsonResult = Json(JsonConvert.SerializeObject(dt), JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;
            }
            catch (Exception ex)
            {
                logger.Error("TopsellerPriceController - TopSellerPriceGrid - " + DateTime.Now + " - " + ex.Message.ToString());
                return Json("", JsonRequestBehavior.AllowGet);
            }
           
        }

        
    }
}