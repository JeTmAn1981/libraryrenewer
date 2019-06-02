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
        public class CityLibrary : Library
        {

            public CityLibrary(List<Account> accounts) : base(accounts) { }

            protected override void SetupVariables()
            {
                startURL = "https://www.spokanelibrary.org/login";
                accountURL = "https://catalog.spokanelibrary.org/client/en_US/pub/search/account?";
                libraryName = "Spokane City Library";

                rowSelector = "#cko li.list-group-item";
                renewButtonSelector = "button.spl-submit-cko";

                checkoutsTabSelector = "a[href='#cko']";
                errorSelector = ".alert";
                itemLinkSelector = "h4 a";
                //    logoutLinkSelector = ".menu-logout a";
                logoutLink = "https://www.spokanelibrary.org/account/?logout";
                selectionBoxSelector = "input.spl-field-cko-select-item";
                dateElementSelector = "dl dd";

                cardNumberFieldSelector = "input[name='username']";
                passwordFieldSelector = "input[name='password']"; ;
                loginButtonSelector = "input.btn[type='submit']";
            }

            protected override void SwitchToIFrame()
            {

            }

            protected override void ConfirmRenew()
            {
      
            }
        }

    }

}
