using Microsoft.AspNetCore.Mvc;

namespace HMSYSTEM.Controllers
{
    public class DailyExpenseController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
