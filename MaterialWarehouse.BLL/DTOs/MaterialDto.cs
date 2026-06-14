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
    // Робимо IsInStock автоматичним властивістю-геттером, 
    // щоб сервісу не довелося передавати його вручну!
    public bool IsInStock => Quantity > 0;
}