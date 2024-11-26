using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel;
using Data;
using TGTOAT.Helpers;
using TGTOAT.Models;
using Models;
using Microsoft.AspNetCore.Http.Extensions;

namespace TGTOAT.Controllers
{
    public class CourseController : Controller
    {
        private readonly UserContext _context;
        private readonly IAuthentication _auth;
        private readonly NotificationService _notificationService;

        public CourseController(UserContext context, IAuthentication auth, NotificationService notificationService)
        {
            _notificationService = notificationService;
            _context = context;
            _auth = auth;
        }

        #region CRUD Courses

            #region Create Courses
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

                    if (model.StartTime == sTime & model.EndTime == eTime)
                    {
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
            #endregion

            #region Edit Courses
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

                if (viewModel != null)
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

                if (courseConnect != null)
                {
                    if (courseConnect.InstructorId != model.SelectedInstructorId)
                    {
                        var InstructorConnection = new InstructorConnection
                        {
                            InstructorId = model.SelectedInstructorId,
                            CourseId = model.CourseId
                        };
                        _context.InstructorConnection.Update(InstructorConnection);
                        await _context.SaveChangesAsync();
                    }
                }

                return RedirectToAction("Courses");


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
            #endregion

            #region Delete Courses
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
                var instructorId = instructorConnection.InstructorId;
                var courseId = instructorConnection.CourseId;
            
                if (instructorConnection != null)
                {
                    var studentClasses = _context.Database.ExecuteSqlRaw(
                    "DELETE FROM InstructorConnection WHERE InstructorId = {0} AND CourseId = {1}",
                    instructorId, courseId
                    );
                    _context.SaveChanges();
                }

                // Remove the course
                _context.Courses.Remove(course);
                _context.SaveChanges();

                return RedirectToAction("Courses");
            }
            #endregion

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

            // Get courses for the current instructor using InstructorConnection table
            var instructorCourses = (from ic in _context.InstructorConnection
                                    join c in _context.Courses on ic.CourseId equals c.CourseId
                                    where ic.InstructorId == currInstruct.UserId
                                    select c).ToList();

            var NewCourses = new List<CourseInfo>();

            foreach (var c in instructorCourses)
            {
                string deptName = _context.Departments.First(d => d.DeptId == c.DeptId).DeptName;

                var CourseModel = new CourseInfo
                {
                    Notifications = _notificationService.GetNotificationsForUser(currInstruct.UserId).ToList(),
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
                Notifications = _notificationService.GetNotificationsForUser(currInstruct.UserId).ToList(),
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
                Description = course.CourseDesc,
                Notifications = _notificationService.GetNotificationsForUser(user.UserId).ToList(),
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
                    Notifications = _notificationService.GetNotificationsForUser(user.UserId).ToList(),

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

        //public ActionResult Assignments(int? id)
        public ActionResult AddAssignment(int? id)
        {

            var user = _auth.getUser();//Grab User Info
            int userId = user.UserId;


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

            var ClassName = _context.Courses
                    .Where(uc => uc.CourseId == id)
                    .Select(uc => uc.CourseName)
                   .FirstOrDefault();

            string CourseName = ClassName.ToString();


           
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
                var message = $"{CourseName}: New assignment '{assignment.AssignName}' has been created.";
                NotifyStudents(message, id);
                return RedirectToAction("Assignments", new { id });

            

            return View(model);
        }
        #endregion

        #region Quizzes

        public ActionResult Quizzes(int? id)
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

            var stuQuizes = (from sq in _context.StudentQuizzes
                             join q in _context.Quizzes on sq.QuizId equals q.QuizId
                             join c in _context.Courses on q.CourseId equals c.CourseId
                             where sq.StudentId == user.UserId
                             where c.CourseId == id
                             select sq).ToList();
                            

            if (course == null)
            {
                return Redirect("User/Index");
            }
            else
            {
                var viewModel = new CourseHome
                {
                    Notifications = _notificationService.GetNotificationsForUser(user.UserId).ToList(),
                    CourseId = course.CourseId,
                    UserRole = _auth.getUser().Role,
                    Department = dept.DeptName,
                    CourseNum = course.CourseNum,
                    Assignments = assigns,
                    Quizzes = quizzes,
                    studentQuizzes = stuQuizes
                };
                return View(viewModel);
            }

        }
        public ActionResult Quiz(int id, int quizId)
        {
            var user = _auth.getUser();

            if (user == null)
            {
                return RedirectToAction("Login", "User");
            }

            var submitted = _context.StudentQuizzes.FirstOrDefault(sq => sq.QuizId == quizId && sq.StudentId == user.UserId);

            if (submitted != null)
            {
                return RedirectToAction("ViewQuiz", new { id, quizId });
            }

            var quizInfo = _context.Quizzes.FirstOrDefault(q => q.QuizId == quizId);

            var quiz = new TakeQuizModel
            {
                Notifications = _notificationService.GetNotificationsForUser(user.UserId).ToList(),
                CourseId = id,
                QuizId = quizId,
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

            var viewModel = new AddQuizViewModel
            {
                Notifications = _notificationService.GetNotificationsForUser(user.UserId).ToList(),


            };

            return View(viewModel);
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


            var ClassName = _context.Courses
          .Where(uc => uc.CourseId == id)
          .Select(uc => uc.CourseName)
       .FirstOrDefault();

            string CourseName = ClassName.ToString();
            
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
                var message = $"A student has submitted '{quiz.QuizName}' Quiz from: {CourseName}";
                NotifyInstructors(message, id);
                return RedirectToAction("Quizzes", new { id });

            

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> SubmitQuiz(TakeQuizModel model)
        {
            var user = _auth.getUser();

            if (user == null)
            {
                return RedirectToAction("Login", "User");
            }

            var quizInfo = _context.Quizzes.FirstOrDefault(q => q.QuizId == model.QuizId);

            string questions = quizInfo.Questions;
            string submission = model.Submission;

            var graded = "";
            var gradePoints = 0;

            var qPart = questions.Split("з");
            var sPart = submission.Split("з");

            for (var i = 0; i < qPart.Length; i++)
            {
                var qType = qPart[i].Split("д");
                var sType = sPart[i].Split("д");

                if (sType[0] == "2")
                {
                    var qPoints = qType[1].Split("п");
                    var sPoints = sType[1].Split("п");

                    var qCorrect = qPoints[0].Split("ж");
                    var sCorrect = sPoints[0].Split("ж");

                    var actualPoints = sPoints[1].Split("/");

                    if (sCorrect[1] == qCorrect[1])
                    {
                        actualPoints[0] = actualPoints[1];
                    }
                    gradePoints += Int32.Parse(actualPoints[0]);

                    graded += (sType[0]);
                    graded += ("д");
                    graded += (sCorrect[0]);
                    graded += ("ж");
                    graded += (sCorrect[1]);
                    graded += ("п");
                    graded += (actualPoints[0] + "/" + actualPoints[1]);
                    graded += ("з");

                }
                else
                {
                    graded += (sPart[i]);
                    graded += ("з");
                }
            }

            var StudentQuiz = new StudentQuizzes
            {

                QuizId = model.QuizId,
                StudentId = user.UserId,
                Points = gradePoints,
                Submitted = DateTime.UtcNow,
                Submission = graded
            };

            _context.StudentQuizzes.Add(StudentQuiz);
            await _context.SaveChangesAsync();

            return RedirectToAction("ViewQuiz", new { id = model.CourseId, quizId = model.QuizId });

        }

        public ActionResult ViewQuiz(int id, int quizId)
        {
                var user = _auth.getUser();

                if (user == null)
                {
                    return RedirectToAction("Login", "User");
                }

                var quizInfo = _context.Quizzes.First(q => q.QuizId == quizId);
                var submittedQuiz = _context.StudentQuizzes.FirstOrDefault(q => q.QuizId == quizId && q.StudentId == user.UserId);

                var quiz = new TakeQuizModel
                {
                    Notifications = _notificationService.GetNotificationsForUser(user.UserId).ToList(),
                    UserRole = user.Role,
                    CourseId = id,
                    QuizId = quizId,
                    QuizName = quizInfo.QuizName,
                    QuizDescription = quizInfo.QuizDesc,
                    Points = submittedQuiz.Points.Value,
                    QuizPoints = quizInfo.MaxPoints,
                    NumQuestions = quizInfo.NumQuestions,
                    Questions = submittedQuiz.Submission,
                    Submitted = submittedQuiz.Submitted,
                    DueDateAndTime = quizInfo.DueDate
                };

                return View(quiz);
        }

        public ActionResult GradeQuiz(int id, int quizId, int stuId)
        {
            {
                var user = _auth.getUser();

                if (user == null)
                {
                    return RedirectToAction("Login", "User");
                }

                var quizInfo = _context.Quizzes.FirstOrDefault(q => q.QuizId == quizId);
                var submittedQuiz = _context.StudentQuizzes.FirstOrDefault(q => q.QuizId == quizId && q.StudentId == stuId);

                var quiz = new TakeQuizModel
                {
                    Notifications = _notificationService.GetNotificationsForUser(user.UserId).ToList(),
                    stuId = stuId,
                    UserRole = user.Role,
                    CourseId = id,
                    QuizId = quizId,
                    QuizName = quizInfo.QuizName,
                    QuizDescription = quizInfo.QuizDesc,
                    Points = submittedQuiz.Points.Value,
                    QuizPoints = quizInfo.MaxPoints,
                    NumQuestions = quizInfo.NumQuestions,
                    Questions = submittedQuiz.Submission,
                    Submitted = submittedQuiz.Submitted,
                    DueDateAndTime = quizInfo.DueDate
                };

                return View(quiz);

            }
        }

        public async Task<IActionResult> UpdateQuizScore(TakeQuizModel model)
        {
            {
                var user = _auth.getUser();

                if (user == null)
                {
                    return RedirectToAction("Login", "User");
                }


                var quiz = new StudentQuizzes
                {
                    QuizId = model.QuizId,
                    StudentId = model.stuId,
                    Submission = model.Questions,
                    Points = model.Points,
                    Submitted = model.Submitted
                };


                _context.StudentQuizzes.Update(quiz);
                await _context.SaveChangesAsync();

                return RedirectToAction("ViewSubmissions", new { id = model.CourseId, quizId = model.QuizId } );

            }
        }

        #endregion

        #region Calendar
        [HttpGet]
        public JsonResult GetCoursesForCalendar()
        {
            var currentUser = _auth.getUser();
            if (currentUser == null)
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
                var instructorCourses = (from ic in _context.InstructorConnection
                                        join c in _context.Courses on ic.CourseId equals c.CourseId
                                        where ic.InstructorId == currentUser.UserId
                                        select c).ToList();

                // Get assignments for instructor's courses
                var assignments = (from a in _context.Assignments
                                 where instructorCourses.Select(c => c.CourseId).Contains(a.CourseId)
                                 select new
                                 {
                                     a.AssignId,
                                     a.AssignName,
                                     a.DueDate,
                                     a.CourseId
                                 }).Distinct().ToList();

                // Get quizzes for instructor's courses
                var quizzes = (from q in _context.Quizzes
                              where instructorCourses.Select(c => c.CourseId).Contains(q.CourseId)
                              select new
                              {
                                  q.QuizId,
                                  q.QuizName,
                                  q.DueDate,
                                  q.CourseId
                              }).Distinct().ToList();

                foreach (var course in instructorCourses)
                {
                    AddCourseToEventList(course, dayMap, courseEvents);
                }

                // Add assignment events
                foreach (var assignment in assignments)
                {
                    courseEvents.Add(new
                    {
                        title = $"Assignment: {assignment.AssignName}",
                        start = assignment.DueDate.ToString("yyyy-MM-ddTHH:mm:ss"),
                        url = $"/Course/ViewSubmissions/{assignment.CourseId}?assignmentId={assignment.AssignId}",
                        backgroundColor = "#3788d8" // Blue color for assignments
                    });
                }

                // Add quiz events
                foreach (var quiz in quizzes)
                {
                    courseEvents.Add(new
                    {
                        title = $"Quiz: {quiz.QuizName}",
                        start = quiz.DueDate.ToString("yyyy-MM-ddTHH:mm:ss"),
                        url = $"/Course/ViewSubmissions/{quiz.CourseId}?quizId={quiz.QuizId}",
                        backgroundColor = "#e74c3c" // Red color for quizzes
                    });
                }
            }
            // student role
            else if (currentUser.Role == "Student")
            {
                var studentCourses = (from s in _context.StudentConnection
                                     join c in _context.Courses on s.CourseId equals c.CourseId
                                     where s.StudentId == currentUser.UserId
                                     select c).ToList();

                // Get assignments for enrolled courses
                var assignments = (from a in _context.Assignments
                                 where studentCourses.Select(c => c.CourseId).Contains(a.CourseId)
                                 select new
                                 {
                                     a.AssignId,
                                     a.AssignName,
                                     a.DueDate,
                                     a.CourseId
                                 }).Distinct().ToList();

                // Get quizzes for enrolled courses
                var quizzes = (from q in _context.Quizzes
                              where studentCourses.Select(c => c.CourseId).Contains(q.CourseId)
                              select new
                              {
                                  q.QuizId,
                                  q.QuizName,
                                  q.DueDate,
                                  q.CourseId
                              }).Distinct().ToList();

                foreach (var course in studentCourses)
                {
                    // Add course events
                    AddCourseToEventList(course, dayMap, courseEvents);
                }

                // Add assignment events
                foreach (var assignment in assignments)
                {
                    string assignmentUrl = currentUser.Role == "Student" 
                        ? $"/Course/SubmitPage/{assignment.CourseId}?assignmentId={assignment.AssignId}"  // Student goes to submission page
                        : $"/Course/ViewSubmissions/{assignment.CourseId}?assignmentId={assignment.AssignId}";  // Instructor goes to view all submissions

                    courseEvents.Add(new
                    {
                        title = $"Assignment: {assignment.AssignName}",
                        start = assignment.DueDate.ToString("yyyy-MM-ddTHH:mm:ss"),
                        url = assignmentUrl,
                        backgroundColor = "#3788d8" // Blue color for assignments
                    });
                }

                // Add quiz events
                foreach (var quiz in quizzes)
                {
                    courseEvents.Add(new
                    {
                        title = $"Quiz: {quiz.QuizName}",
                        start = quiz.DueDate.ToString("yyyy-MM-ddTHH:mm:ss"),
                        url = $"/Course/Quiz/{quiz.CourseId}?quizId={quiz.QuizId}",
                        backgroundColor = "#e74c3c" // Red color for quizzes
                    });
                }
            }

            return Json(courseEvents);
        }

        // Helper method to add course events to the list
        private void AddCourseToEventList(Courses course, Dictionary<string, int> dayMap, List<object> courseEvents)
        {
            // Skip online courses (where Campus is "Onl")
            if (course.Campus?.ToUpper() == "ONL")
            {
                return;
            }

            if (course.StartTime.HasValue && course.StopTime.HasValue && !string.IsNullOrEmpty(course.Days))
            {
                var startTime = course.StartTime.Value;
                var stopTime = course.StopTime.Value;
                var days = course.Days.Split(',');

                // Set the default start and end dates for the semester
                string startRecur = string.Empty;
                string endRecur = string.Empty;

                // Determine the semester and set the correct recurrence dates
                switch (course.Semester?.ToLower())
                {
                    case "fall":
                        startRecur = $"{course.Year}-08-26";
                        endRecur = $"{course.Year}-12-13";
                        break;
                    case "spring":
                        startRecur = $"{course.Year}-01-06";
                        endRecur = $"{course.Year}-04-25";
                        break;
                    case "summer":
                        startRecur = $"{course.Year}-05-05";
                        endRecur = $"{course.Year}-08-15";
                        break;
                }

                var dayCodes = new List<int>();
                foreach (var day in days)
                {
                    string trimmedDay = day.Trim();
                    if (dayMap.ContainsKey(trimmedDay))
                    {
                        dayCodes.Add(dayMap[trimmedDay]);
                    }
                }

                courseEvents.Add(new
                {
                    title = course.CourseName,
                    startTime = startTime.ToString("HH:mm:ss"),
                    endTime = stopTime.ToString("HH:mm:ss"),
                    daysOfWeek = dayCodes,
                    startRecur = startRecur,
                    endRecur = endRecur,
                    backgroundColor = course.Color
                });
            }
        }
        #endregion

        #region Submissions

        //Assignments
        public ActionResult ViewSubmissions(int id, int subId)
        {
            var currInstruct = _auth.getUser();

            if (currInstruct == null)
            {
                return RedirectToAction("Login", "User");
            }

            var type = Request.QueryString;
            var typeInt = type.ToString().Split("=");
            subId = int.Parse(typeInt[1]);

            if (type.ToString().Contains("assignmentId"))
            {
                var submissions = (from sa in _context.StudentAssignment
                                   where sa.AssignId == subId
                                   select sa).ToList();

                var Submissions = new List<StudentSubmission>();

                foreach (var s in submissions)
                {

                    var assigns = _context.Assignments.First(a => a.AssignId == subId);
                    var sInfo = _context.UserInfo.First(ui => ui.UserId == s.StudentId);

                    var submission = new StudentSubmission
                    {
                        StudentFullName = (sInfo.FirstName + " " + sInfo.LastName),
                        Notifications = _notificationService.GetNotificationsForUser(currInstruct.UserId).ToList(),
                        SubmissionDate = s.Submitted,
                        MaxPoints = assigns.MaxPoints,
                        GivenPoints = s.Points.HasValue ? s.Points.Value.ToString() : "UG",
                        AssignmentId = s.AssignId,
                        StudentId = s.StudentId
                    };
                    Submissions.Add(submission);

                }

                var AllSubs = new AllSubmissions
                {
                    Notifications = _notificationService.GetNotificationsForUser(currInstruct.UserId).ToList(),
                    type = "assignment",
                    CourseId = id,
                    Submissions = Submissions
                };
                return View(AllSubs);
            }
            else if (type.ToString().Contains("quizId"))
            {
                var submissions = (from sq in _context.StudentQuizzes
                                   where sq.QuizId == subId
                                   select sq).ToList();

                Console.WriteLine(subId);

                var Submissions = new List<StudentSubmission>();

                foreach (var s in submissions)
                {

                    var quizzes = _context.Quizzes.First(a => a.QuizId == subId);
                    var sInfo = _context.UserInfo.First(ui => ui.UserId == s.StudentId);

                    var submission = new StudentSubmission
                    {
                        StudentFullName = (sInfo.FirstName + " " + sInfo.LastName),
                        Notifications = _notificationService.GetNotificationsForUser(currInstruct.UserId).ToList(),

                        SubmissionDate = s.Submitted,
                        MaxPoints = quizzes.MaxPoints,
                        GivenPoints = s.Points.HasValue ? s.Points.Value.ToString() : "UG",
                        AssignmentId = s.QuizId,
                        StudentId = s.StudentId
                    };
                    Submissions.Add(submission);

                }

                var AllSubs = new AllSubmissions
                {
                    Notifications = _notificationService.GetNotificationsForUser(currInstruct.UserId).ToList(),
                    type = "quiz",
                    CourseId = id,
                    Submissions = Submissions
                };
                return View(AllSubs);
            }

            return RedirectToAction("Login", "User");
        }

        public ActionResult ViewSubmission(int courseId, int assignmentId, int studentId)
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

            var SubmissionDetailViewModel = new SubmissionDetailViewModel
            {
                Notifications = _notificationService.GetNotificationsForUser(user.UserId).ToList(),
                StudentFullName = (sInfo.FirstName + " " + sInfo.LastName),
                DueDate = assignment.DueDate,
                SubmissionDate = submission.Submitted,
                MaxPoints = assignment.MaxPoints,
                GivenPoints = submission.Points.HasValue ? submission.Points.Value.ToString() : "UG",
                TextSubmission = submission.Submission,
                FileSubmission = submission.Submission,
                submissionType = assignment.AssignType,
                AssignmentId = assignment.AssignId, // Include the assignment ID
                StudentId = submission.StudentId // Include the student ID
            };

            return View(SubmissionDetailViewModel);
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
        #endregion

        #region Notifications
        private void NotifyInstructors(string message, int CourseId)
        {

            var stuentIds = _context.InstructorConnection
            .Where(uc => uc.CourseId == CourseId)
            .Select(uc => uc.InstructorId)
            .ToList();

            foreach (var studentId in stuentIds)
            {
                _notificationService.CreateNotification(message, studentId);
            }
        }
        private void NotifyStudents(string message, int CourseId)
        {

            var stuentIds = _context.StudentConnection
            .Where(uc => uc.CourseId == CourseId)
            .Select(uc => uc.StudentId)
            .ToList();

            foreach (var studentId in stuentIds)
            {
                _notificationService.CreateNotification(message, studentId);
            }
        }
        #endregion

        #region PFP
        public IActionResult GetProfileImage()
        {
            var imageBytes = _auth.getUser().PFP;

            return File(imageBytes, "image/png");
        }
        #endregion

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
            var user = _auth.getUser();

            if (user == null)
            {
                return RedirectToAction("Index", "Course", new { id = id });
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
                return RedirectToAction("Index", "Course", new { id = id });
            }

            // Compute grade distribution for all students in the course
            var studentConnections = _context.StudentConnection
                .Where(s => s.CourseId == course.CourseId && s.Grade != null)
                .Select(s => s.Grade)
                .ToList();

            var gradeDistribution = new List<KeyValuePair<string, int>>()
            {
                new KeyValuePair<string, int>("less than 60%", studentConnections.Count(g => g < 60)),
                new KeyValuePair<string, int>("60-70%", studentConnections.Count(g => g >= 60 && g <= 70)),
                new KeyValuePair<string, int>("71-80%", studentConnections.Count(g => g >= 71 && g <= 80)),
                new KeyValuePair<string, int>("81-90%", studentConnections.Count(g => g >= 81 && g <= 90)),
                new KeyValuePair<string, int>("91-100%", studentConnections.Count(g => g >= 91))
            };

            var viewModel = new CourseGradeViewModel
            {
                CourseId = course.CourseId,
                UserRole = user.Role,
                Department = dept.DeptName,
                CourseNum = course.CourseNum.ToString(),
                Grade = user.Role == "Student" ? GetGradeLetter(studentConnections.FirstOrDefault()) : null,
                GradeDistribution = gradeDistribution,
                Notifications = _notificationService.GetNotificationsForUser(user.UserId).ToList(),
            };

            return View(viewModel);
        }

        public void UpdateGrade(int studentId, int courseId)
        {
            // Grab the student course connection
            var StudentCourses = (from c in _context.StudentConnection
                                  where c.StudentId == studentId
                                  select c).AsNoTracking().ToList();

            StudentConnection studentCourseConnection = null;

            foreach (var sc in StudentCourses)
            {
                if(sc.CourseId == courseId)
                {
                    studentCourseConnection = sc;
                    break;
                }
            }

            // Grab the assignments so we can use it in the student assignments (all foreign keys are null for some reason)
            var listOfAssignments = _context.Assignments
                    .Where(a => a.CourseId == courseId)
                    .ToList();

            // Grab the student assignments with the list of assignments as a foreign key
            //   Grab the student assignments that have a grade, are in the list of assignments, and have the same student id as the student

            decimal maxPoints = 0;

            var listAssignments = (from a in _context.Assignments
                                   where a.CourseId == courseId
                                   select a).ToList();

            var listQuizzes = (from q in _context.Quizzes
                                   where q.CourseId == courseId
                                   select q).ToList();

            foreach (var la in listAssignments)
            {
                maxPoints = maxPoints + la.MaxPoints;
            }

            foreach (var lq in listQuizzes)
            {
                maxPoints = maxPoints + lq.MaxPoints;
            }

            decimal rewardedPoints = 0;

            var assignments = (from sa in _context.StudentAssignment
                               join a in _context.Assignments on sa.AssignId equals a.AssignId
                               where a.CourseId == courseId
                               where sa.StudentId == studentId
                               where sa.Points != null
                               select sa).ToList();

            var quizzes = (from sq in _context.StudentQuizzes
                           join q in _context.Quizzes on sq.QuizId equals q.QuizId
                           where q.CourseId == courseId
                           where sq.StudentId == studentId
                            where sq.Points != null
                            select sq).ToList();

            foreach(var sa in assignments)
            {
                rewardedPoints = rewardedPoints + sa.Points.Value;
            }

            foreach(var sq in quizzes)
            {
                rewardedPoints = rewardedPoints + sq.Points.Value;
            }


            // Check if totalPoints is greater than zero to prevent division by zero
            if(studentCourseConnection != null)
            {
                studentCourseConnection.Grade = (rewardedPoints / maxPoints) * 100m;
            }

            // Save the changes to the database
            _context.SaveChanges();


            return;
        }

        #endregion

        #region SubmitAssignments
        /// ////////////////////////////////////////////////////////////////////////////////////////////////////////
        // Ivans changes

        //After Assignment Has Been Submitted
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
                Submission = Submitted.Submission,
                Notifications = _notificationService.GetNotificationsForUser(studentId).ToList(),
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
                Notifications = _notificationService.GetNotificationsForUser(studentId).ToList(),

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
            var user = _auth.getUser();

            if (user  == null)
            {
                return RedirectToAction("Login", "User");
            }

            
            var assignment = _context.Assignments.FirstOrDefault(a => a.AssignId == assignmentId);

            model.AssignmentId = assignmentId;
            model.AssignmentName = assignment.AssignName;
            model.Description = assignment.AssignDesc;
            model.Points = assignment.MaxPoints;
            model.SubmissionType = assignment.AssignType;


            var CourseId = _context.Assignments
          .Where(uc => uc.AssignId == assignmentId)
          .Select(uc => uc.CourseId)
        .FirstOrDefault();

            int CouseID = Convert.ToInt32(CourseId);

            var ClassName = _context.Courses
          .Where(uc => uc.CourseId == CouseID)
          .Select(uc => uc.CourseName)
       .FirstOrDefault();

            string CourseName = ClassName.ToString();

            string newFile = "";

            if (assignment.AssignType == "text")
            {
                // Update the submission column with the text submission
                newFile = TextSubmission;
            }
            else if (assignment.AssignType == "file")
            {
                // Check the file type
                var fileExtension = Path.GetExtension(FileSubmission.FileName).ToLower();
                if (fileExtension != ".pdf" && fileExtension != ".doc" && fileExtension != ".docx")
                {
                    ModelState.AddModelError("", "Invalid file type. Only PDF and Word documents are allowed.");
                    return View(model);
                }

                //Convert the file to a Base64 string
                using (var memoryStream = new MemoryStream())
                {
                    await FileSubmission.CopyToAsync(memoryStream);
                    var fileBytes = memoryStream.ToArray();
                    var base64String = Convert.ToBase64String(fileBytes);

                    // Update the submission column with the base64-encoded file submission
                    newFile = base64String;
                }
                
            }
            else
            {
                ModelState.AddModelError("", "No valid submission found.");
                return View(model);
            }            



            var resubmit = _context.StudentAssignment.FirstOrDefault(sa => sa.AssignId == assignmentId);

            if(resubmit != null)
            {
                _context.Entry<StudentAssignments>(resubmit).State = EntityState.Detached;
            }
            

            if(resubmit == null)
            {
                var newSubmission = new StudentAssignments
                {
                    Submission = newFile,
                    AssignId = assignmentId,
                    StudentId = user.UserId,
                    Points = null,
                    Submitted = DateTime.UtcNow,
                };
                _context.StudentAssignment.Add(newSubmission);
            }
            else
            {
                var newSubmission = new StudentAssignments
                {
                    Submission = newFile,
                    AssignId = assignmentId,
                    StudentId = user.UserId,
                    Points = resubmit.Points,
                    Submitted = DateTime.UtcNow,
                };
                _context.StudentAssignment.Update(newSubmission);
            }

            await _context.SaveChangesAsync();
            var message = $"A student has submitted '{assignment.AssignName}' Assignment from : {ClassName}";
            NotifyInstructors(message, CouseID);

            return RedirectToAction("SubmitPage", new { assignmentId = assignmentId });
        }


    }

}
#endregion









