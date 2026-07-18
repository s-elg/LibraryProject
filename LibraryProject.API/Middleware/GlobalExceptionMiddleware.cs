using System.Net;
using System.Text.Json;
using LibraryProject.API.Common;
using LibraryProject.Application.Exceptions;

namespace LibraryProject.API.Middleware;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (AppException ex)
        {
            // Beklenen iş kuralı hataları - bilgi seviyesinde logla, stack trace kirletmesin
            _logger.LogWarning(ex, "Business rule exception: {Message}", ex.Message);
            await WriteErrorResponseAsync(context, ex.StatusCode, ex.Message);
        }
        catch (Exception ex)
        {
            // Beklenmeyen hatalar - detaylı logla ama kullanıcıya iç detay sızdırma
            _logger.LogError(ex, "Unhandled exception occurred");
            await WriteErrorResponseAsync(
                context,
                HttpStatusCode.InternalServerError,
                "Beklenmeyen bir hata oluştu. Lütfen daha sonra tekrar deneyin.");
        }
    }

    private static async Task WriteErrorResponseAsync(HttpContext context, HttpStatusCode statusCode, string message)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        var response = new ErrorResponseDto(message, context.TraceIdentifier);
        var json = JsonSerializer.Serialize(response);

        await context.Response.WriteAsync(json);
    }
}