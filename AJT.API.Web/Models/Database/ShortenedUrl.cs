using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AJT.API.Web.Models.Database
{
    [Table("ShortenedUrls")]
    public class ShortenedUrl
    {
        public ShortenedUrl()
        {
            ShortenedUrlClicks = new List<ShortenedUrlClick>();
        }

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column(TypeName = "varchar(50)")]
        public string Id { get; set; }

        [Required]
        [Column(TypeName = "varchar(500)")]
        public string LongUrl { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(450)")]
        public string CreatedBy { get; set; }

        [Required]
        public DateTime CreatedOn { get; set; }

        public DateTime? ModifiedOn { get; set; }

        [Required]
        [Column(TypeName = "varchar(100)")]
        public string ShortUrl { get; set; }

        public List<ShortenedUrlClick> ShortenedUrlClicks { get; set; }

        public virtual ApplicationUser ApplicationUser { get; set; }
    }
}
