using System.Net;
using System.Text.Json;
using SMIJobHeader.Common;

namespace SMIJobHeader.Helpers;

public class ErrorHandlerMiddleware
{
    private readonly ILogger _logger;
    private readonly RequestDelegate _next;

    public ErrorHandlerMiddleware(RequestDelegate next, ILogger<ErrorHandlerMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        var response = context.Response;
        response.ContentType = "application/json";
        var result = string.Empty;
        try
        {
            await _next(context);
        }
        catch (BusinessLogicException ex)
        {
            response.StatusCode = (int)HttpStatusCode.BadRequest;
            result = JsonSerializer.Serialize(new { code = (int)ex.ErrorCode, message = ex?.Message });
        }
        catch (Exception error)
        {
            switch (error)
            {
                case AppException e:
                    // custom application error
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    break;
                case KeyNotFoundException e:
                    // not found error
                    response.StatusCode = (int)HttpStatusCode.NotFound;
                    break;
                default:
                    // unhandled error
                    _logger.LogError(error, error.Message);
                    response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    break;
            }

            result = JsonSerializer.Serialize(new { code = 99, message = error?.Message });
        }

        await response.WriteAsync(result);
    }
}