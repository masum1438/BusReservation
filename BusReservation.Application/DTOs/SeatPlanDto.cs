using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusReservation.Application.DTOs
{
    public class SeatPlanDto
    {
        public Guid BusScheduleId { get; set; }
        public List<SeatDto> Seats { get; set; } = new();
    }
}
