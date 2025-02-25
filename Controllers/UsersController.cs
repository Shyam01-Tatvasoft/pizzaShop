using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Org.BouncyCastle.Crypto.Generators;
using PizzaShop.Models;
using PizzaShop.Utils;
using PizzaShop.ViewModels;
public class Users : Controller
{
    private readonly PizzashopContext _context;

    public Users(PizzashopContext context)
    {
        _context = context;
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


        var count = _context.Users.Where(u => u.Isdeleted == false).ToList().Count();
        var totalPage = (int)Math.Ceiling(count / (double)pageSize);
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

    [HttpGet]
    public IActionResult CreateUser()
    {
        var email = Request.Cookies["email"];
        if (String.IsNullOrEmpty(email))
        {
            return RedirectToAction("Login", "Authentication");
        }
        var user = _context.Users.FirstOrDefault(u => u.Email == email);
        // also need to verify is it Admin or not
        if (user == null)
        {
            return RedirectToAction("Login", "Authentication");
        }
        var AllCountries = _context.Countries.ToList();
        var AllStates = _context.States.ToList();
        var AllCities = _context.Cities.ToList();
        ViewBag.AllCountries = AllCountries;
        ViewBag.AllCities = AllCities;
        ViewBag.AllStates = AllStates;

        return View();
    }

     public void SendMail(string ToEmail, string subject, string tempPassword)
    {
        string SenderMail = "test.dotnet@etatvasoft.com";
        string SenderPassword = "P}N^{z-]7Ilp";
        string Host = "mail.etatvasoft.com";
        int Port = 587;

        var smtpClient = new SmtpClient(Host)
        {
            Port = Port,
            Credentials = new NetworkCredential(SenderMail, SenderPassword),
        };

        MailMessage mailMessage = new MailMessage();
        mailMessage.From = new MailAddress(SenderMail);
        mailMessage.To.Add(ToEmail);
        mailMessage.Subject = subject;
        mailMessage.IsBodyHtml = true;
        string mailBody = $"Hello Your Account has been created and your Temporary Password is {tempPassword}";
        mailMessage.Body = mailBody;

        smtpClient.Send(mailMessage);
    }

    [HttpPost]
    public async Task<IActionResult> CreateUser(CreateUserViewModel model)
    {
        if (ModelState.IsValid)
        {
            var email = Request.Cookies["email"];

            // also need to verify is it Admin or not
            if (String.IsNullOrEmpty(email))
            {
                return RedirectToAction("Login", "Authentication");
            }

            var user = _context.Users.FirstOrDefault(u => u.Email == email);

            var AllCountries = await _context.Countries.ToListAsync();
            var AllStates = await _context.States.ToListAsync();
            var AllCities = await _context.Cities.ToListAsync();
            ViewBag.AllCountries = AllCountries;
            ViewBag.AllCities = AllCities;
            ViewBag.AllStates = AllStates;
            var findUser = _context.Users.FirstOrDefault(u => u.Email == model.Email || u.Username == model.Username);
            if (findUser != null)
            {
                ViewData["UserExistMsg"] = "User Already Exist";
                return View();
            }

            var newUser = new User
            {
                Firstname = model.Firstname,
                Lastname = model.Lastname,
                Username = model.Username,
                Roleid = _context.Roles.FirstOrDefault(r => r.Id == int.Parse(model.Role)).Id,
                Email = model.Email,
                Country = model.Country,
                State = model.State,
                City = model.City,
                Zipcode = model.Zipcode,
                Address = model.Address,
                Phone = model.Phone,
                Profileimage = model.Profileimage,
                Createdby = user.Id.ToString()
            };

            string HashPassword = PasswordUtills.HashPassword(model.Password);
            var newAccount = new Account
            {
                Email = model.Email,
                Roleid = _context.Roles.FirstOrDefault(r => r.Id == int.Parse(model.Role)).Id,
                Password = HashPassword,
                Createdby = user.Id.ToString(),
            };
            
            string subject = "Your new Account Details with Temporary password";
            SendMail(email,subject,model.Password);

            _context.Accounts.Add(newAccount);
            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();
            ViewData["SuccessMessage"] = "User Created Successfully";
            return RedirectToAction("Index", "Users");
        }
        else
        {
            var AllCountries = _context.Countries.ToList();
            var AllStates = _context.States.ToList();
            var AllCities = _context.Cities.ToList();
            ViewBag.AllCountries = AllCountries;
            ViewBag.AllCities = AllCities;
            ViewBag.AllStates = AllStates;
            return View();
        }

    }


    [HttpGet]
    public IActionResult UpdateUser(int Id)
    {
        var email = Request.Cookies["email"];
        if (String.IsNullOrEmpty(email))
        {
            return RedirectToAction("Login", "Authentication");
        }
        var user = _context.Users.FirstOrDefault(u => u.Email == email);
        // also need to verify is it Admin or not
        if (user == null)
        {
            return RedirectToAction("Login", "Authentication");
        }
        var AllCountries = _context.Countries.ToList();
        var AllStates = _context.States.ToList();
        var AllCities = _context.Cities.ToList();
        ViewBag.AllCountries = AllCountries;
        ViewBag.AllCities = AllCities;
        ViewBag.AllStates = AllStates;

        var editUser = _context.Users.FirstOrDefault(u => u.Id == Id);

        if (editUser == null)
        {
            ViewData["ErrorMessage"] = "User Not Found";
            return View("Index", "Users");
        }

        var model = new UpdateUserViewModel
        {
            Id = editUser.Id,
            Firstname = editUser.Firstname,
            Lastname = editUser.Lastname,
            Username = editUser.Lastname,
            Email = editUser.Email,
            Address = editUser.Address,
            City = editUser.City,
            State = editUser.State,
            Country = editUser.Country,
            Zipcode = editUser.Zipcode,
            Phone = editUser.Phone,
            Profileimage = editUser.Profileimage,
            Role = editUser.Roleid.ToString(),
            Updatedby = user.Id.ToString(),
            Updateddate = DateTime.Now,
            Status = user.Isactive == true ? "1" : "0"
        };
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> UpdateUser(UpdateUserViewModel model)
    {
        if (ModelState.IsValid)
        {
            var email = Request.Cookies["email"];
            if (String.IsNullOrEmpty(email))
            {
                return RedirectToAction("Login", "Authentication");
            }
            var user = _context.Users.FirstOrDefault(u => u.Email == email);

            var editUser = _context.Users.FirstOrDefault(u => u.Id == model.Id);
            var editAccount = _context.Accounts.FirstOrDefault(a => a.Email == editUser.Email);
            if (editUser != null && editAccount != null)
            {
                editUser.Firstname = model.Firstname;
                editUser.Lastname = model.Lastname;
                editUser.Username = model.Username;
                editUser.Address = model.Address;
                editUser.Phone = model.Phone;
                editUser.City = model.City;
                editUser.State = model.State;
                editUser.Country = model.Country;
                editUser.Zipcode = model.Zipcode;
                editUser.Isactive = model.Status == "1" ? true : false;
                editUser.Roleid = int.Parse(model.Role);
                editUser.Updateddate = DateTime.Now;
                editUser.Updatedby = user.Id.ToString();
                editUser.Profileimage = model.Profileimage;


                editAccount.Roleid = int.Parse(model.Role);
                editAccount.Updatedby = user.Id.ToString();
                editAccount.Updateddate = DateTime.Now;


                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Users");
            }

            return RedirectToAction("Index", "Users");
        }
        else
        {
            foreach (var item in ModelState.Values.SelectMany(V => V.Errors))
            {
                Console.WriteLine(item.ErrorMessage);
            }
            var AllCountries = _context.Countries.ToList();
            var AllStates = _context.States.ToList();
            var AllCities = _context.Cities.ToList();
            ViewBag.AllCountries = AllCountries;
            ViewBag.AllCities = AllCities;
            ViewBag.AllStates = AllStates;
            return View();
        }
    }

    public async Task<IActionResult> DeleteUser(int Id)
    {
        var email = Request.Cookies["email"];
        var user = _context.Users.FirstOrDefault(u => u.Email == email);

        if (user == null)
        {
            return RedirectToAction("Login", "Authentication");
        }

        var deleteUser = _context.Users.FirstOrDefault(u => u.Id == Id);
        var deleteAccount = _context.Accounts.FirstOrDefault(a => a.Email == deleteUser.Email);
        
        deleteUser.Isdeleted = true;
        deleteAccount.Isdeleted = true;
        _context.SaveChanges();
        ViewData["SuccessMsg"] = "User Deleted Successfully";
        return RedirectToAction("Index","Users");
    }
}