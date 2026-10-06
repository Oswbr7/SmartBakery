<#
  Smart Bakery - Levanta los 3 servicios en ventanas separadas.
  Requiere haber ejecutado scripts\setup.ps1 antes.
#>
$Root = Split-Path -Parent $PSScriptRoot

$services = @(
    @{ Name = 'ML Service (8000)'; Dir = 'ml-service';                 Cmd = '.\.venv\Scripts\python.exe -m uvicorn app.main:app --reload --port 8000' },
    @{ Name = 'Backend (5001)';    Dir = 'backend\SmartBakery';        Cmd = 'dotnet run --project src\SmartBakery.API --launch-profile https' },
    @{ Name = 'Frontend (5173)';   Dir = 'frontend\smart-bakery-web';  Cmd = 'npm run dev' }
)

foreach ($s in $services) {
    $dir = Join-Path $Root $s.Dir
    $command = "`$Host.UI.RawUI.WindowTitle = 'Smart Bakery - $($s.Name)'; Set-Location '$dir'; $($s.Cmd)"
    Start-Process powershell -ArgumentList '-NoExit', '-NoProfile', '-ExecutionPolicy', 'Bypass', '-Command', $command
}

Write-Host 'Servicios iniciados:'
Write-Host '  Frontend : http://localhost:5173'
Write-Host '  API      : https://localhost:5001/swagger  (health: /health)'
Write-Host '  ML       : http://localhost:8000/docs      (health: /health)'
