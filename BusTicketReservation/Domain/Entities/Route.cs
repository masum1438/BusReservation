namespace Domain.Entities;

public class Route : BaseEntity
{
    public string FromCity { get; set; } = string.Empty;
    public string ToCity { get; set; } = string.Empty;
    public decimal BasePrice { get; set; }
    public int Distance { get; set; }

    // Navigation properties
    public ICollection<BusSchedule> Schedules { get; set; } = new List<BusSchedule>();
}