using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using TGTOAT.Data;
using TGTOAT.Models;
using TGTOAT.Helpers;

namespace TGTOAT.Controllers
{
    public class UserController : Controller
    {
        private readonly IPasswordHasher _passwordHasher;
        private readonly UserContext _context;

        public UserController(UserContext context, IPasswordHasher passwordHasher)
        {
            _context = context;
            _passwordHasher = passwordHasher;
        }

        // GET: User
        public async Task<IActionResult> Index()
        {
            return View();
        }
        // Calendar action
        public IActionResult Calendar()
        {
            return View(); // This will look for Views/User/Calendar.cshtml
        }

        public async Task<IActionResult> Login(string Email, string Password)
        {
            // Grab the user by email
            var user = _context.User.FirstOrDefault(m => m.Email == Email);

            // If no user is found
            if (user == null)
            {
                return RedirectToAction(nameof(Index));
            }

            // Check if the password matches
            if (_passwordHasher.Verify(user.Password, Password))
            {
                // Find all classes that the user is enrolled in
                var courses = (from connection in _context.UserCourseConnection
                                    join course in _context.Courses on connection.CourseId equals course.CourseId
                                    where connection.UserId == user.Id
                                    select course).ToList();

                var viewModel = new UserLoginViewModel
                {
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    UserRole = user.UserRole,
                    Courses = courses
                };

                // If the password is correct, log the user in
                return View(viewModel);
            }
            else
            {
                // If the password is incorrect, redirect back to login screen
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: User/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.User
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // GET: User/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: User/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RegistrationViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Calculate the user's age based on their birthdate
                var today = DateTime.Today;
                var age = today.Year - model.BirthDate.Year;
                if (model.BirthDate.Date > today.AddYears(-age)) age--;

                // Check if the user is at least 16 years old
                if (age < 16)
                {
                    ModelState.AddModelError("", "You must be at least 16 years old to register.");
                    return View(model);
                }

                // Manually map from ViewModel to User entity
                User user = new User
                {
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Password = model.Password,
                    BirthDate = model.BirthDate,
                    UserRole = model.UserRole == "Student" ? "Student" : "Instructor"
                };

                // Hash the password
                user.Password = _passwordHasher.Hash(user.Password);

                // Add the user and save changes
                _context.Add(user);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Login), new { Email = user.Email, Password = user.Password });
            }

            return View(model);
        }


        // GET: User/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.User.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        // POST: User/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Email,Password,FirstName,LastName,BirthDate")] User user)
        {
            if (id != user.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(user);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }

        // GET: User/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.User
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: User/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _context.User.FindAsync(id);
            if (user != null)
            {
                _context.User.Remove(user);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserExists(int id)
        {
            return _context.User.Any(e => e.Id == id);
        }
    }
}
