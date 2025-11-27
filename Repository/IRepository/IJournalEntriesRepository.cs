using EntityModels.Models;
using SynthesisQBOnline.BAL;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.IRepository
{
    public interface IJournalEntriesRepository
    {
        DateTime? salesGeneralEntries(int storeid);
        IEnumerable GetSalesGeneralEntries_Detail(string strStore, DateTime? salesdate);
        SalesGeneralEntries SalesEntryFindById(int Id);
        DbRawSqlQuery<JournalEntryDetail> GeneralEntryDetail(int salesgeneralId);
        void Entry_SalesgeneralEntry(SalesGeneralEntries data, string Id);
        DbRawSqlQuery<ChildOtherDepositeList> GetChildOtherDeposite(int Id);
        void SalesOtherDepositesbyId(int SalesOtherDepositeID, string ID);
        void delete(int Id);
        void QBSync_PaymentType(List<PaymentMethod> dtPayment, int StoreId);
        void Detail_generalEntries(GeneralEntriesDetail posteddata);
        GeneralEntriesDetail DetailList_generalEntries(int id);
        GeneralEntriesDetail DetailsHttpPost(GeneralEntriesDetail posteddata);
    } 
}
