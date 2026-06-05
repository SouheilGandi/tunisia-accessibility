//entrypoint of the application
using Microsoft.EntityFrameworkCore;
using Tunac.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

//Service registration (DI container setup)
var postgresConnectionString = builder.Configuration.GetConnectionString("Postgres") ?? throw new InvalidOperationException("Postgres connection string is not configured.");

builder.Services.AddDbContext<TunacDbContext>(options => options.UseNpgsql(postgresConnectionString, npgsqlOptions => npgsqlOptions.UseNetTopologySuite()));

//Build and configure the HTTP pipeline
var app = builder.Build();

app.MapGet("/healthz", () => Results.Ok(new { status = "ok" }))
    .WithName("Health");

app.Run();

//public partial class Program;