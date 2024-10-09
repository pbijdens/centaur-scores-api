using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Net;
using System.Security;

namespace CentaurScores
{
    /// <summary>
    /// Middleware class that catches unhandled exceptions on endpoints and turns them into 
    /// Http status codes instead, hiding all details.
    /// </summary>
    /// <remarks>Constructor</remarks>
    public class ErrorHandlingMiddleware(RequestDelegate next)
    {
        /// <inheritdoc/>
        public async Task Invoke(HttpContext context)
        {
            // In an earlier step, a generic logger is added to the context so we do not 
            // have to deal with injection in this class, which should work officially
            // but consistently fails.
            ILogger<Program>? _logger = context.Items["logger"] as ILogger<Program>;

            string id = $"{Guid.NewGuid()}";
            try
            {
                await next(context);
            }
            catch (ArgumentException ex)
            {
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                await context.Response.WriteAsync(ErrorJson("BadRequest", id));
                _logger?.LogError(ex, "BadRequest: {id}", id);
            }
            catch (SecurityException ex)
            {
                context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                await context.Response.WriteAsync(ErrorJson("Forbidden", id));
                _logger?.LogError(ex, "Forbidden: {id}", id);
            }
            catch (UnauthorizedAccessException ex)
            {
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                await context.Response.WriteAsync(ErrorJson("Unauthorized", id));
                _logger?.LogError(ex, "Unauthorized: {id}", id);
            }
            catch (Exception ex)
            {
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                await context.Response.WriteAsync(ErrorJson("InternalServerError", id));
                _logger?.LogError(ex, "InternalServerError: {id}", id);
            }
        }

        private static string ErrorJson(string errorCode, string id)
        {
            Dictionary<string, string> result = new() {
                { "error", errorCode },
                { "id", id },   
            };
            return JsonConvert.SerializeObject(result);
        }
    }
}
