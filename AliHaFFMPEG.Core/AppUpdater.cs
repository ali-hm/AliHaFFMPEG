using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace AliHaFFMPEG.Core
{
    public static class AppVersion
    {
        public static Version Get()
        {
            var assembly = Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly();
            return assembly.GetName().Version ?? new Version(1, 0, 0, 0);
        }

        /// <summary>
        /// User-facing version: the first three parts of the assembly version
        /// ("2.1.0" not "2.1.0.0"). Same value everywhere the app shows itself.
        /// </summary>
        public static string GetDisplay()
        {
            var v = Get();
            return v.Major + "." + v.Minor + "." + v.Build;
        }

        /// <summary>Window and dialog captions, stamped from the same single source.</summary>
        public static string GetTitle()
        {
            return "AliHa FFMPEG v" + GetDisplay();
        }
    }

    /// <summary>
    /// Applies a downloaded update by writing a small script that waits for this process to exit,
    /// replaces the files and restarts the app (works for a portable single-folder install).
    /// </summary>
    public static class AppUpdater
    {
        public static bool CanWriteToInstallDirectory(string installDirectory)
        {
            try
            {
                var probe = Path.Combine(installDirectory, ".aliha_write_test");
                File.WriteAllText(probe, "ok");
                File.Delete(probe);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>Builds the updater script and returns its path.</summary>
        public static string CreateApplyScript(string downloadedFile, string installDirectory, string exeName)
        {
            var script = Path.Combine(Path.GetTempPath(), "aliha_update_" + Guid.NewGuid().ToString("N") + ".bat");
            var exeFullPath = Path.Combine(installDirectory, exeName);
            var lines = new List<string>
            {
                "@echo off",
                "setlocal",
                "echo Updating " + exeName + " ...",
                "timeout /t 3 /nobreak > nul"
            };

            if (downloadedFile.EndsWith(".zip", StringComparison.OrdinalIgnoreCase))
            {
                var extractDir = Path.Combine(Path.GetTempPath(), "aliha_update_" + Guid.NewGuid().ToString("N"));
                lines.Add("powershell -NoProfile -ExecutionPolicy Bypass -Command \"Expand-Archive -LiteralPath '" +
                          downloadedFile + "' -DestinationPath '" + extractDir + "' -Force\"");
                lines.Add("xcopy /E /Y /I \"" + extractDir + "\\*\" \"" + installDirectory + "\\\" > nul");
                lines.Add("rmdir /S /Q \"" + extractDir + "\" > nul");
            }
            else
            {
                lines.Add("copy /Y \"" + downloadedFile + "\" \"" + exeFullPath + "\" > nul");
            }

            lines.Add("start \"\" \"" + exeFullPath + "\"");
            lines.Add("endlocal");

            File.WriteAllLines(script, lines);
            return script;
        }

        public static bool TryStartApplier(string scriptPath, out string error)
        {
            try
            {
                Process.Start(new ProcessStartInfo("cmd.exe", "/c \"" + scriptPath + "\"")
                {
                    CreateNoWindow = true,
                    UseShellExecute = false
                });
                error = null;
                return true;
            }
            catch (Exception ex)
            {
                error = ex.Message;
                return false;
            }
        }

        public static void OpenFolder(string path)
        {
            try
            {
                if (Directory.Exists(path))
                {
                    Process.Start("explorer.exe", "\"" + path + "\"");
                }
            }
            catch
            {
                // cosmetic only
            }
        }
    }
}