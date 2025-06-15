using HMSYSTEM.Repository;
using Microsoft.AspNetCore.Mvc;

namespace HMSYSTEM.Controllers
{
    public class AppointmentController : Controller
    {
        protected readonly IUnitOfWork _unitofWork;

        public AppointmentController(IUnitOfWork unitofWork)
        {
            _unitofWork = unitofWork;
        }

        public IActionResult Index()
        {
           var appointments= _unitofWork.AppointmentRepository.GetAllAppointments();
            return View(appointments);
        }
    }
}
