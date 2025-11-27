using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityModels.Models
{
    public class DailyDashboardXML
    {
        public DateTime Hours { get; set; }
        public DashboardDaily dashboardDaily { get; set; }
    }
}
