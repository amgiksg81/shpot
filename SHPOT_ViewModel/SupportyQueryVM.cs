using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Script.Serialization;

namespace SHPOT_ViewModel
{
    public class SupportQueryVM
    {
        public int SupportQueryID { get; set; }

        [Display(Name = "Enter Your Full Name.")]
        [StringLength(100)]
        [Required(ErrorMessage = "Please Enter Your Full name.")]
        public string FullName { get; set; }

        [Display(Name = "Enter Email Address.")]
        [StringLength(100)]
        [RegularExpression(@"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*", ErrorMessage = "Please enter valie Email ID.")]
        [Required(ErrorMessage = "Please Enter Your Email Address.")]
        [DataType(DataType.EmailAddress)]
        public string EmailAddress { get; set; }

        [Display(Name = "Enter Your Query.")]
        [StringLength(2000)]
        [Required(ErrorMessage = "Please Enter Your Query.")]
        public string Query { get; set; }

        public string IPAddress { get; set; }

        public DateTime QueryDateTime { get; set; }
    }
}