using ChaseLabs.CLConfiguration.List;
using Krympe.Library.Data;
using Newtonsoft.Json.Linq;

namespace Krympe.Library.Objects;

public class WatchedDirectories
{
    #region Fields

    public static WatchedDirectories Instance = Instance ??= new();

    #endregion Fields

    #region Protected Constructors

    protected WatchedDirectories()
    {
        Folders = new();
        if (Configuration.Instance.WatchedPaths.Any())
        {
            Folders.AddRange(Configuration.Instance.WatchedPaths);
        }
    }

    #endregion Protected Constructors

    #region Properties

    private List<string> Folders { get; set; }

    #endregion Properties

    #region Public Methods

    public void Add(string path)
    {
        if (!Contains(path))
        {
            RemovePartial(path);
            Folders.Add(path);
            Configuration.Instance.WatchedPaths = Folders.ToArray();
        }
    }

    public bool Contains(string path) => Folders.Contains(path);

    public FolderItem[] Get()
    {
        try
        {
            List<FolderItem> items = new();
            foreach (string path in Folders)
            {
                FolderItem? item = FSUtil.GetFolder(path);
                if (item != null)
                {
                    items.Add(item);
                }
            }
            return items.ToArray();
        }
        catch (Exception e)
        {
            log.Error($"Unable to get watched directories: {e.Message}", e.StackTrace);
        }
        return null;
    }

    public void Remove(string path)
    {
        if (Folders.Contains(path))
        {
            Folders.Remove(path);
            Configuration.Instance.WatchedPaths = Folders.ToArray();
        }
    }

    public void RemovePartial(string path)
    {
        List<string> partials = new();
        foreach (string folder in Folders)
        {
            if (folder.StartsWith(path))
            {
                partials.Add(folder);
            }
        }
        foreach (string folder in partials)
        {
            Folders.Remove(folder);
        }
    }

    #endregion Public Methods
}