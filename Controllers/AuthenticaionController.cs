using System.Diagnostics;
using System.Net;
using System.Net.Mail;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PizzaShop.Models;
using PizzaShop.ViewModels;

public class Authentication : Controller
{
    private readonly PizzashopContext _context;
    public Authentication(PizzashopContext context)
    {
        _context = context;
    }

    [HttpGet]
    public ActionResult Login()
    {
        Console.WriteLine(Request.Cookies["email"]);

        if (Request.Cookies["email"] != null)
        {
            Console.WriteLine(Request.Cookies["email"]);
            return RedirectToAction("Index", "Home");
        }
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel login)
    {
        if (ModelState.IsValid)
        {
            var email = login.Email;
            var password = login.Password;

            var user = await _context.Accounts.FirstOrDefaultAsync(d => d.Email == email && d.Password == password);

            if (user != null)
            {
                TempData["Email"] = email;
                if (login.RememberMe)
                {
                    var option = new CookieOptions
                    {
                        Expires = DateTime.Now.AddDays(30),
                        Secure = true
                    };
                    Response.Cookies.Append("email", email, option);
                }

                return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewBag.Message = "UserName or password is Wrong";
                ModelState.AddModelError("", "Please Enter Valid Credentials");
                return View();
            }
        }
        return View(login);
    }


    public ActionResult ForgotPassword()
    {
        return View();
    }

    [HttpPost]
    public void SendMail(string ToEmail)
    {
        string SenderMail = "mailto:test.dotnet@etatvasoft.com";
        string SenderPassword = "P}N^{z-]7Ilp";
        string Host = "mail.etatvasoft.com";
        int Port = 587;
        var smtpClient = new SmtpClient(Host)
        {
            Port = Port,
            Credentials = new NetworkCredential(SenderMail, SenderPassword),
            // EnableSsl = true,
        };
        string? resetLink = Url.Action("ResetPassword", "Home", new { email = ToEmail, timeStamp = DateTime.UtcNow.Ticks }, Request.Scheme);

        var mailMessage = new MailMessage
        {
            From = new MailAddress(SenderMail),
            Subject = "To Reset Your Password",
            Body = $"Click <a href = '{resetLink}' > Here </a> to reset your password",
            IsBodyHtml = true,
        };
        mailMessage.To.Add(ToEmail);

        smtpClient.Send(mailMessage);
    }

    public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
    {
        if (ModelState.IsValid)
        {
            var user = _context.Accounts.FirstOrDefaultAsync(u => u.Email == model.Email);

            if (user != null)
            {
                SendMail(model.Email);
            }
            return View("ForgotPasswordConfirmation");
        }
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