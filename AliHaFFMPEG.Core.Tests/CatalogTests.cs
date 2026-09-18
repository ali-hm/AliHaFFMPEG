using System;
using System.Linq;
using AliHaFFMPEG.Core;
using Xunit;

namespace AliHaFFMPEG.Core.Tests
{
    public class EncoderCatalogTests
    {
        private const string SampleEncoderOutput = @"
Encoders:
 V..... = Video
 A..... = Audio
 ------
 V..... libx264              libx264 H.264 / AVC (codec h264)
 V..... libx265              libx265 H.265 / HEVC (codec hevc)
 V..... libvpx-vp9           libvpx VP9 (codec vp9)
 V..X.. libaom-av1           libaom AV1 (codec av1)
 V..... mpeg4                MPEG-4 part 2
 A..... aac                  AAC (Advanced Audio Coding)
 A..... libmp3lame           libmp3lame MP3 (MPEG audio layer 3)
 A..... opus                 Opus
 A..... pcm_s16le            PCM signed 16-bit little-endian
";

        [Fact]
        public void ParseEncoderList_FindsVideoAndAudioNames()
        {
            var names = EncoderCatalog.ParseEncoderList(SampleEncoderOutput);
            Assert.Contains("libx264", names);
            Assert.Contains("libx265", names);
            Assert.Contains("libvpx-vp9", names);
            Assert.Contains("libaom-av1", names);
            Assert.Contains("mpeg4", names);
            Assert.Contains("aac", names);
            Assert.Contains("libmp3lame", names);
            Assert.Contains("opus", names);
            Assert.Contains("pcm_s16le", names);
        }

        [Fact]
        public void ParseEncoderList_IgnoresHeaderLines()
        {
            var names = EncoderCatalog.ParseEncoderList(SampleEncoderOutput);
            Assert.DoesNotContain("Encoders:", names);
            Assert.DoesNotContain("=", names);
        }

        [Fact]
        public void AvailableEncoders_OnlyReturnsSupportedOnes()
        {
            var names = EncoderCatalog.ParseEncoderList(SampleEncoderOutput);
            var video = EncoderCatalog.AvailableVideoEncoders(names);
            var audio = EncoderCatalog.AvailableAudioEncoders(names);

            Assert.DoesNotContain(video, v => v.Name == "libsvtav1"); // not in this build
            Assert.DoesNotContain(video, v => v.Name == "prores");
            Assert.Contains(video, v => v.Name == "libx264");
            Assert.Contains(audio, a => a.Name == "libmp3lame");
            Assert.DoesNotContain(audio, a => a.Name == "alac");
        }

        [Fact]
        public void AvailableEncoders_FallBackWhenNothingDetected()
        {
            var video = EncoderCatalog.AvailableVideoEncoders(null);
            Assert.Contains(video, v => v.Name == "libx264");
            Assert.DoesNotContain(video, v => v.Name == "libsvtav1");
        }

        [Theory]
        [InlineData("libx264", "H.264")]
        [InlineData("copy", "no re-encode")]
        [InlineData("h264_nvenc", "NVIDIA")]
        [InlineData("hevc_qsv", "Intel")]
        [InlineData("h264_amf", "AMD")]
        public void Describe_ExplainsKnownEncoders(string name, string expectedFragment)
        {
            Assert.Contains(expectedFragment, EncoderCatalog.Describe(name));
        }
    }

    public class ContainerCatalogTests
    {
        [Theory]
        [InlineData("mp3", true)]
        [InlineData("m4a", true)]
        [InlineData("wav", true)]
        [InlineData("flac", true)]
        [InlineData("ogg", true)]
        [InlineData("opus", true)]
        [InlineData("wma", true)]
        [InlineData("mkv", false)]
        [InlineData("mp4", false)]
        [InlineData("webm", false)]
        [InlineData("gif", false)]
        public void AudioOnly_ContainersAreFlagged(string extension, bool expected)
        {
            Assert.Equal(expected, CommandBuilder.IsAudioOnlyFormat(extension));
        }

        [Fact]
        public void Gif_IsMarkedImageOnly()
        {
            Assert.True(ContainerCatalog.Find("gif").ImageOnly);
        }

        [Theory]
        [InlineData("movie.mkv", true)]
        [InlineData("clip.MP4", true)]
        [InlineData("song.flac", true)]
        [InlineData("notes.txt", false)]
        [InlineData("installer.exe", false)]
        [InlineData("subtitle.srt", false)]
        public void InputExtensions_AcceptKnownMediaOnly(string path, bool expected)
        {
            Assert.Equal(expected, ContainerCatalog.IsInputMediaFile(path));
        }
    }
}