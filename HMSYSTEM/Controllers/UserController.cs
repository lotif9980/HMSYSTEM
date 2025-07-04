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
            try
            {
                unitofwork.UserRepository.Save(user);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                
                ModelState.AddModelError("Same Name alreay added", ex.Message);
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
            return View(data);
        }
    }
}
