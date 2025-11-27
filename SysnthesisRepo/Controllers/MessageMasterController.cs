using EntityModels.Models;
using Repository;
using Repository.IRepository;
using SynthesisViewModal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SysnthesisRepo.Controllers
{
    public class MessageMasterController : Controller
    {
        // GET: MessageMaster
        private readonly IMessageMasterRepository _messageMasterRepository;
        public MessageMasterController()
        {
            this._messageMasterRepository = new MessageMasterRepository(new DBContext());
        }
        /// <summary>
        /// This method is Get Message mater list.
        /// </summary>
        /// <returns></returns>
        public ActionResult MessageMasterList()
        {
            //Db class is use for Get message list.
            var messagelist = _messageMasterRepository.GetMessageList();
            //Get Module Name.
            ViewBag.disinctmodule = _messageMasterRepository.GetModuleName();
            return View(messagelist);
        }

        /// <summary>
        /// This method is save meassage details.
        /// </summary>
        /// <param name="messageid"></param>
        /// <param name="keystr"></param>
        /// <param name="valueText"></param>
        /// <param name="description"></param>
        /// <param name="moduleName"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult SaveMessageMaster(int messageid, string keystr, string valueText, string description, string moduleName)
        {
            //Save Message master.
            _messageMasterRepository.SaveMessageMaster(messageid, keystr, valueText, description, moduleName);
            return Json(new { success = true, message = "Data saved successfully" });
        }
    }
}