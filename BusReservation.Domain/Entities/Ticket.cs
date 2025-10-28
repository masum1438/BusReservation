using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusReservation.Domain.Entities
{
    public class Ticket
    {
        public Guid Id { get; set; }
        public Guid SeatId { get; set; }
        public Seat Seat { get; set; } = default!;
        public string PassengerName { get; set; } = default!;
        public string Mobile { get; set; } = default!;
        public string BoardingPoint { get; set; } = default!;
        public string DroppingPoint { get; set; } = default!;
        public DateTime BookedAt { get; set; }
        public bool IsConfirmed { get; set; } = false; // if true -> Sold else Booked
    }
}
