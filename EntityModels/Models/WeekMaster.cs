using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace EntityModels.Models
{
    public class WeekMaster
    {
        [Key]
        [System.ComponentModel.DataAnnotations.Schema.DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Editable(false)]
        public int WeekMasterID { get; set; }
        public int StartWeek { get; set; }
        public int EndWeek { get; set; }

        public int Year { get; set; }
        public int WeekNo { get; set; }
        [NotMapped]
        public string StartDates { get; set; }
        [NotMapped]
        public string EndDates { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
    public class DrpWeekRange
    {
        public string Id { get; set; }
        public string Text { get; set; }
    }
}