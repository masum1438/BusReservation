namespace Domain.Entities;

public class Passenger : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string MobileNumber { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;

    // FIX: Link Passenger to User so ownership checks and booking history work correctly
    public Guid? UserId { get; set; }

    // Navigation properties
    public User? User { get; set; }
    public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
}