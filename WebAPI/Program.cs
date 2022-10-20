// LFInteractive LLC. - All Rights Reserved
global using static Krympe.Library.Data.Global;

namespace Krympe.WebAPI;

using Krympe.Library.Data;
using Krympe.Library.Objects;
using System.Diagnostics;

public class Program
{
    #region Private Methods

    private static void Main(string[] args)
    {
        log.Info("Welcome to Krympe Web Compression");
        log.Debug("All Rights Reserved - LFInteractive LLC. (c) 2020-2022");
        _ = DriveUtil.Instance;
        _ = FFmpegUtil.Instance;
        Host.CreateDefaultBuilder().ConfigureWebHostDefaults(builder =>
        {
            log.Info($"Starting server on port {Configuration.Instance.Port}");
            builder.UseIISIntegration();
            builder.UseContentRoot(Directory.GetCurrentDirectory());
            builder.ConfigureKestrel(cfg =>
            {
                cfg.Limits.MaxRequestBodySize = long.MaxValue;
            });
            builder.UseKestrel(options =>
            {
                options.ListenAnyIP(Configuration.Instance.Port);
            });
            builder.UseStartup<Startup>();
            log.Info("Server is now running!");
        }).Build().Run();

        log.Info("Shutting Down");
    }

    #endregion Private Methods

    #region Classes

    private class Startup
    {
        #region Public Methods

        public void Configure(IApplicationBuilder app, IWebHostEnvironment evn)
        {
            app.UseForwardedHeaders();
            app.UseMvc();
            app.UseRouting();
            app.UseStaticFiles();
            app.UseDefaultFiles();
        }

        public void ConfigureServices(IServiceCollection service)
        {
            service.AddMvc(action =>
            {
                action.EnableEndpointRouting = false;
            });
        }

        #endregion Public Methods
    }

    #endregion Classes
}