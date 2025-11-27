using EntityModels.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.IRepository
{
    public interface IDocumentsRepository
    {
        List<Document> DocumentsList();
        List<DocumentCategory> DocumentCategoriesList();
        List<StoreMaster> StoreMastersList();
        void SaveDocuments(Document document);
        void SaveDocumentKeyword(DocumentKeyword DKObj);
        void SaveDocumentFavorite(DocumentFavorite DFObj);
        Task<Document> GetDocumentsById(int? id);
        StoreMaster StoreMastersId(int StoreId);
        Document DocumentsId(int DocumentId);
        void UpdateDocuments(Document document);
        void DocumentKeywordRemove(int DocumentId);
        List<DocumentFavorite> DocumentFavoritesList();
        Document GetDocumentsId(int? id);
        Document DeleteDetailDocument(int Id);
        void UpdateDocumentFavorite(DocumentFavorite data);
        Document GetDocumentsIdString(string DocumentId);
        void SaveAttachFile(string AttachFile, int DocumentId);
        DocumentFavorite GetDocumentFavoritesobj(int DocId, int userid);
    }
}
