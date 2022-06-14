using Krympe.Library.Data;
using Krympe.Library.Objects;
using Microsoft.AspNetCore.Mvc;

namespace Krympe.WebAPI.Controllers.API;

[ApiController]
[Route("/api/config")]
public class ConfigController : ControllerBase
{
    #region Public Methods

    [HttpGet()]
    public IActionResult Index()
    {
        return new JsonResult(new
        {
            web_port = Configuration.Instance.Port,
            port_forward = Configuration.Instance.PortForward,
            concurrent_processes = Configuration.Instance.ConcurrentProcesses,
            overwrite = Configuration.Instance.OverwriteOriginal,
            temp_directory = Configuration.Instance.TempDirectory,
            ffmpeg = Configuration.Instance.FFMpeg_Command,
            extensions = Configuration.Instance.Extensions.AsJson,
        });
    }

    [HttpPost("set")]
    public IActionResult Set([FromForm] string name, [FromForm] string value)
    {
        try
        {
            switch (name)
            {
                case "port":
                    Configuration.Instance.Port = int.Parse(value);
                    break;

                case "concurrent_processes":
                    Configuration.Instance.ConcurrentProcesses = int.Parse(value);
                    break;

                case "port_forward":
                    Configuration.Instance.PortForward = bool.Parse(value);
                    break;
                case "overwrite":
                    Configuration.Instance.OverwriteOriginal = bool.Parse(value);
                    break;

                case "temp_directory":
                    Configuration.Instance.TempDirectory = value;
                    break;

                case "ffmpeg":
                    Configuration.Instance.FFMpeg_Command = value;
                    break;
                case "extensions":
                    Configuration.Instance.Extensions = MediaExtensions.Make(value.Split(';'));
                    break;

                default:
                    break;
            }
        }
        catch
        {
        }
        return Index();
    }

    #endregion Public Methods
}