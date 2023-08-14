using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace EncryptMiddleware;

public class EncryptionMiddleware
{
    private readonly RequestDelegate _next;

    public EncryptionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var originalBodyStream = context.Response.Body;

        try {
                    using (var modifiedStream = new MemoryStream())
        {
                        context.Response.Body = modifiedStream;

            await _next(context);

            modifiedStream.Seek(0, SeekOrigin.Begin);

            // Modify the response content here
            var responseContent = await new StreamReader(modifiedStream).ReadToEndAsync();
            responseContent = ModifyResponseContent(responseContent);

            // Convert the modified content to bytes
            var responseBytes = Encoding.UTF8.GetBytes(responseContent);

            // Update the response content length
            context.Response.ContentLength = responseBytes.Length;

            // Write the modified content back to the response stream
            await context.Response.Body.WriteAsync(responseBytes, 0, responseBytes.Length);
        }
        } catch(Exception e) {
            Console.WriteLine(e.Message);
            await _next(context);
        }
    }

    private string ModifyResponseContent(string content)
    {
        // Modify the content as needed
        return content.ToUpper();
    }
}
