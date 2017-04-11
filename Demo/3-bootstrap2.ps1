param
(
    [string]$Name = 'demo1techash2'
)

mkdir C:\Users\mike.walker\Repos\SummitDemo\Logs\$($Name) -ErrorAction SilentlyContinue | Out-Null
docker run --rm -it -v C:\Users\mike.walker\Repos\SummitDemo\Modules:C:\Modules -e dsc_endpoint="http://192.169.10.1/api/v2" -e dsc_client_name="$Name" -v C:\Users\mike.walker\Repos\SummitDemo\Logs\$($Name):C:\Windows\Temp\Dsc dsc-client:latest -File C:\run-bootstrap.ps1