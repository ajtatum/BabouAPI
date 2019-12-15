using System.Collections.Generic;
using System.Threading.Tasks;
using AJT.API.Web.Models.Database;

namespace AJT.API.Web.Services.Interfaces
{
    public interface IUrlShortenerService
    {
        Task<List<ShortenedUrl>> GetShortenedUrlsByUserId(string userId);
        Task<ShortenedUrl> CreateByUserId(string userId, string longUrl, string domain);
        Task<ShortenedUrl> CreateByUserId(string userId, string longUrl, string token, string domain);
        Task<ShortenedUrl> UpdateById(int id, string longUrl);
        Task<ShortenedUrl> DeleteById(int id);
        Task<bool> CheckIfTokenIsAvailable(string token, string domain);

        /// <summary>
        /// Gets the Short Url from the token.
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        string GetShortUrl(string token);

        /// <summary>
        /// Generates a new Token while checking the database that the token doesn't already exist.
        /// </summary>
        /// <returns></returns>
        Task<string> GetToken();
    }
}
