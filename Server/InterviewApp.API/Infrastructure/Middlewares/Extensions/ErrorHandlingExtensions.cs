namespace InterviewApp.API.Infrastructure.Middlewares.Extensions
{
    public static class ErrorHandlingExtensions
    {
        public static IApplicationBuilder HandlerErrors(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ErrorHandlingMiddleware>();
        }
    }
}