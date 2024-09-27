using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using TGTOAT.Data;
using TGTOAT.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Drawing.Printing;
using TGTOAT.Helpers;
using TGTOAT.ViewModels;

namespace TGTOAT.Controllers
{
    public class CourseController : Controller
    {
        private readonly UserContext _context;
        private readonly IAuthentication _authentication;

        public CourseController(UserContext context, IAuthentication authentication)
        {
            _context = context;
            _authentication = authentication;
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

        public IActionResult Courses()
        {            
            var courses = _context.Courses.Include(c => c.Department).ToList();            
            var currentUser = _authentication.CheckUser();

            // Create the ViewModel that combines courses and user info
            var viewModel = new CourseListViewModel
            {
                Courses = courses,
                UserLoginViewModel = currentUser
            };           
            return View(viewModel);  
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

        // get courses for calendar view
        [HttpGet]
        public JsonResult GetCoursesForCalendar()
        {
            string currentInstructorId = _authentication.GetCurrentUserId();

            if (string.IsNullOrEmpty(currentInstructorId))
            {
                return Json(new { error = "No instructor logged in." });
            }

            var courses = _context.InstructorCourseConnection
                .Include(icc => icc.Course)
                .Where(icc => icc.InstructorID == int.Parse(currentInstructorId))
                .Select(icc => icc.Course)
                .ToList();

            var courseEvents = new List<object>();
            var dayMap = new Dictionary<char, int> {
                { 'M', 1 },
                { 'T', 2 },
                { 'W', 3 },
                { 'R', 4 },  // Thursday
                { 'F', 5 }
            };

            foreach (var course in courses)
            {
                if (course.StartTime.HasValue && course.EndTime.HasValue)
                {
                    var startTime = course.StartTime.Value;
                    var endTime = course.EndTime.Value;

                    // Convert DaysOfTheWeek to uppercase to match dayMap keys
                    foreach (char day in course.DaysOfTheWeek.ToUpper())
                    {
                        if (dayMap.ContainsKey(day))
                        {
                            int dayCode = dayMap[day];
                            courseEvents.Add(new
                            {
                                title = course.CourseName,
                                start = new DateTime(2024, 9, 1, startTime.Hour, startTime.Minute, startTime.Second).ToString("yyyy-MM-ddTHH:mm:ss"),
                                end = new DateTime(2024, 9, 1, endTime.Hour, endTime.Minute, endTime.Second).ToString("yyyy-MM-ddTHH:mm:ss"),
                                daysOfWeek = new[] { dayCode },
                                startRecur = "2024-09-01",
                                endRecur = "2024-12-15"
                            });
                        }
                    }
                }
            }

            // Test event 
            courseEvents.Add(new
            {
                title = "Test Course",
                start = "2024-09-01T09:00:00",
                end = "2024-09-01T10:00:00",
                daysOfWeek = new[] { 1, 3, 5 },  // Monday, Wednesday, Friday
                startRecur = "2024-09-01",
                endRecur = "2024-12-15"
            });

            return Json(courseEvents);
        }

    }
}
