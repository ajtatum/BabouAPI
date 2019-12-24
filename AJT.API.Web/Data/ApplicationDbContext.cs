using AJT.API.Web.Models.Database;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace AJT.API.Web.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        private readonly IConfiguration _configuration;

        //public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        //{
        //}

        public ApplicationDbContext(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<ApplicationService> ApplicationServices { get; set; }
        public DbSet<ApplicationUserService> ApplicationUserServices { get; set; }
        public DbSet<ShortenedUrl> ShortenedUrls { get; set; }
        public DbSet<ShortenedUrlClick> ShortedUrlClicks { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<ShortenedUrl>()
                .HasIndex(p => new { p.Token, p.Domain }).IsUnique();

            builder.Entity<ApplicationUser>()
                .HasMany(x => x.ApplicationUserServices)
                .WithOne(x => x.ApplicationUser)
                .HasForeignKey(x => x.ApplicationUserId)
                .HasPrincipalKey(x => x.Id)
                .IsRequired();

            builder.Entity<ApplicationService>()
                .HasMany(x => x.ApplicationUserServices)
                .WithOne(x => x.ApplicationService)
                .HasForeignKey(x => x.ApplicationServiceId)
                .HasPrincipalKey(x => x.Id)
                .IsRequired();

            builder.Entity<ApplicationUser>()
                .HasMany(x => x.ShortenedUrls)
                .WithOne(x => x.ApplicationUser)
                .HasForeignKey(x => x.CreatedBy)
                .HasPrincipalKey(x => x.Id)
                .IsRequired();

            builder.Entity<ShortenedUrl>()
                .HasMany(x => x.ShortenedUrlClicks)
                .WithOne(x => x.ShortenedUrl)
                .IsRequired();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(_configuration.GetConnectionString("DefaultConnection"));
            }
        }
    }
}
