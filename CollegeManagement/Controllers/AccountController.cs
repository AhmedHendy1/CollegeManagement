using Microsoft.AspNetCore.Mvc;
using CollegeManagement.Models;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;

public class AccountController : Controller
{
    private readonly StudentManagementSystemContext _context;

    public AccountController(StudentManagementSystemContext context)
    {
        _context = context;
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (ModelState.IsValid)
        {
            model.Email = model.Email.Trim();
            model.Password = model.Password.Trim();
            model.Role = model.Role.Trim();

            ClaimsIdentity identity = null;
            bool isAuthenticated = false;

            if (model.Role == "Admin")
            {
                var admin = await _context.Admins.FindAsync(model.Email);
                if (admin != null && BCrypt.Net.BCrypt.Verify(model.Password, admin.Password))
                {
                    identity = new ClaimsIdentity(new[]
                    {
                        new Claim(ClaimTypes.Name, admin.Email),
                        new Claim(ClaimTypes.Role, "Admin")
                    }, CookieAuthenticationDefaults.AuthenticationScheme);

                    isAuthenticated = true;
                }
            }
            else if (model.Role == "Employee")
            {
                var employee = _context.Employees.FirstOrDefault(e => e.Email == model.Email);
                if (employee != null && BCrypt.Net.BCrypt.Verify(model.Password, employee.Password))
                {
                    identity = new ClaimsIdentity(new[]
                    {
                        new Claim(ClaimTypes.Name, employee.Email),
                        new Claim(ClaimTypes.Role, "Employee")
                    }, CookieAuthenticationDefaults.AuthenticationScheme);

                    isAuthenticated = true;
                }
            }
            else if (model.Role == "Student")
            {
                var student = _context.Students.FirstOrDefault(s => s.Email == model.Email);
                if (student != null && BCrypt.Net.BCrypt.Verify(model.Password, student.Password))
                {
                    identity = new ClaimsIdentity(new[]
                    {
                        new Claim(ClaimTypes.Name, student.Email),
                        new Claim(ClaimTypes.Role, "Student")
                    }, CookieAuthenticationDefaults.AuthenticationScheme);

                    isAuthenticated = true;
                }
            }

            if (isAuthenticated)
            {
                var principal = new ClaimsPrincipal(identity);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                // Redirect based on role
                switch (model.Role)
                {
                    case "Admin":
                        return RedirectToAction("Index", "Admin");
                    case "Employee":
                        return RedirectToAction("Index", "Employee");
                    case "Student":
                        return RedirectToAction("Index", "Student");
                }
            }
            else
            {
                ModelState.AddModelError("", "Invalid email, password, or role.");
            }
        }

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Login", "Account");
    }
}
