using System;
using System.IO;
using System.IO.Compression;
using AliHaFFMPEG.Core;
using Xunit;

namespace AliHaFFMPEG.Core.Tests
{
    public class FfmpegManagerTests
    {
        [Fact]
        public void DefaultRepository_IsAliHm()
        {
            Assert.Equal("ali-hm/AliHaFFMPEG", UpdateChecker.DefaultRepository);
        }

        [Theory]
        [InlineData("ffmpeg-n9.0-latest-win64-gpl-shared-9.0.zip", 9, 0)]
        [InlineData("ffmpeg-n8.1-latest-win64-gpl-shared-8.1.zip", 8, 1)]
        [InlineData("ffmpeg-master-latest-win64-gpl-shared.zip", -1, -1)]
        public void ExtractBuildVersion_ReadsStableVersions(string name, int major, int minor)
        {
            var version = FfmpegManager.ExtractBuildVersion(name);
            if (major < 0)
            {
                Assert.Null(version);
            }
            else
            {
                Assert.Equal(major, version.Major);
                Assert.Equal(minor, version.Minor);
            }
        }

        private const string ReleaseJson = @"{
  ""tag_name"": ""latest"",
  ""assets"": [
    { ""name"": ""checksums.sha256"", ""browser_download_url"": ""https://example.com/sums"", ""size"": 1 },
    { ""name"": ""ffmpeg-master-latest-linux64-gpl-shared.tar.xz"", ""browser_download_url"": ""https://example.com/a"", ""size"": 2 },
    { ""name"": ""ffmpeg-master-latest-win64-gpl-shared.zip"", ""browser_download_url"": ""https://example.com/master"", ""size"": 3 },
    { ""name"": ""ffmpeg-master-latest-win64-gpl.zip"", ""browser_download_url"": ""https://example.com/static"", ""size"": 4 },
    { ""name"": ""ffmpeg-n8.1-latest-win64-gpl-shared-8.1.zip"", ""browser_download_url"": ""https://example.com/n81"", ""size"": 5 },
    { ""name"": ""ffmpeg-n9.0-latest-win64-gpl-shared-9.0.zip"", ""browser_download_url"": ""https://example.com/n90"", ""size"": 6 },
    { ""name"": ""ffmpeg-n9.0-latest-winarm64-gpl-shared-9.0.zip"", ""browser_download_url"": ""https://example.com/arm"", ""size"": 7 }
  ]
}";

        [Fact]
        public void ParseWindowsBuildAsset_PrefersNewestStableSharedZip()
        {
            var asset = FfmpegManager.ParseWindowsBuildAsset(ReleaseJson);
            Assert.NotNull(asset);
            Assert.Equal("ffmpeg-n9.0-latest-win64-gpl-shared-9.0.zip", asset.Name);
            Assert.Equal("https://example.com/n90", asset.Url);
            Assert.Equal(6, asset.Size);
            Assert.Equal(new Version(9, 0), asset.Version);
        }

        [Fact]
        public void ParseWindowsBuildAsset_FallsBackToMasterWhenNoStable()
        {
            var json = @"{ ""assets"": [
                { ""name"": ""ffmpeg-master-latest-win64-gpl-shared.zip"", ""browser_download_url"": ""https://example.com/m"", ""size"": 1 },
                { ""name"": ""ffmpeg-master-latest-win64-gpl.zip"", ""browser_download_url"": ""https://example.com/s"", ""size"": 2 }
            ] }";
            var asset = FfmpegManager.ParseWindowsBuildAsset(json);
            Assert.NotNull(asset);
            Assert.Equal("ffmpeg-master-latest-win64-gpl-shared.zip", asset.Name);
            Assert.Null(asset.Version);
        }

        [Theory]
        [InlineData("ffmpeg-master-latest-win64-gpl-shared.zip", true)]
        [InlineData("ffmpeg-n9.0-latest-win64-gpl-shared-9.0.zip", true)]
        [InlineData("ffmpeg-master-latest-win64-gpl.zip", false)]
        [InlineData("ffmpeg-n9.0-latest-win64-lgpl-shared-9.0.zip", false)]
        [InlineData("ffmpeg-n9.0-latest-winarm64-gpl-shared-9.0.zip", false)]
        [InlineData("ffmpeg-master-latest-linux64-gpl-shared.tar.xz", false)]
        [InlineData("checksums.sha256", false)]
        [InlineData(null, false)]
        public void IsPreferredBuildAsset_OnlyMatchesWin64GplSharedZips(string name, bool expected)
        {
            Assert.Equal(expected, FfmpegManager.IsPreferredBuildAsset(name));
        }

        [Theory]
        [InlineData("")]
        [InlineData("not json")]
        [InlineData(null)]
        [InlineData(@"{ ""assets"": [] }")]
        public void ParseWindowsBuildAsset_ReturnsNullWithoutMatch(string json)
        {
            Assert.Null(FfmpegManager.ParseWindowsBuildAsset(json));
        }

        [Fact]
        public void InstallFromArchive_CopiesToolsAndSharedLibraries()
        {
            var root = Path.Combine(Path.GetTempPath(), "aliha_test_" + Guid.NewGuid().ToString("N"));
            try
            {
                var payload = Path.Combine(root, "payload", "ffmpeg-n9.0-latest-win64-gpl-shared", "bin");
                Directory.CreateDirectory(payload);
                File.WriteAllText(Path.Combine(payload, "ffmpeg.exe"), "fake");
                File.WriteAllText(Path.Combine(payload, "ffprobe.exe"), "fake");
                File.WriteAllText(Path.Combine(payload, "avcodec-62.dll"), "fake");
                File.WriteAllText(Path.Combine(payload, "avutil-60.dll"), "fake");
                File.WriteAllText(Path.Combine(root, "payload", "LICENSE.txt"), "fake");

                var archivePath = Path.Combine(root, "ffmpeg.zip");
                System.IO.Compression.ZipFile.CreateFromDirectory(
                    Path.Combine(root, "payload"), archivePath);

                var target = Path.Combine(root, "install");
                var report = FfmpegManager.InstallFromArchive(archivePath, target);

                Assert.True(File.Exists(Path.Combine(target, "ffmpeg.exe")));
                Assert.True(File.Exists(Path.Combine(target, "ffprobe.exe")));
                Assert.True(File.Exists(Path.Combine(target, "avcodec-62.dll")));
                Assert.True(File.Exists(Path.Combine(target, "avutil-60.dll")));
                Assert.False(File.Exists(Path.Combine(target, "LICENSE.txt")));
                Assert.Contains("ffmpeg.exe", report);
                Assert.Contains("2 shared libraries", report);
            }
            finally
            {
                try { Directory.Delete(root, true); } catch { }
            }
        }

        [Fact]
        public void InstallFromArchive_ReportsMissingArchive()
        {
            Assert.Contains("not found", FfmpegManager.InstallFromArchive(
                Path.Combine(Path.GetTempPath(), "no_such_file.zip"), Path.GetTempPath()));
        }
    }
}
