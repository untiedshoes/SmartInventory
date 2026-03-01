using Microsoft.EntityFrameworkCore;
using SmartInventory.Core.Interfaces;
using SmartInventory.Data;
using SmartInventory.Services;

var builder = WebApplication.CreateBuilder(args);

// Register EF Core
builder.Services.AddDbContext<InventoryDbContext>(options => 
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register business services
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

app.Run();