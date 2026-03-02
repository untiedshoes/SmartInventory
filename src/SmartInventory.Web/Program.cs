using Microsoft.EntityFrameworkCore;
using SmartInventory.Core.Interfaces;
using SmartInventory.Data;
using SmartInventory.Services;


var builder = WebApplication.CreateBuilder(args);

// Configure DbContext to use SQLite
builder.Services.AddDbContext<InventoryDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("InventoryDb")));


// Register business services
builder.Services.AddScoped<IProductService, ProductService>();

// Logging
builder.Services.AddLogging(logging =>
{
    logging.AddConsole();
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();
app.Run();