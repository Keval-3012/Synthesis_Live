using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace EntityModels.Models
{
    [Table("QuickBooksStorewiseToken")]
    public class QuickBooksStorewiseToken
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Editable(false)]
        public int QuickBooksStorewiseTokenId { get; set; }

        public int StoreID { get; set; }
        
        public string ClientID { get; set; }

        public string ClientSecret { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}