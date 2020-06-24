using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SHPOT_DAL
{
    public class UserPremium
    {
        [Key]
        public int UserPremiumlID { get; set; }

        [Column(TypeName = "nvarchar")]
        [StringLength(500)]
        public string BrainTreeCustomerID { get; set; }

        [Column(TypeName = "nvarchar")]
        [StringLength(500)]
        public string TransactionID { get; set; }

        [Column(TypeName = "nvarchar")]
        [StringLength(5000)]
        public string BrainTreeClientToken { get; set; }

        [Column(TypeName = "nvarchar")]
        [StringLength(15)]
        public string PaymentMode { get; set; }

        public double Amount { get; set; }

        public DateTime? PaymentDate { get; set; }

        [ForeignKey("User")]
        public int UserID { get; set; }

        [ForeignKey("Business")]
        public int BusinessID { get; set; }

        public virtual User User { get; set; }

        public virtual Business Business { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
