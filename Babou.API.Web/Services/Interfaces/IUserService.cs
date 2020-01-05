using System.Threading.Tasks;
using Babou.API.Web.Models.Database;

namespace Babou.API.Web.Services.Interfaces
{
    public interface IUserService
    {
        /// <summary>
        /// Tries to create a user and adds them to the Member role.
        /// </summary>
        /// <param name="email">Email address of the user</param>
        /// <param name="password">Password for the user. If null or empty, a password is automatically generated.</param>
        /// <param name="username">Username of the user.</param>
        /// <param name="fullName">Full Name of the user.</param>
        /// <returns>Will either return an <see cref="ApplicationUser"/> or null.</returns>
        Task<ApplicationUser> QuickCreateUser(string email, string password, string username, string fullName);
    }
}
