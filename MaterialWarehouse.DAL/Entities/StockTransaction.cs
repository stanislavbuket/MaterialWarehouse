namespace MaterialWarehouse.DAL.Entities;

public enum TransactionType
{
    Receive,
    WriteOff,
    Sale
}

public class StockTransaction
{
    public int Id { get; private set; }

    public int MaterialId { get; private set; }

    public Material? Material { get; private set; }

    public TransactionType Type { get; private set; }

    public int Quantity { get; private set; }

    public DateTime Date { get; private set; }

    public int ManagerId { get; private set; }

    protected StockTransaction() { }

    public StockTransaction(
        int materialId,
        TransactionType type,
        int quantity,
        int managerId)
    {
        MaterialId = materialId;
        Type = type;
        Quantity = quantity;
        ManagerId = managerId;
        Date = DateTime.UtcNow;
    }
}