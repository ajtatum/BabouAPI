using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Spatial;
using NetTopologySuite.Geometries;

namespace Babou.API.Web.Models.Database
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

        [Column(TypeName = "varchar(50)")]
        public string City { get; set; }
        
        [Column(TypeName = "varchar(50)")]
        public string State { get; set; }
        
        [Column(TypeName = "varchar(50)")]
        public string Country { get; set; }

        public Point Geography { get; set; }

        public ShortenedUrl ShortenedUrl { get; set; }
    }
}
