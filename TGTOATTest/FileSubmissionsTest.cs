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

namespace TGTOATTest
{
    [TestClass]
    public class FileSubmissionsTest
    {
        private UserContext _context;
        private CourseController _controller;
        private Mock<IAuthentication> _mockAuth;
        private NotificationService _notificationService;

        [TestMethod]
        public void FSTEST()
        {
            #region Setup
            _mockAuth = new Mock<IAuthentication>();
            var options = new DbContextOptionsBuilder<UserContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _context = new UserContext(options);
            _notificationService = new NotificationService(_context);

            User student = new User
            {
                UserId = 1,
                Email = "student@weber.edu",
                Password = "Test"
            };
            UserInfo studentInfo = new UserInfo
            {
                UserId = 1,
                FirstName = "Test",
                LastName = "Student",
                Role = "Student",
                PFP = "Test",
                BirthDate = new DateOnly(1990, 1, 1)
            };

            Courses course = new Courses
            {
                CourseId = 1,
                CourseName = "Test Course",
                Credits = 3,
                Capacity = 30,
                Color = "Red",
                CourseDesc = "Test Description",
                Semester = "Fall 2024" 
            };


            Assignment assignment = new Assignment
            {
                AssignId = 1,
                CourseId = course.CourseId,
                AssignName = "Test Assignment",
                AssignDesc = "Test Description",
                AssignType = "File",
                MaxPoints = 100,
                DueDate = DateTime.Now.AddDays(1)
            };

            StudentAssignments studentAssignment = new StudentAssignments
            {
                AssignId = assignment.AssignId,
                StudentId = student.UserId,
                Points = null,
                Submitted = DateTime.MinValue,
                Submission = "Test Content"
            };

            _context.User.Add(student);
            _context.UserInfo.Add(studentInfo);
            _context.Courses.Add(course);
            _context.Assignments.Add(assignment);
            _context.StudentAssignment.Add(studentAssignment);
            _context.SaveChanges();

            var mockCurrUser = new CurrUser
            {
                UserId = student.UserId,
                Email = student.Email,
                FirstName = studentInfo.FirstName,
                LastName = studentInfo.LastName,
                Role = studentInfo.Role,
                PFP = new byte[] { },
                BirthDate = studentInfo.BirthDate,
                Notifications = new List<Notifications>()
            };
            #endregion

            #region Test
            _mockAuth.Setup(auth => auth.getUser()).Returns(mockCurrUser); 

            _controller = new CourseController(_context, _mockAuth.Object, _notificationService);

            var result = _controller.SubmitAssignment(assignment.AssignId, false);

            var redirectResult = result as RedirectToActionResult;
            Assert.IsNotNull(redirectResult);
            Assert.AreEqual("SubmitPage", redirectResult.ActionName);

            var updatedSubmission = _context.StudentAssignment
                .FirstOrDefault(sa => sa.AssignId == assignment.AssignId && sa.StudentId == student.UserId);

            Assert.IsNotNull(updatedSubmission);
            Assert.AreEqual(DateTime.MinValue, updatedSubmission.Submitted);
            #endregion
        }

    }
}
