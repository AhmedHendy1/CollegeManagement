using System;
using System.Collections.Generic;

namespace CollegeManagement.Models;

public partial class Student
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public DateTime DateOfBirth { get; set; }

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public int DepartmentId { get; set; }

    public virtual Department? Department { get; set; } = null!;
}
