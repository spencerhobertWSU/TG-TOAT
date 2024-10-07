using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
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

            var course = _context.Courses
                .Include(c => c.Assignments)
                .FirstOrDefault(c => c.CourseId == id);

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
                    Assignments = course.Assignments.ToList()
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
            var course = _context.Courses
                .Include(c => c.Assignments)
                .FirstOrDefault(c => c.CourseId == id);

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
                    Assignments = course.Assignments.ToList()
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

        // Course list page in instructor view
        public IActionResult Courses()
        {
            var currentInstructorId = _auth.GetUser().Id;  

            if (currentInstructorId == 0)
            {
                return RedirectToAction("Index", "Home");  
            }

            // Fetch the courses that are linked to the current instructor
            var courses = _context.InstructorCourseConnection
                .Include(icc => icc.Course)                  
                .ThenInclude(c => c.Department)          
                .Where(icc => icc.InstructorID == currentInstructorId)  
                .Select(icc => icc.Course)                   
                .ToList();

            var currentUser = _auth.GetUser();  

            // Create the ViewModel that combines courses and user info
            var viewModel = new CourseListViewModel
            {
                Courses = courses,
                UserLoginViewModel = currentUser
            };

            return View(viewModel);
        }

        [HttpGet]
        public IActionResult AddAssignment()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddAssignment(int id, AddAssignmentViewModel model)
        {

          
            var userIdInt = _auth.GetCurrentUserId();


            var userCourses = await _context.InstructorCourseConnection
           .Where(uc => uc.InstructorID == userIdInt)
           .Select(uc => uc.Course)
           .ToListAsync();

            var InstructorCourseID = await _context.InstructorCourseConnection
          .Where(uc => uc.InstructorID == userIdInt && uc.CourseId == id)
          .Select(uc => uc.InstructorCourseConnectionId)
          .FirstOrDefaultAsync();

            if (ModelState.IsValid)
            {
                var assignment = new Assignment
                {
                    AssignmentName = model.AssignmentName,
                    AssignmentDescription = model.AssignmentDescription,
                    AssignmentPoints = model.AssignmentPoints,
                    DueDateAndTime = model.DueDateAndTime,
                    AssignmentType = model.AssignmentType,
                    InstructorCourseId = InstructorCourseID,
                    CourseId = id
                };

                
                
                    _context.Assignments.Add(assignment);
                    await _context.SaveChangesAsync();
                return RedirectToAction("Assignments", new { id });

            }

            return View(model);
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
            var dayMap = new Dictionary<string, int> 
            {
                { "Su", 0 },
                { "Mo", 1 },
                { "Tu", 2 },
                { "We", 3 },
                { "Th", 4 },
                { "Fr", 5 },
                { "Sa", 6 }
            };

            // instructor role
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
            // student role
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
                    // Add course events
                    AddCourseToEventList(course, dayMap, courseEvents);

                    // Add assignment events
                    var assignments = _context.Assignments
                        .Where(a => a.InstructorCourse.CourseId == course.CourseId)
                        .ToList();

                    foreach (var assignment in assignments)
                    {
                        courseEvents.Add(new
                        {
                            title = assignment.AssignmentName,
                            start = assignment.DueDateAndTime.ToString("yyyy-MM-ddTHH:mm:ss"),
                            url = $"/Assignments/View/{assignment.AssignmentId}"  // Link to assignment page
                        });
                    }
                }
            }

            // Return all course and assignment events
            return Json(courseEvents);
        }

        // Helper method to add course events to the list
        private void AddCourseToEventList(Courses course, Dictionary<string, int> dayMap, List<object> courseEvents)
        {
            if (course.StartTime.HasValue && course.EndTime.HasValue)
            {
                var startTime = course.StartTime.Value;
                var endTime = course.EndTime.Value;

                // Split the DaysOfTheWeek string by commas
                var days = course.DaysOfTheWeek.Split(',');

                // Set the default start and end dates for the semester
                string startRecur = string.Empty;
                string endRecur = string.Empty;

                // Determine the semester and set the correct recurrence dates
                if (course.Semester == "Fall")
                {
                    startRecur = "2024-08-26";
                    endRecur = "2024-12-13";
                }
                else if (course.Semester == "Spring")
                {
                    startRecur = "2025-01-26";
                    endRecur = "2025-04-25";
                }
                else if (course.Semester == "Summer")
                {
                    startRecur = "2025-05-05";
                    endRecur = "2025-08-15";
                }

                // Loop through the split days and create events
                foreach (string day in days)
                {
                    string trimmedDay = day.Trim();  // Trim any extra spaces

                    if (dayMap.ContainsKey(trimmedDay))
                    {
                        int dayCode = dayMap[trimmedDay];
                        courseEvents.Add(new
                        {
                            title = course.CourseName,
                            start = new DateTime(2024, 9, 1, startTime.Hour, startTime.Minute, startTime.Second).ToString("yyyy-MM-ddTHH:mm:ss"),
                            end = new DateTime(2024, 9, 1, endTime.Hour, endTime.Minute, endTime.Second).ToString("yyyy-MM-ddTHH:mm:ss"),
                            daysOfWeek = new[] { dayCode },
                            startRecur = startRecur,  // Use the start date for the semester
                            endRecur = endRecur       // Use the end date for the semester
                        });
                    }
                    else
                    {
                        Console.WriteLine($"Day {trimmedDay} not found in dayMap");  // Log any days not found in dayMap
                    }
                }
            }
        }        


        public IActionResult GetProfileImage()
        {
            var userLoginInfo = _auth.GetUser();
            var fullUser = _context.User.Find(userLoginInfo.Id);

            var imageBytes = Convert.FromBase64String("");


            if (fullUser == null || string.IsNullOrEmpty(fullUser.ProfileImageBase64))
            {
                imageBytes = Convert.FromBase64String("+IMQ3yEZtXwBVkKazXUlLCAZV4UKaXKsOMIc4olDFdJo/FbADOKRCZ6th3yFeOj4PqRBBA4hnvrwEFKvL11APHW9GjLxOOT2PqROCOYQT81XQ8RnDyG12pJ47H9INkNqhEB8JoT8BNEuPSTVExuQAAAAAElFTkSuQmCC");
            }
            else
            {

                imageBytes = Convert.FromBase64String(fullUser.ProfileImageBase64);

            }

            return File(imageBytes, "image/png");
        }

        


        [HttpGet]
        public IActionResult Delete(int id)
        {
            var course = _context.Courses.FirstOrDefault(c => c.CourseId == id);

            if (course == null)
            {
                return NotFound();
            }

            // Remove instructor-course connection
            var instructorConnection = _context.InstructorCourseConnection.FirstOrDefault(icc => icc.CourseId == id);
            if (instructorConnection != null)
            {
                _context.InstructorCourseConnection.Remove(instructorConnection);
            }

            // Remove the course
            _context.Courses.Remove(course);
            _context.SaveChanges();

            return RedirectToAction("Courses");
        }


    }
}
