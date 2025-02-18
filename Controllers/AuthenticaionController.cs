using System.Diagnostics;
using System.Net;
using System.Net.Mail;
using System.Text;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using PizzaShop.Models;
using PizzaShop.ViewModels;

public class Authentication : Controller
{
    private readonly PizzashopContext _context;
    private readonly IDataProtector _dataProtector;
    public Authentication(PizzashopContext context, IDataProtectionProvider dataProtectionProvider)
    {
        _context = context;
        _dataProtector = dataProtectionProvider.CreateProtector("ResetPasswordProtector");
    }

    [HttpGet]
    public IActionResult Login()
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

    public IActionResult EmailTemplate(String ResetLink)
    {
        return View();
    }

    private static string GetEmailTemplate(string ResetLink)
    {
        var templatePath = Path.Combine(Directory.GetCurrentDirectory(), "Views", "EmailTemplate.html");
        if (!System.IO.File.Exists(templatePath))
        {
            return "<p>Email template Not Fount</p>";
        }
        string emailbody = System.IO.File.ReadAllText(templatePath);
        return emailbody.Replace("{{Link}}", ResetLink);
    }



    public void SendMail(string ToEmail, string subject, string body)
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
        StringBuilder mailBody = new StringBuilder();
        mailMessage.Body = body;

        smtpClient.Send(mailMessage);
    }

    private string GenerateResetToken(string email)
    {
        DateTime expiry = DateTime.UtcNow.AddHours(24);
        string tokenData = $"{email} | {expiry.Ticks}";
        return _dataProtector.Protect(tokenData);       //encrypted token
    }

    [HttpPost]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
    {
        if (ModelState.IsValid)
        {
            var account = await _context.Accounts.FirstOrDefaultAsync(acc => acc.Email == model.Email);
            if (account == null)
            {
                ModelState.AddModelError("Email", "No account found with this email");
                return View(model);
            }

            string resetToken = GenerateResetToken(model.Email);
            string resetLink = $"http://localhost:5017/Authentication/ResetPassword?token={resetToken}";

            string subject = "Password reset request";
            string body = GetEmailTemplate(resetLink);

            SendMail(model.Email, subject, body);

            TempData["ForgotPasswordMsg"] = "Mail Sent Successfully. Please check your mail";
            return RedirectToAction("ForgotPassword");
        }
        return View(model);
    }

    [HttpGet]
    public IActionResult ResetPassword(string token)
    {
        if (string.IsNullOrEmpty(token))
        {
            return BadRequest("Invalid reset token");
        }
        var model = new ResetPasswordViewModel { Token = token };
        Console.WriteLine(model);
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
    {
        if (ModelState.IsValid)
        {
            string unprotectedToken;
            try
            {
                //decrypt the token
                unprotectedToken = _dataProtector.Unprotect(model.Token);
            }
            catch
            {
                ModelState.AddModelError("", "Invalid or expired password reset token");
                return View(model);
            }

            //token has {email} | {expiryticks}
            var parts = unprotectedToken.Split('|');
            if (parts.Length != 2 || !long.TryParse(parts[1], out long expiryTicks))
            {
                ModelState.AddModelError("error", "Invalid token");
                return View(model);
            }

            DateTime expiryDate = new DateTime(expiryTicks, DateTimeKind.Utc);      //converts expiry ticks into datetime object
            if (expiryDate < DateTime.UtcNow)
            {
                ModelState.AddModelError("error", "Password reset token has expired");
                return View(model);
            }

            string email = parts[0].Trim();
            var account = await _context.Accounts.FirstOrDefaultAsync(a => a.Email == email);
            if (account == null)
            {
                ModelState.AddModelError("error", "Account not found");
                return View(model);
            }

            account.Password = model.NewPassword;
            Console.WriteLine("Save changes called");
            _context.SaveChanges();

            TempData["SuccessMessage"] = "Your password has been reset successfully, you can now log in";
            return RedirectToAction("Login", "Authentication");
        }
        return View(model);
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