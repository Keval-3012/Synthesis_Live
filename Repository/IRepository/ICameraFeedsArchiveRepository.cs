using EntityModels.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.IRepository
{
    public interface ICameraFeedsArchiveRepository
    {
        List<CameraList> GetWebcamdetails(int StoreIds);
        List<ListWebcamRecordingHistory> GetWebcamRecordingHistoryListID(object CameraID, int StoreIds, string RecordingDate);
        List<ListWebcamRecordingHistory> GetWebcamRecordingHistoryListID1(int CameraID, int? StoreIds);
    }
}
