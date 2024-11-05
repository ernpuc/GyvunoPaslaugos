using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using PetServiceWebApplication.Models;
using System;
using Microsoft.AspNetCore.Identity;

namespace PetServiceWebApplication.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<PetServiceProvider> PetServiceProviders { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Review> Reviews { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<IdentityRole>().HasData(
                new IdentityRole { Name = "Admin", NormalizedName = "ADMIN" },
                new IdentityRole { Name = "User", NormalizedName = "USER" }
            );

            // PetServiceProvider Configuration
            modelBuilder.Entity<PetServiceProvider>()
                .HasKey(p => p.Id);

            modelBuilder.Entity<PetServiceProvider>()
                .HasMany(p => p.Reviews)
                .WithOne(r => r.PetServiceProvider)
                .HasForeignKey(r => r.PetServiceProviderId)
                .OnDelete(DeleteBehavior.Restrict);  // Restrict delete

            modelBuilder.Entity<PetServiceProvider>()
                .HasMany(p => p.Services)
                .WithOne(s => s.PetServiceProvider)
                .HasForeignKey(s => s.PetServiceProviderId)
                .OnDelete(DeleteBehavior.Restrict);  // Restrict delete

            modelBuilder.Entity<PetServiceProvider>()
                .Property(p => p.Category)
                .HasConversion<string>();  // For storing enum as string

            // Service Configuration
            modelBuilder.Entity<Service>()
                .HasKey(s => s.Id);

            modelBuilder.Entity<Service>()
                .Property(s => s.ServiceType)
                .HasConversion<string>();  // For storing enum as string

            modelBuilder.Entity<Service>()
                .Property(s => s.TargetAnimal)
                .HasConversion<string>();  // For storing enum as string

            modelBuilder.Entity<Service>()
                .HasOne(s => s.PetServiceProvider)
                .WithMany(p => p.Services)
                .HasForeignKey(s => s.PetServiceProviderId)
                .OnDelete(DeleteBehavior.Restrict);  // Restrict delete

            // Booking Configuration
            modelBuilder.Entity<Booking>()
                .HasKey(b => b.Id);

            modelBuilder.Entity<Booking>()
                .HasOne(b => b.ApplicationUser)
                .WithMany(u => u.Bookings)
                .HasForeignKey(b => b.ApplicationUserId)
                .OnDelete(DeleteBehavior.Restrict);  // Restrict delete

            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Service)
                .WithMany(u => u.Bookings)
                .HasForeignKey(b => b.ServiceId)
                .OnDelete(DeleteBehavior.Restrict);  // Restrict delete

            // Review Configuration
            modelBuilder.Entity<Review>()
                .HasKey(r => r.Id);

            modelBuilder.Entity<Review>()
                .HasOne(r => r.ApplicationUser)
                .WithMany(u => u.Reviews)
                .HasForeignKey(r => r.ApplicationUserId)
                .OnDelete(DeleteBehavior.Restrict);  // Restrict delete

            modelBuilder.Entity<Review>()
                .HasOne(r => r.PetServiceProvider)
                .WithMany(p => p.Reviews)
                .HasForeignKey(r => r.PetServiceProviderId)
                .OnDelete(DeleteBehavior.Restrict);  // Restrict delete

            // Setting up DayOfWeek collection in PetServiceProvider
            modelBuilder.Entity<PetServiceProvider>()
                .Property(p => p.AvailableDays)
                .HasConversion(
                    v => string.Join(",", v),            // Convert list to string for storage
                    v => v.Split(',', StringSplitOptions.RemoveEmptyEntries)
                          .Select(d => Enum.Parse<DayOfWeek>(d))
                          .ToList());                  // Convert string back to list on retrieval
        }
    }
}
