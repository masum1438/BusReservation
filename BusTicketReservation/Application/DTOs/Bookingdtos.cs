namespace Application.DTOs;

// ── Search ─────────────────────────────────────────────────────────────────────

public class AvailableBusDto
{
    public Guid BusScheduleId { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public string BusName { get; set; } = string.Empty;
    public DateTime StartTime { get; set; }
    public DateTime ArrivalTime { get; set; }
    public int TotalSeats { get; set; }
    public int SeatsLeft { get; set; }
    public decimal Price { get; set; }
}

// ── Seat Plan ─────────────────────────────────────────────────────────────────

public class SeatPlanDto
{
    public Guid BusScheduleId { get; set; }
    public string BusName { get; set; } = string.Empty;
    public string FromCity { get; set; } = string.Empty;
    public string ToCity { get; set; } = string.Empty;
    public DateTime DepartureTime { get; set; }
    public DateTime ArrivalTime { get; set; }
    public List<SeatDto> Seats { get; set; } = new();
}

public class SeatDto
{
    public Guid SeatId { get; set; }
    public string SeatNumber { get; set; } = string.Empty;
    public int RowNumber { get; set; }
    public int ColumnNumber { get; set; }
    public string Status { get; set; } = string.Empty;
    public bool IsAvailable => Status == "Available";
}

// ── Booking ───────────────────────────────────────────────────────────────────

public class BookSeatInputDto
{
    public Guid BusScheduleId { get; set; }
    public Guid SeatId { get; set; }
    public string PassengerName { get; set; } = string.Empty;
    public string MobileNumber { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string BoardingPoint { get; set; } = string.Empty;
    public string DroppingPoint { get; set; } = string.Empty;
}

public class BookSeatResultDto
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; } = string.Empty;
    public Guid TicketId { get; set; }
    public string TicketNumber { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
}

public class MultipleBookSeatInputDto
{
    public Guid BusScheduleId { get; set; }
    public List<Guid> SeatIds { get; set; } = new();
    public string PassengerName { get; set; } = string.Empty;
    public string MobileNumber { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string BoardingPoint { get; set; } = string.Empty;
    public string DroppingPoint { get; set; } = string.Empty;
}

// ── Ticket ────────────────────────────────────────────────────────────────────

public class UserBookingDto
{
    public Guid TicketId { get; set; }
    public string TicketNumber { get; set; } = string.Empty;
    public string BusName { get; set; } = string.Empty;
    public string CompanyName { get; set; } = string.Empty;
    public string FromCity { get; set; } = string.Empty;
    public string ToCity { get; set; } = string.Empty;
    public DateTime DepartureTime { get; set; }
    public DateTime ArrivalTime { get; set; }
    public string SeatNumber { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime BookingDate { get; set; }
    public DateTime? PaymentDate { get; set; }
}

public class TicketDetailDto
{
    public Guid TicketId { get; set; }
    public string TicketNumber { get; set; } = string.Empty;
    public string PassengerName { get; set; } = string.Empty;
    public string MobileNumber { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string BusName { get; set; } = string.Empty;
    public string CompanyName { get; set; } = string.Empty;
    public string FromCity { get; set; } = string.Empty;
    public string ToCity { get; set; } = string.Empty;
    public DateTime DepartureTime { get; set; }
    public DateTime ArrivalTime { get; set; }
    public string SeatNumber { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime BookingDate { get; set; }
    public string BoardingPoint { get; set; } = string.Empty;
    public string DroppingPoint { get; set; } = string.Empty;
}

// ── Seat Lock ─────────────────────────────────────────────────────────────────

public class SeatLockRequestDto
{
    public Guid BusScheduleId { get; set; }
    public List<string> SeatNumbers { get; set; } = new();
    public int LockDurationMinutes { get; set; } = 10;
}

public class SeatLockResponseDto
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; } = string.Empty;
    public List<LockedSeatDto> LockedSeats { get; set; } = new();
    public DateTime ExpiresAt { get; set; }
}

public class LockedSeatDto
{
    public string SeatNumber { get; set; } = string.Empty;
    public Guid LockId { get; set; }
    public DateTime ExpiresAt { get; set; }
}

public class SeatReleaseRequestDto
{
    public List<Guid> LockIds { get; set; } = new();
}

public class SeatAvailabilityCheckDto
{
    public Guid BusScheduleId { get; set; }
    public List<string> SeatNumbers { get; set; } = new();
}