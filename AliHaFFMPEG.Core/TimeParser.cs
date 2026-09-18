using System;
using System.Globalization;

namespace AliHaFFMPEG.Core
{
    /// <summary>Parses user-entered time values and formats them for ffmpeg.</summary>
    public static class TimeParser
    {
        /// <summary>
        /// Accepts "90", "1:30", "01:02:03", "01:02:03.5". Returns seconds, or null when
        /// empty, malformed, or not positive.
        /// </summary>
        public static double? ParseTimeString(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return null;
            }

            text = text.Trim();
            double value;

            if (text.Contains(":"))
            {
                double seconds = 0;
                foreach (var part in text.Split(':'))
                {
                    if (!double.TryParse(part, NumberStyles.Float, CultureInfo.InvariantCulture, out value))
                    {
                        return null;
                    }

                    seconds = seconds * 60 + value;
                }

                value = seconds;
            }
            else if (!double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out value))
            {
                return null;
            }

            return value > 0 ? value : (double?)null;
        }

        public static string FormatSeconds(double seconds)
        {
            return seconds.ToString("0.###", CultureInfo.InvariantCulture);
        }
    }
}
