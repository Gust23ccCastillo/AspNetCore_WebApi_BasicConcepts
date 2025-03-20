using BookingApplication.Services.MiddlewareGlobal;

namespace BookingApplication.WebApi.MiddlewareApplication
{
    public class GlobalMiddleware
    {
        private readonly ILogger _logger;
        private readonly RequestDelegate _next;

        public GlobalMiddleware(RequestDelegate next, ILogger<GlobalMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                // Procesa la solicitud normalmente
                await _next(context);
            }
            catch (Exception ex)
            {
                // Maneja cualquier excepción que ocurra durante la ejecución
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        object errors = null;
        int statusCode;

        switch (exception)
        {
            case ExecuteMiddlewareGlobalOfProyect customException:
                _logger.LogError(exception, "Excepción personalizada detectada");
                errors = customException._ErrorsRequest;
                statusCode = (int)customException._HttpStatusCode;
                break;
            //CAPTURA DE EXCEPCIONES DE FLUENT VALIDATION
            case FluentValidation.ValidationException validationException:
                _logger.LogError(exception, "Errores de validación detectados");
                errors = validationException.Errors.Select(err => new
                {
                    Property = err.PropertyName,
                    ErrorMessage = err.ErrorMessage
                });
                statusCode = StatusCodes.Status400BadRequest;
                break;

            case OperationCanceledException:
                _logger.LogError(exception, "Operación cancelada por el cliente");
                errors = new { Message = "La operación fue cancelada por el cliente." };
                statusCode = StatusCodes.Status408RequestTimeout;
                break;

            default:
                _logger.LogError(exception, "Error interno del servidor");
                errors = string.IsNullOrWhiteSpace(exception.Message) ? "Error interno" : exception.Message;
                statusCode = StatusCodes.Status500InternalServerError;
                break;
        }

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = statusCode;

        if (errors != null)
        {
            var response = new { Errors = errors };
            await context.Response.WriteAsJsonAsync(response);
        }
    }
    }

}
