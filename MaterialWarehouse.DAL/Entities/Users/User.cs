namespace MaterialWarehouse.DAL.Entities.Users;

public abstract class User
{
    public int Id { get; private set; }
    public string Username { get; private set; } = null!;
    public string Email { get; private set; } = null!;
    public string PasswordHash { get; private set; } = null!;
    public UserRole Role { get; private set; }

    protected User() { }

    protected User(int id, string username, string email, string passwordHash, UserRole role)
    {
        Id = id;
        Username = username;
        Email = email;
        PasswordHash = passwordHash;
        Role = role;
    }
}
