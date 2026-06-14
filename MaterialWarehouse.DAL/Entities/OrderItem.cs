namespace MaterialWarehouse.DAL.Entities;

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