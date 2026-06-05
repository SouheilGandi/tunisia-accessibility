using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tunac.Domain.Users;

namespace Tunac.Infrastructure.Persistence.Configurations;

public sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        //safety check to prevent null reference exceptions during configuration
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable("users");
        builder.HasKey(u => u.Id);

        builder.Property(u => u.Id)
            .HasColumnName("id");

        builder.Property(u => u.Email)
            .HasColumnName("email")
            .HasColumnType("citext")
            .HasMaxLength(255)
            .IsRequired();

        builder.HasIndex(u => u.Email).IsUnique();

        builder.Property(u => u.EmailVerifiedAt)
            .HasColumnName("email_verified_at");

        builder.Property(u => u.PasswordHash)
            .HasColumnName("password_hash")
            .HasMaxLength(255);

        builder.Property(u => u.DisplayHandle)
            .HasColumnName("display_handle")
            .HasMaxLength(50)
            .IsRequired();

        builder.HasIndex(u => u.DisplayHandle).IsUnique();

        builder.Property(u => u.AvatarKey)
            .HasColumnName("avatar_key")
            .HasMaxLength(255);

        builder.Property(u => u.Locale)
            .HasColumnName("locale")
            .HasMaxLength(5)
            .HasDefaultValue("fr")
            .IsRequired();

        builder.Property(u => u.MobilityAid)
            .HasColumnName("mobility_aid")
            .HasMaxLength(50);

        builder.Property(u => u.Role)
            .HasColumnName("role")
            .HasConversion<string>()
            .HasMaxLength(20)
            .HasDefaultValue(UserRole.User)
            .IsRequired();

        builder.Property(u => u.CreatedAt)
            .HasColumnName("created_at")
            .HasDefaultValueSql("now()");

        builder.Property(u => u.UpdatedAt)
            .HasColumnName("updated_at")
            .HasDefaultValueSql("now()");

        builder.Property(u => u.DeletedAt)
            .HasColumnName("deleted_at");

        // Soft delete: hide rows where DeletedAt is set, unless explicitly bypassed.
        builder.HasQueryFilter(u => u.DeletedAt == null);
    }
}