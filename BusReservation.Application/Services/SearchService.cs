using BusReservation.Application.DTOs;
using BusReservation.Application.Interfaces;
using BusReservation.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusReservation.Application.Services
{
    public class SearchService : ISearchService
    {
        private readonly IBusScheduleRepository _repo;

        public SearchService(IBusScheduleRepository repo)
        {
            _repo = repo;
        }

        public async Task<List<AvailableBusDto>> SearchAvailableBusesAsync(string from, string to, DateTime? journeyDate = null)
        {
            var schedules = await _repo.SearchByRouteAndDateAsync(from, to, journeyDate);

            return schedules.Select(s => new AvailableBusDto
            {
                BusScheduleId = s.Id,
                CompanyName = s.Bus.CompanyName,
                BusName = s.Bus.BusName,
                StartTime = s.StartTime,
                ArrivalTime = s.ArrivalTime,
                JourneyDate = s.JourneyDate,
                Price = s.Price,
                SeatsLeft = s.Seats.Count(x => x.Status == SeatStatus.Available)
            }).ToList();
        }
    }
}
