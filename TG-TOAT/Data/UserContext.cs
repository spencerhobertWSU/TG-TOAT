using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TGTOAT.Models;

namespace TGTOAT.Data
{
    public class UserContext : DbContext
    {
        public UserContext (DbContextOptions<UserContext> options)
            : base(options)
        {
        }
        // User entity
        public DbSet<TGTOAT.Models.User> User { get; set; } = default!;

        public DbSet<Departments> Departments { get; set; } = default!;
        public DbSet<Courses> Courses { get; set; } = default!;
        public DbSet<Address> Address { get; set; } = default!;
        public DbSet<UserCourseConnection> UserCourseConnection { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Seed dummy data
            modelBuilder.Entity<Departments>().HasData(
                new Departments { DepartmentId = 1, DepartmentName = "Computer Science" },
                new Departments { DepartmentId = 2, DepartmentName = "Mathematics" },
                new Departments { DepartmentId = 3, DepartmentName = "Physics" },
                new Departments { DepartmentId = 4, DepartmentName = "Biology" },
                new Departments { DepartmentId = 5, DepartmentName = "Chemistry" }
            );
        }
    }


}
