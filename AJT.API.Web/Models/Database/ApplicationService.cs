using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AJT.API.Web.Models.Database
{
    [Table("ApplicationServices")]
    public class ApplicationService
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Column(TypeName = "varchar(100)")]
        [StringLength(100)]
        [Required]
        public string Name { get; set; }
    }
}
