using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SHPOT_DAL
{
    public class UserLogin
    {
        [Key]
        public int UserLoginDetailID { get; set; }

        [Column(TypeName = "nvarchar")]
        [StringLength(2000)]
        public string DeviceToken { get; set; }

        [Column(TypeName = "nvarchar")]
        [StringLength(50)]
        public string DeviceType { get; set; }

        [ForeignKey("User")]
        public int UserID { get; set; }

        public virtual User User { get; set; }
    }
}
