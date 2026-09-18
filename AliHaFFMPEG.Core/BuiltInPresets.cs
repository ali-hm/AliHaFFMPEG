using System;
using System.Collections.Generic;

namespace AliHaFFMPEG.Core
{
    /// <summary>The one-click presets offered in the UI. Unit-tested, so the list is verifiable.</summary>
    public static class BuiltInPresets
    {
        public static readonly Dictionary<string, MyPreset> All = new Dictionary<string, MyPreset>
        {
            ["Web / YouTube 1080p (H.264 + AAC)"] = new MyPreset
            {
                VideoCodec = "libx264",
                CRF = 20,
                Preset = "medium",
                PixFormat = "yuv420p",
                AudioCodec = "aac",
                AudioBitrate = "192k",
                Scale = "1080",
                Format = "mp4",
                Subs = "mov_text"
            },
            ["Half the size (H.265 / x265)"] = new MyPreset
            {
                VideoCodec = "libx265",
                CRF = 28,
                Preset = "medium",
                AudioCodec = "aac",
                AudioBitrate = "128k",
                Format = "mkv"
            },
            ["Fit in 25 MB (target size)"] = new MyPreset
            {
                VideoCodec = "libx264",
                QualityMode = "TargetSizeMb",
                TargetSizeMb = "25",
                Preset = "medium",
                AudioCodec = "aac",
                AudioBitrate = "128k",
                Format = "mkv"
            },
            ["Old TV / USB player (H.264 High 4.1)"] = new MyPreset
            {
                VideoCodec = "libx264",
                CRF = 18,
                Preset = "ultrafast",
                PixFormat = "yuv420p",
                Profile = "high",
                Level = "4.1",
                Scale = "1080",
                AudioCodec = "aac",
                AudioBitrate = "192k",
                Subs = "drop",
                Format = "mp4"
            },
            ["Discord / WhatsApp (small)"] = new MyPreset
            {
                VideoCodec = "libx264",
                CRF = 28,
                Preset = "veryfast",
                PixFormat = "yuv420p",
                AudioCodec = "aac",
                AudioBitrate = "96k",
                Scale = "720",
                Format = "mp4",
                Subs = "drop"
            },
            ["Remux (copy, no re-encode)"] = new MyPreset
            {
                VideoCodec = "copy",
                AudioCodec = "copy",
                Subs = "copy",
                Format = "mkv"
            },
            ["Extract audio (MP3 320k)"] = new MyPreset
            {
                AudioCodec = "mp3",
                AudioBitrate = "320k",
                Format = "mp3"
            },
            ["Extract audio (M4A / AAC 256k)"] = new MyPreset
            {
                AudioCodec = "aac",
                AudioBitrate = "256k",
                Format = "m4a"
            },
            ["Extract audio (WAV lossless)"] = new MyPreset
            {
                Format = "wav"
            },
            ["Web 720p (H.264 fast)"] = new MyPreset
            {
                VideoCodec = "libx264",
                CRF = 23,
                Preset = "veryfast",
                PixFormat = "yuv420p",
                AudioCodec = "aac",
                AudioBitrate = "128k",
                Scale = "720",
                Format = "mp4",
                Subs = "mov_text"
            },
            ["Tiny file for email (480p)"] = new MyPreset
            {
                VideoCodec = "libx264",
                CRF = 30,
                Preset = "superfast",
                PixFormat = "yuv420p",
                AudioCodec = "aac",
                AudioBitrate = "96k",
                Scale = "480",
                Format = "mp4",
                Subs = "drop"
            },
            ["Slow PC / fast convert (x264 ultrafast)"] = new MyPreset
            {
                VideoCodec = "libx264",
                CRF = 23,
                Preset = "ultrafast",
                PixFormat = "yuv420p",
                AudioCodec = "aac",
                AudioBitrate = "128k",
                Format = "mp4"
            },
            ["Best quality archive (x265 slower)"] = new MyPreset
            {
                VideoCodec = "libx265",
                CRF = 24,
                Preset = "slower",
                AudioCodec = "aac",
                AudioBitrate = "192k",
                Format = "mkv"
            },
            ["10-bit HEVC (anime / gradients)"] = new MyPreset
            {
                VideoCodec = "libx265",
                CRF = 24,
                Preset = "slow",
                PixFormat = "yuv420p10le",
                AudioCodec = "copy",
                Format = "mkv"
            },
            ["Vertical video (TikTok / Reels 720x1280)"] = new MyPreset
            {
                VideoCodec = "libx264",
                CRF = 23,
                Preset = "medium",
                PixFormat = "yuv420p",
                AudioCodec = "aac",
                AudioBitrate = "128k",
                Format = "mp4",
                ExtraArgs = "-vf scale=720:-2"
            },
            ["Loudness fix (EBU R128)"] = new MyPreset
            {
                VideoCodec = "copy",
                AudioCodec = "aac",
                AudioBitrate = "192k",
                Format = "mkv",
                ExtraArgs = "-af loudnorm"
            },
            ["Deinterlace (old camcorder / TV)"] = new MyPreset
            {
                VideoCodec = "libx264",
                CRF = 20,
                Preset = "veryfast",
                PixFormat = "yuv420p",
                AudioCodec = "copy",
                Format = "mp4",
                ExtraArgs = "-vf yadif"
            },
            ["Animated GIF (10 fps, 480px)"] = new MyPreset
            {
                Format = "gif"
            }
        };
        static BuiltInPresets()
        {
            // every built-in carries its own name so it can be looked up and shown
            foreach (var pair in All)
            {
                if (string.IsNullOrEmpty(pair.Value.PresetName))
                {
                    pair.Value.PresetName = pair.Key;
                }
            }
        }

        /// <summary>One-line human-readable summary of what a preset does.</summary>
        public static string Describe(MyPreset p)
        {
            if (p == null)
            {
                return string.Empty;
            }

            var parts = new List<string> { (string.IsNullOrEmpty(p.Format) ? "mkv" : p.Format).ToUpperInvariant() };

            if (IsCopy(p.VideoCodec))
            {
                parts.Add("video: copy (no re-encode)");
            }
            else if (!string.IsNullOrEmpty(p.VideoCodec))
            {
                var video = "video: " + p.VideoCodec;
                if (string.Equals(p.QualityMode, "TargetSizeMb", StringComparison.OrdinalIgnoreCase) &&
                    !string.IsNullOrEmpty(p.TargetSizeMb))
                {
                    video += " ~" + p.TargetSizeMb + " MB";
                }
                else if (string.Equals(p.QualityMode, "Bitrate", StringComparison.OrdinalIgnoreCase) &&
                         !string.IsNullOrEmpty(p.VideoBitrate))
                {
                    video += " " + p.VideoBitrate;
                }
                else if (p.CRF.HasValue)
                {
                    video += " CRF " + p.CRF.Value;
                }

                if (!string.IsNullOrEmpty(p.Preset))
                {
                    video += " (" + p.Preset + ")";
                }

                parts.Add(video);
            }

            if (IsCopy(p.AudioCodec))
            {
                parts.Add("audio: copy");
            }
            else if (!string.IsNullOrEmpty(p.AudioCodec))
            {
                parts.Add("audio: " + p.AudioCodec +
                          (string.IsNullOrEmpty(p.AudioBitrate) ? string.Empty : " " + p.AudioBitrate));
            }
            else if (string.Equals(p.Format, "wav", StringComparison.OrdinalIgnoreCase))
            {
                parts.Add("audio: PCM (default)");
            }

            if (!string.IsNullOrEmpty(p.Profile) || !string.IsNullOrEmpty(p.Level))
            {
                parts.Add(("profile " + p.Profile + " " + p.Level).Trim());
            }

            if (!string.IsNullOrEmpty(p.Scale))
            {
                parts.Add(p.Scale + "p");
            }

            if (!string.IsNullOrEmpty(p.Fps))
            {
                parts.Add(p.Fps + " fps");
            }

            if (!string.IsNullOrEmpty(p.Subs))
            {
                parts.Add("subs: " + p.Subs);
            }

            if (!string.IsNullOrEmpty(p.ExtraArgs))
            {
                parts.Add(p.ExtraArgs);
            }

            return string.Join("  |  ", parts);
        }

        private static bool IsCopy(string value)
        {
            return string.Equals(value, "copy", StringComparison.OrdinalIgnoreCase);
        }
    }
}
