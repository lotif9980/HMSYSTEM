using HMSYSTEM.Models;
using HMSYSTEM.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using System.Threading.Tasks;

namespace HMSYSTEM.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public AccountController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        [AllowAnonymous]  // লগইন পেজে এ্যানোনিমাস এক্সেস দিতে হবে
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]  // লগইন পোস্টেও এ্যানোনিমাস এক্সেস দিতে হবে
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = _unitOfWork.UserRepository.GetUser(model.Username, model.Password);
            if (user != null)
            {
                // Claim তৈরি করুন
                var claims = new[]
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    // প্রয়োজন অনুযায়ী অন্য Claim যোগ করতে পারেন
                };

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                // Authentication Cookie সেট করুন
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                // Optional: Session এ ডাটা রাখতে পারেন
                HttpContext.Session.SetString("Username", user.UserName);

                return RedirectToAction("Index", "Home");
            }

            ViewBag.Error = "Invalid Username or Password";
            return View(model);
        }

        [Authorize] 
        public async Task<IActionResult> Logout()
        {
            
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}
