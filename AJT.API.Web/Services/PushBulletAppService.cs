using System;
using System.Threading.Tasks;
using Babou.API.Web.Data;
using Babou.API.Web.Helpers;
using Babou.API.Web.Models.Database;
using Babou.API.Web.Models.Services;
using Babou.API.Web.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Babou.API.Web.Services
{
    public class PushBulletAppService : IPushBulletAppService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<PushBulletAppService> _logger;
        private readonly ICipherService _cipherService;

        public PushBulletAppService(ApplicationDbContext context, ILogger<PushBulletAppService> logger, ICipherService cipherService)
        {
            _context = context;
            _logger = logger;
            _cipherService = cipherService;
        }

        public async Task<ApplicationUserService> GetPushBulletServiceByUserId(string userId)
        {
            return await _context.ApplicationUserServices.FirstOrDefaultAsync(x => x.ApplicationUserId == userId && x.ApplicationServiceId == Constants.Applications.PushBullet);
        }

        /// <summary>
        /// Retrieves the encrypted PushBullet Settings
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<PushBulletSettings> GetPushBulletSettingsByUserId(string userId)
        {
            var pushBulletService = await GetPushBulletServiceByUserId(userId);

            var pushBulletSettings = pushBulletService != null
                ? JsonConvert.DeserializeObject<PushBulletSettings>(pushBulletService.ApplicationSettings)
                : new PushBulletSettings() { ApiKey = string.Empty, EncryptionKey = string.Empty };

            return pushBulletSettings;
        }

        /// <summary>
        /// Retrieves the encrypted PushBullet Settings
        /// </summary>
        /// <param name="pushBulletService"></param>
        /// <returns></returns>
        public PushBulletSettings GetPushBulletSettingsByApplication(ApplicationUserService pushBulletService)
        {
            var pushBulletSettings = pushBulletService != null
                ? JsonConvert.DeserializeObject<PushBulletSettings>(pushBulletService.ApplicationSettings)
                : new PushBulletSettings() { ApiKey = string.Empty, EncryptionKey = string.Empty };

            return pushBulletSettings;
        }

        /// <summary>
        /// Adds or updates the PushBullet Settings with the settings already encrypted.
        /// </summary>
        /// <param name="userId">ApplicationUser UserId</param>
        /// <param name="pushBulletSettings">Encrypted Settings</param>
        /// <returns></returns>
        public async Task<bool> AddOrUpdatePushBulletSettings(string userId, string pushBulletSettings)
        {
            try
            {
                var pushBulletService = await GetPushBulletServiceByUserId(userId);

                if (pushBulletService == null)
                {
                    pushBulletService = new ApplicationUserService()
                    {
                        ApplicationUserId = userId,
                        ApplicationServiceId = Constants.Applications.PushBullet,
                        ApplicationSettings = pushBulletSettings
                    };
                    _context.Add(pushBulletService);
                    await _context.SaveChangesAsync();

                    _logger.LogInformation("PushBulletService: Added settings for {UserId}", userId);
                }
                else
                {
                    pushBulletService.ApplicationSettings = pushBulletSettings;
                    await _context.SaveChangesAsync();

                    _logger.LogInformation("PushBulletService: Updated settings for {UserId}", userId);
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "PushBulletService: Error updating for user {UserId}", userId);
                return false;
            }
        }

        public async Task<bool> RemovePushBulletService(string userId)
        {
            try
            {
                var pushBulletService = await GetPushBulletServiceByUserId(userId);

                if (pushBulletService != null)
                {
                    _context.Remove(pushBulletService);
                    await _context.SaveChangesAsync();
                }
                _logger.LogInformation("PushBulletService: Removed PushBulletService for {UserId}", userId);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "PushBulletService: Error trying to remove PushBulletService for {UserId}", userId);
                return false;
            }
        }

        /// <summary>
        /// Retrieves decrypted PushBullet settings by UserId
        /// </summary>
        /// <param name="userId">ApplicationUser UserId</param>
        /// <returns></returns>
        /// <exception cref="System.NullReferenceException">User must be associated with the PushBullet service.</exception>
        public async Task<PushBulletSettings> RetrieveDecryptedPushBulletSettingsByUserId(string userId)
        {
            var pushBulletService = await GetPushBulletServiceByUserId(userId);

            if(pushBulletService == null)
                throw new NullReferenceException($"There is no PushBullet service associated with {userId}");

            var pushBulletSettings = JsonConvert.DeserializeObject<PushBulletSettings>(pushBulletService.ApplicationSettings);
            pushBulletSettings.ApiKey = _cipherService.Decrypt(pushBulletSettings.ApiKey);
            pushBulletSettings.EncryptionKey = _cipherService.Decrypt(pushBulletSettings.EncryptionKey);

            return pushBulletSettings;
        }
    }
}
