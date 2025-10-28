using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace BusReservation.Domain.Entities
{
    public enum SeatStatus { Available, Booked, Sold }

    public class Seat
    {
        public Guid Id { get; set; }
        public Guid BusScheduleId { get; set; }
        public BusSchedule BusSchedule { get; set; } = default!;
        public string SeatNumber { get; set; } = default!; // e.g. "1A" or "1"
        public int Row { get; set; }
        public SeatStatus Status { get; set; } = SeatStatus.Available;
        public Ticket? Ticket { get; set; }
    }
}
