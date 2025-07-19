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
            ViewBag.Roles=unitofwork.RoleRepository.GetRoles();
            return View();
        }

        [HttpPost]
        public IActionResult Save(User user)
        {
            if (user == null)
            {
                TempData["Message"] = "❌ Invalid user data submitted.";
                TempData["MessageType"] = "error";
                return View();
            }

            try
            {
                unitofwork.UserRepository.Save(user); 
                TempData["Message"] = "✅ Successfully added!";
                TempData["MessageType"] = "success";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Message"] = "❌ " + (ex?.Message ?? "An error occurred.");
                TempData["MessageType"] = "danger";
                ViewBag.Roles = unitofwork.RoleRepository.GetRoles();
                return View(user); 
            }
        }

       
        public IActionResult Delete(int Id)
        {
            unitofwork.UserRepository.Delete(Id);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Edit(int Id)
        {
            var data = unitofwork.UserRepository.Find(Id);

            var roles= unitofwork.RoleRepository.GetRoles();

            ViewBag.Role = roles;
            return View(data);
        }

        [HttpPost]
        public IActionResult Update(User user)
        {
            unitofwork.UserRepository.Update(user);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Details(int Id)
        {
            var data = unitofwork.UserRepository.Find(Id);

            var roles = unitofwork.RoleRepository.GetRoles();

            ViewBag.Role = roles;
            return View(data);
        }

      

       
    }
}
