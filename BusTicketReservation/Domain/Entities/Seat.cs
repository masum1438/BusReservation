namespace Domain.Entities;

public class Seat : BaseEntity
{
    public Guid BusScheduleId { get; set; }
    public string SeatNumber { get; set; } = string.Empty;
    public int RowNumber { get; set; }
    public int ColumnNumber { get; set; }
    public SeatStatus Status { get; set; } = SeatStatus.Available;

    // Navigation properties
    public BusSchedule BusSchedule { get; set; } = null!;
    public Ticket? Ticket { get; set; }
    public ICollection<SeatLock> Locks { get; set; } = new List<SeatLock>();
}

public enum SeatStatus
{
    Available = 1,
    Booked = 2,
    Sold = 3
}