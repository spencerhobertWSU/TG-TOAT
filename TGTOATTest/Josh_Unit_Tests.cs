using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium.Chrome;

namespace StudentDropCourseTest
{
    [TestClass]
    public class Josh_Unit_Tests
    {
        private IWebDriver driver;
        private WebDriverWait wait;

        [TestInitialize]
        public void TestInitialize()
        {
            driver = new ChromeDriver();
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
        }

        [TestCleanup]
        public void TestCleanup()
        {
            driver.Quit();
        }

        [TestMethod]
        public void DropCourseTest()
        {
            // Navigate to the website
            driver.Navigate().GoToUrl("https://localhost:44362/");

            // Login as a student
            wait.Until(drv => drv.FindElement(By.Id("Email"))).SendKeys("bob@gmail.com");
            wait.Until(drv => drv.FindElement(By.Id("Password"))).SendKeys("Pass123");
            wait.Until(drv => drv.FindElement(By.XPath("//input[@value='Log In']"))).Click();
            // have to click login button twice for some reason
            wait.Until(drv => drv.FindElement(By.XPath("//input[@value='Log In']"))).Click();

            // Navigate to course registration page
            wait.Until(drv => drv.FindElement(By.XPath("//a[@href='/User/CourseRegistration']"))).Click();

            // Find and store the initial number of "Drop" buttons
            var initialDropButtonCount = driver.FindElements(By.CssSelector("button.btn-danger")).Count;

            // Find and click the first 'Drop' button
            var dropButton = wait.Until(drv => drv.FindElement(By.CssSelector("button.btn-danger")));
            dropButton.Click();

            // Wait for the page to refresh and verify the course was dropped
            wait.Until(d => d.FindElements(By.CssSelector("button.btn-danger")).Count < initialDropButtonCount);

            // Final verification
            var finalDropButtonCount = driver.FindElements(By.CssSelector("button.btn-danger")).Count;
            Assert.AreEqual(initialDropButtonCount - 1, finalDropButtonCount, 
                "Number of courses with 'Drop' button should decrease by 1");
        }

        [TestMethod]
        public void InstructorEditCourseTest()
        {
            // Generate random test data
            var random = new Random();
            var newCourseName = $"Test Course {random.Next(1000, 9999)}";
            var newRoomNumber = random.Next(100, 999).ToString();
            var newCredits = random.Next(1, 6).ToString();
            var newCapacity = random.Next(10, 50).ToString();
            var newCourseNumber = random.Next(100, 999).ToString();

            // Navigate to the website
            driver.Navigate().GoToUrl("https://localhost:44362/");

            // Login as an instructor
            wait.Until(drv => drv.FindElement(By.Id("Email"))).SendKeys("instructor@weber.edu");
            wait.Until(drv => drv.FindElement(By.Id("Password"))).SendKeys("instructor");
            wait.Until(drv => drv.FindElement(By.XPath("//input[@value='Log In']"))).Click();
            // have to click login button twice for some reason
            wait.Until(drv => drv.FindElement(By.XPath("//input[@value='Log In']"))).Click();

            // Navigate to course list
            wait.Until(drv => drv.FindElement(By.XPath("//a[@href='/Course/Courses']"))).Click();

            // Click edit link for the first course
            wait.Until(drv => drv.FindElement(By.LinkText("Edit"))).Click();

            // Edit course details with random values
            var courseNameInput = wait.Until(drv => drv.FindElement(By.Name("CourseName")));
            courseNameInput.Clear();
            courseNameInput.SendKeys(newCourseName);

            var roomNumberInput = wait.Until(drv => drv.FindElement(By.Name("RoomNumber")));
            roomNumberInput.Clear();
            roomNumberInput.SendKeys(newRoomNumber);

            var courseNumberInput = wait.Until(drv => drv.FindElement(By.Name("CourseNumber")));
            courseNumberInput.Clear();
            courseNumberInput.SendKeys(newCourseNumber);

            var creditsInput = wait.Until(drv => drv.FindElement(By.Name("NumberOfCredits")));
            creditsInput.Clear();
            creditsInput.SendKeys(newCredits);

            var capacityInput = wait.Until(drv => drv.FindElement(By.Name("Capacity")));
            capacityInput.Clear();
            capacityInput.SendKeys(newCapacity);

            // Save changes
            wait.Until(drv => drv.FindElement(By.CssSelector("button[type='submit']"))).Click();

            // Verify the changes were saved by checking if we're back on the course list page
            // and the updated values are present
            wait.Until(drv => drv.Url.Contains("/Course/Courses"));

            // Verify each changed value
            var updatedCourseName = wait.Until(drv => 
                drv.FindElement(By.XPath($"//td[contains(text(), '{newCourseName}')]")));
            var updatedRoomNumber = wait.Until(drv => 
                drv.FindElement(By.XPath($"//td[contains(text(), '{newRoomNumber}')]")));
            var updatedCourseNumber = wait.Until(drv => 
                drv.FindElement(By.XPath($"//td[contains(text(), '{newCourseNumber}')]")));
            var updatedCredits = wait.Until(drv => 
                drv.FindElement(By.XPath($"//td[contains(text(), '{newCredits}')]")));

            // Assertions
            Assert.IsNotNull(updatedCourseName, "Course name was not updated");
            Assert.IsNotNull(updatedRoomNumber, "Room number was not updated");
            Assert.IsNotNull(updatedCourseNumber, "Course number was not updated");
            Assert.IsNotNull(updatedCredits, "Credits were not updated");
        }        
    }
} 