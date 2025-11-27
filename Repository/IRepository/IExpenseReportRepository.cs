using EntityModels.Models;
using SynthesisViewModal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.IRepository
{
    public interface IExpenseReportRepository
    {
        List<ExpenseCheckDetail> GetExpenseDeptIds();
        List<RightsStore> GetRightsStore();
        List<UserTypeMaster> GetUserTypeMasters();
        List<ExpenseCheckSelect> GetExpenseCheckSelect(ExpenseReportViewModal expenseReportViewModal);
    }
}
