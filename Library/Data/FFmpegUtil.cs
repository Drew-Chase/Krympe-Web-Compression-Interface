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
        string ffmpeg_directory = Directory.CreateDirectory(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Krympe", "ffmpeg")).FullName;
        if (!Directory.GetFiles(ffmpeg_directory, "*ffmpeg*", SearchOption.TopDirectoryOnly).Any())
        {
            FFmpegDownloader.GetLatestVersion(FFmpegVersion.Official);
        }
        ffmpeg_exe = Directory.GetFiles(ffmpeg_directory, "*ffmpeg*", SearchOption.TopDirectoryOnly).First();
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
                Arguments = $"-y -i \"{inputFile}\" {arguments} \"{outputFile}\""
            }
        };
    }

    #endregion Public Methods
}