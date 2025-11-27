using EntityModels.Models;
using SynthesisViewModal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.IRepository
{
    public interface ITerminalRepository
    {
        DateTime GetTransactionStartTimeDesc();
        List<OtherDepositData> GetOtherDepositDatas(TerminalViewModel MainTerminalView);
        void GetTotalAvrage(ref Terminal_Select terminal_Select, TerminalViewModel terminal,int StoreID);
        void GetTerminalData(ref Terminal_Select obj, TerminalViewModel model);
        void GetSelectedVendorList(int StoreID, ref Terminal_Select terminal_Select);
        int GetUnassignShiftCount(int StoreID,string Date);
        DayCloseOutStatus GetDayCloseOutStatus_Data(int StoreID, string Date);
        void SetShiftDataGridValue(ref Terminal_Select obj, TerminalViewModel model);
        string GetStoreTerminalName(TerminalViewModel model);
        void GetShiftWisetenderData(ref Terminal_Select terminal_Select, TerminalViewModel model);
        void SetCraditCardDetail(ref Terminal_Select terminal_Select, TerminalViewModel model);
        void SetOtherDepositeData(ref Terminal_Select terminal_Select, TerminalViewModel model);
        bool CheckSalesActivitySummariesExistOrNot(ref Terminal_Select Posteddata, ref TerminalViewModel terminal);
        void UpdateCreditcardDetails(ref Terminal_Select Posteddata, ref TerminalViewModel terminal);
        void SaveOtherDeposit(ref TerminalViewModel terminal);
        int SetDepositeData(ref Terminal_Select terminal_Select, TerminalViewModel terminal);
        void SetOptionList(ref Terminal_Select terminal_Select);
        void SetShiftNameList(ref Terminal_Select terminal_Select);
        void Deleteotherdepositdata(TerminalViewModel terminal);
        string SavePaidOutSettlement(TerminalSettlement terminal);
        void DeleteSettlementEntry(TerminalViewModel terminal);
        List<ddllist> GetPayDetailList(string Id);
        string SetDayCloseOut(TerminalViewModel terminal);
        List<TenderAccountsStoreWise> getTenderAccountsStoreWise(TerminalViewModel terminal);
        void DeleteTenderEntry(TerminalViewModel terminal);
        void DeleteotherdepositFile(TerminalViewModel terminal);
        string GetSalesGeneralEntries_Data(TerminalViewModel terminal);
        CreateInvoiceDetail SetCaseInvoice(TerminalViewModel terminal);
        CreateInvoiceDetail UploadFileExtantionInvalid(TerminalViewModel terminal);
        Task<TerminalViewModel> CreateInvoice(TerminalViewModel terminal);
        Task<TerminalViewModel> CreateInvoiceWithoutQuickInvoice(TerminalViewModel terminal);
        Task<TerminalViewModel> UpdateInvoice(TerminalViewModel terminal);
        string GetVendorName(int VendorId);
        string DeleteCashInvoice(int Id);
        Invoice GetInvoiceByID(int InvoiceID);
        SalesOtherDeposite GetSalesOtherDepositeByID(int OtherDepositeID);
        void UpdateOtherDeposit(ref TerminalViewModel model);
        DateTime? GetPreviousDayCount(TerminalViewModel terminal);
        string GetPaidOutAmountMessage(TerminalViewModel terminal);
        string GetCCOfflineAmountMessage(TerminalViewModel terminal);
        string GetLastThirtydaysClosedOut(TerminalViewModel terminal);
    }
}
