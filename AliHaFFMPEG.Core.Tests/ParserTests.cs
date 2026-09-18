using AliHaFFMPEG.Core;
using Xunit;

namespace AliHaFFMPEG.Core.Tests
{
    public class ProgressParserTests
    {
        [Fact]
        public void OutTimeUs_IsMicroseconds()
        {
            var p = ProgressParser.Parse("out_time_us=123456789");
            Assert.True(p.Updated);
            Assert.Equal(123.456789, p.OutputSeconds.Value, 6);
            Assert.False(p.Speed.HasValue);
        }

        [Fact]
        public void LegacyOutTimeMs_AlsoMicroseconds()
        {
            var p = ProgressParser.Parse("out_time_ms=1000000");
            Assert.True(p.Updated);
            Assert.Equal(1.0, p.OutputSeconds.Value, 6);
        }

        [Fact]
        public void OutTime_ParsesTimeSpan()
        {
            var p = ProgressParser.Parse("out_time=00:00:03.484000");
            Assert.True(p.Updated);
            Assert.Equal(3.484, p.OutputSeconds.Value, 3);
        }

        [Fact]
        public void Speed_ParsesMultiplier()
        {
            var p = ProgressParser.Parse("speed=2.4x");
            Assert.True(p.Updated);
            Assert.Equal(2.4, p.Speed.Value, 6);
            Assert.False(p.OutputSeconds.HasValue);
        }

        [Theory]
        [InlineData("frame=123")]
        [InlineData("progress=end")]
        [InlineData("progress=continue")]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("garbage line without equals")]
        public void UnrelatedLines_AreIgnored(string line)
        {
            Assert.False(ProgressParser.Parse(line).Updated);
        }
    }

    public class TimeParserTests
    {
        [Theory]
        [InlineData("90", 90)]
        [InlineData("1:30", 90)]
        [InlineData("01:02:03", 3723)]
        [InlineData("01:02:03.5", 3723.5)]
        [InlineData("0.5", 0.5)]
        public void ValidInputs_AreParsed(string input, double expected)
        {
            Assert.Equal(expected, TimeParser.ParseTimeString(input).Value, 6);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("abc")]
        [InlineData("1:xx")]
        [InlineData("-5")]
        public void InvalidInputs_ReturnNull(string input)
        {
            Assert.Null(TimeParser.ParseTimeString(input));
        }

        [Fact]
        public void FormatSeconds_UsesInvariantCulture()
        {
            Assert.Equal("1.5", TimeParser.FormatSeconds(1.5));
            Assert.Equal("90", TimeParser.FormatSeconds(90.0));
        }
    }

    public class MediaInfoReaderTests
    {
        private const string SampleJson = @"{
  ""streams"": [
    { ""codec_type"": ""video"", ""codec_name"": ""h264"", ""width"": 1920, ""height"": 1080, ""bit_rate"": ""4194304"" },
    { ""codec_type"": ""audio"", ""codec_name"": ""aac"", ""bit_rate"": ""128000"" }
  ],
  ""format"": { ""duration"": ""123.456000"", ""bit_rate"": ""4469504"" }
}";

        [Fact]
        public void Json_IsParsedIntoMediaInfo()
        {
            var info = MediaInfoReader.ParseMediaInfoJson(SampleJson);
            Assert.NotNull(info);
            Assert.Equal("h264", info.VideoCodec);
            Assert.Equal("aac", info.AudioCodec);
            Assert.Equal(1920, info.Width.Value);
            Assert.Equal(1080, info.Height.Value);
            Assert.Equal(123.456, info.DurationSeconds.Value, 3);
            Assert.Equal("4.2 Mb/s", info.Bitrate);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("not json")]
        public void InvalidJson_ReturnsNull(string json)
        {
            Assert.Null(MediaInfoReader.ParseMediaInfoJson(json));
        }

        [Theory]
        [InlineData("123.45\n", 123.45)]
        [InlineData("N/A", null)]
        [InlineData(null, null)]
        public void DurationOutput_IsParsed(string output, double? expected)
        {
            Assert.Equal(expected, MediaInfoReader.ParseDurationOutput(output));
        }
    }
}
