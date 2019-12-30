using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace HaravanAuthorization.Controllers
{
    [Route("api/[controller]")]
    public partial class WebhookController : Controller
    {
        // POST api/values
        [ServiceFilter(typeof(WebhookAuthorize))]
        [HttpPost]
        public async Task Post(string orgId, string topic, string body, bool test)
        {
            switch (topic)
            {
                case "products/update":
                    break;
                case "products/deleted":
                    break;
                case "app/uninstalled":
                    break;
            }
        }

        [HttpGet]
        public string VerifyWebhook()
        {
            string verify_token = HttpContext.Request.Query["hub.verify_token"].ToString();

            string challenge = HttpContext.Request.Query["hub.challenge"].ToString();

            if (verify_token == "cac97ec3b4414b066acd665d3cc3571a4cd216925d3bbafdf642d770126e19b5")
                return challenge;

            return null;
        }
    }
}