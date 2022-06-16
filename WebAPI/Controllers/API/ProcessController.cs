using ChaseLabs.Math;
using Krympe.Library.Data;
using Krympe.Library.Objects;
using Microsoft.AspNetCore.Mvc;

namespace Krympe.WebAPI.Controllers.API;

[ApiController]
[Route("/api/process")]
public class ProcessController : ControllerBase
{
    #region Public Methods

    [HttpPost("active")]
    public IActionResult AddActiveProcess([FromForm] string path)
    {
        FolderItem? item = FSUtil.GetFolder(path);
        if (item == null)
        {
            return BadRequest(new
            {
                message = $"Is either not a file or no file could be found at this path: \"{path}\""
            });
        }
        CompressionProcesses.Instance.AddActiveProcesses(item);
        return Ok(new
        {
            message = "Process Started"
        });
    }

    [HttpGet("active")]
    public JsonResult GetActiveProcesses()
    {
        List<object> objs = new();
        var processes = CompressionProcesses.Instance.GetActiveProcesses();
        long size = 0;
        foreach (CompressionProcess process in processes)
        {
            try
            {
                size += process.File.Size;
                objs.Add(process.ToObject());
            }
            catch (Exception e)
            {
                log.Error($"Unable to add process to list: {e.Message}", e.StackTrace ?? "");
            }
        }
        return new JsonResult(new
        {
            size = FileMath.AdjustedFileSize(size),
            count = processes.Length,
            processes = objs
        });
    }

    [HttpGet("failed")]
    public JsonResult GetFailedProcesses()
    {
        List<object> objs = new();
        var processes = CompressionProcesses.Instance.GetFailedProcesses();

        foreach (FailedCompressionProcess failed in processes)
        {
            objs.Add(failed.ToObject());
        }
        return new JsonResult(new
        {
            count = processes.Count(),
            processes = objs
        });
    }

    [HttpGet("successful")]
    public JsonResult GetSuccessfulProcesses()
    {
        List<object> objs = new();
        SuccessfulCompressionProcess[] processes = CompressionProcesses.Instance.GetSuccessfulProcesses();
        long size = 0L;
        foreach (SuccessfulCompressionProcess process in processes)
        {
            size += process.File.Size;
            objs.Add(process.ToObject());
        }
        return new JsonResult(new
        {
            size = FileMath.AdjustedFileSize(size),
            count = processes.Length,
            processes = objs
        });
    }

    [HttpGet("todo")]
    public JsonResult GetToDoProcesses()
    {
        List<object> objs = new();
        FileItem[] processes = CompressionProcesses.Instance.GetTodoProcesses();
        long size = 0L;
        foreach (FileItem process in processes)
        {
            size += process.Size;
            objs.Add(process.ToObject());
        }
        return new JsonResult(new
        {
            size = FileMath.AdjustedFileSize(size),
            count = processes.Length,
            processes = objs
        });
    }

    [HttpGet("stop")]
    public IActionResult StopProcesses()
    {
        CompressionProcesses.Instance.Stop();
        return Ok();
    }

    [HttpGet("test")]
    public IActionResult TestFFmpegCommand()
    {
        if (!WatchedDirectories.Instance.Get().Any())
        {
            return BadRequest(new
            {
                message = "The user does NOT have any watched directories"
            });
        }
        bool working = false;
        try
        {
            for (int i = 0; i < 4; i++)
            {
                working = FFmpegUtil.Instance.CheckFFmpegCommand();
                log.Debug($"Running Test #{i}");
                if (!working)
                {
                    log.Error($"Test #{i} failed", $"improper command \"{Configuration.Instance.FFMpeg_Command}\"");
                    break;
                }
            }
        }
        catch
        {
            working = false;
        }
        return new JsonResult(new
        {
            working
        });
    }

    #endregion Public Methods
}