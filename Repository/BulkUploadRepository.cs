using EntityModels.Models;
using NLog;
using Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class BulkUploadRepository : IBulkUploadRepository
    {
        private DBContext _context;
        protected static bool IsArray;
        Logger logger = LogManager.GetCurrentClassLogger();
        public BulkUploadRepository(DBContext context)
        {
            _context = context;
        }

        public void SaveFileChanges(UploadPdf uploadPDF)
        {
            try
            {
                _context.UploadPdfs.Add(uploadPDF);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Error("BulkUploadRepository - SaveFileChanges - " + DateTime.Now + " - " + ex.Message.ToString());
            }
        }
        public void RemoveFileChanges(UploadPdf uploadPDF)
        {
            try
            {
                var UploadPdfObj = _context.UploadPdfs.FirstOrDefault(p => p.UploadPdfId == uploadPDF.UploadPdfId);
                _context.UploadPdfs.Remove(UploadPdfObj);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Error("BulkUploadRepository - SaveFileChanges - " + DateTime.Now + " - " + ex.Message.ToString());
            }
        }

        public List<UploadFile> StoreProcess(string Search, int StoreId)
        {
            List<UploadFile> uploadFile = new List<UploadFile>();
            try
            {
                uploadFile = _context.Database.SqlQuery<UploadFile>("SP_UploadPDF @Mode", new SqlParameter("@Mode", "Select")).Where(a => (a.StoreName.Contains(Search) && a.IsProcess == 0) && a.StoreId == StoreId).ToList();
            }
            catch (Exception ex)
            {
                logger.Error("BulkUploadRepository - StoreProcess - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return uploadFile;
        }

        public List<UploadFile> WithoutStoreProcess(string Search)
        {
            List<UploadFile> uploadFile = new List<UploadFile>();
            try
            {
                uploadFile = _context.Database.SqlQuery<UploadFile>("SP_UploadPDF @Mode", new SqlParameter("@Mode", "Select")).Where(a => (a.StoreName.Contains(Search) && a.IsProcess == 0)).ToList();

            }
            catch (Exception ex)
            {
                logger.Error("BulkUploadRepository - WithoutStoreProcess - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return uploadFile;
        }

        public List<UploadFile> WithStoreProcess(int StoreId)
        {
            List<UploadFile> uploadFile = new List<UploadFile>();
            try
            {
                uploadFile = _context.Database.SqlQuery<UploadFile>("SP_UploadPDF @Mode", new SqlParameter("@Mode", "Select")).Where(a => a.IsProcess == 0 && a.StoreId == StoreId).ToList();

            }
            catch (Exception ex)
            {
                logger.Error("BulkUploadRepository - WithStoreProcess - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return uploadFile;
        }

        public List<UploadFile> WithStoreProcessing()
        {
            List<UploadFile> uploadFile = new List<UploadFile>();
            try
            {
                uploadFile = _context.Database.SqlQuery<UploadFile>("SP_UploadPDF @Mode", new SqlParameter("@Mode", "Select")).Where(a => a.IsProcess == 0).ToList();

            }
            catch (Exception ex)
            {
                logger.Error("BulkUploadRepository - WithStoreProcess - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return uploadFile;
        }

        public List<UploadPdf> UploadPdf()
        {
            List<UploadPdf> uploadPdf = new List<UploadPdf>();
            try
            {

            }
            catch (Exception ex)
            {
                logger.Error("BulkUploadRepository - UploadPdf - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return uploadPdf;
        }
        public async void UploadFileChanges(int? id)
        {
            try
            {
                 await _context.UploadPdfs.FindAsync(id);
            }
            catch (Exception ex)
            {
                logger.Error("BulkUploadRepository - SaveFileChanges - " + DateTime.Now + " - " + ex.Message.ToString());
            }
        }
        public List<UploadFile> GetUploadFiles(int StoreID,int userid,List<StoreMaster> StoreList,List<UserMaster> UserMaster) 
        {
            List<UploadFile> RtnData = new List<UploadFile>();
            try
            {
                if (userid == 0)
                {
                    if (StoreID != 0)
                    {
                        RtnData = _context.UploadPdfs.Where(s => s.StoreId == StoreID).Select(s => new
                        {
                            UploadPdfId = s.UploadPdfId,
                            FileName = s.FileName,
                            StoreId = s.StoreId,
                            CreatedDate = s.CreatedDate,
                            CreatedBy = s.CreatedBy
                        }).ToList().Select(s => new UploadFile
                        {
                            UploadPdfId = s.UploadPdfId,
                            FileName = s.FileName,
                            StoreName = StoreList.Where(w => w.StoreId == s.StoreId).Select(o => o.NickName).FirstOrDefault(),
                            StoreId = s.StoreId,
                            CreatedBy = s.CreatedBy,
                            CreatedName = UserMaster.Where(w => w.UserId == s.CreatedBy).Select(o => o.FirstName).FirstOrDefault(),
                            CreatedDates = s.CreatedDate.ToString("MM/dd/yyyy"),
                        }).ToList();
                    }
                    else
                    {
                        RtnData = _context.UploadPdfs.Select(s => new
                        {
                            UploadPdfId = s.UploadPdfId,
                            FileName = s.FileName,
                            StoreId = s.StoreId,
                            CreatedDate = s.CreatedDate,
                            CreatedBy = s.CreatedBy
                        }).ToList().Select(s => new UploadFile
                        {
                            UploadPdfId = s.UploadPdfId,
                            FileName = s.FileName,
                            StoreName = StoreList.Where(w => w.StoreId == s.StoreId).Select(o => o.NickName).FirstOrDefault(),
                            StoreId = s.StoreId,
                            CreatedBy = s.CreatedBy,
                            CreatedName = UserMaster.Where(w => w.UserId == s.CreatedBy).Select(o => o.FirstName).FirstOrDefault(),
                            CreatedDates = s.CreatedDate.ToString("MM/dd/yyyy"),
                        }).ToList();
                    }
                }
                else 
                {
                    if (StoreID != 0)
                    {
                        RtnData = _context.UploadPdfs.Where(s => s.StoreId == StoreID && s.CreatedBy == userid).Select(s => new
                        {
                            UploadPdfId = s.UploadPdfId,
                            FileName = s.FileName,
                            StoreId = s.StoreId,
                            CreatedDate = s.CreatedDate,
                            CreatedBy = s.CreatedBy
                        }).ToList().Select(s => new UploadFile
                        {
                            UploadPdfId = s.UploadPdfId,
                            FileName = s.FileName,
                            StoreName = StoreList.Where(w => w.StoreId == s.StoreId).Select(o => o.NickName).FirstOrDefault(),
                            StoreId = s.StoreId,
                            CreatedBy = s.CreatedBy,
                            CreatedName = UserMaster.Where(w => w.UserId == s.CreatedBy).Select(o => o.FirstName).FirstOrDefault(),
                            CreatedDates = s.CreatedDate.ToString("MM/dd/yyyy"),
                        }).ToList();
                    }
                    else
                    {
                        RtnData = _context.UploadPdfs.Where(s => s.CreatedBy == userid).Select(s => new
                        {
                            UploadPdfId = s.UploadPdfId,
                            FileName = s.FileName,
                            StoreId = s.StoreId,
                            CreatedDate = s.CreatedDate,
                            CreatedBy = s.CreatedBy
                        }).ToList().Select(s => new UploadFile
                        {
                            UploadPdfId = s.UploadPdfId,
                            FileName = s.FileName,
                            StoreName = StoreList.Where(w => w.StoreId == s.StoreId).Select(o => o.NickName).FirstOrDefault(),
                            StoreId = s.StoreId,
                            CreatedBy = s.CreatedBy,
                            CreatedName = UserMaster.Where(w => w.UserId == s.CreatedBy).Select(o => o.FirstName).FirstOrDefault(),
                            CreatedDates = s.CreatedDate.ToString("MM/dd/yyyy"),
                        }).ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("BulkUploadRepository - GetUploadFiles - " + DateTime.Now + " - " + ex.Message.ToString());
            }

            return RtnData;
        }
    }
}
