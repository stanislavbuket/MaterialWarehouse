using MaterialWarehouse.DAL.Entities;
using MaterialWarehouse.DAL.Entities.Users;

namespace MaterialWarehouse.DAL;

public static class DbInitializer
{
    public static void Initialize(MaterialWarehouseDbContext context)
    {
        context.Database.EnsureCreated();

        if (context.Categories.Any())
        {
            return;
        }

        var categories = new Category[]
        {
            new(1, "Будівельні матеріали", "Матеріали для загальнобудівельних робіт"),
            new(2, "Інструменти", "Ручний та механічний робочий інструмент")
        };

        context.Categories.AddRange(categories);
        context.SaveChanges();

        var materials = new Material[]
        {
            new(1, "Цегла", "Червона керамічна цегла", 5000, "шт", 1, 500),
            new(2, "Цемент М500", "Портландцемент високої міцності", 150, "мішок", 1, 20),
            new(3, "Молоток", "Слюсарний молоток з дерев'яною ручкою", 45, "шт", 2, 5),
            new(4, "Викрутка", "Універсальна викрутка зі змінними бітами", 80, "шт", 2, 10)
        };

        context.Materials.AddRange(materials);
        context.SaveChanges();

        var users = new User[]
        {
            new Administrator(1, "admin_chief", "admin@materialwarehouse.com", "admin_secure_hash"),
            new Manager(2, "manager_john", "john.manager@materialwarehouse.com", "manager_secure_hash", "Склад №1"),
            new RegisteredUser(3, "budpostach_client", "contact@budpostach.ua", "client_secure_hash", "ТОВ БудПостач")
        };

        context.Users.AddRange(users);
        context.SaveChanges();
    }
}
