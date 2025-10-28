using BusReservation.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusReservation.Application.Interfaces
{
    public interface IBusScheduleRepository : IRepository<BusSchedule>
    {
        Task<List<BusSchedule>> SearchByRouteAndDateAsync(string from, string to, DateTime? journeyDate = null);
    }
}
