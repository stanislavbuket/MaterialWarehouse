using MaterialWarehouse.DAL.Entities;
using MaterialWarehouse.DAL.Entities.Users;
using Microsoft.AspNetCore.Identity;

namespace MaterialWarehouse.DAL;

public static class DbInitializer
{
    public static async Task SeedAsync(MaterialWarehouseDbContext context)
    {
        // Перевіряємо, чи база вже не заповнена
        if (context.Users.Any() || context.Categories.Any()) return;

        var hasher = new PasswordHasher<object>();

        // 1. Створюємо перших користувачів з ролями
        var admin = new Administrator(
            id: 0,
            username: "admin_boss",
            email: "admin@warehouse.com",
            passwordHash: hasher.HashPassword(null!, "AdminStrictPassword2026")
        );

        var manager = new Manager(
            id: 0,
            username: "manager_john",
            email: "john@warehouse.com",
            passwordHash: hasher.HashPassword(null!, "ManagerPass123")
        );

        await context.Users.AddRangeAsync(admin, manager);

        // 2. Створюємо базові категорії
        var metalCategory = new Category { Name = "Металопрокат" };
        var plasticCategory = new Category { Name = "Пластик та Полімери" };

        await context.Categories.AddRangeAsync(metalCategory, plasticCategory);
        await context.SaveChangesAsync();

        // 3. Створюємо стартові матеріали
        var steelSheet = new Material(
            id: 0,
            name: "Сталевий лист 4мм",
            description: "Лист гарячекатаний",
            quantity: 150,
            unit: "шт",
            categoryId: metalCategory.Id,
            minStockLimit: 20
        );

        var pvcPipe = new Material(
            id: 0,
            name: "Труба ПВХ 110мм",
            description: "Труба каналізаційна зовнішня",
            quantity: 15,
            unit: "м",
            categoryId: plasticCategory.Id,
            minStockLimit: 30
        );

        await context.Materials.AddRangeAsync(steelSheet, pvcPipe);
        await context.SaveChangesAsync();
    }
}