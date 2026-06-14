namespace MaterialWarehouse.BLL.DTOs;

public record MaterialDto(
    int Id,
    string Name,
    string Description,
    int Quantity,
    string Unit,
    int CategoryId,
    string CategoryName,
    int MinStockLimit,
    int ReservedQuantity
)
{
    public bool IsInStock => Quantity > 0;
}