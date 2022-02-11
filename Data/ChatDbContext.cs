using Microsoft.EntityFrameworkCore;
using ReactProjectsAuthApi.Models;

namespace ReactProjectsAuthApi.Data
{
    public class ChatDbContext:DbContext
    {
        public ChatDbContext(DbContextOptions<ChatDbContext> opts):base(opts)
        {

        }

        public DbSet<MessageModel> Messages{ get; set; }
        public DbSet<RelationshipsModel> Relations { get; set; }
    }
}
