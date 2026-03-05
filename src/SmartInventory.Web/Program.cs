using FluentValidation;
using Microsoft.EntityFrameworkCore;
using SmartInventory.Core.Interfaces;
using SmartInventory.Services;
using SmartInventory.Data;
using SmartInventory.Services.Mapping;
using SmartInventory.Services.Validators;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// -----------------------------
// Configure DbContext
// -----------------------------
builder.Services.AddDbContext<InventoryDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// -----------------------------
// AutoMapper & FluentValidation
// -----------------------------
builder.Services.AddAutoMapper(typeof(MappingProfile));
builder.Services.AddValidatorsFromAssemblyContaining<CreateProductValidator>();

// -----------------------------
// Configure Product Service (real or fake)
// -----------------------------
var useFake = builder.Configuration.GetValue<bool>("UseFakeService");

if (useFake)
{
    // DTO-aware fake service
    builder.Services.AddScoped<FakeProductService>();
    builder.Services.AddScoped<IProductService>(sp => sp.GetRequiredService<FakeProductService>());
}
else
{
    builder.Services.AddScoped<IProductService, ProductService>();
}

// -----------------------------
// Controllers + CORS + JSON options
// -----------------------------
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
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

// -----------------------------
// Apply migrations & seed database
// -----------------------------
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<InventoryDbContext>();
    try
    {
        await context.Database.MigrateAsync();
        await SeedData.InitializeAsync(context);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error applying migrations or seeding: {ex.Message}");
        throw;
    }
}

// -----------------------------
// Use CORS & Map Controllers
// -----------------------------
app.UseCors("AllowReactDev");
app.MapControllers();

// -----------------------------
// Run the app
// -----------------------------
app.Run();