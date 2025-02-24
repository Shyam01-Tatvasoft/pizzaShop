using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Mail;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualBasic;
using PizzaShop.Models;
using PizzaShop.Utils;
using PizzaShop.ViewModels;

public class Authentication : Controller
{
    private readonly PizzashopContext _context;
    private readonly IDataProtector _dataProtector;
    private readonly IConfiguration _config; // for access appsetting.json data we use IConfiguration Interface
    public Authentication(PizzashopContext context, IDataProtectionProvider dataProtectionProvider, IConfiguration config)
    {
        _context = context;
        _dataProtector = dataProtectionProvider.CreateProtector("ResetPasswordProtector");
        _config = config;
    }

    [HttpGet]
    public IActionResult Login()
    {
        var AuthToken = Request.Cookies["AuthToken"];

        if (!string.IsNullOrEmpty(AuthToken))
        {
            return RedirectToAction("Index", "Home");
        }
        return View();
    }

    private string GenerateToken(string email, string role){
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
         var authClaims = new List<Claim>{
            new Claim(ClaimTypes.Email, email),
            new Claim(ClaimTypes.Role, role),
         };

        var token = new JwtSecurityToken(
            issuer: _config["Jwt: Issuer"],
            audience: _config["Jwt:Audience"],
            claims: authClaims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
        
        // return tokenValue;
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
                // var verify = PasswordUtills.VerifyPassword(login.Password,user.Password);
                // if(!verify){
                //     ViewBag.Message = "Invalid Password";
                //     return View();
                // }
                // get Role
                var role = await _context.Roles.FirstOrDefaultAsync(a => a.Id == user.Roleid);
                
                // create JWT Token 
                var token = GenerateToken(email,role.Name);
                
                if(token != ""){
                    TempData["token"] = token;
                    Response.Cookies.Append("AuthToken",token, new CookieOptions{
                        Secure = true,
                        Expires = DateTime.UtcNow.AddMinutes(30),
                        HttpOnly = true
                    });
                }
                
                 var option = new CookieOptions
                    {
                        Expires = DateTime.Now.AddMinutes(30),
                        Secure = true
                    };
                    Response.Cookies.Append("email", email, option);
                // Store Jwt Authentication Toke
                using(var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer",token);
                }

                if (login.RememberMe)
                {
                    Response.Cookies.Append("AuthToken",token, new CookieOptions{
                        Secure = true,
                        Expires = DateTime.UtcNow.AddMinutes(30),
                        HttpOnly = true
                    });
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

    [HttpGet]
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
        Console.WriteLine("TokenData" + tokenData);
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