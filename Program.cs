using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json;
using System;

namespace Model
{
    public class Program
    {
        public static Random                 Random;
        public static JsonSerializerSettings JsonSetting;
        public static void Main(string[] args)
        {
            Random = new Random();
            JsonSetting = new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.Auto };
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
}
