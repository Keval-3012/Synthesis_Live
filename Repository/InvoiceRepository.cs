using EntityModels.Models;
using Microsoft.SqlServer.Server;
using NLog;
using Repository.IRepository;
using Syncfusion.EJ2.Base;
using SynthesisQBOnline;
using SynthesisQBOnline.BAL;
using SynthesisViewModal;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Runtime.Remoting.Contexts;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using System.Web.Security;
using System.Web.UI.WebControls;
using Utility;
using VendorMaster = EntityModels.Models.VendorMaster;

namespace Repository
{
    public class InvoiceRepository : IInvoiceRepository
    {
        private DBContext db;
        UserActivityLogRepository activityLogRepository = null;
        CommonRepository commonRepository = null;
        QBRepository qBRepository = null;
        Logger logger = LogManager.GetCurrentClassLogger();
        protected static string ActivityLogMessage = "";

        public InvoiceRepository(DBContext context)
        {
            db = context;
            activityLogRepository = new UserActivityLogRepository(context);
            commonRepository = new CommonRepository(context);
            qBRepository = new QBRepository(context);

        }

        public List<InvoiceSelect> GetInvoiceSelects(string startdate, string enddate, string payment, int dept, string strStore, string AmtMaximum, string AmtMinimum, int Page, int PageSize, string orderby, string AscDsc, string searchdashbord, int UserTypeId, string SearchFlg)
        {
            List<InvoiceSelect> lstInvoice = new List<InvoiceSelect>();
            try
            {
                if (db.UserTypeMasters.Any(s => s.UserTypeId == UserTypeId && s.IsViewInvoiceOnly == true))
                {
                    lstInvoice = db.Database.SqlQuery<InvoiceSelect>("SP_InvoiceData_Dashboard_DepartmentWise @startdate={0},@enddate={1},@Payment_type={2},@Dept_id={3},@Store_id={4},@IsStatus_id={5},@AmtMaximum={6},@AmtMinimum={7},@startRowIndex={8},@pageSize={9},@OrderBy={10},@AscDsc={11},@searchbox={12},@UserTypeID ={13},@SearchType={14}", (startdate.ToString() == "" ? null : startdate.ToString()), (enddate.ToString() == "" ? null : enddate.ToString()), (payment == "" ? null : payment), dept, (strStore == "0" ? null : strStore), 0, (AmtMaximum == "" ? null : AmtMaximum), (AmtMinimum == "" ? null : AmtMinimum), Page, PageSize, orderby, AscDsc, searchdashbord.Replace("&amp;", "&"), UserTypeId, SearchFlg).ToList(); //.Replace("&amp;","&").Replace("&apos;","'")
                }
                else
                {

                    lstInvoice = db.Database.SqlQuery<InvoiceSelect>("SP_InvoiceData_Dashboard @startdate={0},@enddate={1},@Payment_type={2},@Dept_id={3},@Store_id={4},@IsStatus_id={5},@AmtMaximum={6},@AmtMinimum={7},@startRowIndex={8},@pageSize={9},@OrderBy={10},@AscDsc={11},@searchbox={12},@UserTypeID ={13},@SearchType={14}", (startdate.ToString() == "" ? null : startdate.ToString()), (enddate.ToString() == "" ? null : enddate.ToString()), (payment == "" ? null : payment), dept, (strStore == "0" ? null : strStore), 0, (AmtMaximum == "" ? null : AmtMaximum), (AmtMinimum == "" ? null : AmtMinimum), Page, PageSize, orderby, AscDsc, searchdashbord.Replace("&amp;", "&"), UserTypeId, SearchFlg).ToList(); //.Replace("&amp;","&").Replace("&apos;","'")
                }
            }
            catch (Exception ex)
            {
                logger.Error("InvoiceRepository - GetInvoiceSelects - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return lstInvoice;
        }

        public List<EntityModels.Models.DepartmentMaster> GetDepartments_WithDepartmentCond(int UserTypeId, int StoreId)
        {
            List<EntityModels.Models.DepartmentMaster> lstDepartmentMaster = new List<EntityModels.Models.DepartmentMaster>();
            try
            {
                var InvoiceDeptIds = db.InvoiceDepartmentDetails.Select(s => s.DepartmentId).Distinct().ToList();
                if (db.UserTypeMasters.Any(s => s.UserTypeId == UserTypeId && s.IsViewInvoiceOnly == true))
                {
                    var DeptRightsList = db.RightsStores.Where(s => s.UserTypeId == UserTypeId && s.StoreId == StoreId).Select(k => k.DepartmentId).ToList();
                    InvoiceDeptIds = db.InvoiceDepartmentDetails.Where(s => DeptRightsList.Contains(s.DepartmentId)).Select(s => s.DepartmentId).Distinct().ToList();
                }
                lstDepartmentMaster = db.DepartmentMasters.Where(s => s.IsActive == true && s.StoreId == StoreId && InvoiceDeptIds.Contains(s.DepartmentId)).OrderBy(o => o.DepartmentName).ToList();

            }
            catch (Exception ex)
            {
                logger.Error("InvoiceRepository - GetDepartments_WithDepartmentCond - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return lstDepartmentMaster;
        }

        public string GetInvoice_StoreList(int StoreId, bool RoleFlg, int UserId)
        {
            string strList = "";
            try
            {
                if (RoleFlg == false)
                {
                    if (StoreId == 0)
                    {
                        var list = db.Database.SqlQuery<int>("SP_GetStore_ForDashboard @Mode={0},@ModuleId={1},@UserTypeId={2}", "GetStore_ForDashboard", 1, UserId);
                        if (list != null) { strList = String.Join(",", list); }
                        list = null;
                    }
                    else
                    {
                        strList = StoreId.ToString();
                    }
                }
                else
                {
                    strList = StoreId.ToString();
                }
            }
            catch (Exception ex)
            {
                logger.Error("InvoiceRepository - GetInvoice_StoreList - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return strList;
        }

        public List<EntityModels.Models.DepartmentMaster> getDepartment_WithSP(int StoreId)
        {
            List<EntityModels.Models.DepartmentMaster> DepartmentList = new List<EntityModels.Models.DepartmentMaster>();
            try
            {
                DepartmentList = db.Database.SqlQuery<EntityModels.Models.DepartmentMaster>("SP_DepartmentMaster @Mode = {0},@StoreId = {1}", "SelectExpense_Department", StoreId).ToList();
                DepartmentList = DepartmentList.Where(s => s.StoreId == StoreId).ToList();

            }
            catch (Exception ex)
            {
                logger.Error("InvoiceRepository - GetDepartment_WithSP - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return DepartmentList;
        }

        public List<InvoiceSelect> InvoiceDataDepartmentWise_Get(string deptid, string storeid, int UserTypeId, DataManagerRequest dm)
        {
            List<InvoiceSelect> BindData = new List<InvoiceSelect>();

            try
            {
                if (db.UserTypeMasters.Any(s => s.UserTypeId == UserTypeId && s.IsViewInvoiceOnly == true))
                {
                    if (storeid != "" && deptid != "" && storeid != null && deptid != null && storeid != "0")
                    {
                        BindData = db.Database.SqlQuery<InvoiceSelect>("SP_InvoiceData_Dashboard_DepartmentWise_Beta @startdate={0},@enddate={1},@Payment_type={2},@Dept_id={3},@Store_id={4},@IsStatus_id={5},@AmtMaximum={6},@AmtMinimum={7},@startRowIndex={8},@pageSize={9},@OrderBy={10},@AscDsc={11},@searchbox={12},@UserTypeID ={13},@SearchType={14}", null, null, null, deptid, storeid, 0, null, null, dm.Skip, dm.Take, "InvoiceId", "Desc", "", UserTypeId, "S").ToList(); //.Replace("&amp;","&").Replace("&apos;","'")
                    }
                    else if ((deptid == null || deptid == "") && (storeid != "" && storeid != null && storeid != "0"))
                    {
                        BindData = db.Database.SqlQuery<InvoiceSelect>("SP_InvoiceData_Dashboard_DepartmentWise_Beta @startdate={0},@enddate={1},@Payment_type={2},@Dept_id={3},@Store_id={4},@IsStatus_id={5},@AmtMaximum={6},@AmtMinimum={7},@startRowIndex={8},@pageSize={9},@OrderBy={10},@AscDsc={11},@searchbox={12},@UserTypeID ={13},@SearchType={14}", null, null, null, null, storeid, 0, null, null, dm.Skip, dm.Take, "InvoiceId", "Desc", "", UserTypeId, "S").ToList(); //.Replace("&amp;","&").Replace("&apos;","'")
                    }
                    else
                    {
                        BindData = db.Database.SqlQuery<InvoiceSelect>("SP_InvoiceData_Dashboard_DepartmentWise_Beta @startdate={0},@enddate={1},@Payment_type={2},@Dept_id={3},@Store_id={4},@IsStatus_id={5},@AmtMaximum={6},@AmtMinimum={7},@startRowIndex={8},@pageSize={9},@OrderBy={10},@AscDsc={11},@searchbox={12},@UserTypeID ={13},@SearchType={14}", null, null, null, null, null, 0, null, null, dm.Skip, dm.Take, "InvoiceId", "Desc", "", UserTypeId, "S").ToList(); //.Replace("&amp;","&").Replace("&apos;","'")
                    }
                }
                else
                {
                    if (storeid != "" && deptid != "" && storeid != null && deptid != null && storeid != "0")
                    {
                        BindData = db.Database.SqlQuery<InvoiceSelect>("SP_InvoiceData_Dashboard_Beta @startdate={0},@enddate={1},@Payment_type={2},@Dept_id={3},@Store_id={4},@IsStatus_id={5},@AmtMaximum={6},@AmtMinimum={7},@startRowIndex={8},@pageSize={9},@OrderBy={10},@AscDsc={11},@searchbox={12},@UserTypeID ={13},@SearchType={14}", null, null, null, deptid, storeid, "", null, null, dm.Skip, dm.Take, "InvoiceId", "Desc", "", UserTypeId, "F").ToList(); //.Replace("&amp;","&").Replace("&apos;","'")
                    }
                    else if ((deptid == null || deptid == "") && (storeid != "" && storeid != null && storeid != "0"))
                    {
                        BindData = db.Database.SqlQuery<InvoiceSelect>("SP_InvoiceData_Dashboard_Beta @startdate={0},@enddate={1},@Payment_type={2},@Dept_id={3},@Store_id={4},@IsStatus_id={5},@AmtMaximum={6},@AmtMinimum={7},@startRowIndex={8},@pageSize={9},@OrderBy={10},@AscDsc={11},@searchbox={12},@UserTypeID ={13},@SearchType={14}", null, null, null, null, storeid, "", null, null, dm.Skip, dm.Take, "InvoiceId", "Desc", "", UserTypeId, "F").ToList(); //.Replace("&amp;","&").Replace("&apos;","'")
                    }
                    else
                    {
                        BindData = db.Database.SqlQuery<InvoiceSelect>("SP_InvoiceData_Dashboard_Beta @startdate={0},@enddate={1},@Payment_type={2},@Dept_id={3},@Store_id={4},@IsStatus_id={5},@AmtMaximum={6},@AmtMinimum={7},@startRowIndex={8},@pageSize={9},@OrderBy={10},@AscDsc={11},@searchbox={12},@UserTypeID ={13},@SearchType={14}", "", "", "", "", null, "", null, null, dm.Skip, dm.Take, "InvoiceId", "Desc", "", UserTypeId, "S").ToList(); //.Replace("&amp;","&").Replace("&apos;","'")
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("InvoiceRepository - InvoiceDataDepartmentWise_Get - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return BindData;
        }

        public Invoice getInvoiceObj(Invoice invoice, string val)
        {
            try
            {
                invoice.CheckDup = 0;
                invoice.DepartmentMasters = db.DepartmentMasters.Where(s => s.IsActive == true && s.DepartmentId == 0).ToList();
                UploadPDFAutomation UploadFile = new UploadPDFAutomation();
                if (!String.IsNullOrEmpty(val))
                {
                    UploadFile = db.UploadPDFAutomation.Find(Convert.ToInt32(val));
                    invoice.UploadInvoice = UploadFile.FileName;
                    invoice.StoreId = (int)UploadFile.StoreId;
                    invoice.UploadPdfAutomationId = UploadFile.UploadPdfAutomationId;
                }
            }
            catch (Exception ex)
            {
                logger.Error("InvoiceRepository - GetInvoiceObj - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return invoice;
        }

        public Invoice EditInvoiceObj(int id)
        {
            Invoice invoice = new Invoice();
            try
            {
                invoice = db.Invoices.Find(id);
                invoice.CheckDup = 0;
                int aa = db.Invoices.Where(a => a.RefInvoiceId == id).Select(a => a.InvoiceId).FirstOrDefault();
                invoice.Disc_Dept_id = db.InvoiceDepartmentDetails.Where(a => a.InvoiceId == aa).Select(a => a.DepartmentId).FirstOrDefault();

                if (invoice.VendorMasters.Address == "" || invoice.VendorMasters.Address == null)
                {
                    invoice.VendorMasters.Address = "Not Available";
                }
                else
                {
                    invoice.VendorMasters.Address = (invoice.VendorMasters.Address != null ? (invoice.VendorMasters.Address != "" ? invoice.VendorMasters.Address + "," : "") : "") + (invoice.VendorMasters.Address2 != null ? (invoice.VendorMasters.Address2 != "" ? invoice.VendorMasters.Address2 + "," : "") : "") + (invoice.VendorMasters.City != null ? (invoice.VendorMasters.City != "" ? invoice.VendorMasters.City + "," : "") : "") + (invoice.VendorMasters.State != null ? (invoice.VendorMasters.State != "" ? invoice.VendorMasters.State + " " : "") : "") + (invoice.VendorMasters.Country != null ? (invoice.VendorMasters.Country != "" ? invoice.VendorMasters.Country + " " : "") : "") + (invoice.VendorMasters.PostalCode != null ? (invoice.VendorMasters.PostalCode != "" ? invoice.VendorMasters.PostalCode : "") : "");
                }
                if (invoice.VendorMasters.PhoneNumber == "" || invoice.VendorMasters.PhoneNumber == null)
                {
                    invoice.VendorMasters.PhoneNumber = "Not Available";
                }
                invoice.strInvoiceDate = AdminSiteConfiguration.GetDateTime(invoice.InvoiceDate);
                invoice.StoreName = invoice.StoreMasters.Name;
                if (invoice.CreatedBy != 0 && invoice.CreatedBy != null)
                {
                    invoice.CreatedByUserName = db.UserMasters.Find(invoice.CreatedBy) != null ? db.UserMasters.Find(invoice.CreatedBy).FirstName : "";
                }
                if (invoice.ModifiedBy != 0 && invoice.ModifiedBy != null)
                {
                    invoice.ModifiedByUserName = db.UserMasters.Find(invoice.ModifiedBy) != null ? db.UserMasters.Find(invoice.ModifiedBy).FirstName : "";
                }
                if (invoice.ApproveRejectBy != 0 && invoice.ApproveRejectBy != null)
                {
                    invoice.ApproveRejectUserName = db.UserMasters.Find(invoice.ApproveRejectBy) != null ? db.UserMasters.Find(invoice.ApproveRejectBy).FirstName : "";
                }
                invoice.invoiceProducts = new List<InvoiceProduct>();
                invoice.DepartmentMasters = db.Database.SqlQuery<EntityModels.Models.DepartmentMaster>("SP_DepartmentMaster @Mode = {0},@StoreId = {1}", "SelectExpense_Department", invoice.StoreId).ToList().Where(s => s.StoreId == invoice.StoreId).ToList();

            }
            catch (Exception ex)
            {
                logger.Error("InvoiceRepository - EditInvoiceObj - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return invoice;
        }

        public Invoice deleteInvoice(int id, string From)
        {
            Invoice invoice = new Invoice();
            try
            {
                db.Database.ExecuteSqlCommand("SP_Invoice @Mode = {0},@InvoiceId = {1}", "DeleteFromErrorLog", id);
                if (From == "CashPaidOut")
                {
                    db.Database.ExecuteSqlCommand("SP_Invoice @Mode = {0},@InvoiceId = {1}", "DeleteFromCashPaidoutInvoice", id);
                }
                invoice = db.Invoices.Find(id);

                db.Invoices.Remove(invoice);
                db.SaveChanges();

            }
            catch (Exception ex)
            {
                logger.Error("InvoiceRepository - DeleteInvoice - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return invoice;
        }

        public Invoice invoiceDetail(int Id, int UserId)
        {
            Invoice Data = new Invoice();
            try
            {
                Data = db.Invoices.Find(Id);
                Data.VendorName = Data.VendorMasters.VendorName;
                Data.VendorPhoneNumber = Data.VendorMasters.PhoneNumber;
                Data.VendorAddress = (Data.VendorMasters.Address != null ? (Data.VendorMasters.Address != "" ? Data.VendorMasters.Address + "," : "") : "") + (Data.VendorMasters.Address2 != null ? (Data.VendorMasters.Address2 != "" ? Data.VendorMasters.Address2 + "," : "") : "") + (Data.VendorMasters.City != null ? (Data.VendorMasters.City != "" ? Data.VendorMasters.City + "," : "") : "") + (Data.VendorMasters.State != null ? (Data.VendorMasters.State != "" ? Data.VendorMasters.State + " " : "") : "") + (Data.VendorMasters.Country != null ? (Data.VendorMasters.Country != "" ? Data.VendorMasters.Country + " " : "") : "") + (Data.VendorMasters.PostalCode != null ? (Data.VendorMasters.PostalCode != "" ? Data.VendorMasters.PostalCode : "") : "");
                Data.StoreName = Data.StoreMasters.Name;
                Data.PaymentType = Data.PaymentTypeMasters.PaymentType;
                Data.InvoiceType = Data.InvoiceTypeMasters.InvoiceType;
                var aa = db.Invoices.Where(a => a.RefInvoiceId == Id).FirstOrDefault();
                if (aa != null)
                {
                    Data.RefInvoiceNumber = aa.InvoiceNumber;
                    Data.RefDiscountAmount = aa.TotalAmount;
                }
                if (Data.ApproveRejectBy != null)
                {
                    Data.ApproveRejectUserName = db.UserMasters.Find(Data.ApproveRejectBy) != null ? db.UserMasters.Find(Data.ApproveRejectBy).FirstName : "";
                }
                if (Data.CreatedBy != null)
                {
                    Data.CreatedByUserName = db.UserMasters.Find(Data.CreatedBy) != null ? db.UserMasters.Find(Data.CreatedBy).FirstName : "";
                }
                if (Data.ModifiedBy != null)
                {
                    Data.ModifiedByUserName = db.UserMasters.Find(Data.ModifiedBy) != null ? db.UserMasters.Find(Data.ModifiedBy).FirstName : "";
                }
                if (!Roles.IsUserInRole("Administrator"))
                {
                    var UserTypeId = db.UserMasters.Find(Data.CreatedBy).UserTypeId;
                    var UserModuleId = db.ModuleMasters.Where(s => s.ModuleName == "Invoice").FirstOrDefault().ModuleId;

                    var LevelApprove = db.UserTypeModuleApprovers.Where(s => s.UserTypeId == UserTypeId && s.ModuleId == UserModuleId).Count() > 0 ? db.UserTypeModuleApprovers.Where(s => s.UserTypeId == UserTypeId && s.ModuleId == UserModuleId).FirstOrDefault().LevelsApproverId : 0;
                    if (LevelApprove > 0)
                    {
                        var CurrentUserLevel = db.UserMasters.Find(UserId).UserTypeMasters.LevelsApproverId;
                        if (LevelApprove == CurrentUserLevel)
                        {
                            Data.invoiceAproveFlg = true;
                        }
                    }
                }
                var QBFlag = db.QBOnlineConfigurations.Where(a => a.StoreId == Data.StoreId).FirstOrDefault();
                if (QBFlag != null)
                {
                    if (QBFlag.IsTokenSuspend == 1)
                    {
                        Data.QBStatusField = true;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("InvoiceRepository - invoiceDetail - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return Data;
        }

        public Invoice invoiceDetail(int id)
        {
            throw new NotImplementedException();
        }

        public void InvoiceDetailSave(Invoice PostedData, int ID, string FirstName, int UserId)
        {
            try
            {
                Invoice Invoice_data = db.Invoices.Find(ID);

                var Store = qBRepository.GetStoreOnlineDesktop(Invoice_data.StoreId);
                int Flag = qBRepository.GetStoreOnlineDesktopFlag(Invoice_data.StoreId);
                QBResponse objResponse = new QBResponse();
                string fullname = FirstName;
                Invoice_data.strInvoiceDate = Invoice_data.InvoiceDate.ToString("dd/MM/yyyy");
                Invoice_data.Note = PostedData.Note;
                if (Invoice_data.StatusValue == InvoiceStatusEnm.Pending)
                {
                    if (PostedData.QBtransferss == "1")
                    {
                        Invoice_data.QBTransfer = true;
                    }
                    else
                    {
                        Invoice_data.QBTransfer = false;
                    }
                }

                Invoice_data.ModifiedBy = UserId;
                Invoice_data.ModifiedOn = AdminSiteConfiguration.GetEasternTime(DateTime.Now);
                Invoice_data.LastModifiedOn = AdminSiteConfiguration.GetEasternTime(DateTime.Now);
                db.Entry(Invoice_data).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();

                string ActivityLogMessage = "Invoice Note with this Invoice Number " + "<a href='/Invoices/Details/" + Invoice_data.InvoiceNumber + "'>" + Invoice_data.InvoiceNumber + "</a> Edited by " + fullname + " on " + AdminSiteConfiguration.GetDate(AdminSiteConfiguration.GetEasternTime(DateTime.Now).ToString());
                ActivityLog ActLog2 = new ActivityLog();
                ActLog2.Action = 2;
                ActLog2.Comment = ActivityLogMessage;
                activityLogRepository.ActivityLogInsert(ActLog2);

                if (Store == "Online" && Flag == 1 && Invoice_data.PaymentTypeId == 2) //Check/ACH
                {
                    if (Invoice_data.TXNId.ToString() != "")
                    {
                        if (PostedData.QBTransfer == false)
                        {
                            if (Invoice_data.InvoiceTypeId == 2) //CreditMemo
                            {
                                List<VendorCredirDetail> objList = new List<VendorCredirDetail>();
                                var invDept = db.Database.SqlQuery<InvoiceDepartmentDetail>("SPInvoiceDepartment @Mode = {0},@InvoiceId = {1}", "GetInvoiceDepartment", ID).ToList();
                                foreach (var item in invDept)
                                {
                                    VendorCredirDetail objDetail = new VendorCredirDetail();
                                    objDetail.Description = "";
                                    objDetail.Amount = Convert.ToDecimal(item.Amount);
                                    objDetail.DepartmentID = item.DepartmentListId;
                                    objList.Add(objDetail);
                                    objDetail = null;
                                }

                                qBRepository.QBEditVendorCreditData(Invoice_data.InvoiceId, Invoice_data.TXNId, Invoice_data.InvoiceNumber, Convert.ToDateTime(Invoice_data.InvoiceDate), PostedData.Note, Convert.ToInt32(Invoice_data.StoreId), Convert.ToInt32(Invoice_data.VendorId), objList, ref objResponse);
                            }
                            else
                            {
                                List<BillDetail> objList = new List<BillDetail>();
                                var invDept = db.Database.SqlQuery<InvoiceDepartmentDetail>("SPInvoiceDepartment @Mode = {0},@InvoiceId = {1}", "GetInvoiceDepartment", ID).ToList();
                                foreach (var item in invDept)
                                {
                                    BillDetail objDetail = new BillDetail();
                                    objDetail.Description = "";
                                    objDetail.Amount = Convert.ToDecimal(item.Amount);
                                    objDetail.DepartmentID = db.DepartmentMasters.Where(a => a.DepartmentId == item.DepartmentId).FirstOrDefault().ListId;
                                    objList.Add(objDetail);
                                    objDetail = null;
                                }
                                qBRepository.QBEditBillData(Invoice_data.InvoiceId, Invoice_data.TXNId, Invoice_data.InvoiceNumber, Convert.ToDateTime(Invoice_data.InvoiceDate), PostedData.Note, Convert.ToInt32(Invoice_data.StoreId), Convert.ToInt32(Invoice_data.VendorId), objList, ref objResponse);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("InvoiceRepository - InvoiceDetailSave - " + DateTime.Now + " - " + ex.Message.ToString());
            }
        }

        public void InvoiceReject(int Id, int UserId)
        {
            try
            {
                Invoice Invoice_data = db.Invoices.Find(Id);
                Invoice_data.strInvoiceDate = Invoice_data.InvoiceDate.ToString("dd/MM/yyyy");
                string fullname = db.UserMasters.Where(x => x.UserId == UserId).Select(x => x.FirstName).FirstOrDefault();
                Invoice_data.StatusValue = InvoiceStatusEnm.Rejected;
                Invoice_data.ApproveRejectBy = UserId;
                Invoice_data.ApproveRejectDate = AdminSiteConfiguration.GetEasternTime(DateTime.Now);
                db.Entry(Invoice_data).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();

                //activity Log
                ActivityLog ActLog = new ActivityLog();
                ActLog.Action = 5;
                ActLog.Comment = "Invoice Number " + "<a href='/Invoices/Details/" + Invoice_data.InvoiceId + "'>" + Invoice_data.InvoiceNumber + "</a> Rejected by " + fullname + " on " + AdminSiteConfiguration.GetDate(AdminSiteConfiguration.GetEasternTime(DateTime.Now).ToString());
                activityLogRepository.ActivityLogInsert(ActLog);

                //for remove twise log data from userlog table
                db.Database.ExecuteSqlCommand("SP_UserLogUpdate @Mode = {0}", "RemoveTwiseLogEntry");
            }
            catch (Exception ex)
            {
                logger.Error("InvoiceRepository - InvoiceReject - " + DateTime.Now + " - " + ex.Message.ToString());
            }
        }

        public void InvoiceApprove(int Id, int qbtransfer)
        {
            try
            {
                Invoice Invoice_data = db.Invoices.Find(Id);
                string Store = qBRepository.GetStoreOnlineDesktop(Convert.ToInt32(Invoice_data.StoreId));
                int StoreFlag = qBRepository.GetStoreOnlineDesktopFlag(Convert.ToInt32(Invoice_data.StoreId));
                var UserName = System.Web.HttpContext.Current.User.Identity.Name;
                int userid = commonRepository.getUserId(UserName);
                string fullname = commonRepository.getUserFirstName(UserName);

                if (Store != "")
                {
                    if (Invoice_data != null)
                    {
                        if (Store == "Online" && StoreFlag == 1 && Invoice_data.PaymentTypeId == 2) //Check/ACH
                        {
                            try
                            {
                                QBResponse objResponse = new QBResponse();
                                if (Invoice_data.QBTransfer == true)
                                {
                                    Invoice_data.strInvoiceDate = Invoice_data.InvoiceDate.ToString("dd/MM/yyyy");
                                    Invoice_data.ApproveRejectBy = db.UserMasters.Where(s => s.UserName == UserName).FirstOrDefault().UserId;
                                    Invoice_data.ApproveRejectDate = DateTime.Now;
                                    Invoice_data.LastModifiedOn = DateTime.Now;
                                    Invoice_data.StatusValue = InvoiceStatusEnm.Approved;

                                    db.Entry(Invoice_data).State = System.Data.Entity.EntityState.Modified;
                                    db.SaveChanges();

                                    ActivityLogMessage = "Invoice Number " + "<a href='/Invoices/Details/" + Invoice_data.InvoiceId + "'>" + Invoice_data.InvoiceNumber + "</a> Approved by " + fullname + " on " + AdminSiteConfiguration.GetDate(AdminSiteConfiguration.GetEasternTime(DateTime.Now).ToString());

                                    ActivityLog ActLog1 = new ActivityLog();
                                    ActLog1.Action = 1;
                                    ActLog1.Comment = ActivityLogMessage;
                                    activityLogRepository.ActivityLogInsert(ActLog1);
                                }
                                else
                                {
                                    Invoice_data.strInvoiceDate = Invoice_data.InvoiceDate.ToString("dd/MM/yyyy");
                                    Invoice_data.ApproveRejectBy = db.UserMasters.Where(s => s.UserName == UserName).FirstOrDefault().UserId;
                                    Invoice_data.ApproveRejectDate = DateTime.Now;
                                    Invoice_data.LastModifiedOn = DateTime.Now;
                                    Invoice_data.StatusValue = InvoiceStatusEnm.Approved;
                                    Invoice_data.IsSync = -1;
                                    db.Entry(Invoice_data).State = System.Data.Entity.EntityState.Modified;
                                    db.SaveChanges();
                                    var Invoice_data_IsSync = 0;
                                    ActivityLogMessage = "Invoice Number " + "<a href='/Invoices/Details/" + Invoice_data.InvoiceId + "'>" + Invoice_data.InvoiceNumber + "</a> Approved by " + fullname + " on " + AdminSiteConfiguration.GetDate(AdminSiteConfiguration.GetEasternTime(DateTime.Now).ToString());

                                    ActivityLog ActLog1 = new ActivityLog();
                                    ActLog1.Action = 1;
                                    ActLog1.Comment = ActivityLogMessage;
                                    activityLogRepository.ActivityLogInsert(ActLog1);

                                    if (Invoice_data.InvoiceTypeId == 2) //Invoice = 1, CreditMemo =2
                                    {
                                        if ((Invoice_data.TXNId == "" || Invoice_data.TXNId == null) && Invoice_data_IsSync == 0)
                                        {
                                            List<VendorCredirDetail> objList = new List<VendorCredirDetail>();

                                            var invDept = db.InvoiceDepartmentDetails.Where(s => s.InvoiceId == Invoice_data.InvoiceId).ToList();
                                            foreach (var item in invDept)
                                            {
                                                VendorCredirDetail objDetail = new VendorCredirDetail();
                                                objDetail.Description = "";
                                                objDetail.Amount = Convert.ToDecimal(item.Amount);
                                                objDetail.DepartmentID = db.DepartmentMasters.Where(a => a.StoreId == Invoice_data.StoreId && a.DepartmentId == item.DepartmentId).Count() > 0 ? db.DepartmentMasters.Where(a => a.StoreId == Invoice_data.StoreId && a.DepartmentId == item.DepartmentId).FirstOrDefault().ListId : ""; ;
                                                objList.Add(objDetail);
                                                objDetail = null;
                                            }
                                            qBRepository.CreateVendorCredit(Convert.ToInt32(Invoice_data.InvoiceId), objList, 0);
                                            objList = null;
                                        }

                                    }
                                    else
                                    {
                                        if ((Invoice_data.TXNId == "" || Invoice_data.TXNId == null) && Invoice_data_IsSync == 0)
                                        {
                                            List<BillDetail> objList = new List<BillDetail>();
                                            var invDept = db.InvoiceDepartmentDetails.Where(s => s.InvoiceId == Invoice_data.InvoiceId).ToList();

                                            if (invDept != null)
                                            {
                                                foreach (var val_id in invDept)
                                                {
                                                    BillDetail objDetail = new BillDetail();
                                                    objDetail.Description = "";
                                                    objDetail.Amount = Convert.ToDecimal(val_id.Amount);
                                                    objDetail.DepartmentID = db.DepartmentMasters.Where(a => a.StoreId == Invoice_data.StoreId && a.DepartmentId == val_id.DepartmentId).Count() > 0 ? db.DepartmentMasters.Where(a => a.StoreId == Invoice_data.StoreId && a.DepartmentId == val_id.DepartmentId).FirstOrDefault().ListId : "";
                                                    objList.Add(objDetail);
                                                    objDetail = null;
                                                }
                                            }

                                            qBRepository.CreateBill(Convert.ToInt32(Invoice_data.InvoiceId), objList);
                                            objList = null;
                                        }
                                    }
                                }


                            }
                            catch (Exception ex)
                            {
                                logger.Error("InvoiceRepository - InvoiceApprove - " + DateTime.Now + " - " + ex.Message.ToString());
                            }
                        }
                        else
                        {
                            Invoice_data.strInvoiceDate = Invoice_data.InvoiceDate.ToString("dd/MM/yyyy");
                            Invoice_data.StatusValue = InvoiceStatusEnm.Approved;
                            Invoice_data.ApproveRejectBy = userid;
                            Invoice_data.ApproveRejectDate = AdminSiteConfiguration.GetEasternTime(DateTime.Now);
                            Invoice_data.LastModifiedOn = AdminSiteConfiguration.GetEasternTime(DateTime.Now);
                            Invoice_data.IsSync = 0;
                            db.Entry(Invoice_data).State = System.Data.Entity.EntityState.Modified;
                            db.SaveChanges();

                            //---- activity Log -----//
                            ActivityLog ActLog = new ActivityLog();
                            ActLog.Action = 4;
                            ActLog.Comment = "Invoice Number " + "<a href='/Invoices/Details/" + Invoice_data.InvoiceId + "'>" + Invoice_data.InvoiceNumber + "</a> Approved by " + fullname + " on " + AdminSiteConfiguration.GetDate(AdminSiteConfiguration.GetEasternTime(DateTime.Now).ToString());
                            activityLogRepository.ActivityLogInsert(ActLog);
                        }
                    }
                }
                else
                {
                    Invoice_data.strInvoiceDate = Invoice_data.InvoiceDate.ToString("dd/MM/yyyy");
                    Invoice_data.StatusValue = InvoiceStatusEnm.Approved;
                    Invoice_data.IsSync = 0;
                    Invoice_data.ApproveRejectBy = userid;
                    Invoice_data.ApproveRejectDate = AdminSiteConfiguration.GetEasternTime(DateTime.Now);
                    Invoice_data.LastModifiedOn = AdminSiteConfiguration.GetEasternTime(DateTime.Now);
                    db.Entry(Invoice_data).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();

                    //---- activity Log -----//
                    ActivityLog ActLog = new ActivityLog();
                    ActLog.Action = 4;
                    ActLog.Comment = "Invoice Number " + "<a href='/Invoices/Details/" + Invoice_data.InvoiceId + "'>" + Invoice_data.InvoiceNumber + "</a> Approved by " + fullname + " on " + AdminSiteConfiguration.GetDate(AdminSiteConfiguration.GetEasternTime(DateTime.Now).ToString());
                    activityLogRepository.ActivityLogInsert(ActLog);
                }
                //for remove twise log data from userlog table
                db.Database.ExecuteSqlCommand("SP_UserLogUpdate @Mode = {0}", "RemoveTwiseLogEntry");
            }
            catch (Exception ex)
            {
                logger.Error("InvoiceRepository - InvoiceApprove1 - " + DateTime.Now + " - " + ex.Message.ToString());
            }
        }

        public void InvoiceOnHold(int Id)
        {
            try
            {
                Invoice Invoice_data = db.Invoices.Find(Id);
                Invoice_data.strInvoiceDate = Invoice_data.InvoiceDate.ToString("dd/MM/yyyy");
                var UserName = System.Web.HttpContext.Current.User.Identity.Name;
                int userid = commonRepository.getUserId(UserName);
                string fullname = db.UserMasters.Where(x => x.UserId == userid).Select(x => x.FirstName).FirstOrDefault();
                Invoice_data.StatusValue = InvoiceStatusEnm.OnHold;
                db.Entry(Invoice_data).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();

                //activity Log
                ActivityLog ActLog = new ActivityLog();
                ActLog.Action = 6;
                ActLog.Comment = "Invoice Number " + "<a href='/Invoices/Details/" + Invoice_data.InvoiceId + "'>" + Invoice_data.InvoiceNumber + "</a> On Hold by " + fullname + " on " + AdminSiteConfiguration.GetDate(AdminSiteConfiguration.GetEasternTime(DateTime.Now).ToString());
                activityLogRepository.ActivityLogInsert(ActLog);

                //for remove twise log data from userlog table
                db.Database.ExecuteSqlCommand("SP_UserLogUpdate @Mode = {0}", "RemoveTwiseLogEntry");
            }
            catch (Exception ex)
            {
                logger.Error("InvoiceRepository - InvoiceOnHold - " + DateTime.Now + " - " + ex.Message.ToString());
            }
        }

        public void Dispose(bool disposing)
        {
            try
            {
                if (disposing)
                {
                    db.Dispose();
                }
            }
            catch (Exception ex)
            {
                logger.Error("InvoiceRepository - Dispose - " + DateTime.Now + " - " + ex.Message.ToString());
            }
        }

        public InvoicesViewModel CheckExistence(string vendorid, string invoiceno, string invoicedate, string type, string storeid, string invoiceid, decimal totalamtvalue)
        {
            InvoicesViewModel data = new InvoicesViewModel();
            try
            {
                string message = "";
                int inid = 0;
                if (!string.IsNullOrEmpty(invoiceid))
                {
                    inid = Convert.ToInt32(invoiceid);
                }
                DateTime invoice_Date = Convert.ToDateTime(invoicedate);
                int typeval = Convert.ToInt32(type);
                int storeval = Convert.ToInt32(storeid);
                int vendoridval = Convert.ToInt32(vendorid);
                var exists = db.Database.SqlQuery<CheckExistInvoice>("SP_CheckInvoice_Exist @InvoiceNumber = {0},@InvoiceDate = {1},@StoreId = {2},@InvoiceTypeId = {3},@VendorId = {4},@InvoiceId = {5},@Amount={6}", invoiceno, invoice_Date, storeval, typeval, vendoridval, inid, totalamtvalue).FirstOrDefault();
                if (exists.InvoiceId == 0)
                {
                    message = "";
                }
                else
                {
                    if (exists.InvoiceDate == "")
                    {
                        message = "main";
                        string idval = exists.InvoiceId.ToString();
                        data.idval = idval;
                    }
                    else
                    {
                        message = "sub";
                        data.InvDate = exists.InvoiceDate.ToString();
                        data.idval = exists.InvoiceId.ToString();
                        data.InvoiceNumber = exists.InvoiceNumber.ToString();
                    }
                }

                //var exists = db.Database.SqlQuery<int>("SP_CheckInvoice_Exist @InvoiceNumber = {0},@InvoiceDate = {1},@StoreId = {2},@InvoiceTypeId = {3},@VendorId = {4},@InvoiceId = {5},@Amount={6}", invoiceno, invoice_Date, storeval, typeval, vendoridval, inid,totalamtvalue).FirstOrDefault();
                //if (exists == 0)
                //{
                //    var existence = db.Database.SqlQuery<CheckExistInvoice>("SP_CheckInvoice_Exist_Detail @InvoiceNumber = {0},@StoreId = {1},@InvoiceTypeId = {2},@VendorId = {3},@InvoiceId = {4}", invoiceno, storeval, typeval, vendoridval, inid).FirstOrDefault();
                //    if (existence.InvoiceId > 0)
                //    {
                //        message = "sub";
                //        data.InvDate = existence.InvoiceDate.ToString();
                //        data.idval = existence.InvoiceId.ToString();
                //        data.InvoiceNumber = existence.InvoiceNumber.ToString();
                //    }
                //    else
                //    {
                //        message = "";
                //    }
                //    var rows = db.Database.SqlQuery<CheckExistInvoice>("SP_CheckInvoice_Exist_Detail @InvoiceNumber = {0},@StoreId = {1},@InvoiceTypeId = {2},@VendorId = {3},@InvoiceId = {4}", invoiceno, storeval, typeval, vendoridval, inid).ToArray();

                //}
                //else
                //{
                //    var existence = db.Database.SqlQuery<CheckExistInvoice>("SP_CheckInvoice_Exist_Amount_Detail @InvoiceNumber = {0},@StoreId = {1},@InvoiceTypeId = {2},@VendorId = {3},@InvoiceId = {4},@Amount = {5},@InvoiceDate = {6}", invoiceno, storeval, typeval, vendoridval, inid, totalamtvalue, invoice_Date).FirstOrDefault();
                //    if (existence.InvoiceId == 0)
                //    {
                //        if (inid == 0)
                //        {
                //            message = "sub";
                //            var rows = db.Database.SqlQuery<CheckExistInvoice>("SP_CheckInvoice_Exist_Detail @InvoiceNumber = {0},@StoreId = {1},@InvoiceTypeId = {2},@VendorId = {3},@InvoiceId = {4}", invoiceno, storeval, typeval, vendoridval, inid).ToArray();

                //        }
                //        else
                //        {
                //            message = "";

                //        }
                //    }
                //    else
                //    {

                //        message = "main";
                //        string idval = exists.ToString();
                //        data.idval = idval;

                //    }
                //}
                data.Message = message;

            }
            catch (Exception ex)
            {
                logger.Error("InvoiceRepository - CheckExistence - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return data;
        }

        public InvoiceProduct UpdateProductprice(InvoiceProduct invoiceProduct)
        {
            try
            {
                db.Entry(invoiceProduct).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Error("InvoiceRepository - UpdateProductprice - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return invoiceProduct;
        }

        public void InsertProductprice(InvoiceProduct invoiceProduct)
        {
            try
            {
                db.InvoiceProducts.Add(invoiceProduct);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Error("InvoiceRepository - InsertProductprice - " + DateTime.Now + " - " + ex.Message.ToString());
            }
        }

        public void DeleteProductprice(string InvoiceProductId)
        {
            try
            {
                InvoiceProduct invoiceProduct = db.InvoiceProducts.Find(Convert.ToInt32(InvoiceProductId));
                db.InvoiceProducts.Remove(invoiceProduct);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Error("InvoiceRepository - DeleteProductprice - " + DateTime.Now + " - " + ex.Message.ToString());
            }
        }

        public IEnumerable GetDatapagescroll(int PageSize, int currentPageIndex, string rdcash, string rdcheck, string datval, string datendval, int deptnm, string searchdashbord, string AmtMaximum, string AmtMinimum, string orderby, int IsAsc, string SearchFlg, string chk, string strStore, string ASC)
        {
            IEnumerable RtnData = null;
            try
            {
                var UserName = System.Web.HttpContext.Current.User.Identity.Name;
                var UserTypeId = commonRepository.getUserTypeId(UserName);
                if (db.UserTypeMasters.Any(s => s.UserTypeId == UserTypeId && s.UserType == "View Invoice Only"))
                {
                    RtnData = db.Database.SqlQuery<InvoiceSelect>("SP_InvoiceData_Dashboard_DepartmentWise @startdate={0},@enddate={1},@Payment_type={2},@Dept_id={3},@Store_id={4},@IsStatus_id={5},@AmtMaximum={6},@AmtMinimum={7},@startRowIndex={8},@pageSize={9},@OrderBy={10},@AscDsc={11},@searchbox={12},@UserTypeID={13},@SearchType={14}", (datval.ToString() == "" ? null : datval.ToString()), (datendval.ToString() == "" ? null : datendval.ToString()), (chk == "" ? null : chk), deptnm, (strStore == "0" ? null : strStore), 0, (AmtMaximum == "" ? null : AmtMaximum), (AmtMinimum == "" ? null : AmtMinimum), (currentPageIndex * PageSize), PageSize, orderby, ASC, searchdashbord.Replace("&amp;", "&").Replace("&apos;", "'"), UserTypeId, SearchFlg).ToList();
                }
                else
                {
                    RtnData = db.Database.SqlQuery<InvoiceSelect>("SP_InvoiceData_Dashboard @startdate={0},@enddate={1},@Payment_type={2},@Dept_id={3},@Store_id={4},@IsStatus_id={5},@AmtMaximum={6},@AmtMinimum={7},@startRowIndex={8},@pageSize={9},@OrderBy={10},@AscDsc={11},@searchbox={12},@UserTypeID={13},@SearchType={14}", (datval.ToString() == "" ? null : datval.ToString()), (datendval.ToString() == "" ? null : datendval.ToString()), (chk == "" ? null : chk), deptnm, (strStore == "0" ? null : strStore), 0, (AmtMaximum == "" ? null : AmtMaximum), (AmtMinimum == "" ? null : AmtMinimum), (currentPageIndex * PageSize), PageSize, orderby, ASC, searchdashbord.Replace("&amp;", "&").Replace("&apos;", "'"), UserTypeId, SearchFlg).ToList();
                }
            }
            catch (Exception ex)
            {
                logger.Error("InvoiceRepository - GetDatapagescroll - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return RtnData;
        }

        public string GetroleForApproval(string Role, int? StoreId, int ModuleID)
        {
            string Result = "";
            try
            {
                var UserName = System.Web.HttpContext.Current.User.Identity.Name;
                int UserTypeID = commonRepository.getUserTypeId(UserName);
                Result = db.userRoles.Where(s => s.UserTypeId == UserTypeID && s.Role == Role && s.StoreId == StoreId && s.ModuleId == ModuleID).Select(s => s.Role).FirstOrDefault();
            }
            catch (Exception ex)
            {
                logger.Error("InvoiceRepository - GetroleForApproval - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return Result;
        }

        public EntityModels.Models.CashPaidoutInvoice CashPaidoutInvoices(int InvoiceID)
        {
            EntityModels.Models.CashPaidoutInvoice data = new EntityModels.Models.CashPaidoutInvoice();
            try
            {
                data = db.CashPaidoutInvoices.Where(a => a.InvoiceId == InvoiceID).FirstOrDefault();
            }
            catch (Exception ex)
            {
                logger.Error("InvoiceRepository - CashPaidoutInvoices - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return data;
        }

        public SalesActivitySummary SalesActivitySummaries(EntityModels.Models.CashPaidoutInvoice CPInvoice)
        {
            SalesActivitySummary data = new SalesActivitySummary();
            try
            {
                data = db.SalesActivitySummaries.Where(a => a.SalesActivitySummaryId == CPInvoice.SalesActivitySummaryId).FirstOrDefault();
            }
            catch (Exception ex)
            {
                logger.Error("InvoiceRepository - SalesActivitySummaries - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return data;
        }

        public void UpdateIsSyncStatus_reset(int InvoiceId)
        {
            try
            {
                db.Database.ExecuteSqlCommand("SP_Invoice @Mode = {0},@InvoiceId = {1}", "UpdateIsSyncStatus_reset", InvoiceId);
            }
            catch (Exception ex)
            {
                logger.Error("InvoiceRepository - UpdateIsSyncStatus_reset - " + DateTime.Now + " - " + ex.Message.ToString());
            }
        }

        public Invoice Invoices(int InvoiceID)
        {
            Invoice data = new Invoice();
            try
            {
                data = db.Invoices.Where(a => a.InvoiceId == InvoiceID).FirstOrDefault();
            }
            catch (Exception ex)
            {
                logger.Error("InvoiceRepository - Invoices - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return data;
        }

        public List<int> getVendorDepartmentList(int VendorId)
        {
            List<int> lst = new List<int>();
            try
            {
                if (VendorId != 0)
                {
                    lst = db.VendorDepartmentRelationMasters.Where(s => s.VendorId == VendorId).Select(s => s.DepartmentId).ToList();
                }
            }
            catch (Exception ex)
            {
                logger.Error("InvoiceRepository - getVendorDepartmentList - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return lst;

        }

        public int GetModuleMastersId()
        {
            int ID = 0;
            try
            {
                ID = db.ModuleMasters.Where(s => s.ModuleName == "Invoice").FirstOrDefault().ModuleId;
            }
            catch (Exception ex)
            {
                logger.Error("InvoiceRepository - GetModuleMastersId - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return ID;
        }

        public void InsertInvoices(Invoice Invoices)
        {
            try
            {
                db.Invoices.Add(Invoices);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Error("InvoiceRepository - InsertInvoices - " + DateTime.Now + " - " + ex.Message.ToString());
            }
        }

        public bool CheckUserTypeModuleApprovers(int UserTypeId, int ModuleId)
        {
            bool Ischeck = false;
            try
            {
                Ischeck = db.UserTypeModuleApprovers.Any(s => s.UserTypeId == UserTypeId && s.ModuleId == ModuleId);
            }
            catch (Exception ex)
            {
                logger.Error("InvoiceRepository - CheckUserTypeModuleApprovers - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return Ischeck;

        }

        public List<InvoiceProductSelect> GetItemList(int InvoiceID)
        {
            List<InvoiceProductSelect> lst = new List<InvoiceProductSelect>();
            try
            {
                lst = db.Database.SqlQuery<InvoiceProductSelect>("SP_Invoice @Mode = {0}, @InvoiceId = {1}", "ItemList", InvoiceID).ToList();
            }
            catch (Exception ex)
            {
                logger.Error("InvoiceRepository - GetItemList - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return lst;

        }

        public List<DepartmentMaster> GetDepartmentMastersList(int Storeidval, int iID)
        {
            List<DepartmentMaster> lst = new List<DepartmentMaster>();
            try
            {
                lst = db.DepartmentMasters.Where(a => a.StoreId == Storeidval && a.DepartmentId == iID).ToList();
            }
            catch (Exception ex)
            {
                logger.Error("InvoiceRepository - GetDepartmentMastersList - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return lst;

        }

        public List<DepartmentMaster> GetDepartmentMastersListbyDepartmentId(List<InvoiceDepartmentDetail> Departments)
        {
            List<DepartmentMaster> lst = new List<DepartmentMaster>();
            try
            {
                lst = db.DepartmentMasters.ToList().Where(a => Departments.Any(b => b.DepartmentId == a.DepartmentId)).ToList();
            }
            catch (Exception ex)
            {
                logger.Error("InvoiceRepository - GetDepartmentMastersListbyDepartmentId - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return lst;

        }

        public void UpdateStatus(string id, string value)
        {
            try
            {
                db.Database.ExecuteSqlCommand("SP_LineItemDetail @Mode = {0},@InvoiceProductId = {1},@Approved={2}", "UpdateInvoiceProductStatus", id, value);
            }
            catch (Exception ex)
            {
                logger.Error("InvoiceRepository - UpdateStatus - " + DateTime.Now + " - " + ex.Message.ToString());
            }
        }

        public void InsertInvoiceDepartmentDetails(InvoiceDepartmentDetail deptDetail)
        {
            try
            {
                db.InvoiceDepartmentDetails.Add(deptDetail);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Error("InvoiceRepository - InsertInvoiceDepartmentDetails - " + DateTime.Now + " - " + ex.Message.ToString());
            }
        }

        public List<InvoiceDepartmentDetail> GetInvoiceDepartmentDetailsbyInvoiceID(int InvoiceId)
        {
            List<InvoiceDepartmentDetail> InvoiceDepartmentDetails = new List<InvoiceDepartmentDetail>();
            try
            {
                InvoiceDepartmentDetails = db.InvoiceDepartmentDetails.Where(s => s.InvoiceId == InvoiceId).ToList();
            }
            catch (Exception ex)
            {
                logger.Error("InvoiceRepository - GetInvoiceDepartmentDetailsbyInvoiceID - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return InvoiceDepartmentDetails;
        }

        public Invoice GetInvoiceDetail(int id)
        {
            Invoice invoice = new Invoice();
            try
            {
                invoice = db.Invoices.AsNoTracking().FirstOrDefault(a => a.InvoiceId == id);
            }
            catch (Exception ex)
            {
                logger.Error("InvoiceRepository - GetInvoiceDetail - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return invoice;
        }

        public void ApproveAllStatus(string id)
        {
            try
            {
                db.Database.ExecuteSqlCommand("SP_LineItemDetail @Mode = {0},@InvoiceId = {1}", "ApproveAllInvoiceProduct", id);
            }
            catch (Exception ex)
            {
                logger.Error("InvoiceRepository - ApproveAllStatus - " + DateTime.Now + " - " + ex.Message.ToString());
            }
        }

        public void ResetStatus(string id)
        {
            try
            {

                db.Database.ExecuteSqlCommand("SP_UploadPDF @Spara = {0},@Id = {1}", "Reset_PDFRead", id);
            }
            catch (Exception ex)
            {
                logger.Error("InvoiceRepository - ResetStatus - " + DateTime.Now + " - " + ex.Message.ToString());
            }
        }

        public List<VendorMaster> ViewInvoice()
        {
            List<VendorMaster> lst = new List<VendorMaster>();
            try
            {
                lst = db.VendorMasters.ToList();
            }
            catch (Exception ex)
            {
                logger.Error("InvoiceRepository - ViewInvoice - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return lst;
        }

        public void ReadFiles(string vendor, string startdate, string enddate)
        {
            try
            {
                db.Database.ExecuteSqlCommand("SP_UploadPDFSpecial @Spara={0},@VendorName={1},@StartDate={2},@EndDate={3}", "Special_Add", vendor, startdate, enddate);
            }
            catch (Exception ex)
            {
                logger.Error("InvoiceRepository - ReadFiles - " + DateTime.Now + " - " + ex.Message.ToString());
            }
        }

        public UploadPdf CreateSplitInvoice(int val)
        {
            UploadPdf UploadFile = new UploadPdf();
            try
            {
                UploadFile = db.UploadPdfs.Find(val);
            }
            catch (Exception ex)
            {
                logger.Error("InvoiceRepository - CreateSplitInvoice - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return UploadFile;
        }

        public void UploadPdfId(int? uploadpdfId)
        {
            try
            {
                UploadPdf upload = db.UploadPdfs.Find(uploadpdfId);
                db.UploadPdfs.Remove(upload);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Error("InvoiceRepository - UploadPdfId - " + DateTime.Now + " - " + ex.Message.ToString());
            }
        }

        public List<InvoiceSelect> GetExpenseCheckSelect(InvoiceReportViewModal invoiceReportViewModal)
        {
            List<InvoiceSelect> obj = new List<InvoiceSelect>();
            try
            {
                obj = db.Database.SqlQuery<InvoiceSelect>("SP_InvoiceData @startdate={0},@enddate={1},@Payment_type={2},@Dept_id={3},@Store_id={4},@IsStatus_id={5},@VendorName={6},@AmtMaximum={7},@AmtMinimum={8}", invoiceReportViewModal.start_date, invoiceReportViewModal.end_date, invoiceReportViewModal.payment, invoiceReportViewModal.deptname, invoiceReportViewModal.istoreID, invoiceReportViewModal.Status, invoiceReportViewModal.VendorName, invoiceReportViewModal.AmtMaximum, invoiceReportViewModal.AmtMinimum).ToList();
            }
            catch (Exception ex)
            {
                logger.Error("InvoiceRepository - GetExpenseCheckSelect - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return obj;
        }

        public List<InvoiceSelect> GetInvoiceSelect(InvoiceReportViewModal invoiceReportViewModal)
        {
            List<InvoiceSelect> obj = new List<InvoiceSelect>();
            try
            {
                obj = db.Database.SqlQuery<InvoiceSelect>("SP_InvoiceData @startdate={0},@enddate={1},@Payment_type={2},@Dept_id={3},@Store_id={4},@IsStatus_id={5},@VendorName={6},@AmtMaximum={7},@AmtMinimum={8}", invoiceReportViewModal.start_date, invoiceReportViewModal.end_date, invoiceReportViewModal.payment, invoiceReportViewModal.deptname, Convert.ToInt32(invoiceReportViewModal.storeid), invoiceReportViewModal.Status, invoiceReportViewModal.VendorName, invoiceReportViewModal.AmtMaximum, invoiceReportViewModal.AmtMinimum).ToList();
            }
            catch (Exception ex)
            {
                logger.Error("InvoiceRepository - GetInvoiceSelect - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return obj;
        }

        public InvoiceFlgs CreateInvoicePost(Invoice invoice, bool RoleAdministrator, bool nnfApprovalInvoice, bool FLG, bool roleFlg, string Store, int StoreFlag, string[] ChildDepartmentId, string[] ChildAmount, int Storeidval, string UserName)
        {
            InvoiceFlgs invoiceFlgs = new InvoiceFlgs();
            try
            {
                if ((invoice.QuickInvoice == "1" && (RoleAdministrator == true || nnfApprovalInvoice == true)) || (FLG == true && roleFlg == false))
                {
                    if (Store != "")
                    {
                        if (Store == "Online" && StoreFlag == 1 && invoice.PaymentTypeId == 2) //Check/ACH
                        {
                            if (invoice.QBTransfer == false)
                            {
                                try
                                {
                                    if (invoice.strInvoiceDate != null)
                                    {
                                        invoice.InvoiceDate = Convert.ToDateTime(invoice.strInvoiceDate);
                                    }
                                    invoice.StatusValue = InvoiceStatusEnm.Approved;
                                    invoice.CreatedBy = UserModule.getUserId();
                                    invoice.CreatedOn = DateTime.Now;
                                    invoice.ApproveRejectBy = UserModule.getUserId();
                                    invoice.ApproveRejectDate = DateTime.Now;
                                    invoice.LastModifiedOn = DateTime.Now;
                                    invoice.IsSync = -1;
                                    invoice.IsActive = true;
                                    db.Invoices.Add(invoice);
                                    db.SaveChanges();

                                    invoiceFlgs.iInvoiceId = invoice.InvoiceId;
                                    invoiceFlgs.iInvoiceStatus = invoice.StatusValue.ToString();
                                    if (invoiceFlgs.iInvoiceId > 0)
                                    {
                                        int j = 0;
                                        if (invoice.InvoiceTypeId == 2) //Invoice = 1, CreditMemo =2
                                        {
                                            List<VendorCredirDetail> objList1 = new List<VendorCredirDetail>();
                                            if (ChildDepartmentId != null)
                                            {

                                                foreach (var val_id in ChildDepartmentId)
                                                {
                                                    if (Convert.ToDecimal(ChildAmount[j]) > 0) // Added to stop 0 Amount Invoice
                                                    {
                                                        int iID = Convert.ToInt32(val_id);
                                                        VendorCredirDetail objCredit = new VendorCredirDetail();
                                                        objCredit.DepartmentID = db.DepartmentMasters.Where(a => a.StoreId == Storeidval && a.DepartmentId == iID).FirstOrDefault().ListId;
                                                        objCredit.Amount = Convert.ToDecimal(ChildAmount[j]);
                                                        objList1.Add(objCredit);
                                                        objCredit = null;
                                                    }
                                                    j++;
                                                }
                                            }
                                            qBRepository.CreateVendorCredit(Convert.ToInt32(invoiceFlgs.iInvoiceId), objList1, 0);
                                            objList1 = null;
                                        }
                                        else
                                        {
                                            List<BillDetail> objList = new List<BillDetail>();
                                            if (ChildDepartmentId != null)
                                            {
                                                foreach (var val_id in ChildDepartmentId)
                                                {
                                                    if (Convert.ToDecimal(ChildAmount[j]) > 0) // Added to stop 0 Amount Invoice
                                                    {
                                                        int iID = Convert.ToInt32(val_id);
                                                        BillDetail objDetail = new BillDetail();
                                                        objDetail.Description = "";
                                                        objDetail.Amount = Convert.ToDecimal(ChildAmount[j]);
                                                        objDetail.DepartmentID = db.DepartmentMasters.Where(a => a.StoreId == Storeidval && a.DepartmentId == iID).Count() > 0 ? db.DepartmentMasters.Where(a => a.StoreId == Storeidval && a.DepartmentId == iID).FirstOrDefault().ListId : "";
                                                        objList.Add(objDetail);
                                                        objDetail = null;
                                                    }
                                                    j++;
                                                }
                                            }
                                            qBRepository.CreateBill(Convert.ToInt32(invoiceFlgs.iInvoiceId), objList);
                                            objList = null;
                                        }
                                    }
                                }
                                catch (Exception)
                                { }
                            }
                            else
                            {
                                invoice.CreatedBy = db.UserMasters.Where(s => s.UserName == UserName).FirstOrDefault().UserId;
                                invoice.CreatedOn = DateTime.Now;
                                invoice.LastModifiedOn = DateTime.Now;
                                invoice.Source = "WEB";
                                invoice.StatusValue = InvoiceStatusEnm.Approved;
                                invoice.ApproveRejectBy = db.UserMasters.Where(s => s.UserName == UserName).FirstOrDefault().UserId;
                                invoice.ApproveRejectDate = DateTime.Now;
                                invoice.IsSync = -1;
                                invoice.IsActive = true;
                                db.Invoices.Add(invoice);
                                db.SaveChanges();
                                invoiceFlgs.iInvoiceId = invoice.InvoiceId;
                                invoiceFlgs.iInvoiceStatus = invoice.StatusValue.ToString();
                            }
                        }
                        else
                        {
                            invoice.CreatedBy = db.UserMasters.Where(s => s.UserName == UserName).FirstOrDefault().UserId;
                            invoice.CreatedOn = DateTime.Now;
                            invoice.LastModifiedOn = DateTime.Now;
                            invoice.Source = "WEB";
                            invoice.StatusValue = InvoiceStatusEnm.Approved;
                            invoice.ApproveRejectBy = db.UserMasters.Where(s => s.UserName == UserName).FirstOrDefault().UserId;
                            invoice.ApproveRejectDate = DateTime.Now;
                            invoice.IsActive = true;
                            invoice.IsSync = -1;
                            db.Invoices.Add(invoice);
                            db.SaveChanges();
                            invoiceFlgs.iInvoiceId = invoice.InvoiceId;
                            invoiceFlgs.iInvoiceStatus = invoice.StatusValue.ToString();
                        }
                    }
                    else
                    {
                        invoice.CreatedBy = db.UserMasters.Where(s => s.UserName == UserName).FirstOrDefault().UserId;
                        invoice.CreatedOn = DateTime.Now;
                        invoice.LastModifiedOn = DateTime.Now;
                        invoice.Source = "WEB";
                        invoice.StatusValue = InvoiceStatusEnm.Approved;
                        invoice.ApproveRejectBy = db.UserMasters.Where(s => s.UserName == UserName).FirstOrDefault().UserId;
                        invoice.ApproveRejectDate = DateTime.Now;
                        invoice.IsActive = true;
                        db.Invoices.Add(invoice);
                        db.SaveChanges();
                        invoiceFlgs.iInvoiceId = invoice.InvoiceId;
                        invoiceFlgs.iInvoiceStatus = invoice.StatusValue.ToString();
                    }
                }
                else
                {
                    invoice.CreatedBy = db.UserMasters.Where(s => s.UserName == UserName).FirstOrDefault().UserId;
                    invoice.CreatedOn = DateTime.Now;
                    invoice.LastModifiedOn = DateTime.Now;
                    invoice.Source = "WEB";
                    invoice.StatusValue = InvoiceStatusEnm.Pending;
                    invoice.IsSync = -1;
                    invoice.IsActive = true;
                    db.Invoices.Add(invoice);
                    db.SaveChanges();
                    invoiceFlgs.iInvoiceId = invoice.InvoiceId;
                    invoiceFlgs.iInvoiceStatus = invoice.StatusValue.ToString();
                }
            }
            catch (Exception ex)
            {
                logger.Error("InvoiceRepository - CreateInvoicePost - " + DateTime.Now + " - " + ex.Message.ToString());
                logger.Error("InvoiceRepository - CreateInvoicePostInnerException - " + DateTime.Now + " - " + ex.InnerException.ToString());
                logger.Error("InvoiceRepository - CreateInvoicePostFullException - " + DateTime.Now + " - " + ex);
            }
            return invoiceFlgs;
        }

        public void SaveInvoiceDiscount(int iInvoiceId, int? DiscountTypeId, decimal? DiscountAmount, decimal? DiscountPercent)
        {
            try
            {
                db.Database.ExecuteSqlCommand("SP_InvoiceDiscount @Mode = {0},@InvoiceId = {1},@DiscountTypeId = {2},@DiscountAmount = {3},@DiscountPercent = {4}", "UpdateDiscountDetail", iInvoiceId, DiscountTypeId, DiscountAmount, DiscountPercent);
            }
            catch (Exception ex)
            {
                logger.Error("InvoiceRepository - SaveInvoiceDiscount - " + DateTime.Now + " - " + ex.Message.ToString());
            }
        }

        public int CreateInvoiceCreditmemoPost(Invoice invoice, bool ApproveInvoice, bool Administrator, string Store, int StoreFlag, string UserName, int iInvoiceId, string Credit_Invoiceno, string InvoiceNM, string iInvoiceStatus)
        {
            int iiInvoiceID = 0;
            try
            {
                if (invoice.QuickInvoice == "1" && (ApproveInvoice == true || Administrator == true)) // invoice.QuickCRInvoice == "1"
                {
                    if (Store != "")
                    {
                        if (Store == "Online" && StoreFlag == 1 && invoice.PaymentTypeId == 2)
                        {
                            try
                            {

                                Invoice InvObj = db.Invoices.AsNoTracking().FirstOrDefault(a => a.InvoiceId == iInvoiceId);
                                //InvObj = db.Invoices.Find(iInvoiceId);
                                InvObj.strInvoiceDate = InvObj.InvoiceDate.ToString("dd/MM/yyyy");
                                InvObj.CreatedBy = db.UserMasters.Where(s => s.UserName == UserName).FirstOrDefault().UserId;
                                InvObj.CreatedOn = DateTime.Now;
                                InvObj.LastModifiedOn = DateTime.Now;
                                InvObj.Source = "WEB";
                                InvObj.StatusValue = InvoiceStatusEnm.Approved;
                                InvObj.ApproveRejectBy = db.UserMasters.Where(s => s.UserName == UserName).FirstOrDefault().UserId;
                                InvObj.ApproveRejectDate = DateTime.Now;
                                InvObj.RefInvoiceId = iInvoiceId;
                                InvObj.InvoiceTypeId = 2; //CreditMemo
                                InvObj.PaymentTypeId = 2; //Check/ACH
                                InvObj.DiscountTypeId = 1;
                                InvObj.TotalAmount = InvObj.DiscountAmount != null ? invoice.DiscountAmount.Value : InvObj.TotalAmount;
                                InvObj.DiscountAmount = InvObj.DiscountAmount != null ? invoice.DiscountAmount.Value : 0;
                                InvObj.DiscountPercent = InvObj.DiscountPercent == null ? 0 : invoice.DiscountPercent;
                                InvObj.InvoiceNumber = Credit_Invoiceno;
                                invoice.IsActive = true;
                                InvObj.UploadInvoice = InvoiceNM;
                                db.Invoices.Add(InvObj);
                                db.SaveChanges();
                                iiInvoiceID = InvObj.InvoiceId;

                                if (Convert.ToInt32(iiInvoiceID) > 0)
                                {
                                    InvoiceDepartmentDetail deptDetail = new InvoiceDepartmentDetail();
                                    deptDetail.InvoiceId = iiInvoiceID;
                                    deptDetail.DepartmentId = Convert.ToInt32(invoice.Disc_Dept_id);
                                    deptDetail.Amount = Convert.ToDecimal(InvObj.DiscountAmount != null ? invoice.DiscountAmount.Value : InvObj.TotalAmount);
                                    db.InvoiceDepartmentDetails.Add(deptDetail);
                                    db.SaveChanges();

                                    if (invoice.QBTransfer == false)
                                    {
                                        int iID = Convert.ToInt32(invoice.Disc_Dept_id);
                                        List<VendorCredirDetail> objList = new List<VendorCredirDetail>();
                                        VendorCredirDetail objCredit = new VendorCredirDetail();
                                        objCredit.DepartmentID = db.DepartmentMasters.Where(a => a.StoreId == InvObj.StoreId && a.DepartmentId == iID).FirstOrDefault().ListId;
                                        objCredit.Amount = invoice.DiscountAmount;
                                        objList.Add(objCredit);

                                        qBRepository.CreateVendorCredit(Convert.ToInt32(iiInvoiceID), objList, 2);
                                    }
                                }
                                else
                                {
                                    Invoice iiInvoice = db.Invoices.Find(iiInvoiceID);
                                    iiInvoice.Source = "WEB";
                                    db.Entry(iiInvoice).State = EntityState.Modified;
                                    db.SaveChanges();
                                }
                            }
                            catch (Exception) { }
                        }
                        else
                        {
                            Invoice CreditMemo = new Invoice();
                            CreditMemo = db.Invoices.AsNoTracking().FirstOrDefault(a => a.InvoiceId == iInvoiceId);
                            CreditMemo.strInvoiceDate = CreditMemo.InvoiceDate.ToString("dd/MM/yyyy");
                            CreditMemo.InvoiceTypeId = 2; //CreditMemo
                                                          //CreditMemo.StatusValue = InvoiceStatusEnm.Pending;

                            if (iInvoiceStatus == "Approved")
                            {
                                CreditMemo.StatusValue = InvoiceStatusEnm.Approved;
                                CreditMemo.ApproveRejectBy = UserModule.getUserId();
                                CreditMemo.ApproveRejectDate = DateTime.Now;
                                CreditMemo.LastModifiedOn = DateTime.Now;
                                CreditMemo.IsSync = 1;
                            }
                            else
                            {
                                CreditMemo.StatusValue = InvoiceStatusEnm.Pending;
                                CreditMemo.IsSync = 0;
                            }

                            CreditMemo.PaymentTypeId = 2; //Check/ACH
                            CreditMemo.DiscountTypeId = 1; //N/A
                            CreditMemo.TotalAmount = invoice.DiscountAmount != null ? invoice.DiscountAmount.Value : invoice.TotalAmount;
                            CreditMemo.DiscountAmount = invoice.DiscountAmount != null ? invoice.DiscountAmount.Value : 0;
                            CreditMemo.DiscountPercent = invoice.DiscountPercent != null ? invoice.DiscountPercent.Value : 0;
                            CreditMemo.RefInvoiceId = iInvoiceId;
                            CreditMemo.UploadInvoice = InvoiceNM;
                            CreditMemo.InvoiceNumber = Credit_Invoiceno;
                            CreditMemo.CreatedBy = db.UserMasters.Where(s => s.UserName == UserName).FirstOrDefault().UserId;
                            CreditMemo.CreatedOn = DateTime.Now;
                            CreditMemo.LastModifiedOn = DateTime.Now;
                            CreditMemo.Source = "WEB";
                            CreditMemo.IsActive = true;
                            db.Invoices.Add(CreditMemo);
                            db.SaveChanges();

                            if (CreditMemo.InvoiceId > 0)
                            {
                                InvoiceDepartmentDetail deptDetail1 = new InvoiceDepartmentDetail();
                                deptDetail1.InvoiceId = CreditMemo.InvoiceId;
                                deptDetail1.DepartmentId = Convert.ToInt32(invoice.Disc_Dept_id);
                                deptDetail1.Amount = Convert.ToDecimal(invoice.DiscountAmount != null ? invoice.DiscountAmount.Value : invoice.TotalAmount);
                                db.InvoiceDepartmentDetails.Add(deptDetail1);
                                db.SaveChanges();


                                //New implement
                                if (invoice.QBTransfer == false && iInvoiceStatus == "Approved")
                                {
                                    int iID = Convert.ToInt32(invoice.Disc_Dept_id);
                                    List<VendorCredirDetail> objList = new List<VendorCredirDetail>();
                                    VendorCredirDetail objCredit = new VendorCredirDetail();
                                    objCredit.DepartmentID = db.DepartmentMasters.Where(a => a.StoreId == CreditMemo.StoreId && a.DepartmentId == iID).FirstOrDefault().ListId;
                                    objCredit.Amount = invoice.DiscountAmount;
                                    objList.Add(objCredit);

                                    qBRepository.CreateVendorCredit(Convert.ToInt32(CreditMemo.InvoiceId), objList, 2);
                                }

                            }
                        }
                    }
                    else
                    {
                        Invoice CreditMemo = new Invoice();
                        CreditMemo = db.Invoices.AsNoTracking().FirstOrDefault(a => a.InvoiceId == iInvoiceId);
                        CreditMemo.strInvoiceDate = CreditMemo.InvoiceDate.ToString("dd/MM/yyyy");
                        CreditMemo.InvoiceTypeId = 2;
                        CreditMemo.StatusValue = InvoiceStatusEnm.Approved;
                        CreditMemo.PaymentTypeId = 2; //Check/ACH
                        CreditMemo.DiscountTypeId = 1; //N/A
                        CreditMemo.InvoiceNumber = Credit_Invoiceno;
                        CreditMemo.DiscountAmount = invoice.DiscountAmount != null ? invoice.DiscountAmount.Value : 0;
                        CreditMemo.DiscountPercent = invoice.DiscountPercent != null ? invoice.DiscountPercent.Value : 0;
                        CreditMemo.TotalAmount = invoice.DiscountAmount != null ? invoice.DiscountAmount.Value : invoice.TotalAmount;
                        CreditMemo.CreatedBy = db.UserMasters.Where(s => s.UserName == UserName).FirstOrDefault().UserId;
                        CreditMemo.CreatedOn = DateTime.Now;
                        CreditMemo.ApproveRejectBy = db.UserMasters.Where(s => s.UserName == UserName).FirstOrDefault().UserId;
                        CreditMemo.ApproveRejectDate = DateTime.Now;
                        CreditMemo.LastModifiedOn = DateTime.Now;
                        CreditMemo.RefInvoiceId = iInvoiceId;
                        CreditMemo.UploadInvoice = InvoiceNM;
                        CreditMemo.Source = "WEB";
                        CreditMemo.IsActive = true;
                        db.Invoices.Add(CreditMemo);
                        db.SaveChanges();
                        iiInvoiceID = CreditMemo.InvoiceId;

                        if (iiInvoiceID > 0)
                        {
                            InvoiceDepartmentDetail deptDetail1 = new InvoiceDepartmentDetail();
                            deptDetail1.InvoiceId = iiInvoiceID;
                            deptDetail1.DepartmentId = Convert.ToInt32(invoice.Disc_Dept_id);
                            deptDetail1.Amount = Convert.ToDecimal(invoice.DiscountAmount != null ? invoice.DiscountAmount.Value : invoice.TotalAmount);
                            db.InvoiceDepartmentDetails.Add(deptDetail1);
                            db.SaveChanges();
                        }
                    }
                }
                else
                {
                    Invoice InvObj = db.Invoices.AsNoTracking().FirstOrDefault(a => a.InvoiceId == iInvoiceId);
                    InvObj.strInvoiceDate = invoice.InvoiceDate.ToString("dd/MM/yyyy");
                    InvObj.InvoiceTypeId = 2;
                    InvObj.PaymentTypeId = 2; //Check/ACH
                    InvObj.InvoiceNumber = Credit_Invoiceno;
                    InvObj.IsSync = -1;

                    if (iInvoiceStatus == "Approved")
                    {
                        InvObj.StatusValue = InvoiceStatusEnm.Approved;
                        InvObj.ApproveRejectBy = UserModule.getUserId();
                        InvObj.ApproveRejectDate = DateTime.Now;
                        InvObj.LastModifiedOn = DateTime.Now;
                        InvObj.IsSync = 1;
                    }
                    else
                    {
                        InvObj.StatusValue = InvoiceStatusEnm.Pending;
                        InvObj.IsSync = -1;
                    }
                    InvObj.DiscountTypeId = 1; //N/A
                    InvObj.CreatedBy = db.UserMasters.Where(s => s.UserName == UserName).FirstOrDefault().UserId;
                    InvObj.CreatedOn = DateTime.Now;
                    InvObj.LastModifiedOn = DateTime.Now;
                    InvObj.RefInvoiceId = iInvoiceId;
                    InvObj.Source = "WEB";
                    InvObj.IsActive = true;
                    InvObj.TotalAmount = Convert.ToDecimal(invoice.DiscountAmount != null ? invoice.DiscountAmount.Value : invoice.TotalAmount);
                    InvObj.UploadInvoice = InvoiceNM;
                    db.Invoices.Add(InvObj);
                    db.SaveChanges();
                    iiInvoiceID = InvObj.InvoiceId;

                    InvoiceDepartmentDetail deptDetail = new InvoiceDepartmentDetail();
                    deptDetail.InvoiceId = iiInvoiceID;
                    deptDetail.DepartmentId = Convert.ToInt32(invoice.Disc_Dept_id);
                    deptDetail.Amount = Convert.ToDecimal(invoice.DiscountAmount != null ? invoice.DiscountAmount.Value : InvObj.TotalAmount);
                    db.InvoiceDepartmentDetails.Add(deptDetail);
                    db.SaveChanges();


                    //New implement
                    if (invoice.QBTransfer == false && iInvoiceStatus == "Approved")
                    {
                        int iID = Convert.ToInt32(invoice.Disc_Dept_id);
                        List<VendorCredirDetail> objList = new List<VendorCredirDetail>();
                        VendorCredirDetail objCredit = new VendorCredirDetail();
                        objCredit.DepartmentID = db.DepartmentMasters.Where(a => a.StoreId == InvObj.StoreId && a.DepartmentId == iID).FirstOrDefault().ListId;
                        objCredit.Amount = invoice.DiscountAmount;
                        objList.Add(objCredit);

                        qBRepository.CreateVendorCredit(Convert.ToInt32(iiInvoiceID), objList, 2);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("InvoiceRepository - CreateInvoiceCreditmemoPost - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return iiInvoiceID;
        }

        public int CheckInvoiceExist(Invoice invoice)
        {
            int Flg = 0;
            try
            {
                db.Database.SqlQuery<int>("SP_CheckInvoice_Exist @InvoiceNumber = {0},@InvoiceDate = {1},@StoreId = {2},@InvoiceTypeId = {3},@VendorId = {4},@InvoiceId = {5}", invoice.InvoiceNumber, invoice.InvoiceDate, invoice.StoreId, invoice.InvoiceTypeId, invoice.VendorId, invoice.InvoiceId).FirstOrDefault();
            }
            catch (Exception ex)
            {
                logger.Error("InvoiceRepository - CheckInvoiceExist - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return Flg;
        }

        public CheckExistInvoice CheckInvoiceExistAmountDetails(Invoice invoice)
        {
            CheckExistInvoice checkExistInvoice = new CheckExistInvoice();
            try
            {
                db.Database.SqlQuery<CheckExistInvoice>("SP_CheckInvoice_Exist_Amount_Detail @InvoiceNumber = {0},@StoreId = {1},@InvoiceTypeId = {2},@VendorId = {3},@InvoiceId = {4},@Amount = {5},@InvoiceDate={6}", invoice.InvoiceNumber, invoice.StoreId, invoice.InvoiceTypeId, invoice.VendorId, invoice.InvoiceId, invoice.TotalAmount, invoice.InvoiceDate).FirstOrDefault();
            }
            catch (Exception ex)
            {
                logger.Error("InvoiceRepository - CheckInvoiceExistAmountDetails - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return checkExistInvoice;
        }

        public Invoice ChildDepartmentSave(Invoice invoice, string[] ChildDepartmentId, string[] ChildAmount, string UserName)
        {
            try
            {
                if (ChildDepartmentId != null)
                {
                    int j = 0;
                    List<InvoiceDepartmentDetail> InvoiceDepartmentDetailList = new List<InvoiceDepartmentDetail>();
                    foreach (var val_id in ChildDepartmentId)
                    {
                        if (!String.IsNullOrEmpty(ChildAmount[j].ToString()))
                        {
                            if (ChildAmount[j].ToString() != "")
                            {
                                InvoiceDepartmentDetailList.Add(new InvoiceDepartmentDetail { DepartmentId = Convert.ToInt32(val_id), Amount = Convert.ToDecimal(ChildAmount[j] == "" ? "0" : ChildAmount[j]), InvoiceId = invoice.InvoiceId });
                            }
                        }
                        j++;
                    }
                    invoice.InvoiceDepartmentDetails = InvoiceDepartmentDetailList;

                    db.InvoiceDepartmentDetails.RemoveRange(db.InvoiceDepartmentDetails.Where(s => s.InvoiceId == invoice.InvoiceId));
                    db.InvoiceDepartmentDetails.AddRange(invoice.InvoiceDepartmentDetails);
                }
                invoice.ModifiedBy = db.UserMasters.Where(s => s.UserName == UserName).FirstOrDefault().UserId;
                invoice.ModifiedOn = DateTime.Now;
                invoice.LastModifiedOn = DateTime.Now;
            }
            catch (Exception ex)
            {
                logger.Error("InvoiceRepository - ChildDepartmentSave - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return invoice;
        }

        public void SaveInvoiceDiscountEdit(Invoice invoice)
        {
            try
            {
                decimal? DiscountPercent = 0;
                decimal? DiscountAmount = 0;
                int? RefInvoiceId = 0;
                var iinv = db.Invoices.AsNoTracking().Where(a => a.InvoiceId == invoice.InvoiceId).FirstOrDefault();
                if (iinv != null)
                {
                    DiscountPercent = iinv.DiscountPercent;
                    DiscountAmount = iinv.DiscountAmount;
                    RefInvoiceId = iinv.RefInvoiceId;
                }

                db.Entry(invoice).State = EntityState.Modified;
                db.SaveChanges();
                db.Database.ExecuteSqlCommand("SP_InvoiceDiscount @Mode = {0},@InvoiceId = {1},@DiscountTypeId = {2},@DiscountAmount = {3}, @DiscountPercent = {4}, @RefInvoiceId = {5}", "UpdateDiscountDetail", invoice.InvoiceId, invoice.DiscountTypeId, (DiscountAmount == null ? 0 : DiscountAmount), (DiscountPercent == null ? 0 : DiscountPercent), RefInvoiceId);
                //for remove twise log data from userlog table
                db.Database.ExecuteSqlCommand("SP_UserLogUpdate @Mode = {0}", "RemoveTwiseLogEntry");
            }
            catch (Exception ex)
            {
                logger.Error("InvoiceRepository - SaveInvoiceDiscountEdit - " + DateTime.Now + " - " + ex.Message.ToString());
            }
        }

        public string InvoiceQBSync(Invoice invoice, string[] ChildDepartmentId, string[] ChildAmount, string Store, int Flag)
        {
            string success = "";
            try
            {
                int j = 0;
                QBResponse objResponse = new QBResponse();
                if (!invoice.InvoiceNumber.ToString().Contains("_cr"))
                {
                    if (Store == "Online" && Flag == 1 && invoice.PaymentTypeId == 2) //Check/Ach
                    {
                        if (invoice.TXNId != null)
                        {
                            if (invoice.InvoiceTypeId == 2) //CreditMemo
                            {
                                if (invoice.QBTransfer == false)
                                {
                                    List<VendorCredirDetail> objListVDetail = new List<VendorCredirDetail>();
                                    int j1 = 0;
                                    if (ChildDepartmentId != null && invoice.TXNId != null)
                                    {
                                        foreach (var val_id in ChildDepartmentId)
                                        {
                                            VendorCredirDetail objCredit = new VendorCredirDetail();
                                            int iID = Convert.ToInt32(val_id);
                                            objCredit.Description = "";
                                            objCredit.Amount = Convert.ToDecimal(ChildAmount[j1]);
                                            objCredit.DepartmentID = db.DepartmentMasters.Where(a => a.StoreId == invoice.StoreId && a.DepartmentId == iID).FirstOrDefault().ListId;
                                            objListVDetail.Add(objCredit);
                                            objCredit = null;
                                            j1++;
                                        }
                                    }
                                    if (qBRepository.QBEditVendorCreditData(invoice.InvoiceId, invoice.TXNId, invoice.InvoiceNumber, Convert.ToDateTime(invoice.InvoiceDate), invoice.Note, Convert.ToInt32(invoice.StoreId), invoice.VendorId, objListVDetail, ref objResponse) == true)
                                    {
                                        success = "Valid";
                                    }
                                }
                            }
                            else
                            {
                                if (invoice.QBTransfer == false)
                                {
                                    List<BillDetail> objList = new List<BillDetail>();
                                    if (ChildDepartmentId != null && invoice.TXNId != null)
                                    {
                                        foreach (var val_id in ChildDepartmentId)
                                        {
                                            int iID = Convert.ToInt32(val_id);
                                            BillDetail objDetail = new BillDetail();
                                            objDetail.Description = "";
                                            objDetail.Amount = Convert.ToDecimal(ChildAmount[j]);
                                            objDetail.DepartmentID = db.DepartmentMasters.Where(a => a.StoreId == invoice.StoreId && a.DepartmentId == iID).FirstOrDefault().ListId;
                                            objList.Add(objDetail);
                                            objDetail = null;
                                            j++;
                                        }
                                    }
                                    if (qBRepository.QBEditBillData(invoice.InvoiceId, invoice.TXNId, invoice.InvoiceNumber, Convert.ToDateTime(invoice.InvoiceDate), invoice.Note, Convert.ToInt32(invoice.StoreId), invoice.VendorId, objList, ref objResponse) == true)
                                    {
                                        success = "Valid";
                                    }
                                }
                            }

                            if (success == "Valid")
                            {
                                db.Database.ExecuteSqlCommand("SP_Invoice @Mode = {0},@InvoiceId = {1}", "UpdateEditIsSyncStatus", invoice.InvoiceId);
                            }
                            else
                            {
                                db.Database.ExecuteSqlCommand("SP_Invoice @Mode = {0},@InvoiceId = {1}", "UpdateIsSyncStatus_reset", invoice.InvoiceId);
                            }
                        }
                    }
                    else
                    {
                        db.Database.ExecuteSqlCommand("SP_Invoice @Mode = {0},@InvoiceId = {1}", "UpdateIsSyncStatus_reset", invoice.InvoiceId);
                    }
                }
                else
                {
                    if (invoice.QBTransfer == false)
                    {
                        if (Store == "Online" && Flag == 1 && invoice.PaymentTypeId == 2) //Check/ACH
                        {
                            if (invoice.TXNId != null)
                            {
                                List<VendorCredirDetail> objListVDetail = new List<VendorCredirDetail>();
                                int j1 = 0;
                                if (ChildDepartmentId != null && invoice.TXNId != null)
                                {
                                    foreach (var val_id in ChildDepartmentId)
                                    {
                                        int iID = Convert.ToInt32(val_id);
                                        VendorCredirDetail objCredit = new VendorCredirDetail();
                                        objCredit.Description = "";
                                        objCredit.Amount = Convert.ToDecimal(ChildAmount[j1]);
                                        objCredit.DepartmentID = db.DepartmentMasters.Where(a => a.StoreId == invoice.StoreId && a.DepartmentId == iID).FirstOrDefault().ListId;
                                        objListVDetail.Add(objCredit);
                                        objCredit = null;
                                        j1++;
                                    }
                                }

                                if (qBRepository.QBEditVendorCreditData(invoice.InvoiceId, invoice.TXNId, invoice.InvoiceNumber, Convert.ToDateTime(invoice.InvoiceDate), invoice.Note, Convert.ToInt32(invoice.StoreId), invoice.VendorId, objListVDetail, ref objResponse) == true)
                                {
                                    success = "Valid";
                                }
                                if (success == "Valid")
                                {
                                    //db.Database.ExecuteSqlCommand("UPDATE Invoice Set  IsSync = 1, SyncDate ='" + DateTime.Now + "' Where InvoiceId=" + invoice.InvoiceId);
                                    db.Database.ExecuteSqlCommand("SP_Invoice @Mode = {0},@InvoiceId = {1}", "UpdateEditIsSyncStatus", invoice.InvoiceId);
                                }
                                else
                                {
                                    //db.Database.ExecuteSqlCommand("UPDATE Invoice Set  IsSync = 0 Where InvoiceId=" + invoice.InvoiceId);
                                    db.Database.ExecuteSqlCommand("SP_Invoice @Mode = {0},@InvoiceId = {1}", "UpdateIsSyncStatus_reset", invoice.InvoiceId);
                                }
                            }
                        }
                        else
                        {
                            //db.Database.ExecuteSqlCommand("UPDATE Invoice Set  IsSync = 0 Where InvoiceId=" + invoice.InvoiceId);
                            db.Database.ExecuteSqlCommand("SP_Invoice @Mode = {0},@InvoiceId = {1}", "UpdateIsSyncStatus_reset", invoice.InvoiceId);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("InvoiceRepository - InvoiceQBSync - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return success;
        }

        public string InvoiceCreditmemoSaveEdit(Invoice invoice, bool QuickCRInvoice, bool Administrator, string UserName, string Credit_Invoiceno, string scandocument)
        {
            string success1 = "";
            int MemoInvoiceid = 0;
            try
            {
                if (invoice.QuickInvoice == "1" && (QuickCRInvoice == true || Administrator == true)) //invoice.QuickCRInvoice == "1" //|| Roles.IsUserInRole("ApproveInvoice") && roleFlg == false || Roles.IsUserInRole("nnfapprovalInvoice")
                {
                    Invoice InvObj = db.Invoices.AsNoTracking().FirstOrDefault(a => a.InvoiceId == invoice.InvoiceId);
                    db.Database.ExecuteSqlCommand("SP_InvoiceDiscount @Mode = {0},@InvoiceId = {1}", "UpdateInvoiceDiscountDetail", invoice.InvoiceId);
                    //InvObj = db.Invoices.Find(iInvoiceId);
                    InvObj.strInvoiceDate = InvObj.InvoiceDate.ToString("dd/MM/yyyy");
                    InvObj.CreatedBy = db.UserMasters.Where(s => s.UserName == UserName).FirstOrDefault().UserId;
                    InvObj.CreatedOn = DateTime.Now;
                    InvObj.LastModifiedOn = DateTime.Now;
                    InvObj.Source = "WEB";
                    InvObj.StatusValue = InvoiceStatusEnm.Approved;
                    InvObj.ApproveRejectBy = db.UserMasters.Where(s => s.UserName == UserName).FirstOrDefault().UserId;
                    InvObj.ApproveRejectDate = DateTime.Now;

                    InvObj.RefInvoiceId = invoice.InvoiceId;
                    InvObj.InvoiceTypeId = 2; //CreditMemo
                    InvObj.PaymentTypeId = 2; //Check/ACH
                    InvObj.DiscountTypeId = 1;

                    InvObj.TotalAmount = InvObj.DiscountAmount != null ? invoice.DiscountAmount.Value : InvObj.TotalAmount;
                    InvObj.DiscountAmount = invoice.DiscountAmount != null ? invoice.DiscountAmount.Value : 0;
                    InvObj.DiscountPercent = invoice.DiscountPercent != null ? invoice.DiscountPercent.Value : 0;
                    InvObj.InvoiceNumber = Credit_Invoiceno;
                    invoice.IsActive = true;
                    InvObj.UploadInvoice = scandocument;
                    InvObj.InvoiceReview = invoice.InvoiceReview;
                    db.Invoices.Add(InvObj);
                    db.SaveChanges();
                    MemoInvoiceid = InvObj.InvoiceId;
                    success1 = "Success";

                }
                else
                {
                    Invoice CreditMemo = new Invoice();
                    CreditMemo = db.Invoices.AsNoTracking().FirstOrDefault(a => a.InvoiceId == invoice.InvoiceId);
                    db.Database.ExecuteSqlCommand("SP_InvoiceDiscount @Mode = {0},@InvoiceId = {1}", "UpdateInvoiceDiscountDetail", CreditMemo.InvoiceId);
                    CreditMemo.strInvoiceDate = CreditMemo.InvoiceDate.ToString("dd/MM/yyyy");
                    CreditMemo.InvoiceTypeId = 2; //CreditMemo
                    CreditMemo.StatusValue = InvoiceStatusEnm.Pending;
                    CreditMemo.PaymentTypeId = 2; //Check/ACH
                    CreditMemo.DiscountTypeId = 1; //N/A
                    CreditMemo.TotalAmount = invoice.DiscountAmount != null ? invoice.DiscountAmount.Value : invoice.TotalAmount;
                    CreditMemo.DiscountAmount = invoice.DiscountAmount != null ? invoice.DiscountAmount.Value : 0;
                    CreditMemo.DiscountPercent = invoice.DiscountPercent != null ? invoice.DiscountPercent.Value : 0;
                    CreditMemo.RefInvoiceId = invoice.InvoiceId;
                    CreditMemo.UploadInvoice = scandocument;
                    CreditMemo.InvoiceNumber = Credit_Invoiceno;
                    CreditMemo.CreatedBy = db.UserMasters.Where(s => s.UserName == UserName).FirstOrDefault().UserId;
                    CreditMemo.CreatedOn = DateTime.Now;
                    CreditMemo.LastModifiedOn = DateTime.Now;
                    CreditMemo.Source = "WEB";
                    CreditMemo.IsActive = true;
                    CreditMemo.InvoiceReview = invoice.InvoiceReview;
                    db.Invoices.Add(CreditMemo);
                    db.SaveChanges();
                    MemoInvoiceid = CreditMemo.InvoiceId;
                    success1 = "Success";
                }
                int apprvstatus = 0;
                if (invoice.QuickInvoice == null)
                {
                    apprvstatus = 1;
                }
                else
                {
                    apprvstatus = 2;
                }
                var approvedby = db.UserMasters.Where(s => s.UserName == UserName).FirstOrDefault().UserId;
                if (invoice.QuickInvoice == "1" && (QuickCRInvoice == true || Administrator == true))
                {
                    db.Database.ExecuteSqlCommand("SP_InvoiceDiscount @Mode = {0},@InvoiceId = {1},@DiscountTypeId = {2},@DiscountAmount = {3}, @DiscountPercent = {4}, @StatusValue = {5}, @ApproveRejectBy = {6}, @ApproveRejectDate = {7}, @IsSync = {8}", "UpdateDiscountDetailForApproval", invoice.InvoiceId, invoice.DiscountTypeId, (invoice.DiscountAmount == null ? 0 : invoice.DiscountAmount), (invoice.DiscountPercent == null ? 0 : invoice.DiscountPercent), apprvstatus, approvedby, DateTime.Now, 0);
                }
                else
                {
                    db.Database.ExecuteSqlCommand("SP_InvoiceDiscount @Mode = {0},@InvoiceId = {1},@DiscountTypeId = {2},@DiscountAmount = {3}, @DiscountPercent = {4}, @StatusValue = {5}, @ApproveRejectBy = {6}, @ApproveRejectDate = {7}, @IsSync = {8}", "UpdateDiscountDetailForApproval", invoice.InvoiceId, invoice.DiscountTypeId, (invoice.DiscountAmount == null ? 0 : invoice.DiscountAmount), (invoice.DiscountPercent == null ? 0 : invoice.DiscountPercent), apprvstatus, null, null, invoice.IsSync);
                }

                if (!string.IsNullOrEmpty(success1))
                {
                    InvoiceDepartmentDetail deptDetail = new InvoiceDepartmentDetail();
                    deptDetail.InvoiceId = MemoInvoiceid;
                    deptDetail.DepartmentId = Convert.ToInt32(invoice.Disc_Dept_id);
                    deptDetail.Amount = Convert.ToDecimal(invoice.DiscountAmount != null ? invoice.DiscountAmount.Value : invoice.TotalAmount);
                    db.InvoiceDepartmentDetails.Add(deptDetail);
                    db.SaveChanges();

                    ActivityLogMessage = "Invoice Number " + "<a href='/Invoices/Details/" + MemoInvoiceid + "'>" + Credit_Invoiceno + "</a> Edited by " + UserModule.getUserFirstName() + " on " + AdminSiteConfiguration.GetDate(AdminSiteConfiguration.GetEasternTime(DateTime.Now).ToString());


                    ActivityLog ActLog2 = new ActivityLog();
                    ActLog2.Action = 1;
                    ActLog2.Comment = ActivityLogMessage;
                    activityLogRepository.ActivityLogInsert(ActLog2);

                    if (MemoInvoiceid > 0)
                    {
                        int IID1 = Convert.ToInt32(MemoInvoiceid);
                        List<VendorCredirDetail> objListVDetail = new List<VendorCredirDetail>();
                        var invDept = db.InvoiceDepartmentDetails.Where(s => s.InvoiceId == MemoInvoiceid).ToList();
                        if (invDept != null)
                        {

                            foreach (var item in invDept)
                            {
                                VendorCredirDetail objDetail = new VendorCredirDetail();
                                objDetail.Description = "";
                                objDetail.Amount = Convert.ToDecimal(item.Amount);
                                objDetail.DepartmentID = db.DepartmentMasters.Where(a => a.StoreId == item.Invoices.StoreId && a.DepartmentId == item.DepartmentId).FirstOrDefault().ListId; ;
                                objListVDetail.Add(objDetail);
                            }
                        }
                        if (apprvstatus == 2)
                        {
                            qBRepository.CreateVendorCredit(IID1, objListVDetail, 1);
                        }
                    }
                }
                //for remove twise log data from userlog table
                db.Database.ExecuteSqlCommand("SP_UserLogUpdate @Mode = {0}", "RemoveTwiseLogEntry");
            }
            catch (Exception ex)
            {
                logger.Error("InvoiceRepository - InvoiceCreditmemoSaveEdit - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return success1;
        }

        public List<InvoiceCount> InvoiceCounts(string Date, int InvoiceType)
        {
            List<InvoiceCount> invoiceCount = new List<InvoiceCount>();
            IEnumerable data = "";
            try
            {
                data = db.Database.SqlQuery<InvoiceCount>("SP_Invoice @Mode={0},@InvoiceTypeID={1},@InvoiceDate={2}", "InvoiceCounts", InvoiceType, Date).ToList();
                invoiceCount = data.OfType<InvoiceCount>().ToList();
            }
            catch (Exception ex)
            {
                logger.Error("InvoiceRepository - InvoiceCounts - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return invoiceCount;
        }

        public int SaveAutoMationFileChanges(UploadPDFAutomation uploadPDF)
        {
            int UploadPdfAutomationId = 0;
            try
            {
                db.UploadPDFAutomation.Add(uploadPDF);
                db.SaveChanges();
                UploadPdfAutomationId = uploadPDF.UploadPdfAutomationId;
            }
            catch (Exception ex)
            {
                logger.Error("InvoiceRepository - SaveAutoMationFileChanges - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return UploadPdfAutomationId;
        }

        public List<UploadPDFAutomationList> getuploadautomationlist(int store_idval, int UserId)
        {
            List<UploadPDFAutomationList> automationlist = new List<UploadPDFAutomationList>();
            IEnumerable data = "";
            try
            {
                data = db.Database.SqlQuery<UploadPDFAutomationList>("SP_InvoiceAutomation @Spara={0}, @StoreId={1}, @UserId={2}", "GetInvoiceAutomationList", store_idval, UserId).ToList();
                automationlist = data.OfType<UploadPDFAutomationList>().ToList();
                foreach (var item in automationlist)
                {
                    if (!string.IsNullOrEmpty(item.FileName))
                    {
                        item.FileName = System.IO.Path.GetFileName(item.FileName);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("InvoiceRepository - getuploadautomationlist - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return automationlist;
        }

        public UploadPDFAutomation GetUploadAutomation(int id)
        {
            UploadPDFAutomation UploadFile = new UploadPDFAutomation();
            try
            {
                UploadFile = db.UploadPDFAutomation.Find(id);
            }
            catch (Exception ex)
            {
                logger.Error("InvoiceRepository - GetUploadAutomation - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return UploadFile;
        }

        public void DeleteInvoiceAutomation(int id)
        {
            try
            {
                db.Database.ExecuteSqlCommand("SP_InvoiceAutomation @Spara = {0},@UploadPdfAutomationId = {1}", "DeleteInvoiceAutomation", id);
            }
            catch (Exception ex)
            {
                logger.Error("InvoiceRepository - DeleteInvoiceAutomation - " + DateTime.Now + " - " + ex.Message.ToString());
            }
        }

        public void UploadAutomationPdfId(int? UploadPdfAutomationId)
        {
            try
            {
                UploadPDFAutomation upload = db.UploadPDFAutomation.Find(UploadPdfAutomationId);
                db.UploadPDFAutomation.Remove(upload);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Error("InvoiceRepository - UploadAutomationPdfId - " + DateTime.Now + " - " + ex.Message.ToString());
            }
        }

        public InvoiceAutomation EditInvoiceAutomationObj(int id)
        {
            InvoiceAutomation invoice = new InvoiceAutomation();
            try
            {
                invoice = db.InvoiceAutomation.Find(id);
                if (invoice.VendorId != 0)
                {
                    invoice.Address = db.VendorMasters.Find(invoice.VendorId).Address;
                    invoice.Address2 = db.VendorMasters.Find(invoice.VendorId).Address2;
                    invoice.City = db.VendorMasters.Find(invoice.VendorId).City;
                    invoice.State = db.VendorMasters.Find(invoice.VendorId).State;
                    invoice.Country = db.VendorMasters.Find(invoice.VendorId).Country;
                    invoice.PostalCode = db.VendorMasters.Find(invoice.VendorId).PostalCode;
                    invoice.PhoneNumber = db.VendorMasters.Find(invoice.VendorId).PhoneNumber;

                }
                invoice.ChildAmount = invoice.TotalAmount;
                if (invoice.Address == "" || invoice.Address == null)
                {
                    invoice.Address = "Not Available";
                }
                else
                {
                    invoice.Address = (invoice.Address != null ? (invoice.Address != "" ? invoice.Address + "," : "") : "") + (invoice.Address2 != null ? (invoice.Address2 != "" ? invoice.Address2 + "," : "") : "") + (invoice.City != null ? (invoice.City != "" ? invoice.City + "," : "") : "") + (invoice.State != null ? (invoice.State != "" ? invoice.State + " " : "") : "") + (invoice.Country != null ? (invoice.Country != "" ? invoice.Country + " " : "") : "") + (invoice.PostalCode != null ? (invoice.PostalCode != "" ? invoice.PostalCode : "") : "");
                }
                if (invoice.PhoneNumber == "" || invoice.PhoneNumber == null)
                {
                    invoice.PhoneNumber = "Not Available";
                }
                invoice.strInvoiceDate = AdminSiteConfiguration.GetDateTime(invoice.InvoiceDate);
                invoice.StoreName = db.StoreMasters.Find(invoice.StoreId).NickName;
                if (invoice.CreatedBy != 0 && invoice.CreatedBy != null)
                {
                    invoice.CreatedByUserName = db.UserMasters.Find(invoice.CreatedBy) != null ? db.UserMasters.Find(invoice.CreatedBy).FirstName : "";
                }

                invoice.DepartmentMasters = db.Database.SqlQuery<EntityModels.Models.DepartmentMaster>("SP_DepartmentMaster @Mode = {0},@StoreId = {1}", "SelectExpense_Department", invoice.StoreId).ToList().Where(s => s.StoreId == invoice.StoreId).ToList();

            }
            catch (Exception ex)
            {
                logger.Error("InvoiceRepository - EditInvoiceObj - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return invoice;
        }

        public InvoiceFlgs CreateInvoiceAutomationPost(InvoiceAutomation invoice, bool RoleAdministrator, bool nnfApprovalInvoice, bool FLG, bool roleFlg, string Store, int StoreFlag, string[] ChildDepartmentId, string[] ChildAmount, int Storeidval, string UserName)
        {
            InvoiceFlgs invoiceFlgs = new InvoiceFlgs();
            try
            {
                if ((invoice.QuickInvoice == "1" && (RoleAdministrator == true || nnfApprovalInvoice == true)) || (FLG == true && roleFlg == false))
                {
                    if (Store != "")
                    {
                        if (Store == "Online" && StoreFlag == 1 && invoice.PaymentTypeId == 2) //Check/ACH
                        {
                            if (invoice.QBTransfer == false)
                            {
                                try
                                {
                                    var newInvoice = new Invoice
                                    {
                                        StoreId = invoice.StoreId,
                                        InvoiceTypeId = invoice.InvoiceTypeId,
                                        PaymentTypeId = invoice.PaymentTypeId,
                                        VendorId = invoice.VendorId,
                                        InvoiceDate = invoice.InvoiceDate,
                                        strInvoiceDate = invoice.strInvoiceDate,
                                        InvoiceNumber = invoice.InvoiceNumber,
                                        Note = invoice.Note,
                                        UploadInvoice = invoice.UploadInvoice,
                                        TotalAmount = invoice.TotalAmount,
                                        DiscountTypeId = invoice.DiscountTypeId,
                                        DiscountPercent = invoice.DiscountPercent,
                                        DiscountAmount = invoice.DiscountAmount,
                                        PDFPageCount = invoice.PDFPageCount,
                                        StatusValue = InvoiceStatusEnm.Approved,
                                        CreatedBy = UserModule.getUserId(),
                                        CreatedOn = DateTime.Now,
                                        LastModifiedOn = DateTime.Now,
                                        ApproveRejectBy = UserModule.getUserId(),
                                        ApproveRejectDate = DateTime.Now,
                                        IsSync = -1,
                                        IsActive = true,
                                        QBTransfer = invoice.QBTransfer,
                                        InvoiceReview = invoice.InvoiceReview,
                                    };
                                    db.Invoices.Add(newInvoice);
                                    db.SaveChanges();

                                    invoiceFlgs.iInvoiceId = newInvoice.InvoiceId;
                                    invoiceFlgs.iInvoiceStatus = newInvoice.StatusValue.ToString();
                                    if (invoiceFlgs.iInvoiceId > 0)
                                    {
                                        int j = 0;
                                        if (invoice.InvoiceTypeId == 2) //Invoice = 1, CreditMemo =2
                                        {
                                            List<VendorCredirDetail> objList1 = new List<VendorCredirDetail>();
                                            if (ChildDepartmentId != null)
                                            {

                                                foreach (var val_id in ChildDepartmentId)
                                                {
                                                    if (Convert.ToDecimal(ChildAmount[j]) > 0) // Added to stop 0 Amount Invoice
                                                    {
                                                        int iID = Convert.ToInt32(val_id);
                                                        VendorCredirDetail objCredit = new VendorCredirDetail();
                                                        objCredit.DepartmentID = db.DepartmentMasters.Where(a => a.StoreId == Storeidval && a.DepartmentId == iID).FirstOrDefault().ListId;
                                                        objCredit.Amount = Convert.ToDecimal(ChildAmount[j]);
                                                        objList1.Add(objCredit);
                                                        objCredit = null;
                                                    }
                                                    j++;
                                                }
                                            }
                                            qBRepository.CreateVendorCredit(Convert.ToInt32(invoiceFlgs.iInvoiceId), objList1, 0);
                                            objList1 = null;
                                        }
                                        else
                                        {
                                            List<BillDetail> objList = new List<BillDetail>();
                                            if (ChildDepartmentId != null)
                                            {
                                                foreach (var val_id in ChildDepartmentId)
                                                {
                                                    if (Convert.ToDecimal(ChildAmount[j]) > 0) // Added to stop 0 Amount Invoice
                                                    {
                                                        int iID = Convert.ToInt32(val_id);
                                                        BillDetail objDetail = new BillDetail();
                                                        objDetail.Description = "";
                                                        objDetail.Amount = Convert.ToDecimal(ChildAmount[j]);
                                                        objDetail.DepartmentID = db.DepartmentMasters.Where(a => a.StoreId == Storeidval && a.DepartmentId == iID).Count() > 0 ? db.DepartmentMasters.Where(a => a.StoreId == Storeidval && a.DepartmentId == iID).FirstOrDefault().ListId : "";
                                                        objList.Add(objDetail);
                                                        objDetail = null;
                                                    }
                                                    j++;
                                                }
                                            }
                                            qBRepository.CreateBill(Convert.ToInt32(invoiceFlgs.iInvoiceId), objList);
                                            objList = null;
                                        }
                                    }
                                }
                                catch (Exception)
                                { }
                            }
                            else
                            {
                                var newInvoice = new Invoice
                                {
                                    StoreId = invoice.StoreId,
                                    InvoiceTypeId = invoice.InvoiceTypeId,
                                    PaymentTypeId = invoice.PaymentTypeId,
                                    VendorId = invoice.VendorId,
                                    InvoiceDate = invoice.InvoiceDate,
                                    strInvoiceDate = invoice.strInvoiceDate,
                                    InvoiceNumber = invoice.InvoiceNumber,
                                    Note = invoice.Note,
                                    UploadInvoice = invoice.UploadInvoice,
                                    TotalAmount = invoice.TotalAmount,
                                    DiscountTypeId = invoice.DiscountTypeId,
                                    DiscountPercent = invoice.DiscountPercent,
                                    DiscountAmount = invoice.DiscountAmount,
                                    PDFPageCount = invoice.PDFPageCount,
                                    CreatedBy = db.UserMasters.Where(s => s.UserName == UserName).FirstOrDefault().UserId,
                                    CreatedOn = DateTime.Now,
                                    LastModifiedOn = DateTime.Now,
                                    Source = "WEB",
                                    StatusValue = InvoiceStatusEnm.Approved,
                                    ApproveRejectBy = db.UserMasters.Where(s => s.UserName == UserName).FirstOrDefault().UserId,
                                    ApproveRejectDate = DateTime.Now,
                                    IsSync = -1,
                                    IsActive = true,
                                    QBTransfer = invoice.QBTransfer,
                                    InvoiceReview = invoice.InvoiceReview,
                                };
                                db.Invoices.Add(newInvoice);
                                db.SaveChanges();
                                invoiceFlgs.iInvoiceId = newInvoice.InvoiceId;
                                invoiceFlgs.iInvoiceStatus = newInvoice.StatusValue.ToString();
                            }
                        }
                        else
                        {
                            var newInvoice = new Invoice
                            {
                                StoreId = invoice.StoreId,
                                InvoiceTypeId = invoice.InvoiceTypeId,
                                PaymentTypeId = invoice.PaymentTypeId,
                                VendorId = invoice.VendorId,
                                InvoiceDate = invoice.InvoiceDate,
                                strInvoiceDate = invoice.strInvoiceDate,
                                InvoiceNumber = invoice.InvoiceNumber,
                                Note = invoice.Note,
                                UploadInvoice = invoice.UploadInvoice,
                                TotalAmount = invoice.TotalAmount,
                                DiscountTypeId = invoice.DiscountTypeId,
                                DiscountPercent = invoice.DiscountPercent,
                                DiscountAmount = invoice.DiscountAmount,
                                PDFPageCount = invoice.PDFPageCount,
                                CreatedBy = db.UserMasters.Where(s => s.UserName == UserName).FirstOrDefault().UserId,
                                CreatedOn = DateTime.Now,
                                LastModifiedOn = DateTime.Now,
                                Source = "WEB",
                                StatusValue = InvoiceStatusEnm.Approved,
                                ApproveRejectBy = db.UserMasters.Where(s => s.UserName == UserName).FirstOrDefault().UserId,
                                ApproveRejectDate = DateTime.Now,
                                IsSync = -1,
                                IsActive = true,
                                QBTransfer = invoice.QBTransfer,
                                InvoiceReview = invoice.InvoiceReview,
                            };
                            db.Invoices.Add(newInvoice);
                            db.SaveChanges();
                            invoiceFlgs.iInvoiceId = newInvoice.InvoiceId;
                            invoiceFlgs.iInvoiceStatus = newInvoice.StatusValue.ToString();
                        }
                    }
                    else
                    {
                        var newInvoice = new Invoice
                        {
                            StoreId = invoice.StoreId,
                            InvoiceTypeId = invoice.InvoiceTypeId,
                            PaymentTypeId = invoice.PaymentTypeId,
                            VendorId = invoice.VendorId,
                            InvoiceDate = invoice.InvoiceDate,
                            strInvoiceDate = invoice.strInvoiceDate,
                            InvoiceNumber = invoice.InvoiceNumber,
                            Note = invoice.Note,
                            UploadInvoice = invoice.UploadInvoice,
                            TotalAmount = invoice.TotalAmount,
                            DiscountTypeId = invoice.DiscountTypeId,
                            DiscountPercent = invoice.DiscountPercent,
                            DiscountAmount = invoice.DiscountAmount,
                            PDFPageCount = invoice.PDFPageCount,
                            CreatedBy = db.UserMasters.Where(s => s.UserName == UserName).FirstOrDefault().UserId,
                            CreatedOn = DateTime.Now,
                            LastModifiedOn = DateTime.Now,
                            Source = "WEB",
                            StatusValue = InvoiceStatusEnm.Approved,
                            ApproveRejectBy = db.UserMasters.Where(s => s.UserName == UserName).FirstOrDefault().UserId,
                            ApproveRejectDate = DateTime.Now,
                            IsSync = -1,
                            IsActive = true,
                            QBTransfer = invoice.QBTransfer,
                            InvoiceReview = invoice.InvoiceReview,
                        };
                        db.Invoices.Add(newInvoice);
                        db.SaveChanges();
                        invoiceFlgs.iInvoiceId = newInvoice.InvoiceId;
                        invoiceFlgs.iInvoiceStatus = newInvoice.StatusValue.ToString();
                    }
                }
                else
                {
                    var newInvoice = new Invoice
                    {
                        StoreId = invoice.StoreId,
                        InvoiceTypeId = invoice.InvoiceTypeId,
                        PaymentTypeId = invoice.PaymentTypeId,
                        VendorId = invoice.VendorId,
                        InvoiceDate = invoice.InvoiceDate,
                        strInvoiceDate = invoice.strInvoiceDate,
                        InvoiceNumber = invoice.InvoiceNumber,
                        Note = invoice.Note,
                        UploadInvoice = invoice.UploadInvoice,
                        TotalAmount = invoice.TotalAmount,
                        DiscountTypeId = invoice.DiscountTypeId,
                        DiscountPercent = invoice.DiscountPercent,
                        DiscountAmount = invoice.DiscountAmount,
                        PDFPageCount = invoice.PDFPageCount,
                        CreatedBy = db.UserMasters.Where(s => s.UserName == UserName).FirstOrDefault().UserId,
                        CreatedOn = DateTime.Now,
                        LastModifiedOn = DateTime.Now,
                        Source = "WEB",
                        StatusValue = InvoiceStatusEnm.Pending,
                        IsSync = -1,
                        IsActive = true,
                        QBTransfer = invoice.QBTransfer,
                        InvoiceReview = invoice.InvoiceReview,
                    };
                    db.Invoices.Add(newInvoice);
                    db.SaveChanges();
                    invoiceFlgs.iInvoiceId = newInvoice.InvoiceId;
                    invoiceFlgs.iInvoiceStatus = newInvoice.StatusValue.ToString();
                }
                if (invoiceFlgs.iInvoiceId != 0)
                {
                    db.Database.ExecuteSqlCommand("SP_InvoiceAutomation @Spara = {0},@InvoiceAutomationId = {1}", "UpdateInvoiceAutomationFlag", invoice.InvoiceAutomationId);
                    db.Database.ExecuteSqlCommand("SP_MultiThreadingLogs @Mode = {0},@InvoiceAutomationId = {1}", "UpdateInvoiceCreated", invoice.InvoiceAutomationId);
                }
            }
            catch (Exception ex)
            {
                logger.Error("InvoiceRepository - CreateInvoiceAutomationPost - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return invoiceFlgs;
        }

        public int CreateInvoiceAutomationCreditmemoPost(InvoiceAutomation invoice, bool ApproveInvoice, bool Administrator, string Store, int StoreFlag, string UserName, int iInvoiceId, string Credit_Invoiceno, string InvoiceNM, string iInvoiceStatus)
        {
            int iiInvoiceID = 0;
            try
            {
                if (invoice.QuickInvoice == "1" && (ApproveInvoice == true || Administrator == true)) // invoice.QuickCRInvoice == "1"
                {
                    if (Store != "")
                    {
                        if (Store == "Online" && StoreFlag == 1 && invoice.PaymentTypeId == 2)
                        {
                            try
                            {

                                Invoice InvObj = db.Invoices.AsNoTracking().FirstOrDefault(a => a.InvoiceId == iInvoiceId);
                                //InvObj = db.Invoices.Find(iInvoiceId);
                                InvObj.strInvoiceDate = InvObj.InvoiceDate.ToString("dd/MM/yyyy");
                                InvObj.CreatedBy = db.UserMasters.Where(s => s.UserName == UserName).FirstOrDefault().UserId;
                                InvObj.CreatedOn = DateTime.Now;
                                InvObj.LastModifiedOn = DateTime.Now;
                                InvObj.Source = "WEB";
                                InvObj.StatusValue = InvoiceStatusEnm.Approved;
                                InvObj.ApproveRejectBy = db.UserMasters.Where(s => s.UserName == UserName).FirstOrDefault().UserId;
                                InvObj.ApproveRejectDate = DateTime.Now;
                                InvObj.RefInvoiceId = iInvoiceId;
                                InvObj.InvoiceTypeId = 2; //CreditMemo
                                InvObj.PaymentTypeId = 2; //Check/ACH
                                InvObj.DiscountTypeId = 1;
                                InvObj.TotalAmount = InvObj.DiscountAmount != null ? invoice.DiscountAmount.Value : InvObj.TotalAmount;
                                InvObj.DiscountAmount = InvObj.DiscountAmount != null ? invoice.DiscountAmount.Value : 0;
                                InvObj.DiscountPercent = InvObj.DiscountPercent == null ? 0 : invoice.DiscountPercent;
                                InvObj.InvoiceNumber = Credit_Invoiceno;
                                invoice.IsActive = true;
                                InvObj.UploadInvoice = InvoiceNM;
                                db.Invoices.Add(InvObj);
                                db.SaveChanges();
                                iiInvoiceID = InvObj.InvoiceId;

                                if (Convert.ToInt32(iiInvoiceID) > 0)
                                {
                                    InvoiceDepartmentDetail deptDetail = new InvoiceDepartmentDetail();
                                    deptDetail.InvoiceId = iiInvoiceID;
                                    deptDetail.DepartmentId = Convert.ToInt32(invoice.Disc_Dept_id);
                                    deptDetail.Amount = Convert.ToDecimal(InvObj.DiscountAmount != null ? invoice.DiscountAmount.Value : InvObj.TotalAmount);
                                    db.InvoiceDepartmentDetails.Add(deptDetail);
                                    db.SaveChanges();

                                    if (invoice.QBTransfer == false)
                                    {
                                        int iID = Convert.ToInt32(invoice.Disc_Dept_id);
                                        List<VendorCredirDetail> objList = new List<VendorCredirDetail>();
                                        VendorCredirDetail objCredit = new VendorCredirDetail();
                                        objCredit.DepartmentID = db.DepartmentMasters.Where(a => a.StoreId == InvObj.StoreId && a.DepartmentId == iID).FirstOrDefault().ListId;
                                        objCredit.Amount = invoice.DiscountAmount;
                                        objList.Add(objCredit);

                                        qBRepository.CreateVendorCredit(Convert.ToInt32(iiInvoiceID), objList, 2);
                                    }
                                }
                                else
                                {
                                    Invoice iiInvoice = db.Invoices.Find(iiInvoiceID);
                                    iiInvoice.Source = "WEB";
                                    db.Entry(iiInvoice).State = EntityState.Modified;
                                    db.SaveChanges();
                                }
                            }
                            catch (Exception) { }
                        }
                        else
                        {
                            Invoice CreditMemo = new Invoice();
                            CreditMemo = db.Invoices.AsNoTracking().FirstOrDefault(a => a.InvoiceId == iInvoiceId);
                            CreditMemo.strInvoiceDate = CreditMemo.InvoiceDate.ToString("dd/MM/yyyy");
                            CreditMemo.InvoiceTypeId = 2; //CreditMemo
                                                          //CreditMemo.StatusValue = InvoiceStatusEnm.Pending;

                            if (iInvoiceStatus == "Approved")
                            {
                                CreditMemo.StatusValue = InvoiceStatusEnm.Approved;
                                CreditMemo.ApproveRejectBy = UserModule.getUserId();
                                CreditMemo.ApproveRejectDate = DateTime.Now;
                                CreditMemo.IsSync = -1;
                            }
                            else
                            {
                                CreditMemo.StatusValue = InvoiceStatusEnm.Pending;
                                CreditMemo.IsSync = 0;
                            }

                            CreditMemo.PaymentTypeId = 2; //Check/ACH
                            CreditMemo.DiscountTypeId = 1; //N/A
                            CreditMemo.TotalAmount = invoice.DiscountAmount != null ? invoice.DiscountAmount.Value : invoice.TotalAmount;
                            CreditMemo.DiscountAmount = invoice.DiscountAmount != null ? invoice.DiscountAmount.Value : 0;
                            CreditMemo.DiscountPercent = invoice.DiscountPercent != null ? invoice.DiscountPercent.Value : 0;
                            CreditMemo.RefInvoiceId = iInvoiceId;
                            CreditMemo.UploadInvoice = InvoiceNM;
                            CreditMemo.InvoiceNumber = Credit_Invoiceno;
                            CreditMemo.CreatedBy = db.UserMasters.Where(s => s.UserName == UserName).FirstOrDefault().UserId;
                            CreditMemo.CreatedOn = DateTime.Now;
                            CreditMemo.LastModifiedOn = DateTime.Now;
                            CreditMemo.Source = "WEB";
                            CreditMemo.IsActive = true;
                            db.Invoices.Add(CreditMemo);
                            db.SaveChanges();

                            if (CreditMemo.InvoiceId > 0)
                            {
                                InvoiceDepartmentDetail deptDetail1 = new InvoiceDepartmentDetail();
                                deptDetail1.InvoiceId = CreditMemo.InvoiceId;
                                deptDetail1.DepartmentId = Convert.ToInt32(invoice.Disc_Dept_id);
                                deptDetail1.Amount = Convert.ToDecimal(invoice.DiscountAmount != null ? invoice.DiscountAmount.Value : invoice.TotalAmount);
                                db.InvoiceDepartmentDetails.Add(deptDetail1);
                                db.SaveChanges();


                                //New implement
                                if (invoice.QBTransfer == false && iInvoiceStatus == "Approved")
                                {
                                    int iID = Convert.ToInt32(invoice.Disc_Dept_id);
                                    List<VendorCredirDetail> objList = new List<VendorCredirDetail>();
                                    VendorCredirDetail objCredit = new VendorCredirDetail();
                                    objCredit.DepartmentID = db.DepartmentMasters.Where(a => a.StoreId == CreditMemo.StoreId && a.DepartmentId == iID).FirstOrDefault().ListId;
                                    objCredit.Amount = invoice.DiscountAmount;
                                    objList.Add(objCredit);

                                    qBRepository.CreateVendorCredit(Convert.ToInt32(CreditMemo.InvoiceId), objList, 2);
                                }

                            }
                        }
                    }
                    else
                    {
                        Invoice CreditMemo = new Invoice();
                        CreditMemo = db.Invoices.AsNoTracking().FirstOrDefault(a => a.InvoiceId == iInvoiceId);
                        CreditMemo.strInvoiceDate = CreditMemo.InvoiceDate.ToString("dd/MM/yyyy");
                        CreditMemo.InvoiceTypeId = 2;
                        CreditMemo.StatusValue = InvoiceStatusEnm.Approved;
                        CreditMemo.PaymentTypeId = 2; //Check/ACH
                        CreditMemo.DiscountTypeId = 1; //N/A
                        CreditMemo.InvoiceNumber = Credit_Invoiceno;
                        CreditMemo.DiscountAmount = invoice.DiscountAmount != null ? invoice.DiscountAmount.Value : 0;
                        CreditMemo.DiscountPercent = invoice.DiscountPercent != null ? invoice.DiscountPercent.Value : 0;
                        CreditMemo.TotalAmount = invoice.DiscountAmount != null ? invoice.DiscountAmount.Value : invoice.TotalAmount;
                        CreditMemo.CreatedBy = db.UserMasters.Where(s => s.UserName == UserName).FirstOrDefault().UserId;
                        CreditMemo.CreatedOn = DateTime.Now;
                        CreditMemo.LastModifiedOn = DateTime.Now;
                        CreditMemo.ApproveRejectBy = db.UserMasters.Where(s => s.UserName == UserName).FirstOrDefault().UserId;
                        CreditMemo.ApproveRejectDate = DateTime.Now;
                        CreditMemo.RefInvoiceId = iInvoiceId;
                        CreditMemo.UploadInvoice = InvoiceNM;
                        CreditMemo.Source = "WEB";
                        CreditMemo.IsActive = true;
                        db.Invoices.Add(CreditMemo);
                        db.SaveChanges();
                        iiInvoiceID = CreditMemo.InvoiceId;

                        if (iiInvoiceID > 0)
                        {
                            InvoiceDepartmentDetail deptDetail1 = new InvoiceDepartmentDetail();
                            deptDetail1.InvoiceId = iiInvoiceID;
                            deptDetail1.DepartmentId = Convert.ToInt32(invoice.Disc_Dept_id);
                            deptDetail1.Amount = Convert.ToDecimal(invoice.DiscountAmount != null ? invoice.DiscountAmount.Value : invoice.TotalAmount);
                            db.InvoiceDepartmentDetails.Add(deptDetail1);
                            db.SaveChanges();
                        }
                    }
                }
                else
                {
                    Invoice InvObj = db.Invoices.AsNoTracking().FirstOrDefault(a => a.InvoiceId == iInvoiceId);
                    InvObj.strInvoiceDate = invoice.InvoiceDate.ToString("dd/MM/yyyy");
                    InvObj.InvoiceTypeId = 2;
                    InvObj.PaymentTypeId = 2; //Check/ACH
                    InvObj.InvoiceNumber = Credit_Invoiceno;
                    InvObj.IsSync = -1;

                    if (iInvoiceStatus == "Approved")
                    {
                        InvObj.StatusValue = InvoiceStatusEnm.Approved;
                        InvObj.ApproveRejectBy = UserModule.getUserId();
                        InvObj.ApproveRejectDate = DateTime.Now;
                        InvObj.LastModifiedOn = DateTime.Now;
                        InvObj.IsSync = -1;
                    }
                    else
                    {
                        InvObj.StatusValue = InvoiceStatusEnm.Pending;
                        InvObj.IsSync = 0;
                    }
                    InvObj.DiscountTypeId = 1; //N/A
                    InvObj.CreatedBy = db.UserMasters.Where(s => s.UserName == UserName).FirstOrDefault().UserId;
                    InvObj.CreatedOn = DateTime.Now;
                    InvObj.LastModifiedOn = DateTime.Now;
                    InvObj.RefInvoiceId = iInvoiceId;
                    InvObj.Source = "WEB";
                    InvObj.IsActive = true;
                    InvObj.TotalAmount = Convert.ToDecimal(invoice.DiscountAmount != null ? invoice.DiscountAmount.Value : invoice.TotalAmount);
                    InvObj.UploadInvoice = InvoiceNM;
                    db.Invoices.Add(InvObj);
                    db.SaveChanges();
                    iiInvoiceID = InvObj.InvoiceId;

                    InvoiceDepartmentDetail deptDetail = new InvoiceDepartmentDetail();
                    deptDetail.InvoiceId = iiInvoiceID;
                    deptDetail.DepartmentId = Convert.ToInt32(invoice.Disc_Dept_id);
                    deptDetail.Amount = Convert.ToDecimal(invoice.DiscountAmount != null ? invoice.DiscountAmount.Value : InvObj.TotalAmount);
                    db.InvoiceDepartmentDetails.Add(deptDetail);
                    db.SaveChanges();


                    //New implement
                    if (invoice.QBTransfer == false && iInvoiceStatus == "Approved")
                    {
                        int iID = Convert.ToInt32(invoice.Disc_Dept_id);
                        List<VendorCredirDetail> objList = new List<VendorCredirDetail>();
                        VendorCredirDetail objCredit = new VendorCredirDetail();
                        objCredit.DepartmentID = db.DepartmentMasters.Where(a => a.StoreId == InvObj.StoreId && a.DepartmentId == iID).FirstOrDefault().ListId;
                        objCredit.Amount = invoice.DiscountAmount;
                        objList.Add(objCredit);

                        qBRepository.CreateVendorCredit(Convert.ToInt32(iiInvoiceID), objList, 2);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("InvoiceRepository - CreateInvoiceAutomationCreditmemoPost - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return iiInvoiceID;
        }

        public void UpdateInvoiceReview(int invoiceid, string reviewnote)
        {
            try
            {
                db.Database.ExecuteSqlCommand("SP_Invoice @Mode = {0},@InvoiceId = {1},@InvoiceReview = {2}", "UpdateInvoiceUserReview", invoiceid, reviewnote);
            }
            catch (Exception ex)
            {
                logger.Error("InvoiceRepository - UpdateInvoiceReview - " + DateTime.Now + " - " + ex.Message.ToString());
            }
        }

        public int UploadProcessStart()
        {
            int MultithreadingInvoiceLogId = 0;
            try
            {
                string strcon = "";
                strcon = System.Configuration.ConfigurationManager.ConnectionStrings["DBConn"].ConnectionString;
                SqlConnection Con = new SqlConnection(strcon);
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = Con;
                cmd.CommandText = "[SP_MultiThreadingLogs]";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Mode", "ProcessStart");
                cmd.Parameters.AddWithValue("@Ids", 0).Direction = ParameterDirection.Output;
                Con.Open();
                cmd.ExecuteNonQuery();
                MultithreadingInvoiceLogId = Convert.ToInt32(cmd.Parameters["@Ids"].Value.ToString());
                Con.Close();
            }
            catch (Exception ex)
            {
                logger.Error("InvoiceRepository - UploadProcessStart - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return MultithreadingInvoiceLogId;
        }

        public void UploadProcessFinish(int MultithreadingInvoiceLogId, int UploadPdfAutomationId)
        {
            try
            {
                db.Database.ExecuteSqlCommand("SP_MultiThreadingLogs @Mode = {0},@MultithreadingInvoiceLogId = {1},@UploadPdfAutomationId = {2}", "ProcessFinish", MultithreadingInvoiceLogId, UploadPdfAutomationId);
            }
            catch (Exception ex)
            {
                logger.Error("InvoiceRepository - UploadProcessFinish - " + DateTime.Now + " - " + ex.Message.ToString());
            }
        }

        public void UserlogDeleteInsert(string InvoiceNumber, int DeletedBy)
        {
            try
            {
                db.Database.ExecuteSqlCommand("SP_UserLogUpdate @Mode = {0},@InvoiceNumber = {1},@DeletedBy = {2}", "InvoiceDeleteLogInsert", InvoiceNumber, DeletedBy);
            }
            catch (Exception ex)
            {
                logger.Error("InvoiceRepository - UserlogDeleteInsert - " + DateTime.Now + " - " + ex.Message.ToString());
            }
        }

        public void UpdateInvoiceTasks(int invoiceid, string invoicetask, string invoiceno, int createdby, int priorityid, int assignid, DateTime duedatetask)
        {
            try
            {
                db.Database.ExecuteSqlCommand("SP_Invoice @Mode = {0},@InvoiceId = {1},@TaskDescription = {2},@TaskName = {3},@PriorityId = {4},@AssignTo = {5},@DueDate = {6},@CreatedBy = {7}", "UpdateInvoiceUserTasks", invoiceid, invoicetask, invoiceno, priorityid, assignid, duedatetask, createdby);
            }
            catch (Exception ex)
            {
                logger.Error("InvoiceRepository - UpdateInvoiceTasks - " + DateTime.Now + " - " + ex.Message.ToString());
            }
        }

        public Tuple<List<InvoiceSelect>, int> InvoiceSyncfusionGrid_Data(string SearchTerm, string SortFields, string SortDirections, string Filters, int Skip, int Take, int UserTypeId, string Dept_id, string Store_id)
        {
            List<InvoiceSelect> BindData = new List<InvoiceSelect>();
            int totalCount = 0;
            try
            {
                var totalCountParam = new SqlParameter("@TotalCount", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };

                var sqlCommand = "EXEC SP_InvoiceData_Dashboard_Test @SearchTerm, @SortFields, @SortDirections, @Filters, @Skip, @Take, @UserTypeID, @Dept_id, @Store_id, @TotalCount OUTPUT";
                var parameters = new[]
                {
                    new SqlParameter("@SearchTerm", SearchTerm ?? (object)DBNull.Value),
                    new SqlParameter("@SortFields", SortFields ?? (object)DBNull.Value),
                    new SqlParameter("@SortDirections", SortDirections ?? (object)DBNull.Value),
                    new SqlParameter("@Filters", Filters ?? (object)DBNull.Value),
                    new SqlParameter("@Skip", Skip),
                    new SqlParameter("@Take", Take),
                    new SqlParameter("@UserTypeID", UserTypeId),
                    new SqlParameter("@Dept_id", Dept_id ?? (object)DBNull.Value),
                    new SqlParameter("@Store_id", Store_id ?? (object)DBNull.Value),
                    totalCountParam
                };
                BindData = db.Database.SqlQuery<InvoiceSelect>(sqlCommand, parameters).ToList();


                //BindData = db.Database.SqlQuery<InvoiceSelect>("SP_InvoiceData_Dashboard_Test @SearchTerm = {0}, @SortFields = {1},@SortDirections = {2}, @Filters = {3}, " +
                //    "@Skip = {4}, @Take = {5}, @UserTypeID = {6}, @Dept_id = {7}, @Store_id = {8}, @TotalCount = {9}",
                //    SearchTerm, SortFields, SortDirections, Filters, Skip, Take, UserTypeId, Dept_id, Store_id, totalCountParam).ToList();

                totalCount = (int)totalCountParam.Value;
            }
            catch (Exception ex)
            {
                logger.Error("InvoiceRepository - InvoiceDataDepartmentWise_Get - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return new Tuple<List<InvoiceSelect>, int>(BindData, totalCount);
        }

        public List<InvoiceFilter> GetInvoiceFilteredRecords()
        {
            List<InvoiceFilter> lstInvoice = new List<InvoiceFilter>();
            try
            {
                lstInvoice = db.Database.SqlQuery<InvoiceFilter>("SP_GetInvoiceFilteredRecords").ToList();

            }
            catch (Exception ex)
            {
                logger.Error("InvoiceRepository - GetInvoiceFilteredRecords - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return lstInvoice;
        }

        public List<InvoicePaymentStatusDetailsList> InvoicePaymentStatus(int InvoiceID)
        {
            List<InvoicePaymentStatusDetailsList> data = new List<InvoicePaymentStatusDetailsList>();
            try
            {
                //data = db.InvoicePaymentStatusDetails.Where(a => a.InvoiceId == InvoiceID).OrderByDescending(a => a.QbPaymentDate).ToList();
                data = db.Database.SqlQuery<InvoicePaymentStatusDetailsList>("SPInvoice_Console @Spara={0}, @InvoiceId={1}", 8, InvoiceID).ToList();
            }
            catch (Exception ex)
            {
                logger.Error("InvoiceRepository - InvoicePaymentStatus - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return data;
        }
    }
}
