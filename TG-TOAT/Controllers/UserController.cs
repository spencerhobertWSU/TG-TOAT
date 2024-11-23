using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using TGTOAT.Helpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Security.Cryptography;
using NuGet.Common;
using Stripe.Checkout;
using Stripe.V2;
using Stripe;
using static TGTOAT.Helpers.Authentication;
using Data;
using Stripe.Issuing;
using Models;
using Azure;
using System.Drawing;
using OpenQA.Selenium.BiDi.Modules.Script;
using Microsoft.CodeAnalysis.FlowAnalysis.DataFlow;

namespace TGTOAT.Controllers
{
    public class UserController : Controller
    {
        private readonly IPasswordHasher _passwordHasher;
        private readonly UserContext _context;
        private readonly IAuthentication _auth;
        private readonly NotificationService _notificationService;

        public UserController(NotificationService notification, UserContext context, IPasswordHasher passwordHasher, IAuthentication auth)
        {
            _notificationService = notification;
            _context = context;
            _passwordHasher = passwordHasher;
            _auth = auth;
        }
        private Random rnd = new Random();

        public IActionResult Privacy()
        {
            return View();
        }

        #region Account Creation
        public IActionResult Create()
        {
            return View();
        }


        // POST: User/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserCreateModel model)
        {
            if (ModelState.IsValid)
            {
                // Calculate the user's age based on their birthdate
                var today = DateTime.Today;
                var age = today.Year - model.BirthDate.Year;

                // Check if the user is at least 16 years old
                if (age < 16)
                {
                    ModelState.AddModelError("", "You must be at least 16 years old to register.");
                    return View(model);
                }

                var emails = (from e in _context.User select e.Email).ToList();

                for (int i = 0; i < emails.Count; i++)
                {
                    if (emails[i] == model.Email)
                    {
                        ModelState.AddModelError("", "Email already registered");
                        return View(model);
                    }
                }

                // Manually map from ViewModel to User entity
                User user = new User
                {
                    Email = model.Email,
                    Password = model.Password
                };

                // Hash the password
                user.Password = _passwordHasher.Hash(user.Password);

                // Add the user and save changes
                _context.User.Add(user);
                await _context.SaveChangesAsync();

                string pfpPath = "wwwroot/resources/default-pfp.png";

                string base64String = ConvertToBase64(pfpPath);


                UserInfo newInfo = new UserInfo
                {
                    UserId = user.UserId,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Role = model.UserRole == "Student" ? "Student" : "Instructor",
                    PFP = base64String,
                    BirthDate = model.BirthDate,
                };

                // Add the User Info
                _context.UserInfo.Add(newInfo);

                Addresses newaddress = new Addresses
                {
                    UserId = user.UserId,
                    AddOne = null,
                    AddTwo = null,
                    City = null,
                    State = null,
                    Zip = null
                };


                // Add the Address
                _context.Address.Add(newaddress);



                if(model.UserRole == "Student")
                {
                    Tuition newTuition = new Tuition
                    {
                        UserId = user.UserId,
                        AmountDue = 0
                    };
                    _context.Tuition.Add(newTuition);

                }

                await _context.SaveChangesAsync();




                //return RedirectToAction(nameof(Login), new { Email = user.Email, Password = user.Password });
                return RedirectToAction("Login");
            }

            return View(model);
        }
        #endregion

        #region Login/Logout

        //Logout User and Go to Login page
        public ActionResult Logout()
        {
            DeleteCookie();
            _auth.Logout();
            return RedirectToAction("Login");
        }

        //Login Page - Landing Page
        public ActionResult Login()
        {
            string? Email = Request.Cookies["Email"];
            string? Series = Request.Cookies["Series"];
            string? Token = Request.Cookies["Token"];


            var cookies = new Cookies();

            if (Email != null && Series != null && Token != null)
            {
                var user = _context.User.FirstOrDefault(u => u.Email == Email);

                if(user == null)
                {
                    return RedirectToAction("Logout");
                }
                    try
                    {
                        cookies = _context.Cookies.First(c => c.UserId == user.UserId);
                    }
                    catch
                    {
                        DeleteCookie();
                        return View("Login");
                    }

                if (cookies.Series == "13" && cookies.Token == "token13")
                {

                    if (user == null)
                    {
                        return RedirectToAction("Login");
                    }

                    var userInfo = _context.UserInfo.First(db => db.UserId == user.UserId);
                    var userPFP = Convert.FromBase64String(userInfo.PFP);

                    var User = new CurrUser
                    {
                        Notifications = _notificationService.GetNotificationsForUser(user.UserId).ToList(),
                        UserId = user.UserId,
                        Email = user.Email,
                        FirstName = userInfo.FirstName,
                        LastName = userInfo.LastName,
                        Role = userInfo.Role,
                        PFP = userPFP,
                        BirthDate = userInfo.BirthDate,
                    };

                    _auth.setUser(User);
                    _auth.setIndex();

                    return Redirect("User/Index");
                }
                else
                {
                    return View("Login");
                }
            }
            else
            {
                return View("Login");
            }
        }

        //Login Page - Pressed Login
        [HttpPost]
        public ActionResult Login(LoginViewModel model)
        {
            var user = _context.User.FirstOrDefault(m => m.Email == model.Email);


            if (user == null)
            {
                return RedirectToAction("Login");
            }

            var userInfo = _context.UserInfo.First(db => db.UserId == user.UserId);
            var userPFP = Convert.FromBase64String(userInfo.PFP);

            var User = new CurrUser
            {
                Notifications = _notificationService.GetNotificationsForUser(user.UserId).ToList(),
                UserId = user.UserId,
                Email = user.Email,
                FirstName = userInfo.FirstName,
                LastName = userInfo.LastName,
                Role = userInfo.Role,
                PFP = userPFP,
                BirthDate = userInfo.BirthDate,
            };

            // Check if the password matches
            if (_passwordHasher.Verify(user.Password, model.Password))
            {

                if (model.RememberMe == true)
                {
                    string seriesIden = _auth.createToken(16);
                    string token = _auth.createToken(32);

                    var newCookie = new Cookies
                    {
                        //Series = seriesIden,
                        //Token = token
                        UserId = user.UserId,
                        Series = "13",
                        Token = "token13"
                    };

                    _context.Cookies.Add(newCookie);
                    _context.SaveChangesAsync();

                    CreateCookie(user.Email, seriesIden, token);
                }
                _auth.setUser(User);

                return Redirect("User/Index");
            }
            else
            {
                // If the password is incorrect, redirect back to login screen
                return View(model);
            }
        }

        public ActionResult Notifications()
        {
            var user = _auth.getUser(); // Get the current user's object
            int userId = Convert.ToInt32(user);
            var notifications = _auth.GetNotificationsForUser(userId);
            return View(notifications);
        }

        // Home page for User
        public async Task<IActionResult> Index(UserIndexViewModel Model)
        {
            var user = _auth.getUser();

            if (user == null)
            {
                return RedirectToAction("Login");
            }
            _auth.setIndex();
            var viewModel = _auth.getIndex();
            //Go to index
            return View(viewModel);

        }
        #endregion

        #region Calendar
        // instructor Calendar action
        public IActionResult Calendar()
        {
            var user = _auth.getUser();

            if (user == null)
            {
                return RedirectToAction("Login");
            }

            return View(_auth.getIndex()); // This will look for Views/User/Calendar.cshtml
        }
        
        //student calendar action
        public IActionResult StudentCalendar()
        {
            return View();
        }

        #endregion

        #region Course Registration
        //Course Registration action
        public async Task<IActionResult> CourseRegistration()
        {
            var user = _auth.getUser();

            if (user == null)
            {
                return RedirectToAction("Login");
            }

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

            ViewBag.ErrorMessage = TempData["ErrorMessage"] as string;
            ViewBag.SuccessMessage = TempData["SuccessMessage"] as string;
            //ViewBag.InfoMessage = 

            return View(RegisterCourses); // Pass the populated view model to the view

        }
        public async Task<IActionResult> FilterCourses(int? departmentId, string searchTerm)
        {
            var user = _auth.getUser();//Grab User Info

            if (user == null)
            {
                return RedirectToAction("Login");
            }

            int userId = user.UserId;
            // Fetch all departments for the dropdown
            var departments = await _context.Departments.ToListAsync();

            // Fetch all user-course connections
            var instructorCourseConnections = await _context.InstructorConnection.ToListAsync();

            // Get unique course IDs from user-course connections
            var instructorCourseIds = instructorCourseConnections
                .Select(uc => uc.CourseId)
                .Distinct()
                .ToList();

            //Get courses that match the instructor course IDs
            var courses = await _context.Courses
                .Where(c => instructorCourseIds.Contains(c.CourseId))
                .ToListAsync();

            if (!departmentId.HasValue && string.IsNullOrEmpty(searchTerm))
            {
                return RedirectToAction("CourseRegistration");
            }
            var allCourses = (from db in _context.Courses select db).ToList();

            // Filter based on selected department
            if (departmentId.HasValue)
            {
                courses = (from db in _context.Courses where db.DeptId == departmentId select db).ToList();
            }
            else
            {
                courses = allCourses;
            }

            // Filter based on search term
            if (!string.IsNullOrEmpty(searchTerm))
            {
                courses = courses
                    .Where(c => c.CourseName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }



            var userIdInt = _auth.getUser().UserId;

            // Create the view model to pass to the view
            

            var Courses = new List<CourseInfo>();

            foreach (var c in courses)
            {

                var deptinfo = _context.Departments.First(d => d.DeptId == c.DeptId);

                var CourseModel = new CourseInfo
                {
                    Notifications = _notificationService.GetNotificationsForUser(user.UserId).ToList(),
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

                    CourseDescription = c.CourseDesc,
                };
                Courses.Add(CourseModel);

            };

            var RegisterCourses = new CourseRegisterViewModel
            {
                Notifications = _notificationService.GetNotificationsForUser(user.UserId).ToList(),
                Courses = Courses,
                CurrentStudent = _auth.getUser().UserId,
                Departments = _context.Departments.ToList(),
                DeptId = (int)departmentId
            };

            return View("CourseRegistration", RegisterCourses); // Return the view with the filtered courses
        }

        [HttpPost]
        public IActionResult Register(int CourseId)
        {
            var userIdInt = _auth.getUser().UserId;

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
            if (_context.StudentConnection.Any(uc => uc.StudentId == userId && uc.CourseId == CourseId))
            {
                TempData["ErrorMessage"] = "User is already registered for this course.";
                return RedirectToAction("CourseRegistration");
            }

            //string randomColor = (Color.FromArgb(rnd.Next(256), rnd.Next(256), rnd.Next(256))).ToString();

            var connection = new StudentConnection
            {
                StudentId = userId,
                CourseId = CourseId,
                Grade = 0
            };

            // Add dues to the users account
            var user = _context.User.FirstOrDefault(u => u.UserId == userId);

            var tuition = _context.Tuition.First(t => t.UserId == userId);

            tuition.AmountDue += 100 * _context.Courses.FirstOrDefault(u => u.CourseId == CourseId).Credits;

            _context.User.Update(user);
            _context.StudentConnection.Add(connection);
            _context.Tuition.Update(tuition);

            _context.SaveChanges();
            TempData["SuccessMessage"] = "Successfully registered for the course!";
            return RedirectToAction("CourseRegistration"); // Redirect back to the course registration page
        }

        [HttpPost]
        public IActionResult Drop(int CourseId)
        {
            var user = _auth.getUser();

            if (string.IsNullOrEmpty(user.UserId.ToString()))
            {
                return Unauthorized();
            }

            if (!int.TryParse(user.UserId.ToString(), out int userId))
            {
                TempData["ErrorMessage"] = "Invalid user ID format.";
                return RedirectToAction("CourseRegistration");
            }

            var connection = _context.StudentConnection
                .FirstOrDefault(c => c.StudentId == user.UserId && c.CourseId == CourseId);

            if (connection != null)
            {
                // Remove dues from the users account (connection always had null User and Courses, so I did it this way)
                var userUpdate = _context.User.FirstOrDefault(u => u.UserId == user.UserId);
                var course = _context.Courses.FirstOrDefault(c => c.CourseId == CourseId);
                var tuition = _context.Tuition.First(t => t.UserId == userId);

                tuition.AmountDue -= 100 * course.Credits;

                // Ensure the Users balance doesn't go negative
                if (tuition.AmountDue < 0)
                {
                    tuition.AmountDue = 0;
                }

                _context.User.Update(userUpdate);
                var studentClasses = _context.Database.ExecuteSqlRaw(
                    "DELETE FROM StudentConnection WHERE StudentId = {0} AND CourseId = {1}",
                    user.UserId, CourseId
                );

                _context.SaveChanges();
                TempData["SuccessMessage"] = "Successfully Dropped for the course!";
                return RedirectToAction("CourseRegistration"); // Redirect back to the course registration page
            }

            TempData["ErrorMessage"] = "Registration not found.";
            return RedirectToAction("CourseRegistration");
        }
        #endregion
        
        /*
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
        */

        #region Account Editing
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
        public async Task<IActionResult> Edit(int id, [Bind("Id,Email,Password")] User user)
        {
            if (id != user.UserId)
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
                    if (!UserExists(user.UserId))
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
                .FirstOrDefaultAsync(m => m.UserId == id);
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
            return _context.User.Any(e => e.UserId == id);
        }

        // GET: UserAccount
        public async Task<IActionResult> Account()
        {
            var User = _auth.getUser();

            if (User == null)
            {
                return RedirectToAction("Login");
            }

            var UserAdd = _context.Address.First(a => a.UserId == User.UserId);

            AccountViewModel UserInfo = new AccountViewModel
            {
                UserRole = User.Role,
                Notifications = _auth.GetNotificationsForUser(User.UserId).ToList(),
                FirstName = User.FirstName,
                LastName = User.LastName,
                Address = UserAdd

            };

            return View(UserInfo);
        }
        
        // POST: User/UpdateUser
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateUserName(AccountViewModel model)
        {
            var user = _auth.getUser();
            var userName = _context.UserInfo.First(un => un.UserId == user.UserId);

            

            UserInfo newName = new UserInfo
            {
                UserId = user.UserId,
                FirstName = model.FirstName,
                LastName = model.LastName,
            };
            _context.UserInfo.Update(newName);
            await _context.SaveChangesAsync();

            return RedirectToAction("Account");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateAdd(AccountViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = _auth.getUser();
                var userAdd = _context.Address.First(a => a.UserId == model.Id);

                Addresses newAddress = new Addresses{
                    UserId = user.UserId,
                    AddOne = model.Address.AddOne,
                    AddTwo = model.Address.AddTwo,
                    City = model.Address.City,
                    State = model.Address.State,
                    Zip =   model.Address.Zip,
                };

                _context.Address.Update(newAddress);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Account");
        }

        #endregion

        #region Profile Image

        static string ConvertToBase64(string imagePath)
        {

            byte[] imageBytes = System.IO.File.ReadAllBytes(imagePath);
            string base64String = Convert.ToBase64String(imageBytes);

            return base64String;
        }


        public IActionResult GetProfileImage()
        {
            var imageBytes = _auth.getUser().PFP;

            return File(imageBytes, "image/png");
        }

        // GET: /User/UploadImage
        [HttpGet]
        public async Task<IActionResult> ChangeProfilePicture()
        {
            //Grab user log in info
            var user = _auth.getUser();

            if (user == null)
            {
                return RedirectToAction("Login");
            }

            if (user == null)
            {
                return NotFound();
            }
            return View(_auth.getIndex());
        }



        // POST: /User/UploadImage
        [HttpPost]
        public async Task<IActionResult> ChangeProfilePicture(IFormFile profileImage)
        {
            var userLoginInfo = _auth.getUser();
            var fullUser = await _context.User.FindAsync(userLoginInfo.UserId);

            if (profileImage != null && fullUser != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await profileImage.CopyToAsync(memoryStream);

                    byte[] imageBytes = memoryStream.ToArray();

                    string base64String = Convert.ToBase64String(imageBytes);

                    //fullUser.ProfileImageBase64 = base64String;

                    _context.Update(fullUser);
                    await _context.SaveChangesAsync();
                }

            }

            return View(fullUser);
        }
        #endregion

        #region Cookies
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

        public async void DeleteCookie()
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

            //var CurrCookies = _context.Cookies.First(cc => cc.UserId == _auth.getUser().UserId);

            //_context.Cookies.Remove(CurrCookies);
            //await _context.SaveChangesAsync();
        }
        #endregion

        #region Payment
        // GET: /User/Payment
        [HttpGet]
        public IActionResult Payment(bool? didSucceed, string? receiptUrl)
        {
            var user = _auth.getUser();

            var amountDue = _context.Tuition.First(t => t.UserId == user.UserId).AmountDue;

            if (user == null)
            {
                return RedirectToAction("Login");
            }

            // Check if a receiptUrl was passed in
            if (receiptUrl != null)
            {
                ViewBag.ReceiptUrl = receiptUrl;
            }

            // Check if the payment was successful (or if it went through at all)
            if (didSucceed.HasValue)
            {
                ViewBag.SuccessMessage = didSucceed.Value ? "Payment successful!" : "Payment failed.";
            }

            // Get user and course information
            var courses = (from connection in _context.StudentConnection
                           join course in _context.Courses on connection.CourseId equals course.CourseId
                           where connection.StudentId == user.UserId
                           select course).ToList();

            decimal totalPaid = 0;
            foreach (var course in courses)
            {
                totalPaid += course.Credits * 100;
            }
            totalPaid = (amountDue - totalPaid) * -1; // For some reason if I don't multiply it by 1 it shows up with () around the number

            // Create the view model
            var viewModel = new PaymentViewModel
            {
                Notifications = _auth.GetNotificationsForUser(user.UserId).ToList(),
                FirstName = user.FirstName,
                LastName = user.LastName,
                Courses = courses,
                AmountDue = amountDue,
                AmountPaid = totalPaid
            };

            return View(viewModel);
        }
        
        // Checkout page
        public ActionResult Checkout(decimal amount)
        {
            // Check if the amount is valid
            var user = _auth.getUser();

            var amountDue = _context.Tuition.First(t => t.UserId == user.UserId).AmountDue;

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
                SuccessUrl = $"{Request.Scheme}://{Request.Host}/User/PaymentSuccess?sessionId={{CHECKOUT_SESSION_ID}}&amountPaid={amount}",
                CancelUrl = $"{Request.Scheme}://{Request.Host}/User/PaymentFailure",
            };

            var service = new SessionService();
            Session session = service.Create(options);


            return Redirect(session.Url);
        }

        // If the payment succeeded
        public ActionResult PaymentSuccess(string sessionId)
        {
            // Grab the session details
            var service = new SessionService();
            var session = service.Get(sessionId);

            // Get the PaymentIntent ID
            var paymentIntentId = session.PaymentIntentId;

            // Get the charge associated with the PaymentIntent
            var chargeService = new ChargeService();
            var chargeListOptions = new ChargeListOptions
            {
                PaymentIntent = paymentIntentId,
                Limit = 1
            };

            var charges = chargeService.List(chargeListOptions);

            // Get the receipt URL
            var receiptUrl = charges.Data.First().ReceiptUrl;

            // Grab the amount paid
            var amountPaid = decimal.Parse(Request.Query["amountPaid"]);

            // Update the user's balance
            var user = _context.User.FirstOrDefault(u => u.UserId == _auth.getUser().UserId);
            var tuition = _context.Tuition.First(t => t.UserId == user.UserId);
            tuition.AmountDue -= amountPaid;
            _context.Tuition.Update(tuition);
            _context.SaveChanges();

            // Pass the receipt URL to the view (using ViewBag)
            ViewBag.ReceiptUrl = receiptUrl;

            return RedirectToAction("Payment", "User", new { didSucceed = true, receiptUrl = receiptUrl });
        }

        // If the payment failed
        public ActionResult PaymentFailure()
        {
            return RedirectToAction("Payment", "User", new { didSucceed = false });
        }
        #endregion
    }
}
