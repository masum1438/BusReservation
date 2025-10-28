using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusReservation.Application.DTOs
{
    public class BookSeatResultDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = default!;
        public Guid? TicketId { get; set; }
    }
}
