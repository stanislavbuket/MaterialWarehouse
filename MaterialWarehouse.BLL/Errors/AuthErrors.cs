using MaterialWarehouse.BLL.Common;

namespace MaterialWarehouse.BLL.Errors;

public static class AuthErrors
{
    public static readonly Error EmailAlreadyExists = new("Auth.EmailAlreadyExists", "Користувач із такою електронною поштою вже зареєстрований.");
    public static readonly Error InvalidCredentials = new("Auth.InvalidCredentials", "Неправильна електронна пошта або пароль.");
    public static readonly Error InvalidUserData = new("Auth.InvalidUserData", "Некоректні вхідні дані користувача.");
}
