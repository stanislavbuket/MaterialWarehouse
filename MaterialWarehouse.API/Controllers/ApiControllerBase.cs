using Microsoft.AspNetCore.Mvc;
using MaterialWarehouse.BLL.Common;

namespace MaterialWarehouse.API.Controllers;

[ApiController]
public abstract class ApiControllerBase : ControllerBase
{
    /// <summary>
    /// Автоматично обробляє об'єкт Result з даними і повертає 200 OK або помилку
    /// </summary>
    protected IActionResult HandleResult<T>(Result<T> result)
    {
        if (result.IsSuccess)
            return Ok(result.Value);

        return HandleFailure(result);
    }

    /// <summary>
    /// Обробляє об'єкт Result без даних і повертає 204 No Content або помилку
    /// </summary>
    protected IActionResult HandleResult(Result result)
    {
        if (result.IsSuccess)
            return NoContent();

        return HandleFailure(result);
    }

    /// <summary>
    /// Внутрішній метод для розумного мапінгу бізнес-помилок у HTTP статуси
    /// </summary>
    private IActionResult HandleFailure(Result result)
    {
        // Якщо код помилки закінчується на ".NotFound" — автоматично віддаємо 404
        if (result.Error.Code.EndsWith(".NotFound", StringComparison.OrdinalIgnoreCase))
        {
            return NotFound(new
            {
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.4",
                Title = "Ресурс не знайдено",
                Status = StatusCodes.Status404NotFound,
                Detail = result.Error.Message
            });
        }

        // Для всіх інших бізнес-помилок (наприклад, недостатньо товару) віддаємо 400 Bad Request
        return BadRequest(new
        {
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
            Title = "Помилка валідації бізнес-логіки",
            Status = StatusCodes.Status400BadRequest,
            Detail = result.Error.Message
        });
    }
}