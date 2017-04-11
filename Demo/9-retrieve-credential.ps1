param
(
    [string]$Name = 'testCredential'
)

$inputObject = Invoke-RestMethod -Uri 'http://localhost/api/v2/credentials/1' -Method Get -ContentType 'application/json'

$props = @{}
$key = [System.Convert]::FromBase64String($inputObject.EncryptedKey)
$securePassword = ConvertTo-SecureString -String $inputObject.EncryptedPassword -Key $key
$props['CredentialObject'] = New-Object PSCredential -ArgumentList @($inputObject.Username, $securePassword)
$props['Name'] = $inputObject.Name
$props['Id'] = $inputObject.Id
$props['CertificateThumbprint'] = $inputObject.CertificateThumbprint

New-Object PSObject -Property $props