using MaterialWarehouse.BLL.Common;
using MaterialWarehouse.BLL.DTOs;
using MaterialWarehouse.BLL.Errors;
using MaterialWarehouse.BLL.Interfaces;
using MaterialWarehouse.DAL.Entities;
using MaterialWarehouse.DAL.Interfaces;

namespace MaterialWarehouse.BLL.Services;

public class MaterialService(IUnitOfWork unitOfWork) : IMaterialService
{
    public async Task<Result<IEnumerable<MaterialDto>>> GetInStockMaterialsAsync()
    {
        var repo = unitOfWork.GetRepository<Material>();

    var materials = await repo.GetAllAsync();

        var dtos = materials
            .Where(m => m.Quantity > 0)
            .Select(m => new MaterialDto(
                m.Id,
                m.Name,
                m.Description,
                m.Quantity,
                m.Unit,
                m.CategoryId,
                m.Category?.Name ?? string.Empty,
                m.MinStockLimit,
                m.ReservedQuantity));

        return Result.Success(dtos);
    }

    public async Task<Result<MaterialDto>> GetByIdAsync(int id)
    {
        var repo = unitOfWork.GetRepository<Material>();

        var material = await repo.GetByIdAsync(id);

        if (material == null)
            return Result.Failure<MaterialDto>(MaterialErrors.NotFound);

        var dto = new MaterialDto(
            material.Id,
            material.Name,
            material.Description,
            material.Quantity,
            material.Unit,
            material.CategoryId,
            material.Category?.Name ?? string.Empty,
            material.MinStockLimit,
            material.ReservedQuantity);

        return Result.Success(dto);
    }

    public async Task<Result> AdjustStockAsync(int materialId, int amount)
    {
        var repo = unitOfWork.GetRepository<Material>();

        var material = await repo.GetByIdAsync(materialId);

        if (material == null)
            return Result.Failure(MaterialErrors.NotFound);

        if (material.Quantity + amount < 0)
            return Result.Failure(MaterialErrors.OutOfStock);

        material.AdjustQuantity(amount);

        repo.Update(material);

        await unitOfWork.SaveChangesAsync();

        return Result.Success();
    }

    public async Task<Result> ReceiveStockAsync(
        int materialId,
        int quantity,
        int supplierId,
        int managerId)
    {
        var materialRepo = unitOfWork.GetRepository<Material>();
        var transactionRepo = unitOfWork.GetRepository<StockTransaction>();

        var material = await materialRepo.GetByIdAsync(materialId);

        if (material == null)
            return Result.Failure(MaterialErrors.NotFound);

        material.AdjustQuantity(quantity);

        materialRepo.Update(material);

        var transaction = new StockTransaction(
            materialId,
            TransactionType.Receive,
            quantity,
            managerId);

        await transactionRepo.AddAsync(transaction);

        await unitOfWork.SaveChangesAsync();

        return Result.Success();
    }

    public async Task<Result> WriteOffStockAsync(
        int materialId,
        int quantity,
        int managerId)
    {
        var materialRepo = unitOfWork.GetRepository<Material>();
        var transactionRepo = unitOfWork.GetRepository<StockTransaction>();

        var material = await materialRepo.GetByIdAsync(materialId);

        if (material == null)
            return Result.Failure(MaterialErrors.NotFound);

        if (material.Quantity < quantity)
            return Result.Failure(MaterialErrors.OutOfStock);

        material.AdjustQuantity(-quantity);

        materialRepo.Update(material);

        var transaction = new StockTransaction(
            materialId,
            TransactionType.WriteOff,
            quantity,
            managerId);

        await transactionRepo.AddAsync(transaction);

        await unitOfWork.SaveChangesAsync();

        return Result.Success();
    }

}
