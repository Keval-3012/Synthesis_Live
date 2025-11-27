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
    public class HRStoreListController : ApiController
    {
        DataTable Dt1 = new DataTable();
        ResponseModel Response = new ResponseModel();
        private static Logger logger = LogManager.GetCurrentClassLogger();

        [Authorize]
        [HttpGet]
        [Route("api/HRStoreList/GetStoreList")]
        public async Task<IHttpActionResult> GetStoreList()
        {
            Dt1 = new DataTable();
            try
            {
                logger.Info("HRStoreListController - GetStoreList - " + DateTime.Now.ToString());
                Dt1 = new BALHRStore().GetStoreList();
                if (Dt1.Rows.Count > 0)
                {
                    Response.responseStatus = "200";
                    Response.responseData = Dt1;
                    Response.message = "Successfully!!";
                    logger.Info("HRStoreListController - GetStoreList - " + DateTime.Now.ToString() + " - " + "Successfully!!");
                }
                else
                {
                    Response.responseStatus = "400";
                    Response.responseData = null;
                    Response.message = "Someting Went Wrong!";
                    logger.Error("HRStoreListController - GetStoreList - " + DateTime.Now.ToString() + " - " + "Someting Went Wrong!");
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                Response.responseStatus = "400";
                Response.responseData = null;
                Response.message = ex.Message;
                logger.Error("HRStoreListController - GetStoreList - " + DateTime.Now.ToString() + " - " + ex.Message);
                return Ok(Response);
            }
        }

        [HttpGet]
        [Route("api/HRStoreList/GetStoreListData")]
        public async Task<IHttpActionResult> GetStoreListData(int UserId)
        {
            Dt1 = new DataTable();
            try
            {
                logger.Info("HRStoreListController - GetStoreListData - " + DateTime.Now.ToString());
                Dt1 = new BALHRStore().GetStoreListData(UserId);
                if (Dt1.Rows.Count > 0)
                {
                    Response.responseStatus = "200";
                    Response.responseData = Dt1;
                    Response.message = "Successfully!!";
                    logger.Info("HRStoreListController - GetStoreListData - " + DateTime.Now.ToString() + " - " + "Successfully!!");
                }
                else
                {
                    Response.responseStatus = "400";
                    Response.responseData = null;
                    Response.message = "Someting Went Wrong!";
                    logger.Error("HRStoreListController - GetStoreListData - " + DateTime.Now.ToString() + " - " + "Someting Went Wrong!");
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                Response.responseStatus = "400";
                Response.responseData = null;
                Response.message = ex.Message;
                logger.Error("HRStoreListController - GetStoreListData - " + DateTime.Now.ToString() + " - " + ex.Message);
                return Ok(Response);
            }
        }
    }
}
