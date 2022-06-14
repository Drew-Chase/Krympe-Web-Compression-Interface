using ChaseLabs.CLLogger;
using ChaseLabs.CLLogger.Interfaces;

namespace Krympe.Library.Data;

public static class Global
{
    #region Fields

    public static ILog log = LogManager.Init().SetDumpMethod(DumpType.NoBuffer).SetLogDirectory(Path.Combine(LogsDirectory, "latest.log")).SetMinimumLogType(Lists.LogTypes.All).SetPattern("[%TYPE%: %DATE%]: %MESSAGE%");

    #endregion Fields

    #region Properties

    public static string ConfigDirectory => Directory.CreateDirectory(Path.Combine(RootDirectory, "config")).FullName;
    public static string FFmpegDirectory => Directory.CreateDirectory(Path.Combine(RootDirectory, "ffmpeg")).FullName;
    public static string LogsDirectory => Directory.CreateDirectory(Path.Combine(RootDirectory, "logs")).FullName;
    public static string RootDirectory => Directory.CreateDirectory(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "LFInteractive", "Krympe")).FullName;
    public static string UsersDirectory => Directory.CreateDirectory(Path.Combine(ConfigDirectory, "users")).FullName;

    #endregion Properties
}