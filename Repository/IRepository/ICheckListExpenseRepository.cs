using EntityModels.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Repository.IRepository
{
    public interface ICheckListExpenseRepository
    {
        List<CheckIndexList> Getchecklistbinddata(int StoreId);
        List<CheckIndexList> UpdateMailOutStatus(string id, string value, int UserId);
        List<CheckIndexList> UpdateMailOutAllStatus(string StartDate, string EndDate,string value, int UserId, string Searchtext, string Mail, int StoreId,string CheckFilter);
        List<CheckIndexList> CheckListFilter_ALL(string Mail, string startdate, string enddate, int? StoreIds, string Searchtext, string CheckFilter);
    }
}
