using BusReservation.Application.Interfaces;
using BusReservation.Domain.Entities;
using BusReservation.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusReservation.Infrastructure.Repositories
{
    public class TicketRepository : RepositoryBase<Ticket>, ITicketRepository
    {
        public TicketRepository(AppDbContext db) : base(db) { }
    }
}
