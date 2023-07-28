using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace OnlineWebApp.Filters
{
    public class CustomExceptionFilterAttribute : ExceptionFilterAttribute
    {
        readonly ILogger<CustomExceptionFilterAttribute> _logger;

        public CustomExceptionFilterAttribute(ILogger<CustomExceptionFilterAttribute> logger)
        {
            _logger = logger;
        }

        public override void OnException(ExceptionContext context)
        {
            _logger.LogError(context.Exception, "[全局异常]");

            var result = new ContentResult()
            {
                Content = context.Exception.Message,
                StatusCode = 500
            };

            context.Result = result;
        }
    }
}
