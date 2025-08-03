using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using HMSYSTEM.Models;
using Microsoft.AspNetCore.Authorization;
using HMSYSTEM.Repository;
using HMSYSTEM.ViewModels;

namespace HMSYSTEM.Controllers;
[Authorize]
public class HomeController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<HomeController> _logger;

    

    public HomeController(IUnitOfWork unitOfWork ,ILogger<HomeController> logger)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    [Authorize]
    public IActionResult Index()
    {
        var TotalBed = _unitOfWork.bedRepository.TotalBedCount();
        var totalOccupied = _unitOfWork.bedRepository.TotalOccupied();
        var totalWard=_unitOfWork.wardRepository.TotalWard();
        var totalEmpty = TotalBed - totalOccupied;
        var totalPatient = _unitOfWork.PatienRepo.CountPatinet();
        var totalPrescription = _unitOfWork.PrescriptioRepository.GetCountPrescription();
        var totalAppointment=_unitOfWork.AppointmentRepository.GetAppointmentsCount();


        var data = new HomeViewModel
        {
            TotalBed = TotalBed,
            TotalOccupied= totalOccupied,
            TotalWard= totalWard,
            TotalEmpty= totalEmpty,
            TotalPatient= totalPatient,
            TotalPrescription= totalPrescription,
            TotalAppointment= totalAppointment,
        };


        if (string.IsNullOrEmpty(HttpContext.Session.GetString("Username")))
        {
            return RedirectToAction("Login", "Account");
        }

        return View(data);
    }
    

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
