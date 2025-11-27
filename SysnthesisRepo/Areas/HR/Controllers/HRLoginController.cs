using Aspose.Pdf.Operators;
using EntityModels.HRModels;
using EntityModels.Models;
using NLog;
using Repository;
using Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Filters;
using System.Web.Mvc;

namespace SysnthesisRepo.Areas.HR.Controllers
{
    public class HRLoginController : Controller
    {
        // GET: HR/HRLogin
        protected static string StatusMessage = "";
        private readonly IUserMastersRepository _iUserMastersRepository;
        private readonly IUserActivityLogRepository _activityLogRepository;
        private readonly ISynthesisApiRepository _synthesisApiRepository;
        private readonly ICommonRepository _commonRepository;
        private readonly ILoginRepository _loginRepository;
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public HRLoginController()
        {
            this._iUserMastersRepository = new UserMastersRepository(new DBContext());
            this._activityLogRepository = new UserActivityLogRepository(new DBContext());
            this._synthesisApiRepository = new SynthesisApiRepository();
            this._commonRepository = new CommonRepository(new DBContext());
            this._loginRepository = new LoginRepository(new DBContext());
        }
        public ActionResult Index()
        {
            return View();
        }

        
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Index(HREmployeeMaster model, string ReturnUrl = "")
        {
            try
            {
                logger.Info("HRLoginController - Index - " + DateTime.Now + " - " + model.EmployeeUserName);

                var data = _loginRepository.CheckLogin(model.EmployeeUserName, model.Password);
                if (data == "Success")
                {
                    var EmpData = _loginRepository.LoginEmployee(model.EmployeeUserName, model.Password);                    
                    string AccessTokan = await _synthesisApiRepository.PosttDataEmployeeLogin(model.EmployeeUserName, model.Password);
                    if (EmpData != null)
                    {
                        Session["AccessTokan"] = AccessTokan;
                        Session["EmployeeUserName"] = EmpData.EmployeeUserName;
                        Session["Password"] = EmpData.Password;
                        Session["HREmployeeId"] = EmpData.EmployeeId;
                        Session["IsFirstEmpLogin"] = EmpData.IsFirstLogin;
                        Session["FirstName"] = EmpData.FirstName;
                        Session["LastName"] = EmpData.LastName;
                        //Session["LanguagePrefrence"] = EmpData.LanguageId;
                        Session["IsTraningCompleted"] = EmpData.IsTraningCompleted;
                        if (EmpData.LastSlidename == null) { EmpData.LastSlidename = ""; }
                        Session["LastSlideName"] = EmpData.LastSlidename;
                        if (EmpData.LastSlidename.ToLower().Contains("sp"))
                        {
                            Session["LanguagePrefrence"] = "2";
                        }
                        else
                        {
                            Session["LanguagePrefrence"] = EmpData.LanguageId;
                        }

                        //if (EmpData.IsFirstLogin == true)
                        //{
                        //    return RedirectToAction("ModifyPassword", "HRLogin");
                        //}
                        if(EmpData.IsLanguageSelect == false)
                        {
                            return RedirectToAction("LanguageSelection", "HREmployeeTraining");
                        }
                        else
                        {
                            if (EmpData.LastSlidename.ToLower().Contains("sp"))
                            {
                                return RedirectToAction("SpanishTraining", "HREmployeeTraining");
                            }
                            else
                            {
                                return RedirectToAction("EnglishTraining", "HREmployeeTraining");
                            }
                        }
                    }
                }
                else
                {
                    //ViewBag.Message = data;
                    ModelState.AddModelError("Message", data);
                    return View();
                }

            }
            catch (Exception ex)
            {
                logger.Error("HRLoginController - Index - " + DateTime.Now + " - " + ex.Message.ToString() + " - " + model.EmployeeUserName);
            }
            return View(model);
        }
        
        public ActionResult ModifyPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [SessionExpireAttribute]
        public ActionResult ModifyPassword(HREmployeeMaster model)
        {
            try
            {
                logger.Info("HRLoginController - ModifyPassword - " + DateTime.Now + " - " + model.EmployeeUserName);
                string sessionEId = Session["HREmployeeId"].ToString();
                if (sessionEId != null && sessionEId != "")
                {
                    if (model.Password == null)
                    {
                        ModelState.AddModelError("Password", "Please Enter Password");
                        return View();
                    }
                    else if (model.ConfirmPassword != model.Password)
                    {
                        ModelState.AddModelError("ConfirmPassword", "Please Enter Password");
                        return View();
                    }
                    else
                    {
                        _loginRepository.ModifyPasswordUpdate(Convert.ToInt32(sessionEId),model.Password);
                        return RedirectToAction("EnglishTraining", "HREmployeeTraining");
                    }
                }
                else
                {
                    return RedirectToAction("Index", "HRLogin");
                }
            }
            catch (Exception ex)
            {
                logger.Error("HRLoginController - ModifyPassword - " + DateTime.Now + " - " + ex.Message.ToString() + model.EmployeeUserName);
            }
            return View();
        }
    }
}