using EntityModels.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Repository.IRepository
{
    public interface IPDFReadRepository
    {
        IEnumerable GetpayrollReportsByStoreID(int StoreID);
        void AddPayrollReports(PayrollReport payrollreport);
        void AddPayrollDepartmentDetails(PayrollDepartmentDetails payrollDepartmentDetail);
        int AddPayrollDepartments(PayrollDepartment PayrollDepartments);
        List<PayrollMaster> GetPayrollMasters(PayrollMaster PayrollMasters);
        List<PayrollReport> GetPayrollReports(int[] PayReportIDList, int FileNo);
        string UpdatePayrollReports(PayrollMaster PayrollReports);
        int AddPayrollMasters(PayrollMaster PayrollMaster);
        void UpdateFileNoInPayrollReport(int PayrollReportId, int FileNo);
        string ReomvePayrollReports(int id);
        void UpdatePayrollReportStatus(int PayrollReportId);
        List<PayrollDepartment> GetPayrollDepartmentsList(int StoreId, string TrimDepartmentName);
        List<PayrollCashAnalysis> Getpayrollcashanalysis(int StoreId, string Name);
        int AddpayrollCashAnalyse(PayrollCashAnalysis payrollCashAnalyse);
        void AddPayrollCashAnalysisDetail(PayrollCashAnalysisDetail PayrollCashAnalysisDetail);
        PayrollReport GetPayrollReportbyPayrollReportId(int PayrollReportId);
        void SavePayrollReport(PayrollReport obj);
        PayrollDepartmentDetails GetpayrollDepartmentDetails(int PayrollId, int DepartmentId, string Value2);

        void InsertPayrollFile(DataTable dt, int StoreID, int PayrollID);
        List<PayrollDetails> GetPayrollDepartmentsList(int PayrollReportId);
        List<PayrollDepartment> GetPayrollDepartmentsStorewise(int StoreId);
        int AddmanualPayrollReports(PayrollReport payreport);
        void AddTranscationNo(DateTime StartDate, DateTime EndDate, DateTime EndCheckDate, int StoreId,string TransactionNo);
    }
}
