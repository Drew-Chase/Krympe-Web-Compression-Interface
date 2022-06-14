using ChaseLabs.CLConfiguration.List;
using CLPortmapper;
using Krympe.Library.Objects;
using Newtonsoft.Json.Linq;

namespace Krympe.Library.Data;

public class Configuration
{
    #region Fields

    public static Configuration Instance = Instance ??= new Configuration();

    private string[] _watched_paths = null;
    private ConfigManager manager;

    #endregion Fields

    #region Protected Constructors

    protected Configuration()
    {
        manager = new(Path.Combine(ConfigDirectory, "app.json"));
        manager.Add("port", 2505);
        manager.Add("port_forward", false);
        manager.Add("overwrite_original", false);
        manager.Add("concurrent_processes", 3);
        manager.Add("temp_directory", Directory.CreateDirectory(Path.Combine(RootDirectory, "temp")).FullName);
        manager.Add("ffmpeg", "ffmpeg -hwaccel auto -y -i {INPUT} -b:v 0 -c:v h264 -b:a 0 -c:a aac {OUTPUT}");
        manager.Add("extensions", MediaExtensions.Make("mpegg;mpeg;mp4;mkv;m4a;m4v;f4v;f4a;m4b;m4r;f4b;mov;3gp;3gp2;3g2;3gpp;3gpp2;ogg;oga;ogv;ogx;wmv;wma;flv;avi".Split(";")).Get());
        manager.Add("watched", Array.Empty<string>());
    }

    #endregion Protected Constructors

    #region Properties

    public int ConcurrentProcesses { get => manager.GetConfigByKey("concurrent_processes").Value; set => manager.GetConfigByKey("concurrent_processes").Value = value; }
    public MediaExtensions Extensions { get => MediaExtensions.Make(manager.GetConfigByKey("extensions").Value); set => manager.GetConfigByKey("extensions").Value = value; }
    public string FFMpeg_Command { get => manager.GetConfigByKey("ffmpeg").Value; set => manager.GetConfigByKey("ffmpeg").Value = value; }
    public bool OverwriteOriginal { get => manager.GetConfigByKey("overwrite_original").Value; set => manager.GetConfigByKey("overwrite_original").Value = value; }
    public int Port { get => manager.GetConfigByKey("port").Value; set => manager.GetConfigByKey("port").Value = value; }

    public bool PortForward
    {
        get => manager.GetConfigByKey("port_forward").Value;
        set
        {
            try
            {
                if (value)
                {
                    PortHandler.OpenPort(Port);
                }
                else
                {
                    PortHandler.ClosePort(Port);
                }
            }
            catch
            {
            }
            manager.GetConfigByKey("port_forward").Value = value;
        }
    }

    public string TempDirectory
    {
        get => manager.GetConfigByKey("temp_directory").Value; set
        {
            Directory.Delete(TempDirectory, true);
            manager.GetConfigByKey("temp_directory").Value = Directory.CreateDirectory(value).FullName;
        }
    }

    public string[] WatchedPaths
    {
        get
        {
            if (_watched_paths == null)
            {
                List<string> paths = new();
                foreach (JToken token in manager.GetConfigByKey("watched").Value)
                {
                    paths.Add(token.ToString());
                }
                _watched_paths = paths.ToArray();
            }
            return _watched_paths;
        }
        set
        {
            _watched_paths = value;
            manager.GetConfigByKey("watched").Value = value;
        }
    }

    #endregion Properties

    #region Public Methods

    public static ConfigManager Make(string path)
    {
        return new(path, false, "Krympe");
    }

    #endregion Public Methods
}