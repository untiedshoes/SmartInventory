using Microsoft.EntityFrameworkCore;
using SmartInventory.Core.Interfaces;
using SmartInventory.Services;
using SmartInventory.Data;
using SmartInventory.Core.Entities;

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
// Add Controllers + CORS
// -----------------------------
builder.Services.AddControllers();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactDev", policy =>
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
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

    // ✅ Apply all migrations (creates tables if missing)
    await context.Database.MigrateAsync();

    // ✅ Seed categories + products
    await SeedData.InitializeAsync(context);
}

// -----------------------------
// Run the app
// -----------------------------
app.Run();