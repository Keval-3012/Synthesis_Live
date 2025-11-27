using EntityModels.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SysnthesisRepo.Areas.HR.Controllers
{
    public class LoginController : Controller
    {
        // GET: HR/HRLogin
        public ActionResult Index()
        
        {
            Admin_Login admin_Login = new Admin_Login();
            return View(admin_Login);
        }
    }
}