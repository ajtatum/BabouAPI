using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Babou.API.Web.Models.Database
{
    [Table("ShortenedUrls")]
    public class ShortenedUrl
    {
        public ShortenedUrl()
        {
            ShortenedUrlClicks = new List<ShortenedUrlClick>();
        }

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required]
        public int Id { get; set; }

        [Required]
        [Column(TypeName = "varchar(50)")]
        [RegularExpression("^[a-zA-Z0-9-_+]{2,50}$", ErrorMessage = "Tokens can only contain alphanumeric, dashes, underscores, or plus signs. Must be between 2 and 50 characters long.")]
        public string Token { get; set; }

        [Required]
        [Column(TypeName = "varchar(50)")]
        public string Domain { get; set; }

        [Required]
        [Column(TypeName = "varchar(500)")]
        [Url(ErrorMessage = "Please enter a valid URL.")]
        [Display(Name = "Long Url")]
        public string LongUrl { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(450)")]
        public string CreatedBy { get; set; }

        [Required]
        public DateTime CreatedOn { get; set; }

        public DateTime? ModifiedOn { get; set; }

        [Required]
        [Column(TypeName = "varchar(100)")]
        [Url]
        [Display(Name = "Short Url")]
        public string ShortUrl { get; set; }

        public List<ShortenedUrlClick> ShortenedUrlClicks { get; set; }

        public virtual ApplicationUser ApplicationUser { get; set; }
    }
}
