using ChaseLabs.Math;
using Krympe.Library.Data;
using Newtonsoft.Json.Linq;

namespace Krympe.Library.Objects;

public class FolderItem
{
    #region Fields

    private DirectoryInfo info;

    #endregion Fields

    #region Public Constructors

    public FolderItem(string path)
    {
        Path = path;
        Refresh();
    }

    #endregion Public Constructors

    #region Protected Constructors

    protected FolderItem(JObject obj)
    {
        try
        {
            Name = obj["Name"].ToString();
            Size = long.Parse(obj["TotalSize"].ToString());
            Path = obj["Path"].ToString();
            SubFolders = new();
            foreach (var item in obj["SubFolders"].ToObject<JArray>())
            {
                SubFolders.Add(Make(item.ToObject<JObject>()));
            }
            Files = new();
            foreach(var file in obj["Files"].ToObject<JArray>())
            {
                Files.Add(FileItem.Make(file.ToObject<JObject>()));
            }
        }
        catch (Exception e)
        {
            log.Error($"Unable to create folder item from json: {e.Message}", e.StackTrace);
        }
    }

    #endregion Protected Constructors

    #region Properties

    public List<FileItem> Files { get; private set; }
    public string Name { get; private set; }
    public string Path { get; init; }

    public long Size { get; private set; }

    public List<FolderItem> SubFolders { get; private set; }

    #endregion Properties

    #region Public Methods

    public static FolderItem Make(JObject json)
    {
        return new(json);
    }

    public FolderItem GetSubFolder(string path)
    {
        FolderItem value = null;
        Parallel.ForEach(SubFolders, item =>
        {
            if (path.StartsWith(item.Path))
            {
                if (path == item.Path)
                {
                    value = item;
                }
                else
                {
                    value = item.GetSubFolder(path);
                }
            }
        });
        return value;
    }

    public void Refresh()
    {
        info = new(Path);
        Name = info.Name;

        Files = new();
        string[] files = Directory.GetFiles(Path, "*", SearchOption.TopDirectoryOnly);
        foreach (string file in files)
        {
            FileInfo fi = new(file);

            foreach (string ext in Configuration.Instance.Extensions.Get())
            {
                if (fi.Extension.Equals($".{ext}"))
                {
                    Files.Add(new(file));
                    break;
                }
            }
        }

        Size = 0L;

        foreach (FileItem file in Files)
        {
            Size += file.Size;
        }
        SubFolders = new();
        DirectoryInfo[] subs = new DirectoryInfo(Path).GetDirectories("*", SearchOption.TopDirectoryOnly);
        Parallel.ForEach(subs, sub =>
        {
            try
            {
                FolderItem item = new(sub.FullName);
                SubFolders.Add(item);
                Size += item.Size;
            }
            catch
            {
            }
        });
    }

    public object ToObject(bool tree = false, bool ignoreEmpty = true)
    {
        List<object> files = new();
        if (tree)
        {
            List<object> subs = new();
            for (int i = 0; i < SubFolders.Count; i++)
            {
                try
                {
                    if (SubFolders[i] != null && (!ignoreEmpty || (ignoreEmpty && SubFolders[i].Size > 0)))
                        subs.Add(SubFolders[i].ToObject(tree, ignoreEmpty));
                }
                catch (Exception e)
                {
                    log.Error($"Unable to add sub directory: {e.Message}");
                }
            }

            for (int i = 0; i < Files.Count; i++)
            {
                if (Files[i] != null)
                {
                    files.Add(Files[i].ToObject());
                }
            }

            return new
            {
                Name,
                Path,
                TotalSize = Size,
                Size = FileMath.AdjustedFileSize(Size),
                Files = files,
                SubFolders = subs.ToArray(),
            };
        }

        for (int i = 0; i < Files.Count; i++)
        {
            if (Files[i] != null)
            {
                files.Add(Files[i].ToObject());
            }
        }
        return new
        {
            Name,
            Path,
            TotalSize = Size,
            Size = FileMath.AdjustedFileSize(Size),
            Files = files,
        };
    }

    #endregion Public Methods
}