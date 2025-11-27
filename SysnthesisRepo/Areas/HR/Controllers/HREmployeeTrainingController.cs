using Aspose.Pdf.Operators;
using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.EMMA;
using EntityModels.HRModels;
using EntityModels.Models;
using Newtonsoft.Json;
using NLog;
using Repository;
using Repository.IRepository;
using SynthesisViewModal.HRViewModel;
using SysnthesisRepo.Areas.HR;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Razor.Tokenizer;

namespace SysnthesisRepo.Areas.HR.Controllers
{
    public class HREmployeeTrainingController : Controller
    {
        Logger logger = LogManager.GetCurrentClassLogger();
        string UserName = System.Web.HttpContext.Current.User.Identity.Name;
        private readonly ICommonRepository _commonRepository;
        private readonly IHREmployeeTrainingRepository _hremployeeTrainingRepository;
        private readonly ILoginRepository _loginRepository;
        private readonly ISynthesisApiRepository _synthesisApiRepository;

        public HREmployeeTrainingController()
        {
            this._commonRepository = new CommonRepository(new DBContext());
            this._hremployeeTrainingRepository = new HREmployeeTrainingRepository(new DBContext());
            this._loginRepository = new LoginRepository(new DBContext());
            this._synthesisApiRepository = new SynthesisApiRepository();
        }

        [SessionExpireAttribute]
        public ActionResult EnglishTraining()
        {
            try
            {
                logger.Info("HREmployeeTrainingController - EnglishTraining - " + DateTime.Now + " - " + Session["HREmployeeId"]);
                string sessionEId = Session["HREmployeeId"].ToString();
                //string LanguageId = Session["IsLanguageSelect"].ToString();                
                if (sessionEId != null && sessionEId != "")
                {
                    if (Session["LanguagePrefrence"].ToString() == "2")
                    {
                        return RedirectToAction("SpanishTraining", "HREmployeeTraining");
                    }
                    else
                    {
                        return View();
                    }
                }
                else
                {
                    return RedirectToAction("Index", "HRLogin");
                }
            }
            catch (Exception ex)
            {
                logger.Error("HREmployeeTrainingController - EnglishTraining - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return View();
        }

        [SessionExpireAttribute]
        public ActionResult SpanishTraining(HREmployeeMaster model)
        {
            try
            {
                logger.Info("HREmployeeTrainingController - SpanishTraining - " + DateTime.Now + " - " + model.EmployeeUserName);
                string sessionEId = Session["HREmployeeId"].ToString();
                model.ModifiedOn = DateTime.Now;
                if (sessionEId != null && sessionEId != "")
                {
                    var data = _loginRepository.CheckLogin(model.EmployeeUserName, model.Password);
                    if (data == "Success")
                    {
                        return RedirectToAction("ModifyPassword", "HRLogin");
                    }
                    else
                    {
                        return View();
                    }
                }
                else
                {
                    return RedirectToAction("Index", "HRLogin");
                }
            }
            catch (Exception ex)
            {
                logger.Error("HREmployeeTrainingController - SpanishTraining - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return View();
        }

        public ActionResult InitialChildForm()
        {
            try
            {
                ModelState.Clear();
                var List = new List<SelectListItem>
                        {
                             new SelectListItem{ Text="English (United States)", Value = "1" },
                             new SelectListItem{ Text="Spanish", Value = "2" },
                        };
                ViewBag.LanguageId = new SelectList(List, "Value", "Text", Session["LanguagePrefrence"].ToString());
                //ViewBag.LanguageId = new SelectList(new[] { new EnumDropDown { value = 0, text = "-- Select Language --" } }
                //                           .Concat(from Language e in Enum.GetValues(typeof(Language)) select new EnumDropDown { value = (int)e, text = e.ToString() }), "value", "text");
            }
            catch (Exception ex)
            {
                logger.Error("HREmployeeTrainingController - InitialChildForm - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return PartialView("_ChangeLanguage");
        }

        [SessionExpireAttribute]
        public ActionResult LanguageSelection()
        {
            try
            {
                string sessionEId = Session["HREmployeeId"].ToString();
                logger.Info("HREmployeeTrainingController - LanguageSelection - " + DateTime.Now + " - " + sessionEId);
                if (sessionEId != null && sessionEId != "")
                {
                    return View();
                }
                else
                {
                    return RedirectToAction("Index", "HRLogin");
                }
            }
            catch (Exception ex)
            {
                logger.Error("HREmployeeTrainingController - LanguageSelection - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return View();
        }

        [SessionExpireAttribute]
        public ActionResult SaveLanguageSelection(string LanguageId)
        {
            int sessionEId = Convert.ToInt32(Session["HREmployeeId"]);
            logger.Info("HREmployeeTrainingController - SaveLanguageSelection - " + DateTime.Now + " - " + sessionEId);
            //string Flag = "";
            if (sessionEId != 0)
            {
                var objEmp = _hremployeeTrainingRepository.HREmployeeMaster(sessionEId);
                if (objEmp != null)
                {
                    Session["LastSlideName"] = objEmp.LastSlidename;
                    Session["LanguagePrefrence"] = LanguageId;
                    objEmp.LanguageId = (Language)Convert.ToInt32(LanguageId);
                    objEmp.IsLanguageSelect = true;
                    objEmp.ModifiedOn = DateTime.Now;
                    _hremployeeTrainingRepository.SaveLanguageselection(objEmp);
                    return RedirectToAction("EnglishTraining", "HREmployeeTraining");
                }
                else
                {
                    ViewBag.Message = "No account exists for this user. Please contact the System Administrator";
                    return View("LanguageSelection", "HREmployeeTraining");
                }
            }
            else
            {
                return RedirectToAction("Login", "HRLogin");
            }
        }

        [SessionExpireAttribute]
        [HttpGet]
        public ActionResult UpdateLastSlideTrainig(string LastSlideName)
        {
            int sessionEId = Convert.ToInt32(Session["HREmployeeId"]);
            logger.Info("HREmployeeTrainingController - UpdateLastSlideTrainig - " + DateTime.Now + " - " + sessionEId);
            //string Flag = "";
            if (sessionEId != 0)
            {
                _hremployeeTrainingRepository.UpdateLastSlide(sessionEId, LastSlideName);
            }
            else
            {
                return RedirectToAction("Login", "HRLogin");
            }
            return Json("Ok", JsonRequestBehavior.AllowGet);
        }

        [SessionExpireAttribute]
        [HttpPost]
        public async Task<ActionResult> CompleteTraining(CompleteyTraining data)
        {
            logger.Info("HREmployeeTrainingController - CompleteTraining - " + DateTime.Now + "-" + data.EmployeeId);
            int sessionEId = Convert.ToInt32(Session["HREmployeeId"]);
            string FileName = "";
            //string Flag = "";
            if (sessionEId != 0)
            {
                data.EmployeeId = sessionEId;
                string Response = await _synthesisApiRepository.CompleteTraining(data);
                ResponseModel response = (ResponseModel)JsonConvert.DeserializeObject(Response, (typeof(ResponseModel)));
                string URL = "";
                if (response != null)
                {
                    URL = response.responseData.Rows[0]["URL"].ToString();
                }
                FileName = System.IO.Path.GetFileName(URL);
            }
            else
            {
                return RedirectToAction("Login", "HRLogin");
            }
            return Json(FileName, JsonRequestBehavior.AllowGet);
        }

        [SessionExpireAttribute]
        [HttpPost]
        public async Task<ActionResult> ResetTraining(ResetTraining data)
        {

            int sessionEId = Convert.ToInt32(Session["HREmployeeId"]);
            logger.Info("HREmployeeTrainingController - ResetTraining - " + DateTime.Now + " - " + data.EmployeeId);
            string StatusCode = "";
            if (sessionEId != 0)
            {
                data.EmployeeId = sessionEId;
                string Response = await _synthesisApiRepository.ResetTraining(data);
                ResponseModel response = (ResponseModel)JsonConvert.DeserializeObject(Response, (typeof(ResponseModel)));
                
                if (response != null)
                {
                    StatusCode = response.responseStatus.ToString();
                    if (StatusCode == "200")
                    {
                        Session["IsTraningCompleted"] = false;
                        Session["LastSlideName"] = "";
                       // return RedirectToAction("EnglishTraining", "HREmployeeTraining");
                    }
                }
            }
            else
            {
                return RedirectToAction("Login", "HRLogin");
            }
            return Json(StatusCode, JsonRequestBehavior.AllowGet);
        }

    }
}