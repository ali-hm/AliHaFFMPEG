# AliHa FFMPEG

A friendly Windows GUI on top of **ffmpeg** for everyday media conversion — pick a file,
choose a preset, watch real progress with ETA. No command line required
(the full ffmpeg command is previewed live for those who want it).

![Main window](docs/screenshots/files-and-queue.png)

## Features

- **Files & Queue tab** — select a single file, use **Add Files**, or **drag & drop**
  a whole folder of media; per-file status (Pending / Converting / Done / Failed),
  Retry Failed, Clear, Remove Selected
- **Live media info** — resolution, codecs, duration and bitrate via ffprobe
- **Real progress** — percent, speed and ETA from a machine-readable progress pipe
- **18 built-in one-click presets** (plus your own saved ones):
  Web/YouTube 1080p, Half the size (x265), Fit in 25 MB, Old TV / USB player (H.264 High@4.1),
  Discord / WhatsApp (small), Remux (copy), MP3 / M4A / WAV audio extract,
  Web 720p, Tiny e-mail file, x264 ultrafast, x265 archive, 10-bit HEVC,
  Vertical video (TikTok/Reels), Loudness fix (EBU R128), Deinterlace, Animated GIF
- **Settings tab** — video/audio codec, CRF, preset, tune, pixel format, profile/level,
  scale, frame rate, subtitle handling (copy/mov_text/drop), trim start/end, extra args
- **Hardware encoding** — NVENC / QSV / AMF are auto-detected and listed when present
- **Codec/container compatibility warnings** before a conversion starts
- **Quality modes** — CRF, fixed video bitrate, or target file size (MB)
- **Command & Log tab** — see exactly what ffmpeg runs, copy it, live log,
  per-job log files, open last log
- **Self-updating** — checks GitHub releases silently on startup, one-click
  download + install; can also **download an official ffmpeg build by itself**
- Options: notify when done, shut down PC when done, open output folder

## Screenshots

| Files & Queue | Settings | Command & Log |
|---|---|---|
| ![Files & Queue](docs/screenshots/files-and-queue.png) | ![Settings](docs/screenshots/settings.png) | ![Command & Log](docs/screenshots/command-and-log.png) |

> Screenshots were captured from a Debug build on a 150% display-scaled
> Windows 11 machine. Your layout may differ slightly with display scaling.

## Quick start

1. Download the newest `AliHaFFMPEG-win-x64.zip` from the
   [releases page](https://github.com/ali-hm/AliHaFFMPEG/releases) and extract it anywhere.
2. On first run, if `ffmpeg.exe` is missing next to the app, accept the offer to
   download an official Windows build (~85 MB) — or place your own
   `ffmpeg.exe` / `ffprobe.exe` beside `AliHaFFMPEG.exe`.
3. Pick a file (or drop in several), choose a preset, press **Convert**.

### Build from source

Requires the [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0).

```powershell
git clone https://github.com/ali-hm/AliHaFFMPEG.git
cd AliHaFFMPEG
dotnet build AliHaFFMPEG.sln
dotnet test AliHaFFMPEG.Core.Tests
dotnet run --project AliHaFFMPEG
```

The solution contains three projects:

| Project | What it is |
|---|---|
| `AliHaFFMPEG` | WinForms UI (thin view over Core) |
| `AliHaFFMPEG.Core` | Command builder, presets, progress parser, media info, updaters — no UI |
| `AliHaFFMPEG.Core.Tests` | 140+ xUnit tests for Core |

### Make a release

```powershell
pwsh ./scripts/release.ps1 -Tag v2.2.0        # or: powershell ./scripts/release.ps1 -Tag v2.2.0
git push --tags
```

This builds the self-contained `win-x64` zip, generates `SHA256SUMS.txt`, and —
once the `release` GitHub workflow exists — the "Check for updates" feature
inside the app will find it.

## How updates work

On startup the app quietly asks
`https://api.github.com/repos/ali-hm/AliHaFFMPEG/releases/latest` whether a
release is newer than the running version (`vX.Y.Z` tags, matching
`<Version>` in `AliHaFFMPEG.csproj`). If so, the About dialog can download the
`-win-x64.zip` asset and apply it without admin rights for a per-user install
(the updater script waits for the app to exit, copies files, restarts it).

You can point the app at a fork by setting `"UpdateRepository": "owner/repo"`
in `%APPDATA%\AliHaFFMPEG\settings.json`.

## License

- **AliHa FFMPEG itself is MIT licensed** — see [LICENSE](LICENSE). Do whatever
  you like with it; a credit is appreciated but not required.
- **ffmpeg/ffprobe are GPL-licensed.** We never bundle ffmpeg in this repo or
  in releases — the app downloads an official build at first run. That keeps
  the MIT application and the GPL tools as clearly separate programs.
