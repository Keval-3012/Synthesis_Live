using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityModels.HRModels;
using SynthesisViewModal.HRViewModel;

namespace Repository.IRepository
{
    public interface IHRAdminsRepository
    {
        List<HRAdmins> GetHRAdminList();
    }
}
