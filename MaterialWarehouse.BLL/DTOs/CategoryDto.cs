using System.ComponentModel.DataAnnotations;

namespace MaterialWarehouse.BLL.DTOs;

// Головний об'єкт для виводу категорії
public record CategoryDto(int Id, string Name);

// Об'єкт для створення/оновлення категорії через API
public class CreateUpdateCategoryDto
{
    [Required(ErrorMessage = "Назва категорії є обов'язковою.")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "Назва категорії має бути від 2 до 50 символів.")]
    public string Name { get; set; } = null!;
}