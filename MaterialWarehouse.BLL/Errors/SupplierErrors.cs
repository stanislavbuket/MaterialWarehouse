using MaterialWarehouse.BLL.Common;

namespace MaterialWarehouse.BLL.Errors;

public static class SupplierErrors
{
    public static readonly Error NotFound = new(
        "Supplier.NotFound",
        "Постачальника не знайдено.");

    public static readonly Error DuplicateEmail = new(
        "Supplier.DuplicateEmail",
        "Постачальник з такою електронною адресою вже зареєстрований.");

    public static readonly Error InvalidContactPhone = new(
        "Supplier.InvalidContactPhone",
        "Вказано некоректний формат номеру телефону постачальника.");
}
