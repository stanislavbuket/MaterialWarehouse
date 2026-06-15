using System.ComponentModel.DataAnnotations;

namespace MaterialWarehouse.BLL.DTOs;

// Для виведення історії рухів товару
public record StockTransactionDto(
    int Id,
    int MaterialId,
    string MaterialName,
    int Quantity,
    DateTime TransactionDate,
    string TransactionType // "Надходження" або "Списання"
);

// Для оформлення нової транзакції (наприклад, привезли новий товар)
public class CreateStockTransactionDto
{
    [Range(1, int.MaxValue, ErrorMessage = "Будь ласка, оберіть коректний матеріал.")]
    public int MaterialId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Кількість має бути більшою за 0.")]
    public int Quantity { get; set; }

    [Required(ErrorMessage = "Тип транзакції є обов'язковим.")]
    public string TransactionType { get; set; } = null!; // "In" або "Out"
}