using Microsoft.EntityFrameworkCore;
using SmartInventory.Core.Interfaces;
using SmartInventory.Services;
using SmartInventory.Data;
using SmartInventory.Core.Entities;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// -----------------------------
// Configure DbContext
// -----------------------------
builder.Services.AddDbContext<InventoryDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// -----------------------------
// Configure Product Service
// -----------------------------
var useFake = builder.Configuration.GetValue<bool>("UseFakeService");

if (useFake)
{
    builder.Services.AddScoped<IProductService, FakeProductService>();
}
else
{
    builder.Services.AddScoped<IProductService, ProductService>();
}

// -----------------------------
// Add Controllers + CORS + JSON options
// -----------------------------
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Prevent object cycles during JSON serialization
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.WriteIndented = true;
    });

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactDev",
        policy =>
        {
            policy.WithOrigins("http://localhost:3000")
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

var app = builder.Build();

app.UseCors("AllowReactDev");
app.MapControllers();

// -----------------------------
// Seed Database
// -----------------------------
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<InventoryDbContext>();

    try
    {
        // Apply migrations and create tables if missing
        await context.Database.MigrateAsync();

        // Seed categories + products
        await SeedData.InitializeAsync(context);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error applying migrations or seeding: {ex.Message}");
        throw;
    }
}

// -----------------------------
// Run the app
// -----------------------------
app.Run();