using System.Net;
using System.Security.Authentication;
using System.Text.Json;
using InterviewApp.Models.Shared.ApiResponse;

namespace InterviewApp.API.Infrastructure.Middlewares
{
    public class ErrorHandlingMiddleware : IMiddleware
    {
        public ErrorHandlingMiddleware()
        {
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var code = HttpStatusCode.InternalServerError; // 500 if unexpected
            ApiResponseModel apiResponseModel = null;

            if (exception is AuthenticationException)
            {
                code = HttpStatusCode.Unauthorized;
                apiResponseModel = new ApiResponseModel
                {
                    Status = false,
                    Message = string.Empty
                };
            }
            else if (exception is OperationCanceledException)
            {
                code = HttpStatusCode.Conflict;
                apiResponseModel = new ApiResponseModel
                {
                    Status = false,
                    Message = string.Empty
                };
            }
            else if (exception is AccessViolationException accessViolationException)
            {
                code = HttpStatusCode.Forbidden;
                apiResponseModel = new ApiResponseModel
                {
                    Status = false,
                    Message = exception.Message
                };
            }
            else if (exception != null)
            {
                apiResponseModel = new ApiResponseModel
                {
                    Status = false,
                    Message = exception.Message
                };
            }

            var result = JsonSerializer.Serialize(apiResponseModel);

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)code;
            return context.Response.WriteAsync(result);
        }
    }
}