using Microsoft.EntityFrameworkCore;
using SmartInventory.Core.Interfaces;
using SmartInventory.Data;
using SmartInventory.Services;
using SmartInventory.Web.Services;


var builder = WebApplication.CreateBuilder(args);

// Configure DbContext to use SQLite
builder.Services.AddDbContext<InventoryDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("InventoryDb")));


// Register business services
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IProductService, FakeProductService>(); // For testing without DB


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
            .WithOrigins("http://localhost:3000", "http://localhost:3001") // React dev servers
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