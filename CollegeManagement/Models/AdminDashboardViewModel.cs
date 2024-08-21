using System.Collections.Generic;

namespace CollegeManagement.Models
{
    public class AdminDashboardViewModel
    {
        // Lists for managing employees and admins
        public List<Employee> Employees { get; set; } = new List<Employee>();
        public List<Admins> Admins { get; set; } = new List<Admins>();
   
    }
}
