using Microsoft.EntityFrameworkCore;
using Data;
using TGTOAT;
using TGTOAT.Controllers;
using Moq;
using TGTOAT.Helpers;


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

            // Create department
            Departments department = new Departments
            {
                DeptId = 1,
                DeptName = "Test Department"
            };

            // Create course
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

            // Connect student to course
            StudentConnection studentCourseConnection = new StudentConnection
            {
                StudentId = student.UserId,
                CourseId = course.CourseId
            };

            // Save to database
            _context.User.Add(student);
            _context.UserInfo.Add(studentInfo);
            _context.Courses.Add(course);
            _context.StudentConnection.Add(studentCourseConnection);
            _context.SaveChanges();

            // Create controller
            _controller = new CourseController(_context, _mockAuth.Object);

            // Act - Drop the course by removing the connection
            var connection = _context.StudentConnection
                .FirstOrDefault(scc => scc.StudentId == student.UserId && scc.CourseId == course.CourseId);
            _context.StudentConnection.Remove(connection);
            _context.SaveChanges();

            // Assert
            var studentCourseAfterDrop = _context.StudentConnection
                .FirstOrDefault(scc => scc.StudentId == student.UserId && scc.CourseId == course.CourseId);
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
            Assert.IsNotNull(instructor, "Instructor is null");

            // Create department and initial course
            Departments department = new Departments
            {
                DeptId = 1,
                DeptName = "Test Department"
            };
            Assert.IsNotNull(department, "Department is null");

            Courses course = new Courses
            {
                CourseId = 1,
                DeptId = department.DeptId,
                CourseNum = 1010,
                CourseName = "Original Course Name",
                CourseDesc = "Original Description",
                Credits = 3,
                Capacity = 30,
                Campus = "Ogden",
                Building = "Science Lab",
                Room = 101,
                Days = "MWF",
                StartTime = new TimeOnly(9, 0),
                StopTime = new TimeOnly(10, 0),
                Semester = "Fall",
                Year = 2024,
                Color = "#FF0000",
                Picture = "Test"
            };
            Assert.IsNotNull(course, "Course is null");

            // Connect instructor to course
            InstructorConnection instructorCourseConnection = new InstructorConnection
            {
                InstructorId = instructor.UserId,
                CourseId = course.CourseId
            };
            Assert.IsNotNull(instructorCourseConnection, "InstructorConnection is null");

            try
            {
                // Save to database
                _context.Departments.Add(department);
                _context.User.Add(instructor);
                _context.UserInfo.Add(instructorInfo);
                _context.Courses.Add(course);
                _context.InstructorConnection.Add(instructorCourseConnection);
                _context.SaveChanges();

                // Verify entities were saved
                Assert.IsTrue(_context.Departments.Any(), "No departments in database");
                Assert.IsTrue(_context.User.Any(), "No users in database");
                Assert.IsTrue(_context.Courses.Any(), "No courses in database");
                Assert.IsTrue(_context.InstructorConnection.Any(), "No instructor connections in database");

                // Create controller
                _controller = new CourseController(_context, _mockAuth.Object);
                Assert.IsNotNull(_controller, "Controller is null");

                // Act - Update course properties
                var updatedCourse = _context.Courses
                    .FirstOrDefault(c => c.CourseId == course.CourseId);

                Assert.IsNotNull(updatedCourse, "Failed to retrieve course from database");

                updatedCourse.CourseNum = 2020;
                updatedCourse.CourseName = "Updated Course Name";
                updatedCourse.CourseDesc = "Updated Description";
                updatedCourse.Credits = 4;
                updatedCourse.Capacity = 40;
                updatedCourse.Campus = "Davis";
                updatedCourse.Building = "Engineering";
                updatedCourse.Room = 202;
                updatedCourse.Days = "TR";
                updatedCourse.StartTime = new TimeOnly(13, 30);
                updatedCourse.StopTime = new TimeOnly(14, 45);
                updatedCourse.Semester = "Spring";
                updatedCourse.Year = 2025;
                updatedCourse.Color = "#00FF00";
                updatedCourse.Picture = "Updated Test";
                _context.SaveChanges();

                // Assert - Check all properties
                var courseAfterUpdate = _context.Courses
                    .FirstOrDefault(c => c.CourseId == course.CourseId);

                Assert.IsNotNull(courseAfterUpdate, "Failed to retrieve updated course");

                Assert.AreEqual(2020, courseAfterUpdate.CourseNum, "Course number should be updated");
                Assert.AreEqual("Updated Course Name", courseAfterUpdate.CourseName, "Course name should be updated");
                Assert.AreEqual("Updated Description", courseAfterUpdate.CourseDesc, "Course description should be updated");
                Assert.AreEqual(4, courseAfterUpdate.Credits, "Number of credits should be updated");
                Assert.AreEqual(40, courseAfterUpdate.Capacity, "Capacity should be updated");
                Assert.AreEqual("Davis", courseAfterUpdate.Campus, "Campus should be updated");
                Assert.AreEqual("Engineering", courseAfterUpdate.Building, "Building should be updated");
                Assert.AreEqual(202, courseAfterUpdate.Room, "Room number should be updated");
                Assert.AreEqual("TR", courseAfterUpdate.Days, "Days of the week should be updated");
                Assert.AreEqual(new TimeOnly(13, 30), courseAfterUpdate.StartTime, "Start time should be updated");
                Assert.AreEqual(new TimeOnly(14, 45), courseAfterUpdate.StopTime, "End time should be updated");
                Assert.AreEqual("Spring", courseAfterUpdate.Semester, "Semester should be updated");
                Assert.AreEqual(2025, courseAfterUpdate.Year, "Year should be updated");
                Assert.AreEqual("#00FF00", courseAfterUpdate.Color, "Color should be updated");
                Assert.AreEqual("Updated Test", courseAfterUpdate.Picture, "Picture should be updated");

                // Verify department and instructor relationships remain intact
                Assert.AreEqual(department.DeptId, courseAfterUpdate.DeptId, "Department relationship should remain unchanged");
                Assert.IsTrue(_context.InstructorConnection.Any(ic => 
                    ic.InstructorId == instructor.UserId && 
                    ic.CourseId == courseAfterUpdate.CourseId), 
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