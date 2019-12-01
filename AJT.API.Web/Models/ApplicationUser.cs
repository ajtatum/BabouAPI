using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace AJT.API.Web.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Column(TypeName = "varchar(50)")]
        [StringLength(50)]
        public string ApiAuthKey { get; set; }
    }
}
