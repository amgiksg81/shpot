using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SHPOT_DAL
{
    public class BusinessImage
    {
        [Key]
        public int BusinessImageID { get; set; }

        [Column(TypeName = "nvarchar")]
        [StringLength(200)]
        public string ImageName { get; set; }

        [Column(TypeName = "nvarchar")]
        [StringLength(100)]
        public string ImageType { get; set; }

        [ForeignKey("Business")]
        public int BusinessID { get; set; }

        public bool IsPremiumImage { get; set; } = false;

        public virtual Business Business { get; set; }
    }
}
