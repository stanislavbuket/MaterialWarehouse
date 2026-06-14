using MaterialWarehouse.BLL.Common;

namespace MaterialWarehouse.BLL.Errors;

public static class MaterialErrors
{
    public static readonly Error NotFound = new("Material.NotFound", "Матеріал не знайдено.");
    public static readonly Error OutOfStock = new("Material.OutOfStock", "Недостатньо матеріалу на складі для здійснення операції.");
}

public static class OrderErrors
{
    public static readonly Error NotFound = new("Order.NotFound", "Замовлення не знайдено.");
    public static readonly Error InvalidStateTransition = new("Order.InvalidTransition", "Некоректний перехід у даний стан замовлення.");
}
