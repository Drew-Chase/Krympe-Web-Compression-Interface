using Krympe.Library.Objects;
using System.Diagnostics;
using Xabe.FFmpeg;
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

    public bool CheckFFmpegCommand()
    {
        log.Info("Checking FFMPEG Command");
        string command = Configuration.Instance.FFMpeg_Command;
        Random random = new();
        FolderItem[] folders = WatchedDirectories.Instance.Get();
        FileItem[]? files = FSUtil.GetFiles(folders[random.Next(0, folders.Length - 1)]);
        if (files == null)
            return CheckFFmpegCommand();
        string input = files[random.Next(0, files.Length - 1)].Path;
        string output = Path.Combine(TempDirectory, "test_file.mp4");
        if (FFmpeg.GetMediaInfo(input).Result.Duration.TotalSeconds < 10)
        {
            return CheckFFmpegCommand();
        }
        command = command.Replace("{OUTPUT}", "-t 1 {OUTPUT}");
        Process process = RunFFmpegCommand(input, command, output);
        process.Start();
        process.WaitForExit();
        File.Delete(output);
        return process.ExitCode == 0;
    }

    public Process RunFFmpegCommand(string inputFile, string outputFile) => RunFFmpegCommand(inputFile, Configuration.Instance.FFMpeg_Command, outputFile);

    public Process RunFFmpegCommand(string inputFile, string command, string outputFile)
    {
        log.Debug($"Running FFMPEG Command {command.Replace("{INPUT}", $"\"{inputFile}\"").Replace("{OUTPUT}", $"\"{outputFile}\"").Trim()}");
        return new()
        {
            StartInfo = new()
            {
                FileName = ffmpeg_exe,
                Arguments = command.Replace("{INPUT}", $"\"{inputFile}\"").Replace("{OUTPUT}", $"\"{outputFile}\"").Replace("ffmpeg", "").Trim(),
                UseShellExecute = false,
                WindowStyle = ProcessWindowStyle.Hidden,
            }
        };
    }

    #endregion Public Methods
}