namespace MaterialWarehouse.DAL.Entities;

public class Material
{
    public int Id { get; private set; }
    public string Name { get; private set; } = null!;
    public string Description { get; private set; } = null!;
    public int Quantity { get; private set; }
    public string Unit { get; private set; } = null!;
    public int CategoryId { get; private set; }
    public Category? Category { get; set; }

    public bool IsInStock => Quantity > 0;

    protected Material() { }

    public Material(int id, string name, string description, int quantity, string unit, int categoryId)
    {
        Id = id;
        Name = name;
        Description = description;
        Quantity = quantity;
        Unit = unit;
        CategoryId = categoryId;
    }

    public void AdjustQuantity(int amount)
    {
        Quantity += amount;
    }
}

