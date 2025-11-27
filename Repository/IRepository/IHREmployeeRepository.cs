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
    public interface IHREmployeeRepository
    {
        List<HREmployeeViewModal> GetHREmployeeList(int store_idval,int userid);
        List<HREthnicityMaster> GetHREthnicityList();
        HREmployeeMaster CheckHrEmployeeIfExist(string EmployeeUserName, int EmployeeId);
        int InsertEmployeeMaster(HREmployeeMaster obj);
        int InsertEmployeeChilde(List<HREmployeeChild> obj);
        HREmployeeMaster GetHREmployeeMaster(int EmployeeId);
        List<HREmployeeChild> GetHREmployeeChild(int EmployeeId);
        int DeleteChildExtraRecode(int EmployeeId, int? SrNo);
        bool DeleteEmployee(int EmployeeId, int ModifiedBy);
        int GetModuleMastersId(string ModuleName);
        bool UpdateEmployeePassword(int EmployeeId, int ModifiedBy);
        List<StoreDetail> GetEmployeeUsingStore(int EmployeeId);
        HREmployeeChild GetHREmployeeChildByEmployeeIdandStoreId(int EmployeeId, int StoreId);
        List<string> GetUserRoleList_BaasedOnTypeWise(int UserTypeId, int storeId, int ModuleId);
        string GetSSN(string Password, int EmployeeId,int UserId);
        
        #region Pay Rate Repository
        List<HREmployeePayRate> GetHREmployeePayRateList(int EmployeeId, int StoreId,int EmployeeChildId);
        HREmployeePayRate InsertUpdateHREmployeePayRate(HREmployeePayRate obj);
        HREmployeePayRate GetHREmployeePayRateByID(int EmployeePayRateId);
        HREmployeePayRate RemoveHRPayRate(int EmployeePayRateId);
        decimal GetHourlyRate(string Password, int EmployeePayRateId,int UserId);
        #endregion Pay Rate Repository

        #region Sick Time Repository
        List<HREmployeeSickTimes> GetHREmployeeSickTimeList(int EmployeeId, int StoreId, int EmployeeChildId);
        HREmployeeSickTimes InsertUpdateHREmployeeSickTimes(HREmployeeSickTimes obj);
        HREmployeeSickTimes GetHREmployeeSickTimeByID(int EmployeeSickTimeId);
        HREmployeeSickTimes RemoveHREmployeeSickTime(int EmployeeSickTimeId);
        #endregion Sick Time Repository

        #region Vacation Time Repository
        List<HREmployeeVacationTime> GetHREmployeeVacationTimeList(int EmployeeId, int StoreId, int EmployeeChildId);
        HREmployeeVacationTime InsertUpdateHREmployeeVacationTime(HREmployeeVacationTime obj);
        HREmployeeVacationTime GetHREmployeeVacationByID(int EmployeeVacationTimeId);
        HREmployeeVacationTime RemoveHREmployeeVacationTime(int EmployeeVacationTimeId);
        #endregion Vacation Time Repository

        #region Insurance Repository
        List<HREmployeeInsurance> GetHREmployeeInsurance(int EmployeeId, int StoreId, int EmployeeChildId);
        HREmployeeInsurance InsertUpdateHREmployeeInsurance(HREmployeeInsurance obj);
        HREmployeeInsurance RemoveHREmployeeInsurance(int EmployeeInsuranceId);
        HREmployeeInsurance GetHREmployeeInsuranceID(int EmployeeInsuranceId);
        #endregion Insurance Repository

        #region Documents Repository
        List<HRFileDetailViewModel> GetHREmployeeDocumentsList(int EmployeeId, int StoreId, int EmployeeChildId);
        HREmployeeDocument InsertHREmployeeDocuments(HREmployeeDocument obj);
        HREmployeeDocument RemoveHREmployeeDocument(int DocId, int EmployeeId, string FileName, int ModifiedBy, int Type);
        HREmployeeRetirementInfo InsertHREmployeeDocuments401K(HREmployeeRetirementInfo obj);
        #endregion Documents Repository

        #region Notes Repository
        List<HRNoteViewModal> GetHREmployeeNotesList(int EmployeeId, int StoreId, int EmployeeChildId);
        HREmployeeNotes InsertHREmployeeNotes(HREmployeeNotes obj);

       
        #endregion Notes Repository

        #region Training History Repository
        List<HREmployeeMaster> GetHREmployeeTrainingHistory(int EmployeeId, int StoreId);
        #endregion Training History Repository

        List<HRWarningViewModel> GetHREmployeeWarningList(int EmployeeId, int StoreId, int EmployeeChildId);
        List<HRTerminationViewModel> GetHREmployeeTerminationList(int EmployeeId, int StoreId, int EmployeeChildId);
    }
}
