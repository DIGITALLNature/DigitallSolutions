Push-Location -Path $PSScriptRoot/../src/webresources
Get-ChildItem .\ -include output -Recurse | foreach { remove-item $_.fullname -Force -Recurse }

& npm install
& tsc

New-Item -Force -Type Directory output/Icons
Copy-Item -Force -Recurse build/Icons/* output/Icons

Pop-Location