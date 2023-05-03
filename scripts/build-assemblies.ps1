Push-Location -Path $PSScriptRoot/../
.\scripts\update-model.ps1
.\scripts\git-submodule.ps1

Get-ChildItem .\ -include bin,obj -Recurse | foreach { remove-item $_.fullname -Force -Recurse }

& nuget restore dgt.solutions.Assemblies.sln

if($IsWindows)
{
    $vs = & "${env:ProgramFiles(x86)}\Microsoft Visual Studio\Installer\vswhere.exe" -version "[16.0,18.0)" -products * -requires Microsoft.Component.MSBuild -prerelease -latest -utf8 -format json | ConvertFrom-Json
    $msbuild = [IO.Path]::Combine($vs[0].installationPath, "MSBuild", "Current", "Bin", "MSBuild.exe")
    & $msbuild /t:Build dgt.solutions.Assemblies.sln /property:Configuration=Release
} else {
    & msbuild /t:Build dgt.solutions.Assemblies.sln /property:Configuration=Release
}

Pop-Location

