Push-Location -Path $PSScriptRoot/../
.\scripts\update-dgtp.ps1
.\scripts\build-webresources.ps1

dgtp push ./src/webresources/output/ --solution DIGITALLSolutions --publish --delete-obsolete

Pop-Location
