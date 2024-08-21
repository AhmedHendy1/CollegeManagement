using System.Collections.Generic;

namespace CollegeManagement.Models
{
    public class AdminManagementViewModel
    {
        // Properties for adding and editing an admin
        public string Email { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }

        // List of admins for display and deletion
        public List<Admins> Admins { get; set; } = new List<Admins>();
    }
}
