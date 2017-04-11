Installation
============

1. Install WMF5 RTM
2. Install PackageManagement providers
3. Copy application files to install location
4. Run install.ps1:

```powershell
& 'C:\Program Files\Ticketmaster\DscManager\Install.ps1' -UseDscManagerApi $true -UseJobServer $false -UseWebRepository $true -Role Preprod
```

For a mof building box, `-UseDscManagerApi` and `UseJobServer` should both be true.