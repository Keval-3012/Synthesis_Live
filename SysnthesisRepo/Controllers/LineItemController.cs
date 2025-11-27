using EntityModels.Models;
using NLog;
using Repository;
using Repository.IRepository;
using Syncfusion.EJ2.Base;
using SynthesisViewModal;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using Utility;

namespace SysnthesisRepo.Controllers
{
    public class LineItemController : Controller
    {
        private readonly ILineItemRepository _lineitemRepository;
        private readonly IUserActivityLogRepository _ActivityLogRepository;
        private readonly ICommonRepository _CommonRepository;
        private readonly ISynthesisApiRepository _SynthesisApiRepository;
        Logger logger = LogManager.GetCurrentClassLogger();
        public LineItemController()
        {
            this._lineitemRepository = new LineItemRepository(new DBContext());
            this._ActivityLogRepository = new UserActivityLogRepository(new DBContext());
            this._CommonRepository = new CommonRepository(new DBContext());
            this._SynthesisApiRepository = new SynthesisApiRepository();
        }

        /// <summary>
        /// This method is return Index view.
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            ViewBag.Title = "Line Items - Synthesis";
            _CommonRepository.LogEntries();     //Harsh's code
            int? StoreIds = 0;
            try
            {
                if (Session["storeid"] != null)
                {
                    StoreIds = Convert.ToInt32(Convert.ToString(Session["storeid"]));
                }

                if (StoreIds != 0)
                {
                    //Db class is Get vendor List By storeid
                    ViewBag.VendorList = _lineitemRepository.VendorListByStoreId(StoreIds);
                    ViewBag.VendorList.Insert(0, new { StoreId = 0, VendorName = "All Vendors" });
                }
                else
                {
                    //This db class is Get Vendor List
                    List<VendorNameList> VendorList = _lineitemRepository.VendorList();
                    VendorList.Insert(0, new VendorNameList { StoreId = 0, VendorName = "All Vendors" });
                    ViewBag.VendorList = VendorList;
                }
            }
            catch (Exception ex)
            {
                logger.Error("LineItemController - Index - " + DateTime.Now + " - " + ex.Message.ToString());
            }         
            return View();
        }

        /// <summary>
        /// this method is Url data Source valu for Get Line Item Invoice Details
        /// </summary>
        /// <param name="dm"></param>
        /// <returns></returns>
        public ActionResult UrlDatasourceValue(DataManagerRequest dm)
        {
            int? StoreIds = 0;
            if (Session["storeid"] != null)
            {
                StoreIds = Convert.ToInt32(Convert.ToString(Session["storeid"]));
            }
            string vendorName = "";
            if (Session["vendorname"] != null)
            {
                vendorName = Session["vendorname"].ToString();
            }
            DateTime? Date = null;
            DateTime? easternTime = null;
            if (Session["Date"] != null)
            {
                Date = Convert.ToDateTime(Session["Date"].ToString());
                var dates = TimeZoneInfo.ConvertTimeToUtc((DateTime)Date, TimeZoneInfo.Local);
                TimeZoneInfo easternZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
                easternTime = TimeZoneInfo.ConvertTimeFromUtc(dates, easternZone);
                Common.WriteErrorLogs(easternTime.ToString());
            }
            string checkvalue = "";
            if (Session["checkvalue"] != null)
            {
                checkvalue = Session["checkvalue"].ToString();
            }
            ViewBag.StoreId = StoreIds;
            List<LineItemInvoiceNew> list = new List<LineItemInvoiceNew>();
            //Get Line Item Invoice Details
            list = _lineitemRepository.GetLineItemInvoiceDetails(StoreIds, vendorName, easternTime, checkvalue);
            IEnumerable DataSource = list;
            int count = 0;
            DataOperations operation = new DataOperations();
            try
            {
                if (dm.Search != null && dm.Search.Count > 0)
                {
                    DataSource = operation.PerformSearching(DataSource, dm.Search);  //Search
                }
                if (dm.Sorted != null && dm.Sorted.Count > 0) //Sorting
                {
                    DataSource = operation.PerformSorting(DataSource, dm.Sorted);
                }
                if (dm.Where != null && dm.Where.Count > 0) //Filtering
                {
                    DataSource = operation.PerformFiltering(DataSource, dm.Where, dm.Where[0].Operator);
                }
                count = DataSource.Cast<LineItemInvoiceNew>().Count();
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
                logger.Error("LineItemController - UrlDatasourceValue - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return dm.RequiresCounts ? Json(new { result = DataSource, count = count }, "application/json", Encoding.UTF8, JsonRequestBehavior.AllowGet) : Json(DataSource, "application/json", Encoding.UTF8, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// This method is get Url Data Source for Child List
        /// </summary>
        /// <param name="dm"></param>
        /// <returns></returns>
        public ActionResult UrlDatasourceChildList(DataManagerRequest dm)
        {
            int? StoreIds = 0;
            if (Session["storeid"] != null)
            {
                StoreIds = Convert.ToInt32(Convert.ToString(Session["storeid"]));
            }
            string vendorId = "";
            if (Session["vendorname"] != null)
            {
                vendorId = Session["vendorname"].ToString();
            }
            DateTime? Date = null;
            if (Session["Date"] != null)
            {
                Date = Convert.ToDateTime(Session["Date"].ToString());
            }
            ViewBag.StoreId = StoreIds;
            List<LineItemList> list = new List<LineItemList>();
            if (dm.Where != null && dm.Where.Count > 0) //Filtering
            {
                //Get Line Item Invoice Item Details
                list = _lineitemRepository.GetLineItemInvoiceitemDetail(dm.Where[0].value);
                
            }
            IEnumerable DataSource = list;
            DataOperations operation = new DataOperations();
            if (dm.Search != null && dm.Search.Count > 0)
            {
                DataSource = operation.PerformSearching(DataSource, dm.Search);  //Search
            }
            if (dm.Sorted != null && dm.Sorted.Count > 0) //Sorting
            {
                DataSource = operation.PerformSorting(DataSource, dm.Sorted);
            }

            int count = DataSource.Cast<LineItemList>().Count();
            if (dm.Skip != 0)
            {
                DataSource = operation.PerformSkip(DataSource, dm.Skip);   //Paging
            }
            if (dm.Take != 0)
            {
                DataSource = operation.PerformTake(DataSource, dm.Take);
            }
            try
            {
                foreach (LineItemList item in DataSource)
                {
                    if (item.UPCCode != null)
                        //This class is Update UPC
                        _lineitemRepository.Update_UPC(item.UPCCode);
                }
            }
            catch (Exception ex)
            {
                logger.Error("LineItemController - UrlDatasourceChildList - " + DateTime.Now + " - " + ex.Message.ToString());
            }       
            return dm.RequiresCounts ? Json(new { result = DataSource, count = count }, "application/json", Encoding.UTF8, JsonRequestBehavior.AllowGet) : Json(DataSource, "application/json", Encoding.UTF8, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// This method is Get List of venor name,date and check value
        /// </summary>
        /// <param name="vendorname"></param>
        /// <param name="Date"></param>
        /// <param name="checkvalue"></param>
        /// <returns></returns>
        public ActionResult GetList(string vendorname, string Date, string checkvalue)
        {
            Session["vendorname"] = null;
            if (!String.IsNullOrEmpty(vendorname))
            {
                    Session["vendorname"] = vendorname; 
            }
            Session["Date"] = null;
            if (!String.IsNullOrEmpty(Date))
            {
                Session["Date"] = Date;
            }
            Session["checkvalue"] = null;
            if (!String.IsNullOrEmpty(checkvalue))
            {
                Session["checkvalue"] = checkvalue;
            }
            ViewBag.Reread = _CommonRepository.GetMessageValue("LIRRLI", "Are you sure you want to re-read line items ?");
            return PartialView("Lineitem");
        }

        /// <summary>
        /// This method is Get Invoice Product data
        /// </summary>
        /// <param name="value"></param>
        /// <param name="VendorName"></param>
        /// <returns></returns>
        public ActionResult GetInvoiceProductData(LineItemList value, string VendorName = "")
        {
            int Id = Convert.ToInt32(value.InvoiceProductId);
            //Get Invoice Product by id
            var InvoiceProduct = _lineitemRepository.GetInvoiceProductById(Convert.ToInt32(Id));
            List<ProductsList> lstProducts = new List<ProductsList>();
            //This class is Get Recommended Product List
            lstProducts = _lineitemRepository.GetRecommendedProductList(InvoiceProduct, Id); //.Replace("&amp;","&").Replace("&apos;","'")

            List<productVendorAllList> lstProductVendor = new List<productVendorAllList>();
            //This class  is Get Recommended Product Vendor List
            lstProductVendor = _lineitemRepository.GetRecommendedProductVendorList(InvoiceProduct, Id, VendorName); //.Replace("&amp;","&").Replace("&apos;","'")
            ViewBag.lstProductVendor = lstProductVendor;
            // This class is Get Popup Header data
            ProductStoreAndVendorName data = _lineitemRepository.GetPopupHeaderData(value.InvoiceId);
            ViewBag.store = data.NickName;
            ViewBag.vendorname = data.VendorName;
            //ViewBag.LineItemList = value;
            ViewBag.itemno = value.ItemNo;

            ViewBag.Description = value.Description;
            ViewBag.InvoiceProductId = value.InvoiceProductId;
            ViewBag.InvoiceId = value.InvoiceId;
            ViewBag.upccode = value.UPCCode;
            return PartialView("LinkProductList", lstProducts);
        }

        /// <summary>
        /// This method is Get Invoice Product Data with descending order
        /// </summary>
        /// <param name="InvoiceProductId"></param>
        /// <param name="Description"></param>
        /// <param name="UPCCode"></param>
        /// <param name="ItemNo"></param>
        /// <param name="search"></param>
        /// <param name="InvoiceId"></param>
        /// <param name="search2"></param>
        /// <param name="VendorName"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetInvoiceProductDatawithDesc(int InvoiceProductId = 0, string Description = "", string UPCCode = "", string ItemNo = "", string search = "", int InvoiceId = 0, string search2 = "", string VendorName = "")
        {
            int Id = Convert.ToInt32(InvoiceProductId);
            //This db class is get Invoice Product By Id
            var InvoiceProduct = _lineitemRepository.GetInvoiceProductById(Convert.ToInt32(Id));
            List<ProductsList> lstProducts = new List<ProductsList>();
            try
            {
                if (search != null)
                {
                    search = search.Trim();
                }
                if (search2 != null)
                {
                    search2 = search2.Trim();
                }
                if (string.IsNullOrEmpty(search))
                {
                    // This class is Get Recommended Product List with vendor name
                    lstProducts = _lineitemRepository.GetRecommendedProductListWithVendorName(InvoiceProduct, Id, VendorName); //.Replace("&amp;","&").Replace("&apos;","'")
                }
                else
                {
                    //This class is Select By Filter product
                    lstProducts = _lineitemRepository.SelectByFilterProduct(search, Id); //.Replace("&amp;","&").Replace("&apos;","'")
                }
                List<productVendorAllList> lstProductVendor = new List<productVendorAllList>();
                if (string.IsNullOrEmpty(search2))
                {
                    // Get Recommended Product Vendor List
                    lstProductVendor = _lineitemRepository.GetRecommendedProductVendorList(InvoiceProduct, Id, VendorName); //.Replace("&amp;","&").Replace("&apos;","'")
                }
                else
                {
                    // Select by Filter Product vendor
                    lstProductVendor = _lineitemRepository.SelectByFilterProductVendor(search2, Id); //.Replace("&amp;","&").Replace("&apos;","'")
                }
                ViewBag.lstProductVendor = lstProductVendor;
                ViewBag.Description = Description;
                ViewBag.upccode = UPCCode;
                //get Popup Header data
                ProductStoreAndVendorName data = _lineitemRepository.GetPopupHeaderData(InvoiceId);
                if (data != null)
                {
                    ViewBag.store = data.NickName;
                    ViewBag.vendorname = data.VendorName;
                }
                ViewBag.itemno = ItemNo;
                ViewBag.InvoiceProductId = InvoiceProductId;
                ViewBag.InvoiceId = InvoiceId;
                ViewBag.search = search;
            }
            catch (Exception ex)
            {
                logger.Error("LineItemController - GetInvoiceProductDatawithDesc - " + DateTime.Now + " - " + ex.Message.ToString());
            }            
            return PartialView("_MasterGrid", lstProducts);
        }

        /// <summary>
        /// This method is Get Vendor Product Data with descending order
        /// </summary>
        /// <param name="InvoiceProductId"></param>
        /// <param name="Description"></param>
        /// <param name="UPCCode"></param>
        /// <param name="ItemNo"></param>
        /// <param name="search"></param>
        /// <param name="InvoiceId"></param>
        /// <param name="search2"></param>
        /// <param name="VendorName"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetVendorProductDatawithDesc(int InvoiceProductId = 0, string Description = "", string UPCCode = "", string ItemNo = "", string search = "", int InvoiceId = 0, string search2 = "", string VendorName = "")
        {
            int Id = Convert.ToInt32(InvoiceProductId);
            //Get Invoice product By id
          
            List<ProductsList> lstProducts = new List<ProductsList>();
            try
            {
                var InvoiceProduct = _lineitemRepository.GetInvoiceProductById(Convert.ToInt32(Id));
                if (search != null)
                {
                    search = search.Trim();
                }
                if (search2 != null)
                {
                    search2 = search2.Trim();
                }
                if (string.IsNullOrEmpty(search2))
                {
                    // This class is Get Recommended Product List with vendor name
                    lstProducts = _lineitemRepository.GetRecommendedProductListWithVendorName(InvoiceProduct, Id, VendorName); //.Replace("&amp;","&").Replace("&apos;","'")
                }
                else
                {
                    //This class is Select By Filter product
                    lstProducts = _lineitemRepository.SelectByFilterProduct(search2, Id); //.Replace("&amp;","&").Replace("&apos;","'")
                }
                List<productVendorAllList> lstProductVendor = new List<productVendorAllList>();
                if (string.IsNullOrEmpty(search))
                {
                    // Get Recommended Product Vendor List
                    lstProductVendor = _lineitemRepository.GetRecommendedProductVendorList(InvoiceProduct, Id, VendorName); //.Replace("&amp;","&").Replace("&apos;","'")
                }
                else
                {
                    // Select by Filter Product vendor
                    lstProductVendor = _lineitemRepository.SelectByFilterProductVendor(search, Id); //.Replace("&amp;","&").Replace("&apos;","'")
                }

                ViewBag.lstProductVendor = lstProductVendor;

                ViewBag.Description = Description;
                ViewBag.upccode = UPCCode;
                //This class is Get Popup Header data
                ProductStoreAndVendorName data = _lineitemRepository.GetPopupHeaderData(InvoiceId);
                if (data != null)
                {
                    ViewBag.store = data.NickName;
                    ViewBag.vendorname = data.VendorName;
                }
                ViewBag.itemno = ItemNo;
                ViewBag.InvoiceProductId = InvoiceProductId;
                ViewBag.InvoiceId = InvoiceId;
                ViewBag.search = search;
            }
            catch (Exception ex)
            {
                logger.Error("LineItemController - GetVendorProductDatawithDesc - " + DateTime.Now + " - " + ex.Message.ToString());
            }           
            return PartialView("_VendorGrid", lstProducts);
        }

        /// <summary>
        /// This method is get url data source for search list data
        /// </summary>
        /// <param name="dm"></param>
        /// <param name="Id"></param>
        /// <returns></returns>
        public ActionResult UrlDatasourceSearchListGetList(DataManagerRequest dm, string Id)
        {
            //get Invoice Product By id
            var InvoiceProduct = _lineitemRepository.GetInvoiceProductById(Convert.ToInt32(Id));
            List<ProductsList> lstProducts = new List<ProductsList>();
            IEnumerable DataSource = lstProducts;
            int count = 0;
            try
            {
                //Get Recommended product List
                lstProducts = _lineitemRepository.GetRecommendedProductList(InvoiceProduct, Convert.ToInt32(Id)); //.Replace("&amp;","&").Replace("&apos;","'")               
                DataOperations operation = new DataOperations();
                if (dm.Search != null && dm.Search.Count > 0)
                {
                    DataSource = operation.PerformSearching(DataSource, dm.Search);  //Search
                }
                if (dm.Sorted != null && dm.Sorted.Count > 0) //Sorting
                {
                    DataSource = operation.PerformSorting(DataSource, dm.Sorted);
                }
                if (dm.Where != null && dm.Where.Count > 0) //Filtering
                {
                    DataSource = operation.PerformFiltering(DataSource, dm.Where, dm.Where[0].Operator);
                }
                count = DataSource.Cast<ProductsList>().Count();
                if (dm.Skip != 0)
                {
                    DataSource = operation.PerformSkip(DataSource, dm.Skip);   //Paging
                }
                if (dm.Take != 0)
                {
                    DataSource = operation.PerformTake(DataSource, dm.Take);
                }
            }
            catch (Exception Ex)
            {
                logger.Error("LineItemController - UrlDatasourceSearchListGetList - " + DateTime.Now + " - " + Ex.Message.ToString());
            }          
            return dm.RequiresCounts ? Json(new { result = DataSource, count = count }) : Json(DataSource);
        }
        /// <summary>
        /// This method is get url data source for search list data
        /// </summary>
        /// <param name="dm"></param>
        /// <param name="Id"></param>
        /// <returns></returns>
        public ActionResult UrlDatasourceSearchListGet(DataManagerRequest dm, string Id)
        {
            List<ProductsList> lstProducts = new List<ProductsList>();
            IEnumerable DataSource = lstProducts;
            int count = 0;
            try
            {
                if (dm.Search != null && dm.Search.Count > 0)
                {
                    string search = dm.Search.Select(s => s.Key).First();
                    //This class is used for Filter
                    lstProducts = _lineitemRepository.SelectByFilter(search, Id); //.Replace("&amp;","&").Replace("&apos;","'")
                }
                DataOperations operation = new DataOperations();
                //if (dm.Search != null && dm.Search.Count > 0)
                //{
                //    DataSource = operation.PerformSearching(DataSource, dm.Search);  //Search
                //}
                if (dm.Sorted != null && dm.Sorted.Count > 0) //Sorting
                {
                    DataSource = operation.PerformSorting(DataSource, dm.Sorted);
                }
                if (dm.Where != null && dm.Where.Count > 0) //Filtering
                {
                    DataSource = operation.PerformFiltering(DataSource, dm.Where, dm.Where[0].Operator);
                }
                count = DataSource.Cast<ProductsList>().Count();
                if (dm.Skip != 0)
                {
                    DataSource = operation.PerformSkip(DataSource, dm.Skip);   //Paging
                }
                if (dm.Take != 0)
                {
                    DataSource = operation.PerformTake(DataSource, dm.Take);
                }
            }
            catch (Exception Ex)
            {
                logger.Error("LineItemController - UrlDatasourceSearchListGet - " + DateTime.Now + " - " + Ex.Message.ToString());
            }
            
            return dm.RequiresCounts ? Json(new { result = DataSource, count = count }) : Json(DataSource);
        }

        /// <summary>
        /// This method is set popup Header of line item
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public ActionResult SetPopupHeader(LineItemList value)
        {
            //This class is Get Popup Header data
            ProductStoreAndVendorName data = _lineitemRepository.GetPopupHeaderData(value.InvoiceId);
            return Json(new { vendorname = data.VendorName, Item = value.Description, store = data.NickName, upc = value.UPCCode, itemno = value.ItemNo });
        }

        /// <summary>
        /// This method is add vendor Line Item
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public ActionResult AddVendorLineItem(LineItemList value)
        {
            string message = "";
            try
            {
                //Save Vendor Line Item
                _lineitemRepository.AddVendorLineItem(value);
                message = "Success";
            }
            catch (Exception Ex)
            {
                logger.Error("LineItemController - AddVendorLineItem - " + DateTime.Now + " - " + Ex.Message.ToString());
            }
            return Json(message);
        }

        /// <summary>
        /// This method is Assign Product List
        /// </summary>
        /// <param name="products"></param>
        /// <returns></returns>
        public ActionResult AssignProduct(ProductsList products)
        {
            string message = "";
            try
            {
                //This class is Assign product
                _lineitemRepository.AssignProduct(products);
                message = "Success";
            }
            catch (Exception Ex)
            {
                logger.Error("LineItemController - AssignProduct - " + DateTime.Now + " - " + Ex.Message.ToString());
            }
            return Json(message);
        }

        /// <summary>
        /// This method is Get Invoice File Path by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult GetInvoiceFilePath(string id)
        {
            //get Invoice by ID
            Invoice Data = _lineitemRepository.GetInvoiceById(Convert.ToInt64(id));
            var FilePath = "/UserFiles/Invoices/" + Data.UploadInvoice;
            return Json(FilePath);
        }

        /// <summary>
        /// This method is get Product Master data
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public ActionResult GetProductMasterData(LineItemList value)
        {
            List<ProductsList> lstProducts = new List<ProductsList>();
            //This db class is used for Get product master data
            lstProducts = _lineitemRepository.GetProductMasterData(value);
            ViewBag.DataSourse = lstProducts;
            return PartialView("LinkedProductList");
        }

        /// <summary>
        /// This method is Get master grid of Line Item List
        /// </summary>
        /// <param name="value"></param>
        /// <param name="VendorName"></param>
        /// <returns></returns>
        public ActionResult GetMasterGrid(LineItemList value, string VendorName = "")
        {
            int Id = Convert.ToInt32(value.InvoiceProductId);

            //get Invoice Product By id
            var InvoiceProduct = _lineitemRepository.GetInvoiceProductById(Convert.ToInt32(Id));
            List<ProductsList> lstProducts = new List<ProductsList>();
            try
            {

                lstProducts = _lineitemRepository.GetRecommendedProductListWithVendorName(InvoiceProduct, Id, VendorName); //.Replace("&amp;","&").Replace("&apos;","'")

                List<productVendorAllList> lstProductVendor = new List<productVendorAllList>();
                // Get Recommended Product Vendor List
                lstProductVendor = _lineitemRepository.GetRecommendedProductVendorList(InvoiceProduct, Id, VendorName); //.Replace("&amp;","&").Replace("&apos;","'")
                ViewBag.lstProductVendor = lstProductVendor;
                //This class is Get Popup Header data
                ProductStoreAndVendorName data = _lineitemRepository.GetPopupHeaderData(value.InvoiceId);
                ViewBag.store = data.NickName;
                ViewBag.vendorname = data.VendorName;
                //ViewBag.LineItemList = value;
                ViewBag.itemno = value.ItemNo;

                ViewBag.Description = value.Description;
                ViewBag.InvoiceProductId = value.InvoiceProductId;
                ViewBag.InvoiceId = value.InvoiceId;
                ViewBag.upccode = value.UPCCode;
            }
            catch (Exception Ex)
            {
                logger.Error("LineItemController - GetMasterGrid - " + DateTime.Now + " - " + Ex.Message.ToString());
            }
            // This class is Get Recommended Product List with vendor name

            return PartialView("_MasterGrid", lstProducts);
        }

        /// <summary>
        /// This method is Get Vendor grid of Line Item List
        /// </summary>
        /// <param name="value"></param>
        /// <param name="VendorName"></param>
        /// <returns></returns>
        public ActionResult GetVendorGrid(LineItemList value, string VendorName = "")
        {
            int Id = Convert.ToInt32(value.InvoiceProductId);

            //get Invoice Product By id
            var InvoiceProduct = _lineitemRepository.GetInvoiceProductById(Convert.ToInt32(Id));
            List<ProductsList> lstProducts = new List<ProductsList>();
            // This class is Get Recommended Product List with vendor name
            lstProducts = _lineitemRepository.GetRecommendedProductListWithVendorName(InvoiceProduct, Id, VendorName); //.Replace("&amp;","&").Replace("&apos;","'")

            List<productVendorAllList> lstProductVendor = new List<productVendorAllList>();
            // Get Recommended Product Vendor List
            lstProductVendor = _lineitemRepository.GetRecommendedProductVendorList(InvoiceProduct, Id, VendorName); //.Replace("&amp;","&").Replace("&apos;","'")
            ViewBag.lstProductVendor = lstProductVendor;
            //This class is Get Popup Header data
            ProductStoreAndVendorName data = _lineitemRepository.GetPopupHeaderData(value.InvoiceId);
            ViewBag.store = data.NickName;
            ViewBag.vendorname = data.VendorName;
            ViewBag.itemno = value.ItemNo;

            ViewBag.Description = value.Description;
            ViewBag.InvoiceProductId = value.InvoiceProductId;
            ViewBag.InvoiceId = value.InvoiceId;
            ViewBag.upccode = value.UPCCode;
            return PartialView("_VendorGrid", lstProducts);
        }

        /// <summary>
        /// This method is Get Add Libray 
        /// </summary>
        /// <param name="InvoiceProductId"></param>
        /// <param name="Description"></param>
        /// <param name="UPCCode"></param>
        /// <param name="ItemNo"></param>
        /// <returns></returns>
        public ActionResult GetAddLibrary(int InvoiceProductId = 0, string Description = "", string UPCCode = "", string ItemNo = "")
        {
            int Id = Convert.ToInt32(InvoiceProductId);

            //get Invoice Product By id
            var InvoiceProduct = _lineitemRepository.GetInvoiceProductById(Convert.ToInt32(Id));
            ProductsList obj = new ProductsList();
            obj.ItemNo = ItemNo;
            obj.UPCCode = UPCCode;
            obj.Description = Description;
            obj.InvoiceProductId = InvoiceProductId;
            return PartialView("AddToLibrary", obj);
        }
        /// <summary>
        /// This method is remove product mapping
        /// </summary>
        /// <param name="InvoiceProductId"></param>
        /// <returns></returns>
        public ActionResult RemoveProductMapping(string InvoiceProductId)
        {
            string message = "";
            try
            {
                if (InvoiceProductId != "")
                {

                    //get Invoice Product By id
                    var InvoiceProduct = _lineitemRepository.GetInvoiceProductById(Convert.ToInt32(InvoiceProductId));
                    _lineitemRepository.RemoveProductmapping(InvoiceProduct);
                }
                message = "Success";
            }
            catch (Exception Ex)
            {
                logger.Error("LineItemController - RemoveProductMapping - " + DateTime.Now + " - " + Ex.Message.ToString());
            }
            return Json(message);
        }

        /// <summary>
        /// This method is get Product Details of Line Item
        /// </summary>
        /// <param name="id"></param>
        /// <param name="UPC"></param>
        /// <returns></returns>
        public ActionResult ProductDetailLineItem(int? id, string UPC)
        {
            ProductPriceModel obj = new ProductPriceModel();
            try
            {
                //this db class is get all storid
                ViewBag.StoreId = new SelectList((from dataUser in _lineitemRepository.GetStoreId()
                                                  select new StoreQBSyncModel
                                                  {
                                                      StoreId = dataUser.StoreId,
                                                      StoreName = dataUser.NickName

                                                  }).ToList(), "StoreId", "StoreName");
                //this db class is get all storid
                ViewBag.StoreIds = new SelectList((from dataUser in _lineitemRepository.GetStoreId()
                                                   select new StoreQBSyncModel
                                                   {
                                                       StoreId = dataUser.StoreId,
                                                       StoreName = dataUser.NickName

                                                   }).ToList(), "StoreId", "StoreName");
                //this db class is get all storid
                ViewBag.StoreIdT = new SelectList((from dataUser in _lineitemRepository.GetStoreId()
                                                   select new StoreQBSyncModel
                                                   {
                                                       StoreId = dataUser.StoreId,
                                                       StoreName = dataUser.NickName

                                                   }).ToList(), "StoreId", "StoreName");
                if (id == null)
                {
                    if (UPC != null)
                    {
                        //This class is Invoice products
                        id=_lineitemRepository.InvoiceProducts(UPC);
                    }
                }

              
                //Get Products by Id
                Products products = _lineitemRepository.GetProductsById(id);
                obj.ProductId = (int)id;
                obj.ItemId = products.SynthesisId;
                obj.Name = products.Description;
                obj.Size = products.Size;
                obj.Brand = products.Brand;
                obj.Department = products.Departments;

                obj.DateCreated = products.DateCreated == null ? "" : products.DateCreated.Value.ToString("MM/dd/yyyy");
                obj.ProductImage = products.ProductImage;
                obj.Notes = products.KeyWord == null ? "" : products.KeyWord;
                //Get Max And Averange Price by Id
                var list1 = _lineitemRepository.GetMaxAndAveragePrice(id);
                //Get Max store name by id
                var list2 = _lineitemRepository.GetMAXStoreName(id);
                List<LineItemViewModel> List3 = new List<LineItemViewModel>();
                //Get Invoice quantity by id
                List3 = _lineitemRepository.GetInvoiceQty(id);

                if (list1 != null)
                {
                    obj.MaxPrice = Convert.ToDecimal(list1.MaxPrice);
                    obj.MinPrice = Convert.ToDecimal(list1.MinPrice);
                }
                List<QtyMax> list = new List<QtyMax>();
                if (list2 != null)
                {
                    obj.FPStoreName = list2.Name;
                }
                string q = "";
                string y = "";
                if (List3 != null)
                {
                    foreach (var dr in List3)
                    {
                        QtyMax qty = new QtyMax();
                        qty.StoreName = dr.Name.ToString().Trim();
                        q = dr.Qty.ToString().Trim();
                        for (int i = 0; i < q.Length; i++)
                        {
                            if (Regex.IsMatch(q[i].ToString(), @"^\d+$"))
                            {
                                y += q[i];
                            }
                            else
                            {
                                break;
                            }
                        }
                        qty.Qty = y;
                        list.Add(qty);
                        y = "";
                    }
                }
                var l = list.GroupBy(g => g.StoreName).Select(s => new { StoreName = s.Key, Qty = s.Sum(o => Convert.ToInt32(o.Qty == "" ? "0" : o.Qty)) }).ToList().OrderByDescending(s => s.Qty).FirstOrDefault();
                if (l != null)
                {
                    obj.QPStoreName = l.StoreName;
                }
                
            }
            catch (Exception ex)
            {
                logger.Error("LineItemController - ProductDetailLineItem - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return View(obj);
        }

        /// <summary>
        /// This method is return partial view of Line items lookup
        /// </summary>
        /// <param name="ProductId"></param>
        /// <param name="StoreIds"></param>
        /// <param name="InvoiceNumber"></param>
        /// <returns></returns>
        public ActionResult LineItemsLookup(int ProductId, int StoreIds, string InvoiceNumber)
        {
            try
            {
                List<LineItemsLookUp> list = new List<LineItemsLookUp>();
                //Get list of Line Item Look up
                list = _lineitemRepository.LineItemLookUp(ProductId, StoreIds, InvoiceNumber);
                return PartialView("VendorLookup", list);

            }
            catch (Exception ex)
            {
                logger.Error("LineItemController - LineItemsLookup - " + DateTime.Now + " - " + ex.Message.ToString());
                return PartialView("VendorLookup");
            }
        }
        /// <summary>
        /// This method get vendor list by product id
        /// </summary>
        /// <param name="ProductId"></param>
        /// <returns></returns>
        public ActionResult VendorList(int ProductId)
        {
            try
            {
                List<VendoreLookUp> list = new List<VendoreLookUp>();
                //Get Vendor Look up List
                list = _lineitemRepository.VendorLookupList(ProductId);

                return PartialView("_VendorList", list);

            }
            catch (Exception ex)
            {
                logger.Error("LineItemController - VendorList - " + DateTime.Now + " - " + ex.Message.ToString());
                return PartialView("_VendorList");
            }
        }
        /// <summary>
        /// This method get Line Chart
        /// </summary>
        /// <param name="ProductId"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="storeId"></param>
        /// <returns></returns>
        public ActionResult getlinechart(int ProductId, string startDate, string endDate, string storeId)
        {
            if (storeId == "")
            {
                storeId = null;
            }
            ProductPriceModel obj = new ProductPriceModel();

            try
            {
                List<ProductPriceModel> list = new List<ProductPriceModel>();
                //Get Line chart
                list = _lineitemRepository.Getlinechart(ProductId, startDate, endDate, storeId);

                return Json(list, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error("LineItemController - getlinechart - " + DateTime.Now + " - " + ex.Message.ToString());
                return Json("Notget", JsonRequestBehavior.AllowGet); ;
            }
        }

        /// <summary>
        /// This method is Upload Image
        /// </summary>
        /// <param name="postedFile"></param>
        /// <param name="ProductId"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult UploadImage(HttpPostedFileBase postedFile, int ProductId)
        {
            try
            {
                byte[] bytes;
                using (BinaryReader br = new BinaryReader(postedFile.InputStream))
                {
                    bytes = br.ReadBytes(postedFile.ContentLength);
                }
                //This class is upload Image
                _lineitemRepository.UploadImage(bytes, ProductId);
                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error("LineItemController - UploadImage - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return Json(new { success = false }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// This method is Delete Imag by Product Id
        /// </summary>
        /// <param name="ProductId"></param>
        /// <returns></returns>
        public ActionResult DeleteImage(int ProductId)
        {
            try
            {
                //This class is used for Delete Image
                _lineitemRepository.DeleteImage(ProductId);
                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error("LineItemController - DeleteImage - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return Json(new { success = false }, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// This method is get store pir chart
        /// </summary>
        /// <param name="ProductId"></param>
        /// <returns></returns>
        public ActionResult StoregetPiechart(int ProductId)
        {
            StorePieChartModel obj = new StorePieChartModel();
            try
            {
                List<StorePieChartModel> linechartdate = new List<StorePieChartModel>();
                //Get List of store Pi chart by product id
                linechartdate = _lineitemRepository.StoregetPiechart(ProductId);

                return Json(linechartdate, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error("LineItemController - StoregetPiechart - " + DateTime.Now + " - " + ex.Message.ToString());
                return Json("Notget", JsonRequestBehavior.AllowGet); ;
                
            }
        }

        /// <summary>
        /// This method is Sync Line Item
        /// </summary>
        /// <param name="InvoiceProductId"></param>
        /// <param name="Description"></param>
        /// <param name="UPCCode"></param>
        /// <param name="ItemNo"></param>
        /// <param name="search"></param>
        /// <param name="InvoiceId"></param>
        /// <param name="ProductId"></param>
        /// <param name="Type"></param>
        /// <param name="Brand"></param>
        /// <param name="Size"></param>
        /// <param name="VendorName"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult syncLineItem(int InvoiceProductId = 0, string Description = "", string UPCCode = "", string ItemNo = "", string search = "", int InvoiceId = 0, int ProductId = 0, int Type = 0, string Brand = null, string Size = null, string VendorName = "")
        {
            string message = "";
            int ID = 0;
            try
            {
                if (Type == 1)
                {
                    //Get Invoice product By Id
                    InvoiceProduct invoiceProduct = _lineitemRepository.GetInvoiceProductById(InvoiceProductId);
                    //This class is get order By descending
                    Products Pro = _lineitemRepository.GetOrderByDescending();
                    var Synthesis = "";
                    int SynNo = 0;
                    if (Pro != null)
                    {
                        Synthesis = Pro.SynthesisId.Split('-')[1];
                        SynNo = Convert.ToInt32(Synthesis) + 1;
                    }
                    Products obj = new Products();
                    obj.UPCCode = UPCCode;
                    obj.ItemNo = ItemNo;
                    obj.SynthesisId = "WM-" + SynNo.ToString().PadLeft(9, '0');
                    obj.Description = Description;
                    obj.Vendor = VendorName;
                    obj.Departments = null;//DepartmentName;
                    obj.Brand = Brand;//invoiceProduct.ProductVendors.Brand;
                    obj.Size = Size;//invoiceProduct.ProductVendors.Size;
                    obj.Flag = true;
                    //This class is Insert product data
                    ID = _CommonRepository.InsertProduct(obj);
                }
                else
                {
                    //Get Invoice product By Id
                    InvoiceProduct invoiceProduct = _lineitemRepository.GetInvoiceProductById(InvoiceProductId);
                    ProductVendor obj = new ProductVendor();
                    obj.ItemNo = ItemNo;
                    obj.UPCCode = UPCCode;
                    obj.Description = Description;
                    obj.Vendors = VendorName;//invoiceProduct.ProductVendors.Vendors;
                    obj.Brand = Brand;//invoiceProduct.ProductVendors.Brand;
                    obj.Size = Size;//invoiceProduct.ProductVendors.Size;
                    obj.Price = invoiceProduct.UnitPrice.ToString();
                    obj.Flag = true;
                    //this db class is Insert Product vendor
                    ID = _CommonRepository.InsertProductVendor(obj);
                }
                message = "Success|" + ID;
            }
            catch (Exception Ex)
            {
                logger.Error("LineItemController - syncLineItem - " + DateTime.Now + " - " + Ex.Message.ToString());
            }
            return Json(message);
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
                _lineitemRepository.Unlink_ItemLines(value);
                message = "Success";
            }
            catch (Exception Ex)
            {
                logger.Error("LineItemController - Unlinkitem - " + DateTime.Now + " - " + Ex.Message.ToString());
            }
            return Json(message);
        }
        /// <summary>
        /// This method is used to Unlink Item Line from VendorLookup Page
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>

        //Himanshu 29-04-2024
        public ActionResult UnlinkitemLookup(string itemname)
        {
            string message = "";
            string ActivityLogMessage = "";
            try
            {
                _lineitemRepository.Unlink_ItemLines_Lookup(itemname);

                ActivityLogMessage = "Line Item Unlinked for ItemName " + itemname + " by " + UserModule.getUserFirstName() + " on " + AdminSiteConfiguration.GetDate(AdminSiteConfiguration.GetEasternTime(DateTime.Now).ToString());
                ActivityLog ActLog1 = new ActivityLog();
                ActLog1.Action = 1;
                ActLog1.Comment = ActivityLogMessage;
                //This  class is used for Activity Log Insert.
                _ActivityLogRepository.ActivityLogInsert(ActLog1);

                message = "Success";
            }
            catch (Exception Ex)
            {
                logger.Error("LineItemController - UnlinkitemLookup - " + DateTime.Now + " - " + Ex.Message.ToString());
            }
            return Json(message);
        }
        //Himanshu 29-04-2024
        public ActionResult GetInvoiceProductName(string itemname)
        {
            int invoicecount = 0;
            try
            {
                invoicecount = _lineitemRepository.LineItemInvoiceCount(itemname);
            }
            catch (Exception Ex)
            {
                logger.Error("LineItemController - GetInvoiceProductCount - " + DateTime.Now + " - " + Ex.Message.ToString());
            }
            var result = new { ItemName = itemname, InvoiceCount = invoicecount };
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}