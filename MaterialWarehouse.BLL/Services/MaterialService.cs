using MaterialWarehouse.BLL.Common;
using MaterialWarehouse.BLL.DTOs;
using MaterialWarehouse.BLL.Interfaces;
using MaterialWarehouse.BLL.Mappings;
using MaterialWarehouse.BLL.Errors;
using MaterialWarehouse.DAL.Interfaces;
using MaterialWarehouse.DAL.Entities;

namespace MaterialWarehouse.BLL.Services;

public class MaterialService(IUnitOfWork unitOfWork) : IMaterialService
{
    public async Task<Result<MaterialDto>> GetByIdAsync(int id)
    {
        var repo = unitOfWork.GetRepository<Material>();
        var material = await repo.GetByIdAsync(id);
        if (material == null)
            return Result.Failure<MaterialDto>(MaterialErrors.NotFound);

        return Result.Success(material.ToDto());
    }

    public async Task<Result<IEnumerable<MaterialDto>>> GetAllAsync()
    {
        var repo = unitOfWork.GetRepository<Material>();
        var materials = await repo.GetAllAsync();
        return Result.Success(materials.Select(m => m.ToDto()));
    }

    public async Task<Result<MaterialDto>> CreateAsync(CreateMaterialDto dto)
    {
        var repo = unitOfWork.GetRepository<Material>();

        // Повний фікс CS7036: Передаємо параметри у ПРАВИЛЬНОМУ порядку, як у вашому Material.cs:
        // (id, name, description, quantity, unit, categoryId, minStockLimit)
        var material = new Material(
            0,                          // id (0 для нового запису)
            dto.Name,                   // name
            dto.Description,            // description
            dto.Quantity,               // quantity
            "шт",                       // unit (одиниця виміру за замовчуванням)
            dto.CategoryId,              // categoryId
            dto.MinStockLimit           // minStockLimit
        );

        await repo.AddAsync(material);
        await unitOfWork.SaveChangesAsync();
        return Result.Success(material.ToDto());
    }

    public async Task<Result> UpdateAsync(int id, UpdateMaterialDto dto)
    {
        var repo = unitOfWork.GetRepository<Material>();
        var material = await repo.GetByIdAsync(id);
        if (material == null)
            return Result.Failure(MaterialErrors.NotFound);

        // Повний фікс CS1061: Викликаємо щойно створений метод з домену
        material.UpdateDetails(dto.Name, dto.Description, dto.MinStockLimit, dto.CategoryId);

        // Безпечно коригуємо кількість через ваш рідний метод AdjustQuantity
        int quantityDifference = dto.Quantity - material.Quantity;
        if (quantityDifference != 0)
        {
            material.AdjustQuantity(quantityDifference);
        }

        repo.Update(material);
        await unitOfWork.SaveChangesAsync();
        return Result.Success();
    }

    public async Task<Result> DeleteAsync(int id)
    {
        var repo = unitOfWork.GetRepository<Material>();
        var material = await repo.GetByIdAsync(id);
        if (material == null)
            return Result.Failure(MaterialErrors.NotFound);

        repo.Delete(material);
        await unitOfWork.SaveChangesAsync();
        return Result.Success();
    }

    public async Task<Result<IEnumerable<MaterialDto>>> GetInStockMaterialsAsync()
    {
        var repo = unitOfWork.GetRepository<Material>();
        var materials = await repo.GetAllAsync();
        var inStock = materials.Where(m => m.Quantity > 0).Select(m => m.ToDto());
        return Result.Success(inStock);
    }

    public async Task<Result> AdjustStockAsync(int id, int amount)
    {
        var repo = unitOfWork.GetRepository<Material>();
        var material = await repo.GetByIdAsync(id);
        if (material == null)
            return Result.Failure(MaterialErrors.NotFound);

        if (material.Quantity + amount < 0)
            return Result.Failure(MaterialErrors.OutOfStock);

        material.AdjustQuantity(amount);

        repo.Update(material);
        await unitOfWork.SaveChangesAsync();
        return Result.Success();
    }
}