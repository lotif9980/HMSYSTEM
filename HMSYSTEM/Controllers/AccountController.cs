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
        [AllowAnonymous] 
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = _unitOfWork.UserRepository.GetUser(model.Username?.Trim(), model.Password?.Trim());

            if (user == null)
            {
                ModelState.AddModelError("", "❌ Invalid username or password!");
                return View(model);
            }

            if (user.Status == false)
            {
                ModelState.AddModelError("", "⚠️ Your account is inactive. Please contact admin.");
                return View(model);
            }

            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.Name, user.UserName),
        new Claim("RoleId", user.RoleId?.ToString() ?? "0"),
        new Claim("FullName", user.Name ?? "")
    };

            if (user.DoctorId != null && user.DoctorId > 0)
                claims.Add(new Claim("DoctorId", user.DoctorId.ToString()));

            var claimsIdentity = new ClaimsIdentity(claims, "MyCookieAuth");

            await HttpContext.SignInAsync(
                "MyCookieAuth",
                new ClaimsPrincipal(claimsIdentity),
                new AuthenticationProperties
                {
                    IsPersistent = false,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(30)
                });

            return RedirectToAction("Index", "Home");
        }


        [Authorize] 
        public async Task<IActionResult> Logout()
        {

            await HttpContext.SignOutAsync("MyCookieAuth");
            return RedirectToAction("Login", "Account");
        }
    }
}
