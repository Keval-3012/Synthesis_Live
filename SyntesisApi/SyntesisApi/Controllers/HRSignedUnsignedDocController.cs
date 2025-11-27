using NLog;
using SyntesisApi.BAL;
using SyntesisApi.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using static SyntesisApi.Models.ApiModel;

namespace SyntesisApi.Controllers
{
    public class HRSignedUnsignedDocController : ApiController
    {
        DataTable Dt1 = new DataTable();
        ResponseModel Response = new ResponseModel();
        private static Logger logger = LogManager.GetCurrentClassLogger();
        #region Start Harsh 11-03-2024
        [Authorize]
        [HttpGet]
        [Route("api/HRSignedUnsignedDoc/GetSignedUnSignedDocument")]
        public async Task<IHttpActionResult> GetSignedUnSignedDocument([FromBody] Store obj)
        {
            Dt1 = new DataTable();
            try
            {
                logger.Info("HRSignedUnsignedDocController - GetSignedUnSignedDocument - " + DateTime.Now.ToString() + " - " + obj.StoreID.ToString());
                Dt1 = new BALHREmployee().GetSignedUnsignedDoc(obj);
                if (Dt1.Rows.Count > 0)
                {
                    Response.responseStatus = "200";
                    Response.responseData = Dt1;
                    Response.message = "Successfully!!";
                    logger.Info("HRSignedUnsignedDocController - GetSignedUnSignedDocument - " + DateTime.Now.ToString() + " - " + "Successfully!!");
                }
                else
                {
                    Response.responseStatus = "200";
                    Response.responseData = null;
                    Response.message = "No Record Found!";
                    logger.Error("HRSignedUnsignedDocController - GetSignedUnSignedDocument - " + DateTime.Now.ToString() + " - " + "No Record Found!");
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                Response.responseStatus = "400";
                Response.responseData = null;
                Response.message = ex.Message;
                logger.Error("HRSignedUnsignedDocController - GetSignedUnSignedDocument - " + DateTime.Now.ToString() + " - " + ex.Message);
                return Ok(Response);
            }
        }
        #endregion
    }
}
