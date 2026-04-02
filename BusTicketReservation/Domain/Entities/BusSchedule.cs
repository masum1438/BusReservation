namespace Domain.Entities;

public class BusSchedule : BaseEntity
{
    public Guid BusId { get; set; }
    public Guid RouteId { get; set; }
    public DateTime DepartureTime { get; set; }
    public DateTime ArrivalTime { get; set; }
    public decimal Price { get; set; }

    // Navigation properties
    public Bus Bus { get; set; } = null!;
    public Route Route { get; set; } = null!;
    public ICollection<Seat> Seats { get; set; } = new List<Seat>();
    public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
}