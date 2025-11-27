using EntityModels.Models;
using NLog;
using Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Repository
{
    public class ConfigurationRepository : IConfigurationRepository
    {
        private DBContext db;
        Logger logger = LogManager.GetCurrentClassLogger();
        public ConfigurationRepository(DBContext context)
        {
            db = context;
        }
        public List<Configurationlist> getConfigrationlist(int storeid)
        {
            List<DepartmentNetSales> departmentNetSales = new List<DepartmentNetSales>();
            List<Configurationlist> Configurationlist = new List<Configurationlist>();
            try
            {
                departmentNetSales = db.DepartmentNetSales.Where(s => s.SalesActivitySummary.StoreId == storeid).ToList();
                Configurationlist = db.Database.SqlQuery<SpConfigurationListData>("SP_GetTenderAccountStoreWise @storeId = {0}", storeid).ToList().Select(s => new Configurationlist
                {                    
                    Groupname = s.GroupId.ToString(),
                    groupid = s.GroupId,
                    Storeid = storeid,
                    typicalbalid = s.typicalbalid,
                    typicalbalance = s.typicalBalName,
                    Deptid = s.deptid,
                    flag = s.flag,
                    GroupList = (from a in db.ConfigurationGroups
                                 where a.StoreId.ToString() == storeid.ToString()
                                 orderby a.GroupName ascending
                                 select new DrpList
                                 {
                                     Id = a.ConfigurationGroupId,
                                     Name = a.GroupName.Replace("&amp;", "&")
                                 }).ToList(),
                    DeptSalesList = (from a in departmentNetSales
                                     select new DrpList
                                     {
                                         Id = 0,
                                         Name = a.ToString().Replace("&amp;", "&")
                                     }).ToList(),
                    Deptname = s.DeptName,
                    memo = s.Memo,
                    tendername = s.Title,
                    ggID = s.GroupId
                }).ToList();
            }
            catch (Exception ex)
            {
                logger.Error("ConfigurationRepository - GetConfigrationlist - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return Configurationlist;
        }
        public void SaveDepartmentconfigurations(int IID, int Deptid, int Flag, string TenderName, int StoreID, string memoidval, int typicalbalid)
        {
            try
            {                
                if (IID != 0 && Deptid != 0)
                {
                    if (Flag == 2)
                    {
                        // Handle Departmentconfiguration
                        var existingConfig = db.Departmentconfigurations
                            .FirstOrDefault(s => s.Title == TenderName && s.StoreId == StoreID);

                        if (existingConfig != null)
                        {
                            // Update existing record
                            existingConfig.DepartmentId = Deptid;
                            existingConfig.StoreId = StoreID;
                            existingConfig.ConfigurationGroupId = IID;
                            existingConfig.Memo = memoidval;
                            existingConfig.TypicalBalanceId = typicalbalid;
                            existingConfig.Title = TenderName;
                            db.Entry(existingConfig).State = EntityState.Modified;
                        }
                        else
                        {
                            // Create new record
                            var newConfig = new Departmentconfiguration
                            {
                                DepartmentId = Deptid,
                                StoreId = StoreID,
                                ConfigurationGroupId = IID,
                                Memo = memoidval,
                                TypicalBalanceId = typicalbalid,
                                Title = TenderName
                            };
                            db.Departmentconfigurations.Add(newConfig);
                        }
                        db.SaveChanges();
                    }
                    else
                    {
                        // Handle Configuration (Flag == 0 or 1)
                        var existingConfig = db.Configurations
                            .FirstOrDefault(s => s.Title == TenderName && s.StoreId == StoreID);

                        if (existingConfig != null)
                        {
                            // Update existing record
                            existingConfig.DepartmentId = Deptid;
                            existingConfig.StoreId = StoreID;
                            existingConfig.ConfigurationGroupId = IID;
                            existingConfig.Memo = memoidval;
                            existingConfig.TypicalBalanceId = typicalbalid;
                            existingConfig.Title = TenderName;
                            db.Entry(existingConfig).State = EntityState.Modified;
                        }
                        else
                        {
                            // Create new record
                            var newConfig = new Configuration
                            {
                                DepartmentId = Deptid,
                                StoreId = StoreID,
                                ConfigurationGroupId = IID,
                                Memo = memoidval,
                                TypicalBalanceId = typicalbalid,
                                Title = TenderName
                            };
                            db.Configurations.Add(newConfig);
                        }
                        db.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("ConfigurationRepository - SaveDepartmentconfigurations - " + DateTime.Now + " - " + ex.Message.ToString());
            }
        }
        public List<ConfigurationGroupData> getGroupDatabygroupid(int groupval)
        {
            List<ConfigurationGroupData> configurationGroupDatas = new List<ConfigurationGroupData>();
            try
            {
                configurationGroupDatas = (from a in db.ConfigurationGroups
                                           where a.ConfigurationGroupId == groupval
                                           select new ConfigurationGroupData
                                           {
                                               Deptname = (from b in db.DepartmentMasters where b.DepartmentId == a.DepartmentId select b.DepartmentName).FirstOrDefault(),
                                               DeptId = a.DepartmentId,
                                               memo = a.Memo,
                                               typicalBalId = a.TypicalBalanceId,
                                               typicalbalance = db.TypicalBalanceMasters.Where(b => b.TypicalBalanceId == a.TypicalBalanceId).FirstOrDefault().TypicalBalanceName.ToString()
                                           }).ToList();
            }
            catch (Exception ex)
            {
                logger.Error("ConfigurationRepository - GetGroupDatabygroupid - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return configurationGroupDatas;
        }
        public bool ResetConfiguration(string tenderName, int storeId)
        {
            try
            {
                // First check Department configurations
                var deptEntity = db.Departmentconfigurations.FirstOrDefault(x => x.Title == tenderName && x.StoreId == storeId);

                if (deptEntity != null)
                {
                    db.Departmentconfigurations.Remove(deptEntity);
                    db.SaveChanges();
                    return true;
                }

                var configEntity = db.Configurations.FirstOrDefault(x => x.Title == tenderName && x.StoreId == storeId);
                if (configEntity != null)
                {
                    db.Configurations.Remove(configEntity);
                    db.SaveChanges();
                    return true;
                }
            }
            catch (Exception ex)
            {
                logger.Error("ConfigurationRepository - ResetConfiguration - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return false;
        }
    }
}
