using EntityModels.Models;
using NLog;
using Repository.IRepository;
using SynthesisQBOnline.BAL;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility;

namespace Repository
{
    public class JournalEntriesRepository : IJournalEntriesRepository
    {
        private DBContext _context;
        Logger logger = LogManager.GetCurrentClassLogger();

        public JournalEntriesRepository(DBContext context)
        {
            _context = context;
        }

        public void delete(int Id)
        {
            try
            {
                SalesGeneralEntries salesGeneralEntries = _context.salesGeneralEntries.Find(Id);
                _context.dayCloseOutStatuses.Remove(_context.dayCloseOutStatuses.Where(s => s.StartDate == salesGeneralEntries.SalesDate && s.StoreId == salesGeneralEntries.StoreId).FirstOrDefault());
                _context.SalesOtherDeposites.RemoveRange(_context.SalesOtherDeposites.Where(s => s.SalesGeneralId == Id));
                _context.salesGeneralEntries.Remove(salesGeneralEntries);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Error("JournalEntriesRepository - Delete - " + DateTime.Now + " - " + ex.Message.ToString());
            }
        }

        public GeneralEntriesDetail DetailList_generalEntries(int id)
        {
            List<JournalEntriesList> list = new List<JournalEntriesList>();
            GeneralEntriesDetail Detail = new GeneralEntriesDetail();
            try
            {
                
                Detail.JournalEntriesLists = _context.salesChildEntries.Where(s => s.SalesGeneralId == id).ToList().
                                     Select(dt => new JournalEntriesList
                                     {
                                         CloseOutDate = dt.salesGeneralEntries.SalesDate,
                                         CreatedDate = dt.salesGeneralEntries.CreatedDate,
                                         id = id,
                                         TotalAmount = (dt.Amount < 0 ? dt.Amount * -1 : dt.Amount),
                                         UserName = dt.salesGeneralEntries.UserMasters.FirstName,
                                         Typeid = (dt.Amount < 0 ? (Convert.ToInt32(dt.TypeId) == 1 ? 2 : 1) : dt.TypeId),
                                         Memo = dt.Memo,
                                         ChildSalesId = dt.SalesChildId,
                                         CloseOutDateformat = AdminSiteConfiguration.GetDateDisplay(Convert.ToString(dt.salesGeneralEntries.SalesDate)),
                                         DeptName = dt.DepartmentMasters == null ? "" : dt.DepartmentMasters.DepartmentName.Replace("&amp;", "&"),
                                         Title = dt.Title,
                                         StoreID = dt.salesGeneralEntries.StoreId,
                                         NotConfigureAccount = GetNotConfigureAccount(Convert.ToInt32(dt.salesGeneralEntries.StoreId)),
                                         Sign = (dt.Amount < 0 ? 2 : 1)
                                     }).ToList();
                int StoreID = Detail.JournalEntriesLists.Count() > 0 ? Convert.ToInt32(Detail.JournalEntriesLists.FirstOrDefault().StoreID) : 0;
                var QBFlag = _context.QBOnlineConfigurations.Where(a => a.StoreId == StoreID).FirstOrDefault();
                if (QBFlag != null)
                {
                    if (QBFlag.IsTokenSuspend == 1)
                    {
                        if (Detail.JournalEntriesLists.Count > 0)
                        {
                            Detail.JournalEntriesLists.FirstOrDefault().QBStatusField = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("JournalEntriesRepository - DetailList_generalEntries - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return Detail;
        }
        
        public string GetNotConfigureAccount(int StoreID)
        {
            string Acc = "";
            try
            {
                var AccList = _context.Database.SqlQuery<TenderAccountsStoreWise>("SP_GetTenderAccountStoreWise @Storeid={0}", StoreID).ToList().Where(a => a.GroupId == null && a.DepartmentId == null && a.Flag != 0).ToList();
                foreach (var item in AccList)
                {
                    Acc = (Acc == "" ? item.Title : Acc + ", " + item.Title);
                }
            }
            catch (Exception ex) 
            {
                logger.Error("JournalEntriesRepository - GetNotConfigureAccount - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return Acc;
        }
        
        public void Detail_generalEntries(GeneralEntriesDetail posteddata)
        {
            try
            {
                if (posteddata.JournalEntriesLists.Count > 0)
                {

                    for (int i = 0; i < posteddata.JournalEntriesLists.Count; i++)
                    {
                        SalesChildEntries obj = _context.salesChildEntries.Find(posteddata.JournalEntriesLists[i].ChildSalesId);
                        if (posteddata.JournalEntriesLists[i].Sign == 2)
                        {
                            obj.Amount = posteddata.JournalEntriesLists[i].TotalAmount.Value * -1;
                        }
                        else
                        {
                            obj.Amount = posteddata.JournalEntriesLists[i].TotalAmount.Value;
                        }
                        _context.Entry(obj).State = EntityState.Modified;
                        _context.SaveChanges();

                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("JournalEntriesRepository - Detail_generalEntries - " + DateTime.Now + " - " + ex.Message.ToString());
            }
        }

        public void Entry_SalesgeneralEntry(SalesGeneralEntries data, string Id)
        {
            try
            {
                data.TxnId = Id;
                data.IsSync = 1;
                data.Status = 3;
                _context.Entry(data).State = EntityState.Modified;
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Error("JournalEntriesRepository - Entry_SalesgeneralEntry - " + DateTime.Now + " - " + ex.Message.ToString());
            }
        }

        public DbRawSqlQuery<JournalEntryDetail> GeneralEntryDetail(int salesgeneralId)
        {
            DbRawSqlQuery<JournalEntryDetail> data = null;
            
            try
            {
                data = _context.Database.SqlQuery<JournalEntryDetail>("GeneralEntryDetail @SalesGeneralId={0}", salesgeneralId);
            }
            catch (Exception ex)
            {
                logger.Error("JournalEntriesRepository - GeneralEntryDetail - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return data;
        }

        public DbRawSqlQuery<ChildOtherDepositeList> GetChildOtherDeposite(int Id)
        {
            DbRawSqlQuery<ChildOtherDepositeList> data = null;
            try
            {
                data = _context.Database.SqlQuery<ChildOtherDepositeList>("GetChildOtherDeposite @SalesGeneralId={0}", Id);
            }
            catch (Exception ex)
            {
                logger.Error("JournalEntriesRepository - GetChildOtherDeposite - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return data;
        }

        public IEnumerable GetSalesGeneralEntries_Detail(string strStore, DateTime? salesdate)
        {
            IEnumerable RtnData = null;
            try
            {
                //RtnData = _context.Database.SqlQuery<SalesGeneralEntries>("GetSalesGeneralEntries_Detail @storeid={0},@salesdate={1}", strStore, salesdate);
                RtnData = _context.Database.SqlQuery<SalesGeneralEntriesSyncfusion>("GetSalesGeneralEntries_Detail @storeid={0},@salesdate={1}", strStore, salesdate);
            }
            catch (Exception ex)
            {
                logger.Error("JournalEntriesRepository - GetSalesGeneralEntries_Detail - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return RtnData;
        }

        public void QBSync_PaymentType(List<PaymentMethod> dtPayment, int StoreId)
        {
            try
            {
                foreach (var item in dtPayment)
                {
                    QBPaymentType obj = new QBPaymentType();
                    obj.PaymentType = item.Name;
                    obj.ListId = item.Id;
                    obj.StoreId = StoreId;
                    obj.IsActive = Convert.ToBoolean(item.Active);
                    _context.QBPaymentTypes.Add(obj);
                    _context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                logger.Error("JournalEntriesRepository - QBSync_PaymentType - " + DateTime.Now + " - " + ex.Message.ToString());
            }
        }

        public SalesGeneralEntries SalesEntryFindById(int Id)
        {
            SalesGeneralEntries data = new SalesGeneralEntries();
            try
            {
                data = _context.salesGeneralEntries.Find(Id);
            }
            catch (Exception ex)
            {
                logger.Error("JournalEntriesRepository - SalesEntryFindById - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return data;
        }

        public DateTime? salesGeneralEntries(int storeid)
        {
            SalesGeneralEntries data = new SalesGeneralEntries();
            DateTime? date = null;
            try
            {
                date = _context.salesGeneralEntries.Where(s => s.StoreId == storeid).OrderByDescending(x => x.SalesDate).Select(x => x.SalesDate).FirstOrDefault();
            }
            catch (Exception ex)
            {
                logger.Error("JournalEntriesRepository - SalesGeneralEntries - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return date;
        }

        public void SalesOtherDepositesbyId(int SalesOtherDepositeID, string ID)
        {
            SalesOtherDeposite data = new SalesOtherDeposite();
            try
            {
                SalesOtherDeposite Datas = _context.SalesOtherDeposites.Find(SalesOtherDepositeID);
                Datas.TxnId = ID;
                Datas.IsSync = 1;
                Datas.Status = 3;

                _context.Entry(Datas).State = EntityState.Modified;
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Error("JournalEntriesRepository - SalesOtherDepositesbyId - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            
        }

        public GeneralEntriesDetail DetailsHttpPost(GeneralEntriesDetail posteddata)
        {
            GeneralEntriesDetail Detail = new GeneralEntriesDetail();
            try
            {
                int ID = Convert.ToInt32(posteddata.JournalEntriesLists[0].ChildSalesId);
                Detail.JournalEntriesLists = _context.salesChildEntries.Where(s => s.SalesGeneralId == ID).ToList().
                                         Select(dt => new JournalEntriesList
                                         {
                                             CloseOutDate = dt.salesGeneralEntries.SalesDate,
                                             CreatedDate = dt.salesGeneralEntries.CreatedDate,
                                             id = ID,
                                             TotalAmount = dt.Amount,
                                             UserName = dt.salesGeneralEntries.UserMasters.FirstName,
                                             Typeid = dt.TypeId,
                                             Memo = dt.Memo,
                                             ChildSalesId = dt.SalesChildId,
                                             CloseOutDateformat = AdminSiteConfiguration.GetDateDisplay(Convert.ToString(dt.salesGeneralEntries.SalesDate)),
                                             DeptName = dt.DepartmentMasters == null ? "" : dt.DepartmentMasters.DepartmentName.Replace("&amp;", "&"),
                                             Title = dt.Title
                                         }).ToList();
            }
            catch (Exception ex)
            {
                logger.Error("JournalEntriesRepository - DetailsHttpPost - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return Detail;
        }
    }
}
