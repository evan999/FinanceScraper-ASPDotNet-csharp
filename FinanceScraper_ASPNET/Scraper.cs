using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System.Web.Mvc;
using FinanceScraper_ASPNET.Models;
using System.Data;
using System.Data.Entity;

namespace FinanceScraper_ASPNET
{
    public class Scraper
    {
        public static void RunScraper()
        {
            ChromeOptions option = new ChromeOptions();
            option.AddArgument("--headless");
            option.AddArgument("window-size=1200,1100");
            IWebDriver driver = new ChromeDriver(option);

            driver.Navigate().GoToUrl("https://finance.yahoo.com/");

            WebDriverWait waitLogin = new WebDriverWait(driver, TimeSpan.FromSeconds(60));
            waitLogin.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(By.Id("uh-signedin")));

            IWebElement loginButton = driver.FindElement(By.Id("uh-signedin"));
            loginButton.Click();

            WebDriverWait waitEnterUsername = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            waitEnterUsername.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(By.Id("login-username")));

            IWebElement userName = driver.FindElement(By.Id("login-username"));

            userName.SendKeys("meshberge");
            userName.SendKeys(Keys.Enter);

            WebDriverWait waitEnterPassword = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            waitEnterPassword.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(By.Id("login-passwd")));
            IWebElement password = driver.FindElement(By.Id("login-passwd"));

            password.SendKeys("toonfan1!");
            password.SendKeys(Keys.Enter);

            driver.Navigate().GoToUrl("https://finance.yahoo.com/portfolio/p_1/view/v1");

            WebDriverWait waitDataTable = new WebDriverWait(driver, TimeSpan.FromSeconds(30));
            waitDataTable.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementExists(By.XPath("//tr")));

            IWebElement stockTable = driver.FindElement(By.XPath("//tbody"));
            List<IWebElement> stocks = driver.FindElements(By.XPath("//tr")).ToList();

            List<IWebElement> rows = stockTable.FindElements(By.XPath("//tr")).ToList();
            int rowsCount = rows.Count;

            using (var context = new FinanceDB())
            {
                for (int row = 1; row < rowsCount; row++)
                {
                    List<IWebElement> cells = rows.ElementAt(row).FindElements(By.TagName("td")).ToList();
                    int cellsCount = cells.Count;

                    string symbolData = cells.ElementAt(0).Text;
                    string lastPriceData = cells.ElementAt(1).Text;
                    string changeData = cells.ElementAt(2).Text;
                    string changeRateData = cells.ElementAt(3).Text;
                    string currencyData = cells.ElementAt(4).Text;
                    string marketTimeData = cells.ElementAt(5).Text;
                    string volumeData = cells.ElementAt(6).Text;
                    string shareData = cells.ElementAt(7).Text;
                    string averageVolumeData = cells.ElementAt(8).Text;
                    string marketCapData = cells.ElementAt(12).Text;
                    DateTime timeStampData = DateTime.Now;



                    var stockRecord = new Stock
                    {
                        Symbol = symbolData,
                        LastPrice = lastPriceData,
                        Change = changeData,
                        ChangeRate = changeRateData,
                        Currency = currencyData,
                        MarketTime = marketTimeData,
                        Volume = volumeData,
                        Shares = shareData,
                        AverageVolume = averageVolumeData,
                        MarketCap = marketCapData,
                        Timestamp = timeStampData
                    };

                    context.Stocks.Add(stockRecord);
                    context.SaveChanges();
                }
                driver.Close();
                // return RedirectToAction("Recent");
            }
        }


    }

}