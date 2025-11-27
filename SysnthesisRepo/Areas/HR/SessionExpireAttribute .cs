using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SysnthesisRepo.Areas.HR
{
    public class SessionExpireAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            HttpContext ctx = HttpContext.Current;
            // check  sessions here
            if (HttpContext.Current.Session["HREmployeeId"] == null)
            {
                filterContext.Result = new RedirectResult("~/HR/HRLogin/Index");
                return;
            }
            base.OnActionExecuting(filterContext);
        }
    }
}