using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace EntityModels.Models
{
    [Table("Configuration")]
    public class Configuration
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Editable(false)]
        public int ConfigurationId { get; set; }

        public int? DepartmentId { get; set; }
        [ForeignKey("DepartmentId")]
        public virtual DepartmentMaster DepartmentMasters { get; set; }

        public int? StoreId { get; set; }
        [ForeignKey("StoreId")]
        public virtual StoreMaster StoreMaster { get; set; }

        public int? ConfigurationGroupId { get; set; }
        [ForeignKey("ConfigurationGroupId")]
        public virtual ConfigurationGroup ConfigurationGroups { get; set; }

        [MaxLength(1000)]
        public string Title { get; set; }

        [MaxLength(2000)]
        public string Memo { get; set; }

        public int? TypicalBalanceId { get; set; }
        [ForeignKey("TypicalBalanceId")]
        public virtual TypicalBalanceMaster TypicalBalanceMasters { get; set; }

        [NotMapped]
        public List<Configurationlist> Configrationlist { get; set; }
        
    }

    public class DrpList
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
    public class Configurationlist
    {        
        public int? groupid { get; set; }
        public string Groupname { get; set; }
        public int? Deptid { get; set; }
        public int Storeid { get; set; }
        public int? typicalbalid { get; set; }
        public string memo { get; set; }
        public string typicalbalance { get; set; }
        public string Deptname { get; set; }
        public string tendername { get; set; }
        public int flag { get; set; }
        public List<DrpList> GroupList { get; set; }
        public int? ggID { get; set; }
        public List<DrpList> DeptSalesList { get; set; }
    }

    public class SpConfigurationListData
    {
        public string Title { get; set; }
        public Nullable<int> GroupId { get; set; }
        public Nullable<int> typicalbalid { get; set; }
        public string typicalBalName { get; set; }
        public Nullable<int> deptid { get; set; }
        public string DeptName { get; set; }
        public string Memo { get; set; }
        public int flag { get; set; }
    }
    public class ConfigurationGroupData
    {
        public string Deptname { get; set; }
        public int? DeptId { get; set; }
        public string memo { get; set; }
        public string typicalbalance { get; set; }
        public int? typicalBalId { get; set; }
        
    }

    public class DrpListStr
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }
    //public enum TypicalBalanceEnm
    //{
    //    Credit = 1,
    //    Debit = 2
    //}
}