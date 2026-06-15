using MaterialWarehouse.DAL.Entities;
using MaterialWarehouse.BLL.DTOs;

namespace MaterialWarehouse.BLL.Mappings;

public static class MaterialMappingExtensions
{
    /// <summary>
    /// Безпечно перетворює сутність Material у MaterialDto для фронтенду
    /// </summary>
    public static MaterialDto ToDto(this Material material)
    {
        if (material == null) return null!;

        return new MaterialDto(
            Id: material.Id,
            Name: material.Name,
            Description: material.Description,
            Quantity: material.Quantity,
            Unit: material.Unit,
            CategoryId: material.CategoryId,
            // Якщо категорія не завантажена з бази, пишемо "Без категорії", щоб уникнути помилок
            CategoryName: material.Category?.Name ?? "Без категорії",
            MinStockLimit: material.MinStockLimit,
            ReservedQuantity: material.ReservedQuantity
        );
    }
}