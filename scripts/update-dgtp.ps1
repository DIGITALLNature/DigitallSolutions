Push-Location -Path $PSScriptRoot/../
dotnet tool update dgt.power --global --prerelease
dgtp profile select DigitallSolutionConcept
Pop-Location