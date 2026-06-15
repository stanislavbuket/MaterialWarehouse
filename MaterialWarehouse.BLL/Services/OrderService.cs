using MaterialWarehouse.BLL.Common;
using MaterialWarehouse.BLL.DTOs;
using MaterialWarehouse.BLL.Errors;
using MaterialWarehouse.BLL.Interfaces;
using MaterialWarehouse.DAL.Entities;
using MaterialWarehouse.DAL.Interfaces;

namespace MaterialWarehouse.BLL.Services;

public class OrderService(IUnitOfWork unitOfWork) : IOrderService
{
    public async Task<Result<OrderDto>> CreateOrderAsync(
    int userId,
    IEnumerable<(int materialId, int quantity)> items)
    {
        var materialRepo = unitOfWork.GetRepository<Material>();
        var orderRepo = unitOfWork.GetRepository<Order>();

        var order = new Order(userId);
        int totalItems = 0;

        foreach (var (materialId, quantity) in items)
        {
            if (quantity <= 0)
            {
                return Result.Failure<OrderDto>(OrderErrors.InvalidStateTransition);
            }

            var material = await materialRepo.GetByIdAsync(materialId);

            if (material == null)
            {
                return Result.Failure<OrderDto>(MaterialErrors.NotFound);
            }

            bool isPreOrder = material.AvailableQuantity < quantity;

            if (!isPreOrder)
            {
                material.Reserve(quantity);
                materialRepo.Update(material);
            }

            var orderItem = new OrderItem(
            materialId,
            quantity,
            isPreOrder);

            order.AddItem(orderItem);
            totalItems += quantity;
        }

        await orderRepo.AddAsync(order);
        await unitOfWork.SaveChangesAsync();

        return Result.Success(
        new OrderDto(
        order.Id,
        order.UserId,
        order.State.ToString(),
        order.CreatedAt,
        totalItems));
    }

    public async Task<Result> TransitionOrderStateAsync(
    int orderId,
    string nextState,
    int managerId = 0)
    {
        var orderRepo = unitOfWork.GetRepository<Order>();
        var orderItemRepo = unitOfWork.GetRepository<OrderItem>();
        var materialRepo = unitOfWork.GetRepository<Material>();
        var transactionRepo = unitOfWork.GetRepository<StockTransaction>();

        var order = await orderRepo.GetByIdAsync(orderId);

        if (order == null)
        {
            return Result.Failure(OrderErrors.NotFound);
        }

        if (!Enum.TryParse<OrderState>(nextState, true, out var next))
        {
            return Result.Failure(OrderErrors.InvalidStateTransition);
        }

        var allowedCurrent = next switch
        {
            OrderState.Placed => new[] { OrderState.Created },
            OrderState.Approved => new[] { OrderState.Placed },
            OrderState.Shipped => new[] { OrderState.Approved },
            OrderState.Delivered => new[] { OrderState.Shipped },
            OrderState.Cancelled => new[] { OrderState.Created, OrderState.Placed, OrderState.Approved },
            _ => Array.Empty<OrderState>()
        };

        if (allowedCurrent.Length > 0 && !allowedCurrent.Contains(order.State))
        {
            return Result.Failure(OrderErrors.InvalidStateTransition);
        }

        var allOrderItems = await orderItemRepo.GetAllAsync();

        var orderItems = allOrderItems
        .Where(item => item.OrderId == orderId)
        .ToList();

        if (next == OrderState.Shipped)
        {
            foreach (var item in orderItems.Where(item => !item.IsPreOrder))
            {
                var material = await materialRepo.GetByIdAsync(item.MaterialId);

                if (material == null)
                {
                    return Result.Failure(MaterialErrors.NotFound);
                }

                material.ShipReserved(item.Quantity);
                materialRepo.Update(material);

                var transaction = new StockTransaction(
                item.MaterialId,
                TransactionType.Sale,
                item.Quantity,
                managerId);

                await transactionRepo.AddAsync(transaction);
            }
        }

        if (next == OrderState.Cancelled)
        {
            foreach (var item in orderItems.Where(item => !item.IsPreOrder))
            {
                var material = await materialRepo.GetByIdAsync(item.MaterialId);

                if (material == null)
                {
                    return Result.Failure(MaterialErrors.NotFound);
                }

                material.ReleaseReserve(item.Quantity);
                materialRepo.Update(material);
            }
        }

        order.TransitionTo(next);
        orderRepo.Update(order);

        await unitOfWork.SaveChangesAsync();

        return Result.Success();
    }
}

