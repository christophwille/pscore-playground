# Docs: https://learn.microsoft.com/en-us/powershell/module/exchange/connect-exchangeonline?view=exchange-ps

Connect-ExchangeOnline -AppId $exoAppId `
-Organization $exoOrganization `
-Certificate $exoCertificate `
-ShowProgress:$false `
-ShowBanner:$false `
-SkipLoadingFormatData:$true `
-SkipLoadingCmdletHelp:$true

# EXPLICIT positional everything below for TESTING

<#
[string[]] $emptyStringArray = @("*");
[Microsoft.Online.CSE.RestApiPowerShellModule.Instrumentation.LogLevel] $logLevel = [Microsoft.Online.CSE.RestApiPowerShellModule.Instrumentation.LogLevel]::Default;

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
-BypassMailboxAnchoring:$false `
-Certificate $exoCertificate `
-CertificateFilePath '' `
-CertificatePassword $null `
-CertificateThumbprint '' `
-Credential $null `
-Device:$false `
-EnableErrorReporting:$false `
-InlineCredential:$false `
-LogDirectoryPath '' `
-LogLevel $logLevel `
-ManagedIdentity:$false `
-ManagedIdentityAccountId  '' `
-Organization $exoOrganization `
-PageSize 1000 `
-ShowBanner:$false `
-ShowProgress:$false `
-SkipLoadingFormatData:$true
# -TrackPerformance <Boolean>]
# -UseMultithreading <Boolean>]
# -UserPrincipalName <String>]
# -UseRPSSession]
#>