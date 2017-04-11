param
(
    [string]$Name = 'testCredential'
)

$credential = Get-Credential

$key = New-Object byte[](32)
$rng = [System.Security.Cryptography.RNGCryptoServiceProvider]::Create()
$rng.GetBytes($key)
$encryptedString = ConvertFrom-SecureString -SecureString $credential.Password -Key $key
$body = @{ Name = $Name; Username = $credential.UserName; EncryptedKey = [System.Convert]::ToBase64String($key); EncryptedPassword = $encryptedString }

Invoke-RestMethod -Uri 'http://localhost/api/v2/credentials' -Method Post -Body ($body | ConvertTo-Json) -ContentType 'application/json'

return
if($this.Id -ne 0)
{
    $body.Add('Id', $this.Id)
}
