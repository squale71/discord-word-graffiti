using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace Discord.WordGraffiti.App_Start
{
    public class Configuration
    {
        private static Configuration _instance;

        private IConfigurationRoot _config;

        private Configuration()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            var devEnvironmentVariable = Environment.GetEnvironmentVariable("NETCORE_ENVIRONMENT");
            var isDevelopment = string.IsNullOrEmpty(devEnvironmentVariable) || devEnvironmentVariable.ToLower() == "development";

            if (true)
            {
                builder.AddUserSecrets<AppSecret>();
            }

            _config = builder.Build();
        }

        public static Configuration Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new Configuration();
                }
                return _instance;
            }
        }


        public string Get(string key)
        {
            return _config[$"{key}"];
        }
    }
}
