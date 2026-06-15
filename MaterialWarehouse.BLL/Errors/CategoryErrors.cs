using MaterialWarehouse.BLL.Common; // Тут зазвичай лежить базовий клас або record "Error"

namespace MaterialWarehouse.BLL.Errors;

public static class CategoryErrors
{
    public static readonly Error NotFound = new(
        "Category.NotFound",
        "Вказану категорію матеріалів не знайдено.");

    public static readonly Error DuplicateName = new(
        "Category.DuplicateName",
        "Категорія з такою назвою вже існує в системі.");

    public static readonly Error HasAssociatedMaterials = new(
        "Category.HasAssociatedMaterials",
        "Неможливо видалити категорію, оскільки до неї прив'язані матеріали.");
}