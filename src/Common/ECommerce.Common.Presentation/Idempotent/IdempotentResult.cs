using Microsoft.AspNetCore.Http;

namespace ECommerce.Common.Presentation.Idempotent;

internal sealed class IdempotentResult(int statusCode, object? value) : IResult
{
    public async Task ExecuteAsync(HttpContext httpContext)
    {
        httpContext.Response.StatusCode = statusCode;

        if (value is not null)
        {
            await httpContext.Response.WriteAsJsonAsync(value);
        }
    }
}
