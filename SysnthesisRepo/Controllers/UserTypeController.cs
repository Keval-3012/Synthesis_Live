using EntityModels.Models;
using NLog;
using Repository;
using Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Utility;

namespace SysnthesisRepo.Controllers
{
    public class UserTypeController : Controller
    {
        private readonly IUserTypeRepository _userTypeRepository;
        Logger logger = LogManager.GetCurrentClassLogger();
        public UserTypeController()
        {
            this._userTypeRepository = new UserTypeRepository(new DBContext());
        }

        /// <summary>
        /// This method return index view.
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// This method return create view.
        /// </summary>
        /// <returns></returns>
        public ActionResult Create()
        {
            logger.Info("Error while exporting QE reportlet from Dashboard -");
            UserTypeMaster obj = new UserTypeMaster();
            obj.IsActive = true;
            return View(obj);
        }

        /// <summary>
        /// This method is register user type.
        /// </summary>
        /// <param name="userTypeMaster"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(UserTypeMaster userTypeMaster)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    //Register user type.
                    await _userTypeRepository.Register(userTypeMaster);
                    string mesg = CommonMsg.Success;
                }
            }
            catch (Exception ex)
            {
                logger.Error("UserRolesController - Create - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            
            return RedirectToAction("Create");
        }
    }
}