using EntityModels.Models;
using SynthesisQBOnline;
using SynthesisQBOnline.BAL;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.ModelBinding;
using Utility;

namespace SysnthesisRepo.App_Start
{
    public static class QBClass
    {

        static DBContext db = new DBContext();

        public static string RemoveSpecialCharacters(string str)
        {
            return Regex.Replace(str, "[^a-zA-Z0-9_.]+", "", RegexOptions.Compiled);
        }

        public static bool QBSyncDepartment(string Department, string AccountType, string DetailType, string Description, string AccountNumber, string IsSubAccount, int StoreId, ref SynthesisQBOnline.BAL.QBResponse objRes)
        {
            bool IsFirst = false;
            rerun:;
            bool QBSyncData = false;
            try
            {
                SynthesisQBOnline.BAL.QBOnlineconfiguration objOnlieDetail = GetConfigDetail(StoreId);
                SynthesisQBOnline.BAL.AccountMaster objAcc = new SynthesisQBOnline.BAL.AccountMaster();
                objAcc.Department = Department;
                objAcc.AccountType = AccountType;
                objAcc.DetailType = DetailType;
                objAcc.Description = Description;
                objAcc.AccountNumber = AccountNumber;
                objAcc.SubAccount = IsSubAccount;

                SynthesisQBOnline.QBAccount.CreateDepartment(objAcc, ref objRes, objOnlieDetail);
                AdminSiteConfiguration.WriteErrorLogs("QBSyncDepartment: " + objRes.Status);
                AdminSiteConfiguration.WriteErrorLogs("QBSyncDepartment Message: " + objRes.Message);
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
                            db.Database.ExecuteSqlCommand("SP_QBOnlineConfiguration @Mode = {0},@StoreId = {1}", "UpdateQBIsTokenUpdate", StoreId);
                            return QBSyncData;
                        }
                        QBOnlineConfiguration QBOnlinesq = db.QBOnlineConfigurations.Where(a => a.StoreId == StoreId).FirstOrDefault();
                        QBOnlinesq.QBToken = getnewtokan;
                        db.Entry(QBOnlinesq).State = EntityState.Modified;
                        db.SaveChanges();
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
                AdminSiteConfiguration.WriteErrorLogs("QBSyncDepartment Error: " + ex.InnerException.ToString() + "  Message:" + ex.Message.ToString());
            }
            return QBSyncData;
        }

        public static SynthesisQBOnline.BAL.QBOnlineconfiguration GetConfigDetail(int StoreID)
        {
            SynthesisQBOnline.BAL.QBOnlineconfiguration objOnline = new SynthesisQBOnline.BAL.QBOnlineconfiguration();
            var Dt = db.Database.SqlQuery<QBOnlineConfiguration>("SP_QBOnlineConfiguration @Mode,@StoreId", new SqlParameter("@Mode", "SelectByStoreID"), new SqlParameter("@StoreId", StoreID)).ToList();
            if (Dt.Count > 0)
            {
                objOnline.URL = ConfigurationManager.AppSettings["qboBaseUrl"];
                objOnline.RealmID = Dt.LastOrDefault().RealmId.ToString();
                objOnline.ClientId = Dt.LastOrDefault().ClientId.ToString();
                objOnline.ClientSecretKey = Dt.LastOrDefault().ClientSecretKey.ToString();
                objOnline.QBToken = Dt.LastOrDefault().QBToken.ToString();
                objOnline.QBRefreshToken = Dt.LastOrDefault().QBRefreshToken.ToString();
                objOnline.StoreId = StoreID;

            }
            return objOnline;
        }

        //public static void output(string logMsg)
        //{
        //    //Console.WriteLine(logMsg);
        //    string logPath = "~/Log/";
        //    System.IO.StreamWriter sw = System.IO.File.AppendText(HttpContext.Current.Server.MapPath(logPath) + "OAuth2SampleAppLogs.txt");
        //    try
        //    {
        //        string logLine = System.String.Format(
        //            "{0:G}: {1}.", System.DateTime.Now, logMsg);
        //        sw.WriteLine(logLine);
        //    }
        //    finally
        //    {
        //        sw.Close();
        //    }
        //}

        public static bool QBEditSyncDepartment(string Department, string AccountType, string DetailType, string Description, string AccountNumber, string IsSubAccount, string TxnId, int StoreId, ref SynthesisQBOnline.BAL.QBResponse objRes)
        {
            bool IsFirst = false;
            rerun:;
            bool QBSyncData = false;
            try
            {
                SynthesisQBOnline.BAL.QBOnlineconfiguration objOnlieDetail = GetConfigDetail(StoreId);
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
                    AdminSiteConfiguration.WriteErrorLogs("QBEditSyncDepartment: " + objRes.Status);
                    AdminSiteConfiguration.WriteErrorLogs("QBEditSyncDepartment Message: " + objRes.Message);
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
                            QBOnlineConfiguration QBOnlinesq = db.QBOnlineConfigurations.Where(a => a.StoreId == StoreId).FirstOrDefault();
                            QBOnlinesq.QBToken = getnewtokan;
                            db.Entry(QBOnlinesq).State = EntityState.Modified;
                            db.SaveChanges();
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
                AdminSiteConfiguration.WriteErrorLogs("QBEditSyncDepartment Error: " + ex.InnerException.ToString() + "  Message:" + ex.Message.ToString());
            }
            return QBSyncData;
        }

        public static string GetDepartmentSyncToken(string Id, QBOnlineconfiguration objOnline, ref QBResponse objRes)
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
                        db.Database.ExecuteSqlCommand("SP_QBOnlineConfiguration @Mode = {0},@StoreId = {1}", "UpdateQBIsTokenUpdate", objOnline.StoreId);
                        return sToken;
                    }
                    QBOnlineConfiguration QBOnlinesq = db.QBOnlineConfigurations.Where(a => a.StoreId == objOnline.StoreId).FirstOrDefault();
                    QBOnlinesq.QBToken = getnewtokan;
                    db.Entry(QBOnlinesq).State = EntityState.Modified;
                    db.SaveChanges();
                    QBOnlinesq = null;
                    IsFirst = true;
                    goto rerun;

                }
                sToken = objRes.SyncToken;
            }
            return sToken;
        }

        public static bool QBSyncVendor(EntityModels.Models.VendorMaster objV, int StoreId, ref QBResponse objRes)
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
                //objVendor.Address2 = objV.Address2 == null ? null : objV.Address2;
                objVendor.State = objV.State == null ? null : objV.State;
                objVendor.City = objV.City == null ? null : objV.City;
                objVendor.Mobile = objV.PhoneNumber == null ? null : objV.PhoneNumber;
                objVendor.Country = objV.Country == null ? null : objV.Country;
                objVendor.ZipCode = objV.PostalCode == null ? null : objV.PostalCode;
                objVendor.Email = objV.EMail == null ? null : objV.EMail;
                objVendor.AcctNum = objV.AccountNumber == null ? null : objV.AccountNumber;

                SynthesisQBOnline.QBClass.QBVendor.CreateVendor(objVendor, ref objRes, objOnlieDetail);
                AdminSiteConfiguration.WriteErrorLogs("Status: " + objRes.Status);
                AdminSiteConfiguration.WriteErrorLogs("Message: " + objRes.Message);
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
                            db.Database.ExecuteSqlCommand("SP_QBOnlineConfiguration @Mode = {0},@StoreId = {1}", "UpdateQBIsTokenUpdate", StoreId);
                            return QBSyncData;
                        }
                        QBOnlineConfiguration QBOnlinesq = db.QBOnlineConfigurations.Where(a => a.StoreId == StoreId).FirstOrDefault();
                        QBOnlinesq.QBToken = getnewtokan;
                        db.Entry(QBOnlinesq).State = EntityState.Modified;
                        db.SaveChanges();
                        QBOnlinesq = null;
                        IsFirst = true;
                        goto rerun;
                    }
                }
                objVendor = null;
                objOnlieDetail = null;
            }
            catch (Exception)
            {
            }
            return QBSyncData;
        }

        public static bool QBEditSyncVendor(EntityModels.Models.VendorMaster objV, string TxnId, int StoreId, ref QBResponse objRes)
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
                    AdminSiteConfiguration.WriteErrorLogs("Status: " + objRes.Status);
                    AdminSiteConfiguration.WriteErrorLogs("Message: " + objRes.Message);
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
                                db.Database.ExecuteSqlCommand("SP_QBOnlineConfiguration @Mode = {0},@StoreId = {1}", "UpdateQBIsTokenUpdate", StoreId);
                                return QBSyncData;
                            }
                            QBOnlineConfiguration QBOnlinesq = db.QBOnlineConfigurations.Where(a => a.StoreId == StoreId).FirstOrDefault();
                            QBOnlinesq.QBToken = getnewtokan;
                            db.Entry(QBOnlinesq).State = EntityState.Modified;
                            db.SaveChanges();
                            QBOnlinesq = null;
                            IsFirst = true;
                            goto rerun;
                        }
                    }
                    objVendor = null;
                    objOnlieDetail = null;
                }
            }
            catch (Exception)
            {
            }
            return QBSyncData;
        }

        public static string GetVenSyncToken(string Id, QBOnlineconfiguration objOnline, ref QBResponse objRes)
        {
            string sToken = "";
            SynthesisQBOnline.QBClass.QBVendor.CheckVendorID(Id, objOnline, ref objRes);
            if (objRes.Status != null)
            {
                sToken = objRes.SyncToken;
            }
            return sToken;
        }

        public static bool QBSyncBillData(int dbinvoiceId, string InvoiceNo, DateTime Invoice_Date, string Note, int StoreId, int VendorID, List<BillDetail> objList, ref QBResponse objRes)
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
                    objBill.VendorID = db.VendorMasters.Where(a => a.StoreId == StoreId && a.VendorId == VendorID).FirstOrDefault().ListId;
                    objBill.RefNumber = InvoiceNo;
                    objBill.InvoiceDate = Invoice_Date.ToString();
                    objBill.Notes = Note;
                }
                SynthesisQBOnline.QBClass.QBBill.CreateBill(objBill, objList, ref objRes, objOnlieDetail);
                AdminSiteConfiguration.WriteErrorLogs("Status: " + objRes.Status);
                AdminSiteConfiguration.WriteErrorLogs("Message: " + objRes.Message);
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
                            db.Database.ExecuteSqlCommand("SP_QBOnlineConfiguration @Mode = {0},@StoreId = {1}", "UpdateQBIsTokenUpdate", StoreId);
                            return QBSyncData;
                        }
                        QBOnlineConfiguration QBOnlinesq = db.QBOnlineConfigurations.Where(a => a.StoreId == StoreId).FirstOrDefault();
                        QBOnlinesq.QBToken = getnewtokan;
                        db.Entry(QBOnlinesq).State = EntityState.Modified;
                        db.SaveChanges();
                        QBOnlinesq = null;
                        IsFirst = true;
                        goto rerun;
                    }
                    else
                    {
                        QBClass.Errolog("QbClass", "QBSyncBillData", objRes.Message, StoreId, dbinvoiceId);
                    }
                }
                objBill = null;
                objOnlieDetail = null;
            }
            catch (Exception ex)
            {
            }
            return QBSyncData;
        }

        public static void Errolog(string ControllerName, string FunctionName, string Errormesg, int StoreId, int dbinvoiceId)
        {
            var Dt = db.ErrorLogs.Where(a => a.InvoiceId == dbinvoiceId).ToList();
            if (Dt.Count > 0)
            {
                var obj = db.ErrorLogs.Where(a => a.ErrorLogId == Dt.LastOrDefault().ErrorLogId).FirstOrDefault();
                obj.ControllerName = ControllerName;
                obj.FunctionName = FunctionName;
                obj.Error = Errormesg;
                obj.StoreId = StoreId;
                obj.InvoiceId = dbinvoiceId;
                db.Entry(obj).State = EntityState.Modified;
                db.SaveChanges();
            }
            else
            {
                EntityModels.Models.ErrorLog obj = new EntityModels.Models.ErrorLog();
                obj.ControllerName = ControllerName;
                obj.FunctionName = FunctionName;
                obj.Error = Errormesg;
                obj.StoreId = StoreId;
                obj.InvoiceId = dbinvoiceId;
                db.ErrorLogs.Add(obj);
                db.SaveChanges();
            }
        }

        public static bool QBSyncVendorCreditData(int dbinvoiceId, string InvoiceNo, DateTime Invoice_Date, string Note, int StoreId, int VendorID, List<VendorCredirDetail> objList, ref QBResponse objRes)
        {
            objList = objList.Where(a => a.Amount != 0).ToList();

            bool IsFirst = false;
            rerun:;
            bool QBSyncData = false;
            try
            {
                QBOnlineconfiguration objOnlieDetail = GetConfigDetail(StoreId);
                VendorCredit objVendorCredit = new VendorCredit();
                {
                    objVendorCredit.VendorID = db.VendorMasters.Where(a => a.StoreId == StoreId && a.VendorId == VendorID).FirstOrDefault().ListId;
                    objVendorCredit.RefNumber = InvoiceNo;
                    objVendorCredit.TxnDate = Invoice_Date;
                    objVendorCredit.Notes = Note;
                }
                SynthesisQBOnline.QBClass.QBVendorCredit.CreateVendorCredit(objVendorCredit, objList, ref objRes, objOnlieDetail);
                AdminSiteConfiguration.WriteErrorLogs("Status: " + objRes.Status);
                AdminSiteConfiguration.WriteErrorLogs("Message: " + objRes.Message);
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
                            db.Database.ExecuteSqlCommand("SP_QBOnlineConfiguration @Mode = {0},@StoreId = {1}", "UpdateQBIsTokenUpdate", StoreId);
                            return QBSyncData;
                        }
                        QBOnlineConfiguration QBOnlinesq = db.QBOnlineConfigurations.Where(a => a.StoreId == StoreId).FirstOrDefault();
                        QBOnlinesq.QBToken = getnewtokan;
                        db.Entry(QBOnlinesq).State = EntityState.Modified;
                        db.SaveChanges();
                        QBOnlinesq = null;
                        IsFirst = true;
                        goto rerun;
                    }
                    else
                    {
                        QBClass.Errolog("QbClass", "QBSyncVendorCreditData", objRes.Message, StoreId, dbinvoiceId);
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

        public static string GetStoreOnlineDesktop(int StoreID)
        {
            string Store = "";
            var dt = db.Database.SqlQuery<QBOnlineConfiguration1>("SP_QBOnlineConfiguration @Mode,@StoreId", new SqlParameter("@Mode", "GetQBConfigurationByStoreID"), new SqlParameter("@StoreId", StoreID)).ToList();
            if (dt.Count > 0)
            {
                Store = dt.FirstOrDefault().QBType.ToString();
            }
            return Store;
        }

        public static int GetStoreOnlineDesktopFlag(int StoreID)
        {
            int Store = 0;
            var dt = db.Database.SqlQuery<QBOnlineConfiguration1>("SP_QBOnlineConfiguration @Mode = {0},@StoreId = {1}", "GetQBConfigurationByStoreID", StoreID).ToList();
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

        public static void CreateVendorCredit(int invoiceID, List<VendorCredirDetail> onjVendorCredit, int IsDashboard)
        {
            Invoice Invoice_data = db.Invoices.Find(Convert.ToInt32(invoiceID));
            try
            {
                onjVendorCredit = onjVendorCredit.Where(a => a.Amount != 0).ToList();
                string success = "";
                QBResponse objResponse = new QBResponse();
                if (Invoice_data != null)
                {
                    //db.Database.ExecuteSqlCommand("UPDATE Invoice Set  IsSync = -1 Where InvoiceId=" + invoiceID);
                    db.Database.ExecuteSqlCommand("SP_Invoice @Mode = {0},@InvoiceId = {1}", "UpdateTempStatus", invoiceID);

                    if (QBClass.QBSyncVendorCreditData(invoiceID, Invoice_data.InvoiceNumber, Convert.ToDateTime(Invoice_data.InvoiceDate), Invoice_data.Note, Convert.ToInt32(Invoice_data.StoreId), Convert.ToInt32(Invoice_data.VendorId), onjVendorCredit, ref objResponse) == true)
                    {
                        success = "Valid";
                    }
                    if (success == "Valid")
                    {
                        //db.Database.ExecuteSqlCommand("UPDATE Invoice Set  IsSync = 1, TXNId ='" + objResponse.ID + "', Source ='WEB', SyncDate ='" + DateTime.Now + "' Where InvoiceId=" + invoiceID);   
                        db.Database.ExecuteSqlCommand("SP_Invoice @Mode = {0},@InvoiceId = {1},@TxnID = {2}", "UpdateIsSyncStatusWeb", invoiceID, objResponse.ID);
                    }
                    else
                    {
                        //db.Database.ExecuteSqlCommand("UPDATE Invoice Set IsSync = 0 Where InvoiceId=" + invoiceID);
                        db.Database.ExecuteSqlCommand("SP_Invoice @Mode = {0},@InvoiceId = {1}", "UpdateIsSyncStatus_reset", invoiceID);
                    }
                }
            }
            catch (Exception ex)
            {
                //db.Database.ExecuteSqlCommand("UPDATE Invoice Set IsSync = 0 Where InvoiceId=" + invoiceID);
                db.Database.ExecuteSqlCommand("SP_Invoice @Mode = {0},@InvoiceId = {1}", "UpdateIsSyncStatus_reset", invoiceID);
            }
        }

        public static void CreateBill(int invoiceID, List<BillDetail> onjDepartment)
        {
            Invoice Invoice_data = db.Invoices.Find(Convert.ToInt32(invoiceID));
            try
            {
                onjDepartment = onjDepartment.Where(a => a.Amount != 0).ToList();
                string success = "";
                QBResponse objResponse = new QBResponse();
                if (Invoice_data != null)
                {
                    db.Database.ExecuteSqlCommand("SP_Invoice @Mode = {0},@InvoiceId = {1}", "UpdateTempStatus", invoiceID);
                    //db.Database.ExecuteSqlCommand("UPDATE Invoice Set  IsSync = -1 Where InvoiceId=" + invoiceID);
                    if (QBClass.QBSyncBillData(invoiceID, Invoice_data.InvoiceNumber, Convert.ToDateTime(Invoice_data.InvoiceDate), Invoice_data.Note, Convert.ToInt32(Invoice_data.StoreId), Convert.ToInt32(Invoice_data.VendorId), onjDepartment, ref objResponse) == true)
                    {
                        success = "Valid";
                    }
                    if (success == "Valid")
                    {
                        //db.Database.ExecuteSqlCommand("UPDATE Invoice Set  IsSync = 1, TXNId ='" + objResponse.ID + "', Source ='WEB', SyncDate ='" + DateTime.Now + "' Where InvoiceId=" + invoiceID);
                        db.Database.ExecuteSqlCommand("SP_Invoice @Mode = {0},@InvoiceId = {1},@TxnID = {2}", "UpdateIsSyncStatusWeb", invoiceID, objResponse.ID);
                    }
                    else
                    {
                        db.Database.ExecuteSqlCommand("SP_Invoice @Mode = {0},@InvoiceId = {1}", "UpdateIsSyncStatus_reset", invoiceID);
                        //db.Database.ExecuteSqlCommand("UPDATE Invoice Set  IsSync = 0 Where InvoiceId=" + invoiceID);
                    }
                }
            }
            catch (Exception ex)
            {
                //db.Database.ExecuteSqlCommand("UPDATE Invoice Set  IsSync = 0 Where InvoiceId=" + invoiceID);
                db.Database.ExecuteSqlCommand("SP_Invoice @Mode = {0},@InvoiceId = {1}", "UpdateIsSyncStatus_reset", invoiceID);
            }

        }

        public static bool QBEditVendorCreditData(int dbinvoiceId, string InvoiceID, string InvoiceNo, DateTime Invoice_Date, string Note, int StoreId, int VendorID, List<VendorCredirDetail> objList, ref QBResponse objRes)
        {
            objList = objList.Where(a => a.Amount != 0).ToList();

            bool IsFirst = false;
            bool QBEditSyncData = false;
            rerun:;
            QBOnlineconfiguration objOnlieDetail = GetConfigDetail(StoreId);
            string GetSyscToken = GetVendorSyncToken(InvoiceID, objOnlieDetail, ref objRes);
            if (GetSyscToken != "" || GetSyscToken != null)
            {
                VendorCredit objBill = new VendorCredit();
                {
                    objBill.VendorID = db.VendorMasters.Where(a => a.StoreId == StoreId && a.VendorId == VendorID).FirstOrDefault().ListId;
                    objBill.RefNumber = InvoiceNo;
                    objBill.TxnDate = Invoice_Date;
                    objBill.Notes = Note;
                    objBill.SyncToken = GetSyscToken;
                    objBill.ID = InvoiceID.ToString();
                }
                SynthesisQBOnline.QBClass.QBVendorCredit.ModVendorCredit(objBill, objList, ref objRes, objOnlieDetail);
                AdminSiteConfiguration.WriteErrorLogs("Status: " + objRes.Status);
                AdminSiteConfiguration.WriteErrorLogs("Message: " + objRes.Message);
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
                            db.Database.ExecuteSqlCommand("SP_QBOnlineConfiguration @Mode = {0},@StoreId = {1}", "UpdateQBIsTokenUpdate", StoreId);
                            return QBEditSyncData;
                        }
                        QBOnlineConfiguration QBOnlinesq = db.QBOnlineConfigurations.Where(a => a.StoreId == StoreId).FirstOrDefault();
                        QBOnlinesq.QBToken = getnewtokan;
                        db.Entry(QBOnlinesq).State = EntityState.Modified;
                        db.SaveChanges();
                        QBOnlinesq = null;
                        IsFirst = true;
                        goto rerun;
                    }
                    else
                    {
                        QBClass.Errolog("QbClass", "QBEditVendorCreditData", objRes.Message, StoreId, dbinvoiceId);
                    }
                }
                objBill = null;
            }
            return QBEditSyncData;
        }

        public static string GetVendorSyncToken(string Id, QBOnlineconfiguration objOnline, ref QBResponse objRes)
        {
            string sToken = "";
            SynthesisQBOnline.QBClass.QBVendorCredit.CheckVendorCreditID(Id, objOnline, ref objRes);
            if (objRes.Status != null)
            {
                sToken = objRes.SyncToken;
            }
            return sToken;
        }

        public static bool QBEditBillData(int dbinvoiceId, string InvoiceID, string InvoiceNo, DateTime Invoice_Date, string Note, int StoreId, int VendorID, List<BillDetail> objList, ref QBResponse objRes)
        {
            objList = objList.Where(a => a.Amount != 0).ToList();

            bool IsFirst = false;
            bool QBEditSyncData = false;
            rerun:;
            QBOnlineconfiguration objOnlieDetail = GetConfigDetail(StoreId);
            string GetSyscToken = GetSyncToken(InvoiceID, objOnlieDetail, ref objRes);
            if (GetSyscToken != "" && GetSyscToken != null)
            {
                Bill objBill = new Bill();
                {
                    objBill.VendorID = db.VendorMasters.Where(a => a.StoreId == StoreId && a.VendorId == VendorID).FirstOrDefault().ListId;
                    objBill.RefNumber = InvoiceNo;
                    objBill.InvoiceDate = Invoice_Date.ToString();
                    objBill.Notes = Note;
                    objBill.SyncToken = GetSyscToken;
                    objBill.ID = InvoiceID.ToString();
                }
                SynthesisQBOnline.QBClass.QBBill.ModBill(objBill, objList, ref objRes, objOnlieDetail);
                AdminSiteConfiguration.WriteErrorLogs("Status: " + objRes.Status);
                AdminSiteConfiguration.WriteErrorLogs("Message: " + objRes.Message);
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
                            db.Database.ExecuteSqlCommand("SP_QBOnlineConfiguration @Mode = {0},@StoreId = {1}", "UpdateQBIsTokenUpdate", StoreId);
                            return QBEditSyncData;
                        }
                        QBOnlineConfiguration QBOnlinesq = db.QBOnlineConfigurations.Where(a => a.StoreId == StoreId).FirstOrDefault();
                        QBOnlinesq.QBToken = getnewtokan;
                        db.Entry(QBOnlinesq).State = EntityState.Modified;
                        db.SaveChanges();
                        QBOnlinesq = null;
                        IsFirst = true;
                        goto rerun;
                    }
                    else
                    {
                        QBClass.Errolog("QbClass", "QBEditBillData", objRes.Message, StoreId, dbinvoiceId);
                    }
                }
                objBill = null;
            }
            return QBEditSyncData;
        }

        public static string GetSyncToken(string Id, QBOnlineconfiguration objOnline, ref QBResponse objRes)
        {
            string sToken = "";
            SynthesisQBOnline.QBClass.QBBill.CheckBillID(Id, objOnline, ref objRes);
            if (objRes.Status != null)
            {
                sToken = objRes.SyncToken;
            }
            return sToken;
        }

        public static string QBGetVendorStatus(string TxnId, int StoreId, ref QBResponse objRes)
        {
            string status = "";
            QBOnlineconfiguration objOnlieDetail = GetConfigDetail(StoreId);
            status = GetVenStatus(TxnId, objOnlieDetail, ref objRes);
            return status;
        }

        public static string GetVenStatus(string Id, QBOnlineconfiguration objOnline, ref QBResponse objRes)
        {
            string status = "";
            SynthesisQBOnline.QBClass.QBVendor.CheckVendorID(Id, objOnline, ref objRes);
            if (objRes.Status != null)
            {
                status = objRes.QBStatus;
            }
            return status;
        }
    }
}