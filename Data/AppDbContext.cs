using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ReactProjectsAuthApi.Data
{
    public class AppDbContext : IdentityDbContext
    {
        private readonly IConfiguration _config;

        public AppDbContext(DbContextOptions<AppDbContext> opts,IConfiguration config):base(opts)
        {
            _config = config;
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(_config.GetConnectionString("MainConn"));
        }
    }
}
