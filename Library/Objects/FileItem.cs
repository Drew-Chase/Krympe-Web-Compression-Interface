using ChaseLabs.Math;
using Newtonsoft.Json.Linq;

namespace Krympe.Library.Objects;

public class FileItem
{
    #region Public Constructors

    public FileItem(string path)
    {
        Path = path;
        Refresh();
    }

    #endregion Public Constructors

    #region Properties

    public FileInfo Info { get; private set; }
    public string Name { get; private set; }
    public string Path { get; init; }
    public long Size { get; private set; }

    #endregion Properties

    #region Public Methods

    protected FileItem(JObject obj)
    {
        Path = (string)obj["Path"];
        Refresh();
    }

    public static FileItem Make(JObject obj)
    {
        return new(obj);
    }

    public void Refresh()
    {
        Info = new(Path);
        Name = Info.Name;

        Size = Info.Length;
    }
    public object ToObject()
    {
        return new
        {
            Name,
            Path,
            TotalSize = Size,
            Size = FileMath.AdjustedFileSize(Size),
        };
    }

    #endregion Public Methods
}