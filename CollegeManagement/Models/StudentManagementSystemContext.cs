using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace CollegeManagement.Models;

public partial class StudentManagementSystemContext : DbContext
{
    public StudentManagementSystemContext()
    {
    }

    public StudentManagementSystemContext(DbContextOptions<StudentManagementSystemContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Course> Courses { get; set; }

    public virtual DbSet<Department> Departments { get; set; }

    public virtual DbSet<Employee> Employees { get; set; }

    public virtual DbSet<Student> Students { get; set; }

    public virtual DbSet<StudentCourse> StudentCourses { get; set; }
    public virtual DbSet<Admins> Admins { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)

        => optionsBuilder.UseSqlServer("Server=DESKTOP-RCQTT57;Database=StudentManagementSystem;Trusted_Connection=True;Encrypt=False");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Student>(entity =>
        {
            entity.HasIndex(e => e.DepartmentId, "IX_Students_DepartmentId");

            entity.HasOne(d => d.Department).WithMany(p => p.Students).HasForeignKey(d => d.DepartmentId);
        });

        modelBuilder.Entity<StudentCourse>(entity =>
        {
            entity.HasKey(e => new { e.Stdid, e.Crsid });

            entity.ToTable("StudentCourse");
        });
        modelBuilder.Entity<Admins>(entity =>
        {
            // Define single primary key
            entity.HasKey(e => e.Email);
            entity.HasData(new Admins { Email = "Admin@default.com", Password = BCrypt.Net.BCrypt.HashPassword("Admin123@") });
        });
        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}