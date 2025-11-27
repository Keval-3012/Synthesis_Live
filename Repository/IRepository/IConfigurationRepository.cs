using EntityModels.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Repository.IRepository
{
    public interface IConfigurationRepository
    {
        List<Configurationlist> getConfigrationlist(int storeid);
        void SaveDepartmentconfigurations(int IID, int Deptid, int Flag, string TenderName, int StoreID, string memoidval, int typicalbalid);
        List<ConfigurationGroupData> getGroupDatabygroupid(int groupval);
        bool ResetConfiguration(string tenderName, int storeId);
    }
}
