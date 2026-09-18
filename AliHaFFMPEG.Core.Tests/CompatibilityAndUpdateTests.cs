using System;
using AliHaFFMPEG.Core;
using Xunit;

namespace AliHaFFMPEG.Core.Tests
{
    public class CompatibilityCheckerTests
    {
        [Fact]
        public void Mkv_AcceptsAnything()
        {
            var s = new ConversionSettings { VideoCodec = "prores", AudioCodec = "opus", Format = "mkv" };
            Assert.Empty(CompatibilityChecker.Validate(s, "mkv"));
        }

        [Fact]
        public void Webm_FlagsNonVpxVideoAndNonOpusAudio()
        {
            var s = new ConversionSettings { VideoCodec = "libx264", AudioCodec = "aac", Format = "webm" };
            var warnings = CompatibilityChecker.Validate(s, "webm");
            Assert.Equal(2, warnings.Count);
            Assert.Contains(warnings, w => w.Contains("VP8/VP9/AV1"));
            Assert.Contains(warnings, w => w.Contains("Opus or Vorbis"));
        }

        [Fact]
        public void Webm_IsHappyWithVp9AndOpus()
        {
            var s = new ConversionSettings { VideoCodec = "libvpx-vp9", AudioCodec = "opus", Format = "webm" };
            Assert.Empty(CompatibilityChecker.Validate(s, "webm"));
        }

        [Fact]
        public void AudioContainer_FlagsVideoEncoding()
        {
            var s = new ConversionSettings { VideoCodec = "libx264", Format = "mp3" };
            var warnings = CompatibilityChecker.Validate(s, "mp3");
            Assert.Contains(warnings, w => w.Contains("audio-only container"));
        }

        [Fact]
        public void Mp3Container_FlagsNonMp3Audio()
        {
            var s = new ConversionSettings { AudioCodec = "aac", Format = "mp3" };
            Assert.Contains(CompatibilityChecker.Validate(s, "mp3"), w => w.Contains("libmp3lame"));
        }

        [Fact]
        public void Wav_FlagsLossyAudio()
        {
            var s = new ConversionSettings { AudioCodec = "aac", Format = "wav" };
            Assert.Contains(CompatibilityChecker.Validate(s, "wav"), w => w.Contains("PCM"));
        }

        [Fact]
        public void Mp4_FlagsCopiedSubtitles()
        {
            var s = new ConversionSettings { VideoCodec = "libx264", AudioCodec = "aac", Subtitles = "copy", Format = "mp4" };
            Assert.Contains(CompatibilityChecker.Validate(s, "mp4"), w => w.Contains("mov_text"));
        }

        [Fact]
        public void CopyCodecs_AreNeverFlagged()
        {
            var s = new ConversionSettings { VideoCodec = "copy", AudioCodec = "copy", Format = "mp4" };
            Assert.Empty(CompatibilityChecker.Validate(s, "mp4"));
        }

        [Fact]
        public void ProresAndFfv1_PreferTheirContainers()
        {
            Assert.Contains(CompatibilityChecker.Validate(new ConversionSettings { VideoCodec = "prores" }, "avi"),
                w => w.Contains("ProRes"));
            Assert.Contains(CompatibilityChecker.Validate(new ConversionSettings { VideoCodec = "ffv1" }, "mp4"),
                w => w.Contains("FFV1"));
        }
    }

    public class UpdateCheckerTests
    {
        private const string ReleaseJson = @"{
  ""tag_name"": ""v2.2.0"",
  ""name"": ""AliHaFFMPEG 2.2.0"",
  ""body"": ""- faster conversions"",
  ""html_url"": ""https://github.com/AliHamidi/AliHaFFMPEG/releases/tag/v2.2.0"",
  ""assets"": [
    { ""name"": ""notes.txt"", ""browser_download_url"": ""https://example.com/notes.txt"", ""size"": 10 },
    { ""name"": ""AliHaFFMPEG-win-x64.zip"", ""browser_download_url"": ""https://example.com/app.zip"", ""size"": 5242880 }
  ]
}";

        [Fact]
        public void ParseLatestRelease_ReadsVersionNotesAndAsset()
        {
            var info = UpdateChecker.ParseLatestRelease(ReleaseJson);
            Assert.NotNull(info);
            Assert.Equal("v2.2.0", info.TagName);
            Assert.Equal(new Version(2, 2, 0), info.Version);
            Assert.Contains("faster", info.Notes);
            Assert.Equal("AliHaFFMPEG-win-x64.zip", info.AssetName);
            Assert.Equal("https://example.com/app.zip", info.AssetUrl);
            Assert.Equal(5242880, info.AssetSize);
        }

        [Theory]
        [InlineData("v2.1.0", 2, 1, 0)]
        [InlineData("2.1.0", 2, 1, 0)]
        [InlineData("V3.0", 3, 0, -1)]
        [InlineData("v2.1.0-beta.1", 2, 1, 0)]
        public void ParseVersionFromTag_HandlesCommonTags(string tag, int major, int minor, int build)
        {
            var version = UpdateChecker.ParseVersionFromTag(tag);
            Assert.Equal(major, version.Major);
            Assert.Equal(minor, version.Minor);
            if (build >= 0)
            {
                Assert.Equal(build, version.Build);
            }
        }

        [Theory]
        [InlineData("")]
        [InlineData("latest")]
        [InlineData(null)]
        public void ParseVersionFromTag_ReturnsNullForGarbage(string tag)
        {
            Assert.Null(UpdateChecker.ParseVersionFromTag(tag));
        }

        [Fact]
        public void ParseLatestRelease_ReturnsNullOnApiError()
        {
            Assert.Null(UpdateChecker.ParseLatestRelease("{\"message\":\"Not Found\"}"));
            Assert.Null(UpdateChecker.ParseLatestRelease("not json"));
            Assert.Null(UpdateChecker.ParseLatestRelease(null));
        }

        [Fact]
        public void BuildApiUrl_UsesRepository()
        {
            Assert.Equal("https://api.github.com/repos/AliHamidi/AliHaFFMPEG/releases/latest",
                UpdateChecker.BuildApiUrl("AliHamidi/AliHaFFMPEG"));
        }
    }
}