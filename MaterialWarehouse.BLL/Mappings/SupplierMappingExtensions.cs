using MaterialWarehouse.DAL.Entities;
using MaterialWarehouse.BLL.DTOs;

namespace MaterialWarehouse.BLL.Mappings;

public static class SupplierMappingExtensions
{
    /// <summary>
    /// Перетворює сутність Supplier в SupplierDto
    /// </summary>
    public static SupplierDto ToDto(this Supplier supplier)
    {
        if (supplier == null) return null!;

        return new SupplierDto(
            Id: supplier.Id,
            Name: supplier.Name,
            ContactPhone: "",
            Email: "" 
        );
    }
}
