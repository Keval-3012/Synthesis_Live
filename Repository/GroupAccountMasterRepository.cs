using EntityModels.Models;
using NLog;
using Repository.IRepository;
using SynthesisQBOnline.QBClass;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class GroupAccountMasterRepository : IGroupAccountMasterRepository
    {
        private DBContext db;
        Logger logger = LogManager.GetCurrentClassLogger();

        public GroupAccountMasterRepository(DBContext context)
        {
            db = context;
        }

        public void DeleteConfigGroupById(int Id)
        {
            try
            {
                ConfigurationGroup Data = db.ConfigurationGroups.Find(Id);
                db.ConfigurationGroups.Remove(Data);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Error("GroupAccountMasterRepository - DeleteConfigGroupById - " + DateTime.Now + " - " + ex.Message.ToString());
            }

        }

        public List<ConfigurationGroup> GetConfigurationGroup(int storeid)
        {
            List<ConfigurationGroup> configurationGroup = new List<ConfigurationGroup>();
            try
            {
                configurationGroup = db.ConfigurationGroups.Where(s => s.StoreId == storeid).Select(s => new
                {
                    ConfigurationGroupId = s.ConfigurationGroupId,
                    GroupName = s.GroupName,
                    DepartmentName = s.DepartmentMasters.DepartmentName,
                    DepartmentId = s.DepartmentId,
                    TypicalBalanceId = s.TypicalBalanceId,
                    Memo = s.Memo,
                    TypicalBalanceName = db.TypicalBalanceMasters.Where(a => a.TypicalBalanceId == s.TypicalBalanceId).FirstOrDefault().TypicalBalanceName,
                    Exist = 0,
                    VendorId = s.VendorId,
                    CustomerId = s.CustomerId,
                    Entity = (s.CustomerId != 0 ? db.CustomerMasters.Where(a => a.CustomerId == s.CustomerId).FirstOrDefault().DisplayName : (s.VendorId != 0 ? db.VendorMasters.Where(a => a.VendorId == s.VendorId).FirstOrDefault().VendorName : ""))
                }).ToList()
                .Select(s => new ConfigurationGroup
                {
                    ConfigurationGroupId = s.ConfigurationGroupId,
                    GroupName = s.GroupName,
                    DepartmentName = s.DepartmentName,
                    DepartmentId = s.DepartmentId,
                    TypicalBalanceId = s.TypicalBalanceId,
                    TypicalBalanceName = s.TypicalBalanceName,
                    Memo = s.Memo,
                    VendorId = s.VendorId,
                    CustomerId = s.CustomerId,
                    Entity = s.Entity,
                    Exist = 0
                }).ToList();
            }
            catch (Exception ex)
            {
                logger.Error("GroupAccountMasterRepository - GetConfigurationGroup - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return configurationGroup;
        }

        public ConfigurationGroup GetConfigurationGroupDataById(int Id)
        {
            ConfigurationGroup configurationGroup = new ConfigurationGroup();
            try
            {
                configurationGroup = db.ConfigurationGroups.Find(Id);
            }
            catch (Exception ex)
            {
                logger.Error("GroupAccountMasterRepository - GetConfigurationGroupDataById - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return configurationGroup;
        }

        public string SaveGroupMaster(string Name, string QBAccountid, string Typicalbalid, string memo, string EntityVal,int storeid)
        {
            string message = "";
            try
            {
                string exist = Convert.ToString((from Data in db.ConfigurationGroups.Where(s => s.StoreId == storeid)
                                                 where Data.GroupName == Name
                                                 select Data.ConfigurationGroupId).FirstOrDefault());
                if (Convert.ToInt32(exist) == 0)
                {
                    int Idd = Convert.ToInt32(QBAccountid);
                    string Id = (from a in db.DepartmentMasters
                                 where a.DepartmentId == Idd
                                 select a.AccountDetailTypeId).FirstOrDefault().ToString();

                    ConfigurationGroup Data_Insert = new ConfigurationGroup();
                    Data_Insert.GroupName = Name;
                    Data_Insert.DepartmentId = Convert.ToInt32(QBAccountid);
                    Data_Insert.TypicalBalanceId = db.TypicalBalanceMasters.Where(a => a.TypicalBalanceId.ToString() == Typicalbalid).FirstOrDefault().TypicalBalanceId;
                    Data_Insert.Memo = memo;
                    Data_Insert.StoreId = storeid;
                    if (Id == "49")
                    {
                        Data_Insert.CustomerId = null;
                        Data_Insert.VendorId = Convert.ToInt32(EntityVal);
                    }
                    else if (Id == "1")
                    {
                        Data_Insert.CustomerId = Convert.ToInt32(EntityVal);
                        Data_Insert.VendorId = null;
                    }
                    else
                    {
                        Data_Insert.CustomerId = null;
                        Data_Insert.VendorId = null;
                    }

                    db.ConfigurationGroups.Add(Data_Insert);
                    db.SaveChanges();
                    message = "sucess";
                }
                else
                {
                    message = "Exists";
                }
            }
            catch (Exception ex)
            {
                logger.Error("GroupAccountMasterRepository - SaveGroupMaster - " + DateTime.Now + " - " + ex.Message.ToString());
            }

            return message;
        }

        public string UpdateGroupMaster(int ID, string Name, string QBAccountid, string Typicalbalid, string memo, string EntityVal, int storeid)
        {
            string message = "";
            try
            {
                string exist = Convert.ToString((from Data in db.ConfigurationGroups
                                                 where Data.GroupName == Name && Data.StoreId == storeid && Data.ConfigurationGroupId != ID
                                                 select Data.ConfigurationGroupId).FirstOrDefault());
                if (Convert.ToInt32(exist) == 0)
                {
                    int Idd = Convert.ToInt32(QBAccountid);
                    string Id = (from a in db.DepartmentMasters
                                 where a.DepartmentId == Idd
                                 select a.AccountDetailTypeId).FirstOrDefault().ToString();

                    ConfigurationGroup Groupname_data = db.ConfigurationGroups.Find(ID);

                    Groupname_data.GroupName = Name;
                    Groupname_data.DepartmentId = Convert.ToInt32(QBAccountid);
                    Groupname_data.TypicalBalanceId = db.TypicalBalanceMasters.Where(a => a.TypicalBalanceId.ToString() == Typicalbalid).FirstOrDefault().TypicalBalanceId;
                    Groupname_data.Memo = memo;
                    if (Id == "49")
                    {
                        Groupname_data.CustomerId = null;
                        Groupname_data.VendorId = Convert.ToInt32(EntityVal);
                    }
                    else if (Id == "1")
                    {
                        Groupname_data.CustomerId = Convert.ToInt32(EntityVal);
                        Groupname_data.VendorId = null;
                    }
                    else
                    {
                        Groupname_data.CustomerId = null;
                        Groupname_data.VendorId = null;
                    }
                    db.Entry(Groupname_data).State = EntityState.Modified;
                    db.SaveChanges();
                    message = "Edit";


                    Configuration objConfig = db.Configurations.Where(a => a.ConfigurationGroupId == ID && a.StoreId == storeid).FirstOrDefault();
                    if (objConfig != null)
                    {
                        objConfig.TypicalBalanceId = Convert.ToInt32(Typicalbalid);
                        objConfig.Memo = memo;
                        objConfig.DepartmentId = Convert.ToInt32(QBAccountid);
                        db.Entry(objConfig).State = EntityState.Modified;
                        db.SaveChanges();
                    }

                    Departmentconfiguration objDept = db.Departmentconfigurations.Where(a => a.ConfigurationGroupId == ID && a.StoreId == storeid).FirstOrDefault();
                    if (objDept != null)
                    {
                        objDept.TypicalBalanceId = Convert.ToInt32(Typicalbalid);
                        objDept.Memo = memo;
                        objDept.DepartmentId = Convert.ToInt32(QBAccountid);
                        db.Entry(objDept).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }
                else
                {
                    message = "Exists";
                }
            }
            catch (Exception ex)
            {
                logger.Error("GroupAccountMasterRepository - UpdateGroupMaster - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return message;
        }

        public List<OtherDepositeSetting> OtherDepositeAccount(int StoreId)
        {
            List<OtherDepositeSetting> RtnData = new List<OtherDepositeSetting>();
            try
            {
                CheckOtherDeposite_Exist("Rebate", StoreId);
                CheckOtherDeposite_Exist("Other", StoreId);

                RtnData = db.OtherDepositeSettings.Where(s => s.StoreId == StoreId).ToList();
            }
            catch (Exception ex)
            {
                logger.Error("GroupAccountMasterRepository - OtherDepositeAccount - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return RtnData;
        }

        public void CheckOtherDeposite_Exist(string Type, int StoreID)
        {
            try
            {
                OtherDepositeSetting obj = db.OtherDepositeSettings.Where(a => a.StoreId == StoreID && a.Name == Type).FirstOrDefault();
                if (obj == null)
                {
                    OtherDepositeSetting objs = new OtherDepositeSetting();
                    objs.Name = Type;
                    objs.StoreId = StoreID;
                    objs.BankAccountId = null;
                    db.OtherDepositeSettings.Add(objs);
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                logger.Error("GroupAccountMasterRepository - CheckOtherDeposite_Exist - " + DateTime.Now + " - " + ex.Message.ToString());
            }

        }

        public void SyncVendors(List<SynthesisQBOnline.BAL.VendorMaster> dtVendor, int StoreID, int UserId) {
            try
            {
                foreach (var item in dtVendor)
                {
                    var name = item.DisplayName;
                    var vData1 = db.Database.SqlQuery<VendorMaster>("SP_VendorMaster @Mode = {0},@StoreId = {1},@VendorName = {2}", "SelectByStoreID_Name", StoreID, item.DisplayName).FirstOrDefault();
                    if (vData1 != null)
                    {
                        VendorMaster objVendor = db.VendorMasters.Where(a => a.VendorId == vData1.VendorId && vData1.StoreId == StoreID).FirstOrDefault();
                        objVendor.ListId = item.ID;
                        objVendor.VendorName = item.DisplayName;
                        objVendor.PhoneNumber = item.Mobile;
                        objVendor.CompanyName = item.CompanyName;
                        objVendor.PrintOnCheck = item.PrintOnCheckas;
                        objVendor.IsActive = Convert.ToBoolean(item.IsActive);
                        objVendor.Address = item.Address1;
                        objVendor.City = item.City;
                        objVendor.State = item.State;
                        objVendor.Country = item.Country;
                        objVendor.PostalCode = item.ZipCode;
                        objVendor.ModifiedBy = UserId;
                        objVendor.ModifiedOn = DateTime.Now;
                        objVendor.StoreId = StoreID;
                        objVendor.IsSync = true;
                        db.Entry(objVendor).State = EntityState.Modified;
                        db.SaveChanges();
                        objVendor = null;

                    }
                    else
                    {
                        if (item.IsActive == "true")
                        {
                            var vData = db.Database.SqlQuery<VendorMaster>("SP_VendorMaster @Mode = {0} ,@StoreId = {1},@ListId = {2}", "SelectByStoreID_ListID", StoreID, item.ID).FirstOrDefault();
                            if (vData != null)
                            {
                                var objVendor = db.VendorMasters.Where(a => a.VendorId == vData.VendorId && a.StoreId == StoreID).FirstOrDefault();
                                objVendor.ListId = item.ID;
                                objVendor.VendorName = item.DisplayName;
                                objVendor.PhoneNumber = item.Mobile;
                                objVendor.CompanyName = item.CompanyName;
                                objVendor.PrintOnCheck = item.PrintOnCheckas;
                                objVendor.IsActive = Convert.ToBoolean(item.IsActive);
                                objVendor.Address = item.Address1;
                                objVendor.City = item.City;
                                objVendor.State = item.State;
                                objVendor.Country = item.Country;
                                objVendor.PostalCode = item.ZipCode;
                                objVendor.ModifiedBy = UserId;
                                objVendor.ModifiedOn = DateTime.Now;
                                objVendor.StoreId = StoreID;
                                objVendor.IsSync = true;
                                db.Entry(objVendor).State = EntityState.Modified;
                                db.SaveChanges();
                                objVendor = null;
                            }
                            else
                            {
                                VendorMaster objVendor = new VendorMaster();
                                objVendor.ListId = item.ID;
                                objVendor.VendorName = item.DisplayName;
                                objVendor.PhoneNumber = item.Mobile;
                                objVendor.CompanyName = item.CompanyName;
                                objVendor.PrintOnCheck = item.PrintOnCheckas;
                                objVendor.IsActive = Convert.ToBoolean(item.IsActive);
                                objVendor.Address = item.Address1;
                                objVendor.City = item.City;
                                objVendor.State = item.State;
                                objVendor.Country = item.Country;
                                objVendor.PostalCode = item.ZipCode;
                                objVendor.CreatedBy = UserId;
                                objVendor.CreatedOn = DateTime.Now;
                                objVendor.StoreId = StoreID;
                                objVendor.ModifiedOn = DateTime.Now;
                                objVendor.IsSync = true;
                                objVendor.SyncDate = DateTime.Now;
                                db.VendorMasters.Add(objVendor);
                                db.SaveChanges();
                                objVendor = null;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("GroupAccountMasterRepository - SyncVendors - " + DateTime.Now + " - " + ex.Message.ToString());
            }
        }

        public void SyncDepartment(List<SynthesisQBOnline.BAL.CustomerMaster> dtCustomer, int StoreID, int UserId)
        {
            try
            {
                foreach (var item in dtCustomer)
                {
                    var name = item.DisplayName;
                    var vData1 = db.Database.SqlQuery<CustomerMaster>("SP_CustomerMaster @Mode = {0},@StoreId = {1},@CustomerName = {2}", "SelectByStoreID_Name", StoreID, item.DisplayName).FirstOrDefault();
                    if (vData1 != null)
                    {
                        CustomerMaster objCust = db.CustomerMasters.Where(a => a.CustomerId == vData1.CustomerId && vData1.StoreId == StoreID).FirstOrDefault();
                        objCust.ListID = item.ID;
                        objCust.DisplayName = item.DisplayName;
                        objCust.PrimaryPhone = item.PrimaryPhone;
                        objCust.CompanyName = item.CompanyName;
                        objCust.PrintOnCheckName = item.PrintOnCheckName;
                        objCust.BAddress1 = item.BAddress1;
                        objCust.BAddress2 = item.BAddress2;
                        objCust.BCity = item.BCity;
                        objCust.BState = item.BState;
                        objCust.BCountry = item.BCountry;
                        objCust.BZipCode = item.BZipCode;
                        objCust.StoreId = StoreID;
                        objCust.PrimaryEmailAddr = item.PrimaryEmailAddr;
                        objCust.FirstName = item.FirstName;
                        objCust.LastName = item.LastName;
                        objCust.Active = Convert.ToBoolean(item.Active);
                        objCust.MiddleName = item.MiddleName;
                        objCust.Notes = item.Notes;
                        objCust.Balance = item.Balance;

                        db.Entry(objCust).State = EntityState.Modified;
                        db.SaveChanges();
                        objCust = null;
                    }
                    else
                    {
                        if (item.Active == "true")
                        {
                            var vData = db.Database.SqlQuery<VendorMaster>("SP_CustomerMaster @Mode = {0} ,@StoreId = {1},@ListId = {2}", "SelectByStoreID_ListID", StoreID, item.ID).FirstOrDefault();
                            if (vData != null)
                            {
                                CustomerMaster objCust = db.CustomerMasters.Where(a => a.CustomerId == vData1.CustomerId && vData1.StoreId == StoreID).FirstOrDefault();
                                objCust.ListID = item.ID;
                                objCust.DisplayName = item.DisplayName;
                                objCust.PrimaryPhone = item.PrimaryPhone;
                                objCust.CompanyName = item.CompanyName;
                                objCust.PrintOnCheckName = item.PrintOnCheckName;
                                objCust.BAddress1 = item.BAddress1;
                                objCust.BAddress2 = item.BAddress2;
                                objCust.BCity = item.BCity;
                                objCust.BState = item.BState;
                                objCust.BCountry = item.BCountry;
                                objCust.BZipCode = item.BZipCode;
                                objCust.StoreId = StoreID;
                                objCust.PrimaryEmailAddr = item.PrimaryEmailAddr;
                                objCust.FirstName = item.FirstName;
                                objCust.LastName = item.LastName;
                                objCust.Active = Convert.ToBoolean(item.Active);
                                objCust.MiddleName = item.MiddleName;
                                objCust.Notes = item.Notes;
                                objCust.Balance = item.Balance;

                                db.Entry(objCust).State = EntityState.Modified;
                                db.SaveChanges();
                                objCust = null;
                            }
                            else
                            {
                                CustomerMaster objCust = new CustomerMaster();
                                objCust.ListID = item.ID;
                                objCust.DisplayName = item.DisplayName;
                                objCust.PrimaryPhone = item.PrimaryPhone;
                                objCust.CompanyName = item.CompanyName;
                                objCust.PrintOnCheckName = item.PrintOnCheckName;
                                objCust.BAddress1 = item.BAddress1;
                                objCust.BAddress2 = item.BAddress2;
                                objCust.BCity = item.BCity;
                                objCust.BState = item.BState;
                                objCust.BCountry = item.BCountry;
                                objCust.BZipCode = item.BZipCode;
                                objCust.StoreId = StoreID;
                                objCust.PrimaryEmailAddr = item.PrimaryEmailAddr;
                                objCust.FirstName = item.FirstName;
                                objCust.LastName = item.LastName;
                                objCust.Active = Convert.ToBoolean(item.Active);
                                objCust.MiddleName = item.MiddleName;
                                objCust.Notes = item.Notes;
                                objCust.Balance = item.Balance;
                                objCust.CreatedBy = UserId.ToString();
                                objCust.CreatedOn = DateTime.Now;
                                objCust.StoreId = StoreID;
                                db.CustomerMasters.Add(objCust);
                                db.SaveChanges();
                                objCust = null;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("GroupAccountMasterRepository - SyncDepartment - " + DateTime.Now + " - " + ex.Message.ToString());
            }
        }          

        public string UpdateOtherDeposite_Setting(int ID, int QBAccountid, int StoreId)
        {
            string message = "";
            try
            {
                OtherDepositeSetting data = db.OtherDepositeSettings.Find(ID);
                data.BankAccountId = Convert.ToInt32(QBAccountid);
                db.Entry(data).State = EntityState.Modified;
                db.SaveChanges();
                message = "Edit";
            }
            catch (Exception ex)
            {
                logger.Error("GroupAccountMasterRepository - UpdateOtherDeposite_Setting - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return message;
        }

        public List<ddllist> GetCustomerVendor(string QBId, int StoreId)
        {
            List<ddllist> LstQBAccount = new List<ddllist>();
            try
            {
                int QBIdval = Convert.ToInt32(QBId);
                string Id = (from a in db.DepartmentMasters.Where(a => a.StoreId == StoreId)
                             where a.DepartmentId == QBIdval
                             select a.AccountDetailTypeId).FirstOrDefault().ToString();

                if (Id == "49") //Account Payable
                {
                    LstQBAccount = (from a in db.VendorMasters
                                    where a.StoreId.ToString() == StoreId.ToString() && a.IsActive == true
                                    orderby a.VendorName ascending
                                    select new ddllist
                                    {
                                        Value = a.VendorId.ToString(),
                                        Text = a.VendorName
                                    }).ToList();
                }
                else if (Id == "1") //Account AccountsReceivable
                {
                    LstQBAccount = (from a in db.CustomerMasters
                                    where a.StoreId.ToString() == StoreId.ToString() && a.Active == true
                                    orderby a.DisplayName ascending
                                    select new ddllist
                                    {
                                        Value = a.CustomerId.ToString(),
                                        Text = a.DisplayName
                                    }).ToList();
                }
            }
            catch (Exception ex)
            {
                logger.Error("GroupAccountMasterRepository - GetCustomerVendor - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return LstQBAccount;
        }
    }
}
