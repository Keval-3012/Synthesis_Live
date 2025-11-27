using EntityModels.Models;
using Newtonsoft.Json;
using NLog;
using Repository.IRepository;
using SynthesisQBOnline;
using SynthesisQBOnline.BAL;
using SynthesisViewModal;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility;
using QBOnlineConfiguration = SynthesisQBOnline.BAL.QBOnlineconfiguration;

namespace Repository
{
    public class QBRepository : IQBRepository
    {
        private DBContext _context;
        Logger logger = LogManager.GetCurrentClassLogger();
        public QBRepository(DBContext context)
        {
            _context = context;
        }


        public bool QBSyncDepartment(string Department, string AccountType, string DetailType, string Description, string AccountNumber, string IsSubAccount, int StoreId, ref QBResponse objRes)
        {
            bool IsFirst = false;
        rerun:;
            bool QBSyncData = false;
            try
            {
                QBOnlineconfiguration objOnlieDetail = GetConfigDetail(StoreId);
                AccountMaster objAcc = new AccountMaster();
                objAcc.Department = Department;
                objAcc.AccountType = AccountType;
                objAcc.DetailType = DetailType;
                objAcc.Description = Description;
                objAcc.AccountNumber = AccountNumber;
                objAcc.SubAccount = IsSubAccount;

                SynthesisQBOnline.QBAccount.CreateDepartment(objAcc, ref objRes, objOnlieDetail);
                Common.WriteErrorLogs("QBSyncDepartment: " + objRes.Status);
                Common.WriteErrorLogs("QBSyncDepartment Message: " + objRes.Message);
                if (objRes.Status == "Done")
                {
                    QBSyncData = true;
                }
                else
                {
                    if (objRes.Status == "AuthenticationFailed" && IsFirst == false)
                    {
                        var getnewtokan = SynthesisQBOnline.QBClass.QBRequest.PerformRefreshToken(objOnlieDetail.QBRefreshToken, objOnlieDetail);
                        if (getnewtokan == "")
                        {
                            _context.Database.ExecuteSqlCommand("SP_QBOnlineConfiguration @Mode = {0},@StoreId = {1}", "UpdateQBIsTokenUpdate", StoreId);
                            return QBSyncData;
                        }
                        EntityModels.Models.QBOnlineConfiguration QBOnlinesq = _context.QBOnlineConfigurations.Where(a => a.StoreId == StoreId).FirstOrDefault();
                        QBOnlinesq.QBToken = getnewtokan;
                        _context.Entry(QBOnlinesq).State = EntityState.Modified;
                        _context.SaveChanges();
                        QBOnlinesq = null;

                        IsFirst = true;
                        goto rerun;
                    }
                }
                objAcc = null;
                objOnlieDetail = null;
            }
            catch (Exception ex)
            {
                Common.WriteErrorLogs("QBSyncDepartment Error: " + ex.InnerException.ToString() + "  Message:" + ex.Message.ToString());
            }
            return QBSyncData;
        }

        public QBOnlineconfiguration GetConfigDetail(int StoreID)
        {
            QBOnlineconfiguration objOnline = new QBOnlineconfiguration();
            var Dt = _context.Database.SqlQuery<QBOnlineConfiguration>("SP_QBOnlineConfiguration @Mode,@StoreId", new SqlParameter("@Mode", "SelectByStoreID"), new SqlParameter("@StoreId", StoreID)).ToList();
            if (Dt.Count > 0)
            {
                objOnline.URL = ConfigurationManager.AppSettings["qboBaseUrl"];
                objOnline.RealmID = Dt.LastOrDefault().RealmID.ToString();
                objOnline.ClientId = Dt.LastOrDefault().ClientId.ToString();
                objOnline.ClientSecretKey = Dt.LastOrDefault().ClientSecretKey.ToString();
                objOnline.QBToken = Dt.LastOrDefault().QBToken.ToString();
                objOnline.QBRefreshToken = Dt.LastOrDefault().QBRefreshToken.ToString();
                objOnline.StoreId = StoreID;

            }
            return objOnline;
        }


        public bool QBEditSyncDepartment(string Department, string AccountType, string DetailType, string Description, string AccountNumber, string IsSubAccount, string TxnId, int StoreId, ref SynthesisQBOnline.BAL.QBResponse objRes)
        {
            bool IsFirst = false;
        rerun:;
            bool QBSyncData = false;
            try
            {
                QBOnlineconfiguration objOnlieDetail = GetConfigDetail(StoreId);
                string GetSyscToken = GetDepartmentSyncToken(TxnId, objOnlieDetail, ref objRes);
                if (GetSyscToken != "" || GetSyscToken != null)
                {
                    AccountMaster objAcc = new AccountMaster();
                    objAcc.Department = Department;
                    objAcc.AccountType = AccountType;
                    objAcc.DetailType = DetailType;
                    objAcc.Description = Description;
                    objAcc.AccountNumber = AccountNumber;
                    objAcc.SubAccount = IsSubAccount;
                    objAcc.ID = TxnId;
                    objAcc.SyncToken = GetSyscToken;

                    QBAccount.ModDeapartment(objAcc, ref objRes, objOnlieDetail);
                    Common.WriteErrorLogs("QBEditSyncDepartment: " + objRes.Status);
                    Common.WriteErrorLogs("QBEditSyncDepartment Message: " + objRes.Message);
                    if (objRes.Status == "Done")
                    {
                        QBSyncData = true;
                    }
                    else
                    {
                        if (objRes.Status == "AuthenticationFailed" && IsFirst == false)
                        {
                            var getnewtokan = SynthesisQBOnline.QBClass.QBRequest.PerformRefreshToken(objOnlieDetail.QBRefreshToken, objOnlieDetail);
                            if (getnewtokan == "")
                            {
                                return QBSyncData;
                            }
                            EntityModels.Models.QBOnlineConfiguration QBOnlinesq = _context.QBOnlineConfigurations.Where(a => a.StoreId == StoreId).FirstOrDefault();
                            QBOnlinesq.QBToken = getnewtokan;
                            _context.Entry(QBOnlinesq).State = EntityState.Modified;
                            _context.SaveChanges();
                            QBOnlinesq = null;
                            IsFirst = true;
                            goto rerun;
                        }
                    }
                    objAcc = null;
                    objOnlieDetail = null;
                }
            }
            catch (Exception ex)
            {
                Common.WriteErrorLogs("QBEditSyncDepartment Error: " + ex.InnerException.ToString() + "  Message:" + ex.Message.ToString());
            }
            return QBSyncData;
        }

        public string GetDepartmentSyncToken(string Id, QBOnlineconfiguration objOnline, ref QBResponse objRes)
        {
            bool IsFirst = false;
        rerun:;
            string sToken = "";
            QBAccount.CheckDeapartmentID(Id, objOnline, ref objRes);
            if (objRes.Status != null)
            {
                if ((objRes.Status == "AuthenticationFailed") && IsFirst == false)
                {
                    var getnewtokan = SynthesisQBOnline.QBClass.QBRequest.PerformRefreshToken(objOnline.QBRefreshToken, objOnline);
                    if (getnewtokan == "")
                    {
                        _context.Database.ExecuteSqlCommand("SP_QBOnlineConfiguration @Mode = {0},@StoreId = {1}", "UpdateQBIsTokenUpdate", objOnline.StoreId);
                        return sToken;
                    }
                    EntityModels.Models.QBOnlineConfiguration QBOnlinesq = _context.QBOnlineConfigurations.Where(a => a.StoreId == objOnline.StoreId).FirstOrDefault();
                    QBOnlinesq.QBToken = getnewtokan;
                    _context.Entry(QBOnlinesq).State = EntityState.Modified;
                    _context.SaveChanges();
                    QBOnlinesq = null;
                    IsFirst = true;
                    goto rerun;

                }
                sToken = objRes.SyncToken;
            }
            return sToken;
        }

        public bool QBSyncVendor(EntityModels.Models.VendorMaster objV, int StoreId, ref QBResponse objRes)
        {
            bool IsFirst = false;
        rerun:;
            bool QBSyncData = false;
            try
            {
                QBOnlineconfiguration objOnlieDetail = GetConfigDetail(StoreId);
                SynthesisQBOnline.BAL.VendorMaster objVendor = new SynthesisQBOnline.BAL.VendorMaster();
                objVendor.DisplayName = objV.VendorName == null ? null : objV.VendorName;
                objVendor.CompanyName = objV.CompanyName == null ? null : objV.CompanyName;
                objVendor.PrintOnCheckas = objV.PrintOnCheck == null ? null : objV.PrintOnCheck;
                objVendor.Address1 = objV.Address == null ? null : objV.Address;
                objVendor.State = objV.State == null ? null : objV.State;
                objVendor.City = objV.City == null ? null : objV.City;
                objVendor.Mobile = objV.PhoneNumber == null ? null : objV.PhoneNumber;
                objVendor.Country = objV.Country == null ? null : objV.Country;
                objVendor.ZipCode = objV.PostalCode == null ? null : objV.PostalCode;
                objVendor.Email = objV.EMail == null ? null : objV.EMail;
                objVendor.AcctNum = objV.AccountNumber == null ? null : objV.AccountNumber;

                SynthesisQBOnline.QBClass.QBVendor.CreateVendor(objVendor, ref objRes, objOnlieDetail);
                Common.WriteErrorLogs("Status: " + objRes.Status);
                Common.WriteErrorLogs("Message: " + objRes.Message);
                if (objRes.Status == "Done")
                {
                    QBSyncData = true;
                }
                else
                {
                    if (objRes.Status == "AuthenticationFailed" && IsFirst == false)
                    {
                        var getnewtokan = SynthesisQBOnline.QBClass.QBRequest.PerformRefreshToken(objOnlieDetail.QBRefreshToken, objOnlieDetail);
                        if (getnewtokan == "")
                        {
                            _context.Database.ExecuteSqlCommand("SP_QBOnlineConfiguration @Mode = {0},@StoreId = {1}", "UpdateQBIsTokenUpdate", StoreId);
                            return QBSyncData;
                        }
                        EntityModels.Models.QBOnlineConfiguration QBOnlinesq = _context.QBOnlineConfigurations.Where(a => a.StoreId == StoreId).FirstOrDefault();
                        QBOnlinesq.QBToken = getnewtokan;
                        _context.Entry(QBOnlinesq).State = EntityState.Modified;
                        _context.SaveChanges();
                        QBOnlinesq = null;
                        IsFirst = true;
                        goto rerun;
                    }
                }
                objVendor = null;
                objOnlieDetail = null;
            }
            catch (Exception ex)
            {
                logger.Error("QBRepository - QBSyncVendor - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return QBSyncData;
        }

        public bool QBEditSyncVendor(EntityModels.Models.VendorMaster objV, string TxnId, int StoreId, ref QBResponse objRes)
        {
            bool IsFirst = false;
        rerun:;
            bool QBSyncData = false;
            try
            {
                QBOnlineconfiguration objOnlieDetail = GetConfigDetail(StoreId);
                string GetSyscToken = GetVenSyncToken(TxnId, objOnlieDetail, ref objRes);
                if (GetSyscToken != "" || GetSyscToken != null)
                {
                    SynthesisQBOnline.BAL.VendorMaster objVendor = new SynthesisQBOnline.BAL.VendorMaster();
                    objVendor.DisplayName = objV.VendorName == null ? null : objV.VendorName;
                    objVendor.CompanyName = objV.CompanyName == null ? null : objV.CompanyName;
                    objVendor.PrintOnCheckas = objV.PrintOnCheck == null ? null : objV.PrintOnCheck;
                    objVendor.Address1 = objV.Address == null ? null : objV.Address;
                    //objVendor.Address2 = objV.Address2 == null ? null : objV.Address2;
                    objVendor.State = objV.State == null ? null : objV.State;
                    objVendor.City = objV.City == null ? null : objV.City;
                    objVendor.Mobile = objV.PhoneNumber == null ? null : objV.PhoneNumber;
                    objVendor.Country = objV.Country == null ? null : objV.Country;
                    objVendor.ZipCode = objV.PostalCode == null ? null : objV.PostalCode;
                    objVendor.Email = objV.EMail == null ? null : objV.EMail;
                    objVendor.AcctNum = objV.AccountNumber == null ? null : objV.AccountNumber;
                    objVendor.IsActive = objV.IsActive == null ? null : objV.IsActive.ToString();
                    objVendor.ID = TxnId;
                    objVendor.SyncToken = GetSyscToken;

                    SynthesisQBOnline.QBClass.QBVendor.ModVendor(objVendor, ref objRes, objOnlieDetail);
                    Common.WriteErrorLogs("Status: " + objRes.Status);
                    Common.WriteErrorLogs("Message: " + objRes.Message);
                    if (objRes.Status == "Done")
                    {
                        QBSyncData = true;
                    }
                    else
                    {
                        if (objRes.Status == "AuthenticationFailed" && IsFirst == false)
                        {
                            var getnewtokan = SynthesisQBOnline.QBClass.QBRequest.PerformRefreshToken(objOnlieDetail.QBRefreshToken, objOnlieDetail);
                            if (getnewtokan == "")
                            {
                                _context.Database.ExecuteSqlCommand("SP_QBOnlineConfiguration @Mode = {0},@StoreId = {1}", "UpdateQBIsTokenUpdate", StoreId);
                                return QBSyncData;
                            }
                            EntityModels.Models.QBOnlineConfiguration QBOnlinesq = _context.QBOnlineConfigurations.Where(a => a.StoreId == StoreId).FirstOrDefault();
                            QBOnlinesq.QBToken = getnewtokan;
                            _context.Entry(QBOnlinesq).State = EntityState.Modified;
                            _context.SaveChanges();
                            QBOnlinesq = null;
                            IsFirst = true;
                            goto rerun;
                        }
                    }
                    objVendor = null;
                    objOnlieDetail = null;
                }
            }
            catch (Exception ex)
            {
                logger.Error("QBRepository - QBEditSyncVendor - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return QBSyncData;
        }

        public string GetVenSyncToken(string Id, QBOnlineconfiguration objOnline, ref QBResponse objRes)
        {
            string sToken = "";
            SynthesisQBOnline.QBClass.QBVendor.CheckVendorID(Id, objOnline, ref objRes);
            if (objRes.Status != null)
            {
                sToken = objRes.SyncToken;
            }
            return sToken;
        }

        public bool QBSyncBillData(int dbinvoiceId, string InvoiceNo, DateTime Invoice_Date, string Note, int StoreId, int VendorID, List<BillDetail> objList, ref QBResponse objRes)
        {
            objList = objList.Where(a => a.Amount != 0).ToList();

            bool IsFirst = false;
        rerun:;
            bool QBSyncData = false;
            try
            {
                QBOnlineconfiguration objOnlieDetail = GetConfigDetail(StoreId);
                Bill objBill = new Bill();
                {
                    objBill.VendorID = _context.VendorMasters.Where(a => a.StoreId == StoreId && a.VendorId == VendorID).FirstOrDefault().ListId;
                    objBill.RefNumber = InvoiceNo;
                    objBill.InvoiceDate = Invoice_Date.ToString();
                    objBill.Notes = Note;
                }
                SynthesisQBOnline.QBClass.QBBill.CreateBill(objBill, objList, ref objRes, objOnlieDetail);
                Common.WriteErrorLogs("Status: " + objRes.Status);
                Common.WriteErrorLogs("Message: " + objRes.Message);
                if (objRes.Status == "Done")
                {
                    QBSyncData = true;
                }
                else
                {
                    if (objRes.Status == "AuthenticationFailed" && IsFirst == false)
                    {
                        var getnewtokan = SynthesisQBOnline.QBClass.QBRequest.PerformRefreshToken(objOnlieDetail.QBRefreshToken, objOnlieDetail);
                        if (getnewtokan == "")
                        {
                            _context.Database.ExecuteSqlCommand("SP_QBOnlineConfiguration @Mode = {0},@StoreId = {1}", "UpdateQBIsTokenUpdate", StoreId);
                            return QBSyncData;
                        }
                        EntityModels.Models.QBOnlineConfiguration QBOnlinesq = _context.QBOnlineConfigurations.Where(a => a.StoreId == StoreId).FirstOrDefault();
                        QBOnlinesq.QBToken = getnewtokan;
                        _context.Entry(QBOnlinesq).State = EntityState.Modified;
                        _context.SaveChanges();
                        QBOnlinesq = null;
                        IsFirst = true;
                        goto rerun;
                    }
                    else
                    {
                        Errolog("QbClass", "QBSyncBillData", objRes.Message, StoreId, dbinvoiceId);
                    }
                }
                objBill = null;
                objOnlieDetail = null;
            }
            catch (Exception ex)
            {
                logger.Error("QBRepository - GetVenSyncToken - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return QBSyncData;
        }

        public void Errolog(string ControllerName, string FunctionName, string Errormesg, int StoreId, int dbinvoiceId)
        {
            var Dt = _context.ErrorLogs.Where(a => a.InvoiceId == dbinvoiceId).ToList();
            if (Dt.Count > 0)
            {
                var obj = _context.ErrorLogs.Where(a => a.ErrorLogId == Dt.LastOrDefault().ErrorLogId).FirstOrDefault();
                obj.ControllerName = ControllerName;
                obj.FunctionName = FunctionName;
                obj.Error = Errormesg;
                obj.StoreId = StoreId;
                obj.InvoiceId = dbinvoiceId;
                _context.Entry(obj).State = EntityState.Modified;
                _context.SaveChanges();
            }
            else
            {
                EntityModels.Models.ErrorLog obj = new EntityModels.Models.ErrorLog();
                obj.ControllerName = ControllerName;
                obj.FunctionName = FunctionName;
                obj.Error = Errormesg;
                obj.StoreId = StoreId;
                obj.InvoiceId = dbinvoiceId;
                _context.ErrorLogs.Add(obj);
                _context.SaveChanges();
            }
        }

        public bool QBSyncVendorCreditData(int dbinvoiceId, string InvoiceNo, DateTime Invoice_Date, string Note, int StoreId, int VendorID, List<VendorCredirDetail> objList, ref QBResponse objRes)
        {
            objList = objList.Where(a => a.Amount != 0).ToList();

            bool IsFirst = false;
        rerun:;
            bool QBSyncData = false;
            try
            {
                logger.Info("QBRepository - QBSyncVendorCreditData - " + DateTime.Now + " - " + dbinvoiceId);
                QBOnlineconfiguration objOnlieDetail = GetConfigDetail(StoreId);
                VendorCredit objVendorCredit = new VendorCredit();
                {
                    objVendorCredit.VendorID = _context.VendorMasters.Where(a => a.StoreId == StoreId && a.VendorId == VendorID).FirstOrDefault().ListId;
                    objVendorCredit.RefNumber = InvoiceNo;
                    objVendorCredit.TxnDate = Invoice_Date;
                    objVendorCredit.Notes = Note;
                }
                SynthesisQBOnline.QBClass.QBVendorCredit.CreateVendorCredit(objVendorCredit, objList, ref objRes, objOnlieDetail);
                Common.WriteErrorLogs("Status: " + objRes.Status);
                Common.WriteErrorLogs("Message: " + objRes.Message);
                if (objRes.Status == "Done")
                {
                    QBSyncData = true;
                }
                else
                {
                    if (objRes.Status == "AuthenticationFailed" && IsFirst == false)
                    {
                        var getnewtokan = SynthesisQBOnline.QBClass.QBRequest.PerformRefreshToken(objOnlieDetail.QBRefreshToken, objOnlieDetail);
                        if (getnewtokan == "")
                        {
                            _context.Database.ExecuteSqlCommand("SP_QBOnlineConfiguration @Mode = {0},@StoreId = {1}", "UpdateQBIsTokenUpdate", StoreId);
                            return QBSyncData;
                        }
                        EntityModels.Models.QBOnlineConfiguration QBOnlinesq = _context.QBOnlineConfigurations.Where(a => a.StoreId == StoreId).FirstOrDefault();
                        QBOnlinesq.QBToken = getnewtokan;
                        _context.Entry(QBOnlinesq).State = EntityState.Modified;
                        _context.SaveChanges();
                        QBOnlinesq = null;
                        IsFirst = true;
                        goto rerun;
                    }
                    else
                    {
                        Errolog("QbClass", "QBSyncVendorCreditData", objRes.Message, StoreId, dbinvoiceId);
                    }
                }
                objVendorCredit = null;
                objOnlieDetail = null;
            }
            catch (Exception)
            {
            }
            return QBSyncData;
        }

        public string GetStoreOnlineDesktop(int StoreID)
        {
            string Store = "";
            try
            {
                var dt = _context.Database.SqlQuery<QBOnlineConfiguration1>("SP_QBOnlineConfiguration @Mode,@StoreId", new SqlParameter("@Mode", "GetQBConfigurationByStoreID"), new SqlParameter("@StoreId", StoreID)).ToList();
                if (dt.Count > 0)
                {
                    Store = dt.FirstOrDefault().QBType.ToString();
                }
            }
            catch (Exception ex)
            {
                logger.Error("QBRepository - GetStoreOnlineDesktop - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return Store;
        }

        public int GetStoreOnlineDesktopFlag(int StoreID)
        {
            int Store = 0;
            var dt = _context.Database.SqlQuery<QBOnlineConfiguration1>("SP_QBOnlineConfiguration @Mode = {0},@StoreId = {1}", "GetQBConfigurationByStoreID", StoreID).ToList();
            if (dt.Count > 0)
            {
                string StoreName = dt.FirstOrDefault().QBType.ToString();
                if (StoreName != "")
                {
                    if (StoreName == "Online")
                    {
                        Store = Convert.ToInt32(dt.FirstOrDefault().QBOnlineFlag.ToString());
                    }
                    else if (StoreName == "Desktop")
                    {
                        Store = Convert.ToInt32(dt.FirstOrDefault().QBWebFlag.ToString());
                    }
                }
            }
            return Store;
        }

        public void CreateVendorCredit(int invoiceID, List<VendorCredirDetail> onjVendorCredit, int IsDashboard)
        {
            Invoice Invoice_data = _context.Invoices.Find(Convert.ToInt32(invoiceID));
            try
            {
                logger.Info("QBRepository - CreateVendorCredit - " + DateTime.Now + " - " + invoiceID);
                onjVendorCredit = onjVendorCredit.Where(a => a.Amount != 0).ToList();
                string success = "";
                QBResponse objResponse = new QBResponse();
                if (Invoice_data != null)
                {
                    //_context.Database.ExecuteSqlCommand("UPDATE Invoice Set  IsSync = -1 Where InvoiceId=" + invoiceID);
                    _context.Database.ExecuteSqlCommand("SP_Invoice @Mode = {0},@InvoiceId = {1}", "UpdateTempStatus", invoiceID);

                    if (QBSyncVendorCreditData(invoiceID, Invoice_data.InvoiceNumber, Convert.ToDateTime(Invoice_data.InvoiceDate), Invoice_data.Note, Convert.ToInt32(Invoice_data.StoreId), Convert.ToInt32(Invoice_data.VendorId), onjVendorCredit, ref objResponse) == true)
                    {
                        success = "Valid";
                    }
                    if (success == "Valid")
                    {
                        //_context.Database.ExecuteSqlCommand("UPDATE Invoice Set  IsSync = 1, TXNId ='" + objResponse.ID + "', Source ='WEB', SyncDate ='" + DateTime.Now + "' Where InvoiceId=" + invoiceID);   
                        _context.Database.ExecuteSqlCommand("SP_Invoice @Mode = {0},@InvoiceId = {1},@TxnID = {2}", "UpdateIsSyncStatusWeb", invoiceID, objResponse.ID);
                    }
                    else
                    {
                        //_context.Database.ExecuteSqlCommand("UPDATE Invoice Set IsSync = 0 Where InvoiceId=" + invoiceID);
                        _context.Database.ExecuteSqlCommand("SP_Invoice @Mode = {0},@InvoiceId = {1}", "UpdateIsSyncStatus_reset", invoiceID);
                    }
                }
            }
            catch (Exception ex)
            {
                _context.Database.ExecuteSqlCommand("SP_Invoice @Mode = {0},@InvoiceId = {1}", "UpdateIsSyncStatus_reset", invoiceID);
                logger.Error("QBRepository - CreateVendorCredit - " + DateTime.Now + " - " + ex.Message.ToString());
            }
        }

        public void CreateBill(int invoiceID, List<BillDetail> onjDepartment)
        {
            Invoice Invoice_data = _context.Invoices.Find(Convert.ToInt32(invoiceID));
            try
            {
                onjDepartment = onjDepartment.Where(a => a.Amount != 0).ToList();
                string success = "";
                QBResponse objResponse = new QBResponse();
                if (Invoice_data != null)
                {
                    _context.Database.ExecuteSqlCommand("SP_Invoice @Mode = {0},@InvoiceId = {1}", "UpdateTempStatus", invoiceID);
                    if (QBSyncBillData(invoiceID, Invoice_data.InvoiceNumber, Convert.ToDateTime(Invoice_data.InvoiceDate), Invoice_data.Note, Convert.ToInt32(Invoice_data.StoreId), Convert.ToInt32(Invoice_data.VendorId), onjDepartment, ref objResponse) == true)
                    {
                        success = "Valid";
                    }
                    if (success == "Valid")
                    {
                        _context.Database.ExecuteSqlCommand("SP_Invoice @Mode = {0},@InvoiceId = {1},@TxnID = {2}", "UpdateIsSyncStatusWeb", invoiceID, objResponse.ID);
                    }
                    else
                    {
                        _context.Database.ExecuteSqlCommand("SP_Invoice @Mode = {0},@InvoiceId = {1}", "UpdateIsSyncStatus_reset", invoiceID);
                    }
                }
            }
            catch (Exception ex)
            {
                _context.Database.ExecuteSqlCommand("SP_Invoice @Mode = {0},@InvoiceId = {1}", "UpdateIsSyncStatus_reset", invoiceID);
                logger.Error("QBRepository - CreateBill - " + DateTime.Now + " - " + ex.Message.ToString());
            }

        }

        public bool QBEditVendorCreditData(int dbinvoiceId, string InvoiceID, string InvoiceNo, DateTime Invoice_Date, string Note, int StoreId, int VendorID, List<VendorCredirDetail> objList, ref QBResponse objRes)
        {
            objList = objList.Where(a => a.Amount != 0).ToList();

            bool IsFirst = false;
            bool QBEditSyncData = false;
        rerun:;
            try
            {
                QBOnlineconfiguration objOnlieDetail = GetConfigDetail(StoreId);
                string GetSyscToken = GetVendorSyncToken(InvoiceID, objOnlieDetail, ref objRes);
                if (GetSyscToken != "" || GetSyscToken != null)
                {
                    VendorCredit objBill = new VendorCredit();
                    {
                        objBill.VendorID = _context.VendorMasters.Where(a => a.StoreId == StoreId && a.VendorId == VendorID).FirstOrDefault().ListId;
                        objBill.RefNumber = InvoiceNo;
                        objBill.TxnDate = Invoice_Date;
                        objBill.Notes = Note;
                        objBill.SyncToken = GetSyscToken;
                        objBill.ID = InvoiceID.ToString();
                    }
                    SynthesisQBOnline.QBClass.QBVendorCredit.ModVendorCredit(objBill, objList, ref objRes, objOnlieDetail);
                    Common.WriteErrorLogs("Status: " + objRes.Status);
                    Common.WriteErrorLogs("Message: " + objRes.Message);
                    if (objRes.Status == "Done")
                    {
                        QBEditSyncData = true;
                    }
                    else
                    {
                        if (objRes.Status == "AuthenticationFailed" && IsFirst == false)
                        {
                            var getnewtokan = SynthesisQBOnline.QBClass.QBRequest.PerformRefreshToken(objOnlieDetail.QBRefreshToken, objOnlieDetail);
                            if (getnewtokan == "")
                            {
                                _context.Database.ExecuteSqlCommand("SP_QBOnlineConfiguration @Mode = {0},@StoreId = {1}", "UpdateQBIsTokenUpdate", StoreId);
                                return QBEditSyncData;
                            }
                            EntityModels.Models.QBOnlineConfiguration QBOnlinesq = _context.QBOnlineConfigurations.Where(a => a.StoreId == StoreId).FirstOrDefault();
                            QBOnlinesq.QBToken = getnewtokan;
                            _context.Entry(QBOnlinesq).State = EntityState.Modified;
                            _context.SaveChanges();
                            QBOnlinesq = null;
                            IsFirst = true;
                            goto rerun;
                        }
                        else
                        {
                            Errolog("QbClass", "QBEditVendorCreditData", objRes.Message, StoreId, dbinvoiceId);
                        }
                    }
                    objBill = null;
                }
            }
            catch (Exception ex)
            {
                logger.Error("QBRepository - QBEditVendorCreditData - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return QBEditSyncData;
        }

        public string GetVendorSyncToken(string Id, QBOnlineconfiguration objOnline, ref QBResponse objRes)
        {
            string sToken = "";
            try
            {
                SynthesisQBOnline.QBClass.QBVendorCredit.CheckVendorCreditID(Id, objOnline, ref objRes);
                if (objRes.Status != null)
                {
                    sToken = objRes.SyncToken;
                }
            }
            catch (Exception ex)
            {
                logger.Error("QBRepository - GetVendorSyncToken - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return sToken;
        }

        public bool QBEditBillData(int dbinvoiceId, string InvoiceID, string InvoiceNo, DateTime Invoice_Date, string Note, int StoreId, int VendorID, List<BillDetail> objList, ref QBResponse objRes)
        {
            objList = objList.Where(a => a.Amount != 0).ToList();

            bool IsFirst = false;
            bool QBEditSyncData = false;
        rerun:;
            try
            {
                QBOnlineconfiguration objOnlieDetail = GetConfigDetail(StoreId);
                string GetSyscToken = GetSyncToken(InvoiceID, objOnlieDetail, ref objRes);
                if (GetSyscToken != "" && GetSyscToken != null)
                {
                    Bill objBill = new Bill();
                    {
                        objBill.VendorID = _context.VendorMasters.Where(a => a.StoreId == StoreId && a.VendorId == VendorID).FirstOrDefault().ListId;
                        objBill.RefNumber = InvoiceNo;
                        objBill.InvoiceDate = Invoice_Date.ToString();
                        objBill.Notes = Note;
                        objBill.SyncToken = GetSyscToken;
                        objBill.ID = InvoiceID.ToString();
                    }
                    SynthesisQBOnline.QBClass.QBBill.ModBill(objBill, objList, ref objRes, objOnlieDetail);
                    Common.WriteErrorLogs("Status: " + objRes.Status);
                    Common.WriteErrorLogs("Message: " + objRes.Message);
                    if (objRes.Status == "Done")
                    {
                        QBEditSyncData = true;
                    }
                    else
                    {
                        if (objRes.Status == "AuthenticationFailed" && IsFirst == false)
                        {
                            var getnewtokan = SynthesisQBOnline.QBClass.QBRequest.PerformRefreshToken(objOnlieDetail.QBRefreshToken, objOnlieDetail);
                            if (getnewtokan == "")
                            {
                                _context.Database.ExecuteSqlCommand("SP_QBOnlineConfiguration @Mode = {0},@StoreId = {1}", "UpdateQBIsTokenUpdate", StoreId);
                                return QBEditSyncData;
                            }
                            EntityModels.Models.QBOnlineConfiguration QBOnlinesq = _context.QBOnlineConfigurations.Where(a => a.StoreId == StoreId).FirstOrDefault();
                            QBOnlinesq.QBToken = getnewtokan;
                            _context.Entry(QBOnlinesq).State = EntityState.Modified;
                            _context.SaveChanges();
                            QBOnlinesq = null;
                            IsFirst = true;
                            goto rerun;
                        }
                        else
                        {
                            Errolog("QbClass", "QBEditBillData", objRes.Message, StoreId, dbinvoiceId);
                        }
                    }
                    objBill = null;
                }
            }
            catch (Exception ex)
            {
                logger.Error("QBRepository - QBEditBillData - " + DateTime.Now + " - " + ex.Message.ToString());
            }

            return QBEditSyncData;
        }

        public string GetSyncToken(string Id, QBOnlineconfiguration objOnline, ref QBResponse objRes)
        {
            string sToken = "";
            try
            {
                SynthesisQBOnline.QBClass.QBBill.CheckBillID(Id, objOnline, ref objRes);
                if (objRes.Status != null)
                {
                    sToken = objRes.SyncToken;
                }
            }
            catch (Exception ex)
            {
                logger.Error("QBRepository - GetSyncToken - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return sToken;
        }

        public string QBGetVendorStatus(string TxnId, int StoreId, ref QBResponse objRes)
        {
            string status = "";
            try
            {
                QBOnlineconfiguration objOnlieDetail = GetConfigDetail(StoreId);
                status = GetVenStatus(TxnId, objOnlieDetail, ref objRes);
            }
            catch (Exception ex)
            {
                logger.Error("QBRepository - QBGetVendorStatus - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return status;
        }

        public string GetVenStatus(string Id, QBOnlineconfiguration objOnline, ref QBResponse objRes)
        {
            string status = "";
            SynthesisQBOnline.QBClass.QBVendor.CheckVendorID(Id, objOnline, ref objRes);
            if (objRes.Status != null)
            {
                status = objRes.QBStatus;
            }
            return status;
        }

        public List<QBOnlineConfiguration1> GetQBOnlineConfiguration1()
        {
            List<QBOnlineConfiguration1> qBOnlineConfiguration1 = new List<QBOnlineConfiguration1>();
            try
            {
                qBOnlineConfiguration1 = _context.Database.SqlQuery<QBOnlineConfiguration1>("SP_QBOnlineConfiguration @Mode = {0}", "GetQBConfiguration").ToList();
            }
            catch (Exception ex)
            {
                logger.Error("QBRepository - GetQBOnlineConfiguration1 - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return qBOnlineConfiguration1;
        }
        public List<EntityModels.Models.QBDesktopConfiguration> GetQBDesktopConfiguration(QBConfigurationViewModal qBConfigurationViewModal)
        {
            List<EntityModels.Models.QBDesktopConfiguration> qBDesktopConfiguration = new List<EntityModels.Models.QBDesktopConfiguration>();
            try
            {
                _context.Database.SqlQuery<DepartmentMaster>("SP_QBDesktopConfiguration @Mode = {0}", "GetDepartment").ToList();
                var QBDesktopConfiguration = _context.QBDesktopConfigurations.Where(s => s.StoreId == qBConfigurationViewModal.StoreID).Select(s => new
                {
                    s.QBDesktopId,
                    s.QBCompanyPath,
                    s.IsActive,
                    s.UserName,
                    s.OwnerID,
                    s.FileID,
                    s.AppName,
                    s.Description,
                    s.StoreId,
                    s.BankAccID,
                    s.Password
                }).ToList().Select(s => new EntityModels.Models.QBDesktopConfiguration
                {
                    QBDesktopId = s.QBDesktopId,
                    QBCompanyPath = s.QBCompanyPath,
                    IsActive = s.IsActive,
                    UserName = s.UserName,
                    OwnerID = s.OwnerID,
                    FileID = s.FileID,
                    AppName = s.AppName,
                    Description = s.Description,
                    StoreId = s.StoreId,
                    BankAccID = s.BankAccID,
                    Password = s.Password
                }).FirstOrDefault();
            }
            catch (Exception ex)
            {
                logger.Error("QBRepository - GetQBDesktopConfiguration - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return qBDesktopConfiguration;
        }
        public List<EntityModels.Models.QBDesktopConfiguration> UpdateBankDetails(UpdateBank updateBank)
        {
            List<EntityModels.Models.QBDesktopConfiguration> qBDesktopConfiguration = new List<EntityModels.Models.QBDesktopConfiguration>();
            try
            {
                EntityModels.Models.QBDesktopConfiguration desktop = _context.QBDesktopConfigurations.Where(a => a.StoreId == updateBank.StoreID && a.QBDesktopId == updateBank.QbWebId).FirstOrDefault();
                desktop.BankAccID = updateBank.ListID;
                _context.Entry(desktop).Property("BankAccID").IsModified = true;
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Error("QBRepository - UpdateBankDetails - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return qBDesktopConfiguration;
        }
        public EntityModels.Models.QBDesktopConfiguration GenerateXML(GenerateXML generateXML, int StoreID)
        {
            EntityModels.Models.QBDesktopConfiguration qBDesktopConfiguration = new EntityModels.Models.QBDesktopConfiguration();
            try
            {
                qBDesktopConfiguration = _context.QBDesktopConfigurations.Where(s => s.StoreId == StoreID && s.QBDesktopId == generateXML.ID).Select(s => new
                {
                    s.QBDesktopId,
                    s.QBCompanyPath,
                    s.IsActive,
                    s.UserName,
                    s.OwnerID,
                    s.FileID,
                    s.AppName,
                    s.Description,
                    s.StoreId,
                    s.BankAccID,
                    s.Password
                }).ToList().Select(s => new EntityModels.Models.QBDesktopConfiguration
                {
                    QBDesktopId = s.QBDesktopId,
                    QBCompanyPath = s.QBCompanyPath,
                    IsActive = s.IsActive,
                    UserName = s.UserName,
                    OwnerID = s.OwnerID,
                    FileID = s.FileID,
                    AppName = s.AppName,
                    Description = s.Description,
                    StoreId = s.StoreId,
                    BankAccID = s.BankAccID,
                    Password = s.Password
                }).FirstOrDefault();
            }
            catch (Exception ex)
            {
                logger.Error("QBRepository - GenerateXML - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return qBDesktopConfiguration;
        }

        public List<EntityModels.Models.QBOnlineConfiguration> SelectByStoreID(int StoreID)
        {
            List<EntityModels.Models.QBOnlineConfiguration> QBOnlineConfiguration = new List<EntityModels.Models.QBOnlineConfiguration>();
            try
            {
                QBOnlineConfiguration = _context.Database.SqlQuery<EntityModels.Models.QBOnlineConfiguration>("SP_QBOnlineConfiguration @Mode,@StoreId", new SqlParameter("@Mode", "SelectByStoreID"), new SqlParameter("@StoreId", StoreID)).ToList();
            }
            catch (Exception ex)
            {
                logger.Error("QBRepository - SelectByStoreID - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return QBOnlineConfiguration;
        }

        public void SaveQBOnlineConfigurations(EntityModels.Models.QBOnlineConfiguration QBOnlines)
        {
            try
            {
                _context.QBOnlineConfigurations.Add(QBOnlines);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Error("QBRepository - SaveQBOnlineConfigurations - " + DateTime.Now + " - " + ex.Message.ToString());
            }
        }

        public EntityModels.Models.QBOnlineConfiguration GetQBOnlineId(int QBOnlineId)
        {
            EntityModels.Models.QBOnlineConfiguration obj = new EntityModels.Models.QBOnlineConfiguration();
            try
            {
                obj = _context.QBOnlineConfigurations.Find(QBOnlineId);
            }
            catch (Exception ex)
            {
                logger.Error("QBRepository - GetQBOnlineId - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return obj;
        }
        public void UpdateQBOnlineConfigurations(EntityModels.Models.QBOnlineConfiguration obj2)
        {
            try
            {
                _context.Entry(obj2).State = EntityState.Modified;
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Error("QBRepository - UpdateQBOnlineConfigurations - " + DateTime.Now + " - " + ex.Message.ToString());
            }
        }

        public List<EntityModels.Models.QBDesktopConfiguration> SelectByStoreIDForDesktop(int StoreID)
        {
            List<EntityModels.Models.QBDesktopConfiguration> QBDesktopConfiguration = new List<EntityModels.Models.QBDesktopConfiguration>();
            try
            {
                QBDesktopConfiguration = _context.Database.SqlQuery<EntityModels.Models.QBDesktopConfiguration>("SP_QBDesktopConfiguration @Mode,@StoreId", new SqlParameter("@Mode", "SelectByStoreID"), new SqlParameter("@StoreId", StoreID)).ToList();
            }
            catch (Exception ex)
            {
                logger.Error("QBRepository - SelectByStoreIDForDesktop - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return QBDesktopConfiguration;
        }
        public void GetQBDesktopId(int QBDesktopId)
        {
            try
            {
                EntityModels.Models.QBDesktopConfiguration QBWebConnectors = _context.QBDesktopConfigurations.Find(QBDesktopId);
                QBWebConnectors.IsActive = false;
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Error("QBRepository - GetQBDesktopId - " + DateTime.Now + " - " + ex.Message.ToString());
            }
        }

        public List<QBHistory> SelectByIdQBHistory(int StoreID)
        {
            List<QBHistory> QBHistory = new List<QBHistory>();
            try
            {
                QBHistory = _context.Database.SqlQuery<QBHistory>("SP_QBHistory @Mode,@StoreId", new SqlParameter("@Mode", "SelectById"), new SqlParameter("@StoreId", StoreID)).ToList();
            }
            catch (Exception ex)
            {
                logger.Error("QBRepository - SelectByIdQBHistory - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return QBHistory;
        }
        public void SaveQBHistorys(QBHistory QBHistorys)
        {
            try
            {
                _context.QBHistorys.Add(QBHistorys);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Error("QBRepository - SaveQBHistorys - " + DateTime.Now + " - " + ex.Message.ToString());
            }
        }
        public void UpdateQBHistory(int QBHistoryId)
        {
            try
            {
                QBHistory obj2 = _context.QBHistorys.Find(QBHistoryId);
                obj2.EndDate = DateTime.Now;
                _context.Entry(obj2).State = EntityState.Modified;
                _context.SaveChanges();
                obj2 = null;
            }
            catch (Exception ex)
            {
                logger.Error("QBRepository - UpdateQBHistory - " + DateTime.Now + " - " + ex.Message.ToString());
            }
        }

        public EntityModels.Models.VendorMaster SelectByStoreID_NameVendor(int StoreID, string DisplayName)
        {
            EntityModels.Models.VendorMaster obj = new EntityModels.Models.VendorMaster();
            try
            {
                obj = _context.Database.SqlQuery<EntityModels.Models.VendorMaster>("SP_VendorMaster @Mode = {0},@StoreId = {1},@VendorName = {2}", "SelectByStoreID_Name", StoreID, DisplayName).FirstOrDefault();
            }
            catch (Exception ex)
            {
                logger.Error("QBRepository - SelectByStoreID_NameVendor - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return obj;
        }

        public List<EntityModels.Models.VendorMaster> VendorMasters()
        {
            List<EntityModels.Models.VendorMaster> vendor = new List<EntityModels.Models.VendorMaster>();
            try
            {
                vendor = _context.VendorMasters.ToList();
            }
            catch (Exception ex)
            {
                logger.Error("QBRepository - VendorMasters - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return vendor;
        }
        public void UpdateVendorMaster(EntityModels.Models.VendorMaster objVendor)
        {
            try
            {
                _context.Entry(objVendor).State = EntityState.Modified;
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Error("QBRepository - UpdateVendorMaster - " + DateTime.Now + " - " + ex.Message.ToString());
            }
        }
        public EntityModels.Models.VendorMaster SelectByStoreID_IDVendor(int StoreID, int ID)
        {
            EntityModels.Models.VendorMaster obj = new EntityModels.Models.VendorMaster();
            try
            {
                obj = _context.Database.SqlQuery<EntityModels.Models.VendorMaster>("SP_VendorMaster @Mode = {0} ,@StoreId = {1},@ListId = {2}", "SelectByStoreID_ListID", StoreID, ID).FirstOrDefault();
            }
            catch (Exception ex)
            {
                logger.Error("QBRepository - SelectByStoreID_IDVendor - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return obj;
        }
        public void SaveVendorMasters(EntityModels.Models.VendorMaster objVendor)
        {
            try
            {
                _context.VendorMasters.Add(objVendor);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Error("QBRepository - SaveVendorMasters - " + DateTime.Now + " - " + ex.Message.ToString());
            }
        }
        public List<DepartmentMaster> SelectByStoreID_Department(int StoreID, string Department)
        {
            List<DepartmentMaster> DepartmentMaster = new List<DepartmentMaster>();
            try
            {
                DepartmentMaster = _context.Database.SqlQuery<DepartmentMaster>("SP_DepartmentMaster @Mode = {0},@StoreId = {1},@DepartmentName = {2}", "SelectByStoreID_Name", StoreID, Department).ToList();
            }
            catch (Exception ex)
            {
                logger.Error("QBRepository - SelectByStoreID_Department - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return DepartmentMaster;
        }
        public List<AccountTypeMaster> AccountTypeMastersList()
        {
            List<AccountTypeMaster> accountTypeMaster = new List<AccountTypeMaster>();
            try
            {
                accountTypeMaster = _context.AccountTypeMasters.ToList();
            }
            catch (Exception ex)
            {
                logger.Error("QBRepository - AccountTypeMastersList - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return accountTypeMaster;
        }
        public AccountTypeMaster GetAccountType(string AccountType, string Flag)
        {
            AccountTypeMaster accountTypeMaster = new AccountTypeMaster();
            try
            {
                accountTypeMaster = _context.Database.SqlQuery<AccountTypeMaster>("SP_AccountTypeMaster @Mode = {0} ,@AccountType = {1},@Flag = {2}", "GetAccountType", AccountType, Flag).FirstOrDefault();
            }
            catch (Exception ex)
            {
                logger.Error("QBRepository - GetAccountType - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return accountTypeMaster;
        }
        public void SaveAccountTypeMasters(AccountTypeMaster objAcc)
        {
            try
            {
                _context.AccountTypeMasters.Add(objAcc);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Error("QBRepository - SaveAccountTypeMasters - " + DateTime.Now + " - " + ex.Message.ToString());
            }
        }
        public AccountDetailTypeMaster GetQBDetailType(string DetailType)
        {
            AccountDetailTypeMaster accountDetailTypeMaster = new AccountDetailTypeMaster();
            try
            {
                accountDetailTypeMaster = _context.Database.SqlQuery<AccountDetailTypeMaster>("SP_AccountDetailTypeMaster @Mode = {0} ,@QBDetailType = {1}", "GetQBDetailType", DetailType).FirstOrDefault();
            }
            catch (Exception ex)
            {
                logger.Error("QBRepository - GetQBDetailType - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return accountDetailTypeMaster;
        }
        public void SaveAccountDetailTypeMasters(AccountDetailTypeMaster objDetail)
        {
            try
            {
                _context.AccountDetailTypeMasters.Add(objDetail);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Error("QBRepository - SaveAccountDetailTypeMasters - " + DateTime.Now + " - " + ex.Message.ToString());
            }
        }
        public void UpdateDepartmentMaster(DepartmentMaster objDept)
        {
            try
            {
                _context.Entry(objDept).State = EntityState.Modified;
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Error("QBRepository - UpdateDepartmentMaster - " + DateTime.Now + " - " + ex.Message.ToString());
            }
        }
        public DepartmentMaster SelectByStoreID_ListIDDepartment(int StoreID, string ID)
        {
            DepartmentMaster departmentMaster = new DepartmentMaster();
            try
            {
                departmentMaster = _context.Database.SqlQuery<DepartmentMaster>("SP_DepartmentMaster @Mode = {0} ,@StoreId = {1},@ListId = {2}", "SelectByStoreID_ListID", StoreID, ID).FirstOrDefault();
            }
            catch (Exception ex)
            {
                logger.Error("QBRepository - SelectByStoreID_ListIDDepartment - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return departmentMaster;
        }

        public List<DepartmentMaster> DepartmentMastersList()
        {
            List<DepartmentMaster> departmentMaster = new List<DepartmentMaster>();
            try
            {
                departmentMaster = _context.DepartmentMasters.ToList();
            }
            catch (Exception ex)
            {
                logger.Error("QBRepository - DepartmentMastersList - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return departmentMaster;
        }
        public void SaveDepartmentMasters(DepartmentMaster objDept)
        {
            try
            {
                _context.DepartmentMasters.Add(objDept);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Error("QBRepository - SaveDepartmentMasters - " + DateTime.Now + " - " + ex.Message.ToString());
            }
        }
        public List<StoreMaster> StoreMastersList()
        {
            List<StoreMaster> storeMaster = new List<StoreMaster>();
            try
            {
                storeMaster = _context.StoreMasters.ToList();
            }
            catch (Exception ex)
            {
                logger.Error("QBRepository - StoreMastersList - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return storeMaster;
        }
        public List<Invoice> InvoicesList()
        {
            List<Invoice> invoice = new List<Invoice>();
            try
            {
                invoice = _context.Invoices.ToList();
            }
            catch (Exception ex)
            {
                logger.Error("QBRepository - InvoicesList - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return invoice;
        }
        public List<StoreQBSyncModel> GetQBSyncData()
        {
            List<StoreQBSyncModel> storeQBSyncModel = new List<StoreQBSyncModel>();
            try
            {
                storeQBSyncModel = _context.Database.SqlQuery<StoreQBSyncModel>("SP_QBDesktopConfiguration @Mode = {0}", "GetQBSyncData").ToList();
            }
            catch (Exception ex)
            {
                logger.Error("QBRepository - GetQBSyncData - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return storeQBSyncModel;
        }
        public List<StoreQBTotalCount> GetTotalCount(int StoreId)
        {
            List<StoreQBTotalCount> storeQBTotalCount = new List<StoreQBTotalCount>();
            try
            {
                storeQBTotalCount = _context.Database.SqlQuery<StoreQBTotalCount>("SP_QBDesktopConfiguration @Mode = {0},@StoreId = {1}", "GetTotalCount", StoreId).ToList();
            }
            catch (Exception ex)
            {
                logger.Error("QBRepository - GetTotalCount - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return storeQBTotalCount;
        }
        public List<InvoiceSelect> GetUnsuccessfullInvoice(int StoreID)
        {
            List<InvoiceSelect> invoiceSelect = new List<InvoiceSelect>();
            try
            {
                invoiceSelect = _context.Database.SqlQuery<InvoiceSelect>("SP_QBOnlineConfiguration @Mode = {0},@StoreId = {1}", "GetUnsuccessfullInvoice", StoreID).ToList();
            }
            catch (Exception ex)
            {
                logger.Error("QBRepository - GetUnsuccessfullInvoice - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return invoiceSelect;
        }
        public void SaveQBOnlineConfigurationsById(int StoreID, int Flag, string Type)
        {
            try
            {
                if (Type == "Online")
                {
                    EntityModels.Models.QBOnlineConfiguration QBOnlinesq = _context.QBOnlineConfigurations.Where(a => a.StoreId == StoreID).FirstOrDefault();
                    QBOnlinesq.Flag = Flag;
                    _context.Entry(QBOnlinesq).State = EntityState.Modified;
                    _context.SaveChanges();
                    QBOnlinesq = null;
                }
                else
                {
                    EntityModels.Models.QBDesktopConfiguration QBDesktop = _context.QBDesktopConfigurations.Where(a => a.StoreId == StoreID).FirstOrDefault();
                    QBDesktop.Flag = Flag;
                    _context.Entry(QBDesktop).State = EntityState.Modified;
                    _context.SaveChanges();
                    QBDesktop = null;
                }
            }
            catch (Exception ex)
            {
                logger.Error("QBRepository - SaveQBOnlineConfigurationsById - " + DateTime.Now + " - " + ex.Message.ToString());
            }
        }
        public List<EntityModels.Models.QBDesktopConfiguration> QBDesktopConfigurationsList()
        {
            List<EntityModels.Models.QBDesktopConfiguration> qBDesktopConfiguration = new List<EntityModels.Models.QBDesktopConfiguration>();
            try
            {
                qBDesktopConfiguration = _context.QBDesktopConfigurations.ToList();
            }
            catch (Exception ex)
            {
                logger.Error("QBRepository - QBDesktopConfigurationsList - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return qBDesktopConfiguration;
        }
        public List<EntityModels.Models.QBOnlineConfiguration> QBOnlineConfigurationsList()
        {
            List<EntityModels.Models.QBOnlineConfiguration> qBOnlineConfiguration = new List<EntityModels.Models.QBOnlineConfiguration>();
            try
            {
                qBOnlineConfiguration = _context.QBOnlineConfigurations.ToList();
            }
            catch (Exception ex)
            {
                logger.Error("QBRepository - QBOnlineConfigurationsList - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return qBOnlineConfiguration;
        }
        public QBOnlineConfiguration1 GetQBConfigurationBystore(int Store)
        {
            QBOnlineConfiguration1 qBOnlineConfiguration1 = new QBOnlineConfiguration1();
            try
            {
                qBOnlineConfiguration1 = _context.Database.SqlQuery<QBOnlineConfiguration1>("SP_QBOnlineConfiguration @Mode = {0},@StoreId = {1}", "GetQBConfigurationByStoreID", Store).FirstOrDefault();
            }
            catch (Exception ex)
            {
                logger.Error("QBRepository - GetQBConfigurationBystore - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return qBOnlineConfiguration1;
        }
        public List<DistinctAccountType> GetAccountTypeFlag(string QBFlg)
        {
            List<DistinctAccountType> distinctAccountType = new List<DistinctAccountType>();
            try
            {
                distinctAccountType = _context.Database.SqlQuery<DistinctAccountType>("SP_AccountTypeMaster @Mode = {0},@Flag={1}", "Select", QBFlg).ToList();
            }
            catch (Exception ex)
            {
                logger.Error("QBRepository - GetAccountTypeFlag - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return distinctAccountType;
        }
        public List<AccountDetailTypeMasterSelect> GetAccountDetailTypeFlag(string GetAccountType)
        {
            List<AccountDetailTypeMasterSelect> accountDetailTypeMasterSelect = new List<AccountDetailTypeMasterSelect>();
            try
            {
                accountDetailTypeMasterSelect = _context.Database.SqlQuery<AccountDetailTypeMasterSelect>("SP_AccountDetailTypeMaster @Mode = {0}, @CommonType={1}", "SelectByID", GetAccountType).ToList();
            }
            catch (Exception ex)
            {
                logger.Error("QBRepository - GetAccountDetailTypeFlag - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return accountDetailTypeMasterSelect;
        }
        public List<IsSubAccountsSelect> CommonType_Flag_StoreId(string GetAccountType, string flg, int StoreID)
        {
            List<IsSubAccountsSelect> isSubAccountsSelect = new List<IsSubAccountsSelect>();
            try
            {
                isSubAccountsSelect = _context.Database.SqlQuery<IsSubAccountsSelect>("SP_DepartmentMaster @Mode = {0},@CommonType={1},@Flag={2},@StoreID={3}", "SelectByCommonType_Flag_StoreId", GetAccountType, flg, StoreID).ToList();
            }
            catch (Exception ex)
            {
                logger.Error("QBRepository - CommonType_Flag_StoreId - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return isSubAccountsSelect;
        }
        public void InvoicesUpdateUnsuccess(int[] Inv)
        {
            try
            {
                foreach (var item in Inv)
                {
                    Invoice obj2 = _context.Invoices.Find(Convert.ToInt32(item));
                    obj2.strInvoiceDate = obj2.InvoiceDate.ToString("dd/MM/yyyy");
                    obj2.TXNId = null;
                    _context.Entry(obj2).State = EntityState.Modified;
                    _context.SaveChanges();
                    obj2 = null;
                }
            }
            catch (Exception ex)
            {
                logger.Error("QBRepository - InvoicesUpdateUnsuccess - " + DateTime.Now + " - " + ex.Message.ToString());
            }
        }
        public void InvoicesIgnoreUnsuccess(int[] Inv)
        {
            try
            {
                foreach (var item in Inv)
                {
                    Invoice obj2 = _context.Invoices.Find(Convert.ToInt32(item));
                    obj2.strInvoiceDate = obj2.InvoiceDate.ToString("dd/MM/yyyy");
                    obj2.IsSync = 1;
                    obj2.QBTransfer = true;
                    _context.Entry(obj2).State = EntityState.Modified;
                    _context.SaveChanges();
                    obj2 = null;
                }
            }
            catch (Exception ex)
            {
                logger.Error("QBRepository - InvoicesIgnoreUnsuccess - " + DateTime.Now + " - " + ex.Message.ToString());
            }
        }
        public void UpdateQBFlagDataOnline()
        {
            try
            {
                _context.Database.ExecuteSqlCommand("SP_QBOnlineConfiguration @Mode = {0},@Flag = {1}", "UpdateQBFlagData", 0);
                _context.Database.ExecuteSqlCommand("SP_QBDesktopConfiguration @Mode = {0},@Flag = {1}", "UpdateQBFlagData", 0);
            }
            catch (Exception ex)
            {
                logger.Error("QBRepository - UpdateQBFlagDataOnline - " + DateTime.Now + " - " + ex.Message.ToString());
            }
        }
        public void UpdateQBFlagDataOnline1()
        {
            try
            {
                _context.Database.ExecuteSqlCommand("SP_QBOnlineConfiguration @Mode = {0},@Flag = {1}", "UpdateQBFlagData", 1);
                _context.Database.ExecuteSqlCommand("SP_QBDesktopConfiguration @Mode = {0},@Flag = {1}", "UpdateQBFlagData", 1);
            }
            catch (Exception ex)
            {
                logger.Error("QBRepository - UpdateQBFlagDataOnline1 - " + DateTime.Now + " - " + ex.Message.ToString());
            }
        }

        //Himanshu 01-01-2025
        public bool QBSyncExpense(EntityModels.Models.ExpenseCheck objV, ICollection<ExpenseCheckDetail> objDetail, int StoreId, ref QBResponse objRes)
        {
            logger.Info("QBRepository - QBSyncExpense Start  with this data - " + JsonConvert.SerializeObject(objV) + DateTime.Now);
            bool IsFirst = false;
        rerun:;
            bool QBSyncData = false;
            try
            {
                QBOnlineconfiguration objOnlieDetail = GetConfigDetail(StoreId);
                SynthesisQBOnline.BAL.Expense objExpense = new SynthesisQBOnline.BAL.Expense();
                if (objV.RefType == "V")
                {
                    objExpense.VendorID = _context.VendorMasters.Where(a => a.StoreId == StoreId && a.VendorId == objV.VendorId).FirstOrDefault().ListId;
                }
                else if (objV.RefType == "E")
                {
                    objExpense.VendorID = _context.EmployeeMaster.Where(a => a.StoreId == StoreId && a.EmployeeId == objV.VendorId).FirstOrDefault().ListId.ToString();
                }

                objExpense.AccountID = _context.DepartmentMasters.Where(a => a.DepartmentId == objV.BankAccountId).FirstOrDefault().ListId;
                objExpense.TxnDate = objV.TxnDate;
                objExpense.RefNumber = objV.DocNumber;
                objExpense.Memo = objV.Memo;
                objExpense.Amount = objV.TotalAmt;
                //objV.QBType = _context.ExpensePaymentMethodMaster.Where(a => a.PaymentMethodId == objV.PaymentMethodId).FirstOrDefault().PaymentMethod;
                //if (objV.QBType == "AMEX" || objV.QBType == "Amex")
                //{
                //    objV.QBType = "CreditCard";
                //}
                //if (objV.QBType == "ACH" || objV.QBType == "Check")
                //{
                //    objV.QBType = "Check";
                //    if (objV.PrintLater)
                //    {
                //        objExpense.PrintStatus = "NeedToPrint";
                //    }
                //    else
                //    {
                //        objExpense.PrintStatus = "PrintComplete";
                //    }
                //}
                if (objV.PaymentTypeId == 2)
                {
                    objV.QBType = "Cash";
                }
                else if (objV.PaymentTypeId == 1)
                {
                    objV.QBType = "Check";
                    if (objV.PrintLater)
                    {
                        objExpense.PrintStatus = "NeedToPrint";
                    }
                    else
                    {
                        objExpense.PrintStatus = "PrintComplete";
                    }
                }
                objExpense.PaymentMethod = objV.QBType;
                if (objV.PaymentMethodId != 0)
                {
                    objExpense.PaymentMethodId = _context.ExpensePaymentMethodMaster.Where(a => a.PaymentMethodId == objV.PaymentMethodId).FirstOrDefault().ListId;
                }
                if (objV.RefType == "V")
                {
                    objExpense.EntityType = "Vendor";
                }
                else if (objV.RefType == "E")
                {
                    objExpense.EntityType = "Employee";
                }


                List<SynthesisQBOnline.BAL.ExpenseDetail> expenseDetailsList = new List<SynthesisQBOnline.BAL.ExpenseDetail>();
                foreach (var item in objDetail)
                {
                    SynthesisQBOnline.BAL.ExpenseDetail objExpenseDetails = new SynthesisQBOnline.BAL.ExpenseDetail();
                    {
                        objExpenseDetails.DepartmentID = _context.DepartmentMasters.Where(a => a.DepartmentId == item.DepartmentId).FirstOrDefault().ListId;
                        objExpenseDetails.Amount = item.Amount;
                        objExpenseDetails.Description = item.Description;
                    }
                    expenseDetailsList.Add(objExpenseDetails);
                }


                SynthesisQBOnline.QBClass.QBExpense.Create_Expense(objExpense, expenseDetailsList, ref objRes, objOnlieDetail);
                Common.WriteErrorLogs("Status: " + objRes.Status);
                Common.WriteErrorLogs("Message: " + objRes.Message);
                if (objRes.Status == "Done")
                {
                    QBSyncData = true;
                }
                else
                {
                    if (objRes.Status == "AuthenticationFailed" && IsFirst == false)
                    {
                        var getnewtokan = SynthesisQBOnline.QBClass.QBRequest.PerformRefreshToken(objOnlieDetail.QBRefreshToken, objOnlieDetail);
                        if (getnewtokan == "")
                        {
                            _context.Database.ExecuteSqlCommand("SP_QBOnlineConfiguration @Mode = {0},@StoreId = {1}", "UpdateQBIsTokenUpdate", StoreId);
                            return QBSyncData;
                        }
                        EntityModels.Models.QBOnlineConfiguration QBOnlinesq = _context.QBOnlineConfigurations.Where(a => a.StoreId == StoreId).FirstOrDefault();
                        QBOnlinesq.QBToken = getnewtokan;
                        _context.Entry(QBOnlinesq).State = EntityState.Modified;
                        _context.SaveChanges();
                        QBOnlinesq = null;
                        IsFirst = true;
                        goto rerun;
                    }
                }
                objExpense = null;
                objOnlieDetail = null;
            }
            catch (Exception ex)
            {
                logger.Error("QBRepository - QBSyncExpense - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            logger.Info("QBRepository - QBSyncExpense End - " + DateTime.Now);
            return QBSyncData;
        }

        public bool QBEditSyncExpense(EntityModels.Models.ExpenseCheck objV, ICollection<ExpenseCheckDetail> objDetail, int StoreId, ref QBResponse objRes)
        {
            bool IsFirst = false;
        rerun:;
            bool QBSyncData = false;
            try
            {
                logger.Info("QBRepository - QBEditSyncExpense Start  with this data - " + JsonConvert.SerializeObject(objV) + DateTime.Now);
                QBOnlineconfiguration objOnlieDetail = GetConfigDetail(StoreId);
                SynthesisQBOnline.BAL.Expense objExpense = new SynthesisQBOnline.BAL.Expense();
                if (objV.RefType == "V")
                {
                    objExpense.VendorID = _context.VendorMasters.Where(a => a.StoreId == StoreId && a.VendorId == objV.VendorId).FirstOrDefault().ListId;
                }
                else if (objV.RefType == "E")
                {
                    objExpense.VendorID = _context.EmployeeMaster.Where(a => a.StoreId == StoreId && a.EmployeeId == objV.VendorId).FirstOrDefault().ListId.ToString();
                }
                objExpense.AccountID = _context.DepartmentMasters.Where(a => a.DepartmentId == objV.BankAccountId).FirstOrDefault().ListId;
                objExpense.TxnDate = objV.TxnDate;
                objExpense.RefNumber = objV.DocNumber;
                objExpense.Memo = objV.Memo;
                objExpense.Amount = objV.TotalAmt;
                objV.QBType = _context.ExpensePaymentMethodMaster.Where(a => a.PaymentMethodId == objV.PaymentMethodId).FirstOrDefault().PaymentMethod;
                //if (objV.QBType == "AMEX" || objV.QBType == "Amex")
                //{
                //    objV.QBType = "CreditCard";
                //}
                //if (objV.QBType == "ACH" || objV.QBType == "Check")
                //{
                //    objV.QBType = "Check";
                //    if (objV.PrintLater)
                //    {
                //        objExpense.PrintStatus = "NeedToPrint";
                //    }
                //    else
                //    {
                //        objExpense.PrintStatus = "PrintComplete";
                //    }
                //}
                if (objV.PaymentTypeId == 2)
                {
                    objV.QBType = "Cash";
                }
                else if (objV.PaymentTypeId == 1)
                {
                    objV.QBType = "Check";
                    if (objV.PrintLater)
                    {
                        objExpense.PrintStatus = "NeedToPrint";
                    }
                    else
                    {
                        objExpense.PrintStatus = "PrintComplete";
                    }
                }
                objExpense.PaymentMethod = objV.QBType;
                objExpense.PaymentMethodId = _context.ExpensePaymentMethodMaster.Where(a => a.PaymentMethodId == objV.PaymentMethodId).FirstOrDefault().ListId;
                if (objV.RefType == "V")
                {
                    objExpense.EntityType = "Vendor";
                }
                else if (objV.RefType == "E")
                {
                    objExpense.EntityType = "Employee";
                }
                objExpense.ID = objV.TXNId;
                objExpense.SyncToken = objV.SyncToken;

                List<SynthesisQBOnline.BAL.ExpenseDetail> expenseDetailsList = new List<SynthesisQBOnline.BAL.ExpenseDetail>();
                foreach (var item in objDetail)
                {
                    SynthesisQBOnline.BAL.ExpenseDetail objExpenseDetails = new SynthesisQBOnline.BAL.ExpenseDetail();
                    {
                        objExpenseDetails.DepartmentID = _context.DepartmentMasters.Where(a => a.DepartmentId == item.DepartmentId).FirstOrDefault().ListId;
                        objExpenseDetails.Amount = item.Amount;
                        objExpenseDetails.Description = item.Description;
                    }
                    expenseDetailsList.Add(objExpenseDetails);
                }


                SynthesisQBOnline.QBClass.QBExpense.Update_Expense(objExpense, expenseDetailsList, ref objRes, objOnlieDetail);
                Common.WriteErrorLogs("Status: " + objRes.Status);
                Common.WriteErrorLogs("Message: " + objRes.Message);
                if (objRes.Status == "Done")
                {
                    QBSyncData = true;
                }
                else
                {
                    if (objRes.Status == "AuthenticationFailed" && IsFirst == false)
                    {
                        var getnewtokan = SynthesisQBOnline.QBClass.QBRequest.PerformRefreshToken(objOnlieDetail.QBRefreshToken, objOnlieDetail);
                        if (getnewtokan == "")
                        {
                            _context.Database.ExecuteSqlCommand("SP_QBOnlineConfiguration @Mode = {0},@StoreId = {1}", "UpdateQBIsTokenUpdate", StoreId);
                            return QBSyncData;
                        }
                        EntityModels.Models.QBOnlineConfiguration QBOnlinesq = _context.QBOnlineConfigurations.Where(a => a.StoreId == StoreId).FirstOrDefault();
                        QBOnlinesq.QBToken = getnewtokan;
                        _context.Entry(QBOnlinesq).State = EntityState.Modified;
                        _context.SaveChanges();
                        QBOnlinesq = null;
                        IsFirst = true;
                        goto rerun;
                    }
                }
                objExpense = null;
                objOnlieDetail = null;
            }
            catch (Exception ex)
            {
                logger.Error("QBRepository - QBSyncExpense - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return QBSyncData;
        }
    }
}
