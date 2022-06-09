using Krympe.Library.Data;

namespace Krympe.Library.Objects;

public class FolderItem
{
    #region Fields

    private DirectoryInfo info;

    #endregion Fields

    #region Public Constructors

    public FolderItem(string path, bool ignoreFilter = false, bool ignoreSubFolders = false)
    {
        Path = path;
        Refresh(ignoreFilter, ignoreSubFolders);
    }

    #endregion Public Constructors

    #region Properties

    public List<FileItem> Files { get; private set; }
    public string Name { get; private set; }
    public string Path { get; init; }

    public long Size { get; private set; }

    public List<FolderItem> SubFolders { get; private set; }

    #endregion Properties

    #region Public Methods

    public void Refresh(bool ignoreFilter = false, bool ignoreSubFolders = false)
    {
        info = new(Path);
        Name = info.Name;

        Files = new();
        string[] files = Directory.GetFiles(Path, "*", SearchOption.TopDirectoryOnly);
        foreach (string file in files)
        {
            if (!ignoreFilter)
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
            else
            {
                Files.Add(new(file));
            }
        }

        Size = 0L;

        foreach (FileItem file in Files)
        {
            Size += file.Size;
        }
        if (!ignoreSubFolders)
        {
            SubFolders = new();
            DirectoryInfo[] subs = new DirectoryInfo(Path).GetDirectories("*", SearchOption.TopDirectoryOnly);
            foreach (DirectoryInfo sub in subs)
            {
                FolderItem item = new(sub.FullName, ignoreFilter, ignoreSubFolders);
                SubFolders.Add(item);
                Size += item.Size;
            }
        }
    }

    #endregion Public Methods
}