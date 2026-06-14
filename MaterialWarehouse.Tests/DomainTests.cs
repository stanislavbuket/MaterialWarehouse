using MaterialWarehouse.BLL.Errors;
using MaterialWarehouse.BLL.Services;
using MaterialWarehouse.DAL.Entities;
using MaterialWarehouse.DAL.Interfaces;
using NSubstitute;
using Xunit;

namespace MaterialWarehouse.Tests;

public class DomainTests
{
    [Fact]
    public async Task AdjustStockAsync_WithValidAmount_UpdatesQuantityAndSaves()
    {
        var uow = Substitute.For<IUnitOfWork>();
        var repo = Substitute.For<IRepository<Material>>();
        var material = new Material(
    1,
    "Цегла",
    "Будівельна цегла",
    100,
    "шт",
    1,
    10);

        repo.GetByIdAsync(1).Returns(material);
        uow.GetRepository<Material>().Returns(repo);

        var service = new MaterialService(uow);
        var result = await service.AdjustStockAsync(1, -30);

        Assert.True(result.IsSuccess);
        Assert.Equal(70, material.Quantity);
        repo.Received(1).Update(material);
        await uow.Received(1).SaveChangesAsync();
    }

    [Fact]
    public async Task AdjustStockAsync_BelowZero_ReturnsOutOfStockError()
    {
        var uow = Substitute.For<IUnitOfWork>();
        var repo = Substitute.For<IRepository<Material>>();
        var material = new Material(
    1,
    "Цегла",
    "Будівельна цегла",
    50,
    "шт",
    1,
    10);

        repo.GetByIdAsync(1).Returns(material);
        uow.GetRepository<Material>().Returns(repo);

        var service = new MaterialService(uow);
        var result = await service.AdjustStockAsync(1, -60);

        Assert.True(result.IsFailure);
        Assert.Equal(MaterialErrors.OutOfStock, result.Error);
        repo.DidNotReceive().Update(Arg.Any<Material>());
        await uow.DidNotReceive().SaveChangesAsync();
    }

    [Fact]
    public async Task TransitionOrderStateAsync_ValidSequence_UpdatesState()
    {
        var uow = Substitute.For<IUnitOfWork>();
        var repo = Substitute.For<IRepository<Order>>();
        var order = new Order(1) { Id = 101 };

        repo.GetByIdAsync(101).Returns(order);
        uow.GetRepository<Order>().Returns(repo);

        var service = new OrderService(uow);
        var result = await service.TransitionOrderStateAsync(101, "Placed");

        Assert.True(result.IsSuccess);
        Assert.Equal(OrderState.Placed, order.State);
        repo.Received(1).Update(order);
        await uow.Received(1).SaveChangesAsync();
    }

    [Fact]
    public async Task TransitionOrderStateAsync_InvalidSequence_ReturnsInvalidTransitionError()
    {
        var uow = Substitute.For<IUnitOfWork>();
        var repo = Substitute.For<IRepository<Order>>();
        var order = new Order(1) { Id = 101 };

        repo.GetByIdAsync(101).Returns(order);
        uow.GetRepository<Order>().Returns(repo);

        var service = new OrderService(uow);
        var result = await service.TransitionOrderStateAsync(101, "Shipped");

        Assert.True(result.IsFailure);
        Assert.Equal(OrderErrors.InvalidStateTransition, result.Error);
        repo.DidNotReceive().Update(Arg.Any<Order>());
        await uow.DidNotReceive().SaveChangesAsync();
    }
}
