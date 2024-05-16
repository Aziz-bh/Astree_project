using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ClientAstree.Filters
{
public class CustomExceptionFilter : IExceptionFilter
{
    private readonly ILogger<CustomExceptionFilter> _logger;

    public CustomExceptionFilter(ILogger<CustomExceptionFilter> logger)
    {
        _logger = logger;
    }

    public void OnException(ExceptionContext context)
    {
        _logger.LogError(context.Exception, "An error occurred.");

        var exceptionType = context.Exception.GetType();
        if (exceptionType == typeof(ValidationException))
        {
            context.Result = new BadRequestObjectResult(new { message = context.Exception.Message });
        }
        else if (exceptionType == typeof(UnauthorizedAccessException))
        {
            context.Result = new UnauthorizedResult();
        }
        else
        {
            context.Result = new ObjectResult(new { message = "An unexpected error occurred. Please try again later." })
            {
                StatusCode = 500
            };
        }

        context.ExceptionHandled = true;
    }
}
}