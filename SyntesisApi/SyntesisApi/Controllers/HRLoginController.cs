using SyntesisApi.BAL;
using SyntesisApi.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Services.Description;
using NLog;
using System.Web.Script.Serialization;
using static SyntesisApi.Models.ApiModel;
namespace SyntesisApi.Controllers
{
    public class HRLoginController : ApiController
    {
        DataTable Dt1 = new DataTable();
        DataTable Dt2 = new DataTable();
        ResponseTokenModel Response = new ResponseTokenModel();
        private static Logger logger = LogManager.GetCurrentClassLogger();

        [HttpPost]
        [Route("api/HRLogin/GetUserLogDetails")]
        public async Task<IHttpActionResult> GetUserLogDetails([FromBody] CheckLogin objLogin)
        {
            Response = new ResponseTokenModel();
            try
            {
                var json = new JavaScriptSerializer().Serialize(objLogin);
                logger.Info("HRLoginController - GetUserLogDetails - " + DateTime.Now.ToString() + " - " +json);
                string url = HttpContext.Current.Request.Url.ToString().Replace("/api/HRLogin/GetUserLogDetails","");
                string AccessTokan = await Common.PosttData(objLogin.UserName, objLogin.Password, url);

                Dt1 = new BALHRLogin().CheckUserLogin(objLogin);
                if (Dt1.Rows.Count > 0) 
                {
                    if (Dt1.Rows[0]["UserId"].ToString() != "0")
                    {
                        Dt2 = new BALHRLogin().GetUserDetail(Convert.ToInt32(Dt1.Rows[0]["UserId"]), Dt1.Rows[0]["UserType"].ToString());
                        Response.responseStatus = "200";
                        Response.responseData = Dt2;
                        Response.message = Dt1.Rows[0]["Message"].ToString();
                        Response.Token = AccessTokan;
                        logger.Info("HRLoginController - GetUserLogDetails - " + DateTime.Now.ToString() + " - " + Dt1.Rows[0]["Message"].ToString());
                    }
                    else
                    {
                        Response.responseStatus = "400";
                        Response.responseData = null;
                        Response.message = Dt1.Rows[0]["Message"].ToString();
                        Response.Token = AccessTokan;
                        logger.Error("HRLoginController - GetUserLogDetails - " + DateTime.Now.ToString() + " - " + Dt1.Rows[0]["Message"].ToString());
                    }
                }
                else
                {
                    Response.responseStatus = "400";
                    Response.responseData = null;
                    Response.message = "Someting Went Wrong!";
                    Response.Token = AccessTokan;
                    logger.Error("HRLoginController - GetUserLogDetails - " + DateTime.Now.ToString() + " - " + "Someting Went Wrong!");
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                Response.responseStatus = "400";
                Response.responseData = null;
                Response.message = ex.Message;
                Response.Token = null;
                logger.Error("HRLoginController - GetUserLogDetails - " + DateTime.Now.ToString() + " - " + ex.Message);
                return Ok(Response);
            }
        }

        [HttpPost]
        [Route("api/HRLogin/GetUserLogProducts")]
        public async Task<IHttpActionResult> GetUserLogProducts([FromBody] CheckLogin objLogin)
        {
            ResponseTokenModelUserDetail Response = new ResponseTokenModelUserDetail();
            try
            {
                var json = new JavaScriptSerializer().Serialize(objLogin);
                logger.Info("HRLoginController - GetUserLogProducts - " + DateTime.Now.ToString() + " - " + json);
                string url = HttpContext.Current.Request.Url.ToString().Replace("/api/HRLogin/GetUserLogProducts", "");
                string AccessTokan = await Common.PosttData2(objLogin.UserName, objLogin.Password, url);
                Dt1 = new BALHRLogin().CheckProductUserLogin(objLogin);
                if (Dt1.Rows.Count > 0)
                {
                    if (Dt1.Rows[0]["UserId"].ToString() != "0")
                    {
                        UserDetail user = new BALHRLogin().GetUserDetailProducts(Convert.ToInt32(Dt1.Rows[0]["UserId"]), Dt1.Rows[0]["UserType"].ToString());
                        user.ModuleAccess = new ModuleAccess();
                        Response.responseStatus = "200";
                        Response.responseData = user;
                        Response.message = Dt1.Rows[0]["Message"].ToString();
                        Response.Token = AccessTokan;
                        logger.Info("HRLoginController - GetUserLogProducts - " + DateTime.Now.ToString() + " - " + Dt1.Rows[0]["Message"].ToString());
                    }
                    else
                    {
                        Response.responseStatus = "400";
                        Response.responseData = null;
                        Response.message = Dt1.Rows[0]["Message"].ToString();
                        logger.Info("HRLoginController - GetUserLogProducts - " + DateTime.Now.ToString() + " - " + Dt1.Rows[0]["Message"].ToString());
                    }
                }
                else
                {
                    Response.responseStatus = "400";
                    Response.responseData = null;
                    Response.message = "Someting Went Wrong!";
                    Response.Token = AccessTokan;
                    logger.Info("HRLoginController - GetUserLogProducts - " + DateTime.Now.ToString() + " - " + Dt1.Rows[0]["Message"].ToString());
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                Response.responseStatus = "400";
                Response.responseData = null;
                Response.message = ex.Message;
                Response.Token = null;
                logger.Error("HRLoginController - GetUserLogProducts - " + DateTime.Now.ToString() + " - " + ex.Message);
                return Ok(Response);
            }
        }
    }
}
