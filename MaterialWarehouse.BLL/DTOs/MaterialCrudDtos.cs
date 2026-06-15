namespace MaterialWarehouse.BLL.DTOs;

public record CreateMaterialDto(
    string Name,
    string Description,
    int Quantity,
    int MinStockLimit,
    int CategoryId
);

public record UpdateMaterialDto(
    string Name,
    string Description,
    int Quantity,
    int MinStockLimit,
    int CategoryId
);