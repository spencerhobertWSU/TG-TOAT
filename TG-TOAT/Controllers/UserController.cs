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
using System.Security.Claims;

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
        public async Task<IActionResult> Index(UserLoginViewModel Model)
        {
            var user = _auth.CheckUser();//Grab User Info

            //Don't let user go to index if they aren't logged in
            if (user == null)
            {
                return RedirectToAction("Login");
            }

            //This is used to populate the students main menu with course cards
            int userId = int.TryParse(user.Id, out int tempId) ? tempId : 0;

            var courses = (from connection in _context.StudentCourseConnection
                           join course in _context.Courses on connection.CourseId equals course.CourseId
                           where connection.StudentID == userId
                               select course).ToList();

            var viewModel = new UserLoginViewModel
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                UserRole = user.UserRole,
                Courses = courses,
                Id = user.Id.ToString(),
            };

            //Go to index
            return View(viewModel);



        }

        // Calendar action
        public IActionResult Calendar()
        {
            var user = _auth.CheckUser();//Grab User Info
            return View(user); // This will look for Views/User/Calendar.cshtml
        }
       
        //Course Registration action
        public async Task<IActionResult> CourseRegistration()
        {
            // Get all user-course connections
            var userCourseConnections = await _context.UserCourseConnection.ToListAsync();

            //Get unique course IDs from the user-course connections
            var instructorCourseIds = userCourseConnections
                .Select(uc => uc.CourseId)
                .Distinct()
                .ToList();

            // Get unique user IDs from the user-course connections for the selected courses
            var userIds = userCourseConnections
                .Where(uc => instructorCourseIds.Contains(uc.CourseId))
                .Select(uc => uc.UserId)
                .Distinct()
                .ToList();

            //  Retrieve the instructors associated with those User IDs
            var instructors = await _context.User
                .Where(u => u.UserRole == "Instructor" && userIds.Contains(u.Id))
                .ToListAsync();

            // Get Courses that has a instructorCourseId
            var courses = await _context.Courses
               .Where(c => instructorCourseIds.Contains(c.CourseId))
               .Include(c => c.Instructors) // Assuming there's a navigation property for instructors
               .ToListAsync();

            //// Create the view model to pass to the view
            var viewModel = new CourseRegisterViewModel
            {
                Departments = await _context.Departments.ToListAsync(),
                Courses = courses,
                Instructors = instructors,

                UserCourseConnections = userCourseConnections
            };
            ViewBag.ErrorMessage = TempData["ErrorMessage"] as string;
            ViewBag.SuccessMessage = TempData["SuccessMessage"] as string;
            return View(viewModel); // Pass the populated view model to the view
        }

        public async Task<IActionResult> FilterCourses(int? departmentId, string searchTerm)
        {
            // Fetch all departments for the dropdown
            var departments = await _context.Departments.ToListAsync();

            // Fetch all user-course connections
            var userCourseConnections = await _context.UserCourseConnection.ToListAsync();

            // Get unique course IDs from user-course connections
            var instructorCourseIds = userCourseConnections
                .Select(uc => uc.CourseId)
                .Distinct()
                .ToList();

            // Get courses that match the instructor course IDs
            var courses = await _context.Courses
                .Where(c => instructorCourseIds.Contains(c.CourseId))
                .Include(c => c.Instructors)
                .ToListAsync();

            // Filter based on selected department
            if (departmentId.HasValue)
            {
                courses = courses.Where(c => c.DepartmentId == departmentId.Value).ToList();
            }

            // Filter based on search term
            if (!string.IsNullOrEmpty(searchTerm))
            {
                courses = courses
                    .Where(c => c.CourseName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            // Create the view model to pass to the view
            var viewModel = new CourseRegisterViewModel
            {
                DepartmentId = departmentId.Value,
                Departments = departments,
                Courses = courses,
                Instructors = await _context.User.Where(u => u.UserRole == "Instructor").ToListAsync(),
                UserCourseConnections = userCourseConnections
            };

            return View("CourseRegistration", viewModel); // Return the view with the filtered courses
        }

        [HttpPost]
        public IActionResult Register(int CourseId)
        {
            var userIdString = _auth.GetCurrentUserId();
            if (string.IsNullOrEmpty(userIdString))
            {
                return Unauthorized();
            }
            if (!int.TryParse(userIdString, out int userId))
            {
                TempData["ErrorMessage"] = "Invalid user ID format.";
                return RedirectToAction("CourseRegistration");
            
            } 
            // Check if the user is already registered
            if (_context.StudentCourseConnection.Any(uc => uc.StudentID == userId && uc.CourseId == CourseId))
            {
                TempData["ErrorMessage"] = "User is already registered for this course.";
                return RedirectToAction("CourseRegistration");
            }
            var connection = new StudentCourseConnection
            {
                StudentID = userId,
                CourseId = CourseId
            };

            _context.StudentCourseConnection.Add(connection);
            _context.SaveChanges();
            TempData["SuccessMessage"] = "Successfully registered for the course!";
            return RedirectToAction("CourseRegistration"); // Redirect back to the course registration page
        }

        [HttpPost]
        public IActionResult Drop(int CourseId)
        {
            var userIdString = _auth.GetCurrentUserId();
            if (string.IsNullOrEmpty(userIdString))
            {
                return Unauthorized();
            }

            if (!int.TryParse(userIdString, out int userId))
            {
                TempData["ErrorMessage"] = "Invalid user ID format.";
                return RedirectToAction("CourseRegistration");
            }

            var connection = _context.StudentCourseConnection
                .FirstOrDefault(uc => uc.StudentID == userId && uc.CourseId == CourseId);

            if (connection != null)
            {
                _context.StudentCourseConnection.Remove(connection);
                _context.SaveChanges();
                TempData["SuccessMessage"] = "Successfully Dropped for the course!";
                return RedirectToAction("CourseRegistration"); // Redirect back to the course registration page
            }

            TempData["ErrorMessage"] = "Registration not found.";
            return RedirectToAction("CourseRegistration");
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
                var courses = (from connection in _context.StudentCourseConnection
                                    join course in _context.Courses on connection.CourseId equals course.CourseId
                                    where connection.StudentID == user.Id
                                    select course).ToList();

                var viewModel = new UserLoginViewModel
                {
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    UserRole = user.UserRole,
                    Courses = courses,
                    Id = user.Id.ToString(),
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
