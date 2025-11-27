using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;

namespace EntityModels.Models
{
    public class YearlySales
    {
        public decimal? SalesAmount { get; set; }
        public decimal? CogsAmount { get; set; }
        public decimal? GrossMargin { get; set; }
        public decimal? PayrollAmount { get; set; }
        public decimal? ProfitAfterPayrollAmount { get; set; }
        public decimal? ExpensesAmount { get; set; }
        public decimal? NetProfitAmount { get; set; }

        public decimal? SalesPercent { get; set; }
        public decimal? CogsPercent { get; set; }
        public decimal? GrossMarginPercent { get; set; }
        public decimal? PayrollPercent { get; set; }
        public decimal? ProfitAfterPayrollPercent { get; set; }
        public decimal? ExpensesPercent { get; set; }
        public decimal? NetProfitPercent { get; set; }

        public decimal? SalesAmountYTD { get; set; }
        public decimal? CogsAmountYTD { get; set; }
        public decimal? GrossMarginYTD { get; set; }
        public decimal? PayrollAmountYTD { get; set; }
        public decimal? ProfitAfterPayrollAmountYTD { get; set; }
        public decimal? ExpensesAmountYTD { get; set; }
        public decimal? NetProfitAmountYTD { get; set; }

        public List<DropdownDepartmentList> DepartmentList { get; set; }
    }

    public class YearlyGrowt
    {
        public string SalesWeeks { get; set; }
        public decimal Sales { get; set; }
        public decimal LastYearSales { get; set; }
        public string Type { get; set; }
    }

    public class ChartModel
    {
        public decimal Amount { get; set; }
        public string WeekNo { get; set; }
    }
    public class YearlyChartData
    {
        public YearlyChartData()
        {
            this.Labels = new List<string>();
            this.Data = new List<string>();
            var rnd = new Random();
            this.backgroundColor = new List<string>();
            this.borderColor = new List<string>();
        }
        public List<string> Labels { get; set; }
        public List<string> Data { get; set; }
        public List<string> backgroundColor { get; set; }
        public List<string> borderColor { get; set; }
    }
}