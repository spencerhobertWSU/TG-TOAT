using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using Microsoft.EntityFrameworkCore;
using TGTOAT.Data;
using TGTOAT;
using TGTOAT.Controllers;
using Moq;
using TGTOAT.Helpers;
using Microsoft.AspNetCore.Mvc;
using TGTOAT.Models;


namespace Josh_Tests
{
    [TestClass]
    public class Josh_Unit_Tests
    {
        private UserContext _context;
        private CourseController _controller;
        private Mock<IAuthentication> _mockAuth;
        private NotificationService _notificationService;

        [TestMethod]
        public void StudentDropCourseTest()
        {
            // Setup
            _mockAuth = new Mock<IAuthentication>();
            var options = new DbContextOptionsBuilder<UserContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _context = new UserContext(options);
            _notificationService = new NotificationService(_context);

            // Create student
            User student = new User
            {
                Id = 1,
                Email = "student@weber.edu",
                Password = "Test",
                FirstName = "Test",
                LastName = "Student",
                BirthDate = new DateTime(1990, 1, 1),
                UserRole = "Student"
            };

            // Create department and course
            Departments department = new Departments
            {
                DepartmentId = 1,
                DepartmentName = "Test Department"
            };

            Courses course = new Courses
            {
                CourseId = 1,
                DepartmentId = department.DepartmentId,
                Department = department,
                CourseNumber = "TS1010",
                CourseName = "Test Course",
                CourseDescription = "Test Description",
                NumberOfCredits = 3,
                Capacity = 30,
                Campus = "Ogden",
                Building = "Science",
                RoomNumber = 101,
                DaysOfTheWeek = "MWF",
                StartTime = new TimeOnly(9, 0),
                EndTime = new TimeOnly(10, 0),
                Semester = "Fall",
                Year = 2024
            };

            // Connect student to course
            StudentCourseConnection studentCourseConnection = new StudentCourseConnection
            {
                StudentCourseId = 1,
                StudentID = student.Id,
                User = student,
                CourseId = course.CourseId,
                Course = course
            };

            // Save to database
            _context.User.Add(student);
            _context.Courses.Add(course);
            _context.StudentCourseConnection.Add(studentCourseConnection);
            _context.SaveChanges();

            // Create controller
            _controller = new CourseController(_notificationService, _context, _mockAuth.Object);

            // Act - Drop the course by removing the connection
            var connection = _context.StudentCourseConnection
                .FirstOrDefault(scc => scc.StudentID == student.Id && scc.CourseId == course.CourseId);
            _context.StudentCourseConnection.Remove(connection);
            _context.SaveChanges();

            // Assert
            var studentCourseAfterDrop = _context.StudentCourseConnection
                .FirstOrDefault(scc => scc.StudentID == student.Id && scc.CourseId == course.CourseId);
            Assert.IsNull(studentCourseAfterDrop, "Course should be dropped (connection should be removed)");

            // Cleanup
            _context.Database.EnsureDeleted();
            _context.Dispose();
            _controller.Dispose();
        }

        [TestMethod]
        public void InstructorEditCourseTest()
        {
            // Setup
            _mockAuth = new Mock<IAuthentication>();
            var options = new DbContextOptionsBuilder<UserContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _context = new UserContext(options);
            Assert.IsNotNull(_context, "Context is null");
            
            _notificationService = new NotificationService(_context);
            Assert.IsNotNull(_notificationService, "NotificationService is null");

            // Create instructor
            User instructor = new User
            {
                Id = 1,
                Email = "instructor@weber.edu",
                Password = "Test",
                FirstName = "Test",
                LastName = "Instructor",
                BirthDate = new DateTime(1990, 1, 1),
                UserRole = "Instructor"
            };
            Assert.IsNotNull(instructor, "Instructor is null");

            // Create department and initial course
            Departments department = new Departments
            {
                DepartmentId = 1,
                DepartmentName = "Test Department"
            };
            Assert.IsNotNull(department, "Department is null");

            Courses course = new Courses
            {
                CourseId = 1,
                DepartmentId = department.DepartmentId,
                Department = department,
                CourseNumber = "TS1010",
                CourseName = "Original Course Name",
                CourseDescription = "Original Description",
                NumberOfCredits = 3,
                Capacity = 30,
                Campus = "Ogden",
                Building = "Science Lab",
                RoomNumber = 101,
                DaysOfTheWeek = "MWF",
                StartTime = new TimeOnly(9, 0),
                EndTime = new TimeOnly(10, 0),
                Semester = "Fall",
                Year = 2024
            };
            Assert.IsNotNull(course, "Course is null");

            // Connect instructor to course
            InstructorCourseConnection instructorCourseConnection = new InstructorCourseConnection
            {
                InstructorCourseConnectionId = 1,
                InstructorID = instructor.Id,
                Instructor = instructor,
                CourseId = course.CourseId,
                Course = course
            };
            Assert.IsNotNull(instructorCourseConnection, "InstructorCourseConnection is null");

            course.instructorCourseConnections = new List<InstructorCourseConnection> { instructorCourseConnection };
            Assert.IsNotNull(course.instructorCourseConnections, "Course.instructorCourseConnections is null");

            try
            {
                // Save to database
                _context.Departments.Add(department);
                _context.User.Add(instructor);
                _context.Courses.Add(course);
                _context.InstructorCourseConnection.Add(instructorCourseConnection);
                _context.SaveChanges();

                // Verify entities were saved
                Assert.IsTrue(_context.Departments.Any(), "No departments in database");
                Assert.IsTrue(_context.User.Any(), "No users in database");
                Assert.IsTrue(_context.Courses.Any(), "No courses in database");
                Assert.IsTrue(_context.InstructorCourseConnection.Any(), "No instructor connections in database");

                // Create controller
                _controller = new CourseController(_notificationService, _context, _mockAuth.Object);
                Assert.IsNotNull(_controller, "Controller is null");

                // Act - Update all course properties
                var updatedCourse = _context.Courses
                    .Include(c => c.Department)
                    .Include(c => c.instructorCourseConnections)
                    .FirstOrDefault(c => c.CourseId == course.CourseId);

                Assert.IsNotNull(updatedCourse, "Failed to retrieve course from database");
                Assert.IsNotNull(updatedCourse.Department, "Department is null in retrieved course");
                Assert.IsNotNull(updatedCourse.instructorCourseConnections, "InstructorCourseConnections is null in retrieved course");

                updatedCourse.CourseNumber = "TS2020";
                updatedCourse.CourseName = "Updated Course Name";
                updatedCourse.CourseDescription = "Updated Description";
                updatedCourse.NumberOfCredits = 4;
                updatedCourse.Capacity = 40;
                updatedCourse.Campus = "Davis";
                updatedCourse.Building = "Engineering";
                updatedCourse.RoomNumber = 202;
                updatedCourse.DaysOfTheWeek = "TR";
                updatedCourse.StartTime = new TimeOnly(13, 30);
                updatedCourse.EndTime = new TimeOnly(14, 45);
                updatedCourse.Semester = "Spring";
                updatedCourse.Year = 2025;
                _context.SaveChanges();

                // Assert - Check all properties
                var courseAfterUpdate = _context.Courses
                    .Include(c => c.Department)
                    .Include(c => c.instructorCourseConnections)
                    .FirstOrDefault(c => c.CourseId == course.CourseId);

                Assert.IsNotNull(courseAfterUpdate, "Failed to retrieve updated course");
                Assert.IsNotNull(courseAfterUpdate.Department, "Department is null after update");
                Assert.IsNotNull(courseAfterUpdate.instructorCourseConnections, "Instructor connections are null after update");

                Assert.AreEqual("TS2020", courseAfterUpdate.CourseNumber, "Course number should be updated");
                Assert.AreEqual("Updated Course Name", courseAfterUpdate.CourseName, "Course name should be updated");
                Assert.AreEqual("Updated Description", courseAfterUpdate.CourseDescription, "Course description should be updated");
                Assert.AreEqual(4, courseAfterUpdate.NumberOfCredits, "Number of credits should be updated");
                Assert.AreEqual(40, courseAfterUpdate.Capacity, "Capacity should be updated");
                Assert.AreEqual("Davis", courseAfterUpdate.Campus, "Campus should be updated");
                Assert.AreEqual("Engineering", courseAfterUpdate.Building, "Building should be updated");
                Assert.AreEqual(202, courseAfterUpdate.RoomNumber, "Room number should be updated");
                Assert.AreEqual("TR", courseAfterUpdate.DaysOfTheWeek, "Days of the week should be updated");
                Assert.AreEqual(new TimeOnly(13, 30), courseAfterUpdate.StartTime, "Start time should be updated");
                Assert.AreEqual(new TimeOnly(14, 45), courseAfterUpdate.EndTime, "End time should be updated");
                Assert.AreEqual("Spring", courseAfterUpdate.Semester, "Semester should be updated");
                Assert.AreEqual(2025, courseAfterUpdate.Year, "Year should be updated");

                // Verify department and instructor relationships remain intact
                Assert.AreEqual(department.DepartmentId, courseAfterUpdate.DepartmentId, "Department relationship should remain unchanged");
                Assert.IsTrue(courseAfterUpdate.instructorCourseConnections.Any(icc => icc.InstructorID == instructor.Id), 
                    "Instructor relationship should remain unchanged");
            }
            finally
            {
                if (_context != null)
                {
                    _context.Database.EnsureDeleted();
                    _context.Dispose();
                }
                if (_controller != null)
                {
                    _controller.Dispose();
                }
            }
        }
    }
}