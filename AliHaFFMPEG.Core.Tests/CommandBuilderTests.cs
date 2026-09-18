using System;
using AliHaFFMPEG.Core;
using Xunit;

namespace AliHaFFMPEG.Core.Tests
{
    public class CommandBuilderTests
    {
        [Fact]
        public void Defaults_ProduceMinimalMkvCommand()
        {
            var args = CommandBuilder.Build(new ConversionSettings(), "in.mp4", "out.mkv");
            Assert.Equal("-i \"in.mp4\" -map 0 -map -0:s? \"out.mkv\"", args);
        }

        [Fact]
        public void AudioOnly_Mp3_MapsAudioAndVn()
        {
            var s = new ConversionSettings { Format = "mp3", AudioCodec = "mp3", AudioBitrate = "320k" };
            var args = CommandBuilder.Build(s, "in.mp4", "out.mp3");
            Assert.Equal("-i \"in.mp4\" -map 0:a -vn -c:a mp3 -b:a 320k \"out.mp3\"", args);
        }

        [Fact]
        public void FullX264_PresetContainsAllFlags()
        {
            var s = new ConversionSettings
            {
                VideoCodec = "libx264",
                Crf = 20,
                EncoderPreset = "medium",
                PixFormat = "yuv420p",
                Profile = "high",
                Level = "4.1",
                ScaleHeight = "1080",
                Fps = "30",
                AudioCodec = "aac",
                AudioBitrate = "192k",
                Subtitles = "mov_text",
                Format = "mp4"
            };
            var args = CommandBuilder.Build(s, "in.mp4", "out.mp4");
            Assert.Equal(
                "-i \"in.mp4\" -map 0 -map -0:s? -c:v libx264 -crf 20 -preset medium -pix_fmt yuv420p " +
                "-profile:v high -level:v 4.1 -vf scale=-2:1080 -r 30 -c:a aac -b:a 192k -c:s mov_text \"out.mp4\"",
                args);
        }

        [Fact]
        public void CopyCodec_SkipsQualityFlags()
        {
            var s = new ConversionSettings
            {
                VideoCodec = "copy",
                AudioCodec = "copy",
                Subtitles = "copy",
                Format = "mkv",
                Crf = 20,
                EncoderPreset = "slow"
            };
            var args = CommandBuilder.Build(s, "in.mp4", "out.mkv");
            Assert.Equal("-i \"in.mp4\" -map 0 -map -0:s? -c:v copy -c:a copy -c:s copy \"out.mkv\"", args);
        }

        [Fact]
        public void Nvenc_UsesCqInsteadOfCrf()
        {
            var s = new ConversionSettings { VideoCodec = "h264_nvenc", Crf = 20 };
            var args = CommandBuilder.Build(s, "in.mp4", "out.mkv");
            Assert.Contains("-rc vbr -cq 20 ", args);
            Assert.DoesNotContain("-crf", args);
        }

        [Fact]
        public void Qsv_UsesGlobalQuality()
        {
            var s = new ConversionSettings { VideoCodec = "h264_qsv", Crf = 20 };
            Assert.Contains("-global_quality 20 ", CommandBuilder.Build(s, "a", "b.mkv"));
        }
    }
}
