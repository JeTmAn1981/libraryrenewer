# Library Renewer

This is a C# project that uses the Selenium testing framework for automating the renewal of library books via library websites.  This program works to automatically renew checked out books via the websites of both the Spokane Public Library and the Spokane County Library.  It will work for any number of individual accounts (as many as you put in).  As a heavy library user, this has saved me a lot of busy work and potential late fees by eliminating the need to renew books by hand.  

It uses a set cushion time of three days before books are due for renewing them, giving you enough time to return them if there's a hold on the item or you've run out of renewals.  It will automatically notify you by email if there is a problem renewing an item so you'll know about it right away.

Unfortunately, the Selenium drivers are a bit buggy so sometimes the actual browsing process will crash and need to be rerun.  I keep checking in with the driver developers from time to time but there has not yet been a satisfactory permanent fix for this issue.  But the renewer itself works well outside of that.
