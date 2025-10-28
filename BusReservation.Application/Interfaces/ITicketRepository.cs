using BusReservation.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusReservation.Application.Interfaces
{
    public interface ITicketRepository : IRepository<Ticket> { }

}
