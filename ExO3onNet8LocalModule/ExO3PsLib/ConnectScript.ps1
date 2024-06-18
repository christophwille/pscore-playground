# Docs: https://learn.microsoft.com/en-us/powershell/module/exchange/connect-exchangeonline?view=exchange-ps

$VerbosePreference="Continue"

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
-LogDirectoryPath 'd:' `
-LogLevel $logLevel `
-ManagedIdentityAccountId  '' `
-Organization $exoOrganization `
-PageSize 1000 `
-ShowBanner:$false `
-ShowProgress:$false `
-SigningCertificate $null `
-SkipLoadingCmdletHelp `
-SkipLoadingFormatData `
-TrackPerformance:$true `
-UseMultithreading:$true `
-UserPrincipalName ''

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


# -UseRPSSession]
#>