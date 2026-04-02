namespace Domain.Entities;

public class Ticket : BaseEntity
{
    public string TicketNumber { get; set; } = string.Empty;
    public Guid BusScheduleId { get; set; }
    public Guid SeatId { get; set; }
    public Guid PassengerId { get; set; }

    // FIX: Track owning user directly for fast, reliable ownership checks
    public Guid UserId { get; set; }

    public decimal Price { get; set; }
    public TicketStatus Status { get; set; } = TicketStatus.Pending;
    public DateTime BookingDate { get; set; }
    public DateTime? PaymentDate { get; set; }

    // Navigation properties
    public BusSchedule BusSchedule { get; set; } = null!;
    public Seat Seat { get; set; } = null!;
    public Passenger Passenger { get; set; } = null!;
    public User User { get; set; } = null!;
}

public enum TicketStatus
{
    Pending = 1,
    Confirmed = 2,
    Cancelled = 3,
    Expired = 4
}