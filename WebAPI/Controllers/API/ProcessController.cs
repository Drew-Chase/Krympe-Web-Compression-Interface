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
        foreach (CompressionProcess process in CompressionProcesses.Instance.GetActiveProcesses())
        {
            try
            {
                objs.Add(process.ToObject());
            }
            catch (Exception e)
            {
                log.Error($"Unable to add process to list: {e.Message}", e.StackTrace ?? "");
            }
        }
        return new JsonResult(new
        {
            active = objs
        });
    }

    [HttpGet("failed")]
    public JsonResult GetFailedProcesses()
    {
        List<object> objs = new();
        foreach ((CompressionProcess process, string reason) in CompressionProcesses.Instance.GetFailedProcesses())
        {
            objs.Add(new
            {
                reason,
                process = process.ToObject(),
            });
        }
        return new JsonResult(new
        {
            failed = objs
        });
    }

    [HttpGet("successful")]
    public JsonResult GetSuccessfulProcesses()
    {
        List<object> objs = new();
        foreach (CompressionProcess process in CompressionProcesses.Instance.GetSuccessfulProcesses())
        {
            objs.Add(process.ToObject());
        }
        return new JsonResult(new
        {
            successful = objs
        });
    }

    #endregion Public Methods
}