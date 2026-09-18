using System;
using System.Globalization;
using System.IO;

namespace AliHaFFMPEG.Core
{
    /// <summary>Locates external tools (ffmpeg/ffprobe) next to the app or on PATH.</summary>
    public static class FfmpegLocator
    {
        public static string FindTool(string fileName)
        {
            // look next to the application executable first, then in PATH
            var local = Path.Combine(AppContext.BaseDirectory, fileName);
            if (File.Exists(local))
            {
                return local;
            }

            var pathVar = Environment.GetEnvironmentVariable("PATH") ?? string.Empty;
            foreach (var dir in pathVar.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries))
            {
                try
                {
                    var candidate = Path.Combine(dir.Trim(), fileName);
                    if (File.Exists(candidate))
                    {
                        return candidate;
                    }
                }
                catch (ArgumentException)
                {
                    // ignore malformed PATH entries
                }
            }

            return null;
        }
    }
}
