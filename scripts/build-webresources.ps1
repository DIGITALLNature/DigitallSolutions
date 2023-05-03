Push-Location -Path $PSScriptRoot/../src/webresources
& npm install
& tsc

New-Item -Force -Type Directory output/Icons
Copy-Item -Force -Recurse build/Icons/* output/Icons

Pop-Location