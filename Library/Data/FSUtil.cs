using ChaseLabs.Math;
using Krympe.Library.Objects;

namespace Krympe.Library.Data;

public static class FSUtil
{
    #region Public Methods

    public static object GetDirectoryEntries(string path, bool includeParent = true)
    {
        List<FolderItem> entries = new();

        foreach (DriveItem item in DriveUtil.Instance.GetCachedDrives())
        {
            if (item.Path == path)
            {
                entries.AddRange(item.SubFolders);
            }
            else
            {
                Parallel.ForEach(item.SubFolders, dir =>
                {
                    if (path.StartsWith(dir.Path))
                    {
                        if (dir.Size > 0)
                        {
                            if (path == dir.Path)
                            {
                                foreach (FolderItem subItem in dir.SubFolders)
                                {
                                    if (subItem.Size > 0)
                                        entries.Add(subItem);
                                }
                            }
                            else
                            {
                                FolderItem item = dir.GetSubFolder(path);
                                if (item != null)
                                {
                                    foreach (FolderItem subItem in item.SubFolders)
                                    {
                                        if (subItem.Size > 0)
                                            entries.Add(subItem);
                                    }
                                }
                            }
                        }
                    }
                });
            }
        }
        List<object> objs = new();
        if (includeParent)
        {
            DirectoryInfo parent = Directory.GetParent(path);
            entries = entries.OrderBy(a => a.Size).Reverse().ToList();
            objs.Add(new
            {
                Name = "..",
                Path = parent != null ? parent.FullName : "",
            });
            foreach (FolderItem item in entries)
            {
                if (!WatchedDirectories.Instance.Contains(item.Path))
                {
                    objs.Add(item.ToObject());
                }
            }
        }
        else
        {
            entries = entries.OrderBy(a => a.Size).Reverse().ToList();
            foreach (FolderItem item in entries)
            {
                if (!WatchedDirectories.Instance.Contains(item.Path))
                {
                    objs.Add(item.ToObject());
                }
            }
        }
        return new
        {
            Folders = objs
        };
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
                HumanReadableTotalSize = FileMath.AdjustedFileSize(drive.TotalSize),
                HumanReadableTotalFreespace = FileMath.AdjustedFileSize(drive.TotalFreeSpace),
            };
        }
        return new
        {
            Drives = objs
        };
    }

    // C:\Users\drew_

    public static FileItem[]? GetFiles(FolderItem folder, bool recursive = true)
    {
        List<FileItem> files = new();
        if (recursive)
        {
            files.AddRange(folder.Files);
            Parallel.ForEach(folder.SubFolders, item =>
            {
                FileItem[]? items = GetFiles(item, recursive);
                if (items != null)
                    files.AddRange(items);
            });
        }
        else
        {
            return folder.Files.ToArray();
        }
        return files.ToArray();
    }

    public static FolderItem? GetFolder(string path)
    {
        return GetFolder(DriveUtil.Instance.GetCachedDrive(Path.GetPathRoot(path)), path);
    }

    public static FolderItem? GetFolder(DriveItem? drive, string path)
    {
        if (drive != null && !string.IsNullOrWhiteSpace(path))
        {
            FolderItem[] subFolders = drive.SubFolders;
            while (subFolders != null && subFolders.Any())
            {
                FolderItem[] matching = subFolders.Where(x => path.StartsWith(x.Path)).ToArray();
                if (matching.Length == 1)
                {
                    if (matching[0].Path == path)
                    {
                        return matching[0];
                    }
                    subFolders = matching[0].SubFolders.ToArray();
                    continue;
                }
                else if (matching.Length > 1)
                {
                    string[] pathParts = path.Split(Path.PathSeparator);
                    foreach (FolderItem item in matching)
                    {
                        if (item.Path == path)
                        {
                            return item;
                        }
                        string[] parts = item.Path.Split(Path.PathSeparator);
                        if (parts[^1].Equals(pathParts[^1]))
                        {
                            subFolders = item.SubFolders.ToArray();
                            break;
                        }
                    }
                }
                else
                {
                    return null;
                }
            }
        }
        return null;
    }

    public static FolderItem[]? GetSubFolders(string path)
    {
        return GetSubFolders(DriveUtil.Instance.GetCachedDrive(Path.GetPathRoot(path)), path);
    }

    public static FolderItem[]? GetSubFolders(DriveItem? drive, string path)
    {
        return GetFolder(drive, path)?.SubFolders.ToArray();
    }

    public static bool IsDirectory(string path)
    {
        FileAttributes attr = File.GetAttributes(path);
        return (attr & FileAttributes.Directory) == FileAttributes.Directory;
    }

    #endregion Public Methods
}