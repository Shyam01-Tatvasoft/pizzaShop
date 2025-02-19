using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using PizzaShop.Models;

namespace PizzaShop.Controllers;

public class UserController : Controller
{
    
    private readonly PizzashopContext _context;

    public UserController(PizzashopContext context){
        _context = context;
    } 

    public IActionResult User(){
        
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public ActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}

