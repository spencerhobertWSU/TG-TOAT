using Microsoft.AspNetCore.Mvc;
using Data;
using Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using TGTOAT.Helpers;

namespace TGTOAT.Controllers
{
    public class CourseRegistrationController : Controller
    {
        private readonly UserContext _context;
        private readonly IAuthentication _auth;
        private readonly NotificationService _notificationService;

        public CourseRegistrationController(NotificationService notificationService, UserContext context, IAuthentication auth)
        {
            _context = context;
            _auth = auth;
            _notificationService = notificationService;

        }

        public async Task<IActionResult> CourseRegistration()
        {
            var user = _auth.getUser();

            var userCourses = (from db in _context.StudentConnection
                       where db.StudentId == user.UserId
                       select db).ToList();

            var viewModel = new CourseRegisterViewModel
                {

                Notifications = _notificationService.GetNotificationsForUser(user.UserId).ToList(),
                Id = user.UserId,
                    Departments = _context.Departments.ToList(), // Make sure this is not null
                    //Courses = _context.Courses.ToList(), // Optional, depending on your needs
                    StudentConnection = userCourses,
            };
                return View(viewModel);
        }
    }
}
