namespace Domain.Entities;

// FIX: Extends BaseEntity for consistent Id, CreatedAt, UpdatedAt, IsDeleted handling
public class RefreshToken : BaseEntity
{
    public string Token { get; set; } = string.Empty;
    public Guid UserId { get; set; }
    public DateTime ExpiresAt { get; set; }
    public DateTime? RevokedAt { get; set; }
    public string? RevokedByIp { get; set; }
    public string? CreatedByIp { get; set; }

    // Navigation property
    public User User { get; set; } = null!;

    // Computed helper
    public bool IsActive => RevokedAt == null && ExpiresAt > DateTime.UtcNow;
}