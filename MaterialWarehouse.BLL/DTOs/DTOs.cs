namespace MaterialWarehouse.BLL.DTOs;

public record MaterialDto(int Id, string Name, string Description, int Quantity, string Unit, int CategoryId, string CategoryName);

public record OrderDto(int Id, int UserId, string State, DateTime CreatedAt, int TotalItems);

public record UserDto(int Id, string Username, string Email, string Role);
