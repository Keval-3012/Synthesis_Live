using ClosedXML.Excel;
using EntityModels.Models;
using ExcelDataReader;
using Microsoft.AspNetCore.Http;
using NLog;
using Repository;
using Repository.IRepository;
using Syncfusion.EJ2.Base;
using Syncfusion.EJ2.GridExport;
using Syncfusion.Pdf;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace SysnthesisRepo.Controllers
{
    public class ProductMappingsController : Controller
    {
        private readonly IProductMappingsRepository _ProductMappingsRepository;
        private readonly ISynthesisApiRepository _SynthesisApiRepository;
        private readonly ICommonRepository _CommonRepository;
        Logger logger = LogManager.GetCurrentClassLogger();
        public ProductMappingsController()
        {
            this._ProductMappingsRepository = new ProductMappingsRepository(new DBContext());
            this._SynthesisApiRepository = new SynthesisApiRepository();
            this._CommonRepository = new CommonRepository(new DBContext());
        }

        protected static string StatusMessage = "";
        protected static Array Arr;
        protected static bool IsArray;
        protected static IEnumerable BindData;
        protected static bool IsEdit = false;
        protected static int TotalDataCount;
        protected static string success = null;
        protected static string Error = null;

        /// <summary>
        /// This method is return view of Item library
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "Administrator,ViewItemsLibrary")]
        public ActionResult ImportProductExcel()
        {
            ViewBag.Title = "Items Library - Synthesis";
            _CommonRepository.LogEntries();     //Harsh's code
            return View();
        }

        /// <summary>
        /// This method is Import Product Excel
        /// </summary>
        /// <param name="postedFile"></param>
        /// <returns></returns>
        /// old code
        //[HttpPost]

        //public async Task<ActionResult> ImportProductExcel(HttpPostedFileBase postedFile)
        //{
        //    try
        //    {
        //        string JSONresult = "Done";
        //        string filePath = string.Empty;
        //        if (postedFile != null)
        //        {
        //            //Create a Folder.
        //            string path = Server.MapPath("~/Userfiles/ProductFiles");
        //            if (!Directory.Exists(path))
        //            {
        //                Directory.CreateDirectory(path);
        //            }

        //            filePath = path + System.IO.Path.GetFileName(postedFile.FileName);

        //            ViewBag.FileNames = postedFile.FileName;
        //            postedFile.SaveAs(filePath);

        //            //Read the connection string for the Excel file.
        //            //string conString = this.Configuration.GetConnectionString("ExcelConString");                    
        //            string extension = System.IO.Path.GetExtension(postedFile.FileName);
        //            DataTable dt = new DataTable();
        //            try
        //            {
        //                using (XLWorkbook workBook = new XLWorkbook(filePath))
        //                {
        //                    //Read the first Sheet from Excel file.
        //                    IXLWorksheet workSheet = workBook.Worksheet(1);

        //                    //Create a new DataTable.


        //                    //Loop through the Worksheet rows.
        //                    bool firstRow = true;
        //                    foreach (IXLRow row in workSheet.Rows())
        //                    {
        //                        //Use the first row to add columns to DataTable.
        //                        if (firstRow)
        //                        {
        //                            foreach (IXLCell cell in row.Cells())
        //                            {

        //                                dt.Columns.Add(cell.Value.ToString());
        //                            }
        //                            firstRow = false;
        //                        }
        //                        else
        //                        {
        //                            //Add rows to DataTable.

        //                            int i = 0;
        //                            //foreach (IXLCell cell in row.CellCoun)
        //                            bool IsNotNull = false;
        //                            for (int j = 1; j < row.CellCount(); j++)
        //                            {
        //                                IXLCell cell = row.Cell(j);
        //                                if (cell.Value.ToString() != "")
        //                                {
        //                                    IsNotNull = true;
        //                                    dt.Rows.Add();
        //                                    break;
        //                                }
        //                            }

        //                            if (IsNotNull == true)
        //                            {
        //                                for (int j = 1; j < row.CellCount(); j++)
        //                                {
        //                                    if (dt.Columns.Count >= j)
        //                                    {
        //                                        IXLCell cell = row.Cell(j);

        //                                        dt.Rows[dt.Rows.Count - 1][i] = cell.Value.ToString();


        //                                        i++;
        //                                    }
        //                                    else
        //                                    {
        //                                        break;
        //                                    }
        //                                }
        //                            }
        //                            else
        //                            {
        //                                break;
        //                            }
        //                        }
        //                    }
        //                }
        //            }
        //            catch (Exception ex)
        //            {
        //                logger.Error("ProductMappingsController - ImportProductExcel - " + DateTime.Now + " - " + ex.Message.ToString());
        //                return Content("<script language='javascript' type='text/javascript'>alert('" + ex.Message + "');</script>");
        //            }

        //            //***--Delete Empty row from Datatable --***//
        //            dt = dt.Rows.Cast<DataRow>().Where(row => !row.ItemArray.All(f => f is DBNull)).CopyToDataTable();
        //            DataView view = new DataView(dt);

        //            try
        //            {
        //                //This class is get Product List
        //                Products Pro = _ProductMappingsRepository.GetProduct();
        //                var Synthesis = "";
        //                int SynNo = 0;
        //                if (Pro != null)
        //                {
        //                    Synthesis = Pro.SynthesisId.Split('-')[1];
        //                    SynNo = Convert.ToInt32(Synthesis) + 1;
        //                }
        //                else
        //                {
        //                    SynNo = SynNo + 1;
        //                }

        //                foreach (DataRow dr in dt.Rows)
        //                {
        //                    Products products = new Products();

        //                    if (dt.Columns.Contains("UPCCode"))
        //                    {
        //                        products.UPCCode = dr["UPCCode"].ToString().Trim();
        //                    }
        //                    if (dt.Columns.Contains("ItemNo"))
        //                    {
        //                        products.ItemNo = dr["ItemNo"].ToString().Trim();
        //                    }
        //                    if (dt.Columns.Contains("Description") && dr["Description"] != null)
        //                    {
        //                        products.Description = dr["Description"].ToString().Trim();
        //                    }
        //                    if (dt.Columns.Contains("Brand") && dr["Brand"] != null)
        //                    {
        //                        products.Brand = dr["Brand"].ToString().Trim();
        //                    }
        //                    if (dt.Columns.Contains("Size") && dr["Size"] != null)
        //                    {
        //                        products.Size = dr["Size"].ToString().Trim();
        //                    }
        //                    if (dt.Columns.Contains("DepartmentNumber") && dr["DepartmentNumber"] != null)
        //                    {
        //                        products.DepartmentNumber = Convert.ToInt32(dr["DepartmentNumber"]);
        //                    }

        //                    products.Departments = null;
        //                    products.DateCreated = DateTime.Now;
        //                    //if (dt.Columns.Contains("Department") && dr["Department"] != null)
        //                    //{
        //                    //    products.Departments = dr["Department"].ToString();
        //                    //}
        //                    //if (!db.products.Any(s => s.UPCCode == products.UPCCode || (s.ItemNo!=""&&s.ItemNo == products.ItemNo)))
        //                    //{
        //                    //This class is get Count of Product
        //                    var PVCount = await _ProductMappingsRepository.GetCountProduct(products);
        //                    if (PVCount == 0)
        //                    {
        //                        ////This class is Add Product data
        //                         products.SynthesisId = "WM-" + SynNo.ToString().PadLeft(9, '0');
        //                        _ProductMappingsRepository.ProductAdd(products);
        //                        SynNo += 1;
        //                    }
        //                    else
        //                    {
        //                        _ProductMappingsRepository.ProductUpdate(products);
        //                    }
        //                }
        //            }
        //            catch (Exception ex)
        //            {
        //                logger.Error("ProductMappingsController - ImportProductExcel - " + DateTime.Now + " - " + ex.Message.ToString());
        //                return RedirectToAction("ImportProductExcel", "ProductMappings");
        //            }
        //            return RedirectToAction("ImportProductExcel", "ProductMappings");
        //        }
        //    }
        //    catch (Exception Ex)
        //    {
        //        logger.Error("ProductMappingsController - ImportProductExcel - " + DateTime.Now + " - " + Ex.Message.ToString());
        //    }
        //    return RedirectToAction("ImportProductExcel", "ProductMappings");
        //}

        [HttpPost]
        public async Task<ActionResult> ImportProductExcel(HttpPostedFileBase postedFile)
        {
            var flag = true;
            try
            {
                if (flag == true)
                {
                    if (postedFile != null)
                    {
                        string extension = Path.GetExtension(postedFile.FileName);
                        if (extension == ".xlsx" || extension == ".xlsx")
                        {
                            //Create a Folder.
                            string path = Server.MapPath("~/Userfiles/ProductFiles");
                            if (!Directory.Exists(path))
                            {
                                Directory.CreateDirectory(path);
                            }
                            else
                            {
                                string[] Files = Directory.GetFiles(path);
                                foreach (string file in Files)
                                {
                                    System.IO.File.Delete(file);
                                    //Directory.Delete(path);
                                }
                                //Directory.Delete(path);
                            }

                            try
                            {
                                //Save the uploaded Excel file.
                                string fileName = Path.GetFileName(postedFile.FileName);
                                string filePath = Path.Combine(path, fileName);
                                using (FileStream stream = new FileStream(filePath, FileMode.Create))
                                {
                                    postedFile.InputStream.CopyTo(stream);
                                }
                                FileStream streams = System.IO.File.Open(filePath, FileMode.Open, FileAccess.Read);

                                IExcelDataReader excelReader;
                                excelReader = ExcelReaderFactory.CreateOpenXmlReader(streams);//For .xls file CreateBinaryReader is used and .xlsx file CreateOpenXmlReader is used
                                DataSet result = excelReader.AsDataSet(new ExcelDataSetConfiguration()
                                {
                                    ConfigureDataTable = (_) => new ExcelDataTableConfiguration()
                                    {
                                        UseHeaderRow = true
                                    }
                                });
                                DataTableCollection table = result.Tables;
                                DataTable dtRowData = table[0];
                                excelReader.Close();
                                
                                DataView view = new DataView(dtRowData);

                                InsertBulkCopy_DirectBulkDB(dtRowData, "TempTable_Product");
                                //_ProductMappingsRepository.CheckProductUpdate();


                                return RedirectToAction("ImportProductExcel", "ProductMappings");
                            }
                            catch (Exception ex)
                            {
                                logger.Error("ProductMappingsController - ImportProductExcel - " + DateTime.Now + " - " + ex.Message.ToString());
                                return RedirectToAction("ImportProductExcel", "ProductMappings");
                            }
                        }
                        else
                        {
                            return RedirectToAction("ImportProductExcel", "ProductMappings");
                        }
                    }
                    else
                    {
                        return RedirectToAction("ImportProductExcel", "ProductMappings");

                    }

                }
            }

            catch (Exception ex)
            {
                logger.Error("ProductMappingsController - ImportProductExcel - " + DateTime.Now + " - " + ex.Message.ToString());
                return RedirectToAction("ImportProductExcel", "ProductMappings");
            }
            return RedirectToAction("ImportProductExcel", "ProductMappings");
        }

        /// <summary>
        /// This method is Product Import Vendor Excel
        /// </summary>
        /// <param name="postedFile"></param>
        /// <param name="Vendors"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> ImportProductVendorExcel(HttpPostedFileBase postedFile, string Vendors)
        {
            string message = "";
            try
            {
                if (!String.IsNullOrEmpty(Vendors))
                {
                    string JSONresult = "Done";
                    string filePath = string.Empty;
                    if (postedFile != null)
                    {
                        //Create a Folder.
                        string path = Server.MapPath("~/Userfiles") + "/ProductVendorFiles";
                        if (!Directory.Exists(path))
                        {
                            Directory.CreateDirectory(path);
                        }

                        filePath = path + "/" + System.IO.Path.GetFileName(postedFile.FileName);

                        ViewBag.FileNames = postedFile.FileName;
                        //postedFile.SaveAs( filePath);
                        postedFile.SaveAs(filePath.Replace(".xlsx", ".csv"));

                        //Read the connection string for the Excel file.                                                
                        string extension = System.IO.Path.GetExtension(postedFile.FileName);
                        DataTable dt = new DataTable();
                        DataRow dr = null;

                        int i = 0;
                        using (var rd = new StreamReader(filePath.Replace(".xlsx", ".csv")))
                        {
                            while (!rd.EndOfStream)
                            {
                                var splits = rd.ReadLine().Split(',');
                                if (i == 0)
                                {
                                    foreach (string str in splits)
                                        dt.Columns.Add(new DataColumn(str.Trim()));
                                    dt.Columns.Add("Vendors");
                                    i += 1;
                                }
                                else
                                {
                                    dr = dt.NewRow();
                                    dr[0] = splits[0].ToString().Trim();
                                    dr[1] = splits[1].ToString().Trim();
                                    if (splits.Length > 5)
                                    {
                                        dr[5] = splits[splits.Length - 1].ToString().Replace("?", "").Trim();
                                        dr[5] = (dr[5].ToString() == "" ? "0" : dr[5].ToString());
                                    }
                                    if (splits.Length > 4)
                                    {
                                        dr[4] = splits[splits.Length - 2].ToString().Trim();
                                    }
                                    if (splits.Length > 3)
                                    {
                                        dr[3] = splits[splits.Length - 3].ToString().Trim();
                                    }
                                    if (splits.Length > 2)
                                    {
                                        for (int s = 2; s < splits.Length - 3; s++)
                                        {
                                            if (dr[2].ToString() == "")
                                                dr[2] = splits[s].ToString().Trim();
                                            else
                                                dr[2] = dr[2].ToString() + "," + splits[s].ToString().Trim();
                                        }
                                        if (dr[2].ToString() == "")
                                            dr[2] = splits[2].ToString().Trim();
                                        else
                                            dr[2] = dr[2].ToString().Replace("\"", "");
                                    }
                                    dr["Vendors"] = Vendors;
                                    dt.Rows.Add(dr);
                                }
                            }
                        }
                        //}
                        dt = dt.Rows.Cast<DataRow>().Where(row => !row.ItemArray.All(f => f is DBNull)).CopyToDataTable();


                        BulkInsertToDataBase(ref dt);
                        try
                        {
                            //This class is Add Vendors data 
                            _ProductMappingsRepository.AddVendor(Vendors);
                        }
                        catch (Exception ex)
                        {
                            message = ex.Message;
                        }

                        return RedirectToAction("Index", "InvoicePreview");
                    }
                }
                else
                {
                    message = "Enter Vendor Name.";
                }
            }
            catch (Exception Ex)
            {
                logger.Error("ProductMappingLogController - ImportProductVendorExcel - " + DateTime.Now + " - " + Ex.Message.ToString());
            }
            return RedirectToAction("Index", "InvoicePreview", new { Message = message });
        }

        /// <summary>
        /// This class is Insert Bulk data into database
        /// </summary>
        /// <param name="dtItems"></param>
        private void BulkInsertToDataBase(ref DataTable dtItems)
        {
            try
            {
                string con = ConfigurationManager.ConnectionStrings["DBConn"].ConnectionString;
                //creating object of SqlBulkCopy  
                SqlBulkCopy objbulk = new SqlBulkCopy(con, SqlBulkCopyOptions.KeepNulls);
                objbulk.BatchSize = 500;
                //assigning Destination table name  
                objbulk.DestinationTableName = "Temp_VendorProducts";
                //Mapping Table column  


                objbulk.ColumnMappings.Add("ItemNo", "ItemNo");
                objbulk.ColumnMappings.Add("UPCCode", "UPCCode");
                objbulk.ColumnMappings.Add("Description", "Description");
                objbulk.ColumnMappings.Add("Brand", "Brand");
                objbulk.ColumnMappings.Add("Size", "Size");
                objbulk.ColumnMappings.Add("Price", "Price");
                objbulk.ColumnMappings.Add("Vendors", "Vendors");
                //inserting bulk Records into DataBase   
                objbulk.WriteToServer(dtItems);
            }
            catch (Exception ex)
            {
                logger.Error("ProductMappingLogController - BulkInsertToDataBase - " + DateTime.Now + " - " + ex.Message.ToString());
            }
        }

        /// <summary>
        /// This method is get Product master list
        /// </summary>
        /// <returns></returns>
        public ActionResult ProductMasterList()
        {
            List<Products> lstProducts = new List<Products>();
            //Get all Product master list
            lstProducts = _ProductMappingsRepository.ProductList();
            return View(lstProducts);
        }
        /// <summary>
        /// This method is get product data by Id
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public ActionResult GetProductsData(int ID)
        {
            List<Products> lstProducts = new List<Products>();
            //This class is get Product List by id
            lstProducts = _ProductMappingsRepository.getProductListByID(ID);
            return Json(lstProducts, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// This method is Get search Products data
        /// </summary>
        /// <param name="SearchVal"></param>
        /// <param name="SearchOn"></param>
        /// <returns></returns>
        public ActionResult GetSearchProductsData(string SearchVal, string SearchOn)
        {
            List<Products> lstProducts = new List<Products>();
            try
            {
                if (SearchVal != null && SearchVal != "")
                {
                    //Get serach Products data
                    lstProducts = _ProductMappingsRepository.getSearchdata(SearchVal);
                }
            }
            catch (Exception ex)
            {
                logger.Error("ProductMappingLogController - GetSearchProductsData - " + DateTime.Now + " - " + ex.Message.ToString());
            }

            return Json(lstProducts, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// This method is Assign product Invoice
        /// </summary>
        /// <param name="selectedval"></param>
        /// <param name="SearchOn"></param>
        /// <param name="VendorN"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AssignProduct(string selectedval, string SearchOn, string VendorN)
        {
            try
            {
                if (selectedval != "" && selectedval != null)
                {
                    if (selectedval.Contains("_"))
                    {
                        //This db class is Update Product Invoice data
                        _ProductMappingsRepository.UpdateProductInvoice(selectedval);
                    }
                }
                else
                {
                    return RedirectToAction("Index", "InvoicePreview", new { Vendor = VendorN });
                }
            }
            catch (Exception Ex)
            {
                logger.Error("ProductMappingLogController - AssignProduct - " + DateTime.Now + " - " + Ex.Message.ToString());
            }
            return RedirectToAction("Index", "InvoicePreview", new { Vendor = VendorN });
        }


        /// <summary>
        /// This method is Remove Product mapping
        /// </summary>
        /// <param name="mappedProductId"></param>
        /// <param name="VendorNa"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<ActionResult> RemoveProductMapping(string mappedProductId, string VendorNa)
        {
            try
            {
                if (mappedProductId != "")
                {
                    //This db class is Modify Product Invoice data
                    _ProductMappingsRepository.ModifyProductMapping(mappedProductId);
                }

            }
            catch (Exception Ex)
            {
                logger.Error("ProductMappingLogController - RemoveProductMapping - " + DateTime.Now + " - " + Ex.Message.ToString());
            }
            return RedirectToAction("Index", "InvoicePreview", new { Vendor = VendorNa });
        }


        /// <summary>
        /// This method is get product master data by id
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public ActionResult GetProductMasterData(int ID)
        {
            List<Products> lstProducts = new List<Products>();
            //This class is get Product Master data 
            lstProducts = _ProductMappingsRepository.GetProductMasterData(ID);
            return Json(lstProducts, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// This method is Update Keyword
        /// </summary>
        /// <param name="ProductID"></param>
        /// <param name="KeyWord"></param>
        /// <returns></returns>
        public ActionResult UpdateKeyWord(int ProductID, string KeyWord)
        {
            //This db class is Update Keyword
            _ProductMappingsRepository.updateKeyWord(ProductID, KeyWord);
            string response = "Success";
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        ///This method is grid of Item Library Department 
        /// </summary>
        /// <returns></returns>
        public ActionResult Grid()
        {
            //Get grid of Item Library department
            ViewBag.Department = _ProductMappingsRepository.GetItemLibraryDepartment().Select(s => new { s.ItemLibraryDepartmentId, DepartmentName = s.ItemLibraryDepartmentName }).ToList();
            return View();
        }
        /// <summary>
        /// This method is Get Product for url
        /// </summary>
        /// <param name="dm"></param>
        /// <param name="LinkedItem"></param>
        /// <returns></returns>
        public ActionResult UrlDatasource(DataManagerRequest dm, string LinkedItem)
        {
            //This db class is Get data product for Url
            List<ProductsList> lstProducts = _ProductMappingsRepository.GetDataProductForUrl();
            IEnumerable DataSource;
            if (LinkedItem == "true")
            {
                DataSource = lstProducts.Where(x => x.ProductVendorId != 0).ToList();
            }
            else
            {
                DataSource = lstProducts;
            }
            DataOperations operation = new DataOperations();
            int count = 0;
            try
            {
                if (dm.Search != null && dm.Search.Count > 0)
                {
                    string search = dm.Search[0].Key.ToString().Trim().ToLower();
                    DataSource = lstProducts.ToList().Where(x => (x.Brand != null && x.Brand.ToLower().Contains(search)) || (x.Departments != null && x.Departments.ToLower().Contains(search)) || (x.Description != null && x.Description.ToLower().Contains(search)) || (x.ItemNo != null && x.ItemNo.ToLower().Contains(search))
                   || (x.SynthesisId != null && x.SynthesisId.ToLower().Contains(search)) || (x.KeyWord != null && x.KeyWord.ToLower().Contains(search)) || (x.Size != null && x.Size.ToLower().Contains(search)) || (x.UPCCode != null && x.UPCCode.ToLower().Contains(search))).ToList();

                    //DataSource = operation.PerformSearching(DataSource, dm.Search);  //Search
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
                logger.Error("ProductMappingLogController - UrlDatasource - " + DateTime.Now + " - " + ex.Message.ToString());
            }

            return dm.RequiresCounts ? Json(new { result = DataSource, count = count }) : Json(DataSource);
        }
        /// <summary>
        /// This method is Insert product data
        /// </summary>
        /// <param name="products"></param>
        /// <returns></returns>
        public ActionResult Insert(CRUDModel<Products> products)
        {
            try
            {
                //This db class is Insert Product data
                _ProductMappingsRepository.InsertProduct(products);
                success = "Successfully Added!";
            }
            catch (Exception ex)
            {
                logger.Error("ProductMappingLogController - Insert - " + DateTime.Now + " - " + ex.Message.ToString());
            }

            return Json(new { data = products.Value, success = success, Error = Error });
        }
        /// <summary>
        /// This method is Update Product data
        /// </summary>
        /// <param name="products"></param>
        /// <returns></returns>
        public ActionResult Update(CRUDModel<Products> products)
        {

            try
            {
                //This db class is Update Product data
                _ProductMappingsRepository.UpdateProduct(products);
                success = "Successfully Updated!";
            }
            catch (Exception ex)
            {
                logger.Error("ProductMappingLogController - Update - " + DateTime.Now + " - " + ex.Message.ToString());
            }

            return Json(new { data = products.Value, success = success, Error = Error });
        }
        /// <summary>
        /// This method is Delete product data
        /// </summary>
        /// <param name="products"></param>
        /// <returns></returns>
        public ActionResult Remove(CRUDModel<Products> products)
        {
            Products products1 = new Products();
            try
            {
                //This db class is Remove Product data
                _ProductMappingsRepository.RemoveProduct(products);
                success = "Successfully Deleted!";
            }
            catch (Exception ex)
            {
                logger.Error("ProductMappingLogController - Remove - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return Json(new { data = products.Deleted, success = success, Error = Error });
        }

        /// <summary>
        /// This method is get synthesis Id
        /// </summary>
        /// <returns></returns>
        public ActionResult GetSynthesisId()
        {
            try
            {
                //This db class is Get synthesis ID 
                Products Pro = _ProductMappingsRepository.getSynthesisId();
                var Synthesis = Pro.SynthesisId.Split('-')[1];
                int SynNo = Convert.ToInt32(Synthesis) + 1;

                var SynthesisId = "WM-" + SynNo.ToString().PadLeft(9, '0');
                return Json(SynthesisId, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                logger.Error("ProductMappingLogController - GetSynthesisId - " + DateTime.Now + " - " + ex.Message.ToString());
                return Json("", JsonRequestBehavior.AllowGet);
            }

        }

        /// <summary>
        /// This method is Get Prodcut Details by SynthesisId
        /// </summary>
        /// <param name="SynthesisId"></param>
        /// <returns></returns>
        public ActionResult ProductDetail(string SynthesisId)
        {
            ProductPriceModel obj = new ProductPriceModel();
            try
            {
                //Get all StoreID list
                ViewBag.StoreId = new SelectList((from dataUser in _ProductMappingsRepository.GetStoreId()
                                                  select new StoreQBSyncModel
                                                  {
                                                      StoreId = dataUser.StoreId,
                                                      StoreName = dataUser.NickName

                                                  }).ToList(), "StoreId", "StoreName");
                //Get all StoreID list
                ViewBag.StoreIds = new SelectList((from dataUser in _ProductMappingsRepository.GetStoreId()
                                                   select new StoreQBSyncModel
                                                   {
                                                       StoreId = dataUser.StoreId,
                                                       StoreName = dataUser.NickName

                                                   }).ToList(), "StoreId", "StoreName");

                //This class is Get List of Product Price
                var list1 = _ProductMappingsRepository.ListProductPrice(SynthesisId);
                //Get Store Count
                var list2 = _ProductMappingsRepository.ListStoreCount(SynthesisId);
                //This class is Get Invoices
                var list3 = (from In in _ProductMappingsRepository.GetInvoices()
                                 //get Invoice Product
                             join Ip in _ProductMappingsRepository.GetInvoiceProduct() on In.InvoiceId equals Ip.InvoiceId
                             //Get storeid by storeid
                             join sm in _ProductMappingsRepository.GetStoreId() on In.StoreId equals sm.StoreId
                             //This class is get Product List
                             join p in _ProductMappingsRepository.GetProductList() on Ip.ProductId equals p.ProductId
                             where p.SynthesisId.Trim().Equals(SynthesisId)
                             select new
                             {
                                 Ip.Qty,
                                 sm.Name
                             }).ToList();

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
                if (list3 != null)
                {
                    foreach (var dr in list3)
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
                logger.Error("ProductMappingLogController - ProductDetail - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return View(obj);
        }

        public async Task<ActionResult> ExcelExport(string gridModel)
        {
            List<ProductsList> lstProducts = new List<ProductsList>();
            GridExcelExport exp = new GridExcelExport();

            try
            {
                lstProducts = _ProductMappingsRepository.GetDataProductForUrl();
                exp.FileName = "ItemLibrary.xlsx";
            }
            catch (Exception ex)
            {
                logger.Error("ProductMappingsController - ExcelExport - " + DateTime.Now + " - " + ex.Message.ToString());
            }

            
            Syncfusion.EJ2.Grids.Grid gridProperty = ConvertGridObject(gridModel,lstProducts);
            return exp.ExcelExport<ProductsList>(gridProperty, lstProducts);

        }

        private Syncfusion.EJ2.Grids.Grid ConvertGridObject(string gridProperty, List<ProductsList> data)
        {
            Syncfusion.EJ2.Grids.Grid GridModel = (Syncfusion.EJ2.Grids.Grid)Newtonsoft.Json.JsonConvert.DeserializeObject(gridProperty, typeof(Syncfusion.EJ2.Grids.Grid));

            try
            {
                GridColumnModel cols = (GridColumnModel)Newtonsoft.Json.JsonConvert.DeserializeObject("{\"allowGrouping\":false,\"allowPaging\":false,\"pageSettings\":{\"currentPage\":1,\"pageCount\":2,\"pageSize\":100},\"sortSettings\":{},\"allowPdfExport\":true,\"allowExcelExport\":true,\"aggregates\":[],\"filterSettings\":{\"immediateModeDelay\":1500,\"type\":\"CheckBox\"},\"groupSettings\":{\"columns\":[],\"enableLazyLoading\":false,\"disablePageWiseAggregates\":false},\"columns\":[],\"locale\":\"en-US\",\"searchSettings\":{\"key\":\"\",\"fields\":[]}}", typeof(GridColumnModel));
                cols.columns.Add(new Syncfusion.EJ2.Grids.GridColumn { Field = "UPCCode", HeaderText = "UPCCode" });
                cols.columns.Add(new Syncfusion.EJ2.Grids.GridColumn { Field = "ItemNo", HeaderText = "Item No" });
                cols.columns.Add(new Syncfusion.EJ2.Grids.GridColumn { Field = "SynthesisId", HeaderText = "Synthesis Id" });
                cols.columns.Add(new Syncfusion.EJ2.Grids.GridColumn { Field = "Description", HeaderText = "Name" });
                cols.columns.Add(new Syncfusion.EJ2.Grids.GridColumn { Field = "Brand", HeaderText = "Brand" });
                cols.columns.Add(new Syncfusion.EJ2.Grids.GridColumn { Field = "Size", HeaderText = "Size" });
                cols.columns.Add(new Syncfusion.EJ2.Grids.GridColumn { Field = "Departments", HeaderText = "Department" });
                cols.columns.Add(new Syncfusion.EJ2.Grids.GridColumn { Field = "DepartmentNumber", HeaderText = "Department No" });
                cols.columns.Add(new Syncfusion.EJ2.Grids.GridColumn { Field = "KeyWord", HeaderText = "KeyWord" });

                foreach (var item in cols.columns)
                {
                    item.AutoFit = true;
                    item.Width = "10%";
                }
                foreach (var item in data)
                {
                    foreach (var col in cols.columns)
                    {
                        if (col.Field == "ItemNo")
                        {
                            int maxLineLength = 20;

                            if (!string.IsNullOrEmpty(item.ItemNo))
                            {
                                StringBuilder memoBuilder = new StringBuilder();
                                for (int i = 0; i < item.ItemNo.Length; i += maxLineLength)
                                {
                                    int length = Math.Min(maxLineLength, item.ItemNo.Length - i);
                                    memoBuilder.AppendLine(item.ItemNo.Substring(i, length));
                                }
                                item.ItemNo = memoBuilder.ToString().Trim();
                            }
                        }
                        else if (col.Field == "Description")
                        {
                            int maxLineLength = 25;

                            if (!string.IsNullOrEmpty(item.Description))
                            {
                                StringBuilder memoBuilder = new StringBuilder();
                                for (int i = 0; i < item.Description.Length; i += maxLineLength)
                                {
                                    int length = Math.Min(maxLineLength, item.Description.Length - i);
                                    memoBuilder.AppendLine(item.Description.Substring(i, length));
                                }
                                item.Description = memoBuilder.ToString().Trim();
                            }
                        }
                        else if (col.Field == "Brand")
                        {
                            int maxLineLength = 20;

                            if (!string.IsNullOrEmpty(item.Brand))
                            {
                                StringBuilder memoBuilder = new StringBuilder();
                                for (int i = 0; i < item.Brand.Length; i += maxLineLength)
                                {
                                    int length = Math.Min(maxLineLength, item.Brand.Length - i);
                                    memoBuilder.AppendLine(item.Brand.Substring(i, length));
                                }
                                item.Brand = memoBuilder.ToString().Trim();
                            }
                        }
                        else if (col.Field == "Size")
                        {
                            int maxLineLength = 20;

                            if (!string.IsNullOrEmpty(item.Size))
                            {
                                StringBuilder memoBuilder = new StringBuilder();
                                for (int i = 0; i < item.Size.Length; i += maxLineLength)
                                {
                                    int length = Math.Min(maxLineLength, item.Size.Length - i);
                                    memoBuilder.AppendLine(item.Size.Substring(i, length));
                                }
                                item.Size = memoBuilder.ToString().Trim();
                            }
                        }
                        else if (col.Field == "Departments")
                        {
                            int maxLineLength = 25;

                            if (!string.IsNullOrEmpty(item.Departments))
                            {
                                StringBuilder memoBuilder = new StringBuilder();
                                for (int i = 0; i < item.Departments.Length; i += maxLineLength)
                                {
                                    int length = Math.Min(maxLineLength, item.Departments.Length - i);
                                    memoBuilder.AppendLine(item.Departments.Substring(i, length));
                                }
                                item.Departments = memoBuilder.ToString().Trim();
                            }
                        }
                    }
                }

                GridModel.Columns = cols.columns;
            }
            catch (Exception ex)
            {
                logger.Error("ProductMappingsController - ConvertGridObject - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            
            return GridModel;
        }

        public ActionResult PdfExport(string gridModel)
        {
            List<ProductsList> lstProducts = new List<ProductsList>();
            PdfDocument doc = new PdfDocument();
            GridPdfExport exp = new GridPdfExport();


            try
            {
                lstProducts = _ProductMappingsRepository.GetDataProductForUrl();

                doc.PageSettings.Orientation = PdfPageOrientation.Landscape;
                doc.PageSettings.Size = PdfPageSize.A3;

                exp.Theme = "flat-saffron";
                exp.FileName = "ItemLibrary.pdf";
                exp.PdfDocument = doc;
            }
            catch (Exception ex)
            {
                logger.Error("ProductMappingsController - PdfExport - " + DateTime.Now + " - " + ex.Message.ToString());
            }

            Syncfusion.EJ2.Grids.Grid gridProperty = ConvertGridObject(gridModel, lstProducts);
            return exp.PdfExport<ProductsList>(gridProperty, lstProducts);
        }

        public Boolean InsertBulkCopy_DirectBulkDB(DataTable dtTemp, string DesTable)
        {
            Boolean IsDone = false;
            SqlBulkCopy oCopy = new SqlBulkCopy(ConfigurationManager.ConnectionStrings["DBConn"].ConnectionString , SqlBulkCopyOptions.KeepNulls);
            try
            {
                oCopy.BatchSize = 500;
                oCopy.BulkCopyTimeout = 9999;
                oCopy.DestinationTableName = DesTable;
                for (int i = 0; i < dtTemp.Columns.Count; i++)
                {
                    oCopy.ColumnMappings.Add(new SqlBulkCopyColumnMapping(dtTemp.Columns[i].Caption, dtTemp.Columns[i].Caption));
                }
                oCopy.WriteToServer(dtTemp);
                IsDone = true;
            }
            catch (Exception ex)
            {
                oCopy.Close();
                oCopy = null;
                IsDone = false;
            }
            finally
            {
                oCopy.Close();
                oCopy = null;
            }
            return IsDone;
        }
    }
}