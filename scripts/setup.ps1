<#
  Smart Bakery - Setup de entorno local (Windows)

  Que hace:
    1. Verifica prerequisitos: Git, .NET 10 SDK, Node.js, Python 3.11+, PostgreSQL (psql).
       Si falta Git/.NET/Node/Python y winget esta disponible, ofrece instalarlo.
       PostgreSQL se instala manualmente (requiere definir contrasena).
    2. Backend:  agrega paquetes NuGet, instala dotnet-ef (tool local), restore y build.
    3. Frontend: npm ci.
    4. ML:       crea .venv e instala requirements-dev.txt.
    5. Crea archivos .env desde .env.example (si no existen).
    6. Inicializa Git (sin hacer commit).
    7. (Opcional) Crea la base de datos smart_bakery en PostgreSQL local.

  Uso:
    Doble clic en scripts\setup.cmd
    o bien:  powershell -ExecutionPolicy Bypass -File scripts\setup.ps1
#>

$ErrorActionPreference = 'Stop'
$Root     = Split-Path -Parent $PSScriptRoot
$Backend  = Join-Path $Root 'backend\SmartBakery'
$Frontend = Join-Path $Root 'frontend\smart-bakery-web'
$MlDir    = Join-Path $Root 'ml-service'

$Summary = [System.Collections.Generic.List[string]]::new()

function Write-Step($msg) { Write-Host "`n==> $msg" -ForegroundColor Cyan }
function Write-Ok($msg)   { Write-Host "    [OK] $msg" -ForegroundColor Green }
function Write-Warn2($msg){ Write-Host "    [!]  $msg" -ForegroundColor Yellow }
function Write-Err($msg)  { Write-Host "    [X]  $msg" -ForegroundColor Red }

function Test-Cmd($name) { [bool](Get-Command $name -ErrorAction SilentlyContinue) }

function Update-SessionPath {
    $machine = [Environment]::GetEnvironmentVariable('Path', 'Machine')
    $user    = [Environment]::GetEnvironmentVariable('Path', 'User')
    $env:Path = "$machine;$user"
}

function Confirm-Yes($question) {
    $answer = Read-Host "    $question [S/n]"
    return ($answer -eq '' -or $answer -match '^(s|si|y|yes)$')
}

function Install-WithWinget($id, $label) {
    if (-not (Test-Cmd 'winget')) {
        Write-Err "winget no esta disponible. Instala $label manualmente."
        return $false
    }
    if (-not (Confirm-Yes "$label no se encontro. Instalar con winget ($id)?")) { return $false }
    winget install --id $id -e --accept-package-agreements --accept-source-agreements
    Update-SessionPath
    return $true
}

function Invoke-Native {
    param([string]$File, [string[]]$Arguments)
    & $File @Arguments
    if ($LASTEXITCODE -ne 0) { throw "Fallo: $File $($Arguments -join ' ') (exit $LASTEXITCODE)" }
}

# ---------------------------------------------------------------------------
# 1. Prerequisitos
# ---------------------------------------------------------------------------
Write-Step 'Verificando prerequisitos'

# Git
if (-not (Test-Cmd 'git')) { Install-WithWinget 'Git.Git' 'Git' | Out-Null }
if (Test-Cmd 'git') { Write-Ok "Git $((git --version) -replace 'git version ','')" } else { Write-Err 'Git no disponible' }

# .NET 10 SDK
function Test-Dotnet10 {
    if (-not (Test-Cmd 'dotnet')) { return $false }
    $sdks = dotnet --list-sdks 2>$null
    return [bool]($sdks | Where-Object { $_ -match '^10\.' })
}
if (-not (Test-Dotnet10)) { Install-WithWinget 'Microsoft.DotNet.SDK.10' '.NET 10 SDK' | Out-Null }
$HasDotnet = Test-Dotnet10
if ($HasDotnet) { Write-Ok ".NET SDK $((dotnet --list-sdks | Where-Object { $_ -match '^10\.' } | Select-Object -Last 1).Split(' ')[0])" }
else { Write-Err '.NET 10 SDK no disponible (https://dotnet.microsoft.com/download/dotnet/10.0)' }

# Node.js (Vite requiere Node 20.19+ o 22.12+)
function Test-Node {
    if (-not (Test-Cmd 'node')) { return $false }
    $v = [version]((node --version).TrimStart('v'))
    return ($v -ge [version]'22.12.0') -or ($v.Major -eq 20 -and $v -ge [version]'20.19.0')
}
if (-not (Test-Node)) { Install-WithWinget 'OpenJS.NodeJS.LTS' 'Node.js LTS' | Out-Null }
$HasNode = Test-Node
if ($HasNode) { Write-Ok "Node.js $(node --version) / npm $(npm --version)" }
else { Write-Err 'Node.js 20.19+ / 22.12+ no disponible (https://nodejs.org)' }

# Python 3.11+
function Get-PythonCmd {
    foreach ($candidate in @(@('py', '-3'), @('python'))) {
        if (Test-Cmd $candidate[0]) {
            try {
                $args2 = @()
                if ($candidate.Count -gt 1) { $args2 += $candidate[1] }
                $out = & $candidate[0] @args2 -c "import sys; print('%d.%d' % sys.version_info[:2])" 2>$null
                if ($LASTEXITCODE -eq 0 -and $out -and ([version]$out -ge [version]'3.11')) { return ,$candidate }
            } catch { }
        }
    }
    return $null
}
$Py = Get-PythonCmd
if (-not $Py) { Install-WithWinget 'Python.Python.3.13' 'Python 3.13' | Out-Null; $Py = Get-PythonCmd }
if ($Py) {
    $pyArgs = @(); if ($Py.Count -gt 1) { $pyArgs += $Py[1] }
    Write-Ok "Python $(& $Py[0] @pyArgs --version)"
} else { Write-Err 'Python 3.11+ no disponible (https://www.python.org/downloads/)' }

# PostgreSQL
$HasPsql = Test-Cmd 'psql'
if (-not $HasPsql) {
    $pgBin = Get-ChildItem 'C:\Program Files\PostgreSQL\*\bin\psql.exe' -ErrorAction SilentlyContinue |
             Sort-Object FullName -Descending | Select-Object -First 1
    if ($pgBin) { $env:Path = "$($pgBin.DirectoryName);$env:Path"; $HasPsql = $true }
}
if ($HasPsql) { Write-Ok "PostgreSQL $((psql --version) -replace 'psql \(PostgreSQL\) ','')" }
else { Write-Warn2 'PostgreSQL no encontrado. Instalalo manualmente (https://www.postgresql.org/download/windows/) y recuerda la contrasena del usuario postgres. Necesario a partir de la Fase 1.' }

# ---------------------------------------------------------------------------
# 2. Backend (.NET)
# ---------------------------------------------------------------------------
if ($HasDotnet) {
    Write-Step 'Backend .NET: paquetes NuGet'
    Push-Location $Backend
    try {
        $packages = [ordered]@{
            'src\SmartBakery.Application\SmartBakery.Application.csproj' = @(
                'MediatR',
                'FluentValidation',
                'FluentValidation.DependencyInjectionExtensions'
            )
            'src\SmartBakery.Infrastructure\SmartBakery.Infrastructure.csproj' = @(
                'Microsoft.EntityFrameworkCore',
                'Npgsql.EntityFrameworkCore.PostgreSQL',
                'Microsoft.Extensions.Http'
            )
            'src\SmartBakery.API\SmartBakery.API.csproj' = @(
                'Microsoft.AspNetCore.OpenApi',
                'Swashbuckle.AspNetCore.SwaggerUI',
                'Microsoft.AspNetCore.Authentication.JwtBearer',
                'Microsoft.EntityFrameworkCore.Design',
                'AspNetCore.HealthChecks.NpgSql'
            )
            'tests\SmartBakery.UnitTests\SmartBakery.UnitTests.csproj' = @(
                'Microsoft.NET.Test.Sdk',
                'xunit.v3',
                'xunit.runner.visualstudio',
                'NSubstitute'
            )
            'tests\SmartBakery.IntegrationTests\SmartBakery.IntegrationTests.csproj' = @(
                'Microsoft.NET.Test.Sdk',
                'xunit.v3',
                'xunit.runner.visualstudio',
                'NSubstitute',
                'Microsoft.AspNetCore.Mvc.Testing'
            )
        }

        foreach ($project in $packages.Keys) {
            $content = Get-Content $project -Raw
            foreach ($pkg in $packages[$project]) {
                if ($content -match "Include=`"$([regex]::Escape($pkg))`"") {
                    Write-Ok "$pkg ya presente en $(Split-Path $project -Leaf)"
                    continue
                }
                Write-Host "    + $pkg -> $(Split-Path $project -Leaf)"
                Invoke-Native 'dotnet' @('add', $project, 'package', $pkg)
            }
        }

        Write-Step 'Backend .NET: herramienta dotnet-ef (local)'
        # .NET 10 crea el manifiesto en la raiz (dotnet-tools.json); versiones anteriores en .config\
        $manifest = @('dotnet-tools.json', '.config\dotnet-tools.json') | Where-Object { Test-Path $_ } | Select-Object -First 1
        if (-not $manifest) {
            Invoke-Native 'dotnet' @('new', 'tool-manifest')
            $manifest = @('dotnet-tools.json', '.config\dotnet-tools.json') | Where-Object { Test-Path $_ } | Select-Object -First 1
        }
        if (-not ((Get-Content $manifest -Raw) -match '"dotnet-ef"')) {
            Invoke-Native 'dotnet' @('tool', 'install', 'dotnet-ef')
        }
        Invoke-Native 'dotnet' @('tool', 'restore')

        Write-Step 'Backend .NET: restore + build'
        Invoke-Native 'dotnet' @('restore', 'SmartBakery.slnx')
        Invoke-Native 'dotnet' @('build', 'SmartBakery.slnx', '--no-restore')
        $Summary.Add('Backend: paquetes instalados y solucion compilada')

        Write-Step 'Certificado HTTPS de desarrollo'
        dotnet dev-certs https --check --trust *> $null
        if ($LASTEXITCODE -ne 0) {
            Write-Host '    Se abrira un dialogo de Windows para confiar en el certificado local.'
            dotnet dev-certs https --trust
        }
        Write-Ok 'Certificado de desarrollo listo'
    }
    catch { Write-Err $_; $Summary.Add('Backend: ERROR (ver salida)') }
    finally { Pop-Location }
}

# ---------------------------------------------------------------------------
# 3. Frontend (React + Vite)
# ---------------------------------------------------------------------------
if ($HasNode) {
    Write-Step 'Frontend: npm ci'
    Push-Location $Frontend
    try {
        Invoke-Native 'npm' @('ci')
        $Summary.Add('Frontend: dependencias npm instaladas')
    }
    catch { Write-Err $_; $Summary.Add('Frontend: ERROR (ver salida)') }
    finally { Pop-Location }
}

# ---------------------------------------------------------------------------
# 4. ML Service (Python + FastAPI)
# ---------------------------------------------------------------------------
if ($Py) {
    Write-Step 'ML Service: entorno virtual + dependencias'
    Push-Location $MlDir
    try {
        if (-not (Test-Path '.venv\Scripts\python.exe')) {
            $pyArgs = @(); if ($Py.Count -gt 1) { $pyArgs += $Py[1] }
            Invoke-Native $Py[0] ($pyArgs + @('-m', 'venv', '.venv'))
        }
        $venvPy = Join-Path $MlDir '.venv\Scripts\python.exe'
        Invoke-Native $venvPy @('-m', 'pip', 'install', '--upgrade', 'pip')
        Invoke-Native $venvPy @('-m', 'pip', 'install', '-r', 'requirements-dev.txt')
        $Summary.Add('ML: .venv creado y dependencias instaladas')
    }
    catch { Write-Err $_; $Summary.Add('ML: ERROR (ver salida)') }
    finally { Pop-Location }
}

# ---------------------------------------------------------------------------
# 5. Archivos .env
# ---------------------------------------------------------------------------
Write-Step 'Archivos .env'
foreach ($dir in @($Root, $Frontend, $MlDir)) {
    $example = Join-Path $dir '.env.example'
    $target  = Join-Path $dir '.env'
    if ((Test-Path $example) -and -not (Test-Path $target)) {
        Copy-Item $example $target
        Write-Ok "Creado $target"
    }
}

# Carpeta de CI (GitHub Actions, Fase 9)
New-Item -ItemType Directory -Force -Path (Join-Path $Root '.github\workflows') | Out-Null
New-Item -ItemType File -Force -Path (Join-Path $Root '.github\workflows\.gitkeep') | Out-Null

# ---------------------------------------------------------------------------
# 6. Git
# ---------------------------------------------------------------------------
if (Test-Cmd 'git') {
    Write-Step 'Git'
    if (-not (Test-Path (Join-Path $Root '.git'))) {
        Push-Location $Root
        git init -b main | Out-Null
        Pop-Location
        Write-Ok 'Repositorio inicializado (rama main, sin commits)'
    } else { Write-Ok 'Repositorio Git ya existe' }
}

# ---------------------------------------------------------------------------
# 7. Base de datos (opcional)
# ---------------------------------------------------------------------------
if ($HasPsql) {
    Write-Step 'PostgreSQL'
    if (Confirm-Yes 'Crear la base de datos "smart_bakery" (usuario postgres)?') {
        $exists = psql -U postgres -h localhost -tAc "SELECT 1 FROM pg_database WHERE datname='smart_bakery'"
        if ($exists -eq '1') { Write-Ok 'La base de datos smart_bakery ya existe' }
        else {
            psql -U postgres -h localhost -c 'CREATE DATABASE smart_bakery;'
            if ($LASTEXITCODE -eq 0) { Write-Ok 'Base de datos smart_bakery creada'; $Summary.Add('PostgreSQL: base smart_bakery creada') }
            else { Write-Err 'No se pudo crear la base de datos' }
        }
        Write-Warn2 'Recuerda ajustar usuario/password en .env (DATABASE_CONNECTION_STRING).'
    }
}

# ---------------------------------------------------------------------------
# Resumen
# ---------------------------------------------------------------------------
Write-Step 'Resumen'
$Summary | ForEach-Object { Write-Host "    - $_" }
Write-Host @"

  Para levantar los servicios (una terminal por servicio), o ejecuta scripts\start-dev.cmd:

    ML Service :  cd ml-service ; .\.venv\Scripts\Activate.ps1 ; uvicorn app.main:app --reload --port 8000
    Backend    :  cd backend\SmartBakery ; dotnet run --project src\SmartBakery.API --launch-profile https
    Frontend   :  cd frontend\smart-bakery-web ; npm run dev

  URLs:  Frontend http://localhost:5173 | API https://localhost:5001/swagger | ML http://localhost:8000/docs
"@ -ForegroundColor Gray
