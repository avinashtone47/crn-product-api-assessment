using Microsoft.EntityFrameworkCore;
using Product.Domain.Entities;

namespace Product.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Product.Domain.Entities.Product> Products => Set<Product.Domain.Entities.Product>();
        public DbSet<Item> Items => Set<Item>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product.Domain.Entities.Product>(entity =>
            {
                entity.ToTable("Product");
                entity.HasKey(p => p.Id);
                entity.Property(p => p.ProductName).IsRequired().HasMaxLength(255);
                entity.Property(p => p.CreatedBy).IsRequired().HasMaxLength(100);
                entity.Property(p => p.ModifiedBy).HasMaxLength(100);
            });

            modelBuilder.Entity<Item>(entity =>
            {
                entity.ToTable("Item");
                entity.HasKey(i => i.Id);
                entity.Property(i => i.Quantity).IsRequired();

                entity.HasOne(i => i.Product)
                      .WithMany(p => p.Items)
                      .HasForeignKey(i => i.ProductId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}