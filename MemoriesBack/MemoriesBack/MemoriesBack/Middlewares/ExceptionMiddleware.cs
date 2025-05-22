using Microsoft.AspNetCore.Http;
using System;
using System.Net;
using System.Threading.Tasks;

namespace MemoriesBack.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                context.Response.ContentType = "application/json";

                switch (ex)
                {
                    case ArgumentException:
                        context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                        break;
                    case InvalidOperationException:
                        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        break;
                    default:
                        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        break;
                }

                await context.Response.WriteAsync(ex.Message ?? "Internal server error");
            }
        }
    }
}
