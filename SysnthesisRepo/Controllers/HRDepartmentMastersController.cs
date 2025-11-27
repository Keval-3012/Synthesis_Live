using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Aspose.Pdf.Operators;
using EntityModels.HRModels;
using EntityModels.Models;
using Microsoft.Extensions.Logging;
using NLog;
using Repository;
using Repository.IRepository;
using Syncfusion.EJ2.Base;
using SynthesisCF.Migrations;
using SynthesisViewModal;
using SynthesisViewModal.HRViewModel;
using Utility;

namespace SysnthesisRepo.Controllers
{
    public class HRDepartmentMastersController : Controller
    {
        private readonly IHRDepartmentRepository _hRDepartmentRepository;
        private readonly ICommonRepository _CommonRepository;
        Logger logger = LogManager.GetCurrentClassLogger();
        string SuccessMessage = string.Empty;
        string ErrorMessage = string.Empty;
        string UserName = System.Web.HttpContext.Current.User.Identity.Name;
        public HRDepartmentMastersController() 
        {
            this._hRDepartmentRepository = new HRDepartmentRepository(new DBContext());
            this._CommonRepository = new CommonRepository(new DBContext());
        }

        [Authorize(Roles = "Administrator,ViewManageDepartmentHRModule")]
        public ActionResult Index()
        {
            ViewBag.Title = "Store Departments - Synthesis";
            return View();
        }

        public ActionResult UrlDatasource(DataManagerRequest dm)
        {
            List<HRDepartmentViewModel> HrDeVm = new List<HRDepartmentViewModel>();
            IEnumerable DataSource = new List<HRDepartmentViewModel>();
            int Count = 0;
            try
            {
                logger.Info("HRDepartmentMastersController - UrlDatasource - " + DateTime.Now);
                //Using this db class Get Web cam details by storeid.
                HrDeVm = _hRDepartmentRepository.GetHRDepartmentList();
                DataSource = HrDeVm;
                
                DataOperations operation = new DataOperations();
                if (dm.Search != null && dm.Search.Count > 0)
                {
                    string search = dm.Search[0].Key.ToString().Trim();
                    DataSource = HrDeVm.ToList().Where(x => x.DepartmentName.Contains(search)).ToList();
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
                Count = DataSource.Cast<HRDepartmentViewModel>().Count();
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
                logger.Error("HRDepartmentMastersController - UrlDatasource - " + DateTime.Now + " - " + ex.Message.ToString());
            }

            return dm.RequiresCounts ? Json(new { result = DataSource, count = Count }) : Json(DataSource);
        }

        public async Task<ActionResult> InsertDepartment(CRUDModel<HRDepartmentMaster> hRDepartment)
        {
            HRDepartmentMaster HR = new HRDepartmentMaster();
            SuccessMessage = null;
            ErrorMessage = null;
            try
            {
                logger.Info("HRDepartmentMastersController - InsertDepartment - " + DateTime.Now);
                bool CheckEorN = _hRDepartmentRepository.CheckDepartmentExistOrNot(hRDepartment.Value);
                if (CheckEorN)
                {
                    ErrorMessage = "Department Name Already Exist";
                }
                else
                {
                    HR.CreatedBy = _CommonRepository.getUserId(UserName);
                    HR = _hRDepartmentRepository.InsertHRDepartment(hRDepartment.Value);
                    if(HR.DepartmentName == null)
                    {
                        ErrorMessage = "Please enter department name";
                    }
                    else
                    {
                        SuccessMessage = "Department Created Successfully.";
                    }
                    
                }

            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message.ToString();
                logger.Error("HRDepartmentMastersController - InsertDepartment - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return Json(new { data = HR, success = SuccessMessage, Error = ErrorMessage });
        }

        public async Task<ActionResult> UpdateDepartment(CRUDModel<HRDepartmentMaster> hRDepartment)
        {
            HRDepartmentMaster HR = new HRDepartmentMaster();
            SuccessMessage = null;
            ErrorMessage = null;
            try
            {
                logger.Info("HRDepartmentMastersController - UpdateDepartment - " + DateTime.Now);
                bool CheckEorN = _hRDepartmentRepository.UpdateTimeCheckDepartmentExistOrNot(hRDepartment.Value);
                if (CheckEorN)
                {
                    ErrorMessage = "Department Name Already Exist";
                }
                else
                {
                    HR.ModifiedBy = _CommonRepository.getUserId(UserName);
                    HR = _hRDepartmentRepository.UpdateHRDepartment(hRDepartment.Value);
                    SuccessMessage = "Department Updated Successfully.";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message.ToString();
                logger.Error("HRDepartmentMastersController - UpdateDepartment - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return Json(new { data = HR, success = SuccessMessage, Error = ErrorMessage });
        }

        public async Task<ActionResult> RemoveDepartment(CRUDModel<HRDepartmentMaster> hRDepartment)
        {
            HRDepartmentMaster HR = new HRDepartmentMaster();
            SuccessMessage = null;
            ErrorMessage = null;
            try
            {
                logger.Info("HRDepartmentMastersController - RemoveDepartment - " + DateTime.Now);
                HR = _hRDepartmentRepository.RemoveHRDepartment(Convert.ToInt32(hRDepartment.Key));
                SuccessMessage = "Department Deleted Successfully.";
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message.ToString();
                logger.Error("HRDepartmentMastersController - RemoveDepartment - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return Json(new { data = HR, success = SuccessMessage, Error = ErrorMessage });
        }

        public JsonResult UpdateStatusDepartment(int DepartmentId, bool IsActive)
        {
            string Message = "";
            try
            {
                logger.Info("HRDepartmentMastersController - UpdateStatusDepartment - " + DateTime.Now);
                HRDepartmentViewModel HR = new HRDepartmentViewModel();
                HR.DepartmentId = DepartmentId;
                HR.IsActive = IsActive;
                HR.ModifiedBy = _CommonRepository.getUserId(UserName);
                HR.ModifiedOn = DateTime.Now;
                _hRDepartmentRepository.UpdateDepartmentStatus(HR);
                Message = "Success";
            }
            catch (Exception ex)
            {
                Message = "Error";
                logger.Error("HRDepartmentMastersController - UpdateStatusDepartment - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return Json(Message);
        }
    }
}
