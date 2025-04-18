﻿
using System.Security.Cryptography;
using Data;
using Microsoft.EntityFrameworkCore;
using Models;
using System.Collections.Generic;


namespace TGTOAT.Helpers;

public class Authentication : IAuthentication
{
    private readonly UserContext _context;
    private readonly NotificationService _notificationService;
    public Authentication(UserContext context, NotificationService notification)
    {
        _context = context;
        _notificationService = notification;
    }

    private static CurrUser User;
    private static UserIndexViewModel CurrIndex;
    private static CourseRegisterViewModel CurrReg;

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
        
        var upAssigns = new List<Assignment>();
        var upQuizzes = new List<Quizzes>();

        var ToDo = new List<UpcomingAssign>();
        


        if (user.Role == "Student")
        {
            courses = (from connection in _context.StudentConnection
                       join course in _context.Courses on connection.CourseId equals course.CourseId
                       where connection.StudentId == user.UserId
                       select course).ToList();

            upAssigns = (from a in _context.Assignments
                       join c in _context.Courses on a.CourseId equals c.CourseId
                       join sc in _context.StudentConnection on c.CourseId equals sc.CourseId
                       where sc.StudentId == user.UserId
                       where a.DueDate >= DateTime.Now
                       orderby a.DueDate 
                       select a).ToList();

            upQuizzes = (from q in _context.Quizzes
                            join c in _context.Courses on q.CourseId equals c.CourseId
                            join sc in _context.StudentConnection on c.CourseId equals sc.CourseId
                            where sc.StudentId == user.UserId
                            where q.DueDate >= DateTime.Now
                            orderby q.DueDate
                            select q).ToList();

            var submittedA = (from sa in _context.StudentAssignment
                            select sa).ToList();
            var submittedQ = (from sq in _context.StudentQuizzes
                             select sq).ToList();

            


        }
        else if (user.Role == "Instructor")
        {
            courses = (from connection in _context.InstructorConnection
                       join course in _context.Courses on connection.CourseId equals course.CourseId
                       where connection.InstructorId == user.UserId
                       select course).ToList();

            upAssigns = (from a in _context.Assignments
                         join c in _context.Courses on a.CourseId equals c.CourseId
                         join ic in _context.InstructorConnection on c.CourseId equals ic.CourseId
                         where ic.InstructorId == user.UserId
                         where a.DueDate >= DateTime.Now
                         orderby a.DueDate
                         select a).Take(5).ToList();

            upQuizzes = (from q in _context.Quizzes
                         join c in _context.Courses on q.CourseId equals c.CourseId
                         join ic in _context.InstructorConnection on c.CourseId equals ic.CourseId
                         where ic.InstructorId == user.UserId
                         where q.DueDate >= DateTime.Now
                         orderby q.DueDate
                         select q).Take(5).ToList();
        }
        //0=Assign, 1=Quiz
        var type = 0;
        int quizStart = 0;

        
        for(int i = 0; i < upAssigns.Count; i++)
        {
            for(int j = quizStart; j < upQuizzes.Count; j++)
            {
                if (upAssigns[i].DueDate > upQuizzes[j].DueDate)
                {
                    type = 1;
                    i--;
                    break;
                }
                
            }

            if(type == 0)
            {
                var newUp = new UpcomingAssign
                {
                    AssignId = upAssigns[i].AssignId,
                    Type = type,
                    CourseId = upAssigns[i].CourseId,
                    AssignName = upAssigns[i].AssignName,
                    DueDate = upAssigns[i].DueDate,

                };
                ToDo.Add(newUp);
            }
            else
            {
                var newUp = new UpcomingAssign
                {
                    AssignId = upQuizzes[quizStart].QuizId,
                    Type = type,
                    CourseId = upQuizzes[quizStart].CourseId,
                    AssignName = upQuizzes[quizStart].QuizName,
                    DueDate = upQuizzes[quizStart].DueDate,
                };
                ToDo.Add(newUp);
                type = 0;
                quizStart++;
            }
            if(ToDo.Count >= 5)
            {
                break;
            }
        }

        foreach (var up in ToDo)
        {
            var connect = _context.Courses.First(c => c.CourseId == up.CourseId);

            string deptName = _context.Departments.First(d => d.DeptId == connect.DeptId).DeptName;

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
            up.CourseNum = connect.CourseNum;
            up.DeptName = deptName;
        }


        foreach (var c in courses)
        {
            string deptName = _context.Departments.First(d => d.DeptId == c.DeptId).DeptName;

            var assigns = (from a in _context.Assignments
                           join ci in _context.Courses on a.CourseId equals c.CourseId
                           where ci.CourseId == c.CourseId
                           select a).ToList();

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
                Notifications = _notificationService.GetNotificationsForUser(user.UserId).ToList(),
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
            Notifications = _notificationService.GetNotificationsForUser(user.UserId).ToList(),
            Id = user.UserId,
            FirstName = user.FirstName,
            LastName = user.LastName,
            UserRole = user.Role,
            Courses = Currcourses,
            UpcomingAssignments = ToDo
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

    public void updateRegistration()
    {
        var user = getUser();

        var courses = (from db in _context.Courses select db).ToList();

        var Courses = new List<CourseInfo>();

        foreach (var c in courses)
        {

            var deptinfo = _context.Departments.First(d => d.DeptId == c.DeptId);

            int instructorId = _context.InstructorConnection.First(ic => ic.CourseId == c.CourseId).InstructorId;

            string instructorFName = _context.UserInfo.First(u => u.UserId == instructorId).FirstName;
            string instructorLName = _context.UserInfo.First(u => u.UserId == instructorId).LastName;

            string instructor = instructorFName + " " + instructorLName;

            var CourseModel = new CourseInfo
            {
                CourseId = c.CourseId,
                DeptID = deptinfo.DeptId,
                DeptName = deptinfo.DeptName,
                CourseNumber = c.CourseNum,
                NumberOfCredits = c.Credits,
                CourseName = c.CourseName,
                Campus = c.Campus,
                Building = c.Building,
                Room = c.Room,
                DaysOfTheWeek = c.Days,
                StartTime = c.StartTime,
                EndTime = c.StopTime,
                Capacity = c.Capacity,
                Semester = c.Semester,
                Year = c.Year,
                Instructor = instructor,
                CourseDescription = c.CourseDesc,
            };
            Courses.Add(CourseModel);

        };

        /*
        var StudentCourses = (from connection in _context.StudentConnection
                              join course in _context.Courses on connection.CourseId equals course.CourseId
                              where connection.StudentId == user.UserId
                              select connection).ToList();
        */
        var StudentCourses = (from c in _context.StudentConnection
                              where c.StudentId == user.UserId
                              select c).AsNoTracking().ToList();



        var RegisterCourses = new CourseRegisterViewModel
        {
            Courses = Courses,
            CurrentStudent = user.UserId,
            StudentConnection = StudentCourses,
            Departments = _context.Departments.ToList(),
        };

        CurrReg = RegisterCourses;
    }

    public CourseRegisterViewModel getRegistration()
    {
        return CurrReg;
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
