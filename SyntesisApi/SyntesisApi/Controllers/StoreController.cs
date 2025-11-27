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
    public class StoreController : ApiController
    {
        // GET: Store
        DataTable Dt = new DataTable();

        [HttpPost]
        [Route("api/Store/GetStore")]
        public IHttpActionResult GetStore()
        {
            Dt = new BALAccount().GetStore();
            return Ok(Dt);
        }
        [HttpPost]
        [Route("api/Store/GetVendorName")]
        public IHttpActionResult GetVendorName(string StoreName = "")
        {
            int StoreId = new BALAccount().GetStoreIDbyName(StoreName);
            if (StoreId == 0)
            {
                return Content(HttpStatusCode.BadRequest, "Entered Store Name Is Invalid!");
            }
            Dt = new BALAccount().GetVendorName(StoreId);
            return Ok(Dt);
        }
    }
}