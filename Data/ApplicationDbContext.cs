using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Staybnb.Models;

namespace Staybnb.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<Property> Properties => Set<Property>();
    public DbSet<PropertyImage> PropertyImages => Set<PropertyImage>();
    public DbSet<HostApplication> HostApplications => Set<HostApplication>();
    public DbSet<Booking> Bookings => Set<Booking>();
    public DbSet<CheckInProcess> CheckInProcesses => Set<CheckInProcess>();
    public DbSet<GuestDocument> GuestDocuments => Set<GuestDocument>();
    public DbSet<Message> Messages => Set<Message>();
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<ActivityLog> ActivityLogs => Set<ActivityLog>();
    public DbSet<Review> Reviews => Set<Review>();
    public DbSet<WishlistItem> WishlistItems => Set<WishlistItem>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Property>().Property(p => p.PricePerNight).HasColumnType("decimal(18,2)");
        builder.Entity<Property>().Property(p => p.CleaningFee).HasColumnType("decimal(18,2)");
        builder.Entity<Property>().Property(p => p.ServiceFee).HasColumnType("decimal(18,2)");
        builder.Entity<Booking>().Property(b => b.TotalPrice).HasColumnType("decimal(18,2)");

        builder.Entity<Property>()
            .HasOne(p => p.Host)
            .WithMany(u => u.Properties)
            .HasForeignKey(p => p.HostId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<HostApplication>()
            .HasOne(a => a.Applicant)
            .WithMany(u => u.HostApplications)
            .HasForeignKey(a => a.ApplicantId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Booking>()
            .HasOne(b => b.Guest)
            .WithMany(u => u.Bookings)
            .HasForeignKey(b => b.GuestId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Booking>()
            .HasOne(b => b.Property)
            .WithMany(p => p.Bookings)
            .HasForeignKey(b => b.PropertyId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Message>()
            .HasOne(m => m.Sender)
            .WithMany()
            .HasForeignKey(m => m.SenderId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Message>()
            .HasOne(m => m.Recipient)
            .WithMany()
            .HasForeignKey(m => m.RecipientId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Review>()
            .HasOne(r => r.Property)
            .WithMany(p => p.Reviews)
            .HasForeignKey(r => r.PropertyId)
            .OnDelete(DeleteBehavior.Restrict);

       builder.Entity<Review>()
            .HasOne(r => r.Reviewer)
            .WithMany()
            .HasForeignKey(r => r.ReviewerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<WishlistItem>()
            .HasOne(w => w.Property)
            .WithMany().HasForeignKey(w => w.PropertyId)
            .OnDelete(DeleteBehavior.Cascade); builder
            .Entity<WishlistItem>().HasOne(w => w.User)
            .WithMany().HasForeignKey(w => w.UserId)
            .OnDelete(DeleteBehavior.Cascade); builder
            .Entity<WishlistItem>()
            .HasIndex(w => new { w.UserId, w.PropertyId })
            .IsUnique();
    }

}
