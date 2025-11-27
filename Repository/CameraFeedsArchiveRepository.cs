using EntityModels.Models;
using NLog;
using Repository.IRepository;
using SynthesisViewModal;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class CameraFeedsArchiveRepository : ICameraFeedsArchiveRepository
    {
        private DBContext _context;
        Logger logger = LogManager.GetCurrentClassLogger();

        public CameraFeedsArchiveRepository(DBContext context)
        {
            _context = context;
        }

        public List<CameraList> GetWebcamdetails(int StoreIds)
        {
            List<CameraList> obj = new List<CameraList>();
            try
            {
                obj = _context.Database.SqlQuery<CameraList>("GetWebcamRecordingHistory @Mode = {0},@StoreID={1}", "GetWebcamdetails", StoreIds).ToList();
            }
            catch (Exception ex)
            {
                logger.Error("CameraFeedsArchiveRepository - GetWebcamdetails - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return obj;
        }

        public List<ListWebcamRecordingHistory> GetWebcamRecordingHistoryListID(object CameraID, int StoreIds, string RecordingDate)
        {
            List<ListWebcamRecordingHistory> obj = new List<ListWebcamRecordingHistory>();
            try
            {
                obj = _context.Database.SqlQuery<ListWebcamRecordingHistory>("GetWebcamRecordingHistory @Mode = {0},@CameraID={1},@StoreID={2},@recordingdatetime={3}", "GetWebcamRecordingHistoryListID", CameraID, StoreIds, RecordingDate).ToList();
            }
            catch (Exception ex)
            {
                logger.Error("CameraFeedsArchiveRepository - GetWebcamRecordingHistoryListID - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return obj;
        }

        public List<ListWebcamRecordingHistory> GetWebcamRecordingHistoryListID1(int CameraID, int? StoreIds)
        {
            List<ListWebcamRecordingHistory> obj = new List<ListWebcamRecordingHistory>();
            try
            {
                obj = _context.Database.SqlQuery<ListWebcamRecordingHistory>("GetWebcamRecordingHistory @Mode = {0},@CameraID={1},@StoreID={2}", "GetWebcamRecordingHistoryListID", 0, StoreIds).ToList();
            }
            catch (Exception ex)
            {
                logger.Error("CameraFeedsArchiveRepository - GetWebcamRecordingHistoryListID1 - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return obj;
        }
    }
}
