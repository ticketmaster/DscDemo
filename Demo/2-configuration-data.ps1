param
(
    [string]$Name = 'test1techash2'
)

Invoke-RestMethod -Uri "http://localhost/api/v2/nodes/$Name/configurationData"