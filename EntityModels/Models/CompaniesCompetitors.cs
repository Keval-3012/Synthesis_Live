using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntityModels.Models
{
    [Table("CompaniesCompetitors")]
    public class CompaniesCompetitors
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Editable(false)]
        public int CompaniesCompetitorsId { get; set; }

        [MaxLength(200)]
        public string CompetitorsName { get; set; }
        
        public string CompetitorsNickName { get; set; }

        public string ZipCode { get; set; }
        
        public string CompetitorsStoreId { get; set; }

        public string ImageURL { get; set; }
    }

    public class CompaniesCompetitorsList
    {
        public int CompaniesCompetitorsId { get; set; }
        public string CompetitorsName { get; set; }
    }
}