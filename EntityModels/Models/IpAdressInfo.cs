using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace EntityModels.Models
{
    [Table("IpAdressInfo")]
    public class IpAdressInfo
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Editable(false)]
        public int IpAdressInfoID { get; set; }
        public int StoreId { get; set; }
        
        public string StaticIp { get; set; }
        public string StartIP { get; set; }
        public string EndIP { get; set; }
        public string Location { get; set; }
        public bool IsActive { get; set; }
        public DateTime? CreatedOn { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }

        public int? UpdatedBy { get; set; }
        [NotMapped]
        public string[] MultiStoreId { get; set; }
        [NotMapped]
        public string IP { get; set; }
        [NotMapped]
        public string[] MultiUserId { get; set; }
        [NotMapped]
        public string Users { get; set; }

        [NotMapped]
        public string Status { get; set; }
        [NotMapped]
        public string StoreName { get; set; }
        public LocationType? LocationType { get; set; }

    }

    public enum LocationType
    {
        Office = 1,
        Remote = 2
    }

    [Table("UserIpInfo")]
    public class UserIpInfo
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Editable(false)]
        public int UserInfoID { get; set; }
       
        public int IpAdressInfoID { get; set; }
        public int UserID { get; set; }

    }
    public class IpAdressInfoSelect
    {
        
        public int IpAdressInfoID { get; set; }
        public int StoreId { get; set; }

        public string StaticIp { get; set; }
        public string StartIP { get; set; }
        public string EndIP { get; set; }
        public string Location { get; set; }
        public bool IsActive { get; set; }
        public string Status { get; set; }
        public string StoreName { get; set; }


    }

    public class UserActivityList
    {
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public decimal TotalHoursWorked { get; set; }
        public decimal Regular { get; set; }
        public decimal OverTime { get; set; }
        public decimal? OfficeHours { get; set; }
        public decimal? RemoteHours { get; set; }
    }
    public class GetUserTrackTime
    {
        public int UserId { get; set; }
        public int? InTimeId { get; set; }
        public int? OutTimeId { get; set; }
        public string dt { get; set; }
        public string InTime { get; set; }
        public string OutTime { get; set; }
        public string InLocation { get; set; }
        public string OutLocation { get; set; }
        public decimal? TotalHours { get; set; }

        public string InHour { get; set; }
        public string InMin { get; set; }
        public string InMidday { get; set; }
        public string OutHour { get; set; }
        public string OutMin { get; set; }
        public string OutMidday { get; set; }

        public string dtdate { get; set; }
    }

  

    public class CheckInButtonValues
    {
        public int? UserId { get; set; }
        public decimal? TotalHoursWorked { get; set; }
    }
}