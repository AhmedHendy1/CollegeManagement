namespace CollegeManagement.Models
{
    public class EmployeeDashboardViewModel
    {
        public List<Student> Students { get; set; } = new List<Student>();
        public List<Course> Courses { get; set; } = new List<Course>();
        public List<Department> Departments { get; set; } = new List<Department>();
    }
}
