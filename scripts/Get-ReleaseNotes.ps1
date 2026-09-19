<# Copyright (c) 2026 Ali Hamidi - MIT License. See LICENSE in the repo root.
<#
.SYNOPSIS
    Builds the changelog ("What's Changed") section for a release from git history.
.DESCRIPTION
    Lists commit subjects that landed since the previous vX.Y.Z tag (or since
    the repository root when there is none), one per bullet. Used by
    scripts/release.ps1 and .github/workflows/release.yml so the local
    dry-run notes and the real GitHub Release notes are identical.
    Usage: pwsh ./scripts/Get-ReleaseNotes.ps1 [-Tag v2.2.0]
#>
param(
    [string]$Tag = ""
)

$ErrorActionPreference = "Stop"

# Resolve the repository root from this script's own location so the script works
# no matter which directory the caller is in (release.ps1, the workflow, or you).
$root = Split-Path (Split-Path $MyInvocation.MyCommand.Path -Parent) -Parent

function Get-PreviousVersionTag([string]$currentTag)
{
    $tags = @(git -C $root tag --list 'v[0-9]*' --sort=-v:refname 2>$null)
    foreach ($t in $tags)
    {
        $t = "$t".Trim()
        if ($t -ne "" -and $t -ne $currentTag) { return $t }
    }
    return ""
}

$range = ""
$previous = Get-PreviousVersionTag $Tag
if ($previous -ne "")
{
    # everything reachable from HEAD that is not already in the previous release
    $range = "$previous..HEAD"
}

$lines = @()
if ($range -ne "")
{
    try
    {
        $lines = @(git -C $root log --format=%s --no-merges $range 2>$null | Where-Object { "$_" -ne "" })
    }
    catch
    {
        $lines = @()
    }
}

$output = @()
$output += "## What's Changed"
$output += ""
if ($lines.Count -eq 0)
{
    $output += "- No changes recorded since the previous release."
}
else
{
    foreach ($line in $lines)
    {
        $output += "- " + "$line".Trim()
    }
}

$output -join "`r`n"
