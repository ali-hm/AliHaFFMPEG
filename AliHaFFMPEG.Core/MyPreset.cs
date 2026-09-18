using System;
using System.Collections.Generic;

namespace AliHaFFMPEG.Core
{
    /// <summary>A reusable bundle of conversion settings (saved or built-in).</summary>
    public class MyPreset
    {
        public string PresetName { get; set; }

        public override string ToString()
        {
            return PresetName;
        }

        public int? CRF { get; set; }
        public string VideoCodec { get; set; }
        public string AudioCodec { get; set; }
        public string Profile { get; set; }
        public string Level { get; set; }
        public string Preset { get; set; }
        public string Tune { get; set; }
        public string Format { get; set; }
        public string PixFormat { get; set; }
        public string Scale { get; set; }
        public string Fps { get; set; }
        public string AudioBitrate { get; set; }
        public string Subs { get; set; }
        public string ExtraArgs { get; set; }
        public string QualityMode { get; set; }
        public string VideoBitrate { get; set; }
        public string TargetSizeMb { get; set; }
    }
}
