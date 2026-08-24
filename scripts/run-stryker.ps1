# Copyright © Erickson Lopez. MIT License.
$projects = @(
    "EricksonLopez.DomainPrimitives.csproj",
    "EricksonLopez.DomainPrimitives.Abstractions.csproj",
    "EricksonLopez.DomainPrimitives.AspNetCore.csproj",
    "EricksonLopez.DomainPrimitives.EFCore.csproj",
    "EricksonLopez.DomainPrimitives.Dapper.csproj",
    "EricksonLopez.DomainPrimitives.OpenApi.csproj",
    "EricksonLopez.DomainPrimitives.NewtonsoftJson.csproj",
    "EricksonLopez.DomainPrimitives.Testing.csproj"
)

$testsPath = "tests\EricksonLopez.DomainPrimitives.UnitTests"
$failed = $false

foreach ($proj in $projects) {
    Write-Host "Running Stryker for $proj..." -ForegroundColor Cyan
    Set-Location $PSScriptRoot\$testsPath
    dotnet stryker -p $proj -f ..\..\stryker-config.json
    if ($LASTEXITCODE -ne 0) {
        Write-Host "Stryker failed or fell below break threshold for $proj" -ForegroundColor Red
        $failed = $true
    }
}

Set-Location $PSScriptRoot
if ($failed) {
    exit 1
}
Write-Host "All projects passed Stryker quality gate!" -ForegroundColor Green
exit 0
