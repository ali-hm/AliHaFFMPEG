using System;
using System.Collections.Generic;
using System.Linq;

namespace AliHaFFMPEG.Core
{
    /// <summary>
    /// Warns about codec/container combinations that ffmpeg will usually reject
    /// (e.g. Opus inside MP4, ProRes inside WebM). Warnings only - never blocks a conversion.
    /// </summary>
    public static class CompatibilityChecker
    {
        private static readonly string[] AacLike = { "aac", "libfdk_aac", "libfaac" };
        private static readonly string[] Mp3Like = { "libmp3lame", "mp3" };
        private static readonly string[] PcmLike = { "pcm_s16le", "pcm_s24le", "pcm_u8", "pcm_s32le", "pcm_f32le" };

        public static IList<string> Validate(ConversionSettings settings, string format)
        {
            var warnings = new List<string>();
            if (settings == null)
            {
                return warnings;
            }

            var container = string.IsNullOrEmpty(format) ? settings.Format : format;
            container = (container ?? "mkv").ToLowerInvariant();
            var video = settings.VideoCodec;
            var audio = CommandBuilder.NormalizeAudioCodec(settings.AudioCodec);
            var isCopyVideo = string.Equals(video, "copy", StringComparison.OrdinalIgnoreCase);
            var isCopyAudio = string.Equals(audio, "copy", StringComparison.OrdinalIgnoreCase);
            var hasVideo = !string.IsNullOrEmpty(video) && !isCopyVideo;
            var hasAudio = !string.IsNullOrEmpty(audio) && !isCopyAudio;

            var info = ContainerCatalog.Find(container);
            if (info != null && info.AudioOnly && hasVideo)
            {
                warnings.Add($"'{container}' is an audio-only container, but video encoding was selected - the command will fail. Use mkv/mp4, or set Video Codec to 'copy'/empty.");
            }

            if (info != null && info.ImageOnly && hasAudio)
            {
                warnings.Add("GIF cannot contain audio - the audio codec setting is ignored.");
            }

            if (container == "webm")
            {
                if (hasVideo && !In(video, "libvpx", "libvpx-vp9", "libaom-av1", "libsvtav1"))
                {
                    warnings.Add("WebM only supports VP8/VP9/AV1 video - use libvpx-vp9 or libvpx, otherwise encoding will fail.");
                }

                if (hasAudio && !In(audio, "opus", "libvorbis", "vorbis"))
                {
                    warnings.Add("WebM only supports Opus or Vorbis audio - use opus or libvorbis.");
                }
            }

            if (container == "mp4" || container == "mov")
            {
                if (hasVideo && In(video, "libvpx", "libvpx-vp9", "ffv1", "prores"))
                {
                    warnings.Add($"'{video}' is unusual in {container} and may not play - mkv or webm is a better fit.");
                }

                if (hasAudio && !isCopyAudio && !In(audio, AacLike.Concat(new[] { "ac3", "eac3", "alac" }).Concat(Mp3Like).Concat(PcmLike).ToArray()))
                {
                    warnings.Add($"'{audio}' in {container} is often unsupported - use aac, ac3, alac or mp3.");
                }

                if (string.Equals(settings.Subtitles, "copy", StringComparison.OrdinalIgnoreCase))
                {
                    warnings.Add("MP4/MOV cannot hold many subtitle formats - 'mov_text' is the safe choice for MP4.");
                }
            }

            if (container == "avi")
            {
                if (hasAudio && In(audio, "opus", "libvorbis", "aac", "flac", "alac"))
                {
                    warnings.Add("AVI works best with mp3, ac3 or pcm audio.");
                }
            }

            if (container == "mpg" && hasVideo && !In(video, "mpeg2video", "mpeg1video"))
            {
                warnings.Add("MPEG program stream expects mpeg2video/mpeg1video - use mpg with mpeg2video for old DVD players.");
            }

            if (container == "mp3" && hasAudio && !In(audio, Mp3Like))
            {
                warnings.Add("The mp3 container only accepts MP3 audio - use libmp3lame.");
            }

            if (container == "m4a" && hasAudio && !In(audio, AacLike.Concat(new[] { "alac" }).ToArray()))
            {
                warnings.Add("The m4a container expects AAC or ALAC audio.");
            }

            if (container == "wav" && hasAudio && !In(audio, PcmLike))
            {
                warnings.Add("WAV expects uncompressed PCM audio (pcm_s16le / pcm_s24le).");
            }

            if (container == "flac" && hasAudio && !In(audio, "flac"))
            {
                warnings.Add("The flac container only accepts the flac codec.");
            }

            if (container == "ogg" && hasAudio && !In(audio, "libvorbis", "vorbis", "opus", "flac"))
            {
                warnings.Add("OGG expects Vorbis, Opus or FLAC audio.");
            }

            if (container == "opus" && hasAudio && !In(audio, "opus"))
            {
                warnings.Add("The opus container only accepts the opus codec.");
            }

            if (hasVideo && In(video, "prores") && !In(container, "mov", "mkv"))
            {
                warnings.Add("ProRes is best stored in mov or mkv.");
            }

            if (hasVideo && In(video, "ffv1") && container != "mkv")
            {
                warnings.Add("FFV1 lossless is normally used with the mkv container.");
            }

            return warnings;
        }

        private static bool In(string value, params string[] candidates)
        {
            return !string.IsNullOrEmpty(value) &&
                   candidates.Contains(value, StringComparer.OrdinalIgnoreCase);
        }
    }
}