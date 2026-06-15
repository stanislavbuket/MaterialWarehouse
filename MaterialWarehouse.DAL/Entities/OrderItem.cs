namespace MaterialWarehouse.DAL.Entities;

public class OrderItem
{
    public int Id { get; set; }

    public int OrderId { get; set; }

    public Order? Order { get; set; }

    public int MaterialId { get; set; }

    public Material? Material { get; set; }

    public int Quantity { get; set; }

    public bool IsPreOrder { get; set; }

    public OrderItem()
    {
    }

    public OrderItem(int materialId, int quantity, bool isPreOrder)
    {
        if (quantity <= 0)
        {
            throw new ArgumentException("Кількість матеріалу в замовленні повинна бути більшою за нуль.");
        }

        MaterialId = materialId;
        Quantity = quantity;
        IsPreOrder = isPreOrder;
    }

    public OrderItem(int id, int orderId, int materialId, int quantity, bool isPreOrder)
        : this(materialId, quantity, isPreOrder)
    {
        Id = id;
        OrderId = orderId;
    }
}
    