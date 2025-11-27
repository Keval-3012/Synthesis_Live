using EntityModels.Models;
using Newtonsoft.Json.Linq;
using NLog;
using Repository.IRepository;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;

namespace Repository
{
    public class PDFReadRepository : IPDFReadRepository
    {
        private DBContext db;
        Logger logger = LogManager.GetCurrentClassLogger();

        public PDFReadRepository(DBContext context) { db = context; }
        public IEnumerable GetpayrollReportsByStoreID(int StoreID)
        {
            IEnumerable RtnData = null;
            try
            {
                var dtProcessed = db.PayrollReports.Where(s => s.StoreId == StoreID).ToList();
                RtnData = dtProcessed.ToList()
                    .Select(
                        s => new PayrollFileList
                        {
                            PayrollReportID = s.PayrollReportId,
                            IsRead = s.IsRead,
                            FileName = s.FileName,
                            StartDate = s.PayrollMasters.FirstOrDefault()?.StartDate != null
                                        ? s.PayrollMasters.FirstOrDefault().StartDate.ToString("MM/dd/yyyy")
                                        : string.Empty,
                            EndDate = s.PayrollMasters.FirstOrDefault()?.EndDate != null
                                        ? s.PayrollMasters.FirstOrDefault().EndDate.ToString("MM/dd/yyyy")
                                        : string.Empty,
                            UploadDate = s.UploadDate.ToString("MM/dd/yyyy"),
                            EndCheckDate = s.PayrollMasters.FirstOrDefault()?.EndCheckDate != null
                                        ? s.PayrollMasters.FirstOrDefault().EndCheckDate.Value.ToString("MM/dd/yyyy")
                                        : string.Empty,
                            StoreID = s.StoreId
                        })
                    .ToList();
            }
            catch (Exception ex)
            {
                logger.Error("PDFReadRepository - GetpayrollReportsByStoreID - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return RtnData;
        }
        public void AddPayrollReports(PayrollReport payrollreport)
        {
            try
            {
                db.PayrollReports.Add(payrollreport);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Error("PDFReadRepository - AddPayrollReports - " + DateTime.Now + " - " + ex.Message.ToString());
            }
        }
        public void AddPayrollDepartmentDetails(PayrollDepartmentDetails payrollDepartmentDetail)
        {
            try
            {
                db.payrollDepartmentDetails.Add(payrollDepartmentDetail);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Error("PDFReadRepository - AddPayrollDepartmentDetails - " + DateTime.Now + " - " + ex.Message.ToString());
            }
        }
        public int AddPayrollMasters(PayrollMaster PayrollMaster)
        {
            int PayrollId = 0;
            try
            {
                db.PayrollMasters.Add(PayrollMaster);
                db.SaveChanges();
                PayrollId = PayrollMaster.PayrollId;
            }
            catch (Exception ex)
            {
                logger.Error("PDFReadRepository - AddPayrollMasters - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return PayrollId;
        }
        public int AddpayrollCashAnalyse(PayrollCashAnalysis payrollCashAnalyse)
        {
            int DepartmentId = 0;
            try
            {
                db.payrollCashAnalyses.Add(payrollCashAnalyse);
                db.SaveChanges();
                DepartmentId = payrollCashAnalyse.PayrollCashAnalysisId;
            }
            catch (Exception ex)
            {
                logger.Error("PDFReadRepository - AddpayrollCashAnalyse - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return DepartmentId;
        }
        public void AddPayrollCashAnalysisDetail(PayrollCashAnalysisDetail PayrollCashAnalysisDetail)
        {
            try
            {
                db.payrollCashAnalysisDetails.Add(PayrollCashAnalysisDetail);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Error("PDFReadRepository - AddPayrollCashAnalysisDetail - " + DateTime.Now + " - " + ex.Message.ToString());
            }
        }
        public void UpdateFileNoInPayrollReport(int PayrollReportId, int FileNo)
        {
            try
            {
                PayrollReport payroll = db.PayrollReports.Find(PayrollReportId);
                if (payroll != null)
                {
                    payroll.FIleNo = FileNo;
                }
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Error("PDFReadRepository - UpdateFileNoInPayrollReport - " + DateTime.Now + " - " + ex.Message.ToString());
            }
        }
        public void UpdatePayrollReportStatus(int PayrollReportId)
        {
            try
            {
                db.Database.ExecuteSqlCommand("SP_Payroll @Mode = {0},@PayrollReportId = {1}","UpdatePayrollReportStatus",PayrollReportId);
            }
            catch (Exception ex)
            {
                logger.Error("PDFReadRepository - UpdatePayrollReportStatus - " + DateTime.Now + " - " + ex.Message.ToString());
            }
        }
        public int AddPayrollDepartments(PayrollDepartment PayrollDepartments)
        {
            int PayrollDepartmentId = 0;
            try
            {
                db.PayrollDepartments.Add(PayrollDepartments);
                db.SaveChanges();
                PayrollDepartmentId = PayrollDepartments.PayrollDepartmentId;
            }
            catch (Exception ex)
            {
                logger.Error("PDFReadRepository - AddPayrollDepartments - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return PayrollDepartmentId;
        }
        public string UpdatePayrollReports(PayrollMaster obj)
        {
            string PdfFileName = "";
            try
            {
                var DepartmentData = db.PayrollReports.Find(obj.PayrollReportId);
                DepartmentData.IsRead = true;
                PdfFileName = DepartmentData.FileName;

                db.Entry(DepartmentData).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Error("PDFReadRepository - UpdatePayrollReports - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return PdfFileName;
        }
        public void SavePayrollReport(PayrollReport obj)
        {
            try
            {
                db.Entry(obj).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Error("PDFReadRepository - SavePayrollReport - " + DateTime.Now + " - " + ex.Message.ToString());
            }
        }
        public string ReomvePayrollReports(int ID)
        {
            string FileName = "";
            try
            {
                PayrollReport Data = db.PayrollReports.Find(ID);
                FileName = Data.FileName;
                db.PayrollReports.Remove(Data);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Error("PDFReadRepository - ReomvePayrollReports - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return FileName;
        }
        public List<PayrollDepartment> GetPayrollDepartmentsList(int StoreId, string TrimDepartmentName)
        {
            List<PayrollDepartment> payrolldepartment = new List<PayrollDepartment>();
            try
            {
                payrolldepartment = db.PayrollDepartments
                                        .Where(s => s.StoreId == StoreId && s.DepartmentName == TrimDepartmentName)
                                        .ToList();
            }
            catch (Exception ex)
            {
                logger.Error("PDFReadRepository - GetPayrollDepartmentsList - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return payrolldepartment;
        }
        public List<PayrollCashAnalysis> Getpayrollcashanalysis(int StoreId, string Name)
        {
            List<PayrollCashAnalysis> payrollcashanalysis = new List<PayrollCashAnalysis>();
            try
            {
                payrollcashanalysis = db.payrollCashAnalyses.Where(a => a.StoreId == StoreId && a.Name == Name).ToList();
            }
            catch (Exception ex)
            {
                logger.Error("PDFReadRepository - Getpayrollcashanalysis - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return payrollcashanalysis;
        }
        public List<PayrollMaster> GetPayrollMasters(PayrollMaster obj)
        {
            List<PayrollMaster> payrollmaster = new List<PayrollMaster>();
            try
            {
                payrollmaster = db.PayrollMasters
                                    .Where(
                                        a => a.StartDate == obj.StartDate &&
                                            a.EndDate == obj.EndDate &&
                                            a.EndCheckDate == obj.EndCheckDate &&
                                            a.StoreId == obj.StoreId)
                                    .ToList();
            }
            catch (Exception ex)
            {
                logger.Error("PDFReadRepository - GetPayrollMasters - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return payrollmaster;
        }
        public List<PayrollReport> GetPayrollReports(int[] PayReportIDList, int FileNo)
        {
            List<PayrollReport> payrollreports = new List<PayrollReport>();
            try
            {
                payrollreports = db.PayrollReports
                            .Where(a => PayReportIDList.Contains(a.PayrollReportId) && a.FIleNo == FileNo)
                            .ToList();
            }
            catch (Exception ex)
            {
                logger.Error("PDFReadRepository - GetPayrollReports - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return payrollreports;
        }
        public PayrollReport GetPayrollReportbyPayrollReportId(int PayrollReportId)
        {
            PayrollReport payrollreports = new PayrollReport();
            try
            {
                payrollreports = db.PayrollReports.Where(a => a.PayrollReportId == PayrollReportId).FirstOrDefault();
            }
            catch (Exception ex)
            {
                logger.Error("PDFReadRepository - GetPayrollReportbyPayrollReportId - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return payrollreports;
        }
        public PayrollDepartmentDetails GetpayrollDepartmentDetails(int PayrollId, int DepartmentId, string Value2)
        {
            PayrollDepartmentDetails payrolldepartmentdetails = new PayrollDepartmentDetails();
            try
            {
                payrolldepartmentdetails = db.payrollDepartmentDetails
                                        .Where(
                                            a => a.PayrollId == PayrollId &&
                                                a.PayrollDepartmentId == DepartmentId &&
                                                a.Name == Value2)
                                        .FirstOrDefault();
            }
            catch (Exception ex)
            {
                logger.Error("PDFReadRepository - GetpayrollDepartmentDetails - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return payrolldepartmentdetails;
        }

        public void InsertPayrollFile(DataTable dt, int StoreID, int PayrollID)
        {
            try
            {
                dt.TableName = "PayrollFileOne";
                var PayrollFileOne = new SqlParameter("@PayrollFileOne", SqlDbType.Structured);
                PayrollFileOne.Value = dt;
                PayrollFileOne.TypeName = "PayrollFileOne";
                PayrollFileOne.Direction = ParameterDirection.Input;

                var paramStoreID = new SqlParameter("@StoreID", SqlDbType.Int);
                paramStoreID.Value = StoreID;

                var paramPayrollID = new SqlParameter("@PayRollID", SqlDbType.Int);
                paramPayrollID.Value = PayrollID;

                db.Database.ExecuteSqlCommand("sp_InsertPayrollFileData @PayrollFileOne, @StoreID, @PayRollID",
                    PayrollFileOne, paramStoreID, paramPayrollID);
            }
            catch (Exception ex)
            {
                logger.Error("PDFReadRepository - InsertPayrollFile - " + DateTime.Now + " - " + ex.Message.ToString());
            }
        }
        public List<PayrollDetails> GetPayrollDepartmentsList(int PayrollReportId)
        {
            List<PayrollDetails> obj = new List<PayrollDetails>();
            try
            {
                obj = db.Database.SqlQuery<PayrollDetails>("SP_Payroll @Mode={0},@PayrollReportId={1}", "GetPayrollFileDetails", PayrollReportId).ToList();
            }
            catch (Exception ex)
            {
                logger.Error("PDFReadRepository - GetPayrollDepartmentsList - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return obj;
        }

        public List<PayrollDepartment> GetPayrollDepartmentsStorewise(int StoreId)
        {
            List<PayrollDepartment> payrolldepartment = new List<PayrollDepartment>();
            try
            {
                payrolldepartment = db.PayrollDepartments.Where(s => s.StoreId == StoreId).ToList();
            }
            catch (Exception ex)
            {
                logger.Error("PDFReadRepository - GetPayrollDepartmentsStorewise - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return payrolldepartment;
        }

        public int AddmanualPayrollReports(PayrollReport payreport)
        {
            int PayrollReportId = 0;
            try
            {
                db.PayrollReports.Add(payreport);
                db.SaveChanges();
                PayrollReportId = payreport.PayrollReportId;
            }
            catch (Exception ex)
            {
                logger.Error("PDFReadRepository - AddPayrollReports - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return PayrollReportId;
        }

        public void AddTranscationNo(DateTime StartDate, DateTime EndDate, DateTime EndCheckDate, int StoreId, string TransactionNo)
        {
            List<PayrollMaster> paymasters = new List<PayrollMaster>();
            try
            {
                paymasters = db.PayrollMasters.Where(a => a.StartDate == StartDate && a.EndDate == EndDate && a.EndCheckDate == EndCheckDate && a.StoreId == StoreId).ToList();
                foreach (var item in paymasters)
                {
                    PayrollReport payroll = db.PayrollReports.Find(item.PayrollReportId);
                    if (payroll != null)
                    {
                        payroll.TransactionNo = TransactionNo;
                        db.Entry(payroll).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("PDFReadRepository - AddTranscationNo - " + DateTime.Now + " - " + ex.Message.ToString());
            }
        }
    }
}
