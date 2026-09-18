using System;

namespace AliHaFFMPEG.Core
{
    public enum QualityMode
    {
        Crf,
        Bitrate,
        TargetSizeMb
    }

    /// <summary>UI-independent description of one conversion. Built by the form, consumed by CommandBuilder.</summary>
    public class ConversionSettings
    {
        public string VideoCodec { get; set; }          // null = default, "copy", libx264, h264_nvenc, ...
        public int? Crf { get; set; }
        public QualityMode QualityMode { get; set; } = QualityMode.Crf;
        public string VideoBitrate { get; set; }        // e.g. "3000k" (QualityMode.Bitrate)
        public double? TargetSizeMb { get; set; }       // QualityMode.TargetSizeMb
        public string AudioCodec { get; set; }
        public string AudioBitrate { get; set; }
        public string Profile { get; set; }
        public string Level { get; set; }
        public string EncoderPreset { get; set; }
        public string Tune { get; set; }
        public string PixFormat { get; set; }
        public string ScaleHeight { get; set; }         // "1080" -> -vf scale=-2:1080
        public string Fps { get; set; }
        public string Subtitles { get; set; }           // "copy" | "mov_text" | "drop"
        public string Format { get; set; }              // mkv|mp4|mp3|m4a|wav|gif
        public double? TrimStartSeconds { get; set; }
        public double? TrimEndSeconds { get; set; }
        public string ExtraArgs { get; set; }
    }
}
