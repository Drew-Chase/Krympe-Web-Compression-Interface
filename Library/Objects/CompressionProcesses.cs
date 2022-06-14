using ChaseLabs.Math;
using Krympe.Library.Data;
using System.Collections.Immutable;
using System.Diagnostics;
using Timer = System.Timers.Timer;

namespace Krympe.Library.Objects;

public class CompressionProcesses
{
    #region Fields

    public static CompressionProcesses Instance = Instance ??= new CompressionProcesses();
    private Dictionary<CompressionProcess, string> failed;
    private List<CompressionProcess> successfull, active;

    #endregion Fields

    #region Protected Constructors

    protected CompressionProcesses()
    {
        active = new();
        successfull = new();
        failed = new();
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
        failed.Add(item, reason);
    }

    public void AddSuccessfullProcesses(CompressionProcess item)
    {
        successfull.Add(item);
    }

    public CompressionProcess[] GetActiveProcesses()
    {
        return active.ToArray();
    }

    public IImmutableDictionary<CompressionProcess, string> GetFailedProcesses()
    {
        return failed.ToImmutableDictionary();
    }

    public CompressionProcess[] GetSuccessfulProcesses()
    {
        return successfull.ToArray();
    }

    public void StartActiveProcesses()
    {
        Parallel.ForEach(active, new ParallelOptions() { MaxDegreeOfParallelism = Configuration.Instance.ConcurrentProcesses }, process =>
        {
            process.Start();
        });
    }

    #endregion Public Methods
}

public class CompressionProcess
{
    #region Public Constructors

    public CompressionProcess(FileItem _file)
    {
        File = _file;
        TempPath = Path.Combine(Configuration.Instance.TempDirectory, $"{_file.Name}_compressed{_file.Info.Extension}");
    }

    #endregion Public Constructors

    #region Properties

    public long CurrentSize => System.IO.File.Exists(TempPath) ? new FileInfo(TempPath).Length : 0L;

    public FileItem File { get; private set; }

    public float PercentageOfOriginal => CurrentSize == 0 ? 0f : ((float)CurrentSize / File.Size) * 100;

    public string TempPath { get; private set; }

    #endregion Properties

    #region Public Methods

    public void Start()
    {
        Process process = FFmpegUtil.Instance.RunFFmpegCommand(File.Path, Configuration.Instance.FFMpeg_Command, TempPath);
        bool successfull = true;
        Timer timer = new()
        {
            Enabled = true,
            AutoReset = true,
            Interval = 5000,
        };
        timer.Elapsed += (s, e) =>
        {
            if (PercentageOfOriginal >= 1)
            {
                if (!process.HasExited)
                {
                    process.Kill();
                    CompressionProcesses.Instance.AddFailedProcesses(this, "Original file was already the smallest");
                    successfull = false;
                }
            }
        };
        timer.Start();
        process.Start();
        process.WaitForExit();
        timer.Stop();
        if (successfull)
        {
            successfull = process.ExitCode == 0;
            if (!successfull)
            {
                CompressionProcesses.Instance.AddFailedProcesses(this, "FFmpeg Error");
                return;
            }
        }
        if (successfull)
        {
            try
            {
                if (Configuration.Instance.OverwriteOriginal)
                {
                    System.IO.File.Move(TempPath, File.Path, true);
                }
                else
                {
                    System.IO.File.Move(TempPath, $"{File.Info.Name}_compressed{File.Info.Extension}", false);
                }
            }
            catch (IOException)
            {
                CompressionProcesses.Instance.AddFailedProcesses(this, "Unable to move file to final destination");
                return;
            }
            CompressionProcesses.Instance.AddSuccessfullProcesses(this);
        }
    }

    public object ToObject()
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