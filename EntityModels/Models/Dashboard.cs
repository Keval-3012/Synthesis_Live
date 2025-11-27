using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Linq;
using System.Web;

namespace EntityModels.Models
{
    //----- OperatingRatioReport -------------//

    public class DashboardDaily
    {
        public decimal Date { get; set; }
        public int CustomerCountCurrentDay { get; set; }
        public int AllVoids { get; set; }
        public int ItemCorrects { get; set; }
        public int ItemReturns { get; set; }

        public decimal AllVoidsAmt { get; set; }
        public decimal ItemCorrectsAmt { get; set; }
        public decimal ItemReturnsAmt { get; set; }

        public decimal TotalCash { get; set; }
        public decimal AverageSaleCurrentDay { get; set; }
        public decimal CashPayout { get; set; }
        public decimal Over { get; set; }
        public decimal TotalSalesCurrentDay { get; set; }
        public decimal TotalSalesLastWeek { get; set; }
        public int CustomerCountLastWeek { get; set; }
        public decimal AverageSalesLastWeek { get; set; }

        public decimal SalesGrowth { get; set; }
        public decimal CustomerCountGrowth { get; set; }
        public decimal AveragesalesGrowth { get; set; }

        public decimal Offline { get; set; }
        public decimal Online { get; set; }
        public int InvoicesAdded { get; set; }
        public int InvoicesApproved { get; set; }

        public decimal PayrollOverTime { get; set; }
        public decimal PayrollSickPay { get; set; }
        public decimal PayrollVacation { get; set; }
        public decimal PayrollHolidays { get; set; }
        public decimal PayrollBonus { get; set; }
        public decimal PayrollRegularPay { get; set; }
        public decimal PayrollSalary { get; set; }
        public decimal SalesRegularPay { get; set; }
        public decimal SalesOverTime { get; set; }
        public decimal SalesSalary { get; set; }
        public decimal SalesOtherpay { get; set; }
        public string HourTime { get; set; }

        public string DayName { get; set; }

        public int SignedCount { get; set; }
        public int UnSignedCount { get; set; }

        public List<DropdownList> StoreList { get; set; }
        public List<DropdownDepartmentList> DepartmentList { get; set; }
        public List<DropdownDepartmentList> PayrollDepartmentList { get; set; }
        public List<DropdownList> WeeklyPeriodList { get; set; }
        public List<DashboardDailySelectChart> dailychartlisttype1 { get; set; }
        public List<DashboardDailySelectChart> dailychartlisttype2 { get; set; }
        public List<DashboardDailySelectChart> dailychartlisttype3 { get; set; }
    }
    public class DashboardDailySelect
    {
        public int CustomerCountCurrentDay { get; set; }
        public int AllVoids { get; set; }
        public int ItemCorrects { get; set; }
        public int ItemReturns { get; set; }

        public decimal AllVoidsAmt { get; set; }
        public decimal ItemCorrectsAmt { get; set; }
        public decimal ItemReturnsAmt { get; set; }

        public decimal TotalCash { get; set; }
        public decimal AverageSaleCurrentDay { get; set; }
        public decimal CashPayout { get; set; }
        public decimal Over { get; set; }
        public decimal TotalSalesCurrentDay { get; set; }
        public decimal TotalSalesLastWeek { get; set; }
        public int CustomerCountLastWeek { get; set; }
        public decimal AverageSalesLastWeek { get; set; }

        public decimal SalesGrowth { get; set; }
        public decimal CustomerCountGrowth { get; set; }
        public decimal AveragesalesGrowth { get; set; }

        public decimal Offline { get; set; }
        public decimal Online { get; set; }
        public int InvoicesAdded { get; set; }
        public int InvoicesApproved { get; set; }
        public string HourTime { get; set; }

        public string DayName { get; set; }
    }
    public class DashboardWeeklySelect
    {
        public int CustomerCountCurrentDay { get; set; }
        public int AllVoids { get; set; }
        public int ItemCorrects { get; set; }
        public int ItemReturns { get; set; }

        public decimal AllVoidsAmt { get; set; }
        public decimal ItemCorrectsAmt { get; set; }
        public decimal ItemReturnsAmt { get; set; }

        public decimal TotalCash { get; set; }
        public decimal AverageSaleCurrentDay { get; set; }
        public decimal CashPayout { get; set; }
        public decimal Over { get; set; }
        public decimal TotalSalesCurrentDay { get; set; }
        public decimal TotalSalesLastWeek { get; set; }
        public int CustomerCountLastWeek { get; set; }
        public decimal AverageSalesLastWeek { get; set; }

        public decimal SalesGrowth { get; set; }
        public decimal CustomerCountGrowth { get; set; }
        public decimal AveragesalesGrowth { get; set; }

        public decimal Offline { get; set; }
        public decimal Online { get; set; }
        public int InvoicesAdded { get; set; }
        public int InvoicesApproved { get; set; }


        public decimal? PayrollOverTime { get; set; }
        public decimal? PayrollSickPay { get; set; }
        public decimal? PayrollVacation { get; set; }
        public decimal? PayrollHolidays { get; set; }
        public decimal? PayrollBonus { get; set; }

        public decimal? PayrollSalary { get; set; }
        public decimal? PayrollRegularPay { get; set; }
        public decimal? SalesRegularPay { get; set; }
        public decimal? SalesOverTime { get; set; }
        public decimal? SalesSalary { get; set; }
        public decimal? SalesOtherpay { get; set; }

        public string DayName { get; set; }
    }

    public class ExpensesWeeklyBox
    {
        public string DepartmentName { get; set; }
        public string ExpPercentage { get; set; }
        public decimal Amount { get; set; }
    }

    public class DashboardWeeklyExpenses
    {
        public decimal spvExterminator { get; set; }
        public decimal spvManagement { get; set; }
        public decimal spvOffice { get; set; }
        public decimal spvProduce { get; set; }
        public decimal spvDelivery { get; set; }
        public decimal spvRepairs { get; set; }
        public decimal spvSupplies { get; set; }

        public decimal ExpPercentage { get; set; }

        public string ExpExterminator { get; set; }
        public string ExpManagement { get; set; }
        public string ExpOffice { get; set; }
        public string ExpProduce { get; set; }
        public string ExpDelivery { get; set; }
        public string ExpRepairs { get; set; }
        public string ExpSupplies { get; set; }

        public string spExterminator { get; set; }
        public string spManagement { get; set; }
        public string spOffice { get; set; }
        public string spProduce { get; set; }
        public string spDelivery { get; set; }
        public string spRepairs { get; set; }
        public string spSupplies { get; set; }
    }

    public class SalesOneHourWorkedDataList
    {
        public decimal Amount { get; set; }
        public string StoreName { get; set; }
    }
    public class DashboardDailySelectChart
    {
        public decimal Amount { get; set; }
        public string Hours { get; set; }
        public int StoreId { get; set; }
    }
    public class DashboardDailyTotalData
    {
        public decimal Amount { get; set; }
        public string Type { get; set; }
        public string StoreId { get; set; }
    }
    public class DashboardChartToolTipData
    {
        public decimal Amount { get; set; }
        public string Department { get; set; }
        public string Hour1 { get; set; }
        public string TillTime { get; set; }
        public decimal TAmount { get; set; }
    }
    public class DashboardWeeklySelectChart
    {
        public decimal Amount { get; set; }
        public string Hours { get; set; }
        public int StoreId { get; set; }
    }
    public class WeeklyComparisionData
    {
        public decimal WCCurWeekAmt { get; set; }
        public decimal WCLastWeekAmt { get; set; }
        public decimal WCCurWeekPer { get; set; }
        public decimal WCLYearCurWeekAmt { get; set; }
        public decimal WCLYearCurWeekPer { get; set; }
        public decimal WCAWeekThisYearAmt { get; set; }
        public decimal WCAWeekThisYearPer { get; set; }
        public decimal WCAWeekLastYearAmt { get; set; }
        public decimal WCAWeekLastYearPer { get; set; }

    }
    public class DashboardDougWeeklySelectChart
    {
        public decimal PercentData { get; set; }
        public int Type { get; set; }
        public string Weeks { get; set; }
    }
    public class DropdownList
    {
        public int Id { get; set; }
        public string Text { get; set; }
    }
    public class DropdownDepartmentList
    {
        public string Id { get; set; }
        public string Text { get; set; }
    }

    public class DashboardChartViewModel
    {
        public DashboardChartViewModel() 
        {
            this.Labels = new List<string>();
            this.Labelss = new List<string>();
            this.DatsetLists = new List<DatsetList>();
            this.HourlyDatsetLists = new List<HourlyDatsetList>();
            this.DatsetListDoughnutLists = new List<DatsetListDoughnut>();
        }
        public List<string> Labels { get; set; }
        public List<string> Labelss { get; set; }
        public decimal Total { get; set; }
        public int MyProperty { get; set; }

        public List<DatsetList> DatsetLists { get; set; }
        public List<HourlyDatsetList> HourlyDatsetLists { get; set; }
        public List<DatsetListDoughnut> DatsetListDoughnutLists { get; set; }
        public List<DatsetListDoughnut> DatsetListDoughnutListsC2 { get; set; }
        public List<DatsetListDoughnut> DatsetListDoughnutListsC3 { get; set; }
    }

    public class DatsetList
    {
        public DatsetList()
        {
            this.data = new List<string>();
            var rnd = new Random();
            this.backgroundColor = ColorTranslator.ToHtml(Color.FromArgb(rnd.Next(255), rnd.Next(255), rnd.Next(255)));
            this.borderColor = ColorTranslator.ToHtml(Color.FromArgb(rnd.Next(156), rnd.Next(156), rnd.Next(156)));
        }
        public string label { get; set; }
        //public string type { get; set; }
        public string code { get; set; }
        public string backgroundColor { get; set; }
        public string borderColor { get; set; }
        public bool fill { get; set; }
        //public string stack { get; set; }
        //public string xAxisID { get; set; }
        public List<string> data { get; set; }

        public int lineTension { get; set; }
        public int radius { get; set; }

    }

    public class HourlyChartViewModel
    {
        public HourlyChartViewModel()
        {
            this.Labels = new List<string>();
            this.HourlyDatsetList = new List<HourlyDatsetList>();
        }
        public List<string> Labels { get; set; }

        public List<HourlyDatsetList> HourlyDatsetList { get; set; }
    }

    public class HourlyDatsetList
    {
        public HourlyDatsetList()
        {
            List<hourlydata> data = new List<hourlydata>();
            var rnd = new Random();
            this.backgroundColor = ColorTranslator.ToHtml(Color.FromArgb(rnd.Next(255), rnd.Next(255), rnd.Next(255)));

        }
        public string label { get; set; }
        public string backgroundColor { get; set; }
        public int radius { get; set; }
        public List<hourlydata> data = new List<hourlydata>();
    }
    public class hourlydata
    {
        public string x { get; set; }
        public string y { get; set; }
    }


    public class DatsetListDoughnut
    {
        public DatsetListDoughnut()
        {
            this.data = new List<string>();
            var rnd = new Random();
            this.backgroundColor = new List<string>();
            this.borderColor = ColorTranslator.ToHtml(Color.FromArgb(rnd.Next(156), rnd.Next(156), rnd.Next(156)));
        }
        public string label { get; set; }
        //public string type { get; set; }
        public List<string> backgroundColor { get; set; }
        public string borderColor { get; set; }

        public List<string> data { get; set; }

    }

    public class BankAccountDetail
    {
        public string AccountNo { get; set; }
        public decimal? Balance { get; set; }
        public DateTime? LastSyncDate { get; set; }

        // Added by Subhash 04-07-2023
        public decimal? Mailed6month { get; set; }
        public decimal? MailedLastWeek { get; set; }

        public decimal? MailedLastWeek1 { get; set; }
        public decimal? MailedLastWeek2 { get; set; }
        public decimal? MailedLastWeek3 { get; set; }
        public decimal? MailedLastWeek4 { get; set; }
        public decimal? UnMailed6month { get; set; }
        public decimal? UnMailedLastWeek { get; set; }
        public decimal? UnMailedLastWeek1 { get; set; }
        public decimal? UnMailedLastWeek2 { get; set; }
        public decimal? UnMailedLastWeek3 { get; set; }
        public decimal? UnMailedLastWeek4 { get; set; }
        public int? IsVisible { get; set; }

    }

    public class BankAccountDetailRefresh
    {
        public int? BankAccountId { get; set; }
        public string ItemID { get; set; }
        public string AccessToken { get; set; }
        public int? StoreID { get; set; }
        public string AccountNo { get; set; }
        public decimal? Balance { get; set; }
        public DateTime? LastSyncDate { get; set; }
    }

    public class SalesDataModel
    {
        public DateTime StartDate { get; set; }
        public string StoreName { get; set; }
        public Dictionary<string, decimal> CategorySales { get; set; }
        public decimal CustomerCount { get; set; }
        public decimal AverageSale { get; set; }
    }


    public class ExportSalesDataResult
    {
        public DateTime StartDate { get; set; }
        public string StoreName { get; set; }
        public string MergedDepartment { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal CustomerCount { get; set; }
        public decimal AverageSale { get; set; }
    }
}