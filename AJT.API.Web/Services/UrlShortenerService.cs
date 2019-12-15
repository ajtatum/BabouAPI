using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AJT.API.Web.Data;
using AJT.API.Web.Models;
using AJT.API.Web.Models.Database;
using AJT.API.Web.Services.Interfaces;
using BabouExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace AJT.API.Web.Services
{
    public class UrlShortenerService : IUrlShortenerService
    {
        private readonly ApplicationDbContext _context;
        private readonly AppSettings _appSettings;
        private string Token { get; set; }
        private static readonly Random Random = new Random();
        private const string BigAlphas = "ABCDEFGHIJKLMNOPQRSTUVWXY";
        private const string SmallAlphas = "abcdefghjlkmnopqrstuvwxyz";
        private const string Numbers = "0123456789";
        private static readonly string Pool = $"{BigAlphas}{SmallAlphas}{Numbers}";

        public UrlShortenerService(ApplicationDbContext context, IOptionsMonitor<AppSettings> appSettings)
        {
            _context = context;
            _appSettings = appSettings.CurrentValue;
        }

        public async Task<List<ShortenedUrl>> GetShortenedUrlsByUserId(string userId)
        {
            var shortenedUrls = await _context.ShortenedUrls
                .Include(x => x.ShortenedUrlClicks)
                .Where(x => x.CreatedBy == userId)
                .OrderBy(x => x.CreatedOn)
                .ToListAsync();

            return shortenedUrls;
        }

        public async Task<ShortenedUrl> CreateByUserId(string userId, string longUrl)
        {
            var token = await GetToken();

            var shortenUrl = new ShortenedUrl()
            {
                Id = token,
                LongUrl = longUrl,
                ShortUrl = GetShortUrl(token),
                CreatedBy = userId,
                CreatedOn = DateTime.Now
            };

            _context.Add(shortenUrl);
            await _context.SaveChangesAsync();

            return shortenUrl;
        }

        public async Task<ShortenedUrl> CreateByUserId(string userId, string longUrl, string token)
        {
            var shortenUrl = new ShortenedUrl()
            {
                Id = token,
                LongUrl = longUrl,
                ShortUrl = GetShortUrl(token),
                CreatedBy = userId,
                CreatedOn = DateTime.Now
            };

            _context.Add(shortenUrl);
            await _context.SaveChangesAsync();

            return shortenUrl;
        }

        public async Task<ShortenedUrl> UpdateById(string id, string longUrl)
        {
            var shortenedUrl = await _context.ShortenedUrls.FirstOrDefaultAsync(x => x.Id == id);
            if (shortenedUrl == null)
            {
                throw new NullReferenceException($"Shortened Url with Id {id} doesn't exist.");
            }

            shortenedUrl.LongUrl = longUrl;
            shortenedUrl.ModifiedOn = DateTime.Now;
            _context.Update(shortenedUrl);
            await _context.SaveChangesAsync();

            return shortenedUrl;
        }

        public async Task<ShortenedUrl> DeleteById(string id)
        {
            var shortenedUrl = await _context.ShortenedUrls.FirstOrDefaultAsync(x => x.Id == id);
            if (shortenedUrl == null)
            {
                throw new NullReferenceException($"Shortened Url with Id {id} doesn't exist.");
            }

            _context.Remove(shortenedUrl);
            await _context.SaveChangesAsync();

            return shortenedUrl;
        }

        /// <summary>
            /// Gets the Short Url from the token.
            /// </summary>
            /// <param name="token"></param>
            /// <returns></returns>
            public string GetShortUrl(string token)
        {
            return $"{_appSettings.BaseShortenedUrl}{token}";
        }

        /// <summary>
        /// Generates a new Token while checking the database that the token doesn't already exist.
        /// </summary>
        /// <returns></returns>
        public async Task<string> GetToken()
        {
            var currentTokens = await _context.ShortenedUrls.Select(x => x.Id).ToListAsync();

            while (!currentTokens.Exists(x => x == GenerateRandomToken()))
            {
                return Token;
            }
            throw new DataException("Unable to generate an unique token at this time.");
        }

        private static readonly object ThreadLock = new object();

        /// <summary>
        /// Generates a random string of a specified length
        /// </summary>
        /// <returns>A random string</returns>
        private string GenerateRandomToken()
        {
            string pool;
            Random rand;

            lock (ThreadLock)
            {
                pool = Pool;
                rand = Random;
            }

            var poolBuilder = new StringBuilder(pool);

            var poolBuilderString = poolBuilder.ToString();

            var output = new char[8];
            for (var i = 0; i < 8; i++)
            {
                var charIndex = rand.Next(0, poolBuilderString.Length);
                output[i] = poolBuilderString[charIndex];
            }

            Token = new string(output);
            return Token;
        }
    }
}
