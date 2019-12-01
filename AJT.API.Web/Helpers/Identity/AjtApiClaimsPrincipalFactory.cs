using System.Security.Claims;
using System.Threading.Tasks;
using AJT.API.Web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace AJT.API.Web.Helpers.Identity
{
    public class AjtApiClaimsPrincipalFactory : UserClaimsPrincipalFactory<ApplicationUser>
    {
        public AjtApiClaimsPrincipalFactory(
            UserManager<ApplicationUser> userManager,
            IOptions<IdentityOptions> optionsAccessor)
            : base(userManager, optionsAccessor)
        {
        }

        protected override async Task<ClaimsIdentity> GenerateClaimsAsync(ApplicationUser user)
        {
            var identity = await base.GenerateClaimsAsync(user);
            identity.AddClaim(new Claim("ApiAuthKey", user.ApiAuthKey));
            return identity;
        }
    }
}
