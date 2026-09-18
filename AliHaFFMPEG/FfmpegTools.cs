using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using Newtonsoft.Json.Linq;

namespace AliHaFFMPEG
{
    public class MediaInfo
    {
        public int? Width { get; set; }
        public int? Height { get; set; }
        public string VideoCodec { get; set; }
        public string AudioCodec { get; set; }
        public double? DurationSeconds { get; set; }
        public string Bitrate { get; set; }
    }

    public static class FfmpegTools
    {
        public static readonly string[] HardwareEncoderNames =
        {
            "h264_nvenc", "hevc_nvenc", "h264_qsv", "hevc_qsv", "h264_amf", "hevc_amf"
        };

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

        public static double? GetDurationSeconds(string ffprobePath, string inputFile)
        {
            if (string.IsNullOrEmpty(ffprobePath) || string.IsNullOrEmpty(inputFile) || !File.Exists(inputFile))
            {
                return null;
            }

            var output = RunProcessCapture(ffprobePath,
                $"-v error -show_entries format=duration -of default=nw=1:nk=1 \"{inputFile}\"", 5000);
            if (output == null)
            {
                return null;
            }

            if (double.TryParse(output.Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out var seconds) &&
                seconds > 0)
            {
                return seconds;
            }

            return null;
        }

        public static MediaInfo GetMediaInfo(string ffprobePath, string inputFile)
        {
            if (string.IsNullOrEmpty(ffprobePath) || string.IsNullOrEmpty(inputFile) || !File.Exists(inputFile))
            {
                return null;
            }

            var json = RunProcessCapture(ffprobePath,
                "-v error -show_entries format=duration,bit_rate:stream=codec_type,codec_name,width,height,bit_rate -of json \"" +
                inputFile + "\"", 5000);
            if (string.IsNullOrEmpty(json))
            {
                return null;
            }

            try
            {
                var root = JObject.Parse(json);
                var info = new MediaInfo();

                var format = root["format"] as JObject;
                if (format != null)
                {
                    info.DurationSeconds = GetPositiveDouble(format["duration"]);
                    var totalBitrate = GetPositiveLong(format["bit_rate"]);
                    if (totalBitrate.HasValue)
                    {
                        info.Bitrate = FormatBitrate(totalBitrate.Value);
                    }
                }

                if (root["streams"] is JArray streams)
                {
                    foreach (var stream in streams)
                    {
                        var type = (string)stream["codec_type"];
                        if (type == "video" && info.VideoCodec == null)
                        {
                            info.VideoCodec = (string)stream["codec_name"];
                            info.Width = (int?)stream["width"];
                            info.Height = (int?)stream["height"];
                            var vbr = GetPositiveLong(stream["bit_rate"]);
                            if (vbr.HasValue)
                            {
                                info.Bitrate = FormatBitrate(vbr.Value);
                            }
                        }
                        else if (type == "audio" && info.AudioCodec == null)
                        {
                            info.AudioCodec = (string)stream["codec_name"];
                        }
                    }
                }

                return info;
            }
            catch
            {
                return null;
            }
        }

        private static double? GetPositiveDouble(JToken token)
        {
            if (token == null)
            {
                return null;
            }

            return double.TryParse(token.ToString(), NumberStyles.Float, CultureInfo.InvariantCulture, out var v) &&
                   v > 0
                ? v
                : (double?)null;
        }

        private static long? GetPositiveLong(JToken token)
        {
            if (token == null)
            {
                return null;
            }

            return long.TryParse(token.ToString(), NumberStyles.Integer, CultureInfo.InvariantCulture, out var v) &&
                   v > 0
                ? v
                : (long?)null;
        }

        private static string FormatBitrate(long bitsPerSecond)
        {
            return bitsPerSecond >= 1000000
                ? $"{bitsPerSecond / 1000000.0:0.#} Mb/s"
                : $"{bitsPerSecond / 1000.0:0.#} kb/s";
        }

        public static List<string> DetectHardwareEncoders(string ffmpegPath)
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
                foreach (var name in HardwareEncoderNames)
                {
                    if (line.Contains(" " + name + " ") && !found.Contains(name))
                    {
                        found.Add(name);
                    }
                }
            }

            return found;
        }
    }
}
