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

// Serve the built Angular app from the same origin as the API when present
// (Spa:Root config, or ./wwwroot). Lets the whole app run as one process.
var spaRoot = app.Configuration["Spa:Root"] ?? Path.Combine(app.Environment.ContentRootPath, "wwwroot");
var serveSpa = Directory.Exists(spaRoot) && File.Exists(Path.Combine(spaRoot, "index.html"));
if (serveSpa)
{
    var spaFiles = new Microsoft.Extensions.FileProviders.PhysicalFileProvider(spaRoot);
    app.UseDefaultFiles(new DefaultFilesOptions { FileProvider = spaFiles });
    app.UseStaticFiles(new StaticFileOptions { FileProvider = spaFiles });
}

app.UseConfiguredCors(app.Configuration);
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<SyncHub>("/hubs/sync");
app.MapMcp("/mcp");
app.MapGet("/health", () => Results.Ok());

if (serveSpa)
{
    app.MapFallbackToFile("index.html", new StaticFileOptions
    {
        FileProvider = new Microsoft.Extensions.FileProviders.PhysicalFileProvider(spaRoot),
    });
}

app.Run();

// Exposes the compiler-generated Program class publicly so
// WebApplicationFactory<Program> (Api.IntegrationTests) can reference it.
public partial class Program;
