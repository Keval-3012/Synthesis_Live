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
    public class PurchaseController : ApiController
    {
        // GET: Purchase
        DataTable Dt = new DataTable();

        [HttpPost]
        [Route("api/Purchase/GetPurchase")]
        public IHttpActionResult GetPurchase(string StoreName = "", string VendorName = "", string StartDate = "", string EndDate = "", int PageNumber = 1)
        {
            int StoreId = new BALAccount().GetStoreIDbyName(StoreName);
            int VendorID = 0;
            if (StoreId > 0)
            {
                VendorID = new BALAccount().GetVendorIDbyName(VendorName, StoreId);
            }
            if (StoreId == 0)
            {
                return Content(HttpStatusCode.BadRequest, "Entered Store Name Is Invalid!");
            }
            else if (VendorID == 0)
            {
                return Content(HttpStatusCode.BadRequest, "Entered Vendor Name Is Invalid!");
            }
            else if (StartDate == "" || EndDate == "" || Convert.ToDateTime(StartDate) > Convert.ToDateTime(EndDate))
            {
                return Content(HttpStatusCode.BadRequest, "Entered StartDate and EndDate is Invalid!");
            }
            else
            {
                Dt = new BALAccount().GetPurchase(StoreId, VendorID, StartDate, EndDate, PageNumber);
                return Ok(Dt);
            }
        }
    }
}