using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Linq;
using TGTOAT.Controllers;
using Models;
using TGTOAT.Helpers;
using TGTOAT;
using System.Threading.Tasks;
using Data;
using Microsoft.AspNetCore.Mvc;

namespace Scott_Test
{
    [TestClass]
    public class UnitTests
    {
        private Mock<IAuthentication> _mockAuth;
        private Mock<IPasswordHasher> _mockHasher;
        private UserController _controller;
        private DbContextOptions<UserContext> _options;
        private UserContext _context;

        public UnitTests()
        {
            _mockAuth = new Mock<IAuthentication>();
            _mockHasher = new Mock<IPasswordHasher>();

            _options = new DbContextOptionsBuilder<UserContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _context = new UserContext(_options);
            _context.Add(new Addresses
            {
                UserId = 1,
                AddOne = "123 Main St",
                City = "Ogden",
                State = "UT",
                Zip = 84401
            });
            _context.SaveChanges();

            _controller = new UserController(null, _context, _mockHasher.Object, _mockAuth.Object);
        }

        [TestMethod]
        public async Task Test_AccountPage_ReturnsCorrectUserInfo()
        {
            var mockUser = new CurrUser
            {
                UserId = 1,
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com",
                Role = "Student",
                PFP = new byte[] { },
                BirthDate = new DateOnly(1990, 1, 1),
                Notifications = new List<Notifications>()
            };

            _mockAuth.Setup(auth => auth.getUser()).Returns(mockUser);

            var result = await _controller.Account() as ViewResult;
            Assert.IsNotNull(result, "Expected a ViewResult, but received null");

            var model = result.Model as AccountViewModel;
            Assert.IsNotNull(model, "Expected the model to be of type AccountViewModel, but it was null");

            Assert.AreEqual(mockUser.FirstName, model.FirstName);
            Assert.AreEqual(mockUser.LastName, model.LastName);
            Assert.AreEqual(mockUser.Role, model.UserRole);
            Assert.AreEqual("123 Main St", model.Address.AddOne);
            Assert.AreEqual("Ogden", model.Address.City);
            Assert.AreEqual("UT", model.Address.State);
            Assert.AreEqual(84401, model.Address.Zip);
        }
    }
}
