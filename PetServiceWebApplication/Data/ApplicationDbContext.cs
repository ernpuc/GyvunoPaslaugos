using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using PetServiceWebApplication.Models;
using System;
using Microsoft.AspNetCore.Identity;

namespace PetServiceWebApplication.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        private readonly PasswordHasher<ApplicationUser> _passwordHasher = new PasswordHasher<ApplicationUser>();

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<PetServiceProvider> PetServiceProviders { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Review> Reviews { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<IdentityRole>().HasData(
                new IdentityRole { Id= "71e0ace6-27db-4891-947a-7af9634b6ad2", Name = "Admin", NormalizedName = "ADMIN" },
                new IdentityRole { Id= "7ecec5ed-2b44-406c-8ff3-4cbed083961f", Name = "User", NormalizedName = "USER" },
                new IdentityRole { Id= "d5c866d9-eae9-4a14-99e9-d2a44fbd5d70", Name = "ServiceAdmin", NormalizedName = "SERVICEADMIN" }
            );

            // Seed super admin user
            var adminUser = new ApplicationUser
            {
                Id = "f7c638da-17e3-4bcc-960f-706b8922c2d8",
                UserName = "admin@gmail.com",
                Email = "admin@gmail.com",
                FirstName = "Admin",
                LastName = "User",
                EmailConfirmed = true
            };

            adminUser.NormalizedUserName = adminUser.UserName.ToUpper();
            adminUser.NormalizedEmail = adminUser.Email.ToUpper();

            // Hash the password before seeding the user
            adminUser.PasswordHash = _passwordHasher.HashPassword(adminUser, "Admin_123");

            // Seed the user
            modelBuilder.Entity<ApplicationUser>().HasData(adminUser);

            // Seed the relationship between the user and the "Admin" role (manually set the role)
            modelBuilder.Entity<IdentityUserRole<string>>().HasData(
                new IdentityUserRole<string>
                {
                    UserId = adminUser.Id,
                    RoleId = "71e0ace6-27db-4891-947a-7af9634b6ad2"     // Admin role id
                }
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
