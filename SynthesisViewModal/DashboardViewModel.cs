using EntityModels.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace SynthesisViewModal
{
    public class DashboardViewModel
    {
        public DashboardViewModel()
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
        public List<YearlyGrowt> growt { get; set; }
        public YearlySales sales { get; set; }
        public YearlySales salesL { get; set; }
        public List<ChartModel> charts { get; set; }
        public YearlyChartData result { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
    }
}
