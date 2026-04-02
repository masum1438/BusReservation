using Application.DTOs;
using Domain.Entities;
using Domain.Interfaces;
using Domain.Services;

namespace Application.Services;

public interface IBookingService
{
    Task<SeatPlanDto> GetSeatPlanAsync(Guid busScheduleId);
    Task<BookSeatResultDto> BookSeatAsync(BookSeatInputDto input, Guid userId);
    Task<BookSeatResultDto> BookMultipleSeatsAsync(MultipleBookSeatInputDto input, Guid userId);
    Task<TicketDetailDto> GetTicketByGuidAsync(Guid ticketId);
    Task<TicketDetailDto> GetTicketByNumberAsync(string ticketNumber);
}

public class BookingService : IBookingService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ISeatAvailabilityService _seatAvailabilityService;

    public BookingService(IUnitOfWork unitOfWork, ISeatAvailabilityService seatAvailabilityService)
    {
        _unitOfWork = unitOfWork;
        _seatAvailabilityService = seatAvailabilityService;
    }

    // ── Seat Plan ──────────────────────────────────────────────────────────────

    public async Task<SeatPlanDto> GetSeatPlanAsync(Guid busScheduleId)
    {
        var scheduleRepo = _unitOfWork.Repository<BusSchedule>();
        var schedule = await scheduleRepo.GetByIdAsync(busScheduleId);
        if (schedule == null || schedule.IsDeleted)
            throw new KeyNotFoundException("Bus schedule not found");

        var busRepo = _unitOfWork.Repository<Bus>();
        var routeRepo = _unitOfWork.Repository<Route>();
        var seatRepo = _unitOfWork.Repository<Seat>();

        var bus = await busRepo.GetByIdAsync(schedule.BusId);
        var route = await routeRepo.GetByIdAsync(schedule.RouteId);
        var seats = await seatRepo.FindAsync(s => s.BusScheduleId == busScheduleId && !s.IsDeleted);

        var seatPlan = new SeatPlanDto
        {
            BusScheduleId = schedule.Id,
            BusName = bus?.BusName ?? string.Empty,
            FromCity = route?.FromCity ?? string.Empty,
            ToCity = route?.ToCity ?? string.Empty,
            DepartureTime = schedule.DepartureTime,
            ArrivalTime = schedule.ArrivalTime,
            Seats = new List<SeatDto>()
        };

        foreach (var seat in seats.OrderBy(s => s.RowNumber).ThenBy(s => s.ColumnNumber))
        {
            var isAvailable = await _seatAvailabilityService.IsSeatAvailableAsync(seat.Id);
            seatPlan.Seats.Add(new SeatDto
            {
                SeatId = seat.Id,
                SeatNumber = seat.SeatNumber,
                RowNumber = seat.RowNumber,
                ColumnNumber = seat.ColumnNumber,
                Status = isAvailable ? "Available" : seat.Status.ToString()
            });
        }

        return seatPlan;
    }

    // ── Book Single Seat ───────────────────────────────────────────────────────

    // FIX: userId now passed in from controller JWT claim — was Guid.NewGuid() placeholder
    public async Task<BookSeatResultDto> BookSeatAsync(BookSeatInputDto input, Guid userId)
    {
        await _unitOfWork.BeginTransactionAsync();

        try
        {
            var seatRepo = _unitOfWork.Repository<Seat>();
            var scheduleRepo = _unitOfWork.Repository<BusSchedule>();
            var ticketRepo = _unitOfWork.Repository<Ticket>();
            var passengerRepo = _unitOfWork.Repository<Passenger>();

            var isAvailable = await _seatAvailabilityService.IsSeatAvailableAsync(input.SeatId);
            if (!isAvailable)
                return new BookSeatResultDto { IsSuccess = false, Message = "Seat is not available" };

            var seat = await seatRepo.GetByIdAsync(input.SeatId);
            var schedule = await scheduleRepo.GetByIdAsync(input.BusScheduleId);

            if (seat == null || schedule == null || schedule.IsDeleted)
                return new BookSeatResultDto { IsSuccess = false, Message = "Invalid seat or schedule" };

            var passenger = await GetOrCreatePassengerAsync(input, passengerRepo, userId);

            var ticket = new Ticket
            {
                Id = Guid.NewGuid(),
                TicketNumber = GenerateTicketNumber(),
                BusScheduleId = input.BusScheduleId,
                SeatId = input.SeatId,
                PassengerId = passenger.Id,
                // FIX: Store owning user for fast ownership checks
                UserId = userId,
                Price = schedule.Price,
                Status = TicketStatus.Confirmed,
                BookingDate = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow
            };

            await ticketRepo.AddAsync(ticket);
            await _seatAvailabilityService.BookSeatAsync(input.SeatId, passenger);
            await _unitOfWork.CommitAsync();

            return new BookSeatResultDto
            {
                IsSuccess = true,
                Message = "Booking successful",
                TicketId = ticket.Id,
                TicketNumber = ticket.TicketNumber,
                TotalAmount = ticket.Price
            };
        }
        catch
        {
            await _unitOfWork.RollbackAsync();
            throw;
        }
    }

    // ── Book Multiple Seats ────────────────────────────────────────────────────

    public async Task<BookSeatResultDto> BookMultipleSeatsAsync(MultipleBookSeatInputDto input, Guid userId)
    {
        await _unitOfWork.BeginTransactionAsync();

        try
        {
            var scheduleRepo = _unitOfWork.Repository<BusSchedule>();
            var ticketRepo = _unitOfWork.Repository<Ticket>();
            var passengerRepo = _unitOfWork.Repository<Passenger>();

            // Validate all seats are available before booking any
            foreach (var seatId in input.SeatIds)
            {
                if (!await _seatAvailabilityService.IsSeatAvailableAsync(seatId))
                    return new BookSeatResultDto { IsSuccess = false, Message = $"Seat {seatId} is not available" };
            }

            var schedule = await scheduleRepo.GetByIdAsync(input.BusScheduleId);
            if (schedule == null || schedule.IsDeleted)
                return new BookSeatResultDto { IsSuccess = false, Message = "Invalid schedule" };

            var proxyInput = new BookSeatInputDto
            {
                PassengerName = input.PassengerName,
                MobileNumber = input.MobileNumber,
                Email = input.Email,
                BoardingPoint = input.BoardingPoint,
                DroppingPoint = input.DroppingPoint
            };

            var passenger = await GetOrCreatePassengerAsync(proxyInput, passengerRepo, userId);

            decimal totalAmount = 0;
            var ticketNumbers = new List<string>();
            Guid firstTicketId = Guid.Empty;

            foreach (var seatId in input.SeatIds)
            {
                var ticketId = Guid.NewGuid();
                var ticketNumber = GenerateTicketNumber();

                var ticket = new Ticket
                {
                    Id = ticketId,
                    TicketNumber = ticketNumber,
                    BusScheduleId = input.BusScheduleId,
                    SeatId = seatId,
                    PassengerId = passenger.Id,
                    UserId = userId,
                    Price = schedule.Price,
                    Status = TicketStatus.Confirmed,
                    BookingDate = DateTime.UtcNow,
                    CreatedAt = DateTime.UtcNow
                };

                await ticketRepo.AddAsync(ticket);
                await _seatAvailabilityService.BookSeatAsync(seatId, passenger);

                totalAmount += schedule.Price;
                ticketNumbers.Add(ticketNumber);
                if (firstTicketId == Guid.Empty) firstTicketId = ticketId;
            }

            await _unitOfWork.CommitAsync();

            return new BookSeatResultDto
            {
                IsSuccess = true,
                Message = $"Successfully booked {input.SeatIds.Count} seat(s)",
                TicketId = firstTicketId,
                TicketNumber = string.Join(", ", ticketNumbers),
                TotalAmount = totalAmount
            };
        }
        catch
        {
            await _unitOfWork.RollbackAsync();
            throw;
        }
    }

    // ── Get Ticket ─────────────────────────────────────────────────────────────

    public async Task<TicketDetailDto> GetTicketByGuidAsync(Guid ticketId)
    {
        var ticket = await _unitOfWork.Repository<Ticket>().GetByIdAsync(ticketId);
        if (ticket == null || ticket.IsDeleted)
            throw new KeyNotFoundException("Ticket not found");
        return await MapToTicketDetailDtoAsync(ticket);
    }

    public async Task<TicketDetailDto> GetTicketByNumberAsync(string ticketNumber)
    {
        var tickets = await _unitOfWork.Repository<Ticket>()
            .FindAsync(t => t.TicketNumber == ticketNumber && !t.IsDeleted);
        var ticket = tickets.FirstOrDefault()
            ?? throw new KeyNotFoundException("Ticket not found");
        return await MapToTicketDetailDtoAsync(ticket);
    }

    // ── Private Helpers ────────────────────────────────────────────────────────

    // FIX: Now links Passenger to User via UserId
    private static async Task<Passenger> GetOrCreatePassengerAsync(
        BookSeatInputDto input,
        IRepository<Passenger> passengerRepo,
        Guid userId)
    {
        var existing = await passengerRepo.FindAsync(p => p.MobileNumber == input.MobileNumber);
        var passenger = existing.FirstOrDefault();

        if (passenger == null)
        {
            passenger = new Passenger
            {
                Id = Guid.NewGuid(),
                Name = input.PassengerName,
                MobileNumber = input.MobileNumber,
                Email = input.Email,
                UserId = userId,
                CreatedAt = DateTime.UtcNow
            };
            await passengerRepo.AddAsync(passenger);
        }
        else
        {
            passenger.Name = input.PassengerName;
            passenger.Email = input.Email;
            passenger.UserId ??= userId;
            passenger.UpdatedAt = DateTime.UtcNow;
            await passengerRepo.UpdateAsync(passenger);
        }

        return passenger;
    }

    // FIX: Use UtcNow — was DateTime.Now (local time)
    private static string GenerateTicketNumber()
    {
        var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
        var random = Guid.NewGuid().ToString("N")[..8].ToUpper();
        return $"TKT-{timestamp}-{random}";
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