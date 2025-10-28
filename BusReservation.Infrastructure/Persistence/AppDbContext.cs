using BusReservation.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusReservation.Infrastructure.Persistence
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> opts) : base(opts) { }

        public DbSet<Bus> Buses => Set<Bus>();
        public DbSet<Route> Routes => Set<Route>();
        public DbSet<BusSchedule> BusSchedules => Set<BusSchedule>();
        public DbSet<Seat> Seats => Set<Seat>();
        public DbSet<Ticket> Tickets => Set<Ticket>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // simple seat seeding sample removed for brevity
            modelBuilder.Entity<Seat>().Property(s => s.Status).HasConversion<string>();
        }
    }
}
