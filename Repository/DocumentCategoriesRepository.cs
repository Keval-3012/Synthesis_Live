using EntityModels.Models;
using NLog;
using Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class DocumentCategoriesRepository : IDocumentCategoriesRepository
    {
        private DBContext db;
        Logger logger = LogManager.GetCurrentClassLogger();
        List<DocumentCategory> documentCategories = new List<DocumentCategory>();
        DocumentCategory DocumentCategory = new DocumentCategory();
        public DocumentCategoriesRepository(DBContext context)
        {
            db = context;
        }

        public List<DocumentCategory> GetAllDocumentCategory()
        {
            try
            {
                documentCategories = db.DocumentCategories.ToList();
            }
            catch (Exception ex)
            {
                logger.Error("DocumentCategoriesRepository - GetAllDocumentCategory - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return documentCategories;
        }
        public DocumentCategory GetAllDocumentCategorybyID(int ID)
        {
            try
            {
                DocumentCategory = db.DocumentCategories.Find(Convert.ToInt32(ID));
            }
            catch (Exception ex)
            {
                logger.Error("DocumentCategoriesRepository - GetAllDocumentCategorybyID - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return DocumentCategory;
        }

        public void AddDocumentCategory(DocumentCategory DocumentCategory)
        {
            try
            {
                db.DocumentCategories.Add(DocumentCategory);
                db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                logger.Error("DocumentCategoriesRepository - AddDocumentCategory - " + DateTime.Now + " - " + ex.Message.ToString());
            }
        }

        public void RemoveDocumentCategory(DocumentCategory DocumentCategory)
        {
            try
            {
                db.DocumentCategories.Remove(DocumentCategory);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Error("DocumentCategoriesRepository - RemoveDocumentCategory - " + DateTime.Now + " - " + ex.Message.ToString());
            }
        }

        public void UpdateDocumentCategory(DocumentCategory documentCategory)
        {
            try
            {
                DocumentCategory document = db.DocumentCategories.Find(documentCategory.DocumentCategoryId);
                document.Name = documentCategory.Name;
                document.IsActive = documentCategory.IsActive;
                document.ModifiedBy = documentCategory.ModifiedBy;
                document.ModifiedOn = DateTime.Now;
                db.Entry(document).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Error("DocumentCategoriesRepository - UpdateDocumentCategory - " + DateTime.Now + " - " + ex.Message.ToString());
            }
        }
    }
}