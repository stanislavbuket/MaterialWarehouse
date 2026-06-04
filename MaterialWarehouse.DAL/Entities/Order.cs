using MaterialWarehouse.DAL.Entities.Users;

namespace MaterialWarehouse.DAL.Entities;

public enum OrderState { Created, Placed, Approved, Shipped, Delivered, Cancelled }

public class Order
{
    private readonly List<OrderItem> _items = [];

    public int Id { get; set; }
    public int UserId { get; set; }
    public User? User { get; set; }
    public OrderState State { get; set; } = OrderState.Created;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();

    public Order() { }

    public Order(int userId)
    {
        UserId = userId;
    }

    public void AddItem(OrderItem item)
    {
        _items.Add(item);
    }

    public void TransitionTo(OrderState next)
    {
        State = next;
    }
}

public class OrderItem
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public int MaterialId { get; set; }
    public Material? Material { get; set; }
    public int Quantity { get; set; }
    public bool IsPreOrder { get; set; }

    public OrderItem() { }

    public OrderItem(int id, int orderId, int materialId, int quantity, bool isPreOrder)
    {
        Id = id;
        OrderId = orderId;
        MaterialId = materialId;
        Quantity = quantity;
        IsPreOrder = isPreOrder;
    }
}
