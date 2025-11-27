using EntityModels.HRModels;
using EntityModels.Models;
using NLog;
using Repository.IRepository;
using SynthesisQBOnline.BAL;
using SynthesisViewModal;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;

namespace Repository
{
    public class ExpenseAccountsRepository : IExpenseAccountsRepository
    {
        private DBContext db;
        Logger logger = LogManager.GetCurrentClassLogger();
        List<SpExpenseCheck_Setting> lstSpExpenseCheck_Setting = new List<SpExpenseCheck_Setting>();

        public ExpenseAccountsRepository(DBContext context)
        {
            db = context;
        }
        public List<SpExpenseCheck_Setting> ExpenseCheck_SettingList(int storeid, int Type)
        {
            try
            {
                var DepartmentList = db.DepartmentMasters.Where(s => s.StoreId == storeid).ToList();

                lstSpExpenseCheck_Setting = db.Database.SqlQuery<SpExpenseCheck_Setting>("GetExpenseCheck_Setting @storeId = {0},@TypeId={1}", storeid, Type).ToList().Select(s => new SpExpenseCheck_Setting
                {
                    DepartmentId = s.DepartmentId,
                    DepartmentName = s.DepartmentName,
                    IsActive = Convert.ToBoolean(s.IsActive),
                    Type = s.Type,
                    StoreId = s.StoreId,
                    ExcludeList = s.ExcludeList,
                    DeptExcludeList = (from a in DepartmentList
                                       select new DrpListStr
                                       {
                                           Id = s.DepartmentId + "_" + a.DepartmentId,
                                           Name = a.DepartmentName.ToString().Replace("&amp;", "&")
                                       }).ToList(),
                }).ToList();
            }
            catch (Exception ex)
            {
                logger.Error("ExpenseAccountsRepository - ExpenseCheck_SettingList - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return lstSpExpenseCheck_Setting;
        }
        public void ExpenseCheck_SettingsRemoveRange(int StoreIDs, int TypeID)
        {
            try
            {
                db.ExpenseCheck_Settings.RemoveRange(db.ExpenseCheck_Settings.Where(s => s.StoreId == StoreIDs && s.Type == TypeID));
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Error("ExpenseAccountsRepository - ExpenseCheck_SettingsRemoveRange - " + DateTime.Now + " - " + ex.Message.ToString());
            }
        }
        public void AddExpenseCheck_Settings(ExpenseCheck_Setting obj)
        {
            try
            {
                db.ExpenseCheck_Settings.Add(obj);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Error("ExpenseAccountsRepository - AddExpenseCheck_Settings - " + DateTime.Now + " - " + ex.Message.ToString());
            }
        }
        public string GetExpenseCheck_StoreList(int StoreId, int UserID, bool IsAuth)
        {
            string strList = "";
            try
            {
                if (IsAuth)
                {
                    if (StoreId == 0)
                    {
                        var list = db.Database.SqlQuery<int>("SP_GetStore_ForDashboard @Mode={0},@ModuleId={1},@UserTypeId={2}", "GetStore_ForExpenseCheck", 17, UserID);
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
                logger.Error("ExpenseAccountsRepository - GetExpenseCheck_StoreList - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return strList;
        }
        public string GetStoreName(int StoreId)
        {
            string strList = "";
            try
            {
                if (StoreId == 0)
                {
                    strList = db.StoreMasters.Where(a => a.StoreId == StoreId).FirstOrDefault().NickName;
                }
                else
                {
                    strList = StoreId.ToString();
                }
            }
            catch (Exception ex)
            {
                logger.Error("ExpenseAccountsRepository - GetStoreName - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return strList;
        }
        public List<ExpenseCheckSelect> GetExpenseCheck_BindData(ExpenseCheckSettingViewModal ExpenseCheckSettingViewModal, string strStore, int UserTypeID, string AscDsc)
        {
            List<ExpenseCheckSelect> lstExpenseCheckSelect = new List<ExpenseCheckSelect>();
            try
            {
                db.Database.SqlQuery<ExpenseCheckSelect>("[SP_ExpenseCheckPreviewData] @searchbox={0},@startdate={1},@enddate={2},@SearchType={3},@StoreID={4},@UserTypeID={5},@AmtMaximum={6},@AmtMinimum={7},@Payment_type={8},@Dept_id={9},@orderby={10},@AscDsc={11}", ExpenseCheckSettingViewModal.searchdashbord.Replace("&amp;", "&"), (ExpenseCheckSettingViewModal.startdate.ToString() == "" ? null : ExpenseCheckSettingViewModal.startdate.ToString()), (ExpenseCheckSettingViewModal.enddate.ToString() == "" ? null : ExpenseCheckSettingViewModal.enddate.ToString()), ExpenseCheckSettingViewModal.SearchFlg, ((strStore == "" || strStore == "0") ? null : strStore.ToString()), UserTypeID, ExpenseCheckSettingViewModal.AmtMaximum == 0 ? null : ExpenseCheckSettingViewModal.AmtMaximum, ExpenseCheckSettingViewModal.AmtMinimum == 0 ? null : ExpenseCheckSettingViewModal.AmtMinimum, ExpenseCheckSettingViewModal.payment, ExpenseCheckSettingViewModal.deptid == 0 ? null : ExpenseCheckSettingViewModal.deptid, ExpenseCheckSettingViewModal.orderby, AscDsc).ToList();
            }
            catch (Exception ex)
            {
                logger.Error("ExpenseAccountsRepository - GetExpenseCheck_BindData - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return lstExpenseCheckSelect;
        }
        public List<int> GetExpenseCheckDetails()
        {
            List<int> lstExpenseCheckDetail = new List<int>();
            try
            {
                lstExpenseCheckDetail = db.ExpenseCheckDetails.Where(s => s.DepartmentId != null).Select(s => s.DepartmentId.Value).Distinct().ToList();
            }
            catch (Exception ex)
            {
                logger.Error("ExpenseAccountsRepository - GetExpenseCheckDetails - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return lstExpenseCheckDetail;
        }
        public List<ExpenseCheckDocuments> GETExpenseCheckDocuments(int ExpenseCheckID)
        {
            List<ExpenseCheckDocuments> lstExpenseCheckDocuments = new List<ExpenseCheckDocuments>();
            try
            {
                lstExpenseCheckDocuments = db.expenseCheckDocuments.Where(w => w.ExpenseCheckId == ExpenseCheckID).ToList();
            }
            catch (Exception ex)
            {
                logger.Error("ExpenseAccountsRepository - GETExpenseCheckDocuments - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return lstExpenseCheckDocuments;
        }
        public bool CheckUserTypeMasters(int UserTypeId)
        {
            return db.UserTypeMasters.Any(s => s.UserTypeId == UserTypeId && s.IsViewInvoiceOnly == true);
        }
        public List<int> GetExpenseDeptIds(int UserTypeId, int StoreID)
        {
            List<int?> RightsStore = new List<int?>();
            List<int> ExpenseDeptIds = new List<int>();
            try
            {
                RightsStore = db.RightsStores.Where(s => s.UserTypeId == UserTypeId && s.StoreId == StoreID).Select(k => k.DepartmentId).ToList();
                ExpenseDeptIds = db.ExpenseCheckDetails.Where(s => RightsStore.Contains(s.DepartmentId) && s.DepartmentId != null).Select(s => s.DepartmentId.Value).Distinct().ToList();
            }
            catch (Exception ex)
            {
                logger.Error("ExpenseAccountsRepository - GetExpenseDeptIds - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return ExpenseDeptIds;
        }
        public List<DepartmentMaster> GetDepartmentMasters(List<int> ExpenseDeptIds, int StoreID)
        {
            List<DepartmentMaster> lstDepartmentMaster = new List<DepartmentMaster>();
            try
            {
                lstDepartmentMaster = db.DepartmentMasters.Where(s => s.IsActive == true && s.StoreId == StoreID && ExpenseDeptIds.Contains(s.DepartmentId)).OrderBy(o => o.DepartmentName).ToList();
            }
            catch (Exception ex)
            {
                logger.Error("ExpenseAccountsRepository - GetDepartmentMasters - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return lstDepartmentMaster;
        }
        public List<ExpenseCheckSelect> ExpenseCheckPreviewData(ExpenseCheckSettingViewModal ExpenseCheckSettingViewModal, int UserTypeId, string strStore)
        {
            List<ExpenseCheckSelect> lstExpenseCheckSelect = new List<ExpenseCheckSelect>();
            try
            {
                lstExpenseCheckSelect = db.Database.SqlQuery<ExpenseCheckSelect>("[SP_ExpenseCheckPreviewData] @searchbox={0},@startdate={1},@enddate={2},@SearchType={3},@StoreID={4},@UserTypeID={5}", ExpenseCheckSettingViewModal.searchdashbord.Replace("&amp;", "&"), (ExpenseCheckSettingViewModal.startdate.ToString() == "" ? null : ExpenseCheckSettingViewModal.startdate.ToString()), (ExpenseCheckSettingViewModal.enddate.ToString() == "" ? null : ExpenseCheckSettingViewModal.enddate.ToString()), "F", ((strStore == "" || strStore == "0") ? null : strStore.ToString()), UserTypeId).ToList();
            }
            catch (Exception ex)
            {
                logger.Error("ExpenseAccountsRepository - ExpenseCheckPreviewData - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return lstExpenseCheckSelect;
        }
        public List<ExpenseCheckSelect> ExpenseCheckPreviewData_Beta(string payment, string Orderby, string AscDsc, string Status, string ForceStatus, int UserTypeId, string strStore, string Startdate = null, string Enddate = null)
        {
            List<ExpenseCheckSelect> lstExpenseCheckSelect = new List<ExpenseCheckSelect>();
            try
            {
                lstExpenseCheckSelect = db.Database.SqlQuery<ExpenseCheckSelect>("[SP_ExpenseCheckPreviewData_Beta] @searchbox={0},@startdate={1},@enddate={2},@SearchType={3},@StoreID={4},@UserTypeID={5},@AmtMaximum={6},@AmtMinimum={7},@Payment_type={8},@Dept_id={9},@orderby={10},@AscDsc={11},@Type={12},@Status={13}", "", Startdate, Enddate, "", ((strStore == "" || strStore == "0") ? null : strStore.ToString()), UserTypeId, null, null, payment, null, Orderby, AscDsc, Status, ForceStatus).ToList();
            }
            catch (Exception ex)
            {
                logger.Error("ExpenseAccountsRepository - ExpenseCheckPreviewData_Beta - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return lstExpenseCheckSelect;
        }
        public List<ExpenseCheckExcludedList> ExpenseCheckPreviewData(int UserTypeId, string strStore)
        {
            List<ExpenseCheckExcludedList> lstExpenseCheckExcludedList = new List<ExpenseCheckExcludedList>();
            try
            {
                lstExpenseCheckExcludedList = db.Database.SqlQuery<ExpenseCheckExcludedList>("[SP_ExpenseCheckExcludedList] @StoreID={0},@UserTypeID={1}", ((strStore == "" || strStore == "0") ? null : strStore.ToString()), UserTypeId).ToList();
            }
            catch (Exception ex)
            {
                logger.Error("ExpenseAccountsRepository - ExpenseCheckPreviewData - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return lstExpenseCheckExcludedList;
        }
        public string GetStore_ForExpenseCheck(int UserId)
        {
            string strList = "";
            try
            {
                var list = db.Database.SqlQuery<int>("SP_GetStore_ForDashboard @Mode={0},@ModuleId={1},@UserTypeId={2}", "GetStore_ForExpenseCheck", 17, UserId);
                if (list != null) { strList = String.Join(",", list); }
                list = null;
            }
            catch (Exception ex)
            {
                logger.Error("ExpenseAccountsRepository - GetStore_ForExpenseCheck - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return strList;
        }
        public void AddexpenseCheckDocuments(ExpenseCheckDocuments expenseCheckDocuments)
        {
            try
            {
                db.expenseCheckDocuments.Add(expenseCheckDocuments);
                db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                logger.Error("ExpenseAccountsRepository - AddexpenseCheckDocuments - " + DateTime.Now + " - " + ex.Message.ToString());
            }
        }
        public void UpdateExpenseCheckDetail(int ExpenseId, bool IncludeBySetting)
        {
            try
            {
                ExpenseCheckDetail obj = new ExpenseCheckDetail();
                obj = db.ExpenseCheckDetails.Find(ExpenseId);
                obj.IncludeBySetting = IncludeBySetting;
                db.Entry(obj).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Error("ExpenseAccountsRepository - UpdateExpenseCheckDetail - " + DateTime.Now + " - " + ex.Message.ToString());
            }
        }
        public void UpdateExpenseCheckDetail(bool flag, int ExpenseId)
        {
            try
            {
                ExpenseCheckDetail obj = new ExpenseCheckDetail();
                obj = db.ExpenseCheckDetails.Find(ExpenseId);
                obj.Flag = flag;
                db.Entry(obj).State = EntityState.Modified;
                db.SaveChanges();
                obj = null;
            }
            catch (Exception ex)
            {
                logger.Error("ExpenseAccountsRepository - UpdateExpenseCheckDetail - " + DateTime.Now + " - " + ex.Message.ToString());
            }
        }
        public string DeleteexpenseCheckDocuments(int Id)
        {
            string DocumentName = "";
            try
            {
                ExpenseCheckDocuments Data = db.expenseCheckDocuments.Find(Id);
                DocumentName = Data.DocumentName;
                db.expenseCheckDocuments.Remove(Data);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Error("ExpenseAccountsRepository - DeleteexpenseCheckDocuments - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return DocumentName;
        }

        public ExpenseCheck InsertExpenseData(ExpenseCheck expense)
        {
            try
            {
                logger.Info("ExpenseAccountsRepository - InsertExpenseData - " + DateTime.Now);
                //db.Database.ExecuteSqlCommand("SP_ExpenseCheck @Mode={0},@BankAccountId = {1},@PaymentTypeId = {2},@VendorId = {3},@StoreId = {4},@TotalAmt = {5},@DocNumber = {6},@Memo = {7},@RefType = {8},@FileName = {9},@CreatedBy = {10}", "SaveorUpdateExpenseCheckMaster", expense.BankAccountId, null, expense.VendorId, expense.StoreId, expense.TotalAmt, expense.DocNumber,expense.Memo, null, null, expense.CreatedBy);
                var idParam = new SqlParameter("@Ids", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };

                var sqlCommand = "EXEC SP_ExpenseCheck @Mode, @ExpenseCheckId, @BankAccountId, @PaymentType, @VendorId, @StoreId, " +
                    "@TotalAmt, @DocNumber, @Memo, @RefType,@FileName, @SyncToken,@TXNId,@TxnDate,@CreatedBy, @CreateTime,@UpdatedBy,@UpdatedDate,@ExpenseCheckDetailId," +
                    "@DepartmentId,@Amount,@Description,@PrintLater,@MailingAddress,@PaymentMethodId,@PaymentMethod,@PayQbId,@IsActive,@ExpenseCheckIds,@QBErrorMessage,@PaymentDate,@Ids OUTPUT";
                var parameters = new[]
                {
                    new SqlParameter("@Mode", "SaveorUpdateExpenseCheckMaster"),
                    new SqlParameter("@ExpenseCheckId", expense.ExpenseCheckId),
                    new SqlParameter("@BankAccountId", expense.BankAccountId ?? (object)DBNull.Value),
                    new SqlParameter("@PaymentType", expense.PaymentTypeId ?? (object)DBNull.Value),
                    new SqlParameter("@VendorId", expense.VendorId ?? (object)DBNull.Value),
                    new SqlParameter("@StoreId", expense.StoreId ?? (object)DBNull.Value),
                    new SqlParameter("@TotalAmt", expense.TotalAmt ?? 0.00M) { DbType = DbType.Decimal,Precision = 18,Scale = 2},
                    new SqlParameter("@DocNumber", expense.DocNumber ?? (object)DBNull.Value),
                    new SqlParameter("@Memo", expense.Memo ?? (object)DBNull.Value),
                    new SqlParameter("@RefType", expense.RefType ?? (object)DBNull.Value),
                    new SqlParameter("@FileName", (object)DBNull.Value),
                    new SqlParameter("@SyncToken", (object)DBNull.Value),
                    new SqlParameter("@TXNId", (object)DBNull.Value),
                    new SqlParameter("@TxnDate", expense.TxnDate ?? (object)DBNull.Value),
                    new SqlParameter("@CreatedBy", expense.CreatedBy ?? (object)DBNull.Value),
                    new SqlParameter("@CreateTime", (object)DBNull.Value),
                    new SqlParameter("@UpdatedBy", expense.UpdatedBy ?? (object)DBNull.Value),
                    new SqlParameter("@UpdatedDate", (object)DBNull.Value),
                    new SqlParameter("@ExpenseCheckDetailId", (object)DBNull.Value),
                    new SqlParameter("@DepartmentId", (object)DBNull.Value),
                    new SqlParameter("@Amount", (object)DBNull.Value),
                    new SqlParameter("@Description", (object)DBNull.Value),
                    new SqlParameter("@PrintLater", expense.PrintLater),
                    new SqlParameter("@MailingAddress", expense.MailingAddress ?? (object)DBNull.Value),
                    new SqlParameter("@PaymentMethodId", expense.PaymentMethodId ?? (object)DBNull.Value),
                    new SqlParameter("@PaymentMethod", (object)DBNull.Value),
                    new SqlParameter("@PayQbId", (object)DBNull.Value),
                    new SqlParameter("@IsActive", (object)DBNull.Value),
                    new SqlParameter("@ExpenseCheckIds", (object)DBNull.Value),
                    new SqlParameter("@QBErrorMessage",expense.QBErrorMessage ?? (object)DBNull.Value),
                    new SqlParameter("@PaymentDate", (object)DBNull.Value),
                    idParam
            };
                db.Database.ExecuteSqlCommand(sqlCommand, parameters);

                expense.ExpenseCheckId = (int)idParam.Value;
            }
            catch (Exception ex)
            {
                logger.Error("ExpenseAccountsRepository - InsertExpenseData - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return expense;
        }

        public List<ExpensePaymentMethodMaster> PaymentMethodList()
        {
            List<ExpensePaymentMethodMaster> obj = new List<ExpensePaymentMethodMaster>();
            try
            {
                obj = db.ExpensePaymentMethodMaster.ToList();
            }
            catch (Exception ex)
            {
                logger.Error("ExpenseAccountsRepository - PaymentMethodList - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return obj;
        }

        public ExpenseCheckDetail InsertExpenseDetailsData(ExpenseCheckDetail expensedetails)
        {
            try
            {
                logger.Info("ExpenseAccountsRepository - InsertExpenseDetailsData - " + DateTime.Now);
                db.Database.ExecuteSqlCommand("SP_ExpenseCheck @Mode={0},@ExpenseCheckDetailId={1},@ExpenseCheckId={2},@DepartmentId={3},@Amount={4},@Description={5}", "SaveorUpdateExpenseCheckMasterDetails", expensedetails.ExpenseCheckDetailId, expensedetails.ExpenseCheckId, expensedetails.DepartmentId, expensedetails.Amount, expensedetails.Description);
            }
            catch (Exception ex)
            {
                logger.Error("ExpenseAccountsRepository - InsertExpenseDetailsData - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return expensedetails;
        }

        public List<ExpenseCheckList> GetExpenseCheckList(int storeid)
        {
            List<ExpenseCheckList> obj = new List<ExpenseCheckList>();
            try
            {
                obj = db.Database.SqlQuery<ExpenseCheckList>("SP_ExpenseCheck @Mode = {0}, @StoreId = {1}", "GetExpenseList", storeid).ToList();
            }
            catch (Exception ex)
            {
                logger.Error("ExpenseAccountsRepository - GetExpenseCheckList - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return obj;
        }

        public ExpenseCheck GetExpenseCheckById(int ExpenseCheckId)
        {
            ExpenseCheck obj = new ExpenseCheck();
            try
            {
                logger.Info("ExpenseAccountsRepository - GetExpenseCheckById - " + DateTime.Now);
                obj = db.Database.SqlQuery<ExpenseCheck>("SP_ExpenseCheck @Mode = {0},@ExpenseCheckId = {1}", "GetExpenseCheckById", ExpenseCheckId).FirstOrDefault();
            }
            catch (Exception ex)
            {
                logger.Error("ExpenseAccountsRepository - GetExpenseCheckById - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return obj;
        }

        public List<ExpenseCheckDetail> GetExpenseCheckDetailsById(int ExpenseCheckId)
        {
            List<ExpenseCheckDetail> obj = new List<ExpenseCheckDetail>();
            try
            {
                logger.Info("ExpenseAccountsRepository - GetExpenseCheckDetailsById - " + DateTime.Now);
                obj = db.Database.SqlQuery<ExpenseCheckDetail>("SP_ExpenseCheck @Mode = {0},@ExpenseCheckId = {1}", "GetExpenseCheckDetailById", ExpenseCheckId).ToList();
            }
            catch (Exception ex)
            {
                logger.Error("ExpenseAccountsRepository - GetExpenseCheckDetailsById - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return obj;
        }

        public void DeleteExpenseDetailsData(int ExpenseCheckDetailId)
        {
            try
            {
                db.Database.ExecuteSqlCommand("SP_ExpenseCheck @Mode={0},@ExpenseCheckDetailId={1}", "DeleteExpenseCheckDetailById", ExpenseCheckDetailId);
            }
            catch (Exception ex)
            {
                logger.Error("ExpenseAccountsRepository - DeleteExpenseDetailsData - " + DateTime.Now + " - " + ex.Message.ToString());
            }
        }

        public ExpenseCheckDocuments InsertExpenseFileData(ExpenseCheckDocuments expensefile)
        {
            try
            {
                logger.Info("ExpenseAccountsRepository - InsertExpenseFileData - " + DateTime.Now);
                db.Database.ExecuteSqlCommand("SP_ExpenseCheck @Mode={0},@ExpenseCheckId={1},@FileName={2},@CreatedBy={3}", "SaveExpenseFile", expensefile.ExpenseCheckId, expensefile.DocumentName, expensefile.CreatedBy);
            }
            catch (Exception ex)
            {
                logger.Error("ExpenseAccountsRepository - InsertExpenseFileData - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return expensefile;
        }

        public void DeleteExpenseCheckData(ExpenseCheck expensevalue)
        {
            try
            {
                db.Database.ExecuteSqlCommand("SP_ExpenseCheck @Mode={0},@ExpenseCheckId={1},@UpdatedBy={2}", "DeleteExpenseCheckById", expensevalue.ExpenseCheckId, expensevalue.UpdatedBy);
            }
            catch (Exception ex)
            {
                logger.Error("ExpenseAccountsRepository - DeleteExpenseCheckData - " + DateTime.Now + " - " + ex.Message.ToString());
            }
        }

        public List<UploadedFiles> GetUploadedExpenseFiles(int ExpenseCheckId)
        {
            List<UploadedFiles> obj = new List<UploadedFiles>();
            try
            {
                obj = db.Database.SqlQuery<UploadedFiles>("SP_ExpenseCheck @Mode = {0}, @ExpenseCheckId = {1}", "GetExpenseUploadedFilesList", ExpenseCheckId).ToList();
            }
            catch (Exception ex)
            {
                logger.Error("ExpenseAccountsRepository - GetUploadedExpenseFiles - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return obj;
        }

        public List<DepartmentMaster> DepartmentAccountTypeList(int? StoreId)
        {
            List<DepartmentMaster> obj = new List<DepartmentMaster>();
            try
            {
                obj = db.Database.SqlQuery<DepartmentMaster>("SP_ExpenseCheck @Mode = {0}, @StoreId = {1}", "GetDepartmentAccountTypeList", StoreId).ToList();
            }
            catch (Exception ex)
            {
                logger.Error("ExpenseAccountsRepository - DepartmentAccountTypeList - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return obj;
        }

        public List<DepartmentMaster> CheckDepartmentAccountTypeList(int? StoreId)
        {
            List<DepartmentMaster> obj = new List<DepartmentMaster>();
            try
            {
                obj = db.Database.SqlQuery<DepartmentMaster>("SP_ExpenseCheck @Mode = {0}, @StoreId = {1}", "GetCheckDepartmentAccountTypeList", StoreId).ToList();
            }
            catch (Exception ex)
            {
                logger.Error("ExpenseAccountsRepository - CheckDepartmentAccountTypeList - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return obj;
        }

        public List<DepartmentMaster> DepartmentDetailList(int? StoreId)
        {
            List<DepartmentMaster> obj = new List<DepartmentMaster>();
            try
            {
                obj = db.Database.SqlQuery<DepartmentMaster>("SP_ExpenseCheck @Mode = {0}, @StoreId = {1}", "GetDepartmentDetailList", StoreId).ToList();
            }
            catch (Exception ex)
            {
                logger.Error("ExpenseAccountsRepository - PaymentMethodList - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return obj;
        }

        public string GetPayeeMailingAddressById(int payeeid)
        {
            string mailingaddress = "";
            try
            {
                var vendordata = db.VendorMasters.Find(payeeid);
                if (vendordata != null)
                {
                    mailingaddress = $"{vendordata.Address}, {vendordata.City}, {vendordata.State} {vendordata.PostalCode}";
                    //mailingaddress = vendordata.Address;
                }
            }
            catch (Exception ex)
            {
                logger.Error("ExpenseAccountsRepository - GetPayeeMailingAddressById - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return mailingaddress;
        }

        public void InsertExpenseSyncId(int ExpenseCheckId, QBResponse objRes)
        {
            try
            {
                db.Database.ExecuteSqlCommand("SP_ExpenseCheck @Mode={0},@ExpenseCheckId={1},@TXNId={2},@SyncToken={3}", "InsertSyncExpenseCheck", ExpenseCheckId, objRes.ID, objRes.SyncToken);
            }
            catch (Exception ex)
            {
                logger.Error("ExpenseAccountsRepository - InsertExpenseSyncId - " + DateTime.Now + " - " + ex.Message.ToString());
            }
        }

        public void UpdateExpenseSyncId(int ExpenseCheckId, QBResponse objRes)
        {
            try
            {
                db.Database.ExecuteSqlCommand("SP_ExpenseCheck @Mode={0},@ExpenseCheckId={1},@SyncToken={2}", "UpdateSyncExpenseCheck", ExpenseCheckId, objRes.SyncToken);
            }
            catch (Exception ex)
            {
                logger.Error("ExpenseAccountsRepository - UpdateExpenseSyncId - " + DateTime.Now + " - " + ex.Message.ToString());
            }
        }

        public List<ExpenseCheckSelect> GetPrintCheckList(int StoreId)
        {
            List<ExpenseCheckSelect> obj = new List<ExpenseCheckSelect>();
            try
            {
                obj = db.Database.SqlQuery<ExpenseCheckSelect>("SP_ExpenseCheck @Mode = {0}, @StoreId = {1}", "GetPrintCheckRecord", StoreId).ToList();
            }
            catch (Exception ex)
            {
                logger.Error("ExpenseAccountsRepository - GetPrintCheckList - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return obj;
        }

        public List<CheckPdfPrint> GetPrintCheckPayeeData(string expenseCheckIds)
        {
            List<CheckPdfPrint> obj = new List<CheckPdfPrint>();
            try
            {
                obj = db.Database.SqlQuery<CheckPdfPrint>("SP_ExpenseCheck @Mode = {0}, @ExpenseCheckIds = {1}", "GetPrintCheckPayeeData", expenseCheckIds).ToList();
            }
            catch (Exception ex)
            {
                logger.Error("ExpenseAccountsRepository - GetPrintCheckPayeeData - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return obj;
        }

        public List<CheckPdfPrintDetails> GetPrintCheckDetails(int ExpenseCheckId)
        {
            List<CheckPdfPrintDetails> obj = new List<CheckPdfPrintDetails>();
            try
            {
                obj = db.Database.SqlQuery<CheckPdfPrintDetails>("SP_ExpenseCheck @Mode = {0}, @ExpenseCheckId = {1}", "GetPrintCheckDetailsData", ExpenseCheckId).ToList();
            }
            catch (Exception ex)
            {
                logger.Error("ExpenseAccountsRepository - GetPrintCheckDetails - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return obj;
        }

        public string GetQbErrorMessageById(int expensecheckid)
        {
            string qberrormessage = "";
            try
            {
                var expensecheckdata = db.ExpenseChecks.Find(expensecheckid);
                if (expensecheckdata != null)
                {
                    qberrormessage = expensecheckdata.QBErrorMessage;
                }
            }
            catch (Exception ex)
            {
                logger.Error("ExpenseAccountsRepository - GetQbErrorMessageById - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return qberrormessage;
        }

        public void InsertExpenseErrorSyncId(int ExpenseCheckId, QBResponse objRes)
        {
            try
            {
                db.Database.ExecuteSqlCommand("SP_ExpenseCheck @Mode={0},@ExpenseCheckId={1},@QBErrorMessage={2}", "InsertErrorSyncExpenseCheck", ExpenseCheckId, objRes.Message);
            }
            catch (Exception ex)
            {
                logger.Error("ExpenseAccountsRepository - InsertExpenseErrorSyncId - " + DateTime.Now + " - " + ex.Message.ToString());
            }
        }

        public List<ExpenseCheckSelectMain> BulkCheckPreviewData_Beta(string payment, string Orderby, string AscDsc, string Status, string ForceStatus, int UserTypeId, string strStore, string Startdate = null, string Enddate = null)
        {
            List<ExpenseCheckSelectMain> lstExpenseCheckSelect = new List<ExpenseCheckSelectMain>();
            try
            {
                lstExpenseCheckSelect = db.Database.SqlQuery<ExpenseCheckSelectMain>("[SP_BulkCheckPreviewData_Beta] @searchbox={0},@startdate={1},@enddate={2},@SearchType={3},@StoreID={4},@UserTypeID={5},@AmtMaximum={6},@AmtMinimum={7},@Payment_type={8},@Dept_id={9},@orderby={10},@AscDsc={11},@Type={12},@Status={13}", "", Startdate, Enddate, "", ((strStore == "" || strStore == "0") ? null : strStore.ToString()), UserTypeId, null, null, payment, null, Orderby, AscDsc, Status, ForceStatus).ToList();
            }
            catch (Exception ex)
            {
                logger.Error("ExpenseAccountsRepository - BulkCheckPreviewData_Beta - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return lstExpenseCheckSelect;
        }

        public List<ExpenseCheckSelectMain> BulkExpensePreviewData_Beta(string payment, string Orderby, string AscDsc, string Status, string ForceStatus, int UserTypeId, string strStore, string Startdate = null, string Enddate = null)
        {
            List<ExpenseCheckSelectMain> lstExpenseCheckSelect = new List<ExpenseCheckSelectMain>();
            try
            {
                lstExpenseCheckSelect = db.Database.SqlQuery<ExpenseCheckSelectMain>("[SP_BulkExpensePreviewData_Beta] @searchbox={0},@startdate={1},@enddate={2},@SearchType={3},@StoreID={4},@UserTypeID={5},@AmtMaximum={6},@AmtMinimum={7},@Payment_type={8},@Dept_id={9},@orderby={10},@AscDsc={11},@Type={12},@Status={13}", "", Startdate, Enddate, "", ((strStore == "" || strStore == "0") ? null : strStore.ToString()), UserTypeId, null, null, payment, null, Orderby, AscDsc, Status, ForceStatus).ToList();
            }
            catch (Exception ex)
            {
                logger.Error("ExpenseAccountsRepository - BulkExpensePreviewData_Beta - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return lstExpenseCheckSelect;
        }
    }
}
