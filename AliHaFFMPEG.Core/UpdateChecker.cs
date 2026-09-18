using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace AliHaFFMPEG.Core
{
    public class UpdateInfo
    {
        public Version Version { get; set; }
        public string TagName { get; set; }
        public string ReleaseName { get; set; }
        public string Notes { get; set; }
        public string HtmlUrl { get; set; }
        public string AssetName { get; set; }
        public string AssetUrl { get; set; }
        public long AssetSize { get; set; }
    }

    /// <summary>Checks GitHub releases for a newer version (parsing is unit-tested; the HTTP call is not).</summary>
    public static class UpdateChecker
    {
        /// <summary>Change this to your own repository ("owner/repo") once the project is published.</summary>
        public const string DefaultRepository = "AliHamidi/AliHaFFMPEG";

        public static string BuildApiUrl(string repository)
        {
            return "https://api.github.com/repos/" + repository + "/releases/latest";
        }

        public static string BuildReleasesPageUrl(string repository)
        {
            return "https://github.com/" + repository + "/releases";
        }

        /// <summary>Accepts "v2.1.0", "2.1.0", "v2.1.0-beta.1".</summary>
        public static Version ParseVersionFromTag(string tag)
        {
            if (string.IsNullOrWhiteSpace(tag))
            {
                return null;
            }

            var cleaned = tag.Trim().TrimStart('v', 'V');
            var dash = cleaned.IndexOf('-');
            if (dash > 0)
            {
                cleaned = cleaned.Substring(0, dash);
            }

            return Version.TryParse(cleaned, out var version) ? version : null;
        }

        public static UpdateInfo ParseLatestRelease(string json)
        {
            if (string.IsNullOrEmpty(json))
            {
                return null;
            }

            try
            {
                var root = JObject.Parse(json);
                if (root["message"] != null || root["tag_name"] == null)
                {
                    // GitHub error payload (404 / 403 / rate limited)
                    return null;
                }

                var tagName = (string)root["tag_name"];
                var info = new UpdateInfo
                {
                    TagName = tagName,
                    Version = ParseVersionFromTag(tagName),
                    ReleaseName = (string)root["name"],
                    Notes = (string)root["body"],
                    HtmlUrl = (string)root["html_url"]
                };

                if (root["assets"] is JArray assets)
                {
                    var asset = assets.FirstOrDefault(a => IsPreferredAsset(a, "win-x64", ".zip"))
                                ?? assets.FirstOrDefault(a => IsPreferredAsset(a, null, ".zip"))
                                ?? assets.FirstOrDefault(a => IsPreferredAsset(a, null, ".exe"));

                    if (asset != null)
                    {
                        info.AssetName = (string)asset["name"];
                        info.AssetUrl = (string)asset["browser_download_url"];
                        info.AssetSize = (long?)asset["size"] ?? 0;
                    }
                }

                return info;
            }
            catch
            {
                return null;
            }
        }

        private static bool IsPreferredAsset(JToken asset, string namePart, string extension)
        {
            var name = (string)asset["name"];
            if (string.IsNullOrEmpty(name) || !name.EndsWith(extension, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            return namePart == null || name.IndexOf(namePart, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        /// <summary>Returns the newer release, or null when up to date / unreachable.</summary>
        public static async Task<UpdateInfo> CheckForUpdateAsync(string repository, Version currentVersion)
        {
            if (string.IsNullOrWhiteSpace(repository))
            {
                return null;
            }

            using (var client = CreateClient())
            {
                var json = await client.GetStringAsync(BuildApiUrl(repository)).ConfigureAwait(false);
                var info = ParseLatestRelease(json);
                if (info?.Version == null)
                {
                    return null;
                }

                return info.Version > currentVersion ? info : null;
            }
        }

        private static HttpClient CreateClient()
        {
            var client = new HttpClient { Timeout = TimeSpan.FromSeconds(15) };
            client.DefaultRequestHeaders.UserAgent.ParseAdd("AliHaFFMPEG-Updater");
            client.DefaultRequestHeaders.Accept.ParseAdd("application/vnd.github+json");
            return client;
        }
    }
}