using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Babou.API.Web.Models.Database
{
    [Table("ApplicationServices")]
    public class ApplicationService
    {
        public ApplicationService()
        {
            ApplicationUserServices = new List<ApplicationUserService>();
        }

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Column(TypeName = "varchar(100)")]
        [StringLength(100)]
        [Required]
        public string Name { get; set; }

        public List<ApplicationUserService> ApplicationUserServices { get; set; }
    }
}
