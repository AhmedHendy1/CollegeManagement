using CollegeManagement.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CollegeManagement.Controllers
{
    [Authorize(Roles = "Employee")]
    public class EmployeeController : Controller
    {
        private readonly StudentManagementSystemContext _context;
        public EmployeeController(StudentManagementSystemContext context)
        {
            _context = context;
        }
        // GET: Courses
        public async Task<IActionResult> Courses()
        {
            return View(await _context.Courses.ToListAsync());
        }

        // GET: Courses/Create
        public IActionResult CreateCourse()
        {
            return View();
        }

        // POST: Courses/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCourse([Bind("Id,Title,Description")] Course course)
        {
            if (ModelState.IsValid)
            {
                _context.Add(course);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Courses));
            }
            return View(course);
        }

        // GET: Courses/Edit/5
        public async Task<IActionResult> EditCourse(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = await _context.Courses.FindAsync(id);
            if (course == null)
            {
                return NotFound();
            }
            return View(course);
        }

        // POST: Courses/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCourse(int id, [Bind("Id,Title,Description")] Course course)
        {
            if (id != course.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(course);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CourseExists(course.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Courses));
            }
            return View(course);
        }

        // GET: Courses/Delete/5
        public async Task<IActionResult> DeleteCourse(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = await _context.Courses
                .FirstOrDefaultAsync(m => m.Id == id);
            if (course == null)
            {
                return NotFound();
            }

            return View(course);
        }

        // POST: Courses/Delete/5
        [HttpPost, ActionName("DeleteCourse")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCourseConfirmed(int id)
        {
            // Find the course to delete
            var course = await _context.Courses.FindAsync(id);
            if (course == null)
            {
                return NotFound();
            }

            // Check if there are any students assigned to the course
            var studentCourses = _context.StudentCourses.Any(sc => sc.Crsid == id);
            if (studentCourses)
            {
                // Add a model state error if students are assigned
                ModelState.AddModelError("", "Cannot delete the course because it has students assigned to it.");
                return View("DeleteCourse", course); // Return to the Delete view with the course details
            }

            // Remove the course
            _context.Courses.Remove(course);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Courses));
        }

        private bool CourseExists(int id)
        {
            return _context.Courses.Any(e => e.Id == id);
        }
        // GET: Students
        public async Task<IActionResult> Students()
        {
            var students = await _context.Students.Include(s => s.Department).ToListAsync();
            return View(students);
        }

        // GET: Students/Create
        public IActionResult CreateStudent()
        {
            ViewData["DepartmentId"] = new SelectList(_context.Departments, "Id", "Name");
            return View();
        }

        // POST: Students/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateStudent([Bind("Id,Name,DateOfBirth,Email,Password,DepartmentId")] Student student)
        {
            if (ModelState.IsValid)
            {
                // Ensure the DepartmentId is valid
                if (!_context.Departments.Any(d => d.Id == student.DepartmentId))
                {
                    ModelState.AddModelError("DepartmentId", "Invalid Department.");
                    ViewData["DepartmentId"] = new SelectList(_context.Departments, "Id", "Name", student.DepartmentId);
                    return View(student);
                }

                // Ensure the Email is unique
                if (_context.Students.Any(s => s.Email == student.Email))
                {
                    ModelState.AddModelError("Email", "An account with this email already exists.");
                    ViewData["DepartmentId"] = new SelectList(_context.Departments, "Id", "Name", student.DepartmentId);
                    return View(student);
                }
                student.Password= BCrypt.Net.BCrypt.HashPassword(student.Password);
                _context.Add(student);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Students));
            }

            ViewData["DepartmentId"] = new SelectList(_context.Departments, "Id", "Name", student.DepartmentId);
            return View(student);
        }



        // GET: Students/Edit/5
        public async Task<IActionResult> EditStudent(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Students
                .Include(s => s.Department) // Ensure you include the Department if needed
                .FirstOrDefaultAsync(m => m.Id == id);
            if (student == null)
            {
                return NotFound();
            }

            ViewData["DepartmentId"] = new SelectList(_context.Departments, "Id", "Name", student.DepartmentId);
            return View(student);
        }

        // POST: Students/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditStudent(int id, [Bind("Id,Name,DateOfBirth,Email,Password,DepartmentId")] Student student)
        {
            if (id != student.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Ensure the Email is unique
                    if (_context.Students.Any(s => s.Email == student.Email && s.Id != student.Id))
                    {
                        ModelState.AddModelError("Email", "An account with this email already exists.");
                        ViewData["DepartmentId"] = new SelectList(_context.Departments, "Id", "Name", student.DepartmentId);
                        return View(student);
                    }
                    student.Password = BCrypt.Net.BCrypt.HashPassword(student.Password);
                    _context.Update(student);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!(StudentExists(student.Id)))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Students));
            }

            ViewData["DepartmentId"] = new SelectList(_context.Departments, "Id", "Name", student.DepartmentId);
            return View(student);
        }

        private bool StudentExists(int id)
        {
            return _context.Students.Any(e => e.Id == id);
        }



        // GET: Students/Delete/5
        public async Task<IActionResult> DeleteStudent(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Students
                .Include(s => s.Department)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }

        // POST: Students/Delete/5
        [HttpPost, ActionName("DeleteStudent")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteStudentConfirmed(int id)
        {
            // Find the student to delete
            var student = await _context.Students.FindAsync(id);
            if (student == null)
            {
                return NotFound();
            }

            // Remove related StudentCourse records
            var studentCourses = _context.StudentCourses.Where(sc => sc.Stdid == id);
            _context.StudentCourses.RemoveRange(studentCourses);

            // Remove the student
            _context.Students.Remove(student);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Students));
        }
        public async Task<IActionResult> DetailsStudent(int id)
        {

            var student = _context.Students
       .Include(s => s.Department)  // Include department information if needed
       .Where(s => s.Id == id)
       .FirstOrDefault(); // Synchronous method

            if (student == null)
            {
                return NotFound();
            }

            // Fetch the course IDs associated with the student
            var courseIds = await _context.StudentCourses
                .Where(sc => sc.Stdid == id)
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

        // GET: Students/EnrollCourses/5
        public async Task<IActionResult> EnrollCourses(int id)
        {
            var student = await _context.Students
                .FirstOrDefaultAsync(s => s.Id == id);

            if (student == null)
            {
                return NotFound();
            }

            var availableCourses = await _context.Courses.ToListAsync();
            var enrolledCourses = await _context.StudentCourses
                .Where(sc => sc.Stdid == id)
                .Select(sc => sc.Crsid)
                .ToListAsync();

            var viewModel = new EnrollmentViewModel
            {
                StudentId = id,
                AvailableCourses = availableCourses,
                EnrolledCourses = enrolledCourses
            };

            return View(viewModel);
        }


        // POST: Students/EnrollCourses
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EnrollCourses(EnrollmentViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                foreach (var courseId in viewModel.SelectedCourseIds)
                {
                    // Check if the enrollment already exists
                    var existingEnrollment = await _context.StudentCourses
                        .AnyAsync(sc => sc.Stdid == viewModel.StudentId && sc.Crsid == courseId);

                    if (!existingEnrollment)
                    {
                        var enrollment = new StudentCourse
                        {
                            Stdid = viewModel.StudentId,
                            Crsid = courseId
                        };

                        _context.StudentCourses.Add(enrollment);
                    }
                }

                await _context.SaveChangesAsync();
                return RedirectToAction("Students");
            }

            // Reload available courses if model state is not valid
            viewModel.AvailableCourses = await _context.Courses.ToListAsync();
            return View(viewModel);
        }
        
        public async Task<IActionResult> Departments()
        {
            return View(await _context.Departments.ToListAsync());
        }

        // GET: Departments/Create
        public IActionResult CreateDepartment()
        {
            return View();
        }

        // POST: Departments/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateDepartment([Bind("Id,Name")] Department department)
        {
            if (ModelState.IsValid)
            {
                _context.Add(department);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Departments));
            }
            return View(department);
        }

        // GET: Departments/Edit/5
        public async Task<IActionResult> EditDepartment(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var department = await _context.Departments.FindAsync(id);
            if (department == null)
            {
                return NotFound();
            }
            return View(department);
        }

        // POST: Departments/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditDepartment(int id, [Bind("Id,Name")] Department department)
        {
            if (id != department.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(department);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DepartmentExists(department.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Departments));
            }
            return View(department);
        }
        // GET: Departments/Delete/5
        public async Task<IActionResult> DeleteDepartment(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var department = await _context.Departments
                .Include(d => d.Students) // Include related students
                .FirstOrDefaultAsync(m => m.Id == id);

            if (department == null)
            {
                return NotFound();
            }

            // Display the department details for confirmation
            return View(department);
        }

        // POST: Departments/Delete/5
        [HttpPost, ActionName("DeleteDepartment")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteDepartmentConfirmed(int id)
        {
            var department = await _context.Departments
                .Include(d => d.Students) // Include related students
                .FirstOrDefaultAsync(m => m.Id == id);

            if (department != null)
            {
                if (department.Students.Any()) // Check if there are students in the department
                {
                    // Redirect to a separate error view if there are students
                    TempData["ErrorMessage"] = "Cannot delete department. There are students enrolled in this department.";
                    return RedirectToAction(nameof(DeleteDepartment), new { id = id });
                }

                _context.Departments.Remove(department);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Departments));
            }

            return NotFound();
        }

        private bool DepartmentExists(int id)
        {
            return _context.Departments.Any(e => e.Id == id);
        }
        public IActionResult Index()
        {
            var viewModel = new EmployeeDashboardViewModel
            {
                Students = _context.Students.ToList(),
                Courses = _context.Courses.ToList(),
                Departments = _context.Departments.ToList()
            };
            return View(viewModel);
        }

    }

}
