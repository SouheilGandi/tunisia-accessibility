using Microsoft.EntityFrameworkCore;

namespace Tunac.Infrastructure.Persistence;

public sealed class TunacDbContext : DbContext
{
    public TunacDbContext(DbContextOptions<TunacDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Required Postgres extensions. EF Core's migration generator turns these
        // into `CREATE EXTENSION IF NOT EXISTS ...` statements.
        modelBuilder.HasPostgresExtension("postgis");     // geometry / geography types
        modelBuilder.HasPostgresExtension("pg_trgm");     // trigram-based fuzzy search (for searching place names with typos and Arabic transliterations)
        modelBuilder.HasPostgresExtension("unaccent");    // accent-insensitive search ("café" ≈ "cafe")
        modelBuilder.HasPostgresExtension("citext");      // case-insensitive text type (for emails)
    }
}