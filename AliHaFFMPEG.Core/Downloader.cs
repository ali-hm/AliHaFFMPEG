using System;
using System.IO;
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
        /// <summary>Official Windows build with ffmpeg.exe + ffprobe.exe (essentials variant, ~40 MB).</summary>
        public const string DefaultWindowsBuildUrl = "https://www.gyan.dev/ffmpeg/builds/ffmpeg-release-essentials.zip";

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

        /// <summary>
        /// Downloads a zip build, extracts it and copies ffmpeg.exe / ffprobe.exe into the target folder.
        /// Returns a short report of what was installed.
        /// </summary>
        public static async Task<string> DownloadAndInstallAsync(string buildUrl, string installDirectory,
            IProgress<int> progress)
        {
            var tempRoot = Path.Combine(Path.GetTempPath(), "aliha_ffmpeg_" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(tempRoot);

            try
            {
                var zipPath = Path.Combine(tempRoot, "ffmpeg.zip");
                await Downloader.DownloadFileAsync(buildUrl, zipPath, progress).ConfigureAwait(false);

                var extractDir = Path.Combine(tempRoot, "extracted");
                System.IO.Compression.ZipFile.ExtractToDirectory(zipPath, extractDir);

                var installed = new System.Collections.Generic.List<string>();
                foreach (var tool in new[] { "ffmpeg.exe", "ffprobe.exe" })
                {
                    var found = Directory.GetFiles(extractDir, tool, SearchOption.AllDirectories);
                    if (found.Length == 0)
                    {
                        continue;
                    }

                    var target = Path.Combine(installDirectory, tool);
                    System.IO.File.Copy(found[0], target, true);
                    installed.Add(tool);
                }

                return installed.Count > 0
                    ? "Installed: " + string.Join(", ", installed) + " into " + installDirectory
                    : "The downloaded archive did not contain ffmpeg.exe / ffprobe.exe.";
            }
            finally
            {
                try { Directory.Delete(tempRoot, true); } catch { }
            }
        }
    }
}