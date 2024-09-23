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
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace TGTOAT.Controllers
{
    public class UserController : Controller
    {
        private readonly IPasswordHasher _passwordHasher;
        private readonly UserContext _context;
        private readonly IAuthentication _auth;

        public UserController(UserContext context, IPasswordHasher passwordHasher, IAuthentication auth)
        {
            _context = context;
            _passwordHasher = passwordHasher;
            _auth = auth;
        }

        //Logout User and Go to Login page
        public ActionResult Logout()
        {
            _auth.Logout();
            return RedirectToAction("Login");
        }

        // Home page for User
        public async Task<IActionResult> Index()
        {
            var user = _auth.CheckUser();//Grab User Info

            //Don't let user go to index if they aren't logged in
            if (user == null)
            {
                return RedirectToAction("Login");
            }

            //Go to index
            return View(user);
        }

        // Calendar action
        public IActionResult Calendar()
        {
            var user = _auth.CheckUser();//Grab User Info
            return View(user); // This will look for Views/User/Calendar.cshtml
        }

        public ActionResult Login(LoginViewModel model)
        {
            // Grab the user by email
            var user = _context.User.FirstOrDefault(m => m.Email == model.Email);

            // If no user is found
            if (user == null)
            {
                return View();
            }

            // Check if the password matches
            if (_passwordHasher.Verify(user.Password, model.Password))
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
                _auth.SetUser(viewModel);
                return Redirect("User/Index");
            }
            else
            {
                // If the password is incorrect, redirect back to login screen
                return View();
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

                Address address = new Address
                {
                    UserId = user.Id,
                    AddressLineOne = "Address Line One",
                    AddressLineTwo = "Address Line Two",
                    ZipCode = "Zip Code"
                };

                // Add the Address and save changes
                _context.Address.Add(address);


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


















        // GET: UserAccount
        public async Task<IActionResult> Account(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.User
                .Include(u => u.Address) // Include the Address navigation property
                .FirstOrDefaultAsync(m => m.Id == id);

            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }






        // POST: User/UpdateUser
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateUser(UserUpdateViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Find the existing user
                    var existingUser = await _context.User.Include(u => u.Address).FirstOrDefaultAsync(u => u.Id == model.Id);
                    if (existingUser == null)
                    {
                        return NotFound();
                    }

                    // Update only the fields you want to change
                    existingUser.FirstName = model.FirstName;
                    existingUser.LastName = model.LastName;

                    // Update Address if it exists
                    if (model.Address != null)
                    {
                        // If the user's Address is null, create a new one
                        if (existingUser.Address == null)
                        {
                            existingUser.Address = new Address
                            {
                                UserId = existingUser.Id
                            };
                        }

                        existingUser.Address.AddressLineOne = model.Address.AddressLineOne;
                        existingUser.Address.AddressLineTwo = model.Address.AddressLineTwo;
                        existingUser.Address.ZipCode = model.Address.ZipCode;

                        _context.Update(existingUser.Address);
                    }

                    // Save changes
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Account), new { id = existingUser.Id });
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(model.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            return RedirectToAction(nameof(Account), new { id = model.Id });
        }



    }
}
