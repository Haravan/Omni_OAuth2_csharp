using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace HaravanAuthorization.Controllers
{
    public class WebhookAuthorize : Attribute, IAsyncActionFilter
    {
        private readonly string clientSecret;

        public WebhookAuthorize()
        {
            clientSecret = "cac97ec3b4414b066acd665d3cc3571a4cd216925d3bbafdf642d770126e19b5";
        }

        public static string HashHmacSHA256(string body, string secretKey)
        {
            var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secretKey));
            return Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(body)));
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var request = context.HttpContext.Request;

            var hmac = (string)request.Headers["X-Haravan-HmacSha256"];

            if (string.IsNullOrWhiteSpace(hmac))
            {
                context.Result = new StatusCodeResult(401);
                return;
            }

            using (var reader = new StreamReader(request.Body))
            {
                var body_string = await reader.ReadToEndAsync();

                var signature = HashHmacSHA256(body_string, clientSecret);

                if (signature != hmac)
                {
                    context.Result = new StatusCodeResult(401);
                    return;
                }

                context.ActionArguments.Add("topic", (string)request.Headers["X-Haravan-Topic"]);
                context.ActionArguments.Add("orgId", (string)request.Headers["X-Haravan-Org-Id"]);
                context.ActionArguments.Add("body", body_string);
                context.ActionArguments.Add("test", request.Headers.ContainsKey("X-Haravan-Test"));
            }

            await next();
        }
    }
}
