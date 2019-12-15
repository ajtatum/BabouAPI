using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AJT.API.Web.Models.Database
{
    [Table("ShortenedUrlClicks")]
    public class ShortenedUrlClick
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int ShortenedUrlId { get; set; }

        [Required]
        public DateTime ClickDate { get; set; }

        [Column(TypeName = "varchar(500)")]
        public string Referrer { get; set; }

        public ShortenedUrl ShortenedUrl { get; set; }
    }
}
