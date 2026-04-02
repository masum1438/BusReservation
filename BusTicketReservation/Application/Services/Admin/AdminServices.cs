using Application.DTOs.Admin;
using Domain.Entities;
using Domain.Interfaces;

namespace Application.Services.Admin;

// ══════════════════════════════════════════════════════════════════════════════
// BUS SERVICE
// ══════════════════════════════════════════════════════════════════════════════

public interface IAdminBusService
{
    Task<BusResponseDto> CreateBusAsync(BusCreateDto dto);
    Task<BusResponseDto> UpdateBusAsync(Guid id, BusCreateDto dto);
    Task DeleteBusAsync(Guid id);
    Task<BusResponseDto> GetBusAsync(Guid id);
    Task<List<BusResponseDto>> GetAllBusesAsync();
}

public class AdminBusService : IAdminBusService
{
    private readonly IUnitOfWork _unitOfWork;

    public AdminBusService(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<BusResponseDto> CreateBusAsync(BusCreateDto dto)
    {
        var repo = _unitOfWork.Repository<Bus>();
        var bus = new Bus
        {
            Id = Guid.NewGuid(),
            BusNumber = dto.BusNumber,
            CompanyName = dto.CompanyName,
            BusName = dto.BusName,
            TotalSeats = dto.TotalSeats,
            Type = dto.Type,
            CreatedAt = DateTime.UtcNow
        };
        await repo.AddAsync(bus);
        await _unitOfWork.SaveChangesAsync();
        return MapBus(bus);
    }

    public async Task<BusResponseDto> UpdateBusAsync(Guid id, BusCreateDto dto)
    {
        var repo = _unitOfWork.Repository<Bus>();
        var bus = await repo.GetByIdAsync(id)
            ?? throw new KeyNotFoundException("Bus not found");

        bus.BusNumber = dto.BusNumber;
        bus.CompanyName = dto.CompanyName;
        bus.BusName = dto.BusName;
        bus.TotalSeats = dto.TotalSeats;
        bus.Type = dto.Type;
        bus.UpdatedAt = DateTime.UtcNow;

        await repo.UpdateAsync(bus);
        await _unitOfWork.SaveChangesAsync();
        return MapBus(bus);
    }

    public async Task DeleteBusAsync(Guid id)
    {
        var repo = _unitOfWork.Repository<Bus>();
        var bus = await repo.GetByIdAsync(id)
            ?? throw new KeyNotFoundException("Bus not found");
        bus.IsDeleted = true;
        bus.UpdatedAt = DateTime.UtcNow;
        await repo.UpdateAsync(bus);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<BusResponseDto> GetBusAsync(Guid id)
    {
        var bus = await _unitOfWork.Repository<Bus>().GetByIdAsync(id);
        if (bus == null || bus.IsDeleted) throw new KeyNotFoundException("Bus not found");
        return MapBus(bus);
    }

    public async Task<List<BusResponseDto>> GetAllBusesAsync()
    {
        var buses = await _unitOfWork.Repository<Bus>().FindAsync(b => !b.IsDeleted);
        return buses.Select(MapBus).ToList();
    }

    private static BusResponseDto MapBus(Bus bus) => new()
    {
        Id = bus.Id,
        BusNumber = bus.BusNumber,
        CompanyName = bus.CompanyName,
        BusName = bus.BusName,
        TotalSeats = bus.TotalSeats,
        Type = bus.Type.ToString(),
        CreatedAt = bus.CreatedAt
    };
}

// ══════════════════════════════════════════════════════════════════════════════
// ROUTE SERVICE
// ══════════════════════════════════════════════════════════════════════════════

public interface IAdminRouteService
{
    Task<RouteResponseDto> CreateRouteAsync(RouteCreateDto dto);
    Task<RouteResponseDto> UpdateRouteAsync(Guid id, RouteCreateDto dto);
    Task DeleteRouteAsync(Guid id);
    Task<RouteResponseDto> GetRouteAsync(Guid id);
    Task<List<RouteResponseDto>> GetAllRoutesAsync();
}

public class AdminRouteService : IAdminRouteService
{
    private readonly IUnitOfWork _unitOfWork;

    public AdminRouteService(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<RouteResponseDto> CreateRouteAsync(RouteCreateDto dto)
    {
        var route = new Domain.Entities.Route
        {
            Id = Guid.NewGuid(),
            FromCity = dto.FromCity,
            ToCity = dto.ToCity,
            BasePrice = dto.BasePrice,
            Distance = dto.Distance,
            CreatedAt = DateTime.UtcNow
        };
        await _unitOfWork.Repository<Domain.Entities.Route>().AddAsync(route);
        await _unitOfWork.SaveChangesAsync();
        return MapRoute(route);
    }

    public async Task<RouteResponseDto> UpdateRouteAsync(Guid id, RouteCreateDto dto)
    {
        var repo = _unitOfWork.Repository<Domain.Entities.Route>();
        var route = await repo.GetByIdAsync(id)
            ?? throw new KeyNotFoundException("Route not found");
        route.FromCity = dto.FromCity;
        route.ToCity = dto.ToCity;
        route.BasePrice = dto.BasePrice;
        route.Distance = dto.Distance;
        route.UpdatedAt = DateTime.UtcNow;
        await repo.UpdateAsync(route);
        await _unitOfWork.SaveChangesAsync();
        return MapRoute(route);
    }

    public async Task DeleteRouteAsync(Guid id)
    {
        var repo = _unitOfWork.Repository<Domain.Entities.Route>();
        var route = await repo.GetByIdAsync(id)
            ?? throw new KeyNotFoundException("Route not found");
        route.IsDeleted = true;
        route.UpdatedAt = DateTime.UtcNow;
        await repo.UpdateAsync(route);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<RouteResponseDto> GetRouteAsync(Guid id)
    {
        var route = await _unitOfWork.Repository<Domain.Entities.Route>().GetByIdAsync(id);
        if (route == null || route.IsDeleted) throw new KeyNotFoundException("Route not found");
        return MapRoute(route);
    }

    public async Task<List<RouteResponseDto>> GetAllRoutesAsync()
    {
        var routes = await _unitOfWork.Repository<Domain.Entities.Route>().FindAsync(r => !r.IsDeleted);
        return routes.Select(MapRoute).ToList();
    }

    private static RouteResponseDto MapRoute(Domain.Entities.Route r) => new()
    {
        Id = r.Id,
        FromCity = r.FromCity,
        ToCity = r.ToCity,
        BasePrice = r.BasePrice,
        Distance = r.Distance
    };
}

// ══════════════════════════════════════════════════════════════════════════════
// SCHEDULE SERVICE
// ══════════════════════════════════════════════════════════════════════════════

public interface IAdminScheduleService
{
    Task<ScheduleResponseDto> CreateScheduleAsync(ScheduleCreateDto dto);
    Task<ScheduleResponseDto> UpdateScheduleAsync(Guid id, ScheduleCreateDto dto);
    Task DeleteScheduleAsync(Guid id);
    Task<ScheduleResponseDto> GetScheduleAsync(Guid id);
    Task<List<ScheduleResponseDto>> GetAllSchedulesAsync();
    Task<List<ScheduleResponseDto>> GetSchedulesByBusAsync(Guid busId);
    Task<List<ScheduleResponseDto>> GetSchedulesByRouteAsync(Guid routeId);
}

public class AdminScheduleService : IAdminScheduleService
{
    private readonly IUnitOfWork _unitOfWork;

    public AdminScheduleService(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<ScheduleResponseDto> CreateScheduleAsync(ScheduleCreateDto dto)
    {
        var bus = await _unitOfWork.Repository<Bus>().GetByIdAsync(dto.BusId)
            ?? throw new KeyNotFoundException("Bus not found");
        var route = await _unitOfWork.Repository<Domain.Entities.Route>().GetByIdAsync(dto.RouteId)
            ?? throw new KeyNotFoundException("Route not found");

        var schedule = new BusSchedule
        {
            Id = Guid.NewGuid(),
            BusId = dto.BusId,
            RouteId = dto.RouteId,
            DepartureTime = dto.DepartureTime,
            ArrivalTime = dto.ArrivalTime,
            Price = dto.Price,
            CreatedAt = DateTime.UtcNow
        };

        await _unitOfWork.Repository<BusSchedule>().AddAsync(schedule);
        await CreateSeatsForScheduleAsync(schedule.Id, bus.TotalSeats);
        await _unitOfWork.SaveChangesAsync();

        return await MapScheduleAsync(schedule);
    }

    public async Task<ScheduleResponseDto> UpdateScheduleAsync(Guid id, ScheduleCreateDto dto)
    {
        var repo = _unitOfWork.Repository<BusSchedule>();
        var schedule = await repo.GetByIdAsync(id)
            ?? throw new KeyNotFoundException("Schedule not found");

        schedule.BusId = dto.BusId;
        schedule.RouteId = dto.RouteId;
        schedule.DepartureTime = dto.DepartureTime;
        schedule.ArrivalTime = dto.ArrivalTime;
        schedule.Price = dto.Price;
        schedule.UpdatedAt = DateTime.UtcNow;

        await repo.UpdateAsync(schedule);
        await _unitOfWork.SaveChangesAsync();
        return await MapScheduleAsync(schedule);
    }

    public async Task DeleteScheduleAsync(Guid id)
    {
        var repo = _unitOfWork.Repository<BusSchedule>();
        var schedule = await repo.GetByIdAsync(id)
            ?? throw new KeyNotFoundException("Schedule not found");
        schedule.IsDeleted = true;
        schedule.UpdatedAt = DateTime.UtcNow;
        await repo.UpdateAsync(schedule);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<ScheduleResponseDto> GetScheduleAsync(Guid id)
    {
        var schedule = await _unitOfWork.Repository<BusSchedule>().GetByIdAsync(id);
        if (schedule == null || schedule.IsDeleted) throw new KeyNotFoundException("Schedule not found");
        return await MapScheduleAsync(schedule);
    }

    public async Task<List<ScheduleResponseDto>> GetAllSchedulesAsync()
        => await MapScheduleListAsync(await _unitOfWork.Repository<BusSchedule>().FindAsync(s => !s.IsDeleted));

    public async Task<List<ScheduleResponseDto>> GetSchedulesByBusAsync(Guid busId)
        => await MapScheduleListAsync(await _unitOfWork.Repository<BusSchedule>()
            .FindAsync(s => s.BusId == busId && !s.IsDeleted));

    public async Task<List<ScheduleResponseDto>> GetSchedulesByRouteAsync(Guid routeId)
        => await MapScheduleListAsync(await _unitOfWork.Repository<BusSchedule>()
            .FindAsync(s => s.RouteId == routeId && !s.IsDeleted));

    private async Task<List<ScheduleResponseDto>> MapScheduleListAsync(IEnumerable<BusSchedule> schedules)
    {
        var result = new List<ScheduleResponseDto>();
        foreach (var s in schedules) result.Add(await MapScheduleAsync(s));
        return result;
    }

    private async Task CreateSeatsForScheduleAsync(Guid scheduleId, int totalSeats)
    {
        var seatRepo = _unitOfWork.Repository<Seat>();
        const int seatsPerRow = 4;

        for (int i = 1; i <= totalSeats; i++)
        {
            var row = (i - 1) / seatsPerRow + 1;
            var col = (i - 1) % seatsPerRow;
            var letter = (char)('A' + col);

            await seatRepo.AddAsync(new Seat
            {
                Id = Guid.NewGuid(),
                BusScheduleId = scheduleId,
                SeatNumber = $"{row}{letter}",
                RowNumber = row,
                ColumnNumber = col + 1,
                Status = SeatStatus.Available,
                CreatedAt = DateTime.UtcNow
            });
        }
    }

    private async Task<ScheduleResponseDto> MapScheduleAsync(BusSchedule schedule)
    {
        var bus = await _unitOfWork.Repository<Bus>().GetByIdAsync(schedule.BusId);
        var route = await _unitOfWork.Repository<Domain.Entities.Route>().GetByIdAsync(schedule.RouteId);
        var seats = await _unitOfWork.Repository<Seat>().FindAsync(s =>
            s.BusScheduleId == schedule.Id && s.Status == SeatStatus.Available);

        return new ScheduleResponseDto
        {
            Id = schedule.Id,
            BusName = bus?.BusName ?? string.Empty,
            CompanyName = bus?.CompanyName ?? string.Empty,
            FromCity = route?.FromCity ?? string.Empty,
            ToCity = route?.ToCity ?? string.Empty,
            DepartureTime = schedule.DepartureTime,
            ArrivalTime = schedule.ArrivalTime,
            Price = schedule.Price,
            AvailableSeats = seats.Count()
        };
    }
}

// ══════════════════════════════════════════════════════════════════════════════
// BOOKING SERVICE (Admin)
// ══════════════════════════════════════════════════════════════════════════════

public interface IAdminBookingService
{
    Task<List<BookingReportDto>> GetAllBookingsAsync(BookingFilterDto filter);
    Task<BookingReportDto> GetBookingDetailsAsync(Guid ticketId);
    Task<BookingReportDto> GetBookingByTicketNumberAsync(string ticketNumber);
    Task CancelBookingAsync(Guid ticketId, string reason);
    Task<List<BookingReportDto>> GetBookingsByDateRangeAsync(DateTime from, DateTime to);
    Task<BookingStatisticsDto> GetBookingStatisticsAsync(DateTime? from, DateTime? to);
}

public class AdminBookingService : IAdminBookingService
{
    private readonly IUnitOfWork _unitOfWork;

    public AdminBookingService(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<List<BookingReportDto>> GetAllBookingsAsync(BookingFilterDto filter)
    {
        var tickets = (await _unitOfWork.Repository<Ticket>().FindAsync(t => !t.IsDeleted)).AsQueryable();

        if (filter.FromDate.HasValue)
            tickets = tickets.Where(t => t.BookingDate >= filter.FromDate.Value);
        if (filter.ToDate.HasValue)
            tickets = tickets.Where(t => t.BookingDate <= filter.ToDate.Value);
        if (!string.IsNullOrEmpty(filter.TicketNumber))
            tickets = tickets.Where(t => t.TicketNumber.Contains(filter.TicketNumber));
        if (!string.IsNullOrEmpty(filter.Status) && Enum.TryParse<TicketStatus>(filter.Status, out var status))
            tickets = tickets.Where(t => t.Status == status);
        if (!string.IsNullOrEmpty(filter.BusName))
        {
            // Filter by bus name: load schedule/bus to match
            var matched = new List<Ticket>();
            foreach (var t in tickets)
            {
                var schedule = await _unitOfWork.Repository<BusSchedule>().GetByIdAsync(t.BusScheduleId);
                if (schedule == null) continue;
                var bus = await _unitOfWork.Repository<Bus>().GetByIdAsync(schedule.BusId);
                if (bus != null && bus.BusName.Contains(filter.BusName, StringComparison.OrdinalIgnoreCase))
                    matched.Add(t);
            }
            tickets = matched.AsQueryable();
        }

        var paged = tickets.Skip((filter.Page - 1) * filter.PageSize).Take(filter.PageSize);
        var result = new List<BookingReportDto>();
        foreach (var t in paged) result.Add(await MapToReportDtoAsync(t));
        return result;
    }

    public async Task<BookingReportDto> GetBookingDetailsAsync(Guid ticketId)
    {
        var ticket = await _unitOfWork.Repository<Ticket>().GetByIdAsync(ticketId)
            ?? throw new KeyNotFoundException("Ticket not found");
        return await MapToReportDtoAsync(ticket);
    }

    public async Task<BookingReportDto> GetBookingByTicketNumberAsync(string ticketNumber)
    {
        var tickets = await _unitOfWork.Repository<Ticket>()
            .FindAsync(t => t.TicketNumber == ticketNumber && !t.IsDeleted);
        var ticket = tickets.FirstOrDefault()
            ?? throw new KeyNotFoundException("Ticket not found");
        return await MapToReportDtoAsync(ticket);
    }

    public async Task CancelBookingAsync(Guid ticketId, string reason)
    {
        await _unitOfWork.BeginTransactionAsync();
        try
        {
            var ticketRepo = _unitOfWork.Repository<Ticket>();
            var seatRepo = _unitOfWork.Repository<Seat>();

            var ticket = await ticketRepo.GetByIdAsync(ticketId)
                ?? throw new KeyNotFoundException("Ticket not found");
            if (ticket.Status == TicketStatus.Cancelled)
                throw new InvalidOperationException("Ticket is already cancelled");

            ticket.Status = TicketStatus.Cancelled;
            ticket.UpdatedAt = DateTime.UtcNow;
            await ticketRepo.UpdateAsync(ticket);

            var seat = await seatRepo.GetByIdAsync(ticket.SeatId);
            if (seat != null)
            {
                seat.Status = SeatStatus.Available;
                seat.UpdatedAt = DateTime.UtcNow;
                await seatRepo.UpdateAsync(seat);
            }

            await _unitOfWork.CommitAsync();
        }
        catch
        {
            await _unitOfWork.RollbackAsync();
            throw;
        }
    }

    public async Task<List<BookingReportDto>> GetBookingsByDateRangeAsync(DateTime from, DateTime to)
    {
        var tickets = await _unitOfWork.Repository<Ticket>()
            .FindAsync(t => t.BookingDate >= from && t.BookingDate <= to && !t.IsDeleted);
        var result = new List<BookingReportDto>();
        foreach (var t in tickets) result.Add(await MapToReportDtoAsync(t));
        return result;
    }

    public async Task<BookingStatisticsDto> GetBookingStatisticsAsync(DateTime? from, DateTime? to)
    {
        var all = (await _unitOfWork.Repository<Ticket>().FindAsync(t => !t.IsDeleted)).AsQueryable();
        if (from.HasValue) all = all.Where(t => t.BookingDate >= from.Value);
        if (to.HasValue) all = all.Where(t => t.BookingDate <= to.Value);
        var list = all.ToList();

        var stats = new BookingStatisticsDto
        {
            TotalBookings = list.Count,
            TotalRevenue = list.Sum(t => t.Price),
            ConfirmedBookings = list.Count(t => t.Status == TicketStatus.Confirmed),
            CancelledBookings = list.Count(t => t.Status == TicketStatus.Cancelled),
            PendingBookings = list.Count(t => t.Status == TicketStatus.Pending),
            AverageTicketPrice = list.Any() ? list.Average(t => t.Price) : 0
        };

        foreach (var ticket in list)
        {
            var schedule = await _unitOfWork.Repository<BusSchedule>().GetByIdAsync(ticket.BusScheduleId);
            if (schedule == null) continue;

            var bus = await _unitOfWork.Repository<Bus>().GetByIdAsync(schedule.BusId);
            if (bus != null)
            {
                stats.BookingsByBus.TryAdd(bus.BusName, 0);
                stats.BookingsByBus[bus.BusName]++;
            }

            var route = await _unitOfWork.Repository<Domain.Entities.Route>().GetByIdAsync(schedule.RouteId);
            if (route != null)
            {
                var key = $"{route.FromCity} → {route.ToCity}";
                stats.BookingsByRoute.TryAdd(key, 0);
                stats.BookingsByRoute[key]++;
            }
        }

        return stats;
    }

    private async Task<BookingReportDto> MapToReportDtoAsync(Ticket ticket)
    {
        var schedule = await _unitOfWork.Repository<BusSchedule>().GetByIdAsync(ticket.BusScheduleId);
        var seat = await _unitOfWork.Repository<Seat>().GetByIdAsync(ticket.SeatId);
        var passenger = await _unitOfWork.Repository<Passenger>().GetByIdAsync(ticket.PassengerId);
        Bus? bus = null;
        Domain.Entities.Route? route = null;

        if (schedule != null)
        {
            bus = await _unitOfWork.Repository<Bus>().GetByIdAsync(schedule.BusId);
            route = await _unitOfWork.Repository<Domain.Entities.Route>().GetByIdAsync(schedule.RouteId);
        }

        return new BookingReportDto
        {
            TicketId = ticket.Id,
            TicketNumber = ticket.TicketNumber,
            PassengerName = passenger?.Name ?? string.Empty,
            MobileNumber = passenger?.MobileNumber ?? string.Empty,
            BusName = bus?.BusName ?? string.Empty,
            FromCity = route?.FromCity ?? string.Empty,
            ToCity = route?.ToCity ?? string.Empty,
            DepartureTime = schedule?.DepartureTime ?? DateTime.MinValue,
            SeatNumber = seat?.SeatNumber ?? string.Empty,
            Price = ticket.Price,
            Status = ticket.Status.ToString(),
            BookingDate = ticket.BookingDate,
            PaymentDate = ticket.PaymentDate
        };
    }
}