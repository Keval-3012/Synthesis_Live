using EntityModels.HRModels;
using EntityModels.Models;
using SynthesisViewModal.HRViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.IRepository
{
    public interface IHRConsentMastersRepository
    {
        List<HRConsentViewModel> GetConsentList();

        List<ConsentModel> consentList();
        List<ConsentModelDetail> GetConsentDetail(int ConsentId);
        HRConsentMaster GetConsentByID(ConsentModel objConsent);
        void EditConsentDetails(HRConsentMaster hRConsentMaster, ConsentModel objConsent);
        void DeleteConsentDetail(int ConsentId);
        void SaveConsent(ConsentModel objConsent, HRConsentMaster obj);
        List<HRFormTypeMaster> GetFormList();
        List<ConsentStatusListModel> GetConsentStatusList(string fromdate, string todate, int? signcheck, int? storeid,bool checkrole);
        List<StoreMaster> GetRolewiseStore(int userid);
    }
}
