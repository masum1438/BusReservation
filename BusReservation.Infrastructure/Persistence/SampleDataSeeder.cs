using BusReservation.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusReservation.Infrastructure.Persistence
{
    public static class SampleDataSeeder
    {
        public static async Task SeedAsync(AppDbContext db)
        {
            if (db.Buses.Any()) return;

            var route = new Route { Id = Guid.NewGuid(), From = "Dhaka", To = "Chattogram" };
            db.Routes.Add(route);

            var bus = new Bus
            {
                Id = Guid.NewGuid(),
                CompanyName = "Wafi Travels",
                BusName = "Wafi Super Deluxe",
                TotalSeats = 20
            };
            db.Buses.Add(bus);

            var schedule = new BusSchedule
            {
                Id = Guid.NewGuid(),
                Bus = bus,
                Route = route,
                JourneyDate = DateTime.UtcNow.Date.AddDays(2),
                StartTime = new TimeSpan(8, 0, 0),
                ArrivalTime = new TimeSpan(12, 0, 0),
                Price = 800
            };
            db.BusSchedules.Add(schedule);

            // create seats:
            for (int i = 1; i <= bus.TotalSeats; i++)
            {
                db.Seats.Add(new Seat
                {
                    Id = Guid.NewGuid(),
                    BusSchedule = schedule,
                    SeatNumber = i.ToString(),
                    Row = (i - 1) / 4 + 1,
                    Status = SeatStatus.Available
                });
            }

            await db.SaveChangesAsync();
        }
    }
}
