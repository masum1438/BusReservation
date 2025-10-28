using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusReservation.Application.DTOs
{
    public class AvailableBusDto
    {
        public Guid BusScheduleId { get; set; }
        public string CompanyName { get; set; } = default!;
        public string BusName { get; set; } = default!;
        public TimeSpan StartTime { get; set; }
        public TimeSpan ArrivalTime { get; set; }
        public int SeatsLeft { get; set; }
        public decimal Price { get; set; }
        public DateTime JourneyDate { get; set; }
    }
}
