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
    public class ProductDetailsController : ApiController
    {
        // GET: ProductDetails
        DataTable Dt = new DataTable();

        [HttpPost]
        [Route("api/ProductDetails/GetProductDetails")]
        public IHttpActionResult GetProductDetails(int PageNumber = 1)
        {
            Dt = new BALAccount().GetProductDetails(PageNumber);
            return Ok(Dt);
        }
    }
}