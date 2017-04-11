param
(
    [Hashtable]$ConfigurationData,
    [string]$OutputPath,
	[string]$ConfigurationPackageName,
	[Version]$ConfigurationPackageVersion,
    [string[]]$IncludeConfiguration,
    [string[]]$ExcludeConfiguration,
    [switch]$SuppressDocumentation,
    [string]$CertificateThumbprint,
	[string]$DscManagerEndpoint
)

$params = $PSBoundParameters
if ([string]::IsNullOrEmpty($params['CertificateThumbprint']))
{
	$params.Remove('CertificateThumbprint') | Out-Null
}

Import-Module -Name $ConfigurationPackageName -Version $ConfigurationPackageVersion -ErrorAction Stop

Write-Verbose 'Configuration module has been imported. Launching configuration...'
Invoke-DscConfigurationBuild @params