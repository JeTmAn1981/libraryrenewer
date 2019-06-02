using OpenQA.Selenium.Interactions;

// Requires reference to WebDriver.Support.dll

namespace LibraryRenewer
{
    partial class Program
    {

        public class Account
        {
            public string cardNumber;
            public string password;
            
            public Account(string cardNumber, string password)
            {
                this.cardNumber = cardNumber;
                this.password = password;
            }

        }

    }

}
