Push-Location -Path $PSScriptRoot/../
.\scripts\update-model.ps1
.\scripts\git-submodule.ps1

& dotnet build dgt.solutions.Assemblies.sln -c Release

Pop-Location

