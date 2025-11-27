using EntityModels.Models;
using NLog;
using Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Repository
{
    public class DocumentsRepository : IDocumentsRepository
    {
        private DBContext _context;
        Logger logger = LogManager.GetCurrentClassLogger();

        public DocumentsRepository(DBContext context)
        {
            _context = context;
        }

        public List<Document> DocumentsList()
        {
            List<Document> obj = new List<Document>();
            try
            {
                obj = _context.Documents.ToList();
            }
            catch (Exception ex)
            {
                logger.Error("DocumentsRepository - DocumentsList - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return obj;
        }

        public List<DocumentCategory> DocumentCategoriesList()
        {
            List<DocumentCategory> obj = new List<DocumentCategory>();
            try
            {
                obj = _context.DocumentCategories.ToList();
            }
            catch (Exception ex)
            {
                logger.Error("DocumentsRepository - DocumentCategoriesList - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return obj;
        }

        public List<StoreMaster> StoreMastersList()
        {
            List<StoreMaster> obj = new List<StoreMaster>();
            try
            {
                obj = _context.StoreMasters.ToList();
            }
            catch (Exception ex)
            {
                logger.Error("DocumentsRepository - StoreMastersList - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return obj;
        }

        public void SaveDocuments(Document document)
        {
            try
            {
                _context.Documents.Add(document);
                _context.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                logger.Error("DocumentsRepository - SaveDocuments - " + DateTime.Now + " - " + ex.Message.ToString());
            }
        }

        public void SaveDocumentKeyword(DocumentKeyword DKObj)
        {
            try
            {
                _context.DocumentKeywords.Add(DKObj);
                _context.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                logger.Error("DocumentsRepository - SaveDocumentKeyword - " + DateTime.Now + " - " + ex.Message.ToString());
            }
        }

        public void SaveDocumentFavorite(DocumentFavorite DFObj)
        {
            try
            {
                _context.DocumentFavorites.Add(DFObj);
                _context.SaveChanges();

            }
            catch (Exception ex)
            {
                logger.Error("DocumentsRepository - SaveDocumentFavorite - " + DateTime.Now + " - " + ex.Message.ToString());
            }
        }

        public async Task<Document> GetDocumentsById(int? id)
        {
            Document obj = new Document();
            try
            {
                obj = await _context.Documents.FindAsync(id);

            }
            catch (Exception ex)
            {
                logger.Error("DocumentsRepository - GetDocumentsById - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return obj;
        }

        public StoreMaster StoreMastersId(int StoreId)
        {
            StoreMaster obj = new StoreMaster();
            try
            {
                obj = _context.StoreMasters.Where(x => x.StoreId == StoreId).AsNoTracking().FirstOrDefault();
            }
            catch (Exception ex)
            {
                logger.Error("DocumentsRepository - StoreMastersId - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return obj;
        }

        public Document DocumentsId(int DocumentId)
        {
            Document obj = new Document();
            try
            {
                obj = _context.Documents.Where(x => x.DocumentId == DocumentId).AsNoTracking().FirstOrDefault();
            }
            catch (Exception ex)
            {
                logger.Error("DocumentsRepository - DocumentsId - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return obj;
        }

        public void UpdateDocuments(Document document)
        {
            try
            {
                _context.Entry(document).State = EntityState.Modified;
                _context.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                logger.Error("DocumentsRepository - UpdateDocuments - " + DateTime.Now + " - " + ex.Message.ToString());
            }
        }

        public void DocumentKeywordRemove(int DocumentId)
        {
            try
            {
                _context.DocumentKeywords.RemoveRange(_context.DocumentKeywords.Where(s => s.DocumentId == DocumentId));
            }
            catch (Exception ex)
            {
                logger.Error("DocumentsRepository - DocumentKeywordRemove - " + DateTime.Now + " - " + ex.Message.ToString());
            }
        }

        public List<DocumentFavorite> DocumentFavoritesList()
        {
            List<DocumentFavorite> obj = new List<DocumentFavorite>();
            try
            {
                obj = _context.DocumentFavorites.ToList();
            }
            catch (Exception ex)
            {
                logger.Error("DocumentsRepository - DocumentFavoritesList - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return obj;
        }
        public DocumentFavorite GetDocumentFavoritesobj(int DocId, int userid)
        {
            DocumentFavorite obj = new DocumentFavorite();
            try
            {
                obj = _context.DocumentFavorites.Where(x => x.DocumentId == DocId && x.UserId == userid).FirstOrDefault();
            }
            catch (Exception ex)
            {
                logger.Error("DocumentsRepository - DocumentFavoritesList - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return obj;
        }

        public Document GetDocumentsId(int? id)
        {
            Document obj = new Document();
            try
            {
                obj = _context.Documents.Find(id);
            }
            catch (Exception ex)
            {
                logger.Error("DocumentsRepository - GetDocumentsId - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return obj;
        }

        public Document DeleteDetailDocument(int Id)
        {
            Document obj = new Document();
            try
            {
                obj = _context.Documents.Where(x => x.DocumentId == Id).FirstOrDefault();
                obj.IsDelete = true;
                _context.SaveChanges();

            }
            catch (Exception ex)
            {
                logger.Error("DocumentsRepository - DeleteDetailDocument - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return obj;
        }

        public void UpdateDocumentFavorite(DocumentFavorite data)
        {
            try
            {
                _context.Entry(data).State = System.Data.Entity.EntityState.Modified;
                _context.SaveChanges();

            }
            catch (Exception ex)
            {
                logger.Error("DocumentsRepository - UpdateDocumentFavorite - " + DateTime.Now + " - " + ex.Message.ToString());
            }
        }

        public Document GetDocumentsIdString(string DocumentId)
        {
            Document obj = new Document();
            try
            {
                obj = _context.Documents.Find(Convert.ToInt32(DocumentId));
            }
            catch (Exception ex)
            {
                logger.Error("DocumentsRepository - GetDocumentsIdString - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return obj;
        }

        public void SaveAttachFile(string AttachFile, int DocumentId)
        {
            try
            {
                var LastData = _context.Documents.Where(s => s.DocumentId == DocumentId).FirstOrDefault();
                LastData.AttachFile = AttachFile + "_" + DocumentId.ToString();
                _context.SaveChanges();

            }
            catch (Exception ex)
            {
                logger.Error("DocumentsRepository - SaveAttachFile - " + DateTime.Now + " - " + ex.Message.ToString());
            }
        }
    }
}
