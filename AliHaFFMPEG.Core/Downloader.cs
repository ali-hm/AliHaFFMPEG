using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace AliHaFFMPEG.Core
{
    /// <summary>Downloads a release asset and (for ffmpeg) installs the tools next to the app.</summary>
    public static class Downloader
    {
        public static async Task<string> DownloadFileAsync(string url, string targetFile, IProgress<int> progress)
        {
            var directory = Path.GetDirectoryName(targetFile);
            if (!string.IsNullOrEmpty(directory))
            {
                Directory.CreateDirectory(directory);
            }

            using (var client = new HttpClient { Timeout = TimeSpan.FromMinutes(60) })
            using (var response = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false))
            {
                response.EnsureSuccessStatusCode();
                var total = response.Content.Headers.ContentLength ?? 0;

                using (var source = await response.Content.ReadAsStreamAsync().ConfigureAwait(false))
                using (var destination = File.Create(targetFile))
                {
                    var buffer = new byte[81920];
                    long read = 0;
                    int count;
                    while ((count = await source.ReadAsync(buffer, 0, buffer.Length).ConfigureAwait(false)) > 0)
                    {
                        await destination.WriteAsync(buffer, 0, count).ConfigureAwait(false);
                        read += count;
                        if (progress != null && total > 0)
                        {
                            progress.Report((int)Math.Min(100, read * 100 / total));
                        }
                    }
                }
            }

            return targetFile;
        }
    }

    /// <summary>Reads the installed ffmpeg version and can fetch a fresh build into the app folder.</summary>
    public static class FfmpegManager
    {
        /// <summary>BtbN builds: shared-library Windows builds (ffmpeg.exe + ffprobe.exe + DLLs).</summary>
        public const string DefaultBuildRepository = "BtbN/FFmpeg-Builds";

        /// <summary>Asset we look for: the win64 gpl shared build.</summary>
        public const string PreferredAssetSuffix = "win64-gpl-shared.zip";

        /// <summary>
        /// True for "ffmpeg-master-latest-win64-gpl-shared.zip" and
        /// "ffmpeg-n9.0-latest-win64-gpl-shared-9.0.zip"; false for lgpl, winarm64, non-zip files.
        /// </summary>
        public static bool IsPreferredBuildAsset(string name)
        {
            if (string.IsNullOrEmpty(name) ||
                !name.EndsWith(".zip", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            const string marker = "win64-gpl-shared";
            var markerIndex = name.IndexOf(marker, StringComparison.OrdinalIgnoreCase);
            if (markerIndex < 0)
            {
                return false;
            }

            var rest = name.Substring(markerIndex + marker.Length);
            var withoutExtension = rest.Substring(0, rest.Length - ".zip".Length);
            if (withoutExtension.Length == 0)
            {
                return true;
            }

            return withoutExtension.StartsWith("-") &&
                   Version.TryParse(withoutExtension.Substring(1), out _);
        }

        /// <summary>
        /// Always-available fallback (the rolling "master" shared build) used when the
        /// release list cannot be queried.
        /// </summary>
        public const string FallbackWindowsBuildUrl =
            "https://github.com/BtbN/FFmpeg-Builds/releases/download/latest/ffmpeg-master-latest-win64-gpl-shared.zip";

        public class BuildAsset
        {
            public string Name { get; set; }
            public string Url { get; set; }
            public long Size { get; set; }
            public Version Version { get; set; }
        }

        public static string GetVersion(string ffmpegPath)
        {
            if (string.IsNullOrEmpty(ffmpegPath) || !System.IO.File.Exists(ffmpegPath))
            {
                return null;
            }

            try
            {
                var psi = new System.Diagnostics.ProcessStartInfo
                {
                    FileName = ffmpegPath,
                    Arguments = "-hide_banner -version",
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                };

                using (var process = System.Diagnostics.Process.Start(psi))
                {
                    if (process == null)
                    {
                        return null;
                    }

                    var readTask = process.StandardOutput.ReadToEndAsync();
                    if (!readTask.Wait(5000))
                    {
                        try { process.Kill(); } catch { }
                        return null;
                    }

                    process.WaitForExit(2000);
                    var firstLine = readTask.Result.Split('\n')[0].Trim();
                    return string.IsNullOrEmpty(firstLine) ? null : firstLine;
                }
            }
            catch
            {
                return null;
            }
        }

        /// <summary>Version in a build name like "ffmpeg-n9.0-latest-win64-gpl-shared-9.0.zip" (null for master builds).</summary>
        public static Version ExtractBuildVersion(string assetName)
        {
            if (string.IsNullOrEmpty(assetName))
            {
                return null;
            }

            var marker = assetName.IndexOf("-latest-win64-gpl-shared", StringComparison.OrdinalIgnoreCase);
            if (marker < 0)
            {
                return null;
            }

            var head = assetName.Substring(0, marker);
            var dash = head.LastIndexOf('-');
            var token = (dash >= 0 ? head.Substring(dash + 1) : head).TrimStart('n', 'N');
            return Version.TryParse(token, out var version) ? version : null;
        }

        /// <summary>
        /// Picks the best Windows shared build from a GitHub release payload:
        /// highest stable version first, the rolling "master" build last.
        /// </summary>
        public static BuildAsset ParseWindowsBuildAsset(string releaseJson)
        {
            if (string.IsNullOrEmpty(releaseJson))
            {
                return null;
            }

            try
            {
                var root = Newtonsoft.Json.Linq.JObject.Parse(releaseJson);
                if (root["assets"] is not Newtonsoft.Json.Linq.JArray assets)
                {
                    return null;
                }

                BuildAsset best = null;
                foreach (var asset in assets)
                {
                    var name = (string)asset["name"];
                    if (!IsPreferredBuildAsset(name))
                    {
                        continue;
                    }

                    var candidate = new BuildAsset
                    {
                        Name = name,
                        Url = (string)asset["browser_download_url"],
                        Size = (long?)asset["size"] ?? 0,
                        Version = ExtractBuildVersion(name)
                    };

                    if (best == null || IsBetter(candidate, best))
                    {
                        best = candidate;
                    }
                }

                return best;
            }
            catch
            {
                return null;
            }
        }

        private static bool IsBetter(BuildAsset candidate, BuildAsset current)
        {
            // A versioned release always beats the rolling master build.
            if (candidate.Version == null)
            {
                return false;
            }

            if (current.Version == null)
            {
                return true;
            }

            return candidate.Version > current.Version;
        }

        /// <summary>Asks the GitHub API for the newest win64 shared build.</summary>
        public static async Task<BuildAsset> ResolveWindowsBuildAsync(string repository = DefaultBuildRepository)
        {
            try
            {
                using (var client = new System.Net.Http.HttpClient { Timeout = TimeSpan.FromSeconds(20) })
                {
                    client.DefaultRequestHeaders.UserAgent.ParseAdd("AliHaFFMPEG-Updater");
                    client.DefaultRequestHeaders.Accept.ParseAdd("application/vnd.github+json");

                    var json = await client
                        .GetStringAsync("https://api.github.com/repos/" + repository + "/releases/latest")
                        .ConfigureAwait(false);

                    return ParseWindowsBuildAsset(json);
                }
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Downloads a BtbN shared build and installs ffmpeg.exe, ffprobe.exe and the shared
        /// libraries (DLLs) into the target folder. Returns a short report.
        /// </summary>
        public static async Task<string> DownloadAndInstallAsync(string installDirectory, IProgress<int> progress,
            string repository = DefaultBuildRepository)
        {
            var asset = await ResolveWindowsBuildAsync(repository).ConfigureAwait(false);
            var url = asset?.Url ?? FallbackWindowsBuildUrl;
            var fileName = asset?.Name ?? "ffmpeg-master-latest-win64-gpl-shared.zip";

            var tempRoot = Path.Combine(Path.GetTempPath(), "aliha_ffmpeg_" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(tempRoot);

            try
            {
                var zipPath = Path.Combine(tempRoot, fileName);
                await Downloader.DownloadFileAsync(url, zipPath, progress).ConfigureAwait(false);

                var report = InstallFromArchive(zipPath, installDirectory);
                return report + " (from " + fileName + ")";
            }
            finally
            {
                try { Directory.Delete(tempRoot, true); } catch { }
            }
        }

        /// <summary>
        /// Extracts a BtbN archive and copies the whole "bin" folder (ffmpeg.exe, ffprobe.exe and
        /// the shared libraries) into <paramref name="installDirectory"/>. Synchronous and testable.
        /// </summary>
        public static string InstallFromArchive(string archivePath, string installDirectory)
        {
            if (string.IsNullOrEmpty(archivePath) || !File.Exists(archivePath))
            {
                return "Archive not found: " + archivePath;
            }

            var extractRoot = Path.Combine(Path.GetTempPath(), "aliha_extract_" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(extractRoot);

            try
            {
                System.IO.Compression.ZipFile.ExtractToDirectory(archivePath, extractRoot);

                // The payload lives in <archive-name>/bin next to the main executable.
                var binDirectory = Directory
                    .GetDirectories(extractRoot, "bin", SearchOption.AllDirectories)
                    .FirstOrDefault(d => Directory.GetFiles(d, "ffmpeg.exe").Length > 0);

                var sourceDirectory = binDirectory ?? Path.GetDirectoryName(
                    Directory.GetFiles(extractRoot, "ffmpeg.exe", SearchOption.AllDirectories).FirstOrDefault() ?? string.Empty);

                if (string.IsNullOrEmpty(sourceDirectory) || !Directory.Exists(sourceDirectory))
                {
                    return "The downloaded archive did not contain ffmpeg.exe.";
                }

                Directory.CreateDirectory(installDirectory);

                var tools = new List<string>();
                var libraries = 0;
                foreach (var file in Directory.GetFiles(sourceDirectory))
                {
                    File.Copy(file, Path.Combine(installDirectory, Path.GetFileName(file)), true);

                    if (file.EndsWith(".dll", StringComparison.OrdinalIgnoreCase))
                    {
                        libraries++;
                    }
                    else
                    {
                        tools.Add(Path.GetFileName(file));
                    }
                }

                return "Installed " + string.Join(", ", tools) +
                       (libraries > 0 ? " and " + libraries + " shared libraries" : string.Empty) +
                       " into " + installDirectory;
            }
            finally
            {
                try { Directory.Delete(extractRoot, true); } catch { }
            }
        }
    }
}