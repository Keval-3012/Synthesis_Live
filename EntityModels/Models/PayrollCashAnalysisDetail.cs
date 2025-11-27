using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace EntityModels.Models
{
    [Table("PayrollCashAnalysisDetail")]
    public class PayrollCashAnalysisDetail
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Editable(false)]
        public int PayrollCashAnalysisDetailId { get; set; }

        public int? PayrollCashAnalysisId { get; set; }
        [ForeignKey("PayrollCashAnalysisId")]
        public virtual PayrollCashAnalysis PayrollCashAnalysis { get; set; }

        public decimal? Amount { get; set; }

        public int? PayrollId { get; set; }
        [ForeignKey("PayrollId")]
        public virtual PayrollMaster PayrollMaster { get; set; }
        public int ETaxCalc { get; set; }
    }
}