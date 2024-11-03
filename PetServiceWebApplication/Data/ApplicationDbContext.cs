// using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PetServiceWebApplication.Models;

namespace PetServiceWebApplication.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Pet> Pets { get; set; }
        public DbSet<PetServiceProvider> PetServiceProviders { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Payment> Payments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // User Table
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("User");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.FirstName).IsRequired().HasMaxLength(300);
                entity.Property(e => e.LastName).IsRequired().HasMaxLength(300);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(320);
                entity.Property(e => e.Password).IsRequired().HasMaxLength(128);
                entity.Property(e => e.Phone).HasMaxLength(16);
            });

            // Pet Table
            modelBuilder.Entity<Pet>(entity =>
            {
                entity.ToTable("Pet");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.UserId).IsRequired();
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Species).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Breed).HasMaxLength(100);
                entity.Property(e => e.Size).IsRequired().HasMaxLength(10);
                entity.Property(e => e.BirthDate).HasColumnType("date");

                entity.HasOne(d => d.User)
                      .WithMany(p => p.Pets)
                      .HasForeignKey(d => d.UserId);

                entity.ToTable(b => b.HasCheckConstraint("CK_Pet_Size", "[Size] IN ('Small', 'Medium', 'Large', 'ExtraLarge')"));
            });

            // PetServiceProvider Table
            modelBuilder.Entity<PetServiceProvider>(entity =>
            {
                entity.ToTable("PetServiceProvider");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Address).IsRequired().HasMaxLength(300);
                entity.Property(e => e.Phone).IsRequired().HasMaxLength(16);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(320);
            });

            // Service Table
            modelBuilder.Entity<Service>(entity =>
            {
                entity.ToTable("Service");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.ProviderId).IsRequired();
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Description).HasMaxLength(500);
                entity.Property(e => e.Price).IsRequired().HasColumnType("decimal(7, 2)");

                entity.HasOne(d => d.PetServiceProvider)
                      .WithMany(p => p.Services)
                      .HasForeignKey(d => d.ProviderId);
            });

            // Booking Table
            modelBuilder.Entity<Booking>(entity =>
            {
                entity.ToTable("Booking");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.UserId).IsRequired();
                entity.Property(e => e.ServiceId).IsRequired();
                entity.Property(e => e.Date).IsRequired().HasColumnType("datetime");

                entity.HasOne(d => d.User)
                      .WithMany(p => p.Bookings)
                      .HasForeignKey(d => d.UserId);
                entity.HasOne(d => d.Service)
                      .WithMany(p => p.Bookings)
                      .HasForeignKey(d => d.ServiceId);
            });

            // Review Table
            modelBuilder.Entity<Review>(entity =>
            {
                entity.ToTable("Review");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.UserId).IsRequired();
                entity.Property(e => e.ServiceId).IsRequired();
                entity.Property(e => e.Rating).IsRequired();
                entity.Property(e => e.Comment).HasMaxLength(1000);
                entity.Property(e => e.Timestamp)
                      .IsRequired()
                      .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.HasOne(d => d.User)
                      .WithMany(p => p.Reviews)
                      .HasForeignKey(d => d.UserId);
                entity.HasOne(d => d.Service)
                      .WithMany(p => p.Reviews)
                      .HasForeignKey(d => d.ServiceId);

                entity.ToTable(b => b.HasCheckConstraint("CK_Review_Rating", "[Rating] >= 1 AND [Rating] <= 5"));
            });

            // Payment Table
            modelBuilder.Entity<Payment>(entity =>
            {
                entity.ToTable("Payment");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.BookingId).IsRequired();
                entity.Property(e => e.PaymentType).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Amount).IsRequired().HasColumnType("decimal(7, 2)");
                entity.Property(e => e.InitiationTs)
                      .IsRequired()
                      .HasDefaultValueSql("CURRENT_TIMESTAMP");
                //entity.Property(e => e.StripePayId).HasMaxLength(36);
                entity.Property(e => e.ActualPayTs).HasColumnType("datetime");
                entity.Property(e => e.Status).HasMaxLength(10);

                entity.HasOne(d => d.Booking)
                      .WithMany(p => p.Payments)
                      .HasForeignKey(d => d.BookingId);

                entity.ToTable(b => b.HasCheckConstraint("CK_Payment_Status", "[Status] IN ('success', 'failed', 'waiting', 'canceled', 'refunded')"));
            });
        }
    }
}
