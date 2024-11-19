using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Microsoft.EntityFrameworkCore;
using Data;
using TGTOAT;
using TGTOAT.Controllers;
using Moq;
using TGTOAT.Helpers;
using Microsoft.AspNetCore.Mvc;
using TGTOAT.Models;

namespace Spencer_Unit_Tests
{
    [TestClass]
    public class Spencer_Unit_Tests
    {
        private IWebDriver driver;
        private UserContext _context;
        private CourseController _controller;
        private Mock<IAuthentication> _mockAuth;
        private NotificationService _notificationService;

        [TestMethod]
        public void StudentRegisterForCourse()
        {
            // Figure out how many courses the student is registered for
            // Register the student for a course
            // Check if the student is now registered for one more course

            // Create the web driver
            driver = new ChromeDriver();

            // Navigate to the website
            try
            {
                driver.Navigate().GoToUrl("https://localhost:44362/");
            }
            catch
            {
                driver.Navigate().GoToUrl("https://localhost:7244/");
            }

            // Login to the website (IDK WHY BUT YOU HAVE TO SEND THE ENTER KEY? YOU CAN'T CLICK IT?)
            driver.FindElement(By.Id("Email")).SendKeys("bob@gmail.com");
            driver.FindElement(By.Id("Password")).SendKeys("Pass123");
            driver.FindElement(By.XPath("//input[@value='Log In']")).SendKeys(Keys.Enter);

            // Click on the registration tab
            driver.FindElement(By.XPath("//a[@href='/User/CourseRegistration']")).Click();

            // Count how many 'Drop' buttons there are
            int numOfDropButtonsBefore = driver.FindElements(By.Id("dropBtn")).Count();

            // Click on the first 'Register' button
            driver.FindElement(By.Id("registerBtn")).SendKeys(Keys.Enter);

            // Count how many 'Drop' buttons there are now
            int numOfDropButtonsAfter = driver.FindElements(By.Id("dropBtn")).Count();

            // Check if the number of drop buttons has increased by 1
            if (numOfDropButtonsAfter == numOfDropButtonsBefore + 1)
            {
                Assert.IsTrue(true);
            }
            else
            {
                Assert.IsTrue(false);
            }

            // Quit the driver
            driver.Quit();
        }

        [TestMethod]
        public void InstructorGradeQuiz()
        {
            // Setup Moq and InMemoryDatabase
            // Create an instructor and student
            // Create a course
            // Connect the instructor and student to the course
            // Create a quiz
            // Save them ALL to the database
            // Grade the quiz with a random number between 0 and the max score
            // Check if the student's grade is equal to the grade given

            _mockAuth = new Mock<IAuthentication>();
            var options = new DbContextOptionsBuilder<UserContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _context = new UserContext(options);
            _notificationService = new NotificationService(_context);

            // Create the instructor
            User instructor = new User
            {
                UserId = 1,
                Email = "instructor@weber.edu",
                Password = "Test"
            };
            UserInfo instructorInfo = new UserInfo
            {
                UserId = 1,
                FirstName = "Test",
                LastName = "Instructor",
                Role = "Instructor",
                PFP = "Test",
                BirthDate = new DateOnly(1990, 1, 1)
            };

            // Create the student
            User student = new User
            {
                UserId = 2,
                Email = "student@weber.edu",
                Password = "Test"
            };
            UserInfo studentInfo = new UserInfo
            {
                UserId = 2,
                FirstName = "Test",
                LastName = "Student",
                Role = "Student",
                PFP = "Test",
                BirthDate = new DateOnly(1990, 1, 1)
            };

            // Create the department (for the course)
            Departments department = new Departments
            {
                DeptId = 1,
                DeptName = "Test Department"
            };

            // Create the course
            Courses course = new Courses
            {
                CourseId = 1,
                DeptId = department.DeptId,
                CourseNum = 1010,
                CourseName = "Test Course",
                CourseDesc = "Test Description",
                Credits = 3,
                Capacity = 30,
                Campus = "Ogden",
                Building = "Science",
                Room = 101,
                Days = "MWF",
                StartTime = new TimeOnly(9, 0),
                StopTime = new TimeOnly(10, 0),
                Semester = "Fall",
                Year = 2024,
                Color = "Test",
                Picture = "Test"
            };

            // Connect the instructor and student to the course
            InstructorConnection instructorCourseConnection = new InstructorConnection
            {
                InstructorId = instructor.UserId,
                CourseId = course.CourseId
            };
            StudentConnection studentCourseConnection = new StudentConnection
            {
                StudentId = student.UserId,
                CourseId = course.CourseId
            };

            // Create the quiz
            Assignment quiz = new Assignment
            {
                AssignId = 1,
                CourseId = course.CourseId,
                AssignName = "Quiz",
                AssignDesc = "Test Quiz",
                AssignType = "Text",
                MaxPoints = 100,
                DueDate = DateTime.Now,
            };

            // Connect the student to the quiz
            StudentAssignments studentQuiz = new StudentAssignments
            {
                AssignId = quiz.AssignId,
                StudentId = student.UserId,
                Points = null,
                Submitted = DateTime.Now,
                Submission = "Test Submission"
            };

            // Save them ALL to the database
            _context.User.Add(instructor);
            _context.UserInfo.Add(instructorInfo);
            _context.User.Add(student);
            _context.UserInfo.Add(studentInfo);
            _context.Courses.Add(course);
            _context.InstructorConnection.Add(instructorCourseConnection);
            _context.StudentConnection.Add(studentCourseConnection);
            _context.Assignments.Add(quiz);
            _context.StudentAssignment.Add(studentQuiz);
            _context.SaveChanges();

            // Create the controller
            _controller = new CourseController( _context, _mockAuth.Object);

            // Grade the quiz with a random number between 0 and the max score
            Random random = new Random();
            string grade = random.Next(0, quiz.MaxPoints).ToString();
            _controller.UpdateSubmission(quiz.AssignId, student.UserId, grade);

            // Check if the student's grade is equal to the grade given
            StudentAssignments quizFromDb = _context.StudentAssignment
                .FirstOrDefault(q => q.AssignId == quiz.AssignId && q.StudentId == student.UserId);
            Assert.AreEqual(grade, quizFromDb.Points.ToString());

            // Cleanup EVERYTHING (Moq, InMemoryDatabase, controller, everything)
            _context.Database.EnsureDeleted();
            _context.Dispose();
            _controller.Dispose();
        }

        [TestMethod]
        public void InstructorGradeAssignment()
        {
            _mockAuth = new Mock<IAuthentication>();
            var options = new DbContextOptionsBuilder<UserContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _context = new UserContext(options);
            _notificationService = new NotificationService(_context);

            // Create the instructor
            User instructor = new User
            {
                UserId = 1,
                Email = "instructor@weber.edu",
                Password = "Test"
            };
            UserInfo instructorInfo = new UserInfo
            {
                UserId = 1,
                FirstName = "Test",
                LastName = "Instructor",
                Role = "Instructor",
                PFP = "Test",
                BirthDate = new DateOnly(1990, 1, 1)
            };

            // Create the student
            User student = new User
            {
                UserId = 2,
                Email = "student@weber.edu",
                Password = "Test"
            };
            UserInfo studentInfo = new UserInfo
            {
                UserId = 2,
                FirstName = "Test",
                LastName = "Student",
                Role = "Student",
                PFP = "Test",
                BirthDate = new DateOnly(1990, 1, 1)
            };

            // Create the department (for the course)
            Departments department = new Departments
            {
                DeptId = 1,
                DeptName = "Test Department"
            };

            // Create the course
            Courses course = new Courses
            {
                CourseId = 1,
                DeptId = department.DeptId,
                CourseNum = 1010,
                CourseName = "Test Course",
                CourseDesc = "Test Description",
                Credits = 3,
                Capacity = 30,
                Campus = "Ogden",
                Building = "Science",
                Room = 101,
                Days = "MWF",
                StartTime = new TimeOnly(9, 0),
                StopTime = new TimeOnly(10, 0),
                Semester = "Fall",
                Year = 2024,
                Color = "Test",
                Picture = "Test"
            };

            // Connect the instructor and student to the course
            InstructorConnection instructorCourseConnection = new InstructorConnection
            {
                InstructorId = instructor.UserId,
                CourseId = course.CourseId
            };
            StudentConnection studentCourseConnection = new StudentConnection
            {
                StudentId = student.UserId,
                CourseId = course.CourseId
            };

            // Create the assignment
            Assignment assignment = new Assignment
            {
                AssignId = 1,
                CourseId = course.CourseId,
                AssignName = "assignment",
                AssignDesc = "Test assignment",
                AssignType = "Text",
                MaxPoints = 100,
                DueDate = DateTime.Now,
            };

            // Connect the student to the assignment
            StudentAssignments studentAssignment = new StudentAssignments
            {
                AssignId = assignment.AssignId,
                StudentId = student.UserId,
                Points = null,
                Submitted = DateTime.Now,
                Submission = "Test Submission"
            };

            // Save them ALL to the database
            _context.User.Add(instructor);
            _context.UserInfo.Add(instructorInfo);
            _context.User.Add(student);
            _context.UserInfo.Add(studentInfo);
            _context.Courses.Add(course);
            _context.InstructorConnection.Add(instructorCourseConnection);
            _context.StudentConnection.Add(studentCourseConnection);
            _context.Assignments.Add(assignment);
            _context.StudentAssignment.Add(studentAssignment);
            _context.SaveChanges();

            // Create the controller
            _controller = new CourseController(_context, _mockAuth.Object);

            // Grade the quiz with a random number between 0 and the max score
            Random random = new Random();
            string grade = random.Next(0, assignment.MaxPoints).ToString();
            _controller.UpdateSubmission(assignment.AssignId, student.UserId, grade);

            // Check if the student's grade is equal to the grade given
            StudentAssignments quizFromDb = _context.StudentAssignment
                .FirstOrDefault(q => q.AssignId == assignment.AssignId && q.StudentId == student.UserId);
            Assert.AreEqual(grade, quizFromDb.Points.ToString());

            // Cleanup EVERYTHING (Moq, InMemoryDatabase, controller, everything)
            _context.Database.EnsureDeleted();
            _context.Dispose();
            _controller.Dispose();
        }
    }
}
