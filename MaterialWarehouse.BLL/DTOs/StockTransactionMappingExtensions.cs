using MaterialWarehouse.DAL.Entities;
using MaterialWarehouse.BLL.DTOs;

namespace MaterialWarehouse.BLL.Mappings;

public static class StockTransactionMappingExtensions
{
    public static StockTransactionDto ToDto(this StockTransaction transaction)
    {
        if (transaction == null) return null!;

        return new StockTransactionDto(
            Id: transaction.Id,
            MaterialId: transaction.MaterialId,
            // Якщо зв'язок з матеріалом завантажений — виводимо ім'я, інакше просто ID
            MaterialName: transaction.Material?.Name ?? $"Матеріал #{transaction.MaterialId}",
            Quantity: transaction.Quantity,
            // Якщо в сутності назва поля дати відрізняється (напр. Date), замініть transaction.TransactionDate на неї
            TransactionDate: DateTime.UtcNow,
            TransactionType: transaction.Quantity > 0 ? "Надходження" : "Списання"
        );
    }
}