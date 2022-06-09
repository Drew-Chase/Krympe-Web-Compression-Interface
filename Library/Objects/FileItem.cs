namespace Krympe.Library.Objects;

public class FileItem
{
    #region Fields

    private FileInfo info;

    #endregion Fields

    #region Public Constructors

    public FileItem(string path)
    {
        Path = path;
        Refresh();
    }

    #endregion Public Constructors

    #region Properties

    public string Name { get; private set; }
    public string Path { get; init; }

    public long Size { get; private set; }

    #endregion Properties

    #region Public Methods

    public void Refresh()
    {
        info = new(Path);
        Name = info.Name;

        Size = info.Length;
    }

    #endregion Public Methods
}