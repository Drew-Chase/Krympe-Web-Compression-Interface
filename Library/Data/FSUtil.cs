using Krympe.Library.Objects;

namespace Krympe.Library.Data;

public static class FSUtil
{
    #region Public Methods

    public static FolderItem[] GetDirectoryEntries(string path)
    {
        List<FolderItem> entries = new();
        foreach (string dir in Directory.GetFileSystemEntries(path))
        {
            entries.Add(new(dir));
        }
        return entries.ToArray();
    }

    public static object GetDrives()
    {
        object[] objs = new object[DriveInfo.GetDrives().Length];
        for (int i = 0; i < DriveInfo.GetDrives().Length; i++)
        {
            DriveInfo drive = DriveInfo.GetDrives()[i];
            objs[i] = new
            {
                Name = drive.Name,
                TotalSize = drive.TotalSize,
                TotalFreeSpace = drive.TotalFreeSpace,
                AvailableFreeSpace = drive.AvailableFreeSpace,
            };
        }
        object obj = new
        {
            Drives = objs
        };
        return obj;
    }

    #endregion Public Methods
}