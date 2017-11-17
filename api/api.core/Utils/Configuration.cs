using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace api.core.Utils
{
    public static class Configuration
    {
        public static IConfigurationRoot Config { get; set; }

        static Configuration()
        {
            var builder = new ConfigurationBuilder()
             .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json");
            Config = builder.Build();

            Configuration.Config = Config;
        }

        public static string DbConnection => Config["DefaultConnection"];

        public static int DealAgeInHoursToRemove => Config.GetValue<int>("DealAgeInHoursToRemove");

        public static string RegisterPassword => Config.GetValue<string>("RegisterPassword");

        public static string FrontEndUrl => Config.GetValue<string>("FrontEndUrl");

    }
}
