using System;
using System.Collections.Generic;
using System.Linq;

namespace AliHaFFMPEG.Core
{
    public class ContainerOption
    {
        public string Extension { get; set; }
        public string Label { get; set; }
        public bool AudioOnly { get; set; }
        public bool ImageOnly { get; set; }

        public override string ToString()
        {
            return Label;
        }
    }

    /// <summary>Output containers (also the file extension) and which inputs count as media files.</summary>
    public static class ContainerCatalog
    {
        public static readonly ContainerOption[] All =
        {
            new ContainerOption { Extension = "mkv", Label = "mkv  (Matroska - accepts anything)" },
            new ContainerOption { Extension = "mp4", Label = "mp4  (MP4 - universal playback)" },
            new ContainerOption { Extension = "mov", Label = "mov  (QuickTime - editing/ProRes)" },
            new ContainerOption { Extension = "webm", Label = "webm  (WebM - VP9/AV1 + Opus)" },
            new ContainerOption { Extension = "avi", Label = "avi  (AVI - legacy)" },
            new ContainerOption { Extension = "ts", Label = "ts  (MPEG-TS - TV/streaming)" },
            new ContainerOption { Extension = "mpg", Label = "mpg  (MPEG program - DVD)" },
            new ContainerOption { Extension = "flv", Label = "flv  (Flash video)" },
            new ContainerOption { Extension = "gif", Label = "gif  (Animated GIF)", ImageOnly = true },
            new ContainerOption { Extension = "mp3", Label = "mp3  (audio only)", AudioOnly = true },
            new ContainerOption { Extension = "m4a", Label = "m4a  (audio only, AAC/ALAC)", AudioOnly = true },
            new ContainerOption { Extension = "aac", Label = "aac  (audio only, ADTS)", AudioOnly = true },
            new ContainerOption { Extension = "wav", Label = "wav  (audio only, PCM)", AudioOnly = true },
            new ContainerOption { Extension = "flac", Label = "flac  (audio only, lossless)", AudioOnly = true },
            new ContainerOption { Extension = "ogg", Label = "ogg  (audio only, Vorbis/Opus)", AudioOnly = true },
            new ContainerOption { Extension = "opus", Label = "opus  (audio only)", AudioOnly = true },
            new ContainerOption { Extension = "wma", Label = "wma  (audio only, Windows)", AudioOnly = true }
        };

        /// <summary>Extensions accepted from drag &amp; drop and the file dialog.</summary>
        public static readonly string[] InputExtensions =
        {
            ".mkv", ".mp4", ".m4v", ".mov", ".avi", ".wmv", ".flv", ".webm", ".ts", ".mts", ".m2ts",
            ".mpg", ".mpeg", ".vob", ".3gp", ".3g2", ".ogv", ".asf", ".rm", ".rmvb", ".divx", ".f4v",
            ".m2v", ".dv", ".y4m", ".mp3", ".wav", ".aac", ".flac", ".ogg", ".oga", ".m4a", ".wma",
            ".opus", ".aiff", ".aif", ".ape", ".wv", ".amr", ".mka", ".ac3", ".dts", ".mp2", ".ra"
        };

        public static bool IsInputMediaFile(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return false;
            }

            var ext = System.IO.Path.GetExtension(path);
            return !string.IsNullOrEmpty(ext) &&
                   InputExtensions.Contains(ext, StringComparer.OrdinalIgnoreCase);
        }

        public static ContainerOption Find(string extension)
        {
            return All.FirstOrDefault(c => string.Equals(c.Extension, extension, StringComparison.OrdinalIgnoreCase));
        }
    }
}