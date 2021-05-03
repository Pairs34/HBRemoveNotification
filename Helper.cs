using System;
using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace HBRemoveNotification
{
    public static class Helper
    {
        public static void WaitForPageLoad(this IWebDriver driver)
        {
            try
            {
                IJavaScriptExecutor javaScriptExecutor = (IJavaScriptExecutor)driver;
                WebDriverWait webDriverWait = new WebDriverWait(driver, new TimeSpan(0, 0, 15));
                webDriverWait.Until<bool>((IWebDriver wd) => javaScriptExecutor.ExecuteScript("return document.readyState")
                    .Equals("complete"));
            }
            catch (Exception)
            {
            }
        }
        
        public static IWebElement IsElementPresent(this IWebDriver driver, By by,int timeout = 25)
        {
            IWebElement Founded = null;

            try
            {
                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeout))
                {
                    PollingInterval = TimeSpan.FromSeconds(5),
                    Message = "Zaman aşımı"
                };

                IWebElement element = wait.Until(x => driver.FindElement(@by));
                if (element.Displayed)
                {
                    Founded = element;
                }
                else
                {
                    Founded = null;
                }
            }
            catch (Exception)
            {
            }

            return Founded;
        }
        
        public static void WaitSomeSecond(int second)
        {
            Thread.Sleep(TimeSpan.FromSeconds(second));
        }
    }
}