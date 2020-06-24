using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SHPOT_DAL
{
    public class BusinessRating
    {
        [Key]
        public int BusinessRatingID { get; set; }

        [Column(TypeName = "nvarchar")]
        [StringLength(200)]
        public string Review { get; set; }

        public double Rating { get; set; }

        [ForeignKey("User")]
        public int UserID { get; set; }

        [ForeignKey("Business")]
        public int? BusinessID { get; set; }

        [Column(TypeName = "nvarchar")]
        [StringLength(200)]
        public string PlaceID { get; set; }

        public virtual Business Business { get; set; }

        public virtual User User { get; set; }
    }
}
