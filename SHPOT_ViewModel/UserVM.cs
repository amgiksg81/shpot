using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace SHPOT_ViewModel
{
    public class UserVM
    {
        public int UserID { get; set; }

        public string UserName { get; set; }

        public string Email { get; set; }

        [ScriptIgnore]
        [Required(ErrorMessage = "Please Enter Password.")]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [RegularExpression(@"((?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[@#$%]).{6,20})", ErrorMessage = "Passwords must be at least 6 characters and contain at 3 of 4 of the following: <br/>Upper case (A-Z), Lower case (a-z), Number (0-9) and Special character (e.g., !@#$%^&*)")]
        [DisplayName("Password")]
        [DataType(DataType.Password)]

        public string Password { get; set; }

        [ScriptIgnore]
        [StringLength(100)]
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "Password and Confirm Password must match.")]
        [DisplayName("Confirm Password")]
        [Required(ErrorMessage = "Please Enter Confirm Password.")]
        public string ConfirmPassword { get; set; }

        [ScriptIgnore]
        public string NewPassword { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string UserType { get; set; }

        [ScriptIgnore]
        public string HeaderToken { get; set; }

        public string IPAddress { get; set; }

        public bool IsActive { get; set; } = true;

        public string ProfileImageUrl { get; set; } = null;

        public string SocialID { get; set; } = null;

        public bool IsSocialUser { get; set; } = false;

        public bool IsUserPremium { get; set; }

        [ScriptIgnore]
        public string ResetToken { get; set; }

        [ScriptIgnore]
        public bool IsResetTokenActive { get; set; }

        [ScriptIgnore]
        public DateTime? ResetTokenExpiryDate { get; set; }

        [ScriptIgnore]
        public string BrainTreeClientToken { get; set; }

        [ScriptIgnore]
        public string BrainTreeCustomerID { get; set; }
    }
}
