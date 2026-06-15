using MaterialWarehouse.BLL.Common;
using MaterialWarehouse.BLL.DTOs;

namespace MaterialWarehouse.BLL.Interfaces;

public interface ISupplierService
{
    Task<Result<SupplierDto>> GetByIdAsync(int id);
    Task<Result<IEnumerable<SupplierDto>>> GetAllAsync();
    Task<Result<SupplierDto>> CreateAsync(CreateUpdateSupplierDto dto);
    Task<Result> UpdateAsync(int id, CreateUpdateSupplierDto dto);
    Task<Result> DeleteAsync(int id);
}