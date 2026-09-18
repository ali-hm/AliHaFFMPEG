using AliHaFFMPEG.Core;
using Xunit;

namespace AliHaFFMPEG.Core.Tests
{
    public class MoreCommandBuilderTests
    {
        [Fact]
        public void Amf_UsesConstQp()
        {
            var s = new ConversionSettings { VideoCodec = "h264_amf", Crf = 20 };
            Assert.Contains("-rc cqp -qp_i 20 -qp_p 20 ", CommandBuilder.Build(s, "a", "b.mkv"));
        }

        [Fact]
        public void BitrateMode_UsesBvAndIgnoresCrf()
        {
            var s = new ConversionSettings
            {
                VideoCodec = "libx264",
                QualityMode = QualityMode.Bitrate,
                VideoBitrate = "3000k",
                Crf = 20
            };
            var args = CommandBuilder.Build(s, "in.mp4", "out.mkv");
            Assert.Contains("-b:v 3000k ", args);
            Assert.DoesNotContain("-crf", args);
        }

        [Fact]
        public void TargetSize_ComputesVideoBitrate()
        {
            var s = new ConversionSettings
            {
                VideoCodec = "libx264",
                QualityMode = QualityMode.TargetSizeMb,
                TargetSizeMb = 25,
                AudioBitrate = "128k"
            };
            var args = CommandBuilder.Build(s, "in.mp4", "out.mkv", 600);
            Assert.Contains("-b:v 213k ", args);
        }

        [Fact]
        public void TargetSize_WithoutDuration_OmitsQuality()
        {
            var s = new ConversionSettings
            {
                VideoCodec = "libx264",
                QualityMode = QualityMode.TargetSizeMb,
                TargetSizeMb = 25
            };
            var args = CommandBuilder.Build(s, "in.mp4", "out.mkv", null);
            Assert.DoesNotContain("-b:v", args);
        }

        [Fact]
        public void ComputeTargetBitrate_UsesAudioBitrateAndClampsTo64k()
        {
            var tiny = new ConversionSettings { TargetSizeMb = 1, AudioBitrate = "320k" };
            Assert.Equal(64, CommandBuilder.ComputeTargetBitrate(tiny, 3600));

            var none = new ConversionSettings { TargetSizeMb = 25 };
            Assert.Null(CommandBuilder.ComputeTargetBitrate(none, null));
        }

        [Fact]
        public void Gif_UsesPaletteFilterAndNoAudio()
        {
            var s = new ConversionSettings { Format = "gif" };
            var args = CommandBuilder.Build(s, "in.mp4", "out.gif");
            Assert.Contains("-vf \"fps=10,scale=-2:480:flags=lanczos", args);
            Assert.Contains("palettegen", args);
            Assert.Contains("-an -loop 0 ", args);
            Assert.DoesNotContain("-c:v", args);
        }

        [Fact]
        public void TrimStartAndEnd_UsesFastSeekAndDuration()
        {
            var s = new ConversionSettings { TrimStartSeconds = 90, TrimEndSeconds = 510 };
            var args = CommandBuilder.Build(s, "in.mp4", "out.mkv", 600);
            Assert.StartsWith("-ss 90 -i \"in.mp4\" -t 420 ", args);
        }

        [Fact]
        public void TrimEndOnly_UsesTo()
        {
            var s = new ConversionSettings { TrimEndSeconds = 90 };
            var args = CommandBuilder.Build(s, "in.mp4", "out.mkv");
            Assert.Contains("-i \"in.mp4\" -to 90 ", args);
        }

        [Fact]
        public void SubtitlesDrop_AddsSn()
        {
            var s = new ConversionSettings { Subtitles = "drop" };
            Assert.Contains("-sn ", CommandBuilder.Build(s, "a", "b.mkv"));
        }

        [Fact]
        public void ExtraArgs_AppendedBeforeOutput()
        {
            var s = new ConversionSettings { ExtraArgs = "-af loudnorm" };
            var args = CommandBuilder.Build(s, "in.mp4", "out.mkv");
            Assert.Equal("-i \"in.mp4\" -map 0 -map -0:s? -af loudnorm \"out.mkv\"", args);
        }
    }
}
