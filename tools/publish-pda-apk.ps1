# Build Stock Control PDA (Release APK) and copy to dist/
$ErrorActionPreference = "Stop"
$root = Split-Path -Parent $PSScriptRoot
$proj = Join-Path $root "src\StockControl.PDA\StockControl.PDA.csproj"
$dist = Join-Path $root "dist"

Write-Host "Publishing Release APK..."
dotnet publish $proj -f net10.0-android -c Release -p:AndroidPackageFormat=apk

$signed = Join-Path $root "src\StockControl.PDA\bin\Release\net10.0-android\publish\com.companyname.stockcontrol.pda-Signed.apk"
if (-not (Test-Path $signed)) {
    throw "Signed APK not found: $signed"
}

New-Item -ItemType Directory -Force -Path $dist | Out-Null
$out = Join-Path $dist "StockControl.PDA-1.0.apk"
Copy-Item $signed $out -Force
Write-Host "Done: $out ($([math]::Round((Get-Item $out).Length / 1MB, 2)) MB)"
