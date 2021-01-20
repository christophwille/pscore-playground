#  Exchange Online PowerShell V2 Module Tests 

Tested with https://www.powershellgallery.com/packages/ExchangeOnlineManagement/ 2.0.4-Preview8 

Secrets need to be entered in

* ExoV203CertAuthN\Program.cs
* ExoV203CertAuthN\ExoCertAuthN.cs

(by the directory name you can tell the testing started with first release of 2.0.3)

A note on the base64 encoded cert: I tested initially with KeyVault, and then simply base64 encoded that on
retrieval to have an easier debug/test experience across various demo projects (yep, lazy and wanted to avoid
the version conflict that is documented in the code).
