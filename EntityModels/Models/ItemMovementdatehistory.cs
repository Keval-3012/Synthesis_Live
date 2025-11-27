using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace EntityModels.Models
{
    [Table("ItemMovementdatehistory")]
    public class ItemMovementdatehistory
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Editable(false)]
        public int ItemMovementdatehistoryID { get; set; }
        public int StoreID { get; set; }
        public DateTime Startdate { get; set; }
        public DateTime Enddate { get; set; }
        public string FileName { get; set; }
    }
    public class ItemMovementdatehistorySelect
    {
       
        public int ItemMovementdatehistoryID { get; set; }
        public string StoreName { get; set; }
        public DateTime Startdate { get; set; }
        public DateTime Enddate { get; set; }
        public string FileName { get; set; }
    }
}