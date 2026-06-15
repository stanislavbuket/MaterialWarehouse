using MaterialWarehouse.BLL.Common;
using MaterialWarehouse.BLL.DTOs;

namespace MaterialWarehouse.BLL.Interfaces;

public interface IReportService
{
    Task<Result<IEnumerable<MaterialDto>>> GetLowStockReportAsync();

    /// <summary>
    /// Контракт для отримання дефіцитних матеріалів (Етап 5.2)
    /// </summary>
    Task<Result<IEnumerable<MaterialDeficitDto>>> GetDeficitReportAsync();
}