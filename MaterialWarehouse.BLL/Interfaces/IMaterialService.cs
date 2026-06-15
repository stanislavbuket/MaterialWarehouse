using MaterialWarehouse.BLL.Common;
using MaterialWarehouse.BLL.DTOs;

namespace MaterialWarehouse.BLL.Interfaces;

public interface IMaterialService
{
    Task<Result<MaterialDto>> GetByIdAsync(int id);
    Task<Result<IEnumerable<MaterialDto>>> GetAllAsync();
    Task<Result<MaterialDto>> CreateAsync(CreateMaterialDto dto);
    Task<Result> UpdateAsync(int id, UpdateMaterialDto dto);
    Task<Result> DeleteAsync(int id);
    Task<Result<IEnumerable<MaterialDto>>> GetInStockMaterialsAsync();
    Task<Result> AdjustStockAsync(int id, int amount);
}