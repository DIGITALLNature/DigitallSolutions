Push-Location -Path $PSScriptRoot/../
.\scripts\update-dgtp.ps1
dgtp codegeneration ./libs/d365.extension.Model/ -c  ./libs/d365.extension.Model/model.json
Pop-Location