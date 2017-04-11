param
(
    [string]$Name = 'test1techash2'
)

$p = Invoke-RestMethod -Uri 'http://localhost/api/v2/configurationProperties/14' | Select-Object Id, Name, Scope, Target, Type, Value
$p.Value = @('A', 'Different', 'Value')
$build = Invoke-RestMethod -uri 'http://localhost/api/v2/configurationProperties/' -Method put -Body ($p | ConvertTo-Json) -ContentType 'application/json'
$build.Build

do 
{
    Write-Host "Waiting for build to complete..."
    Start-Sleep -Seconds 30
    $status = Invoke-RestMethod -Uri "http://localhost/api/v2/builds/$($build.Build.Id)"
} while($status.Status -ne 'Succeeded')
$status