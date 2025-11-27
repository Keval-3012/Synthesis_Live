using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SyntesisApi.Models
{
    public class WebCamCamera
    {
        public string Location { get; set; }
        public string CameraName { get; set; }
        public int StoreId { get; set; }
    }

    public class WebCamCameraHistory
    {
        public string WebCamCameraListID { get; set; }
        public DateTime? RecordingDate { get; set; }
        public string RecordingStartTime { get; set; }
        public string RecordingEndTime { get; set;}
        public string FileName { get; set;}
        public int EndHour { get; set; }
        public string FolderName { get; set; }
    }
}