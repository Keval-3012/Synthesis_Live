using Aspose.Pdf.Operators;
using EntityModels.HRModels;
using EntityModels.Models;
using Microsoft.Extensions.Logging;
using NLog;
using Repository;
using Repository.IRepository;
using Syncfusion.EJ2.Base;
using SynthesisViewModal.HRViewModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace SysnthesisRepo.Controllers
{
    public class HRStoreManagersController : Controller
    {
        private readonly IHRStoreManagersRepository _hRStoreManagersRepository;
        private readonly IMastersBindRepository _MastersBindRepository;
        private readonly IUserMastersRepository _UserMastersRepository;
        private readonly ICommonRepository _CommonRepository;
        Logger logger = LogManager.GetCurrentClassLogger();
        string SuccessMessage = string.Empty;
        string ErrorMessage = string.Empty;
        string UserName = System.Web.HttpContext.Current.User.Identity.Name;
        public HRStoreManagersController()
        {
            this._hRStoreManagersRepository = new HRStoreManagersRepository(new DBContext());
            this._MastersBindRepository = new MastersBindRepository(new DBContext());
            this._UserMastersRepository = new UserMastersRepository(new DBContext());
            this._CommonRepository = new CommonRepository(new DBContext());
        }
        public ActionResult Index()
        {
            ViewBag.Title = "Mobile App User - Synthesis";
            return View();
        }
        public ActionResult UrlDatasource(DataManagerRequest dm)
        {
            List<HRStoreList> HrDeVm = new List<HRStoreList>();
            IEnumerable DataSource = new List<HRStoreList>();
            int Count = 0;
            try
            {
                logger.Info("HRStoreManagersController - UrlDatasource - " + DateTime.Now);
                //Using this db class Get Web cam details by storeid.
                HrDeVm = _hRStoreManagersRepository.GetHRStoreManagerList();
                DataSource = HrDeVm;
                
                DataOperations operation = new DataOperations();
                if (dm.Search != null && dm.Search.Count > 0)
                {
                    string search = dm.Search[0].Key.ToString().Trim();
                    DataSource = HrDeVm.ToList().Where(x => x.FirstName.Contains(search) || x.UserName.Contains(search) || x.StoreName.Contains(search)).ToList();
                    //DataSource = operation.PerformSearching(DataSource, dm.Search);  //Search
                }
                if (dm.Sorted != null && dm.Sorted.Count > 0) //Sorting
                {
                    DataSource = operation.PerformSorting(DataSource, dm.Sorted);
                }
                if (dm.Where != null && dm.Where.Count > 0) //Filtering
                {
                    DataSource = operation.PerformFiltering(DataSource, dm.Where, dm.Where[0].Operator);
                }
                Count = DataSource.Cast<HRStoreList>().Count();
                if (dm.Skip != 0)
                {
                    DataSource = operation.PerformSkip(DataSource, dm.Skip);   //Paging
                }
                if (dm.Take != 0)
                {
                    DataSource = operation.PerformTake(DataSource, dm.Take);
                }
                

            }
            catch (Exception ex)
            {
                logger.Error("HRStoreManagersController - UrlDatasource - " + DateTime.Now + " - " + ex.Message.ToString());
            }

            return dm.RequiresCounts ? Json(new { result = DataSource, count = Count }) : Json(DataSource);
        }

        public ActionResult AddPartial()
        {
            HRStoreManagers hRStoreManagers = new HRStoreManagers();
            try
            {
                logger.Info("HRStoreManagersController - AddPartial - " + DateTime.Now);
                hRStoreManagers.FirstName = hRStoreManagers.FirstName;
                hRStoreManagers.UserName = hRStoreManagers.UserName;
                hRStoreManagers.Password = hRStoreManagers.Password;
                hRStoreManagers.ConfirmPassword = hRStoreManagers.ConfirmPassword;
                ViewBag.StoreName = _hRStoreManagersRepository.GetStoreMasters().Select(s => new { Text = s.Name, Value = s.StoreId }).OrderBy(o => o.Text).ToList();
                //ViewBag.UserName = _hRStoreManagersRepository.GetAllUserMastersForAdd().Select(s => new { Text = s.FirstName, Value = s.UserId }).OrderBy(o => o.Text).ToList();
            }
            catch (Exception ex)
            {
                logger.Error("HRStoreManagersController - AddPartial - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return PartialView("_DialogAddpartial");
        }

        public ActionResult EditPartial(HRStoreManagers value)
        {
            HRStoreManagers hRStoreManagers = new HRStoreManagers();
            try
            {
                logger.Info("HRStoreManagersController - EditPartial - " + DateTime.Now);
                hRStoreManagers = _hRStoreManagersRepository.GetHRStoreManagersByID(value.StoreManagerId);
                ViewBag.StoreName = _hRStoreManagersRepository.GetStoreMasters().Select(s => new { Text = s.Name, Value = s.StoreId }).OrderBy(o => o.Text).ToList();
                //ViewBag.UserName = _hRStoreManagersRepository.GetAllUserMasters(value.StoreManageId).Select(s => new { Text = s.FirstName, Value = s.UserId }).OrderBy(o => o.Text).ToList();
            }
            catch (Exception ex)
            {
                logger.Error("HRStoreManagersController - EditPartial - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return PartialView("_DialogEditpartial", value);
        }

        public async Task<ActionResult> InsertStoreManager(CRUDModel<HRStoreManagers> hRStoreManagers)
        {
            HRStoreManagers SM = new HRStoreManagers();
            SuccessMessage = null;
            ErrorMessage = null;
            try
            {
                logger.Info("HRStoreManagersController - InsertStoreManager - " + DateTime.Now);
                hRStoreManagers.Value.IsActive = true;
                hRStoreManagers.Value.CreatedBy = _CommonRepository.getUserId(UserName);
                SM = _hRStoreManagersRepository.InsertHRStoreManager(hRStoreManagers.Value);
                SuccessMessage = "Mobile App User Created Successfully.";
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message.ToString();
                logger.Error("HRStoreManagersController - InsertStoreManager - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return Json(new { data = SM, success = SuccessMessage, Error = ErrorMessage });
        }
        public async Task<ActionResult> UpdateStoreManager(CRUDModel<HRStoreManagers> hRStoreManagers)
        {
            HRStoreManagers SM = new HRStoreManagers();
            SuccessMessage = null;
            ErrorMessage = null;
            try
            {
                logger.Info("HRStoreManagersController - UpdateStoreManager - " + DateTime.Now);
                hRStoreManagers.Value.ModifiedBy = _CommonRepository.getUserId(UserName);
                SM = _hRStoreManagersRepository.UpdateHRStoreManager(hRStoreManagers.Value);
                SuccessMessage = "Mobile App User Updated Successfully.";
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message.ToString();
                logger.Error("HRStoreManagersController - UpdateStoreManager - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return Json(new { data = SM, success = SuccessMessage, Error = ErrorMessage });
        }
        public JsonResult UpdateStatusStoreManager(int StoreId, bool IsActive)
        {
            string Message = "";
            try
            {
                logger.Info("HRStoreManagersController - UpdateStatusStoreManager - " + DateTime.Now);
                HRStoreManagerViewModel SM = new HRStoreManagerViewModel();
                SM.StoreId = StoreId;
                SM.IsActive = IsActive;
                SM.ModifiedBy = _CommonRepository.getUserId(UserName);
                SM.ModifiedOn = DateTime.Now;
                _hRStoreManagersRepository.UpdateStoreManagerStatus(SM);
                Message = "Success";
            }
            catch (Exception ex)
            {
                Message = "Error";
                logger.Error("HRStoreManagersController - UpdateStatusStoreManager - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return Json(Message);
        }
    }
}