using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Brix.Models
{
    public class BrixIdentityDbContext : IdentityDbContext<IdentityUser>
    {
        public BrixIdentityDbContext(DbContextOptions<BrixIdentityDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
        }
    }

    public class BrixDatabaseContext : DbContext
    {
        public BrixDatabaseContext(DbContextOptions<BrixDatabaseContext> options) : base(options) 
        {
        }

        public DbSet<Product> Products { get; set; }

        public DbSet<Customer> Customers { get; set; }

        public DbSet<LineItem> LineItems { get; set; }

        public DbSet<Order> Orders { get; set; }

        public DbSet<ProductRecommendation> ProductRecommendations { get; set; }
        public DbSet<CustomerProductRecommendations> CustomerProductsRecommendations { get; set; }
    }
}