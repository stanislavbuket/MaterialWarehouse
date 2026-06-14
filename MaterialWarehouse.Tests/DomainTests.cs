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

    // ==========================================
    // НОВІ ДОДАТКОВІ ТЕСТИ (НАРОЩУЄМО ОБ'ЄМ)
    // ==========================================

    [Fact]
    public async Task AdjustStockAsync_MaterialNotFound_ReturnsNotFoundError()
    {
        // Arrange
        var uow = Substitute.For<IUnitOfWork>();
        var repo = Substitute.For<IRepository<Material>>();

        // Повертаємо null, ніби матеріалу з Id = 999 не існує в базі
        repo.GetByIdAsync(999).Returns((Material)null);
        uow.GetRepository<Material>().Returns(repo);

        var service = new MaterialService(uow);

        // Act
        var result = await service.AdjustStockAsync(999, 10);

        // Assert
        Assert.True(result.IsFailure);
        repo.DidNotReceive().Update(Arg.Any<Material>());
        await uow.DidNotReceive().SaveChangesAsync();
    }

    [Fact]
    public async Task AdjustStockAsync_WithPositiveAmount_SuccessfullyAddsStock()
    {
        // Arrange
        var uow = Substitute.For<IUnitOfWork>();
        var repo = Substitute.For<IRepository<Material>>();
        var material = new Material(2, "Кабель", "Електричний кабель", 50, "м", 1, 5);

        repo.GetByIdAsync(2).Returns(material);
        uow.GetRepository<Material>().Returns(repo);

        var service = new MaterialService(uow);

        // Act - додаємо 50 одиниць до залишку
        var result = await service.AdjustStockAsync(2, 50);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(100, material.Quantity); // Було 50 + стало 50 = 100
        repo.Received(1).Update(material);
        await uow.Received(1).SaveChangesAsync();
    }

    [Fact]
    public async Task TransitionOrderStateAsync_OrderNotFound_ReturnsNotFoundError()
    {
        // Arrange
        var uow = Substitute.For<IUnitOfWork>();
        var repo = Substitute.For<IRepository<Order>>();

        repo.GetByIdAsync(999).Returns((Order)null);
        uow.GetRepository<Order>().Returns(repo);

        var service = new OrderService(uow);

        // Act
        var result = await service.TransitionOrderStateAsync(999, "Placed");

        // Assert
        Assert.True(result.IsFailure);
        repo.DidNotReceive().Update(Arg.Any<Order>());
        await uow.DidNotReceive().SaveChangesAsync();
    }
}
