using EntityModels.Models;
using NLog;
using Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class BankAccountsRepository : IBankAccountsRepository
    {
        private DBContext _context;
        Logger logger = LogManager.GetCurrentClassLogger();

        public BankAccountsRepository(DBContext context)
        {
            _context = context;
        }
        public List<BankAccountSettingDetail> GetBankDetail(int StoreID)
        {
            List<BankAccountSettingDetail> BankAccDetails = new List<BankAccountSettingDetail>();
            try
            {
                BankAccDetails = _context.Database.SqlQuery<BankAccountSettingDetail>("SP_BankAccountSetting @Spara = {0},@StoreID = {1}", "GetBankDetail", StoreID).ToList();
            }
            catch (Exception ex)
            {
                logger.Error("BankAccountsRepository - GetBankDetail - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return BankAccDetails;
        }
        public BankAccountSettingModel GetBankDetailsByID(int? ID)
        {
            BankAccountSettingModel bnkAcc = new BankAccountSettingModel();
            try
            {
                bnkAcc = _context.BankAccountSettingModels.Find(ID);
            }
            catch (Exception ex)
            {
                logger.Error("BankAccountsRepository - GetBankDetailsByID - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return bnkAcc;
        }
        public void SaveBankDetails(BankAccountSettingModel BankAccDetails)
        {
            try
            {
                _context.BankAccountSettingModels.Add(BankAccDetails);
                _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                logger.Error("BankAccountsRepository - SaveBankDetails - " + DateTime.Now + " - " + ex.Message.ToString());
            }
        }
        public StoreMaster GetStoreMastersdata(BankAccountSettingModel bankAccountSettingModel)
        {
            StoreMaster storemaster = new StoreMaster();
            try
            {
                storemaster = _context.StoreMasters.Where(s => s.StoreId == bankAccountSettingModel.StoreID).FirstOrDefault();
            }
            catch (Exception ex)
            {
                logger.Error("BankAccountsRepository - GetStoreMastersdata - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return storemaster;
        }
        public void DeleteBankDetails(BankAccountSettingModel bankdetails)
        {
            try
            {
                _context.BankAccountSettingModels.Remove(bankdetails);
                _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                logger.Error("BankAccountsRepository - DeleteBankDetails - " + DateTime.Now + " - " + ex.Message.ToString());
            }
        }
    }
}
