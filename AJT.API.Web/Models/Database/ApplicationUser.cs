using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace AJT.API.Web.Models.Database
{
    public class ApplicationUser : IdentityUser
    {
        public ApplicationUser()
        {
            ApplicationUserServices = new List<ApplicationUserService>();
            ShortenedUrls = new List<ShortenedUrl>();
        }

        [Column(TypeName = "varchar(50)")]
        [StringLength(50)]
        public string ApiAuthKey { get; set; }

        public List<ApplicationUserService> ApplicationUserServices { get; set; }
        public List<ShortenedUrl> ShortenedUrls { get; set; }
    }
}
