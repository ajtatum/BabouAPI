using System.Threading.Tasks;
using AJT.API.Web.Models.Database;
using AJT.API.Web.Models.Services;

namespace AJT.API.Web.Services.Interfaces
{
    public interface IPushBulletAppService
    {
        Task<ApplicationUserService> GetPushBulletServiceByUserId(string userId);

        /// <summary>
        /// Retrieves the encrypted PushBullet Settings
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<PushBulletSettings> GetPushBulletSettingsByUserId(string userId);

        /// <summary>
        /// Retrieves the encrypted PushBullet Settings
        /// </summary>
        /// <param name="pushBulletService"></param>
        /// <returns></returns>
        PushBulletSettings GetPushBulletSettingsByApplication(ApplicationUserService pushBulletService);
        
        /// <summary>
        /// Adds or updates the PushBullet Settings with the settings already encrypted.
        /// </summary>
        /// <param name="userId">ApplicationUser UserId</param>
        /// <param name="pushBulletSettings">Encrypted Settings</param>
        /// <returns></returns>
        Task<bool> AddOrUpdatePushBulletSettings(string userId, string pushBulletSettings);
        Task<bool> RemovePushBulletService(string userId);

        /// <summary>
        /// Retrieves decrypted PushBullet settings by UserId
        /// </summary>
        /// <param name="userId">ApplicationUser UserId</param>
        /// <returns></returns>
        /// <exception cref="System.NullReferenceException">User must be associated with the PushBullet service.</exception>
        Task<PushBulletSettings> RetrieveDecryptedPushBulletSettingsByUserId(string userId);
    }
}
