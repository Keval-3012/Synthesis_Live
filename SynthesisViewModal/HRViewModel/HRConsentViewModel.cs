using EntityModels.HRModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;

namespace SynthesisViewModal.HRViewModel
{
    public class HRConsentViewModel
    {
        public int ConsentId { get; set; }
        public int FormTypeId { get; set; }
        public string FormType { get; set; }
        public int? CreatedBy { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? Created { get; set; }
        public DateTime? Updated { get; set; }
    }
    public class ConsentModel
    {
        public long ConsentId { get; set; }
        public Nullable<int> FormTypeId { get; set; }
        public Nullable<int> LanguageId { get; set; }
        public Nullable<int> TypeId { get; set; }
        public Nullable<long> CreatedBy { get; set; }
        public Nullable<long> UpdatedBy { get; set; }
        public Nullable<System.DateTime> Created { get; set; }
        public Nullable<System.DateTime> Updated { get; set; }

        public string FormType { get; set; }
        public List<ConsentModelDetail> ConsentModelDetails { get; set; }
    }
    public class ConsentModelDetail
    {
        public long DetailId { get; set; }
        public int ConsentId { get; set; }
        public Language? Language { get; set; }
        public string LanguageName
        {
            get
            {
                if (Language.HasValue)
                {
                    return Enum.GetName(typeof(Language), Language.Value);
                }
                return null;
            }
        }
        public InputTypes? Type { get; set; }
        public string TypeName
        {
            get
            {
                if (Type.HasValue)
                {
                    return Enum.GetName(typeof(InputTypes), Type.Value);
                }
                return null;
            }
        }
        public string Detail { get; set; }

        public string LanguageType { get; set; }
        public string Types { get; set; }

        public int? LanguageId { get; set; }
        public int? TypeId { get; set; }

    }
    public partial class SPConsentDetail_Result
    {
        public int DetailId { get; set; }
        public Nullable<int> ConsentId { get; set; }
        public Language Language { get; set; }
        public InputTypes Type { get; set; }
        public string Detail { get; set; }
        public string LanguageType { get; set; }
        public string Types { get; set; }
    }

    public class ConsentStatusListModel
    {
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public int? SignCheck { get; set; }
        public int? StoreId { get; set; }
        public int EmployeeDocumentId { get; set; }
        public int? EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string DepartmentName { get; set; }
        public string CreatedDate { get; set; }
        public string FileName { get; set; }
    }
}
