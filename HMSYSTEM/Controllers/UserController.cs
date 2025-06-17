using HMSYSTEM.Models;
using HMSYSTEM.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HMSYSTEM.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        protected readonly IUnitOfWork unitofwork;

        public UserController(IUnitOfWork unitofwork)
        {
            this.unitofwork = unitofwork;
        }

        [Authorize]
        public IActionResult Index()
        {
          var data=  unitofwork.UserRepository.GetAll();
            return View(data);
        }

        [HttpGet]
        public IActionResult Save()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Save(User user)
        {
            unitofwork.UserRepository.Save(user);
            return RedirectToAction("Index");
        }

    }
}
