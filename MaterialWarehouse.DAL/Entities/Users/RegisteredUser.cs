namespace MaterialWarehouse.DAL.Entities.Users;

public class RegisteredUser : User
{
    private readonly List<Order> _orders = [];

    public string Organization { get; private set; } = null!;
    public IReadOnlyCollection<Order> Orders => _orders.AsReadOnly();

    protected RegisteredUser() { }

    public RegisteredUser(int id, string username, string email, string passwordHash, string organization)
        : base(id, username, email, passwordHash, UserRole.Registered)
    {
        Organization = organization;
    }

    public void AddOrder(Order order)
    {
        ArgumentNullException.ThrowIfNull(order);
        _orders.Add(order);
    }
}
