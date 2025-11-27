using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntityModels.Models
{
    [Table("WeeklyPeriod")]
    public class WeeklyPeriod
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Editable(false)]
        public int WeeklyPeriodId { get; set; }

        [Required(ErrorMessage = "Required..")]
        [MaxLength(100)]
        public string WeekNo { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public int Year { get; set; }

        public int WNumber { get; set; }
    }

}