using MaterialWarehouse.API.Infrastructure;
using MaterialWarehouse.BLL.Interfaces;
using MaterialWarehouse.BLL.Services;
using MaterialWarehouse.BLL.DTOs;
using MaterialWarehouse.DAL;
using MaterialWarehouse.DAL.Interfaces;
using MaterialWarehouse.DAL.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Scalar.AspNetCore;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
});

// Налаштування Entity Framework Core з InMemory Database
builder.Services.AddDbContext<MaterialWarehouseDbContext>(options =>
       options.UseInMemoryDatabase("MaterialWarehouseDb"));

// Реєстрація інфраструктурних сервісів
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Реєстрація бізнес-сервісів
builder.Services.AddScoped<IMaterialService, MaterialService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IReportService, ReportService>();

// Налаштування JWT-автентифікації
var jwtKey = "SuperSecretKey123456789012345678901234567890";
var jwtIssuer = "MaterialWarehouseAPI";
var jwtAudience = "MaterialWarehouseClients";

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtIssuer,
        ValidAudience = jwtAudience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
    };
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOrManager", policy =>
        policy.RequireRole("Admin", "Manager"));
});

// Реєстрація глобального обробника виключень
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

var app = builder.Build();

// Ініціалізація бази даних
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<MaterialWarehouseDbContext>();
        await DbInitializer.SeedAsync(context);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Помилка під час ініціалізації бази даних.");
    }
}

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

// Ендпоінти для авторизації
var authGroup = app.MapGroup("/api/auth");

authGroup.MapPost("/register", async (RegisterUserDto dto, IAuthService service) =>
{
    var result = await service.RegisterAsync(dto);
    return result.IsSuccess ? Results.Ok(result.Value) : Results.BadRequest(result.Error);
});

authGroup.MapPost("/login", async (LoginUserDto dto, IAuthService service) =>
{
    var result = await service.LoginAsync(dto);
    if (!result.IsSuccess)
    {
        return Results.BadRequest(result.Error);
    }

    var user = result.Value;

    var tokenHandler = new JwtSecurityTokenHandler();
    var key = Encoding.UTF8.GetBytes(jwtKey);
    var tokenDescriptor = new SecurityTokenDescriptor
    {
        Subject = new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role)
        }),
        Expires = DateTime.UtcNow.AddHours(2),
        Issuer = jwtIssuer,
        Audience = jwtAudience,
        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
    };

    var token = tokenHandler.CreateToken(tokenDescriptor);
    var tokenString = tokenHandler.WriteToken(token);

    return Results.Ok(new { Token = tokenString, User = user });
});

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

materialsGroup.MapPost("/{id:int}/adjust", async (int id, int amount, ClaimsPrincipal user, IMaterialService service) =>
{
    var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    int managerId = int.TryParse(userIdClaim, out var parsedId) ? parsedId : 0;
    var result = await service.AdjustStockAsync(id, amount, managerId);
    return result.IsSuccess ? Results.Ok() : Results.BadRequest(result.Error);
}).RequireAuthorization("AdminOrManager");

// Ендпоінти для роботи із замовленнями
var ordersGroup = app.MapGroup("/api/orders");

ordersGroup.MapPost("/", async (CreateOrderRequest request, IOrderService service) =>
{
    var items = request.Items.Select(i => (i.MaterialId, i.Quantity));
    var result = await service.CreateOrderAsync(request.UserId, items);
    return result.IsSuccess ? Results.Created($"/api/orders/{result.Value.Id}", result.Value) : Results.BadRequest(result.Error);
}).RequireAuthorization();

ordersGroup.MapPost("/{id:int}/state", async (int id, string nextState, ClaimsPrincipal user, IOrderService service) =>
{
    var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    int managerId = int.TryParse(userIdClaim, out var parsedId) ? parsedId : 0;
    var result = await service.TransitionOrderStateAsync(id, nextState, managerId);
    return result.IsSuccess ? Results.Ok() : Results.BadRequest(result.Error);
}).RequireAuthorization("AdminOrManager");

// Ендпоінти для звітів
var reportsGroup = app.MapGroup("/api/reports");

reportsGroup.MapGet("/low-stock", async (IReportService service) =>
{
    var result = await service.GetLowStockReportAsync();
    return result.IsSuccess ? Results.Ok(result.Value) : Results.BadRequest(result.Error);
}).RequireAuthorization("AdminOrManager");

reportsGroup.MapGet("/deficit", async (IReportService service) =>
{
    var result = await service.GetDeficitReportAsync();
    return result.IsSuccess ? Results.Ok(result.Value) : Results.BadRequest(result.Error);
}).RequireAuthorization("AdminOrManager");

app.Run();

// DTO для запитів Minimal API
public record CreateOrderRequest(int UserId, List<OrderItemRequest> Items);
public record OrderItemRequest(int MaterialId, int Quantity);

public partial class Program { }