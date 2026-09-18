using System;
using System.Linq;
using AliHaFFMPEG.Core;
using Xunit;

namespace AliHaFFMPEG.Core.Tests
{
    public class BuiltInPresetsTests
    {
        [Fact]
        public void AllBuiltIns_HaveNamesAssigned()
        {
            Assert.All(BuiltInPresets.All, pair =>
            {
                Assert.False(string.IsNullOrWhiteSpace(pair.Value.PresetName));
                Assert.Equal(pair.Key, pair.Value.PresetName);
            });
        }

        [Theory]
        [InlineData("Old TV / USB player (H.264 High 4.1)")]
        [InlineData("Fit in 25 MB (target size)")]
        [InlineData("Web / YouTube 1080p (H.264 + AAC)")]
        [InlineData("Discord / WhatsApp (small)")]
        [InlineData("Remux (copy, no re-encode)")]
        [InlineData("Extract audio (MP3 320k)")]
        [InlineData("Animated GIF (10 fps, 480px)")]
        public void ExpectedPresets_Exist(string name)
        {
            Assert.True(BuiltInPresets.All.ContainsKey(name), $"Missing preset: {name}");
        }

        [Fact]
        public void EveryPreset_HasAnOutputFormat()
        {
            Assert.All(BuiltInPresets.All, pair => Assert.False(string.IsNullOrWhiteSpace(pair.Value.Format)));
        }

        [Fact]
        public void EveryPreset_ProducesBuildableCommand()
        {
            foreach (var pair in BuiltInPresets.All)
            {
                var preset = pair.Value;
                var settings = new ConversionSettings
                {
                    VideoCodec = preset.VideoCodec,
                    Crf = preset.CRF,
                    AudioCodec = preset.AudioCodec,
                    AudioBitrate = preset.AudioBitrate,
                    Profile = preset.Profile,
                    Level = preset.Level,
                    EncoderPreset = preset.Preset,
                    PixFormat = preset.PixFormat,
                    ScaleHeight = preset.Scale,
                    Format = preset.Format,
                    Subtitles = preset.Subs,
                    ExtraArgs = preset.ExtraArgs,
                    QualityMode = preset.QualityMode == "TargetSizeMb" ? QualityMode.TargetSizeMb : QualityMode.Crf,
                    TargetSizeMb = preset.TargetSizeMb == null ? (double?)null : double.Parse(preset.TargetSizeMb)
                };

                var args = CommandBuilder.Build(settings, "in.mkv", "out." + preset.Format, 600);
                Assert.False(string.IsNullOrWhiteSpace(args), $"Empty command for {pair.Key}");
                Assert.EndsWith($"\"out.{preset.Format}\"", args, StringComparison.Ordinal);
            }
        }

        [Fact]
        public void OldTvPreset_IsMaximallyCompatible()
        {
            var p = BuiltInPresets.All["Old TV / USB player (H.264 High 4.1)"];
            Assert.Equal("libx264", p.VideoCodec);
            Assert.Equal("aac", p.AudioCodec);
            Assert.Equal("high", p.Profile);
            Assert.Equal("4.1", p.Level);
            Assert.Equal("yuv420p", p.PixFormat);
            Assert.Equal("ultrafast", p.Preset);
            Assert.Equal(18, p.CRF);
            Assert.Equal("mp4", p.Format);
            Assert.Equal("drop", p.Subs);
        }

        [Fact]
        public void Describe_ListsTheImportantSettings()
        {
            var summary = BuiltInPresets.Describe(BuiltInPresets.All["Old TV / USB player (H.264 High 4.1)"]);
            Assert.Contains("MP4", summary);
            Assert.Contains("libx264", summary);
            Assert.Contains("CRF 18", summary);
            Assert.Contains("ultrafast", summary);
            Assert.Contains("high 4.1", summary);
            Assert.Contains("1080p", summary);
        }

        [Fact]
        public void Describe_HandlesCopyAndTargetSize()
        {
            Assert.Contains("copy (no re-encode)", BuiltInPresets.Describe(BuiltInPresets.All["Remux (copy, no re-encode)"]));
            Assert.Contains("~25 MB", BuiltInPresets.Describe(BuiltInPresets.All["Fit in 25 MB (target size)"]));
            Assert.Contains("PCM", BuiltInPresets.Describe(BuiltInPresets.All["Extract audio (WAV lossless)"]));
            Assert.Equal(string.Empty, BuiltInPresets.Describe(null));
        }

        [Fact]
        public void PresetCount_IsAtLeast18()
        {
            Assert.True(BuiltInPresets.All.Count >= 18,
                $"Only {BuiltInPresets.All.Count} built-in presets found");
        }
    }
}