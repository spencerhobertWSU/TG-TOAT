using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TGTOAT.Data;
using TGTOAT.Helpers;
using TGTOAT.Models;
using TGTOAT.ViewModels;

namespace TGTOAT.Controllers
{
    public class CourseController : Controller
    {
        private readonly UserContext _context;
        private readonly IAuthentication _auth;

        public CourseController(UserContext context, IAuthentication auth)
        {
            _context = context;
            _auth = auth;
        }

        // Display the AddCourse view with the list of departments
        [HttpGet]
        public IActionResult Create()
        {
            var departments = _context.Departments.ToList();
            var semesters = new List<SelectListItem>
            {
                new SelectListItem { Value = "Spring", Text = "Spring" },
                new SelectListItem { Value = "Summer", Text = "Summer" },
                new SelectListItem { Value = "Fall", Text = "Fall" }
            };
            var campus = new List<SelectListItem>
            {
                new SelectListItem { Value = "Ogden", Text = "Ogden Campus" },
                new SelectListItem { Value = "Davis", Text = "Davis Campus" }
            };


            var instructors = _context.User.Where(u => u.UserRole == "Instructor").ToList();

            var UserRole = _auth.GetRole();

            var viewModel = new AddCourseViewModel
            {
                UserRole = UserRole,
                Departments = departments,
                CampusList = campus,
                SemesterList = semesters,
                CurrYear = DateTime.Now.Year,
                Instructors = instructors
            };
            return View(viewModel);
        }

        //[ActionName("Index")]
        public ActionResult Index(int? id)
        {
            var user = _auth.GetUser();

            if (user == null)
            {
                return RedirectToAction("Login", "User");
            }

            var course = _context.Courses.FirstOrDefault(c => c.CourseId == id);
            var dept = _context.Departments.FirstOrDefault(d => d.DepartmentId == course.DepartmentId);
            if (course == null)
            {
                return Redirect("User/Index");
            }
            else
            {
                var viewModel = new CourseHome
                {
                    CourseId = course.CourseId,
                    UserRole = _auth.GetRole(),
                    Department = dept.DepartmentName,
                    CourseNum = course.CourseNumber,
                };
                return View(viewModel);
            }

        }

        public ActionResult Assignments(int? id)
        {
            if (_auth.GetUser == null)
            {
                return Redirect("User/Login");
            }
            var course = _context.Courses.FirstOrDefault(c => c.CourseId == id);
            var dept = _context.Departments.FirstOrDefault(d => d.DepartmentId == course.DepartmentId);
            if (course == null)
            {
                return Redirect("User/Index");
            }
            else
            {
                var viewModel = new CourseHome
                {
                    CourseId = course.CourseId,
                    UserRole = _auth.GetRole(),
                    Department = dept.DepartmentName,
                    CourseNum = course.CourseNumber,
                };
                return View(viewModel);
            }

        }

        // Handle the form submission for adding a course
        [HttpPost]
        public async Task<IActionResult> Create(AddCourseViewModel model)
        {
            if (ModelState.IsValid)
            {
                var course = new Courses
                {
                    CourseNumber = model.CourseNumber,
                    CourseName = model.CourseName,
                    DepartmentId = model.SelectedDepartmentId,
                    Capacity = model.Capacity,
                    Campus = model.Campus,
                    Building = model.Building,
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
            model.CampusList = new List<SelectListItem>
            {
                new SelectListItem { Value = "Ogden", Text = "Ogden Campus" },
                new SelectListItem { Value = "Davis", Text = "Davis Campus" }
            };
            model.Instructors = _context.User.Where(u => u.UserRole == "Instructor").ToList();
            return View(model);
        }       

        public IActionResult Courses()
        {            
            var courses = _context.Courses.Include(c => c.Department).ToList();            
            var currentUser = _auth.GetUser();

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

        [HttpGet]
        public JsonResult GetCoursesForCalendar()
        {
            var currentUser = _auth.GetUser();  // Get the current user's details
            var userIdString = currentUser.ToString();

            if (currentUser == null || string.IsNullOrEmpty(userIdString))
            {
                return Json(new { error = "No user logged in." });
            }

            var courseEvents = new List<object>();
            var dayMap = new Dictionary<char, int> {
                { 'M', 1 },
                { 'T', 2 },
                { 'W', 3 },
                { 'R', 4 },  // Thursday
                { 'F', 5 }
            };

            if (currentUser.UserRole == "Instructor")
            {
                // Fetch courses for the instructor
                var instructorCourses = _context.InstructorCourseConnection
                    .Include(icc => icc.Course)
                    .Where(icc => icc.InstructorID == currentUser.Id)
                    .Select(icc => icc.Course)
                    .ToList();

                foreach (var course in instructorCourses)
                {
                    AddCourseToEventList(course, dayMap, courseEvents);
                }
            }
            else if (currentUser.UserRole == "Student")
            {
                // Fetch courses for the student
                var studentCourses = _context.StudentCourseConnection
                    .Include(scc => scc.Course)
                    .Where(scc => scc.StudentID == currentUser.Id)
                    .Select(scc => scc.Course)
                    .ToList();

                foreach (var course in studentCourses)
                {
                    AddCourseToEventList(course, dayMap, courseEvents);
                }
            }

            return Json(courseEvents);
        }

        // Helper method to add course events to the list
        private void AddCourseToEventList(Courses course, Dictionary<char, int> dayMap, List<object> courseEvents)
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

    }
}
