using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SHPOT_DAL
{
    public class SupportQuery
    {
        [Key]
        public int SupportQueryID { get; set; }

        [Column(TypeName = "nvarchar")]
        [StringLength(100)]
        public string FullName { get; set; }

        [Column(TypeName = "nvarchar")]
        [StringLength(100)]
        public string EmailAddress { get; set; }

        [Column(TypeName = "nvarchar")]
        [StringLength(2000)]
        public string Query { get; set; }

        [Column(TypeName = "nvarchar")]
        [StringLength(50)]
        public string IPAddress { get; set; }

        public DateTime QueryDateTime { get; set; } = DateTime.Now;
    }
}
