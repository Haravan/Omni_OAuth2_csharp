using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using HaravanAuthorization.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;

namespace HaravanAuthorization.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            var userName = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            return View();
        }

        [Route("/request_grant_callback")]
        public async Task<IActionResult> request_grant_callback()
        {
            var userName = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var result = await HttpContext.AuthenticateAsync("Haravan");
            var userToken = await HttpContext.GetTokenAsync("access_token");
            var appToken = result.Properties.GetTokens().Where(m => m.Name == "access_token").FirstOrDefault()?.Value;
            //var userName = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            return Content("test");
        }

        [Route("/login_callback")]
        [AllowAnonymous]
        public async Task<IActionResult> login_callback()
        {
            var result = await HttpContext.AuthenticateAsync("Haravan");
            await HttpContext.SignInAsync(result.Principal, result.Properties);
            return Redirect("/");
        }
        

        [Route("/request_grant")]
        public IActionResult request_grant()
        {
            var userName = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var properties = new Microsoft.AspNetCore.Authentication.AuthenticationProperties();;
            properties.SetParameter("grant_service", true);
            properties.RedirectUri = $"http://localhost:5002/request_grant_callback";
                return Challenge(properties, "Haravan");
        }

        [Route("/account/login")]
        [AllowAnonymous]
        public IActionResult account_login()
        {
            var userName = HttpContext.User.Identity.Name;
            var properties = new Microsoft.AspNetCore.Authentication.AuthenticationProperties();
            properties.RedirectUri = $"http://localhost:5002/login_callback";
                return Challenge(properties, "Haravan");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
