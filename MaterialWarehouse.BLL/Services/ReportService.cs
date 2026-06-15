using MaterialWarehouse.BLL.Common;
using MaterialWarehouse.BLL.DTOs;
using MaterialWarehouse.BLL.Interfaces;
using MaterialWarehouse.BLL.Mappings;
using MaterialWarehouse.DAL.Entities;
using MaterialWarehouse.DAL.Interfaces;

namespace MaterialWarehouse.BLL.Services;

public class ReportService(IUnitOfWork unitOfWork) : IReportService
{
    public async Task<Result<IEnumerable<MaterialDto>>> GetLowStockReportAsync()
    {
        var materialRepo = unitOfWork.GetRepository<Material>();
        var allMaterials = await materialRepo.GetAllAsync();

        var lowStockMaterials = allMaterials
            .Where(m => m.Quantity <= m.MinStockLimit)
            .Select(m => m.ToDto())
            .ToList();

        return Result.Success<IEnumerable<MaterialDto>>(lowStockMaterials);
    }

    /// <summary>
    /// Етап 5.2: Реалізація звіту про дефіцит матеріалів
    /// </summary>
    public async Task<Result<IEnumerable<MaterialDeficitDto>>> GetDeficitReportAsync()
    {
        var materialRepo = unitOfWork.GetRepository<Material>();
        var allMaterials = await materialRepo.GetAllAsync();

        // Продумана бізнес-формула: дефіцит виникає, якщо зарезервовано більше, ніж є на складі
        var deficitMaterials = allMaterials
            .Where(m => m.ReservedQuantity > m.Quantity)
            .Select(m => new MaterialDeficitDto(
                MaterialId: m.Id,
                MaterialName: m.Name,
                AvailableQuantity: m.Quantity,
                ReservedQuantity: m.ReservedQuantity,
                DeficitQuantity: m.ReservedQuantity - m.Quantity // Вираховуємо точну кількість дефіциту
            ))
            .ToList();

        return Result.Success<IEnumerable<MaterialDeficitDto>>(deficitMaterials);
    }
}