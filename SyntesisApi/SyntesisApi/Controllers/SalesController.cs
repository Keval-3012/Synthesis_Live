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
    public class SalesController : ApiController
    {
        // GET: Sales
        DataTable Dt = new DataTable();

        [HttpPost]
        [Route("api/Sales/GetSales")]
        public IHttpActionResult GetSales(string StoreName = "", string StartDate ="", string EndDate = "", int PageNumber = 1)
        {
            int StoreId = new BALAccount().GetStoreIDbyName(StoreName);
            if (StoreId == 0)
            {
                return Content(HttpStatusCode.BadRequest, "Entered Store Name Is Invalid!");
            }
            else if(StartDate == "" || EndDate == "" || Convert.ToDateTime(StartDate) > Convert.ToDateTime(EndDate))
            {
                return Content(HttpStatusCode.BadRequest, "Entered StartDate and EndDate is Invalid!");
            }
            else
            {
                Dt = new BALAccount().GetSales(StoreId, StartDate, EndDate, PageNumber);
                return Ok(Dt);
            }
        }
    }
}