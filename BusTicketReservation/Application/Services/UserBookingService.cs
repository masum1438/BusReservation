using Application.DTOs;
using Domain.Entities;
using Domain.Interfaces;

namespace Application.Services;

public interface IUserBookingService
{
    Task<List<UserBookingDto>> GetUserBookingsAsync(Guid userId);
    Task<TicketDetailDto> GetTicketByGuidAsync(Guid ticketId, Guid userId);
    Task<TicketDetailDto> GetTicketByNumberAsync(string ticketNumber, Guid userId);
    Task<bool> CancelUserBookingAsync(Guid ticketId, Guid userId);
}

public class UserBookingService : IUserBookingService
{
    private readonly IUnitOfWork _unitOfWork;

    public UserBookingService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    // FIX: Query by ticket.UserId directly — previous code compared Passenger.Id to userId which always failed
    public async Task<List<UserBookingDto>> GetUserBookingsAsync(Guid userId)
    {
        var tickets = await _unitOfWork.Repository<Ticket>()
            .FindAsync(t => t.UserId == userId && !t.IsDeleted);

        var result = new List<UserBookingDto>();
        foreach (var ticket in tickets)
            result.Add(await MapToUserBookingDtoAsync(ticket));

        return result.OrderByDescending(b => b.BookingDate).ToList();
    }

    // FIX: Ownership verified against ticket.UserId
    public async Task<TicketDetailDto> GetTicketByGuidAsync(Guid ticketId, Guid userId)
    {
        var ticket = await _unitOfWork.Repository<Ticket>().GetByIdAsync(ticketId);
        if (ticket == null || ticket.IsDeleted)
            throw new KeyNotFoundException("Ticket not found");

        if (ticket.UserId != userId)
            throw new UnauthorizedAccessException("Access denied");

        return await MapToTicketDetailDtoAsync(ticket);
    }

    public async Task<TicketDetailDto> GetTicketByNumberAsync(string ticketNumber, Guid userId)
    {
        var tickets = await _unitOfWork.Repository<Ticket>()
            .FindAsync(t => t.TicketNumber == ticketNumber && !t.IsDeleted);
        var ticket = tickets.FirstOrDefault()
            ?? throw new KeyNotFoundException("Ticket not found");

        // FIX: Enforce ownership
        if (ticket.UserId != userId)
            throw new UnauthorizedAccessException("Access denied");

        return await MapToTicketDetailDtoAsync(ticket);
    }

    public async Task<bool> CancelUserBookingAsync(Guid ticketId, Guid userId)
    {
        await _unitOfWork.BeginTransactionAsync();

        try
        {
            var ticketRepo = _unitOfWork.Repository<Ticket>();
            var seatRepo = _unitOfWork.Repository<Seat>();
            var scheduleRepo = _unitOfWork.Repository<BusSchedule>();

            var ticket = await ticketRepo.GetByIdAsync(ticketId)
                ?? throw new KeyNotFoundException("Ticket not found");

            // FIX: Use ticket.UserId for ownership check — Passenger.Id != User.Id
            if (ticket.UserId != userId)
                throw new UnauthorizedAccessException("Unauthorized to cancel this booking");

            if (ticket.Status == TicketStatus.Cancelled)
                throw new InvalidOperationException("Ticket is already cancelled");

            var schedule = await scheduleRepo.GetByIdAsync(ticket.BusScheduleId);
            if (schedule != null && schedule.DepartureTime <= DateTime.UtcNow.AddHours(2))
                throw new InvalidOperationException("Cannot cancel ticket within 2 hours of departure");

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
            return true;
        }
        catch
        {
            await _unitOfWork.RollbackAsync();
            throw;
        }
    }

    // ── Private Helpers ────────────────────────────────────────────────────────

    private async Task<UserBookingDto> MapToUserBookingDtoAsync(Ticket ticket)
    {
        var schedule = await _unitOfWork.Repository<BusSchedule>().GetByIdAsync(ticket.BusScheduleId);
        var seat = await _unitOfWork.Repository<Seat>().GetByIdAsync(ticket.SeatId);
        Bus? bus = null;
        Domain.Entities.Route? route = null;

        if (schedule != null)
        {
            bus = await _unitOfWork.Repository<Bus>().GetByIdAsync(schedule.BusId);
            route = await _unitOfWork.Repository<Domain.Entities.Route>().GetByIdAsync(schedule.RouteId);
        }

        return new UserBookingDto
        {
            TicketId = ticket.Id,
            TicketNumber = ticket.TicketNumber,
            BusName = bus?.BusName ?? string.Empty,
            CompanyName = bus?.CompanyName ?? string.Empty,
            FromCity = route?.FromCity ?? string.Empty,
            ToCity = route?.ToCity ?? string.Empty,
            DepartureTime = schedule?.DepartureTime ?? DateTime.MinValue,
            ArrivalTime = schedule?.ArrivalTime ?? DateTime.MinValue,
            SeatNumber = seat?.SeatNumber ?? string.Empty,
            Price = ticket.Price,
            Status = ticket.Status.ToString(),
            BookingDate = ticket.BookingDate,
            PaymentDate = ticket.PaymentDate
        };
    }

    private async Task<TicketDetailDto> MapToTicketDetailDtoAsync(Ticket ticket)
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

        return new TicketDetailDto
        {
            TicketId = ticket.Id,
            TicketNumber = ticket.TicketNumber,
            PassengerName = passenger?.Name ?? string.Empty,
            MobileNumber = passenger?.MobileNumber ?? string.Empty,
            Email = passenger?.Email ?? string.Empty,
            BusName = bus?.BusName ?? string.Empty,
            CompanyName = bus?.CompanyName ?? string.Empty,
            FromCity = route?.FromCity ?? string.Empty,
            ToCity = route?.ToCity ?? string.Empty,
            DepartureTime = schedule?.DepartureTime ?? DateTime.MinValue,
            ArrivalTime = schedule?.ArrivalTime ?? DateTime.MinValue,
            SeatNumber = seat?.SeatNumber ?? string.Empty,
            Price = ticket.Price,
            Status = ticket.Status.ToString(),
            BookingDate = ticket.BookingDate
        };
    }
}