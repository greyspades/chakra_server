using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Net.Http.Headers;
using System.Text;
using AES;
using Newtonsoft.Json;
using Candidate.Models;

namespace InputFormat;

public class XInputFormatter: InputFormatter
{
    public XInputFormatter()
    {
        // SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/x-payload"));
        SupportedMediaTypes.Add("application/*");
        SupportedMediaTypes.Add("text/*");
    }

    public override async Task<InputFormatterResult> ReadRequestBodyAsync(InputFormatterContext context)
    {
        var request = context.HttpContext.Request;

        using (var reader = new StreamReader(request.Body, Encoding.UTF8))
        {
            var httpContext = context.HttpContext;

            var controllerName = httpContext.Request.RouteValues["controller"] as string;
            
            var actionName = httpContext.Request.RouteValues["action"] as string;

            Console.WriteLine(actionName);
            Console.WriteLine(controllerName);

            if(actionName == "CreateCandidate") {
                Console.WriteLine("is create");
                var encryptedPayload = await reader.ReadToEndAsync();

                var payload = JsonConvert.DeserializeObject<CreateCandidateDto>(encryptedPayload);

                // var decryptedPayload = AEShandler.Decrypt((string)payload.Data, "yy7a1^.^^j_ii^c2^5^ho_@.9^d7bi^." , "h!!_2bz^(@?yyq!.");

                // var deserializedData = JsonConvert.DeserializeObject<CandidateModel>(decryptedPayload);

                // payload.Data = deserializedData;

                // return await InputFormatterResult.SuccessAsync(payload);
                return await InputFormatterResult.NoValueAsync();
            }
            else {
                Console.WriteLine("is not create");
                var encryptedPayload = await reader.ReadToEndAsync();

            var decryptedPayload = AEShandler.Decrypt(encryptedPayload, "yy7a1^.^^j_ii^c2^5^ho_@.9^d7bi^." , "h!!_2bz^(@?yyq!.");

            var payload = JsonConvert.DeserializeObject(decryptedPayload);
                return await InputFormatterResult.SuccessAsync(payload);
            }
        }
    }

    // public async Task<InputFormatterResult> ReadAsync(InputFormatterContext context)
    // {
        

    //     // Continue with your custom input formatting logic
    //     // ...
    // }

}

// internal class T
// {
// }