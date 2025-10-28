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
    public class SeatRepository : RepositoryBase<Seat>, ISeatRepository
    {
        public SeatRepository(AppDbContext db) : base(db) { }

        public async Task<List<Seat>> GetSeatsByScheduleIdAsync(Guid scheduleId)
        {
            return await _db.Seats
                .Where(s => s.BusScheduleId == scheduleId)
                .Include(s => s.Ticket)
                .ToListAsync();
        }
    }
}
