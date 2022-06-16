using Krympe.Library.Objects;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Krympe.Library.Data;

public class DriveUtil
{
    #region Fields

    public static DriveUtil Instance = Instance ??= new();

    private List<DriveItem> _drives;
    private string file = Path.Combine(ConfigDirectory, "cached_drives.json");

    #endregion Fields

    #region Protected Constructors

    protected DriveUtil()
    {
        _drives = new();
        LoadCachedDrives();
    }

    #endregion Protected Constructors

    #region Public Methods

    public DriveItem? GetCachedDrive(string? name)
    {
        if (!string.IsNullOrWhiteSpace(name))
        {
            foreach (DriveItem item in _drives)
            {
                if (item.Name.Equals(name))
                {
                    return item;
                }
            }
        }
        return null;
    }

    public DriveItem[] GetCachedDrives()
    {
        return _drives.ToArray();
    }

    public void LoadCachedDrives()
    {
        if (!File.Exists(file))
        {
            RefreshCachedDrives();
            return;
        }
        try
        {
            log.Warn("Loading cached drives from file");
            long startTime = DateTime.Now.Ticks;
            _drives = new();
            using StreamReader reader = new(new FileStream(file, FileMode.OpenOrCreate, FileAccess.Read));
            JArray json = JsonConvert.DeserializeObject<JArray>(reader.ReadToEnd());
            if (json != null)
            {
                foreach (var token in json)
                {
                    try
                    {
                        JObject obj = token.ToObject<JObject>();
                        if (obj != null)
                        {
                            _drives.Add(DriveItem.Make(obj));
                        }
                    }
                    catch (Exception e)
                    {
                        log.Error($"Unable to add loaded cached drive: {e.Message}");
                    }
                }
            }
            log.Info("Done loading cached drive info");
            log.Debug($"Process took {new TimeSpan(DateTime.Now.Ticks - startTime):hh\\:mm\\:ss}");
        }
        catch (Exception e)
        {
            log.Error($"Unable to load Cached Drive Information: {e.Message}");
        }
        if (!_drives.Any())
            RefreshCachedDrives();
    }

    public Task RefreshCachedDrives()
    {
        return Task.Run(() =>
        {
            try
            {
                log.Warn($"Refreshing Cached Drives");
                log.Debug("This will take a moment!");
                long startTime = DateTime.Now.Ticks;
                _drives = new();
                foreach (var drive in DriveInfo.GetDrives())
                {
                    _drives.Add(new DriveItem(drive.Name));
                }
                SaveCachedDrives();

                log.Info("Done Refreshing Cached Drives");
                log.Debug($"Process took {new TimeSpan(DateTime.Now.Ticks - startTime):hh\\:mm\\:ss}");
            }
            catch (Exception e)
            {
                log.Error($"Unable to Refresh Cached Drvies: {e.Message}");
            }
        });
    }

    public void SaveCachedDrives()
    {
        try
        {
            log.Warn("Saving cached drive info to file");

            List<object> json = new();
            for (int i = 0; i < _drives.Count; i++)
            {
                List<object> drive = new();
                DriveItem item = _drives[i];
                for (int j = 0; j < item.SubFolders.Count(); j++)
                {
                    FolderItem sub = item.SubFolders[j];
                    if (sub.Size > 0)
                        drive.Add(sub.ToObject(true));
                }
                json.Add(new
                {
                    Name = item.Name,
                    Path = item.Path,
                    TotalSize = item.TotalSize,
                    TotalFreeSpace = item.TotalFreeSpace,
                    SubFolders = drive.ToArray()
                });
            }
            using StreamWriter writer = new(new FileStream(file, FileMode.OpenOrCreate, FileAccess.Write));
            writer.Write(JsonConvert.SerializeObject(json.ToArray(), Formatting.Indented));
            writer.Flush();
            writer.Dispose();
            writer.Close();
        }
        catch (Exception e)
        {
            log.Error($"Unable to save cached drives: {e.Message}");
        }
        log.Info("Done saving cached drive info to file");
    }

    #endregion Public Methods
}