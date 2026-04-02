using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<Bus> Buses { get; set; }
    public DbSet<Route> Routes { get; set; }
    public DbSet<BusSchedule> BusSchedules { get; set; }
    public DbSet<Seat> Seats { get; set; }
    public DbSet<Ticket> Tickets { get; set; }
    public DbSet<Passenger> Passengers { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<SeatLock> SeatLocks { get; set; }
    // FIX: RefreshToken DbSet was missing from original
    public DbSet<RefreshToken> RefreshTokens { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ── Bus ───────────────────────────────────────────────────────────────
        modelBuilder.Entity<Bus>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.BusNumber).IsRequired().HasMaxLength(20);
            e.Property(x => x.CompanyName).IsRequired().HasMaxLength(100);
            e.Property(x => x.BusName).IsRequired().HasMaxLength(100);
            e.HasMany(x => x.Schedules)
             .WithOne(x => x.Bus)
             .HasForeignKey(x => x.BusId)
             .OnDelete(DeleteBehavior.Restrict);
        });

        // ── Route ─────────────────────────────────────────────────────────────
        modelBuilder.Entity<Domain.Entities.Route>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.FromCity).IsRequired().HasMaxLength(100);
            e.Property(x => x.ToCity).IsRequired().HasMaxLength(100);
            e.Property(x => x.BasePrice).HasPrecision(18, 2);
            e.HasMany(x => x.Schedules)
             .WithOne(x => x.Route)
             .HasForeignKey(x => x.RouteId)
             .OnDelete(DeleteBehavior.Restrict);
        });

        // ── BusSchedule ───────────────────────────────────────────────────────
        modelBuilder.Entity<BusSchedule>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Price).HasPrecision(18, 2);
            e.HasMany(x => x.Seats)
             .WithOne(x => x.BusSchedule)
             .HasForeignKey(x => x.BusScheduleId)
             .OnDelete(DeleteBehavior.Cascade);
            e.HasMany(x => x.Tickets)
             .WithOne(x => x.BusSchedule)
             .HasForeignKey(x => x.BusScheduleId)
             .OnDelete(DeleteBehavior.Restrict);
        });

        // ── Seat ──────────────────────────────────────────────────────────────
        modelBuilder.Entity<Seat>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.SeatNumber).IsRequired().HasMaxLength(10);
            // One-to-one Seat -> Ticket
            e.HasOne(x => x.Ticket)
             .WithOne(x => x.Seat)
             .HasForeignKey<Ticket>(x => x.SeatId)
             .OnDelete(DeleteBehavior.Restrict);
            e.HasMany(x => x.Locks)
             .WithOne(x => x.Seat)
             .HasForeignKey(x => x.SeatId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        // ── Ticket ────────────────────────────────────────────────────────────
        modelBuilder.Entity<Ticket>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.TicketNumber).IsRequired().HasMaxLength(50);
            e.Property(x => x.Price).HasPrecision(18, 2);
            e.HasIndex(x => x.TicketNumber).IsUnique();
            e.HasOne(x => x.Passenger)
             .WithMany(x => x.Tickets)
             .HasForeignKey(x => x.PassengerId)
             .OnDelete(DeleteBehavior.Restrict);
            // FIX: New FK to User for ownership tracking
            e.HasOne(x => x.User)
             .WithMany()
             .HasForeignKey(x => x.UserId)
             .OnDelete(DeleteBehavior.Restrict);
        });

        // ── Passenger ─────────────────────────────────────────────────────────
        modelBuilder.Entity<Passenger>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Name).IsRequired().HasMaxLength(100);
            e.Property(x => x.MobileNumber).IsRequired().HasMaxLength(15);
            e.HasIndex(x => x.MobileNumber).IsUnique();
            // FIX: Optional link to User
            e.HasOne(x => x.User)
             .WithMany()
             .HasForeignKey(x => x.UserId)
             .IsRequired(false)
             .OnDelete(DeleteBehavior.SetNull);
        });

        // ── User ──────────────────────────────────────────────────────────────
        modelBuilder.Entity<User>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Username).IsRequired().HasMaxLength(50);
            e.Property(x => x.Email).IsRequired().HasMaxLength(100);
            e.Property(x => x.PasswordHash).IsRequired();
            e.Property(x => x.FullName).IsRequired().HasMaxLength(100);
            e.Property(x => x.MobileNumber).HasMaxLength(15);
            e.HasIndex(x => x.Username).IsUnique();
            e.HasIndex(x => x.Email).IsUnique();
            // Navigation to refresh tokens
            e.HasMany(x => x.RefreshTokens)
             .WithOne(x => x.User)
             .HasForeignKey(x => x.UserId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        // ── RefreshToken ──────────────────────────────────────────────────────
        // FIX: Now extends BaseEntity so Id/CreatedAt/UpdatedAt/IsDeleted are consistent
        modelBuilder.Entity<RefreshToken>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Token).IsRequired();
            e.HasIndex(x => x.Token).IsUnique();
        });

        // ── SeatLock ──────────────────────────────────────────────────────────
        modelBuilder.Entity<SeatLock>(e =>
        {
            e.HasKey(x => x.Id);
            e.HasIndex(x => new { x.SeatId, x.IsActive });
        });
    }
}