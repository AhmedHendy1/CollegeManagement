namespace CollegeManagement.Models
{
    public class LoginViewModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string Role { get; set; } // Admin, Employee, or Student
    }
}
