using Microsoft.AspNetCore.Mvc;
using TGTOAT.Data;
using TGTOAT.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace TGTOAT.Controllers
{
    public class CourseRegistrationController : Controller
    {
        private readonly UserContext _context;

        public CourseRegistrationController(UserContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> CourseRegistration()
        {
                var viewModel = new CourseRegisterViewModel
                {
                    Departments = _context.Departments.ToList(), // Make sure this is not null
                    Courses = _context.Courses.ToList() // Optional, depending on your needs
                };
                return View(viewModel);
        }
    }
}
