﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel;
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
        private readonly NotificationService _notificationService;

        public CourseController(NotificationService notificationService, UserContext context, IAuthentication auth)
        {
            _context = context;
            _auth = auth;
            _notificationService = notificationService;
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
            var user = _auth.GetUser();//Grab User Info
            int userId = user.Id;

            var viewModel = new AddCourseViewModel
            {
                Notifications = _notificationService.GetNotificationsForUser(userId).ToList(),
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
                    Notifications = _notificationService.GetNotificationsForUser(user.Id).ToList(),
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
            var user = _auth.GetUser();
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
                    Notifications = _notificationService.GetNotificationsForUser(user.Id).ToList(),
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
                var user = _auth.GetUser();//Grab User Info
                int userId = user.Id;
                var course = new Courses
                {
                    Notifications = _notificationService.GetNotificationsForUser(userId).ToList(),
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
            var currInstruct = _auth.GetUser();

            if (currInstruct == null)
            {
                return RedirectToAction("Login", "User");
            }
            var user = _auth.GetUser();//Grab User Info
            int userId = user.Id;
            // Fetch the courses that are linked to the current instructor
            var courses = _context.InstructorCourseConnection
                .Include(icc => icc.Course)
                .ThenInclude(c => c.Department)
                .Where(icc => icc.InstructorID == currInstruct.Id)
                .Select(icc => icc.Course)
                .ToList();

            var currentUser = _auth.GetUser();

            // Create the ViewModel that combines courses and user info
            var viewModel = new CourseListViewModel
            {
                Notifications = _notificationService.GetNotificationsForUser(userId).ToList(),
                Courses = courses,
                UserLoginViewModel = currentUser
            };

            return View(viewModel);
        }

        [HttpGet]

        //public ActionResult Assignments(int? id)
        public ActionResult AddAssignment(int? id)
        {

            var user = _auth.GetUser();//Grab User Info
            int userId = user.Id;


                var viewModel = new AddAssignmentViewModel
                {
                    Notifications = _notificationService.GetNotificationsForUser(userId).ToList(),


                };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddAssignment(int id, AddAssignmentViewModel model) 
        {
              var user = _auth.GetUser();//Grab User Info
            int userId = user.Id;

            var userIdInt = _auth.GetCurrentUserId();
            var ClassName = _context.Courses
          .Where(uc => uc.CourseId == id)
          .Select(uc => uc.CourseName)
         .FirstOrDefault();

            string CourseName = ClassName.ToString();

            var userCourses = await _context.InstructorCourseConnection
           .Where(uc => uc.InstructorID == userIdInt)
           .Select(uc => uc.Course)
           .ToListAsync();

            var InstructorCourseID = await _context.InstructorCourseConnection
          .Where(uc => uc.InstructorID == userIdInt && uc.CourseId == id)
          .Select(uc => uc.InstructorCourseConnectionId)
          .FirstOrDefaultAsync();

       

         
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
                var message = $"{CourseName}: New assignment '{assignment.AssignmentName}' has been created.";
                NotifyStudents(message, id);
                return RedirectToAction("Assignments", new { id });

          

            return View(model);
        }
        private void NotifyInstructors(string message, int CourseId)
        {

            var stuentIds = _context.InstructorCourseConnection
            .Where(uc => uc.CourseId == CourseId)
            .Select(uc => uc.InstructorID)
            .ToList();

            foreach (var studentId in stuentIds)
            {
                _notificationService.CreateNotification(message, studentId);
            }
        }
        private void NotifyStudents(string message, int CourseId)
        {

            var stuentIds = _context.StudentCourseConnection
            .Where(uc => uc.CourseId == CourseId)
            .Select(uc => uc.StudentID)
            .ToList();

            foreach (var studentId in stuentIds)
            {
                _notificationService.CreateNotification(message, studentId);
            }
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
                            url = $"/Course/SubmitPage?assignmentId={assignment.AssignmentId}"
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


        public ActionResult Edit(int id)
        {
            var course = _context.Courses.FirstOrDefault(c => c.CourseId == id);
            if (course == null)
            {
                return NotFound();
            }
            var user = _auth.GetUser();//Grab User Info
            int userId = user.Id;

            var viewModel = new AddCourseViewModel
            {
                Notifications = _notificationService.GetNotificationsForUser(userId).ToList(),
                CourseId = course.CourseId,
                CourseNumber = course.CourseNumber,
                CourseName = course.CourseName,
                NumberOfCredits = course.NumberOfCredits,
                SelectedDepartmentId = course.DepartmentId,
                Departments = _context.Departments?.ToList() ?? new List<Departments>(),
                CampusList = new List<SelectListItem>
                {
                    new SelectListItem { Value = "Ogden", Text = "Ogden Campus" },
                    new SelectListItem { Value = "Davis", Text = "Davis Campus" }
                },
                Building = course.Building,  // This ensures the selected building is set
                Buildings = GetBuildingListByCampus(course.Campus),  // Prepopulate buildings based on current campus
                CourseDescription = course.CourseDescription,
                DaysOfTheWeek = course.DaysOfTheWeek,
                StartTime = course.StartTime,
                EndTime = course.EndTime,
                RoomNumber = course.RoomNumber,
                Capacity = course.Capacity,
                Campus = course.Campus,
                Semester = course.Semester,
                SemesterList = new List<SelectListItem>
                {
                    new SelectListItem { Value = "Spring", Text = "Spring" },
                    new SelectListItem { Value = "Summer", Text = "Summer" },
                    new SelectListItem { Value = "Fall", Text = "Fall" }
                },
                Year = course.Year,
                SelectedInstructorId = _context.InstructorCourseConnection.FirstOrDefault(icc => icc.CourseId == id)?.InstructorID ?? 0,
                Instructors = _context.User?.Where(u => u.UserRole == "Instructor").ToList() ?? new List<User>()
            };

            return View(viewModel);
        }       

        [HttpPost]
        public async Task<ActionResult> Edit(AddCourseViewModel model)
        {                

            if (ModelState.IsValid)
            {
                try
                {
                    var course = await _context.Courses.FindAsync(model.CourseId);
                    if (course == null)
                    {
                        Console.WriteLine("Course not found");
                        return NotFound();
                    }

                    // Update course properties
                    course.Notifications = model.Notifications;
                    course.DepartmentId = model.SelectedDepartmentId;
                    course.CourseNumber = model.CourseNumber;
                    course.CourseName = model.CourseName;
                    course.CourseDescription = model.CourseDescription;
                    course.NumberOfCredits = model.NumberOfCredits;
                    course.Capacity = model.Capacity;
                    course.Campus = model.Campus;
                    course.Building = model.Building;
                    course.RoomNumber = model.RoomNumber;
                    course.DaysOfTheWeek = model.DaysOfTheWeek;
                    course.StartTime = model.StartTime;
                    course.EndTime = model.EndTime;
                    course.Semester = model.Semester;
                    course.Year = model.Year;

                    _context.Update(course);
                    await _context.SaveChangesAsync();
                    Console.WriteLine("Course updated successfully");

                    // Update instructor connection
                    var instructorConnection = await _context.InstructorCourseConnection
                        .FirstOrDefaultAsync(icc => icc.CourseId == course.CourseId);

                    if (instructorConnection != null)
                    {
                        if (instructorConnection.InstructorID != model.SelectedInstructorId)
                        {
                            instructorConnection.InstructorID = model.SelectedInstructorId;
                            _context.Update(instructorConnection);
                            await _context.SaveChangesAsync();
                            Console.WriteLine("Instructor connection updated");
                        }
                    }
                    else
                    {
                        var newInstructorConnection = new InstructorCourseConnection
                        {
                            InstructorID = model.SelectedInstructorId,
                            CourseId = course.CourseId
                        };
                        _context.InstructorCourseConnection.Add(newInstructorConnection);
                        await _context.SaveChangesAsync();
                        Console.WriteLine("New instructor connection created");
                    }

                    return RedirectToAction(nameof(Courses));
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error occurred: {ex.Message}");
                    ModelState.AddModelError("", "An error occurred while saving the changes.");
                }
            }

            // If we got this far, something failed, redisplay form
            model.Departments = await _context.Departments.ToListAsync();
            model.CampusList = new List<SelectListItem>
            {
                new SelectListItem { Value = "Ogden", Text = "Ogden Campus" },
                new SelectListItem { Value = "Davis", Text = "Davis Campus" }
            };
            model.Buildings = GetBuildingListByCampus(model.Campus);
            model.SemesterList = new List<SelectListItem>
            {
                new SelectListItem { Value = "Spring", Text = "Spring" },
                new SelectListItem { Value = "Summer", Text = "Summer" },
                new SelectListItem { Value = "Fall", Text = "Fall" }
            };
            model.Instructors = await _context.User.Where(u => u.UserRole == "Instructor").ToListAsync();
            
            return View(model);
        }



        [HttpGet]
        private List<SelectListItem> GetBuildingListByCampus(string campus)
        {
            var buildings = new List<SelectListItem>();

            if (campus == "Ogden")
            {
                buildings = new List<SelectListItem>
                {
                    new SelectListItem { Value = "", Text = "Select a Building" },
                    new SelectListItem { Value = "BC", Text = "Browing Center" },
                    new SelectListItem { Value = "EH", Text = "Elizabeth Hall" },
                    new SelectListItem { Value = "ET", Text = "Engineering Technology" },
                    new SelectListItem { Value = "HC", Text = "Hurst Center For Lifelong Learning" },
                    new SelectListItem { Value = "IE", Text = "Interprofessional Education Building" },
                    new SelectListItem { Value = "KA", Text = "Kimball Visual Arts Center" },
                    new SelectListItem { Value = "LP", Text = "Lampros Hall" },
                    new SelectListItem { Value = "LL", Text = "Lind Lecture Hall" },
                    new SelectListItem { Value = "LH", Text = "Lindquist Hall" },
                    new SelectListItem { Value = "MH", Text = "Marriott Health Services" },
                    new SelectListItem { Value = "ED", Text = "McKay Education" },
                    new SelectListItem { Value = "NB", Text = "Noorda Engineering, Applied Science & Technology" },
                    new SelectListItem { Value = "HB", Text = "Noorda High Bay" },
                    new SelectListItem { Value = "SU", Text = "Shepherd Union" },
                    new SelectListItem { Value = "LI", Text = "Steward Library" },
                    new SelectListItem { Value = "SC", Text = "Student Services Center" },
                    new SelectListItem { Value = "SW", Text = "Swenson Building" },
                    new SelectListItem { Value = "TY", Text = "Tracey Hall Science Center" },
                    new SelectListItem { Value = "WB", Text = "Wattis Building" }
                };
            }
            else if (campus == "Davis")
            {
                buildings = new List<SelectListItem>
                {
                    new SelectListItem { Value = "", Text = "Select a Building" },
                    new SelectListItem { Value = "D2", Text = "Building D2" },
                    new SelectListItem { Value = "D13", Text = "Building D13" },
                    new SelectListItem { Value = "DSC", Text = "Stewart Center" },
                    new SelectListItem { Value = "CCE", Text = "Center for Continuing Education" },
                    new SelectListItem { Value = "CAE", Text = "Computer & Automotive Engineering" }
                };
            }

            return buildings;
        }

        [HttpGet]
        public JsonResult GetBuildingsByCampus(string campus)
        {
            var buildings = GetBuildingListByCampus(campus);  // Reuse the helper method
            return Json(buildings);  // Return the list as JSON for AJAX
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


        // Smegma

        public ActionResult ViewSubmissions(int assignmentId)
        {
            var submissions = _context.StudentAssignment
                .Include(sa => sa.Assignments)
                .Where(sa => sa.AssignmentId == assignmentId)
                .ToList();

            var viewModel = submissions.Select(sa => new StudentSubmissionViewModel
            {
                StudentFullName = _context.User
                    .Where(u => u.Id == sa.StudentId)
                    .Select(u => u.FullName)
                    .FirstOrDefault() ?? "Unknown",

                SubmissionDate = sa.SubmissionDate,
                MaxPoints = sa.Assignments?.AssignmentPoints ?? 0,
                GivenPoints = sa.Grade.HasValue ? sa.Grade.Value.ToString() : "UG",
                AssignmentId = sa.AssignmentId, 
                StudentId = sa.StudentId          
            }).ToList();

            return View(viewModel);
        }




        public ActionResult ViewSubmission(int assignmentId, int studentId)
        {
            var assignment = _context.Assignments
                .FirstOrDefault(a => a.AssignmentId == assignmentId);

            var submission = _context.StudentAssignment
                .Include(sa => sa.studentCourseConnection)
                .FirstOrDefault(sa => sa.AssignmentId == assignmentId && sa.StudentId == studentId);

            if (assignment == null || submission == null)
            {
                return NotFound();
            }

            var user = _context.User
                .FirstOrDefault(u => u.Id == submission.StudentId);

            if (user == null)
            {
                return NotFound();
            }

            var viewModel = new List<SubmissionDetailViewModel>
    {
        new SubmissionDetailViewModel
        {
            StudentFullName = user.FullName,
            DueDate = assignment.DueDateAndTime,
            SubmissionDate = submission.SubmissionDate,
            MaxPoints = assignment.AssignmentPoints,
            GivenPoints = submission.Grade.HasValue ? submission.Grade.Value.ToString() : "UG",
            TextSubmission = submission.TextSubmission,
            FileSubmission = submission.FileSubmission,
            HasFile = !string.IsNullOrEmpty(submission.FileSubmission),
            AssignmentId = assignment.AssignmentId, // Include the assignment ID
            StudentId = submission.StudentId // Include the student ID
        }
    };

            return View(viewModel);
        }





        [HttpPost]
        public ActionResult UpdateSubmission(int assignmentId, int studentId, string givenPoints)
        {
           
            var submission = _context.StudentAssignment
                .FirstOrDefault(sa => sa.AssignmentId == assignmentId && sa.StudentId == studentId);

            if (submission != null)
            {
                if (int.TryParse(givenPoints, out int grade))
                {
                    // Submit the assignment grade
                    submission.Grade = grade;
                    _context.SaveChanges();
                    
                    // Update the students course grade
                    var assignment = _context.Assignments
                        .FirstOrDefault(a => a.AssignmentId == assignmentId);
                    UpdateGrade(studentId, assignment.CourseId);

                    var ClassName = _context.Courses
                    .Where(uc => uc.CourseId == assignment.CourseId)
                    .Select(uc => uc.CourseName)
                    .FirstOrDefault();

                    string CourseName = ClassName.ToString();

                    var message = $"{CourseName}: assignment '{assignment.AssignmentName}' has been Graded.";
                    NotifyStudents(message, assignment.CourseId);

                    return RedirectToAction("ViewSubmissions", new { assignmentId });
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid grade input. Please enter a number.");
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Submission not found.");
            }

            
            var submissions = _context.StudentAssignment
                .Include(sa => sa.Assignments)
                .Where(sa => sa.AssignmentId == assignmentId)
                .ToList();

            var viewModel = submissions.Select(sa => new StudentSubmissionViewModel
            {
                StudentFullName = _context.User
                    .Where(u => u.Id == sa.StudentId)
                    .Select(u => u.FullName)
                    .FirstOrDefault() ?? "Unknown",

                SubmissionDate = sa.SubmissionDate,
                MaxPoints = sa.Assignments?.AssignmentPoints ?? 0,
                GivenPoints = sa.Grade.HasValue ? sa.Grade.Value.ToString() : "UG",
                AssignmentId = sa.AssignmentId,
                StudentId = sa.StudentId
            }).ToList();

            return View("ViewSubmissions", viewModel);
        }



        #region Grades

        public string GetGradeLetter(decimal? grade)
        {
            if (grade == null)
            {
                return "N/A";
            }
            else if (grade >= 94)
            {
                return "A";
            }
            else if (grade >= 90)
            {
                return "A-";
            }
            else if (grade >= 87)
            {
                return "B+";
            }
            else if (grade >= 84)
            {
                return "B";
            }
            else if (grade >= 80)
            {
                return "B-";
            }
            else if (grade >= 77)
            {
                return "C+";
            }
            else if (grade >= 74)
            {
                return "C";
            }
            else if (grade >= 70)
            {
                return "C-";
            }
            else if (grade >= 67)
            {
                return "D+";
            }
            else if (grade >= 64)
            {
                return "D";
            }
            else if (grade >= 60)
            {
                return "D-";
            }
            else
            {
                return "E";
            }
        }

        public ActionResult Grades(int? id)
        {
            var user = _auth.GetUser();

            if (user == null)
            {
                return RedirectToAction("Index", "Course", new { id = id });
            }

            var course = _context.Courses
                .Include(c => c.Assignments)
                .FirstOrDefault(c => c.CourseId == id);
            var dept = _context.Departments.FirstOrDefault(d => d.DepartmentId == course.DepartmentId);

            if (course == null)
            {
                return RedirectToAction("Index", "Course", new { id = id });
            }

            // Compute grade distribution for all students in the course
            var studentConnections = _context.StudentCourseConnection
                .Where(s => s.CourseId == course.CourseId && s.Grade != null)
                .Select(s => s.Grade)
                .ToList();

            var gradeDistribution = new List<KeyValuePair<string, int>>()
    {
        new KeyValuePair<string, int>("<60%", studentConnections.Count(g => g < 60)),
        new KeyValuePair<string, int>("60-70%", studentConnections.Count(g => g >= 60 && g <= 70)),
        new KeyValuePair<string, int>("71-80%", studentConnections.Count(g => g >= 71 && g <= 80)),
        new KeyValuePair<string, int>("81-90%", studentConnections.Count(g => g >= 81 && g <= 90)),
        new KeyValuePair<string, int>("91-100%", studentConnections.Count(g => g >= 91))
    };

            var viewModel = new CourseGradeViewModel
            {
                CourseId = course.CourseId,
                UserRole = _auth.GetRole(),
                Department = dept.DepartmentName,
                CourseNum = course.CourseNumber,
                Grade = user.UserRole == "Student" ? GetGradeLetter(studentConnections.FirstOrDefault()) : null,
                GradeDistribution = gradeDistribution,
                Notifications = _notificationService.GetNotificationsForUser(user.Id).ToList(),
            };

            return View(viewModel);
        }


        public void UpdateGrade(int studentId, int courseId)
        {
            // Grab the student course connection
            var studentCourseConnection = _context.StudentCourseConnection
                .FirstOrDefault(scc => scc.CourseId == courseId && scc.StudentID == studentId);

            // Grab the assignments so we can use it in the student assignments (all foreign keys are null for some reason)
            var listOfAssignments = _context.Assignments
                    .Where(a => a.CourseId == courseId)
                    .ToList();

            // Grab the student assignments with the list of assignments as a foreign key
            //   Grab the student assignments that have a grade, are in the list of assignments, and have the same student id as the student
            var studentAssignments = _context.StudentAssignment
                .Where(a => listOfAssignments.Select(a => a.AssignmentId).ToList().Contains(a.AssignmentId) && a.Grade != null)
                .Where(a => a.StudentId == studentId)
                .ToList();

            // If any of them are null, return with no changes
            if (studentAssignments != null)
            {
                // Divide the sum of the points rewarded by the sum of the total points for all assignments, and multiply by 100 to get the percentage
                decimal rewardedPoints = studentAssignments.Sum(a => a.Grade ?? 0);
                decimal totalPoints = listOfAssignments.Sum(a => a.AssignmentPoints);

                studentCourseConnection.Grade = (rewardedPoints / totalPoints) * 100;
                _context.SaveChanges();
            }

            return;
        }

        #endregion


        #region SubmitAssignments
        /// ////////////////////////////////////////////////////////////////////////////////////////////////////////
        // Ivans changes
        public ActionResult SubmitPage(int assignmentId)
        {
            if (_auth.GetUser() == null)
            {
                return RedirectToAction("Login", "User");
            }
            var studentId = _auth.GetCurrentUserId();

            var Submitted = _context.StudentAssignment
           .Where(sa => sa.AssignmentId == assignmentId && sa.StudentId == studentId)
           .FirstOrDefault();


            var assignment = _context.Assignments.FirstOrDefault(a => a.AssignmentId == assignmentId);

            if (assignment == null)
            {
                return NotFound();
            }

            var viewModel = new SubmitAssignmentViewModel
            {
                Notifications = _notificationService.GetNotificationsForUser(studentId).ToList(),
                AssignmentId = assignment.AssignmentId,
                AssignmentName = assignment.AssignmentName,
                Description = assignment.AssignmentDescription,
                Points = assignment.AssignmentPoints,
                SubmissionType = assignment.AssignmentType,
                Grade = Submitted?.Grade ?? 0,
                DueDateAndTime = assignment.DueDateAndTime,
            };

            return View(viewModel);
        }

        public ActionResult ReSubmitPage(int assignmentId)
        {

            return RedirectToAction("SubmitAssignment", new { assignmentId = assignmentId, isresubmitted = true });
        }

        public ActionResult SubmitAssignment(int assignmentId, bool isresubmitted)
        {
            if (_auth.GetUser() == null)
            {
                return RedirectToAction("Login", "User");
            }
            var assignment = _context.Assignments.FirstOrDefault(a => a.AssignmentId == assignmentId);

            var studentId = _auth.GetCurrentUserId(); // Get the current user (student)
            var existingSubmission = _context.StudentAssignment
                .Where(sa => sa.AssignmentId == assignmentId && sa.StudentId == studentId)
                .FirstOrDefault();
            if (existingSubmission != null && isresubmitted == false)
            {
                return RedirectToAction("SubmitPage", new { assignmentId = assignmentId });
            }
                if (assignment == null)
            {
                return NotFound();
            }

            var viewModel = new SubmitAssignmentViewModel
            {
                Notifications = _notificationService.GetNotificationsForUser(studentId).ToList(),
                AssignmentId = assignment.AssignmentId,
                AssignmentName = assignment.AssignmentName,
                Description = assignment.AssignmentDescription,
                Points = assignment.AssignmentPoints,
                SubmissionType = assignment.AssignmentType
            };

            return View(viewModel);
        }
    

        [HttpPost]
        public async Task<ActionResult> SubmitAssignment(SubmitAssignmentViewModel model, int assignmentId, IFormFile FileSubmission, string TextSubmission) // Place notification code for instructors
        {
            

                var CourseId = _context.Assignments
          .Where(uc => uc.AssignmentId == assignmentId)
          .Select(uc => uc.CourseId)
        .FirstOrDefault();

                int CouseID = Convert.ToInt32(CourseId);

                var ClassName = _context.Courses
              .Where(uc => uc.CourseId == CouseID)
              .Select(uc => uc.CourseName)
           .FirstOrDefault();

                string CourseName = ClassName.ToString();
                var assignment = _context.Assignments.FirstOrDefault(a => a.AssignmentId == assignmentId);

                model.AssignmentId = assignmentId;
                model.AssignmentName = assignment.AssignmentName;
                model.Description = assignment.AssignmentDescription;
                model.Points = assignment.AssignmentPoints;
                model.SubmissionType = assignment.AssignmentType;


                string NewTextSubmission = "";
                string NewFileSubmission = "";
                if (_auth.GetUser() == null)
                {
                    return RedirectToAction("Login", "User");
                }
                var studentId = _auth.GetCurrentUserId(); // Get the current user (student)
                var existingSubmission = await _context.StudentAssignment
                    .Where(sa => sa.AssignmentId == assignmentId && sa.StudentId == studentId)
                    .FirstOrDefaultAsync();


                if (existingSubmission != null)
                {
                    if (assignment.AssignmentType == "text" && !string.IsNullOrEmpty(TextSubmission))
                    {
                        // Update the submission column with the text submission
                        existingSubmission.TextSubmission = TextSubmission;
                    }
                    else if (assignment.AssignmentType == "file" && FileSubmission != null && FileSubmission.Length > 0)
                    {
                        // Check the file type
                        var fileExtension = Path.GetExtension(FileSubmission.FileName).ToLower();
                        if (fileExtension != ".pdf" && fileExtension != ".doc" && fileExtension != ".docx")
                        {
                            ModelState.AddModelError("", "Invalid file type. Only PDF and Word documents are allowed.");
                            return View(model);
                        }

                        // Convert the file to a Base64 string
                        using (var memoryStream = new MemoryStream())
                        {
                            await FileSubmission.CopyToAsync(memoryStream);
                            var fileBytes = memoryStream.ToArray();
                            var base64String = Convert.ToBase64String(fileBytes);

                            // Update the submission column with the base64-encoded file submission
                            existingSubmission.TextSubmission = base64String;
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "No valid submission found.");
                        return View(model);
                    }
                    existingSubmission.SubmissionDate = DateTime.UtcNow;
                    await _context.SaveChangesAsync();
                    var message = $"A student has submitted '{assignment.AssignmentName}' Assignment from: {CourseName}";
                    NotifyInstructors(message, CouseID);
                }


                else
                {
                    if (assignment.AssignmentType == "text" && !string.IsNullOrEmpty(TextSubmission))
                    {
                        // Update the submission column with the text submission
                        NewTextSubmission = TextSubmission;
                    }
                    else if (assignment.AssignmentType == "file" && FileSubmission != null && FileSubmission.Length > 0)
                    {
                        // Check the file type
                        var fileExtension = Path.GetExtension(FileSubmission.FileName).ToLower();
                        if (fileExtension != ".pdf" && fileExtension != ".doc" && fileExtension != ".docx")
                        {
                            ModelState.AddModelError("", "Invalid file type. Only PDF and Word documents are allowed.");
                            return View(model);
                        }

                        // Convert the file to a Base64 string
                        using (var memoryStream = new MemoryStream())
                        {
                            await FileSubmission.CopyToAsync(memoryStream);
                            var fileBytes = memoryStream.ToArray();
                            var base64String = Convert.ToBase64String(fileBytes);

                            // Update the submission column with the base64-encoded file submission
                            NewFileSubmission = base64String;
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "No valid submission found.");
                        return View(model);
                    }
                    var newSubmission = new StudentAssignments
                    {
                        TextSubmission = NewTextSubmission,
                        FileSubmission = NewFileSubmission,
                        AssignmentId = assignmentId,
                        StudentId = studentId,
                        Grade = null,
                        SubmissionDate = DateTime.UtcNow,


                    };
                    _context.StudentAssignment.Add(newSubmission);
                    await _context.SaveChangesAsync();

                    var message = $"A student has submitted '{assignment.AssignmentName}' Assignment from : {CourseName}";
                    NotifyInstructors(message, CouseID);
                }
                return RedirectToAction("SubmitPage", new { assignmentId = assignmentId });
        }
        
        #endregion


    }

}








