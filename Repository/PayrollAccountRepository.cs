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

namespace Repository
{
    public class PayrollAccountRepository : IPayrollAccountRepository
    {
        private DBContext db;
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public PayrollAccountRepository(DBContext context)
        {
            db = context;
        }
        public List<PayrollAccount_Select> GetBindData(string SearchTitle, int StoreId)
        {
            List<PayrollAccount_Select> payrollAccount_Select = new List<PayrollAccount_Select>();
            try
            {
                payrollAccount_Select = db.Database.SqlQuery<PayrollAccount_Select>("SpPayrollCashAnalysis_Search @name = {0},@StoreId = {1}", SearchTitle, StoreId).ToList();
            }
            catch (Exception ex)
            {
                logger.Error("PayrollAccountRepository - GetBindData - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return payrollAccount_Select;
        }
        public void UpdatePRAccount(UpdatePRAccount updatePRAccount)
        {
            try
            {
                // Get the current record being updated
                PayrollCashAnalysis objPayDept = db.payrollCashAnalyses.Find(updatePRAccount.ID);

                if (objPayDept != null)
                {
                    bool isBeingDisabled = !Convert.ToBoolean(updatePRAccount.IsActive) && objPayDept.IsActive == true;
                    int oldSortingNo = objPayDept.NewSortingNo ?? 0;
                    int storeId = (int)objPayDept.StoreId;

                    // SPECIAL CASE: Disabling a record
                    if (isBeingDisabled)
                    {
                        // If the record being disabled has a sorting number, shift down all active records above it
                        if (oldSortingNo > 0)
                        {
                            var recordsToShiftDown = db.payrollCashAnalyses
                                .Where(a => a.StoreId == storeId
                                        && a.PayrollCashAnalysisId != updatePRAccount.ID
                                        && a.IsActive == true // Only shift ACTIVE records
                                        && a.NewSortingNo != null
                                        && a.NewSortingNo > oldSortingNo)
                                .OrderBy(a => a.NewSortingNo)
                                .ToList();

                            // Shift all active records down by 1 to fill the gap
                            foreach (var record in recordsToShiftDown)
                            {
                                record.NewSortingNo = record.NewSortingNo - 1;
                            }
                        }

                        // Clear all fields when disabling
                        objPayDept.DepartmentId = null;
                        objPayDept.ValueIn = null;
                        objPayDept.Description = null;
                        objPayDept.NewSortingNo = null;
                        objPayDept.IsActive = false;
                    }
                    else
                    {
                        // NORMAL UPDATE LOGIC (when not disabling)

                        // Handle null sorting number from JavaScript
                        int? newSortingNo = updatePRAccount.NewSortingNo != null ? Convert.ToInt32(updatePRAccount.NewSortingNo) : (int?)null;

                        // IMPORTANT: Only process if sorting number actually changed
                        if (oldSortingNo != (newSortingNo ?? 0))
                        {
                            // SCENARIO 1: Removing sorting number (set to null)
                            // Example: Had 4, now setting to null → Shift 5→4, 6→5, 7→6
                            if (oldSortingNo > 0 && (newSortingNo == null || newSortingNo == 0))
                            {
                                // Find all ACTIVE records with sorting number > old sorting number
                                var recordsToShiftDown = db.payrollCashAnalyses
                                    .Where(a => a.StoreId == storeId
                                            && a.PayrollCashAnalysisId != updatePRAccount.ID
                                            && a.IsActive == true // Only shift ACTIVE records
                                            && a.NewSortingNo != null
                                            && a.NewSortingNo > oldSortingNo)
                                    .OrderBy(a => a.NewSortingNo)
                                    .ToList();

                                // Shift all records down by 1 to fill the gap
                                foreach (var record in recordsToShiftDown)
                                {
                                    record.NewSortingNo = record.NewSortingNo - 1;
                                }
                            }
                            // SCENARIO 2: Adding or changing sorting number to a new value
                            else if (newSortingNo > 0)
                            {
                                // CHECK IF NEW SORTING NUMBER IS ALREADY TAKEN BY ANOTHER ACTIVE RECORD
                                bool isSortingNumberTaken = db.payrollCashAnalyses
                                    .Any(a => a.StoreId == storeId
                                          && a.PayrollCashAnalysisId != updatePRAccount.ID
                                          && a.IsActive == true // Only check ACTIVE records
                                          && a.NewSortingNo == newSortingNo);

                                // Only shift if the sorting number is already taken by another record
                                if (isSortingNumberTaken)
                                {
                                    // First, if old sorting number existed, shift down records above it to fill the gap
                                    if (oldSortingNo > 0)
                                    {
                                        var recordsToShiftDown = db.payrollCashAnalyses
                                            .Where(a => a.StoreId == storeId
                                                    && a.PayrollCashAnalysisId != updatePRAccount.ID
                                                    && a.IsActive == true // Only shift ACTIVE records
                                                    && a.NewSortingNo != null
                                                    && a.NewSortingNo > oldSortingNo)
                                            .OrderBy(a => a.NewSortingNo)
                                            .ToList();

                                        foreach (var record in recordsToShiftDown)
                                        {
                                            record.NewSortingNo = record.NewSortingNo - 1;
                                        }
                                    }

                                    // Then, shift up records at or above the new sorting number to make space
                                    var conflictingRecords = db.payrollCashAnalyses
                                        .Where(a => a.StoreId == storeId
                                                && a.PayrollCashAnalysisId != updatePRAccount.ID
                                                && a.IsActive == true // Only shift ACTIVE records
                                                && a.NewSortingNo != null
                                                && a.NewSortingNo >= newSortingNo)
                                        .OrderByDescending(a => a.NewSortingNo)
                                        .ToList();

                                    // Shift all conflicting records up by 1
                                    foreach (var record in conflictingRecords)
                                    {
                                        record.NewSortingNo = record.NewSortingNo + 1;
                                    }
                                }
                                else
                                {
                                    // Sorting number is available (not taken)
                                    // Just fill the old gap if it existed
                                    if (oldSortingNo > 0)
                                    {
                                        var recordsToShiftDown = db.payrollCashAnalyses
                                            .Where(a => a.StoreId == storeId
                                                    && a.PayrollCashAnalysisId != updatePRAccount.ID
                                                    && a.IsActive == true // Only shift ACTIVE records
                                                    && a.NewSortingNo != null
                                                    && a.NewSortingNo > oldSortingNo)
                                            .OrderBy(a => a.NewSortingNo)
                                            .ToList();

                                        foreach (var record in recordsToShiftDown)
                                        {
                                            record.NewSortingNo = record.NewSortingNo - 1;
                                        }
                                    }
                                    // No need to shift up - the number is available!
                                }
                            }
                        }
                        // If oldSortingNo == newSortingNo, no sorting logic needed - just update other fields

                        // Update the current record - Normal update logic

                        // QBAccountid - Only update if value provided
                        if (!string.IsNullOrEmpty(updatePRAccount.QBAccountid))
                        {
                            objPayDept.DepartmentId = Convert.ToInt32(updatePRAccount.QBAccountid);
                        }

                        // Description - Allow empty/null
                        objPayDept.Description = updatePRAccount.Description;

                        // ValueIn - Only update if value provided
                        if (!string.IsNullOrEmpty(updatePRAccount.ValueIn))
                        {
                            objPayDept.ValueIn = Convert.ToInt32(updatePRAccount.ValueIn);
                        }

                        // IsActive - Always update
                        objPayDept.IsActive = Convert.ToBoolean(updatePRAccount.IsActive);

                        // NewSortingNo - Can be null
                        objPayDept.NewSortingNo = newSortingNo;
                    }

                    db.Entry(objPayDept).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                logger.Error("PayrollAccountRepository - UpdatePRAccount - " + DateTime.Now + " - " + ex.Message.ToString());
            }
        }
        public List<PayrollBankAccount> GetPayrollBankAccounts(int storeid)
        {
            List<PayrollBankAccount> payrollBankAccount = new List<PayrollBankAccount>();
            try
            {
                PayrollBankAccount obj = db.PayrollBankAccounts.ToList().Where(a => a.StoreId == storeid).FirstOrDefault();
                if (obj == null)
                {
                    obj.StoreId = storeid;
                    db.PayrollBankAccounts.Add(obj);
                    db.SaveChanges();
                }
                payrollBankAccount.Add(obj);
            }
            catch (Exception ex)
            {
                logger.Error("PayrollAccountRepository - GetPayrollBankAccounts - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return payrollBankAccount;
        }
        public void UpdatePayrollBankAccounts(UpdateBankAccount_Setting updateBankAccount_Setting, int storeid)
        {
            try
            {
                PayrollBankAccount data = db.PayrollBankAccounts.Find(updateBankAccount_Setting.ID);
                if (data != null)
                {
                    data.BankAccountId = Convert.ToInt32(updateBankAccount_Setting.QBAccountid);
                    data.VendorId = Convert.ToInt32(updateBankAccount_Setting.VendorId);
                    data.StoreId = storeid;
                    db.Entry(data).State = EntityState.Modified;
                    db.SaveChanges();
                }
                else
                {
                    PayrollBankAccount data1 = new PayrollBankAccount();
                    data.BankAccountId = Convert.ToInt32(updateBankAccount_Setting.QBAccountid);
                    data.VendorId = Convert.ToInt32(updateBankAccount_Setting.VendorId);
                    data1.StoreId = storeid;
                    db.PayrollBankAccounts.Add(data1);
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                logger.Error("PayrollAccountRepository - UpdatePayrollBankAccounts - " + DateTime.Now + " - " + ex.Message.ToString());
            }
        }
    }
}
