using System.Collections.Generic;
using System.Diagnostics;

namespace AliHaFFMPEG.Core
{
    /// <summary>Detects hardware encoders (NVENC / QSV / AMF) supported by the installed ffmpeg.</summary>
    public static class HardwareEncoderDetector
    {
        public static List<string> Detect(string ffmpegPath)
        {
            var found = new List<string>();
            if (string.IsNullOrEmpty(ffmpegPath))
            {
                return found;
            }

            var output = RunProcessCapture(ffmpegPath, "-hide_banner -encoders", 10000);
            if (string.IsNullOrEmpty(output))
            {
                return found;
            }

            foreach (var line in output.Split('\n'))
            {
                foreach (var name in CommandBuilder.HardwareEncoderNames)
                {
                    if (line.Contains(" " + name + " ") && !found.Contains(name))
                    {
                        found.Add(name);
                    }
                }
            }

            return found;
        }

        private static string RunProcessCapture(string fileName, string arguments, int timeoutMs)
        {
            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName = fileName,
                    Arguments = arguments,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                };
                using (var p = Process.Start(psi))
                {
                    if (p == null)
                    {
                        return null;
                    }

                    var readTask = p.StandardOutput.ReadToEndAsync();
                    if (!readTask.Wait(timeoutMs))
                    {
                        try { p.Kill(); } catch { }
                        return null;
                    }

                    p.WaitForExit(2000);
                    return readTask.Result;
                }
            }
            catch
            {
                return null;
            }
        }
    }
}
