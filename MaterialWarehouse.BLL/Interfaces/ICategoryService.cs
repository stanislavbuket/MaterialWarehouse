using MaterialWarehouse.BLL.Common;
using MaterialWarehouse.BLL.DTOs;

namespace MaterialWarehouse.BLL.Interfaces;

public interface ICategoryService
{
    Task<Result<CategoryDto>> GetByIdAsync(int id);
    Task<Result<IEnumerable<CategoryDto>>> GetAllAsync();
    Task<Result<CategoryDto>> CreateAsync(CreateUpdateCategoryDto dto);
    Task<Result> UpdateAsync(int id, CreateUpdateCategoryDto dto);
    Task<Result> DeleteAsync(int id);
}