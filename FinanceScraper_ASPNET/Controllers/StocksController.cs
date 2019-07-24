using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using FinanceScraper_ASPNET.Models;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace FinanceScraper_ASPNET.Controllers
{
    public class StocksController : Controller
    {
        private FinanceEntities db = new FinanceEntities();

        // GET: Stocks
        public ActionResult Index()
        {
            return View(db.Stocks.ToList());
        }

        // GET: Stocks/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Stock stock = db.Stocks.Find(id);
            if (stock == null)
            {
                return HttpNotFound();
            }
            return View(stock);
        }

        // GET: Stocks/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Stocks/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Symbol,LastPrice,Change,ChangeRate,Currency,MarketTime,Volume,Shares,AverageVolume,DayRange,C52WeekRange,MarketCap")] Stock stock)
        {
            if (ModelState.IsValid)
            {
                db.Stocks.Add(stock);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(stock);
        }

        // GET: Stocks/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Stock stock = db.Stocks.Find(id);
            if (stock == null)
            {
                return HttpNotFound();
            }
            return View(stock);
        }

        // POST: Stocks/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Symbol,LastPrice,Change,ChangeRate,Currency,MarketTime,Volume,Shares,AverageVolume,DayRange,C52WeekRange,MarketCap")] Stock stock)
        {
            if (ModelState.IsValid)
            {
                db.Entry(stock).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(stock);
        }

        // GET: Stocks/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Stock stock = db.Stocks.Find(id);
            if (stock == null)
            {
                return HttpNotFound();
            }
            return View(stock);
        }

        // POST: Stocks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Stock stock = db.Stocks.Find(id);
            db.Stocks.Remove(stock);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        
        public ActionResult Scrape()
        {
            IWebDriver driver = new ChromeDriver();

            driver.Navigate().GoToUrl("https://finance.yahoo.com/");

            WebDriverWait waitLogin = new WebDriverWait(driver, TimeSpan.FromSeconds(20));
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

            WebDriverWait waitDataTable = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            waitDataTable.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementExists(By.XPath("//tr")));

            IWebElement stockTable = driver.FindElement(By.XPath("//tbody"));
            List<IWebElement> stocks = driver.FindElements(By.XPath("//tr")).ToList();

            List<IWebElement> rows = stockTable.FindElements(By.XPath("//tr")).ToList();
            int rowsCount = rows.Count;

            using (var context = new FinanceEntities())
            {
                //var stockTable = driver.FindElement(By.XPath("//tbody"));

                for (int row = 1; row < rowsCount; row++)
                {
                    List<IWebElement> cells = rows.ElementAt(row).FindElements(By.TagName("td")).ToList();
                    int cellsCount = cells.Count;


                    // string cellText = cells.ElementAt(cell).ToString();
                    // Console.WriteLine("Data: " + cellText);
                    string symbolData = cells.ElementAt(0).Text;
                    Console.WriteLine("Symbol: " + symbolData);
                    string lastPriceData = cells.ElementAt(1).Text;
                    Console.WriteLine("Last Price: " + lastPriceData);
                    string changeData = cells.ElementAt(2).Text;
                    Console.WriteLine("Change: " + changeData);
                    string changeRateData = cells.ElementAt(3).Text;
                    Console.WriteLine("Change Rate: " + changeRateData);
                    string currencyData = cells.ElementAt(4).Text;
                    Console.WriteLine("Currency: " + currencyData);
                    string marketTimeData = cells.ElementAt(5).Text;
                    Console.WriteLine("Market Time: " + marketTimeData);
                    string volumeData = cells.ElementAt(6).Text;
                    Console.WriteLine("Volume: " + volumeData);
                    string shareData = cells.ElementAt(7).Text;
                    Console.WriteLine("Shares: " + shareData);
                    string averageVolumeData = cells.ElementAt(8).Text;
                    Console.WriteLine("Avg Vol Data: " + averageVolumeData);
                    string marketCapData = cells.ElementAt(12).Text;
                    Console.WriteLine("Market Cap: " + marketCapData);

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
                        MarketCap = marketCapData
                    };

                    context.Stocks.Add(stockRecord);
                    context.SaveChanges();
                }

                return View();
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
