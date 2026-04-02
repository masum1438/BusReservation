namespace Domain.Entities;

public class SeatLock : BaseEntity
{
    public Guid SeatId { get; set; }
    public Guid UserId { get; set; }
    public DateTime LockedAt { get; set; }
    public DateTime ExpiresAt { get; set; }
    public bool IsActive { get; set; }

    // Navigation property
    public Seat Seat { get; set; } = null!;
}