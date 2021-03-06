using System.IO;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Sio.Cms.Lib;

namespace Sio.Cms.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            var config = new ConfigurationBuilder()
           .SetBasePath(Directory.GetCurrentDirectory())
           .AddJsonFile(SioConstants.CONST_FILE_APPSETTING, optional: true, reloadOnChange: true)
           .Build();

            return WebHost.CreateDefaultBuilder(args)
                .UseConfiguration(config)
                .UseSetting("https_port", "443")
                .UseStartup<Startup>().UseKestrel(options =>
                {
                    options.Limits.MaxRequestBodySize = 209715200;
                });
        }
    }
}
