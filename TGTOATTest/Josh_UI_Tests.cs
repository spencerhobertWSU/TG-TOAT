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
            var options = new ChromeOptions();
            options.AddArgument("--start-maximized");
            options.AddArgument("--disable-notifications");
            options.AddArgument("--ignore-certificate-errors");
            options.AddArgument("--disable-popup-blocking");
            options.AddArgument("--disable-infobars");
            
            driver = new ChromeDriver(options);
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(30));
            
            // Set implicit wait as well
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
            driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(30);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            // Print any browser console logs
            var logs = driver.Manage().Logs.GetLog(LogType.Browser);
            foreach (var log in logs)
            {
                Console.WriteLine($"Browser Log: {log.Level} - {log.Message}");
            }

            if (driver != null)
            {
                driver.Quit();
            }
        }

        private string GetAvailableUrl()
        {
            string[] urls = new string[] 
            { 
                "https://localhost:7244/",
                "https://localhost:44362/"
            };

            foreach (var url in urls)
            {
                try
                {
                    driver.Navigate().GoToUrl(url);
                    // Update to use the correct ID from the Login view
                    var element = wait.Until(drv => drv.FindElement(By.Id("floatingInput")));
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
            Thread.Sleep(1000);

            try {
                // Login with updated selectors and credentials
                wait.Until(drv => drv.FindElement(By.Id("floatingInput"))).SendKeys("drew@gmail.com");
                wait.Until(drv => drv.FindElement(By.Id("floatingPass"))).SendKeys("Password");
                wait.Until(drv => drv.FindElement(By.CssSelector(".btn.btn-primary.login-btn"))).Click();
                Thread.Sleep(1000);

                // Navigate to course registration page
                wait.Until(drv => drv.FindElement(By.XPath("//a[@href='/User/CourseRegistration']"))).Click();
                Thread.Sleep(1000);

                // Find and store the initial number of "Drop" buttons
                var initialDropButtonCount = driver.FindElements(By.CssSelector("button.btn-danger")).Count;
                Console.WriteLine($"Initial drop button count: {initialDropButtonCount}");

                // Find and click the first 'Drop' button
                var dropButton = wait.Until(drv => drv.FindElement(By.CssSelector("button.btn-danger")));
                dropButton.Click();
                Thread.Sleep(1000);

                // Wait for the page to refresh and verify the course was dropped
                wait.Until(d => d.FindElements(By.CssSelector("button.btn-danger")).Count < initialDropButtonCount);

                // Final verification
                var finalDropButtonCount = driver.FindElements(By.CssSelector("button.btn-danger")).Count;
                Assert.AreEqual(initialDropButtonCount - 1, finalDropButtonCount, 
                    "Number of courses with 'Drop' button should decrease by 1");
            }
            catch (Exception ex) {
                var screenshot = ((ITakesScreenshot)driver).GetScreenshot();
                var timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
                screenshot.SaveAsFile($"drop_course_error_{timestamp}.png");
                
                Console.WriteLine($"Failed with error: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                throw;
            }
        }

        [TestMethod]
        public void InstructorEditCourseTest()
        {
            // Generate random test data
            var random = new Random();
            string newCourseName = $"Test Course {random.Next(1000, 9999)}";
            string newRoomNumber = random.Next(100, 999).ToString();
            string newCourseNumber = random.Next(1000, 9999).ToString();
            string newCredits = random.Next(1, 6).ToString();
            string newCapacity = random.Next(10, 50).ToString();

            // Navigate to the website
            var baseUrl = GetAvailableUrl();
            driver.Navigate().GoToUrl(baseUrl);
            Thread.Sleep(1000);

            try {
                // Login
                wait.Until(drv => drv.FindElement(By.Id("floatingInput"))).SendKeys("josh@gmail.com");
                wait.Until(drv => drv.FindElement(By.Id("floatingPass"))).SendKeys("Password");
                wait.Until(drv => drv.FindElement(By.CssSelector(".btn.btn-primary.login-btn"))).Click();
                Thread.Sleep(1000);

                // Navigate to Courses
                wait.Until(drv => drv.FindElement(By.CssSelector("a[href*='Courses']"))).Click();
                Thread.Sleep(1000);

                // Find and click Edit button
                var editButton = wait.Until(drv => drv.FindElement(
                    By.XPath("//a[contains(@class, 'btn-primary') and contains(text(), 'Edit')]")));
                editButton.Click();
                Thread.Sleep(1000);

                // Edit course details
                var departmentSelect = wait.Until(drv => drv.FindElement(By.Name("SelectedDepartmentId")));
                var selectElement = new SelectElement(departmentSelect);
                selectElement.SelectByIndex(1);
                Thread.Sleep(500);

                var courseNameInput = wait.Until(drv => drv.FindElement(By.Name("CourseName")));
                courseNameInput.Clear();
                courseNameInput.SendKeys(newCourseName);

                var courseNumberInput = wait.Until(drv => drv.FindElement(By.Name("CourseNumber")));
                courseNumberInput.Clear();
                courseNumberInput.SendKeys(newCourseNumber);

                var creditsInput = wait.Until(drv => drv.FindElement(By.Name("NumberOfCredits")));
                creditsInput.Clear();
                creditsInput.SendKeys(newCredits);

                // Select days (Mon, Wed, Fri)
                var monLabel = wait.Until(drv => drv.FindElement(By.CssSelector("label[for='monBtn']")));
                var wedLabel = wait.Until(drv => drv.FindElement(By.CssSelector("label[for='wedBtn']")));
                var friLabel = wait.Until(drv => drv.FindElement(By.CssSelector("label[for='friBtn']")));
                
                monLabel.Click();
                Thread.Sleep(200);
                wedLabel.Click();
                Thread.Sleep(200);
                friLabel.Click();
                Thread.Sleep(1000);

                // Handle campus selection
                var campusSelect = wait.Until(drv => drv.FindElement(By.Name("Campus")));
                var campusSelectElement = new SelectElement(campusSelect);
                campusSelectElement.SelectByText("Ogden Campus");
                Thread.Sleep(1000);

                // Handle building selection
                var buildingSelect = wait.Until(drv => drv.FindElement(By.Name("Building")));
                var buildingSelectElement = new SelectElement(buildingSelect);
                buildingSelectElement.SelectByValue("TY");
                Thread.Sleep(500);

                // Room Number
                var roomNumberInput = wait.Until(drv => drv.FindElement(By.Name("RoomNumber")));
                roomNumberInput.Clear();
                roomNumberInput.SendKeys(newRoomNumber);

                // Capacity
                var capacityInput = wait.Until(drv => drv.FindElement(By.Name("Capacity")));
                capacityInput.Clear();
                capacityInput.SendKeys(newCapacity);

                // Handle Start Time
                var startTimeInput = wait.Until(drv => drv.FindElement(By.Name("StartTime")));
                startTimeInput.Click();
                startTimeInput.SendKeys(Keys.Control + "a"); // Select all existing text
                startTimeInput.SendKeys(Keys.Delete); // Clear it
                startTimeInput.SendKeys("0900AM"); // Type 9:00 AM with AM/PM
                Thread.Sleep(500);

                // Handle End Time
                var endTimeInput = wait.Until(drv => drv.FindElement(By.Name("EndTime")));
                endTimeInput.Click();
                endTimeInput.SendKeys(Keys.Control + "a"); // Select all existing text
                endTimeInput.SendKeys(Keys.Delete); // Clear it
                endTimeInput.SendKeys("1015AM"); // Type 10:15 AM with AM/PM
                Thread.Sleep(500);

                // Submit the form
                var submitButton = wait.Until(drv => drv.FindElement(By.Id("courseSubmit")));
                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", submitButton);
                Thread.Sleep(500);
                submitButton.Click();

                // Wait for redirect and verify changes
                wait.Until(drv => drv.Url.Contains("/Course/Courses"));

                // Verify the changes were saved
                wait.Until(drv => drv.FindElement(By.XPath($"//td[contains(text(), '{newCourseName}')]")));
                wait.Until(drv => drv.FindElement(By.XPath($"//td[contains(text(), '{newCourseNumber}')]")));
                wait.Until(drv => drv.FindElement(By.XPath($"//td[contains(text(), '{newCredits}')]")));

                // Final assertions
                var updatedCourseName = driver.FindElement(By.XPath($"//td[contains(text(), '{newCourseName}')]"));
                var updatedCourseNumber = driver.FindElement(By.XPath($"//td[contains(text(), '{newCourseNumber}')]"));
                var updatedCredits = driver.FindElement(By.XPath($"//td[contains(text(), '{newCredits}')]"));

                Assert.IsNotNull(updatedCourseName, "Course name was not updated");
                Assert.IsNotNull(updatedCourseNumber, "Course number was not updated");
                Assert.IsNotNull(updatedCredits, "Credits were not updated");
            }
            catch (Exception ex) {
                var screenshot = ((ITakesScreenshot)driver).GetScreenshot();
                var timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
                screenshot.SaveAsFile($"edit_course_error_{timestamp}.png");
                
                Console.WriteLine($"Failed with error: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                throw;
            }
        }        
    }
} 