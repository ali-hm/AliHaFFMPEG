using System;
using System.Collections.Generic;
using System.Linq;

namespace AliHaFFMPEG.Core
{
    public class EncoderOption
    {
        public string Name { get; set; }
        public string Label { get; set; }

        public override string ToString()
        {
            return Label;
        }
    }

    /// <summary>Preferred encoders with friendly labels, filtered by what the installed ffmpeg actually supports.</summary>
    public static class EncoderCatalog
    {
        public static readonly EncoderOption[] PreferredVideo =
        {
            new EncoderOption { Name = "libx264", Label = "libx264  (H.264 - best compatibility)" },
            new EncoderOption { Name = "libx265", Label = "libx265  (H.265 / HEVC - smaller)" },
            new EncoderOption { Name = "libvpx-vp9", Label = "libvpx-vp9  (VP9 - WebM)" },
            new EncoderOption { Name = "libaom-av1", Label = "libaom-av1  (AV1 - newest, slow)" },
            new EncoderOption { Name = "libsvtav1", Label = "libsvtav1  (AV1 - fast)" },
            new EncoderOption { Name = "libvpx", Label = "libvpx  (VP8 - WebM)" },
            new EncoderOption { Name = "mpeg4", Label = "mpeg4  (DivX/Xvid players)" },
            new EncoderOption { Name = "mpeg2video", Label = "mpeg2video  (DVD / very old players)" },
            new EncoderOption { Name = "prores", Label = "prores  (Apple ProRes - editing)" },
            new EncoderOption { Name = "ffv1", Label = "ffv1  (lossless archiving, in MKV)" }
        };

        public static readonly EncoderOption[] PreferredAudio =
        {
            new EncoderOption { Name = "aac", Label = "aac  (AAC-LC - universal)" },
            new EncoderOption { Name = "libmp3lame", Label = "libmp3lame  (MP3)" },
            new EncoderOption { Name = "ac3", Label = "ac3  (AC-3 - DVD/old TVs)" },
            new EncoderOption { Name = "eac3", Label = "eac3  (E-AC-3)" },
            new EncoderOption { Name = "opus", Label = "opus  (Opus - smallest)" },
            new EncoderOption { Name = "libvorbis", Label = "libvorbis  (Vorbis - OGG)" },
            new EncoderOption { Name = "flac", Label = "flac  (lossless)" },
            new EncoderOption { Name = "alac", Label = "alac  (Apple lossless)" },
            new EncoderOption { Name = "pcm_s16le", Label = "pcm_s16le  (WAV 16-bit)" },
            new EncoderOption { Name = "pcm_s24le", Label = "pcm_s24le  (WAV 24-bit)" }
        };

        /// <summary>Conservative fallback when ffmpeg is missing, so the UI is still usable.</summary>
        public static readonly string[] FallbackEncoders =
        {
            "libx264", "libx265", "libvpx-vp9", "mpeg4", "mpeg2video",
            "aac", "libmp3lame", "ac3", "opus", "libvorbis", "flac", "alac", "pcm_s16le"
        };

        /// <summary>Parses the output of "ffmpeg -encoders" into the set of encoder names.</summary>
        public static HashSet<string> ParseEncoderList(string output)
        {
            var names = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            if (string.IsNullOrEmpty(output))
            {
                return names;
            }

            foreach (var rawLine in output.Split('\n'))
            {
                var line = rawLine.TrimEnd('\r');
                var tokens = line.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                if (tokens.Length < 2 || tokens[0].Length > 6)
                {
                    continue;
                }

                // the first column is the flag string: V/A/S plus capability letters
                var flags = tokens[0];
                if (flags[0] != 'V' && flags[0] != 'A' && flags[0] != 'S')
                {
                    continue;
                }

                if (flags.All(c => "VASFDXBS.EILT".IndexOf(c) >= 0))
                {
                    var name = tokens[1];
                    if (name == "=" || name.StartsWith("."))
                    {
                        // header row such as " V..... = Video"
                        continue;
                    }

                    names.Add(name);
                }
            }

            return names;
        }

        /// <summary>Video encoders to offer: preferred entries that exist, in the preferred order.</summary>
        public static List<EncoderOption> AvailableVideoEncoders(HashSet<string> available)
        {
            return Filter(PreferredVideo, available);
        }

        public static List<EncoderOption> AvailableAudioEncoders(HashSet<string> available)
        {
            return Filter(PreferredAudio, available);
        }

        private static List<EncoderOption> Filter(EncoderOption[] preferred, HashSet<string> available)
        {
            if (available == null || available.Count == 0)
            {
                return preferred.Where(p => FallbackEncoders.Contains(p.Name, StringComparer.OrdinalIgnoreCase)).ToList();
            }

            return preferred.Where(p => available.Contains(p.Name)).ToList();
        }

        /// <summary>Runs "ffmpeg -encoders" and returns the encoders this build supports.</summary>
        public static HashSet<string> DetectAvailable(string ffmpegPath)
        {
            if (string.IsNullOrEmpty(ffmpegPath))
            {
                return new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            }

            try
            {
                var psi = new System.Diagnostics.ProcessStartInfo
                {
                    FileName = ffmpegPath,
                    Arguments = "-hide_banner -encoders",
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                };

                using (var process = System.Diagnostics.Process.Start(psi))
                {
                    if (process == null)
                    {
                        return new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                    }

                    var readTask = process.StandardOutput.ReadToEndAsync();
                    if (!readTask.Wait(15000))
                    {
                        try { process.Kill(); } catch { }
                        return new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                    }

                    process.WaitForExit(2000);
                    return ParseEncoderList(readTask.Result);
                }
            }
            catch
            {
                return new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            }
        }

        /// <summary>Friendly description of an encoder name (falls back to the raw name).</summary>
        public static string Describe(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return "default encoder for the chosen container";
            }

            if (string.Equals(name, "copy", StringComparison.OrdinalIgnoreCase))
            {
                return "copy - no re-encode, just remux (fastest)";
            }

            var option = PreferredVideo.Concat(PreferredAudio)
                .FirstOrDefault(o => string.Equals(o.Name, name, StringComparison.OrdinalIgnoreCase));
            if (option != null)
            {
                return option.Label;
            }

            if (name.EndsWith("_nvenc", StringComparison.OrdinalIgnoreCase))
            {
                return name + " - NVIDIA hardware encoder (very fast)";
            }

            if (name.EndsWith("_qsv", StringComparison.OrdinalIgnoreCase))
            {
                return name + " - Intel Quick Sync hardware encoder (fast)";
            }

            if (name.EndsWith("_amf", StringComparison.OrdinalIgnoreCase))
            {
                return name + " - AMD hardware encoder (fast)";
            }

            return name;
        }
    }
}