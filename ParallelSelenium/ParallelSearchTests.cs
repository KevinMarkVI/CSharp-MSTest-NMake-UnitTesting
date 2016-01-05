using System;
using System.Net;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ParallelSelenium.PageObjects.Bing;
using ParallelSelenium.Utils;
using System.Diagnostics;

namespace ParallelSelenium
{
    [TestClass()]
    public class ParallelSearchTests
    {
        private IWebDriver driver;

        [TestInitialize()]
        public void Init()
        {
            /* Web proxy setup to be used with the underlying Rest requests.
            **/
            /*
            WebProxy iProxy = new WebProxy("192.168.1.159:808", true);
            iProxy.UseDefaultCredentials = true;
            iProxy.Credentials = new NetworkCredential("test", "hello123");
            WebRequest.DefaultWebProxy = iProxy;
            */

            String seleniumUri = "http://{0}:{1}/wd/hub";
            DesiredCapabilities capabilities = new DesiredCapabilities();
            capabilities.SetCapability(CapabilityType.BrowserName, "chrome");
            capabilities.SetCapability(CapabilityType.Version, "46");
            capabilities.SetCapability(CapabilityType.Platform, "OS X 10.10");
            //Sauce Connect setup.
            //Requires a named tunnel.
            if (Constants.tunnelId != null)
            {
                capabilities.SetCapability("tunnel-identifier", Constants.tunnelId);
            }
            if(Constants.buildTag != null)
            {
                capabilities.SetCapability("build", Constants.buildTag);
            }
            if(Constants.seleniumRelayPort != null && Constants.seleniumRelayHost != null)
            {
                seleniumUri = String.Format(seleniumUri, Constants.seleniumRelayHost, Constants.seleniumRelayPort);
            } else {
                seleniumUri = "http://ondemand.saucelabs.com:80/wd/hub";
            }
            capabilities.SetCapability("username", Constants.sauceUser);
            capabilities.SetCapability("accessKey", Constants.sauceKey);
            capabilities.SetCapability("name", "INSERT TEST NAME HERE");

            Trace.Listeners.Add(new TextWriterTraceListener(Console.Out));
            Trace.WriteLine("Hello World");
            //String.Format("{0}:{1}: [{2}]",
            //TestContext.CurrentContext.Test.ClassName,
            //TestContext.CurrentContext.Test.MethodName,
            //TestContext.CurrentContext.Test.Properties.Get("Description")));
            driver = new CustomRemoteWebDriver(new Uri(seleniumUri), capabilities, TimeSpan.FromSeconds(600));
            driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(10));
        }

        [TestMethod()]
        public void BingSearchHello()
        {
            //this code is repeated below intentionally.
            //Feel free to modify and experiment.
            string query = "hello!";
            driver.Navigate().GoToUrl(BingSearchPage.URL);
            BingSearchPage searchPage = new BingSearchPage(driver);
            BingResultPage resultPage = searchPage.search(query);
            Assert.IsTrue(resultPage.title.StartsWith(query),
                String.Format("Title: {0} does not start with query: {1}!", resultPage.title, query));
        }

        [TestMethod()]
        public void BingSearchBye()
        {
            string query = "bye!";
            driver.Navigate().GoToUrl(BingSearchPage.URL);
            BingSearchPage searchPage = new BingSearchPage(driver);
            BingResultPage resultPage = searchPage.search(query);
            Assert.IsTrue(resultPage.title.StartsWith(query),
            String.Format("Title: {0} does not start with query: {1}!", resultPage.title, query));
        }

        [TestCleanup()]
        public void Cleanup()
        {
            //Test results specified here. Replace 'true' with your preferred test reporting method
            bool passed = true;
            try
            {
                // Logs the result to Sauce Labs
                ((IJavaScriptExecutor)driver).ExecuteScript("sauce:job-result=" + (passed ? "passed" : "failed"));
            }
            finally
            {
                //Read by the jenkins plugin if standard output is being printed to the console
                Console.WriteLine(String.Format("SauceOnDemandSessionID={0} job-name={1}", ((CustomRemoteWebDriver)driver).getSessionId(), passed));
                // Terminates the remote webdriver session
                driver.Quit();
            }
        }
    }
}
