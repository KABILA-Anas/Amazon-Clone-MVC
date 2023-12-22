using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;

namespace ECommerce.Controllers
{
    public class AccessController : Controller
    {
        public IActionResult Login()
        {
            ClaimsPrincipal currentUser = HttpContext.User;
            if (currentUser.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Products");
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string email ,string password, bool rememberMe)
        {
            if (email == "admin@gmail.com" && password == "admin")
            {
                List<Claim> claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Email, email),
                    new Claim(ClaimTypes.Role, "Administrator"),
                };

                ClaimsIdentity claimsIdentity = 
                    new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                AuthenticationProperties authProperties = new AuthenticationProperties
                {
                    AllowRefresh = true,
                    IsPersistent = rememberMe
                };

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);

                return RedirectToAction("Index", "Products");

            } else if (email == "customer@gmail.com" && password == "customer")
            {
                List<Claim> claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Email, email),
                    new Claim(ClaimTypes.Role, "Customer"),
                };

                ClaimsIdentity claimsIdentity = 
                    new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                AuthenticationProperties authProperties = new AuthenticationProperties
                {
                    AllowRefresh = true,
                    IsPersistent = rememberMe
                };

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);

                return RedirectToAction("Index", "Products");
            }

            ViewData["Error"] = "Invalid email or password";
            return View();


        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }
    }
}
