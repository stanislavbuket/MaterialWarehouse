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

    [Fact]
    public async Task CreateOrderAsync_WithInStockMaterial_ReservesStockAndCreatesOrder()
    {
        // Arrange
        var uow = Substitute.For<IUnitOfWork>();
        var materialRepo = Substitute.For<IRepository<Material>>();
        var orderRepo = Substitute.For<IRepository<Order>>();

        var material = new Material(1, "Цегла", "Будівельна цегла", 10, "шт", 1, 5);

        materialRepo.GetByIdAsync(1).Returns(material);
        uow.GetRepository<Material>().Returns(materialRepo);
        uow.GetRepository<Order>().Returns(orderRepo);

        var service = new OrderService(uow);
        var items = new List<(int materialId, int quantity)> { (1, 4) };

        // Act
        var result = await service.CreateOrderAsync(1, items);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(4, material.ReservedQuantity);
        Assert.Equal(6, material.AvailableQuantity);
        materialRepo.Received(1).Update(material);
        await orderRepo.Received(1).AddAsync(Arg.Any<Order>());
        await uow.Received(1).SaveChangesAsync();
    }

    [Fact]
    public async Task CreateOrderAsync_WithPreOrderMaterial_MarksAsPreOrderWithoutReservation()
    {
        // Arrange
        var uow = Substitute.For<IUnitOfWork>();
        var materialRepo = Substitute.For<IRepository<Material>>();
        var orderRepo = Substitute.For<IRepository<Order>>();

        var material = new Material(1, "Цегла", "Будівельна цегла", 10, "шт", 1, 5);

        materialRepo.GetByIdAsync(1).Returns(material);
        uow.GetRepository<Material>().Returns(materialRepo);
        uow.GetRepository<Order>().Returns(orderRepo);

        var service = new OrderService(uow);
        var items = new List<(int materialId, int quantity)> { (1, 12) };

        // Act
        var result = await service.CreateOrderAsync(1, items);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(0, material.ReservedQuantity);
        Assert.Equal(10, material.AvailableQuantity);
        materialRepo.DidNotReceive().Update(Arg.Any<Material>());
        await orderRepo.Received(1).AddAsync(Arg.Is<Order>(o => o.Items.First().IsPreOrder == true));
        await uow.Received(1).SaveChangesAsync();
    }

    [Fact]
    public async Task TransitionOrderStateAsync_ToShipped_ShipsReservedAndLogsTransaction()
    {
        // Arrange
        var uow = Substitute.For<IUnitOfWork>();
        var orderRepo = Substitute.For<IRepository<Order>>();
        var orderItemRepo = Substitute.For<IRepository<OrderItem>>();
        var materialRepo = Substitute.For<IRepository<Material>>();
        var transactionRepo = Substitute.For<IRepository<StockTransaction>>();

        var material = new Material(1, "Цегла", "Будівельна цегла", 10, "шт", 1, 5);
        material.Reserve(4);

        var order = new Order(1) { Id = 101, State = OrderState.Approved };
        var orderItem = new OrderItem(1, 101, 1, 4, false);

        orderRepo.GetByIdAsync(101).Returns(order);
        materialRepo.GetByIdAsync(1).Returns(material);
        orderItemRepo.GetAllAsync().Returns(new List<OrderItem> { orderItem });

        uow.GetRepository<Order>().Returns(orderRepo);
        uow.GetRepository<OrderItem>().Returns(orderItemRepo);
        uow.GetRepository<Material>().Returns(materialRepo);
        uow.GetRepository<StockTransaction>().Returns(transactionRepo);

        var service = new OrderService(uow);

        // Act
        var result = await service.TransitionOrderStateAsync(101, "Shipped", 42);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(6, material.Quantity);
        Assert.Equal(0, material.ReservedQuantity);
        materialRepo.Received(1).Update(material);
        await transactionRepo.Received(1).AddAsync(Arg.Is<StockTransaction>(t =>
            t.MaterialId == 1 &&
            t.Type == TransactionType.Sale &&
            t.Quantity == 4 &&
            t.ManagerId == 42));
        await uow.Received(1).SaveChangesAsync();
    }

    [Fact]
    public async Task TransitionOrderStateAsync_ToCancelled_ReleasesReserved()
    {
        // Arrange
        var uow = Substitute.For<IUnitOfWork>();
        var orderRepo = Substitute.For<IRepository<Order>>();
        var orderItemRepo = Substitute.For<IRepository<OrderItem>>();
        var materialRepo = Substitute.For<IRepository<Material>>();

        var material = new Material(1, "Цегла", "Будівельна цегла", 10, "шт", 1, 5);
        material.Reserve(4);

        var order = new Order(1) { Id = 101, State = OrderState.Approved };
        var orderItem = new OrderItem(1, 101, 1, 4, false);

        orderRepo.GetByIdAsync(101).Returns(order);
        materialRepo.GetByIdAsync(1).Returns(material);
        orderItemRepo.GetAllAsync().Returns(new List<OrderItem> { orderItem });

        uow.GetRepository<Order>().Returns(orderRepo);
        uow.GetRepository<OrderItem>().Returns(orderItemRepo);
        uow.GetRepository<Material>().Returns(materialRepo);

        var service = new OrderService(uow);

        // Act
        var result = await service.TransitionOrderStateAsync(101, "Cancelled");

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(10, material.Quantity);
        Assert.Equal(0, material.ReservedQuantity);
        materialRepo.Received(1).Update(material);
        await uow.Received(1).SaveChangesAsync();
    }
}
