using EntityModels.Models;
using NLog;
using NLog.Fluent;
using Repository.IRepository;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class CustomerReciptRepository : ICustomerRecipt
    {
        private DBContext _Context;
        Logger logger = LogManager.GetCurrentClassLogger();

        public CustomerReciptRepository(DBContext context)
        {
            _Context = context;
        }

        public CustomersReceiveablesReceipts InsertRecipt(CustomersReceiveablesReceipts _Cm)
        {            
            try
            {
                _Context.Database.ExecuteSqlCommand("SP_CustomerInfo @Spara={0},@CompanyNameId={1},@FileName={2},@IsEmailTrigger={3},@StoreId={4}", "ReceiptInsert", _Cm.CompanyNameId, _Cm.FileName, _Cm.IsEmailTriggered, _Cm.StoreId);                
            }
            catch (Exception ex)
            {
                logger.Error("CustomerReciptRepository - InsertRecipt - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return _Cm;
        }
        public CustomersReceiveablesReceipts getcustomerreciptdata(int customerreceiptid)
        {
            CustomersReceiveablesReceipts data = new CustomersReceiveablesReceipts();
            try
            {
                data = _Context.CustomersReceiveablesReceipt.Find(customerreceiptid);
            }
            catch (Exception ex)
            {
                logger.Error("CustomerReciptRepository - getcustomerreciptdata - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return data;
        }

        public CustomersReceiveablesManagement getcustomerdata(int customerreceiptid)
        {
            CustomersReceiveablesManagement data = new CustomersReceiveablesManagement();
            try
            {
                int? CompanyNameId = 0;
                CompanyNameId = _Context.CustomersReceiveablesReceipt.Where(x => x.CustomersReceiveablesReceiptsId == customerreceiptid).Select(x => x.CompanyNameId).FirstOrDefault();

                data = _Context.CustomersReceiveablesManagement.Find(CompanyNameId);
            }
            catch (Exception ex)
            {
                logger.Error("CustomerReciptRepository - getcustomerdata - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return data;
        }

        public List<CustomersReceiveablesReceiptsListModel> GetCustomerRecieptList(int? StoreId)
        {
            List<CustomersReceiveablesReceiptsListModel> CustomersReceiveablesReceipts = new List<CustomersReceiveablesReceiptsListModel>();
            IEnumerable data = "";
            try
            {
                data = _Context.Database.SqlQuery<CustomersReceiveablesReceiptsListModel>("SP_CustomerInfo @Spara={0},@StoreId={1}", "GetCustomerRecieptList",StoreId).ToList();
                CustomersReceiveablesReceipts = data.OfType<CustomersReceiveablesReceiptsListModel>().ToList();
                foreach (var item in CustomersReceiveablesReceipts)
                {
                    if (!string.IsNullOrEmpty(item.FileName))
                    {
                        item.FileName = System.IO.Path.GetFileName(item.FileName);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("APIKeyConfigurationRepository - GetKeyConfiguartions - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return CustomersReceiveablesReceipts;
        }

        public void UpdateIsEmailTriggered(int customerreceiptid)
        {
            try
            {
                _Context.Database.ExecuteSqlCommand("SP_CustomerInfo @Spara={0},@CustomersReceiveablesReceiptsId={1}", "UpdareEmailTriggered", customerreceiptid);
            }
            catch (Exception ex)
            {
                logger.Error("CustomerReciptRepository - UpdateIsEmailTriggered - " + DateTime.Now + " - " + ex.Message.ToString());
            }
        }

        public void DeleteCustomerReceipt(int? id)
        {
            try
            {
                _Context.Database.ExecuteSqlCommand("SP_CustomerInfo @Spara={0},@CustomersReceiveablesReceiptsId={1}", "DeleteCustomerReceivedReceipt", id);
            }
            catch (Exception ex)
            {
                logger.Error("CustomerReciptRepository - DeleteCustomerReceipt - " + DateTime.Now + " - " + ex.Message.ToString());
            }
        }

        public void UpdateCompanyNameCustomerReceipt(int CustomersReceiveablesReceiptsId, int CompanyNameId)
        {
            try
            {
                _Context.Database.ExecuteSqlCommand("SP_CustomerInfo @Spara={0},@CustomersReceiveablesReceiptsId={1},@CompanyNameId={2}", "UpdateCompanyNameReceipt", CustomersReceiveablesReceiptsId, CompanyNameId);
            }
            catch (Exception ex)
            {
                logger.Error("CustomerReciptRepository - UpdateCompanyNameCustomerReceipt - " + DateTime.Now + " - " + ex.Message.ToString());
            }
        }
    }
}
