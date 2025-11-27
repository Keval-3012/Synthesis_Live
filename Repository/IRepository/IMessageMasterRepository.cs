using EntityModels.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.IRepository
{
    public interface IMessageMasterRepository
    {
        List<MessageMaster> GetMessageList();
        void SaveMessageMaster(int messageid, string keystr, string valueText, string description, string moduleName);
        List<string> GetModuleName();
    }
}
