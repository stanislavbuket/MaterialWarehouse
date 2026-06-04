using MaterialWarehouse.BLL.Common;
using MaterialWarehouse.BLL.DTOs;

namespace MaterialWarehouse.BLL.Interfaces;

public interface IOrderService
{
    Task<Result<OrderDto>> CreateOrderAsync(int userId, IEnumerable<(int materialId, int quantity)> items);
    Task<Result> TransitionOrderStateAsync(int orderId, string nextState);
}
