Push-Location -Path $PSScriptRoot/../
.\scripts\build-assemblies.ps1
dgtp push ./src/assemblies/dgt.solutions.Plugins/bin/Release/dgt.solutions.Plugins.dll

Pop-Location