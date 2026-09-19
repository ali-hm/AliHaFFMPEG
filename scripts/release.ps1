<# Copyright (c) 2026 Ali Hamidi - MIT License. See LICENSE in the repo root.
<#
.SYNOPSIS
    Release a new version. Two modes: Local (zip built here) or Remote (zip built by CI).
.DESCRIPTION
    Both modes take -Version, write it into the csproj, run the tests, commit
    "Release vX.Y.Z", tag it and create the GitHub Release. They differ only in
    who builds the zip:

    -Mode Local  - the zip is built on this machine:
        pwsh ./scripts/release.ps1 -Mode Local -Version 2.2.0
      1. Refuses to run on a dirty working tree (commit or stash first).
      2. Writes <Version>2.2.0</Version> into AliHaFFMPEG/AliHaFFMPEG.csproj.
      3. Runs the tests - a red build can never be released.
      4. Commits the bump as "Release v2.2.0" and tags it v2.2.0.
      5. Builds release/AliHaFFMPEG-win-x64.zip + SHA256SUMS.txt
         + RELEASE_NOTES.md (changelog from commits since the previous tag).
      6. Pushes the branch, then creates the GitHub Release and uploads the
         local zip + checksums. gh creates the remote tag at the release
         commit - no tag-push event, so CI does not build a second zip.

    -Mode Remote - the GitHub workflow builds the zip:
        pwsh ./scripts/release.ps1 -Mode Remote -Version 2.2.0
      1-4. Same as Local (bump, test, commit, tag).
      5. Writes release/RELEASE_NOTES.md but does NOT build a zip.
      6. Pushes the branch and the tag - the tag push triggers the "release"
         workflow, which builds, zips and uploads the asset.
      7. Creates the GitHub Release itself (without assets) so the notes and
         ordering are deterministic; if the workflow already created it, the
         script just refreshes the notes.

    Both modes need the GitHub CLI for the release step (gh + `gh auth login`);
    Local mode refuses to start without it, Remote mode falls back to letting
    the workflow create the release.

    Flags: -SkipBuild (Local: reuse the existing publish\ output; invalid in
    Remote), -NoCommit, -NoTag, -NoPush (dry run: stop before any remote action).
#>
param(
    [ValidateSet("Local", "Remote")]
    [string]$Mode = "",
    [string]$Version = "",
    [switch]$SkipBuild,
    [switch]$NoCommit,
    [switch]$NoTag,
    [switch]$NoPush
)

$ErrorActionPreference = "Stop"
$root = Split-Path (Split-Path $MyInvocation.MyCommand.Path -Parent) -Parent
$csproj = Join-Path $root "AliHaFFMPEG/AliHaFFMPEG.csproj"
$notesScript = Join-Path $root "scripts/Get-ReleaseNotes.ps1"

# --- 0. Mode + version are required in both modes ---
if ($Mode -eq "")
{
    throw "Pass -Mode Local (zip built on this machine) or -Mode Remote (zip built by the GitHub workflow)."
}

if ($Version -eq "")
{
    throw "-Version x.y.z is required in both modes, e.g. -Mode $Mode -Version 2.2.0."
}

if ($Version -notmatch '^\d+\.\d+\.\d+$')
{
    throw "Version must look like 2.2.0 (got '$Version')."
}

$Tag = "v$Version"

if ($Mode -eq "Remote" -and $SkipBuild)
{
    throw "-SkipBuild only makes sense in -Mode Local - Remote mode never builds locally."
}

function Test-GhAvailable
{
    return $null -ne (Get-Command gh -ErrorAction SilentlyContinue)
}

# In Local mode the zip IS the release, so without gh we could push a tag but
# never publish the asset. Fail before spending minutes on a build.
if ($Mode -eq "Local" -and -not $NoPush -and -not (Test-GhAvailable))
{
    throw "gh (GitHub CLI) was not found - it is required to create the release with the local zip.`r`n" +
          "Install it:  winget install GitHub.cli    then:  gh auth login"
}

# --- 1. Refuse to release a dirty tree. This runs BEFORE the version bump below,
#        otherwise the bump itself would make the tree dirty and always fail. ---
$dirty = @(git -C $root status --porcelain)
if ($dirty.Count -gt 0)
{
    throw "Working tree is not clean - commit or stash first:`r`n$($dirty -join "`r`n")"
}

# --- 2. Bump <Version> in the csproj (the single source of truth) ---
$text = Get-Content $csproj -Raw
if ($text -notmatch '<Version>\d+\.\d+\.\d+</Version>')
{
    throw "Could not find a <Version>x.y.z</Version> line in $csproj"
}

$updated = $text -replace '<Version>\d+\.\d+\.\d+</Version>', "<Version>$Version</Version>"
$bumped = $updated -ne $text
if ($bumped)
{
    [System.IO.File]::WriteAllText($csproj, $updated)
    Write-Host "Bumped <Version> to $Version in AliHaFFMPEG.csproj" -ForegroundColor Green
}
else
{
    Write-Host "<Version> is already $Version - no bump needed." -ForegroundColor Green
}

[xml]$xml = Get-Content $csproj
$fileVersion = "$($xml.Project.PropertyGroup.Version)"
if ([string]::IsNullOrWhiteSpace($fileVersion)) { throw "Could not read <Version> from $csproj" }

# --- 3. Tests must pass before anything is committed or tagged ---
Write-Host "Running tests..." -ForegroundColor Green
dotnet test (Join-Path $root "AliHaFFMPEG.Core.Tests/AliHaFFMPEG.Core.Tests.csproj") -c Release
if ($LASTEXITCODE -ne 0) { throw "tests failed - refusing to release" }

# --- 4 + 5. Commit the version bump and create the tag ---
# Only commit when the bump actually changed the csproj - a re-run for the same
# version (e.g. after a failed build) has nothing to commit and must not abort.
if ($bumped -and -not $NoCommit)
{
    git -C $root add -- AliHaFFMPEG/AliHaFFMPEG.csproj
    git -C $root commit -m "Release $Tag"
    if ($LASTEXITCODE -ne 0) { throw "git commit failed" }
    Write-Host "Committed release $Tag" -ForegroundColor Green
}

if (-not $NoTag)
{
    $existing = @(git -C $root tag --list $Tag)
    if ($existing.Count -gt 0)
    {
        throw "Tag '$Tag' already exists. Delete it first if you really want to redo this release."
    }

    git -C $root tag -a $Tag -m "AliHa FFMPEG $Tag"
    if ($LASTEXITCODE -ne 0) { throw "git tag failed" }
    Write-Host "Created tag $Tag" -ForegroundColor Green
}

$releaseSha = (git -C $root rev-parse HEAD).Trim()

# --- 6. Release notes from git history (same source the workflow uses) ---
Write-Host "Writing release notes ..." -ForegroundColor Green
$releaseDir = Join-Path $root "release"
New-Item -ItemType Directory -Force $releaseDir | Out-Null
$changelog = & $notesScript -Tag $Tag
$notesPath = Join-Path $releaseDir "RELEASE_NOTES.md"

# --- 7. Mode-specific work ---
$zipName = "AliHaFFMPEG-win-x64.zip"
$zipPath = Join-Path $releaseDir $zipName

if ($Mode -eq "Local")
{
    # Build the zip on this machine - this is what gets attached to the release.
    if (-not $SkipBuild)
    {
        $stagingDir = Join-Path $root "publish"
        # start from a clean staging folder so stale files never leak into the zip
        if (Test-Path $stagingDir) { Remove-Item $stagingDir -Recurse -Force }

        Write-Host "Publishing AliHa FFMPEG v$fileVersion (win-x64, self-contained, single file)..." -ForegroundColor Green
        dotnet publish (Join-Path $root "AliHaFFMPEG/AliHaFFMPEG.csproj") `
            -c Release -r win-x64 --self-contained true `
            /p:PublishSingleFile=true `
            /p:IncludeNativeLibrariesForSelfExtract=true `
            /p:PublishTrimmed=false `
            /p:DebugType=none `
            -o $stagingDir
        if ($LASTEXITCODE -ne 0) { throw "dotnet publish failed" }

        $exePath = Join-Path $stagingDir "AliHaFFMPEG.exe"
        if (-not (Test-Path $exePath)) { throw "Expected $exePath but it does not exist." }

        if (Test-Path $zipPath) { Remove-Item $zipPath -Force }
        Write-Host "Zipping $zipName ..." -ForegroundColor Green
        Compress-Archive -Path $exePath -DestinationPath $zipPath -Force

        Write-Host "Writing checksums ..." -ForegroundColor Green
        $hash = (Get-FileHash $zipPath -Algorithm SHA256).Hash.ToLowerInvariant()
        "$hash  $zipName" | Set-Content -NoNewline (Join-Path $releaseDir "SHA256SUMS.txt")
    }

    if (-not (Test-Path $zipPath))
    {
        throw "Local mode needs $zipPath - drop -SkipBuild so the script builds it."
    }

    $hash = (Get-FileHash $zipPath -Algorithm SHA256).Hash.ToLowerInvariant()
    $notes = @(
        "Self-contained Windows build of AliHa FFMPEG $Tag (x64).",
        "Extract the zip anywhere and run AliHaFFMPEG.exe.",
        "ffmpeg itself is downloaded on first run (About > Download ffmpeg).",
        "",
        $changelog,
        "",
        "SHA256 ($zipName): $hash"
    ) -join "`r`n"
    $notes | Set-Content -NoNewline $notesPath

    Write-Host ""
    Write-Host "Local artifacts:" -ForegroundColor Green
    Write-Host "  $zipPath"
    Write-Host "  $(Join-Path $releaseDir 'SHA256SUMS.txt')"
    Write-Host "  $notesPath"
}
else
{
    # Remote: CI builds the zip. The script only writes the notes here; the
    # workflow attaches zip + checksums to the release this script creates.
    $notes = @(
        "Self-contained Windows build of AliHa FFMPEG $Tag (x64).",
        "Extract the zip anywhere and run AliHaFFMPEG.exe.",
        "ffmpeg itself is downloaded on first run (About > Download ffmpeg).",
        "",
        $changelog
    ) -join "`r`n"
    $notes | Set-Content -NoNewline $notesPath
    Write-Host "  $notesPath (the workflow builds and attaches the zip + checksums)"
}

# --- 8. Push and create the release ---
if ($NoPush)
{
    Write-Host ""
    Write-Host "Dry run (-NoPush) - nothing was pushed or published. When ready:" -ForegroundColor Yellow
    if ($Mode -eq "Local")
    {
        Write-Host "  git -C `"$root`" push origin HEAD"
        Write-Host "  gh release create $Tag `"$zipPath`" `"$(Join-Path $releaseDir 'SHA256SUMS.txt')`" --title `"AliHa FFMPEG $Tag`" --notes-file `"$notesPath`" --target $releaseSha --latest"
        Write-Host "(gh release create also creates the remote tag at --target - do NOT push the tag yourself,"
        Write-Host " a tag push would trigger the release workflow and build a second zip.)"
    }
    else
    {
        Write-Host "  git -C `"$root`" push origin HEAD"
        Write-Host "  git -C `"$root`" push origin $Tag   # triggers the release workflow"
        Write-Host "  gh release create $Tag --title `"AliHa FFMPEG $Tag`" --notes-file `"$notesPath`"   # or let the workflow create it"
    }
    return
}

$repoUrl = git -C $root remote get-url origin
# gh wants OWNER/REPO, not a URL - parse it out of the remote
$repoSlug = $repoUrl -replace '\.git$', '' -replace '.*github\.com[:/]', ''
if ($repoSlug -notmatch '^[^/]+/[^/]+$')
{
    throw "Could not derive OWNER/REPO from the origin remote ('$repoUrl')."
}

if ($Mode -eq "Local")
{
    git -C $root push origin HEAD
    if ($LASTEXITCODE -ne 0) { throw "git push failed" }

    # gh release create publishes the tag remotely (at --target) together with
    # the release and the assets. Because no tag PUSH happens, the release
    # workflow is not triggered and CI does not build a duplicate zip.
    gh release create $Tag $zipPath (Join-Path $releaseDir "SHA256SUMS.txt") `
        --repo $repoSlug `
        --title "AliHa FFMPEG $Tag" `
        --notes-file $notesPath `
        --target $releaseSha `
        --latest
    if ($LASTEXITCODE -ne 0) { throw "gh release create failed" }

    Write-Host "Released $Tag with the locally built zip - see it on GitHub." -ForegroundColor Green
}
else
{
    git -C $root push origin HEAD
    if ($LASTEXITCODE -ne 0) { throw "git push failed" }

    git -C $root push origin $Tag
    if ($LASTEXITCODE -ne 0) { throw "git push of tag $Tag failed" }
    Write-Host "Pushed tag $Tag - the release workflow is building the zip." -ForegroundColor Green

    if (-not (Test-GhAvailable))
    {
        Write-Host "gh not found - skipping release creation here; the workflow will create the release itself." -ForegroundColor Yellow
        return
    }

    # Create the release shell (no assets - the workflow attaches the zip).
    gh release create $Tag --repo $repoSlug `
        --title "AliHa FFMPEG $Tag" --notes-file $notesPath --latest
    if ($LASTEXITCODE -ne 0)
    {
        # The workflow may have won the race and created it already.
        gh release edit $Tag --repo $repoSlug --notes-file $notesPath
        if ($LASTEXITCODE -ne 0) { throw "could not create or edit the release for $Tag" }
        Write-Host "Release $Tag already existed (workflow created it) - notes refreshed." -ForegroundColor Yellow
    }
    else
    {
        Write-Host "Created release $Tag - the workflow will attach the zip + checksums." -ForegroundColor Green
    }
}