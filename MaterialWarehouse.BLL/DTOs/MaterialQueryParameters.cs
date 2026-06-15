namespace MaterialWarehouse.BLL.DTOs;

/// <summary>
/// Параметри для пошуку, фільтрації та пагінації матеріалів
/// </summary>
public class MaterialQueryParameters
{
    private const int MaxPageSize = 50;
    private int _pageSize = 10;

    // Пошук за назвою або описом
    public string? SearchTerm { get; set; }

    // Фільтрація за конкретною категорією
    public int? CategoryId { get; set; }

    // Показувати тільки ті матеріали, які закінчуються (менше ліміту)
    public bool? OnlyLowStock { get; set; }

    // Налаштування сторінок
    public int PageNumber { get; set; } = 1;

    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = value > MaxPageSize ? MaxPageSize : value;
    }
}
