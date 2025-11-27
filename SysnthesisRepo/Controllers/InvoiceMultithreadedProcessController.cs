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

namespace SysnthesisRepo.Controllers
{
    public class InvoiceMultithreadedProcessController : Controller
    {
        // GET: KeyConfiguration
        Logger logger = LogManager.GetCurrentClassLogger();
        private readonly ICommonRepository _CommonRepository;
        private readonly IInvoiceMultithreadedProcessRepository _APIKeyConfigurationRepository;
        protected static string StatusMessageString = "";
        public InvoiceMultithreadedProcessController()
        {
            this._APIKeyConfigurationRepository = new InvoiceMultithreadedProcessRepository(new DBContext());
            this._CommonRepository = new CommonRepository(new DBContext());
        }

        public ActionResult Index()
        {            
            var data = _APIKeyConfigurationRepository.GetKeyConfiguartions();
            return View(data);
        }

        [HttpPost]
        public ActionResult Create(string apikey)
        {
            string message = "";
            try
            {
                var UserId = UserModule.getUserId();
                _APIKeyConfigurationRepository.SaveKey(apikey, UserId);
                message = "API Key Created Sucessfully";
            }
            catch (Exception ex)
            {
                logger.Error("APIKeyConfigurationController - Create - " + DateTime.Now + " - " + ex.Message.ToString());
                message = "Error";
            }
            return Json(new { success = "Create" });
        }

        public ActionResult Edit(int id)
        {
            ApiKeyConfiguartion _Api = new ApiKeyConfiguartion();
            try
            {
                _Api = _APIKeyConfigurationRepository.GetFindApiKey(id);
            }
            catch (Exception ex)
            {
                logger.Error("APIKeyConfigurationController - Edit - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return Json(new { apikeyid = _Api.APIKeyId,apikeyname = _Api.APIKeyName });
        }

        [HttpPost]
        public ActionResult Update(int apikeyid,string apikey)
        {
            string message = "";
            try
            {
                var UserId = UserModule.getUserId();
                _APIKeyConfigurationRepository.UpdateKey(apikeyid, apikey, UserId);
                message = "API Key Updated Sucessfully";
            }
            catch (Exception ex)
            {
                logger.Error("APIKeyConfigurationController - Update - " + DateTime.Now + " - " + ex.Message.ToString());
                message = "Error";
            }

            return Json(new { success = "Update" });
        }

        public ActionResult Delete(int? id)
        {
            string message = "";
            try
            {
                var UserId = UserModule.getUserId();
                _APIKeyConfigurationRepository.DeleteKey(id,UserId);
                message = "API Key Deleted Sucessfully";
            }
            catch (Exception ex)
            {
                logger.Error("APIKeyConfigurationController - Delete - " + DateTime.Now + " - " + ex.Message.ToString());
                message = "Error";
            }

            return Json(new { success = "Delete" });
        }

        public ActionResult EditBucketSize()
        {
            BucketSizeMaster bucket = new BucketSizeMaster();
            try
            {
                bucket = _APIKeyConfigurationRepository.GetBucketSize();
            }
            catch (Exception ex)
            {
                logger.Error("APIKeyConfigurationController - EditBucketSize - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return Json(new { bucketid = bucket.BucketId, bucketsize = bucket.BucketSize});
        }

        [HttpPost]
        public ActionResult UpdateBucketSize(int bucketid, int bucketsize)
        {
            string message = "";
            try
            {
                _APIKeyConfigurationRepository.UpdateBucketSize(bucketid, bucketsize);
                message = "Bucket Size Updated Sucessfully";
            }
            catch (Exception ex)
            {
                logger.Error("APIKeyConfigurationController - UpdateBucketSize - " + DateTime.Now + " - " + ex.Message.ToString());
                message = "Error";
            }

            return Json(new { success = "Update" });
        }
    }
}