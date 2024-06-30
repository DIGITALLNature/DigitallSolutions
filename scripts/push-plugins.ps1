Push-Location -Path $PSScriptRoot/../
.\scripts\build-assemblies.ps1
dgtp push ./src/assemblies/dgt.solutions.Plugins/bin/Release/net462/dgt.solutions.Plugins.dll

Pop-Location