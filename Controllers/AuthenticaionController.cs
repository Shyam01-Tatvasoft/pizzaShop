using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using PizzaShop.Models;

public class Authentication: Controller
{
    private readonly ILogger<Authentication> _logger;
    public Authentication(ILogger<Authentication> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    public ActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public ActionResult Login(string email, string password)
    {
        
        return View();
    }


    public ActionResult ForgotPassword()
    {
        return View();
    }

    public ActionResult ResetPassword()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public ActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    [HttpGet]
    public ActionResult Test()
    {
        return View();
    }
}