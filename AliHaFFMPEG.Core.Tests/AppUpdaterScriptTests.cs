using System;
using System.IO;
using AliHaFFMPEG.Core;
using Xunit;

namespace AliHaFFMPEG.Core.Tests
{
    public class AppUpdaterScriptTests
    {
        [Fact]
        public void ZipUpdate_DeletesDownloadedZipOnlyAfterSuccessfulCopy()
        {
            var script = AppUpdater.CreateApplyScript(@"C:\temp\AliHaFFMPEG-win-x64.zip", @"C:\app", "AliHaFFMPEG.exe");
            try
            {
                var content = File.ReadAllText(script);
                var xcopy = content.IndexOf("xcopy /E /Y /I", StringComparison.Ordinal);
                var del = content.IndexOf("if not errorlevel 1 del /F /Q \"C:\\temp\\AliHaFFMPEG-win-x64.zip\"", StringComparison.Ordinal);
                var rmdir = content.IndexOf("rmdir /S /Q", StringComparison.Ordinal);
                var start = content.IndexOf("start \"\" \"C:\\app\\AliHaFFMPEG.exe\"", StringComparison.Ordinal);
                Assert.True(xcopy >= 0, "script must copy the extracted files");
                Assert.True(del > xcopy, "zip must be deleted after the copy, guarded by the copy succeeding");
                Assert.True(rmdir > del, "temp extract dir cleanup must still run");
                Assert.True(start > del, "app must restart after the cleanup");
            }
            finally
            {
                try { File.Delete(script); } catch { }
            }
        }

        [Fact]
        public void ExeUpdate_DeletesDownloadedExeOnlyAfterSuccessfulCopy()
        {
            var script = AppUpdater.CreateApplyScript(@"C:\temp\AliHaFFMPEG.exe", @"C:\app", "AliHaFFMPEG.exe");
            try
            {
                var content = File.ReadAllText(script);
                var copy = content.IndexOf("copy /Y \"C:\\temp\\AliHaFFMPEG.exe\"", StringComparison.Ordinal);
                var del = content.IndexOf("if not errorlevel 1 del /F /Q \"C:\\temp\\AliHaFFMPEG.exe\"", StringComparison.Ordinal);
                Assert.True(copy >= 0, "script must copy the downloaded exe");
                Assert.True(del > copy, "downloaded exe must be deleted after the copy, guarded by the copy succeeding");
            }
            finally
            {
                try { File.Delete(script); } catch { }
            }
        }

        [Fact]
        public void ExeUpdate_NeverDeletesWhenSourceIsTheInstalledExe()
        {
            var script = AppUpdater.CreateApplyScript(@"C:\app\AliHaFFMPEG.exe", @"C:\app", "AliHaFFMPEG.exe");
            try
            {
                var content = File.ReadAllText(script);
                Assert.DoesNotContain("del /F /Q", content);
            }
            finally
            {
                try { File.Delete(script); } catch { }
            }
        }
    }
}