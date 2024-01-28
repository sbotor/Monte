$sourcePath = ".\\"
$destinationPath = "$env:ProgramData\\Monte"

if (!(Test-Path -Path $destinationPath)) {
    New-Item -ItemType Directory -Path $destinationPath
}

Get-ChildItem -Path $sourcePath -Recurse | Copy-Item -Destination $destinationPath

$nssmPath = "$destinationPath\\nssm.exe"
$exePath = "$destinationPath\\main.exe" 

$serviceName = "MonteAgent"

& $nssmPath install $serviceName $exePath
& $nssmPath set $serviceName AppParameters "--config production"
& $nssmPath set $serviceName DisplayName "Agent for Monte"


$service = Get-Service -Name $serviceName -ErrorAction SilentlyContinue

if ($service.Status -ne 'Running') {
    Start-Service -Name $serviceName -ErrorAction Stop
    Write-Host "Service $serviceName started."
} else {
    Write-Host "Service $serviceName already started."
}
