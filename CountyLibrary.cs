using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Linq;

// Requires reference to WebDriver.Support.dll

namespace LibraryRenewer
{
    partial class Program
    {
        public class CountyLibrary : Library
        {
            public CountyLibrary(List<Account> accounts) : base(accounts) { }
            
            protected override void SetupVariables()
            {
                accountURL = "https://scld.ent.sirsi.net/client/en_US/default/search/account?";
                startURL = "https://scld.ent.sirsi.net/client/en_US/ext/search/patronlogin/https:$002f$002fscld.ent.sirsi.net$002fclient$002fen_US$002fext$002fsearch$002faccount$003f";
                rowSelector = ".checkoutsLine";
                libraryName = "Spokane County Library";

                checkoutsTabSelector = "a[href='#checkoutsTab']";
                errorSelector = "p.authBreak .checkoutsError";
                itemLinkSelector = "[id*='checkoutTitleLinkDiv'] a";
                selectionBoxSelector = ".checkoutsCoverArt input";
                dateElementSelector = ".checkoutsDueDate";
                //logoutLinkSelector = ".loginLink a";
                logoutLink = "https://scld.ent.sirsi.net/client/en_US/default/index.template.header.mainmenu_0.logout?";

                renewButtonSelector = "#myCheckouts_checkoutslist_topCheckoutsRenewButton";
                confirmRenewSelector = "#myCheckouts_checkoutslist_checkoutsDialogConfirm";

                cardNumberFieldSelector = "#j_username";
                passwordFieldSelector = "#j_password";
                loginButtonSelector = "#submit_0";
            }

            protected override void NavigateToAccountPage()
            {
                  
            }

            protected override void ConfirmRenew()
            {
                var confirmRenew = driver.FindElement(By.CssSelector(confirmRenewSelector));
                confirmRenew.SendKeys(Keys.Space);
            }

            protected override void SwitchToIFrame()
            {
                try
                {
                    var iFrames = driver.FindElement(By.CssSelector("iframe"));
                    var id = iFrames.GetAttribute("name");

                    driver.SwitchTo().DefaultContent(); // you are now outside both frames
                    driver.SwitchTo().Frame(id);
                }
                catch (Exception ex)
                {

                }
            }
                      
          
        }

    }

}
