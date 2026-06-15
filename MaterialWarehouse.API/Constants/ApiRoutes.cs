namespace MaterialWarehouse.API.Constants;

public static class ApiRoutes
{
    private const string Base = "api";

    public static class Materials
    {
        public const string BaseRoute = $"{Base}/materials";
        public const string GetById = "{id:int}";
        public const string StockLimit = "low-stock";
    }

    public static class Categories
    {
        public const string BaseRoute = $"{Base}/categories";
        public const string GetById = "{id:int}";
    }

    public static class Suppliers
    {
        public const string BaseRoute = $"{Base}/suppliers";
        public const string GetById = "{id:int}";
    }

    public static class Orders
    {
        public const string BaseRoute = $"{Base}/orders";
        public const string GetById = "{id:int}";
        public const string TransitionState = "{id:int}/state";
    }
}