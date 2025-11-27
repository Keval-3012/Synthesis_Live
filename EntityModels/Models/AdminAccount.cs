using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EntityModels.Models
{
    public class Admin_Login
    {
        [Required(ErrorMessage ="Please Enter User Name....")]
        [Display(Name = "Username")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Please Enter Password....")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me next time.")]
        public bool RememberMe { get; set; }

        public string Message { get; set; }
    }

    public class AdminChangePasswordModel
    {

        [Required(ErrorMessage = " * Required")]
        [DataType(DataType.Password)]
        [Display(Name = "Current password")]
        public string OldPassword { get; set; }

        [Required(ErrorMessage = " * Required")]
        [RegularExpression(@"^(?=^.{8,}$)((?=.*\d)|(?=.*\W+))(?![.\n])(?=.*[A-Z])(?=.*[a-z]).*$", ErrorMessage = "Password required at least 8 characters, 1 capital letter, 1 number, and one special character")]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = " * Required")]
        [DataType(DataType.Password)]
        //[Display(Name = "Confirm new password")]
        //[Compare("NewPassword", ErrorMessage = "The new password and confirmation password does not match.")]
        public string ConfirmPassword { get; set; }
    }
    public class Header
    {
        public int Flg { get; set; }
        public string FirstName { get; set; }
        public string UserRole { get; set; }
        public int HeaderStoreId { get; set; }
        public int UserDropData { get; set; }
        public int PriorityDropData { get; set; }
        public List<UserNotListData> UserNoteList { get; set; }
        public List<UsertaskListData> UserTaskList { get; set; }
        public List<UserReminderListData> UserReminderList { get; set; }
    }

    public class UserNotListData
    {
        public int NoteId { get; set; }
        public string NoteName { get; set; }
        public string NoteDescription { get; set; }
        public DateTime? CreatedOn { get; set; }
        public int? InvoiceId { get; set; }
    }

    public class UsertaskListData
    {
        public int TaskId { get; set; }
        public string TaskName { get; set; }
        public string TaskDescription { get; set; }
        public int PriorityId { get; set; }
        public int AssignTo { get; set; }
        public string AssignByName { get; set; }
        public string AssignToName { get; set; }
        public DateTime? DueDate { get; set; }
        public int Createdby { get; set; }
        public DateTime? CreatedOn { get; set; }
        public bool IsCompleted { get; set; }
        public int? InvoiceId { get; set; }
    }

    public class UserReminderListData
    {
        public int ReminderId { get; set; }
        public string ReminderName { get; set; }
        public string ReminderDescription { get; set; }
        public DateTime? ReminderDate { get; set; }
        public DateTime? CreatedOn { get; set; }
    }
}