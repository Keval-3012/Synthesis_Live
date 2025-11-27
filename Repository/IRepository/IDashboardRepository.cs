using EntityModels.Models;
using SynthesisViewModal;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.IRepository
{
    public interface IDashboardRepository
    {
        List<string> CheckError(int UserTypeID, int StoreID);
        string Startdate(int StoreID);
        DashboardDaily Dashboard_Daily(string Startdate,int StoreID,int? StateId,int? GroupID);
        List<StateMaster> ViewBagStateId();
        DashboardDaily getDashboardDailyData(string date,string StoreID,int? StateId, int? GroupID);
        List<SalesOneHourWorkedDataList> GetSalesOneHourWorkedData(string date,int StoreID,string department,int? StateId, int? GroupID);
        DashboardChartViewModel GetChartData(string StoreId, string Department, int Type, string Date, int? StateId, int? GroupID);
        List<DashboardDailyTotalData> GetSalesTotalDetailsdata(string StoreId, string Department, int Type, string Date, int? StateId, int? GroupID);
        List<DashboardDailyTotalData> GetWeeklySalesTotalDetailsdata(string StoreId, string Department, int Type, string WeeklyPeriodId, int? StateId);
        List<DashboardDailyTotalData> GetWeeklySalesDailyTotalDetailsdata(string StoreId, string Department, int Type, string WeeklyPeriodId, int? StateId);
        HourlyChartViewModel GetHourlyChartData(string StoreId, string Department, int Type, string Date, int? StateId, int? GroupID);
        DashboardChartViewModel GetChartDataDoughnut(string StoreId, string Department, string WeekNo, int Type, int? StateId);
        DashboardChartViewModel GetChartDataBar(string StoreId, string Department, int Type, string Date, int? StateId, int? GroupID);
        List<BankAccountDetail> BankAccountDetailValue(int UserTypeId,int StoreID,string UserNm);
        List<BankAccountDetail> BankAccountDetailValueRefresh(int UserTypeId, int StoreID);
        DashboardDaily Weekly(string Startdate,int StoreID,int? StateId);
        DashboardDaily Weekly_1(int? StateId, ref DashboardDaily obj);
        int WeeklyPeriodId(DateTime dtStart);
        List<DropdownList> ChartStoreDepartmentData(int? StateId, DateTime dtStart);
        DashboardDaily getDashboardWeeklyData(int WeeklyPeriodId, int? StateId, int StoreID);
        DashboardDaily GetPayrollSalesBoxesData(int WeeklyPeriodId, string Department, int? StateId, int StoreID);
        DashboardChartViewModel GetWeeklyChartData(string StoreId, string Department, int Type, int WeeklyPeriodId, int? StateId);
        DashboardChartViewModel GetWeeklyBarChartData(string StoreId, string Department, int Type, int WeeklyPeriodId, int? StateId);
        WeeklyComparisionData GetWeeklyComparisionData(string StoreId, string Department, int Type, int WeeklyPeriodId, int? StateId);
        HourlyChartViewModel GetHourlyChartDataWeek(string StoreId, string Department, int Type, int WeeklyPeriodId, int? StateId);
        List<SalesOneHourWorkedDataList> GetSalesOneHourWorkedDataWeekly(int WeeklyPeriodId, string department, int? StateId, int StoreID);
        List<DashboardChartToolTipData> getToolTipData(string StoreId, string Department, int Type, string Date, string Hours, string Mode);
        List<DashboardChartToolTipData> getWeeklyHourlyChartToolTipData(string StoreId, string Department, int Type, int WeeklyPeriodId);
        DashboardWeeklyExpenses GetExpensesBoxesData(int WeeklyPeriodId, string StoreID, int? StateId);
        DateTime GetEndDateWeeklyPeriod();
        DateTime EndDate();
        List<DropdownList> ViewBagWeeklyPeriodId();
        DashboardDaily Periodic(string Startdate,int StoreID);
        DashboardDaily getDashboardPeriodicData(string StartDate, string EndDate, int? StateId, DateTime Start, DateTime End, int StoreID);
        List<SalesOneHourWorkedDataList> GetSalesOneHourWorkedDataPeriodic(string StartDate, string EndDate, string department, int? StateId, DateTime Start, DateTime End, int StoreID);
        List<DashboardDailyTotalData> GetPeriodicSalesTotalDetailsdata(string StoreId, string Department, int Type, DateTime Start, DateTime End);
        DashboardChartViewModel GetPeriodicChartData(string StoreId, string Department, int Type, int? StateId, DateTime Start, DateTime End);
        DashboardChartViewModel GetChartDataDoughnutPeriodic(string StoreId, string Department, int Type, int? StateId, DateTime Start, DateTime End);
        DashboardChartViewModel GetPeriodicBarChartData(string StoreId, string Department, int Type, int StateId, DateTime Start, DateTime End);
        HourlyChartViewModel GetHourlyChartDataPeriodic(string StoreId, string Department, int Type, int? StateId, DateTime Start, DateTime End);
        List<DashboardChartToolTipData> getPeriodicHourlyChartToolTipData(string StoreId, string Department, int Type, DateTime Start, DateTime End);
        DashboardDaily GetPayrollSalesBoxesDataPeriodic(string Department, int? StateId, int StoreID, DateTime Start, DateTime End);
        DashboardWeeklyExpenses GetExpensesBoxesDataPeriodic(string StoreID, int? StateId, DateTime Start, DateTime End);
        WeeklyPeriod GetWeekDate(string WeeklyPeriodId);
        WeeklyComparisionData GetPeriodicComparisionData(string StoreId, string Department, int Type, int? StateId, DateTime Start, DateTime End);
        string ViewBagPeriod();
        YearlySales DepartmentList();
        List<int> ViewbagYear();
        DashboardViewModel GetYearlySalesData(string Departments, int? StateId, int Year, int StoreID);
        DashboardViewModel GetQatarDate(string Qatar);
        DashboardViewModel GetYTDDate(string Qatar);

        DashboardDaily ddlPeriodic();
        string GetStateName(int? StateId);
        List<DashboardDailySelectChart> GetChartDataForPDF(string StoreId, string Department, int Type, string Date, int? StateId, int? GroupID);
        List<int> GetStoreBySelectedStateId(int? stateid);

        List<SalesDataModel> GetNetSalesData(DateTime startdate, DateTime enddate, string storeids);
        List<GroupWiseStateStore> ViewBagGroupId(int UserId);
        List<int> GetStoreBySelectedGroupId(int? GroupID, int UserId);
    }
}
