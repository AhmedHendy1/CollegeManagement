using CollegeManagement.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly StudentManagementSystemContext _context;

    public AdminController(StudentManagementSystemContext context)
    {
        _context = context;
    }

    // GET: Admin Dashboard
    public IActionResult Index()
    {
        var viewModel = new AdminDashboardViewModel
        {
            Employees = _context.Employees.ToList(),
            Admins = _context.Admins.ToList()
        };
        return View(viewModel);
    }

    // GET: Redirect to Employees Management
    public IActionResult ManageEmployees()
    {
        return RedirectToAction("Employees", "Admin");
    }

    // GET: View all Admins
    public IActionResult ViewAdmins()
    {
        var admins = _context.Admins.ToList();
        return View(admins);
    }

    // GET: Admin/Add
    public IActionResult AddAdmin()
    {
        return View();
    }

    // POST: Admin/Add
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddAdmin(AdminManagementViewModel model)
    {
        if (ModelState.IsValid)
        {
            // Check if the email is already taken
            if (await _context.Admins.AnyAsync(a => a.Email == model.Email))
            {
                ModelState.AddModelError(nameof(model.Email), "This email is already registered as an admin.");
                return View(model);
            }

            // Check if the password is strong
            if (!IsPasswordStrong(model.Password))
            {
                ModelState.AddModelError(nameof(model.Password), "The password must be at least 8 characters long, include uppercase and lowercase letters, a number, and a special character.");
                return View(model);
            }
            if (!(model.Password==model.ConfirmPassword))
            {
                ModelState.AddModelError(nameof(model.ConfirmPassword), "The password must be the same ");
                return View(model);
            }
            var newAdmin = new Admins
            {
                Email = model.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(model.Password)
            };
            newAdmin.Email = newAdmin.Email.Trim();
            _context.Admins.Add(newAdmin);
            await _context.SaveChangesAsync();

            return RedirectToAction("ViewAdmins");
        }

        return View(model);
    }

    // GET: Admin/EditAdmin/{email}
    public IActionResult EditAdmin(string email)
    {
        if (string.IsNullOrEmpty(email))
        {
            return NotFound();
        }

        var admin = _context.Admins.SingleOrDefault(a => a.Email == email);
        if (admin == null)
        {
            return NotFound();
        }

        var model = new AdminManagementViewModel
        {
            Email = admin.Email,
            Password = "", // Optionally set a default value or leave blank
            ConfirmPassword = ""
        };

        return View(model);
    }



    // POST: Admin/EditAdmin
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult EditAdmin(AdminManagementViewModel model)
    {
        if (ModelState.IsValid)
        {
            var admin = _context.Admins.SingleOrDefault(a => a.Email == model.Email);
            if (admin == null)
            {
                return NotFound();
            }

            // Update password if provided and confirmed
            if (!string.IsNullOrEmpty(model.Password))
            {
                if (model.Password != model.ConfirmPassword)
                {
                    ModelState.AddModelError("ConfirmPassword", "Passwords do not match.");
                    return View(model);
                }

                if (!IsPasswordStrong(model.Password))
                {
                    ModelState.AddModelError("Password", "The password must be at least 8 characters long, include uppercase and lowercase letters, a number, and a special character.");
                    return View(model);
                }

                admin.Password = BCrypt.Net.BCrypt.HashPassword(model.Password);
            }
            admin.Email = admin.Email.Trim();
            _context.Admins.Update(admin);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        return View(model);
    }

    // GET: Admin/Delete/{email}
    public async Task<IActionResult> DeleteAdmin(string email)
    {
        var admin = await _context.Admins.FindAsync(email);
        if (admin == null)
        {
            return NotFound();
        }

        var viewModel = new AdminManagementViewModel
        {
            Email = admin.Email
        };

        return View(viewModel);
    }

    // POST: Admin/Delete
    [HttpPost, ActionName("DeleteAdmin")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(string email)
    {
        var admin = await _context.Admins.FindAsync(email);
        if (admin == null)
        {
            return NotFound();
        }

        _context.Admins.Remove(admin);
        await _context.SaveChangesAsync();

        return RedirectToAction("ViewAdmins");
    }

    private bool IsPasswordStrong(string password)
    {
        if (password.Length < 8)
            return false;

        bool hasUpperCase = false;
        bool hasLowerCase = false;
        bool hasDigit = false;
        bool hasSpecialChar = false;

        foreach (var c in password)
        {
            if (char.IsUpper(c))
                hasUpperCase = true;
            else if (char.IsLower(c))
                hasLowerCase = true;
            else if (char.IsDigit(c))
                hasDigit = true;
            else if (!char.IsLetterOrDigit(c))
                hasSpecialChar = true;

            if (hasUpperCase && hasLowerCase && hasDigit && hasSpecialChar)
                return true;
        }

        return false;
    }
    // GET: Employees
    public async Task<IActionResult> Employees()
    {
        return View(await _context.Employees.ToListAsync());
    }

    // GET: Employees/Create
    public IActionResult CreateEmployee()
    {
        return View();
    }

    // POST: Employees/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateEmployee([Bind("Id,Name,Email,Password,Role")] Employee employee)
    {
        if (ModelState.IsValid)
        {
            employee.Password=BCrypt.Net.BCrypt.HashPassword(employee.Password);
            _context.Add(employee);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Employees));
        }
        return View(employee);
    }

    // GET: Employees/Edit/5
    public async Task<IActionResult> EditEmployee(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var employee = await _context.Employees.FindAsync(id);
        if (employee == null)
        {
            return NotFound();
        }
        return View(employee);
    }

    // POST: Employees/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditEmployee(int id, [Bind("Id,Name,Email,Password,Role")] Employee employee)
    {
        if (id != employee.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                employee.Password = BCrypt.Net.BCrypt.HashPassword(employee.Password);
                _context.Update(employee);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EmployeeExists(employee.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Employees));
        }
        return View(employee);
    }

    // GET: Employees/Delete/5
    public async Task<IActionResult> DeleteEmployee(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var employee = await _context.Employees
            .FirstOrDefaultAsync(m => m.Id == id);
        if (employee == null)
        {
            return NotFound();
        }

        return View(employee);
    }

    // POST: Employees/Delete/5
    [HttpPost, ActionName("DeleteEmployee")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteEmployeeConfirmed(int id)
    {
        var employee = await _context.Employees.FindAsync(id);
        if (employee != null)
        {
            _context.Employees.Remove(employee);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Employees));
    }
    private bool EmployeeExists(int id)
    {
        return _context.Employees.Any(e => e.Id == id);
    }
}
