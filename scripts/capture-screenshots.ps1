<#
.SYNOPSIS
    Captures README screenshots of the running AliHa FFMPEG app (Files, Settings, Command tabs).
.DESCRIPTION
    Launches the Debug build (or attaches to a running instance), brings the main window
    to the foreground, selects each tab via SendKeys (Ctrl+Tab), and saves a cropped
    screenshot of the app window to docs/screenshots/*.png.
    Usage: pwsh ./scripts/capture-screenshots.ps1
#>
$ErrorActionPreference = "Stop"
Add-Type -AssemblyName System.Windows.Forms, System.Drawing

$root = Split-Path (Split-Path $MyInvocation.MyCommand.Path -Parent) -Parent
$exe = Join-Path $root "AliHaFFMPEG\bin\Debug\net10.0-windows\AliHaFFMPEG.exe"
$outDir = Join-Path $root "docs\screenshots"
New-Item -ItemType Directory -Force $outDir | Out-Null

$sig = @"
using System;
using System.Runtime.InteropServices;
public static class Win32 {
    [DllImport("user32.dll")] public static extern bool SetForegroundWindow(IntPtr hWnd);
    [DllImport("user32.dll")] public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
    [DllImport("user32.dll")] public static extern bool GetWindowRect(IntPtr hWnd, out RECT r);
    public const int SW_RESTORE = 9;
    [StructLayout(LayoutKind.Sequential)]
    public struct RECT { public int Left, Top, Right, Bottom; }
}
"@
Add-Type -TypeDefinition $sig | Out-Null

function Get-AppProcess {
    $p = Get-Process -Name "AliHaFFMPEG" -ErrorAction SilentlyContinue |
        Where-Object { $_.MainWindowHandle -ne 0 } | Select-Object -First 1
    if (-not $p) {
        Write-Host "Launching $exe ..." -ForegroundColor Green
        $p = Start-Process -FilePath $exe -PassThru
        $p.WaitForInputIdle(15000) | Out-Null
        Start-Sleep -Seconds 3
        $p = Get-Process -Id $p.Id -ErrorAction SilentlyContinue
    }
    return $p
}

function Capture-Tab([string]$name, [int]$tabIndex) {
    $p = Get-AppProcess
    if (-not $p -or $p.MainWindowHandle -eq 0) { throw "App window not found." }
    $h = $p.MainWindowHandle
    [Win32]::ShowWindow($h, [Win32]::SW_RESTORE) | Out-Null
    [Win32]::SetForegroundWindow($h) | Out-Null
    Start-Sleep -Milliseconds 600

    # Select tab: focus tab control then Ctrl+Tab tabIndex times from first tab.
    # Simpler + robust: send Ctrl+1/2/3? WinForms TabControl has no such shortcut,
    # so use Home then Ctrl+Tab steps.
    [System.Windows.Forms.SendKeys]::SendWait("{HOME}")
    Start-Sleep -Milliseconds 200
    for ($i = 0; $i -lt $tabIndex; $i++) {
        [System.Windows.Forms.SendKeys]::SendWait("^{TAB}")
        Start-Sleep -Milliseconds 400
    }
    Start-Sleep -Milliseconds 600

    # Dismiss a possible "ffmpeg not found" modal so the main window shows.
    $modal = Get-Process -Name "AliHaFFMPEG" -ErrorAction SilentlyContinue |
        Where-Object { $_.MainWindowHandle -ne 0 -and $_.MainWindowHandle -ne $h }
    foreach ($m in $modal) {
        [Win32]::SetForegroundWindow($m.MainWindowHandle) | Out-Null
        Start-Sleep -Milliseconds 300
        [System.Windows.Forms.SendKeys]::SendWait("{ESC}")
        Start-Sleep -Milliseconds 300
    }
    [Win32]::SetForegroundWindow($h) | Out-Null
    Start-Sleep -Milliseconds 400

    $rect = New-Object Win32+RECT
    [Win32]::GetWindowRect($h, [ref]$rect) | Out-Null
    $w = $rect.Right - $rect.Left; $hgt = $rect.Bottom - $rect.Top
    if ($w -le 0 -or $hgt -le 0) { throw "Got empty window rect." }

    $bmp = New-Object System.Drawing.Bitmap($w, $hgt)
    try {
        $g = [System.Drawing.Graphics]::FromImage($bmp)
        try { $g.CopyFromScreen($rect.Left, $rect.Top, 0, 0, (New-Object System.Drawing.Size($w, $hgt))) }
        finally { $g.Dispose() }
        $path = Join-Path $outDir "$name.png"
        $bmp.Save($path, [System.Drawing.Imaging.ImageFormat]::Png)
        Write-Host "Saved $path (${w}x${hgt})" -ForegroundColor Green
    }
    finally { $bmp.Dispose() }
}

Capture-Tab "files-and-queue" 0
Capture-Tab "settings" 1
Capture-Tab "command-and-log" 2

Write-Host ""
Write-Host "Done. Review the PNGs, then close the app (it was left running for inspection)."
