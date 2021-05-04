using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        public static void KillAllDrivers()
        {
            new[] { "geckodriver", "chromedriver" }.ForEach((x) =>
            {
                foreach (var process in Process.GetProcessesByName(x))
                {
                    process.Kill();
                }
            });
        }
        public static void ForEach<T>(this IEnumerable<T> enumeration, Action<T> action)
        {
            foreach (T item in enumeration)
            {
                action(item);
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

        public static object RunJsCommand(this IWebDriver driver, string jsCommand, object[] options = null)
        {
            IJavaScriptExecutor javaScriptExecutor = (IJavaScriptExecutor)driver;
            if (options != null)
            {
                return javaScriptExecutor.ExecuteScript(jsCommand, options);
            }
            else
            {
                return javaScriptExecutor.ExecuteScript(jsCommand);
            }
        }

        public static void WaitSomeSecond(int second)
        {
            Thread.Sleep(TimeSpan.FromSeconds(second));
        }
    }
}