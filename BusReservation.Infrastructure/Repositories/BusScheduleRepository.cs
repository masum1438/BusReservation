using BusReservation.Application.Interfaces;
using BusReservation.Domain.Entities;
using BusReservation.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusReservation.Infrastructure.Repositories
{
    public class BusScheduleRepository : RepositoryBase<BusSchedule>, IBusScheduleRepository
    {
        public BusScheduleRepository(AppDbContext db) : base(db) { }

       

        public async Task<List<BusSchedule>> SearchByRouteAndDateAsync(string from, string to, DateTime? journeyDate = null)
        {
            var query = _db.BusSchedules
                .Include(s => s.Bus)
                .Include(s => s.Route)
                .Include(s => s.Seats)
                .AsQueryable();

            query = query.Where(s =>
                EF.Functions.ILike(s.Route.From, from) &&
                EF.Functions.ILike(s.Route.To, to));

            if (journeyDate.HasValue)
            {
                var utcDate = DateTime.SpecifyKind(journeyDate.Value, DateTimeKind.Utc);
                query = query.Where(s => s.JourneyDate.Date == utcDate.Date);
            }

            return await query.ToListAsync();
        }

    }
}
