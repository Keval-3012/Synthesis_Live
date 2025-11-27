using EntityModels.Models;
using EntityModels.HRModels;
using NLog;
using Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SynthesisViewModal.HRViewModel;

namespace Repository
{
    public class HRConsentMastersRepository : IHRConsentMastersRepository
    {
        private DBContext _context;
        Logger logger = LogManager.GetCurrentClassLogger();
        public HRConsentMastersRepository(DBContext context)
        {
            _context = context;
        }

        public List<ConsentModel> consentList()
        {
            List<ConsentModel> data = new List<ConsentModel> ();
            try
            {
                logger.Info("HRConsentMastersRepository - consentList - " + DateTime.Now);
                data = (from result in _context.HRConsentMaster
                        select new ConsentModel
                        {
                            ConsentId = result.ConsentId,
                            FormTypeId = result.FormTypeId
                        }).ToList();
            }
            catch (Exception ex)
            {
                logger.Error("HRConsentMastersRepository - consentList - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return data;
        }

        public void DeleteConsentDetail(int ConsentId)
        {
            try
            {
                logger.Info("HRConsentMastersRepository - DeleteConsentDetail - " + DateTime.Now);
                HRConsentMaster obj = new HRConsentMaster();
                obj.ConsentId = ConsentId;
                _context.HRConsentDetails.Where(p => p.ConsentId == ConsentId).ToList().ForEach(p => _context.HRConsentDetails.Remove(p));
                _context.SaveChanges();
                _context.Entry(obj).State = System.Data.Entity.EntityState.Deleted;
                _context.SaveChanges();
                obj = null;  
            }
            catch (Exception ex)
            {
                logger.Error("HRConsentMastersRepository - DeleteConsentDetail - " + DateTime.Now + " - " + ex.Message.ToString());
            }
        }

        public HRConsentMaster GetConsentByID(ConsentModel objConsent)
        {
            HRConsentMaster data = new HRConsentMaster();
            try
            {
                logger.Info("HRConsentMastersRepository - GetConsentByID - " + DateTime.Now);
                data = _context.HRConsentMaster.SingleOrDefault(b => b.ConsentId == objConsent.ConsentId);
            }
            catch (Exception ex)
            {
                logger.Error("HRConsentMastersRepository - GetConsentByID - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return data;
        }

        public List<ConsentModelDetail> GetConsentDetail(int ConsentId)
        {
            List<ConsentModelDetail> details = new List<ConsentModelDetail>();
            try
            {
                logger.Info("HRConsentMastersRepository - GetConsentDetail - " + DateTime.Now);
                details = (from result1 in _context.Database.SqlQuery<SPConsentDetail_Result>("SP_GetAllHRConsent @Mode = {0},@ConsentId = {1}", "GetConsentDetail",ConsentId)
                           select new ConsentModelDetail
                           {
                               DetailId = result1.DetailId,
                               ConsentId = Convert.ToInt32(result1.ConsentId),
                               Language = result1.Language,
                               Type = result1.Type,
                               LanguageType = result1.LanguageType,
                               Types = result1.Types,
                               Detail = result1.Detail
                           }).ToList();
            }
            catch (Exception ex)
            {
                logger.Error("HRConsentMastersRepository - GetConsentDetail - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return details;
        }

        public List<HRConsentViewModel> GetConsentList()
        {
            List<HRConsentViewModel> data = new List<HRConsentViewModel>();
            try
            {
                logger.Info("HRConsentMastersRepository - GetConsentList - " + DateTime.Now);
                data = _context.Database.SqlQuery<HRConsentViewModel>("SP_GetAllHRConsent @Mode = {0}", "Select").ToList();
            }
            catch (Exception ex)
            {
                logger.Error("HRConsentMastersRepository - GetConsentList - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return data;
        }

        public void EditConsentDetails(HRConsentMaster hRConsentMaster, ConsentModel objConsent)
        {
            try
            {
                logger.Info("HRConsentMastersRepository - EditConsentDetails - " + DateTime.Now);
                _context.SaveChanges();
                int iConsentId = Convert.ToInt32(hRConsentMaster.ConsentId);
                if (iConsentId > 0)
                {
                    _context.HRConsentDetails.Where(p => p.ConsentId == iConsentId).ToList().ForEach(p => _context.HRConsentDetails.Remove(p));
                    _context.SaveChanges();

                    foreach (var item in objConsent.ConsentModelDetails)
                    {
                        HRConsentDetails objDetail = new HRConsentDetails();
                        objDetail.ConsentId = iConsentId;
                        objDetail.LanguageId = item.Language;
                        objDetail.TypeId = item.Type;
                        objDetail.Description = item.Detail;
                        _context.HRConsentDetails.Add(objDetail);
                        _context.SaveChanges();
                    }                    
                }
            }
            catch (Exception ex)
            {
                logger.Error("HRConsentMastersRepository - EditConsentDetails - " + DateTime.Now + " - " + ex.Message.ToString());
            }
        }

        public void SaveConsent(ConsentModel objConsent, HRConsentMaster obj)
        {
            try
            {
                logger.Info("HRConsentMastersRepository - SaveConsent - " + DateTime.Now);
                _context.HRConsentMaster.Add(obj);
                _context.SaveChanges();
                int iConsentId = Convert.ToInt32(obj.ConsentId);
                if (iConsentId > 0)
                {
                    foreach (var item in objConsent.ConsentModelDetails)
                    {
                        HRConsentDetails objDetail = new HRConsentDetails();
                        objDetail.ConsentId = iConsentId;
                        objDetail.LanguageId = item.Language;
                        objDetail.TypeId = item.Type;
                        objDetail.Description = item.Detail;
                        _context.HRConsentDetails.Add(objDetail);
                        _context.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("HRConsentMastersRepository - SaveConsent - " + DateTime.Now + " - " + ex.Message.ToString());
            }
        }

        public List<HRFormTypeMaster> GetFormList()
        {
            List<HRFormTypeMaster> list = new List<HRFormTypeMaster>();
            try
            {
                logger.Info("HRConsentMastersRepository - GetFormList - " + DateTime.Now);
                list = _context.HRFormTypeMaster.ToList();
            }
            catch (Exception ex)
            {
                logger.Error("HRConsentMastersRepository - GetFormList - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return list;
        }

        public List<ConsentStatusListModel> GetConsentStatusList(string fromdate, string todate, int? signcheck, int? storeid,bool checkrole)
        {
            List<ConsentStatusListModel> data = new List<ConsentStatusListModel>();
            try
            {
                logger.Info("HRConsentMastersRepository - GetConsentStatusList - " + DateTime.Now);
                data = _context.Database.SqlQuery<ConsentStatusListModel>("SP_GetAllHRConsent @Mode = {0}, @FromDate = {1}, @ToDate = {2}, @SignCheck = {3}, @StoreId = {4},@CheckRole = {5}", "GetConsentStatusList",fromdate,todate,signcheck,storeid,checkrole).ToList();
            }
            catch (Exception ex)
            {
                logger.Error("HRConsentMastersRepository - GetConsentStatusList - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return data;
        }
        public List<StoreMaster> GetRolewiseStore(int userid)
        {
            List<StoreMaster> data = new List<StoreMaster>();
            try
            {
                logger.Info("HRConsentMastersRepository - GetRolewiseStore - " + DateTime.Now);
                var result = _context.Database.SqlQuery<StoreDetail>("SP_GetAllHRConsent @Mode = {0}, @UserId = {1}", "GetConsentRolewiseStore", userid).ToList();
                data = result.Select(x => new StoreMaster { StoreId = x.StoreId, Name = x.Name }).ToList();
            }
            catch (Exception ex)
            {
                logger.Error("HRConsentMastersRepository - GetRolewiseStore - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return data;
        }

    }
}
