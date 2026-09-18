using System;
using System.Globalization;

namespace AliHaFFMPEG.Core
{
    public struct ProgressLine
    {
        public bool Updated;
        public double? OutputSeconds;
        public double? Speed;
    }

    /// <summary>Parses lines emitted by "ffmpeg -progress pipe:1". Pure and unit-tested.</summary>
    public static class ProgressParser
    {
        public static ProgressLine Parse(string line)
        {
            var result = new ProgressLine();
            if (string.IsNullOrEmpty(line))
            {
                return result;
            }

            var eq = line.IndexOf('=');
            if (eq <= 0)
            {
                return result;
            }

            var key = line.Substring(0, eq).Trim();
            var value = line.Substring(eq + 1).Trim();

            switch (key)
            {
                case "out_time_us":
                case "out_time_ms":
                    // ffmpeg reports microseconds here (out_time_ms is a legacy alias with the same unit)
                    if (double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out var micro) &&
                        micro > 0)
                    {
                        result.OutputSeconds = micro / 1000000.0;
                    }
                    break;

                case "out_time":
                    if (TimeSpan.TryParse(value, CultureInfo.InvariantCulture, out var time) && time.TotalSeconds > 0)
                    {
                        result.OutputSeconds = time.TotalSeconds;
                    }
                    break;

                case "speed":
                    var numeric = value.EndsWith("x") ? value.Substring(0, value.Length - 1) : value;
                    if (double.TryParse(numeric, NumberStyles.Float, CultureInfo.InvariantCulture, out var speed) &&
                        speed > 0)
                    {
                        result.Speed = speed;
                    }
                    break;
            }

            result.Updated = result.OutputSeconds.HasValue || result.Speed.HasValue;
            return result;
        }
    }
}
