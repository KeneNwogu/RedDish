using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace RedDish.Models
{
    public class RedDishDbContext : DbContext
    {
        private readonly IConfiguration _config;
        public DbSet<Menu> Menus { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }

        public DbSet<Order> Orders { get; set; }

        public DbSet<User> Users { get; set; }

        public string DbPath { get; }
       
        public RedDishDbContext(DbContextOptions<RedDishDbContext> options, IConfiguration configuration)
        : base(options)
        {
            _config = configuration;

            //string workingDirectory = Environment.CurrentDirectory;
            string workingDirectory = Environment.CurrentDirectory;

            //// This will get the current PROJECT directory
            string projectDirectory = Directory.GetParent(workingDirectory).FullName;
            DbPath = System.IO.Path.Join(workingDirectory, Environment.GetEnvironmentVariable("DB_CONNECTION_STRING") ?? "reddish.db");
        }

        // The following configures EF to create a Sqlite database file in the
        // special "local" folder for your platform.
        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite($"Data Source={DbPath}");
    }
}
