namespace Tunac.Domain.Users;

public sealed class RefreshToken
{
    public Guid Id { get; set; } = Guid.CreateVersion7();

    public required Guid UserId { get; set; }

    public required string TokenHash { get; set; }

    public string? UserAgent { get; set; }

    public string? IpAddress { get; set; }

    public required DateTimeOffset ExpiresAt { get; set; }

    public DateTimeOffset? RevokedAt { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public User User { get; set; } = null!;
}