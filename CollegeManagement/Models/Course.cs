using System;
using System.Collections.Generic;

namespace CollegeManagement.Models;

public partial class Course
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string Description { get; set; } = null!;
}
