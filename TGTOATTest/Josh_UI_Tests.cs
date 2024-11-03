using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium.Chrome;

namespace Josh_Tests
{
    [TestClass]
    public class Josh_UI_Tests
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

        private string GetAvailableUrl()
        {
            string[] urls = new string[] 
            { 
                "https://localhost:44362/",
                "https://localhost:7244/"
            };

            foreach (var url in urls)
            {
                try
                {
                    driver.Navigate().GoToUrl(url);
                    // Check if we can find a common element that should be present on the page
                    var element = wait.Until(drv => drv.FindElement(By.Id("Email")));
                    return url; // If successful, return the working URL
                }
                catch (Exception)
                {
                    continue; // If this URL fails, try the next one
                }
            }
            
            throw new Exception("No available URLs found. Both URLs are unreachable.");
        }

        [TestMethod]
        public void DropCourseTest()
        {
            // Navigate to the website using the first available URL
            var baseUrl = GetAvailableUrl();
            driver.Navigate().GoToUrl(baseUrl);

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

            // Navigate to the website using the first available URL
            var baseUrl = GetAvailableUrl();
            driver.Navigate().GoToUrl(baseUrl);

            // Login as an instructor
            wait.Until(drv => drv.FindElement(By.Id("Email"))).SendKeys("instructor@weber.edu");
            wait.Until(drv => drv.FindElement(By.Id("Password"))).SendKeys("instructor");
            wait.Until(drv => drv.FindElement(By.XPath("//input[@value='Log In']"))).Click();
            // have to click login button twice for some reason
            wait.Until(drv => drv.FindElement(By.XPath("//input[@value='Log In']"))).Click();

            // Navigate to course list
            wait.Until(drv => drv.FindElement(By.XPath("//a[@href='/Course/Courses']"))).Click();

            // Add explicit wait and scroll into view before clicking edit
            var editLink = wait.Until(drv => drv.FindElement(By.LinkText("Edit")));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", editLink);
            // Add a small delay to let the page settle
            Thread.Sleep(500);
            editLink.Click();

            // Edit course details with random values - add waits and clear existing values
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

            // Save changes - scroll submit button into view before clicking
            var submitButton = wait.Until(drv => drv.FindElement(By.CssSelector("button[type='submit']")));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", submitButton);
            Thread.Sleep(500);
            submitButton.Click();

            // Verify the changes were saved
            wait.Until(drv => drv.Url.Contains("/Course/Courses"));

            // Add explicit waits for verifications
            wait.Until(drv => drv.FindElement(By.XPath($"//td[contains(text(), '{newCourseName}')]")));
            wait.Until(drv => drv.FindElement(By.XPath($"//td[contains(text(), '{newRoomNumber}')]")));
            wait.Until(drv => drv.FindElement(By.XPath($"//td[contains(text(), '{newCourseNumber}')]")));
            wait.Until(drv => drv.FindElement(By.XPath($"//td[contains(text(), '{newCredits}')]")));

            // Assertions remain the same
            var updatedCourseName = driver.FindElement(By.XPath($"//td[contains(text(), '{newCourseName}')]"));
            var updatedRoomNumber = driver.FindElement(By.XPath($"//td[contains(text(), '{newRoomNumber}')]"));
            var updatedCourseNumber = driver.FindElement(By.XPath($"//td[contains(text(), '{newCourseNumber}')]"));
            var updatedCredits = driver.FindElement(By.XPath($"//td[contains(text(), '{newCredits}')]"));

            Assert.IsNotNull(updatedCourseName, "Course name was not updated");
            Assert.IsNotNull(updatedRoomNumber, "Room number was not updated");
            Assert.IsNotNull(updatedCourseNumber, "Course number was not updated");
            Assert.IsNotNull(updatedCredits, "Credits were not updated");
        }        
    }
} 