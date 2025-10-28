using BusReservation.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusReservation.Application.Interfaces
{
    public interface ISearchService
    {
        Task<List<AvailableBusDto>> SearchAvailableBusesAsync(string from, string to, DateTime? journeyDate = null);
    }
}
