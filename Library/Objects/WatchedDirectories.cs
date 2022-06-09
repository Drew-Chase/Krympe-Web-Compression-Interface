namespace Krympe.Library.Objects;

public class WatchedDirectories
{
    #region Fields

    public static WatchedDirectories Instance = Instance ??= new();
    private List<FolderItem> _folders;

    #endregion Fields

    #region Protected Constructors

    protected WatchedDirectories()
    {
        _folders = new List<FolderItem>();
    }

    #endregion Protected Constructors

    #region Public Methods

    public void Add(string path)
    {
        _folders.Add(new(path));
    }

    public FolderItem[] Get()
    {
        return _folders.ToArray();
    }

    #endregion Public Methods
}