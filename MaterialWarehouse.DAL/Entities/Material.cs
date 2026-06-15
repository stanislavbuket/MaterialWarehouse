namespace MaterialWarehouse.DAL.Entities;

public class Material
{
    public int Id { get; private set; }
    public string Name { get; private set; } = null!;
    public string Description { get; private set; } = null!;
    public int Quantity { get; private set; }
    public int MinStockLimit { get; private set; }
    public int ReservedQuantity { get; private set; }
    public string Unit { get; private set; } = null!;
    public int CategoryId { get; private set; }
    public Category? Category { get; set; }

    public bool IsInStock => Quantity > 0;
    public int AvailableQuantity => Quantity - ReservedQuantity;

    protected Material() { }

    public Material(
        int id,
        string name,
        string description,
        int quantity,
        string unit,
        int categoryId,
        int minStockLimit)
    {
        Id = id;
        Name = name;
        Description = description;
        Quantity = quantity;
        Unit = unit;
        CategoryId = categoryId;
        MinStockLimit = minStockLimit;
        ReservedQuantity = 0;
    }

    public void AdjustQuantity(int amount)
    {
        if (Quantity + amount < 0)
        {
            throw new InvalidOperationException("Кількість матеріалу на складі не може бути меншою за нуль.");
        }
        Quantity += amount;
    }

    public void Reserve(int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("Кількість для резервування повинна бути більшою за нуль.");

        if (Quantity - ReservedQuantity < quantity)
        {
            throw new InvalidOperationException("Недостатньо вільного товару на складі для резервування.");
        }
        ReservedQuantity += quantity;
    }

    public void ReleaseReserve(int quantity)
    {
        if (quantity <= 0)
        {
            throw new ArgumentException("Кількість для зняття з резерву повинна бути більшою за нуль.");
        }

        if (ReservedQuantity - quantity < 0)
        {
            throw new InvalidOperationException("Кількість зарезервованого товару не може стати меншою за нуль.");
        }

        ReservedQuantity -= quantity;
    }

    public void ShipReserved(int quantity)
    {
        if (quantity <= 0)
        {
            throw new ArgumentException("Кількість для відвантаження повинна бути більшою за нуль.");
        }

        if (ReservedQuantity < quantity)
        {
            throw new InvalidOperationException("Недостатньо зарезервованого товару для відвантаження.");
        }

        if (Quantity < quantity)
        {
            throw new InvalidOperationException("Недостатньо фізичного залишку товару на складі.");
        }

        ReservedQuantity -= quantity;
        Quantity -= quantity;
    }

    public void UpdateDetails(string name, string description, int minStockLimit, int categoryId)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Назва матеріалу не може бути порожньою.");

        Name = name;
        Description = description;
        MinStockLimit = minStockLimit;
        CategoryId = categoryId;
    }
}
