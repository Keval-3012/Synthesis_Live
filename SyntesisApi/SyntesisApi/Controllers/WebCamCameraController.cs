using SyntesisApi.BAL;
using SyntesisApi.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web.Http;

namespace SyntesisApi.Controllers
{
    public class WebCamCameraController : ApiController
    {
        DataTable dt = new DataTable();

        [HttpPost]
        [Route("api/WebCamCamera/GetStore")]
        public IHttpActionResult GetStore([FromBody] WebCamCamera Web)
        {
            int StoreId = new BALWebCam().GetStore(Web.Location);
            return Ok(StoreId);
        }

        [HttpPost]
        [Route("api/WebCamCamera/InsertCamera")]
        public IHttpActionResult InsertCamera([FromBody] WebCamCamera Web)
        {
            int Id = new BALWebCam().InsertCamera(Web);
            return Ok(Id);
        }

        [HttpPost]
        [Route("api/WebCamCamera/InsertHistory")]
        public IHttpActionResult InsertHistory([FromBody] WebCamCameraHistory Web)
        {
            int Id = new BALWebCam().InsertHistory(Web);
            return Ok(Id);
        }

        [HttpPost]
        [Route("api/WebCamCamera/UpdateHistoryDownload")]
        public IHttpActionResult UpdateHistoryDownload([FromBody] int WebcamRecordingHistoryID)
        {
            int Id = new BALWebCam().UpdateHistoryDownload(WebcamRecordingHistoryID);
            return Ok(Id);
        }

        [HttpPost]
        [Route("api/WebCamCamera/UpdateHistoryUpload")]
        public IHttpActionResult UpdateHistoryUpload([FromBody] int WebcamRecordingHistoryID)
        {
            int Id = new BALWebCam().UpdateHistoryUpload(WebcamRecordingHistoryID);
            return Ok(Id);
        }

        [HttpPost]
        [Route("api/WebCamCamera/GetWebCamera")]
        public DataTable GetWebCamera([FromBody] int StoreId)
        {
            dt = new BALWebCam().GetWebCamera(StoreId);
            return dt;
        }

        [HttpPost]
        [Route("api/WebCamCamera/GetWebCamHistory")]
        public DataTable GetWebCamHistory([FromBody] WebCamCameraHistory Web)
        {
            dt = new BALWebCam().GetWebCamHistory(Web);
            return dt;
        }

        [HttpPost]
        [Route("api/WebCamCamera/GetWebCamHistoryList")]
        public DataTable GetWebCamHistoryList([FromBody] WebCamCameraHistory Web)
        {
            dt = new BALWebCam().GetWebCamHistoryList(Web);
            return dt;
        }

        [HttpPost]
        [Route("api/WebCamCamera/GetWebCamHistoryList1")]
        public DataTable GetWebCamHistoryList1([FromBody] WebCamCameraHistory Web)
        {
            dt = new BALWebCam().GetWebCamHistoryList1(Web);
            return dt;
        }

    }
}
