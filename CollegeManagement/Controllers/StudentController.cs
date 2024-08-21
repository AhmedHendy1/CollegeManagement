using Microsoft.AspNetCore.Mvc;
using CollegeManagement.Models;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

[Authorize(Roles = "Student")]
public class StudentController : Controller
{
    private readonly StudentManagementSystemContext _context;

    public StudentController(StudentManagementSystemContext context)
    {
        _context = context;
    }

    // GET: Student/Index
    public async Task<IActionResult> Index()
    {
        var email = User.Identity?.Name;
        var student = await _context.Students
            .Include(s => s.Department)
            .FirstOrDefaultAsync(s => s.Email == email);

        // Fetch the course IDs associated with the student
        var courseIds = await _context.StudentCourses
            .Where(sc => sc.Stdid == student.Id)
            .Select(sc => sc.Crsid)
            .ToListAsync();

        // Fetch the course details based on the course IDs
        var courses = await _context.Courses
            .Where(c => courseIds.Contains(c.Id))
            .ToListAsync();

        var viewModel = new StudentCourseViewModel
        {
            Student = student,
            Courses = courses
        };

        return View(viewModel);
    }

    // GET: Student/Details
    public async Task<IActionResult> Details()
    {
        var email = User.Identity?.Name;
        var student = await _context.Students
            .Include(s => s.Department)
            .FirstOrDefaultAsync(s => s.Email == email);

        // Fetch the course IDs associated with the student
        var courseIds = await _context.StudentCourses
            .Where(sc => sc.Stdid == student.Id)
            .Select(sc => sc.Crsid)
            .ToListAsync();

        // Fetch the course details based on the course IDs
        var courses = await _context.Courses
            .Where(c => courseIds.Contains(c.Id))
            .ToListAsync();

        var viewModel = new StudentCourseViewModel
        {
            Student = student,
            Courses = courses
        };
        return View(viewModel);
    }

    // GET: Student/ChangePassword
    public IActionResult ChangePassword()
    {
        return View();
    }

    // POST: Student/ChangePassword
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
    {
        if (ModelState.IsValid)
        {
            var email = User.Identity.Name;
            var student = await _context.Students.FirstOrDefaultAsync(s => s.Email == email);

            if (student == null)
            {
                return NotFound();
            }

            // Verify the old password
            if (!BCrypt.Net.BCrypt.Verify(model.OldPassword, student.Password))
            {
                ModelState.AddModelError("", "Old password is incorrect.");
                return View(model);
            }

            // Update password if old password is correct
            student.Password = BCrypt.Net.BCrypt.HashPassword(model.NewPassword);
            _context.Update(student);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Password changed successfully.";
            return RedirectToAction(nameof(Index));
        }

        return View(model);
    }
}
