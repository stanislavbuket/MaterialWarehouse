using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace MaterialWarehouse.API.Infrastructure;

/// <summary>
/// Патерн: Глобальний ланцюжок обробки помилок (Chain of Responsibility).
/// Ловить БУДЬ-ЯКІ непередбачувані помилки у системі, про які розробник навіть не здогадувався.
/// </summary>
public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> _logger)
    {
        this._logger = _logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        // 1. Логуємо абсолютно все: яка саме взаємодія зламалася і де
        _logger.LogError(exception, "🚨 Виникла непередбачена помилка системи: {Message}", exception.Message);

        // 2. Формуємо красиву відповідь за міжнародним стандартом RFC 7807
        var problemDetails = new ProblemDetails
        {
            Status = (int)HttpStatusCode.InternalServerError,
            Title = "Внутрішня помилка сервера",
            Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1",
            Detail = "На сервері виникла непередбачена ситуація. Інженери вже працюють над цим.",
            Instance = httpContext.Request.Path
        };

        // 3. Відправляємо клієнту (фронтенду) безпечний статус 500 замість "сирого" тексту помилки
        httpContext.Response.StatusCode = problemDetails.Status!.Value;
        httpContext.Response.ContentType = "application/json";

        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        // Повертаємо true, що означає: помилка успішно перехоплена і знешкоджена!
        return true;
    }
}