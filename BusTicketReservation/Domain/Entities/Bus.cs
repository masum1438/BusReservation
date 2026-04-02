using Domain.ValueObjects;

namespace Domain.Entities;

public class Bus : BaseEntity
{
    public string BusNumber { get; set; } = string.Empty;
    public string CompanyName { get; set; } = string.Empty;
    public string BusName { get; set; } = string.Empty;
    public int TotalSeats { get; set; }
    public BusType Type { get; set; }

    // Navigation properties
    public ICollection<BusSchedule> Schedules { get; set; } = new List<BusSchedule>();
}