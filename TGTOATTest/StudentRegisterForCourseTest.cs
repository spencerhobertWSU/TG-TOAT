using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace TGTOATTest
{
    [TestClass]
    public class StudentRegisterForCourseTest
    {
        private IWebDriver driver;

        [TestInitialize]
        public void Setup()
        {
            driver = new ChromeDriver();
        }

        [TestCleanup]
        public void Teardown()
        {
            driver.Quit();
        }

        [TestMethod]
        public void StudentRegisterForCourse()
        {
            // Figure out how many courses the student is registered for
            // Register the student for a course
            // Check if the student is now registered for one more course

            // Navigate to the website
            driver.Navigate().GoToUrl("https://localhost:44362/");

            // Login to the website (IDK WHY BUT YOU HAVE TO SEND THE ENTER KEY? YOU CAN'T CLICK IT?)
            driver.FindElement(By.Id("Email")).SendKeys("bob@gmail.com");
            driver.FindElement(By.Id("Password")).SendKeys("Pass123");
            driver.FindElement(By.XPath("//input[@value='Log In']")).SendKeys(Keys.Enter);

            // Click on the registration tab
            driver.FindElement(By.XPath("//a[@href='/User/CourseRegistration']")).Click();

            // Count how many 'Drop' buttons there are
            int numOfDropButtonsBefore = driver.FindElements(By.Id("dropBtn")).Count();

            // Click on the first 'Register' button
            driver.FindElement(By.Id("registerBtn")).SendKeys(Keys.Enter);

            // Count how many 'Drop' buttons there are now
            int numOfDropButtonsAfter = driver.FindElements(By.Id("dropBtn")).Count();

            // Check if the number of drop buttons has increased by 1
            if (numOfDropButtonsAfter == numOfDropButtonsBefore + 1)
            {
                Assert.IsTrue(true);
            }
            else
            {
                Assert.IsTrue(false);
            }
        }
    }
}
