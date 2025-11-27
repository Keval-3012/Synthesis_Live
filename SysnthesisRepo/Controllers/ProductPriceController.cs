using EntityModels.Models;
using Newtonsoft.Json;
using NLog;
using Repository;
using Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace SysnthesisRepo.Controllers
{
    public class ProductPriceController : Controller
    {
        private readonly IProductPriceRepository _ProductPriceRepository;
        private readonly ISynthesisApiRepository _SynthesisApiRepository;
        private readonly ICommonRepository _CommonRepository;
        Logger logger = LogManager.GetCurrentClassLogger();
        public ProductPriceController()
        {
            this._ProductPriceRepository = new ProductPriceRepository(new DBContext());
            this._SynthesisApiRepository = new SynthesisApiRepository();
            this._CommonRepository = new CommonRepository(new DBContext());
        }
        public DataTable dt = null;
        
        // GET: ProductPrice
        /// <summary>
        /// This method is get sales Price list
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            try
            {
                _CommonRepository.LogEntries();     //Harsh's code
                //Get sale Price List
                var list = _ProductPriceRepository.GetsalePriceList();
                ViewBag.Vendor = new SelectList(list, "VendorName", "VendorName");
                ViewBag.txttodate = DateTime.Now.ToString("MM/dd/yyyy");
                var date = DateTime.Now.AddDays(-15);
                ViewBag.txtfromdate = DateTime.Now.AddDays(-15).ToString("MM/dd/yyyy");
            }
            catch (Exception ex)
            {
                logger.Error("ProductPriceController - Index - " + DateTime.Now + " - " + ex.Message.ToString());
            }
          
            return View();
        }

        /// <summary>
        /// This method is get Product Price list
        /// </summary>
        /// <param name="Upccode"></param>
        /// <param name="FromDate"></param>
        /// <param name="ToDate"></param>
        /// <returns></returns>
        public ActionResult getProductPriceList(string Upccode, string FromDate, string ToDate)
        {
           
            try
            {
                DataTable dt = new DataTable();
                //Get All Product Price List
                dt = _ProductPriceRepository.getProductPriceList(Upccode, FromDate, ToDate);
                string JSONresult;
                JSONresult = JsonConvert.SerializeObject(dt);

                return Json(JSONresult, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error("ProductPriceController - getProductPriceList - " + DateTime.Now + " - " + ex.Message.ToString());
                return Json("", JsonRequestBehavior.AllowGet);
            }
            
        }

        /// <summary>
        /// This method is get Line chart
        /// </summary>
        /// <param name="Upccode"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="storeId"></param>
        /// <returns></returns>
        public ActionResult getlinechart(string Upccode, string startDate, string endDate, string storeId)
        {
            try
            {
                //get Line chart
                var linechartdate = _ProductPriceRepository.getlinechart(Upccode, startDate, endDate, storeId);

                return Json(linechartdate, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error("ProductPriceController - getlinechart - " + DateTime.Now + " - " + ex.Message.ToString());
                return Json("Error", JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// This method is get Vendor Pie chart
        /// </summary>
        /// <param name="Upccode"></param>
        /// <returns></returns>
        public ActionResult VendorgetPiechart(string Upccode)
        {
            try
            {
                //Db class is get Vendor Pie chart
                var linechartdate = _ProductPriceRepository.VendorgetPiechart(Upccode);

                return Json(linechartdate, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error("ProductPriceController - VendorgetPiechart - " + DateTime.Now + " - " + ex.Message.ToString());
                return Json("Notget", JsonRequestBehavior.AllowGet); ;
            }
        }

        /// <summary>
        /// This method is get Store pie chart
        /// </summary>
        /// <param name="Upccode"></param>
        /// <returns></returns>
        public ActionResult StoregetPiechart(string Upccode)
        {
            try
            {
                //This class is get store pie chart
                var linechartdate = _ProductPriceRepository.StoregetPiechart(Upccode);

                return Json(linechartdate, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error("ProductPriceController - StoregetPiechart - " + DateTime.Now + " - " + ex.Message.ToString());
                return Json("Notget", JsonRequestBehavior.AllowGet); ;
            }
        }

        public ActionResult testing()
        {
            return View();
        }

        /// <summary>
        /// This method is PartialView of Line Items Lookup
        /// </summary>
        /// <param name="Upccode"></param>
        /// <param name="StoreIds"></param>
        /// <param name="InvoiceNumber"></param>
        /// <returns></returns>
        public ActionResult LineItemsLookup(string Upccode, int StoreIds, string InvoiceNumber)
        {
            try
            {
                List<LineItemsLookUp> list = new List<LineItemsLookUp>();
                //Get list of Items Look Up
                list = _ProductPriceRepository.LineItemsLookup(Upccode, StoreIds, InvoiceNumber);
                return PartialView("VendorLookup", list);

            }
            catch (Exception ex)
            {
                logger.Error("ProductPriceController - LineItemsLookup - " + DateTime.Now + " - " + ex.Message.ToString());
                return PartialView("VendorLookup");
            }
        }

        /// <summary>
        /// This method is PartialView of Invoice Lookup
        /// </summary>
        /// <param name="Upccode"></param>
        /// <returns></returns>
        public ActionResult InvoiceLookup(string Upccode)
        {
            try
            {
                //Get List of Invoice Lookup
                List<InvoiceLookup> list = _ProductPriceRepository.InvoiceLookup(Upccode);
                return PartialView("InvoiceLookup", list);

            }
            catch (Exception ex)
            {
                logger.Error("ProductPriceController - InvoiceLookup - " + DateTime.Now + " - " + ex.Message.ToString());
                return PartialView("InvoiceLookup");
            }
        }

        /// <summary>
        /// This method is PartialView of Store Wise Lookup
        /// </summary>
        /// <param name="Upccode"></param>
        /// <returns></returns>
        public ActionResult StoreWiseLookup(string Upccode)
        {
            try
            {
                //Get list of Store wise lookup
                List<StoreWiseLookup> list = _ProductPriceRepository.StoreWiseLookup(Upccode);
                return PartialView("StoreWiseLookup", list);
            }
            catch (Exception ex)
            {
                logger.Error("ProductPriceController - StoreWiseLookup - " + DateTime.Now + " - " + ex.Message.ToString());
                return PartialView("StoreWiseLookup");
            }
        }


        /// <summary>
        /// This method is get Product price view
        /// </summary>
        /// <returns></returns>
        public ActionResult ProductPrice()
        {
            ViewBag.Title = "Price Comparison - Synthesis";
            _CommonRepository.LogEntries();     //Harsh's code
            //this db class is Get All Vendor
            List<VendorNameList> list = _ProductPriceRepository.ViewBagVendor();
            ViewBag.Vendor = list;
            //This class is Get all Department
            ViewBag.Department = _ProductPriceRepository.ViewBagDepartment();
            ViewBag.EndDate = DateTime.Now;
            ViewBag.StartDate = DateTime.Now.AddDays(-15);
            return View();
        }
        /// <summary>
        /// This class is Convert data table 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dt"></param>
        /// <returns></returns>
        private static List<T> ConvertDataTable<T>(DataTable dt)
        {
            List<T> data = new List<T>();
            foreach (DataRow row in dt.Rows)
            {
                T item = GetItem<T>(row);
                data.Add(item);
            }
            return data;
        }
        /// <summary>
        /// This class is Get Item
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dr"></param>
        /// <returns></returns>
        private static T GetItem<T>(DataRow dr)
        {
            Type temp = typeof(T);
            T obj = Activator.CreateInstance<T>();

            foreach (DataColumn column in dr.Table.Columns)
            {
                foreach (PropertyInfo pro in temp.GetProperties())
                {
                    if (pro.Name == column.ColumnName)
                        pro.SetValue(obj, dr[column.ColumnName], null);
                    else
                        continue;
                }
            }
            return obj;
        }
        /// <summary>
        /// This method is Get Product price Grid 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="Upccode"></param>
        /// <param name="vendorname"></param>
        /// <param name="ColorFilter"></param>
        /// <param name="Radio"></param>
        /// <returns></returns>
        public ActionResult ProductPriceGrid(string[] value, string Upccode, string vendorname, string ColorFilter, int Radio)
        {
            try
            {
                DataTable dt = new DataTable();
                //Get Product price Grid 
                dt = _ProductPriceRepository.ProductPriceGrid(value, Upccode, vendorname, ColorFilter, Radio);
                var jsonResult = Json(JsonConvert.SerializeObject(dt), JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;
            }
            catch (Exception ex)
            {
                logger.Error("ProductPriceController - ProductPriceGrid - " + DateTime.Now + " - " + ex.Message.ToString());
                return Json("", JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// This method is get Unlink Item Lines
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public ActionResult Unlinkitem(LineItemList value)
        {
            string message = "";
            try
            {
                //This class is get Unlink Item Lines
                _ProductPriceRepository.Unlink_ItemLines(value);
                message = "Success";
            }
            catch (Exception Ex)
            {
                logger.Error("LineItemController - Unlinkitem - " + DateTime.Now + " - " + Ex.Message.ToString());
            }
            return Json(message);
        }
    }
}