namespace MaterialWarehouse.BLL.DTOs;

public record MaterialDeficitDto(
    int MaterialId,
    string MaterialName,
    int AvailableQuantity, // Скільки є фізично
    int ReservedQuantity,  // Скільки заброньовано покупцями
    int DeficitQuantity    // Чого не вистачає (Резерв - Наявність)
);
