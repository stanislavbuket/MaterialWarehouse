namespace MaterialWarehouse.DAL.Entities.Users;

public class Manager : User
{
    public string Department { get; private set; } = null!;

    protected Manager() { }

    public Manager(int id, string username, string email, string passwordHash, string department)
        : base(id, username, email, passwordHash, UserRole.Manager)
    {
        Department = department;
    }
}
