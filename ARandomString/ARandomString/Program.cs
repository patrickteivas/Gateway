using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ARandomString
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var rnd = new Random();
            CreateWebHostBuilder(args, rnd.Next(6000, 6999)).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args, int port) =>
            WebHost.CreateDefaultBuilder(args)
            .UseUrls("http://*:" + port)
                .UseStartup<Startup>();
    }
}
