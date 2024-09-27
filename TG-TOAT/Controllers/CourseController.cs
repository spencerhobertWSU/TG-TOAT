using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using TGTOAT.Data;
using TGTOAT.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Drawing.Printing;

namespace TGTOAT.Controllers
{
    public class CourseController : Controller
    {
        private readonly UserContext _context;

        public CourseController(UserContext context)
        {
            _context = context;
        }

        // Display the AddCourse view with the list of departments
        [HttpGet]
        public IActionResult AddCourse()
        {
            var departments = _context.Departments.ToList();
            var semesters = new List<SelectListItem>
            {
                new SelectListItem { Value = "Spring", Text = "Spring" },
                new SelectListItem { Value = "Summer", Text = "Summer" },
                new SelectListItem { Value = "Fall", Text = "Fall" }
            };
            var instructors = _context.User.Where(u => u.UserRole == "Instructor").ToList();

            var viewModel = new AddCourseViewModel
            {
                Departments = departments,
                SemesterList = semesters,
                Instructors = instructors
            };

            return View(viewModel);
        }

        // Handle the form submission for adding a course
        [HttpPost]
        public async Task<IActionResult> AddCourse(AddCourseViewModel model)
        {
            if (ModelState.IsValid)
            {
                var course = new Courses
                {
                    CourseNumber = model.CourseNumber,
                    CourseName = model.CourseName,
                    DepartmentId = model.SelectedDepartmentId,
                    Capacity = model.Capacity,
                    Location = model.Location,
                    RoomNumber = model.RoomNumber,
                    DaysOfTheWeek = model.DaysOfTheWeek,
                    StartTime = model.StartTime,
                    EndTime = model.EndTime,
                    NumberOfCredits = model.NumberOfCredits,
                    CourseDescription = model.CourseDescription,
                    Semester = model.Semester,
                    Year = model.Year
                };

                _context.Courses.Add(course);
                await _context.SaveChangesAsync();

                var instructorCourseConnection = new InstructorCourseConnection
                {
                    InstructorID = model.SelectedInstructorId,
                    CourseId = course.CourseId
                };

                _context.InstructorCourseConnection.Add(instructorCourseConnection);
                await _context.SaveChangesAsync();

                return RedirectToAction("Courses");
            }

            // Re-populate if the model is invalid
            model.Departments = _context.Departments.ToList();
            model.SemesterList = new List<SelectListItem>
            {
                new SelectListItem { Value = "Spring", Text = "Spring" },
                new SelectListItem { Value = "Summer", Text = "Summer" },
                new SelectListItem { Value = "Fall", Text = "Fall" }
            };
            model.Instructors = _context.User.Where(u => u.UserRole == "Instructor").ToList();
            return View(model);
        }

        // Placeholder for course listing page
        public IActionResult Courses()
        {
            var courses = _context.Courses.Include(c => c.Department).ToList();
            return View(courses); // This will look for Views/Course/Courses.cshtml
        }
        public IActionResult ViewCourse(int id)
        {
            var course = _context.Courses.Include(c => c.Department).FirstOrDefault(c => c.CourseId == id);
            if (course == null)
            {
                return NotFound(); // Handle case where course is not found
            }
            return View(course);
        }
    }
}
