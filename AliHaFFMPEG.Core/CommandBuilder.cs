using System;
using System.Linq;
using System.Text;

namespace AliHaFFMPEG.Core
{
    /// <summary>Builds the ffmpeg argument line from ConversionSettings. Pure and unit-tested.</summary>
    public static class CommandBuilder
    {
        public static readonly string[] HardwareEncoderNames =
        {
            "h264_nvenc", "hevc_nvenc", "h264_qsv", "hevc_qsv", "h264_amf", "hevc_amf"
        };

        public static readonly string[] AudioOnlyFormats = { "mp3", "m4a", "wav" };

        public static bool IsAudioOnlyFormat(string format)
        {
            return format != null && AudioOnlyFormats.Contains(format, StringComparer.OrdinalIgnoreCase);
        }

        public static bool IsHardwareEncoder(string codec)
        {
            return codec != null && HardwareEncoderNames.Contains(codec, StringComparer.OrdinalIgnoreCase);
        }

        public static string Build(ConversionSettings s, string inputPath, string outputPath,
            double? totalDurationSeconds = null)
        {
            if (s == null)
            {
                throw new ArgumentNullException(nameof(s));
            }

            var sb = new StringBuilder();
            var videoCodec = s.VideoCodec;
            var audioCodec = s.AudioCodec;
            var format = string.IsNullOrEmpty(s.Format) ? "mkv" : s.Format;
            var isAudioOnly = IsAudioOnlyFormat(format);
            var isGif = string.Equals(format, "gif", StringComparison.OrdinalIgnoreCase);
            var reEncodingVideo = !isAudioOnly && !isGif && !string.IsNullOrEmpty(videoCodec) && videoCodec != "copy";

            // trim: -ss before -i for fast seek; end-trim with a start-trim becomes a duration (-t)
            if (s.TrimStartSeconds.HasValue)
            {
                sb.AppendFormat("-ss {0} ", TimeParser.FormatSeconds(s.TrimStartSeconds.Value));
            }

            if (!string.IsNullOrEmpty(inputPath))
            {
                sb.AppendFormat("-i \"{0}\" ", inputPath);
            }

            if (s.TrimEndSeconds.HasValue)
            {
                if (s.TrimStartSeconds.HasValue && totalDurationSeconds.HasValue)
                {
                    var duration = Math.Max(0.1, s.TrimEndSeconds.Value - s.TrimStartSeconds.Value);
                    sb.AppendFormat("-t {0} ", TimeParser.FormatSeconds(duration));
                }
                else
                {
                    sb.AppendFormat("-to {0} ", TimeParser.FormatSeconds(s.TrimEndSeconds.Value));
                }
            }

            sb.Append(isAudioOnly ? "-map 0:a -vn " : "-map 0 -map -0:s? ");

            if (!isAudioOnly && !isGif)
            {
                if (!string.IsNullOrEmpty(videoCodec))
                {
                    sb.AppendFormat("-c:v {0} ", videoCodec);
                }

                if (reEncodingVideo)
                {
                    AppendQuality(sb, s, totalDurationSeconds);

                    if (!string.IsNullOrEmpty(s.EncoderPreset))
                    {
                        sb.AppendFormat("-preset {0} ", s.EncoderPreset);
                    }

                    if (!string.IsNullOrEmpty(s.Tune))
                    {
                        sb.AppendFormat("-tune {0} ", s.Tune);
                    }

                    if (!string.IsNullOrEmpty(s.PixFormat))
                    {
                        sb.AppendFormat("-pix_fmt {0} ", s.PixFormat);
                    }
                }

                if (videoCodec == "libx264" && !string.IsNullOrEmpty(s.Profile))
                {
                    sb.AppendFormat("-profile:v {0} ", s.Profile);
                    if (!string.IsNullOrEmpty(s.Level))
                    {
                        sb.AppendFormat("-level:v {0} ", s.Level);
                    }
                }

                if (!string.IsNullOrEmpty(s.ScaleHeight))
                {
                    sb.AppendFormat("-vf scale=-2:{0} ", s.ScaleHeight);
                }

                if (!string.IsNullOrEmpty(s.Fps))
                {
                    sb.AppendFormat("-r {0} ", s.Fps);
                }
            }

            if (isGif)
            {
                var scaleHeight = string.IsNullOrEmpty(s.ScaleHeight) ? "480" : s.ScaleHeight;
                sb.Append($"-vf \"fps=10,scale=-2:{scaleHeight}:flags=lanczos,split[a][b];[a]palettegen[p];[b][p]paletteuse\" ");
                sb.Append("-an -loop 0 ");
            }

            if (!string.IsNullOrEmpty(audioCodec))
            {
                sb.AppendFormat("-c:a {0} ", audioCodec);
            }

            if (!string.IsNullOrEmpty(s.AudioBitrate) && !string.IsNullOrEmpty(audioCodec) && audioCodec != "copy")
            {
                sb.AppendFormat("-b:a {0} ", s.AudioBitrate);
            }

            if (!isAudioOnly && !isGif)
            {
                if (s.Subtitles == "drop")
                {
                    sb.Append("-sn ");
                }
                else if (s.Subtitles == "copy")
                {
                    sb.Append("-c:s copy ");
                }
                else if (s.Subtitles == "mov_text")
                {
                    sb.Append("-c:s mov_text ");
                }
            }

            if (!string.IsNullOrEmpty(s.ExtraArgs))
            {
                sb.Append(s.ExtraArgs.Trim()).Append(' ');
            }

            if (!string.IsNullOrEmpty(outputPath))
            {
                sb.AppendFormat("\"{0}\" ", outputPath);
            }

            return sb.ToString().TrimEnd();
        }

        private static void AppendQuality(StringBuilder sb, ConversionSettings s, double? totalDurationSeconds)
        {
            switch (s.QualityMode)
            {
                case QualityMode.Bitrate:
                    if (!string.IsNullOrEmpty(s.VideoBitrate))
                    {
                        sb.AppendFormat("-b:v {0} ", s.VideoBitrate);
                    }
                    break;

                case QualityMode.TargetSizeMb:
                    var videoKbps = ComputeTargetBitrate(s, totalDurationSeconds);
                    if (videoKbps.HasValue)
                    {
                        sb.AppendFormat("-b:v {0}k ", videoKbps.Value);
                    }
                    break;

                default:
                    if (s.Crf.HasValue)
                    {
                        AppendCrf(sb, s.VideoCodec, s.Crf.Value);
                    }
                    break;
            }
        }

        private static void AppendCrf(StringBuilder sb, string videoCodec, int crf)
        {
            switch (videoCodec)
            {
                case "h264_nvenc":
                case "hevc_nvenc":
                    sb.AppendFormat("-rc vbr -cq {0} ", crf);
                    break;
                case "h264_qsv":
                case "hevc_qsv":
                    sb.AppendFormat("-global_quality {0} ", crf);
                    break;
                case "h264_amf":
                case "hevc_amf":
                    sb.AppendFormat("-rc cqp -qp_i {0} -qp_p {0} ", crf);
                    break;
                default:
                    sb.AppendFormat("-crf {0} ", crf);
                    break;
            }
        }

        /// <summary>
        /// Video bitrate (kbps) that fits TargetSizeMb over totalDurationSeconds, minus the audio bitrate.
        /// Returns null when inputs are missing.
        /// </summary>
        public static int? ComputeTargetBitrate(ConversionSettings s, double? totalDurationSeconds)
        {
            if (!s.TargetSizeMb.HasValue || s.TargetSizeMb.Value <= 0 ||
                !totalDurationSeconds.HasValue || totalDurationSeconds.Value <= 0)
            {
                return null;
            }

            var totalKbps = s.TargetSizeMb.Value * 8192.0 / totalDurationSeconds.Value;

            var audioKbps = 128.0;
            if (!string.IsNullOrEmpty(s.AudioBitrate))
            {
                var digits = new string(s.AudioBitrate.TakeWhile(char.IsDigit).ToArray());
                if (double.TryParse(digits, out var parsed) && parsed > 0)
                {
                    audioKbps = parsed;
                }
            }

            return (int)Math.Max(64, Math.Round(totalKbps - audioKbps));
        }
    }
}
