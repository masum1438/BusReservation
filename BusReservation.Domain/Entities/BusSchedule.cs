using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusReservation.Domain.Entities
{
    public class BusSchedule
    {
        public Guid Id { get; set; }
        public Guid BusId { get; set; }
        public Bus Bus { get; set; } = default!;
        public Guid RouteId { get; set; }
        public Route Route { get; set; } = default!;
        public DateTime JourneyDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan ArrivalTime { get; set; }
        public decimal Price { get; set; }
        public ICollection<Seat> Seats { get; set; } = new List<Seat>();
    }
}
