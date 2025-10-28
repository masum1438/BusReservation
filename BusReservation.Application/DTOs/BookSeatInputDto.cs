using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusReservation.Application.DTOs
{
    public class BookSeatInputDto
    {
        public Guid BusScheduleId { get; set; }
        public Guid SeatId { get; set; }
        public string PassengerName { get; set; } = default!;
        public string Mobile { get; set; } = default!;
        public string BoardingPoint { get; set; } = default!;
        public string DroppingPoint { get; set; } = default!;
        public bool ConfirmNow { get; set; } = true;
    }
}
