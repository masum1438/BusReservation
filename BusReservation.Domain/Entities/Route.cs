using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusReservation.Domain.Entities
{
    public class Route
    {
        public Guid Id { get; set; }
        public string From { get; set; } = default!;
        public string To { get; set; } = default!;
    }
}
