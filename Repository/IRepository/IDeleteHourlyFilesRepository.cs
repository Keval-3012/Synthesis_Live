using EntityModels.Models;
using SynthesisViewModal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.IRepository
{
    public interface IDeleteHourlyFilesRepository
    {
        List<StoreMaster> GetNameStoreMaster();
        List<DeleteHourlyFilesModel> DeleteHourlyFList(DeleteHourlyFilesViewModal deleteHourlyFilesViewModal);
        Task<SalesActivitySummaryHourly> GetSalesSummaryId(int? id);
    }
}
