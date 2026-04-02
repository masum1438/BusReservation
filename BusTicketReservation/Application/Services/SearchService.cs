using Application.DTOs;
using Domain.Entities;
using Domain.Interfaces;

namespace Application.Services;

public interface ISearchService
{
    Task<List<AvailableBusDto>> SearchAvailableBusesAsync(string from, string to, DateTime journeyDate);
}

public class SearchService : ISearchService
{
    private readonly IUnitOfWork _unitOfWork;

    public SearchService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<List<AvailableBusDto>> SearchAvailableBusesAsync(string from, string to, DateTime journeyDate)
    {
        var scheduleRepo = _unitOfWork.Repository<BusSchedule>();
        var seatRepo = _unitOfWork.Repository<Seat>();
        var busRepo = _unitOfWork.Repository<Bus>();
        var routeRepo = _unitOfWork.Repository<Domain.Entities.Route>();

        // Get all schedules for the given date, not deleted
        var allSchedules = await scheduleRepo.FindAsync(bs =>
            bs.DepartureTime.Date == journeyDate.Date && !bs.IsDeleted);

        var result = new List<AvailableBusDto>();

        foreach (var schedule in allSchedules)
        {
            var route = await routeRepo.GetByIdAsync(schedule.RouteId);
            if (route == null || route.IsDeleted) continue;
            if (!route.FromCity.Equals(from, StringComparison.OrdinalIgnoreCase)) continue;
            if (!route.ToCity.Equals(to, StringComparison.OrdinalIgnoreCase)) continue;

            var bus = await busRepo.GetByIdAsync(schedule.BusId);
            if (bus == null || bus.IsDeleted) continue;

            var bookedSeats = await seatRepo.FindAsync(s =>
                s.BusScheduleId == schedule.Id && s.Status != SeatStatus.Available);

            var seatsLeft = bus.TotalSeats - bookedSeats.Count();

            result.Add(new AvailableBusDto
            {
                BusScheduleId = schedule.Id,
                CompanyName = bus.CompanyName,
                BusName = bus.BusName,
                StartTime = schedule.DepartureTime,
                ArrivalTime = schedule.ArrivalTime,
                TotalSeats = bus.TotalSeats,
                SeatsLeft = seatsLeft,
                Price = schedule.Price
            });
        }

        return result.OrderBy(b => b.StartTime).ToList();
    }
}