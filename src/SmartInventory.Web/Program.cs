using Microsoft.EntityFrameworkCore;
using SmartInventory.Core.Interfaces;
using SmartInventory.Data;
using SmartInventory.Services;



var builder = WebApplication.CreateBuilder(args);

// Configure DbContext to use SQLite
builder.Services.AddDbContext<InventoryDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("InventoryDb")));

// Register business services - use the real ProductService by default.
builder.Services.AddScoped<IProductService, ProductService>();

// Optionally override with the fake implementation for local testing.
// Set `UseFakeService` to true in appsettings.Development.json to enable.
var useFake = builder.Configuration.GetValue<bool>("UseFakeService");
if (useFake)
{
    builder.Services.AddScoped<IProductService, FakeProductService>(); // For testing without DB
}

// Logging
builder.Services.AddLogging(logging =>
{
    logging.AddConsole();
});

builder.Services.AddControllers();

// Enable CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactDev",
        policy => policy
            .SetIsOriginAllowed(origin => origin != null && (origin.StartsWith("http://localhost") || origin.StartsWith("https://localhost")))
            .AllowAnyMethod()
            .AllowAnyHeader()
    );
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Use CORS before routing
app.UseCors("AllowReactDev");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();
app.Run();