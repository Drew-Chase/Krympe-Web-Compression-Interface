using ChaseLabs.CLConfiguration.List;
using ChaseLabs.Math;
using Krympe.Library.Data;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Dynamic;
using Timer = System.Timers.Timer;

namespace Krympe.Library.Objects;

public class CompressionProcess
{
    #region Fields

    private bool completed_successfully = true;
    private Process process;

    #endregion Fields

    #region Public Constructors

    public CompressionProcess(FileItem _file)
    {
        File = _file;
        TempPath = Path.Combine(Configuration.Instance.TempDirectory, $"{Path.GetFileNameWithoutExtension(File.Path)}_compressed{File.Info.Extension}");
    }

    #endregion Public Constructors

    #region Properties

    public long CurrentSize => System.IO.File.Exists(TempPath) ? new FileInfo(TempPath).Length : 0L;

    public FileItem File { get; private set; }

    public float PercentageOfOriginal => (float)Math.Round(CurrentSize == 0 ? 0f : ((float)CurrentSize / File.Size) * 100, 2, MidpointRounding.ToZero);

    public string TempPath { get; private set; }

    #endregion Properties

    #region Public Methods

    public void Start()
    {
        process = FFmpegUtil.Instance.RunFFmpegCommand(File.Path, TempPath);
        long original = File.Size;
        Timer timer = new()
        {
            Enabled = true,
            AutoReset = true,
            Interval = 5000,
        };
        timer.Elapsed += (s, e) =>
        {
            if (PercentageOfOriginal >= 100)
            {
                if (!process.HasExited)
                {
                    CompressionProcesses.Instance.AddFailedProcesses(this, "Original file was already the smallest");
                    completed_successfully = false;
                    process.Kill();
                }
            }
        };
        timer.Start();
        process.Start();
        process.WaitForExit();
        timer.Stop();
        if (completed_successfully)
        {
            completed_successfully = process.ExitCode == 0;
            if (!completed_successfully)
            {
                CompressionProcesses.Instance.AddFailedProcesses(this, "FFmpeg Error");
            }
        }
        if (completed_successfully)
        {
            try
            {
                if (Configuration.Instance.OverwriteOriginal)
                {
                    System.IO.File.Move(TempPath, File.Path, true);
                }
                else
                {
                    System.IO.File.Move(TempPath, Path.Combine(Path.GetDirectoryName(File.Path) ?? "", $"{Path.GetFileNameWithoutExtension(File.Path)}_compressed{File.Info.Extension}"), false);
                }
            }
            catch (IOException)
            {
                CompressionProcesses.Instance.AddFailedProcesses(this, "Unable to move file to final destination");
                return;
            }
            CompressionProcesses.Instance.AddSuccessfullProcesses(this, original);
        }
        Task.Run(() =>
        {
            while (System.IO.File.Exists(TempPath))
            {
                try
                {
                    System.IO.File.Delete(TempPath);
                }
                catch
                {
                }
            }
        });
    }

    public void Stop()
    {
        if (process != null && !process.HasExited)
        {
            completed_successfully = false;
            process.Kill();
        }
    }

    public virtual object ToObject()
    {
        return new
        {
            Name = File.Name,
            File.Path,
            Percentage = PercentageOfOriginal,
            CurrentSize = FileMath.AdjustedFileSize(CurrentSize),
            TotalSize = FileMath.AdjustedFileSize(File.Size),
        };
    }

    #endregion Public Methods
}

public class CompressionProcesses
{
    #region Fields

    public static CompressionProcesses Instance = Instance ??= new CompressionProcesses();
    private List<CompressionProcess> active, current;
    private List<FileItem> completed;
    private List<FailedCompressionProcess> failed;
    private ConfigManager manager;
    private bool stop = false;
    private List<SuccessfulCompressionProcess> successful;

    #endregion Fields

    #region Protected Constructors

    protected CompressionProcesses()
    {
        active = new();
        successful = new();
        failed = new();
        completed = new();
        current = new();
        manager = Configuration.Make(Path.Combine(ConfigDirectory, "db.json"));
        manager.Add("successful", Array.Empty<object>());
        manager.Add("failed", Array.Empty<object>());
    }

    #endregion Protected Constructors

    #region Public Methods

    public void AddActiveProcesses(FolderItem folder)
    {
        FileItem[]? files = FSUtil.GetFiles(folder);
        if (files != null)
        {
            foreach (FileItem file in files)
            {
                CompressionProcess process = new(file);
                active.Add(process);
            }
        }
        StartActiveProcesses();
    }

    public void AddFailedProcesses(CompressionProcess item, string reason)
    {
        failed.Add(new(item.File, reason));
        completed.Add(item.File);
        List<object> objs = new();
        foreach (var f in failed)
        {
            objs.Add(f.ToObject());
        }
        manager.GetConfigByKey("failed").Value = objs.ToArray();
    }

    public void AddSuccessfullProcesses(CompressionProcess item, long original)
    {
        successful.Add(new(item.File, original));
        completed.Add(item.File);
        List<object> objs = new();
        foreach (var success in successful)
        {
            objs.Add(success.ToObject());
        }
        manager.GetConfigByKey("successful").Value = objs.ToArray();
    }

    public CompressionProcess[] GetActiveProcesses()
    {
        List<CompressionProcess> activeProcesses = new List<CompressionProcess>();
        foreach (CompressionProcess process in active)
        {
            if (process.PercentageOfOriginal < 100)
            {
                activeProcesses.Add(process);
            }
        }
        return activeProcesses.ToArray();
    }

    public FailedCompressionProcess[] GetFailedProcesses()
    {
        return failed.ToArray();
    }

    public SuccessfulCompressionProcess[] GetSuccessfulProcesses()
    {
        return successful.ToArray();
    }

    public FileItem[] GetTodoProcesses()
    {
        completed.Clear();
        foreach (var item in successful)
        {
            completed.Add(item.File);
        }
        foreach (var item in failed)
        {
            completed.Add(item.File);
        }
        List<FileItem> items = new();
        Parallel.ForEach(WatchedDirectories.Instance.GetFiles(), file =>
        {
            if (!completed.Contains(file))
            {
                items.Add(file);
            }
        });
        return items.ToArray();
    }

    public void StartActiveProcesses()
    {
        stop = false;
        try
        {
            Parallel.ForEach(active, new ParallelOptions() { MaxDegreeOfParallelism = Configuration.Instance.ConcurrentProcesses }, process =>
            {
                if (stop)
                {
                    return;
                }
                if (!stop)
                {
                    current.Add(process);
                    process.Start();
                    current.Remove(process);
                }
            });
        }
        catch
        {
        }
    }

    public void Stop()
    {
        stop = true;

        active.Clear();
        Parallel.ForEach(current, item =>
        {
            item.Stop();
        });

        current.Clear();
        stop = false;
    }

    #endregion Public Methods
}

public class SuccessfulCompressionProcess : CompressionProcess
{
    #region Public Constructors

    public SuccessfulCompressionProcess(FileItem _file, long originalSize) : base(_file)
    {
        OriginalSize = originalSize;
    }

    #endregion Public Constructors

    #region Properties

    public long OriginalSize { get; private set; }

    #endregion Properties

    #region Public Methods

    public override object ToObject()
    {
        return new
        {
            Name = File.Name,
            File.Path,
            OriginalSize,
            Percentage = PercentageOfOriginal,
            CurrentSize = FileMath.AdjustedFileSize(CurrentSize),
            TotalSize = FileMath.AdjustedFileSize(File.Size),
        };
    }

    #endregion Public Methods
}

public class FailedCompressionProcess : CompressionProcess
{
    #region Public Constructors

    public FailedCompressionProcess(FileItem _file, string reason) : base(_file)
    {
        Reason = reason;
    }

    #endregion Public Constructors

    #region Properties

    public string Reason { get; private set; }

    #endregion Properties

    #region Public Methods

    public override object ToObject()
    {
        return new
        {
            Name = File.Name,
            File.Path,
            Reason,
            Percentage = PercentageOfOriginal,
            CurrentSize = FileMath.AdjustedFileSize(CurrentSize),
            TotalSize = FileMath.AdjustedFileSize(File.Size),
        };
    }

    #endregion Public Methods
}