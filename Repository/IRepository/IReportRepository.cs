using EntityModels.Models;
using SynthesisQBOnline.BAL;
using SynthesisViewModal;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.IRepository
{
    public interface IReportRepository
    {
        void operatingRatioReport(ref OperatingRatioReport obj, string UserName);
        List<OperatingRatioList> Get_Expense_Detail_Storewise(ReportModel model, ReportViewModel obj, string UserName);
        List<OperatingRatioList> GetTerminalTracedata(ReportModel model, ReportViewModel obj, string UserName);
        List<OperatingRatioList> GetTerminalTracedatadepartmentwise(ReportModel obj, ReportViewModel model, string UserName);
        string GetSalesSammarieStartDate();
        void ShiftWiseTenderReport_Select(ref ShiftWiseTenderReport_Select shift, ref ReportViewModel viewModel, ReportModel model, string FromDate, string ToDate);
        List<TerminalMaster> terminalMasters();
        IEnumerable GetData(int SearchRecords = 0, string startdate = "", string enddate = "", int Storeid = 0, int Terminalid = 0, string ShiftName = "", string UserName = "");
        string GetPOS_StoreList(int StoreId, string UserName);
        void InsertStorewisePDFUpload(StorewisePDFUpload uploadPDF);
        List<ddllist> GetBindTerminalByStoreId(int StoreId = 0);
        void getDailyPosFeedsDetails(ref DailyPosFeedsDetails Storedetail, int id);
        void DeleteDailyPOS(int Id = 0);
        IEnumerable PayrollExpenseGridGetData(int SearchRecords = 0, string SearchTitle = "", int StoreId = 0, string UserName = "", int UserID = 0, int? year = null);
        string GetPayrollExpense_StoreList(int StoreId, string UserName);
        string GetNotConfigureAccount(int StoreID);
        void PayrollExpenseDetailss(ref PayrollExpenseDetailss Detail, int StoreID, int id);
        void PayrollCashAnalysisDetailAmount(ref PayrollExpenseDetailss posteddata);
        PayrollMaster PayrollMaster(int Id);
        string GetVendorListIDFromBankAccount(int StoreID);
        string GetAccountListIDFromBankAccount(int StoreID);
        int InsertExpenseDetail(ref Expense obj,ref List<ExpenseDetail> details, int storeID,int Id);
        List<int> GetPayrollReportIdList(int PayrollReportId);
        PayrollReport UpdatePayrollReports(string ResponseID, int item);
        List<OperatingRatioList> GetTerminalTracedataDaily(ReportModel model, ReportViewModel obj, string UserName);
        List<OperatingRatioList> Get_Terminal_Trace_data_department_wise_Daily(ReportModel model, ReportViewModel obj , string UserName);
        List<PayrollAnalysisList> GetPayrollAnalysisdata(ReportModel model, ReportViewModel obj , string UserName);
        List<PayrollAnalysisList> GetPayrollAnalysisdataDepartmentWise(ReportModel model, ReportViewModel obj , string UserName);
        List<PayrollAnalysisTotal> Get_PayrollAnalysis_data_Total(ReportModel model, ReportViewModel obj , string UserName);
        List<OperatingRatioList> getStoreWiseOperatingRationDailyAllTotal(ReportModel model, ReportViewModel obj, string UserName);
    }
}
