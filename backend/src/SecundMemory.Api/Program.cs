using SecundMemory.Api.Extensions;
using SecundMemory.Api.Hubs;
using SecundMemory.Api.Middleware;
using SecundMemory.Application;
using SecundMemory.Infrastructure;
using SecundMemory.Infrastructure.Persistence;
using SecundMemory.Infrastructure.Persistence.Seed;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddApiDocumentation();
builder.Services.AddConfiguredCors(builder.Configuration);
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddSignalR();

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddMcpServer()
    .WithHttpTransport()
    .WithToolsFromAssembly();

var app = builder.Build();

app.UseApiDocumentation();

if (app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await DbInitializer.MigrateAsync(db, useSqlite: builder.Configuration.GetValue<bool>("Testing:UseSqlite"));
    await DbInitializer.SeedAsync(db);
}

app.UseMiddleware<RequestLoggingMiddleware>();
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseConfiguredCors(app.Configuration);
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<SyncHub>("/hubs/sync");
app.MapMcp("/mcp");
app.MapGet("/health", () => Results.Ok());

app.Run();

// Exposes the compiler-generated Program class publicly so
// WebApplicationFactory<Program> (Api.IntegrationTests) can reference it.
public partial class Program;
