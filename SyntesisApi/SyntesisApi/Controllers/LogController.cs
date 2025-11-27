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
    public class LogController : ApiController
    {

        DataTable Dt = new DataTable();

        [Authorize]
        [HttpPost]
        [Route("api/Log/Write")]
        public IHttpActionResult WriteLog([FromBody] LogMaster log)
        {
            var identity = (ClaimsIdentity)User.Identity;
            var UserID = identity.Claims
                      .FirstOrDefault(c => c.Type == "UserId").Value;
            BALLog ballog = new BALLog();
            ballog.WriteLog(log);
            return Ok("Done");
        }


        [Authorize]
        [HttpPost]
        [Route("api/Log/GetUserLogDetails")]

        public DataTable GetUserLogDetails(UserLogDetails log)
        {
            Dt = new BALAccount().GetUserLogDetails(log);
            return Dt;
        }

        [Authorize]
        [HttpPost]
        [Route("api/Log/GetUserName")]
        public DataTable GetUserName()
        {
            Dt = new BALAccount().GetUser();
            return Dt;
        }


        [Authorize]
        [HttpPost]
        [Route("api/Log/GetModuleandActionName")]
        public DataSet GetModuleandActionName()
        {
            DataSet Ds = new DataSet();
            Ds = new BALAccount().GetModuleandAction();
            return Ds;
        }
        [Authorize]
        [HttpPost]
        [Route("api/Log/GetuserDetailsNew")]
        public DataTable GetuserDetailsNew(UserLogDetails log)
        {
            DataTable Ds = new DataTable();
            Ds = new BALAccount().GetuserDetailsNew(log);
            return Ds;
        }
        [Authorize]
        [HttpPost]
        [Route("api/Log/GetuserDetailsNewChild")]
        public DataTable GetuserDetailsNewChild(UserLogDetails log)
        {
            DataTable Ds = new DataTable();
            Ds = new BALAccount().GetuserDetailsNewChild(log);
            return Ds;
        }
    }
}
