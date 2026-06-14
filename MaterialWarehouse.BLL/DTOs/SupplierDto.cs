using System.ComponentModel.DataAnnotations;

namespace MaterialWarehouse.BLL.DTOs;

// Головний об'єкт для виводу постачальника
public record SupplierDto(int Id, string Name, string ContactPhone, string Email);

// Об'єкт для створення та редагування постачальника через API
public class CreateUpdateSupplierDto
{
    [Required(ErrorMessage = "Назва постачальника є обов'язковою.")]
    [StringLength(150, MinimumLength = 2, ErrorMessage = "Назва має бути від 2 to 150 символів.")]
    public string Name { get; set; } = null!;

    [Required(ErrorMessage = "Номер телефону є обов'язковим.")]
    [Phone(ErrorMessage = "Некоректний формат номеру телефону.")]
    public string ContactPhone { get; set; } = null!;

    [Required(ErrorMessage = "Email є обов'язковим.")]
    [EmailAddress(ErrorMessage = "Некоректний формат Email адреси.")]
    public string Email { get; set; } = null!;
}