using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Webapi.shared
{
	public class Context : IdentityDbContext<ApplicationUser>
	{

        // Your existing custom entities
        public DbSet<AccountRoles> AccountRoles { get; set; }
        public DbSet<SecureAccountA> SecureAccountA { get; set; }
		public DbSet<Order> Order { get; set; }
		public DbSet<OrderDetails> OrderDetails { get; set; }
		public DbSet<OrderStatus> OrderStatus { get; set; }
		public DbSet<Product> Product { get; set; }
		public DbSet<ProductCategory> ProductCategory { get; set; }
		public DbSet<ProductData> ProductData { get; set; }
		public DbSet<ProductDetails> ProductDetails { get; set; }
		public DbSet<ProductImage> ProductImage { get; set; }
		public DbSet<ProductReviews> ProductReview { get; set; }
		public DbSet<Review> Review { get; set; }
		public DbSet<Roles> Roles { get; set; }
		public DbSet<ApplicationUser> Users { get; set; }

		public Context(DbContextOptions<Context> options) : base(options)
		{
			
		}
		protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder); // Ensures Identity tables are created

            // Add custom configurations here if needed
            
        }
	}
}
