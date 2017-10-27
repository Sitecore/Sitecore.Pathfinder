function Initialize-Project {
    & dotnet $PSScriptRoot/bin/Sitecore.Pathfinder.App.dll init-project $args
}

function New-Code {
    & dotnet $PSScriptRoot/bin/Sitecore.Pathfinder.App.dll generate-code $args
}

function New-File {
    & dotnet $PSScriptRoot/bin/Sitecore.Pathfinder.App.dll generate-file $args
}

function New-Schemas {
    & dotnet $PSScriptRoot/bin/Sitecore.Pathfinder.App.dll generate-schemas $args
}

function Publish-Project {
    & dotnet $PSScriptRoot/bin/Sitecore.Pathfinder.App.dll publish-project $args
}

function Show-Config {
    & dotnet $PSScriptRoot/bin/Sitecore.Pathfinder.App.dll show-config $args
}

function Show-Help {
    & dotnet $PSScriptRoot/bin/Sitecore.Pathfinder.App.dll help $args
}

function Test-Project {
    & dotnet $PSScriptRoot/bin/Sitecore.Pathfinder.App.dll check-project $args
}

function Write-Items {
    & dotnet $PSScriptRoot/bin/Sitecore.Pathfinder.App.dll extract-items $args
}

Export-ModuleMember -function Initialize-Project
Export-ModuleMember -function New-Code
Export-ModuleMember -function New-File
Export-ModuleMember -function New-Schemas
Export-ModuleMember -function Publish-Project
Export-ModuleMember -function Show-Config
Export-ModuleMember -function Show-Help
Export-ModuleMember -function Test-Project
Export-ModuleMember -function Write-Items
