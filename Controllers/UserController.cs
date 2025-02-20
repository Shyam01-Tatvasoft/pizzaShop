using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PizzaShop.Models;
using PizzaShop.ViewModels;

namespace PizzaShop.Controllers;

public class UserController : Controller
{

    private readonly PizzashopContext _context;

    public UserController(PizzashopContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> Profile()
    {
        var AuthToken = Request.Cookies["AuthToken"];

        if (string.IsNullOrEmpty(AuthToken))
        {
            return RedirectToAction("Login", "Authentication");
        }
        else
        {
            var email = Request.Cookies["email"];
            var account = _context.Accounts.FirstOrDefault(a => a.Email == email);
            if (account == null)
            {
                return RedirectToAction("Login", "Authentication");
            }

            var role = _context.Roles.FirstOrDefault(a => a.Id == account.Roleid).Name;
            var user = _context.Users.FirstOrDefault(a => a.Email == account.Email);
            var country = _context.Countries.FirstOrDefault(a => a.Id == int.Parse(user.Country));
            var city = _context.Cities.FirstOrDefault(a => a.Id == int.Parse(user.City));
            var state = _context.States.FirstOrDefault(a => a.Id == int.Parse(user.State));

            var AllCountries = await _context.Countries.ToListAsync();
            var AllStates = _context.States.Where(s => s.Countryid == country.Id).ToList();
            var AllCities = _context.Cities.Where(c => c.Stateid == city.Id).ToList();
            ViewBag.AllCountries = AllCountries;
            ViewBag.AllCities = AllCities;
            ViewBag.AllStates = AllStates;

            ViewData["role"] = role;
            ViewData["email"] = email;
            ViewData["FirstName"] = user.Firstname;
            ViewData["LastName"] = user.Lastname;
            // Console.WriteLine("Data " + AllStates[1].Name);
            Console.WriteLine("Data " + user.Address);
            Console.WriteLine("Data " + user.Email);
            var userViewModel = new ProfileViewModel
            {
                FirstName = user.Firstname,
                LastName = user.Lastname,
                UserName = user.Username,
                Phone = user.Phone,
                Country = country.Id.ToString(),
                State = state.Id.ToString(),
                City = city.Id.ToString(),
                ZipCode = user.Zipcode,
                Address = user.Address
            };
            return View(userViewModel);
        }
    }

    [HttpPost]
    public async Task<IActionResult> Profile(ProfileViewModel model)
    {
        if (ModelState.IsValid)
        {
            var email = Request.Cookies["email"];

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null)
            {
                return View("Login", "Authentication");
            }

            user.Firstname = model.FirstName;
            user.Lastname = model.LastName;
            user.Username = model.UserName;
            user.Phone = model.Phone;
            user.City = model.City;
            user.State = model.State;
            user.Country = model.Country;
            user.Address = model.Address;
            user.Zipcode = model.ZipCode;
            await _context.SaveChangesAsync();
            var AllCountries = await _context.Countries.ToListAsync();
            var AllStates = _context.States.Where(s => s.Countryid == int.Parse(user.Country)).ToList();
            var AllCities = _context.Cities.Where(c => c.Stateid == int.Parse(user.State)).ToList();
            var role = _context.Roles.FirstOrDefault(a => a.Id == user.Roleid).Name;

            ViewBag.AllCountries = AllCountries;
            ViewBag.AllCities = AllCities;
            ViewBag.AllStates = AllStates;
            ViewData["ProfileSuccessMessage"] = "Profile Updated SucessFully";
            ViewData["role"] = role;
            ViewData["email"] = email;
            ViewData["FirstName"] = user.Firstname;
            ViewData["LastName"] = user.Lastname;
            return View();
        }
        else
        {
            return View();
        }
    }

    [HttpGet]
    public ActionResult ChangePassword()
    {
        var email = Request.Cookies["email"];

        var user = _context.Users.FirstOrDefault(u => u.Email == email);
        if (user == null)
        {
            return View("Login", "Authentication");
        }
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
    {
        if (ModelState.IsValid)
        {
            var email = Request.Cookies["email"];

            var account = _context.Accounts.FirstOrDefault(u => u.Email == email);
            if (account == null){
                return RedirectToAction("Login", "Authentication");
            }
            if (account.Password == model.CurrentPassword){
                account.Password = model.NewPassword;
                await _context.SaveChangesAsync();
                ViewData["ChangePassword"] = "Password Changed Successfully";
                return View();
            }
            else{
                ViewData["WrongPassword"] = "Wrong current Password Please Try Again !";
                return View();
            }
        }
        return View();
    }

    public IActionResult Logout(){
        Console.WriteLine("Logout");
        var token = Request.Cookies["AuthToken"];
        if(string.IsNullOrEmpty(token)){
            return RedirectToAction("Login","Authentication");
        }
        Response.Cookies.Delete("AuthToken");
        Response.Cookies.Delete("email");
        return RedirectToAction("Login","Authentication");
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public ActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}

