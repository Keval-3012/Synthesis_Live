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
    public class VendorProductDetailsController : ApiController
    {
        // GET: VendorProductDetails
        DataTable Dt = new DataTable();

        [HttpPost]
        [Route("api/VendorProductDetails/GetVendorProductDetails")]
        public IHttpActionResult GetVendorProductDetails(string VendorName = "", int PageNumber = 1)
        {
            Dt = new BALAccount().GetVendorProductDetails(VendorName,PageNumber);
            return Ok(Dt);
        }
    }
}