//entrypoint of the application
var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/healthz", () => Results.Ok(new { status = "ok" }))
    .WithName("Health");

app.Run();
