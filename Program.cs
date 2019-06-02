using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Chrome;

// Requires reference to WebDriver.Support.dll
using OpenQA.Selenium.Support.UI;
using System.Net.Mail;
using System.Net;
using System.Diagnostics;

namespace LibraryRenewer
{
    partial class Program
    {
        public static IWebDriver driver;
        private static List<Library> libraries;
        private static List<String> errors = new List<String>();
        private static bool test = false;
        
        static void Main(string[] args)
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            try
            {
                DoSetup();

                libraries.ForEach(library => library.RenewItems());

                SendNotification();
                CloseDriver();
            }
            catch (Exception ex)
            {
                var localEX = ex;

                SendErrorNotification(ex);
                CloseDriver();
            }

            stopWatch.Stop();
        }

        private static void DoSetup()
        {
            SetupDriver();
            SetupLibraries();
        }

        private static void SetupDriver()
        {
            var options = new ChromeOptions();

            options.AddArgument("no-sandbox");
            //options.AddArgument("headless");
            options.AddArgument("disable-gpu");
            //driver = new FirefoxDriver();
            //driver = new ChromeDriver(options);
            driver = new ChromeDriver(@"C:\LibraryRenewer\bin\Release", options, TimeSpan.FromSeconds(130));

            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
        }

        private static void CloseDriver()
        {
            try
            {
                driver.Close();
                driver.Quit();
            }
            catch (Exception)
            {
            }
        }

        private static void SetupLibraries()
        {
            libraries = new List<Library>();

            if (test)
            {
                List<Account> accounts = new List<Account>();

                accounts.Add(new Account("redacted", "redacted"));
                accounts.Add(new Account("redacted", "redacted"));


                libraries.Add(new CountyLibrary(accounts));
            }
            else
            {
                AddCityLibrary();
                AddCountyLibrary();
            }
        }

        private static void AddCountyLibrary()
        {
            List<Account> accounts = new List<Account>();

            accounts.Add(new Account("redacted", "redacted"));
            accounts.Add(new Account("redacted", "redacted"));
            libraries.Add(new CountyLibrary(accounts));
        }

        private static void AddCityLibrary()
        {
            List<Account> accounts = new List<Account>();

            accounts.Add(new Account("redacted", "redacted"));
            accounts.Add(new Account("redacted", "redacted"));

            libraries.Add(new CityLibrary(accounts));

        }

        private static void SendErrorNotification(Exception ex)
        {
            MailMessage msg;
            SmtpClient client;

            SetupEmail(out msg, out client);

            msg.Subject = "Error encountered during library renewal";
            msg.Body = "The following error was encountered:" + Environment.NewLine + Environment.NewLine + ex.ToString();
            client.Send(msg);
            msg.Dispose();
        }


        private static void SendNotification()
        {
            MailMessage msg;
            SmtpClient client;

            SetupEmail(out msg, out client);

            if (test)
            {
                msg.Subject = "Library Renewer Test";
                    msg.Body = "Test email.";
            }
                else
            {
                if (errors.Count > 0)
                {
                    msg.Subject += " - Renewal Failed";
                    msg.Body = "The following error messages were encountered:" + Environment.NewLine + Environment.NewLine;

                    foreach (var error in errors)
                    {
                        msg.Body += error + Environment.NewLine + Environment.NewLine;
                    }

                    client.Send(msg);
                    msg.Dispose();
                }
            }

            //else
            //{
            //    msg.Subject += " - No Renewals Required";
            //}

        }

        private static void SetupEmail(out MailMessage msg, out SmtpClient client)
        {
            string emailAddress = "redacted@redacted.com";

            msg = new MailMessage();
            msg.From = new MailAddress(emailAddress);
            msg.To.Add(emailAddress);
            msg.Subject = "Library Renewal Status";

            client = new SmtpClient();
            client.UseDefaultCredentials = true;
            client.Host = "smtp.gmail.com";
            client.Port = 587;
            client.EnableSsl = true;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.Credentials = new NetworkCredential(emailAddress, "redacted");
            client.Timeout = 20000;
        }

    }

}
