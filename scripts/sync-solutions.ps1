Push-Location -Path $PSScriptRoot/../solutions\DIGITALLSolutions
pac install latest
pac auth select --name DigitallSolutionConcept
pac solution sync --async
Pop-Location