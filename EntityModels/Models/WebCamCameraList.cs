using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EntityModels.Models
{
    public class WebCamCameraList
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Editable(false)]
        public int WebCamCameraListId { get; set; }

        [MaxLength(500),Required]
        public string CameraName { get; set;}

        public int StoreId { get; set; }
        [ForeignKey("StoreId")]
        public virtual StoreMaster StoreMasters { get; set; }

        public virtual ICollection<WebcamRecordingHistory> WebcamRecordingHistory { get; set; }

    }
}