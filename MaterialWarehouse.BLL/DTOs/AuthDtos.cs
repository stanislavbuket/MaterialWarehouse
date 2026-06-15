namespace MaterialWarehouse.BLL.DTOs;

public record RegisterUserDto(string Username, string Email, string Password, string Organization);
public record LoginUserDto(string Email, string Password);
