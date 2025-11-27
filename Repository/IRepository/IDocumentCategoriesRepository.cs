using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityModels.Models;
namespace Repository.IRepository
{
    public interface IDocumentCategoriesRepository
    {
        List<DocumentCategory> GetAllDocumentCategory();
        DocumentCategory GetAllDocumentCategorybyID(int ID);
        void AddDocumentCategory(DocumentCategory DocumentCategory);
        void UpdateDocumentCategory(DocumentCategory DocumentCategory);
        void RemoveDocumentCategory(DocumentCategory DocumentCategory);
    }
}
