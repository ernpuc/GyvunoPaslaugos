// using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PetServiceWebApplication.Models;

namespace PetServiceWebApplication.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<PetServiceProvider> PetServiceProviders { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Service> Services { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Booking
            modelBuilder.Entity<Booking>()
                .HasKey(b => b.Id);

            modelBuilder.Entity<Booking>()
                .HasOne(b => b.User)
                .WithMany(u => u.Bookings)
                .HasForeignKey(b => b.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Service)
                .WithMany()
                .HasForeignKey(b => b.ServiceId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Booking>()
                .Property(b => b.BookingDate)
                .HasDefaultValueSql("GETDATE()");

            modelBuilder.Entity<Booking>()
                .Property(b => b.IsCompleted)
                .HasDefaultValue(false);

            modelBuilder.Entity<Booking>()
                .Property(b => b.IsPaid)
                .HasDefaultValue(false);

            // Configure PetServiceProvider
            modelBuilder.Entity<PetServiceProvider>()
                .HasKey(p => p.Id);

            modelBuilder.Entity<PetServiceProvider>()
                .Property(p => p.Name)
                .IsRequired();

            modelBuilder.Entity<PetServiceProvider>()
                .Property(p => p.Address)
                .IsRequired();

            modelBuilder.Entity<PetServiceProvider>()
                .Property(p => p.Phone)
                .IsRequired();

            modelBuilder.Entity<PetServiceProvider>()
                .Property(p => p.Email)
                .IsRequired();

            modelBuilder.Entity<PetServiceProvider>()
                .Property(p => p.OpeningTime)
                .HasDefaultValue(new TimeSpan(9, 0, 0));

            modelBuilder.Entity<PetServiceProvider>()
                .Property(p => p.ClosingTime)
                .HasDefaultValue(new TimeSpan(17, 0, 0));

            modelBuilder.Entity<PetServiceProvider>()
                .Ignore(p => p.Rating); // Rating is computed, not stored

            // Configure Review
            modelBuilder.Entity<Review>()
                .HasKey(r => r.Id);

            modelBuilder.Entity<Review>()
                .Property(r => r.Date)
                .IsRequired();

            modelBuilder.Entity<Review>()
                .HasOne(r => r.PetServiceProvider)
                .WithMany(p => p.Reviews)
                .HasForeignKey(r => r.PetServiceProviderId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Review>()
                .HasOne(r => r.User)
                .WithMany(u => u.Reviews)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure Service
            modelBuilder.Entity<Service>()
                .HasKey(s => s.Id);

            modelBuilder.Entity<Service>()
                .Property(s => s.Name)
                .IsRequired();

            modelBuilder.Entity<Service>()
                .Property(s => s.Description)
                .IsRequired();

            modelBuilder.Entity<Service>()
                .Property(s => s.Price)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Service>()
                .Property(s => s.IsActive)
                .HasDefaultValue(true);

            modelBuilder.Entity<Service>()
                .HasOne(s => s.PetServiceProvider)
                .WithMany(p => p.Services)
                .HasForeignKey(s => s.PetServiceProviderId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure User
            modelBuilder.Entity<User>()
                .HasKey(u => u.Id);

            modelBuilder.Entity<User>()
                .Property(u => u.Email)
                .IsRequired(false); // Nullable in this model

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();
        }
    }
}
