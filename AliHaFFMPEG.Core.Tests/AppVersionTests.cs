
using AliHaFFMPEG.Core;
using Xunit;

namespace AliHaFFMPEG.Core.Tests
{
    public class AppVersionTests
    {
        [Fact]
        public void Title_ContainsNameAndThreePartVersion()
        {
            var title = AppVersion.GetTitle();
            Assert.StartsWith("AliHa FFMPEG v", title);

            // exactly one "v" and three numeric parts after it ("v2.1.0", never "v2.1.0.0")
            Assert.Equal(1, title.Split('v').Length - 1);
            Assert.Matches(@"^AliHa FFMPEG v\d+\.\d+\.\d+$", title);
        }

        [Fact]
        public void Display_MatchesFirstThreePartsOfAssemblyVersion()
        {
            var v = AppVersion.Get();
            Assert.Equal($"{v.Major}.{v.Minor}.{v.Build}", AppVersion.GetDisplay());
        }
    }
}
