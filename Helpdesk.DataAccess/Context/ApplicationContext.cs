using Helpdesk.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace SSO.DataAccess.Portgresql.Context {
    public class ApplicationContext : DbContext {
        public ApplicationContext(DbContextOptions<ApplicationContext> options) :
            base(options) {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
            optionsBuilder.LogTo(Console.WriteLine, Microsoft.Extensions.Logging.LogLevel.Information);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationContext).Assembly);
        }

        public DbSet<BotUser> BotUsers { get; set; }
        public DbSet<PhoneReference> PhoneReferences { get; set; }
        public DbSet<Article> Articles { get; set; }
        public DbSet<Room> Rooms { get; set; }
    }
}