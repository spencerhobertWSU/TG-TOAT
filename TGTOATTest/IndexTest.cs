using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Collections.Generic;
using System.Linq;
using TGTOAT.Controllers;
using Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.EntityFrameworkCore;
using Data;
using TGTOAT.Helpers;
using TGTOAT;
using OpenQA.Selenium.BiDi.Modules.Session;


namespace Scott_Test
{
    [TestClass]
    public class UserControllerTests
    {
        private UserContext _context;
        private UserController _controller;
        private Mock<IAuthentication> _mockAuth;
        private NotificationService _notificationService;
        private Mock<IPasswordHasher> _mockHasher;

        public UserControllerTests()
        {
            _mockAuth = new Mock<IAuthentication>();
            _mockHasher = new Mock<IPasswordHasher>();
            _controller = new UserController(_notificationService, _context, _mockHasher.Object, _mockAuth.Object);
        }

        [TestMethod]
        public async Task Index_UserNotLoggedIn()
        {
            _mockAuth.Setup(auth => auth.getUser()).Returns((CurrUser)null);

            var result = await _controller.Index(It.IsAny<UserIndexViewModel>()) as RedirectToActionResult;

            Assert.IsNotNull(result);
            Assert.AreEqual("Login", result.ActionName);
        }

        [TestMethod]
        public async Task Index_UserLoggedIn()
        {
            var course1 = new CurrClasses
            {
                CourseId = 1,
                CourseNum = 1010,
                CourseName = "Intro to Computer Science",
                Campus = "Ogden",
                Building = "Science",
                Room = 101,
                Days = "MWF",
                StartTime = new TimeOnly(9, 0),
                StopTime = new TimeOnly(10, 0),
                Semester = "Fall",
                Year = 2024,
                Color = "Red",
                Picture = "computer-science.jpg"
            };
            var course2 = new CurrClasses
            {
                CourseId = 2,
                CourseNum = 2020,
                CourseName = "Calculus I",
                Campus = "Ogden",
                Building = "Math",
                Room = 202,
                Days = "TR",
                StartTime = new TimeOnly(10, 30),
                StopTime = new TimeOnly(12, 0),
                Semester = "Spring",
                Year = 2024,
                Color = "Blue",
                Picture = "calculus.jpg"
            };

            var mockUser = new CurrUser
            {
                UserId = 1,
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com",
                Role = "Student",
                PFP = new byte[] { },
                BirthDate = new DateOnly(1990, 1, 1),
                Notifications = new List<Notifications>()
            };

            _mockAuth.Setup(auth => auth.getUser()).Returns(mockUser);

            var mockViewModel = new UserIndexViewModel
            {
                Id = mockUser.UserId,
                FirstName = mockUser.FirstName,
                LastName = mockUser.LastName,
                UserRole = mockUser.Role,
                Notifications = mockUser.Notifications,
                Courses = { course1, course2 },
                UpcomingAssignments = new List<UpcomingAssign>(),
                Assignments = new List<Assignment>()
            };

            _mockAuth.Setup(auth => auth.setIndex());
            _mockAuth.Setup(auth => auth.getIndex()).Returns(mockViewModel);

            var result = await _controller.Index(It.IsAny<UserIndexViewModel>()) as ViewResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(mockViewModel, result.Model);

            var viewModel = (UserIndexViewModel)result.Model;
            Assert.IsNotNull(viewModel.Courses);
            Assert.AreEqual(2, viewModel.Courses.Count);
            Assert.AreEqual("Intro to Computer Science", viewModel.Courses[0].CourseName);
            Assert.AreEqual("Calculus I", viewModel.Courses[1].CourseName);
        }

    }
}