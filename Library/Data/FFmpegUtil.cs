using System.Diagnostics;
using Xabe.FFmpeg.Downloader;

namespace Krympe.Library.Data;

public class FFmpegUtil
{
    #region Fields

    public static FFmpegUtil Instance = Instance ??= new();
    private string ffmpeg_exe;

    #endregion Fields

    #region Protected Constructors

    protected FFmpegUtil()
    {
        log.Warn("Downloading FFMPEG...");
        if (!Directory.GetFiles(FFmpegDirectory, "*ffmpeg*", SearchOption.AllDirectories).Any())
        {
            try
            {
                FFmpegDownloader.GetLatestVersion(FFmpegVersion.Official, FFmpegDirectory).Wait();
            }
            catch (Exception e)
            {
                log.Fatal($"Unable to download FFMPEG: {e.Message}", e.StackTrace);
                Environment.Exit(1);
            }
        }
        try
        {
            ffmpeg_exe = Directory.GetFiles(FFmpegDirectory, "*ffmpeg*", SearchOption.AllDirectories).First();
        }
        catch (Exception e)
        {
            log.Fatal($"Unable to initialize FFMPEG: {e.Message}", e.StackTrace);
            Environment.Exit(1);
        }
    }

    #endregion Protected Constructors

    #region Public Methods

    public Process RunFFmpegCommand(string inputFile, string arguments, string outputFile)
    {
        return new()
        {
            StartInfo = new()
            {
                FileName = ffmpeg_exe,
                Arguments = arguments.Replace("{INPUT}", $"\"{inputFile}\"").Replace("{OUTPUT}", $"\"{outputFile}\"").Replace("ffmpeg", "").Trim(),
                UseShellExecute = true,
            }
        };
    }

    #endregion Public Methods
}