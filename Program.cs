using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Newtonsoft.Json;
using OpenQA.Selenium;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Opera;
using WebDriverManager;
using WebDriverManager.DriverConfigs.Impl;
using WebDriverManager.Helpers;
using static HBRemoveNotification.Helper;

namespace HBRemoveNotification
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            new DriverManager().SetUpDriver(new OperaConfig(), "Latest", Architecture.Auto);
            string OperaProfilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Opera Software\\Opera Stable");
            var jsonText = File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"accounts.json"));
            var jsonAccounts = JsonConvert.DeserializeObject<List<Account>>(jsonText);
            OperaOptions opt = new OperaOptions();
            opt.PageLoadStrategy = PageLoadStrategy.Eager;
            opt.AddArgument($"user-data-dir={OperaProfilePath}");
            opt.AddArguments(new[] { "--incognito" });
            opt.AddAdditionalCapability("useAutomationExtension", false);
            opt.AddExcludedArgument("enable-automation");

            OperaDriverService driverService = OperaDriverService.CreateDefaultService();
            driverService.HideCommandPromptWindow = true;

            for (int i = 0; i<= jsonAccounts.Count;i++)
            {
                KillAllDrivers();
                IWebDriver driver = new OperaDriver(driverService,opt);
                try
                {
                    driver.Url = "https://giris.hepsiburada.com";
            
                    driver.WaitForPageLoad();
            
                    IWebElement elMail = driver.IsElementPresent(By.XPath("//input[@id='txtUserName']"));
                    IWebElement elPass = driver.IsElementPresent(By.XPath("//input[@id='txtPassword']"));
                    IWebElement elBtnLogin = driver.IsElementPresent(By.XPath("//button[contains(text(),'Giriş')]"));

                    if (elMail == null && elPass == null && elBtnLogin == null)
                    {
                        throw new Exception("CANNOT_FIND_LOGINPAGE");
                    }
                    elMail.Click();
                    elMail?.Clear();
                    elMail?.SendKeys(jsonAccounts[i].name);
                    WaitSomeSecond(1);
                    elPass?.Clear();
                    elPass?.SendKeys(jsonAccounts[i].pass);
                    WaitSomeSecond(1);
                    var touchActions = new Actions(driver);
                    touchActions.MoveToElement(elBtnLogin).Perform();
                    touchActions.SendKeys(Keys.Enter).Perform();
                    WaitSomeSecond(1);

                    driver.WaitForPageLoad();

                    WaitSomeSecond(6);

                    if (!CheckIsLoggedorRegistered(driver,true))
                        throw new Exception("CANNOT_LOGIN");

                    driver.Navigate().GoToUrl("https://hesabim.hepsiburada.com/iletisim-tercihlerim");
                
                    driver.WaitForPageLoad();

                    WaitSomeSecond(5);

                    var mailNotify = driver.RunJsCommand("return document.querySelector('div.x0LYwM_8u4ipmQOzUpbEM:nth-child(2) > div:nth-child(2) > div:nth-child(1) > label:nth-child(1) > span:nth-child(2)');");

                    if (mailNotify != null)
                    {
                        driver.RunJsCommand("arguments[0].click()", new []{mailNotify});
                        Console.WriteLine($"{jsonAccounts[i].name} başarılı");
                        WaitSomeSecond(3);
                    }
                    else
                    {
                        throw new Exception("CANNOT_FOUND_MAIL_NOTIFY");
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Hesap Adı : {jsonAccounts[i].name}");
                    Console.WriteLine(e);
                    Thread.Sleep(TimeSpan.FromSeconds(new Random().Next(5, 10)));
                }
                driver.Quit();
                Thread.Sleep(TimeSpan.FromSeconds(new Random().Next(30, 70)));
            }
        }
        public static bool CheckIsLoggedorRegistered(IWebDriver driver,bool isLogin = false)
        {
            bool isExistErrorBoxElement;
            try
            {
                if (isLogin)
                    driver.FindElement(By.XPath("//div[@type='ERROR']"));
                else
                    driver.FindElement(By.XPath("//div[@type='WARNING']"));


                isExistErrorBoxElement = false;
            }
            catch
            {
                isExistErrorBoxElement = true;
            }

            return isExistErrorBoxElement;
        }
    }
}