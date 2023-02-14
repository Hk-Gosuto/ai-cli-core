#!/usr/bin/env pwsh
[CmdletBinding(PositionalBinding = $false)]
param(
    [ValidateSet('Debug', 'Release')]
    $Configuration = $null,
    [switch]
    $ci
)

Set-StrictMode -Version 1
$ErrorActionPreference = 'Stop'

Import-Module -Force -Scope Local "$PSScriptRoot/src/common.psm1"

if (!$Configuration) {
    $Configuration = if ($ci) { 'Release' } else { 'Debug' }
}

$artifacts = "$PSScriptRoot/artifacts/"

Remove-Item -Recurse $artifacts -ErrorAction Ignore

exec dotnet tool restore

[string[]] $formatArgs=@()
if ($ci) {
    $formatArgs += '--check'
}

exec dotnet tool run dotnet-format -- -v detailed @formatArgs
exec dotnet build --configuration $Configuration
exec dotnet pack --no-build --configuration $Configuration -o $artifacts
exec dotnet test --no-build --configuration $Configuration `
    --collect:"XPlat Code Coverage"

write-host -f green 'BUILD SUCCEEDED'
