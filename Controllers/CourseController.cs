using Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using TGTOAT.Helpers;
using Models;
using TGTOAT.Data;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using NuGet.Packaging.Signing;

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

        #region Create/Edit Courses
        // Display the AddCourse view with the list of departments
        //[HttpGet]
        public IActionResult Create()
        {
            var user = _auth.getUser();

            if (user == null)
            {
                return RedirectToAction("Login", "User");
            }

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

            var instructors = (from u in _context.User 
                               join ui in _context.UserInfo on u.UserId equals ui.UserId
                               where ui.Role == "Instructor"
                               select ui).ToList();

            var UserRole = _auth.getUser();

            var viewModel = new AddCourseViewModel
            {
                Departments = departments,
                CampusList = campus,
                SemesterList = semesters,
                CurrYear = DateTime.Now.Year,
                Instructors = instructors
            };

            return View(viewModel);
        }

        // Handle the form submission for adding a course
        [HttpPost]
        public async Task<IActionResult> Create(AddCourseViewModel model)
        {
            if (ModelState.IsValid)
            {
                string coursePic = null;

                if (model.Picture != null)
                {
                    byte[] imageBytes = System.IO.File.ReadAllBytes(model.Picture);
                    string base64String = Convert.ToBase64String(imageBytes);

                    coursePic = base64String;
                }

                TimeOnly sTime = new TimeOnly(04, 20, 00);
                TimeOnly eTime = new TimeOnly(16, 20, 00);

                if (model.StartTime == sTime & model.EndTime == eTime) {
                    model.StartTime = null;
                    model.EndTime = null;
                }

                var course = new Courses
                {
                    CourseNum = model.CourseNumber,
                    CourseName = model.CourseName,
                    DeptId = model.SelectedDepartmentId,
                    Capacity = model.Capacity,
                    Campus = model.Campus,
                    Building = model.Building,
                    Room = model.RoomNumber,
                    Days = model.DaysOfTheWeek,
                    StartTime = model.StartTime,
                    StopTime = model.EndTime,
                    Credits = model.NumberOfCredits,
                    CourseDesc = model.CourseDescription,
                    Semester = model.Semester,
                    Year = model.Year,
                    Color = model.Color,
                    Picture = coursePic,
                };

                _context.Courses.Add(course);
                await _context.SaveChangesAsync();

                var InstructorConnection = new InstructorConnection
                {
                    InstructorId = model.SelectedInstructorId,
                    CourseId = course.CourseId
                };

                _context.InstructorConnection.Add(InstructorConnection);
                await _context.SaveChangesAsync();

                return RedirectToAction("Courses");
            }

            // Re-populate if the model is invalid
            var departments = _context.Departments.ToList();

            var dempartmentslist = new List<SelectListItem>();

            foreach (var department in departments)
            {
                new SelectListItem { Value = department.DeptId.ToString(), Text = department.DeptName };
            }
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
            model.Instructors = _context.UserInfo.Where(u => u.Role == "Instructor").ToList();
            return View(model);
        }
        
        public ActionResult Edit(int id)
        {
            var user = _auth.getUser();

            if (user == null)
            {
                return RedirectToAction("Login", "User");
            }

            var course = _context.Courses.FirstOrDefault(c => c.CourseId == id);

            if (course == null)
            {
                return NotFound();
            }

            var viewModel = new AddCourseViewModel
            {
                Notifications = _auth.GetNotificationsForUser(user.UserId).ToList(),
                CourseId = id,
                CourseNumber = course.CourseNum,
                CourseName = course.CourseName,
                NumberOfCredits = course.Credits,
                SelectedDepartmentId = course.DeptId,
                Departments = _context.Departments?.ToList() ?? new List<Departments>(),
                CampusList = new List<SelectListItem>
                {
                    new SelectListItem { Value = "Ogden", Text = "Ogden Campus" },
                    new SelectListItem { Value = "Davis", Text = "Davis Campus" }
                },
                Building = course.Building,  // This ensures the selected building is set
                Buildings = GetBuildingListByCampus(course.Campus),  // Prepopulate buildings based on current campus
                CourseDescription = course.CourseDesc,
                DaysOfTheWeek = course.Days,
                StartTime = course.StartTime,
                EndTime = course.StopTime,
                RoomNumber = course.Room,
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
                Color = course.Color,
                Picture = course.Picture,
                SelectedInstructorId = _context.InstructorConnection.FirstOrDefault(icc => icc.CourseId == id)?.InstructorId ?? 0,
                Instructors = _context.UserInfo?.Where(u => u.Role == "Instructor").ToList() ?? new List<UserInfo>()
            };

            if(viewModel != null)
            {
                return View(viewModel);
            }

            return View(new AddCourseViewModel());


        }

        [HttpPost]
        public async Task<ActionResult> Edit(AddCourseViewModel model)
        {
            var user = _auth.getUser();

            if (user == null)
            {
                return RedirectToAction("Login", "User");
            }

            var course = new Courses
            {
                CourseId = model.CourseId,
                CourseNum = model.CourseNumber,
                CourseName = model.CourseName,
                DeptId = model.SelectedDepartmentId,
                Capacity = model.Capacity,
                Campus = model.Campus,
                Building = model.Building,
                Room = model.RoomNumber,
                Days = model.DaysOfTheWeek,
                StartTime = model.StartTime,
                StopTime = model.EndTime,
                Credits = model.NumberOfCredits,
                CourseDesc = model.CourseDescription,
                Semester = model.Semester,
                Year = model.Year,
                Color = model.Color,
                Picture = model.Picture,
            };

            _context.Courses.Update(course);
            await _context.SaveChangesAsync();

            var courseConnect = _context.InstructorConnection.FirstOrDefault(db => db.CourseId == model.CourseId);

            if(courseConnect.InstructorId != model.SelectedInstructorId)
            {
                var InstructorConnection = new InstructorConnection
                {
                    InstructorId = model.SelectedInstructorId,
                    CourseId = model.CourseId
                };
                _context.InstructorConnection.Update(InstructorConnection);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Courses");


        }

        #endregion

        #region Course Pages
        //Course/Courses
        public IActionResult Courses()
        {
            var currInstruct = _auth.getUser();

            if (currInstruct == null)
            {
                return RedirectToAction("Login", "User");
            }

            var courses = (from db in _context.Courses select db).ToList();


            var NewCourses = new List<CourseInfo>();

            foreach (var c in courses)
            {

                string deptName = _context.Departments.First(d => d.DeptId == c.DeptId).DeptName;

                var CourseModel = new CourseInfo
                {
                    CourseId = c.CourseId,
                    DeptName = deptName,
                    CourseNumber = c.CourseNum,
                    CourseName = c.CourseName,
                    Campus = c.Campus,
                    Building = c.Building,
                    NumberOfCredits = c.Credits,
                    Room = c.Room,
                    DaysOfTheWeek = c.Days,
                    StartTime = c.StartTime,
                    EndTime = c.StopTime,
                    Capacity = c.Capacity,
                    Semester = c.Semester,
                    Year = c.Year,
                };
                NewCourses.Add(CourseModel);

            };

            var CourseList = new CourseListViewModel
            {
                Courses = NewCourses,
            };

            return View(CourseList);
        }

        //Course/Index/id
        public ActionResult Index(int? id)
        {
            var user = _auth.getUser();

            if (user == null)
            {
                return RedirectToAction("Login", "User");
            }

            var course = _context.Courses.First(c => c.CourseId == id);
           
            var dept = _context.Departments.First(d => d.DeptId == course.DeptId);

            var assigns = (from a in _context.Assignments
                           join c in _context.Courses on a.CourseId equals c.CourseId
                           where c.CourseId == id
                           select a).ToList();

            var viewModel = new CourseHome
            {
                CourseId = course.CourseId,
                UserRole = _auth.getUser().Role,
                Department = dept.DeptName,
                CourseNum = course.CourseNum,
                Assignments = assigns
            };
            return View(viewModel);

        }


        public IActionResult ViewCourse(int id)
        {
            var course = _context.Courses.Include(c => c.DeptId).FirstOrDefault(c => c.CourseId == id);
            if (course == null)
            {
                return NotFound(); // Handle case where course is not found
            }
            return View(course);
        }
        #endregion

        #region Assignments
        public ActionResult Assignments(int? id)
        {
            var user = _auth.getUser();

            if (user == null)
            {
                return RedirectToAction("Login", "User");
            }
            var course = _context.Courses.First(c => c.CourseId == id);

            var dept = _context.Departments.FirstOrDefault(d => d.DeptId == course.DeptId);

            var assigns = (from a in _context.Assignments
                                    join c in _context.Courses on a.CourseId equals c.CourseId
                                    where c.CourseId == id
                                    select a).ToList();

            var quizzes = (from q in _context.Quizzes
                           join c in _context.Courses on q.CourseId equals c.CourseId
                           where c.CourseId == id
                           select q).ToList();

            if (course == null)
            {
                return Redirect("User/Index");
            }
            else
            {
                var viewModel = new CourseHome
                {
                    CourseId = course.CourseId,
                    UserRole = _auth.getUser().Role,
                    Department = dept.DeptName,
                    CourseNum = course.CourseNum,
                    Assignments = assigns,
                    Quizzes = quizzes
                };
                return View(viewModel);
            }

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

            var user = _auth.getUser();
           

            if (user == null)
            {
                return RedirectToAction("Login", "User");
            }

            var userIdInt = _auth.getUser().UserId;

            var userCourses = await _context.InstructorConnection
           .Where(uc => uc.InstructorId == userIdInt)
           .Select(uc => uc.CourseId)
           .ToListAsync();


            if (ModelState.IsValid)
            {
                var assignment = new Assignment
                {
                    CourseId = id,
                    AssignName = model.AssignmentName,
                    AssignDesc = model.AssignmentDescription,
                    MaxPoints = model.AssignmentPoints,
                    DueDate = model.DueDateAndTime,
                    AssignType = model.AssignmentType,
                };



                _context.Assignments.Add(assignment);
                await _context.SaveChangesAsync();
                return RedirectToAction("Assignments", new { id });

            }

            return View(model);
        }
        #endregion

        #region Quizzes
        public ActionResult Quiz(int id)
        {
            var user = _auth.getUser();

            if (user == null)
            {
                return RedirectToAction("Login", "User");
            }

            var quizInfo = _context.Quizzes.FirstOrDefault(q => q.QuizId == id);

            AddQuizViewModel quiz = new AddQuizViewModel
            {
                QuizId = id,
                QuizName = quizInfo.QuizName,
                QuizDescription = quizInfo.QuizDesc,
                QuizPoints = quizInfo.MaxPoints,
                NumQuestions = quizInfo.NumQuestions,
                Questions = quizInfo.Questions,
                DueDateAndTime = quizInfo.DueDate
            };

            return View(quiz);

        }

        [HttpGet]
        public IActionResult AddQuiz()
        {

            var user = _auth.getUser();

            if (user == null)
            {
                return RedirectToAction("Login", "User");
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddQuiz(int id, AddQuizViewModel model)
        {

            var user = _auth.getUser();

            if (user == null)
            {
                return RedirectToAction("Login", "User");
            }

            var userIdInt = _auth.getUser().UserId;

            var userCourses = await _context.InstructorConnection
           .Where(uc => uc.InstructorId == userIdInt)
           .Select(uc => uc.CourseId)
           .ToListAsync();


            if (ModelState.IsValid)
            {
                var quiz = new Quizzes
                {
                    CourseId = id,
                    QuizName = model.QuizName,
                    QuizDesc = model.QuizDescription,
                    MaxPoints = model.QuizPoints,
                    DueDate = model.DueDateAndTime,
                    NumQuestions = model.NumQuestions,
                    Questions = model.Questions
                };

                _context.Quizzes.Add(quiz);
                await _context.SaveChangesAsync();
                return RedirectToAction("Quizzes", new { id });

            }

            return View(model);
        }
        #endregion

        #region Calendar
        [HttpGet]
                public JsonResult GetCoursesForCalendar()
                {
                    var currentUser = _auth.getUser();  // Get the current user's details
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
                    if (currentUser.Role == "Instructor")
                    {
                        // Fetch courses for the instructor
                        var instructorCourses = (from ic in _context.InstructorConnection 
                                                join c in _context.Courses on ic.CourseId equals c.CourseId
                                                where ic.InstructorId == currentUser.UserId
                                                select c).ToList();

                        foreach (var course in instructorCourses)
                        {
                            AddCourseToEventList(course, dayMap, courseEvents);
                        }
                    }
                    // student role
                    else if (currentUser.Role == "Student")
                    {
                    // Fetch courses for the student
                    var studentCourses = (from s in _context.StudentConnection
                                            join c in _context.Courses on s.CourseId equals c.CourseId
                                            where s.StudentId == currentUser.UserId
                                            select c).ToList();

                    var assignments = (from s in _context.StudentAssignment
                                       join a in _context.Assignments on s.AssignId equals a.AssignId
                                       where s.StudentId == currentUser.UserId
                                       select a).ToList();

                foreach (var course in studentCourses)
                        {
                            // Add course events
                            AddCourseToEventList(course, dayMap, courseEvents);

                            foreach (var assignment in assignments)
                            {
                                courseEvents.Add(new
                                {
                                    title = assignment.AssignName,
                                    start = assignment.DueDate.ToString("yyyy-MM-ddTHH:mm:ss"),
                                    url = $"/Course/Assignments/{course.CourseId}"  // Link to course's assignment page
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
                    if (course.StartTime.HasValue && course.StopTime.HasValue)
                    {
                        var startTime = course.StartTime.Value;
                        var stopTime = course.StopTime.Value;

                        // Split the DaysOfTheWeek string by commas
                        var days = course.Days.Split(',');

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
                                    end = new DateTime(2024, 9, 1, stopTime.Hour, stopTime.Minute, stopTime.Second).ToString("yyyy-MM-ddTHH:mm:ss"),
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
        #endregion

        #region Submissions

        public ActionResult ViewSubmissions(int assignmentId)
        {
            var submissions = _context.StudentAssignment
                .Where(sa => sa.AssignId == assignmentId)
                .ToList();

            var Submissions = new List<StudentSubmission>();

            foreach (var s in submissions)
            {
                
                var assigns = _context.Assignments.First(a => a.AssignId == assignmentId);
                var sInfo = _context.UserInfo.First(ui => ui.UserId == s.StudentId);

                var submission = new StudentSubmission
                {
                    StudentFullName = (sInfo.FirstName+" "+sInfo.LastName),

                    SubmissionDate = s.Submitted,
                    MaxPoints = assigns.MaxPoints,
                    GivenPoints = s.Points.HasValue ? s.Points.Value.ToString() : "UG",
                    AssignmentId = s.AssignId,
                    StudentId = s.StudentId
                };
                Submissions.Add(submission);
            }
            return View(Submissions);
        }

        public ActionResult ViewSubmission(int assignmentId, int studentId)
        {
            var assignment = _context.Assignments
                .FirstOrDefault(a => a.AssignId == assignmentId);

            var submission = _context.StudentAssignment.First(sa => sa.StudentId == studentId);
            var sInfo = _context.UserInfo.First(ui => ui.UserId == studentId);

            if (assignment == null || submission == null)
            {
                return NotFound();
            }

            var user = _context.User
                .FirstOrDefault(u => u.UserId == submission.StudentId);

            if (user == null)
            {
                return NotFound();
            }

            var viewModel = new List<SubmissionDetailViewModel>
    {
        new SubmissionDetailViewModel
        {
            StudentFullName = (sInfo.FirstName+" "+sInfo.LastName),
            DueDate = assignment.DueDate,
            SubmissionDate = submission.Submitted,
            MaxPoints = assignment.MaxPoints,
            GivenPoints = submission.Points.HasValue ? submission.Points.Value.ToString() : "UG",
            TextSubmission = submission.Submission,
            FileSubmission = submission.Submission,
            HasFile = !string.IsNullOrEmpty(submission.Submission),
            AssignmentId = assignment.AssignId, // Include the assignment ID
            StudentId = submission.StudentId // Include the student ID
        }
    };

            return View(viewModel);
        }





        [HttpPost]
        public ActionResult UpdateSubmission(int assignmentId, int studentId, string givenPoints)
        {
           
            var submission = _context.StudentAssignment
                .FirstOrDefault(sa => sa.AssignId == assignmentId && sa.StudentId == studentId);
            var sInfo = _context.UserInfo.First(ui => ui.UserId == studentId);

            if (submission != null)
            {
                if (int.TryParse(givenPoints, out int grade))
                {
                    // Submit the assignment grade
                    submission.Points = grade;
                    _context.SaveChanges();
                    
                    // Update the students course grade
                    var assignment = _context.Assignments
                        .FirstOrDefault(a => a.AssignId == assignmentId);
                    UpdateGrade(studentId, assignment.CourseId);

                    var ClassName = _context.Courses
                    .Where(uc => uc.CourseId == assignment.CourseId)
                    .Select(uc => uc.CourseName)
                    .FirstOrDefault();

                    string CourseName = ClassName.ToString();

                    var message = $"{CourseName}: assignment '{assignment.AssignName}' has been Graded.";
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

            return View("ViewSubmissions");
        }

        private void NotifyStudents(string message, int CourseId)
        {

            var stuentIds = _context.StudentConnection
            .Where(uc => uc.CourseId == CourseId)
            .Select(uc => uc.StudentId)
            .ToList();

            foreach (var studentId in stuentIds)
            {
                _auth.CreateNotification(message, studentId);
            }
        }

        public void UpdateGrade(int studentId, int courseId)
        {
            // Grab the student course connection
            var studentCourseConnection = _context.StudentConnection
                .FirstOrDefault(scc => scc.CourseId == courseId && scc.StudentId == studentId);

            // Grab the assignments so we can use it in the student assignments (all foreign keys are null for some reason)
            var listOfAssignments = _context.Assignments
                    .Where(a => a.CourseId == courseId)
                    .ToList();

            // Grab the student assignments with the list of assignments as a foreign key
            //   Grab the student assignments that have a grade, are in the list of assignments, and have the same student id as the student
            var studentAssignments = _context.StudentAssignment
                .Where(a => listOfAssignments.Select(a => a.AssignId).ToList().Contains(a.AssignId) && a.Points != null)
                .Where(a => a.StudentId == studentId)
                .ToList();

            // If any of them are null, return with no changes
            if (studentAssignments != null)
            {
                // Divide the sum of the points rewarded by the sum of the total points for all assignments, and multiply by 100 to get the percentage
                decimal rewardedPoints = studentAssignments.Sum(a => a.Points ?? 0);
                decimal totalPoints = listOfAssignments.Sum(a => a.MaxPoints);

                studentCourseConnection.Grade = (rewardedPoints / totalPoints) * 100;
                _context.SaveChanges();
            }

            return;
        }

        #endregion

        public IActionResult GetProfileImage()
        {
            var imageBytes = _auth.getUser().PFP;

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
                    var instructorConnection = _context.InstructorConnection.FirstOrDefault(icc => icc.CourseId == id);
                    if (instructorConnection != null)
                    {
                        _context.InstructorConnection.Remove(instructorConnection);
                    }

                    // Remove the course
                    _context.Courses.Remove(course);
                    _context.SaveChanges();

                    return RedirectToAction("Courses");
                }


        // Smegma



        #region SubmitAssignments
        /// ////////////////////////////////////////////////////////////////////////////////////////////////////////
        // Ivans changes

        public ActionResult SubmitPage(int assignmentId)
        {
            if (_auth.getUser() == null)
            {
                return RedirectToAction("Login", "User");
            }
            var studentId = _auth.getUser().UserId;

            var Submitted = _context.StudentAssignment
           .Where(sa => sa.AssignId == assignmentId && sa.StudentId == studentId)
           .FirstOrDefault();


            var assignment = _context.Assignments.FirstOrDefault(a => a.AssignId == assignmentId);

            if (assignment == null)
            {
                return NotFound();
            }

            var viewModel = new SubmitAssignmentViewModel
            {
                AssignmentId = assignment.AssignId,
                AssignmentName = assignment.AssignName,
                Description = assignment.AssignDesc,
                Points = assignment.MaxPoints,
                SubmissionType = assignment.AssignType,
                //Grade = Submitted?.Grade ?? 0,
                DueDateAndTime = assignment.DueDate,
            };

            return View(viewModel);
        }

        public ActionResult ReSubmitPage(int assignmentId)
        {

            return RedirectToAction("SubmitAssignment", new { assignmentId = assignmentId, isresubmitted = true });
        }

        public ActionResult SubmitAssignment(int assignmentId, bool isresubmitted)
        {
            if (_auth.getUser() == null)
            {
                return RedirectToAction("Login", "User");
            }
            var assignment = _context.Assignments.FirstOrDefault(a => a.AssignId == assignmentId);

            var studentId = _auth.getUser().UserId; // Get the current user (student)
            var existingSubmission = _context.StudentAssignment
                .Where(sa => sa.AssignId == assignmentId && sa.StudentId == studentId)
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
                AssignmentId = assignment.AssignId,
                AssignmentName = assignment.AssignName,
                Description = assignment.AssignDesc,
                Points = assignment.MaxPoints,
                SubmissionType = assignment.AssignType
            };

            return View(viewModel);
        }
    

    [HttpPost]
        public async Task<ActionResult> SubmitAssignment(SubmitAssignmentViewModel model, int assignmentId, IFormFile FileSubmission, string TextSubmission)
        {
            var assignment = _context.Assignments.FirstOrDefault(a => a.AssignId == assignmentId);

            model.AssignmentId = assignmentId;
            model.AssignmentName = assignment.AssignName;
            model.Description = assignment.AssignDesc;
            model.Points = assignment.MaxPoints;
            model.SubmissionType = assignment.AssignType;


            string NewTextSubmission = "";
            string NewFileSubmission = "";
            if (_auth.getUser() == null)
            {
                return RedirectToAction("Login", "User");
            }
            var studentId = _auth.getUser().UserId; // Get the current user (student)
            var existingSubmission = await _context.StudentAssignment
                .Where(sa => sa.AssignId == assignmentId && sa.StudentId == studentId)
                .FirstOrDefaultAsync();


            if (existingSubmission != null)
            {
                if (assignment.AssignType == "text" && !string.IsNullOrEmpty(TextSubmission))
                {
                    // Update the submission column with the text submission
                    existingSubmission.Submission = TextSubmission;
                }
                else if (assignment.AssignType == "file" && FileSubmission != null && FileSubmission.Length > 0)
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
                        existingSubmission.Submission = base64String;
                    }
                }
                else
                {
                    ModelState.AddModelError("", "No valid submission found.");
                    return View(model);
                }
                existingSubmission.Submitted = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }


            else
            {
                if (assignment.AssignType == "text" && !string.IsNullOrEmpty(TextSubmission))
                {
                    // Update the submission column with the text submission
                    NewTextSubmission = TextSubmission;
                }
                else if (assignment.AssignType == "file" && FileSubmission != null && FileSubmission.Length > 0)
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
                    //Submission = NewTextSubmission,
                    //FileSubmission = NewFileSubmission,
                    AssignId = assignmentId,
                    StudentId = studentId,
                    Points = null,
                    Submitted = DateTime.UtcNow,


                };
                _context.StudentAssignment.Add(newSubmission);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("SubmitPage", new { assignmentId = assignmentId });
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

    }

}
#endregion