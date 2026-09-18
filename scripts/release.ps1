<# Copyright (c) 2026 Ali Hamidi - MIT License. See LICENSE in the repo root.
<#
.SYNOPSIS
    Builds the self-contained win-x64 release zip of AliHa FFMPEG.
.DESCRIPTION
    Reads the version from AliHaFFMPEG/AliHaFFMPEG.csproj (<Version>),
    publishes a single-file self-contained win-x64 build WITHOUT ffmpeg
    (ffmpeg is downloaded at first run - see About > Download ffmpeg),
    zips it as AliHaFFMPEG-win-x64.zip and writes SHA256SUMS.txt.
    Usage:  pwsh ./scripts/release.ps1 [-Tag v2.2.0] [-SkipBuild]
#>
param(
    [string]$Tag = "",
    [switch]$SkipBuild
)

$ErrorActionPreference = "Stop"
$root = Split-Path (Split-Path $MyInvocation.MyCommand.Path -Parent) -Parent
$csproj = Join-Path $root "AliHaFFMPEG/AliHaFFMPEG.csproj"

[xml]$xml = Get-Content $csproj
$version = $xml.Project.PropertyGroup.Version
if ([string]::IsNullOrWhiteSpace($version)) { throw "Could not read <Version> from $csproj" }

if ($Tag -ne "")
{
    $expected = "v$version"
    if ($Tag -ne $expected)
    {
        throw "Tag '$Tag' does not match <Version>$version</Version> ($expected). Keep csproj and tag in sync."
    }
}

$publishDir = Join-Path $root "publish"
if (-not $SkipBuild)
{
    Write-Host "Publishing AliHa FFMPEG v$version (win-x64, self-contained, single file)..." -ForegroundColor Green
    dotnet publish (Join-Path $root "AliHaFFMPEG/AliHaFFMPEG.csproj") `
        -c Release -r win-x64 --self-contained true `
        /p:PublishSingleFile=true `
        /p:IncludeNativeLibrariesForSelfExtract=true `
        /p:PublishTrimmed=false `
        -o $publishDir
    if ($LASTEXITCODE -ne 0) { throw "dotnet publish failed" }
}

$zipName = "AliHaFFMPEG-win-x64.zip"
$zipPath = Join-Path $root $zipName
if (Test-Path $zipPath) { Remove-Item $zipPath -Force }

Write-Host "Zipping $zipName ..." -ForegroundColor Green
Compress-Archive -Path (Join-Path $publishDir "*") -DestinationPath $zipPath -Force

Write-Host "Writing checksums ..." -ForegroundColor Green
$hash = (Get-FileHash $zipPath -Algorithm SHA256).Hash.ToLowerInvariant()
"$hash  $zipName" | Set-Content -NoNewline (Join-Path $root "SHA256SUMS.txt")

Write-Host ""
Write-Host "Done:" -ForegroundColor Green
Write-Host "  $zipPath"
Write-Host "  SHA256: $hash"
Write-Host ""
Write-Host "Next: create release 'v$version' on GitHub, attach $zipName, paste the SHA256SUMS.txt content into the notes."