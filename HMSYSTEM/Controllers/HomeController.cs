using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using HMSYSTEM.Models;
using Microsoft.AspNetCore.Authorization;
using HMSYSTEM.Repository;

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
        ViewBag.TotalBed = TotalBed;

        var totalOccupied = _unitOfWork.bedRepository.TotalOccupied();
        ViewBag.TotalOccupied = totalOccupied;

        var totalWard=_unitOfWork.wardRepository.TotalWard();
        ViewBag.TotalWard= totalWard;

        var totalEmpty = TotalBed - totalOccupied;
        ViewBag.TotalEmpty = totalEmpty;

        var totalPatient = _unitOfWork.PatienRepo.CountPatinet();
        ViewBag.TotalPatient = totalPatient;

        var totalPrescription = _unitOfWork.PrescriptioRepository.GetCountPrescription();
        ViewBag.TotalPrescription= totalPrescription;



        if (string.IsNullOrEmpty(HttpContext.Session.GetString("Username")))
        {
            return RedirectToAction("Login", "Account");
        }

        return View();
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
