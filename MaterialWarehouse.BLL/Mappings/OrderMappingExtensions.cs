using MaterialWarehouse.DAL.Entities;
using MaterialWarehouse.BLL.DTOs;

namespace MaterialWarehouse.BLL.Mappings;

public static class OrderMappingExtensions
{
    /// <summary>
    /// Елегантно перетворює сутність Order на об'єкт передачі даних OrderDto
    /// </summary>
    public static OrderDto ToDto(this Order order, int totalItems)
    {
        if (order == null) return null!;

        return new OrderDto(
            Id: order.Id,
            UserId: order.UserId,
            State: order.State.ToString(),
            CreatedAt: order.CreatedAt,
            TotalItems: totalItems
        );
    }
}