using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AJT.API.Web.Models.Database
{
    [Table("ApplicationUserServices")]
    public class ApplicationUserService
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int ApplicationServiceId { get; set; }

        [Required]
        public string ApplicationUserId { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        [Required]
        public string ApplicationSettings { get; set; }

        public ApplicationService ApplicationService { get; set; }

        public ApplicationUser ApplicationUser { get; set; }
    }
}
