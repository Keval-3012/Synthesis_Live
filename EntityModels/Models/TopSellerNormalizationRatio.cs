using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace EntityModels.Models
{
    [Table("TopSellerNormalizationRatio")]
    public class TopSellerNormalizationRatio
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Editable(false)]

        public int TopSellerNormalizationID { get; set; }
        public int storeID { get; set; }
        public decimal Normalizationvalue { get; set; }

    }
}