using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium.Support.UI;
using System;
using System.Threading;

namespace Scott_Test
{
    [TestClass]
    public class LoginTest
    {
        private IWebDriver driver;

        [TestInitialize]
        public void SetUp()
        {
            driver = new FirefoxDriver();
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
        }

        [TestCleanup]
        public void TearDown()
        {
            driver.Quit();
        }

        [TestMethod]
        public void Test_Login()
        {
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

            driver.Navigate().GoToUrl("https://localhost:7244/");

            var loginInputs = driver.FindElements(By.TagName("input"));

            Assert.IsTrue(loginInputs.Count >= 2, "Expected at least two input fields (email, password).");

            string[] inputs = { "Scott@gmail.com", "Password" };

            for (int i = 0; i < inputs.Length; i++)
            {
                loginInputs[i].SendKeys(inputs[i]);
            }

            driver.FindElement(By.ClassName("login-btn")).Click();

            var classCard = wait.Until(d => d.FindElement(By.ClassName("card")));
            Assert.IsNotNull(classCard, "Class Card not found, login might have failed.");

            driver.FindElement(By.ClassName("class-info")).Click();

            var quizzesLink = wait.Until(d => d.FindElement(By.LinkText("Quizzes")));
            Assert.AreEqual("Quizzes", quizzesLink.Text, "Failed to navigate to Quizzes.");
            quizzesLink.Click();

            var addQuizBtn = wait.Until(d => d.FindElement(By.LinkText("Add Quiz")));
            Assert.AreEqual("Add Quiz", addQuizBtn.Text, "Failed to navigate to Add a Quiz.");
            addQuizBtn.Click();

            string[] quizInfo = { "Quiz Title", "Quiz Description"};

            var quizInput = driver.FindElements(By.TagName("input"));

            for (int i = 0; i < quizInput.Count; i++)
            {
                Console.WriteLine(quizInput[i]);
                if (i == 0 || i == 1)
                {
                    quizInput[i].Clear();
                    quizInput[i].SendKeys(quizInfo[i]);
                }
                else if (i == 4)
                {
                    quizInput[i].SendKeys("2024-12-28");
                    break;
                }
            }
            quizInput[8].SendKeys("1");
            quizInput[8].SendKeys(Keys.Tab);

            IWebElement dropdown = driver.FindElement(By.TagName("select"));

            SelectElement select = new SelectElement(dropdown);

            select.SelectByIndex(1);

            var question1 = wait.Until(d => d.FindElement(By.Id("q1")));
            question1.SendKeys("Question 1");

            var point1 = wait.Until(d => d.FindElement(By.Id("p1")));
            point1.SendKeys("5");

            Assert.AreEqual("Quiz Title", quizInput[0].GetAttribute("value"), "Quiz Title is incorrect.");
            Assert.AreEqual("Quiz Description", quizInput[1].GetAttribute("value"), "Quiz Description is incorrect.");
            Assert.AreEqual("2024-12-28", quizInput[4].GetAttribute("value"), "Quiz Due Date is incorrect.");

            IWebElement submitButton = driver.FindElement(By.CssSelector("button[type='submit']"));
            submitButton.Click();
        }
    }
}
