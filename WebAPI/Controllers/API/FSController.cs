using Krympe.Library.Data;
using Krympe.Library.Objects;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Krympe.WebAPI.Controllers.API;

[ApiController]
[Route("/api/fs")]
public class FSController : ControllerBase
{
    #region Public Methods

    [HttpPost()]
    public IActionResult Get([FromForm] string? path = "")
    {
        if (string.IsNullOrEmpty(path))
        {
            return Ok(FSUtil.GetDrives());
        }
        return Ok(FSUtil.GetDirectoryEntries(path));
    }

    [HttpGet("watch")]
    public IActionResult GetWatched()
    {
        try
        {
            List<object> objs = new();
            FolderItem[] items = WatchedDirectories.Instance.Get();
            if (items == null) return BadRequest();
            foreach (FolderItem item in items)
            {
                objs.Add(item.ToObject());
            }
            return Ok(objs);
        }
        catch (Exception e)
        {
            log.Error($"Unable to return watched directories: {e.Message}", e.StackTrace);
            return BadRequest(new
            {
                message = e.Message
            });
        }
    }

    [HttpDelete("watch")]
    public IActionResult RemoveWatched([FromForm] string path)
    {
        WatchedDirectories.Instance.Remove(path);
        return Ok();
    }

    [HttpGet("tree")]
    public JsonResult Tree()
    {
        var drives = DriveUtil.Instance.GetCachedDrives();
        object[] json = new object[drives.Count()];
        for (int i = 0; i < drives.Count(); i++)
        {
            json[i] = drives[i].ToObject(true);
        }
        return new JsonResult(json);
    }

    [HttpPost("watch")]
    public IActionResult Watch([FromForm] string path)
    {
        if (string.IsNullOrEmpty(path) || !Directory.Exists(path))
        {
            return BadRequest(new
            {
                message = $"\"{path}\" is not a valid path"
            });
        }
        WatchedDirectories.Instance.Add(path);
        return Ok();
    }

    #endregion Public Methods
}