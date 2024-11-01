using Microsoft.AspNetCore.Mvc;
using TGTOAT.Data;
using TGTOAT.Models;
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

        public CourseRegistrationController(UserContext context, IAuthentication auth)
        {
            _context = context;
            _auth = auth;
        }

        public async Task<IActionResult> CourseRegistration()
        {
            var user = _auth.GetUser();
            var userCourses = (from db in _context.StudentCourseConnection
                       where db.StudentID == user.Id
                       select db).ToList();

            var viewModel = new CourseRegisterViewModel
                {

                    Id = user.Id,
                    Departments = _context.Departments.ToList(), // Make sure this is not null
                    Courses = _context.Courses.ToList(), // Optional, depending on your needs
                    StudentCourseConnection = userCourses,
            };
                return View(viewModel);
        }
    }
}
