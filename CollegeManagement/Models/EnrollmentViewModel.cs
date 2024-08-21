using System.Collections.Generic;

namespace CollegeManagement.Models
{
    public class EnrollmentViewModel
    {
        public int StudentId { get; set; }
        public IEnumerable<Course> AvailableCourses { get; set; } = new List<Course>();
        public List<int> SelectedCourseIds { get; set; } = new List<int>();
        public List<int> EnrolledCourses { get; set; } = new List<int>(); // Add this property
    }
}
