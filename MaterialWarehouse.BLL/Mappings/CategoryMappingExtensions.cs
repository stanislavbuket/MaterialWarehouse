using MaterialWarehouse.DAL.Entities;
using MaterialWarehouse.BLL.DTOs;

namespace MaterialWarehouse.BLL.Mappings;

public static class CategoryMappingExtensions
{
    /// <summary>
    /// Перетворює сутність Category в CategoryDto
    /// </summary>
    public static CategoryDto ToDto(this Category category)
    {
        if (category == null) return null!;

        return new CategoryDto(
            Id: category.Id,
            Name: category.Name
        );
    }
}