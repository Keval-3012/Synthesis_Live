using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Data;

namespace EntityModels.Models
{
    [Table("WebcamRecordingHistory")]
    public class WebcamRecordingHistory
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Editable(false)]
        public int WebcamRecordingHistoryID { get; set; }

        public int WebCamCameraListID { get; set; }
        [ForeignKey("WebCamCameraListID")]
        public virtual WebCamCameraList GetWebCamCameraList { get; set; }

        public DateTime RecordingDate { get; set; }
        [MaxLength(50),Required]
        public string RecordingStartTime { get; set; }
        [MaxLength(50), Required]
        public string RecordingEndTime { get; set; }
        [MaxLength(2000)]
        public string FileName { get; set; }
        public int IsArchive { get; set; }
        public int? IsDownload { get; set; }
        public int? IsUploaded { get; set; }
        public DateTime  CreateDate { get; set; }
        public DateTime? DownloadDate { get; set; }
        public DateTime? UploadDate { get; set; }
        public string FolderName { get; set; }
        public int? IsDeleted { get; set; }
        public DateTime? DeletedDate { get; set; }
        public int EndHour { get; set; }

    }
    public class ListWebcamRecordingHistory
    {
        public int ID { get; set; } 
        public string StoreName { get; set; }
        public string FileUrl { get; set; }
        public DateTime? Date { get; set; }
        public int? StoreId { get; set; }
        public int IsUploaded { get; set; }
        public string Time { get; set; }
        public string ArchiveStartedOn { get; set; }
        public string SaveToS3 { get; set; }
        public int WebcamRecordingHistoryID { get; set; }
        public string CameraName { get; set; }
    }
    public class CameraDropDown
    {
        public int value { get; set; } 
        public string text { get; set; }
    }
}