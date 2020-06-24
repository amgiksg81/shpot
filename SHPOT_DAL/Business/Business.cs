using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace SHPOT_DAL
{
    public class Business
    {
        public Business()
        {
            BusinessImages = new HashSet<BusinessImage>();
            BusinessRatings = new HashSet<BusinessRating>();
            FavouritePlaces = new HashSet<FavouritePlace>();
        }

        [Key]
        public int BusinessID { get; set; }

        [Column(TypeName = "nvarchar")]
        [StringLength(200)]
        public string BusinessName { get; set; }

        [Column(TypeName = "nvarchar")]
        [StringLength(100)]
        public string Latitude { get; set; }

        [Column(TypeName = "nvarchar")]
        [StringLength(100)]
        public string Longitude { get; set; }

        [Column(TypeName = "nvarchar")]
        [StringLength(500)]
        public string Location { get; set; }

        [Column(TypeName = "nvarchar")]
        [StringLength(50)]
        public string ContactNo { get; set; }

        [Column(TypeName = "nvarchar")]
        [StringLength(200)]
        public string Website { get; set; }

        [Column(TypeName = "nvarchar")]
        [StringLength(100)]
        public string StartTime { get; set; }

        [Column(TypeName = "nvarchar")]
        [StringLength(100)]
        public string EndTime { get; set; }

        [Column(TypeName = "nvarchar")]
        [StringLength(200)]
        public string Flavours { get; set; }

        [Column(TypeName = "nvarchar")]
        [StringLength(2000)]
        public string PremiumImageContent { get; set; }

        [ForeignKey("User")]
        public int UserID { get; set; }

        public DateTime? CreatedDate { get; set; }

        public virtual User User { get; set; }

        public virtual ICollection<BusinessImage> BusinessImages { get; set; }
        public virtual ICollection<BusinessRating> BusinessRatings { get; set; }
        public virtual ICollection<FavouritePlace> FavouritePlaces { get; set; }
    }
}
