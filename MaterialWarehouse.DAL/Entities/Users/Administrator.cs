namespace MaterialWarehouse.DAL.Entities.Users;

public class Administrator : User
{
    public Administrator(int id, string username, string email, string passwordHash)
        : base(id, username, email, passwordHash, UserRole.Admin)
    {
    }
}
