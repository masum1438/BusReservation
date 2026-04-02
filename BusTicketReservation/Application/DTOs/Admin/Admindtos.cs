using Domain.ValueObjects;

namespace Application.DTOs.Admin;

// ── Bus ──────────────────────────────────────────────────────────────────────

public class BusCreateDto
{
    public string BusNumber { get; set; } = string.Empty;
    public string CompanyName { get; set; } = string.Empty;
    public string BusName { get; set; } = string.Empty;
    public int TotalSeats { get; set; }
    public BusType Type { get; set; }
}

public class BusResponseDto
{
    public Guid Id { get; set; }
    public string BusNumber { get; set; } = string.Empty;
    public string CompanyName { get; set; } = string.Empty;
    public string BusName { get; set; } = string.Empty;
    public int TotalSeats { get; set; }
    public string Type { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

// ── Route ─────────────────────────────────────────────────────────────────────

public class RouteCreateDto
{
    public string FromCity { get; set; } = string.Empty;
    public string ToCity { get; set; } = string.Empty;
    public decimal BasePrice { get; set; }
    public int Distance { get; set; }
}

public class RouteResponseDto
{
    public Guid Id { get; set; }
    public string FromCity { get; set; } = string.Empty;
    public string ToCity { get; set; } = string.Empty;
    public decimal BasePrice { get; set; }
    public int Distance { get; set; }
}

// ── Schedule ──────────────────────────────────────────────────────────────────

public class ScheduleCreateDto
{
    public Guid BusId { get; set; }
    public Guid RouteId { get; set; }
    public DateTime DepartureTime { get; set; }
    public DateTime ArrivalTime { get; set; }
    public decimal Price { get; set; }
}

public class ScheduleResponseDto
{
    public Guid Id { get; set; }
    public string BusName { get; set; } = string.Empty;
    public string CompanyName { get; set; } = string.Empty;
    public string FromCity { get; set; } = string.Empty;
    public string ToCity { get; set; } = string.Empty;
    public DateTime DepartureTime { get; set; }
    public DateTime ArrivalTime { get; set; }
    public decimal Price { get; set; }
    public int AvailableSeats { get; set; }
}

// ── Booking ───────────────────────────────────────────────────────────────────

public class BookingReportDto
{
    public Guid TicketId { get; set; }
    public string TicketNumber { get; set; } = string.Empty;
    public string PassengerName { get; set; } = string.Empty;
    public string MobileNumber { get; set; } = string.Empty;
    public string BusName { get; set; } = string.Empty;
    public string FromCity { get; set; } = string.Empty;
    public string ToCity { get; set; } = string.Empty;
    public DateTime DepartureTime { get; set; }
    public string SeatNumber { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime BookingDate { get; set; }
    public DateTime? PaymentDate { get; set; }
}

public class BookingFilterDto
{
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public string? BusName { get; set; }
    public string? Status { get; set; }
    public string? TicketNumber { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}

public class BookingStatisticsDto
{
    public int TotalBookings { get; set; }
    public decimal TotalRevenue { get; set; }
    public int ConfirmedBookings { get; set; }
    public int CancelledBookings { get; set; }
    public int PendingBookings { get; set; }
    public decimal AverageTicketPrice { get; set; }
    public Dictionary<string, int> BookingsByBus { get; set; } = new();
    public Dictionary<string, int> BookingsByRoute { get; set; } = new();
}