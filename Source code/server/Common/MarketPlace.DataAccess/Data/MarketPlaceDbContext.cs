using MarketPlace.DataAccess.Model;
using Microsoft.EntityFrameworkCore;

namespace MarketPlace.DataAccess.Data
{
    public class MarketPlaceDbContext : DbContext
    {
        public MarketPlaceDbContext(DbContextOptions<MarketPlaceDbContext> options) : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasIndex(user => user.Email)
                .IsUnique();

            modelBuilder.Entity<Admin>()
                .HasIndex(user => user.Email)
                .IsUnique();

            modelBuilder.Entity<Agent>()
                .HasIndex(agent => agent.Email)
                .IsUnique();

            modelBuilder.Entity<Category>()
               .HasIndex(category => category.CategoryName)
               .IsUnique();
        }

        public DbSet<Admin> Admins { get; set; } = null!;

        public DbSet<User> Users { get; set; } = null!;

        public DbSet<Product> Products { get; set; } = null!;

        public DbSet<WishList> WishLists { get; set; } = null!;

        public DbSet<Orders> Orders { get; set; } = null!;

        public DbSet<Category> Categories { get; set; } = null!;

        public DbSet<OrderDetails> OrderDetails { get; set; } = null!;

        public DbSet<Photos> Photos { get; set; } = null!;

        public DbSet<Cart> Cart { get; set; } = null!;

        public DbSet<DeliveryAddress> DeliveryAddresses { get; set; } = null!;

        public DbSet<Notification> Notifications { get; set; } = null!;

        public DbSet<Agent> Agents { get; set; } = null!;

        public DbSet<OrderHistory> OrderHistory { get; set; } = null!;
    }
}