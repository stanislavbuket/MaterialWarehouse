using MaterialWarehouse.API.Infrastructure;
using MaterialWarehouse.BLL.Interfaces;
using MaterialWarehouse.BLL.Services;
using MaterialWarehouse.DAL;
using MaterialWarehouse.DAL.Interfaces;
using MaterialWarehouse.DAL.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

// Налаштування Entity Framework Core з MS SQL Server
builder.Services.AddDbContext<MaterialWarehouseDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Реєстрація інфраструктурних сервісів
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Реєстрація бізнес-сервісів
builder.Services.AddScoped<IMaterialService, MaterialService>();
builder.Services.AddScoped<IOrderService, OrderService>();

// Реєстрація глобального обробника виключень
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

var app = builder.Build();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

// Ендпоінти для роботи з матеріалами
var materialsGroup = app.MapGroup("/api/materials");

materialsGroup.MapGet("/", async (IMaterialService service) =>
{
    var result = await service.GetInStockMaterialsAsync();
    return result.IsSuccess ? Results.Ok(result.Value) : Results.BadRequest(result.Error);
});

materialsGroup.MapGet("/{id:int}", async (int id, IMaterialService service) =>
{
    var result = await service.GetByIdAsync(id);
    return result.IsSuccess ? Results.Ok(result.Value) : Results.NotFound(result.Error);
});

materialsGroup.MapPost("/{id:int}/adjust", async (int id, int amount, IMaterialService service) =>
{
    var result = await service.AdjustStockAsync(id, amount);
    return result.IsSuccess ? Results.Ok() : Results.BadRequest(result.Error);
});

// Ендпоінти для роботи із замовленнями
var ordersGroup = app.MapGroup("/api/orders");

ordersGroup.MapPost("/", async (CreateOrderRequest request, IOrderService service) =>
{
    var items = request.Items.Select(i => (i.MaterialId, i.Quantity));
    var result = await service.CreateOrderAsync(request.UserId, items);
    return result.IsSuccess ? Results.Created($"/api/orders/{result.Value.Id}", result.Value) : Results.BadRequest(result.Error);
});

ordersGroup.MapPost("/{id:int}/state", async (int id, string nextState, IOrderService service) =>
{
    var result = await service.TransitionOrderStateAsync(id, nextState);
    return result.IsSuccess ? Results.Ok() : Results.BadRequest(result.Error);
});

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<MaterialWarehouse.DAL.MaterialWarehouseDbContext>();
    await MaterialWarehouse.DAL.DbInitializer.SeedAsync(context);
}

app.Run();

// DTO для запитів Minimal API
public record CreateOrderRequest(int UserId, List<OrderItemRequest> Items);
public record OrderItemRequest(int MaterialId, int Quantity);
