using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Krympe.Library.Objects;

public class DriveItem
{
    #region Public Constructors

    public DriveItem(string path)
    {
        var info = DriveInfo.GetDrives().First(d => d.Name.Equals(path));
        Name = info.Name;
        Path = path;
        TotalSize = info.TotalSize;
        TotalFreeSpace = info.TotalFreeSpace;
        SubFolders = new FolderItem(path).SubFolders.ToArray();
    }

    #endregion Public Constructors

    #region Protected Constructors

    protected DriveItem(JObject json)
    {
        Name = (string)json["Name"];
        Path = (string)json["Path"];
        TotalSize = (long)json["TotalSize"];
        TotalFreeSpace = (long)json["TotalFreeSpace"];
        List<FolderItem> list = new();
        foreach (var item in json["SubFolders"].ToArray())
        {
            list.Add(FolderItem.Make(item.ToObject<JObject>()));
        }
        SubFolders = list.ToArray();
    }

    #endregion Protected Constructors

    #region Properties

    public string Name { get; private set; }
    public string Path { get; private set; }
    public FolderItem[] SubFolders { get; private set; }
    public long TotalFreeSpace { get; private set; }
    public long TotalSize { get; private set; }

    #endregion Properties

    #region Public Methods

    public static DriveItem Make(JObject json)
    {
        return new(json);
    }

    public object ToObject(bool tree = false, bool ignoreEmpty = true)
    {
        List<object> obj = new();
        foreach (var item in SubFolders)
        {
            obj.Add(item.ToObject(tree, ignoreEmpty));
        }
        return new
        {
            Name,
            Path,
            TotalSize,
            TotalFreeSpace,
            SubFolders = obj,
        };
    }

    #endregion Public Methods
}