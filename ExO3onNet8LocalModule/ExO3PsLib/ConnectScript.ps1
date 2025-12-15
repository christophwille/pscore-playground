# Docs: https://learn.microsoft.com/en-us/powershell/module/exchange/connect-exchangeonline?view=exchange-ps

$VerbosePreference="Continue"

# https://github.com/MicrosoftDocs/PowerShell-Docs/blob/main/reference/7.2/Microsoft.PowerShell.Utility/Set-TraceSource.md
# Set-TraceSource -Name '*' -FilePath 'd:\createdbutremainsempty-toinvestigate.txt' -ListenerOption "ProcessId,TimeStamp"

[string[]] $emptyStringArray = @("*");
[Microsoft.Online.CSE.RestApiPowerShellModule.Instrumentation.LogLevel] $logLevel = [Microsoft.Online.CSE.RestApiPowerShellModule.Instrumentation.LogLevel]::All;

Connect-ExchangeOnline -ConnectionUri '' `
-AzureADAuthorizationEndpointUri '' `
-ExchangeEnvironmentName 'O365Default' `
-PSSessionOption $null `
-DelegatedOrganization '' `
-Prefix '' `
-CommandName $emptyStringArray `
-FormatTypeName $emptyStringArray `
-AccessToken '' `
-AppId $exoAppId `
-Certificate $exoCertificate `
-CertificateFilePath '' `
-CertificatePassword $null `
-CertificateThumbprint '' `
-Credential $null `
-EnableErrorReporting `
-LogDirectoryPath 'z:' `
-LogLevel $logLevel `
-ManagedIdentityAccountId  '' `
-Organization $exoOrganization `
-PageSize 1000 `
-ShowBanner:$false `
-ShowProgress:$false `
-SigningCertificate $null `
-SkipLoadingCmdletHelp:$true `
-SkipLoadingFormatData:$true `
-TrackPerformance:$true `
-UseMultithreading:$true `
-UserPrincipalName ''

<#
  v3.9.1-Preview1 :
    1. Introduced -EXOModuleBasePath  switch in Connect-ExchangeOnline, which enables to store the temporary EXO module files in a custom path.
#>

<#
Connect-ExchangeOnline -AppId $exoAppId `
-Organization $exoOrganization `
-Certificate $exoCertificate `
-ShowBanner:$false `
-SkipLoadingFormatData:$true `
-SkipLoadingCmdletHelp:$true `
#>

# EXPLICIT positional everything below for TESTING

<#

<# 
  v3.9.2-Preview1 :
    2. Deprecated UseRpsSession parameter from Connect-ExchangeOnline and Connect-IPPSSession.
#>

# Set-TraceSource -name '*' -RemoveFileListener