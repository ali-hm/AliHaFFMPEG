using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace AliHaFFMPEG
{
    /// <summary>
    /// The application icon, embedded in the assembly (LogicalName "AliHaFFMPEG.app.ico")
    /// so every form shows it even in single-file / published builds with no loose .ico file.
    /// </summary>
    internal static class AppIcon
    {
        private static readonly Icon Cached = LoadFromResource();

        public static Icon Get()
        {
            return Cached;
        }

        /// <summary>Applies the icon to a form (no-op when the icon could not be loaded).</summary>
        public static void Apply(Form form)
        {
            if (form == null || Cached == null)
            {
                return;
            }

            form.Icon = Cached;
            form.ShowIcon = true;
        }

        private static Icon LoadFromResource()
        {
            try
            {
                var assembly = Assembly.GetExecutingAssembly();

                using (var stream = assembly.GetManifestResourceStream("AliHaFFMPEG.app.ico"))
                {
                    if (stream != null)
                    {
                        return new Icon(stream);
                    }
                }

                // fallback for layouts that still ship a loose file next to the exe
                var loosePath = Path.Combine(AppContext.BaseDirectory, "app.ico");
                return File.Exists(loosePath) ? new Icon(loosePath) : null;
            }
            catch
            {
                return null; // the icon is cosmetic only
            }
        }
    }
}
