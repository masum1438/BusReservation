using BusReservation.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusReservation.Application.DTOs
{
    public class SeatDto
    {
        public Guid SeatId { get; set; }
        public string SeatNumber { get; set; } = default!;
        public int Row { get; set; }
        public SeatStatus Status { get; set; }
    }
}
