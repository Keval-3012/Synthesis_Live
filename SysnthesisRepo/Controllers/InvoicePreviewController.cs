using Syncfusion.EJ2.Base;
using Syncfusion.EJ2.GridExport;
using Syncfusion.Pdf;
using EntityModels.Models;
using Repository;
using Repository.IRepository;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Security;
using Utility;
using Syncfusion.EJ2.Popups;
using SynthesisViewModal;
using NLog;
using System.Text;

namespace SysnthesisRepo.Controllers
{
    public class InvoicePreviewController : Controller
    {
        #region variables
        private readonly IInvoicePreviewRepository _InvoicePreviewRepository;
        private readonly ICommonRepository _CommonRepository;
        private readonly IUserActivityLogRepository _ActivityLogRepository;
        protected static string DeleteMessage = "";
        protected static string StatusMessage = "";
        protected static string ExistCode = "";
        protected static string ActivityLogMessage = "";
        protected static int TotalDataCount;
        protected static Array Arr;
        protected static bool IsArray;
        protected static string IsFilter = "0";
        protected static bool IsEdit = false;
        protected static int Quickvalue;
        Logger logger = LogManager.GetCurrentClassLogger();
        #endregion
        public InvoicePreviewController()
        {
            this._InvoicePreviewRepository = new InvoicePreviewRepository(new DBContext());
            this._CommonRepository = new CommonRepository(new DBContext());
            this._ActivityLogRepository = new UserActivityLogRepository(new DBContext());
        }

        // GET: Invoices
        /// <summary>
        /// This method return Index view with MSG.
        /// </summary>
        /// <param name="MSG"></param>
        /// <returns></returns>
        [Authorize(Roles = "Administrator,ViewInvoiceProducts")]
        public async Task<ActionResult> Index(string MSG)
        {
            ViewBag.Title = "Vendor Product Lists - Synthesis";
            try
            {
                if (StatusMessage == "Success1" || StatusMessage == "Success" || StatusMessage == "Delete")
                {
                    ViewBag.StatusMessage = StatusMessage;
                    //StatusMessage = "";
                }
                else
                {
                    ViewBag.StatusMessage = "";
                }
            }
            catch (Exception ex)
            {
                logger.Error("InvoicePreviewController - Index - " + DateTime.Now + " - " + ex.Message.ToString());
            }
           
            return View();
        }

        /// <summary>
        /// This Grid method return product vendor list.
        /// </summary>
        /// <returns></returns>
        public ActionResult Grid()
        {
            try
            {
                var a = _InvoicePreviewRepository.GetproductVendorsList();
                ViewBag.VendorName = a.Select(c => c.Vendors == null ? "" : c.Vendors.Trim()).Distinct().ToList();
                ViewBag.VendorName.Insert(0, "All");
                List<DialogDialogButton> buttons = new List<DialogDialogButton>() { };
                buttons.Add(new DialogDialogButton() { Click = "dlgButtonClick", ButtonModel = new DefaultButtonModels() { content = "Save Mapping", isPrimary = true } });
                buttons.Add(new DialogDialogButton() { Click = "CancelBouttonClick", ButtonModel = new DefaultButtonModels() { content = "Cancel" } });
                ViewBag.DefaultButtons = buttons;

                List<DialogDialogButton> buttonsdata = new List<DialogDialogButton>() { };
                buttonsdata.Add(new DialogDialogButton() { Click = "RemoveButtonClick", ButtonModel = new DefaultButtonModels() { content = "Remove Linked", isPrimary = true } });
                buttonsdata.Add(new DialogDialogButton() { Click = "LinkedCancelBouttonClick", ButtonModel = new DefaultButtonModels() { content = "Cancel" } });
                ViewBag.DefaultButtonsLinked = buttonsdata;
                ViewBag.VendorNameforExcel = _InvoicePreviewRepository.GetproductVendorsList().Select(o => o.Vendors == null ? "" : o.Vendors.Trim()).Distinct().ToList();
                if (!Roles.IsUserInRole("Administrator"))
                {
                    var UserName = System.Web.HttpContext.Current.User.Identity.Name;
                    ViewBag.DrpLstStore = new SelectList(_CommonRepository.GetStoreList_RoleWise(1, "CreateInvoice", UserName), "StoreId", "NickName");
                }
                else
                {
                    ViewBag.DrpLstStore = new SelectList(_InvoicePreviewRepository.GetStoreMasters().OrderBy(o => o.NickName).ToList(), "StoreId", "NickName");
                }
                ViewBag.DrpLstVendor = new SelectList(_InvoicePreviewRepository.GetPriceFinderVendor(), "VendorId", "VendorName");
                ViewBag.Successmapped = _CommonRepository.GetMessageValue("VPLS", "Successfully Mapped!");
                ViewBag.VendorName1 = _CommonRepository.GetMessageValue("VPEVN", "Please Enter Vendor Name");
                ViewBag.File = _CommonRepository.GetMessageValue("VPPSF", "Please Select File");
                ViewBag.RemoveMap = _CommonRepository.GetMessageValue("VPLSR", "Successfully Removed Mapping!");
                ViewBag.AddVendor = _CommonRepository.GetMessageValue("VPSVI", "Successfully add vendor item!");
                ViewBag.Suredelete = _CommonRepository.GetMessageValue("VPLWD", "Are you sure you want to delete?");
                ViewBag.DeleteSuccess = _CommonRepository.GetMessageValue("VPLDV", "Successfully deleted vendor product!");
            }
            catch (Exception ex) 
            {
                logger.Error("InvoicePreviewController - Grid - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return View();
        }

        /// <summary>
        /// This method is get all the products data.      
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public ActionResult GetProductsData(ProductVendor value)
        {
            try
            {
                //db class is used to get all Product vendor by id.
                var InvoiceProduct = _InvoicePreviewRepository.GetproductVendorsListByID(value.ProductVendorId);
                List<Products> lstProducts = new List<Products>();
                if (InvoiceProduct != null)
                {
                    //db class is used to get all product list.
                    lstProducts = _InvoicePreviewRepository.GetProductsList(InvoiceProduct);
                }
                ViewBag.Count = lstProducts.Count();
                ViewBag.ProductVendorId = value.ProductVendorId;
            }
            catch (Exception ex)
            {
                logger.Error("InvoicePreviewController - GetProductsData - " + DateTime.Now + " - " + ex.Message.ToString());
            }


            return PartialView("_MappingProductList");
        }
        /// <summary>
        /// This method is set the Popup Header with vender name.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public ActionResult SetPopupHeader(ProductVendor value)
        {

            var vendorname = value.Vendors;
            var Item = value.Description;
            var upccode = value.UPCCode;
            //This db class is used to get select product Lisdt by vendor name.
            var storelist = _InvoicePreviewRepository.SelectProductListbyvendorname(vendorname);
            var store = string.Join(", ", storelist.Select(s => s.NickName));
            return Json(new { vendorname = vendorname, Item = Item, store = store, upc = upccode });
        }
        //public ActionResult GetList(string vendorname)
        //{
        //    Session["vendorname"] = null;
        //    if (!String.IsNullOrEmpty(vendorname))
        //    {
        //        Session["vendorname"] = vendorname;
        //    }

        //    return PartialView("_ProductListGrid");
        //}
        /// <summary>
        /// This method return all the List unmappped vendername.
        /// </summary>
        /// <param name="vendorname"></param>
        /// <returns></returns>
        public ActionResult GetListUnmapped(string vendorname)
        {
            Session["vendorname"] = null;
            if (!String.IsNullOrEmpty(vendorname))
            {
                Session["vendorname"] = vendorname;
            }

            return PartialView("_ProductListGridUnmapped");
        }
        /// <summary>
        /// This method is used get to Url datasource to search list.
        /// </summary>
        /// <param name="dm"></param>
        /// <param name="Id"></param>
        /// <returns></returns>
        public ActionResult UrlDatasourceSearchListGetList(DataManagerRequest dm, string Id)
        {
            //db class is used to get all Product vendor by id.
            var InvoiceProduct = _InvoicePreviewRepository.GetproductVendorsListByID(Convert.ToInt32(Id));
            List<ProductsList> lstProducts = new List<ProductsList>();
            IEnumerable DataSource = lstProducts;
            DataOperations operation = new DataOperations();
            int count = 0;
            
            try
            {
                if (Id == "")
                {
                    Id = "0";
                }

                if (InvoiceProduct != null)
                {
                    //db class is used to get all Productlists.

                    lstProducts = _InvoicePreviewRepository.GetProductsLists(InvoiceProduct);
                }
                foreach (var item in lstProducts)
                {
                    item.ProductVendorId = Convert.ToInt32(Id);
                }

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
            catch (Exception ex)
            {
                logger.Error("InvoicePreviewController - UrlDatasourceSearchListGetList - " + DateTime.Now + " - " + ex.Message.ToString());
            }
           
            
            return dm.RequiresCounts ? Json(new { result = DataSource, count = count }) : Json(DataSource);
        }
        /// <summary>
        /// This method use get  to Url datasourceserachlist   
        /// </summary>
        /// <param name="dm"></param>
        /// <param name="Id"></param>
        /// <returns></returns>
        public ActionResult UrlDatasourceSearchListGet(DataManagerRequest dm, string Id)
        {

            List<ProductsList> lstProducts = new List<ProductsList>();
            IEnumerable DataSource = new List<ProductsList>();
            DataOperations operation = new DataOperations();
            int count = 0;
            try
            {
                if (dm.Search != null && dm.Search.Count > 0)
                {
                    string search = dm.Search.Select(s => s.Key).First().Trim();
                    //This db class is used to serach data source url list.
                    lstProducts = _InvoicePreviewRepository.UrlDatasourceSearchListGet(search);
                }
                foreach (var item in lstProducts)
                {
                    item.ProductVendorId = Convert.ToInt32(Id);
                }
                DataSource = lstProducts;
                
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
            catch (Exception ex)
            {
                logger.Error("InvoicePreviewController - UrlDatasourceSearchListGet - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            
           
            //if (dm.Search != null && dm.Search.Count > 0)
            //{
            //    DataSource = operation.PerformSearching(DataSource, dm.Search);  //Search
            //}
            
            return dm.RequiresCounts ? Json(new { result = DataSource, count = count }) : Json(DataSource);
        }
        /// <summary>
        /// This method is return all the url datasource.
        /// </summary>
        /// <param name="dm"></param>
        /// <returns></returns>
        public ActionResult UrlDatasource(DataManagerRequest dm)
        {
            int? StoreIds = 0;
             List<ProductVendor> productVendors = new List<ProductVendor>();
            //This db class is used to get all vendor product previed data with vendorname.
            string vendorname = "";
            productVendors = _InvoicePreviewRepository.VendorProductPreviewData(vendorname);
                IEnumerable DataSource = productVendors;
                DataOperations operation = new DataOperations();
            int count = 0;
            try
            {
                if (Session["storeid"] != null)
                {
                    StoreIds = Convert.ToInt32(Convert.ToString(Session["storeid"]));
                }
              
                if (Session["vendorname"] != null)
                {
                    vendorname = Session["vendorname"].ToString();
                }
               
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
                count = DataSource.Cast<ProductVendor>().Count();
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
                logger.Error("InvoicePreviewController - UrlDatasource - " + DateTime.Now + " - " + ex.Message.ToString());
            }
           
            return dm.RequiresCounts ? Json(new { result = DataSource, count = count }) : Json(DataSource);
        }
        /// <summary>
        /// This method is return all the url datasource with unmapped.
        /// </summary>
        /// <param name="dm"></param>
        /// <returns></returns>
        public ActionResult UrlDatasourceUnmapped(DataManagerRequest dm)
        {
            int? StoreIds = 0;
            if (Session["storeid"] != null)
            {
                StoreIds = Convert.ToInt32(Convert.ToString(Session["storeid"]));
            }
            string vendorname = "";
            if (Session["vendorname"] != null)
            {
                vendorname = Session["vendorname"].ToString();
            }
            List<productVendorAllList> productVendors = new List<productVendorAllList>();

            //db class is used to get all productVendor List.

            productVendors = _InvoicePreviewRepository.productVendorAllList(vendorname);
            IEnumerable DataSource = productVendors;

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
            int count = DataSource.Cast<productVendorAllList>().Count();
            if (dm.Skip != 0)
            {
                DataSource = operation.PerformSkip(DataSource, dm.Skip);   //Paging
            }
            if (dm.Take != 0)
            {
                DataSource = operation.PerformTake(DataSource, dm.Take);
            }
            return dm.RequiresCounts ? Json(new { result = DataSource, count = count }) : Json(DataSource);
        }
        /// <summary>
        /// This method is used to  Assign product to vendore. 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="VendorN"></param>
        /// <returns></returns>
        public ActionResult AssignProduct(List<Products> value, string VendorN)
        {
            string message = "";
            try
            {
                foreach (var products in value)
                {
                    //db class is used to get all Product vendor by id.

                    var InvoiceProduct = _InvoicePreviewRepository.GetproductVendorsListByID(products.ProductVendorId);
                    List<ProductVendor> lstInvoiceProduct = new List<ProductVendor>();
                    //This db class is used to SelectInvoiceMatchProducts
                    lstInvoiceProduct = _InvoicePreviewRepository.SelectInvoiceMatchProducts(InvoiceProduct);
                    if (lstInvoiceProduct.Count() > 0)
                    {
                        foreach (var item in lstInvoiceProduct)
                        {
                            //db class is used to update product invoice.

                            _InvoicePreviewRepository.UpdateProductInvoice(products, item.ProductVendorId);
                        }
                    }
                    else
                    {
                        //db class is used to update product invoice.

                        _InvoicePreviewRepository.UpdateProductInvoice(products, InvoiceProduct.ProductVendorId);
                    }
                }
                message = "Success";
            }
            catch (Exception ex)
            {
                logger.Error("InvoicePreviewController - AssignProduct - " + DateTime.Now + " - " + ex.Message.ToString());
                message = "Error";
            }
            return Json(message);
        }
        /// <summary>
        /// This method is used to get all product Master.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public ActionResult GetProductMasterData(ProductVendor value)
        {
            List<Products> lstProducts = new List<Products>();
            //db class is used for get all products list.

            lstProducts = _InvoicePreviewRepository.GetproductsList(value.ProductId);
            ViewBag.DataSourse = lstProducts;
            return PartialView("_LinkedProductMasterList");
        }

        /// <summary>
        /// This method is used to renove product mapping with vendor.
        /// </summary>
        /// <param name="ProductVendorId"></param>
        /// <param name="VendorN"></param>
        /// <returns></returns>
        public ActionResult RemoveProductMapping(string ProductVendorId, string VendorN)
        {
            string message = "";
            try
            {
                if (ProductVendorId != "")
                {
                    //db class is used to update product Vendors.

                    _InvoicePreviewRepository.UpdateProductVendors(Convert.ToInt32(ProductVendorId));
                }
                message = "Success";
            }
            catch (Exception ex)
            {
            	logger.Error("InvoicePreviewController - RemoveProductMapping - " + DateTime.Now + " - " + ex.Message.ToString());
                message = "Error";
            }
            return Json(message);
        }
        /// <summary>
        /// This method is used to add vendor item.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public ActionResult AddVendorItem(ProductVendor value)
        {
            string message = "";
            try
            {
                //db class is used to get all Products list.

                Products Pro = _InvoicePreviewRepository.GetproductsList().OrderByDescending(u => u.ProductId).FirstOrDefault();
                var Synthesis = "";
                int SynNo = 0;
                if (Pro != null)
                {
                    Synthesis = Pro.SynthesisId.Split('-')[1];
                    SynNo = Convert.ToInt32(Synthesis) + 1;
                }
                Products products = new Products();
                products.ItemNo = value.ItemNo;
                products.UPCCode = value.UPCCode;
                products.Description = (value.Description == null ? null : value.Description.ToString().ToUpper());
                products.Vendor = value.Vendors;
                products.Size = value.Size;
                products.Brand = (value.Brand == null ? null : value.Brand.ToString().ToUpper());
                products.SynthesisId = "WM-" + SynNo.ToString().PadLeft(9, '0');
                products.Departments = null;
                products.DateCreated = DateTime.Now;
                //db class is used to Add products.

                _InvoicePreviewRepository.AddproductsList(products);

                int ProductId = products.ProductId;
                if (ProductId > 0)
                {
                    //This db class is used to update products.
                    _InvoicePreviewRepository.UpdateProductInvoice1(ProductId, value.ProductVendorId);
                }
                message = "Success";
            }
            catch (Exception ex)
            {
            	logger.Error("InvoicePreviewController - AddVendorItem - " + DateTime.Now + " - " + ex.Message.ToString());
                message = "Error";
            }
            return Json(message);
        }
        /// <summary>
        /// This method is used to delete vendor item.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public async Task<ActionResult> DeleteVendorItem(ProductVendor value)
        {
            string message = "";
            try
            {
                //This db class is used to get all Product vendorslist by id.
                ProductVendor pv = _InvoicePreviewRepository.GetproductVendorsListByID(value.ProductVendorId);

                //This db class is used to delete Vendor Item.
                _InvoicePreviewRepository.DeleteVendorItem(pv);

                ActivityLog ActLog = new ActivityLog();
                ActLog.Action = 3;
                var UserName = System.Web.HttpContext.Current.User.Identity.Name;
                //This Db class is used for get user firstname.
                ActLog.Comment = "ProductVendor " + value.ProductVendorId + " Deleted by " + _CommonRepository.getUserFirstName(UserName) + " on " + AdminSiteConfiguration.GetDate(AdminSiteConfiguration.GetEasternTime(DateTime.Now).ToString());
                //This  class  is used  for Activity Log Insert.
                _ActivityLogRepository.ActivityLogInsert(ActLog);
                message = "Success";
            }
            catch (Exception ex)
            {
            	logger.Error("InvoicePreviewController - DeleteVendorItem - " + DateTime.Now + " - " + ex.Message.ToString());
                message = "Error";
            }
            return Json(message);
        }

        /// <summary>
        /// This method is used to delete vendor item with selected value.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public async Task<ActionResult> DeleteVendorItembyselect(string value)
        {
            string message = "";
            try
            {
                string[] data = value.Split(',');
                for (int i = 0; i < data.Length; i++)
                {
                    if (data[i].ToString() != "")
                    {
                        //This db class is used to get all Product vendorslist by id.
                        ProductVendor pv = _InvoicePreviewRepository.GetproductVendorsListByID(Convert.ToInt32(data[i]));

                        //This db class is used to delete Vendor Item by selected value.
                        _InvoicePreviewRepository.DeleteVendorItem(pv);

                        ActivityLog ActLog = new ActivityLog();
                        ActLog.Action = 3;
                        var UserName = System.Web.HttpContext.Current.User.Identity.Name;
                        //This Db class  is used  for get user firstname.


                        ActLog.Comment = "ProductVendor " + value + " Deleted by " + _CommonRepository.getUserFirstName(UserName) + " on " + AdminSiteConfiguration.GetDate(AdminSiteConfiguration.GetEasternTime(DateTime.Now).ToString());
                        //This  class  is used  for Activity Log Insert.
                        _ActivityLogRepository.ActivityLogInsert(ActLog);
                    }
                }

                message = "Success";
            }
            catch (Exception ex)
            {
				logger.Error("InvoicePreviewController - DeleteVendorItembyselect - " + DateTime.Now + " - " + ex.Message.ToString());
                message = "Error";
            }
            return Json(message);
        }


        /// <summary>
        /// This method is return Linqtodatatable.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="varlist"></param>
        /// <returns></returns>
        public DataTable LINQToDataTable<T>(IEnumerable<T> varlist)
        {
            DataTable dtReturn = new DataTable();

            // column names 
            PropertyInfo[] oProps = null;
            try
            {
                if (varlist == null)
                {
                    return dtReturn;
                }

                foreach (T rec in varlist)
                {
                    // Use reflection to get property names, to create table, Only first time, others 

                    if (oProps == null)
                    {
                        oProps = ((Type)rec.GetType()).GetProperties();
                        foreach (PropertyInfo pi in oProps)
                        {
                            Type colType = pi.PropertyType;

                            if ((colType.IsGenericType) && (colType.GetGenericTypeDefinition()
                            == typeof(Nullable<>)))
                            {
                                colType = colType.GetGenericArguments()[0];
                            }

                            dtReturn.Columns.Add(new DataColumn(pi.Name, colType));
                        }
                    }

                    DataRow dr = dtReturn.NewRow();

                    foreach (PropertyInfo pi in oProps)
                    {
                        dr[pi.Name] = pi.GetValue(rec, null) == null ? DBNull.Value : pi.GetValue
                        (rec, null);
                    }

                    dtReturn.Rows.Add(dr);
                }
            }
            catch (Exception ex)
            {
                logger.Error("InvoicePreviewController - LINQToDataTable - " + DateTime.Now + " - " + ex.Message.ToString());
            }
        
            return dtReturn;
        }

        public async Task<ActionResult> ExcelExport(string gridModel,string vendorname)
        {
            List<productVendorAllList> lstProducts = new List<productVendorAllList>();
            GridExcelExport exp = new GridExcelExport();

            try
            {
                lstProducts = _InvoicePreviewRepository.productVendorAllList(vendorname);

                ViewBag.Loadtest = false;
                exp.FileName = "Product.xlsx";
            }
            catch (Exception ex)
            {
                logger.Error("InvoicePreviewController - ExcelExport - " + DateTime.Now + " - " + ex.Message.ToString());
            }

            
            Syncfusion.EJ2.Grids.Grid gridProperty = ConvertGridObject(gridModel, lstProducts);
            return exp.ExcelExport<productVendorAllList>(gridProperty, lstProducts);

        }

        private Syncfusion.EJ2.Grids.Grid ConvertGridObject(string gridProperty, List<productVendorAllList> data)
        {
            Syncfusion.EJ2.Grids.Grid GridModel = (Syncfusion.EJ2.Grids.Grid)Newtonsoft.Json.JsonConvert.DeserializeObject(gridProperty, typeof(Syncfusion.EJ2.Grids.Grid));

            try
            {
                GridColumnModel cols = (GridColumnModel)Newtonsoft.Json.JsonConvert.DeserializeObject("{\"allowGrouping\":false,\"allowPaging\":false,\"pageSettings\":{\"currentPage\":1,\"pageCount\":2,\"pageSize\":100},\"sortSettings\":{},\"allowPdfExport\":true,\"allowExcelExport\":true,\"aggregates\":[],\"filterSettings\":{\"immediateModeDelay\":1500,\"type\":\"CheckBox\"},\"groupSettings\":{\"columns\":[],\"enableLazyLoading\":false,\"disablePageWiseAggregates\":false},\"columns\":[],\"locale\":\"en-US\",\"searchSettings\":{\"key\":\"\",\"fields\":[]}}", typeof(GridColumnModel));
                cols.columns.Add(new Syncfusion.EJ2.Grids.GridColumn { Field = "Vendors", HeaderText = "Vendore Name" });
                cols.columns.Add(new Syncfusion.EJ2.Grids.GridColumn { Field = "ItemNo", HeaderText = "Item No" });
                cols.columns.Add(new Syncfusion.EJ2.Grids.GridColumn { Field = "Description", HeaderText = "Description" });
                cols.columns.Add(new Syncfusion.EJ2.Grids.GridColumn { Field = "UPCCode", HeaderText = "UPC Code" });
                cols.columns.Add(new Syncfusion.EJ2.Grids.GridColumn { Field = "Brand", HeaderText = "Brand" });
                cols.columns.Add(new Syncfusion.EJ2.Grids.GridColumn { Field = "Size", HeaderText = "Size" });
                cols.columns.Add(new Syncfusion.EJ2.Grids.GridColumn { Field = "Price", HeaderText = "Price" });
                cols.columns.Add(new Syncfusion.EJ2.Grids.GridColumn { Field = "DateCreateds", HeaderText = "Date" });
                cols.columns.Add(new Syncfusion.EJ2.Grids.GridColumn { Field = "CreatedBys", HeaderText = "Created By" });

                foreach (var item in cols.columns)
                {
                    item.AutoFit = true;
                    item.Width = "10%";
                }

                GridModel.Columns = cols.columns;
            }
            catch (Exception ex)
            {
                logger.Error("InvoicePreviewController - ConvertGridObject - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            
            return GridModel;
        }

        public ActionResult PdfExport(string gridModel,string vendorname)
        {
            List<productVendorAllList> lstProducts = new List<productVendorAllList>();
            PdfDocument doc = new PdfDocument();
            GridPdfExport exp = new GridPdfExport();

            try
            {
                lstProducts = _InvoicePreviewRepository.productVendorAllList(vendorname);

                doc.PageSettings.Orientation = PdfPageOrientation.Landscape;
                doc.PageSettings.Size = PdfPageSize.A3;


                exp.Theme = "flat-saffron";
                exp.FileName = "Product.pdf";
                exp.PdfDocument = doc;
            }
            catch (Exception ex)
            {
                logger.Error("InvoicePreviewController - PdfExport - " + DateTime.Now + " - " + ex.Message.ToString());
            }

            Syncfusion.EJ2.Grids.Grid gridProperty = ConvertGridObject(gridModel, lstProducts);
            return exp.PdfExport<productVendorAllList>(gridProperty, lstProducts);
        }

    }
}