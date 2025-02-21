using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PizzaShop.Models;
using PizzaShop.ViewModels;
public class Users : Controller
{
    private readonly PizzashopContext _context;

    public Users(PizzashopContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> GetUsers(int pageIndex, int pageSize)
    {
        var users = await _context.Users
            .OrderBy(u => u.Id)
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var count = await _context.Users.CountAsync();
        var totalPages = (int)Math.Ceiling(count / (double)pageSize);

        return Ok(users);
    }


    [HttpGet]
    public async Task<IActionResult> Index(string searchString, int pageIndex = 1, int pageSize = 3)
    {
        var AuthToken = Request.Cookies["AuthToken"];
        if (string.IsNullOrEmpty(AuthToken))
        {
            return RedirectToAction("Login", "Authentication");
        }

        var userQuery = _context.Users.Where(u => u.Isdeleted == false);

        if (!string.IsNullOrEmpty(searchString))
        {
            userQuery = userQuery.Where(n => n.Firstname.Contains(searchString) || n.Lastname.Contains(searchString) || n.Phone.Contains(searchString));
        }

        var userList = new List<UsersViewModel>();

        var usersList = userQuery
            .OrderBy(u => u.Id)
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToList();


        var count = _context.Users.Count();
        Console.WriteLine("count" + count);
        Console.WriteLine("pageSize" + pageSize);
        Console.WriteLine("user1" + _context.Users.Count());
        var totalPage = (int)Math.Ceiling(count / (double)pageSize);
        Console.WriteLine("total" + totalPage);
        ViewBag.Count = count;
        ViewBag.pageIndex = pageIndex;
        ViewBag.pageSize = pageSize;
        ViewBag.TotalPage = totalPage;
        ViewBag.SearchString = searchString;

        foreach (var item in usersList)
        {
            userList.Add(new UsersViewModel
            {
                Id = item.Id,
                Firstname = item.Firstname,
                Lastname = item.Lastname,
                Profileimage = item.Profileimage,
                Email = item.Email,
                Phone = item.Phone,
                Country = item.Country,
                State = item.State,
                City = item.City,
                Zipcode = item.Zipcode,
                Address = item.Address,
                Role = item.Roleid == 1 ? "Admin" : item.Roleid == 2 ? "Account" : "Chef",
                Isactive = item.Isactive == true ? "Active" : "InActive",
                Username = item.Username
            });
        }

        return View(userList);
    }
}