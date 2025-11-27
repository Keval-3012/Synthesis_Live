using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using EntityModels.HRModels;

namespace EntityModels.Models
{
    [Table("UserMaster")]
    public class UserMaster
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Editable(false)]
        public int UserId { get; set; }

        [Required(ErrorMessage =" ")]
        public int UserTypeId { get; set; }
        [ForeignKey("UserTypeId")]
        public virtual UserTypeMaster UserTypeMasters { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = " ")]
        [RegularExpression(@"^([a-zA-Z ]{3,})*$", ErrorMessage = "Minimum 3 letters,No Number or special character")]
        public string FirstName { get; set; }

        //[Required(AllowEmptyStrings = false, ErrorMessage = "Enter LastName")]
        //[RegularExpression(@"^([a-zA-Z ]{3,})*$", ErrorMessage = "Minimum 3 letters,No Number or special character")]
        //public string LastName { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = " ")]
        [RegularExpression(@"^([A-Za-z0-9-@#&+\w\s]{3,})+$", ErrorMessage = "Minimum 3 letters, User Name can contain only this special characters like   @ # & _ - +   ")]
        public string UserName { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = " ")]
        [DataType(DataType.Password)]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z])(?=\S+$).{8,25}$", ErrorMessage = "Required at least 8 characters, 1 capital letter, 1 special character, and 1 number without spaces.")]
        //[RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,25}$", ErrorMessage = "Required at least 8 characters,1 capital and 1 special letter,1 number")]
        public string Password { get; set; }

        [NotMapped]
        [Required(AllowEmptyStrings = false, ErrorMessage = " ")]
        [Compare("Password", ErrorMessage = "Password doesn't match.")]
        public string ConfirmPassword { get; set; }

        public int CreatedBy { get; set; }

        public int ModifiedBy { get; set; }

        public DateTime? CreatedOn { get; set; }

        public DateTime? ModifiedOn { get; set; }
        public Boolean IsActive { get; set; }
        public Boolean TrackHours { get; set; }

        public string EmailAddress { get; set; }


        public int? GroupId { get; set; }
        [ForeignKey("GroupId")]
        public virtual GroupMaster GroupMasters { get; set; }

        public bool IsFirstLogin { get; set; }
        public bool IsHRAdmin { get; set; }
        public bool IsHRSuperAdmin { get; set;}        
        public string CustomPassword { get; set; }
        public string PhoneNumber { get; set; }
        public Boolean ForWhatsAppNotification { get; set; }
        public Boolean IsProductScanApp {  get; set; }
        public string StoreAccess { get; set; }        
        public string DataStoreAccess { get; set; }        
        public int UserRightsforStoreAccess { get; set; }
        public int? GroupWiseStateStoreId { get; set; }
        public Boolean ProductImageUpload { get; set; }
        public Boolean UpdateProductDetails { get; set; }
        public Boolean IsAbleExpiryChange { get; set; }
        public Language LanguageId { get; set; }
        public Boolean IsCustomCompetitors { get; set; }
        public string CompetitorsId { get; set; }
        public int DesignatedStore { get; set; }
        public string FCMTokenApp { get; set; }
        public string FCMTokenWeb { get; set; }
        public string PlayerId { get; set; }
        public string DeviceType { get; set; }
        public int? DesignatedPageId { get; set; }
        [ForeignKey("MenuId")]
        public virtual MenuMaster MenuMasters { get; set; }
        public bool IsOnline { get; set; }
        [NotMapped]
        public string[] MuiltiStoreAccess { get; set; }
        [NotMapped]
        public string[] MuiltiStoreAccessData { get; set; }
        [NotMapped]
        public string[] StoreName { get; set; }
        [NotMapped]
        public string[] CompaniesCompetitors { get; set; }
        [NotMapped]
        public string UserType { get; set; }
        [NotMapped]
        public string Group { get; set; }
        [NotMapped]
        //[Required(AllowEmptyStrings = false, ErrorMessage = " ")]
        public string OldPassword { get; set; }

        public virtual ICollection<StoreChild> StoreChilds { get; set; }
        public virtual ICollection<SalesGeneralEntries> salesGeneralEntries { get; set; }
        public virtual ICollection<SalesGeneralEntriesHistory> salesGeneralEntriesHistory { get; set; }
        public virtual ICollection<UserWiseStickyNote> UserWiseStickyNotes { get; set; }
        
        //changes 02-07-2025 by himanshu
        public virtual ICollection<ChatReactions> chatReactions { get; set; }
        public virtual ICollection<ChatMessenger> senderChatMessenger { get; set; }
        public virtual ICollection<ChatMessenger> receiverChatMessenger { get; set; }

        public virtual ICollection<ChatGroupMembers> chatGroupMembers { get; set; }
    }

    public class User_Resetpassword
    {
        public int Reg_userid { get; set; }
        [Required(ErrorMessage = " ")]

        [DataType(DataType.Password)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,15}$", ErrorMessage = "Password required at least 8 characters, 1 capital letter, 1 number, and one special character")]
        public string Password { get; set; }

        [Required(ErrorMessage = " ")]
        [Compare("Password", ErrorMessage = "Password and confirm password doesn't match.")]
        public string ConfirmPassword { get; set; }
    }

    public class UserList
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
    }

    public class UserRoleStoreList
    {
        public int StoreId { get; set;}
        public int UserId { get; set;}
        public string MergeId { get; set;}
        public string StoreName { get; set; }
        public string ModuleName { get; set; }
        public string Roles { get; set; }
    }
}