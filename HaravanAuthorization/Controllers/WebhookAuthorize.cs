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

        private string HashHmacSHA1(string originalData, string secretKey)
        {
            var sha1 = null as byte[];

            using (var crypto = new HMACSHA1(Encoding.UTF8.GetBytes(secretKey)))
                sha1 = crypto.ComputeHash(Encoding.UTF8.GetBytes(originalData));

            var hashString = new StringBuilder();

            foreach (var b in sha1)
                hashString.Append(b.ToString("x2"));

            var result = hashString.ToString();

            return result;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var request = context.HttpContext.Request;

            var hmac = (string)request.Headers["X-Haravan-Hmac-Sha256"];

            if (string.IsNullOrWhiteSpace(hmac))
            {
                context.Result = new StatusCodeResult(401);
                return;
            }

            using (var reader = new StreamReader(request.Body))
            {
                var body_string = await reader.ReadToEndAsync();

                var signature = HashHmacSHA1(body_string, clientSecret);

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
