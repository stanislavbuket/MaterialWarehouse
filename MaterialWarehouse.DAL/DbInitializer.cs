using MaterialWarehouse.DAL.Entities;
using MaterialWarehouse.DAL.Entities.Users;

namespace MaterialWarehouse.DAL;

public static class DbInitializer
{
    public static async Task SeedAsync(MaterialWarehouseDbContext context)
    {
        // Перевіряємо, чи база вже не заповнена
        if (context.Users.Any() || context.Categories.Any()) return;

        // 1. Створюємо перших користувачів з ролями
        var admin = new Administrator(
            id: 0,
            username: "admin_boss",
            email: "admin@warehouse.com",
            passwordHash: "AdminStrictPassword2026_Hashed"
        );

        var manager = new Manager(
            id: 0,
            username: "manager_john",
            email: "john@warehouse.com",
            passwordHash: "ManagerPass123_Hashed",
            department: "Головний склад"
        );

        await context.Users.AddRangeAsync(admin, manager);

        // 2. Створюємо базові категорії через конструктор 
        // ВИПРАВЛЕНО (CS7036): додано обов'язковий третій параметр — опис категорії
        var metalCategory = new Category(0, "Металопрокат", "Різноманітні вироби з металу, сталі та арматури");
        var plasticCategory = new Category(0, "Пластик та Полімери", "Труби, пластикові деталі та будівельні з'єднувачі");

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