using MaterialWarehouse.BLL.Common;
using MaterialWarehouse.BLL.DTOs;
using MaterialWarehouse.BLL.Errors;
using MaterialWarehouse.BLL.Interfaces;
using MaterialWarehouse.DAL.Interfaces;
using MaterialWarehouse.DAL.Entities;

namespace MaterialWarehouse.BLL.Services;

public class MaterialService(IUnitOfWork unitOfWork) : IMaterialService
{
    public async Task<Result<IEnumerable<MaterialDto>>> GetInStockMaterialsAsync()
    {
        var repo = unitOfWork.GetRepository<Material>();
        var materials = await repo.GetAllAsync();
        var dtos = materials
            .Where(m => m.Quantity > 0)
            .Select(m => new MaterialDto(m.Id, m.Name, m.Description, m.Quantity, m.Unit, m.CategoryId, m.Category?.Name ?? string.Empty));
        return Result.Success(dtos);
    }

    public async Task<Result<MaterialDto>> GetByIdAsync(int id)
    {
        var repo = unitOfWork.GetRepository<Material>();
        var m = await repo.GetByIdAsync(id);
        if (m == null)
            return Result.Failure<MaterialDto>(MaterialErrors.NotFound);

        var dto = new MaterialDto(m.Id, m.Name, m.Description, m.Quantity, m.Unit, m.CategoryId, m.Category?.Name ?? string.Empty);
        return Result.Success(dto);
    }

    public async Task<Result> AdjustStockAsync(int materialId, int amount)
    {
        var repo = unitOfWork.GetRepository<Material>();
        var m = await repo.GetByIdAsync(materialId);
        if (m == null)
            return Result.Failure(MaterialErrors.NotFound);

        if (m.Quantity + amount < 0)
            return Result.Failure(MaterialErrors.OutOfStock);

        m.AdjustQuantity(amount);

        repo.Update(m);
        await unitOfWork.SaveChangesAsync();
        return Result.Success();
    }
}
