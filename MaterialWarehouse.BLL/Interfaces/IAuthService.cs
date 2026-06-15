using MaterialWarehouse.BLL.Common;
using MaterialWarehouse.BLL.DTOs;

namespace MaterialWarehouse.BLL.Interfaces;

public interface IAuthService
{
    Task<Result<UserDto>> RegisterAsync(RegisterUserDto dto);
    Task<Result<UserDto>> LoginAsync(LoginUserDto dto);
}
