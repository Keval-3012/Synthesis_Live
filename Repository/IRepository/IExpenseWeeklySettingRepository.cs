using EntityModels.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.IRepository
{
    public interface IExpenseWeeklySettingRepository
    {
        List<DepartmentMaster> GetDepartmentMasters();
        List<VendorMaster> GetVendorMasters();
        List<HomeExpenseWeeklySalesSetting> HomeExpenseWeeklySalesSettingsByStoreID(int StoreID);
        void SaveHomeExpenseWeeklySalesSettings(HomeExpenseWeeklySalesSetting objs);
        HomeExpenseWeeklySalesSetting HomeExpenseWeeklySalesSettingsByID(int ID);
        void UpdateHomeExpenseWeeklySalesSettings(HomeExpenseWeeklySalesSetting objs);
        void ChangeStatus(int HomeexpenseID);
    }
}
