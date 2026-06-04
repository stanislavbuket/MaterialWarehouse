using MaterialWarehouse.BLL.Common;
using MaterialWarehouse.BLL.DTOs;

namespace MaterialWarehouse.BLL.Interfaces;

public interface IMaterialService
{
    Task<Result<IEnumerable<MaterialDto>>> GetInStockMaterialsAsync();
    Task<Result<MaterialDto>> GetByIdAsync(int id);
    Task<Result> AdjustStockAsync(int materialId, int amount);
}
