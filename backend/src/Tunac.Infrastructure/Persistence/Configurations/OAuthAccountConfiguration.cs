using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tunac.Domain.Users;

namespace Tunac.Infrastructure.Persistence.Configurations;

public sealed class OAuthAccountConfiguration : IEntityTypeConfiguration<OAuthAccount>
{
    public void Configure(EntityTypeBuilder<OAuthAccount> builder)
    {
        //safety check to prevent null reference exceptions during configuration
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable("oauth_accounts");
        builder.HasKey(o => o.Id);

        builder.Property(o => o.Id)
            .HasColumnName("id");

        builder.Property(o => o.UserId)
            .HasColumnName("user_id")
            .IsRequired();

        builder.Property(o => o.Provider)
            .HasColumnName("provider")
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(o => o.ProviderUserId)
            .HasColumnName("provider_user_id")
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(o => o.CreatedAt)
            .HasColumnName("created_at")
            .HasDefaultValueSql("now()");

        // Composite unique index: same (provider, providerUserId) pair can only exist once.
        // Example: one Google account ID can only ever be linked to one of our users.
        builder.HasIndex(o => new { o.Provider, o.ProviderUserId }).IsUnique();

        // Foreign key relationship: OAuthAccount.User → User.Id
        builder.HasOne(o => o.User)
            .WithMany()
            .HasForeignKey(o => o.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}