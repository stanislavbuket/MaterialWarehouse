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
        if (State != OrderState.Created)
        {
            throw new InvalidOperationException("Неможливо додати позицію до замовлення, яке вже обробляється або закрите.");
        }
        _items.Add(item);
    }

    public void TransitionTo(OrderState next)
    {
        bool isValid = State switch
        {
            OrderState.Created => next == OrderState.Placed || next == OrderState.Cancelled,
            OrderState.Placed => next == OrderState.Approved || next == OrderState.Cancelled,
            OrderState.Approved => next == OrderState.Shipped || next == OrderState.Cancelled,
            OrderState.Shipped => next == OrderState.Delivered,
            OrderState.Delivered => false, // Кінцевий статус
            OrderState.Cancelled => false, // Кінцевий статус
            _ => false
        };

        if (!isValid)
        {
            throw new InvalidOperationException($"Некоректний перехід зі статусу {State} до {next}.");
        }

        State = next;
    }
}