using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Middleware.NoSniff;

public class NoSniffMiddleware
{
    private readonly RequestDelegate _next;

    public NoSniffMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
        await _next(context);
    }
}
