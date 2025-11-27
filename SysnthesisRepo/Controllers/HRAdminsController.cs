using EntityModels.Models;
using NLog;
using Repository;
using Repository.IRepository;
using Syncfusion.EJ2.Base;
using SynthesisViewModal.HRViewModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SysnthesisRepo.Controllers
{
    public class HRAdminsController : Controller
    {
        // GET: HRAdmins
        private readonly ICommonRepository _CommonRepository;
        private readonly IHRAdminsRepository _HRAdminsRepository;
        Logger logger = LogManager.GetCurrentClassLogger();
        string SuccessMessage = string.Empty;
        string ErrorMessage = string.Empty;
        string UserName = System.Web.HttpContext.Current.User.Identity.Name;

        public HRAdminsController()
        {
            this._HRAdminsRepository = new HRAdminsRepository(new DBContext());
            this._CommonRepository = new CommonRepository(new DBContext());
        }
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult UrlDatasource(DataManagerRequest dm)
        {
            List<HRAdmins> data = new List<HRAdmins>();
            IEnumerable DataSource = new List<HRAdmins>();
            int Count = 0;
            try
            {
                logger.Info("HRAdmins - UrlDatasource - " + DateTime.Now);
                data = _HRAdminsRepository.GetHRAdminList();
                DataSource = data;
                Count = DataSource.Cast<HRAdmins>().Count();
                DataOperations operation = new DataOperations();
                if (dm.Search != null && dm.Search.Count > 0)
                {
                    string search = dm.Search[0].Key.ToString().Trim();
                    DataSource = data.ToList().Where(x => x.UserName.Contains(search)).ToList();
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
                logger.Error("HRAdmins - UrlDatasource - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return dm.RequiresCounts ? Json(new { result = DataSource, count = Count }, JsonRequestBehavior.AllowGet) : Json(DataSource, JsonRequestBehavior.AllowGet);
        }
    }
}