using iTunesSearcher.Entities.Tables;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace iTunesSearcher.Entities
{
    public class DB : DbContext
    {
        public DbSet<Album> Albums { get; set; }

        public DB()
        {
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var builder = new SqliteConnectionStringBuilder()
            {
                DataSource = "iTunes_base.db",
                Mode = SqliteOpenMode.ReadWriteCreate,
            };
            optionsBuilder.UseSqlite(builder.ConnectionString, options =>
            {
                options.MigrationsAssembly(Assembly.GetExecutingAssembly().FullName);
            });

            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Album>();

            base.OnModelCreating(modelBuilder);
        }
    }
}
