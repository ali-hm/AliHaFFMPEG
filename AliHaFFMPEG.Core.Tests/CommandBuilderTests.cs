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
            // "mp3" is normalized to ffmpeg's real encoder name
            Assert.Equal("-i \"in.mp4\" -map 0:a -vn -c:a libmp3lame -b:a 320k \"out.mp3\"", args);
        }

        [Theory]
        [InlineData("mp3", "libmp3lame")]
        [InlineData("MP3", "libmp3lame")]
        [InlineData("libmp3lame", "libmp3lame")]
        [InlineData("aac", "aac")]
        [InlineData("copy", "copy")]
        [InlineData(null, null)]
        public void NormalizeAudioCodec_MapsAliases(string input, string expected)
        {
            Assert.Equal(expected, CommandBuilder.NormalizeAudioCodec(input));
        }

        [Fact]
        public void Vp9_CrfNeedsZeroBitrateAndNoPreset()
        {
            var s = new ConversionSettings { VideoCodec = "libvpx-vp9", Crf = 30, EncoderPreset = "medium", Format = "webm" };
            var args = CommandBuilder.Build(s, "in.mp4", "out.webm");
            Assert.Contains("-crf 30 -b:v 0 ", args);
            Assert.DoesNotContain("-preset", args);
        }

        [Fact]
        public void Av1_CrfNeedsZeroBitrate()
        {
            var s = new ConversionSettings { VideoCodec = "libaom-av1", Crf = 30, Format = "mkv" };
            Assert.Contains("-crf 30 -b:v 0 ", CommandBuilder.Build(s, "in.mp4", "out.mkv"));
        }

        [Theory]
        [InlineData(0, 1)]
        [InlineData(51, 31)]
        public void Mpeg4_MapsCrfToQScale(int crf, int expectedQ)
        {
            var s = new ConversionSettings { VideoCodec = "mpeg4", Crf = crf };
            var args = CommandBuilder.Build(s, "in.mp4", "out.avi");
            Assert.Contains($"-q:v {expectedQ} ", args);
            Assert.DoesNotContain("-crf", args);
        }

        [Fact]
        public void Mpeg2Video_UsesQScaleAndNoPreset()
        {
            var s = new ConversionSettings { VideoCodec = "mpeg2video", Crf = 20, EncoderPreset = "medium", Format = "mpg" };
            var args = CommandBuilder.Build(s, "in.mp4", "out.mpg");
            Assert.Contains("-q:v ", args);
            Assert.DoesNotContain("-preset", args);
        }

        [Fact]
        public void LosslessEncoders_OmitCrf()
        {
            Assert.DoesNotContain("-crf", CommandBuilder.Build(
                new ConversionSettings { VideoCodec = "ffv1", Crf = 18 }, "in.mp4", "out.mkv"));
            Assert.DoesNotContain("-crf", CommandBuilder.Build(
                new ConversionSettings { VideoCodec = "prores", Crf = 18 }, "in.mp4", "out.mov"));
        }

        [Fact]
        public void X265_KeepsPresetAndTune()
        {
            var s = new ConversionSettings { VideoCodec = "libx265", Crf = 24, EncoderPreset = "slow", Tune = "grain" };
            var args = CommandBuilder.Build(s, "in.mp4", "out.mkv");
            Assert.Contains("-preset slow ", args);
            Assert.Contains("-tune grain ", args);
        }

        [Fact]
        public void X265_DoesNotGetH264Profile()
        {
            var s = new ConversionSettings { VideoCodec = "libx265", Profile = "high", Level = "4.1" };
            Assert.DoesNotContain("-profile:v", CommandBuilder.Build(s, "in.mp4", "out.mkv"));
        }

        [Fact]
        public void NewAudioCodecs_AreEmitted()
        {
            foreach (var codec in new[] { "opus", "flac", "alac", "eac3", "pcm_s24le" })
            {
                var args = CommandBuilder.Build(new ConversionSettings { AudioCodec = codec }, "in.mp4", "out.mkv");
                Assert.Contains($"-c:a {codec} ", args);
            }
        }

        [Fact]
        public void NewContainers_AreAcceptedForAudioOnly()
        {
            foreach (var format in new[] { "m4a", "aac", "flac", "ogg", "opus", "wma" })
            {
                var args = CommandBuilder.Build(new ConversionSettings { Format = format }, "in.mkv", "out." + format);
                Assert.Contains("-map 0:a -vn", args);
            }
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
