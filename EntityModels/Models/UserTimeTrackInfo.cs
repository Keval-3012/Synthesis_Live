using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace EntityModels.Models
{
    public class UserTimeTrackInfo
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Editable(false)]
        public int UserTimeTrackInfoID { get; set; }
        public int UserId { get; set; }
        public DateTime? StartTime { get; set; }
        public string Location { get; set; }
        public string IpAddress { get; set; }
        public int ActivityType { get; set; }
        public DateTime? CreatedOn { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public int? UpdatedBy { get; set; }
        public CheckInOutType? InOutType { get; set; }
        public string TimeDuration { get; set; }

        public Boolean IsSystemEntry { get; set; }

        public Boolean IsTimeCardEntry { get; set; }
    }

    public enum CheckInOutType
    {
        CheckIn = 1,
        CheckOut = 2
    }
}