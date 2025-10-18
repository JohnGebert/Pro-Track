using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ProTrack.Models;

namespace ProTrack.Data
{
    /// <summary>
    /// Application database context inheriting from IdentityDbContext for user management
    /// </summary>
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        // Demo user constants for seeding
        private const string DEMO_USER_ID = "demo-user-123";
        private const string DEMO_USER_EMAIL = "demo@protrack.com";
        private const string DEMO_USER_NAME = "demo@protrack.com";

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // DbSets for custom entities
        public DbSet<Client> Clients { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<TimeEntry> TimeEntries { get; set; }
        public DbSet<Invoice> Invoices { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Configure ApplicationUser
            builder.Entity<ApplicationUser>(entity =>
            {
                entity.Property(e => e.FirstName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.LastName).HasMaxLength(100);
                entity.Property(e => e.CompanyName).HasMaxLength(200);
                entity.Property(e => e.Address).HasMaxLength(500);
                entity.Property(e => e.CreatedDate).HasDefaultValueSql("GETUTCDATE()");
            });

            // Configure Client
            builder.Entity<Client>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
                entity.Property(e => e.ContactEmail).HasMaxLength(256);
                entity.Property(e => e.PhoneNumber).HasMaxLength(20);
                entity.Property(e => e.Address).HasMaxLength(500);
                entity.Property(e => e.Notes).HasMaxLength(1000);
                entity.Property(e => e.CreatedDate).HasDefaultValueSql("GETUTCDATE()");
                entity.Property(e => e.IsActive).HasDefaultValue(true);

                // Configure relationships
                entity.HasOne(c => c.User)
                      .WithMany(u => u.Clients)
                      .HasForeignKey(c => c.UserId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(e => new { e.UserId, e.Name }).IsUnique();
            });

            // Configure Project
            builder.Entity<Project>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Description).HasMaxLength(2000);
                entity.Property(e => e.HourlyRate).HasColumnType("decimal(18,2)");
                entity.Property(e => e.Status).HasConversion<int>();
                entity.Property(e => e.CreatedDate).HasDefaultValueSql("GETUTCDATE()");
                entity.Property(e => e.LastModified).HasDefaultValueSql("GETUTCDATE()");

                // Configure relationships
                entity.HasOne(p => p.User)
                      .WithMany(u => u.Projects)
                      .HasForeignKey(p => p.UserId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(p => p.Client)
                      .WithMany(c => c.Projects)
                      .HasForeignKey(p => p.ClientId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(e => new { e.UserId, e.Title }).IsUnique();
            });

            // Configure TimeEntry
            builder.Entity<TimeEntry>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Description).IsRequired().HasMaxLength(1000);
                entity.Property(e => e.IsBilled).HasDefaultValue(false);
                entity.Property(e => e.CreatedDate).HasDefaultValueSql("GETUTCDATE()");
                entity.Property(e => e.LastModified).HasDefaultValueSql("GETUTCDATE()");

                // Configure relationships
                entity.HasOne(te => te.User)
                      .WithMany(u => u.TimeEntries)
                      .HasForeignKey(te => te.UserId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(te => te.Project)
                      .WithMany(p => p.TimeEntries)
                      .HasForeignKey(te => te.ProjectId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // Configure Invoice
            builder.Entity<Invoice>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.TotalAmount).HasColumnType("decimal(18,2)");
                entity.Property(e => e.IsPaid).HasDefaultValue(false);
                entity.Property(e => e.Notes).HasMaxLength(500);
                entity.Property(e => e.InvoiceNumber).IsRequired().HasMaxLength(100);
                entity.Property(e => e.InvoiceDate).HasDefaultValueSql("GETUTCDATE()");
                entity.Property(e => e.CreatedDate).HasDefaultValueSql("GETUTCDATE()");
                entity.Property(e => e.LastModified).HasDefaultValueSql("GETUTCDATE()");

                // Configure relationships
                entity.HasOne(i => i.User)
                      .WithMany(u => u.Invoices)
                      .HasForeignKey(i => i.UserId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(i => i.Client)
                      .WithMany(c => c.Invoices)
                      .HasForeignKey(i => i.ClientId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(e => new { e.UserId, e.InvoiceNumber }).IsUnique();
            });

            // Configure many-to-many relationship between Invoice and TimeEntry
            builder.Entity<Invoice>()
                .HasMany(i => i.TimeEntries)
                .WithMany()
                .UsingEntity<Dictionary<string, object>>(
                    "InvoiceTimeEntries",
                    j => j.HasOne<TimeEntry>().WithMany().HasForeignKey("TimeEntriesId").OnDelete(DeleteBehavior.NoAction),
                    j => j.HasOne<Invoice>().WithMany().HasForeignKey("InvoiceId").OnDelete(DeleteBehavior.NoAction)
                );

            // Seed demonstration data
            SeedData(builder);
        }

        /// <summary>
        /// Seeds the database with demonstration data for development and testing
        /// </summary>
        /// <param name="builder">Model builder instance</param>
        private void SeedData(ModelBuilder builder)
        {
            // Seed demo user
            builder.Entity<ApplicationUser>().HasData(
                new ApplicationUser
                {
                    Id = DEMO_USER_ID,
                    UserName = DEMO_USER_NAME,
                    NormalizedUserName = DEMO_USER_NAME.ToUpper(),
                    Email = DEMO_USER_EMAIL,
                    NormalizedEmail = DEMO_USER_EMAIL.ToUpper(),
                    EmailConfirmed = true,
                    FirstName = "Demo",
                    LastName = "User",
                    CompanyName = "Demo Company",
                    Address = "123 Demo Street, Demo City, DC 12345",
                    CreatedDate = DateTime.UtcNow,
                    SecurityStamp = Guid.NewGuid().ToString(),
                    ConcurrencyStamp = Guid.NewGuid().ToString(),
                    PhoneNumber = "+1-555-0123",
                    PhoneNumberConfirmed = true,
                    TwoFactorEnabled = false,
                    LockoutEnabled = true,
                    AccessFailedCount = 0
                }
            );

            // Seed clients
            builder.Entity<Client>().HasData(
                new Client
                {
                    Id = 1,
                    UserId = DEMO_USER_ID,
                    Name = "Tech Innovators Corp",
                    ContactEmail = "contact@techinnovators.com",
                    PhoneNumber = "+1-555-TECH-001",
                    Address = "456 Innovation Drive, Silicon Valley, CA 94000",
                    Notes = "Leading technology company specializing in cutting-edge software solutions. Known for innovative approaches to complex technical challenges.",
                    CreatedDate = DateTime.UtcNow,
                    IsActive = true
                },
                new Client
                {
                    Id = 2,
                    UserId = DEMO_USER_ID,
                    Name = "Global Marketing Agency",
                    ContactEmail = "info@globalmarketing.com",
                    PhoneNumber = "+1-555-MARKET-02",
                    Address = "789 Marketing Boulevard, New York, NY 10001",
                    Notes = "Full-service marketing agency with global reach. Expert in digital marketing, brand strategy, and creative campaigns.",
                    CreatedDate = DateTime.UtcNow,
                    IsActive = true
                }
            );

            // Seed projects
            builder.Entity<Project>().HasData(
                new Project
                {
                    Id = 1,
                    UserId = DEMO_USER_ID,
                    ClientId = 1, // Tech Innovators Corp
                    Title = "Q4 E-commerce Platform Relaunch",
                    Description = "Complete redesign and relaunch of the e-commerce platform with modern UI/UX, improved performance, and enhanced user experience. Includes mobile optimization, payment gateway integration, and analytics implementation.",
                    HourlyRate = 75.00m,
                    Status = ProjectStatus.Active,
                    StartDate = DateTime.UtcNow.AddDays(-30),
                    EndDate = DateTime.UtcNow.AddDays(60),
                    CreatedDate = DateTime.UtcNow,
                    LastModified = DateTime.UtcNow
                },
                new Project
                {
                    Id = 2,
                    UserId = DEMO_USER_ID,
                    ClientId = 2, // Global Marketing Agency
                    Title = "2026 Brand Strategy Documentation",
                    Description = "Comprehensive brand strategy documentation including market analysis, competitive positioning, brand guidelines, and implementation roadmap. Focus on digital transformation and brand consistency across all touchpoints.",
                    HourlyRate = 60.00m,
                    Status = ProjectStatus.Active,
                    StartDate = DateTime.UtcNow.AddDays(-15),
                    EndDate = DateTime.UtcNow.AddDays(45),
                    CreatedDate = DateTime.UtcNow,
                    LastModified = DateTime.UtcNow
                }
            );
        }
    }
}
