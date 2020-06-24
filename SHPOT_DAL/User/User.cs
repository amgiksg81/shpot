using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SHPOT_DAL
{
    public class User
    {
        public User()
        {
            UserLogins = new HashSet<UserLogin>();
            UserPremiums = new HashSet<UserPremium>();
            Businesses = new HashSet<Business>();
        }
        [Key]
        public int UserID { get; set; }

        [Column(TypeName = "nvarchar")]
        [StringLength(50)]
        public string UserName { get; set; }

        [Column(TypeName = "nvarchar")]
        [StringLength(100)]
        public string Email { get; set; }

        [Column(TypeName = "nvarchar")]
        [StringLength(100)]
        public string Password { get; set; }

        [Column(TypeName = "nvarchar")]
        [StringLength(50)]
        public string FirstName { get; set; }

        [Column(TypeName = "nvarchar")]
        [StringLength(50)]
        public string LastName { get; set; }

        [Column(TypeName = "nvarchar")]
        [StringLength(50)]
        public string UserType { get; set; }

        [Column(TypeName = "nvarchar")]
        [StringLength(2000)]
        public string HeaderToken { get; set; }

        public DateTime? CreatedDate { get; set; }

        [Column(TypeName = "nvarchar")]
        [StringLength(50)]
        public string IPAddress { get; set; }

        public bool IsActive { get; set; }

        /* Social Login Details  */
        [Column(TypeName = "nvarchar")]
        [StringLength(500)]
        public string ProfileImageUrl { get; set; }

        [Column(TypeName = "nvarchar")]
        [StringLength(200)]
        public string SocialID { get; set; }

        public bool IsSocialUser { get; set; }
        /* Social Login Details  */

        [Column(TypeName = "nvarchar")]
        [StringLength(2000)]
        public string ResetToken { get; set; }

        public bool IsResetTokenActive { get; set; }

        public DateTime? ResetTokenExpiryDate { get; set; }

        [Column(TypeName = "nvarchar")]
        [StringLength(5000)]
        public string BrainTreeClientToken { get; set; }

        [Column(TypeName = "nvarchar")]
        [StringLength(2000)]
        public string BrainTreeCustomerID { get; set; }

        public virtual ICollection<UserLogin> UserLogins { get; set; }

        public virtual ICollection<UserPremium> UserPremiums { get; set; }

        public virtual ICollection<Business> Businesses { get; set; }
    }
}
