using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusReservation.Domain.Entities
{
    public class Bus
    {
        public Guid Id { get; set; }
        public string CompanyName { get; set; } = default!;
        public string BusName { get; set; } = default!;
        public int TotalSeats { get; set; }
        public ICollection<BusSchedule> Schedules { get; set; } = new List<BusSchedule>();
    }
}
