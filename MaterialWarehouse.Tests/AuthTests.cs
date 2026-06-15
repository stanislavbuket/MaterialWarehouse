using MaterialWarehouse.BLL.Common;
using MaterialWarehouse.BLL.DTOs;
using MaterialWarehouse.BLL.Errors;
using MaterialWarehouse.BLL.Services;
using MaterialWarehouse.DAL.Entities.Users;
using MaterialWarehouse.DAL.Interfaces;
using NSubstitute;
using Xunit;

namespace MaterialWarehouse.Tests;

public class AuthTests
{
    [Fact]
    public async Task RegisterAsync_WithValidData_CreatesUserAndReturnsSuccess()
    {
        // Arrange
        var uow = Substitute.For<IUnitOfWork>();
        var repo = Substitute.For<IRepository<User>>();
        
        repo.GetAllAsync().Returns(new List<User>());
        uow.GetRepository<User>().Returns(repo);

        var service = new AuthService(uow);
        var dto = new RegisterUserDto("test_user", "test@test.com", "securepassword", "Test Org");

        // Act
        var result = await service.RegisterAsync(dto);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("test_user", result.Value.Username);
        Assert.Equal("test@test.com", result.Value.Email);
        Assert.Equal(UserRole.Registered.ToString(), result.Value.Role);
        
        await repo.Received(1).AddAsync(Arg.Any<RegisteredUser>());
        await uow.Received(1).SaveChangesAsync();
    }

    [Fact]
    public async Task RegisterAsync_WithDuplicateEmail_ReturnsEmailAlreadyExistsError()
    {
        // Arrange
        var uow = Substitute.For<IUnitOfWork>();
        var repo = Substitute.For<IRepository<User>>();
        var existingUser = new RegisteredUser(1, "existing", "test@test.com", "hash", "Org");

        repo.GetAllAsync().Returns(new List<User> { existingUser });
        uow.GetRepository<User>().Returns(repo);

        var service = new AuthService(uow);
        var dto = new RegisterUserDto("new_user", "test@test.com", "password", "New Org");

        // Act
        var result = await service.RegisterAsync(dto);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(AuthErrors.EmailAlreadyExists, result.Error);
        await repo.DidNotReceive().AddAsync(Arg.Any<User>());
        await uow.DidNotReceive().SaveChangesAsync();
    }

    [Fact]
    public async Task LoginAsync_WithValidCredentials_ReturnsUserDto()
    {
        // Arrange
        var uow = Substitute.For<IUnitOfWork>();
        var repo = Substitute.For<IRepository<User>>();
        var password = "mysecretpassword";
        var hashedPassword = PasswordHasher.HashPassword(password);
        var user = new RegisteredUser(5, "bob", "bob@example.com", hashedPassword, "Bob Org");

        repo.GetAllAsync().Returns(new List<User> { user });
        uow.GetRepository<User>().Returns(repo);

        var service = new AuthService(uow);
        var dto = new LoginUserDto("bob@example.com", password);

        // Act
        var result = await service.LoginAsync(dto);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("bob", result.Value.Username);
        Assert.Equal("bob@example.com", result.Value.Email);
    }

    [Fact]
    public async Task LoginAsync_WithInvalidCredentials_ReturnsInvalidCredentialsError()
    {
        // Arrange
        var uow = Substitute.For<IUnitOfWork>();
        var repo = Substitute.For<IRepository<User>>();
        var user = new RegisteredUser(5, "bob", "bob@example.com", "hash", "Bob Org");

        repo.GetAllAsync().Returns(new List<User> { user });
        uow.GetRepository<User>().Returns(repo);

        var service = new AuthService(uow);
        var dto = new LoginUserDto("bob@example.com", "wrongpassword");

        // Act
        var result = await service.LoginAsync(dto);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(AuthErrors.InvalidCredentials, result.Error);
    }
}
