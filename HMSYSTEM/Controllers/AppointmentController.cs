using HMSYSTEM.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HMSYSTEM.Controllers
{
    [Authorize]
    public class AppointmentController : Controller
    {
        protected readonly IUnitOfWork _unitofWork;

        public AppointmentController(IUnitOfWork unitofWork)
        {
            _unitofWork = unitofWork;
        }

        [Authorize]
        public IActionResult Index()
        {
           var appointments= _unitofWork.AppointmentRepository.GetAllAppointments();
            return View(appointments);
        }
    }
}
