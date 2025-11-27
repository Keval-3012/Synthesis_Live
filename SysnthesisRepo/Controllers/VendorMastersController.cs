using Aspose.Pdf.Operators;
using EntityModels.Models;
using NLog;
using Repository;
using Repository.IRepository;
using Syncfusion.EJ2.Base;
using Syncfusion.EJ2.GridExport;
using Syncfusion.Pdf;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Utility;
using VendorMaster = EntityModels.Models.VendorMaster;

namespace SysnthesisRepo.Controllers
{
    public class VendorMastersController : Controller
    {
        private readonly IVendorMasterRepository _vendorMasterRepository;
        private readonly IMastersBindRepository _MastersBindRepository;
        private readonly IUserActivityLogRepository _ActivityLogRepository;
        private readonly ICommonRepository _CommonRepository;
        private readonly ISynthesisApiRepository _SynthesisApiRepository;
        private readonly IQBRepository _qBRepository;
        Logger logger = LogManager.GetCurrentClassLogger();

        public string Message { get; set; }
        public VendorMastersController()
        {
            this._vendorMasterRepository = new VendorMasterRepository(new DBContext());
            this._MastersBindRepository = new MastersBindRepository(new DBContext());
            this._ActivityLogRepository = new UserActivityLogRepository(new DBContext());
            this._CommonRepository = new CommonRepository(new DBContext());
            this._SynthesisApiRepository = new SynthesisApiRepository();
            this._qBRepository = new QBRepository(new DBContext());
        }
        SynthesisQBOnline.BAL.QBResponse objResponse = new SynthesisQBOnline.BAL.QBResponse();
        // GET: VendorMasters
        /// <summary>
        /// This method return view of Vendors.
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            ViewBag.Title = "Vendors - Synthesis";
            _CommonRepository.LogEntries();     //Harsh's code
            ViewBag.VendorStatus = _CommonRepository.GetMessageValue("VMSU", "Vendor Status Successfully Updated.");
            return View();
        }

        /// <summary>
        /// This method is url Data Source for vendor master.
        /// </summary>
        /// <param name="dm"></param>
        /// <returns></returns>
        public ActionResult UrlDatasource(DataManagerRequest dm)
        {
            int? StoreIds = 0;
            if (Session["storeid"] != null)
            {
                StoreIds = Convert.ToInt32(Convert.ToString(Session["storeid"]));
            }
            ViewBag.StoreId = StoreIds;
            List<VenodrMasterSelect> vendorMasters = new List<VenodrMasterSelect>();
            //Using this class Get Vendor Master data
            vendorMasters = _vendorMasterRepository.GetVendorMastersDataSP(StoreIds);
            IEnumerable DataSource = vendorMasters;
            DataOperations operation = new DataOperations();
            int count = 0;
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
                count = DataSource.Cast<VenodrMasterSelect>().Count();
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
                logger.Error("VendorMastersController - Index - " + DateTime.Now + " - " + ex.Message.ToString());
            }
           

            return dm.RequiresCounts ? Json(new { result = DataSource, count = count }) : Json(DataSource);
        }

        /// <summary>
        /// This method is return partial view of edit.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public ActionResult Editpartial(VendorMaster value)
        {
            //Get vendor by VendorId
            VendorMaster vendorMaster = _vendorMasterRepository.getVendor(value.VendorId);
            try
            {
                //Get All State master.
                ViewBag.State = _MastersBindRepository.GetStateMasters().Select(s => s.StateCode).ToList();
                ViewBag.State.Insert(0, "Select State");
                string[] commomtype = new string[] { "Expense", "Cost of Goods Sold", "Fixed Asset" };
                //Get All Account Type Master
                var AccountTypeId = _MastersBindRepository.GetAccountTypeMasters().Where(s => commomtype.Contains(s.CommonType)).Select(s => s.AccountTypeId).ToList();
                //Get All Vendor Departments Ids
                ViewBag.DepartmentID = _vendorMasterRepository.GetVendorDepartmentIds(vendorMaster.VendorId, vendorMaster.StoreId.Value);
                //Get All DepartMent Masrter
                ViewBag.MultiDepartmentId = _MastersBindRepository.GetDepartmentMasters(vendorMaster.StoreId.Value).Where(s => AccountTypeId.Contains(s.AccountTypeId)).Select(s => new { s.DepartmentId, DepartmentName = s.DepartmentName }).OrderBy(o => o.DepartmentName).ToList();
            }
            catch (Exception ex)
            {
                logger.Error("VendorMastersController - EditPartial - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return PartialView("_DialogEditpartial", vendorMaster);
        }

        /// <summary>
        ///  This method is return partial view of Add.
        /// </summary>
        /// <returns></returns>
        public ActionResult AddPartial()
        {
            try
            {
                //Get All State master.
                ViewBag.State = _MastersBindRepository.GetStateMasters().Select(s => s.StateCode).ToList();
                ViewBag.State.Insert(0, "Select State");
                var UserName = System.Web.HttpContext.Current.User.Identity.Name;
                //Using this db class get StoreList with role Wise.
                var StoreList = _CommonRepository.GetStoreList_RoleWise(10, "CreateQBVendor", UserName).Select(s => s.StoreId);
                //Get Store Masters By storeid
                ViewBag.MultiStoreId = _MastersBindRepository.GetStoreMasters().Where(s => StoreList.Contains(s.StoreId)).Select(s => new { s.StoreId, s.Name }).OrderBy(o => o.Name).ToList();
                
            }
            catch (Exception ex)
            {
                logger.Error("VendorMastersController - AddPartial - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return PartialView("_DialogAddpartial");
        }

        /// <summary>
        /// This method set Deaprtment
        /// </summary>
        /// <param name="Mult"></param>
        /// <returns></returns>
        public ActionResult SetDepartment(int[] Mult)
        {
            try
            {
                var Data = Mult.Select(s => s).ToList();
                string[] commomtype = new string[] { "Expense", "Cost of Goods Sold", "Fixed Asset" };
                //Get All Account Type Master
                var AccountTypeId = _MastersBindRepository.GetAccountTypeMasters().Where(s => commomtype.Contains(s.CommonType)).Select(s => s.AccountTypeId).ToList();
                //Get All DepartMent Masrter with selection of storeid,accountid and departmentId
                var Department = _MastersBindRepository.GetAllDepartmentMasters().Where(s => Data.Contains(s.StoreId.Value) && AccountTypeId.Contains(s.AccountTypeId)).Select(s => new { s.DepartmentId, DepartmentName = s.DepartmentName + "(" + s.StoreMasters.NickName + ")", s.StoreId }).OrderBy(o => o.StoreId).ToList();
                return Json(Department, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error("VendorMastersController - SetDepartment - " + DateTime.Now + " - " + ex.Message.ToString());
                return Json("", JsonRequestBehavior.AllowGet);
            }
           
        }

        /// <summary>
        /// This method is Insert Vendor details.
        /// </summary>
        /// <param name="VendorMaster"></param>
        /// <returns></returns>
        public ActionResult Insert(CRUDModel<VendorMaster> VendorMaster)
        {
            string PhoneNumber = null;
            string StoreName = "";
            string error = null;
            string Sucsses = null;
            try
            {
                //This db class is uesd for Insert vendor master.
                var Msg = _vendorMasterRepository.InsertVendor(VendorMaster);
                if (Msg != null)
                {
                    if (Msg == "success")
                    {
                        ActivityLog ActLog = new ActivityLog();
                        ActLog.Action = 1;
                        ActLog.Comment = "Vendor " + VendorMaster.Value.VendorId + "'>" + VendorMaster.Value.VendorName + "</a> Edited by " + UserModule.getUserFirstName() + " on " + AdminSiteConfiguration.GetDate(AdminSiteConfiguration.GetEasternTime(DateTime.Now).ToString());
                        //This  class is used for Activity Log Insert.
                        _ActivityLogRepository.ActivityLogInsert(ActLog);
                        //Sucsses = "Vendor Created Successfully..";
                        Sucsses = _CommonRepository.GetMessageValue("VMC", "Vendor Created Successfully..");
                    }
                    else
                    {
                        ActivityLog ActLog = new ActivityLog();
                        ActLog.Action = 1;
                        ActLog.Comment = "Vendor " + VendorMaster.Value.VendorId + "'>" + VendorMaster.Value.VendorName + "</a> Edited by " + UserModule.getUserFirstName() + " on " + AdminSiteConfiguration.GetDate(AdminSiteConfiguration.GetEasternTime(DateTime.Now).ToString());
                        //This  class is used for Activity Log Insert.
                        _ActivityLogRepository.ActivityLogInsert(ActLog);
                        //error = "Vendor Name Already Exist..";
                        error = _CommonRepository.GetMessageValue("VMCE", "Vendor Name Already Exist..");

                    }
                }               
            }
            catch (Exception ex)
            {
                logger.Error("VendorMastersController - Insert - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            VendorMaster.Value.Address = (!String.IsNullOrEmpty(VendorMaster.Value.Address) ? VendorMaster.Value.Address + "," : "")
                    + (!String.IsNullOrEmpty(VendorMaster.Value.Address) ? VendorMaster.Value.City + "," : "")
                    + (!String.IsNullOrEmpty(VendorMaster.Value.Address) ? VendorMaster.Value.State + ' ' : "") + VendorMaster.Value.PostalCode;
            VendorMaster.Value.Status = VendorMaster.Value.IsActive == true ? "Active" : "Inactive";
            //Get Store Masters By storeid
            VendorMaster.Value.StoreName = _MastersBindRepository.GetStoreMasters().Where(w => w.StoreId == VendorMaster.Value.StoreId).FirstOrDefault().NickName;
            return Json(new { data = VendorMaster.Value, success = Sucsses, Error = error });
        }

        /// <summary>
        ///  This method is Update Vendor details.
        /// </summary>
        /// <param name="VendorMaster"></param>
        /// <returns></returns>
        public ActionResult Update(CRUDModel<VendorMaster> VendorMaster)
        {

            string StoreName = "";
            string error = null;
            string Sucsses = null;

            try
            {
                //This db class is uesd for Update vendor master.
                var Msg = _vendorMasterRepository.UpdateVendor(VendorMaster);
                if (Msg != null)
                {
                    if (Msg == "success")
                    {
                        //Sucsses = "Vendor Updated Successfully..";
                        ActivityLog ActLog = new ActivityLog();
                        ActLog.Action = 2;
                        ActLog.Comment = "Vendor " + VendorMaster.Value.VendorId + "'>" + VendorMaster.Value.VendorName + "</a> Edited by " + UserModule.getUserFirstName() + " on " + AdminSiteConfiguration.GetDate(AdminSiteConfiguration.GetEasternTime(DateTime.Now).ToString());
                        //This  class is used for Activity Log Insert.
                        _ActivityLogRepository.ActivityLogInsert(ActLog);
                        Sucsses = _CommonRepository.GetMessageValue("VME", "Vendor Updated Successfully..");
                    }
                    else
                    {
                        error = "Vendor Name Already Exist..";
                        if (Msg == "error")
                        {
                            ActivityLog ActLog = new ActivityLog();
                            ActLog.Action = 2;
                            ActLog.Comment = "Vendor " + VendorMaster.Value.VendorId + "'>" + VendorMaster.Value.VendorName + "</a> Edited by " + UserModule.getUserFirstName() + " on " + AdminSiteConfiguration.GetDate(AdminSiteConfiguration.GetEasternTime(DateTime.Now).ToString());
                            //This  class is used for Activity Log Insert.
                            _ActivityLogRepository.ActivityLogInsert(ActLog);
                            //Sucsses = "Vendor Updated Successfully..";
                            Sucsses = _CommonRepository.GetMessageValue("VME", "Vendor Updated Successfully..");
                            return Json(new { data = VendorMaster.Value, success = Sucsses, Error = error });
                        }
                    }
                    if (error != null)
                    {
                        ActivityLog ActLog = new ActivityLog();
                        ActLog.Action = 2;
                        ActLog.Comment = "Vendor " + VendorMaster.Value.VendorId + "'>" + VendorMaster.Value.VendorName + "</a> Edited by " + UserModule.getUserFirstName() + " on " + AdminSiteConfiguration.GetDate(AdminSiteConfiguration.GetEasternTime(DateTime.Now).ToString());
                        //This  class is used for Activity Log Insert.
                        _ActivityLogRepository.ActivityLogInsert(ActLog);
                        //Sucsses = "Vendor Updated Successfully..";
                        Sucsses = _CommonRepository.GetMessageValue("VME", "Vendor Updated Successfully..");
                        return Json(new { data = VendorMaster.Value, success = Sucsses, Error = error });
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("VendorMastersController - Update - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            //+" " + VendorMaster.Value.Address2
            VendorMaster.Value.Address = (!String.IsNullOrEmpty(VendorMaster.Value.Address) ? VendorMaster.Value.Address + "," : "")
                + (!String.IsNullOrEmpty(VendorMaster.Value.Address) ? VendorMaster.Value.City + "," : "")
                + (!String.IsNullOrEmpty(VendorMaster.Value.Address) ? VendorMaster.Value.State + ' ' : "") + VendorMaster.Value.PostalCode;
            VendorMaster.Value.Status = VendorMaster.Value.IsActive == true ? "Active" : "Inactive";
            return Json(new { data = VendorMaster.Value, success = Sucsses, Error = error });
        }

        /// <summary>
        ///  This method is Pdf Export for Vendor Master.
        /// </summary>
        /// <param name="gridModel"></param>
        /// <returns></returns>
        public ActionResult PdfExport(string gridModel)
        {
            int? StoreIds = 0;
            GridPdfExport exp = new GridPdfExport();
            if (Session["storeid"] != null)
            {
                StoreIds = Convert.ToInt32(Convert.ToString(Session["storeid"]));
            }
            List<VenodrMasterSelect> vendorMasters = new List<VenodrMasterSelect>();
            try
            {
                //Using this class Get Vendor Master data
                vendorMasters = _vendorMasterRepository.GetVendorMastersDataSP(StoreIds);
                PdfDocument doc = new PdfDocument();
                doc.PageSettings.Orientation = PdfPageOrientation.Landscape;
                doc.PageSettings.Size = PdfPageSize.A2;
                
                exp.Theme = "flat-saffron";
                exp.FileName = "VendorDetail.pdf";
                exp.IsAutoFit = true;
                exp.PdfDocument = doc;
            }
            catch (Exception ex)
            {
                logger.Error("VendorMastersController - PdfExport - " + DateTime.Now + " - " + ex.Message.ToString());
            }
           
            Syncfusion.EJ2.Grids.Grid gridProperty = ConvertGridObject(gridModel, vendorMasters);
            return exp.PdfExport<VenodrMasterSelect>(gridProperty, vendorMasters);
        }

        /// <summary>
        /// This method is EXcel Export for Vendor Master.
        /// </summary>
        /// <param name="gridModel"></param>
        /// <returns></returns>
        public ActionResult ExcelExport(string gridModel)
        {
            int? StoreIds = 0;
          
            List<VenodrMasterSelect> vendorMasters = new List<VenodrMasterSelect>();
            GridExcelExport exp = new GridExcelExport();
            try
            {
                if (Session["storeid"] != null)
                {
                    StoreIds = Convert.ToInt32(Convert.ToString(Session["storeid"]));
                }
                //Using this class Get Vendor Master data
                vendorMasters = _vendorMasterRepository.GetVendorMastersDataSP(StoreIds);
               
                exp.Theme = "flat-saffron";
                exp.FileName = "VendorDetail.xlsx";
            }
            catch (Exception ex)
            {
                logger.Error("VendorMastersController - ExcelExport - " + DateTime.Now + " - " + ex.Message.ToString());
            }
          
            Syncfusion.EJ2.Grids.Grid gridProperty = ConvertGridObject(gridModel, vendorMasters);
            return exp.ExcelExport<VenodrMasterSelect>(gridProperty, vendorMasters);
        }

        /// <summary>
        /// This is Convert Grid object.
        /// </summary>
        /// <param name="gridProperty"></param>
        /// <returns></returns>
        private Syncfusion.EJ2.Grids.Grid ConvertGridObject(string gridProperty, List<VenodrMasterSelect> data)
        {
            Syncfusion.EJ2.Grids.Grid GridModel = (Syncfusion.EJ2.Grids.Grid)Newtonsoft.Json.JsonConvert.DeserializeObject(gridProperty, typeof(Syncfusion.EJ2.Grids.Grid));
            try
            {
                GridColumnModel cols = (GridColumnModel)Newtonsoft.Json.JsonConvert.DeserializeObject("{\"allowGrouping\":false,\"allowPaging\":false,\"pageSettings\":{\"currentPage\":1,\"pageCount\":2,\"pageSize\":100},\"sortSettings\":{},\"allowPdfExport\":true,\"allowExcelExport\":true,\"aggregates\":[],\"filterSettings\":{\"immediateModeDelay\":1500,\"type\":\"CheckBox\"},\"groupSettings\":{\"columns\":[],\"enableLazyLoading\":false,\"disablePageWiseAggregates\":false},\"columns\":[],\"locale\":\"en-US\",\"searchSettings\":{\"key\":\"\",\"fields\":[]}}", typeof(GridColumnModel));
                cols.columns.Add(new Syncfusion.EJ2.Grids.GridColumn { Field = "VendorName", HeaderText = "Display Name" });
                cols.columns.Add(new Syncfusion.EJ2.Grids.GridColumn { Field = "StoreName", HeaderText = "Store Name" });
                cols.columns.Add(new Syncfusion.EJ2.Grids.GridColumn { Field = "CompanyName", HeaderText = "Company Name" });
                cols.columns.Add(new Syncfusion.EJ2.Grids.GridColumn { Field = "Address", HeaderText = "Address" });
                cols.columns.Add(new Syncfusion.EJ2.Grids.GridColumn { Field = "PhoneNumber", HeaderText = "Phone Number" });
                cols.columns.Add(new Syncfusion.EJ2.Grids.GridColumn { Field = "EMail", HeaderText = "Email" });
                cols.columns.Add(new Syncfusion.EJ2.Grids.GridColumn { Field = "AccountNumber", HeaderText = "Account Number" });
                cols.columns.Add(new Syncfusion.EJ2.Grids.GridColumn { Field = "Departments", HeaderText = "Default Accounts" });
                cols.columns.Add(new Syncfusion.EJ2.Grids.GridColumn { Field = "Status", HeaderText = "Status" });

                foreach (var item in cols.columns)
                {
                    item.AutoFit = true;
                    item.Width = "10%";
                }

                foreach (var item in data)
                {
                    foreach (var col in cols.columns)
                    {
                        if (col.Field == "VendorName")
                        {
                            int maxLineLength = 25;

                            if (!string.IsNullOrEmpty(item.VendorName))
                            {
                                StringBuilder memoBuilder = new StringBuilder();
                                for (int i = 0; i < item.VendorName.Length; i += maxLineLength)
                                {
                                    int length = Math.Min(maxLineLength, item.VendorName.Length - i);
                                    memoBuilder.AppendLine(item.VendorName.Substring(i, length));
                                }
                                item.VendorName = memoBuilder.ToString().Trim();
                            }
                        }
                        else if (col.Field == "CompanyName")
                        {
                            int maxLineLength = 20;

                            if (!string.IsNullOrEmpty(item.CompanyName))
                            {
                                StringBuilder memoBuilder = new StringBuilder();
                                for (int i = 0; i < item.CompanyName.Length; i += maxLineLength)
                                {
                                    int length = Math.Min(maxLineLength, item.CompanyName.Length - i);
                                    memoBuilder.AppendLine(item.CompanyName.Substring(i, length));
                                }
                                item.CompanyName = memoBuilder.ToString().Trim();
                            }
                        }
                        else if (col.Field == "Address")
                        {
                            int maxLineLength = 20;

                            if (!string.IsNullOrEmpty(item.Address))
                            {
                                StringBuilder memoBuilder = new StringBuilder();
                                for (int i = 0; i < item.Address.Length; i += maxLineLength)
                                {
                                    int length = Math.Min(maxLineLength, item.Address.Length - i);
                                    memoBuilder.AppendLine(item.Address.Substring(i, length));
                                }
                                item.Address = memoBuilder.ToString().Trim();
                            }
                        }
                        else if (col.Field == "EMail")
                        {
                            int maxLineLength = 23;

                            if (!string.IsNullOrEmpty(item.EMail))
                            {
                                StringBuilder memoBuilder = new StringBuilder();
                                for (int i = 0; i < item.EMail.Length; i += maxLineLength)
                                {
                                    int length = Math.Min(maxLineLength, item.EMail.Length - i);
                                    memoBuilder.AppendLine(item.EMail.Substring(i, length));
                                }
                                item.EMail = memoBuilder.ToString().Trim();
                            }
                        }
                        else if (col.Field == "AccountNumber")
                        {
                            int maxLineLength = 15;

                            if (!string.IsNullOrEmpty(item.AccountNumber))
                            {
                                StringBuilder memoBuilder = new StringBuilder();
                                for (int i = 0; i < item.AccountNumber.Length; i += maxLineLength)
                                {
                                    int length = Math.Min(maxLineLength, item.AccountNumber.Length - i);
                                    memoBuilder.AppendLine(item.AccountNumber.Substring(i, length));
                                }
                                item.AccountNumber = memoBuilder.ToString().Trim();
                            }
                        }
                        else if (col.Field == "Departments")
                        {
                            int maxLineLength = 20;

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
                logger.Error("VendorMastersController - ConvertGridObject - " + DateTime.Now + " - " + ex.Message.ToString());
            }
           
            return GridModel;
        }

        /// <summary>
        ///This method return create view. 
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "Administrator,CreateQBVendor")]
        public ActionResult Create()
        {
            if (Message != "")
            {
                ViewBag.StatusMessageString = Message;
                Message = "";
            }
            VendorMaster obj = new VendorMaster();
            try
            {
                //Get All State master.
                ViewBag.State = new SelectList(_MastersBindRepository.GetStateMasters().OrderBy(s => s.StateName), "StateName", "StateName");
                var UserName = System.Web.HttpContext.Current.User.Identity.Name;
                //Using this db class get StoreList with role Wise.
                var StoreList = _CommonRepository.GetStoreList_RoleWise(10, "CreateQBVendor", UserName).Select(s => s.StoreId);
                //Get Store Masters By storeid
                ViewBag.MultiStoreId = new MultiSelectList(_MastersBindRepository.GetStoreMasters().Where(s => StoreList.Contains(s.StoreId)).Select(s => new { s.StoreId, s.Name }).OrderBy(o => o.Name).ToList(), "StoreId", "Name");
                ViewBag.MultiDepartmentId = new MultiSelectList("");
            }
            catch (Exception ex)
            {
                logger.Error("VendorMastersController - Create - " + DateTime.Now + " - " + ex.Message.ToString());
            }
           
            return View(obj);
        }

        /// <summary>
        /// This method get Vendor by id for update vendor.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = "Administrator,UpdateQBVendor")]
        // GET: VendorMasters/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //Get Vendor by  Ids
            VendorMaster vendorMaster = _vendorMasterRepository.getVendor(id.Value);
            try
            {
                //Get All State master.
                ViewBag.State = new SelectList(_MastersBindRepository.GetStateMasters().OrderBy(s => s.StateName), "StateName", "StateName", vendorMaster.State);
                //Get Store Masters By storeid
                ViewBag.StoreId = new SelectList(_MastersBindRepository.GetStoreMasters().OrderBy(o => o.Name), "StoreId", "Name", vendorMaster.StoreId);
                string[] commomtype = new string[] { "Expense", "Cost of Goods Sold", "Fixed Asset" };
                //Get All Account Type Master
                var AccountTypeId = _MastersBindRepository.GetAccountTypeMasters().Where(s => commomtype.Contains(s.CommonType)).Select(s => s.AccountTypeId).ToList();
                //Get All Vendor Departments Ids

                var list = _vendorMasterRepository.GetVendorDepartmentIds(vendorMaster.VendorId, vendorMaster.StoreId.Value);
                //Get All DepartMent Masrter
                ViewBag.MultiDepartmentId = new MultiSelectList(_MastersBindRepository.GetDepartmentMasters(vendorMaster.StoreId.Value).Where(s => s.StoreId == vendorMaster.StoreId && s.IsActive == true && AccountTypeId.Contains(s.AccountTypeId)).Select(s => new { s.DepartmentId, DepartmentName = s.DepartmentName }).OrderBy(o => o.DepartmentName).ToList(), "DepartmentId", "DepartmentName", list);
            }
            catch (Exception ex)
            {
                logger.Error("VendorMastersController - Edit - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            if (vendorMaster == null)
            {
                return HttpNotFound();
            }
        
            return View(vendorMaster);
        }

        // POST: VendorMasters/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        /// <summary>
        /// This method is Update vendor Master.
        /// </summary>
        /// <param name="vendorMaster"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(VendorMaster vendorMaster)
        {
            TempData["SucessmeV"] = null;
            if (ModelState.IsValid)
            {
                try
                {
                    int StoreID = Convert.ToInt32(vendorMaster.StoreId);
                    int iVendorId = 0;
                    //Using this db class get stores on Line desktop.
                    string Store = _qBRepository.GetStoreOnlineDesktop(Convert.ToInt32(vendorMaster.StoreId));
                    //Using this db class get stores on Line desktop Flag.
                    int StoreFlag = _qBRepository.GetStoreOnlineDesktopFlag(Convert.ToInt32(vendorMaster.StoreId));
                    if (Store != "")
                    {
                        SynthesisQBOnline.BAL.QBResponse objResponse = new SynthesisQBOnline.BAL.QBResponse();
                        if (Store == "Online")
                        {
                            if (StoreFlag == 1)
                            {
                                //Get vendor by VendorId
                                var VendorExist = _vendorMasterRepository.getVendor(vendorMaster.VendorId);
                                if (VendorExist != null)
                                {
                                    //Get QB Edit Sync Vendor
                                    _qBRepository.QBEditSyncVendor(vendorMaster, VendorExist.ListId, Convert.ToInt32(vendorMaster.StoreId), ref objResponse);
                                    if (objResponse.ID != "0" || objResponse.Status == "Done")
                                    {
                                        //Get vendor by StoreId ListId.
                                        var objVendorExist = _vendorMasterRepository.getVendorByStoreId_LisId(StoreID, objResponse.ID);

                                        if (objVendorExist != null)
                                        {
                                            //Using this db class check vendor if exist or not.
                                            var vData = _vendorMasterRepository.checkVendorExist(vendorMaster);
                                            if (vData != null)
                                            {
                                                AlreadyExist(vendorMaster);
                                            }
                                            vendorMaster.ListId = objResponse.ID;
                                            //Get UserID by Username.
                                            vendorMaster.ModifiedBy = _CommonRepository.getUserId(User.Identity.Name);
                                            //Update vendore Master data.
                                            iVendorId = _vendorMasterRepository.UpdateVendorMaster(vendorMaster);

                                            if (iVendorId == 0)
                                            {
                                                //Message = "Vendor Not Synched in QuickBook.";
                                                Message = _CommonRepository.GetMessageValue("VMSQ", "Vendor Not Synched in QuickBook.");
                                                TempData["Sucessme"] = Message;
                                                Session["Sucessmesg"] = Message;
                                                return RedirectToAction("Edit/", new { Id = vendorMaster.VendorId, StoreID = vendorMaster.StoreId });
                                            }
                                            else
                                            {
                                                //TempData["SucessmeV"] = "Vendor Save Successfully..";
                                                TempData["SucessmeV"] = _CommonRepository.GetMessageValue("VMSQS", "Vendor Save Successfully..");
                                            }
                                        }
                                        else
                                        {
                                            //Using this db class check vendor if exist or not.
                                            var vData = _vendorMasterRepository.checkVendorExist(vendorMaster);
                                            if (vData != null)
                                            {
                                                AlreadyExist(vendorMaster);
                                            }
                                            vendorMaster.ListId = objResponse.ID;
                                            //Get UserID by Username.
                                            vendorMaster.CreatedBy = _CommonRepository.getUserId(User.Identity.Name);
                                            //Save Vendor Master data.
                                            iVendorId = _vendorMasterRepository.SaveVendorMaster(vendorMaster);
                                            //TempData["SucessmeV"] = "Vendor Save Successfully..";
                                            TempData["SucessmeV"] = _CommonRepository.GetMessageValue("VMSQS", "Vendor Save Successfully..");
                                        }
                                    }
                                    else
                                    {
                                        Message = objResponse.Status.ToString();
                                    }
                                }
                                else
                                {
                                    //This db class is QB sync Vendor
                                    _qBRepository.QBSyncVendor(vendorMaster, Convert.ToInt32(vendorMaster.StoreId), ref objResponse);
                                    if (objResponse.ID != "0" || objResponse.Status == "Done")
                                    {
                                        //Using this db class check vendor if exist or not.
                                        var vData = _vendorMasterRepository.checkVendorExist(vendorMaster);
                                        if (vData != null)
                                        {
                                            AlreadyExist(vendorMaster);
                                        }
                                        //Save Vendor Master data.
                                        iVendorId = _vendorMasterRepository.SaveVendorMaster(vendorMaster);
                                        //TempData["SucessmeV"] = "Vendor Save Successfully..";
                                        TempData["SucessmeV"] = _CommonRepository.GetMessageValue("VMSQS", "Vendor Save Successfully..");
                                    }
                                }
                            }
                            else
                            {
                                //Using this class Get Vendor Master data using storeid.
                                var objVendorExist = _vendorMasterRepository.GetVendorMastersDataSP(vendorMaster.StoreId).ToList().Where(p => p.VendorId == vendorMaster.VendorId).FirstOrDefault();
                                if (objVendorExist != null)
                                {
                                    //Using this db class check vendor if exist or not.
                                    var vData = _vendorMasterRepository.checkVendorExist(vendorMaster);
                                    if (vData != null)
                                    {
                                        AlreadyExist(vendorMaster);
                                    }
                                    // Update Vendor Master data.
                                    iVendorId = _vendorMasterRepository.UpdateVendorMaster(vendorMaster);
                                    if (iVendorId == 0)
                                    {
                                        Message = "Vendor Not Updated.";
                                        TempData["Sucessme"] = Message;
                                        Session["Sucessmesg"] = Message;
                                        return RedirectToAction("Edit/", new { Id = vendorMaster.VendorId, StoreID = vendorMaster.StoreId });
                                    }
                                    else
                                    {
                                        //TempData["SucessmeV"] = "Vendor Save Successfully..";
                                        TempData["SucessmeV"] = _CommonRepository.GetMessageValue("VMSQS", "Vendor Save Successfully..");
                                    }
                                }
                                else
                                {
                                    //Using this db class check vendor if exist or not.
                                    var vData = _vendorMasterRepository.checkVendorExist(vendorMaster);
                                    if (vData != null)
                                    {
                                        AlreadyExist(vendorMaster);
                                    }
                                    //Save Vendor Master data.
                                    iVendorId = _vendorMasterRepository.SaveVendorMaster(vendorMaster);
                                    //TempData["SucessmeV"] = "Vendor Save Successfully..";
                                    TempData["SucessmeV"] = _CommonRepository.GetMessageValue("VMSQS", "Vendor Save Successfully..");
                                }
                            }
                        }
                        else
                        {
                            //Using this class Get Vendor Master data using storeid
                            var objVendorExist = _vendorMasterRepository.GetVendorMastersDataSP(vendorMaster.StoreId).ToList().Where(p => p.VendorId == vendorMaster.VendorId).FirstOrDefault();
                            if (objVendorExist != null)
                            {
                                //Using this db class check vendor if exist or not.
                                var vData = _vendorMasterRepository.checkVendorExist(vendorMaster);
                                if (vData != null)
                                {
                                    AlreadyExist(vendorMaster);
                                }
                                //update Vendor Master data.
                                iVendorId = _vendorMasterRepository.UpdateVendorMaster(vendorMaster);
                                if (iVendorId == 0)
                                {
                                    //Message = "Vendor Not Updated.";
                                    Message = _CommonRepository.GetMessageValue("VNU", "Vendor Not Updated.");
                                    TempData["Sucessme"] = Message;
                                    Session["Sucessmesg"] = Message;
                                    return RedirectToAction("Edit/", new { Id = vendorMaster.VendorId, StoreID = vendorMaster.StoreId });
                                }
                                else
                                {
                                    //TempData["SucessmeV"] = "Vendor Save Successfully..";
                                    TempData["SucessmeV"] = _CommonRepository.GetMessageValue("VMSQS", "Vendor Save Successfully..");
                                }
                            }
                            else
                            {
                                //Using this db class check vendor if exist or not.
                                var vData = _vendorMasterRepository.checkVendorExist(vendorMaster);
                                if (vData != null)
                                {
                                    AlreadyExist(vendorMaster);
                                }
                                //Save Vendor Master data.
                                iVendorId = _vendorMasterRepository.SaveVendorMaster(vendorMaster);
                                //TempData["SucessmeV"] = "Vendor Save Successfully..";
                                TempData["SucessmeV"] = _CommonRepository.GetMessageValue("VMSQS", "Vendor Save Successfully..");
                            }
                        }
                    }
                    else
                    {
                        //Using this class Get Vendor Master data using storeid
                        var objVendorExist = _vendorMasterRepository.GetVendorMastersDataSP(vendorMaster.StoreId).ToList().Where(p => p.VendorId == vendorMaster.VendorId).FirstOrDefault();
                        if (objVendorExist != null)
                        {
                            //Using this db class check vendor if exist or not.
                            var vData = _vendorMasterRepository.checkVendorExist(vendorMaster);
                            if (vData != null)
                            {
                                AlreadyExist(vendorMaster);
                            }
                            //Using this db class check vendor if exist or not.
                            _vendorMasterRepository.checkVendorExist(vendorMaster);
                            if (iVendorId == 0)
                            {
                                //Message = "Vendor Not Updated.";
                                Message = _CommonRepository.GetMessageValue("VNU", "Vendor Not Updated.");
                                TempData["Sucessme"] = Message;
                                Session["Sucessmesg"] = Message;
                                return RedirectToAction("Edit/", new { Id = vendorMaster.VendorId, StoreID = vendorMaster.StoreId });
                            }
                            else
                            {
                                //TempData["SucessmeV"] = "Vendor Save Successfully..";
                                TempData["SucessmeV"] = _CommonRepository.GetMessageValue("VMSQS", "Vendor Save Successfully..");
                            }
                        }
                        else
                        {
                            //Using this db class check vendor if exist or not.
                            var vData = _vendorMasterRepository.checkVendorExist(vendorMaster);
                            if (vData != null)
                            {
                                AlreadyExist(vendorMaster);
                            }
                            //Save Vendor Master data.
                            _vendorMasterRepository.SaveVendorMaster(vendorMaster);
                            //TempData["SucessmeV"] = "Vendor Save Successfully..";
                            TempData["SucessmeV"] = _CommonRepository.GetMessageValue("VMSQS", "Vendor Save Successfully..");
                        }
                    }
                    //Save Vendor Department data.
                    _vendorMasterRepository.VendorDepartmentSave(StoreID, vendorMaster);
                    if (TempData["SucessmeV"] != null)
                    {
                        ActivityLog ActLog = new ActivityLog();
                        ActLog.Action = 2;
                        //This Db class is used for get user firstname.
                        ActLog.Comment = "Vendor " + "<a href='/VendorMasters/Details/" + vendorMaster.VendorId + "'>" + vendorMaster.VendorName + "</a> Edited by " + _CommonRepository.getUserFirstName(User.Identity.Name) + " on " + AdminSiteConfiguration.GetDate(AdminSiteConfiguration.GetEasternTime(DateTime.Now).ToString());
                        //This  class is used for Activity Log Insert.
                        _ActivityLogRepository.ActivityLogInsert(ActLog);
                        Session["Sucessmesg"] = TempData["SucessmeV"].ToString();
                        //Message = "Vendor Updated Successfully..";
                        Message = _CommonRepository.GetMessageValue("VME", "Vendor Updated Successfully..");
                        ViewBag.StatusMessageString = Message;
                        return RedirectToAction("Index");
                    }
                    Session["Sucessmesg"] = TempData["Sucessme"].ToString();
                    TempData["Sucessme"] = null;
                }
                catch (Exception ex)
                {
                    logger.Error("VendorMastersController - Edit - " + DateTime.Now + " - " + ex.Message.ToString());
                }
            }
            //Get All State master.
            ViewBag.State = new SelectList(_MastersBindRepository.GetStateMasters().OrderBy(s => s.StateName), "StateName", "StateName", vendorMaster.State);
            //Get Store Masters By storeid
            ViewBag.StoreId = new SelectList(_MastersBindRepository.GetStoreMasters().OrderBy(o => o.Name), "StoreId", "Name", vendorMaster.StoreId);
            string[] commomtype = new string[] { "Expense", "Cost of Goods Sold", "Fixed Asset" };
            //Get All Account Type Master
            var AccountTypeId = _MastersBindRepository.GetAccountTypeMasters().Where(s => commomtype.Contains(s.CommonType)).Select(s => s.AccountTypeId).ToList();
            //Get All Vendor Departments Ids
            var list = _vendorMasterRepository.GetVendorDepartmentIds(vendorMaster.VendorId, vendorMaster.StoreId.Value);
            //Get All DepartMent Masrter
            ViewBag.MultiDepartmentId = new MultiSelectList(_MastersBindRepository.GetDepartmentMasters(vendorMaster.StoreId.Value).Where(s => AccountTypeId.Contains(s.AccountTypeId)).Select(s => new { s.DepartmentId, DepartmentName = s.DepartmentName }).OrderBy(o => o.DepartmentName).ToList(), "DepartmentId", "DepartmentName", list);
            return View(vendorMaster);
        }
        /// <summary>
        /// This method get Already Exist vendor
        /// </summary>
        /// <param name="vendorMaster"></param>
        /// <returns></returns>
        public ActionResult AlreadyExist(VendorMaster vendorMaster)
        {
            Message = _CommonRepository.GetMessageValue("VMCE", "Vendor Name Already Exist..");
            TempData["Sucessme"] = Message;
            Session["Sucessmesg"] = Message;
            return RedirectToAction("Edit/", new { Id = vendorMaster.VendorId, StoreID = vendorMaster.StoreId });
        }

        // GET: VendorMasters/Delete/5
        /// <summary>
        /// This method get Vendor by id for delete vendor.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //Get vendor by VendorId
            VendorMaster vendorMaster = _vendorMasterRepository.getVendor(id.Value);
            if (vendorMaster == null)
            {
                return HttpNotFound();
            }
            return View(vendorMaster);
        }

        // POST: VendorMasters/Delete/5
        /// <summary>
        /// This method delete Vendor by id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            try
            {
                //This db class is used for delete vendor
                _vendorMasterRepository.DeleteVendor(id, User.Identity.Name);

                //Message = "Vendor Deleted Successfully..";
                Message = _CommonRepository.GetMessageValue("VMD", "Vendor Deleted Successfully..");
                ViewBag.StatusMessageString = Message;
                
            }
            catch (Exception ex)
            {
                logger.Error("VendorMastersController - DeleteConfirmed - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return RedirectToAction("Index");
        }

        /// <summary>
        /// This method get Store Valuefor vendor.
        /// </summary>
        /// <param name="vendorId"></param>
        /// <param name="VendorName"></param>
        /// <returns></returns>
        public ActionResult GetStorevalueforVendor(string vendorId, string VendorName)
        {
            List<VenodrMasterCopy> list = new List<VenodrMasterCopy>();
            try
            {
                //Get store valur for vendor
                list = _vendorMasterRepository.GetStorevalueforVendor(VendorName);
            }
            catch (Exception ex)
            {
                logger.Error("VendorMastersController - GetStorevalueforVendor - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            ViewBag.dataSource = list;
            return PartialView("_CopyVendorPartial");
        }

        /// <summary>
        /// This method get Vendor data store Wise.
        /// </summary>
        /// <param name="StoreId"></param>
        /// <param name="vendorId"></param>
        /// <returns></returns>
        public ActionResult PasteVendordatastoreWise(string StoreId, string vendorId)
        {
            try
            {
                int VendorID = Convert.ToInt32(vendorId);
                int StoreID = Convert.ToInt32(StoreId);
                //Get UserID by Username.
                int UserId = _CommonRepository.getUserId(User.Identity.Name);
                //Get vendor by VendorId
                VendorMaster vendorMaster = _vendorMasterRepository.getVendor(VendorID);
                VendorMaster objVendor = new VendorMaster();
                SynthesisQBOnline.BAL.QBResponse objResponse1 = new SynthesisQBOnline.BAL.QBResponse();
                //This db class is QB sync Vendor
                _qBRepository.QBSyncVendor(vendorMaster, StoreID, ref objResponse1);
                int VendorCopyID = 0;
                logger.Info("VendorMastersController - PasteVendordatastoreWise - " + DateTime.Now + " - " + objResponse1.ID + " - " + objResponse1.Status);
                if (objResponse1.ID != "0" && objResponse1.Status == "Done")
                {
                    string ListId = objResponse1.ID;
                    vendorMaster.CreatedBy = UserId;
                    //Save Vendor Master data.
                    VendorCopyID = _vendorMasterRepository.SaveCopyVendorMaster(vendorMaster, StoreID, ListId);
                }
                //Save Vendor department Relation master valur 
                _vendorMasterRepository.VendorDepartmentRelationMasterSave(VendorID, VendorCopyID, StoreID, UserId);

                List<VenodrMasterCopy> list = new List<VenodrMasterCopy>();
                //Get store valur for vendor
                list = _vendorMasterRepository.GetStorevalueforVendor(vendorMaster.VendorName);

                ViewBag.dataSource = list;
                if (VendorCopyID > 0)
                {
                    ViewBag.Message = _CommonRepository.GetMessageValue("VMCS", "Copy Vendor Succefully");
                }
                else
                {
                    ViewBag.Message = "Something went Wrong";
                }
            }
            catch (Exception ex)
            {
                AdminSiteConfiguration.WriteErrorLogs("Controller : PasteVendordatastoreWise Method : PasteVendordatastoreWise Message:" + ex.Message + "Internal Message:" + ex.InnerException);
            }
            return PartialView("_CopyVendorPartial");
        }

        public JsonResult UpdateVendorStatus(int VendorId, bool IsActive)
        {
            string Message = "";
            try
            {
                _vendorMasterRepository.UpdateVendorStatus(VendorId, IsActive);
                Message = "Success";
            }
            catch (Exception)
            {
                Message = "Error";
            }
            return Json(Message);
        }

        #region merge vendor
        public async Task<ActionResult> MergeVendor()
        {
            ViewBag.Title = "MergeVendors - Synthesis";
            _CommonRepository.LogEntries();     //Harsh's code
            ViewBag.VendorList1 = _vendorMasterRepository.GetMergeVendor("SelectVendorForMerge",1);
          
            ViewBag.VendorList2 = _vendorMasterRepository.GetMergeVendor("SelectVendorForMerge", 2);

            ViewBag.VendorList3 = _vendorMasterRepository.GetMergeVendor("SelectVendorForMerge", 3);

            ViewBag.VendorList4 = _vendorMasterRepository.GetMergeVendor("SelectVendorForMerge", 4);

            ViewBag.VendorList5 = _vendorMasterRepository.GetMergeVendor("SelectVendorForMerge", 5);

            ViewBag.VendorList6 = _vendorMasterRepository.GetMergeVendor("SelectVendorForMerge", 6);

            ViewBag.VendorList7 = _vendorMasterRepository.GetMergeVendor("SelectVendorForMerge", 7);

            ViewBag.VendorList9 = _vendorMasterRepository.GetMergeVendor("SelectVendorForMerge", 9);

            Session["store1Vendor"] = null;
            Session["store2Vendor"] = null;
            Session["store3Vendor"] = null;
            Session["store4Vendor"] = null;
            Session["store5Vendor"] = null;
            Session["store6Vendor"] = null;
            Session["store7Vendor"] = null;
            Session["store9Vendor"] = null;
            return View();
        }

        [HttpPost]
        public ActionResult RemoveMergeVendor1(string vendorid)
        {
            string message = "";
            try
            {
                if (vendorid != "" && vendorid != null)
                {
                    if (Session["store1Vendor"] != null)
                    {
                        List<AddMergeVendor> BindData = new List<AddMergeVendor>();
                        BindData = (List<AddMergeVendor>)Session["store1Vendor"];
                        if (BindData != null)
                        {
                            var itemToRemove = BindData.SingleOrDefault(b => b.VendorId.ToString() == vendorid);
                            BindData.Remove(itemToRemove);
                        }
                        Session["store1Vendor"] = BindData;
                        message = "success";
                    }
                }
            }
            catch (Exception ex)
            {
                message = "Something went wrong..!!";
            }
            return Json(message);
        }

        public ActionResult AddVendorlistStore1(DataManagerRequest dm, string vendorid, string vendorname)
        {
            List<AddMergeVendor> BindData = new List<AddMergeVendor>();
            if (Session["store1Vendor"] != null)
            {
                BindData = (List<AddMergeVendor>)Session["store1Vendor"];
            }
            if (vendorid != null && vendorname != null)
            {
                var IsActive = _vendorMasterRepository.getVendor(Convert.ToInt32(vendorid)).IsActive;
                BindData.Add(new AddMergeVendor() { VendorId = vendorid, VendorName = vendorname.ToString().Replace("^", "&"), Status = IsActive.ToString() });
            }
            Session["store1Vendor"] = BindData;
            IEnumerable DataSource = BindData;
            int count = DataSource.Cast<AddMergeVendor>().Count();

            return dm.RequiresCounts ? Json(new { result = DataSource, count = count }) : Json(DataSource);
        }

        [HttpPost]
        public ActionResult UpdateMergeVendorStore1(string mastervendorid)
        {
            List<GetMergeVendor> obj = new List<GetMergeVendor>();
            List<MergeVendorInvoiceList> InvList = new List<MergeVendorInvoiceList>();
            string message = "";
            try
            {
                if (mastervendorid != "" && mastervendorid != null)
                {
                    if (Session["store1Vendor"] != null)
                    {
                        List<AddMergeVendor> BindData = new List<AddMergeVendor>();
                        BindData = (List<AddMergeVendor>)Session["store1Vendor"];
                        string arr = "";
                        foreach (var b in BindData)
                        {
                            arr += b.VendorId + ",";
                        }

                        InvList = _vendorMasterRepository.GetMergeVendorInvoiceList("GetAllInvoiceVendorId", arr, 1);
                        if (InvList != null)
                        {
                            foreach (var a in BindData)
                            {
                                //db.Database.ExecuteSqlCommand("SP_Invoice @Mode={0},@MasterVendorId={1},@VendorId={2},@StoreId={3}", "MergeInvoiceVendorId", mastervendorid, a.VendorId, 1);
                                _vendorMasterRepository.MergeInvoiceVendor("MergeInvoiceVendorId", mastervendorid, a.VendorId, 1);
                            }
                            MoveFile(InvList, mastervendorid);
                        }
                        Session["store1Vendor"] = null;
                        message = "success";

                        obj = _vendorMasterRepository.GetMergeVendor("SelectVendorForMerge", 1);
                    }
                }
            }
            catch (Exception ex)
            {
                message = "Something went wrong..!!";
            }
            return Json(new { message = message, obj });
        }

        public ActionResult AddVendorlistStore2(DataManagerRequest dm, string vendorid, string vendorname)
        {
            List<AddMergeVendor> BindData = new List<AddMergeVendor>();
            if (Session["store2Vendor"] != null)
            {
                BindData = (List<AddMergeVendor>)Session["store2Vendor"];
            }
            if (vendorid != null && vendorname != null)
            {
                var IsActive = _vendorMasterRepository.getVendor(Convert.ToInt32(vendorid)).IsActive;
                BindData.Add(new AddMergeVendor() { VendorId = vendorid, VendorName = vendorname.ToString().Replace("^", "&"), Status = IsActive.ToString() });
            }
            Session["store2Vendor"] = BindData;
            IEnumerable DataSource = BindData;
            int count = DataSource.Cast<AddMergeVendor>().Count();

            return dm.RequiresCounts ? Json(new { result = DataSource, count = count }) : Json(DataSource);
        }

        [HttpPost]
        public ActionResult UpdateMergeVendorStore2(string mastervendorid)
        {
            List<GetMergeVendor> obj = new List<GetMergeVendor>(); List<MergeVendorInvoiceList> InvList = new List<MergeVendorInvoiceList>();
            string message = "";
            try
            {
                if (mastervendorid != "" && mastervendorid != null)
                {
                    if (Session["store2Vendor"] != null)
                    {
                        List<AddMergeVendor> BindData = new List<AddMergeVendor>();
                        BindData = (List<AddMergeVendor>)Session["store2Vendor"];
                        string arr = "";
                        foreach (var b in BindData)
                        {
                            arr += b.VendorId + ",";
                        }
                        InvList = _vendorMasterRepository.GetMergeVendorInvoiceList("GetAllInvoiceVendorId", arr, 2); ;
                        
                        if (InvList != null)
                        {
                            foreach (var a in BindData)
                            {
                                _vendorMasterRepository.MergeInvoiceVendor("MergeInvoiceVendorId", mastervendorid, a.VendorId, 2);
                            }
                            MoveFile(InvList, mastervendorid);
                        }
                        Session["store2Vendor"] = null;
                        message = "success";
                        obj = _vendorMasterRepository.GetMergeVendor("SelectVendorForMerge", 2);
                    }
                }
            }
            catch (Exception ex)
            {
                message = "Something went wrong..!!";
            }
            return Json(new { message = message, obj });
        }

        public ActionResult AddVendorlistStore3(DataManagerRequest dm, string vendorid, string vendorname)
        {
            List<AddMergeVendor> BindData = new List<AddMergeVendor>();
            if (Session["store3Vendor"] != null)
            {
                BindData = (List<AddMergeVendor>)Session["store3Vendor"];
            }
            if (vendorid != null && vendorname != null)
            {
                var IsActive = _vendorMasterRepository.getVendor(Convert.ToInt32(vendorid)).IsActive;
                BindData.Add(new AddMergeVendor() { VendorId = vendorid, VendorName = vendorname.ToString().Replace("^", "&"), Status = IsActive.ToString() });
            }
            Session["store3Vendor"] = BindData;
            IEnumerable DataSource = BindData;
            int count = DataSource.Cast<AddMergeVendor>().Count();

            return dm.RequiresCounts ? Json(new { result = DataSource, count = count }) : Json(DataSource);
        }

        [HttpPost]
        public ActionResult UpdateMergeVendorStore3(string mastervendorid)
        {
            List<GetMergeVendor> obj = new List<GetMergeVendor>(); List<MergeVendorInvoiceList> InvList = new List<MergeVendorInvoiceList>();
            string message = "";
            try
            {
                if (mastervendorid != "" && mastervendorid != null)
                {
                    if (Session["store3Vendor"] != null)
                    {
                        List<AddMergeVendor> BindData = new List<AddMergeVendor>();
                        BindData = (List<AddMergeVendor>)Session["store3Vendor"];
                        string arr = "";
                        foreach (var b in BindData)
                        {
                            arr += b.VendorId + ",";
                        }
                        InvList = _vendorMasterRepository.GetMergeVendorInvoiceList("GetAllInvoiceVendorId", arr, 3); ;

                        if (InvList != null)
                        {
                            foreach (var a in BindData)
                            {
                                _vendorMasterRepository.MergeInvoiceVendor("MergeInvoiceVendorId", mastervendorid, a.VendorId, 3);
                            }
                            MoveFile(InvList, mastervendorid);
                        }
                        Session["store3Vendor"] = null;
                        message = "success";
                        obj = _vendorMasterRepository.GetMergeVendor("SelectVendorForMerge", 3);
                    }
                }
            }
            catch (Exception ex)
            {
                message = "Something went wrong..!!";
            }
            return Json(new { message = message, obj });
        }

        public ActionResult AddVendorlistStore4(DataManagerRequest dm, string vendorid, string vendorname)
        {
            List<AddMergeVendor> BindData = new List<AddMergeVendor>();
            if (Session["store4Vendor"] != null)
            {
                BindData = (List<AddMergeVendor>)Session["store4Vendor"];
            }
            if (vendorid != null && vendorname != null)
            {
                var IsActive = _vendorMasterRepository.getVendor(Convert.ToInt32(vendorid)).IsActive;
                BindData.Add(new AddMergeVendor() { VendorId = vendorid, VendorName = vendorname.ToString().Replace("^", "&"), Status = IsActive.ToString() });
            }
            Session["store4Vendor"] = BindData;
            IEnumerable DataSource = BindData;
            int count = DataSource.Cast<AddMergeVendor>().Count();

            return dm.RequiresCounts ? Json(new { result = DataSource, count = count }) : Json(DataSource);
        }

        [HttpPost]
        public ActionResult UpdateMergeVendorStore4(string mastervendorid)
        {
            List<GetMergeVendor> obj = new List<GetMergeVendor>(); List<MergeVendorInvoiceList> InvList = new List<MergeVendorInvoiceList>();
            string message = "";
            try
            {
                if (mastervendorid != "" && mastervendorid != null)
                {
                    if (Session["store4Vendor"] != null)
                    {
                        List<AddMergeVendor> BindData = new List<AddMergeVendor>();
                        BindData = (List<AddMergeVendor>)Session["store4Vendor"];
                        string arr = "";
                        foreach (var b in BindData)
                        {
                            arr += b.VendorId + ",";
                        }
                        InvList = _vendorMasterRepository.GetMergeVendorInvoiceList("GetAllInvoiceVendorId", arr, 4); ;

                        if (InvList != null)
                        {
                            foreach (var a in BindData)
                            {
                                _vendorMasterRepository.MergeInvoiceVendor("MergeInvoiceVendorId", mastervendorid, a.VendorId, 4);
                            }
                            MoveFile(InvList, mastervendorid);
                        }
                        Session["store4Vendor"] = null;
                        message = "success";
                        obj = _vendorMasterRepository.GetMergeVendor("SelectVendorForMerge", 4);
                    }
                }
            }
            catch (Exception ex)
            {
                message = "Something went wrong..!!";
            }
            return Json(new { message = message, obj });
        }

        public ActionResult AddVendorlistStore5(DataManagerRequest dm, string vendorid, string vendorname)
        {
            List<AddMergeVendor> BindData = new List<AddMergeVendor>();
            if (Session["store5Vendor"] != null)
            {
                BindData = (List<AddMergeVendor>)Session["store5Vendor"];
            }
            if (vendorid != null && vendorname != null)
            {
                var IsActive = _vendorMasterRepository.getVendor(Convert.ToInt32(vendorid)).IsActive;
                BindData.Add(new AddMergeVendor() { VendorId = vendorid, VendorName = vendorname.ToString().Replace("^", "&"), Status = IsActive.ToString() });
            }
            Session["store5Vendor"] = BindData;
            IEnumerable DataSource = BindData;
            int count = DataSource.Cast<AddMergeVendor>().Count();

            return dm.RequiresCounts ? Json(new { result = DataSource, count = count }) : Json(DataSource);
        }

        [HttpPost]
        public ActionResult UpdateMergeVendorStore5(string mastervendorid)
        {
            List<GetMergeVendor> obj = new List<GetMergeVendor>(); List<MergeVendorInvoiceList> InvList = new List<MergeVendorInvoiceList>();
            string message = "";
            try
            {
                if (mastervendorid != "" && mastervendorid != null)
                {
                    if (Session["store5Vendor"] != null)
                    {
                        List<AddMergeVendor> BindData = new List<AddMergeVendor>();
                        BindData = (List<AddMergeVendor>)Session["store5Vendor"];
                        string arr = "";
                        foreach (var b in BindData)
                        {
                            arr += b.VendorId + ",";
                        }
                        InvList = _vendorMasterRepository.GetMergeVendorInvoiceList("GetAllInvoiceVendorId", arr, 5); ;

                        if (InvList != null)
                        {
                            foreach (var a in BindData)
                            {
                                _vendorMasterRepository.MergeInvoiceVendor("MergeInvoiceVendorId", mastervendorid, a.VendorId, 5);
                            }
                            MoveFile(InvList, mastervendorid);
                        }
                        Session["store5Vendor"] = null;
                        message = "success";
                        obj = _vendorMasterRepository.GetMergeVendor("SelectVendorForMerge", 5);
                    }
                }
            }
            catch (Exception ex)
            {
                message = "Something went wrong..!!";
            }
            return Json(new { message = message, obj });
        }

        public ActionResult AddVendorlistStore6(DataManagerRequest dm, string vendorid, string vendorname)
        {
            List<AddMergeVendor> BindData = new List<AddMergeVendor>();
            if (Session["store6Vendor"] != null)
            {
                BindData = (List<AddMergeVendor>)Session["store6Vendor"];
            }
            if (vendorid != null && vendorname != null)
            {
                var IsActive = _vendorMasterRepository.getVendor(Convert.ToInt32(vendorid)).IsActive;
                BindData.Add(new AddMergeVendor() { VendorId = vendorid, VendorName = vendorname.ToString().Replace("^", "&"), Status = IsActive.ToString() });
            }
            Session["store6Vendor"] = BindData;
            IEnumerable DataSource = BindData;
            int count = DataSource.Cast<AddMergeVendor>().Count();

            return dm.RequiresCounts ? Json(new { result = DataSource, count = count }) : Json(DataSource);
        }

        [HttpPost]
        public ActionResult UpdateMergeVendorStore6(string mastervendorid)
        {
            List<GetMergeVendor> obj = new List<GetMergeVendor>(); List<MergeVendorInvoiceList> InvList = new List<MergeVendorInvoiceList>();
            string message = "";
            try
            {
                if (mastervendorid != "" && mastervendorid != null)
                {
                    if (Session["store6Vendor"] != null)
                    {
                        List<AddMergeVendor> BindData = new List<AddMergeVendor>();
                        BindData = (List<AddMergeVendor>)Session["store6Vendor"];
                        string arr = "";
                        foreach (var b in BindData)
                        {
                            arr += b.VendorId + ",";
                        }
                        InvList = _vendorMasterRepository.GetMergeVendorInvoiceList("GetAllInvoiceVendorId", arr, 6); ;

                        if (InvList != null)
                        {
                            foreach (var a in BindData)
                            {
                                _vendorMasterRepository.MergeInvoiceVendor("MergeInvoiceVendorId", mastervendorid, a.VendorId, 6);
                            }
                            MoveFile(InvList, mastervendorid);
                        }
                        Session["store6Vendor"] = null;
                        message = "success";
                        obj = _vendorMasterRepository.GetMergeVendor("SelectVendorForMerge", 6);
                    }
                }
            }
            catch (Exception ex)
            {
                message = "Something went wrong..!!";
            }
            return Json(new { message = message, obj });
        }

        public ActionResult AddVendorlistStore7(DataManagerRequest dm, string vendorid, string vendorname)
        {
            List<AddMergeVendor> BindData = new List<AddMergeVendor>();
            if (Session["store7Vendor"] != null)
            {
                BindData = (List<AddMergeVendor>)Session["store7Vendor"];
            }
            if (vendorid != null && vendorname != null)
            {
                var IsActive = _vendorMasterRepository.getVendor(Convert.ToInt32(vendorid)).IsActive;
                BindData.Add(new AddMergeVendor() { VendorId = vendorid, VendorName = vendorname.ToString().Replace("^", "&"), Status = IsActive.ToString() });
            }
            Session["store7Vendor"] = BindData;
            IEnumerable DataSource = BindData;
            int count = DataSource.Cast<AddMergeVendor>().Count();

            return dm.RequiresCounts ? Json(new { result = DataSource, count = count }) : Json(DataSource);
        }

        [HttpPost]
        public ActionResult UpdateMergeVendorStore7(string mastervendorid)
        {
            List<GetMergeVendor> obj = new List<GetMergeVendor>(); List<MergeVendorInvoiceList> InvList = new List<MergeVendorInvoiceList>();
            string message = "";
            try
            {
                if (mastervendorid != "" && mastervendorid != null)
                {
                    if (Session["store7Vendor"] != null)
                    {
                        List<AddMergeVendor> BindData = new List<AddMergeVendor>();
                        BindData = (List<AddMergeVendor>)Session["store7Vendor"];
                        string arr = "";
                        foreach (var b in BindData)
                        {
                            arr += b.VendorId + ",";
                        }
                        InvList = _vendorMasterRepository.GetMergeVendorInvoiceList("GetAllInvoiceVendorId", arr, 7); ;

                        if (InvList != null)
                        {
                            foreach (var a in BindData)
                            {
                                _vendorMasterRepository.MergeInvoiceVendor("MergeInvoiceVendorId", mastervendorid, a.VendorId, 7);
                            }
                            MoveFile(InvList, mastervendorid);
                        }
                        Session["store7Vendor"] = null;
                        message = "success";
                        obj = _vendorMasterRepository.GetMergeVendor("SelectVendorForMerge", 7);
                    }
                }
            }
            catch (Exception ex)
            {
                message = "Something went wrong..!!";
            }
            return Json(new { message = message, obj });
        }

        public ActionResult AddVendorlistStore9(DataManagerRequest dm, string vendorid, string vendorname)
        {
            List<AddMergeVendor> BindData = new List<AddMergeVendor>();
            if (Session["store9Vendor"] != null)
            {
                BindData = (List<AddMergeVendor>)Session["store9Vendor"];
            }
            if (vendorid != null && vendorname != null)
            {
                var IsActive = _vendorMasterRepository.getVendor(Convert.ToInt32(vendorid)).IsActive;
                BindData.Add(new AddMergeVendor() { VendorId = vendorid, VendorName = vendorname.ToString().Replace("^", "&"), Status = IsActive.ToString() });
            }
            Session["store9Vendor"] = BindData;
            IEnumerable DataSource = BindData;
            int count = DataSource.Cast<AddMergeVendor>().Count();

            return dm.RequiresCounts ? Json(new { result = DataSource, count = count }) : Json(DataSource);
        }

        [HttpPost]
        public ActionResult UpdateMergeVendorStore9(string mastervendorid)
        {
            List<GetMergeVendor> obj = new List<GetMergeVendor>(); List<MergeVendorInvoiceList> InvList = new List<MergeVendorInvoiceList>();
            string message = "";
            try
            {
                if (mastervendorid != "" && mastervendorid != null)
                {
                    if (Session["store9Vendor"] != null)
                    {
                        List<AddMergeVendor> BindData = new List<AddMergeVendor>();
                        BindData = (List<AddMergeVendor>)Session["store9Vendor"];
                        string arr = "";
                        foreach (var b in BindData)
                        {
                            arr += b.VendorId + ",";
                        }
                        InvList = _vendorMasterRepository.GetMergeVendorInvoiceList("GetAllInvoiceVendorId", arr, 9); ;

                        if (InvList != null)
                        {
                            foreach (var a in BindData)
                            {
                                _vendorMasterRepository.MergeInvoiceVendor("MergeInvoiceVendorId", mastervendorid, a.VendorId, 9);
                            }
                            MoveFile(InvList, mastervendorid);
                        }
                        Session["store9Vendor"] = null;
                        message = "success";
                        obj = _vendorMasterRepository.GetMergeVendor("SelectVendorForMerge", 9);
                    }
                }
            }
            catch (Exception ex)
            {
                message = "Something went wrong..!!";
            }
            return Json(new { message = message, obj });
        }
        #endregion

        #region
        public void MoveFile(List<MergeVendorInvoiceList> objData, string mastervendorid)
        {
            try
            {
                foreach (var b in objData)
                {
                    string InvoiceType = "CR"; string DBPath = "";
                    if (b.InvoiceTypeId.ToString() == "1")
                        InvoiceType = "INV";

                    try
                    {
                        string URL = CreateDirectory(Convert.ToDateTime(b.InvoiceDate.ToString()).ToString("MMddyyyy"), mastervendorid, b.StoreId.ToString(), ref DBPath);
                        if (URL != "" && DBPath != "")
                        {
                            if ((Directory.Exists(URL)))
                            {
                                AdminSiteConfiguration.WriteErrorLogs("Directory created successfully");

                                string VName = _MastersBindRepository.GetVendorName(Convert.ToInt32(mastervendorid));
                                
                                if (VName != "")
                                {
                                    string InvNo = GetInvoiceNo(b.InvoiceNumber.ToString(), InvoiceType);
                                    if (InvNo != "")
                                    {
                                        string fileToMove = Request.PhysicalApplicationPath + "UserFiles\\Invoices" + "\\" + b.UploadInvoice.ToString();
                                        string moveTo = URL;
                                        string NewFileName = Convert.ToDateTime(b.InvoiceDate.ToString()).ToString("MMddyyyy") + "-" + VName + "-" + InvNo + "-" + InvoiceType + ".pdf";

                                        AdminSiteConfiguration.WriteErrorLogs("From Location: " + fileToMove);
                                        AdminSiteConfiguration.WriteErrorLogs("To Location: " + moveTo + "\\" + NewFileName);

                                        //moving file   
                                        System.IO.File.Move(fileToMove, moveTo + "\\" + NewFileName);

                                        AdminSiteConfiguration.WriteErrorLogs("File move successfully done...!!");
                                        Console.WriteLine("File move successfully done...!!");

                                        _vendorMasterRepository.UpdateUploadInvoice("UpdateUploadInvoice", b.InvoiceId.ToString(), DBPath + "\\" + NewFileName);
                                        //db.Database.ExecuteSqlCommand("SP_Invoice @Mode={0},@InvoiceId={1},@UploadInvoice={2}", "UpdateUploadInvoice", b.InvoiceId.ToString(), DBPath + "\\" + NewFileName);

                                        AdminSiteConfiguration.WriteErrorLogs("Path updated successfully in DB;Path:" + DBPath + "\\" + NewFileName + " ,InvoiceId: " + b.InvoiceId.ToString());
                                        Console.WriteLine("Path updated successfully in DB;Path:" + DBPath + "\\" + NewFileName + " ,InvoiceId: " + b.InvoiceId.ToString());
                                    }
                                    else
                                    {
                                        AdminSiteConfiguration.WriteErrorLogs("Get InvoiceNumber is blank after remove special characters; InvoiceNumber: " + InvNo);
                                        Console.WriteLine("Get InvoiceNumber is blank after remove special characters; InvoiceNumber: " + InvNo);
                                    }
                                }
                                else
                                {
                                    AdminSiteConfiguration.WriteErrorLogs("Get VendorName is blank by VendorId; VendorName: " + VName);
                                    Console.WriteLine("Get VendorName is blank by VendorId; VendorName: " + VName);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        AdminSiteConfiguration.WriteErrorLogs("Error:" + ex.Message + "; InvoiceId: " + b.InvoiceId.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                AdminSiteConfiguration.WriteErrorLogs("Error:Function:MoveFile;Message: " + ex.Message);
            }
        }

        public string CreateDirectory(string Date, string VendorId, string StoreId, ref string Path)
        {
            var RootURL = Request.PhysicalApplicationPath + "UserFiles\\Invoices" + "\\";
            var RelationalURL = RootURL + Date.Substring(4, 4);
            Path = Date.Substring(4, 4);
            try
            {
                if (!(Directory.Exists(RelationalURL)))
                {
                    Directory.CreateDirectory(RelationalURL);
                    RelationalURL = RelationalURL + "\\" + StoreId;
                    Path = Path + "\\" + StoreId;
                    if (!(Directory.Exists(RelationalURL)))
                    {
                        Directory.CreateDirectory(RelationalURL);
                        RelationalURL = RelationalURL + "\\" + _CommonRepository.GetMonthName(Convert.ToInt32(Date.Substring(0, 2)));
                        Path = Path + "\\" + _CommonRepository.GetMonthName(Convert.ToInt32(Date.Substring(0, 2)));
                        if (!(Directory.Exists(RelationalURL)))
                        {
                            Directory.CreateDirectory(RelationalURL);
                            RelationalURL = RelationalURL + "\\" + VendorId;
                            Path = Path + "\\" + VendorId;
                            if (!(Directory.Exists(RelationalURL)))
                            {
                                Directory.CreateDirectory(RelationalURL);
                                AdminSiteConfiguration.WriteErrorLogs("Created directory : " + RelationalURL);
                                Console.WriteLine("Created directory : " + RelationalURL);
                            }
                        }
                    }
                }
                else
                {
                    RelationalURL = RelationalURL + "\\" + StoreId;
                    Path = Path + "\\" + StoreId;
                    if (!(Directory.Exists(RelationalURL)))
                    {
                        Directory.CreateDirectory(RelationalURL);
                        RelationalURL = RelationalURL + "\\" + _CommonRepository.GetMonthName(Convert.ToInt32(Date.Substring(0, 2)));
                        Path = Path + "\\" + _CommonRepository.GetMonthName(Convert.ToInt32(Date.Substring(0, 2)));
                        if (!(Directory.Exists(RelationalURL)))
                        {
                            Directory.CreateDirectory(RelationalURL);
                            RelationalURL = RelationalURL + "\\" + VendorId;
                            Path = Path + "\\" + VendorId;
                            if (!(Directory.Exists(RelationalURL)))
                            {
                                Directory.CreateDirectory(RelationalURL);
                                AdminSiteConfiguration.WriteErrorLogs("Created directory : " + RelationalURL);
                                Console.WriteLine("Created directory : " + RelationalURL);
                            }
                        }
                    }
                    else
                    {
                        RelationalURL = RelationalURL + "\\" + _CommonRepository.GetMonthName(Convert.ToInt32(Date.Substring(0, 2)));
                        Path = Path + "\\" + _CommonRepository.GetMonthName(Convert.ToInt32(Date.Substring(0, 2)));
                        if (!(Directory.Exists(RelationalURL)))
                        {
                            Directory.CreateDirectory(RelationalURL);
                            RelationalURL = RelationalURL + "\\" + VendorId;
                            Path = Path + "\\" + VendorId;
                            if (!(Directory.Exists(RelationalURL)))
                            {
                                Directory.CreateDirectory(RelationalURL);
                                AdminSiteConfiguration.WriteErrorLogs("Created directory : " + RelationalURL);
                                Console.WriteLine("Created directory : " + RelationalURL);
                            }
                        }
                        else
                        {
                            RelationalURL = RelationalURL + "\\" + VendorId;
                            Path = Path + "\\" + VendorId;
                            if (!(Directory.Exists(RelationalURL)))
                            {
                                Directory.CreateDirectory(RelationalURL);
                                AdminSiteConfiguration.WriteErrorLogs("Created directory : " + RelationalURL);
                                Console.WriteLine("Created directory : " + RelationalURL);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                AdminSiteConfiguration.WriteErrorLogs("Function:CreateDirectory; Message:" + ex.Message);
            }
            return RelationalURL;
        }
        
        private static string GetInvoiceNo(string InvoiceNo, string InvoiceType)
        {
            string Inv = "";
            try
            {
                string invno = InvoiceNo;
                if (InvoiceType == "CR")
                {
                    string[] data = InvoiceNo.Split('_');
                    invno = data[0].ToString();
                }
                Inv = AdminSiteConfiguration.RemoveSpecialCharacterInvoice(invno);
            }
            catch (Exception ex)
            {
                AdminSiteConfiguration.WriteErrorLogs("Function:GetInvoiceNo; Message:" + ex.Message);
            }
            return Inv;
        }
        #endregion
    }
}