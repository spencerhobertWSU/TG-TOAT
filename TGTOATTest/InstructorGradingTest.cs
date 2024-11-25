using Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGTOAT.Controllers;
using TGTOAT.Helpers;
using TGTOAT;

namespace TGTOATTest
{
    [TestClass]
    public class InstructorGradingTest
    {
        private UserContext _context;
        private CourseController _controller;
        private Mock<IAuthentication> _mockAuth;
        private NotificationService _notificationService;

        [TestMethod]
        public void InstructorGradesAssignment_SuccessfulGrading()
        {
            _mockAuth = new Mock<IAuthentication>();
            var options = new DbContextOptionsBuilder<UserContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase_Grading")
                .Options;

            _context = new UserContext(options);
            _notificationService = new NotificationService(_context);

            User instructor = new User
            {
                UserId = 1,
                Email = "instructor@weber.edu",
                Password = "Test"
            };
            UserInfo instructorInfo = new UserInfo
            {
                UserId = 1,
                FirstName = "Instructor",
                LastName = "Test",
                Role = "Instructor",
                PFP = "Test",
                BirthDate = new DateOnly(1985, 1, 1)
            };

            User student = new User
            {
                UserId = 2,
                Email = "student@weber.edu",
                Password = "Test"
            };
            UserInfo studentInfo = new UserInfo
            {
                UserId = 2,
                FirstName = "Student",
                LastName = "Test",
                Role = "Student",
                PFP = "Test",
                BirthDate = new DateOnly(2000, 1, 1)
            };

            Courses course = new Courses
            {
                CourseId = 1,
                CourseName = "Test Course",
                Credits = 3,
                Capacity = 30,
                Color = "Blue", 
                CourseDesc = "Test Course Description", 
                Semester = "Fall 2024" 
            };


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
                Submitted = DateTime.Now,
                Submission = "Test File Content"
            };

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

            _mockAuth.Setup(auth => auth.getUser()).Returns(new CurrUser
            {
                UserId = instructor.UserId,
                Email = instructor.Email,
                FirstName = instructorInfo.FirstName,
                LastName = instructorInfo.LastName,
                Role = instructorInfo.Role,
                BirthDate = instructorInfo.BirthDate,
                Notifications = new List<Notifications>()
            });

            _controller = new CourseController(_context, _mockAuth.Object, _notificationService);

            string grade = "85"; 
            _controller.UpdateSubmission(assignment.AssignId, student.UserId, grade);

            var gradedAssignment = _context.StudentAssignment
                .FirstOrDefault(sa => sa.AssignId == assignment.AssignId && sa.StudentId == student.UserId);

            Assert.IsNotNull(gradedAssignment);
            Assert.AreEqual(85, gradedAssignment.Points);
            Assert.AreEqual("Test File Content", gradedAssignment.Submission);
        }

    }
}
