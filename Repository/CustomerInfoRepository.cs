using EntityModels.Models;
using NLog;
using Repository.IRepository;
using Syncfusion.EJ2.Linq;
using SynthesisQBOnline;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class CustomerInfoRepository : ICustomerInfoRepository
    {
        private DBContext _Context;
        Logger logger = LogManager.GetCurrentClassLogger();

        public CustomerInfoRepository(DBContext context) 
        {
            _Context = context;
        }

        public CustomersReceiveablesManagement  InsertInformation(CustomersReceiveablesManagement Cm, int StoreId)
        {
            try
            {
                string PhoneNumber = null;

                if (!String.IsNullOrEmpty(Cm.PhoneNumber))
                {
                    PhoneNumber = Convert.ToInt64(Cm.PhoneNumber).ToString("(###) ###-####");
                }
                Cm.PhoneNumber = PhoneNumber;
                _Context.Database.ExecuteSqlCommand("SP_CustomerInfo @Spara={0},@CompanyName={1},@EmailAddress={2},@PhoneNumber={3},@Address={4},@ContactPersonName={5},@StoreId={6}", "Insert", Cm.CompanyName, Cm.EmailAddress, Cm.PhoneNumber,Cm.Address,Cm.ContactPersonName,StoreId);                
            }
            catch (Exception ex)
            {
                logger.Error("CustomerInfoRepository - InsertInformation - " + DateTime.Now + " - " + ex.Message.ToString());               
            }
           return Cm;
        }

        public CustomersReceiveablesManagement UpdateInformation(CustomersReceiveablesManagement Cm)
        {
            try
            {
                string PhoneNumber = null;

                //if (!String.IsNullOrEmpty(Cm.PhoneNumber))
                //{
                //    PhoneNumber = Convert.ToInt64(Cm.PhoneNumber).ToString("(###) ###-####");
                //}
                Cm.PhoneNumber = PhoneNumber;
                _Context.Database.ExecuteSqlCommand("SP_CustomerInfo @Spara={0},@CompanyName={1},@EmailAddress={2},@PhoneNumber={3},@Address={4},@ContactPersonName={5},@CompanyNameId={6},@StoreId={6}", "Update", Cm.CompanyName, Cm.EmailAddress, Cm.PhoneNumber, Cm.Address, Cm.ContactPersonName,Cm.CompanyNameId,Cm.StoreId);
            }
            catch (Exception ex)
            {
                logger.Error("CustomerInfoRepository - UpdateInformation - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return Cm;
        }

        public CustomersReceiveablesManagement DeleteInformation(int CompanyNameId)
        {
            CustomersReceiveablesManagement Cm = new CustomersReceiveablesManagement();
            try
            {
                Cm = _Context.CustomersReceiveablesManagement.Find(CompanyNameId);
                _Context.CustomersReceiveablesManagement.Remove(Cm);
                _Context.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Error("CustomerInfoRepository - DeleteInformation - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return Cm;
        }

        public List<CustomersReceiveablesManagement> GetInformation(int? StoreId)
        {
            List<CustomersReceiveablesManagement> Cm = new List<CustomersReceiveablesManagement>();
            try
            {             
                Cm = _Context.Database.SqlQuery<CustomersReceiveablesManagement>("SP_CustomerInfo @Spara={0},@StoreId={1}", "Select",StoreId).ToList();
            }
            catch (Exception ex)
            {
                logger.Error("CustomerInfoRepository - GetInformation - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return Cm;
        }
    }
}
