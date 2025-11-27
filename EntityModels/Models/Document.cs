using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace EntityModels.Models
{
    public class Document
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Editable(false)]
        public int DocumentId { get; set; }

        [MaxLength(1000)]
        [Required]
        public string Title { get; set; }

        public int DocumentCategoryId { get; set; }
        [ForeignKey("DocumentCategoryId")]
        public virtual DocumentCategory DocumentCategories { get; set; }

        public int StoreId { get; set; }
        [ForeignKey("StoreId")]
        public virtual StoreMaster StoreMasters { get; set; }

        [DataType(DataType.Text)]
        public string Notes { get; set; }

        [DataType(DataType.Text)]
        public string Description { get; set; }

        [MaxLength(200)]
        public string FilePath { get; set; }

        [MaxLength(100)]
        public string AttachFile { get; set; }

        [MaxLength(100)]
        public string AttachExtention { get; set; }

        public int? CreatedBy { get; set; }

        public DateTime? CreatedOn { get; set; }

        public int? ModifiedBy { get; set; }

        public DateTime? ModifiedOn { get; set; }

        public bool IsPrivate { get; set; }

        public bool IsDelete { get; set; }

        [NotMapped]
        public string chkPrivate { get; set; }

        [NotMapped]
        public string chkFav { get; set; }

        [NotMapped]
        public string strErrMessage { get; set; }

        [NotMapped]
        public bool IsFavorite { get; set; }

        [NotMapped]
        public string KeyWords { get; set; }
        [NotMapped]
        public string StoreName { get; set; }

        [NotMapped]
        public string CategoryName { get; set; }
        [NotMapped]
        public string FileTypeImage { get; set; }
        [NotMapped]
        public string CreatedDateFormated { get; set; }
        [NotMapped]
        public string Type { get; set; }
        [NotMapped]
        public string AttachLink { get; set; }

        [NotMapped]
        public int IsStatus_id { get; set; }
        [NotMapped]
        public int FavId { get; set; }

        [NotMapped]
        public int FavUserId { get; set; }

        [NotMapped]
        public string CreatedByName { get; set; }
        [NotMapped]
        public string ModifyByName { get; set; }

        public virtual ICollection<DocumentKeyword> DocumentKeywords { get; set; }
        public virtual ICollection<DocumentFavorite> DocumentFavorites { get; set; }
    }
    public class DocumentKeyword
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Editable(false)]
        public int DocumentKeywordId { get; set; }

        public int DocumentId { get; set; }
        [ForeignKey("DocumentId")]
        public virtual Document Documents { get; set; }

        [MaxLength(1000)]
        [Required]
        public string Title { get; set; }

        public bool IsActive { get; set; }
    }

    public class DocumentFavorite
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Editable(false)]
        public int DocumentFavoriteId { get; set; }

        public int DocumentId { get; set; }
        [ForeignKey("DocumentId")]
        public virtual Document Documents { get; set; }

        public int UserId { get; set; }

        public DateTime? CreatedOn { get; set; }

        public bool IsFavorite { get; set; }
    }

    public class FilesPathInfo
    {
        public string text;

    }

    public class ExportData
    {
        public string fileName;
        public string documentData;
    }

    public class DocumentBulk
    {
        public HttpPostedFileBase[] File { get; set; }

        public string[] item { get; set; }
        //public HttpPostedFileBase[] File { get; set; }
    }
}