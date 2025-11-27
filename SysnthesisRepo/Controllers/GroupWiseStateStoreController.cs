using EntityModels.Models;
using NLog;
using Repository;
using Repository.IRepository;
using Syncfusion.EJ2.Base;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace SysnthesisRepo.Controllers
{
    public class GroupWiseStateStoreController : Controller
    {
        // GET: KeyConfiguration
        Logger logger = LogManager.GetCurrentClassLogger();
        private readonly IGroupWiseStateStoreRepository _GroupWiseStateStoreRepository;
        private readonly IUserMastersRepository _UserMastersRepository;
        private readonly ICommonRepository _CommonRepository;
        private readonly IUserActivityLogRepository _ActivityLogRepository;
        protected static string StatusMessageString = "";
        public GroupWiseStateStoreController()
        {
            this._GroupWiseStateStoreRepository = new GroupWiseStateStoreRepository(new DBContext());
            this._UserMastersRepository = new UserMastersRepository(new DBContext());
            this._CommonRepository = new CommonRepository(new DBContext());
            this._ActivityLogRepository = new UserActivityLogRepository(new DBContext());
        }

        public ActionResult Index()
        {
            ViewBag.Title = "Group Wise State Store - Synthesis";
            return View();
        }

        public ActionResult UrlDatasource(DataManagerRequest dm)
        {
            List<GroupWiseStateStoreSelect> HrEvm = new List<GroupWiseStateStoreSelect>();
            int count = 0;
            try
            {
                logger.Info("GroupWiseStateStoreController - UrlDatasource - " + DateTime.Now);
                HrEvm = _GroupWiseStateStoreRepository.GetInformation();
                IEnumerable DataSource = HrEvm;

                DataOperations operation = new DataOperations();
                if (dm.Search != null && dm.Search.Count > 0)
                {
                    DataSource = operation.PerformSearching(DataSource, dm.Search);  //Search
                }
                if (dm.Sorted != null && dm.Sorted.Count > 0) //Sorting
                {
                    DataSource = operation.PerformSorting(DataSource, dm.Sorted);
                }
                if (dm.Where != null && dm.Where.Count > 0) //Filtering
                {
                    DataSource = operation.PerformFiltering(DataSource, dm.Where, dm.Where[0].Operator);
                }
                count = DataSource.Cast<GroupWiseStateStoreSelect>().Count();
                if (dm.Skip != 0)
                {
                    DataSource = operation.PerformSkip(DataSource, dm.Skip);//Paging
                }
                if (dm.Take != 0)
                {
                    DataSource = operation.PerformTake(DataSource, dm.Take);
                }
                return dm.RequiresCounts ? Json(new { result = DataSource, count = count }) : Json(DataSource);
            }
            catch (Exception ex)
            {
                logger.Error("GroupWiseStateStoreController - UrlDatasource - " + DateTime.Now + " - " + ex.Message.ToString());
                return Json("", JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult AddPartial()
        {
            GroupWiseStateStore obj = new GroupWiseStateStore();
            try
            {
                logger.Info("GroupWiseStateStoreController - AddPartial - " + DateTime.Now);
                obj.GroupName = obj.GroupName;
                ViewBag.MultipleStorer = new SelectList(_UserMastersRepository.GetStoreMastersLists(), "StoreId", "NickName");
                //ViewBag.UserId = new SelectList(_GroupWiseStateStoreRepository.GetUserMasters().Select(s => new { s.UserId, s.FirstName }), "UserId", "FirstName", obj.UserId);
            }
            catch (Exception ex)
            {
                logger.Error("GroupWiseStateStoreController - AddPartial - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return PartialView("_DialogGroupAddpartial",obj);
        }

        public async Task<ActionResult> EditPartial(GroupWiseStateStore value)
        {
            GroupWiseStateStore obj = new GroupWiseStateStore();
            try
            {
                logger.Info("GroupWiseStateStoreController - EditPartial - " + DateTime.Now);
                obj = await _GroupWiseStateStoreRepository.GetFindGroupStateWiseStores(value.GroupWiseStateStoreId);
                //ViewBag.UserId = new SelectList(_GroupWiseStateStoreRepository.GetUserMasters().Select(s => new { s.UserId, s.FirstName }), "UserId", "FirstName", obj.UserId);
                if (obj.StoreName == null)
                {
                    ViewBag.MultipleStorer = new SelectList(_UserMastersRepository.GetStoreMastersLists(), "StoreId", "NickName", obj.MuiltiStoreAccess);
                }
                else
                {
                    obj.MuiltiStoreAccess = obj.StoreName.Split(',');
                    ViewBag.MultipleStorer = new SelectList(_UserMastersRepository.GetStoreMastersLists(), "StoreId", "NickName", obj.MuiltiStoreAccess);
                }
            }
            catch (Exception ex)
            {
                logger.Error("GroupWiseStateStoreController - EditPartial - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return PartialView("_DialogGroupEditpartial", obj);
        }

        public ActionResult InsertGroupWiseStateStore(GroupWiseStateStore value)
        {
            string SuccessMessage = null;
            string ErrorMessage = null;
            try
            {

                logger.Info("GroupWiseStateStoreController - InsertGroupWiseStateStore - " + DateTime.Now);
                var UserName = System.Web.HttpContext.Current.User.Identity.Name;
                int UserId = _CommonRepository.getUserId(UserName);
                value.CreatedBy = UserId;
                if (value.MuiltiStoreAccess == null)
                {
                    value.StoreName = null;
                }
                else
                {
                    value.StoreName = string.Join(",", value.MuiltiStoreAccess);
                }
                _GroupWiseStateStoreRepository.InsertInformation(value);
                SuccessMessage = "Group Wise State Store Created Successfully.";
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message.ToString();
                logger.Error("GroupWiseStateStoreController - InsertGroupWiseStateStore - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return Json(new { data = value, success = SuccessMessage, Error = ErrorMessage });
        }

        public ActionResult UpdateGroupWiseStateStore(GroupWiseStateStore value)
        {
            string SuccessMessage = null;
            string ErrorMessage = null;
            try
            {
                logger.Info("GroupWiseStateStoreController - UpdateGroupWiseStateStore - " + DateTime.Now);
                var UserName = System.Web.HttpContext.Current.User.Identity.Name;
                int UserId = _CommonRepository.getUserId(UserName);
                value.ModifiedBy = UserId;
                if (value.MuiltiStoreAccess == null)
                {
                    value.StoreName = null;
                }
                else
                {
                    value.StoreName = string.Join(",", value.MuiltiStoreAccess);
                }                
                _GroupWiseStateStoreRepository.UpdateInformation(value);                
                SuccessMessage = "Group Wise State Store Updated Successfully.";

            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message.ToString();
                logger.Error("GroupWiseStateStoreController - UpdateGroupWiseStateStore - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return Json(new { data = value, success = SuccessMessage, Error = ErrorMessage });
        }

        public async Task<ActionResult> DeleteGroupWiseStateStore(CRUDModel<GroupWiseStateStore> value)       
        {
            string SuccessMessage = null;
            string ErrorMessage = null;
            try
            {
                logger.Info("GroupWiseStateStoreController - DeleteGroupWiseStateStore - " + DateTime.Now);
                _GroupWiseStateStoreRepository.DeleteInformation(Convert.ToInt32(value.Key));
                SuccessMessage = "Group Wise State Store Deleted Succeesfully.";
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message.ToString();
                logger.Info("GroupWiseStateStoreController - DeleteGroupWiseStateStore - " + DateTime.Now + " - " + ex.Message.ToString());

            }
            return Json(new { data = value.Deleted, success = SuccessMessage, Error = ErrorMessage });
        }
    }
}