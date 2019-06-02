using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

// Requires reference to WebDriver.Support.dll

namespace LibraryRenewer
{
    partial class Program
    {
        public abstract class Library
        {
            private const int CUSHION_DAYS = 3;
            protected string checkoutsTabSelector;
            protected string errorSelector;
            protected string itemLinkSelector;
            protected string cardNumberFieldSelector, passwordFieldSelector, loginButtonSelector;
            protected string selectionBoxSelector,dateElementSelector;
            protected string logoutLink;
            protected string accountURL, startURL, libraryName;
            protected string rowSelector;
            protected string renewButtonSelector = "#myCheckouts_checkoutslist_topCheckoutsRenewButton";
            protected string confirmRenewSelector = "#myCheckouts_checkoutslist_checkoutsDialogConfirm";

            protected List<Account> accounts;

            protected void Login(string cardNumber, string password)
            {
                driver.Navigate().GoToUrl(startURL);

                IWebElement cardNumberField;

                try
                {
                     cardNumberField = driver.FindElement(By.CssSelector(cardNumberFieldSelector));
                }
                catch (Exception ex)
                {

                    throw;
                }

                IWebElement passwordField = driver.FindElement(By.CssSelector(passwordFieldSelector));
                IWebElement submitButton = driver.FindElement(By.CssSelector(loginButtonSelector));
                
                cardNumberField.SendKeys(cardNumber);
                passwordField.SendKeys(password);

                submitButton.Click();
            }

            public void FindErrors(List<IWebElement> items)
            {
                
                            foreach (var item in items)
                {
                    try
                    {

                        var itemText = item.Text;
                        var itemString = item.ToString();
                        var html = item.GetAttribute("outerHTML");

                        var foundErrors = item.FindElements(By.CssSelector(errorSelector)).ToList();

                        if (foundErrors.Count > 0)
                        {
                            var error = foundErrors[0];

                            var itemLink = item.FindElement(By.CssSelector(itemLinkSelector));

                            if (!error.Text.Contains("Item renewed successfully."))
                            {
                                errors.Add(libraryName + ": " + error.Text + Environment.NewLine + itemLink.Text + Environment.NewLine + itemLink.GetAttribute("href"));
                            }

                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
            }

            protected void ShowLoginInputs()
            {
                IWebElement loginLink = driver.FindElement(By.CssSelector("a.loginLink"));
                loginLink.Click();
            }

            protected abstract void SwitchToIFrame();

            private void Renew()
            {
                var rows = driver.FindElements(By.CssSelector(rowSelector)).ToList().Where(row => row.FindElements(By.CssSelector(dateElementSelector)).Count > 0).ToList();

                var eachDate = rows.Select(row => Regex.Replace(row.FindElements(By.CssSelector(dateElementSelector))[0].GetAttribute("innerText"),@"[^\d\-\\\/]","").Trim()).ToList();

                int renewCount = 0;

                for (int currentRow = 0; currentRow < rows.Count(); currentRow++)
                {
                    if (ItemNeedsRenewal(rows[currentRow]))
                    {
                        SelectItem(rows[currentRow]);
                        renewCount++;
                    }
                }

                if (renewCount > 0)
                {
                    try
                    {
                        ClickRenewButton();
                        ConfirmRenew();
                    }
                    catch (Exception ex)
                    {

                        throw;
                    }
                    System.Threading.Thread.Sleep(10000);
                                }

                rows = driver.FindElements(By.CssSelector(rowSelector)).ToList().Where(row => row.FindElements(By.CssSelector(dateElementSelector)).Count > 0).ToList();

                FindErrors(rows);
            }

            
            private void ClickRenewButton()
            {
                var renewButton = driver.FindElement(By.CssSelector(renewButtonSelector));
                renewButton.SendKeys(Keys.Space);
            }

            protected abstract void ConfirmRenew();

            public void RenewItems()
            {
                accounts.ForEach(account =>
                {
                    Login(account.cardNumber, account.password);
                    ShowCheckouts();
                    Renew();
                    Logout();
                });
            }

            private void SelectItem(IWebElement item)
            {
                var selectionBox = item.FindElements(By.CssSelector(selectionBoxSelector)).First();
                var disabled = selectionBox.GetAttribute("disabled");

                if (disabled != "true")
                {
                    selectionBox.SendKeys(Keys.Space);
                }
                
            }

            private bool ItemNeedsRenewal(IWebElement item)
            {
                var dateElement = item.FindElements(By.CssSelector(dateElementSelector))[0];

                DateTime dueDate = new DateTime();

                var currentDate = DateTime.Now;
                string retrievedDate = "";
                string month = "";
                string day = "";
                string  year = "";

                try
                {
                    var foundDate = Regex.Match(dateElement.GetAttribute("innerText"), @"(\d{1,2})\/(\d{1,2})\/(\d{2})|(\d{4})\-(\d{1,2})\-(\d{1,2})");
                   
                    if (foundDate.Groups[1].ToString() != "")
                    {
                        year = foundDate.Groups[3].ToString();
                        month = foundDate.Groups[1].ToString();
                        day = foundDate.Groups[2].ToString();
                    }

                    else if (foundDate.Groups[4].ToString() != "")
                    {
                        year = foundDate.Groups[4].ToString();
                        month = foundDate.Groups[5].ToString();
                        day = foundDate.Groups[6].ToString();
                    }
                    else
                    {
                        errors.Add("Could not find valid due date for: " + item.ToString() + Environment.NewLine);
                        return false;
                    }

                    dueDate = Convert.ToDateTime(month + "/" + day + "/" + year);

                    var test1 = Regex.Match("1/8/2019", @"(\d{1,2})\/(\d{1,2})\/(\d{2})|(\d{4})\-(\d{1,2})\-(\d{1,2})");
                    var test2 = Regex.Match("2019-1-8", @"(\d{1,2})\/(\d{1,2})\/(\d{2})|(\d{4})\-(\d{1,2})\-(\d{1,2})");
                }
                catch
                {
                  errors.Add("Could not find valid due date for: " + item.ToString() + Environment.NewLine);
                    return false;
                }

                //Test date
                //DateTime.TryParse("2/20/2018", out currentDate);

                var dateDue = dueDate.Date;

                if (dateDue <= currentDate.Date.AddDays(CUSHION_DAYS))
                {
                    return true;
                }
                else if (dueDate.Date < currentDate.Date)
                {
                    errors.Add("Item past due!  Was due on " + dueDate.Date + Environment.NewLine);
                }
                                           
                return false;
            }

            protected abstract void SetupVariables();

            protected void Logout()
            {
                driver.Navigate().GoToUrl(logoutLink);
                System.Threading.Thread.Sleep(2000);
            }
            protected void ShowCheckouts()
            {
                NavigateToAccountPage();
                ClickCheckoutsTab();
            }
            protected virtual void NavigateToAccountPage()
            {

            }

            private void ClickCheckoutsTab()
            {
                var checkoutsTab = driver.FindElement(By.CssSelector(checkoutsTabSelector));
                checkoutsTab.Click();
            }

            public Library(List<Account> accounts)
            {
                this.accounts = accounts;
                SetupVariables();
            }
        }

    }

}
