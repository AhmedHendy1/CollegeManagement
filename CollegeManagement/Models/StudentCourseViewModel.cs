using System.Collections.Generic;

namespace CollegeManagement.Models
{
    public class StudentCourseViewModel
    {
        public Student Student { get; set; } = null!;
        public List<Course> Courses { get; set; } = new List<Course>();
    }
}
