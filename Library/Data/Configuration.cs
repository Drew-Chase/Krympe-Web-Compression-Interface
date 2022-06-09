using ChaseLabs.CLConfiguration.List;
using CLPortmapper;
using Krympe.Library.Objects;

namespace Krympe.Library.Data;

public class Configuration
{
    #region Fields

    public static Configuration Instance = Instance ??= new Configuration();

    private ConfigManager manager;

    #endregion Fields

    #region Protected Constructors

    protected Configuration()
    {
        manager = new(Path.Combine(Global.ConfigDirectory, "app.cfg"));
        manager.Add("Port", 2505);
        manager.Add("Port Forward", false);
        manager.Add("Admin Token", Guid.NewGuid().ToString());
        manager.Add("extensions", MediaExtensions.Make("mpegg;mpeg;mp4;mkv;m4a;m4v;f4v;f4a;m4b;m4r;f4b;mov;3gp;3gp2;3g2;3gpp;3gpp2;ogg;oga;ogv;ogx;wmv;wma;flv;avi").ToString());
    }

    #endregion Protected Constructors

    #region Properties

    public MediaExtensions Extensions { get => MediaExtensions.Make(manager.GetConfigByKey("extensions").Value); }
    public int Port { get => manager.GetConfigByKey("Port").Value; set => manager.GetConfigByKey("Port").Value = value; }

    public bool PortForward
    {
        get => manager.GetConfigByKey("Port Forward").Value;
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
            manager.GetConfigByKey("Port Forward").Value = value;
        }
    }

    public Guid Token { get => Guid.Parse(manager.GetConfigByKey("Admin Token").Value); set => manager.GetConfigByKey("Admin Token").Value = value.ToString(); }

    #endregion Properties

    #region Public Methods

    public static ConfigManager Make(string path)
    {
        return new(path, false, "Krympe");
    }

    #endregion Public Methods
}