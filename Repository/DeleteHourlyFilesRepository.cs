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
    public class DeleteHourlyFilesRepository : IDeleteHourlyFilesRepository
    {
        private DBContext _context;
        private IDeleteHourlyFilesRepository _deleteHourlyFilesRepository;
        Logger logger = LogManager.GetCurrentClassLogger();

        public DeleteHourlyFilesRepository(DBContext context)
        {
            _context = context;
        }

        public List<DeleteHourlyFilesModel> DeleteHourlyFList(DeleteHourlyFilesViewModal deleteHourlyFilesViewModal)
        {
            List<DeleteHourlyFilesModel> list = new List<DeleteHourlyFilesModel>();
            try
            {
                list = _context.SalesActivitySummaryHourlies.Where(w => w.StartDate == deleteHourlyFilesViewModal.start_date && w.StoreId == deleteHourlyFilesViewModal.istoreID && w.IsActive == true).Select(s => new
                {
                    SalesActivitySummaryHourlyIDs = s.SalesActivitySummaryHourlyId,
                    FileName = s.FileName,
                    StartDates = s.StartDate,
                    HSequence = s.HSequence
                }).ToList().Select(s => new DeleteHourlyFilesModel
                {
                    Id = s.SalesActivitySummaryHourlyIDs,
                    FileName = s.FileName,
                    StartDate = s.StartDates,
                    HSequence = s.HSequence
                }).ToList();
            }
            catch (Exception ex)
            {
                logger.Error("DeleteHourlyFilesRepository - DeleteHourlyFList - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return list;
        }

        public List<StoreMaster> GetNameStoreMaster()
        {
            List<StoreMaster> obj = new List<StoreMaster>();
            try
            {
                obj = _context.StoreMasters.ToList();
            }
            catch (Exception ex)
            {
                logger.Error("DeleteHourlyFilesRepository - GetNameStoreMaster - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return obj;
        }

        public async Task<SalesActivitySummaryHourly> GetSalesSummaryId(int? id)
        {
            SalesActivitySummaryHourly obj = new SalesActivitySummaryHourly();
            try
            {
                obj = await _context.SalesActivitySummaryHourlies.FindAsync(id);
                _context.Database.ExecuteSqlCommand("Delete_HoulyFiles_Proc @ActicvityID = {0}", id);
            }
            catch (Exception ex)
            {
                logger.Error("DeleteHourlyFilesRepository - GetSalesSummaryId - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return obj;
        }
    }
}
