using EntityModels.Models;
using NLog;
using Repository.IRepository;
using SynthesisViewModal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class ExpenseReportRepository : IExpenseReportRepository
    {
        private DBContext db;
        Logger logger = LogManager.GetCurrentClassLogger();
        public ExpenseReportRepository(DBContext context)
        {
            db = context;
        }
        public List<ExpenseCheckDetail> GetExpenseDeptIds()
        {
            List<ExpenseCheckDetail> obj = new List<ExpenseCheckDetail>();
            try
            {
                obj = db.ExpenseCheckDetails.ToList();
            }
            catch (Exception ex)
            {
                logger.Error("ExpenseReportRepository - GetExpenseDeptIds - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return obj;
        }
        public List<RightsStore> GetRightsStore()
        {
            List<RightsStore> obj = new List<RightsStore>();
            try
            {
                obj = db.RightsStores.ToList();
            }
            catch (Exception ex)
            {
                logger.Error("ExpenseReportRepository - GetRightsStore - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return obj;
        }
        public List<UserTypeMaster> GetUserTypeMasters()
        {
            List<UserTypeMaster> obj = new List<UserTypeMaster>();
            try
            {
                obj = db.UserTypeMasters.ToList();
            }
            catch (Exception ex)
            {
                logger.Error("ExpenseReportRepository - GetUserTypeMasters - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return obj;
        }
        public List<ExpenseCheckSelect> GetExpenseCheckSelect(ExpenseReportViewModal expenseReportViewModal)
        {
            List<ExpenseCheckSelect> obj = new List<ExpenseCheckSelect>();
            try
            {
                obj = db.Database.SqlQuery<ExpenseCheckSelect>("SP_ExpenseReportData @startdate={0},@enddate={1},@Payment_type={2},@Dept_id={3},@Store_id={4},@IsStatus_id={5},@VendorName={6},@AmtMaximum={7},@AmtMinimum={8}",
                    expenseReportViewModal.start_date, expenseReportViewModal.end_date, expenseReportViewModal.payment, expenseReportViewModal.deptname, expenseReportViewModal.istoreID, expenseReportViewModal.Status, expenseReportViewModal.VendorName, expenseReportViewModal.AmtMaximum, expenseReportViewModal.AmtMinimum).ToList();
            }
            catch (Exception ex)
            {
                logger.Error("ExpenseReportRepository - GetExpenseCheckSelect - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return obj;
        }
    }
}
