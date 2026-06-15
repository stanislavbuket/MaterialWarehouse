using MaterialWarehouse.BLL.Common;
using MaterialWarehouse.BLL.DTOs;
using MaterialWarehouse.BLL.Errors;
using MaterialWarehouse.BLL.Interfaces;
using MaterialWarehouse.DAL.Entities.Users;
using MaterialWarehouse.DAL.Interfaces;

namespace MaterialWarehouse.BLL.Services;

public class AuthService(IUnitOfWork unitOfWork) : IAuthService
{
    public async Task<Result<UserDto>> RegisterAsync(RegisterUserDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Username) || 
            string.IsNullOrWhiteSpace(dto.Email) || 
            string.IsNullOrWhiteSpace(dto.Password) ||
            string.IsNullOrWhiteSpace(dto.Organization))
        {
            return Result.Failure<UserDto>(AuthErrors.InvalidUserData);
        }

        var repo = unitOfWork.GetRepository<User>();
        var allUsers = await repo.GetAllAsync();
        
        if (allUsers.Any(u => u.Email.Equals(dto.Email, StringComparison.OrdinalIgnoreCase)))
        {
            return Result.Failure<UserDto>(AuthErrors.EmailAlreadyExists);
        }

        var hashedPassword = PasswordHasher.HashPassword(dto.Password);
        
        var newUser = new RegisteredUser(0, dto.Username, dto.Email, hashedPassword, dto.Organization);

        await repo.AddAsync(newUser);
        await unitOfWork.SaveChangesAsync();

        return Result.Success(new UserDto(newUser.Id, newUser.Username, newUser.Email, newUser.Role.ToString()));
    }

    public async Task<Result<UserDto>> LoginAsync(LoginUserDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Password))
        {
            return Result.Failure<UserDto>(AuthErrors.InvalidCredentials);
        }

        var repo = unitOfWork.GetRepository<User>();
        var allUsers = await repo.GetAllAsync();
        
        var user = allUsers.FirstOrDefault(u => u.Email.Equals(dto.Email, StringComparison.OrdinalIgnoreCase));
        if (user == null)
        {
            return Result.Failure<UserDto>(AuthErrors.InvalidCredentials);
        }

        if (!PasswordHasher.VerifyPassword(dto.Password, user.PasswordHash))
        {
            return Result.Failure<UserDto>(AuthErrors.InvalidCredentials);
        }

        return Result.Success(new UserDto(user.Id, user.Username, user.Email, user.Role.ToString()));
    }
}
