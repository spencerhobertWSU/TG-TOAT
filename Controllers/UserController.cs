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
using System.Security.Cryptography;
using NuGet.Common;
using Stripe.Checkout;
using Stripe.V2;

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
            DeleteCookie();
            _auth.Logout();
            return RedirectToAction("Login");
        }

        //Login Page
        public ActionResult Login(LoginViewModel model)
        {
            string? Email = Request.Cookies["Email"];
            string? Series = Request.Cookies["Series"];
            string? Token = Request.Cookies["Token"];

            var user = _context.User.FirstOrDefault(m => m.Email == model.Email);

            // If no user is found
            if (user == null)
            {
                return View(model);
            }

            if (Email != null && Series != null && Token != null)
            {
                user = _context.User.FirstOrDefault(u => u.Email == Email);
                if (Series == "13" && Token == "token13")
                {
                    // Find all classes that the user is enrolled in
                    var courses = (from connection in _context.StudentCourseConnection
                                   join course in _context.Courses on connection.CourseId equals course.CourseId
                                   where connection.StudentID == user.Id
                                   select course).ToList();

                    var viewModel = new UserIndexViewModel
                    {
                        Id = user.Id,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        UserRole = user.UserRole,
                        Courses = courses,

                    };

                    _auth.SetRole(user.UserRole);
                    _auth.SetUser(viewModel);
                    return Redirect("User/Index");
                }
                else
                {
                    return View(model);
                }
            }

            // Check if the password matches
            if (_passwordHasher.Verify(user.Password, model.Password))
            {
                // Find all classes that the user is enrolled in
                var courses = (from connection in _context.StudentCourseConnection
                               join course in _context.Courses on connection.CourseId equals course.CourseId
                               where connection.StudentID == user.Id
                               select course).ToList();

                var viewModel = new UserIndexViewModel
                {
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    UserRole = user.UserRole,
                    Courses = courses,
                    Id = user.Id,
                };

                // If the password is correct, log the user in
                _auth.SetRole(user.UserRole);
                _auth.SetUser(viewModel);

                if (model.RememberMe == true)
                {
                    string seriesIden = _auth.createToken(16);

                    string token = _auth.createToken(32);

                    seriesIden = "13";
                    token = "token13";

                    //_context.Cookies.Add(seriesIden, token);

                    CreateCookie(user.Email, seriesIden, token);
                }

                return Redirect("User/Index");
            }
            else
            {
                // If the password is incorrect, redirect back to login screen
                return View(model);
            }
        }

        // Home page for User
        public async Task<IActionResult> Index(UserIndexViewModel Model)
        {
            var user = _auth.GetUser();//Grab User Info

            //Don't let user go to index if they aren't logged in
            if (user == null)
            {
                return RedirectToAction("Login");
            }

            //This is used to populate the students main menu with course cards
            int userId = user.Id;

            var courses = new List<Courses>();
            if (user.UserRole == "Student")
            {
                courses = (from connection in _context.StudentCourseConnection
                               join course in _context.Courses on connection.CourseId equals course.CourseId
                               where connection.StudentID == userId
                               select course).ToList();
            }
            else if (user.UserRole == "Instructor")
            {
                courses = (from connection in _context.InstructorCourseConnection
                           join course in _context.Courses on connection.CourseId equals course.CourseId
                           where connection.InstructorID == userId
                           select course).ToList();
            }
            

            var viewModel = new UserIndexViewModel
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                UserRole = user.UserRole,
                Courses = courses,
                Id = user.Id,
            };

            //Go to index
            return View(viewModel);

        }

        // instructor Calendar action
        public IActionResult Calendar()
        {
            var user = _auth.GetUser();//Grab User Info
            return View(user); // This will look for Views/User/Calendar.cshtml
        }
        
        //student calendar action
        public IActionResult StudentCalendar()
        {
            return View();
        }

        #region Course Registration
        //Course Registration action
        public async Task<IActionResult> CourseRegistration()
        {
            // Get all user-course connections
            var instructorCourseConnections = await _context.InstructorCourseConnection.ToListAsync();

            //Get unique course IDs from the user-course connections
            var instructorCourseIds = instructorCourseConnections
                .Select(uc => uc.CourseId)
                .Distinct()
                .ToList();

            // Get unique user IDs from the user-course connections for the selected courses
            var userIds = instructorCourseConnections
                .Where(uc => instructorCourseIds.Contains(uc.CourseId))
                .Select(uc => uc.InstructorID)
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

            var userIdInt = _auth.GetCurrentUserId();
            //// Create the view model to pass to the view
            var viewModel = new CourseRegisterViewModel
            {
                Departments = await _context.Departments.ToListAsync(),
                Courses = courses,
                Instructors = instructors,
                CurrentStudent = userIdInt,
                StudentCourseConnection = await _context.StudentCourseConnection.ToListAsync(),
                InstructorCourseConnections = instructorCourseConnections


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
            var instructorCourseConnections = await _context.InstructorCourseConnection.ToListAsync();

            // Get unique course IDs from user-course connections
            var instructorCourseIds = instructorCourseConnections
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
            var userIdInt = _auth.GetCurrentUserId();
            // Create the view model to pass to the view
            var viewModel = new CourseRegisterViewModel
            {
                DepartmentId = departmentId ?? 0,
                Departments = departments,
                Courses = courses,
                CurrentStudent = userIdInt,
                Instructors = await _context.User.Where(u => u.UserRole == "Instructor").ToListAsync(),
                InstructorCourseConnections = instructorCourseConnections,
                StudentCourseConnection = await _context.StudentCourseConnection.ToListAsync(),
            };

            return View("CourseRegistration", viewModel); // Return the view with the filtered courses
        }

        [HttpPost]
        public IActionResult Register(int CourseId)
        {
            var userIdInt = _auth.GetCurrentUserId();
            var userIdString = userIdInt.ToString();
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

            // Add dues to the users account
            var user = _context.User.FirstOrDefault(u => u.Id == userId);
            user.AmountDue += 100 * _context.Courses.FirstOrDefault(u => u.CourseId == CourseId).NumberOfCredits;

            _context.User.Update(user);
            _context.StudentCourseConnection.Add(connection);
            _context.SaveChanges();
            TempData["SuccessMessage"] = "Successfully registered for the course!";
            return RedirectToAction("CourseRegistration"); // Redirect back to the course registration page
        }

        [HttpPost]
        public IActionResult Drop(int CourseId)
        {
            var userIdInt = _auth.GetCurrentUserId();
            var userIdString = userIdInt.ToString();
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
                // Remove dues from the users account (connection always had null User and Courses, so I did it this way)
                var user = _context.User.FirstOrDefault(u => u.Id == userId);
                var course = _context.Courses.FirstOrDefault(c => c.CourseId == CourseId);
                user.AmountDue -= 100 * course.NumberOfCredits;

                _context.User.Update(user);
                _context.StudentCourseConnection.Remove(connection);
                _context.SaveChanges();
                TempData["SuccessMessage"] = "Successfully Dropped for the course!";
                return RedirectToAction("CourseRegistration"); // Redirect back to the course registration page
            }

            TempData["ErrorMessage"] = "Registration not found.";
            return RedirectToAction("CourseRegistration");
        }
        #endregion
        

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
                    AddressLineOne = null,
                    AddressLineTwo = null,
                    //City = null,
                    //State = null,
                    ZipCode = null
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
        public async Task<IActionResult> Account()
        {
            var User = _auth.GetUser();

            if (User == null)
            {
                return RedirectToAction("Login");
            }

            var UserAdd = _context.Address.First(a => a.UserId == User.Id);

            AccountViewModel UserInfo = new AccountViewModel
            {
                UserRole = User.UserRole,
                FirstName = User.FirstName,
                LastName = User.LastName,
                Address = UserAdd

            };

            return View(UserInfo);
        }

        // POST: User/UpdateUser
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateUser(AccountViewModel model)
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

            return RedirectToAction("Account");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateAdd(AccountViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = _auth.GetUser();
                var userAdd = _context.Address.First(a => a.UserId == model.Id);

                userAdd.AddressLineOne = model.Address.AddressLineOne;
                userAdd.AddressLineTwo = model.Address.AddressLineTwo;
                userAdd.ZipCode = model.Address.ZipCode;



                _context.Address.Update(userAdd);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Account");
        }

        public IActionResult GetProfileImage()
        {
            var userLoginInfo = _auth.GetUser();
            var fullUser = _context.User.Find(userLoginInfo.Id);

            var imageBytes = Convert.FromBase64String("");


            if (fullUser == null || string.IsNullOrEmpty(fullUser.ProfileImageBase64))
            {
                imageBytes = Convert.FromBase64String("+IMQ3yEZtXwBVkKazXUlLCAZV4UKaXKsOMIc4olDFdJo/FbADOKRCZ6th3yFeOj4PqRBBA4hnvrwEFKvL11APHW9GjLxOOT2PqROCOYQT81XQ8RnDyG12pJ47H9INkNqhEB8JoT8BNEuPSTVExuQAAAAAElFTkSuQmCC");
            }
            else
            {

                imageBytes = Convert.FromBase64String(fullUser.ProfileImageBase64);

            }

            return File(imageBytes, "image/png");
        }

        // GET: /User/UploadImage
        [HttpGet]
        public async Task<IActionResult> ChangeProfilePicture()
        {
            //Grab user log in info
            var userLoginInfo = _auth.GetUser();
            if (userLoginInfo == null)
            {
                return RedirectToAction("Login");
            }

            //Grab full user info
            var user = await _context.User
                .Include(u => u.Address)
                .FirstOrDefaultAsync(m => m.Id == userLoginInfo.Id);
            if (user == null)
            {
                return NotFound();
            }


            return View(user);
        }




        // POST: /User/UploadImage
        [HttpPost]
        public async Task<IActionResult> ChangeProfilePicture(IFormFile profileImage)
        {
            var userLoginInfo = _auth.GetUser();
            var fullUser = await _context.User.FindAsync(userLoginInfo.Id);

            if (profileImage != null && fullUser != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await profileImage.CopyToAsync(memoryStream);

                    byte[] imageBytes = memoryStream.ToArray();

                    string base64String = Convert.ToBase64String(imageBytes);

                    fullUser.ProfileImageBase64 = base64String;

                    _context.Update(fullUser);
                    await _context.SaveChangesAsync();
                }

            }

            return View(fullUser);
        }


        public void CreateCookie(String Email, String Series, String Token)
        {
            CookieOptions options = new CookieOptions
            {
                //Domain = "example.com", // Set the domain for the cookie
                Expires = DateTime.Now.AddDays(7), // Set expiration date to 7 days from now
                Path = "/", // Cookie is available within the entire application
                Secure = true, // Ensure the cookie is only sent over HTTPS
                HttpOnly = true, // Prevent client-side scripts from accessing the cookie
                MaxAge = TimeSpan.FromDays(7), // Another way to set the expiration time
                IsEssential = true // Indicates the cookie is essential for the application to function
            };
            Response.Cookies.Append("Email", Email, options);
            Response.Cookies.Append("Series", Series, options);
            Response.Cookies.Append("Token", Token, options);
        }

        public void DeleteCookie()
        {
            CookieOptions options = new CookieOptions
            {
                //Domain = "example.com", // Set the domain for the cookie
                Expires = DateTime.Now.AddDays(7), // Set expiration date to 7 days from now
                Path = "/", // Cookie is available within the entire application
                Secure = true, // Ensure the cookie is only sent over HTTPS
                HttpOnly = true, // Prevent client-side scripts from accessing the cookie
                MaxAge = TimeSpan.FromDays(7), // Another way to set the expiration time
                IsEssential = true // Indicates the cookie is essential for the application to function
            };
            Response.Cookies.Append("Email", "", options);
            Response.Cookies.Append("Series", "", options);
            Response.Cookies.Append("Token", "", options);
        }

        // GET: /User/Payment
        [HttpGet]
        public IActionResult Payment(bool? didSucceed)
        {
            // Check if the payment was successful (or if it went through at all)
            if (didSucceed.HasValue)
            {
                ViewBag.SuccessMessage = didSucceed.Value ? "Payment successful!" : "Payment failed.";
            }

            // Get user and course information
            var user = _context.User.FirstOrDefault(u => u.Id == _auth.GetCurrentUserId());
            var courses = (from connection in _context.StudentCourseConnection
                           join course in _context.Courses on connection.CourseId equals course.CourseId
                           where connection.StudentID == user.Id
                           select course).ToList();

            decimal totalPaid = 0;
            foreach (var course in courses)
            {
                totalPaid += course.NumberOfCredits * 100;
            }
            totalPaid = (user.AmountDue - totalPaid) * -1; // For some reason if I don't multiply it by 1 it shows up with () around the number

            // Create the view model
            var viewModel = new PaymentViewModel
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Courses = courses,
                AmountDue = user.AmountDue,
                AmountPaid = totalPaid
            };

            return View(viewModel);
        }

        // Checkout page
        public ActionResult Checkout(decimal amount)
        {
            // Check if the amount is valid
            var amountDue = _context.User.FirstOrDefault(u => u.Id == _auth.GetCurrentUserId()).AmountDue;
            if (amount <= 0 || amount > amountDue)
            {
                return RedirectToAction("PaymentFailure", "User");
            }

            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = new List<SessionLineItemOptions>
            {
                new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        Currency = "usd",
                        UnitAmount = (long)(amount * 100), // For some reason Stripe requires the amount in cents
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = "Course Payment",
                        },
                    },
                    Quantity = 1,
                },
            },
                Mode = "payment",
                SuccessUrl = Url.Action("PaymentSuccess", "User", new { amountPaid = amount }, Request.Scheme),
                CancelUrl = Url.Action("PaymentFailure", "User", null, Request.Scheme),
            };

            var service = new SessionService();
            Session session = service.Create(options);

            return Redirect(session.Url);
        }

        // If the payment succeeded
        public ActionResult PaymentSuccess()
        {
            // Grab the amount paid
            var amountPaid = decimal.Parse(Request.Query["amountPaid"]);

            // Update the user's balance
            var user = _context.User.FirstOrDefault(u => u.Id == _auth.GetCurrentUserId());
            user.AmountDue -= amountPaid;
            _context.User.Update(user);
            _context.SaveChanges();

            return RedirectToAction("Payment", "User", new { didSucceed = true });
        }

        // If the payment failed
        public ActionResult PaymentFailure()
        {
            return RedirectToAction("Payment", "User", new { didSucceed = false });
        }

    }
}
