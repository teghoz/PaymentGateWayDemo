using System;
using System.IO;
using Microsoft.Extensions.Configuration;

namespace PaymentGateway
{
    static class ConfigurationManager
    {
        public static IConfiguration AppSetting { get; }
        static ConfigurationManager()
        {
            AppSetting = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json")
                    .Build();
        }
    }

    static class ConnectionManager
    {
        public static IConfiguration Connection { get; }
        static ConnectionManager()
        {
            Connection = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("connection.json")
                    .Build();
        }
    }
}
