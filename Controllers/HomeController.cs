using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using PizzaShop.Models;

namespace PizzaShop.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        var AuthToken = Request.Cookies["AuthToken"];
        if(string.IsNullOrEmpty(AuthToken)){
            return RedirectToAction("Login", "Authentication");
        }else{
            return View();
        }
    }

    public IActionResult Privacy()
    {
        return View();
    }

    public IActionResult Login(){
        return View();  
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
