using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace EntityModels.ViewModels
{
    public class Admin_Login
    {
        [Required(ErrorMessage = "Please Enter User Name....")]
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
    }
}
