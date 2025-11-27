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
    public class HREmployeeController : ApiController
    {
        DataTable Dt1 = new DataTable();
        ResponseModel Response = new ResponseModel();
        private static Logger logger = LogManager.GetCurrentClassLogger();
        
        // Updated by Dani on 09-04-2025
        [Authorize]
        [HttpGet]
        [Route("api/HREmployee/GetEmployeeList")]
        public async Task<IHttpActionResult> GetEmployeeList()
        {
            Dt1 = new DataTable();
            try
            {
                logger.Info("HREmployeeController - GetEmployeeList - " + DateTime.Now.ToString());
                Dt1 = new BALHREmployee().GetEmployeeList();
                if (Dt1.Rows.Count > 0)
                {
                    Response.responseStatus = "200";
                    Response.responseData = Dt1;
                    Response.message = "Successfully!!";
                    logger.Info("HREmployeeController - GetEmployeeList - " + DateTime.Now.ToString() + " - " + "Successfully!!");
                }
                else
                {
                    Response.responseStatus = "400";
                    Response.responseData = null;
                    Response.message = "Someting Went Wrong!";
                    logger.Error("HREmployeeController - GetEmployeeList - " + DateTime.Now.ToString() + " - " + "Someting Went Wrong!");
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                Response.responseStatus = "400";
                Response.responseData = null;
                Response.message = ex.Message;
                logger.Error("HREmployeeController - GetEmployeeList - " + DateTime.Now.ToString() + " - " + ex.Message);
                return Ok(Response);
            }
        }

        [Authorize]
        [HttpGet]
        [Route("api/HREmployee/GetEmployeeByStoreId")]
        public async Task<IHttpActionResult> GetEmployeeByStoreId([FromBody] StoreList obj)
        {
            Dt1 = new DataTable();
            try
            {
                logger.Info("HRStoreListController - GetEmployeeByStoreId - " + DateTime.Now.ToString() + " - " + obj.StoreId.ToString());
                Dt1 = new BALHREmployee().GetEmployeeByStoreId(obj.StoreId);
                if (Dt1.Rows.Count > 0)
                {
                    Response.responseStatus = "200";
                    Response.responseData = Dt1;
                    Response.message = "Successfully!!";
                    logger.Info("HRStoreListController - GetEmployeeByStoreId - " + DateTime.Now.ToString() + " - " + "Successfully!!");
                }
                else
                {
                    Response.responseStatus = "200";
                    Response.responseData = null;
                    Response.message = "No Record Found!";
                    logger.Error("HRStoreListController - GetEmployeeByStoreId - " + DateTime.Now.ToString() + " - " + "No Record Found!");
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                Response.responseStatus = "400";
                Response.responseData = null;
                Response.message = ex.Message;
                logger.Error("HRStoreListController - GetEmployeeByStoreId - " + DateTime.Now.ToString() + " - " + ex.Message);
                return Ok(Response);
            }
        }        
    }
}
