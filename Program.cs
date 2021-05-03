using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Newtonsoft.Json;
using OpenQA.Selenium;
using OpenQA.Selenium.Edge;
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

            var jsonText = File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"accounts.json"));
            var jsonAccounts = JsonConvert.DeserializeObject<List<Account>>(jsonText);

            foreach (var jsonAccount in jsonAccounts)
            {
                OperaOptions opt = new OperaOptions();
                opt.AddArguments(new []{ "--incognito" });
                opt.AddArgument($"user-data-dir={Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Opera Software\\Opera Stable")}");
                IWebDriver driver = new OperaDriver(options:opt);
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
                    elMail?.Clear();
                    elMail?.SendKeys(jsonAccount.name);
                    WaitSomeSecond(1);
                    elPass?.Clear();
                    elPass?.SendKeys(jsonAccount.pass);
                    WaitSomeSecond(1);
                    elBtnLogin.Click();
                    WaitSomeSecond(1);

                    driver.WaitForPageLoad();

                    WaitSomeSecond(3);
                
                    driver.Navigate().GoToUrl("https://hesabim.hepsiburada.com/iletisim-tercihlerim");
                
                    driver.WaitForPageLoad();

                    var elNotification = driver.IsElementPresent(By.XPath("//input[@id='isSendEmailAvailable']"));

                    if (elNotification != null)
                    {
                        elNotification.Click();
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Hesap Adı : {jsonAccount.name}");
                    Console.WriteLine(e);
                    Thread.Sleep(new Random().Next(5,10));
                }
                driver.Quit();
                Thread.Sleep(new Random().Next(30,70));
            }
        }
    }
}