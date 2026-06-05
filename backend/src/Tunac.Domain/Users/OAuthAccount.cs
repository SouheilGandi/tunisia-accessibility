namespace Tunac.Domain.Users;

public sealed class OAuthAccount
{
    public Guid Id { get; set; } = Guid.CreateVersion7();

    public required Guid UserId { get; set; }

    public required string Provider { get; set; }       // e.g. "google", "apple"

    public required string ProviderUserId { get; set; } // the user's ID at that provider

    public DateTimeOffset CreatedAt { get; set; }

    // Navigation property — see explanation below
    public User User { get; set; } = null!;
}