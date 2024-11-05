
using System.Security.Cryptography;
using Data;
using Microsoft.EntityFrameworkCore;
using Models;
using System.Collections.Generic;


namespace TGTOAT.Helpers;

public class Authentication : IAuthentication
{
    private readonly UserContext _context;
    public Authentication(UserContext context)
    {
        _context = context;
    }

    private static CurrUser User;
    private static UserIndexViewModel CurrIndex;

    public void setUser(CurrUser UserData)
    {
        User = UserData;
    }

    public CurrUser getUser()
    {
        return User;
    }

    public void Logout()
    {
        User = null;
        CurrIndex = null;
    }

    public void setIndex()
    {

        var user = getUser();
        var courses = new List<Courses>();
        var Currcourses = new List<CurrClasses>();

        if (user.Role == "Student")
        {
            courses = (from connection in _context.StudentConnection
                       join course in _context.Courses on connection.CourseId equals course.CourseId
                       where connection.StudentId == user.UserId
                       select course).ToList();

            
        }
        else if (user.Role == "Instructor")
        {
            courses = (from connection in _context.InstructorConnection
                       join course in _context.Courses on connection.CourseId equals course.CourseId
                       where connection.InstructorId == user.UserId
                       select course).ToList();
        }

        var upcoming = new List<UpcomingAssign>();

        foreach (var c in courses)
        {
            string deptName = _context.Departments.First(d => d.DeptId == c.DeptId).DeptName;

            var assigns = (from a in _context.Assignments
                           join ci in _context.Courses on a.CourseId equals c.CourseId
                           where ci.CourseId == c.CourseId
                           select a).ToList();

            foreach (var a in assigns)
            {
                var newAssign = new UpcomingAssign
                {
                    AssignId = a.AssignId,
                    AssignName = a.AssignName,
                    CourseNum = c.CourseNum,
                    DeptName = deptName,
                    DueDate = a.DueDate,
                };

            }

            switch (deptName)
            {
                case "Compuer Science":
                    deptName = "CS";
                    break;
                case "Mathematics":
                    deptName = "MATH";
                    break;
                case "Physics":
                    deptName = "PHYS";
                    break;
                case "Biology":
                    deptName = "BIOL";
                    break;
                case "Chemistry":
                    deptName = "CHEM";
                    break;
            }

            var CourseModel = new CurrClasses
            {
                DeptName = deptName,
                CourseId = c.CourseId,
                CourseNum = c.CourseNum,
                CourseName = c.CourseName,
                Campus = c.Campus,
                Building = c.Building,
                Room = c.Room,
                Days = c.Days,
                StartTime = c.StartTime,
                StopTime = c.StopTime,
                Semester = c.Semester,
                Year = c.Year,
                Color = c.Color,
                Picture = c.Picture
            };
            Currcourses.Add(CourseModel);


        }


        var viewModel = new UserIndexViewModel
        {
            Id = user.UserId,
            FirstName = user.FirstName,
            LastName = user.LastName,
            UserRole = user.Role,
            Courses = Currcourses,
        };

        CurrIndex = viewModel;

    }

    public UserIndexViewModel getIndex()
    {
        return CurrIndex;
    }

    public string createToken(int ranNum)
    {
        var randomNumber = new byte[ranNum];
        string token = "";


        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(randomNumber);
            token = Convert.ToBase64String(randomNumber);
        }

        return token;
    }

    public void CreateNotification(string message, int studentId)
    {
        var notification = new Notifications
        {
            Message = message,
            CreatedAt = DateTime.UtcNow,
            StudentId = studentId
        };

        //_context.Notifications.Add(notification);
        _context.SaveChanges();
    }

    public IEnumerable<Notifications> GetNotificationsForUser(int studentId)
    {
        return _context.Notifications
            .Where(n => n.StudentId == studentId)
            .ToList();
    }

    public void MarkAsRead(int notificationId)
    {
        var notification = _context.Notifications.Find(notificationId);
        if (notification != null)
        {
            notification.IsRead = true;
            _context.SaveChanges();
        }
    }




}
