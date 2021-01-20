Connect-ExchangeOnline -AppId $exoAppId -Organization $exoOrganization -Certificate $exoCertificate
Out-File -FilePath d:\Process.txt -InputObject $exoAppId