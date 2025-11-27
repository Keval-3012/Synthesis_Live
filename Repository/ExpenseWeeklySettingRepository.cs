using EntityModels.Models;
using NLog;
using Repository.IRepository;
using SynthesisViewModal;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility;

namespace Repository
{
    public class ExpenseWeeklySettingRepository : IExpenseWeeklySettingRepository
    {
        private DBContext db;
        SpExpenseCheck_Setting SpExpenseCheck_Setting = new SpExpenseCheck_Setting();
        List<SpExpenseCheck_Setting> lstSpExpenseCheck_Setting = new List<SpExpenseCheck_Setting>();
        Logger logger = LogManager.GetCurrentClassLogger();

        public ExpenseWeeklySettingRepository(DBContext context)
        {
            db = context;
        }
        public List<DepartmentMaster> GetDepartmentMasters()
        {
            List<DepartmentMaster> departmentMasters = new List<DepartmentMaster>();
            try
            {
                departmentMasters = db.DepartmentMasters.ToList();
            }
            catch (Exception ex)
            {
                logger.Error("ExpenseWeeklySettingRepository - GetDepartmentMasters - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return departmentMasters;
        }
        public List<VendorMaster> GetVendorMasters()
        {
            List<VendorMaster> vendorMasters = new List<VendorMaster>();
            try
            {
                vendorMasters = db.VendorMasters.ToList();
            }
            catch (Exception ex)
            {
                logger.Error("ExpenseWeeklySettingRepository - GetVendorMasters - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return vendorMasters;
        }
        public List<HomeExpenseWeeklySalesSetting> HomeExpenseWeeklySalesSettingsByStoreID(int StoreID)
        {
            List<HomeExpenseWeeklySalesSetting> homeexpenseweeklysalesSettings = new List<HomeExpenseWeeklySalesSetting>();
            try
            {
                homeexpenseweeklysalesSettings = db.HomeExpenseWeeklySalesSettings.Where(s => s.StoreId == StoreID).ToList();
            }
            catch (Exception ex)
            {
                logger.Error("ExpenseWeeklySettingRepository - HomeExpenseWeeklySalesSettingsByStoreID - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return homeexpenseweeklysalesSettings;
        }
        public HomeExpenseWeeklySalesSetting HomeExpenseWeeklySalesSettingsByID(int ID)
        {
            HomeExpenseWeeklySalesSetting homeexpenseweeklysalesSettings = new HomeExpenseWeeklySalesSetting();
            try
            {
                homeexpenseweeklysalesSettings = db.HomeExpenseWeeklySalesSettings.Find(ID);
            }
            catch (Exception ex)
            {
                logger.Error("ExpenseWeeklySettingRepository - HomeExpenseWeeklySalesSettingsByID - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return homeexpenseweeklysalesSettings;
        }
        public void SaveHomeExpenseWeeklySalesSettings(HomeExpenseWeeklySalesSetting objs)
        {
            try
            {
                db.HomeExpenseWeeklySalesSettings.Add(objs);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Error("ExpenseWeeklySettingRepository - SaveHomeExpenseWeeklySalesSettings - " + DateTime.Now + " - " + ex.Message.ToString());
            }
        }
        public void UpdateHomeExpenseWeeklySalesSettings(HomeExpenseWeeklySalesSetting objs)
        {
            try
            {
                db.Entry(objs).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Error("ExpenseWeeklySettingRepository - UpdateHomeExpenseWeeklySalesSettings - " + DateTime.Now + " - " + ex.Message.ToString());
            }
        }
        public void ChangeStatus(int HomeexpenseID)
        {
            try
            {
                HomeExpensesDetail obj = new HomeExpensesDetail();
                obj = db.HomeExpensesDetails.Find(HomeexpenseID);
                obj.Status = 1;
                db.Entry(obj).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Error("ExpenseWeeklySettingRepository - ChangeStatus - " + DateTime.Now + " - " + ex.Message.ToString());
            }
        }
    }
}
