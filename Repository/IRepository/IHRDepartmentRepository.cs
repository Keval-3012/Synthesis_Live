using EntityModels.HRModels;
using SynthesisViewModal.HRViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.IRepository
{
    public interface IHRDepartmentRepository
    {
        List<HRDepartmentViewModel> GetHRDepartmentList();
        bool CheckDepartmentExistOrNot(HRDepartmentMaster hR);
        bool UpdateTimeCheckDepartmentExistOrNot(HRDepartmentMaster hR);
        HRDepartmentMaster InsertHRDepartment(HRDepartmentMaster hR);
        HRDepartmentMaster UpdateHRDepartment(HRDepartmentMaster hR);
        HRDepartmentMaster RemoveHRDepartment(int DepartmentId);
        void UpdateDepartmentStatus(HRDepartmentViewModel HR);
        HRDepartmentMaster GetHRDepartmentById(int DepartmentId);
    }
}
