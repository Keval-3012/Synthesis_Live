using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace EntityModels.Models
{
    [Table("PayrollCashAnalysis")]
    public class PayrollCashAnalysis
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Editable(false)]
        public int PayrollCashAnalysisId { get; set; }

        public int? StoreId { get; set; }
        [ForeignKey("StoreId")]
        public virtual StoreMaster StoreMasters { get; set; }

        [MaxLength(500)]
        public string Name { get; set; }

        public int? DepartmentId { get; set; }
        [ForeignKey("DepartmentId")]
        public virtual DepartmentMaster DepartmentMasters { get; set; }

        public int? ValueIn { get; set; }

        [MaxLength(2000)]
        public string Description { get; set; }

        public bool IsActive { get; set; }

        public int? SortingNo { get; set; }
        
        public int? NewSortingNo { get; set; }

        public int? ETaxCalc { get; set; }

        public virtual ICollection<PayrollCashAnalysisDetail> PayrollCashAnalysisDetails { get; set; }
    }
}